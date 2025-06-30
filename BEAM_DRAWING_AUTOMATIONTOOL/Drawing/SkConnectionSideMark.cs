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

using Tekla.Structures.Drawing;
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;
using Tekla.Structures;

namespace SK.Tekla.Drawing.Automation.Drawing
{
    public class SkConnectionSideMark
    {
        public SkConnectionSideMark() { }

        public void connecting_side_mark(TSM.Model currentModel, TSD.Drawing mydrg, TSM.Part part)
        {
            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingObjectEnumerator enum_for_views = mydrg.GetSheet().GetAllViews();
            while (enum_for_views.MoveNext())
            {
                TSD.View current_view = enum_for_views.Current as TSD.View;
                if (current_view.ViewType == TSD.View.ViewTypes.TopView)
                {
                    TSD.DrawingObjectEnumerator enum_for_orientation = current_view.GetAllObjects(type_for_part);
                    while (enum_for_orientation.MoveNext())
                    {
                        TSD.Part mypart = enum_for_orientation.Current as TSD.Part;
                        mypart.Attributes.DrawConnectingSideMarks = true;
                        mypart.Modify();
                        current_view.Modify();
                        mydrg.CommitChanges();
                    }
                }
                else if (current_view.ViewType == TSD.View.ViewTypes.BottomView)
                {
                    TSD.DrawingObjectEnumerator enum_for_orientation = current_view.GetAllObjects(type_for_part);
                    while (enum_for_orientation.MoveNext())
                    {
                        TSD.Part mypart = enum_for_orientation.Current as TSD.Part;
                        mypart.Attributes.DrawConnectingSideMarks = true;
                        mypart.Modify();
                        current_view.Modify();
                    }
                }
                else if (current_view.ViewType == TSD.View.ViewTypes.SectionView)
                {
                    TSD.DrawingObjectEnumerator enum_for_orientation = current_view.GetAllObjects(type_for_part);
                    while (enum_for_orientation.MoveNext())
                    {
                        TSD.Part mypart = enum_for_orientation.Current as TSD.Part;
                        if (mypart != null)

                        {
                            if ((currentModel.SelectModelObject(mypart.ModelIdentifier) as TSM.Part).Identifier.GUID.ToString() != part.Identifier.GUID.ToString())
                            {
                                mypart.Attributes.DrawConnectingSideMarks = true;
                                mypart.Modify();
                                current_view.Modify();
                            }
                        }
                    }
                }
            }
            mydrg.CommitChanges();
        }


        public void Arrange_part_marks(TSD.AssemblyDrawing mydrg, TSD.StraightDimension ovr_dim, double actual_distance, TSM.Part mainpart, List<Guid> near_side_parts, List<Guid> far_side_parts, List<SectionLocationWithParts> list, List<Guid> top_partmark_to_delete, List<TSM.Part> list_of_parts_for_bottom_view_mark_retain)
        {

            List<Guid> BOTTOM_PART_MARK_TO_RETAIN = new List<Guid>();
            foreach (TSM.Part MYPARTTO_DLT in list_of_parts_for_bottom_view_mark_retain)
            {
                BOTTOM_PART_MARK_TO_RETAIN.Add(MYPARTTO_DLT.Identifier.GUID);
            }
            Type[] type_for_mark = new Type[] { typeof(TSD.Mark), typeof(TSD.WeldMark), typeof(TSD.AngleDimension), typeof(TSD.StraightDimensionSet) };

            TSD.DrawingObjectEnumerator enum_for_views = mydrg.GetSheet().GetAllViews();
            string mainpartmark = mainpart.GetPartMark();

            double length = 0;
            mainpart.GetReportProperty("LENGTH", ref length);

            TSM.Model model = new TSM.Model();



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
                        TSD.DrawingObjectEnumerator enumcheck = mymark.GetRelatedObjects();

                        while (enumcheck.MoveNext())
                        {
                            var mark_part = enumcheck.Current;
                            if (mark_part.GetType().Equals(typeof(TSD.Part)))
                            {
                                TSM.Part modelpart = new TSM.Model().SelectModelObject((mark_part as TSD.Part).ModelIdentifier) as TSM.Part;

                                Guid guid = modelpart.Identifier.GUID;

                                if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                                {
                                    //bool check3 = near_side_parts.Any(s => s == guid);



                                    //if (check3 == true)
                                    //{
                                    //    mark.Delete();
                                    //}
                                    //else
                                    //{
                                    //    bool check_3_dup = top_partmark_to_delete.Any(p => p == guid);
                                    //    bool CHECK4 = BOTTOM_PART_MARK_TO_RETAIN.Any(P => P == guid);
                                    //    if ((check_3_dup == true) || (CHECK4 == true))
                                    //    {
                                    //        mark.Delete();
                                    //    }


                                    //}
                                }

                                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                                {
                                    bool check4 = far_side_parts.Any(s => s == guid);

                                    if (check4 == true)
                                    {


                                        //mark.Delete();
                                    }
                                }
                                if (guid == mainpart.Identifier.GUID)
                                {
                                    //mymark.Attributes.PreferredPlacing = new TSD.PlacingTypes.AlongLinePlacing(new TSG.Point(0, 0, 0), new TSG.Point(length, 0, 0));
                                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                                    {
                                        //if (mainpart.Profile.ProfileString.Contains('W'))
                                        //{
                                        //    mymark.Attributes.PreferredPlacing = TSD.PreferredPlacingTypes.AlongLinePlacingType();
                                        //    mymark.Modify();


                                        //}
                                        //else
                                        //{ 
                                        //}
                                        //TSD.AlongLinePlacing newcheck = new TSD.AlongLinePlacing(new TSG.Point(0, 0, 0), new TSG.Point(length, 0, 0));
                                        //mymark.Attributes.PreferredPlacing = newcheck;
                                    }
                                    else
                                    {
                                        //mymark.Delete();
                                    }
                                }
                            }
                            else if (mark_part.GetType().Equals(typeof(TSD.Bolt)))
                            {
                                TSM.BoltGroup bolt = new TSM.Model().SelectModelObject((mark_part as TSD.Bolt).ModelIdentifier) as TSM.BoltGroup;
                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                                TSG.Vector X_VECTOR = bolt.GetCoordinateSystem().AxisX;
                                TSG.Vector Y_vector = bolt.GetCoordinateSystem().AxisY;
                                TSG.Point origin_for_bolt = bolt.GetCoordinateSystem().Origin;
                                Y_vector.Normalize();
                                TSG.Vector Z_Vector = X_VECTOR.Cross(Y_vector);
                                Z_Vector.Normalize();
                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                if (Convert.ToInt64(Z_Vector.Z) != 0)
                                {
                                    //double x_val = bolt.SlottedHoleX;
                                    //double y_val = bolt.SlottedHoleY;
                                    //TSD.SymbolInfo slotsymbol = null;
                                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                                    {
                                        if (origin_for_bolt.Z > 0)
                                        {

                                        }
                                        else
                                        {
                                            //mark.Delete();

                                        }
                                    }

                                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.SectionView))
                                    {
                                        //if ((x_val > 0) && (y_val == 0))
                                        //{
                                        //    slotsymbol = new TSD.SymbolInfo("xsteel", 74);
                                        //    TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(-50, 0, 0);
                                        //    TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                        //    newsymbol.Attributes.Height = 12.7;
                                        //    newsymbol.Insert();
                                        //}
                                        //if ((y_val > 0) && (x_val == 0))
                                        //{
                                        //    slotsymbol = new TSD.SymbolInfo("xsteel", 75);
                                        //    TSG.Point insertionpoint = mymark.InsertionPoint - new TSG.Point(0, 50, 0);
                                        //    TSD.Symbol newsymbol = new TSD.Symbol(current_view, insertionpoint, slotsymbol);
                                        //    newsymbol.Attributes.Height = 12.7;
                                        //    newsymbol.Insert();
                                        //}
                                    }
                                }
                                else
                                {
                                    mark.Delete();
                                }





                            }

                        }

                        //while (enumcheck2.MoveNext())
                        //{
                        //    var check11 = enumcheck.Current;
                        //}
                        try
                        {
                            TSD.Mark.MarkAttributes check = mymark.Attributes;
                            TSD.ContainerElement ele = check.Content;
                            IEnumerator check1 = ele.GetEnumerator();
                            while (check1.MoveNext())
                            {
                                var name = check1.Current;
                                if (name.GetType().Equals(typeof(TSD.TextElement)))
                                {
                                    TSD.TextElement check2 = check1.Current as TSD.TextElement;
                                    string value = check2.Value;
                                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                                    {
                                        if (value.Contains("~"))
                                        {
                                            mark.Delete();
                                        }

                                        //bool check3 = near_side_parts.Any(s => s.Contains(value));

                                        //if (check3 == true)
                                        //{
                                        //    mark.Delete();
                                        //}

                                    }
                                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                                    {
                                        //bool check4 = far_side_parts.Any(s => s.Contains(value));

                                        //if (check4 == true)
                                        //{
                                        //    mark.Delete();
                                        //}
                                    }

                                }
                                if (name.GetType().Equals(typeof(TSD.PropertyElement)))
                                {
                                    TSD.PropertyElement check2 = check1.Current as TSD.PropertyElement;
                                    string value = check2.Value;
                                    if ((value.Contains(mainpartmark)) && ((current_view.ViewType.Equals(TSD.View.ViewTypes.SectionView)) || ((current_view.ViewType.Equals(TSD.View.ViewTypes.EndView)))))
                                    {
                                        mark.Delete();
                                    }

                                }

                            }
                        }
                        catch
                        {

                        }
                        mymark.Attributes.PlacingAttributes.IsFixed = false;
                        if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                        {
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.BottomLeft = true;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.BottomRight = true;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.TopLeft = false;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.TopRight = false;
                        }

                        if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                        {
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.BottomLeft = false;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.BottomRight = false;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.TopLeft = true;
                            mymark.Attributes.PlacingAttributes.PlacingQuarter.TopRight = true;
                        }

                        mymark.Modify();
                    }
                    else if (mark.GetType().Equals(typeof(TSD.AngleDimension)))
                    {
                        TSD.AngleDimension angledim = mark as TSD.AngleDimension;
                        angledim.Attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Free;
                        angledim.Modify();


                    }
                    else if (mark.GetType().Equals(typeof(TSD.RadiusDimension)))
                    {
                        TSD.RadiusDimension angledim = mark as TSD.RadiusDimension;
                        angledim.Attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Free;
                        angledim.Modify();

                    }
                    else if (mark.GetType().Equals(typeof(TSD.StraightDimensionSet)))
                    {
                        TSD.StraightDimensionSet strdim = mark as TSD.StraightDimensionSet;
                        strdim.Attributes.Placing.Placing = DimensionSetBaseAttributes.Placings.Free;
                        strdim.Modify();

                    }
                    else if (mark.GetType().Equals(typeof(TSD.WeldMark)))
                    {
                        List<TSM.Part> list_of_parts = new List<TSM.Part>();
                        TSD.WeldMark weldmark = mark as TSD.WeldMark;
                        Identifier id = weldmark.ModelIdentifier;
                        TSM.BaseWeld weld = (new TSM.Model().SelectModelObject(id) as TSM.BaseWeld);
                        TSM.Part main_part = weld.MainObject as TSM.Part;
                        TSM.Part secondary_part = weld.SecondaryObject as TSM.Part;
                        list_of_parts.Add(main_part);
                        list_of_parts.Add(secondary_part);
                        bool check1 = list_of_parts.Any(p => SkTeklaDrawingUtility.get_report_properties(p, "PROFILE_TYPE") == "L");
                        bool check2 = (weld.TypeAbove.Equals(TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET) || (weld.TypeBelow.Equals(TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET)));






                        if (current_view.ViewType.Equals(TSD.View.ViewTypes.SectionView))
                        {
                            if ((check1 == true) && (check2 == true))
                            {
                                weldmark.Delete();
                            }
                            else
                            {

                            }
                        }
                        else if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                        {
                            if ((check1 == true) && (check2 == true))
                            {

                            }
                            else
                            {
                                weldmark.Delete();


                            }
                        }
                        else
                        {
                            weldmark.Delete();
                        }
























                    }



                }





            }

            mydrg.Modify();
        }

    }
}
