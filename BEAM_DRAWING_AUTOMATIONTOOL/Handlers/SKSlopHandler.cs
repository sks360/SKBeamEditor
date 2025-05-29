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
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures.Drawing;
using MySqlX.XDevAPI;
using SK.Tekla.Drawing.Automation.Models;
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKSlopHandler
    {
        private readonly CustomInputModel _userInput;

        private string client; //client

        private FontSizeSelector fontSize;

        private readonly SKSortingHandler sortingHandler;

        public SKSlopHandler(SKSortingHandler sortingHandler, CustomInputModel userInput)
        {
            this.sortingHandler = sortingHandler;
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }

        public TSG.Vector VectorForSlope(TSM.BoltGroup boltGroup, TSD.View currentView)
        {
            var model = new TSM.Model();
            var workPlaneHandler = model.GetWorkPlaneHandler();
            workPlaneHandler.SetCurrentTransformationPlane(new TSM.TransformationPlane());
            workPlaneHandler.SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.ViewCoordinateSystem));

            TSG.CoordinateSystem boltCoord = boltGroup.GetCoordinateSystem();
            bool isXAxisPositive = Convert.ToInt64(boltCoord.AxisX.X) > 0;
            bool isYAxisPositive = Convert.ToInt64(boltCoord.AxisX.Y) > 0;

            return isYAxisPositive ? boltCoord.AxisX : boltCoord.AxisY;
        }

       

        public void slope_bolt_logic(List<TSG.Point> singlebolts1, TSG.Matrix to_rotate_matrix, 
            TSD.View current_view, double output, List<double> MAINPART_PROFILE_VALUES, TSG.Vector myvector_for_slope_bolt, TSG.Vector yvector_for_slope_bolt, string drg_att)
        {
            double angle_check_FOR_NOT_IN_VIEW = Math.Abs(SKUtility.RadianToDegree((myvector_for_slope_bolt.GetAngleBetween(new TSG.Vector(1, 0, 0)))));
            TSG.Matrix to_rotate = new TSG.Matrix();
            int a = 1;
            if (Convert.ToInt64(myvector_for_slope_bolt.X) > 0)
            {
                a = -1;
            }
            int b = 1;
            if (Convert.ToInt64(yvector_for_slope_bolt.Y) > 0)
            {
                b = -1;
            }
            double view_scale = current_view.Attributes.Scale;
            TSD.StraightDimensionSetHandler bolt_combine_dim = new TSD.StraightDimensionSetHandler();
            TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            fixed_attributes.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            //singlebolts1.Sort(new sort_by_x_value_max());
            sortingHandler.SortPoints(singlebolts1, SKSortingHandler.
                                            SortBy.X, SKSortingHandler.SortOrder.Descending);
            List<slope_bolt_class> myclass = new List<slope_bolt_class>();
            foreach (TSG.Point pt in singlebolts1)
            {
                TSG.Point myconverted_pt = to_rotate_matrix.Transform(pt);
                double x_value_rotated = Convert.ToInt64(myconverted_pt.X);
                myclass.Add(new slope_bolt_class { converted_pts = myconverted_pt, original_pt = pt, x_dist_of_rotated = Math.Abs(x_value_rotated) });
            }
            int increment_for_grouping1 = 1;
            for (int i = 0; i < myclass.Count - 1; i++)
            {
                int current = i;
                int next = i + 1;
                long x_current = Convert.ToInt64(myclass[current].converted_pts.X);
                long x_next = Convert.ToInt64(myclass[next].converted_pts.X);
                double difference = x_next - x_current;
                if (difference < 150)
                {
                    myclass[i].value = increment_for_grouping1;
                    if (next == myclass.Count - 1)
                    {
                        myclass[i + 1].value = increment_for_grouping1;
                    }
                }
                else
                {
                    myclass[i].value = increment_for_grouping1;
                    increment_for_grouping1++;
                    if (next == myclass.Count - 1)
                    {
                        myclass[i + 1].value = increment_for_grouping1;
                    }
                }
            }
            var groupedbolts = (from x_value in myclass
                                group x_value by x_value.x_dist_of_rotated into newlist
                                orderby newlist.Key ascending
                                select new
                                {
                                    x_dist = newlist.Key,
                                    point_in_group = (newlist.OrderBy(y => y.converted_pts.Y).ToList())
                                }).ToList();

            TSD.AngleDimensionAttributes myangle_att = new TSD.AngleDimensionAttributes();
            myangle_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            myangle_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


            double distance_for_bolt_dim = 0;
            if (groupedbolts.Count > 1)
            {
                if (groupedbolts[0].x_dist < 150)
                {
                    double REST_BOX_MIN = Math.Abs(current_view.RestrictionBox.MinPoint.X);

                    distance_for_bolt_dim = groupedbolts[0].x_dist + REST_BOX_MIN + 85;
                }
                else
                {
                    distance_for_bolt_dim = 30;
                }
            }

            int h = 1;
            TSD.PointList ptlist_for_boltdim_TO_CHECK_THE_DISTANCE = new TSD.PointList();
            for (int i = 0; i < groupedbolts.Count; i++)
            {

                TSD.PointList ptlist_for_boltdim = new TSD.PointList();
                TSD.PointList ptlist_for_boltdim_first_bolt = new TSD.PointList();
                TSD.PointList ptlist_for_boltdim_rd = new TSD.PointList();
                ptlist_for_boltdim_rd.Add(new TSG.Point(0, 0, 0));


                if (i < groupedbolts.Count)
                {
                    int number_of_bolts_current = 0;
                    int number_of_bolts_next = 0;
                    try
                    {
                        number_of_bolts_current = groupedbolts[i].point_in_group.Count;
                        number_of_bolts_next = groupedbolts[i + 1].point_in_group.Count;

                        if (number_of_bolts_current == number_of_bolts_next)
                        {
                            for (int j = 0; j < number_of_bolts_current; j++)
                            {
                                long y_value_current = Convert.ToInt64(groupedbolts[i].point_in_group[j].converted_pts.Y);
                                long y_value_next = Convert.ToInt64(groupedbolts[i + 1].point_in_group[j].converted_pts.Y);
                                if (y_value_current == y_value_next)
                                {
                                    int threshold_value_for_boltdim_combine = 145;
                                    long x_value_current = Convert.ToInt64(groupedbolts[i].point_in_group[j].converted_pts.X);
                                    long x_value_next = Convert.ToInt64(groupedbolts[i + 1].point_in_group[j].converted_pts.X);
                                    long difference = Math.Abs(x_value_current - x_value_next);
                                    if (difference < threshold_value_for_boltdim_combine)
                                    {
                                        if (j == number_of_bolts_current - 1)
                                        {
                                            distance_for_bolt_dim = distance_for_bolt_dim + difference;
                                            h++;

                                        }
                                        ptlist_for_boltdim_rd.Add(groupedbolts[i].point_in_group[j].original_pt);


                                    }
                                    else
                                    {

                                        for (int k = 0; k < groupedbolts[i].point_in_group.Count; k++)
                                        {
                                            ptlist_for_boltdim.Add(groupedbolts[i].point_in_group[k].original_pt);
                                        }


                                        ptlist_for_boltdim_first_bolt.Add(new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]));
                                        ptlist_for_boltdim_first_bolt.Add(new TSG.Point(groupedbolts[i].point_in_group[j].original_pt.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));

                                        //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_first_bolt, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim+25, fixed_attributes);
                                        //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, fixed_attributes);
                                        bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, a * myvector_for_slope_bolt, distance_for_bolt_dim, fixed_attributes);
                                        TSG.Point origin_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]);

                                        TSG.Point last_pt_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[0]);
                                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(last_pt_for_bolt_angle_dim, origin_for_bolt_angle_dim) + 200) / view_scale;
                                        TSG.Vector myvector_angle_dim = new TSG.Vector(origin_for_bolt_angle_dim - last_pt_for_bolt_angle_dim);
                                        TSD.AngleDimension myangle_dim = new TSD.AngleDimension(current_view as TSD.ViewBase, origin_for_bolt_angle_dim, -1 * myvector_angle_dim, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, myangle_att);
                                        myangle_dim.Insert();
                                        h = 1;
                                        distance_for_bolt_dim = 30;
                                        break;
                                    }
                                }
                                else
                                {

                                    for (int k = 0; k < groupedbolts[i].point_in_group.Count; k++)
                                    {
                                        ptlist_for_boltdim.Add(groupedbolts[i].point_in_group[k].original_pt);
                                    }
                                    ptlist_for_boltdim_first_bolt.Add(new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]));
                                    ptlist_for_boltdim_first_bolt.Add(new TSG.Point(groupedbolts[i].point_in_group[j].original_pt.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));

                                    //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_first_bolt, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim+25, fixed_attributes);
                                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, a * myvector_for_slope_bolt, distance_for_bolt_dim, fixed_attributes);


                                    TSG.Point origin_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]);

                                    TSG.Point last_pt_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[0]);
                                    double dist_for_anglular_dim = (TSG.Distance.PointToPoint(last_pt_for_bolt_angle_dim, origin_for_bolt_angle_dim) + 200) / view_scale;
                                    TSG.Vector myvector_angle_dim = new TSG.Vector(origin_for_bolt_angle_dim - last_pt_for_bolt_angle_dim);
                                    TSD.AngleDimension myangle_dim = new TSD.AngleDimension(current_view as TSD.ViewBase, origin_for_bolt_angle_dim, -1 * myvector_angle_dim, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, myangle_att);

                                    myangle_dim.Insert();

                                    h = 1;
                                    distance_for_bolt_dim = 30;
                                    break;
                                }

                            }
                        }
                        else
                        {
                            for (int k = 0; k < groupedbolts[i].point_in_group.Count; k++)
                            {
                                ptlist_for_boltdim.Add(groupedbolts[i].point_in_group[k].original_pt);
                            }
                            ptlist_for_boltdim_first_bolt.Add(new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]));
                            ptlist_for_boltdim_first_bolt.Add(new TSG.Point(ptlist_for_boltdim[0].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                            //ptlist_for_boltdim.Add(new TSG.Point(ptlist_for_boltdim[0].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));

                            //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_first_bolt, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim+25, fixed_attributes);
                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, a * myvector_for_slope_bolt, distance_for_bolt_dim, fixed_attributes);
                            TSG.Point origin_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]);
                            TSG.Point last_pt_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[0]);
                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(last_pt_for_bolt_angle_dim, origin_for_bolt_angle_dim) + 200) / view_scale;
                            TSG.Vector myvector_angle_dim = new TSG.Vector(origin_for_bolt_angle_dim - last_pt_for_bolt_angle_dim);
                            TSD.AngleDimension myangle_dim = new TSD.AngleDimension(current_view as TSD.ViewBase, origin_for_bolt_angle_dim, -1 * myvector_angle_dim, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, myangle_att);
                            myangle_dim.Insert();
                            h = 1;
                            distance_for_bolt_dim = 30;



                        }
                    }



                    catch
                    {
                        if (groupedbolts.Count > 1)
                        {


                            double threshold_value = groupedbolts[i].x_dist - groupedbolts[i - 1].x_dist;


                            if (threshold_value > 140)
                            {

                                for (int k = 0; k < groupedbolts[i].point_in_group.Count; k++)
                                {
                                    ptlist_for_boltdim.Add(groupedbolts[i].point_in_group[k].original_pt);
                                }

                                if (groupedbolts[i].x_dist > output - 500)
                                {
                                    double REST_BOX_MAX = Math.Abs(current_view.RestrictionBox.MaxPoint.X);
                                    distance_for_bolt_dim = Math.Abs(groupedbolts[i].x_dist - REST_BOX_MAX);
                                }
                                else
                                {

                                    distance_for_bolt_dim = 0;
                                }
                            }
                            else
                            {
                                for (int k = 0; k < groupedbolts[groupedbolts.Count - 2].point_in_group.Count; k++)
                                {
                                    ptlist_for_boltdim.Add(groupedbolts[groupedbolts.Count - 2].point_in_group[k].original_pt);
                                }
                                if (groupedbolts[i].x_dist > output - 500)
                                {
                                    double REST_BOX_MAX = Math.Abs(current_view.RestrictionBox.MaxPoint.X);
                                    distance_for_bolt_dim = Math.Abs(groupedbolts[i - 1].x_dist - REST_BOX_MAX);
                                }
                                else
                                {
                                    distance_for_bolt_dim = Math.Abs(groupedbolts[i - 1].x_dist - groupedbolts[i].x_dist);
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < groupedbolts[i].point_in_group.Count; k++)
                            {
                                ptlist_for_boltdim.Add(groupedbolts[i].point_in_group[k].original_pt);
                            }

                            if (groupedbolts[i].x_dist > output - 500)
                            {
                                double REST_BOX_MAX = Math.Abs(current_view.RestrictionBox.MaxPoint.X);
                                distance_for_bolt_dim = Math.Abs(groupedbolts[i].x_dist - REST_BOX_MAX);
                            }
                            else
                            {

                                distance_for_bolt_dim = 0;
                            }
                        }
                        TSG.Vector new_vector = new TSG.Vector();


                        if (groupedbolts[i].x_dist > output - 500)
                        {

                            new_vector = a * myvector_for_slope_bolt;
                        }
                        else
                        {

                            new_vector = a * myvector_for_slope_bolt;
                        }




                        try
                        {
                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, a * new_vector, distance_for_bolt_dim + 100, fixed_attributes);

                        }
                        catch
                        {
                        }
                        try
                        {
                            TSG.Point origin_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[ptlist_for_boltdim.Count - 1]);
                            TSG.Point last_pt_for_bolt_angle_dim = new TSG.Point(ptlist_for_boltdim[0]);
                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(last_pt_for_bolt_angle_dim, origin_for_bolt_angle_dim) + 200) / view_scale;
                            TSG.Vector myvector_angle_dim = new TSG.Vector(origin_for_bolt_angle_dim - last_pt_for_bolt_angle_dim);
                            TSD.AngleDimension myangle_dim = new TSD.AngleDimension(current_view as TSD.ViewBase, origin_for_bolt_angle_dim, -1 * myvector_angle_dim, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, myangle_att);
                            myangle_dim.Insert();
                        }
                        catch
                        {
                        }


                    }
                }

            }
            TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
            rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            rd_att.Color = DrawingColors.Gray70;
            rd_att.Text.Font.Color = DrawingColors.Gray70;
            rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
            rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            TSD.PointList mypt_for_rd = new TSD.PointList();
            mypt_for_rd.Add(new TSG.Point(0, 0, 0));
            for (int z = 0; z < groupedbolts.Count - 1; z++)
            {

                int current = z;
                int next = z + 1;
                double current_xvalue = groupedbolts[current].x_dist;
                double next_xvalue = groupedbolts[next].x_dist;
                double difference = next_xvalue - current_xvalue;
                int s = groupedbolts[z].point_in_group.Count;
                int s_next = groupedbolts[z + 1].point_in_group.Count;
                TSG.Point pp = groupedbolts[z].point_in_group[s - 1].original_pt;
                TSG.Point pp_next = groupedbolts[z + 1].point_in_group[s_next - 1].original_pt;
                TSG.Point pp_first = groupedbolts[z].point_in_group[0].original_pt;
                TSD.PointList mypt_for_bolt_single_vertical = new TSD.PointList();
                TSG.Point pp1 = new TSG.Point();

                mypt_for_rd.Add(pp);
                if (next == groupedbolts.Count - 1)
                {

                    mypt_for_rd.Add(pp_next);

                }
                if (difference < 150)
                {
                    TSD.PointList mypt_for_boltpitch = new TSD.PointList();
                    mypt_for_boltpitch.Add(pp);
                    mypt_for_boltpitch.Add(pp_next);

                    double distance_for_pitch = TSG.Distance.PointToPoint(pp_first, pp);
                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_boltpitch, b * yvector_for_slope_bolt, distance_for_pitch + 75, fixed_attributes);


                    mypt_for_bolt_single_vertical.Add(pp);
                    mypt_for_bolt_single_vertical.Add(new TSG.Point(pp.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_bolt_single_vertical, new TSG.Vector(-1, 0, 0), 50, fixed_attributes);
                    if (next == groupedbolts.Count - 1)
                    {
                        TSD.PointList mypt_for_bolt_single_vertical1 = new TSD.PointList();
                        mypt_for_bolt_single_vertical1.Add(pp_next);
                        mypt_for_bolt_single_vertical1.Add(new TSG.Point(pp_next.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                        bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_bolt_single_vertical1, new TSG.Vector(-1, 0, 0), 50, fixed_attributes);


                    }

                }
                else
                {
                    mypt_for_bolt_single_vertical.Add(pp);
                    mypt_for_bolt_single_vertical.Add(new TSG.Point(pp.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_bolt_single_vertical, new TSG.Vector(1, 0, 0), 50, fixed_attributes);
                    if (next == groupedbolts.Count - 1)
                    {
                        TSD.PointList mypt_for_bolt_single_vertical1 = new TSD.PointList();
                        mypt_for_bolt_single_vertical1.Add(pp_next);
                        mypt_for_bolt_single_vertical1.Add(new TSG.Point(pp_next.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                        bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_bolt_single_vertical1, new TSG.Vector(1, 0, 0), 50, fixed_attributes);


                    }

                }





            }
            if (groupedbolts.Count == 1)
            {


                double current_xvalue = groupedbolts[0].x_dist;


                int s = groupedbolts[0].point_in_group.Count;

                TSG.Point pp = groupedbolts[0].point_in_group[s - 1].original_pt;


                TSD.PointList mypt_for_bolt_single_vertical = new TSD.PointList();
                TSG.Point pp1 = new TSG.Point();
                mypt_for_rd.Add(pp);

                mypt_for_bolt_single_vertical.Add(pp);
                mypt_for_bolt_single_vertical.Add(new TSG.Point(pp.X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_bolt_single_vertical, new TSG.Vector(-1, 0, 0), 50, fixed_attributes);
            }

            try
            {

                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_rd, new TSG.Vector(0, 1, 0), current_view.RestrictionBox.MaxPoint.Y + 100, rd_att);
            }
            catch
            {
            }
        }


    }


}
