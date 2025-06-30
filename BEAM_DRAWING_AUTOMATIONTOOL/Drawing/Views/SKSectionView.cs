using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using System.Windows.Forms;
using SK.Tekla.Drawing.Automation.Support;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SK.Tekla.Drawing.Automation.Drawing;
using SK.Tekla.Drawing.Automation.Models;
using System.Reflection;
using SK.Tekla.Drawing.Automation.Drawing.Dimensions;
using SK.Tekla.Drawing.Automation.Drawing.Views;
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures;
using System.Diagnostics;
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using System.Web.UI.WebControls.WebParts;


namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    public class SKSectionView
    {
        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private readonly SKFacePointHandler facePointHandler;

        private readonly SKDrawingHandler drawingHandler;

        private readonly DuplicateRemover duplicateRemover;

        private readonly CustomInputModel _inputModel;

        private readonly StreamlineDrawing streamlineDrawing;

        private readonly SKAngleHandler skAngleHandler;

        private readonly SKBoltHandler skBoltHandler;

        public SKSectionView(SKCatalogHandler catalogHandler,
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
            SKFacePointHandler facePointHandler,
             SKDrawingHandler drawingHandler, DuplicateRemover duplicateRemover, CustomInputModel inputModel,
            StreamlineDrawing streamlineDrawing, SKAngleHandler skAngleHandler, SKBoltHandler skBoltHandler)
        {

            this.catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            this.boltMatrixHandler = boltMatrixHandler ?? throw new ArgumentNullException(nameof(boltMatrixHandler));
            this.boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            this.sortingHandler = sortingHandler ?? throw new ArgumentNullException(nameof(sortingHandler));
            this.facePointHandler = facePointHandler ?? throw new ArgumentNullException(nameof(facePointHandler));
            this.drawingHandler = drawingHandler ?? throw new ArgumentNullException(nameof(drawingHandler));
            this.duplicateRemover = duplicateRemover ?? throw new ArgumentNullException(nameof(duplicateRemover));
            _inputModel = inputModel ?? throw new ArgumentNullException(nameof(inputModel));
            this.streamlineDrawing = streamlineDrawing ?? throw new ArgumentNullException(nameof(streamlineDrawing));
            this.skAngleHandler = skAngleHandler ?? throw new ArgumentNullException(nameof(skAngleHandler));
            this.skBoltHandler = skBoltHandler ?? throw new ArgumentNullException(nameof(skBoltHandler));
        }

       
        private StraightDimensionSet.StraightDimensionSetAttributes CreateDimensionAttributes(string defaultADFile)
        {
            var attributes = new StraightDimensionSet.StraightDimensionSetAttributes
            {
                Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed },
                Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client,
                _inputModel.FontSize, defaultADFile) } }
            };
            return attributes;
        }

        public void SectionView(
           Model model,
           DrawingHandler drawingHandler,
           string defaultADFile,
           TSM.Part mainPart,
           List<SectionLocationWithParts> sectionList,
           double scale,
           List<double> mainPartProfileValues,
           StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes,
           List<double> catalogValues,
           List<TSD.View> sectionViewsInDrawing)
        {
            var dimAttributes = CreateDimensionAttributes(defaultADFile);

            foreach (var section in sectionList)
            {
                var currentView = section.MyView;
                if (currentView == null) continue;

                PrepareView(currentView, scale, drawingHandler, sectionViewsInDrawing);
                var (partMarksToRetain, boltMarksToRetainPos, boltMarksToRetainNeg)= 
                    ProcessSectionParts(section, currentView, model, mainPart, mainPartProfileValues, 
                    catalogValues, dimAttributes, fixedAttributes, defaultADFile);
                ManageMarks(currentView, section.RequiredPartList, drawingHandler,
                    partMarksToRetain, boltMarksToRetainPos, boltMarksToRetainNeg);
            }
        }

        private void PrepareView(TSD.View view, double scale, DrawingHandler drawingHandler, List<TSD.View> sectionViewsInDrawing)
        {
            DeleteExistingDimensions(view);
            view.Attributes.Scale = scale + _inputModel.SecScale;
            view.Modify();
            streamlineDrawing.SECTION_VIEW_PART_MARK_DELETE(view, drawingHandler);
            sectionViewsInDrawing.Add(view);
        }

        private void DeleteExistingDimensions(TSD.View view)
        {
            var dimensionTypes = new[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), 
                typeof(TSD.AngleDimension) };
            DrawingObjectEnumerator enumerator = view.GetAllObjects(dimensionTypes);
            while (enumerator.MoveNext())
            {
                enumerator.Current?.Delete();
            }
        }

        private (List<Guid> partMarksToRetain, List<Guid>  boltMarksToRetainPos, List<Guid>  boltMarksToRetainNeg) ProcessSectionParts(
            SectionLocationWithParts section,
            TSD.View currentView,
            Model model,
            TSM.Part mainPart,
            List<double> mainPartProfileValues,
            List<double> catalogValues,
            StraightDimensionSet.StraightDimensionSetAttributes dimAttributes,
            StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes,
            string defaultADFile)
        {
            var partMarksToRetain = new List<Guid>();
            var boltMarksToRetainPos = new List<Guid>();
            var boltMarksToRetainNeg = new List<Guid>();
            double height = (int)(mainPartProfileValues[0] / 2);
            double minY = currentView.RestrictionBox.MinPoint.Y;
            double maxY = currentView.RestrictionBox.MaxPoint.Y;

            foreach (var part in section.RequiredPartList.OfType<TSM.Part>())
            {
                partMarksToRetain.Add(part.Identifier.GUID);
                CreateBoltGageDimensions(part, currentView, mainPart, mainPartProfileValues, 
                    catalogValues, height, minY, maxY, dimAttributes,
                    partMarksToRetain, boltMarksToRetainPos, boltMarksToRetainNeg);
                Create3x3Dimensions(part, currentView, mainPartProfileValues, height, 
                    fixedAttributes, ref boltMarksToRetainPos, ref boltMarksToRetainNeg);
                HandlePourStopper(part, currentView, mainPart, mainPartProfileValues, fixedAttributes, minY, maxY);
                HandleSeatingAngle(part, currentView, mainPart, mainPartProfileValues, 
                    fixedAttributes, catalogValues, defaultADFile);
            }
            return (partMarksToRetain,boltMarksToRetainPos,boltMarksToRetainNeg);
        }

        private void CreateBoltGageDimensions(
            TSM.Part part,
            TSD.View view,
            TSM.Part mainPart,
            List<double> mainPartProfileValues,
            List<double> catalogValues,
            double height,
            double minY,
            double maxY,
            StraightDimensionSet.StraightDimensionSetAttributes attributes,
            List<Guid> partMarksToRetain, List<Guid> boltMarksToRetainPos, List<Guid> boltMarksToRetainNeg)
        {
            var rdPointList = new PointList();
            var profileType = SkTeklaDrawingUtility.get_report_properties(part, "PROFILE_TYPE");

            if (profileType == "L")
            {
                var anglePoints = boundingBoxHandler.BoundingBoxSort(part, view, SKSortingHandler.SortBy.Y);
                skAngleHandler.angle_place_check_for_hole_locking(anglePoints, 
                    out rdPointList, part.GetBolts(), view, 
                    ref partMarksToRetain, ref boltMarksToRetainPos, ref boltMarksToRetainNeg);
            }
            else
            {
                var bolts = part.GetBolts();
                {
                    while (bolts.MoveNext())
                    {
                        if (bolts.Current is TSM.BoltGroup boltGroup)
                        {
                            var boltMatrix = boltMatrixHandler.GetBoltMatrix(boltGroup, view);
                            FilterBoltMatrixPoints(boltMatrix, view, rdPointList);
                        }
                    }
                }
            }

            var finalRdList = duplicateRemover.RemoveDuplicateXValues(rdPointList);
            var (withinHeight, aboveFlange, belowFlange) = SplitPointsByHeight(finalRdList, height, mainPart, catalogValues);
            CreateDimensions(view, withinHeight, aboveFlange, belowFlange, minY, maxY, attributes);
        }

        private (PointList withinHeight, PointList aboveFlange, PointList belowFlange) SplitPointsByHeight(
            PointList points,
            double height,
            TSM.Part mainPart,
            List<double> catalogValues)
        {
            var withinHeight = new PointList();
            var aboveFlange = new PointList();
            var belowFlange = new PointList();
            double wt = CalculateWebThickness(mainPart, catalogValues);

            withinHeight.Add(new TSG.Point(wt, mainPart.Profile.ProfileString.Contains("U") ? catalogValues[0] / 2 : 0, 0));
            aboveFlange.Add(new TSG.Point(wt, mainPart.Profile.ProfileString.Contains("U") ? catalogValues[0] / 2 : 0, 0));
            belowFlange.Add(new TSG.Point(wt, mainPart.Profile.ProfileString.Contains("U") ? catalogValues[0] / 2 : 0, 0));

            foreach (TSG.Point point in points)
            {
                int y = (int)point.Y;
                if (y < height && y > -height) withinHeight.Add(point);
                else if (y > height) aboveFlange.Add(point);
                else if (y < -height) belowFlange.Add(point);
            }

            sortingHandler.SortPoints(withinHeight, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
            sortingHandler.SortPoints(aboveFlange, SKSortingHandler.SortBy.Y);
            sortingHandler.SortPoints(belowFlange, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);

            return (withinHeight, aboveFlange, belowFlange);
        }

        private double CalculateWebThickness(TSM.Part mainPart, List<double> catalogValues)
        {
            if (!mainPart.Profile.ProfileString.Contains("U")) return 0;

            var zVector = mainPart.GetCoordinateSystem().AxisX.Cross(mainPart.GetCoordinateSystem().AxisY);
            zVector.Normalize();
            return zVector.X > 0 ? -catalogValues[1] / 2 : catalogValues[1] / 2;
        }

        private void FilterBoltMatrixPoints(TSG.Point[,] boltMatrix, TSD.View view, PointList rdPointList)
        {
            if (boltMatrix == null) return;

            double upperLimit = view.RestrictionBox.MaxPoint.Z;
            double lowerLimit = view.RestrictionBox.MinPoint.Z;

            if (boltMatrix[0, 0].Z > lowerLimit && boltMatrix[0, 0].Z < upperLimit)
            {
                int yLength = boltMatrix.GetLength(0);
                for (int i = 0; i < boltMatrix.GetLength(1); i++)
                {
                    rdPointList.Add(boltMatrix[yLength - 1, i]);
                }
            }
        }

        private void CreateDimensions(TSD.View view, PointList withinHeight, PointList aboveFlange, PointList belowFlange, double minY, double maxY, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var handler = new StraightDimensionSetHandler();
            {
                TryCreateDimensionSet(handler, view, withinHeight, new TSG.Vector(0, 1, 0), Math.Abs(withinHeight[0].Y - maxY) + 75, attributes);
                TryCreateDimensionSet(handler, view, aboveFlange, new TSG.Vector(0, 1, 0), Math.Abs(aboveFlange[0].Y - maxY) + 150, attributes);
                TryCreateDimensionSet(handler, view, belowFlange, new TSG.Vector(0, -1, 0), Math.Abs(belowFlange[0].Y - minY) + 75, attributes);
            }
        }

        private void TryCreateDimensionSet(StraightDimensionSetHandler handler, TSD.View view, PointList points, TSG.Vector vector, double distance, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            if (points.Count < 2) return;
            try
            {
                handler.CreateDimensionSet(view, points, vector, distance, attributes);
            }
            catch (Exception ex)
            {
                // Log exception here if logging is available
                System.Diagnostics.Debug.WriteLine($"Failed to create dimension set: {ex.Message}");
            }
        }

        private void Create3x3Dimensions(
            TSM.Part part,
            TSD.View view,
            List<double> mainPartProfileValues,
            double height,
            StraightDimensionSet.StraightDimensionSetAttributes attributes,
            ref List<Guid> boltMarksToRetainPos,
            ref List<Guid> boltMarksToRetainNeg)
        {
            if (skBoltHandler.BOLT_IN_VIEW(part, view) != "NEED") return;

            var boltPoints = Collect3x3BoltPoints(part, view);
            var (positivePoints, negativePoints) = Split3x3Points(boltPoints);
            var profileType = SkTeklaDrawingUtility.get_report_properties(part, "PROFILE_TYPE");

            if (profileType == "L")
            {
                skBoltHandler.ANGLE_BOLT_DIM(positivePoints, negativePoints, height, view, mainPartProfileValues, view.RestrictionBox.MaxPoint.X, view.RestrictionBox.MinPoint.X, "");
            }
            else
            {
                positivePoints.Add(new TSG.Point(mainPartProfileValues[1] / 2, mainPartProfileValues[0] / 2, 0));
                negativePoints.Add(new TSG.Point(-mainPartProfileValues[1] / 2, mainPartProfileValues[0] / 2, 0));

                var handler = new StraightDimensionSetHandler();
                {
                    TryCreateDimensionSet(handler, view, positivePoints, new TSG.Vector(1, 0, 0), Math.Abs(positivePoints[0].X - view.RestrictionBox.MaxPoint.X) + 75, attributes);
                    TryCreateDimensionSet(handler, view, negativePoints, new TSG.Vector(-1, 0, 0), Math.Abs(negativePoints[0].X - view.RestrictionBox.MinPoint.X) + 75, attributes);
                }
            }
        }

        private PointList Collect3x3BoltPoints(TSM.Part part, TSD.View view)
        {
            var points = new PointList();
            var bolts = part.GetBolts();
            {
                while (bolts.MoveNext())
                {
                    if (bolts.Current is TSM.BoltGroup boltGroup)
                    {
                        var matrix = boltMatrixHandler.GetBoltMatrix(boltGroup, view);
                        if (matrix != null && IsWithinDepth(matrix, view))
                        {
                            Add3x3Points(matrix, points);
                        }
                    }
                }
            }
            return points;
        }

        private bool IsWithinDepth(TSG.Point[,] matrix, TSD.View view)
        {
            return matrix[0, 0].Z > view.RestrictionBox.MinPoint.Z && matrix[0, 0].Z < view.RestrictionBox.MaxPoint.Z;
        }

        private void Add3x3Points(TSG.Point[,] matrix, PointList points)
        {
            int xLength = matrix.GetLength(1);
            int yLength = matrix.GetLength(0);

            if (matrix[0, 0].X < 0 && matrix[0, xLength - 1].X > 0 || matrix[0, 0].X > 0 && matrix[0, xLength - 1].X < 0)
            {
                // Handle cross-zero case if needed
            }
            else if (matrix[0, 0].X > 0)
            {
                for (int i = 0; i < yLength; i++) points.Add(matrix[i, xLength - 1]);
            }
            else if (matrix[0, 0].X < 0)
            {
                for (int i = 0; i < yLength; i++) points.Add(matrix[i, 0]);
            }
        }

        private (PointList positive, PointList negative) Split3x3Points(PointList points)
        {
            var positive = new PointList();
            var negative = new PointList();
            foreach (TSG.Point point in points)
            {
                if ((int)point.X > 0) positive.Add(point);
                else if ((int)point.X < 0) negative.Add(point);
            }
            return (positive, negative);
        }

        private void HandlePourStopper(
            TSM.Part part,
            TSD.View view,
            TSM.Part mainPart,
            List<double> mainPartProfileValues,
            StraightDimensionSet.StraightDimensionSetAttributes attributes,
            double minY,
            double maxY)
        {
            if (part.Name.Contains("STUD") || part.Equals(mainPart) || part.Name.Contains("GUSSET")) return;

            var points = boundingBoxHandler.BoundingBoxSort(part, view, SKSortingHandler.SortBy.Z);
            if (points.Count < 2) return;

            double height = (int)(mainPartProfileValues[0] / 2);
            var profileType = SkTeklaDrawingUtility.get_report_properties(part, "PROFILE_TYPE");

            if (IsAboveHeight(points, height))
            {
                ProcessPourStopperAbove(points, view, part, profileType, maxY, attributes);
            }
            else if (IsBelowHeight(points, height))
            {
                ProcessPourStopperBelow(points, view, part, profileType, minY, attributes);
            }
        }

        private bool IsAboveHeight(PointList points, double height) => (int)points[0].Y >= height && (int)points[1].Y >= height;
        private bool IsBelowHeight(PointList points, double height) => (int)points[0].Y <= -height && (int)points[1].Y >= -height;

        private void ProcessPourStopperAbove(PointList points, TSD.View view, TSM.Part part, string profileType, double maxY, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var finalPoints = new PointList();
            var legPoints = new PointList();
            bool hasBolts = part.GetBolts().GetSize() > 0;

            if (hasBolts && profileType != "L" && AreBoltsOutside(part, view))
            {
                AddPourStopperPoints(points, finalPoints, legPoints, (int)points[0].X, (int)points[1].X);
                CreatePourStopperDimensions(view, finalPoints, legPoints, maxY, attributes);
            }
            else if (!hasBolts)
            {
                AddPourStopperPoints(points, finalPoints, legPoints, (int)points[0].X, (int)points[1].X);
                CreatePourStopperDimensions(view, finalPoints, legPoints, maxY, attributes);
            }
        }

        private void ProcessPourStopperBelow(PointList points, TSD.View view, TSM.Part part, string profileType, double minY, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var finalPoints = new PointList();
            var legPoints = new PointList();
            bool hasBolts = part.GetBolts().GetSize() > 0;

            if (hasBolts && profileType != "L" && AreBoltsOutside(part, view))
            {
                AddPourStopperPoints(points, finalPoints, legPoints, (int)points[0].X, (int)points[1].X);
                CreatePourStopperDimensions(view, finalPoints, legPoints, minY, attributes, true);
            }
            else if (!hasBolts)
            {
                AddPourStopperPoints(points, finalPoints, legPoints, (int)points[0].X, (int)points[1].X);
                CreatePourStopperDimensions(view, finalPoints, legPoints, minY, attributes, true);
            }
        }

        private bool AreBoltsOutside(TSM.Part part, TSD.View view)
        {
            var bolts = part.GetBolts();
            {
                var boltMatrices = new List<TSG.Point[,]>();
                while (bolts.MoveNext())
                {
                    if (bolts.Current is TSM.BoltGroup boltGroup)
                    {
                        boltMatrices.Add(boltMatrixHandler.GetBoltMatrix(boltGroup, view));
                    }
                }
                return boltMatrices.All(m => m == null);
            }
        }

        private void AddPourStopperPoints(PointList points, PointList finalPoints, PointList legPoints, int x0, int x1)
        {
            if (x0 >= 0 && x1 >= 0)
            {
                finalPoints.Add(points[1]);
                finalPoints.Add(new TSG.Point(0, 0, 0));
                legPoints.Add(points[0]);
                legPoints.Add(points[1]);
            }
            else if (x0 < 0 && x1 < 0)
            {
                finalPoints.Add(new TSG.Point(points[0].X, points[1].Y, 0));
                finalPoints.Add(new TSG.Point(0, 0, 0));
                legPoints.Add(points[0]);
                legPoints.Add(points[1]);
            }
            else if (x0 < 0 && x1 > 0)
            {
                finalPoints.Add(new TSG.Point(points[1].X, points[1].Y, 0));
                finalPoints.Add(new TSG.Point(0, 0, 0));
                legPoints.Add(points[0]);
                legPoints.Add(points[1]);
            }
        }

        private void CreatePourStopperDimensions(TSD.View view, PointList finalPoints, PointList legPoints, double yBoundary, StraightDimensionSet.StraightDimensionSetAttributes attributes, bool below = false)
        {
            var handler = new StraightDimensionSetHandler();
            {
                var vector = below ? new TSG.Vector(0, -1, 0) : new TSG.Vector(0, 1, 0);
                TryCreateDimensionSet(handler, view, finalPoints, vector, Math.Abs(finalPoints[0].Y - yBoundary) + 150, attributes);

                var legVector = finalPoints[0].X > 0 ? new TSG.Vector(1, 0, 0) : new TSG.Vector(-1, 0, 0);
                TryCreateDimensionSet(handler, view, legPoints, legVector, Math.Abs(legPoints[0].X - view.RestrictionBox.MaxPoint.X) + 150, attributes);
            }
        }

        private void HandleSeatingAngle(
            TSM.Part part,
            TSD.View view,
            TSM.Part mainPart,
            List<double> mainPartProfileValues,
            StraightDimensionSet.StraightDimensionSetAttributes attributes,
            List<double> catalogValues,
            string defaultADFile)
        {
            var profileType = SkTeklaDrawingUtility.get_report_properties(part, "PROFILE_TYPE");
            if (profileType != "B") return;

            var zVector = part.GetCoordinateSystem().AxisX.Cross(part.GetCoordinateSystem().AxisY);
            if (zVector.Z == 0) return;

            var points = boundingBoxHandler.BoundingBoxSort(part, view, SKSortingHandler.SortBy.Z);
            double height = (int)(mainPartProfileValues[0] / 2);

            var bolts = part.GetBolts();
            {
                if (bolts.GetSize() == 0)
                {
                    ProcessNoBoltSeatingAngle(part, view, points, height, attributes);
                }
                else
                {
                    ProcessBoltSeatingAngle(part, view, points, height, catalogValues, defaultADFile);
                }
            }
        }

        private void ProcessNoBoltSeatingAngle(TSM.Part part, TSD.View view, PointList points, double height, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var finalPoints = new PointList();
            var legPoints = new PointList { points[0], points[1] };

            finalPoints.Add(GetSeatingAnglePoint(part, points));
            finalPoints.Add(new TSG.Point(0, height, 0));

            var vector = points[1].X > 0 ? new TSG.Vector(1, 0, 0) : new TSG.Vector(-1, 0, 0);
            var handler = new StraightDimensionSetHandler();
            {
                TryCreateDimensionSet(handler, view, finalPoints, vector, Math.Abs(finalPoints[0].Y - view.RestrictionBox.MinPoint.Y) + 150, attributes);
                TryCreateDimensionSet(handler, view, legPoints, vector, Math.Abs(legPoints[0].X - view.RestrictionBox.MaxPoint.X) + 150, attributes);
            }
        }

        private TSG.Point GetSeatingAnglePoint(TSM.Part part, PointList points)
        {
            if (part.Position.Rotation == TSM.Position.RotationEnum.BACK)
                return new TSG.Point(points[1].X, points[1].Y, 0);
            if (part.Position.Rotation == TSM.Position.RotationEnum.FRONT && (part.Position.Plane == TSM.Position.PlaneEnum.LEFT || part.Position.Plane == TSM.Position.PlaneEnum.RIGHT))
                return new TSG.Point(points[1].X, points[0].Y, 0);
            return points[1];
        }

        private void ProcessBoltSeatingAngle(TSM.Part part, TSD.View view, PointList points, double height, List<double> catalogValues, string defaultADFile)
        {
            var bolts = part.GetBolts();
            while (bolts.MoveNext())
            {
                if (bolts.Current is TSM.BoltGroup boltGroup)
                {
                    var matrix = boltMatrixHandler.GetBoltMatrix(boltGroup, view);
                    if (matrix == null)
                    {
                        var connectingPoints = GetConnectingSidePoints(part, boltGroup, view, points);
                        connectingPoints.Add(new TSG.Point(0, height, 0));
                        var handler = new StraightDimensionSetHandler();
                        {
                            var vector = points[0].X > 0 ? new TSG.Vector(-1, 0, 0) : new TSG.Vector(1, 0, 0);
                            TryCreateDimensionSet(handler, view, connectingPoints,
                                vector, Math.Abs(view.RestrictionBox.MaxPoint.X) + 100,
                                CreateDimensionAttributes(defaultADFile));
                        }
                    }
                }
            }
        }

        private PointList GetConnectingSidePoints(TSM.Part part, TSM.BoltGroup boltGroup, TSD.View view, PointList points)
        {
            var connectingPoints = new PointList();
            var yPoints = boundingBoxHandler.BoundingBoxSort(part, view, SKSortingHandler.SortBy.Y);
            var partToBeBolted = boltGroup.PartToBeBolted;
            var partToBoltTo = boltGroup.PartToBoltTo;

            if (!partToBeBolted.Identifier.ID.Equals(part.Identifier.ID))
            {
                AddConnectingPoint(partToBeBolted, view, yPoints, connectingPoints);
            }
            if (!partToBoltTo.Identifier.ID.Equals(part.Identifier.ID))
            {
                AddConnectingPoint(partToBoltTo, view, yPoints, connectingPoints);
            }
            return connectingPoints;
        }

        private void AddConnectingPoint(TSM.Part relatedPart, TSD.View view, PointList yPoints, PointList connectingPoints)
        {
            var coord = relatedPart.GetCoordinateSystem();
            var transformedOrigin = TSG.MatrixFactory.ToCoordinateSystem(view.ViewCoordinateSystem).Transform(coord.Origin);
            int yValue = (int)transformedOrigin.Y;

            if (yValue > (int)yPoints[1].Y)
                connectingPoints.Add(new TSG.Point(yPoints[1].X, yPoints[1].Y, yPoints[0].Z));
            else if (yValue < (int)yPoints[0].Y)
                connectingPoints.Add(new TSG.Point(yPoints[0].X, yPoints[0].Y, yPoints[0].Z));
        }

        private void ManageMarks(TSD.View view, List<TSM.Part> requiredParts, DrawingHandler drawingHandler,
            List<Guid> partMarksToRetain, List<Guid> boltMarksToRetainPos, List<Guid> boltMarksToRetainNeg)
        {
            var partMarks = CollectPartMarks(view, requiredParts, partMarksToRetain);
            var boltMarksPos = CollectBoltMarks(view, true, boltMarksToRetainPos);
            var boltMarksNeg = CollectBoltMarks(view, false, boltMarksToRetainNeg);

            ApplyMarks(drawingHandler, partMarks, boltMarksPos, boltMarksNeg);
        }

        private ArrayList CollectPartMarks(TSD.View view, List<TSM.Part> requiredParts, List<Guid> partMarksToRetain)
        {
            ArrayList partMarks = new ArrayList();
            //var partMarks = new List<TSD.Part>();
            var enumerator = view.GetAllObjects(new[] { typeof(TSD.Part) });
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is TSD.Part part)
                    {
                        var modelPart = new Model().SelectModelObject(part.ModelIdentifier);
                        if (partMarksToRetain.Any(p => p.Equals(modelPart.Identifier.ID)))
                        {
                            partMarks.Add(part);
                        }
                    }
                }
            }
            return partMarks;
        }

        private ArrayList CollectBoltMarks(TSD.View view, bool positive, List<Guid> boltMarksToRetain)
        {
            ArrayList boltMarks = new ArrayList();
            //var boltMarks = new List<TSD.Bolt>();
            var enumerator = view.GetAllObjects(new[] { typeof(TSD.Bolt) });
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is TSD.Bolt bolt)
                    {
                        TSM.ModelObject modelBolt = new TSM.Model().SelectModelObject(bolt.ModelIdentifier);
                        if (boltMarksToRetain.Any(p => p.Equals(modelBolt.Identifier.ID)))
                        {
                            boltMarks.Add(bolt);
                        }
                    }
                }
            }
            return boltMarks;
        }

        private void ApplyMarks(DrawingHandler drawingHandler, ArrayList partMarks,
            ArrayList boltMarksPos, ArrayList boltMarksNeg)
        {
            drawingHandler.GetDrawingObjectSelector().SelectObjects(boltMarksPos, true);
            TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
            drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();

            drawingHandler.GetDrawingObjectSelector().SelectObjects(boltMarksNeg, true);
            TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
            drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();

            foreach (var part in partMarks)
            {
                drawingHandler.GetDrawingObjectSelector().SelectObject((TSD.Part)part);
                TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();
            }
        }

    }
}
