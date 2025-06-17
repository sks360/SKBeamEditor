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
    public class SKBottomView
    {
        private readonly CustomInputModel _userInput;

        private SKFlangeOutDimension flangeOutDimension;

        private string client; //client


        private readonly SKSortingHandler sortingHandler;

        public SKBottomView(SKSortingHandler sortingHandler,  SKFlangeOutDimension flangeOutDimension,
            CustomInputModel userInput)
        {
            this.sortingHandler = sortingHandler;
            _userInput = userInput;
            this.client = userInput.Client;
            this.flangeOutDimension = flangeOutDimension;

        }

        public void BOTTOMVIEW_CHECK(TSM.Beam MAINPART, out string bottom_view_creation,
            double height_of_mainpart, out TSG.Point pt1_for_bottom_view,
            out TSG.Point pt2_for_bottom_view, double output, out string bottom_view_Create_check)
        {

            bottom_view_creation = "";
            bottom_view_Create_check = "";
            Type bolpart1 = typeof(TSM.BooleanPart);
            Type fit1 = typeof(TSM.Fitting);
            Type type_for_contourplate = typeof(TSM.ContourPlate);

            ArrayList pts_in_viewco1 = new ArrayList();
            TSD.PointList POSI = new TSD.PointList();
            TSD.PointList NEGI = new TSD.PointList();
            pt1_for_bottom_view = null;
            pt2_for_bottom_view = null;
            TSG.Matrix toview = TSG.MatrixFactory.ToCoordinateSystem(MAINPART.GetCoordinateSystem());



            TSM.ModelObjectEnumerator ENUM_FOR_BOLT_CHECK = MAINPART.GetBolts();
            while (ENUM_FOR_BOLT_CHECK.MoveNext())
            {
                TSM.BoltGroup MYBOLTGROUP = ENUM_FOR_BOLT_CHECK.Current as TSM.BoltGroup;
                TSG.CoordinateSystem boltcoord1 = MYBOLTGROUP.GetCoordinateSystem();
                TSG.Vector XVECTOR_FOR_BOLT = boltcoord1.AxisX;
                TSG.Vector YVECTOR_FOR_BOLT = boltcoord1.AxisY;
                TSG.Vector ZVECTOR_FOR_BOLT = TSG.Vector.Cross(YVECTOR_FOR_BOLT, XVECTOR_FOR_BOLT);

                int NO_OF_BOLT = MYBOLTGROUP.BoltPositions.Count;
                if (NO_OF_BOLT > 0)
                {
                    if ((MYBOLTGROUP.BoltPositions[0] as TSG.Point).Z < ((MAINPART.EndPoint.Z) - height_of_mainpart / 2))
                    {
                        if ((ZVECTOR_FOR_BOLT.Z != 0))
                        {
                            bottom_view_creation = "ON";
                            bottom_view_Create_check = "ON";
                            break;

                        }
                        else
                        {
                            bottom_view_creation = "OFF";
                        }
                    }
                }
            }
            if ((bottom_view_creation.Equals("OFF")) || (bottom_view_creation.Equals("")))
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

                            if (Convert.ToInt64(toview.Transform(pts[0] as TSG.Point).Y) == Convert.ToInt64(toview.Transform(pts[2] as TSG.Point).Y))
                            {
                                foreach (TSG.Point bolpart_point in pts)
                                {
                                    TSG.Point CONVERTED_BOLL_POINT = toview.Transform(bolpart_point);


                                    if (Convert.ToInt64(CONVERTED_BOLL_POINT.Y) <= 0)
                                    {


                                        pts_in_viewco1.Add(CONVERTED_BOLL_POINT);
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
                            if (PTS.Y <= 0)
                            {
                                POSI.Add(PTS);
                            }
                        }
                        else if ((Convert.ToInt16(PTS.X) > 1000) && (Convert.ToInt16(PTS.X) < output))
                        {
                            if (PTS.Y <= 0)
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

                        bottom_view_Create_check = "BOTH";
                        pt1_for_bottom_view = POSI[0];
                        pt2_for_bottom_view = NEGI[0];

                    }
                    else if ((POSI.Count) > 1)
                    {

                        bottom_view_Create_check = "LEFT";
                        pt1_for_bottom_view = POSI[0];


                    }
                    else if ((NEGI.Count) > 1)
                    {

                        bottom_view_Create_check = "RIGHT";
                        pt2_for_bottom_view = NEGI[0];

                    }






                }
            }




        }

        public void bottom_view_creation(TSM.Beam MAINPART, TSD.View view_for_bottom_view, double output, double height_of_mainpart, out TSD.View bottom_view, out TSD.View bottom_view1, TSG.Point p1, TSG.Point p2, string BOTTOM, double SHORTNENING_VALUE_FOR_BOTTOM_VIEW, List<TSM.Part> mark_retain_partlist, out List<TSD.View> bottom_view_list, out List<TSD.View> BOTTOM_VIEW_FLANGE_CUT_LIST, string drg_attribute)
        {
            bottom_view = null;
            bottom_view1 = null;

            TSD.SectionMark sec = null;
            TSD.SectionMark sec1 = null;

            Type type_for_weld = typeof(TSD.WeldMark);
            bottom_view_list = new List<TSD.View>();
            BOTTOM_VIEW_FLANGE_CUT_LIST = new List<TSD.View>();


            if ((BOTTOM == "ON"))
            {
                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 30, 0, 0), new TSG.Point(-30, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();

                }
                else
                {
                    bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 30, 0, 0), new TSG.Point(-30, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                }

                //bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 30, 0, 0), new TSG.Point(-30, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);



                //bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 30, 0, 0), new TSG.Point(-30, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                bottom_view_list.Add(bottom_view);
                arrange_parts_for_bottom_view(bottom_view, mark_retain_partlist);


                //bottom_view.Attributes.Shortening.MinimumLength = SHORTNENING_VALUE_FOR_BOTTOM_VIEW;
                //bottom_view.Attributes.Shortening.CutPartType = TSD.View.ShorteningCutPartType.X_Direction;
                //bottom_view.Modify();
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

                double change_min_x = Math.Abs(view_for_bottom_view.RestrictionBox.MinPoint.X);
                double change_max_x = Math.Abs(view_for_bottom_view.RestrictionBox.MaxPoint.X);

                bottom_view.RestrictionBox.MaxPoint.X = change_max_x;
                bottom_view.RestrictionBox.MinPoint.X = -change_min_x;
                bottom_view.Modify();


                TSD.FontAttributes FONT = new TSD.FontAttributes();
                FONT.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);

                bottom_view1 = null;

                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(1, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);
                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;
                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                bottom_view.Modify();
                // sec.Modify();
                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                TSD.DrawingObjectEnumerator dim_drg = bottom_view.GetAllObjects(type_for_dim);
                while (dim_drg.MoveNext())
                {
                    var obj = dim_drg.Current;
                    obj.Delete();

                }
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view, MAINPART, drg_attribute);

            }
            else if (BOTTOM == "LEFT")
            {
                //   bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(500, -30, 0), new TSG.Point(p1.X - 300, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);

                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();

                }
                else
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                }
                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);


                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);


                BOTTOM_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
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
                FONT.Color = TSD.DrawingColors.Green;
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
                // sec.Modify();
                Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                TSD.DrawingObjectEnumerator dim_drg = bottom_view.GetAllObjects(type_for_dim);
                while (dim_drg.MoveNext())
                {
                    var obj = dim_drg.Current;
                    obj.Delete();

                }
                flangeOutDimension.Create_FLANGE_CUT_dimensions_bottom(bottom_view, MAINPART, drg_attribute);

            }
            else if (BOTTOM == "RIGHT")
            {
                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                }
                else
                {
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                }

                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);

                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);

                BOTTOM_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
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



                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
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
                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                newsymbol.Attributes.Height = 25.4;
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
            //  bool result = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output, 0, 0), new TSG.Point(0, 0, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes,TSD.SectionMarkBase.SectionMarkAttributes.Equals(TSD.View.ViewTypes.FrontView), out bottom_view, out sec);

            else if (BOTTOM == "BOTH")
            {
                if (drg_attribute == "SK_BEAM_A1")
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view1, out sec1);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view1.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                    bottom_view1.Modify();

                }
                else
                {
                    bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                    bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view1, out sec1);
                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view1.Attributes.LoadAttributes("SK_BEAM_A1");
                    bottom_view.Modify();
                    bottom_view1.Modify();

                }

                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view1, out sec1);


                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view1, out sec1);
                BOTTOM_VIEW_FLANGE_CUT_LIST.Add(bottom_view);
                BOTTOM_VIEW_FLANGE_CUT_LIST.Add(bottom_view1);
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
                //bottom_view.RestrictionBox.MaxPoint = new TSG.Point(p1.X + 300, -30, 0);
                //bottom_view.RestrictionBox.MinPoint = new TSG.Point(-500, -30, 0);

                //bottom_view1.RestrictionBox.MaxPoint = new TSG.Point(output + 500, -30, 0);
                //bottom_view1.RestrictionBox.MinPoint = new TSG.Point(p2.X - 300, -30, 0);
                //bottom_view.Modify();
                //bottom_view1.Modify();

                TSD.FontAttributes FONT = new TSD.FontAttributes();
                FONT.Color = TSD.DrawingColors.Magenta;
                FONT.Height = Convert.ToInt16(3.96875);



                TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };



                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
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

                // sec.Modify();
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
                FONT1.Height = Convert.ToInt16(3.96875);




                TSD.TextElement textelement21 = new TSD.TextElement(sec1.Attributes.MarkName, FONT);
                TSD.TextElement textelement31 = new TSD.TextElement("-", FONT1);
                TSD.ContainerElement sectionmark1 = new TSD.ContainerElement { textelement21, textelement31, textelement21 };



                sec1.Attributes.LineColor = TSD.DrawingColors.Magenta;
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
                //   sec1.Modify();

            }
            if (bottom_view != null)
            {
                try
                {
                    TSD.DrawingObjectEnumerator weld_marks = bottom_view.GetAllObjects(type_for_weld);
                    while (weld_marks.MoveNext())
                    {
                        var weldmark = weld_marks.Current;
                        weldmark.Delete();
                    }
                }
                catch
                {
                }
            }
            if (bottom_view1 != null)
            {
                try
                {
                    TSD.DrawingObjectEnumerator weld_marks = bottom_view1.GetAllObjects(type_for_weld);
                    while (weld_marks.MoveNext())
                    {
                        var weldmark = weld_marks.Current;
                        weldmark.Delete();
                    }
                }
                catch
                {
                }
            }
        }

        public void arrange_parts_for_bottom_view(TSD.View current_view, List<TSM.Part> list)
        {
            Type[] type_for_mark = new Type[] { typeof(TSD.Mark), typeof(TSD.WeldMark) };
            TSD.DrawingObjectEnumerator enum_for_mark = current_view.GetAllObjects(type_for_mark);
            List<int> list_of_id = new List<int>();
            foreach (TSM.Part id_for_part in list)
            {
                list_of_id.Add(id_for_part.Identifier.ID);

            }


            while (enum_for_mark.MoveNext())
            {
                var mark = enum_for_mark.Current;

                if (mark.GetType().Equals(typeof(TSD.Mark)))
                {
                    TSD.Mark mymark = mark as TSD.Mark;

                    TSD.DrawingObjectEnumerator enumcheck = mymark.GetRelatedObjects();

                    while (enumcheck.MoveNext())
                    {
                        var mark_part = enumcheck.Current;
                        if (mark_part.GetType().Equals(typeof(TSD.Part)))
                        {
                            TSM.Part modelpart = new TSM.Model().SelectModelObject((mark_part as TSD.Part).ModelIdentifier) as TSM.Part;

                            Guid guid = modelpart.Identifier.GUID;

                            bool part_to_retain = list_of_id.Contains(modelpart.Identifier.ID);

                            if (part_to_retain == true)
                            {
                            }
                            else
                            {
                                mymark.Delete();
                            }


                        }



                    }
                }


                else if (mark.GetType().Equals(typeof(TSD.WeldMark)))
                {
                    TSD.WeldMark weldmark = mark as TSD.WeldMark;




                    {
                        //TSD.DrawingObjectEnumerator enumcheck1 = weldmark.GetObjects();
                        Identifier id = weldmark.ModelIdentifier;
                        TSM.BaseWeld weld = (new TSM.Model().SelectModelObject(id) as TSM.BaseWeld);
                        TSM.Part mainpart = (weld.MainObject as TSM.Part);
                        TSM.Part secondary_part = (weld.SecondaryObject as TSM.Part);

                    }

                }
            }


        }


        public void TOPVIEW_CHECK(TSM.Beam MAINPART, out string top_view_creation, double height_of_mainpart, out TSG.Point pt1_for_bottom_view, out TSG.Point pt2_for_bottom_view, double output, out string top_view_Create_check)
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


        public void TOPVIEW_needed(TSM.Beam MAINPART, double height_of_mainpart, double output, out string top_view_Create_check)
        {

            TSM.Model mymodel = new TSM.Model();
            top_view_Create_check = "";
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
        }

        public void TOP_view_creation(TSM.Beam MAINPART, TSD.View view_for_bottom_view, 
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
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);

                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
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
                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view,new TSG.Point(output +500, -30, 0), new TSG.Point(p2.X-300, -30, 0),  new TSG.Point(view_for_bottom_view.ExtremaCenter.X, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view1, out sec1);
                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, -30, 0), new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view1, out sec1);

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

                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view1, out sec1);


                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p1.X + 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(-500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(output + 500, 30 + (height_of_mainpart / 2), 0), new TSG.Point(p2.X - 300, 30 + (height_of_mainpart / 2), 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view1, out sec1);
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
                //bool result11 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(-500, -30, 0), new TSG.Point(p1.X + 300, -30, 0), new TSG.Point(p1.X + 300, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                //bool result12 = TSD.View.CreateSectionView(view_for_bottom_view, new TSG.Point(p2.X - 300, -30, 0), new TSG.Point(output + 500, -30, 0), new TSG.Point(output + 500, 200, 0), height_of_mainpart / 2 + 50, 30, view_for_bottom_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view1, out sec1);




                //bottom_view.RestrictionBox.MaxPoint = new TSG.Point(p1.X + 300, -30, 0);
                //bottom_view.RestrictionBox.MinPoint = new TSG.Point(-500, -30, 0);

                //bottom_view1.RestrictionBox.MaxPoint = new TSG.Point(output + 500, -30, 0);
                //bottom_view1.RestrictionBox.MinPoint = new TSG.Point(p2.X - 300, -30, 0);
                //bottom_view.Modify();
                //bottom_view1.Modify();

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
                    int size_for_dim_check = 0;
                    size_for_dim_check = my_top_view_dimension_check.GetSize();
                    if (size_for_dim_check > 0)
                    {

                    }
                    else if (TOP_VIEW_needed == "yes")
                    {
                    }
                    //else if (farside_parts.Count > 0)
                    //{
                    //}
                    else
                    {
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
}
