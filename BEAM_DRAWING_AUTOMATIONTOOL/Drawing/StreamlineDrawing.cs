using System;
using System.Collections;
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
using Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;

using Tekla.Structures.Drawing.Operations;

namespace SK.Tekla.Drawing.Automation.Drawing
{
    public class StreamlineDrawing
    {
        public StreamlineDrawing() { }


        public void SECTION_VIEW_PART_MARK_DELETE(TSD.View current_view, TSD.DrawingHandler my_handler)
        {
            ArrayList PART_MARK_TO_DELETE = new ArrayList();

            Type type_for_MARK = typeof(TSD.Mark);
            TSD.View MYVIEW = current_view;

            if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.SectionView))
            {

                TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_MARK);
                while (my_top_view_dimension_check.MoveNext())
                {

                    TSD.Mark MYMARK = my_top_view_dimension_check.Current as TSD.Mark;


                    PART_MARK_TO_DELETE.Add(MYMARK);

                }

            }
            my_handler.GetDrawingObjectSelector().SelectObjects(PART_MARK_TO_DELETE, true);
            TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
            my_handler.GetDrawingObjectSelector().UnselectAllObjects();
        }
        public void REMOVING_HLS(TSD.Mark mark_part)
        {



            TSD.ContainerElement CONTAINER1 = mark_part.Attributes.Content;


            IEnumerator CHECKFOR_HLS = CONTAINER1.GetEnumerator();
            ArrayList MYLIST_FOR_REMOVING_HLS = new ArrayList();
            while (CHECKFOR_HLS.MoveNext())
            {
                var NAME = CHECKFOR_HLS.Current;
                MYLIST_FOR_REMOVING_HLS.Add(NAME);
            }


            foreach (var NAME in MYLIST_FOR_REMOVING_HLS)
            {

                if (NAME.GetType().Equals(typeof(TSD.TextElement)))
                {
                    TSD.TextElement CHECK2 = NAME as TSD.TextElement;
                    string VALUE = CHECK2.Value;
                    if (VALUE.Contains("HLS"))
                    {

                        CHECK2.Value = "";
                        mark_part.Modify();


                    }
                }
            }








        }

        public void REMOVING_HLSTOHL(int BOLT_NUMBER, TSD.Mark mark_part)
        {

            if (BOLT_NUMBER == 1)
            {

                TSD.ContainerElement CONTAINER1 = mark_part.Attributes.Content;




                IEnumerator CHECKFOR_HLS = CONTAINER1.GetEnumerator();
                ArrayList MYLIST_FOR_REMOVING_HLS = new ArrayList();
                while (CHECKFOR_HLS.MoveNext())
                {

                    var NAME = CHECKFOR_HLS.Current;

                    MYLIST_FOR_REMOVING_HLS.Add(NAME);
                }


                foreach (var NAME in MYLIST_FOR_REMOVING_HLS)
                {


                    if (NAME.GetType().Equals(typeof(TSD.TextElement)))
                    {
                        TSD.TextElement CHECK2 = NAME as TSD.TextElement;
                        string VALUE = CHECK2.Value;
                        if (VALUE.Contains("HLS"))
                        {

                            CHECK2.Value = "HL";
                            mark_part.Modify();


                        }
                    }
                    else
                    {
                        string MM = NAME.ToString();
                    }
                }
            }

        }

        public void slot_symbol(TSD.Drawing mydrg, TSM.Part mainpart)
        {
            Type[] type_for_mark = new Type[] { typeof(TSD.Mark) };

            TSM.Model model = new TSM.Model();
            TSD.DrawingObjectEnumerator enum_for_views = mydrg.GetSheet().GetAllViews();
            string mainpartmark = mainpart.GetPartMark();
            while (enum_for_views.MoveNext())
            {

                TSD.View current_view = enum_for_views.Current as TSD.View;
                //TSD.DrawingObjectEnumerator enum_for_mark = mydrg.GetSheet().GetAllObjects(type_for_mark);
                TSD.DrawingObjectEnumerator enum_for_mark = current_view.GetAllObjects(type_for_mark);


                while (enum_for_mark.MoveNext())
                {
                    var mark = enum_for_mark.Current;

                    if (mark.GetType().Equals(typeof(TSD.Mark)))
                    {
                        TSD.Mark mymark = mark as TSD.Mark;

                        TSD.ContainerElement CONTAINER = mymark.Attributes.Content;
                        List<string> RESULT_FOR_SLOT = new List<string>();
                        IEnumerator CHECK1 = CONTAINER.GetEnumerator();
                        while (CHECK1.MoveNext())
                        {
                            var NAME = CHECK1.Current;
                            if (NAME.GetType().Equals(typeof(TSD.PropertyElement)))
                            {
                                TSD.PropertyElement CHECK2 = CHECK1.Current as TSD.PropertyElement;
                                string VALUE = CHECK2.Value;
                                if (VALUE.Contains("SLOTS"))
                                {
                                    RESULT_FOR_SLOT.Add("TRUE");



                                }
                            }
                        }
                        CHECK1.Reset();

                        bool RESULT = RESULT_FOR_SLOT.Any(X => X.Contains("TRUE"));




                        TSD.DrawingObjectEnumerator enumcheck = mymark.GetRelatedObjects();

                        while (enumcheck.MoveNext())
                        {
                            var mark_part = enumcheck.Current;


                            if (mark_part.GetType().Equals(typeof(TSD.Bolt)))
                            {
                                if (RESULT == true)
                                {

                                    TSM.BoltGroup bolt = new TSM.Model().SelectModelObject((mark_part as TSD.Bolt).ModelIdentifier) as TSM.BoltGroup;
                                    TSD.Mark mymark1 = mark as TSD.Mark;

                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                    /////////////////////////////////////////////////////////getting bolt coordinate system for checking of out of plane bolts/////////////////////////////////////////////////
                                    TSG.CoordinateSystem boltcoord1 = bolt.GetCoordinateSystem();
                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                    bool check_for_bolt_coord = (boltcoord1.AxisX.X.Equals(0)) && (boltcoord1.AxisY.Y.Equals(0));


                                    double x_val = bolt.SlottedHoleX;
                                    double y_val = bolt.SlottedHoleY;
                                    TSD.SymbolInfo slotsymbol = null;
                                    if (bolt.RotateSlots.Equals(TSM.BoltGroup.BoltRotateSlotsEnum.ROTATE_SLOTS_PARALLEL))
                                    {

                                        if (check_for_bolt_coord == true)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                //slotsymbol = new TSD.SymbolInfo("xsteel", 74);// CHECKED ERROR31-10-2018 burkecommunity church
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);




                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                //slotsymbol = new TSD.SymbolInfo("xsteel", 75);// CHECKED ERROR 31-10-2018 burkecommunity church
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);




                                            }
                                        }
                                        else if (check_for_bolt_coord == false)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);

                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);


                                            }
                                        }
                                    }
                                    else if (bolt.RotateSlots.Equals(TSM.BoltGroup.BoltRotateSlotsEnum.ROTATE_SLOTS_EVEN))
                                    {

                                        if (check_for_bolt_coord == true)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);

                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);


                                            }
                                        }
                                        else if (check_for_bolt_coord == false)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);

                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);


                                            }
                                        }
                                    }
                                    else if (bolt.RotateSlots.Equals(TSM.BoltGroup.BoltRotateSlotsEnum.ROTATE_SLOTS_ODD))
                                    {

                                        if (check_for_bolt_coord == true)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);

                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);


                                            }
                                        }
                                        else if (check_for_bolt_coord == false)
                                        {
                                            if ((x_val > 0) && (y_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 140, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);

                                            }
                                            if ((y_val > 0) && (x_val == 0))
                                            {
                                                slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                                TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                                TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                                newsymbol.Insert();
                                                newsymbol.Attributes.Height = 15;
                                                newsymbol.Attributes.Color = TSD.DrawingColors.Green;
                                                newsymbol.Modify();
                                                REMOVING_HLS(mymark1);


                                            }
                                        }
                                    }




                                }

                                //else
                                //{
                                //    TSM.BoltGroup bolt = new TSM.Model().SelectModelObject((mark_part as TSD.Bolt).ModelIdentifier) as TSM.BoltGroup;
                                // int NO_OF_BOLTS =  bolt.BoltPositions.Count;
                                //    TSD.Mark mymark1 = mark as TSD.Mark;
                                //    //REMOVING_HLSTOHL(NO_OF_BOLTS, mymark1);

                                //}


                            }
                        }
                    }
                }

            }



        }
    }
}
