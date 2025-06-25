using System;
using System.Collections.Generic;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Drawing.Dimensions;
using System.Linq;

namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    /// <summary>
    /// Builds the top view drawing with dimensions
    /// </summary>
    public class SKTopViewBuilder
    {
        #region Fields and Constants
        private readonly CustomInputModel _inputModel;
        private readonly SKFlangeOutDimension _flangeOutDimension;
        private readonly string _client;
        private readonly SKSortingHandler _sortingHandler;
        private readonly DuplicateRemover _duplicateRemover;
        private readonly BoltMatrixHandler _boltMatrixHandler;

        private const string ATTRIB_A1 = "SK_BEAM_A1";
        private const string ATTRIB_STANDARD = "standard";
        private const string SymbolFile = "sections";
        private const int SymbolIndex = 48;
        private const double SymbolHeight = 25.4;
        private const TSD.DrawingColors SymbolColor = TSD.DrawingColors.Green;
        private const TSD.DrawingColors MarkColor = TSD.DrawingColors.Magenta;
        private const TSD.DrawingColors LineColor = TSD.DrawingColors.Blue;
        private const double FontHeight = 3.96875;
        #endregion

        #region Constructor
        public SKTopViewBuilder(
            SKSortingHandler sortingHandler,
            SKFlangeOutDimension flangeOutDimension,
            DuplicateRemover duplicateRemover,
            BoltMatrixHandler boltMatrixHandler,
            CustomInputModel inputModel)
        {
            _sortingHandler = sortingHandler ?? throw new ArgumentNullException(nameof(sortingHandler));
            _flangeOutDimension = flangeOutDimension ?? throw new ArgumentNullException(nameof(flangeOutDimension));
            _duplicateRemover = duplicateRemover ?? throw new ArgumentNullException(nameof(duplicateRemover));
            _boltMatrixHandler = boltMatrixHandler ?? throw new ArgumentNullException(nameof(boltMatrixHandler));
            _inputModel = inputModel ?? throw new ArgumentNullException(nameof(inputModel));
            _client = inputModel.Client;
        }
        #endregion

        #region Private Methods
        private void CreateTopViewSection(
            TSD.View viewForBottomView,
            TSG.Point startPoint,
            TSG.Point endPoint,
            TSG.Point labelPoint,
            double height,
            double depth,
            string drgAttribute,
            out TSD.View bottomView,
            out TSD.SectionMark sectionMark)
        {
            bottomView = null;
            sectionMark = null;

            string sectionMarkAttr = (drgAttribute == ATTRIB_A1) ? ATTRIB_A1 : ATTRIB_STANDARD;
            bool result = TSD.View.CreateSectionView(
                viewForBottomView,
                startPoint,
                endPoint,
                labelPoint,
                height,
                depth,
                viewForBottomView.Attributes,
                new TSD.SectionMarkBase.SectionMarkAttributes(sectionMarkAttr),
                out bottomView,
                out sectionMark);

            if (!result) return;

            bottomView.Attributes.LoadAttributes(sectionMarkAttr);
            bottomView.Modify();

            AdjustRestrictionBox(bottomView);
            ConfigureSectionMark(bottomView, sectionMark);
            AddSymbol(bottomView);
            DeleteExistingDimensions(bottomView);
            _flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottomView, MAINPART, drgAttribute);
        }

        private void AdjustRestrictionBox(TSD.View view)
        {
            double changeMin = Math.Abs(view.RestrictionBox.MinPoint.Y);
            double changeMax = Math.Abs(view.RestrictionBox.MaxPoint.Y);
            if (changeMin > changeMax)
            {
                view.RestrictionBox.MaxPoint.Y = changeMin;
            }
            else
            {
                view.RestrictionBox.MinPoint.Y = -changeMax;
            }
            view.Modify();
        }

        private void ConfigureSectionMark(TSD.View view, TSD.SectionMark sectionMark)
        {
            TSD.FontAttributes font = new TSD.FontAttributes { Color = MarkColor, Height = FontHeight };
            TSD.TextElement textElement = new TSD.TextElement(sectionMark.Attributes.MarkName, font);
            TSD.TextElement dashElement = new TSD.TextElement("-", font);
            TSD.ContainerElement sectionMarkContainer = new TSD.ContainerElement { textElement, dashElement, textElement };

            sectionMark.Attributes.LineColor = LineColor;
            sectionMark.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(
                TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides,
                TSD.TagLocation.AboveLine,
                new TSG.Vector(1, 0, 0),
                TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal,
                new TSD.ContainerElement { textElement });

            view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(
                new TSG.Vector(1, 0, 0),
                TSD.TagLocation.AboveLine,
                TSD.TextAlignment.Center,
                sectionMarkContainer);
            view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
            view.Attributes.MarkSymbolColor = MarkColor;
            sectionMark.Attributes.SymbolColor = MarkColor;
            sectionMark.Attributes.LineColor = MarkColor;
            view.Modify();
        }

        private void AddSymbol(TSD.View view)
        {
            TSD.SymbolInfo symbolInfo = new TSD.SymbolInfo(SymbolFile, SymbolIndex);
            TSG.Point insertionPoint = new TSG.Point(view.RestrictionBox.MaxPoint.X, 0, 0);
            TSD.Symbol symbol = new TSD.Symbol(view, insertionPoint, symbolInfo);
            symbol.Insert();
            symbol.Attributes.Height = SymbolHeight;
            symbol.Attributes.Color = SymbolColor;
            symbol.Modify();
        }

        private void DeleteExistingDimensions(TSD.View view)
        {
            Type[] dimTypes = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
            TSD.DrawingObjectEnumerator dimEnumerator = view.GetAllObjects(dimTypes);
            while (dimEnumerator.MoveNext())
            {
                dimEnumerator.Current.Delete();
            }
        }

        private void TOP_view_creation(
            TSM.Beam mainPart,
            TSD.View viewForBottomView,
            double output,
            double heightOfMainPart,
            out TSD.View bottomView,
            out TSD.View bottomView1,
            TSG.Point p1,
            TSG.Point p2,
            string bottom,
            out List<TSD.View> topViewFlangeCutList,
            string drgAttribute)
        {
            bottomView = null;
            bottomView1 = null;
            topViewFlangeCutList = new List<TSD.View>();
            double height = heightOfMainPart / 2 + 50;
            double depth = 30;

            switch (bottom)
            {
                case "LEFT":
                    CreateTopViewSection(
                        viewForBottomView,
                        new TSG.Point(p1.X + 300, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(-500, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(viewForBottomView.ExtremaCenter.X, 200, 0),
                        height,
                        depth,
                        drgAttribute,
                        out bottomView,
                        out _);
                    if (bottomView != null) topViewFlangeCutList.Add(bottomView);
                    break;

                case "RIGHT":
                    CreateTopViewSection(
                        viewForBottomView,
                        new TSG.Point(output + 500, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(p2.X - 300, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(viewForBottomView.ExtremaCenter.X, 200, 0),
                        height,
                        depth,
                        drgAttribute,
                        out bottomView,
                        out _);
                    if (bottomView != null) topViewFlangeCutList.Add(bottomView);
                    break;

                case "BOTH":
                    CreateTopViewSection(
                        viewForBottomView,
                        new TSG.Point(p1.X + 300, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(-500, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(p1.X + 300, 200, 0),
                        height,
                        depth,
                        drgAttribute,
                        out bottomView,
                        out _);
                    if (bottomView != null) topViewFlangeCutList.Add(bottomView);

                    CreateTopViewSection(
                        viewForBottomView,
                        new TSG.Point(output + 500, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(p2.X - 300, 30 + (heightOfMainPart / 2), 0),
                        new TSG.Point(output + 500, 200, 0),
                        height,
                        depth,
                        drgAttribute,
                        out bottomView1,
                        out _);
                    if (bottomView1 != null) topViewFlangeCutList.Add(bottomView1);
                    break;
            }
        }
        #endregion

        #region Public Methods
        public List<TSD.View> CreateTopView(
            Type typeForBolt,
            double output,
            string topViewToCreate,
            string topViewNeeded,
            string drgAtt,
            List<double> mainPartProfileValues,
            TSM.Beam main,
            TSG.Point p3Bottom,
            TSG.Point p4Bottom,
            TSD.StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes,
            TSD.View currentView)
        {
            List<TSD.View> topViewFlangeCutList = new List<TSD.View>();
            double heightOfMainPart = mainPartProfileValues[0];

            if (topViewNeeded != "yes")
            {
                TOP_view_creation(
                    main,
                    currentView,
                    output,
                    heightOfMainPart,
                    out TSD.View bottomView,
                    out TSD.View bottomView1,
                    p3Bottom,
                    p4Bottom,
                    topViewToCreate,
                    out topViewFlangeCutList,
                    drgAtt);

                if (bottomView != null) _flangeOutDimension.Create_FLANGE_CUT_dimensions_top(bottomView, main, drgAtt);
                if (bottomView1 != null) _flangeOutDimension.Create_FLANGE_CUT_dimensions_top(bottomView1, main, drgAtt);
            }

            foreach (var view in topViewFlangeCutList)
            {
                CreateBoltDimensions(view, main, heightOfMainPart, typeForBolt);
                Create3x3Dimensions(view, main, heightOfMainPart, typeForBolt, fixedAttributes);
            }

            return topViewFlangeCutList;
        }

        private void CreateBoltDimensions(TSD.View view, TSM.Beam main, double heightOfMainPart, Type typeForBolt)
        {
            TSG.Matrix bottomMat = TSG.MatrixFactory.ToCoordinateSystem(view.ViewCoordinateSystem);
            TSD.PointList rdPointList = new TSD.PointList();

            TSD.DrawingObjectEnumerator boltEnumerator = view.GetAllObjects(typeForBolt);
            while (boltEnumerator.MoveNext())
            {
                if (boltEnumerator.Current is TSD.Bolt drgBolt)
                {
                    TSM.BoltArray bolt = new TSM.Model().SelectModelObject(drgBolt.ModelIdentifier) as TSM.BoltArray;
                    if (bolt != null && bottomMat.Transform(bolt.BoltPositions[0] as TSG.Point).Z < bottomMat.Transform(main.EndPoint).Z - heightOfMainPart / 2)
                    {
                        TSG.Point[,] boltMatrix = _boltMatrixHandler.GetBoltMatrix(drgBolt, view);
                        if (boltMatrix != null)
                        {
                            int cols = boltMatrix.GetLength(1);
                            for (int i = 0; i < cols; i++)
                            {
                                rdPointList.Add(boltMatrix[boltMatrix.GetLength(0) - 1, i]);
                            }
                        }
                    }
                }
            }

            TSD.PointList finalRdList = _duplicateRemover.RemoveDuplicateXValues(rdPointList);
            finalRdList.Add(new TSG.Point(0, 0, 0));
            _sortingHandler.SortPoints(finalRdList);

            TSD.StraightDimensionSetHandler boltRdDim = new TSD.StraightDimensionSetHandler();
            TSD.StraightDimensionSet.StraightDimensionSetAttributes rdAtt = new TSD.StraightDimensionSet.StraightDimensionSetAttributes
            {
                Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2,
                ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No,
                Color = TSD.DrawingColors.Gray70,
                Text = { Font = { Color = TSD.DrawingColors.Gray70, Height = SkTeklaDrawingUtility.GetFontHeight(_client, _inputModel.FontSize, drgAtt) } },
                Arrowhead = { Head = TSD.ArrowheadTypes.FilledArrow }
            };

            double distance = heightOfMainPart / 2;
            TSG.Point p1 = finalRdList[finalRdList.Count - 1] as TSG.Point;
            double distanceValue = TSG.Distance.PointToPoint(p1, new TSG.Point(p1.X, distance, 0));
            boltRdDim.CreateDimensionSet(view, finalRdList, new TSG.Vector(0, 1, 0), distanceValue, rdAtt);
        }

        private void Create3x3Dimensions(TSD.View view, TSM.Beam main, double heightOfMainPart, Type typeForBolt, TSD.StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes)
        {
            TSG.Matrix bottomMat = TSG.MatrixFactory.ToCoordinateSystem(view.ViewCoordinateSystem);
            TSD.DrawingObjectEnumerator boltEnumerator = view.GetAllObjects(typeForBolt);

            while (boltEnumerator.MoveNext())
            {
                if (boltEnumerator.Current is TSD.Bolt drgBolt)
                {
                    TSM.BoltArray bolt = new TSM.Model().SelectModelObject(drgBolt.ModelIdentifier) as TSM.BoltArray;
                    if (bolt != null && bottomMat.Transform(bolt.BoltPositions[0] as TSG.Point).Z < bottomMat.Transform(main.EndPoint).Z - heightOfMainPart / 2)
                    {
                        TSG.Point[,] boltMatrix = _boltMatrixHandler.GetBoltMatrix(drgBolt, view);
                        if (boltMatrix != null)
                        {
                            TSD.PointList list3x3 = new TSD.PointList();
                            int rows = boltMatrix.GetLength(0);
                            int cols = boltMatrix.GetLength(1);
                            for (int i = 0; i < rows; i++)
                            {
                                list3x3.Add(boltMatrix[i, cols - 1]);
                            }

                            TSD.PointList finalList3x3 = _duplicateRemover.RemoveDuplicateYValues(list3x3);
                            finalList3x3.Add(new TSG.Point(list3x3[0].X, 0, 0));

                            TSD.StraightDimensionSetHandler dim3x3 = new TSD.StraightDimensionSetHandler();
                            dim3x3.CreateDimensionSet(view, finalList3x3, new TSG.Vector(1, 0, 0), 200, fixedAttributes);
                        }
                    }
                }
            }
        }

        public void TOPVIEW_CHECK(
            TSM.Beam mainPart,
            out string topViewCreation,
            double heightOfMainPart,
            out TSG.Point pt1ForBottomView,
            out TSG.Point pt2ForBottomView,
            double output,
            out string topViewCreateCheck)
        {
            topViewCreation = "";
            topViewCreateCheck = "";
            pt1ForBottomView = null;
            pt2ForBottomView = null;

            TSD.PointList posi = new TSD.PointList();
            TSD.PointList negi = new TSD.PointList();
            TSG.Matrix toView = TSG.MatrixFactory.ToCoordinateSystem(mainPart.GetCoordinateSystem());

            foreach (TSM.ModelObject partCut in mainPart.GetBooleans())
            {
                if (partCut is TSM.BooleanPart booleanPart && booleanPart.OperativePart is TSM.ContourPlate contourPlate)
                {
                    List<TSG.Point> pts = new List<TSG.Point>(contourPlate.Contour.ContourPoints.Cast<TSG.Point>());
                    if (pts.Count > 3 && Math.Abs(toView.Transform(pts[0]).Y - toView.Transform(pts[2]).Y) < 1e-6)
                    {
                        foreach (TSG.Point point in pts)
                        {
                            TSG.Point transformedPoint = toView.Transform(point);
                            if (transformedPoint.Y >= 0)
                            {
                                if (transformedPoint.X < 1000 && transformedPoint.X > 0) posi.Add(transformedPoint);
                                else if (transformedPoint.X > 1000 && transformedPoint.X < output) negi.Add(transformedPoint);
                            }
                        }
                    }
                }
            }

            _sortingHandler.SortPoints(posi);
            _sortingHandler.SortPoints(negi);

            if (posi.Count > 1 && negi.Count > 1)
            {
                topViewCreateCheck = "BOTH";
                pt1ForBottomView = posi[0] as TSG.Point;
                pt2ForBottomView = negi[0] as TSG.Point;
            }
            else if (posi.Count > 1)
            {
                topViewCreateCheck = "LEFT";
                pt1ForBottomView = posi[0] as TSG.Point;
            }
            else if (negi.Count > 1)
            {
                topViewCreateCheck = "RIGHT";
                pt2ForBottomView = negi[0] as TSG.Point;
            }
        }

        public string TOPVIEW_needed(TSM.Beam mainPart, double heightOfMainPart, double output)
        {
            TSG.Matrix toView = TSG.MatrixFactory.ToCoordinateSystem(mainPart.GetCoordinateSystem());

            foreach (TSM.ModelObject partCut in mainPart.GetBooleans())
            {
                if (partCut is TSM.CutPlane cutPlane)
                {
                    TSG.Vector zVector = cutPlane.Plane.AxisY.Cross(cutPlane.Plane.AxisX).GetNormal();
                    if (zVector.X != 0 && zVector.Y != 0) return "yes";
                }
                else if (partCut is TSM.BooleanPart booleanPart && booleanPart.OperativePart is TSM.ContourPlate contourPlate)
                {
                    List<TSG.Point> pts = new List<TSG.Point>(contourPlate.Contour.ContourPoints.Cast<TSG.Point>());
                    if (pts.Count > 2 && Math.Abs(toView.Transform(pts[0]).Y - toView.Transform(pts[2]).Y) < 1e-6)
                    {
                        foreach (TSG.Point point in pts)
                        {
                            if (toView.Transform(point).Y > -10) return "yes";
                        }
                    }
                }
            }
            return "";
        }

        public void top_view_check_for_dim(TSD.AssemblyDrawing beamAssemblyDrg, string topViewNeeded)
        {
            TSD.DrawingObjectEnumerator views = beamAssemblyDrg.GetSheet().GetAllViews();
            while (views.MoveNext())
            {
                if (views.Current is TSD.View view && view.ViewType == TSD.View.ViewTypes.TopView)
                {
                    int dimCount = view.GetAllObjects(typeof(TSD.DimensionBase)).GetSize();
                    if (dimCount > 0 || topViewNeeded == "yes") continue;

                    if (_client == "SME")
                    {
                        TSD.DrawingObjectEnumerator marks = view.GetAllObjects(typeof(TSD.Mark));
                        while (marks.MoveNext()) marks.Current.Delete();
                    }
                    else
                    {
                        view.Delete();
                    }
                }
            }
        }
        #endregion
    }
}