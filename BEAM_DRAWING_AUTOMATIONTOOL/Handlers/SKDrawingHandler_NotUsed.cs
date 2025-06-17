using System;
using System.Collections.Generic;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Utils;
using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Support;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKDrawingHandler_NotUsed
    {
        private readonly CustomInputModel _userInput;
        private readonly SKBoundingBoxHandler _boundingBoxHandler;
        private readonly SKCatalogHandler _catalogHandler;

        private const double DistanceThreshold = 125;
        private const double ComparisonThreshold = 300;
        private const double HeightAdjustment = 25.4;

        public SKDrawingHandler_NotUsed(SKBoundingBoxHandler boundingBoxHandler, SKCatalogHandler catalogHandler, CustomInputModel userInput)
        {
            _boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            _catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
        }

        public TSD.AssemblyDrawing ResetDrawingDimensionsExceptAssemblyDimensions(
            TSM.Model myModel, TSM.ModelObject currentBeam, string drgAttribute, 
            TSM.Part mainPart, double output, TSM.Assembly assembly,
            out TSD.StraightDimension overallDim, out double dimDistance, 
            out double actualDis, out List<SectionLocationWithParts> sectionList,
            double scale, double miniLength, out List<SectionLocationWithParts> flangeSectionList, 
            out List<SectionLocationWithParts> flangeSectionDuplicateList,
            out List<TSM.Part> bottomPartMarkRetainList, out List<Guid> topViewDeleteGuids, 
            out TSD.StraightDimension overallDimension,
            out List<TSD.RadiusDimension> radiusDimList)
        {
            var drgHandler = new TSD.DrawingHandler();

            dimDistance = 0;
            actualDis = 0;
            overallDim = null;
            overallDimension = null;

            sectionList = new List<SectionLocationWithParts>();
            flangeSectionList = new List<SectionLocationWithParts>();
            flangeSectionDuplicateList = new List<SectionLocationWithParts>();
            bottomPartMarkRetainList = new List<TSM.Part>();
            topViewDeleteGuids = new List<Guid>();
            radiusDimList = new List<TSD.RadiusDimension>();

            var assemblyDrawing = new TSD.AssemblyDrawing(assembly.Identifier, drgAttribute);
            assemblyDrawing.Insert();
            drgHandler.SetActiveDrawing(assemblyDrawing, true);

            ModifyViewToScale(scale, miniLength, assemblyDrawing);

            TSD.PointList assemblyBoundingBox = _boundingBoxHandler.BoundingBoxForDimension(assembly);
            TSG.Point startWorkPoint = assemblyBoundingBox[0];
            TSG.Point endWorkPoint = assemblyBoundingBox[1];

            ProcessDrawingViews(assemblyDrawing, startWorkPoint, endWorkPoint, ref overallDimension, radiusDimList);
            ProcessPartsForSections(myModel, assemblyDrawing, mainPart, output, sectionList, flangeSectionList, bottomPartMarkRetainList, topViewDeleteGuids, drgAttribute);

            assemblyDrawing.CommitChanges();
            return assemblyDrawing;
        }

        private void ProcessDrawingViews(TSD.AssemblyDrawing drawing, TSG.Point startWorkPoint, TSG.Point endWorkPoint,
            ref TSD.StraightDimension overallDimension, List<TSD.RadiusDimension> radiusDimList)
        {
            var drawingViews = drawing.GetSheet().GetAllViews();
            while (drawingViews.MoveNext())
            {
                if (drawingViews.Current is TSD.View currentView)
                {
                    overallDimension = DeleteViewDimensionByType(startWorkPoint, endWorkPoint, currentView);
                }
            }
            drawingViews.Reset();
            ExtractRadiusDimension(radiusDimList, drawingViews);
        }

        private void ProcessPartsForSections(TSM.Model myModel, TSD.AssemblyDrawing drawing, TSM.Part mainPart, double output,
            List<SectionLocationWithParts> sectionList, List<SectionLocationWithParts> flangeSectionList,
            List<TSM.Part> bottomPartMarkRetainList, List<Guid> topViewDeleteGuids, string drgAttribute)
        {
            var parts = GetPartsForSections(drawing, mainPart);
            myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(mainPart.GetCoordinateSystem()));

            var viewEnumerator = drawing.GetSheet().GetAllViews();
            while (viewEnumerator.MoveNext())
            {
                TSD.View view = viewEnumerator.Current as TSD.View;
                if (view.ViewType == TSD.View.ViewTypes.FrontView)
                {
                    ProcessFrontView(view, mainPart, parts, output, sectionList, flangeSectionList,
                        bottomPartMarkRetainList, topViewDeleteGuids, drgAttribute);
                }
            }

            myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
        }

        private List<TSM.Part> GetPartsForSections(TSD.AssemblyDrawing drawing, TSM.Part mainPart)
        {
            var parts = new List<TSM.Part>();
            var viewEnumerator = drawing.GetSheet().GetAllViews();
            while (viewEnumerator.MoveNext())
            {
                if (viewEnumerator.Current is TSD.View view && view.ViewType == TSD.View.ViewTypes.FrontView)
                {
                    var partEnumerator = view.GetAllObjects(typeof(TSD.Part));
                    while (partEnumerator.MoveNext())
                    {
                        if (partEnumerator.Current is TSD.Part drawingPart)
                        {
                            var modelPart = new TSM.Model().SelectModelObject(drawingPart.ModelIdentifier) as TSM.Part;
                            if (modelPart != null && !modelPart.Identifier.ID.Equals(mainPart.Identifier.ID) && !modelPart.Name.Contains("FALLTECH"))
                            {
                                parts.Add(modelPart);
                            }
                        }
                    }
                }
            }
            return parts;
        }

        private void ProcessFrontView(TSD.View view, TSM.Part mainPart, List<TSM.Part> parts, double output,
            List<SectionLocationWithParts> sectionList, List<SectionLocationWithParts> flangeSectionList,
            List<TSM.Part> bottomPartMarkRetainList, List<Guid> topViewDeleteGuids, string drgAttribute)
        {
            var points = new List<req_pts>();
            double heightPosNeg = _catalogHandler.Getcatalog_values(mainPart)[0] / 2;

            foreach (var part in parts)
            {
                TSD.PointList pointList = _boundingBoxHandler.BoundingBoxSort(part, mainPart as TSM.Beam);
                TSD.PointList convertedPoints = ConvertPoints(pointList, mainPart as TSM.Beam, view);

                double distanceX = (convertedPoints[0].X + convertedPoints[1].X) / 2;
                double distanceY = (convertedPoints[0].Y + convertedPoints[1].Y) / 2;
                string partMark = SkTeklaDrawingUtility.get_report_properties(part, "PART_POS");

                var pointData = new req_pts { distance = distanceX, list_of_points = convertedPoints, distance_for_y = distanceY, part = part, PART_MARK = partMark };
                points.Add(pointData);

                if (IsOutsideBounds(convertedPoints, output))
                {
                    topViewDeleteGuids.Add(part.Identifier.GUID);
                }
                if (convertedPoints[0].Y <= -heightPosNeg && convertedPoints[1].Y <= -heightPosNeg)
                {
                    bottomPartMarkRetainList.Add(part);
                }
            }

            points = points.OrderBy(x => x.distance).ToList();
            sectionList.AddRange(CreateSectionLocations(points, mainPart));
            CreateSectionViews(view, sectionList, mainPart, drgAttribute);
            flangeSectionList.AddRange(CompareSectionViews(points, view, mainPart, drgAttribute));
        }

        private bool IsOutsideBounds(TSD.PointList points, double output)
        {
            return points[0].X < 0 || points[1].X < 0 || points[0].X > output || points[1].X > output;
        }

        private List<SectionLocationWithParts> CreateSectionLocations(List<req_pts> points, TSM.Part mainPart)
        {
            var sectionList = new List<SectionLocationWithParts>();
            var currentParts = new List<TSM.Part>();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1 || Math.Abs(points[i + 1].distance - points[i].distance) > DistanceThreshold)
                {
                    currentParts.Add(points[i].part);
                    sectionList.Add(new SectionLocationWithParts { PartList = new List<TSM.Part>(currentParts), Distance = points[i].distance });
                    currentParts.Clear();
                }
                else
                {
                    currentParts.Add(points[i].part);
                }
            }

            MarkUniqueSections(sectionList, mainPart);
            return sectionList;
        }

        private void MarkUniqueSections(List<SectionLocationWithParts> sectionList, TSM.Part mainPart)
        {
            for (int i = sectionList.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    sectionList[i].SectionViewNeeded = "YES";
                }
                else
                {
                    bool isUnique = true;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (ComparePartMarkOrientation(sectionList[i].PartList, sectionList[j].PartList, mainPart as TSM.Beam))
                        {
                            isUnique = false;
                            sectionList[i].SectionViewNeeded = "NO";
                            sectionList[i].IndexOfSameSection = i - j - 1;
                            break;
                        }
                    }
                    if (isUnique)
                    {
                        sectionList[i].SectionViewNeeded = "YES";
                    }
                }
            }
        }

        private void CreateSectionViews(TSD.View currentView, List<SectionLocationWithParts> sectionList, TSM.Part mainPart, string drgAttribute)
        {
            var sectionMarks = new List<TSD.SectionMark>();
            foreach (var section in sectionList.Where(s => s.SectionViewNeeded == "YES"))
            {
                try
                {
                    double minX = section.PartList.Min(p => _boundingBoxHandler.BoundingBoxSort(p, currentView)[0].X);
                    double maxX = section.PartList.Max(p => _boundingBoxHandler.BoundingBoxSort(p, currentView)[1].X);
                    double distanceX = (minX + maxX) / 2;

                    TSG.Point p1 = new TSG.Point(distanceX, currentView.RestrictionBox.MaxPoint.Y, 0);
                    TSG.Point p2 = new TSG.Point(distanceX, currentView.RestrictionBox.MinPoint.Y, 0);

                    double depUp = Math.Min(maxX - distanceX, 5);
                    double depDown = Math.Min(distanceX - minX, 5);

                    TSD.View bottomView;
                    TSD.SectionMark sectionMark;
                    bool result = TSD.View.CreateSectionView(currentView, p2, p1, new TSG.Point(currentView.ExtremaCenter.X, 0, 0),
                        Convert.ToInt64(depUp) + 100, Convert.ToInt64(depDown) + 100,
                        new TSD.View.ViewAttributes(drgAttribute), new TSD.SectionMarkBase.SectionMarkAttributes(drgAttribute),
                        out bottomView, out sectionMark);

                    if (result)
                    {
                        bottomView.Attributes.LoadAttributes(drgAttribute);
                        bottomView.Modify();
                        section.MyView = bottomView;
                        sectionMarks.Add(sectionMark);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating section view: {ex.Message}");
                }
            }
        }

        private void ExtractRadiusDimension(List<TSD.RadiusDimension> radiusDimList, TSD.DrawingObjectEnumerator drawingViews)
        {
            while (drawingViews.MoveNext())
            {
                if (drawingViews.Current is TSD.View currentView && currentView.ViewType == TSD.View.ViewTypes.FrontView)
                {
                    var dimEnumerator = currentView.GetAllObjects(typeof(TSD.RadiusDimension));
                    while (dimEnumerator.MoveNext())
                    {
                        if (dimEnumerator.Current is TSD.RadiusDimension dimension)
                        {
                            radiusDimList.Add(dimension);
                        }
                    }
                }
            }
            drawingViews.Reset();
        }

        private TSD.StraightDimension DeleteViewDimensionByType(TSG.Point startWorkPoint, TSG.Point endWorkPoint, TSD.View currentView)
        {
            var dimensionTypes = new[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
            var dimensions = new List<DIMENSION_WITH_DIFFERNCE>();
            var originalDimensions = new List<DIMENSION_WITH_DIFFERNCE>();

            var dimEnumerator = currentView.GetAllObjects(dimensionTypes);
            while (dimEnumerator.MoveNext())
            {
                var dim = dimEnumerator.Current;
                if (currentView.ViewType == TSD.View.ViewTypes.FrontView)
                {
                    if (dim is TSD.StraightDimension straightDim)
                    {
                        double difference = straightDim.EndPoint.X - straightDim.StartPoint.X;
                        dimensions.Add(new DIMENSION_WITH_DIFFERNCE { MTDIM = straightDim, DIFFER = difference, MYVECTOR = straightDim.UpDirection });
                        originalDimensions.Add(new DIMENSION_WITH_DIFFERNCE { MTDIM = straightDim, DIFFER = difference, MYVECTOR = straightDim.UpDirection });
                    }
                    else if (dim is TSD.StraightDimensionSet set && set.Attributes.DimensionType == TSD.DimensionSetBaseAttributes.DimensionTypes.Elevation)
                    {
                        dim.Delete();
                    }
                    else
                    {
                        dim.Delete();
                    }
                }
                else
                {
                    dim.Delete();
                }
            }

            dimensions.RemoveAll(x => x.MYVECTOR.Y > 0);
            var maxDiff = dimensions.Max(d => d.DIFFER);
            var overallDim = dimensions.FirstOrDefault(d => d.DIFFER == maxDiff);
            var toDelete = originalDimensions.Where(d => d.DIFFER != maxDiff && (!_userInput.KnockOffDimension || !HasKnockOffDimension(d.MTDIM)));

            foreach (var dim in toDelete)
            {
                dim.MTDIM.Delete();
            }

            return overallDim?.MTDIM;
        }

        private void ModifyViewToScale(double scale, double miniLength, TSD.AssemblyDrawing drawing)
        {
            foreach (TSD.View view in drawing.GetSheet().GetAllViews())
            {
                view.Attributes.Scale = scale;
                view.Attributes.Shortening.MinimumLength = miniLength;
                view.Attributes.Shortening.CutPartType = TSD.View.ShorteningCutPartType.X_Direction;
                view.Modify();
            }
            drawing.Modify();
        }

        private bool HasKnockOffDimension(TSD.StraightDimension dimension)
        {
            return dimension.Value.Cast<object>().OfType<TSD.TextElement>().Any(text => text.Value.Contains("("));
        }

        private bool ComparePartMarkOrientation(List<TSM.Part> list1, List<TSM.Part> list2, TSM.Beam mainPart)
        {
            if (list1.Count != list2.Count) return false;

            var model = new TSM.Model();
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(mainPart.GetCoordinateSystem()));

            foreach (var part1 in list1)
            {
                bool matchFound = false;
                foreach (var part2 in list2)
                {
                    if (SkTeklaDrawingUtility.get_report_properties(part1, "PART_POS") == SkTeklaDrawingUtility.get_report_properties(part2, "PART_POS"))
                    {
                        var coord1 = part1.GetCoordinateSystem();
                        var coord2 = part2.GetCoordinateSystem();
                        if (Math.Round(coord1.Origin.Y) == Math.Round(coord2.Origin.Y) &&
                            Math.Round(coord1.Origin.Z) == Math.Round(coord2.Origin.Z) &&
                            VectorCheck(coord1.AxisX, coord2.AxisX) && VectorCheck(coord1.AxisY, coord2.AxisY))
                        {
                            matchFound = true;
                            break;
                        }
                    }
                }
                if (!matchFound) return false;
            }

            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            return true;
        }

        private bool VectorCheck(TSG.Vector vector1, TSG.Vector vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            return Math.Round(vector1.X) == Math.Round(vector2.X) &&
                   Math.Round(vector1.Y) == Math.Round(vector2.Y) &&
                   Math.Round(vector1.Z) == Math.Round(vector2.Z);
        }

        private List<SectionLocationWithParts> CompareSectionViews(List<req_pts> points, 
            TSD.View currentView, TSM.Part mainPart, string drgAttribute)
        {
            var sectionList = CreateSectionLocations(points, mainPart);
            CreateSectionViews(currentView, sectionList, mainPart, drgAttribute);
            return sectionList;
        }

        private TSD.PointList ConvertPoints(TSD.PointList points, TSM.Beam mainPart, TSD.View currentView)
        {
            var transformedPoints = new TSD.PointList();
            var toViewMatrix = TSG.MatrixFactory.FromCoordinateSystem(mainPart.GetCoordinateSystem());
            var toDisplayMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.DisplayCoordinateSystem);

            foreach (TSG.Point point in points)
            {
                TSG.Point transformed = toViewMatrix.Transform(point);
                transformedPoints.Add(toDisplayMatrix.Transform(transformed));
            }
            return transformedPoints;
        }

        public TSG.Point ConvertedPointsForChannel(TSM.Model mymodel, TSD.View current_view)
        {
            TSG.Point point = new TSG.Point(0, 0, 0);
            TSG.Matrix toGlobal = mymodel.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;

            TSG.Matrix toDisplayMatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            return toDisplayMatrix.Transform(toGlobal.Transform(point));
        }
    }

  
}