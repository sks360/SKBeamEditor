using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;

using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;

using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using Tekla.Structures.Geometry3d;
using System.Collections;
using System.Net;
using Tekla.Structures.Drawing;

namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class CopeDimensions
    {
        private readonly CustomInputModel _userInput;

        private readonly string client;
        private readonly FontSizeSelector fontSize;
        private readonly SKCatalogHandler catalogHandler;

        private const double XDimDistance = 70;
        private const double YDimDistance = 85;
        private const double CutDimDistance = 40;
        private const double Tolerance = 1e-6;

        public CopeDimensions(SKCatalogHandler catalogHandler,  CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }


        public void CreateCopeDimensions(TSD.View currentView, TSM.Beam mainPart, string drawingAttributes)
        {
            if (currentView == null || mainPart == null || string.IsNullOrEmpty(drawingAttributes))
                throw new ArgumentException("Invalid input parameters.");

            TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                new TSD.StraightDimensionSet.StraightDimensionSetAttributes
            {
                Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No,
                Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
            };

            List<double> values = catalogHandler.Getcatalog_values(mainPart);
            double size1M = Convert.ToDouble(values[0]);
            double size2M = Convert.ToDouble(values[1]);
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.DisplayCoordinateSystem);
            double size = currentView.ViewType == TSD.View.ViewTypes.FrontView ? size1M / 2 : size2M / 2;
            double length = 0;
            mainPart.GetReportProperty("LENGTH", ref length);

            TSM.ModelObjectEnumerator booleanParts = mainPart.GetBooleans();
            while (booleanParts.MoveNext())
            {
                TSM.ModelObject partCut = booleanParts.Current;
                if (partCut is TSM.BooleanPart booleanPart && booleanPart.OperativePart is TSM.ContourPlate contourPlate)
                {
                    List<Point> pointsInViewCoords = contourPlate.Contour.ContourPoints
                        .Cast<Point>()
                        .Select(p => toViewMatrix.Transform(p))
                        .ToList();

                    if (pointsInViewCoords.Count != 4) continue;

                    double y0 = pointsInViewCoords[0].Y;
                    double y2 = pointsInViewCoords[2].Y;
                    if (Math.Abs(y0 - y2) > Tolerance)
                    {
                        List<double> xList = pointsInViewCoords.Select(p => p.X).ToList();
                        List<double> yList = pointsInViewCoords.Select(p => p.Y).ToList();
                        double xMin = xList.Min();
                        double xMax = xList.Max();
                        double yMin = yList.Min();
                        double yMax = yList.Max();

                        Point point1, point2;
                        Vector dimVectorForXDim, dimVectorForYDim;

                        if (xMin <= 0 && yMin <= 0) // Third quadrant
                        {
                            point2 = new Point(0, yMax, 0);
                            point1 = new Point(xMax, -size, 0);
                            dimVectorForXDim = new Vector(0, -1, 0);
                            dimVectorForYDim = new Vector(-1, 0, 0);
                        }
                        else if (xMin <= 0 && yMin >= 0) // Second quadrant
                        {
                            point2 = new Point(0, yMin, 0);
                            point1 = new Point(xMax, size, 0);
                            dimVectorForXDim = new Vector(0, 1, 0);
                            dimVectorForYDim = new Vector(-1, 0, 0);
                        }
                        else if (xMin >= 0 && yMin <= 0) // Fourth quadrant
                        {
                            point1 = new Point(xMin, -size, 0);
                            point2 = new Point(length, yMax, 0);
                            dimVectorForXDim = new Vector(0, -1, 0);
                            dimVectorForYDim = new Vector(1, 0, 0);
                        }
                        else // First quadrant
                        {
                            point1 = new Point(xMin, size, 0);
                            point2 = new Point(length, yMin, 0);
                            dimVectorForXDim = new Vector(0, 1, 0);
                            dimVectorForYDim = new Vector(1, 0, 0);
                        }

                        TSD.PointList pointsX = new TSD.PointList { point1, point2 };
                        CreateDimensionSet(currentView, pointsX, dimVectorForXDim, XDimDistance, attributes);

                        TSD.PointList pointsY = new TSD.PointList { point2, point1 };
                        CreateDimensionSet(currentView, pointsY, dimVectorForYDim, YDimDistance, attributes);
                    }
                }
            }
        }

        public void ProvideFittingCutDimensions(TSM.Model currentModel, TSD.View currentView, 
            TSM.Beam mainPart, string drawingAttributes)
        {
            if (currentModel == null || currentView == null || mainPart == null || string.IsNullOrEmpty(drawingAttributes))
                throw new ArgumentException("Invalid input parameters.");

            currentModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            double width = 0;
            mainPart.GetReportProperty("WIDTH", ref width);
            Matrix toViewMatrix = MatrixFactory.ToCoordinateSystem(currentView.DisplayCoordinateSystem);
            Matrix toPartMatrix = MatrixFactory.ToCoordinateSystem(mainPart.GetCoordinateSystem());

            List<TSS.Edge> topEdges = new List<TSS.Edge>();
            List<TSS.Edge> bottomEdges = new List<TSS.Edge>();
            Dictionary<Point, Point> allPointsLocal = new Dictionary<Point, Point>();

            TSS.EdgeEnumerator edges = mainPart.GetSolid().GetEdgeEnumerator();
            while (edges.MoveNext())
            {
                TSS.Edge edge = edges.Current as TSS.Edge;
                if (edge != null)
                {
                    Point start = edge.StartPoint;
                    Point end = edge.EndPoint;
                    //fetch distinct values
                    if (!allPointsLocal.ContainsKey(start)) allPointsLocal[start] = toPartMatrix.Transform(start);
                    if (!allPointsLocal.ContainsKey(end)) allPointsLocal[end] = toPartMatrix.Transform(end);
                }
            }
            //TSG.Point min = allPointsLocal.Aggregate((l, r) => l.Value.Y < r.Value.Y ? l : r).Value;
            //TSG.Point max = allPointsLocal.Aggregate((l, r) => l.Value.Y > r.Value.Y ? l : r).Value;
            //double min_Z = Math.Round(min.Y, 2);
            //double max_Z = Math.Round(max.Y, 2);
            //Console.WriteLine($"old code: {min_Z}: {max_Z}");
            double maxZ = allPointsLocal.Values.Max(p => p.Y);
            double minZ = allPointsLocal.Values.Min(p => p.Y);
            Console.WriteLine($"new code: {minZ}: {maxZ}");
            edges.Reset();
            //filter top & bottom edge faces
            while (edges.MoveNext())
            {
                TSS.Edge edge = edges.Current as TSS.Edge;
                Point startLocal = allPointsLocal[edge.StartPoint];
                Point endLocal = allPointsLocal[edge.EndPoint];
                if (Math.Abs(startLocal.Y - endLocal.Y) < Tolerance)
                {
                    if (Math.Abs(startLocal.Y - maxZ) < Tolerance) topEdges.Add(edge);
                    else if (Math.Abs(startLocal.Y - minZ) < Tolerance) bottomEdges.Add(edge);
                }
            }
            if (topEdges.Count < 5)
            {
                return;
            }
            TSS.Edge topChamferEdge = topEdges.FirstOrDefault(e => !IsAxisAligned(e, mainPart));
            TSS.Edge bottomChamferEdge = bottomEdges.FirstOrDefault(e => !IsAxisAligned(e, mainPart));

            if (topChamferEdge != null)
            {
                TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                    new TSD.StraightDimensionSet.StraightDimensionSetAttributes
                {
                    Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                    Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
                };
                Point referencePoint = toViewMatrix.Transform(mainPart.GetCoordinateSystem().Origin);
                CreateCutDimensions(currentView, toViewMatrix, referencePoint, topChamferEdge, width, mainPart, attributes);
            }

            if (bottomChamferEdge != null)
            {
                TSD.AssemblyDrawing beamDrawing = currentView.GetDrawing() as TSD.AssemblyDrawing;
                TSD.DrawingObjectEnumerator viewEnumerator = beamDrawing.GetSheet().GetAllViews();
                while (viewEnumerator.MoveNext())
                {
                    TSD.View view = viewEnumerator.Current as TSD.View;
                    if (view != null && view.ViewType == TSD.View.ViewTypes.FrontView)
                    {
                        Matrix toViewMatrix1 = TSG.MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
                        Point tp1 = toViewMatrix1.Transform(bottomChamferEdge.StartPoint + new Point(0, width / 2, 0));
                        Point tp2 = toViewMatrix1.Transform(bottomChamferEdge.EndPoint + new Point(0, width / 2, 0));
                        Point p1, p2;
                        if (tp1.X > tp2.X)
                        {
                            p1 = tp1 + new Point(150, 40, 0);
                            p2 = tp2 - new Point(150, -40, 0);
                        }
                        else
                        {
                            p1 = tp2 + new Point(150, 0, 0);
                            p2 = tp1 - new Point(150, 0, 0);
                        }

                        TSD.View sectionView;
                        TSD.SectionMark sectionMark;
                        bool result = TSD.View.CreateSectionView(view, p1, p2, 
                            new Point(currentView.ExtremaCenter.X, 0, 0), 280, 280,
                            new TSD.View.ViewAttributes("SK_BEAM_A1"), 
                            new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"),
                            out sectionView, out sectionMark);

                        if (result)
                        {
                            sectionView.Attributes.LoadAttributes("SK_BEAM_A1");
                            sectionView.Modify();

                            Matrix toViewMatrix2 = TSG.MatrixFactory.ToCoordinateSystem(sectionView.DisplayCoordinateSystem);
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                                new TSD.StraightDimensionSet.StraightDimensionSetAttributes
                            {
                                Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                                Text = { 
                                        Font = { Height = 
                                        SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes),
                                        Color = TSD.DrawingColors.Magenta} }
                            };
                            ConfigureSectionView(sectionView, sectionMark);
                            Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), /*typeof(TSD.AngleDimension),*/ typeof(TSD.Mark) };
                            TSD.DrawingObjectEnumerator dim_drg = sectionView.GetAllObjects(type_for_dim);
                            while (dim_drg.MoveNext())
                            {
                                var obj = dim_drg.Current;
                                obj.Delete();
                            }

                            Point referencePoint1 = toViewMatrix2.Transform(mainPart.GetCoordinateSystem().Origin);
                            CreateCutDimensions(sectionView, toViewMatrix2, referencePoint1, 
                                bottomChamferEdge, width, mainPart, attributes);
                            //on center line
                            sectionView.GetDrawing().CommitChanges();
                            Type type_for_part = typeof(TSD.Part);
                            TSD.DrawingObjectEnumerator drawingObjectEnumerator1 = sectionView.GetAllObjects(type_for_part);
                            while (drawingObjectEnumerator1.MoveNext())
                            {
                                TSD.Part part = drawingObjectEnumerator1.Current as TSD.Part;
                                if (part != null)
                                {
                                    if ((currentModel.SelectModelObject(part.ModelIdentifier) as TSM.Part).Identifier.GUID.ToString() 
                                        == mainPart.Identifier.GUID.ToString())
                                    {
                                        part.Attributes.DrawCenterLine = true;
                                        part.Attributes.DrawOrientationMark = false;
                                        part.Attributes.SymbolOffset = 0;
                                        part.Modify();
                                        sectionView.Modify();
                                        sectionView.GetDrawing().CommitChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsAxisAligned(TSS.Edge edge, TSM.Beam mainPart)
        {
            TSG.Line line = new TSG.Line(edge.StartPoint, edge.EndPoint);
            double angle = line.Direction.GetAngleBetween(mainPart.GetCoordinateSystem().AxisX) * 180 / Math.PI;
            return Math.Abs(angle % 90) < Tolerance || Math.Abs((angle % 90) - 90) < Tolerance;
        }

        private void CreateDimensionSet(TSD.View view, TSD.PointList points, Vector dimensionVector, 
            double distance, TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            TSD.StraightDimensionSetHandler dimensionHandler = new TSD.StraightDimensionSetHandler();
            try
            {
                dimensionHandler.CreateDimensionSet(view, points, dimensionVector, distance, attributes);
            }
            catch (Exception ex)
            {
                //TODO replace with error log 
                Console.WriteLine($"Error creating dimension set: {ex.Message}");
            }
        }

        private void CreateCutDimensions(TSD.View view, Matrix toViewMatrix, 
            Point referencePoint, TSS.Edge chamferEdge, double width, TSM.Beam mainPart,
            TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            TSD.StraightDimensionSetHandler dimensionSet = new TSD.StraightDimensionSetHandler();
            Point start = toViewMatrix.Transform(chamferEdge.StartPoint);
            Point end = toViewMatrix.Transform(chamferEdge.EndPoint);
            TSD.PointList points = new TSD.PointList();

            if (start.Y > referencePoint.Y && end.Y > referencePoint.Y)
            {
                points.Add(start);
                points.Add(end);
                dimensionSet.CreateDimensionSet(view, points, new Vector(0, 1, 0), CutDimDistance, attributes);
            }
            else
            {
                points.Add(start);
                points.Add(end);
                dimensionSet.CreateDimensionSet(view, points, new Vector(0, -1, 0), CutDimDistance, attributes);
            }

            points.Clear();
            double midX = (toViewMatrix.Transform(mainPart.GetSolid().MinimumPoint).X + 
                toViewMatrix.Transform(mainPart.GetSolid().MaximumPoint).X) / 2;

            if (start.X < midX && end.X < midX)
            {
                if (start.X < end.X)
                {
                    points.Add(start);
                    points.Add(start.Y < referencePoint.Y ?
                        end + new Point(0, width / 2, 0) : 
                        end - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(-1, 0, 0), CutDimDistance, attributes);
                }
                else
                {
                    points.Add(end);
                    points.Add(end.Y < referencePoint.Y ? 
                        start + new Point(0, width / 2, 0) : 
                        start - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(-1, 0, 0), CutDimDistance, attributes);
                }
            }
            else
            {
                if (start.X < end.X)
                {
                    points.Add(end);
                    points.Add(end.Y < referencePoint.Y ? 
                        start + new Point(0, width / 2, 0) : 
                        start - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(1, 0, 0), CutDimDistance, attributes);
                }
                else
                {
                    points.Add(start);
                    points.Add(start.Y < referencePoint.Y ? 
                        end + new Point(0, width / 2, 0) : 
                        end - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(1, 0, 0), CutDimDistance, attributes);
                }
            }
        }

        private void ConfigureSectionView(TSD.View sectionView, TSD.SectionMark sectionMark)
        {
            TSD.FontAttributes font = new TSD.FontAttributes
            {
                Color = TSD.DrawingColors.Magenta,
                Height = Convert.ToInt16(3.96875)
            };

            TSD.TextElement textElement1 = new TSD.TextElement(sectionMark.Attributes.MarkName, font);
            TSD.TextElement textElement2 = new TSD.TextElement("-", font);
            TSD.ContainerElement sectionMarkContent = new TSD.ContainerElement { textElement1, textElement2, textElement1 };

            sectionMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            sectionMark.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
            sectionMark.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(
                TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides,
                TSD.TagLocation.AboveLine,
                new Vector(1, 0, 0),
                TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal,
                sectionMarkContent);

            sectionView.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(
                new Vector(1, 0, 0),
                TSD.TagLocation.AboveLine,
                TSD.TextAlignment.Center,
                sectionMarkContent);
            sectionView.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
            sectionView.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
            sectionView.Modify();
        }

        //public void provide_fitting_cut_dims(TSM.Model MyModel, TSD.View current_view, TSM.Beam main_part, string drg_att)
        //{
        //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
        //    TSS.Face top_face = null;
        //    TSS.Face btm_face = null;
        //    List<TSS.Edge> top_edges = new List<TSS.Edge>();
        //    List<TSS.Edge> btm_edges = new List<TSS.Edge>();

        //    double max_Z = 0;
        //    double min_Z = 0;
        //    double width = 0;
        //    main_part.GetReportProperty("WIDTH", ref width);
        //    TSS.Edge top_chamfer_edge = null;
        //    TSS.Edge btm_chamfer_edge = null;
        //    TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
        //    TSG.Matrix topartmatrix = TSG.MatrixFactory.ToCoordinateSystem(main_part.GetCoordinateSystem());
        //    TSS.EdgeEnumerator edges = (main_part as TSM.Part).GetSolid().GetEdgeEnumerator();

        //    List<TSG.Point> all_points = new List<TSG.Point>();
        //    Dictionary<TSG.Point, TSG.Point> all_points_local = new Dictionary<TSG.Point, TSG.Point>();
        //    while (edges.MoveNext())
        //    {
        //        TSS.Edge edge = edges.Current as TSS.Edge;
        //        if (edge != null)
        //        {
        //            all_points.Add(edge.StartPoint);
        //            all_points.Add(edge.EndPoint);
        //        }
        //    }
        //    all_points = all_points.Distinct().ToList();

        //    foreach (TSG.Point point in all_points)
        //    {
        //        all_points_local.Add(point, topartmatrix.Transform(point));
        //    }


        //    TSG.Point min = all_points_local.Aggregate((l, r) => l.Value.Y < r.Value.Y ? l : r).Value;
        //    TSG.Point max = all_points_local.Aggregate((l, r) => l.Value.Y > r.Value.Y ? l : r).Value;
        //    min_Z = Math.Round(min.Y, 2);
        //    max_Z = Math.Round(max.Y, 2);



        //    //Filter Top Face edges
        //    edges.Reset();
        //    while (edges.MoveNext())
        //    {
        //        TSS.Edge edge = edges.Current as TSS.Edge;


        //        if (Math.Round(topartmatrix.Transform(edge.StartPoint).Y, 2) == Math.Round(topartmatrix.Transform(edge.EndPoint).Y, 2))
        //        {
        //            if (Math.Round(topartmatrix.Transform(edge.StartPoint).Y, 2) == max_Z)
        //            {
        //                top_edges.Add(edge);
        //            }

        //        }
        //    }
        //    top_edges = top_edges.Distinct().ToList();

        //    //Filter Bottom Face edges
        //    edges.Reset();
        //    while (edges.MoveNext())
        //    {
        //        TSS.Edge edge = edges.Current as TSS.Edge;
        //        if (Math.Round(topartmatrix.Transform(edge.StartPoint).Y, 2) == Math.Round(topartmatrix.Transform(edge.EndPoint).Y, 2))
        //        {
        //            if (Math.Round(topartmatrix.Transform(edge.StartPoint).Y, 2) == min_Z)
        //            {
        //                btm_edges.Add(edge);
        //            }

        //        }
        //    }
        //    btm_edges = btm_edges.Distinct().ToList();


        //    if (top_edges.Count == 5)
        //    {
        //        foreach (TSS.Edge edge in top_edges)
        //        {
        //            TSG.Line line = new TSG.Line(edge.StartPoint, edge.EndPoint);
        //            double angle = Math.Round(line.Direction.GetAngleBetween(main_part.GetCoordinateSystem().AxisX) * 180 / Math.PI, 2);
        //            if (angle != 0 && angle != 90 && angle != 180 && angle != 270 && angle != 360)
        //            {
        //                top_chamfer_edge = edge;
        //                break;
        //            }
        //        }

        //        foreach (TSS.Edge edge in btm_edges)
        //        {
        //            TSG.Line line = new TSG.Line(edge.StartPoint, edge.EndPoint);
        //            double angle = Math.Round(line.Direction.GetAngleBetween(main_part.GetCoordinateSystem().AxisX) * 180 / Math.PI, 2);
        //            if (angle != 0 && angle != 90 && angle != 180 && angle != 270 && angle != 360)
        //            {
        //                btm_chamfer_edge = edge;
        //                break;
        //            }
        //        }



        //        //Create dimnsion for top flange Cut
        //        TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
        //        dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
        //        if ((drg_att == "SK_BEAM_A1") || fontSize == FontSizeSelector.OneBy8)
        //        {
        //            if (client.Equals("HILLSDALE") || fontSize == FontSizeSelector.NineBy64)
        //            {
        //                dim_font_height.Text.Font.Height = 3.571875;
        //            }
        //            else
        //            {
        //                dim_font_height.Text.Font.Height = 3.175;
        //            }
        //        }
        //        else
        //        {
        //            dim_font_height.Text.Font.Height = 2.38125;


        //        }
        //        TSD.StraightDimensionSetHandler dimset = new TSD.StraightDimensionSetHandler();
        //        PointList mypt = new PointList();
        //        TSG.Point ref_pt = toviewmatrix.Transform(main_part.GetCoordinateSystem().Origin);

        //        //Creating cut dimwnsions based upon flange cut location


        //        if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).Y > ref_pt.Y && toviewmatrix.Transform(top_chamfer_edge.EndPoint).Y > ref_pt.Y)
        //        {
        //            mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //            mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //            dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(0, 1, 0), 40, dim_font_height);
        //        }
        //        else
        //        {
        //            mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //            mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //            dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(0, -1, 0), 40, dim_font_height);
        //        }

        //        mypt.Clear();


        //        if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).X < (toviewmatrix.Transform(main_part.GetSolid().MinimumPoint).X + toviewmatrix.Transform(main_part.GetSolid().MaximumPoint).X) / 2
        //            && toviewmatrix.Transform(top_chamfer_edge.EndPoint).X < (toviewmatrix.Transform(main_part.GetSolid().MinimumPoint).X + toviewmatrix.Transform(main_part.GetSolid().MaximumPoint).X) / 2)
        //        {
        //            if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).X < toviewmatrix.Transform(top_chamfer_edge.EndPoint).X)
        //            {
        //                if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).Y < ref_pt.Y)
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint + new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                }
        //                else
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint - new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                }

        //            }
        //            else
        //            {
        //                if (toviewmatrix.Transform(top_chamfer_edge.EndPoint).Y < ref_pt.Y)
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint + new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                }
        //                else
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint - new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                }
        //            }

        //        }
        //        else
        //        {
        //            if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).X < toviewmatrix.Transform(top_chamfer_edge.EndPoint).X)
        //            {
        //                if (toviewmatrix.Transform(top_chamfer_edge.EndPoint).Y < ref_pt.Y)
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint + new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                }
        //                else
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint - new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                }

        //            }
        //            else
        //            {
        //                if (toviewmatrix.Transform(top_chamfer_edge.StartPoint).Y < ref_pt.Y)
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint + new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                }
        //                else
        //                {
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.StartPoint));
        //                    mypt.Add(toviewmatrix.Transform(top_chamfer_edge.EndPoint - new TSG.Point(0, (width / 2), 0)));
        //                    dimset.CreateDimensionSet(current_view, mypt, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                }
        //            }

        //        }

        //        //TSD.AngleDimension angleDimension = new AngleDimension(current_view as TSD.ViewBase, toviewmatrix.Transform(top_chamfer_edge.StartPoint),
        //        //                                                        new TSG.Point((toviewmatrix.Transform(top_chamfer_edge.StartPoint).X + toviewmatrix.Transform(top_chamfer_edge.EndPoint).X) / 2,
        //        //                                                        (toviewmatrix.Transform(top_chamfer_edge.StartPoint).Y + toviewmatrix.Transform(top_chamfer_edge.EndPoint).Y) / 2),
        //        //                                                        toviewmatrix.Transform(top_chamfer_edge.EndPoint), 50);

        //        //angleDimension.Insert();
        //        //angleDimension.Attributes.Arrowhead.Head = ArrowheadTypes.NoArrow;
        //        //angleDimension.Attributes.Color = DrawingColors.Green;
        //        //angleDimension.Modify();
        //        //Creating a section view for bottmom flange cut
        //        AssemblyDrawing beam_dwg = current_view.GetDrawing() as AssemblyDrawing;
        //        DrawingObjectEnumerator drawingObjectEnumerator = beam_dwg.GetSheet().GetAllViews();
        //        while (drawingObjectEnumerator.MoveNext())
        //        {
        //            TSD.View view = drawingObjectEnumerator.Current as TSD.View;
        //            if (view != null && view.ViewType == TSD.View.ViewTypes.FrontView)
        //            {
        //                TSD.View section_view = null;
        //                TSD.SectionMark sec = null;
        //                TSG.Matrix toviewmatrix1 = TSG.MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
        //                TSG.Point TP1 = toviewmatrix1.Transform(btm_chamfer_edge.StartPoint + new TSG.Point(0, width / 2, 0));
        //                TSG.Point TP2 = toviewmatrix1.Transform(btm_chamfer_edge.EndPoint + new TSG.Point(0, width / 2, 0));
        //                TSG.Point P1 = new TSG.Point();
        //                TSG.Point P2 = new TSG.Point();
        //                if (TP1.X > TP2.X)
        //                {
        //                    P1 = TP1 + new TSG.Point(150, 40, 0);
        //                    P2 = TP2 - new TSG.Point(150, -40, 0);
        //                }
        //                else
        //                {
        //                    P1 = TP2 + new TSG.Point(150, 0, 0);
        //                    P2 = TP1 - new TSG.Point(150, 0, 0);
        //                }



        //                bool result = TSD.View.CreateSectionView(view, P1, P2, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), 280, 280, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out section_view, out sec);
        //                section_view.Attributes.LoadAttributes("SK_BEAM_A1");
        //                section_view.Modify();
        //                TSG.Matrix toviewmatrix2 = TSG.MatrixFactory.ToCoordinateSystem(section_view.DisplayCoordinateSystem);

        //                TSD.FontAttributes FONT = new TSD.FontAttributes();
        //                FONT.Color = TSD.DrawingColors.Magenta;
        //                FONT.Height = Convert.ToInt16(3.96875);

        //                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
        //                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
        //                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



        //                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
        //                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

        //                section_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);
        //                section_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
        //                section_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
        //                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
        //                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
        //                section_view.Modify();
        //                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), /*typeof(TSD.AngleDimension),*/ typeof(TSD.Mark) };
        //                TSD.DrawingObjectEnumerator dim_drg = section_view.GetAllObjects(type_for_dim);
        //                while (dim_drg.MoveNext())
        //                {
        //                    var obj = dim_drg.Current;
        //                    obj.Delete();

        //                }


        //                //Creating cut dimwnsions based upon flange cut location at bottom flange

        //                TSG.Point ref_pt1 = toviewmatrix2.Transform(main_part.GetCoordinateSystem().Origin);
        //                PointList mypt1 = new PointList();


        //                if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).Y > ref_pt1.Y && toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).Y > ref_pt1.Y)
        //                {
        //                    mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                    mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                    dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(0, 1, 0), 40, dim_font_height);
        //                }
        //                else
        //                {
        //                    mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                    mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                    dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(0, -1, 0), 40, dim_font_height);
        //                }

        //                mypt1.Clear();

        //                if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).X < (toviewmatrix2.Transform(main_part.GetSolid().MinimumPoint).X + toviewmatrix2.Transform(main_part.GetSolid().MaximumPoint).X) / 2
        //                         && toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).X < (toviewmatrix2.Transform(main_part.GetSolid().MinimumPoint).X + toviewmatrix2.Transform(main_part.GetSolid().MaximumPoint).X) / 2)
        //                {
        //                    if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).X < toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).X)
        //                    {
        //                        if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).Y < ref_pt1.Y)
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint + new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                        }
        //                        else
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint - new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                        }

        //                    }
        //                    else
        //                    {
        //                        if (toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).Y < ref_pt1.Y)
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint + new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                        }
        //                        else
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint - new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(-1, 0, 0), 40, dim_font_height);
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).X < toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).X)
        //                    {
        //                        if (toviewmatrix2.Transform(btm_chamfer_edge.EndPoint).Y < ref_pt1.Y)
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint + new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                        }
        //                        else
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint - new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                        }

        //                    }
        //                    else
        //                    {
        //                        if (toviewmatrix2.Transform(btm_chamfer_edge.StartPoint).Y < ref_pt1.Y)
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint + new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                        }
        //                        else
        //                        {
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.StartPoint));
        //                            mypt1.Add(toviewmatrix2.Transform(btm_chamfer_edge.EndPoint - new TSG.Point(0, (width / 2), 0)));
        //                            dimset.CreateDimensionSet(section_view, mypt1, new TSG.Vector(1, 0, 0), 40, dim_font_height);
        //                        }
        //                    }

        //                }


        //                //ON centreline
        //                section_view.GetDrawing().CommitChanges();
        //                Type type_for_part = typeof(TSD.Part);
        //                TSD.DrawingObjectEnumerator drawingObjectEnumerator1 = section_view.GetAllObjects(type_for_part);
        //                while (drawingObjectEnumerator1.MoveNext())
        //                {
        //                    TSD.Part part = drawingObjectEnumerator1.Current as TSD.Part;
        //                    if (part != null)
        //                    {
        //                        if ((MyModel.SelectModelObject(part.ModelIdentifier) as TSM.Part).Identifier.GUID.ToString() == main_part.Identifier.GUID.ToString())
        //                        {
        //                            part.Attributes.DrawCenterLine = true;
        //                            part.Attributes.DrawOrientationMark = false;
        //                            part.Attributes.SymbolOffset = 0;
        //                            part.Modify();
        //                            section_view.Modify();
        //                            section_view.GetDrawing().CommitChanges();
        //                        }

        //                    }

        //                }

        //            }
        //        }

        //    }

        //}


        //private ArrayList Getcatalog_values(TSM.Part main_part)
        //{
        //    ArrayList values = new ArrayList();
        //    double size1_m = 0, size3_m = 0, size2_m = 0;
        //    LibraryProfileItem mainpro = new LibraryProfileItem { ProfileName = main_part.Profile.ProfileString };
        //    mainpro.Select();
        //    ArrayList parameters_for_main = mainpro.aProfileItemParameters;
        //    ProfileItemParameter bm = parameters_for_main[0] as ProfileItemParameter;
        //    ProfileItemParameter cm = parameters_for_main[2] as ProfileItemParameter;
        //    ProfileItemParameter dm = parameters_for_main[1] as ProfileItemParameter;
        //    size1_m = bm.Value;
        //    size3_m = cm.Value;
        //    size2_m = dm.Value;
        //    values.Add(size1_m);
        //    values.Add(size2_m);
        //    values.Add(size3_m);
        //    return values;
        //}

        //public void Create_cope_dimensions(TSD.View current_view, TSM.Beam main_part, string drg_att)
        //{

        //    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
        //    fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
        //    fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
        //    if ((drg_att == "SK_BEAM_A1") || fontSize == FontSizeSelector.OneBy8)
        //    {
        //        if (_userInput.Client.Equals("HILLSDALE") || fontSize == FontSizeSelector.NineBy64)
        //        {
        //            fixed_attributes.Text.Font.Height = 3.571875;
        //        }
        //        else
        //        {
        //            fixed_attributes.Text.Font.Height = 3.175;
        //        }
        //    }
        //    else
        //    {
        //        fixed_attributes.Text.Font.Height = 2.38125;


        //    }
        //    TSD.StraightDimensionSetHandler cope_locking_dimesion = new TSD.StraightDimensionSetHandler();

        //    //TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
        //    //dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
        //    //if (drg_att == "SK_BEAM_A1")
        //    //{
        //    //    dim_font_height.Text.Font.Height = 3.175;
        //    //}
        //    //else
        //    //{
        //    //    dim_font_height.Text.Font.Height = 2.38125;
        //    //    //dim_font_height.Text.Font.Height = 3.175;


        //    //}

        //    ArrayList values = Getcatalog_values(main_part);
        //    double size1_m = Convert.ToDouble(values[0]);
        //    double size2_m = Convert.ToDouble(values[1]);
        //    TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
        //    Type type_for_contourplate = typeof(TSM.ContourPlate);
        //    double size = 0;
        //    double output = 0;
        //    main_part.GetReportProperty("LENGTH", ref output);
        //    if ((current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
        //    {
        //        size = size1_m / 2;
        //    }
        //    else
        //    {
        //        size = size2_m / 2;
        //    }

        //    //////copedimesnison/////
        //    Type bolpart = typeof(TSM.BooleanPart);
        //    Type fit = typeof(TSM.Fitting);
        //    TSM.ModelObjectEnumerator test_bool = main_part.GetBooleans();
        //    ArrayList cuts = new ArrayList();
        //    TSG.Point fittrans_origin = new TSG.Point();
        //    double workpoint = output;
        //    TSG.Point point1 = new TSG.Point();
        //    TSG.Point point2 = new TSG.Point();
        //    TSG.Point point3 = new TSG.Point();
        //    TSG.Point point4 = new TSG.Point();
        //    while (test_bool.MoveNext())
        //    {
        //        int d = 0;
        //        ArrayList pts_in_viewco = new ArrayList();
        //        var partcut = test_bool.Current;
        //        if (partcut.GetType().Equals(bolpart))
        //        {
        //            TSM.BooleanPart fitobj = partcut as TSM.BooleanPart;

        //            if (fitobj.OperativePart.GetType().Equals(type_for_contourplate))
        //            {
        //                TSM.ContourPlate platecut = fitobj.OperativePart as TSM.ContourPlate;
        //                ArrayList pts = platecut.Contour.ContourPoints;
        //                ArrayList x_list = new ArrayList();
        //                ArrayList y_list = new ArrayList();
        //                ArrayList z_list = new ArrayList();
        //                foreach (TSG.Point bolpart_point in pts)
        //                {
        //                    TSG.Point tran_point = toviewmatrix.Transform(bolpart_point);
        //                    pts_in_viewco.Add(tran_point);

        //                }
        //                bool RESULT = false;
        //                bool RESULT1 = false;




        //                try
        //                {
        //                    long P1 = Convert.ToInt64((pts_in_viewco[0] as TSG.Point).X);
        //                    long P2 = Convert.ToInt64((pts_in_viewco[1] as TSG.Point).X);
        //                    long P3 = Convert.ToInt64((pts_in_viewco[2] as TSG.Point).X);
        //                    long P4 = Convert.ToInt64((pts_in_viewco[3] as TSG.Point).X);

        //                    RESULT = P1.Equals(P4);
        //                    RESULT1 = P2.Equals(P3);
        //                }
        //                catch
        //                {
        //                }
        //                if (((RESULT == true) && (RESULT1 == true) || (RESULT == false) && (RESULT1 == false)))
        //                {
        //                    if (Convert.ToInt16((pts_in_viewco[0] as TSG.Point).Y) != Convert.ToInt16((pts_in_viewco[2] as TSG.Point).Y))
        //                    {
        //                        foreach (TSG.Point pt in pts_in_viewco)
        //                        {


        //                            x_list.Add(pt.X);
        //                            y_list.Add(pt.Y);
        //                            z_list.Add(pt.Z);

        //                        }

        //                        x_list.Sort();
        //                        y_list.Sort();
        //                        ////////////////////Lower boolean dim///////////////////
        //                        TSG.Point pt1 = new TSG.Point();
        //                        TSG.Point pt2 = new TSG.Point();
        //                        TSG.Vector dim_vect_for_x_dim = new TSG.Vector();
        //                        TSG.Vector dim_vect_for_y_dim = new TSG.Vector();

        //                        //////////Third quadrant/////////////////////////
        //                        if ((Convert.ToDouble(y_list[0]) <= 0) && ((Convert.ToDouble(x_list[0]) <= 0)))
        //                        {

        //                            pt2 = new TSG.Point(0, Convert.ToDouble(y_list[y_list.Count - 1]), 0);
        //                            pt1 = new TSG.Point(Convert.ToDouble(x_list[x_list.Count - 1]), -size, 0);
        //                            dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
        //                            dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);
        //                            TSD.PointList MYPTLIST = new TSD.PointList();
        //                            MYPTLIST.Add(pt1);
        //                            MYPTLIST.Add(pt2);
        //                            TSD.PointList MYPTLIST1 = new TSD.PointList();
        //                            MYPTLIST1.Add(pt2);
        //                            MYPTLIST1.Add(pt1);


        //                            try
        //                            {
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST, dim_vect_for_x_dim, 70, fixed_attributes);

        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST1, dim_vect_for_y_dim, 85, fixed_attributes);
        //                            }
        //                            catch
        //                            {
        //                            }

        //                            //TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt2, dim_vect_for_x_dim, 70, dim_font_height);
        //                            //bool_dim_x.Insert();



        //                            //TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt2, dim_vect_for_y_dim, 85, dim_font_height);
        //                            //bool_dim_y.Insert();


        //                        }
        //                        //////////Second quadrant//////////////////////////
        //                        else if ((Convert.ToDouble(x_list[0]) <= 0) && ((Convert.ToDouble(y_list[0]) >= 0)))
        //                        {
        //                            pt2 = new TSG.Point(0, Convert.ToDouble(y_list[0]), 0);
        //                            pt1 = new TSG.Point(Convert.ToDouble(x_list[x_list.Count - 1]), size, 0);
        //                            point2 = pt1;
        //                            dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
        //                            dim_vect_for_y_dim = new TSG.Vector(-1, 0, 0);
        //                            //TSD.PointList final_pts = new TSD.PointList();
        //                            //final_pts.Add(pt1);
        //                            //final_pts.Add(pt2);
        //                            TSD.PointList MYPTLIST = new TSD.PointList();
        //                            MYPTLIST.Add(pt1);
        //                            MYPTLIST.Add(pt2);
        //                            TSD.PointList MYPTLIST1 = new TSD.PointList();
        //                            MYPTLIST1.Add(pt2);
        //                            MYPTLIST1.Add(pt1);

        //                            try
        //                            {

        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST, dim_vect_for_x_dim, 70, fixed_attributes);
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST1, dim_vect_for_y_dim, 85, fixed_attributes);
        //                            }
        //                            catch
        //                            {
        //                            }




        //                            //TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt2, dim_vect_for_x_dim, 70, dim_font_height);
        //                            //bool_dim_x.Insert();



        //                            //TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt2, dim_vect_for_y_dim, 85, dim_font_height);
        //                            //bool_dim_y.Insert();


        //                        }
        //                        /////////Fourth Quadrant////////////////////////////
        //                        else if ((Convert.ToDouble(x_list[0]) >= 0) && ((Convert.ToDouble(y_list[0]) <= 0)))
        //                        {

        //                            pt1 = new TSG.Point(Convert.ToDouble(x_list[0]), -size, 0);
        //                            pt2 = new TSG.Point(workpoint, Convert.ToDouble(y_list[y_list.Count - 1]), 0);
        //                            dim_vect_for_x_dim = new TSG.Vector(0, -1, 0);
        //                            dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);
        //                            TSD.PointList MYPTLIST = new TSD.PointList();
        //                            MYPTLIST.Add(pt1);
        //                            MYPTLIST.Add(pt2);
        //                            TSD.PointList MYPTLIST1 = new TSD.PointList();
        //                            MYPTLIST1.Add(pt2);
        //                            MYPTLIST1.Add(pt1);
        //                            try
        //                            {
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST, dim_vect_for_x_dim, 70, fixed_attributes);
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST1, dim_vect_for_y_dim, 85, fixed_attributes);
        //                            }
        //                            catch
        //                            {
        //                            }



        //                            //TSD.StraightDimension bool_dim_x = new TSD.StraightDimension(current_view as TSD.ViewBase, pt1, pt2, dim_vect_for_x_dim, 70, dim_font_height);

        //                            //bool_dim_x.Insert();


        //                            //TSD.StraightDimension bool_dim_y = new TSD.StraightDimension(current_view as TSD.ViewBase, pt2, pt1, dim_vect_for_y_dim, 85, dim_font_height);
        //                            //bool_dim_y.Insert();



        //                        }
        //                        //////////////First Quadrant////////////////
        //                        else if ((Convert.ToDouble(x_list[0]) >= 0) && ((Convert.ToDouble(y_list[0]) >= 0)))
        //                        {

        //                            pt1 = new TSG.Point(Convert.ToDouble(x_list[0]), size, 0);
        //                            pt2 = new TSG.Point(workpoint, Convert.ToDouble(y_list[0]), 0);
        //                            point3 = pt1;
        //                            dim_vect_for_x_dim = new TSG.Vector(0, 1, 0);
        //                            dim_vect_for_y_dim = new TSG.Vector(1, 0, 0);
        //                            TSD.PointList MYPTLIST = new TSD.PointList();
        //                            MYPTLIST.Add(pt1);
        //                            MYPTLIST.Add(pt2);
        //                            TSD.PointList MYPTLIST1 = new TSD.PointList();
        //                            MYPTLIST1.Add(pt2);
        //                            MYPTLIST1.Add(pt1);
        //                            try
        //                            {
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST, dim_vect_for_x_dim, 70, fixed_attributes);
        //                                cope_locking_dimesion.CreateDimensionSet(current_view, MYPTLIST1, dim_vect_for_y_dim, 85, fixed_attributes);
        //                            }
        //                            catch
        //                            {
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            cuts.Add(partcut);
        //            TSG.Point fittrans_origin1 = toviewmatrix.Transform(main_part.EndPoint);
        //        }
        //        else
        //        {
        //            point2 = new TSG.Point(0, size1_m / 2, 0);
        //            point3 = new TSG.Point(output, size1_m / 2, 0);
        //        }
        //        d = d + 100;
        //    }
        //}
    }
}