using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

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

namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    public class ConfigureViewPlacement
    {
        public ConfigureViewPlacement() { }


        public void view_placement(TSD.Drawing beam_assembly_drg, string drg_att, TSG.Point start_pt_for_section_view_aling, List<TSD.View> bottom_view_list, List<TSD.View> sectionviews_in_drawing, List<TSD.View> bottom_flange_cut_list, List<TSD.View> top_flange_cut_list)
        {
            //BOM_DETAILS.BOM_DETAILS bom = new BOM_DETAILS.BOM_DETAILS();
            //bom.constantRowHeight = .5;
            //bom.variableRowHeight 

            TSG.Point rbb_lower_left = new TSG.Point();
            TSG.Point rbb_lower_right = new TSG.Point();
            List<TSD.View> top_front = new List<TSD.View>();
            List<TSD.View> ALL_VIEWS = new List<TSD.View>();
            TSD.DrawingObjectEnumerator RBB_FOR_FRONT_VIEW = beam_assembly_drg.GetSheet().GetAllViews();

            while (RBB_FOR_FRONT_VIEW.MoveNext())
            {
                TSD.View MYVIEW = RBB_FOR_FRONT_VIEW.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    top_front.Add(MYVIEW);
                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    top_front.Add(MYVIEW);
                }
                ALL_VIEWS.Add(MYVIEW);

            }
            TSG.Point p_1 = new TSG.Point();

            for (int I = 0; I < top_front.Count; I++)
            {


                if (drg_att == "SK_BEAM_A3")
                {
                    if (ALL_VIEWS.Count < 2)
                    {
                        p_1 = new TSG.Point(35, -15, 0);
                    }
                    else
                    {
                        p_1 = new TSG.Point(0, -15, 0);
                    }
                }
                else if (drg_att == "SK_BEAM_A2")
                {
                    p_1 = new TSG.Point(35, -15, 0);
                    //p_1 = new TSG.Point(50, -75, 0);
                }
                else if (drg_att == "SK_BEAM_A1")
                {
                    //p_1 = new TSG.Point(50, -75, 0);
                }


                top_front[I].Modify();
                TSD.RectangleBoundingBox RBB = top_front[I].GetAxisAlignedBoundingBox();
                TSG.Point origin = top_front[I].Origin;
                TSG.Point RBB_lower_left = RBB.LowerLeft;
                TSG.Point RBB_lower_right = RBB.LowerRight;
                TSG.Vector REF_VECTOR = new TSG.Vector(top_front[I].Origin - RBB.UpperLeft);


                TSG.Point origin_point_for_front = origin + p_1;
                TSG.Point P3 = origin_point_for_front;
                top_front[I].Origin = P3;
                top_front[I].Modify();


            }
            RBB_FOR_FRONT_VIEW.Reset();

            while (RBB_FOR_FRONT_VIEW.MoveNext())
            {
                TSD.View MYVIEW = RBB_FOR_FRONT_VIEW.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    MYVIEW.Modify();
                    TSD.RectangleBoundingBox RBB = MYVIEW.GetAxisAlignedBoundingBox();

                    TSG.Point P1 = RBB.UpperRight;
                    TSG.Point P2 = RBB.LowerLeft;
                    rbb_lower_left = RBB.LowerLeft;
                    rbb_lower_right = RBB.LowerRight;

                    start_pt_for_section_view_aling = P2;
                    top_front.Add(MYVIEW);
                }

            }
            double length1 = TSG.Distance.PointToPoint(rbb_lower_left, rbb_lower_right);

            for (int I = 0; I < bottom_view_list.Count; I++)
            {
                bottom_view_list[I].Modify();
                TSD.RectangleBoundingBox RBB = bottom_view_list[I].GetAxisAlignedBoundingBox();
                TSG.Point RBB_lower_left = RBB.LowerLeft;
                TSG.Point RBB_lower_right = RBB.LowerRight;
                double length2 = TSG.Distance.PointToPoint(RBB_lower_left, RBB_lower_right);
                double difference = length1 - length2;
                TSG.Vector REF_VECTOR = new TSG.Vector(bottom_view_list[I].Origin - RBB.UpperLeft);
                TSG.Point P3 = start_pt_for_section_view_aling + REF_VECTOR;
                bottom_view_list[I].Origin = P3;
                bottom_view_list[I].Modify();
                bottom_view_list[I].Origin = P3 + new TSG.Point(difference / 2, 0, 0);
                bottom_view_list[I].Modify();
                TSD.RectangleBoundingBox RBB1 = bottom_view_list[I].GetAxisAlignedBoundingBox();
                start_pt_for_section_view_aling = RBB1.LowerLeft;

            }
            List<TSD.View> all_sectionviews_in_drawing = new List<TSD.View>();
            all_sectionviews_in_drawing = sectionviews_in_drawing;
            foreach (TSD.View my_view in bottom_flange_cut_list)
            {
                all_sectionviews_in_drawing.Add(my_view);

            }
            foreach (TSD.View my_view in top_flange_cut_list)
            {
                all_sectionviews_in_drawing.Add(my_view);

            }

            ArrayList HEIGHT_TO_FIX = new ArrayList();

            foreach (TSD.View MYVIEW in all_sectionviews_in_drawing)
            {
                MYVIEW.Modify();
                TSD.RectangleBoundingBox RBB = MYVIEW.GetAxisAlignedBoundingBox();
                HEIGHT_TO_FIX.Add(RBB.Height);
            }
            HEIGHT_TO_FIX.Sort();
            double HEIGHT = 0;
            if (HEIGHT_TO_FIX.Count > 0)
            {
                HEIGHT = Convert.ToDouble(HEIGHT_TO_FIX[HEIGHT_TO_FIX.Count - 1]);
            }

            for (int I = 0; I < all_sectionviews_in_drawing.Count; I++)
            {
                if (all_sectionviews_in_drawing[I] == all_sectionviews_in_drawing.First<TSD.ViewBase>())
                {
                    all_sectionviews_in_drawing[I].Modify();
                    TSD.RectangleBoundingBox RBB = all_sectionviews_in_drawing[I].GetAxisAlignedBoundingBox();
                    TSG.Vector REF_VECTOR = new TSG.Vector(all_sectionviews_in_drawing[I].Origin - RBB.LowerLeft);

                    //TSG.Point TEST = new TSG.Point(start_pt_for_section_view_aling.X, start_pt_for_section_view_aling.Y + HEIGHT, start_pt_for_section_view_aling.Z);

                    TSG.Point P3 = start_pt_for_section_view_aling + REF_VECTOR;
                    all_sectionviews_in_drawing[I].Origin = P3 - new TSG.Point(0, HEIGHT, 0);
                    all_sectionviews_in_drawing[I].Modify();
                }
                else
                {
                    all_sectionviews_in_drawing[I].Modify();
                    TSD.RectangleBoundingBox RBB = all_sectionviews_in_drawing[I].GetAxisAlignedBoundingBox();
                    TSD.RectangleBoundingBox PREVIOUS_RBB = all_sectionviews_in_drawing[I - 1].GetAxisAlignedBoundingBox();
                    TSG.Vector REF_VECTOR = new TSG.Vector(all_sectionviews_in_drawing[I].Origin - RBB.LowerLeft);
                    start_pt_for_section_view_aling = PREVIOUS_RBB.LowerRight;
                    TSG.Point P3 = start_pt_for_section_view_aling + REF_VECTOR;
                    all_sectionviews_in_drawing[I].Origin = P3;
                    all_sectionviews_in_drawing[I].Modify();
                }

            }

        }

    }
}
