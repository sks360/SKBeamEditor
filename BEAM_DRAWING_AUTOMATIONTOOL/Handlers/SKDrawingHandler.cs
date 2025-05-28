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
using System.Collections;
using SK.Tekla.Drawing.Automation.Support;
namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKDrawingHandler
    {
        private bool KnockOffDimension = true; //TODO VEERA this must be fetched from form


        private readonly BoundingBoxHandler boundingBoxHandler;

        private readonly SKCatalogHandler catalogHandler;

        public SKDrawingHandler(BoundingBoxHandler boundingBoxHandler,
            SKCatalogHandler catalogHandler)
        {
            this.boundingBoxHandler = boundingBoxHandler;
            this.catalogHandler = catalogHandler;

        }

        public void Drawing_create_and_delete_all_dimensions_except_assembly_dim(TSM.Model mymodel,
            TSM.ModelObject currentBeam, string drg_attribute,
            out TSD.AssemblyDrawing ASSEMBLY_DRAWING,
            out TSM.Part main_part, out double output, out TSM.Assembly ASSEMBLY,
            out TSD.StraightDimension overall_dim, out double DIM_DISTANCE,
            out double ACTUAL_DIS, out List<section_loc_with_parts> list2, double SCALE,
            double MINI_LENGTH, out List<section_loc_with_parts> list_for_flange_section,
            out List<section_loc_with_parts> list_for_flange_section_for_duplicate,
            out List<TSM.Part> list_of_parts_for_bottom_part_mark_retain,
            out List<Guid> list_of_guid_in_top_view_to_delete,
            out TSD.StraightDimension OVERALL_DIMENSION,
            out List<TSD.RadiusDimension> list_of_radius_dim)
        {
            /////////////Drawing creation and insert//////////////////////////////////////////////


            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            ASSEMBLY_DRAWING = null;
            ASSEMBLY = null;
            main_part = null;
            DIM_DISTANCE = 0;
            ACTUAL_DIS = 0;
            output = 0;
            overall_dim = null;
            ASSEMBLY = currentBeam as TSM.Assembly;
            OVERALL_DIMENSION = null;
            main_part = (ASSEMBLY.GetMainPart()) as TSM.Beam;

            list2 = new List<section_loc_with_parts>();
            list_for_flange_section = new List<section_loc_with_parts>();
            list_for_flange_section_for_duplicate = new List<section_loc_with_parts>();
            list_of_parts_for_bottom_part_mark_retain = new List<TSM.Part>();
            list_of_guid_in_top_view_to_delete = new List<Guid>();
            list_of_radius_dim = new List<TSD.RadiusDimension>();

            //////////////////////////////Getting workpoints of assembly from bounding box/////////////////////
            output = 0;
            main_part.GetReportProperty("LENGTH", ref output);



            ASSEMBLY_DRAWING = new TSD.AssemblyDrawing(ASSEMBLY.Identifier, drg_attribute);



            ASSEMBLY_DRAWING.Insert();
            drg_handler.SetActiveDrawing(ASSEMBLY_DRAWING, true);



            TSD.DrawingObjectEnumerator enum_for_drg_views_del_1 = ASSEMBLY_DRAWING.GetSheet().GetAllViews();
            while (enum_for_drg_views_del_1.MoveNext())
            {


                TSD.View current_view = enum_for_drg_views_del_1.Current as TSD.View;
                current_view.Attributes.Scale = SCALE;

                current_view.Attributes.Shortening.MinimumLength = MINI_LENGTH;
                current_view.Attributes.Shortening.CutPartType = TSD.View.ShorteningCutPartType.X_Direction;

                current_view.Modify();
                ASSEMBLY_DRAWING.Modify();


            }



            TSD.DrawingObjectEnumerator enum_for_drg_views_del = ASSEMBLY_DRAWING.GetSheet().GetAllViews();

            TSD.PointList ASSEMBLY_BOUNDING_BOX = boundingBoxHandler.bounding_box_FOR_DIM(ASSEMBLY);

            TSG.Point workpointst_1 = ASSEMBLY_BOUNDING_BOX[0];
            TSG.Point workpointend_1 = ASSEMBLY_BOUNDING_BOX[1];
            Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet),
                typeof(TSD.AngleDimension) };
            List<DIMENSION_WITH_DIFFERNCE> MYDIM_WITH_DIFFER = new List<DIMENSION_WITH_DIFFERNCE>();
            List<DIMENSION_WITH_DIFFERNCE> MYDIM_WITH_DIFFER_ORIGINAL = new List<DIMENSION_WITH_DIFFERNCE>();
            //////////////////Enumerating different views/////////////////////////////
            while (enum_for_drg_views_del.MoveNext())
            {


                TSD.View current_view = enum_for_drg_views_del.Current as TSD.View;

                TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(type_for_dim);
                #region front_view_dimension_delete

                while (dim_drg.MoveNext())
                {
                    var dim_del = dim_drg.Current;
                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                    {

                        TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
                        TSG.Point workpointst2 = toviewmatrix.Transform(workpointst_1);

                        TSG.Point workpointend2 = toviewmatrix.Transform(workpointend_1);
                        if (dim_del.GetType().Equals(typeof(TSD.StraightDimension)))
                        {
                            TSD.StraightDimension DIM = dim_del as TSD.StraightDimension;
                            double X = DIM.StartPoint.X;
                            double X1 = DIM.EndPoint.X;
                            double DIFFERENCE = X1 - X;
                            TSG.Vector MYVECTOR = DIM.UpDirection;

                            MYDIM_WITH_DIFFER.Add(new DIMENSION_WITH_DIFFERNCE { MTDIM = DIM, DIFFER = DIFFERENCE, MYVECTOR = MYVECTOR });
                            MYDIM_WITH_DIFFER_ORIGINAL.Add(new DIMENSION_WITH_DIFFERNCE { MTDIM = DIM, DIFFER = DIFFERENCE, MYVECTOR = MYVECTOR });


                        }
                        else if (dim_del.GetType().Equals(typeof(TSD.StraightDimensionSet)))
                        {
                            TSD.StraightDimensionSet RD = dim_del as TSD.StraightDimensionSet;

                            if (RD.Attributes.DimensionType == TSD.DimensionSetBaseAttributes.DimensionTypes.Elevation)
                            {
                                dim_del.Delete();
                            }
                        }
                        else if (dim_del.GetType().Equals(typeof(TSD.AngleDimension)))
                        {
                            dim_del.Delete();
                        }
                        else
                        {
                            dim_del.Delete();
                        }

                    }
                    else if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                    {
                        if (!dim_del.GetType().Equals(typeof(TSD.AngleDimension)))
                        { 
                            dim_del.Delete();
                        }
                    }
                    else
                    {
                        dim_del.Delete();
                    }

                }


                #endregion


                MYDIM_WITH_DIFFER.RemoveAll(X => X.MYVECTOR.Y > 0);

                List<DIMENSION_WITH_DIFFERNCE> OVERALL_DIM = MYDIM_WITH_DIFFER.Where((X => (X.DIFFER.Equals(MYDIM_WITH_DIFFER.Max(Y => Y.DIFFER))))).ToList();

                //List<DIMENSION_WITH_DIFFERNCE> IO =        MYDIM_WITH_DIFFER.Where(X =>! X.DIFFER.Equals(MYDIM_WITH_DIFFER.Max(Y => Y.DIFFER))).ToList();
                List<DIMENSION_WITH_DIFFERNCE> IO = MYDIM_WITH_DIFFER_ORIGINAL.Where(X => !X.DIFFER.Equals(OVERALL_DIM[0].DIFFER)).ToList();

                if (KnockOffDimension)
                {
                    IO.RemoveAll(X => KNOCKOFF_DIM(X.MTDIM) == true);
                }
                //COMMENTED BY VEERA
                //if (chkknockoffdim.Checked == true)
                //{
                //    IO.RemoveAll(X => KNOCKOFF_DIM(X.MTDIM) == true);
                //}

                foreach (var MYDIM in IO)
                {

                    MYDIM.MTDIM.Delete();

                }
                foreach (var MYDIM in OVERALL_DIM)
                {
                    OVERALL_DIMENSION = MYDIM.MTDIM;
                }
            }
            enum_for_drg_views_del.Reset();
            Type[] radius_dim = new Type[] { typeof(TSD.RadiusDimension) };
            while (enum_for_drg_views_del.MoveNext())
            {
                TSD.View current_view = enum_for_drg_views_del.Current as TSD.View;
                TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(radius_dim);
                while (dim_drg.MoveNext())
                {
                    var dim_del = dim_drg.Current;
                    if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                    {
                        if (dim_del.GetType().Equals(typeof(TSD.RadiusDimension)))
                        {
                            TSD.RadiusDimension dimension = dim_del as TSD.RadiusDimension;
                            list_of_radius_dim.Add(dimension);
                        }
                    }
                }
            }
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(main_part.GetCoordinateSystem()));
            ArrayList list_of = new ArrayList();
            TSD.DrawingObjectEnumerator part_enum_for_section = ASSEMBLY_DRAWING.GetSheet().GetAllViews();
            while (part_enum_for_section.MoveNext())
            {
                TSD.View current_view = part_enum_for_section.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    TSD.DrawingObjectEnumerator mypart_en = current_view.GetAllObjects(typeof(TSD.Part));
                    while (mypart_en.MoveNext())
                    {
                        TSD.Part mypart = mypart_en.Current as TSD.Part;

                        TSM.ModelObject mymodel_obj = new TSM.Model().SelectModelObject(mypart.ModelIdentifier);
                        TSM.Part mypart_for_sect = mymodel_obj as TSM.Part;
                        if (!mypart_for_sect.Identifier.ID.Equals(main_part.Identifier.ID))
                        {
                            if (mypart_for_sect.Name.Contains("FALLTECH") == false)
                            {
                                list_of.Add(mypart_for_sect);
                            }
                        }
                    }
                }
            }


            ArrayList list_of_obj = new ArrayList();
            foreach (TSM.Part mypl in list_of)
            {

                TSD.PointList ptlist = boundingBoxHandler.bounding_box_sort_x(mypl, main_part as TSM.Beam);
                double distance = Convert.ToInt16(ptlist[0].Z) + Convert.ToInt16(ptlist[1].Z);
                TSG.Matrix TO_VIEW_MATRIX = TSG.MatrixFactory.ToCoordinateSystem(main_part.GetCoordinateSystem());
                TSG.Point P1 = TO_VIEW_MATRIX.Transform((main_part as TSM.Beam).StartPoint);
                TSG.Point P2 = TO_VIEW_MATRIX.Transform((main_part as TSM.Beam).EndPoint);

                if (distance != 0)
                {
                    list_of_obj.Add(mypl);
                }

                else if (distance == 0)
                {
                    double plt_condition_start = (P1.X + 200);
                    double plt_condition_end = (P2.X - 200);

                    if ((ptlist[1].X) < (plt_condition_start))
                    {
                        list_of_obj.Add(mypl);

                        string s = "";
                        mypl.GetReportProperty("PROFILE_TYPE", ref s);


                        string name = "(CTRD)";
                        mypl.SetUserProperty("USERDEFINED.NOTES7", name);

                    }
                    else if ((ptlist[0].X) > Convert.ToInt32(plt_condition_end))
                    {
                        list_of_obj.Add(mypl);
                        string s = "";
                        mypl.GetReportProperty("PROFILE_TYPE", ref s);


                        string name = "(CTRD)";
                        mypl.SetUserProperty("USERDEFINED.NOTES7", name);

                    }
                    else
                    {
                        list_of_obj.Add(mypl);
                        string s = "";
                        mypl.GetReportProperty("PROFILE_TYPE", ref s);
                        string name = "(CTRD)";
                        mypl.SetUserProperty("USERDEFINED.NOTES7", name);

                    }
                }

            }

            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            List<double> mainpart_values = catalogHandler.Getcatalog_values(main_part);

            double heightpos_neg = mainpart_values[0] / 2;
            TSD.View currentview = enum_for_drg_views_del.Current as TSD.View;
            TSD.DrawingObjectEnumerator enum_for_drg_views_del1 = ASSEMBLY_DRAWING.GetSheet().GetAllViews();
            List<List<int>> MYLIST = new List<List<int>>();
            List<req_pts> mypoints1 = new List<req_pts>();
            List<req_pts> mypoints1_duplicate = new List<req_pts>();
            List<req_pts> mypoints_duplicate_for_dimension = new List<req_pts>();
            List<int> list_ide = new List<int>();

            //int while_start = Environment.TickCount;
            while (enum_for_drg_views_del1.MoveNext())
            {


                TSD.View current_view = enum_for_drg_views_del1.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    ArrayList list_of_x = new ArrayList();
                    List<req_pts> mypoints = new List<req_pts>();
                    ArrayList list_of_points = new ArrayList();
                    foreach (TSM.Part myplaye in list_of_obj)
                    {
                        TSD.PointList ptlist = boundingBoxHandler.bounding_box_sort_x(myplaye, main_part as TSM.Beam);


                        TSD.PointList m1 = converted_points(ptlist, main_part as TSM.Beam, current_view);


                        if (((ptlist[0].Y > -heightpos_neg) && (ptlist[1].Y < heightpos_neg)) || ((ptlist[0].Y < -heightpos_neg) && (ptlist[1].Y > heightpos_neg)))
                        {

                            double distanceofx = ((m1[0].X + m1[1].X) / 2);
                            double distanceofy = ((m1[0].Y + m1[1].Y) / 2);
                            double distanceofZ = ((m1[0].Z + m1[1].Z) / 2);


                            TSM.Part mypart = myplaye;
                            string PARTMARK = get_report_properties(myplaye, "PART_POS");
                            mypoints.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            mypoints_duplicate_for_dimension.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            if (((Convert.ToInt64(m1[0].X) < 0) || (Convert.ToInt64(m1[1].X) < 0)) || ((Convert.ToInt64(m1[0].X) > output) || (Convert.ToInt64(m1[1].X) > output)))
                            {
                                list_of_guid_in_top_view_to_delete.Add(mypart.Identifier.GUID);
                            }

                        }
                        else
                        {
                            double distanceofx = ((m1[0].X + m1[1].X) / 2);
                            double distanceofy = ((m1[0].Y + m1[1].Y) / 2);
                            double distanceofZ = ((m1[0].Z + m1[1].Z) / 2);


                            TSM.Part mypart = myplaye;
                            string PARTMARK = get_report_properties(myplaye, "PART_POS");
                            list_ide.Add(mypart.Identifier.ID);
                            mypoints1.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            mypoints1_duplicate.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });

                            mypoints_duplicate_for_dimension.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            if ((Convert.ToInt64(ptlist[0].Y) <= -Convert.ToInt64(heightpos_neg)) && (Convert.ToInt64(ptlist[1].Y) <= -Convert.ToInt64(heightpos_neg)))
                            {
                                list_of_parts_for_bottom_part_mark_retain.Add(mypart);
                            }

                            //(((Convert.ToInt64(m1[0].Y) < -heightpos_neg)&&(Convert.ToInt64(m1[0].Y) < 0))&&((Convert.ToInt64(m1[1].Y) < -heightpos_neg)&&(Convert.ToInt64(m1[1].Y) < 0)))
                            if (((Convert.ToInt64(m1[0].X) < 0) || (Convert.ToInt64(m1[1].X) < 0)) || ((Convert.ToInt64(m1[0].X) > output) || (Convert.ToInt64(m1[1].X) > output)))
                            {
                                list_of_guid_in_top_view_to_delete.Add(mypart.Identifier.GUID);
                            }


                        }
                    }

                    if (mypoints1.Count > 0)
                    {
                        double change_min = Math.Abs(current_view.RestrictionBox.MinPoint.Y);
                        double change_max = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);
                        if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                        {
                            current_view.RestrictionBox.MaxPoint.Y = change_min;
                            current_view.Modify();

                        }
                        else
                        {
                            current_view.RestrictionBox.MinPoint.Y = -change_max;
                            current_view.Modify();

                        }
                    }
                    else
                    {

                        double change_min = Math.Abs(current_view.RestrictionBox.MinPoint.Y);
                        double change_max = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);

                        current_view.RestrictionBox.MaxPoint.Y = change_max + 65;



                        current_view.RestrictionBox.MinPoint.Y = -change_min - 65;
                        current_view.Modify();



                    }


                    mypoints = mypoints.OrderBy(x => x.distance).ToList();



                    List<req_pts> final_distance = new List<req_pts>();

                    for (int i = 0; i < mypoints.Count; i++)
                    {

                        if (i == Convert.ToInt16(mypoints.Count - 1))
                        {
                            final_distance.Add(mypoints[i]);

                        }
                        else
                        {
                            double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                            //if (ditsnace > 25)
                            if (ditsnace > 125)
                            {
                                final_distance.Add(mypoints[i]);

                            }

                            else
                            {
                                if (mypoints[i].distance != mypoints[i + 1].distance)
                                {
                                    if (mypoints[i].distance_for_y > mypoints[i + 1].distance_for_y)
                                    {
                                        final_distance.Add(mypoints[i]);
                                    }
                                    else
                                    {
                                        final_distance.Add(mypoints[i + 1]);
                                        //final_distance.Add(mypoints[i ]);
                                    }
                                }
                            }
                        }
                    }
                    //for (int i = 0; i < mypoints1.Count; i++)
                    //{

                    //    if (i == Convert.ToInt16(mypoints1.Count - 1))
                    //    {
                    //        final_distance.Add(mypoints1[i]);

                    //    }
                    //    else
                    //    {
                    //        double ditsnace = (Convert.ToInt16(mypoints1[i + 1].distance) - Convert.ToInt16(mypoints1[i].distance));
                    //        //if (ditsnace > 25)
                    //        if (ditsnace > 125)
                    //        {
                    //            final_distance.Add(mypoints1[i]);

                    //        }

                    //        else
                    //        {
                    //            if (mypoints1[i].distance != mypoints1[i + 1].distance)
                    //            {
                    //                if (mypoints1[i].distance_for_y > mypoints1[i + 1].distance_for_y)
                    //                {
                    //                    final_distance.Add(mypoints1[i]);
                    //                }
                    //                else
                    //                {
                    //                    final_distance.Add(mypoints1[i + 1]);
                    //                    //final_distance.Add(mypoints[i ]);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    TSM.Part main = main_part;

                    List<double> final_distance_UNIQUE = new List<double>();


                    section_loc_with_parts obj1 = new section_loc_with_parts();
                    List<TSM.Part> list1 = new List<TSM.Part>();


                    for (int i = 0; i < mypoints.Count; i++)
                    {

                        if (i == Convert.ToInt16(mypoints.Count - 1))
                        {
                            final_distance_UNIQUE.Add(mypoints[i].distance);
                            list1.Add(mypoints[i].part);
                            list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });

                        }
                        else
                        {
                            double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                            //if (ditsnace > 25)
                            if (ditsnace > 125)
                            {

                                list1.Add(mypoints[i].part);
                                list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });
                                list1 = new List<TSM.Part>();
                            }

                            else
                            {
                                list1.Add(mypoints[i].part);

                            }
                        }
                    }


                    //for (int i = 0; i < mypoints1.Count; i++)
                    //{

                    //    if (i == Convert.ToInt16(mypoints1.Count - 1))
                    //    {
                    //        final_distance_UNIQUE.Add(mypoints1[i].distance);
                    //        list1.Add(mypoints1[i].part);
                    //        list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints1[i].distance });

                    //    }
                    //    else
                    //    {
                    //        double ditsnace = (Convert.ToInt16(mypoints1[i + 1].distance) - Convert.ToInt16(mypoints1[i].distance));
                    //        //if (ditsnace > 25)
                    //        if (ditsnace > 125)
                    //        {

                    //            list1.Add(mypoints1[i].part);
                    //            list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints1[i].distance });
                    //            list1 = new List<TSM.Part>();
                    //        }

                    //        else
                    //        {
                    //            list1.Add(mypoints1[i].part);

                    //        }
                    //    }
                    //}



                    List<TSM.Part> final_list = new List<TSM.Part>();
                    List<section_loc_with_parts> f1 = new List<section_loc_with_parts>();
                    List<section_loc_with_parts> f2 = new List<section_loc_with_parts>();


                    List<section_loc_with_parts> FINAL = list2.GroupBy(X => X.partlist.Count).Select(Y => Y.FirstOrDefault()).ToList();
                    List<string> final_check_for_unique = new List<string>();
                    for (int i = list2.Count - 1; i >= 0; i--)
                    {
                        if (i == 0)
                        {
                            list2[i].sectionview_needed = "YES";
                        }
                        else
                        {

                            List<string> check_for_unique = new List<string>();
                            for (int j = i - 1; j >= 0; j--)
                            {


                                var first_loop = list2[i].partlist;
                                var second_loop = list2[j].partlist;

                                if (!(first_loop.Count == second_loop.Count))
                                {
                                    check_for_unique.Add("UNIQUE");

                                }
                                else
                                {
                                    bool result = FUNCTION_FOR_COMPARING_PARTMARKS_AND_ORIENTATION(first_loop, second_loop, main_part as TSM.Beam);

                                    if (result == true)
                                    {

                                        check_for_unique.Add("SAME");
                                    }
                                    else
                                    {
                                        check_for_unique.Add("UNIQUE");
                                    }

                                }
                            }

                            if (!check_for_unique.Contains("SAME"))
                            {
                                list2[i].sectionview_needed = "YES";
                            }
                            else
                            {
                                list2[i].sectionview_needed = "NO";
                                int check = check_for_unique.LastIndexOf("SAME");
                                int check2 = check_for_unique.Count - (check + 1);
                                list2[i].index_of_same_sec = check2;
                            }

                        }
                    }





                    list2 = list2.OrderBy(x => x.distance).ToList();
                    List<section_loc_with_parts> section = new List<section_loc_with_parts>();
                    List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();

                    for (int i = 0; i < list2.Count; i++)
                    {
                        if ((list2[i].sectionview_needed == "YES"))
                        {
                            //if (list2[i].partlist.Count > 1)
                            //{
                            //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                            //}
                            double minx = 0;
                            double maxx = 0;
                            double mny = 0;
                            double mxy = 0;

                            minx = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].X);
                            maxx = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].X);
                            mny = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].Y);
                            mxy = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].Y);

                            double miny = Convert.ToInt64(mny);
                            double maxy = Convert.ToInt64(mxy);

                            double distanceofx = ((minx + maxx) / 2);
                            double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


                            double distance_of_y = ((miny + maxy) / 2);
                            double height = mainpart_values[0];
                            double height_for_view = 0;
                            double height_for_view1 = 0;

                            double POSIH = Convert.ToInt64(height / 2);
                            double NEGAH = -Convert.ToInt64(height / 2);



                            if ((maxy <= POSIH) && (miny >= NEGAH))
                            {
                                height_for_view = -((height / 2));
                                height_for_view1 = (height / 2);
                            }
                            else if ((maxy >= POSIH) && (miny >= POSIH))
                            {
                                height_for_view = (height / 2);
                                height_for_view1 = maxy;

                            }
                            else if ((maxy <= NEGAH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = (height / 2);

                            }
                            else if ((maxy >= POSIH) && (miny >= NEGAH))
                            {
                                height_for_view = -((height / 2));
                                height_for_view1 = maxy;
                            }

                            else if ((maxy <= POSIH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = (height / 2);
                            }
                            else if ((maxy >= POSIH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = maxy;
                            }




                            TSD.View bottom_view = null;
                            TSD.SectionMark sec = null;
                            double distancefor_depthup = 0;
                            double distancefor_depthdown = 0;
                            if (DISTANCE_TO_COMPARE < 300)
                            {
                                distancefor_depthup = Math.Abs(minx - distanceofx);
                                distancefor_depthdown = Math.Abs(maxx - distanceofx);
                            }
                            else if (DISTANCE_TO_COMPARE > 300)
                            {
                                distancefor_depthup = 0;
                                distancefor_depthdown = 0;

                            }

                            //if (height_for_view > 0)
                            //{



                            //    height_for_view = height_for_view + 25.4;

                            //}


                            //else
                            //{
                            //    height_for_view = height_for_view - 25.4;
                            //}

                            //if (height_for_view1 > 0)
                            //{
                            //    if (height_for_view1 > POSIH )
                            //    {
                            //        height_for_view1 = height_for_view1;

                            //    }
                            //    else
                            //    {
                            //        height_for_view1 = height_for_view1 + 25.4;
                            //    }
                            //}
                            //else
                            //{
                            //    height_for_view1 = height_for_view1 - 25.4;
                            //}

                            //TSG.Point P1 = new TSG.Point(distanceofx, height_for_view - 1, 0);
                            //TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1 + 1, 0);
                            height_for_view = current_view.RestrictionBox.MaxPoint.Y;
                            height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
                            TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
                            TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);
                            double dep_up = maxx - distanceofx;
                            double dep_down = distanceofx - minx;
                            if (dep_up > 100)
                            {
                                dep_up = 5;
                            }
                            else
                            {
                            }
                            if (dep_down > 100)
                            {
                                dep_down = 5;
                            }
                            else
                            {
                            }


                            try
                            {
                                if (drg_attribute == "SK_BEAM_A1")
                                {
                                    bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                                    bottom_view.Modify();
                                }
                                else
                                {
                                    bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                                    bottom_view.Modify();

                                }

                                //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);


                                //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);

                                double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.X);
                                double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.X);
                                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                                {
                                    bottom_view.RestrictionBox.MaxPoint.X = change_min;
                                    bottom_view.Modify();

                                }
                                else
                                {
                                    bottom_view.RestrictionBox.MinPoint.X = -change_max;
                                    bottom_view.Modify();

                                }


                                bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
                                bottom_view.Modify();


                                TSD.FontAttributes FONT = new TSD.FontAttributes();
                                FONT.Color = TSD.DrawingColors.Magenta;
                                FONT.Height = Convert.ToInt16(3.96875);




                                //TSD.PropertyElement VIEW_LABEL = new TSD.PropertyElement(TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName);

                                TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
                                TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
                                X.Font.Color = TSD.DrawingColors.Magenta;
                                X.Font.Height = Convert.ToInt64(3.96875);

                                //TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                                //TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };

                                TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };


                                //sec.Attributes.TagsAttributes



                                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

                                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

                                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

                                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                                bottom_view.Attributes.Scale.Equals(current_view.Attributes.Scale);

                                bottom_view.Modify();
                                //  sec.Modify();
                                list2[i].myview = bottom_view;
                                sectionmarklist.Add(sec);
                                TSD.DrawingObjectEnumerator BOTTOM = bottom_view.GetAllObjects(typeof(TSD.Part));
                                List<int> list_req = new List<int>();
                                List<TSM.Part> mypart_list_for_section = new List<TSM.Part>();
                                while (BOTTOM.MoveNext())
                                {
                                    TSD.Part MYDRG_PART = BOTTOM.Current as TSD.Part;
                                    TSM.ModelObject MMODEL = new TSM.Model().SelectModelObject(MYDRG_PART.ModelIdentifier);
                                    TSD.PointList bounding_box_z = boundingBoxHandler.bounding_box_sort_z(MMODEL, bottom_view);

                                    if ((Convert.ToInt64(bounding_box_z[1].Z) >= Convert.ToInt64(bottom_view.RestrictionBox.MinPoint.Z)) && (Convert.ToInt64(bounding_box_z[0].Z) <= Convert.ToInt64(bottom_view.RestrictionBox.MaxPoint.Z)))
                                    {
                                        TSM.Part mmpart = MMODEL as TSM.Part;
                                        if (!mmpart.Identifier.ID.Equals(main_part.Identifier.ID))
                                        {
                                            list_req.Add(mmpart.Identifier.ID);
                                            //try
                                            //{
                                            //list2[i].req_partlist.Add(mmpart);
                                            mypart_list_for_section.Add(mmpart);

                                            //}
                                            //catch
                                            //{
                                            //}
                                            //MYLIST.Add(mmpart.Identifier.ID);
                                        }

                                    }


                                }
                                list2[i].req_partlist = mypart_list_for_section;
                                MYLIST.Add(list_req);
                            }
                            catch
                            {
                            }

                        }

                        else
                        {

                            TSD.SectionMark sec_dummy = null;
                            sectionmarklist.Add(sec_dummy);
                            TSD.SectionMark mysec = sectionmarklist[list2[i].index_of_same_sec];

                            mysec.LeftPoint.X = list2[i].distance;
                            mysec.RightPoint.X = list2[i].distance;
                            mysec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                            mysec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;






                            mysec.Insert();

                        }






                    }



                }
                ASSEMBLY_DRAWING.PlaceViews();





            }
            //int end_while = Environment.TickCount;
            //Console.WriteLine("Time elapsed for outer while ---> " + (end_while-while_start));

            TSD.DrawingObjectEnumerator enum_for_flange_sect = ASSEMBLY_DRAWING.GetSheet().GetAllViews();
            while (enum_for_flange_sect.MoveNext())
            {


                TSD.View current_view = enum_for_flange_sect.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    List<List<int>> final_result = new List<List<int>>();
                    foreach (List<int> myllist in MYLIST)
                    {

                        List<int> dup_list = myllist.Intersect(list_ide).ToList();
                        final_result.Add(dup_list);



                    }


                    foreach (List<int> section_list in final_result)
                    {
                        for (int p = 0; p < mypoints1.Count; p++)
                        {
                            bool result = section_list.Any(x => x.Equals(mypoints1[p].part.Identifier.ID));
                            if (result == true)
                            {
                                mypoints1_duplicate.RemoveAll(x => x.part.Identifier.ID.Equals(mypoints1[p].part.Identifier.ID));
                            }
                        }

                    }
                    if (mypoints1_duplicate.Count > 0)
                    {



                        SEC_VIEW_COMPARE(mypoints1_duplicate, current_view, main_part, MYLIST, ASSEMBLY_DRAWING, out list_for_flange_section, drg_attribute);

                    }

                    else
                    {
                        list_for_flange_section = new List<section_loc_with_parts>();


                    }
                }
            }
            ASSEMBLY_DRAWING.CommitChanges();
        }
     
        private bool KNOCKOFF_DIM(TSD.StraightDimension MYDIM)
        {
            bool RESULT = false;



            TSD.ContainerElement dimval = MYDIM.Value;



            IEnumerator check1 = dimval.GetEnumerator();

            List<string> list_of_texts = new List<string>();
            while (check1.MoveNext())
            {
                var name = check1.Current;
                if (name.GetType().Equals(typeof(TSD.TextElement)))
                {
                    TSD.TextElement check2 = check1.Current as TSD.TextElement;
                    string value = check2.Value;

                    list_of_texts.Add(value);


                }
            }



            bool checkFOR_BRACKET = list_of_texts.Any(x => x.Contains("("));

            if (checkFOR_BRACKET == true)
            {
                RESULT = true;
            }
            else
            {
                RESULT = false;
            }
            return RESULT;
        }

        public bool FUNCTION_FOR_COMPARING_PARTMARKS_AND_ORIENTATION(List<TSM.Part> LIST1, List<TSM.Part> LIST2, TSM.Beam MAINPART)
        {
            TSM.Model MODEL = new TSM.Model();
            bool result;
            MODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(MAINPART.GetCoordinateSystem()));
            List<string> CHECK_FOR_SAME_SECTION = new List<string>();
            foreach (TSM.Part MYPART in LIST1)
            {
                string CHECK_FOR_SAME_PART = "";
                List<TSM.Part> LISTOFPARTS = new List<TSM.Part>();
                LISTOFPARTS.Add(MYPART);
                foreach (TSM.Part MYPART1 in LIST2)
                {

                    string mypartMARK_FOR_FIRST = get_report_properties(MYPART, "PART_POS");
                    string mypartMARK_FOR_SECOND = get_report_properties(MYPART1, "PART_POS");
                    if (mypartMARK_FOR_FIRST == mypartMARK_FOR_SECOND)
                    {
                        TSG.CoordinateSystem COORD1 = MYPART.GetCoordinateSystem();
                        TSG.CoordinateSystem COORD2 = MYPART1.GetCoordinateSystem();

                        if ((Convert.ToInt64(COORD1.Origin.Y) == Convert.ToInt64(COORD2.Origin.Y)) && (Convert.ToInt64(COORD1.Origin.Z) == Convert.ToInt64(COORD2.Origin.Z)))
                        {
                            if ((function_for_vector_check(COORD1.AxisX, COORD2.AxisX) && (function_for_vector_check(COORD1.AxisY, COORD2.AxisY))))
                            {
                                CHECK_FOR_SAME_PART = "SAME_PART";
                                break;
                            }

                        }

                    }


                }

                if (CHECK_FOR_SAME_PART == "SAME_PART")
                {
                    CHECK_FOR_SAME_SECTION.Add("SAME_SECTION");
                }

            }

            if (CHECK_FOR_SAME_SECTION.Count == LIST1.Count)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            MODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            return result;
        }
        public bool function_for_vector_check(TSG.Vector vector1, TSG.Vector vector2)
        {
            bool result;
            vector1.Normalize();
            vector2.Normalize();
            if ((Convert.ToInt64(vector1.X) == Convert.ToInt64(vector2.X)) && (Convert.ToInt64(vector1.Y) == Convert.ToInt64(vector2.Y)) && (Convert.ToInt64(vector1.Z) == Convert.ToInt64(vector2.Z)))
            {
                result = true;

            }
            else
            {
                result = false;
            }
            return result;

        }

        public void SEC_VIEW_COMPARE(List<req_pts> mypoints, TSD.View current_view, TSM.Part main_part,
            List<List<int>> mypart_list_of_created_sect_view, TSD.AssemblyDrawing ASSEMBLY_DRAWING, out List<section_loc_with_parts> list2, string drg_att)
        {
            List<double> mainpart_values = catalogHandler.Getcatalog_values(main_part);

            list2 = new List<section_loc_with_parts>();
            mypoints = mypoints.OrderBy(x => x.distance).ToList();



            List<req_pts> final_distance = new List<req_pts>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance.Add(mypoints[i]);

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 25)
                    {
                        final_distance.Add(mypoints[i]);

                    }

                    else
                    {
                        if (mypoints[i].distance != mypoints[i + 1].distance)
                        {
                            if (mypoints[i].distance_for_y > mypoints[i + 1].distance_for_y)
                            {
                                final_distance.Add(mypoints[i]);
                            }
                            else
                            {
                                final_distance.Add(mypoints[i + 1]);
                                //final_distance.Add(mypoints[i ]);
                            }
                        }
                    }
                }
            }
            TSM.Part main = main_part;

            List<double> final_distance_UNIQUE = new List<double>();


            section_loc_with_parts obj1 = new section_loc_with_parts();
            List<TSM.Part> list1 = new List<TSM.Part>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance_UNIQUE.Add(mypoints[i].distance);
                    list1.Add(mypoints[i].part);
                    list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 25)
                    {

                        list1.Add(mypoints[i].part);
                        list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });
                        list1 = new List<TSM.Part>();
                    }

                    else
                    {
                        list1.Add(mypoints[i].part);

                    }
                }
            }

            List<TSM.Part> final_list = new List<TSM.Part>();
            List<section_loc_with_parts> f1 = new List<section_loc_with_parts>();
            List<section_loc_with_parts> f2 = new List<section_loc_with_parts>();


            List<section_loc_with_parts> FINAL = list2.GroupBy(X => X.partlist.Count).Select(Y => Y.FirstOrDefault()).ToList();
            List<string> final_check_for_unique = new List<string>();
            for (int i = list2.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    list2[i].sectionview_needed = "YES";
                }
                else
                {

                    List<string> check_for_unique = new List<string>();
                    for (int j = i - 1; j >= 0; j--)
                    {


                        var first_loop = list2[i].partlist;
                        var second_loop = list2[j].partlist;

                        if (!(first_loop.Count == second_loop.Count))
                        {
                            check_for_unique.Add("UNIQUE");

                        }
                        else
                        {
                            bool result = FUNCTION_FOR_COMPARING_PARTMARKS_AND_ORIENTATION(first_loop, second_loop, main_part as TSM.Beam);

                            if (result == true)
                            {

                                check_for_unique.Add("SAME");
                            }
                            else
                            {
                                check_for_unique.Add("UNIQUE");
                            }

                        }
                    }

                    if (!check_for_unique.Contains("SAME"))
                    {
                        list2[i].sectionview_needed = "YES";
                    }
                    else
                    {
                        list2[i].sectionview_needed = "NO";
                        int check = check_for_unique.LastIndexOf("SAME");
                        int check2 = check_for_unique.Count - (check + 1);
                        list2[i].index_of_same_sec = check2;
                    }

                }
            }





            list2 = list2.OrderBy(x => x.distance).ToList();
            List<section_loc_with_parts> section = new List<section_loc_with_parts>();
            List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();








            for (int i = 0; i < list2.Count; i++)
            {
                if ((list2[i].sectionview_needed == "YES"))
                {
                    //if (list2[i].partlist.Count > 1)
                    //{
                    //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                    //}
                    double minx = 0;
                    double maxx = 0;
                    double mny = 0;
                    double mxy = 0;

                    minx = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].X);
                    maxx = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].X);
                    mny = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].Y);
                    mxy = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].Y);

                    double miny = Convert.ToInt64(mny);
                    double maxy = Convert.ToInt64(mxy);

                    double distanceofx = ((minx + maxx) / 2);
                    double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


                    double distance_of_y = ((miny + maxy) / 2);
                    double height = Convert.ToDouble(mainpart_values[0]);
                    double height_for_view = 0;
                    double height_for_view1 = 0;

                    double POSIH = Convert.ToInt64(height / 2);
                    double NEGAH = -Convert.ToInt64(height / 2);



                    if ((maxy <= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny >= POSIH))
                    {
                        height_for_view = (height / 2);
                        height_for_view1 = maxy;

                    }
                    else if ((maxy <= NEGAH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);

                    }
                    else if ((maxy >= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = maxy;
                    }

                    else if ((maxy <= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = maxy;
                    }




                    TSD.View bottom_view = null;


                    TSD.SectionMark sec = null;

                    double distancefor_depthup = 0;
                    double distancefor_depthdown = 0;
                    if (DISTANCE_TO_COMPARE < 300)
                    {
                        distancefor_depthup = Math.Abs(minx - distanceofx);
                        distancefor_depthdown = Math.Abs(maxx - distanceofx);
                    }
                    else if (DISTANCE_TO_COMPARE > 300)
                    {
                        distancefor_depthup = 0;
                        distancefor_depthdown = 0;

                    }

                    //if (height_for_view > 0)
                    //{



                    //    height_for_view = height_for_view + 25.4;

                    //}


                    //else
                    //{
                    //    height_for_view = height_for_view - 25.4;
                    //}

                    //if (height_for_view1 > 0)
                    //{
                    //    if (height_for_view1 > POSIH )
                    //    {
                    //        height_for_view1 = height_for_view1;

                    //    }
                    //    else
                    //    {
                    //        height_for_view1 = height_for_view1 + 25.4;
                    //    }
                    //}
                    //else
                    //{
                    //    height_for_view1 = height_for_view1 - 25.4;
                    //}

                    //TSG.Point P1 = new TSG.Point(distanceofx, height_for_view - 1, 0);
                    //TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1 + 1, 0);
                    height_for_view = current_view.RestrictionBox.MaxPoint.Y;
                    height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
                    TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
                    TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);
                    //TSG.Point P1 = new TSG.Point(minx, height_for_view, 0);
                    //TSG.Point P2 = new TSG.Point(maxx, height_for_view1, 0);



                    double dep_up = maxx - distanceofx;
                    double dep_down = distanceofx - minx;
                    try
                    {
                        //TSD.View.ViewMarkTagAttributes tag = new TSD.View.ViewMarkTagAttributes();
                        //tag.TagContent.Add(TSD.PropertyElement.PropertyElementType.SectionViewLabelMarkPropertyElementTypes.SectionName());

                        if (drg_att == "SK_BEAM_A1")
                        {
                            bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                            bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                            bottom_view.Modify();


                        }
                        else
                        {
                            bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                            bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                            bottom_view.Modify();
                        }
                        //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);


                        //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                        //bottom_view.Attributes.Scale = current_view.Attributes.Scale;
                        //bottom_view.Modify();
                        //ASSEMBLY_DRAWING.CommitChanges();

                        sec.Attributes.LineLengthOffset = 0;
                        sec.Modify();

                        double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.X);
                        double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.X);


                        if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                        {
                            bottom_view.RestrictionBox.MaxPoint.X = change_min;
                            bottom_view.Modify();

                        }
                        else
                        {
                            bottom_view.RestrictionBox.MinPoint.X = -change_max;
                            bottom_view.Modify();

                        }


                        bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
                        bottom_view.Modify();


                        TSD.FontAttributes FONT = new TSD.FontAttributes();
                        FONT.Color = TSD.DrawingColors.Magenta;
                        FONT.Height = Convert.ToInt16(3.96875);
                        //bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewFrame;
                        //bottom_view.Modify();


                        //TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                        TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);

                        TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
                        TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
                        X.Font.Color = TSD.DrawingColors.Magenta;
                        X.Font.Height = Convert.ToInt64(3.96875);


                        //TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };

                        TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };



                        sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

                        bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

                        bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

                        bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        bottom_view.Attributes.Scale.Equals(current_view.Attributes.Scale);

                        bottom_view.Modify();


                        list2[i].myview = bottom_view;
                        sectionmarklist.Add(sec);


                    }
                    catch
                    {
                    }

                }

                else
                {

                    TSD.SectionMark sec_dummy = null;
                    sectionmarklist.Add(sec_dummy);
                    TSD.SectionMark mysec = sectionmarklist[list2[i].index_of_same_sec];

                    mysec.LeftPoint.X = list2[i].distance;
                    mysec.RightPoint.X = list2[i].distance;
                    //mysec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                    //mysec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;






                    mysec.Insert();

                }






            }

        }





        public void SEC_VIEW_COMPARE_not_for_section(List<req_pts> mypoints, TSD.View current_view, TSM.Part main_part, List<List<int>> mypart_list_of_created_sect_view, TSD.AssemblyDrawing ASSEMBLY_DRAWING, out List<section_loc_with_parts> list2)
        {
            List<double> mainpart_values = catalogHandler.Getcatalog_values(main_part);

            list2 = new List<section_loc_with_parts>();
            mypoints = mypoints.OrderBy(x => x.distance).ToList();



            List<req_pts> final_distance = new List<req_pts>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance.Add(mypoints[i]);

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 125)
                    {
                        final_distance.Add(mypoints[i]);

                    }

                    else
                    {
                        if (mypoints[i].distance != mypoints[i + 1].distance)
                        {
                            if (mypoints[i].distance_for_y > mypoints[i + 1].distance_for_y)
                            {
                                final_distance.Add(mypoints[i]);
                            }
                            else
                            {
                                final_distance.Add(mypoints[i + 1]);
                                //final_distance.Add(mypoints[i ]);
                            }
                        }
                    }
                }
            }
            TSM.Part main = main_part;

            List<double> final_distance_UNIQUE = new List<double>();


            section_loc_with_parts obj1 = new section_loc_with_parts();
            List<TSM.Part> list1 = new List<TSM.Part>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance_UNIQUE.Add(mypoints[i].distance);
                    list1.Add(mypoints[i].part);
                    list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 125)
                    {

                        list1.Add(mypoints[i].part);
                        list2.Add(new section_loc_with_parts() { partlist = list1, distance = mypoints[i].distance });
                        list1 = new List<TSM.Part>();
                    }

                    else
                    {
                        list1.Add(mypoints[i].part);

                    }
                }
            }

            List<TSM.Part> final_list = new List<TSM.Part>();
            List<section_loc_with_parts> f1 = new List<section_loc_with_parts>();
            List<section_loc_with_parts> f2 = new List<section_loc_with_parts>();


            List<section_loc_with_parts> FINAL = list2.GroupBy(X => X.partlist.Count).Select(Y => Y.FirstOrDefault()).ToList();
            List<string> final_check_for_unique = new List<string>();
            for (int i = list2.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    list2[i].sectionview_needed = "YES";
                }
                else
                {

                    List<string> check_for_unique = new List<string>();
                    for (int j = i - 1; j >= 0; j--)
                    {


                        var first_loop = list2[i].partlist;
                        var second_loop = list2[j].partlist;

                        if (!(first_loop.Count == second_loop.Count))
                        {
                            check_for_unique.Add("UNIQUE");

                        }
                        else
                        {
                            bool result = FUNCTION_FOR_COMPARING_PARTMARKS_AND_ORIENTATION(first_loop, second_loop, main_part as TSM.Beam);

                            if (result == true)
                            {

                                check_for_unique.Add("SAME");
                            }
                            else
                            {
                                check_for_unique.Add("UNIQUE");
                            }

                        }
                    }

                    if (!check_for_unique.Contains("SAME"))
                    {
                        list2[i].sectionview_needed = "YES";
                    }
                    else
                    {
                        list2[i].sectionview_needed = "NO";
                        int check = check_for_unique.LastIndexOf("SAME");
                        int check2 = check_for_unique.Count - (check + 1);
                        list2[i].index_of_same_sec = check2;
                    }

                }
            }





            list2 = list2.OrderBy(x => x.distance).ToList();
            List<section_loc_with_parts> section = new List<section_loc_with_parts>();
            List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();








            for (int i = 0; i < list2.Count; i++)
            {
                if ((list2[i].sectionview_needed == "YES"))
                {
                    //if (list2[i].partlist.Count > 1)
                    //{
                    //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                    //}
                    double minx = 0;
                    double maxx = 0;
                    double mny = 0;
                    double mxy = 0;

                    minx = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].X);
                    maxx = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].X);
                    mny = list2[i].partlist.Min(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[0].Y);
                    mxy = list2[i].partlist.Max(x => boundingBoxHandler.bounding_box_sort_x(x, current_view)[1].Y);

                    double miny = Convert.ToInt64(mny);
                    double maxy = Convert.ToInt64(mxy);

                    double distanceofx = ((minx + maxx) / 2);
                    double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


                    double distance_of_y = ((miny + maxy) / 2);
                    double height = mainpart_values[0];
                    double height_for_view = 0;
                    double height_for_view1 = 0;

                    double POSIH = Convert.ToInt64(height / 2);
                    double NEGAH = -Convert.ToInt64(height / 2);



                    if ((maxy <= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny >= POSIH))
                    {
                        height_for_view = (height / 2);
                        height_for_view1 = maxy;

                    }
                    else if ((maxy <= NEGAH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);

                    }
                    else if ((maxy >= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = maxy;
                    }

                    else if ((maxy <= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = maxy;
                    }




                    TSD.View bottom_view = null;


                    TSD.SectionMark sec = null;

                    double distancefor_depthup = 0;
                    double distancefor_depthdown = 0;
                    if (DISTANCE_TO_COMPARE < 300)
                    {
                        distancefor_depthup = Math.Abs(minx - distanceofx);
                        distancefor_depthdown = Math.Abs(maxx - distanceofx);
                    }
                    else if (DISTANCE_TO_COMPARE > 300)
                    {
                        distancefor_depthup = 0;
                        distancefor_depthdown = 0;

                    }

                    //if (height_for_view > 0)
                    //{



                    //    height_for_view = height_for_view + 25.4;

                    //}


                    //else
                    //{
                    //    height_for_view = height_for_view - 25.4;
                    //}

                    //if (height_for_view1 > 0)
                    //{
                    //    if (height_for_view1 > POSIH )
                    //    {
                    //        height_for_view1 = height_for_view1;

                    //    }
                    //    else
                    //    {
                    //        height_for_view1 = height_for_view1 + 25.4;
                    //    }
                    //}
                    //else
                    //{
                    //    height_for_view1 = height_for_view1 - 25.4;
                    //}

                    //TSG.Point P1 = new TSG.Point(distanceofx, height_for_view - 1, 0);
                    //TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1 + 1, 0);
                    height_for_view = current_view.RestrictionBox.MaxPoint.Y;
                    height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
                    TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
                    TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);
                    //TSG.Point P1 = new TSG.Point(minx, height_for_view, 0);
                    //TSG.Point P2 = new TSG.Point(maxx, height_for_view1, 0);

                    double dep_up = maxx - distanceofx;
                    double dep_down = distanceofx - minx;
                    try
                    {
                        //TSD.View.ViewMarkTagAttributes tag = new TSD.View.ViewMarkTagAttributes();
                        //tag.TagContent.Add(TSD.PropertyElement.PropertyElementType.SectionViewLabelMarkPropertyElementTypes.SectionName());

                        ////bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);
                        ////bottom_view.Attributes.Scale = current_view.Attributes.Scale;
                        ////bottom_view.Modify();
                        ////ASSEMBLY_DRAWING.CommitChanges();


                        //TSD.FontAttributes FONT = new TSD.FontAttributes();
                        //FONT.Color = TSD.DrawingColors.Magenta;
                        //FONT.Height = Convert.ToInt16(3.96875);



                        //TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                        //TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);




                        //TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };





                        //sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        //sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { textelement2 });

                        //bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

                        //bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

                        //bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                        //sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                        //sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        //bottom_view.Attributes.Scale.Equals(current_view.Attributes.Scale);

                        //bottom_view.Modify();
                        //double size = 0;


                        list2[i].myview = bottom_view;
                        sectionmarklist.Add(sec);


                    }
                    catch
                    {
                    }

                }
            }

        }


        public TSD.PointList converted_points(TSD.PointList list_of_points, TSM.Beam mainpart, TSD.View current_view)
        {
            TSD.PointList webb = new TSD.PointList();

            TSG.Matrix toviewpart = TSG.MatrixFactory.FromCoordinateSystem(mainpart.GetCoordinateSystem());

            foreach (TSG.Point pt in list_of_points)
            {
                TSG.Point mtpt = toviewpart.Transform(pt);
                webb.Add(mtpt);
            }

            TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            TSD.PointList webb1 = new TSD.PointList();
            foreach (TSG.Point pt in webb)
            {
                TSG.Point mtpt = toviewpart1.Transform(pt);
                webb1.Add(mtpt);
            }

            return webb1;
        }
        public TSD.PointList converted_points(TSD.PointList list_of_points, TSM.Part mainpart, TSD.View current_view)
        {
            TSD.PointList webb = new TSD.PointList();

            TSG.Matrix toviewpart = TSG.MatrixFactory.FromCoordinateSystem(mainpart.GetCoordinateSystem());

            foreach (TSG.Point pt in list_of_points)
            {
                TSG.Point mtpt = toviewpart.Transform(pt);
                webb.Add(mtpt);
            }

            TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            TSD.PointList webb1 = new TSD.PointList();
            foreach (TSG.Point pt in webb)
            {
                TSG.Point mtpt = toviewpart1.Transform(pt);
                webb1.Add(mtpt);
            }

            return webb1;
        }

        public TSD.PointList converted_points_FOR_ATTRIBUTE_CHECK(TSD.PointList list_of_points, TSM.Beam mainpart)
        {
            TSD.PointList webb = new TSD.PointList();

            TSG.Matrix toviewpart = TSG.MatrixFactory.ToCoordinateSystem(mainpart.GetCoordinateSystem());

            foreach (TSG.Point pt in list_of_points)
            {
                TSG.Point mtpt = toviewpart.Transform(pt);
                webb.Add(mtpt);
            }

            //TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            //TSD.PointList webb1 = new TSD.PointList();
            //foreach (TSG.Point pt in webb)
            //{
            //    TSG.Point mtpt = toviewpart1.Transform(pt);
            //    webb1.Add(mtpt);
            //}

            return webb;
        }




        public TSG.Point converted_points_FOR_CHANNEL(TSM.Model mymodel, TSG.Point POINT_TO_CONVERT, TSM.Beam mainpart, TSD.View current_view)
        {

            TSG.Matrix toglobal = mymodel.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;


            TSG.Matrix toviewpart = TSG.MatrixFactory.ToCoordinateSystem(mainpart.GetCoordinateSystem());


            TSG.Point mtpt = toglobal.Transform(POINT_TO_CONVERT);


            TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
            TSD.PointList webb1 = new TSD.PointList();

            TSG.Point mtpt1 = toviewpart1.Transform(mtpt);

            return mtpt1;


        }


        public string get_report_properties(TSM.Part part, string property)
        {

            string output = "";
            part.GetReportProperty(property, ref output);
            return output;
        }
        public string get_report_properties1(TSM.Assembly part, string property)
        {

            string output = "";
            part.GetReportProperty(property, ref output);
            return output;
        }
        public double get_report_properties_double(TSM.Part part, string property)
        {

            double output = 0;
            part.GetReportProperty(property, ref output);
            return Math.Round((output), 2);
        }

    }
}
