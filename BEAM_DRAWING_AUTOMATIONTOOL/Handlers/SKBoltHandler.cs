using MySqlX.XDevAPI;
using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Utils;
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
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures.Drawing;
using MySqlX.XDevAPI;
using SK.Tekla.Drawing.Automation.Models;
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;
using Tekla.Structures.Concrete;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKBoltHandler
    {
        private readonly CustomInputModel _userInput;

        private string client; //client

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        public SKBoltHandler(SKCatalogHandler catalogHandler, BoltMatrixHandler boltMatrixHandler, CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler;
            this.boltMatrixHandler = boltMatrixHandler;
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }

        public void ANGLE_BOLT_DIM(TSD.PointList FINAL_list3x3_positive, TSD.PointList FINAL_list3x3_negative, 
            double height, TSD.View current_view, List<double> MAINPART_PROFILE_VALUES, double maxx, double minx, string drg_att)
        {
            TSD.StraightDimensionSetHandler dim_3x3 = new TSD.StraightDimensionSetHandler();


            TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            fixed_attributes.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
           TSD.PointList FINAL_RD_LIST_within_ht_POS = new TSD.PointList();

            TSD.PointList FINAL_RD_LIST_above_fl_POS = new TSD.PointList();
            TSD.PointList FINAL_RD_LIST_below_fl_POS = new TSD.PointList();


            TSD.PointList FINAL_RD_LIST_within_ht_NEG = new TSD.PointList();

            TSD.PointList FINAL_RD_LIST_above_fl_NEG = new TSD.PointList();
            TSD.PointList FINAL_RD_LIST_below_fl_NEG = new TSD.PointList();

            foreach (TSG.Point PT in FINAL_list3x3_positive)
            {
                if ((Convert.ToInt64(PT.Y) < height) && (Convert.ToInt64(PT.Y) > -height))
                {
                    FINAL_RD_LIST_within_ht_POS.Add(PT);

                }
                else if (Convert.ToInt64(PT.Y) > height)
                {
                    FINAL_RD_LIST_above_fl_POS.Add(PT);

                }
                else if (Convert.ToInt64(PT.Y) < -height)
                {
                    FINAL_RD_LIST_below_fl_POS.Add(PT);

                }
            }
            foreach (TSG.Point PT in FINAL_list3x3_negative)
            {
                if ((Convert.ToInt64(PT.Y) < height) && (Convert.ToInt64(PT.Y) > -height))
                {
                    FINAL_RD_LIST_within_ht_NEG.Add(PT);

                }
                else if (Convert.ToInt64(PT.Y) > height)
                {
                    FINAL_RD_LIST_above_fl_NEG.Add(PT);

                }
                else if (Convert.ToInt64(PT.Y) < -height)
                {
                    FINAL_RD_LIST_below_fl_NEG.Add(PT);
                }
            }


            FINAL_RD_LIST_within_ht_POS.Add(new TSG.Point((Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
            FINAL_RD_LIST_above_fl_POS.Add(new TSG.Point((Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
            FINAL_RD_LIST_below_fl_POS.Add(new TSG.Point((Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
            TSG.Vector VECTOR1 = new TSG.Vector(1, 0, 0);

            FINAL_RD_LIST_within_ht_NEG.Add(new TSG.Point(-(Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
            FINAL_RD_LIST_above_fl_NEG.Add(new TSG.Point(-(Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
            FINAL_RD_LIST_below_fl_NEG.Add(new TSG.Point(-(Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));

            TSG.Vector VECTOR2 = new TSG.Vector(-1, 0, 0);





            try
            {
                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_within_ht_POS[0].X) - Math.Abs(maxx));
                dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_within_ht_POS, VECTOR1, distance1 + 75, fixed_attributes);

            }
            catch
            {
            }

            try
            {
                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_within_ht_NEG[0].X) - Math.Abs(minx));
                dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_within_ht_NEG, VECTOR2, distance1 + 75, fixed_attributes);

            }
            catch
            {
            }

            try
            {

                if (FINAL_RD_LIST_above_fl_POS.Count > 2)
                {

                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl_POS[0].X) - Math.Abs(maxx));
                    dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl_POS, VECTOR1, distance1 + 75, fixed_attributes);
                }
                else
                {
                    TSD.PointList FINAL_RD_LIST_above_fl_POS_duplicate = new TSD.PointList();
                    FINAL_RD_LIST_above_fl_POS_duplicate.Add(FINAL_RD_LIST_above_fl_POS[1]);
                    FINAL_RD_LIST_above_fl_POS_duplicate.Add(FINAL_RD_LIST_above_fl_POS[0]);
                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl_POS[0].X) - Math.Abs(maxx));
                    dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl_POS_duplicate, VECTOR1, distance1 + 75, fixed_attributes);

                }

            }
            catch
            {
            }

            try
            {
                if (FINAL_RD_LIST_above_fl_NEG.Count > 2)
                {
                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl_NEG[0].X) - Math.Abs(minx));
                    dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl_NEG, VECTOR2, distance1 + 75, fixed_attributes);
                }
                else
                {
                    TSD.PointList FINAL_RD_LIST_above_fl_NEG_duplicate = new TSD.PointList();
                    FINAL_RD_LIST_above_fl_NEG_duplicate.Add(FINAL_RD_LIST_above_fl_NEG[1]);
                    FINAL_RD_LIST_above_fl_NEG_duplicate.Add(FINAL_RD_LIST_above_fl_NEG[0]);
                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl_NEG[0].X) - Math.Abs(minx));
                    dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl_NEG_duplicate, VECTOR2, distance1 + 75, fixed_attributes);

                }



            }
            catch
            {
            }
            try
            {
                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl_POS[0].X) - Math.Abs(maxx));
                dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl_POS, VECTOR1, distance1 + 150, fixed_attributes);

            }
            catch
            {
            }

            try
            {
                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl_NEG[0].X) - Math.Abs(minx));
                dim_3x3.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl_NEG, VECTOR2, distance1 + 150, fixed_attributes);

            }
            catch
            {
            }





        }

        public void partmark_for_bolt_dim_attachments(TSD.View current_view, ref List<Guid> PARTMARK_TO_RETAIN)
        {
            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingObjectEnumerator enum_part_REMOVE_DUPLICATE_MARKS = current_view.GetAllObjects(type_for_part);
            while (enum_part_REMOVE_DUPLICATE_MARKS.MoveNext())
            {
                TSD.Part part_drg = enum_part_REMOVE_DUPLICATE_MARKS.Current as TSD.Part;
                TSM.Part part_model = new TSM.Model().SelectModelObject(part_drg.ModelIdentifier) as TSM.Part;

                string PARTMARK = SkTeklaDrawingUtility.get_report_properties(part_model, "PART_POS");
                TSM.ModelObjectEnumerator enum_for_bolt1 = part_model.GetBolts();
                while (enum_for_bolt1.MoveNext())
                {



                    TSM.BoltGroup drgbolt = enum_for_bolt1.Current as TSM.BoltGroup;
                    TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);

                    if (POINT_FOR_BOLT_MATRIX != null)
                    {
                        PARTMARK_TO_RETAIN.Add(part_model.Identifier.GUID);

                    }


                }

            }
        }

        public TSD.PointList PT_LIST_FOR_LAST_BOLT(TSD.PointList MYPT_LIST)
        {
            TSD.PointList FINAL_RD_LIST = boltMatrixHandler.GetSortingHandler().SortPoints(MYPT_LIST);
            FINAL_RD_LIST.RemoveAt(0);
            TSD.PointList LAST_BOLT = new TSD.PointList();
            for (int I = 0; I < FINAL_RD_LIST.Count; I++)
            {
                int A = FINAL_RD_LIST.Count;
                int B = I + 1;
                if (A > B)
                {

                    long X = Convert.ToInt64(FINAL_RD_LIST[I + 1].X);
                    long X1 = Convert.ToInt64(FINAL_RD_LIST[I].X);
                    long DIFFERENCE = X - X1;
                    if (DIFFERENCE < 150)
                    {

                        LAST_BOLT.Add(FINAL_RD_LIST[I + 1]);
                    }

                }

            }
            return (LAST_BOLT);
        }


        public void bolt_logic(List<TSG.Point> singlebolts, TSD.View current_view, double output, List<double> MAINPART_PROFILE_VALUES, string drg_att)
        {



            var groupedbolts = (from points in singlebolts
                                group points by Convert.ToInt64(points.X) into newlist
                                orderby newlist.Key ascending
                                select new
                                {
                                    x_dist = newlist.Key,
                                    point_in_group = (newlist.OrderBy(y => y.Y).ToList())

                                }).ToList();

            TSD.StraightDimensionSetHandler bolt_combine_dim = new TSD.StraightDimensionSetHandler();
            TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            fixed_attributes.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


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
                                long y_value_current = Convert.ToInt64(groupedbolts[i].point_in_group[j].Y);
                                long y_value_next = Convert.ToInt64(groupedbolts[i + 1].point_in_group[j].Y);
                                if (y_value_current == y_value_next)
                                {
                                    int threshold_value_for_boltdim_combine = 145;
                                    long x_value_current = Convert.ToInt64(groupedbolts[i].point_in_group[j].X);
                                    long x_value_next = Convert.ToInt64(groupedbolts[i + 1].point_in_group[j].X);
                                    long difference = Math.Abs(x_value_current - x_value_next);
                                    if (difference < threshold_value_for_boltdim_combine)
                                    {
                                        if (j == number_of_bolts_current - 1)
                                        {
                                            distance_for_bolt_dim = distance_for_bolt_dim + difference;
                                            h++;

                                        }
                                        ptlist_for_boltdim_rd.Add(groupedbolts[i].point_in_group[j]);


                                    }
                                    else
                                    {
                                        foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                        {
                                            ptlist_for_boltdim.Add(pt);

                                        }

                                        ptlist_for_boltdim.Add(new TSG.Point(groupedbolts[i].point_in_group[j].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));



                                        bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, fixed_attributes);
                                        h = 1;
                                        distance_for_bolt_dim = 50;
                                        break;
                                    }
                                }
                                else
                                {
                                    foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                    {
                                        ptlist_for_boltdim.Add(pt);
                                    }
                                    ptlist_for_boltdim.Add(new TSG.Point(groupedbolts[i].point_in_group[j].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, fixed_attributes);
                                    h = 1;
                                    distance_for_bolt_dim = 50;
                                    break;
                                }

                            }
                        }
                        else
                        {
                            foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                            {
                                ptlist_for_boltdim.Add(pt);
                            }
                            ptlist_for_boltdim.Add(new TSG.Point(ptlist_for_boltdim[0].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));
                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, fixed_attributes);
                            h = 1;
                            distance_for_bolt_dim = 50;
                        }
                    }



                    catch
                    {
                        int value = 0;
                        if (groupedbolts.Count > 1)
                        {
                            double threshold_value = groupedbolts[i].x_dist - groupedbolts[i - 1].x_dist;
                            if (threshold_value > 140)
                            {
                                foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                {
                                    ptlist_for_boltdim.Add(pt);
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



                                for (int ll = groupedbolts.Count - 1; ll > 0; ll--)
                                {
                                    double x_dist_current = groupedbolts[ll].x_dist;
                                    double x_dist_before = groupedbolts[ll - 1].x_dist;
                                    double differece = x_dist_current - x_dist_before;
                                    if (differece > 140)
                                    {
                                        value = ll;
                                        break;

                                    }
                                }
                                foreach (TSG.Point pt in groupedbolts[value].point_in_group)
                                {
                                    ptlist_for_boltdim.Add(pt);

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
                                distance_for_bolt_dim = groupedbolts[groupedbolts.Count - 1].x_dist - groupedbolts[value].x_dist;
                            }
                        }
                        else
                        {
                            foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                            {
                                ptlist_for_boltdim.Add(pt);

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


                        if (groupedbolts[value].x_dist > output - 500)
                        {

                            new_vector = new TSG.Vector(1, 0, 0);
                        }
                        else
                        {

                            new_vector = new TSG.Vector(1, 0, 0);
                        }


                        ptlist_for_boltdim.Add(new TSG.Point(ptlist_for_boltdim[0].X, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2, 0));


                        try
                        {
                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new_vector, distance_for_bolt_dim + 150, fixed_attributes);
                        }
                        catch
                        {
                        }


                    }
                }

            }



            TSD.PointList mypt_for_rd = new TSD.PointList();
            mypt_for_rd.Add(new TSG.Point(0, 0, 0));
            for (int z = 0; z < groupedbolts.Count; z++)
            {
                int s = groupedbolts[z].point_in_group.Count;
                TSG.Point pp = groupedbolts[z].point_in_group[0];
                mypt_for_rd.Add(pp);
            }


            try
            {
                TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rd_att.Color = DrawingColors.Gray70;
                rd_att.Text.Font.Color = DrawingColors.Gray70;
                rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


                double MAXY = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);

                double DISTANCE_FOR_BOLT = Math.Abs(MAXY - mypt_for_rd[0].Y);

                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, mypt_for_rd, new TSG.Vector(0, 1, 0), DISTANCE_FOR_BOLT + 50, rd_att);
            }
            catch
            {
            }
        }


        public string BOLT_IN_VIEW(TSM.Part anglepart, TSD.View currentview)
        {

            TSM.Model mymodel = new TSM.Model();
            List<string> list_of_bolt = new List<string>();
            TSM.ModelObjectEnumerator mybolt_enum = anglepart.GetBolts();
            int BOLT_COUNT = mybolt_enum.GetSize();

            while (mybolt_enum.MoveNext())
            {
                TSM.BoltGroup boltgrp = mybolt_enum.Current as TSM.BoltGroup;

                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentview.ViewCoordinateSystem));
                TSG.CoordinateSystem boltcheck = boltgrp.GetCoordinateSystem();
                TSG.Vector xaxis = boltcheck.AxisX;
                TSG.Vector yaxis = boltcheck.AxisY;
                TSG.Vector zaxis = yaxis.Cross(xaxis);
                zaxis.Normalize();
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                double angle_check_FOR_NOT_IN_VIEW = Math.Abs(SKUtility.RadianToDegree((zaxis.GetAngleBetween(new TSG.Vector(1, 0, 0)))));
                double angle_check_FOR_SLOPE_AND_NORMAL = Math.Abs(SKUtility.RadianToDegree((xaxis.GetAngleBetween(new TSG.Vector(0, 1, 0)))));
                if (Convert.ToInt64(angle_check_FOR_NOT_IN_VIEW) == 90)
                {

                    list_of_bolt.Add("TRUE");

                }
                else
                {
                    list_of_bolt.Add("FALSE");

                }



            }


            bool RESULT = list_of_bolt.Any(X => X.Equals("TRUE"));
            string angle_dim;
            if (RESULT == true)
            {
                angle_dim = "NEED";

            }
            else
            {
                angle_dim = "NOT_NEED";

            }

            return angle_dim;
        }

        public void bolt_rd_in_front_view_for_st_beam(TSD.View current_view, double distance_for_placing, TSD.StraightDimensionSet.StraightDimensionSetAttributes myattribute, double output)
        {
            TSD.PointList boltrdlist = new TSD.PointList();
            TSG.Point mystartbolt_pt = new TSG.Point(0, 0, 0);
            ////////////////////////adding first point //////////////////////////////////////////////////////////////////////////////
            boltrdlist.Add(mystartbolt_pt);
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            ///////////////////////bolt filter //////////////////////////////////////////////////////////////////////////////////////
            Type[] type_for_bolt = new Type[] { typeof(TSD.Bolt) };
            TSD.DrawingObjectEnumerator enum_for_bolt3 = current_view.GetAllObjects(type_for_bolt);
            //////////////////////adding bolt pts in pointlist///////////////////////////////////////////////////////////////////////
            while (enum_for_bolt3.MoveNext())
            {
                TSD.Bolt bolt_drg_part = enum_for_bolt3.Current as TSD.Bolt;
                TSM.BoltArray bolt_front_view = (new TSM.Model().SelectModelObject(bolt_drg_part.ModelIdentifier)) as TSM.BoltArray;
                if (bolt_front_view.BoltType.Equals(TSM.BoltArray.BoltTypeEnum.BOLT_TYPE_SITE))
                {
                    if (bolt_front_view.BoltPositions.Count > 1)
                    {
                        ///////////////////// adding bolt last points in pointlist /////////////////////////////////////////////////////////////////
                        boltrdlist.Add(toviewmatrix.Transform(bolt_front_view.BoltPositions[bolt_front_view.BoltPositions.Count - 1] as TSG.Point));
                    }
                }
            }
            /////////////////////// adding last point/////////////////////////////////////////////////////////////////////////////////
            TSG.Point mylastbolt_pt = new TSG.Point(output, 0, 0);
            boltrdlist.Add(mylastbolt_pt);
            //////////////////////dimension creation /////////////////////////////////////////////////////////////////////////////////
            TSD.StraightDimensionSetHandler bolt_rd = new TSD.StraightDimensionSetHandler();
            bolt_rd.CreateDimensionSet(current_view, boltrdlist, new TSG.Vector(0, 1, 0), distance_for_placing, myattribute);
        }

        public void bolt_3_dim(TSM.ModelObject model_object_of_bolt, TSD.View current_view, TSG.Vector myvector, double distance)
        {
            ///////////////////////////////////////////////////////bolt_dimension_creation_for_3333///////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            if (model_object_of_bolt.GetType().Equals(typeof(TSM.BoltArray)))
            {
                TSM.BoltArray mymodelboltarray = model_object_of_bolt as TSM.BoltArray;
                TSD.PointList boltpoints_in_array = new TSD.PointList();
                TSD.PointList boltpoint_for_dim = new TSD.PointList();
                foreach (TSG.Point boltpoint in mymodelboltarray.BoltPositions)
                {
                    TSG.Point mypoint = toviewmatrix.Transform(boltpoint);
                    boltpoint_for_dim.Add(mypoint);
                }
                TSD.StraightDimensionSetHandler bolt_dim = new TSD.StraightDimensionSetHandler();
                bolt_dim.CreateDimensionSet(current_view as TSD.ViewBase, boltpoint_for_dim, myvector, distance);
            }
            if (model_object_of_bolt.GetType().Equals(typeof(TSM.BoltXYList)))
            {
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        public void BOLTMARK_EXACT(TSD.Drawing mydrg)
        {
            Type type_for_BOLT = typeof(TSD.Bolt);
            TSD.DrawingObjectEnumerator enum_for_views = mydrg.GetSheet().GetAllViews();
            while (enum_for_views.MoveNext())
            {
                TSD.View current_view = enum_for_views.Current as TSD.View;
                TSD.Part.PartAttributes orientation = new TSD.Part.PartAttributes();

                TSD.DrawingObjectEnumerator enum_for_orientation = current_view.GetAllObjects(type_for_BOLT);
                while (enum_for_orientation.MoveNext())
                {

                    TSD.Bolt mypart = enum_for_orientation.Current as TSD.Bolt;
                    TSM.ModelObject model_bolt = new TSM.Model().SelectModelObject(mypart.ModelIdentifier);
                    TSM.BoltGroup MYBOLT = model_bolt as TSM.BoltGroup;
                    if (MYBOLT.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP))
                    {
                        mypart.Attributes.Representation = TSD.Bolt.Representation.ExactSolid;
                        mypart.Attributes.Color = TSD.DrawingColors.Green;
                        mypart.Modify();
                    }





                }

            }
            mydrg.CommitChanges();

        }



        public TSG.Point[,] Get_Bolt_properties_matrix_for_gusset(TSM.BoltGroup model_bolt, TSD.View currentview, string sorted_condition)
        {

            TSM.Model mymodel = new TSM.Model();
            TSG.Point[,] pointarray = null;
            TSG.Point[,] pointarray1 = null;
            ////////////Converting drawing object to model object///////////////////////
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);

            TSD.PointList bolt_ptlist_sorted = new TSD.PointList();
         

            //if (model_bolt.GetType().Equals(typeof(TSM.BoltArray)))
            {
                TSM.BoltArray boltarray = model_bolt as TSM.BoltArray;


                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentview.ViewCoordinateSystem));


                TSG.CoordinateSystem boltcoord1 = boltarray.GetCoordinateSystem();

                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                if ((Convert.ToInt64(boltcoord1.AxisX.Z) == 0) && (Convert.ToInt64(boltcoord1.AxisY.Z) == 0))
                {
                    int number_of_rows = boltarray.GetBoltDistXCount();
                    int number_of_columns = boltarray.GetBoltDistYCount();

                    if ((boltarray.GetBoltDistY(0) == 0) && ((boltarray.GetBoltDistX(0) == 0)))
                    {

                        pointarray = new TSG.Point[1, 1];
                    }

                    else if ((boltarray.GetBoltDistY(0) == 0) && ((boltarray.GetBoltDistX(0) != 0)))
                    {


                        pointarray = new TSG.Point[number_of_rows + 1, number_of_columns];
                    }
                    else if ((boltarray.GetBoltDistY(0) != 0) && ((boltarray.GetBoltDistX(0) == 0)))
                    {


                        pointarray = new TSG.Point[number_of_rows, number_of_columns + 1];
                    }
                    else
                    {

                        pointarray = new TSG.Point[number_of_rows + 1, number_of_columns + 1];

                    }

                    int count = boltarray.BoltPositions.Count;
                    ///////////Condition for single bolt///////////////////////

                    TSG.Matrix to_bolt_matrix = TSG.MatrixFactory.ToCoordinateSystem(boltarray.GetCoordinateSystem());
                    if (count == 1)
                    {
                        bolt_ptlist_sorted.Add(to_bolt_matrix.Transform(boltarray.BoltPositions[0] as TSG.Point));
                    }

                    else
                    {
                        bolt_ptlist_sorted = boltMatrixHandler.GetBoltPoints(boltarray, currentview);
                      
                    }





                    TSD.PointList pointlist_deleted = new TSD.PointList();



                    boltMatrixHandler.GetSortingHandler().SortPoints(bolt_ptlist_sorted);

                    ArrayList list_of_x = new ArrayList();
                    for (int i = 0; i < bolt_ptlist_sorted.Count; i++)
                    {
                        double x_value = (bolt_ptlist_sorted[i] as TSG.Point).X;

                        if (i < bolt_ptlist_sorted.Count - 1)
                        {
                            bool result = SKUtility.AlmostEqual(bolt_ptlist_sorted[i + 1].X, bolt_ptlist_sorted[i].X);
                            if (result == false)
                            {
                                list_of_x.Add(Convert.ToInt64(bolt_ptlist_sorted[i].X));
                            }
                        }
                        else
                        {
                            list_of_x.Add(Convert.ToInt64(bolt_ptlist_sorted[i].X));
                        }

                    }
                    int b = 0;

                    foreach (long x_value in list_of_x)
                    {
                        int a = 0;
                        TSD.PointList pointlist_x_grp = new TSD.PointList();
                        for (int i = 0; i < bolt_ptlist_sorted.Count; i++)
                        {
                            double difference = Math.Abs(x_value - Convert.ToInt64(bolt_ptlist_sorted[i].X));
                            if (difference <= 1)
                            {
                                pointlist_x_grp.Add(bolt_ptlist_sorted[i]);
                            }
                        }
                        boltMatrixHandler.GetSortingHandler().SortPoints(pointlist_x_grp, SKSortingHandler.SortBy.Y,
                            SKSortingHandler.SortOrder.Descending);
                        TSG.Matrix from_bolt_to_global = TSG.MatrixFactory.FromCoordinateSystem(boltarray.GetCoordinateSystem());
                        TSG.Matrix from_view_to_global = TSG.MatrixFactory.FromCoordinateSystem(boltarray.GetCoordinateSystem());
                        try
                        {
                            foreach (TSG.Point pt in pointlist_x_grp)
                            {
                                TSG.Point pt1 = toviewmatrix.Transform(from_bolt_to_global.Transform(pt));
                                pointarray[b, a] = pt1;
                                a++;
                            }
                        }
                        catch
                        {

                        }
                        b++;
                    }
                    pointarray1 = pointarray;

                    int c = pointarray.GetLength(0);
                    int d = pointarray.GetLength(1);
                    TSG.Vector vector_for_sep = new TSG.Vector(boltcoord1.AxisX);

                    if (pointarray.GetLength(0) > 1)
                    {
                        if (pointarray1[0, 0].Y > 0)
                        {


                            if ((vector_for_sep.X > 0) && (vector_for_sep.Y > 0))
                            {
                                //top_right_bolts_list.Add(bolt);
                                pointarray1 = new TSG.Point[c, d];
                                for (int i = 0; i < pointarray.GetLength(0); i++)
                                {
                                    for (int j = 0; j < pointarray.GetLength(1); j++)
                                    {
                                        pointarray1[i, j] = pointarray[c - i - 1, j];

                                    }

                                }
                            }
                            else if ((vector_for_sep.X < 0) && (vector_for_sep.Y > 0))
                            {
                                //top_left_bolts_list.Add(bolt);
                                pointarray1 = new TSG.Point[c, d];
                                for (int i = 0; i < pointarray.GetLength(0); i++)
                                {
                                    for (int j = 0; j < pointarray.GetLength(1); j++)
                                    {
                                        pointarray1[i, j] = pointarray[c - i - 1, j];

                                    }

                                }
                            }
                        }
                        else if (pointarray1[0, 0].Y < 0)
                        {

                            if ((vector_for_sep.X < 0) && (vector_for_sep.Y < 0))
                            {
                                //bottom_left_bolts_list.Add(bolt);
                                pointarray1 = new TSG.Point[c, d];
                                for (int i = 0; i < pointarray.GetLength(0); i++)
                                {
                                    for (int j = 0; j < pointarray.GetLength(1); j++)
                                    {
                                        pointarray1[i, j] = pointarray[c - i - 1, j];

                                    }

                                }
                            }
                            else if ((vector_for_sep.X > 0) && (vector_for_sep.Y < 0))
                            {
                                //bottom_right_bolts_list.Add(bolt);
                                pointarray1 = new TSG.Point[c, d];
                                for (int i = 0; i < pointarray.GetLength(0); i++)
                                {
                                    for (int j = 0; j < pointarray.GetLength(1); j++)
                                    {
                                        pointarray1[i, j] = pointarray[c - i - 1, j];

                                    }

                                }
                            }

                        }
                    }
                }
            }
            return pointarray1;
        }


    }
}
