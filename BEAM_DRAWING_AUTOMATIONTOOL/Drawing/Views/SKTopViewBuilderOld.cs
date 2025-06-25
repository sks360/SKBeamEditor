using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;

using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using SK.Tekla.Drawing.Automation.Drawing.Dimensions;
using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;
using MySqlX.XDevAPI;
using Tekla.Structures;
using Tekla.Structures.Drawing;

namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    public class SKTopViewBuilderOld
    {
        private readonly CustomInputModel _inputModel;

        private SKFlangeOutDimension flangeOutDimension;

        private string client; //client


        private readonly SKSortingHandler sortingHandler;
        private readonly DuplicateRemover duplicateRemover;
        private readonly BoltMatrixHandler boltMatrixHandler;



        public SKTopViewBuilderOld(SKSortingHandler sortingHandler, SKFlangeOutDimension flangeOutDimension,
            DuplicateRemover duplicateRemover,
            BoltMatrixHandler boltMatrixHandler,
            CustomInputModel inputModel)
        {
            this.sortingHandler = sortingHandler;
            _inputModel = inputModel;
            this.client = inputModel.Client;
            this.flangeOutDimension = flangeOutDimension;

        }

        private void TOP_view_creation(TSM.Beam MAINPART, TSD.View view_for_bottom_view,
           double output, double height_of_mainpart, out TSD.View bottom_view,
           out TSD.View bottom_view1, TSG.Point p1, TSG.Point p2, string BOTTOM,
           out List<TSD.View> TOP_VIEW_FLANGE_CUT_LIST, string drg_attribute)
        {
            bottom_view = null;
            bottom_view1 = null;

            TSD.SectionMark sec = null;
            TSD.SectionMark sec1 = null;
            TOP_VIEW_FLANGE_CUT_LIST = new List<TSD.View>();
            if (BOTTOM == "LEFT")
            {
                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();

                }
                else
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();

                }
                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);


                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                TOP_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
                double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.Y);
                double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.Y);
                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                {
                    bottom_view.RestrictionBox.MaxPoint.Y = change_min;
                    bottom_view.Modify();

                }
                else
                {
                    bottom_view.RestrictionBox.MinPoint.Y = -change_max;
                    bottom_view.Modify();

                }
                TSD.FontAttributes FONT = new TSD.FontAttributes();
                FONT.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);

                bottom_view1 = null;

                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



                sec.Attributes.LineColor = TSD.DrawingColors.Blue;
                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);
                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                bottom_view.Modify();
                TSD.SymbolInfo slotsymbol = new TSD.SymbolInfo("sections", 48);
                TSG.Point insertionpoint = new TSG.Point(bottom_view.RestrictionBox.MaxPoint.X, 0, 0);
                TSD.Symbol newsymbol = new TSD.Symbol(bottom_view, insertionpoint, slotsymbol);
                newsymbol.Insert();
                newsymbol.Attributes.Height = 25.4;
                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                newsymbol.Modify();
                //   sec.Modify();
                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                TSD.DrawingObjectEnumerator dim_drg = bottom_view.GetAllObjects(type_for_dim);
                while (dim_drg.MoveNext())
                {
                    var obj = dim_drg.Current;
                    obj.Delete();

                }
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view, MAINPART, drg_attribute);
            }

            if (BOTTOM == "RIGHT")
            {
                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();


                }
                else
                {
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();

                }

                TOP_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
                double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.Y);
                double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.Y);
                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                {
                    bottom_view.RestrictionBox.MaxPoint.Y = change_min;
                    bottom_view.Modify();

                }
                else
                {
                    bottom_view.RestrictionBox.MinPoint.Y = -change_max;
                    bottom_view.Modify();

                }
                TSD.FontAttributes FONT = new TSD.FontAttributes();
                FONT.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);

                bottom_view1 = null;

                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



                sec.Attributes.LineColor = TSD.DrawingColors.Blue;
                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);
                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                bottom_view.Modify();
                TSD.SymbolInfo slotsymbol = new TSD.SymbolInfo("sections", 48);
                TSG.Point insertionpoint = new TSG.Point(bottom_view.RestrictionBox.MinPoint.X, 0, 0);
                TSD.Symbol newsymbol = new TSD.Symbol(bottom_view, insertionpoint, slotsymbol);
                newsymbol.Insert();
                newsymbol.Attributes.Height = 25.4;
                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                newsymbol.Modify();
                //  sec.Modify();
                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                TSD.DrawingObjectEnumerator dim_drg = bottom_view.GetAllObjects(type_for_dim);
                while (dim_drg.MoveNext())
                {
                    var obj = dim_drg.Current;
                    obj.Delete();

                }
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view, MAINPART, drg_attribute);
            }
            //  bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output, 0, 0), new TSG.Point(0, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes,TSD.SectionMarkBase.SectionMarkAttributes.Equals(TSD.View.ViewTypes.FrontView), out bottom_view, out sec);

            if (BOTTOM == "BOTH")
            {

                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view1, out sec1);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view1.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                    bottom_view1.Modify();

                }
                else
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view1, out sec1);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view1.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                    bottom_view1.Modify();

                }
                TOP_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
                TOP_VIEW_FLANGE_CUT_LIST.Add(bottom_view1);
                double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.Y);
                double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.Y);
                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                {
                    bottom_view.RestrictionBox.MaxPoint.Y = change_min;
                    bottom_view.Modify();

                }
                else
                {
                    bottom_view.RestrictionBox.MinPoint.Y = -change_max;
                    bottom_view.Modify();

                }
                double change_min_1 = Math.Abs(bottom_view1.RestrictionBox.MinPoint.Y);
                double change_max_1 = Math.Abs(bottom_view1.RestrictionBox.MaxPoint.Y);
                if (Convert.ToInt64(change_min_1) > Convert.ToInt64(change_max_1))
                {
                    bottom_view1.RestrictionBox.MaxPoint.Y = change_min;
                    bottom_view1.Modify();

                }
                else
                {
                    bottom_view1.RestrictionBox.MinPoint.Y = -change_max;
                    bottom_view1.Modify();

                }


                TSD.FontAttributes FONT = new TSD.FontAttributes();
                FONT.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);



                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



                sec.Attributes.LineColor = TSD.DrawingColors.Blue;
                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);
                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                bottom_view.Modify();
                TSD.SymbolInfo slotsymbol = new TSD.SymbolInfo("sections", 48);
                TSG.Point insertionpoint = new TSG.Point(bottom_view.RestrictionBox.MaxPoint.X, 0, 0);
                TSD.Symbol newsymbol = new TSD.Symbol(bottom_view, insertionpoint, slotsymbol);
                newsymbol.Insert();
                newsymbol.Attributes.Height = 25.4;
                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                newsymbol.Modify();
                //  sec.Modify();
                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                TSD.DrawingObjectEnumerator dim_drg = bottom_view.GetAllObjects(type_for_dim);
                while (dim_drg.MoveNext())
                {
                    var obj = dim_drg.Current;
                    obj.Delete();

                }
                TSD.DrawingObjectEnumerator dim_drg1 = bottom_view1.GetAllObjects(type_for_dim);
                while (dim_drg1.MoveNext())
                {
                    var obj = dim_drg1.Current;
                    obj.Delete();

                }
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view, MAINPART, drg_attribute);
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view1, MAINPART, drg_attribute);



                TSD.FontAttributes FONT1 = new TSD.FontAttributes();
                FONT1.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);



                TSD.TextElement textelement21 = new TSD.TextElement(sec1.Attributes.MarkName, FONT);
                TSD.TextElement textelement31 = new TSD.TextElement("-", FONT1);
                TSD.ContainerElement sectionmark1 = new TSD.ContainerElement { textelement21, textelement31, textelement21 };



                sec1.Attributes.LineColor = TSD.DrawingColors.Blue;
                sec1.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement21 });

                bottom_view1.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark1);
                bottom_view1.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
                bottom_view1.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                sec1.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                sec1.Attributes.LineColor = TSD.DrawingColors.Magenta;
                bottom_view1.Modify();
                TSD.SymbolInfo slotsymbol1 = new TSD.SymbolInfo("sections", 48);
                TSG.Point insertionpoint1 = new TSG.Point(bottom_view1.RestrictionBox.MinPoint.X, 0, 0);
                TSD.Symbol newsymbol1 = new TSD.Symbol(bottom_view1, insertionpoint1, slotsymbol1);
                newsymbol1.Insert();
                newsymbol1.Attributes.Height = 25.4;
                newsymbol1.Attributes.Color = TSD.DrawingColors.Green;
                newsymbol1.Modify();
                //  sec1.Modify();

            }


        }


        public List<TSD.View> CreateTopView(Type type_for_bolt, double output, string TOP_VIEW_TOCREATE,
            string TOP_VIEW_needed, string drg_att, List<double> MAINPART_PROFILE_VALUES, TSM.Beam main,
            TSG.Point p3_bottm, TSG.Point p4_bottm,
            StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes, TSD.View current_view)
        {
            List<TSD.View> TOP_view_FLANGE_CUT_LIST = new List<TSD.View>();

            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, drg_att);

            //////////////////////////////////////////BOTTOM VIEW CREATION USING FUNCTION//////////////////////////////////////////

            TSD.View CREATE_BOTTOM_VIEW = null;
            TSD.View CREATE_BOTTOM_VIEW1 = null;
            if (TOP_VIEW_needed != "yes")
            {
                TOP_view_creation(main, current_view, output, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]), 
                    out CREATE_BOTTOM_VIEW, out CREATE_BOTTOM_VIEW1, p3_bottm, p4_bottm, TOP_VIEW_TOCREATE, 
                    out TOP_view_FLANGE_CUT_LIST, drg_att);
                flangeOutDimension.Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW, main, drg_att);
                flangeOutDimension.Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW1, main, drg_att);
            }

            if ((CREATE_BOTTOM_VIEW != null) || (CREATE_BOTTOM_VIEW1 != null))
            {
                //////////////////////////////////////////////////FLANGE CUT DIMENSION ///////////////////////////////////////////////////////
                if (TOP_VIEW_TOCREATE.Contains("ON") || TOP_VIEW_TOCREATE.Contains("RIGHT") || TOP_VIEW_TOCREATE.Contains("LEFT") || TOP_VIEW_TOCREATE.Contains("BOTH"))
                {


                    //Create_FLANGE_CUT_dimensions(CREATE_BOTTOM_VIEW, main);


                    ///////////////////////////////////////////////////////////////////////

                    /////////BOLT RD DIMENSION//////////////
                    ///////////////////////////////////////////////////filtering bolts from all parts in top view/////////////////////////////////////////////////////////////////////////////
                    TSD.DrawingObjectEnumerator enum_for_bolt2 = CREATE_BOTTOM_VIEW.GetAllObjects(type_for_bolt);
                    TSG.Matrix bottom_mat = TSG.MatrixFactory.ToCoordinateSystem(CREATE_BOTTOM_VIEW.ViewCoordinateSystem);
                    TSD.PointList rd_point_list1 = new TSD.PointList();
                    ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
                    while (enum_for_bolt2.MoveNext())
                    {
                        TSD.Bolt drgbolt = enum_for_bolt2.Current as TSD.Bolt;
                        TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                        TSM.BoltArray bolt = modelbolt as TSM.BoltArray;


                        if (bottom_mat.Transform((bolt.BoltPositions[0]) as TSG.Point).Z < (bottom_mat.Transform(main.EndPoint).Z) - Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2)
                        {
                            TSD.Bolt drgbolt1 = drgbolt;
                            TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, CREATE_BOTTOM_VIEW);
                            if (POINT_FOR_BOLT_MATRIX != null)
                            {
                                int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                for (int i = 0; i < x; i++)
                                {
                                    ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                    rd_point_list1.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                                }
                            }
                        }
                    }
                    /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                    TSD.PointList FINAL_RD_LIST2 = duplicateRemover.RemoveDuplicateXValues(rd_point_list1);
                    FINAL_RD_LIST2.Add(new TSG.Point(0, 0, 0));
                    sortingHandler.SortPoints(FINAL_RD_LIST2);
                    /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
                    TSD.StraightDimensionSetHandler bolt_rd_dim1 = new TSD.StraightDimensionSetHandler();
                    try
                    {
                        TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                        rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                        rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                        rd_att.Color = DrawingColors.Gray70;
                        rd_att.Text.Font.Color = DrawingColors.Gray70;
                        rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                        rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, drg_att);
                        ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                        double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                        TSG.Point p1 = (FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point);
                        TSG.Point p2 = new TSG.Point((FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point).X, distance, 0);
                        double distance_value = TSG.Distance.PointToPoint(p1, p2);
                        /////////////////////////////////////////////////////rd dimension creation //////////////////////////////////////////////////////////////////////////////////////////////////////
                        bolt_rd_dim1.CreateDimensionSet(CREATE_BOTTOM_VIEW, FINAL_RD_LIST2, new TSG.Vector(0, 1, 0), distance_value, rd_att);
                    }
                    catch
                    {
                    }
                    //////////////End of bolt rd dimension FOR BottomVIEW //////////////////////////






                    //////////////3x3 dimension for top view/////////////////////
                    TSD.DrawingObjectEnumerator enum_for_bolt3 = CREATE_BOTTOM_VIEW.GetAllObjects(type_for_bolt);
                    while (enum_for_bolt3.MoveNext())
                    {
                        TSD.PointList list3x3 = new TSD.PointList();
                        TSD.Bolt drgbolt = enum_for_bolt3.Current as TSD.Bolt;
                        TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                        TSM.BoltArray bolt = modelbolt as TSM.BoltArray;

                        if (bottom_mat.Transform((bolt.BoltPositions[0]) as TSG.Point).Z < (bottom_mat.Transform(main.EndPoint).Z) - Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2)
                        {
                            TSD.Bolt drgbolt1 = drgbolt;
                            TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, CREATE_BOTTOM_VIEW);
                            if (POINT_FOR_BOLT_MATRIX != null)
                            {
                                int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                for (int i = 0; i < y; i++)
                                {
                                    /////////////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                    list3x3.Add(POINT_FOR_BOLT_MATRIX[i, x - 1]);
                                }
                                /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                                TSD.PointList FINAL_list3x3 = duplicateRemover.RemoveDuplicateYValues(list3x3);
                                FINAL_list3x3.Add(new TSG.Point(list3x3[0].X, 0, 0));
                                /////////////////////////////////////////////////// inserting bolt 3X3 dimension ////////////////////////////////////////////////////////////////////////////////////////
                                TSD.StraightDimensionSetHandler dim_3x3 = new TSD.StraightDimensionSetHandler();
                                try
                                {

                                    //TSG.Point p1 = (FINAL_list3x3[FINAL_list3x3.Count - 1] as TSG.Point);
                                    //TSG.Point p2 = new TSG.Point((FINAL_list3x3[FINAL_list3x3.Count - 1] as TSG.Point).X, 0, 0);
                                    //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                                    dim_3x3.CreateDimensionSet(CREATE_BOTTOM_VIEW, FINAL_list3x3, new TSG.Vector(1, 0, 0), 200, fixed_attributes);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    ////////////////////end of 3x3 dimension  for top view///////////////////////







                    ///////////////////////////////////////////////////////////////////////

                }
            }


            return TOP_view_FLANGE_CUT_LIST;
        }



        public void TOPVIEW_CHECK(TSM.Beam MAINPART, out string top_view_creation, double height_of_mainpart,
            out TSG.Point pt1_for_bottom_view, out TSG.Point pt2_for_bottom_view,
            double output, out string top_view_Create_check)
        {

            top_view_creation = "";
            top_view_Create_check = "";
            Type bolpart1 = typeof(TSM.BooleanPart);
            Type fit1 = typeof(TSM.Fitting);
            Type type_for_contourplate = typeof(TSM.ContourPlate);

            ArrayList pts_in_viewco1 = new ArrayList();
            TSD.PointList POSI = new TSD.PointList();
            TSD.PointList NEGI = new TSD.PointList();
            pt1_for_bottom_view = null;
            pt2_for_bottom_view = null;
            TSG.Matrix toview = TSG.MatrixFactory.ToCoordinateSystem(MAINPART.GetCoordinateSystem());

            if ((top_view_creation.Equals("OFF")) || (top_view_creation.Equals("")))
            {
                TSM.ModelObjectEnumerator test_boolFOR_BOTTOM = MAINPART.GetBooleans();
                while (test_boolFOR_BOTTOM.MoveNext())
                {
                    var partcut1 = test_boolFOR_BOTTOM.Current;
                    if (partcut1.GetType().Equals(fit1))
                    {
                    }
                    else if (partcut1.GetType().Equals(bolpart1))
                    {
                        TSM.BooleanPart fitobj = partcut1 as TSM.BooleanPart;

                        if (fitobj.OperativePart.GetType().Equals(type_for_contourplate))
                        {
                            TSM.ContourPlate platecut = fitobj.OperativePart as TSM.ContourPlate;
                            ArrayList pts = platecut.Contour.ContourPoints;
                            ArrayList x_list = new ArrayList();
                            ArrayList y_list = new ArrayList();
                            ArrayList z_list = new ArrayList();
                            if (pts.Count > 3)
                            {
                                if (Convert.ToInt64(toview.Transform(pts[0] as TSG.Point).Y) == Convert.ToInt64(toview.Transform(pts[2] as TSG.Point).Y))
                                {
                                    foreach (TSG.Point bolpart_point in pts)
                                    {
                                        TSG.Point CONVERTED_BOLL_POINT = toview.Transform(bolpart_point);
                                        if (Convert.ToInt64(CONVERTED_BOLL_POINT.Y) >= 0)
                                        {
                                            pts_in_viewco1.Add(CONVERTED_BOLL_POINT);
                                        }
                                    }
                                }
                            }

                        }

                    }
                }

                if (pts_in_viewco1.Count > 1)
                {

                    foreach (TSG.Point PTS in pts_in_viewco1)
                    {
                        if ((Convert.ToInt16(PTS.X) < 1000) && (Convert.ToInt16(PTS.X) > 0))
                        {
                            if (PTS.Y >= 0)
                            {
                                POSI.Add(PTS);
                            }
                        }
                        else if ((Convert.ToInt16(PTS.X) > 1000) && (Convert.ToInt16(PTS.X) < output))
                        {
                            if (PTS.Y >= 0)
                            {
                                NEGI.Add(PTS);
                            }
                        }

                    }
                    try
                    {
                        sortingHandler.SortPoints(POSI);
                    }
                    catch
                    {
                    }
                    try
                    {
                        sortingHandler.SortPoints(NEGI);
                    }
                    catch
                    {
                    }

                    if ((POSI.Count > 1) && (NEGI.Count > 1))
                    {

                        top_view_Create_check = "BOTH";
                        pt1_for_bottom_view = POSI[0];
                        pt2_for_bottom_view = NEGI[0];

                    }
                    else if ((POSI.Count) > 1)
                    {

                        top_view_Create_check = "LEFT";
                        pt1_for_bottom_view = POSI[0];


                    }
                    else if ((NEGI.Count) > 1)
                    {

                        top_view_Create_check = "RIGHT";
                        pt2_for_bottom_view = NEGI[0];

                    }






                }
            }




        }


        public string TOPVIEW_needed(TSM.Beam MAINPART, double height_of_mainpart, double output)
        {

            TSM.Model mymodel = new TSM.Model();
            string top_view_Create_check = "";
            Type bolpart1 = typeof(TSM.BooleanPart);
            Type fit1 = typeof(TSM.Fitting);
            Type cut_plane = typeof(TSM.CutPlane);

            Type type_for_contourplate = typeof(TSM.ContourPlate);

            ArrayList pts_in_viewco1 = new ArrayList();
            TSD.PointList POSI = new TSD.PointList();
            TSD.PointList NEGI = new TSD.PointList();

            TSG.Matrix toview = TSG.MatrixFactory.ToCoordinateSystem(MAINPART.GetCoordinateSystem());





            TSM.ModelObjectEnumerator test_boolFOR_BOTTOM = MAINPART.GetBooleans();
            while (test_boolFOR_BOTTOM.MoveNext())
            {
                var partcut1 = test_boolFOR_BOTTOM.Current;
                if (partcut1.GetType().Equals(fit1))
                {
                }
                else if (partcut1.GetType().Equals(cut_plane))
                {
                    TSM.CutPlane fitobj = partcut1 as TSM.CutPlane;
                    TSG.Vector x_vector = fitobj.Plane.AxisX;
                    TSG.Vector y_vector = fitobj.Plane.AxisY;
                    TSG.Vector Z_vector = (y_vector.Cross(x_vector));
                    Z_vector.Normalize();
                    if ((Z_vector.X != 0) && (Z_vector.Y != 0))
                    {
                        top_view_Create_check = "yes";
                    }


                }
                else if (partcut1.GetType().Equals(bolpart1))
                {
                    TSM.BooleanPart fitobj = partcut1 as TSM.BooleanPart;

                    if (fitobj.OperativePart.GetType().Equals(type_for_contourplate))
                    {
                        TSM.ContourPlate platecut = fitobj.OperativePart as TSM.ContourPlate;
                        ArrayList pts = platecut.Contour.ContourPoints;
                        ArrayList x_list = new ArrayList();
                        ArrayList y_list = new ArrayList();
                        ArrayList z_list = new ArrayList();
                        if (pts.Count > 2)
                        {
                            if (Convert.ToInt16(toview.Transform(pts[0] as TSG.Point).Y) == Convert.ToInt16(toview.Transform(pts[2] as TSG.Point).Y))
                            {
                                foreach (TSG.Point bolpart_point in pts)
                                {
                                    TSG.Point CONVERTED_BOLL_POINT = toview.Transform(bolpart_point);

                                    if (Convert.ToInt16(CONVERTED_BOLL_POINT.Y) > -10)
                                    {
                                        top_view_Create_check = "yes";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return top_view_Create_check;
        }


        public void top_view_check_for_dim(TSD.AssemblyDrawing beam_assembly_drg, string TOP_VIEW_needed)
        {
            TSD.DrawingObjectEnumerator MYDRG_VIEWS_for_top_view_delete = beam_assembly_drg.GetSheet().GetAllViews();
            while (MYDRG_VIEWS_for_top_view_delete.MoveNext())
            {
                Type type_for_dim = typeof(TSD.DimensionBase);
                TSD.View MYVIEW = MYDRG_VIEWS_for_top_view_delete.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_dim);
                    int size_for_dim_check = my_top_view_dimension_check.GetSize();
                    if (size_for_dim_check > 0 || TOP_VIEW_needed == "yes")
                    {
                        continue;
                    }
                    if (client == "SME")
                    {
                        DrawingObjectEnumerator drawingObjects = MYVIEW.GetAllObjects(typeof(Mark));
                        while (drawingObjects.MoveNext())
                        {
                            drawingObjects.Current.Delete();
                        }
                    }
                    else
                    {
                        MYVIEW.Delete();
                    }
                }



            }
        }

    }
}
