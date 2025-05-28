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

namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class CopeDimensions
    {
        private readonly string client;
        private readonly FontSizeSelector fontSize;
        private readonly SKCatalogHandler catalogHandler;

        private const double XDimDistance = 70;
        private const double YDimDistance = 85;
        private const double CutDimDistance = 40;
        private const double Tolerance = 1e-6;

        public CopeDimensions(SKCatalogHandler catalogHandler, string client, FontSizeSelector fontSize)
        {
            this.catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.fontSize = fontSize;
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
                    if (!allPointsLocal.ContainsKey(start)) allPointsLocal[start] = toPartMatrix.Transform(start);
                    if (!allPointsLocal.ContainsKey(end)) allPointsLocal[end] = toPartMatrix.Transform(end);
                }
            }

            double maxZ = allPointsLocal.Values.Max(p => p.Y);
            double minZ = allPointsLocal.Values.Min(p => p.Y);

            edges.Reset();
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
                        Matrix toViewMatrix1 = MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
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
                        bool result = TSD.View.CreateSectionView(view, p1, p2, new Point(currentView.ExtremaCenter.X, 0, 0), 280, 280,
                            new TSD.View.ViewAttributes("SK_BEAM_A1"), 
                            new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"),
                            out sectionView, out sectionMark);

                        if (result)
                        {
                            sectionView.Attributes.LoadAttributes("SK_BEAM_A1");
                            sectionView.Modify();

                            Matrix toViewMatrix2 = MatrixFactory.ToCoordinateSystem(sectionView.DisplayCoordinateSystem);
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                                new TSD.StraightDimensionSet.StraightDimensionSetAttributes
                            {
                                Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                                Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
                            };

                            Point referencePoint1 = toViewMatrix2.Transform(mainPart.GetCoordinateSystem().Origin);
                            CreateCutDimensions(sectionView, toViewMatrix2, referencePoint1, bottomChamferEdge, width, mainPart, attributes);

                            ConfigureSectionView(sectionView, sectionMark);
                            sectionView.GetDrawing().CommitChanges();
                        }
                    }
                }
            }
        }

        private bool IsAxisAligned(TSS.Edge edge, TSM.Beam mainPart)
        {
            Line line = new Line(edge.StartPoint, edge.EndPoint);
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
                    points.Add(start.Y < referencePoint.Y ? end + new Point(0, width / 2, 0) : end - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(-1, 0, 0), CutDimDistance, attributes);
                }
                else
                {
                    points.Add(end);
                    points.Add(end.Y < referencePoint.Y ? start + new Point(0, width / 2, 0) : start - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(-1, 0, 0), CutDimDistance, attributes);
                }
            }
            else
            {
                if (start.X < end.X)
                {
                    points.Add(end);
                    points.Add(end.Y < referencePoint.Y ? start + new Point(0, width / 2, 0) : start - new Point(0, width / 2, 0));
                    dimensionSet.CreateDimensionSet(view, points, new Vector(1, 0, 0), CutDimDistance, attributes);
                }
                else
                {
                    points.Add(start);
                    points.Add(start.Y < referencePoint.Y ? end + new Point(0, width / 2, 0) : end - new Point(0, width / 2, 0));
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
    }
}