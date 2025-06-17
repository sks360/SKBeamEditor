using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;
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
using RenderData;
namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class AttachmentDimension
    {
        private readonly CustomInputModel _userInput;

        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private readonly SKFacePointHandler facePointHandler;

        private readonly SKDrawingHandler drawingHandler;

        private readonly bool isRdConnectMark; 

        private readonly DuplicateRemover duplicateRemover;

        public AttachmentDimension(SKCatalogHandler catalogHandler, 
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
            SKFacePointHandler facePointHandler,
             SKDrawingHandler drawingHandler, DuplicateRemover duplicateRemover, CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler;
            this.boltMatrixHandler = boltMatrixHandler;
            this.boundingBoxHandler = boundingBoxHandler;
            this.sortingHandler = sortingHandler;
            _userInput = userInput;
            this.client = _userInput.Client;
            this.fontSize = _userInput.FontSize;
            this.facePointHandler = facePointHandler;
            this.drawingHandler = drawingHandler;
            this.isRdConnectMark = _userInput.NeedRDConnectionMark;
            this.duplicateRemover = duplicateRemover;
            
        }


        public void CreateDimensionInsideFlangeFront(TSM.Beam main_part, TSD.View current_view, double output, ref List<Guid> PARTMARK_TO_RETAIN, string drg_att)
        {

            TSD.StraightDimensionSet.StraightDimensionSetAttributes RDATT = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            RDATT.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute;
            RDATT.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            RDATT.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            RDATT.Arrowhead.Head = ArrowheadTypes.FilledArrow;
            RDATT.Text.Font.Color = DrawingColors.Gray70;
            RDATT.Color = DrawingColors.Gray70;
            RDATT.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
            
            string profile_type = "";
            main_part.GetReportProperty("PROFILE_TYPE", ref profile_type);

            List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
            double top_front;
            ///////////////////Values based on which gusset plates are filtered, Outside flange or Web based on view type///////////////////
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
            {


                top_front = (catalog_values[0]);
            }
            else
            {
                top_front = (catalog_values[1]);
            }

            Type[] type_forplate = new Type[] { typeof(TSM.Beam), typeof(TSM.ContourPlate) };
            TSM.Model model = new TSM.Model();
            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            TSD.StraightDimensionSetHandler dim_set_handler = new TSD.StraightDimensionSetHandler();
            TSD.PointList ptlist_for_attachments_top = new TSD.PointList();

            ptlist_for_attachments_top.Add(new TSG.Point(0, 0, 0));
            TSD.PointList ptlist_for_attachments_bottom = new TSD.PointList();
            ptlist_for_attachments_bottom.Add(new TSG.Point(0, 0, 0));
            TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(typeof(TSD.Part));
            List<TSM.Part> list_of_angle = new List<TSM.Part>();

            while (enum_for_parts_drg.MoveNext())
            {
                TSD.Part mypart = enum_for_parts_drg.Current as TSD.Part;

                TSM.Part plate = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.Part;
                TSD.PointList ptlist_for_attachments_pl_offset = new TSD.PointList();

                string prof_type = "";

                plate.GetReportProperty("PROFILE_TYPE", ref prof_type);
                //////////////////////////////////Filtering all the plates////////////////////////////////

                TSD.PointList bounding_box_x = boundingBoxHandler.BoundingBoxSort(plate, current_view);
                TSD.PointList bounding_box_y = boundingBoxHandler.BoundingBoxSort(plate, current_view,SortBy.Y);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                TSG.Vector x_vec_of_plate = plate.GetCoordinateSystem().AxisX;
                TSG.Vector y_vec_of_plate = plate.GetCoordinateSystem().AxisY;
                TSG.Vector z_vec_of_plate = x_vec_of_plate.Cross(y_vec_of_plate);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                string profile = plate.Profile.ProfileString;
                double angle_check = SKUtility.RadianToDegree((z_vec_of_plate.GetAngleBetween(new TSG.Vector(1, 0, 0))));

                //if (((prof_type == "B") && (z_vec_of_plate.X != 0)) || ((prof_type == "L") && ((x_vec_of_plate.Y == 0))) || ((prof_type == "T") && ((x_vec_of_plate.X != 0))) || ((prof_type == "U") && ((x_vec_of_plate.Y != 0))))
                double higher_x = Convert.ToInt64(bounding_box_y[1].Y);
                double lower_x = Convert.ToInt64(bounding_box_y[0].Y);
                double flange_val = Convert.ToInt64(top_front / 2);

                //if (((Convert.ToInt64(bounding_box_x[1].X) > -Convert.ToInt64(top_front / 2))&&((Convert.ToInt64(bounding_box_x[1].X) > -Convert.ToInt64(top_front / 2))) || (Convert.ToInt64(bounding_box_x[0].X) < Convert.ToInt64(top_front / 2)))
                if ((higher_x > 0) || (lower_x > 0))
                {
                    if (((higher_x <= flange_val) && (lower_x >= -flange_val)) || ((higher_x >= flange_val) && (lower_x < flange_val)) || ((higher_x >= -flange_val) && (lower_x <= -flange_val)) || ((higher_x <= flange_val) && (lower_x >= -flange_val)))
                    {


                        if (((prof_type == "B") && (z_vec_of_plate.X != 0)) || 
                            ((prof_type == "T") && ((x_vec_of_plate.X != 0))) || 
                            ((prof_type == "U") && ((y_vec_of_plate.Y != 0))))
                        {


                            if (prof_type == "T")
                            {
                                ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint(plate, current_view));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }

                            else if (prof_type == "B")
                            {

                                if ((Convert.ToInt64(angle_check) == 0) || (Convert.ToInt64(angle_check) == 90) || (Convert.ToInt64(angle_check) == 180) || (Convert.ToInt64(angle_check) == 270) || (Convert.ToInt64(angle_check) == 360))
                                {
                                    TSM.ModelObjectEnumerator platebolts = plate.GetBolts();

                                    int a = platebolts.GetSize();
                                    if (a > 0)
                                    {
                                        TSD.PointList p1 = boundingBoxHandler.BoundingBoxSort(plate, current_view);


                                        while (platebolts.MoveNext())
                                        {
                                            TSM.BoltGroup bolt = platebolts.Current as TSM.BoltGroup;
                                            TSG.CoordinateSystem m = bolt.GetCoordinateSystem();
                                            TSM.Part mw = bolt.PartToBeBolted;
                                            TSM.Part mw1 = bolt.PartToBoltTo;
                                            ArrayList mw2 = bolt.OtherPartsToBolt;

                                            if (!mw.Identifier.ID.Equals(plate.Identifier.ID))
                                            {
                                                //TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                //TSG.Point pz = tokkk.Transform(kl.Origin);
                                                //double x_value = pz.X;

                                                double x_value = 0;
                                                string prof_type_for_channel_check = "";
                                                mw.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                                if (prof_type_for_channel_check != "U")
                                                {

                                                    TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                    TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                    TSG.Point pz = tokkk.Transform(kl.Origin);
                                                    x_value = pz.X;
                                                }
                                                else
                                                {
                                                    TSG.CoordinateSystem channel_coord = mw.GetCoordinateSystem();
                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                                    List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw);
                                                    double offset = (catalog_values_for_channel[1]);
                                                    TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                                    kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                                    TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model, current_view);


                                                    x_value = pz.X;

                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                                }
                                                if (isRdConnectMark)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);

                                                }

                                            }
                                            if (!mw1.Identifier.ID.Equals(plate.Identifier.ID))
                                            {
                                                //TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();

                                                //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                //TSG.Point pz = tokkk.Transform(kl.Origin);
                                                //double x_value = pz.X;



                                                double x_value = 0;
                                                string prof_type_for_channel_check = "";
                                                mw1.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                                if (prof_type_for_channel_check != "U")
                                                {
                                                    TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                    TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();


                                                    TSG.Point pz = tokkk.Transform(kl.Origin);
                                                    x_value = pz.X;
                                                }
                                                else
                                                {
                                                    TSG.CoordinateSystem channel_coord = mw1.GetCoordinateSystem();
                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                                    List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw1);
                                                    double offset = (catalog_values_for_channel[1]);
                                                    TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                                    kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                                    TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model, current_view);


                                                    x_value = pz.X;

                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                                }


                                                if (isRdConnectMark)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);

                                                }
                                            }

                                            //else 
                                            //{
                                            //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                            //}

                                        }
                                    }

                                    else
                                    {
                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }

                                }
                            }



                            else
                            {
                                TSG.Point face_point_for_angle_bswelded = facePointHandler.GetFacePointForAngleBothsideWeldedLogic(plate, current_view);
                                ptlist_for_attachments_top.Add(new TSG.Point(face_point_for_angle_bswelded.X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);

                                //ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_y[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));

                            }



                        }

                        else if (prof_type == "L")
                        {

                            if ((Convert.ToInt64(x_vec_of_plate.X) != 0) && (Convert.ToInt64(y_vec_of_plate.Y) != 0))
                            {
                                ////////////////////////////////2018/////////////////////////////////////////////////////
                                ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);

                            }
                            else
                            {
                                string angle_dim = CheckAngleDimensionNeeded(plate, current_view);

                                if (angle_dim == "NEED")
                                {

                                    List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                    TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                                    ptlist_for_attachments_top.Add(midpt_of_angle);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);
                                }

                                if ((prof_type == "L") && ((x_vec_of_plate.Y != 0)))
                                {

                                    list_of_angle.Add(plate);

                                    #region angle_3.5_dim removed
                                    #endregion
                                }

                            }

                        }
                    }
                }
            }

            double HT2 = (catalog_values[0]);
            double HT = Convert.ToInt64(HT2 / 2);


            TSD.PointList pt_list_z_positive = new TSD.PointList();


            if (profile_type == "U")
            {
                double WT2 = (catalog_values[1]);
                double WT = Convert.ToInt64(WT2 / 2);
                foreach (TSG.Point pt in ptlist_for_attachments_top)
                {
                    if (pt != null)
                    {
                        if ((Convert.ToInt16(pt.X) > 0) && (Convert.ToInt16(pt.X) < output))
                        {

                            if (Convert.ToInt16(pt.Z) > -WT)
                            {
                                pt_list_z_positive.Add(pt);
                            }
                        }
                    }
                }

            }
            else
            {
                foreach (TSG.Point pt in ptlist_for_attachments_top)
                {
                    if (pt != null)
                    {
                        if ((Convert.ToInt16(pt.X) > 0) && (Convert.ToInt16(pt.X) < output))
                        {

                            if (Convert.ToInt16(pt.Z) > 0)
                            {
                                pt_list_z_positive.Add(pt);
                            }
                        }
                    }
                }
            }
            TSD.PointList final_ptlist_for_attachments_top = duplicateRemover.RemoveDuplicateXValues(pt_list_z_positive);
            final_ptlist_for_attachments_top.Add(new TSG.Point(0, 0, 0));
            sortingHandler.SortPoints(final_ptlist_for_attachments_top);
            double MAXY = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);

            double PART_DISTANCE_FOR_INSIDE_FLANGE = Math.Abs(MAXY - final_ptlist_for_attachments_top[0].Y);

            try
            {
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_ptlist_for_attachments_top, new TSG.Vector(0, 1, 0), PART_DISTANCE_FOR_INSIDE_FLANGE + 115, RDATT);
            }
            catch
            {
            }

            List<angle_dim_3_5> pt_list_for_list_of_angle = new List<angle_dim_3_5>();
            TSM.Model mymodel = new TSM.Model();
            foreach (TSM.Part mypart in list_of_angle)
            {



                List<TSG.Point> angle_3_5_dim = new List<TSG.Point>();
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                TSG.CoordinateSystem angle_coord = mypart.GetCoordinateSystem();
                TSG.Vector x_axis = angle_coord.AxisX;
                TSG.Vector y_axis = angle_coord.AxisY;
                TSG.Vector z_axis = x_axis.Cross(y_axis);


                List<TSG.Point> allpts_in_angle = pts_in_each_face(mypart, current_view);
                TSG.Point minx1 = allpts_in_angle.Find(x => x.X.Equals(allpts_in_angle.Min(y => y.X)));
                TSG.Point maxx2 = allpts_in_angle.Find(x => x.X.Equals(allpts_in_angle.Max(y => y.X)));
                TSG.Point miny1 = allpts_in_angle.Find(x => x.Y.Equals(allpts_in_angle.Min(y => y.Y)));

                //TSG.Point req_pt1 = allpts_in_angle.Find(x=>Convert.ToInt64( x. X).Equals(allpts_in_angle.Min(y => y.X))

                TSD.PointList mypt_list = new TSD.PointList();
                mypt_list.Add(new TSG.Point(minx1.X, minx1.Y));
                mypt_list.Add(new TSG.Point(maxx2.X, maxx2.Y));


                TSG.Point x1 = new TSG.Point(Convert.ToInt64(minx1.X), Convert.ToInt64(minx1.Y));
                TSG.Point x2 = new TSG.Point(Convert.ToInt64(maxx2.X), Convert.ToInt64(maxx2.Y));

                angle_3_5_dim.Add(x1);
                angle_3_5_dim.Add(x2);

                #region new_logic_for_3.5_dim
                if (((Convert.ToInt64(z_axis.Y)) != 0) || ((Convert.ToInt64(z_axis.Z)) != 0))
                {
                    if (((Convert.ToInt64(x_axis.X)) < 0) && ((Convert.ToInt64(x_axis.Y)) < 0))
                    {

                    }
                    else
                    {
                        if (Convert.ToInt64(z_axis.Z) != 0)
                        {

                        }
                        else
                        {

                            x_axis = -1 * x_axis;
                        }
                    }
                }
                else
                {

                    if (((Convert.ToInt64(x_axis.X)) > 0) || ((Convert.ToInt64(x_axis.Y)) > 0))
                    {
                        x_axis = x_axis;
                    }
                    else
                    {
                        x_axis = -1 * x_axis;

                    }
                }



                pt_list_for_list_of_angle.Add(new angle_dim_3_5 { pt1 = x1, pt2 = x2, x_axis = x_axis, myptlist = mypt_list, angle = mypart });


                double dist = Math.Abs(current_view.RestrictionBox.MaxPoint.Y) - Math.Abs(angle_3_5_dim[0].Y);
                TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;


                try
                {

                    //dim_for_3_5.CreateDimensionSet(current_view, angle_3_5_dim, x_axis, dist + 50, att);

                }
                catch
                {
                }
                #endregion

            }


            List<angle_dim_3_5> delete_list_of_angle = pt_list_for_list_of_angle.Distinct(new REMOVING_DUPLICATE_angle_VALUE_IN_CURRENT_VIEW()).ToList();

            foreach (var obj in delete_list_of_angle)
            {
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                ArrayList array_list_of_points = new ArrayList();
                foreach (TSG.Point mypt in obj.myptlist)
                {
                    array_list_of_points.Add(mypt);

                }

                TSD.PointList sort_x_ptlist = sortingHandler.SortPoints(obj.myptlist);

                TSG.Line l1 = new TSG.Line(sort_x_ptlist[1], obj.x_axis);
                TSG.GeometricPlane plane1 = new TSG.GeometricPlane(sort_x_ptlist[0], obj.x_axis);
                TSG.Point p1 = TSG.Intersection.LineToPlane(l1, plane1);
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                TSD.PointList angle_leg_dim = new TSD.PointList();
                angle_leg_dim.Add(sort_x_ptlist[0]);
                angle_leg_dim.Add(p1);
                double dist = Math.Abs(current_view.RestrictionBox.MaxPoint.Y) - Math.Abs(obj.myptlist[0].Y);
                TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                TSG.Vector x_axis = obj.x_axis;

                if (x_axis.Y > 0)
                {
                    x_axis = -1 * x_axis;

                }
                else
                {


                }

                try
                {

                    dim_for_3_5.CreateDimensionSet(current_view, angle_leg_dim, x_axis, dist + 50, att);

                }
                catch
                {
                }

            }

        }

        public void CreateDimensionInsideFlangeTop(TSM.Beam main_part, TSD.View current_view, double output, ref List<Guid> PARTMARK_TO_RETAIN, string drg_att)
        {
            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
            TSD.StraightDimensionSet.StraightDimensionSetAttributes RDATT = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            RDATT.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute;
            RDATT.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            RDATT.Text.Font.Color = DrawingColors.Gray70;
            RDATT.Arrowhead.Head = ArrowheadTypes.FilledArrow;
            RDATT.Color = DrawingColors.Gray70;
            RDATT.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            RDATT.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
            string profile_type = "";
            main_part.GetReportProperty("PROFILE_TYPE", ref profile_type);
            double view_scale = current_view.Attributes.Scale;

            List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
            double top_front;
            ///////////////////Values based on which gusset plates are filtered, Outside flange or Web based on view type///////////////////
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
            {


                top_front = (catalog_values[0]);
            }
            else
            {
                top_front = (catalog_values[1]);
            }

            Type[] type_forplate = new Type[] { typeof(TSM.Beam), typeof(TSM.ContourPlate) };
            TSM.Model model = new TSM.Model();
            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            TSD.StraightDimensionSetHandler dim_set_handler = new TSD.StraightDimensionSetHandler();
            TSD.PointList ptlist_for_attachments_top = new TSD.PointList();
            TSD.PointList ptlist_for_attachments_top_plate_nearside = new TSD.PointList();
            TSD.PointList ptlist_for_attachments_angle = new TSD.PointList();
            ptlist_for_attachments_angle.Add(new TSG.Point(0, 0, 0));
            ptlist_for_attachments_top.Add(new TSG.Point(0, 0, 0));
            ptlist_for_attachments_top_plate_nearside.Add(new TSG.Point(0, 0, 0));
            TSD.PointList ptlist_for_attachments_bottom = new TSD.PointList();
            ptlist_for_attachments_bottom.Add(new TSG.Point(0, 0, 0));
            TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(typeof(TSD.Part));

            while (enum_for_parts_drg.MoveNext())
            {
                TSD.Part mypart = enum_for_parts_drg.Current as TSD.Part;

                TSM.Part plate = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.Part;


                string prof_type = "";

                plate.GetReportProperty("PROFILE_TYPE", ref prof_type);
                //////////////////////////////////Filtering all the plates////////////////////////////////

                TSD.PointList bounding_box_x = boundingBoxHandler.BoundingBoxSort(plate, current_view);
                TSD.PointList bounding_box_y = boundingBoxHandler.BoundingBoxSort(plate, current_view, SortBy.Y);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                TSG.Vector x_vec_of_plate = plate.GetCoordinateSystem().AxisX;
                TSG.Vector y_vec_of_plate = plate.GetCoordinateSystem().AxisY;
                TSG.Vector z_vec_of_plate = x_vec_of_plate.Cross(y_vec_of_plate);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                string profile = plate.Profile.ProfileString;
                double angle_check = SKUtility.RadianToDegree((z_vec_of_plate.GetAngleBetween(new TSG.Vector(1, 0, 0))));


                if (((prof_type == "B") && (z_vec_of_plate.X != 0)) || ((prof_type == "T") && ((x_vec_of_plate.X != 0))) || ((prof_type == "U") && ((x_vec_of_plate.Y != 0))))
                {
                    double higher_x = Convert.ToInt64(bounding_box_y[1].Y);
                    double lower_x = Convert.ToInt64(bounding_box_y[0].Y);
                    double flange_val = Convert.ToInt64(top_front / 2);
                    if ((lower_x > 0) && (higher_x > 0))
                    {
                        //if (((Convert.ToInt64(bounding_box_x[1].X) > -Convert.ToInt64(top_front / 2))&&((Convert.ToInt64(bounding_box_x[1].X) > -Convert.ToInt64(top_front / 2))) || (Convert.ToInt64(bounding_box_x[0].X) < Convert.ToInt64(top_front / 2)))
                        if (((higher_x < flange_val) && (lower_x > -flange_val)) || ((higher_x > flange_val) && (lower_x < flange_val)) || ((higher_x > -flange_val) && (lower_x < -flange_val)) || ((higher_x < flange_val) && (lower_x > -flange_val)))
                        {

                            if (prof_type == "T")
                            {
                                ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint(plate, current_view));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }


                            else if (prof_type == "U")
                            {
                                ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint(plate, current_view));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if (prof_type == "B")
                            {
                                if ((Convert.ToInt64(angle_check) == 0) || (Convert.ToInt64(angle_check) == 90) || (Convert.ToInt64(angle_check) == 180) || (Convert.ToInt64(angle_check) == 270) || (Convert.ToInt64(angle_check) == 360))
                                {
                                    TSM.ModelObjectEnumerator platebolts = plate.GetBolts();
                                    int a = platebolts.GetSize();
                                    if (a > 0)
                                    {
                                        TSD.PointList p1 = boundingBoxHandler.BoundingBoxSort(plate, current_view);


                                        while (platebolts.MoveNext())
                                        {
                                            TSM.BoltGroup bolt = platebolts.Current as TSM.BoltGroup;
                                            TSG.CoordinateSystem m = bolt.GetCoordinateSystem();
                                            TSM.Part mw = bolt.PartToBeBolted;
                                            TSM.Part mw1 = bolt.PartToBoltTo;
                                            ArrayList mw2 = bolt.OtherPartsToBolt;

                                            if (!mw.Identifier.ID.Equals(plate.Identifier.ID))
                                            {
                                                //TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                //TSG.Point pz = tokkk.Transform(kl.Origin);
                                                //double x_value = pz.X;




                                                double x_value = 0;
                                                string prof_type_for_channel_check = "";
                                                mw.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                                if (prof_type_for_channel_check != "U")
                                                {

                                                    TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                    TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                    TSG.Point pz = tokkk.Transform(kl.Origin);
                                                    x_value = pz.X;
                                                }
                                                else
                                                {
                                                    TSG.CoordinateSystem channel_coord = mw.GetCoordinateSystem();
                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                                    List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw);
                                                    double offset = (catalog_values_for_channel[1]);
                                                    TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                                    kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                                    TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model, current_view);


                                                    x_value = pz.X;

                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                                }




                                                if (isRdConnectMark)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);

                                                }

                                            }
                                            else if (!mw1.Identifier.ID.Equals(plate.Identifier.ID))
                                            {
                                                //TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();
                                                //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                //TSG.Point pz = tokkk.Transform(kl.Origin);
                                                //double x_value = pz.X;



                                                double x_value = 0;
                                                string prof_type_for_channel_check = "";
                                                mw1.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                                if (prof_type_for_channel_check != "U")
                                                {
                                                    TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                    TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();


                                                    TSG.Point pz = tokkk.Transform(kl.Origin);
                                                    x_value = pz.X;
                                                }
                                                else
                                                {
                                                    TSG.CoordinateSystem channel_coord = mw1.GetCoordinateSystem();
                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                                    List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw1);
                                                    double offset = (catalog_values_for_channel[1]);
                                                    TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                                    kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                                    TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model,  current_view);


                                                    x_value = pz.X;

                                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                                }
                                                if (isRdConnectMark)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);

                                                }
                                            }

                                            //else 
                                            //{
                                            //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, 0));
                                            //}

                                        }


                                    }
                                    else
                                    {
                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, 0));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                }
                                else
                                {
                                    TSM.ModelObjectEnumerator platebolts = plate.GetBolts();
                                    int a = platebolts.GetSize();
                                    if (a > 0)
                                    {
                                        List<TSG.Point> p1_yasc = facePointHandler.GetFacePointsForPlateTest(plate, current_view);
                                        List<TSG.Point> p_x_asc = new List<TSG.Point>();
                                        foreach (TSG.Point mypoint in p1_yasc)
                                        {
                                            p_x_asc.Add(mypoint);
                                        }

                                        p1_yasc.Distinct();
                                        p_x_asc.Distinct();

                                        sortingHandler.SortPoints(p1_yasc, SKSortingHandler.SortBy.Y);
                                        
                                        //p1_yasc.Sort(new sort_by_y_value_asc());
                                        //p_x_asc.Sort(new sort_by_x_value_max());
                                        sortingHandler.SortPoints(p_x_asc, SKSortingHandler.
                                            SortBy.X,SKSortingHandler.SortOrder.Descending);

                                        ptlist_for_attachments_top.Add(p1_yasc[0]);

                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);





                                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


                                        z_vec_of_plate.Normalize();
                                        if (Convert.ToInt64(z_vec_of_plate.Y) > 0)
                                        {
                                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[0], p_x_asc[0]) + 100) / view_scale;

                                            TSG.Vector myvector = new TSG.Vector(p_x_asc[0] - p1_yasc[0]);
                                            TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[0], myvector, new TSG.Vector(1, 0, -1), dist_for_anglular_dim, fi);
                                            angledim1.Insert();
                                            TSG.Vector vector_for_pitch = new TSG.Vector();
                                            List<TSG.Point> p1 = mypoint_for_bolt_skew(plate, current_view, out vector_for_pitch);
                                            List<TSG.Point> P2 = p1.Distinct(new REMOVING_DUPLICATE_Z_VALUE_IN_CURRENT_VIEW()).ToList();
                                            TSD.PointList mypt_list = new TSD.PointList();
                                            foreach (TSG.Point PT in P2)
                                            {
                                                mypt_list.Add(PT);

                                            }

                                            mypt_list.Add(p1_yasc[0]);
                                            TSD.StraightDimensionSetHandler myset = new TSD.StraightDimensionSetHandler();
                                            myset.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list, vector_for_pitch, 100, dim_font_height);
                                        }
                                        else if (Convert.ToInt64(z_vec_of_plate.Y) < 0)
                                        {
                                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[0], p_x_asc[p_x_asc.Count - 1]) + 100) / view_scale;
                                            TSG.Vector myvector = new TSG.Vector(p_x_asc[p_x_asc.Count - 1] - p1_yasc[0]);
                                            TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[p_x_asc.Count - 1], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                            angledim1.Insert();
                                            TSG.Vector vector_for_pitch = new TSG.Vector();
                                            List<TSG.Point> p1 = mypoint_for_bolt_skew(plate, current_view, out vector_for_pitch);
                                            List<TSG.Point> P2 = p1.Distinct(new REMOVING_DUPLICATE_Z_VALUE_IN_CURRENT_VIEW()).ToList();
                                            TSD.PointList mypt_list = new TSD.PointList();
                                            foreach (TSG.Point PT in P2)
                                            {
                                                mypt_list.Add(PT);

                                            }
                                            mypt_list.Add(p1_yasc[0]);
                                            TSG.Vector new_vect = new TSG.Vector(p_x_asc[p_x_asc.Count - 1] - p1[0]);
                                            //TSG.Vector vector_for_pitch = new_vect.Cross(new TSG.Vector(0, 0, 1));
                                            //vector_for_pitch.Normalize();
                                            TSD.StraightDimensionSetHandler myset = new TSD.StraightDimensionSetHandler();
                                            myset.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list, vector_for_pitch, 100, dim_font_height);

                                        }




                                    }
                                    else
                                    {
                                        List<TSG.Point> p1_yasc = facePointHandler.GetFacePointsForPlateTest(plate, current_view);
                                        List<TSG.Point> p_x_asc = new List<TSG.Point>();
                                        foreach (TSG.Point mypoint in p1_yasc)
                                        {
                                            p_x_asc.Add(mypoint);
                                        }

                                        p1_yasc.Distinct();
                                        p_x_asc.Distinct();

                                        sortingHandler.SortPoints(p1_yasc, SKSortingHandler.SortBy.Y);
                                        sortingHandler.SortPoints(p_x_asc, SKSortingHandler.
                                             SortBy.X, SKSortingHandler.SortOrder.Descending);
                                        //p1_yasc.Sort(new sort_by_y_value_asc());
                                        // p_x_asc.Sort(new sort_by_x_value_max());


                                        ptlist_for_attachments_top.Add(p1_yasc[0]);

                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);





                                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                        z_vec_of_plate.Normalize();
                                        if (Convert.ToInt64(z_vec_of_plate.Y) > 0)
                                        {
                                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[0], p_x_asc[0]) + 100) / view_scale;

                                            TSG.Vector myvector = new TSG.Vector(p_x_asc[0] - p1_yasc[0]);
                                            TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[0], myvector, new TSG.Vector(1, 0, -1), dist_for_anglular_dim, fi);
                                            angledim1.Insert();
                                        }
                                        else if (Convert.ToInt64(z_vec_of_plate.Y) < 0)
                                        {
                                            double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[0], p_x_asc[p_x_asc.Count - 1]) + 100) / view_scale;
                                            TSG.Vector myvector = new TSG.Vector(p_x_asc[p_x_asc.Count - 1] - p1_yasc[0]);
                                            TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[p_x_asc.Count - 1], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                            angledim1.Insert();
                                        }

                                    }
                                }
                            }
                            //else if (prof_type == "L")
                            //{
                            //    ///////////hillsdale angle dim intopview//////////////////////////
                            //    // ptlist_for_attachments_angle.Add(new TSG.Point(bounding_box_y[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //    TSM.ModelObjectEnumerator platebolts1 = plate.GetBolts();




                            //    List<TSM.BoltGroup> list_of_bolts = new List<TSM.BoltGroup>();


                            //    TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                            //    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                            //    while (platebolts1.MoveNext())
                            //    {
                            //        TSM.BoltGroup model_bolt = platebolts1.Current as TSM.BoltGroup;



                            //        list_of_bolts.Add(model_bolt);

                            //    }


                            //    IEnumerable<TSM.BoltGroup> result1 = (from b in list_of_bolts
                            //                                          where (Convert.ToInt64((b.GetCoordinateSystem().AxisX.Cross(b.GetCoordinateSystem().AxisY)).Z) == 0)
                            //                                          select b).ToList();


                            //    IEnumerable<TSM.BoltGroup> result = (from b in result1
                            //                                         where (Convert.ToInt64((b.GetCoordinateSystem().AxisX.Cross(b.GetCoordinateSystem().AxisY)).Y) != 0)
                            //                                         select b).ToList();
                            //    //var result = (from b in list_of_bolts
                            //    //              select new
                            //    //              {
                            //    //                  bolt = b,
                            //    //                  mm = (b.GetCoordinateSystem().AxisX.Cross(b.GetCoordinateSystem().AxisY))
                            //    //              }
                            //    //                                        ).ToList();

                            //    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                            //    if (result.Count() == 0)
                            //    {

                            //        TSM.ModelObjectEnumerator platebolts = plate.GetBolts();

                            //        int a = platebolts.GetSize();
                            //        if (a > 0)
                            //        {
                            //            TSD.PointList p1 = boundingBoxHandler.BoundingBoxSort(plate, current_view);


                            //            while (platebolts.MoveNext())
                            //            {
                            //                TSM.BoltGroup bolt = platebolts.Current as TSM.BoltGroup;
                            //                TSG.CoordinateSystem m = bolt.GetCoordinateSystem();
                            //                TSM.Part mw = bolt.PartToBeBolted;
                            //                TSM.Part mw1 = bolt.PartToBoltTo;
                            //                ArrayList mw2 = bolt.OtherPartsToBolt;

                            //                if (!mw.Identifier.ID.Equals(plate.Identifier.ID))
                            //                {




                            //                    double x_value = 0;
                            //                    string prof_type_for_channel_check = "";
                            //                    mw.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                            //                    if (prof_type_for_channel_check != "U")
                            //                    {

                            //                        TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                            //                        TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                            //                        TSG.Point pz = tokkk.Transform(kl.Origin);
                            //                        x_value = pz.X;
                            //                    }
                            //                    else
                            //                    {
                            //                        TSG.CoordinateSystem channel_coord = mw.GetCoordinateSystem();
                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                            //                        List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw);
                            //                        double offset = (catalog_values_for_channel[1]);
                            //                        TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                            //                        kl.Origin = new TSG.Point(0, 0, -offset / 2);



                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                            //                        TSG.Point pz = drawingHandler.converted_points_FOR_CHANNEL(model, kl.Origin, mw as TSM.Beam, current_view);


                            //                        x_value = pz.X;

                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                            //                    }


                            //                    TSG.Point face_point_for_angle_bswelded = facePointHandler.GetFacePoint_for_angle_bothside_weldedlogic_top(plate, current_view);
                            //                    ptlist_for_attachments_top.Add(new TSG.Point(face_point_for_angle_bswelded.X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                    Guid ID = plate.Identifier.GUID;
                            //                    PARTMARK_TO_RETAIN.Add(ID);



                            //                    //if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                            //                    //{
                            //                    //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                    //}
                            //                    //else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                            //                    //{
                            //                    //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                    //}
                            //                    //else
                            //                    //{
                            //                    //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                    //}

                            //                }
                            //                else if (!mw1.Identifier.ID.Equals(plate.Identifier.ID))
                            //                {



                            //                    double x_value = 0;
                            //                    string prof_type_for_channel_check = "";
                            //                    mw1.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                            //                    if (prof_type_for_channel_check != "U")
                            //                    {
                            //                        TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                            //                        TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();


                            //                        TSG.Point pz = tokkk.Transform(kl.Origin);
                            //                        x_value = pz.X;
                            //                    }
                            //                    else
                            //                    {
                            //                        TSG.CoordinateSystem channel_coord = mw1.GetCoordinateSystem();
                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                            //                        List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw1);
                            //                        double offset = (catalog_values_for_channel[1]);
                            //                        TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                            //                        kl.Origin = new TSG.Point(0, 0, -offset / 2);



                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                            //                        TSG.Point pz = drawingHandler.converted_points_FOR_CHANNEL(model, kl.Origin, mw1 as TSM.Beam, current_view);


                            //                        x_value = pz.X;

                            //                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                            //                    }
                            //                    TSG.Point face_point_for_angle_bswelded = facePointHandler.GetFacePoint_for_angle_bothside_weldedlogic_top(plate, current_view);
                            //                    ptlist_for_attachments_top.Add(new TSG.Point(face_point_for_angle_bswelded.X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                    Guid ID = plate.Identifier.GUID;
                            //                    PARTMARK_TO_RETAIN.Add(ID);

                            //                }

                            //                else
                            //                {
                            //                    //  ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //                }

                            //            }
                            //        }


                            //        else
                            //        {
                            //            TSG.Point face_point_for_angle_bswelded = facePointHandler.GetFacePoint_for_angle_bothside_weldedlogic_top(plate, current_view);
                            //            ptlist_for_attachments_top.Add(new TSG.Point(face_point_for_angle_bswelded.X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                            //            Guid ID = plate.Identifier.GUID;
                            //            PARTMARK_TO_RETAIN.Add(ID);

                            //        }



                            //    }
                            //    else
                            //    {







                            //    }

                            //    # region angle_3.5_dim

                            //    //TSD.PointList angle_3_5_dim = new TSD.PointList();

                            //    //TSM.Model mymodel = new TSM.Model();
                            //    //mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                            //    //TSG.CoordinateSystem angle_coord = plate.GetCoordinateSystem();
                            //    //mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


                            //    //if ((profile_type == "U") && (Convert.ToInt64(angle_coord.AxisX.X) == 0))
                            //    //{
                            //    //    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                            //    //    TSG.Vector zvector = main_part.GetCoordinateSystem().AxisX.Cross(main_part.GetCoordinateSystem().AxisY);
                            //    //    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                            //    //    zvector.Normalize();
                            //    //    double WT = 0;


                            //    //    double WT2 = (catalog_values[1]);
                            //    //    if (zvector.Y > 0)
                            //    //    {
                            //    //        WT = (-WT2 / 2);
                            //    //    }
                            //    //    else
                            //    //    {
                            //    //        WT = (WT2 / 2);
                            //    //    }

                            //    //    if (Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(WT))
                            //    //    {
                            //    //        TSG.Point p1 = new TSG.Point();
                            //    //        p1 = new TSG.Point(bounding_box_y[1].X, bounding_box_y[0].Y, 0);
                            //    //        TSG.Point p2 = new TSG.Point();
                            //    //        p2 = new TSG.Point(bounding_box_y[0].X, bounding_box_y[0].Y, 0);
                            //    //        if (bounding_box_x[1].X > output)
                            //    //        {
                            //    //            angle_3_5_dim.Add(p2);
                            //    //            angle_3_5_dim.Add(p1);


                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            angle_3_5_dim.Add(p1);
                            //    //            angle_3_5_dim.Add(p2);
                            //    //        }
                            //    //        double dist = Math.Abs(current_view.RestrictionBox.MaxPoint.Y) - Math.Abs(angle_3_5_dim[0].Y);
                            //    //        TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                            //    //        TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            //    //        att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            //    //        att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                            //    //        try
                            //    //        {
                            //    //            dim_for_3_5.CreateDimensionSet(current_view, angle_3_5_dim, new TSG.Vector(0, 1, 0), dist + 50 + Math.Abs(WT), att);
                            //    //        }
                            //    //        catch
                            //    //        {
                            //    //        }
                            //    //    }
                            //    //    if (Convert.ToInt64(bounding_box_y[1].Y) <= Convert.ToInt64(WT))
                            //    //    {
                            //    //        TSG.Point p1 = new TSG.Point();
                            //    //        p1 = new TSG.Point(bounding_box_y[1].X, bounding_box_y[1].Y, 0);
                            //    //        TSG.Point p2 = new TSG.Point();
                            //    //        p2 = new TSG.Point(bounding_box_y[0].X, bounding_box_y[1].Y, 0);
                            //    //        if (bounding_box_x[1].X > output)
                            //    //        {
                            //    //            angle_3_5_dim.Add(p2);
                            //    //            angle_3_5_dim.Add(p1);


                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            angle_3_5_dim.Add(p1);
                            //    //            angle_3_5_dim.Add(p2);
                            //    //        }
                            //    //        double dist = Math.Abs(current_view.RestrictionBox.MinPoint.Y) - Math.Abs(angle_3_5_dim[0].Y);
                            //    //        TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                            //    //        TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            //    //        att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            //    //        att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                            //    //        try
                            //    //        {
                            //    //            dim_for_3_5.CreateDimensionSet(current_view, angle_3_5_dim, new TSG.Vector(0, -1, 0), dist + 50 + Math.Abs(WT), att);
                            //    //        }
                            //    //        catch
                            //    //        {
                            //    //        }
                            //    //    }
                            //    //}

                            //    //else if ((Convert.ToInt64(angle_coord.AxisX.X) == 0))
                            //    //{
                            //    //    if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                            //    //    {
                            //    //        TSG.Point p1 = new TSG.Point();
                            //    //        p1 = new TSG.Point(bounding_box_y[1].X, bounding_box_y[0].Y, 0);
                            //    //        TSG.Point p2 = new TSG.Point();
                            //    //        p2 = new TSG.Point(bounding_box_y[0].X, bounding_box_y[0].Y, 0);

                            //    //        if (bounding_box_x[1].X > output)
                            //    //        {
                            //    //            angle_3_5_dim.Add(p2);
                            //    //            angle_3_5_dim.Add(p1);


                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            angle_3_5_dim.Add(p1);
                            //    //            angle_3_5_dim.Add(p2);
                            //    //        }
                            //    //        double dist = Math.Abs(current_view.RestrictionBox.MaxPoint.Y) - Math.Abs(angle_3_5_dim[0].Y);
                            //    //        TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                            //    //        TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            //    //        att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            //    //        att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;

                            //    //        try
                            //    //        {
                            //    //            dim_for_3_5.CreateDimensionSet(current_view, angle_3_5_dim, new TSG.Vector(0, 1, 0), dist + 50, att);
                            //    //        }
                            //    //        catch
                            //    //        {
                            //    //        }
                            //    //    }
                            //    //    if (Convert.ToInt64(bounding_box_y[1].Y) < 0)
                            //    //    {
                            //    //        TSG.Point p1 = new TSG.Point();
                            //    //        p1 = new TSG.Point(bounding_box_y[1].X, bounding_box_y[1].Y, 0);
                            //    //        TSG.Point p2 = new TSG.Point();
                            //    //        p2 = new TSG.Point(bounding_box_y[0].X, bounding_box_y[1].Y, 0);
                            //    //        if (bounding_box_x[1].X > output)
                            //    //        {
                            //    //            angle_3_5_dim.Add(p2);
                            //    //            angle_3_5_dim.Add(p1);


                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            angle_3_5_dim.Add(p1);
                            //    //            angle_3_5_dim.Add(p2);
                            //    //        }
                            //    //        double dist = Math.Abs(current_view.RestrictionBox.MinPoint.Y) - Math.Abs(angle_3_5_dim[0].Y);
                            //    //        TSD.StraightDimensionSetHandler dim_for_3_5 = new TSD.StraightDimensionSetHandler();
                            //    //        TSD.StraightDimensionSet.StraightDimensionSetAttributes att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            //    //        att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            //    //        att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                            //    //        try
                            //    //        {
                            //    //            dim_for_3_5.CreateDimensionSet(current_view, angle_3_5_dim, new TSG.Vector(0, -1, 0), dist + 50, att);
                            //    //        }
                            //    //        catch
                            //    //        {
                            //    //        }
                            //    //    }
                            //    //}
                            //    # endregion

                            //}



                        }
                    }

                    else
                    {
                        if (prof_type == "B")
                        {


                            if ((Convert.ToInt64(angle_check) == 0) || (Convert.ToInt64(angle_check) == 90) || (Convert.ToInt64(angle_check) == 180) || (Convert.ToInt64(angle_check) == 270) || (Convert.ToInt64(angle_check) == 360))
                            {

                            }
                            else
                            {
                                TSM.ModelObjectEnumerator platebolts = plate.GetBolts();
                                int a = platebolts.GetSize();
                                if (a > 0)
                                {
                                    List<TSG.Point> p1_yasc = facePointHandler.GetFacePointsForPlateTest(plate, current_view);
                                    p1_yasc.Distinct();
                                    List<TSG.Point> p_x_asc = new List<TSG.Point>();
                                    foreach (TSG.Point mypoint in p1_yasc)
                                    {
                                        p_x_asc.Add(mypoint);
                                    }

                                    p1_yasc.Distinct();
                                    p_x_asc.Distinct();

                                    sortingHandler.SortPoints(p1_yasc, SKSortingHandler.SortBy.Y);
                                    sortingHandler.SortPoints(p_x_asc, SKSortingHandler.
                                            SortBy.X, SKSortingHandler.SortOrder.Descending);
                                    //p1_yasc.Sort(new sort_by_y_value_asc());
                                    //p_x_asc.Sort(new sort_by_x_value_max());




                                    ptlist_for_attachments_top_plate_nearside.Add(p1_yasc[p1_yasc.Count - 1]);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);





                                    TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                                    fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                    fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                    z_vec_of_plate.Normalize();
                                    if (Convert.ToInt64(z_vec_of_plate.Y) > 0)
                                    {
                                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[p1_yasc.Count - 1], p_x_asc[0]) + 100) / view_scale;
                                        TSG.Vector myvector = new TSG.Vector(p_x_asc[0] - p1_yasc[p1_yasc.Count - 1]);
                                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[0], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                        angledim1.Insert();
                                        TSG.Vector vector_for_pitch = new TSG.Vector();
                                        List<TSG.Point> p1 = mypoint_for_bolt_skew(plate, current_view, out vector_for_pitch);
                                        List<TSG.Point> P2 = p1.Distinct(new REMOVING_DUPLICATE_Z_VALUE_IN_CURRENT_VIEW()).ToList();
                                        TSD.PointList mypt_list = new TSD.PointList();
                                        foreach (TSG.Point PT in P2)
                                        {
                                            mypt_list.Add(PT);

                                        }
                                        mypt_list.Add(p1_yasc[p1_yasc.Count - 1]);


                                        TSD.StraightDimensionSetHandler myset = new TSD.StraightDimensionSetHandler();
                                        myset.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list, vector_for_pitch, 100, dim_font_height);

                                    }
                                    else if (Convert.ToInt64(z_vec_of_plate.Y) < 0)
                                    {
                                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[p1_yasc.Count - 1], p_x_asc[p_x_asc.Count - 1]) + 100) / view_scale;

                                        TSG.Vector myvector = new TSG.Vector(p_x_asc[p_x_asc.Count - 1] - p1_yasc[p1_yasc.Count - 1]);
                                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[p_x_asc.Count - 1], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                        angledim1.Insert();
                                        TSG.Vector vector_for_pitch = new TSG.Vector();
                                        List<TSG.Point> p1 = mypoint_for_bolt_skew(plate, current_view, out vector_for_pitch);
                                        List<TSG.Point> P2 = p1.Distinct(new REMOVING_DUPLICATE_Z_VALUE_IN_CURRENT_VIEW()).ToList();
                                        TSD.PointList mypt_list = new TSD.PointList();
                                        foreach (TSG.Point PT in P2)
                                        {
                                            mypt_list.Add(PT);

                                        }
                                        TSD.StraightDimensionSetHandler myset = new TSD.StraightDimensionSetHandler();
                                        myset.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list, vector_for_pitch, 100, dim_font_height);
                                        /////corrrct/////////


                                    }

                                }
                                else
                                {
                                    List<TSG.Point> p1_yasc = facePointHandler.GetFacePointsForPlateTest(plate, current_view);
                                    p1_yasc.Distinct();
                                    List<TSG.Point> p_x_asc = new List<TSG.Point>();
                                    foreach (TSG.Point mypoint in p1_yasc)
                                    {
                                        p_x_asc.Add(mypoint);
                                    }

                                    p1_yasc.Distinct();
                                    p_x_asc.Distinct();

                                    sortingHandler.SortPoints(p1_yasc, SKSortingHandler.SortBy.Y);
                                    sortingHandler.SortPoints(p_x_asc, SKSortingHandler.
                                            SortBy.X, SKSortingHandler.SortOrder.Descending);
                                    //p1_yasc.Sort(new sort_by_y_value_asc());
                                    //p_x_asc.Sort(new sort_by_x_value_max());



                                    ptlist_for_attachments_top_plate_nearside.Add(p1_yasc[p1_yasc.Count - 1]);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);





                                    TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                                    fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                    fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                    z_vec_of_plate.Normalize();
                                    if (Convert.ToInt64(z_vec_of_plate.Y) > 0)
                                    {
                                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[p1_yasc.Count - 1], p_x_asc[0]) + 100) / view_scale;
                                        TSG.Vector myvector = new TSG.Vector(p_x_asc[0] - p1_yasc[p1_yasc.Count - 1]);
                                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[0], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                        angledim1.Insert();

                                    }
                                    else if (Convert.ToInt64(z_vec_of_plate.Y) < 0)
                                    {
                                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(p1_yasc[p1_yasc.Count - 1], p_x_asc[p_x_asc.Count - 1]) + 100) / view_scale;

                                        TSG.Vector myvector = new TSG.Vector(p_x_asc[p_x_asc.Count - 1] - p1_yasc[p1_yasc.Count - 1]);
                                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, p_x_asc[p_x_asc.Count - 1], myvector, new TSG.Vector(1, 0, 1), dist_for_anglular_dim, fi);
                                        angledim1.Insert();
                                    }
                                }
                            }
                        }


                    }
                }

                else if (prof_type == "L")
                {

                    if ((Convert.ToInt64(x_vec_of_plate.X) != 0) && (Convert.ToInt64(y_vec_of_plate.Y) != 0))
                    {
                        if ((angle_check == 0) || (angle_check == 90) || (angle_check == 180) || (angle_check == 360))
                        {
                            ////////////////////////////////2018/////////////////////////////////////////////////////
                            ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                            Guid ID = plate.Identifier.GUID;
                            PARTMARK_TO_RETAIN.Add(ID);
                        }

                    }
                    else
                    {
                        string angle_dim = CheckAngleDimensionNeeded(plate, current_view);

                        if (angle_dim == "NEED")
                        {

                            List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                            TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                            ptlist_for_attachments_top.Add(midpt_of_angle);
                            Guid ID = plate.Identifier.GUID;
                            PARTMARK_TO_RETAIN.Add(ID);
                        }



                    }


                }


            }
            double HT2 = (catalog_values[0]);
            double HT = Convert.ToInt64(HT2 / 2);
            double WTs = 0;
            if (profile_type == "U")
            {
                double WTs2 = (catalog_values[1]);
                WTs = Convert.ToInt64(WTs2 / 2);
            }
            else
            {
                WTs = 0;
            }
            ///////////////////////////////////////////FOR PLATE/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (ptlist_for_attachments_top.Count > 1)
            {
                TSD.PointList pt_list_z_positive = new TSD.PointList();
                foreach (TSG.Point pt in ptlist_for_attachments_top)
                {
                    if ((Convert.ToInt16(pt.X) > WTs) && (Convert.ToInt16(pt.X) < output))
                    {

                        if ((pt.Y > 0) && (Convert.ToInt64(pt.Z) < HT))
                        {

                            pt_list_z_positive.Add(pt);
                        }
                    }
                }



                TSD.PointList final_ptlist_for_attachments_top = duplicateRemover.RemoveDuplicateXValues(pt_list_z_positive);
                final_ptlist_for_attachments_top.Insert(0, (new TSG.Point(0, 0, 0)));
                sortingHandler.SortPoints(final_ptlist_for_attachments_top);

                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_ptlist_for_attachments_top, new TSG.Vector(0, 1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 225, RDATT);
                }
                catch
                {
                }

                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_attachments_top_plate_nearside, new TSG.Vector(0, -1, 0), Math.Abs(current_view.RestrictionBox.MinPoint.Y) + 100, RDATT);
                }
                catch
                {
                }
            }
            ///////////////////////////////////END FOR PLATE ///////////////////////////////////////////////////////////////////////////////
        }
        public void CreateDimensionOutsideFlange(TSM.Beam main_part, TSD.View current_view, double output, ref List<Guid> PARTMARK_TO_RETAIN, string drg_att)
        {


            List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
            double top_front;
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
            {
                top_front = (catalog_values[0]);
            }
            else
            {
                top_front = (catalog_values[1]);
            }

            TSM.Model model = new TSM.Model();
            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            TSD.StraightDimensionSetHandler dim_set_handler = new TSD.StraightDimensionSetHandler();
            TSD.PointList ptlist_for_attachments_top = new TSD.PointList();
            ptlist_for_attachments_top.Add(new TSG.Point(0, 0, 0));
            TSD.PointList ptlist_for_attachments_bottom = new TSD.PointList();
            TSD.PointList bottom_platefilter = new TSD.PointList();
            TSD.PointList top_platefilter = new TSD.PointList();
            ptlist_for_attachments_bottom.Add(new TSG.Point(0, 0, 0));
            TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(typeof(TSD.Part));

            while (enum_for_parts_drg.MoveNext())
            {
                TSD.Part mypart = enum_for_parts_drg.Current as TSD.Part;

                TSM.Part plate = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.Part;


                string prof_type = "";

                plate.GetReportProperty("PROFILE_TYPE", ref prof_type);
                //////////////////////////////////Filtering all the plates////////////////////////////////

                TSD.PointList bounding_box_y = boundingBoxHandler.BoundingBoxSort(plate, current_view, SortBy.Y);
                TSD.PointList bounding_box_x = boundingBoxHandler.BoundingBoxSort(plate, current_view);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                TSG.Vector x_vec_of_plate = plate.GetCoordinateSystem().AxisX;
                TSG.Vector y_vec_of_plate = plate.GetCoordinateSystem().AxisY;
                TSG.Vector z_vec_of_plate = x_vec_of_plate.Cross(y_vec_of_plate);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                double angle_check = SKUtility.RadianToDegree((z_vec_of_plate.GetAngleBetween(new TSG.Vector(1, 0, 0))));
                string profile = plate.Profile.ProfileString;

                //if(plate.Name.Contains("FALLTECH")==false)
                {
                    if ((plate.GetType().Equals(typeof(TSM.PolyBeam))))
                    {
                        if ((y_vec_of_plate.X != 0) && (!profile.Contains("BOLT")))
                        {


                            if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                            {
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                                ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[1].Y, bounding_box_x[0].Z));


                            }
                            else if ((Convert.ToInt64(bounding_box_y[1].Y) <= -Convert.ToInt64(top_front / 2)))
                            {
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                                ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[1].Y, bounding_box_x[0].Z));

                            }
                        }


                        #region bolt
                        TSD.DrawingObjectEnumerator enum_for_bolt = current_view.GetAllObjects(typeof(TSD.Bolt));
                        TSD.PointList rd_point_list = new TSD.PointList();
                        TSG.Matrix top_mat = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                        ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
                        while (enum_for_bolt.MoveNext())
                        {
                            TSD.Bolt drgbolt = enum_for_bolt.Current as TSD.Bolt;
                            TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                            TSM.BoltArray bolt = modelbolt as TSM.BoltArray;


                            if (top_mat.Transform((bolt.BoltPositions[0]) as TSG.Point).Y > Convert.ToInt64(top_front / 2))
                            {
                                TSD.Bolt drgbolt1 = drgbolt;
                                TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, current_view);
                                if (POINT_FOR_BOLT_MATRIX != null)
                                {
                                    int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                    int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                    for (int i = 0; i < x; i++)
                                    {
                                        ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                        rd_point_list.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                                    }
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);
                                }
                            }
                        }
                        rd_point_list.Add(new TSG.Point(0, 0, 0));

                        sortingHandler.SortPoints(rd_point_list);


                        // TSD.PointList FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);
                        /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
                        TSD.StraightDimensionSetHandler bolt_rd_dim = new TSD.StraightDimensionSetHandler();

                        try
                        {
                            ////////////////////////////////////////////////////dimension distance placing linking ////////////////////////////////////////////////////////////////////////////////////////////////                
                            //double distance = Convert.ToDouble(top_front) / 2;
                            // TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point);
                            // TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                            //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                            ///////////////////////////////////////////////////rd dimension creation///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                            rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            rd_att.Color = DrawingColors.Gray70;
                            rd_att.Text.Font.Color = DrawingColors.Gray70;
                            rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                            rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                            //FINAL_RD_LIST.Add(new TSG.Point(0, 0, 0));
                            //sortingHandler.SortPoints(FINAL_RD_LIST);
                            //  bolt_rd_dim.CreateDimensionSet(current_view, rd_point_list, new TSG.Vector(0, 1, 0), 200 + 200, rd_att);

                        }
                        catch
                        {
                        }
                        #endregion







                    }
                    else if (prof_type == "B")
                    {

                        if ((Convert.ToInt64(angle_check) == 0) || (Convert.ToInt64(angle_check) == 90) || (Convert.ToInt64(angle_check) == 180) || (Convert.ToInt64(angle_check) == 270) || (Convert.ToInt64(angle_check) == 360))
                        {

                            TSM.ModelObjectEnumerator platebolts = plate.GetBolts();

                            int a = platebolts.GetSize();
                            if (a > 0)
                            {
                                TSD.PointList p1 = boundingBoxHandler.BoundingBoxSort(plate, current_view);


                                while (platebolts.MoveNext())
                                {
                                    TSM.BoltGroup bolt = platebolts.Current as TSM.BoltGroup;
                                    TSG.CoordinateSystem m = bolt.GetCoordinateSystem();
                                    TSM.Part mw = bolt.PartToBeBolted;
                                    TSM.Part mw1 = bolt.PartToBoltTo;
                                    ArrayList mw2 = bolt.OtherPartsToBolt;

                                    if (!mw.Identifier.ID.Equals(plate.Identifier.ID))
                                    {
                                        //TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                        //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                        //TSG.Point pz = tokkk.Transform(kl.Origin);
                                        //double x_value = pz.X;

                                        double x_value = 0;
                                        string prof_type_for_channel_check = "";
                                        mw.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                        if (prof_type_for_channel_check != "U")
                                        {

                                            TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                            TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                            TSG.Point pz = tokkk.Transform(kl.Origin);
                                            x_value = pz.X;
                                        }
                                        else
                                        {
                                            TSG.CoordinateSystem channel_coord = mw.GetCoordinateSystem();
                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                            List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw);
                                            double offset = (catalog_values_for_channel[1]);
                                            TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                            kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                            TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model,  current_view);


                                            x_value = pz.X;

                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                        }
                                        if (isRdConnectMark)
                                        {

                                            bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                            bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                            if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                            {
                                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {

                                                        ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }

                                                }

                                            }



                                        }
                                        else
                                        {

                                            bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                            bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                            if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                            {
                                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);
                                                }
                                                else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                                {
                                                    ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);
                                                }


                                            }
                                            else if ((Convert.ToInt64(bounding_box_y[0].Y) > top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) > top_front / 2))
                                            {
                                                ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                Guid ID = plate.Identifier.GUID;
                                                PARTMARK_TO_RETAIN.Add(ID);

                                            }
                                            else if ((Convert.ToInt64(bounding_box_y[0].Y) < -top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) < -top_front / 2))
                                            {
                                                ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                Guid ID = plate.Identifier.GUID;
                                                PARTMARK_TO_RETAIN.Add(ID);

                                            }



                                        }

                                    }
                                    if (!mw1.Identifier.ID.Equals(plate.Identifier.ID))
                                    {
                                        //TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();

                                        //TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                        //TSG.Point pz = tokkk.Transform(kl.Origin);
                                        //double x_value = pz.X;



                                        double x_value = 0;
                                        string prof_type_for_channel_check = "";
                                        mw1.GetReportProperty("PROFILE_TYPE", ref prof_type_for_channel_check);

                                        if (prof_type_for_channel_check != "U")
                                        {
                                            TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                            TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();


                                            TSG.Point pz = tokkk.Transform(kl.Origin);
                                            x_value = pz.X;
                                        }
                                        else
                                        {
                                            TSG.CoordinateSystem channel_coord = mw1.GetCoordinateSystem();
                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(channel_coord));
                                            List<double> catalog_values_for_channel = catalogHandler.Getcatalog_values(mw1);
                                            double offset = (catalog_values_for_channel[1]);
                                            TSG.CoordinateSystem kl = new TSG.CoordinateSystem();
                                            kl.Origin = new TSG.Point(0, 0, -offset / 2);



                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(kl));


                                            TSG.Point pz = drawingHandler.ConvertedPointsForChannel(model, current_view);


                                            x_value = pz.X;

                                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                        }


                                        if (isRdConnectMark)
                                        {
                                            bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                            bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                            if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                            {
                                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                }
                                                else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                                {
                                                    if (Convert.ToInt64(x_value) >= Convert.ToInt64(p1[1].X))
                                                    {
                                                        ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }
                                                    else if (Convert.ToInt64(x_value) <= Convert.ToInt64(p1[0].X))
                                                    {
                                                        ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                        Guid ID = plate.Identifier.GUID;
                                                        PARTMARK_TO_RETAIN.Add(ID);
                                                    }

                                                }
                                            }



                                        }
                                        else
                                        {
                                            bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                            bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                            if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                            {
                                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                                {
                                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);
                                                }
                                                else if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                                {
                                                    ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                    Guid ID = plate.Identifier.GUID;
                                                    PARTMARK_TO_RETAIN.Add(ID);
                                                }
                                            }

                                            else if ((Convert.ToInt64(bounding_box_y[0].Y) > top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) > top_front / 2))
                                            {
                                                ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                Guid ID = plate.Identifier.GUID;
                                                PARTMARK_TO_RETAIN.Add(ID);

                                            }
                                            else if ((Convert.ToInt64(bounding_box_y[0].Y) < -top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) < -top_front / 2))
                                            {
                                                ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                                Guid ID = plate.Identifier.GUID;
                                                PARTMARK_TO_RETAIN.Add(ID);

                                            }


                                        }
                                    }

                                    //else 
                                    //{
                                    //    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                    //}

                                }
                            }

                            else
                            {
                                bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                {
                                    if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                    {
                                        ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                    else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                    {
                                        ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                }
                                else if ((Convert.ToInt64(bounding_box_y[0].Y) > top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) > top_front / 2))
                                {
                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                                else if ((Convert.ToInt64(bounding_box_y[0].Y) < -top_front / 2) && (Convert.ToInt64(bounding_box_y[1].Y) < -top_front / 2))
                                {
                                    ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, bounding_box_y[0].Z));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }



                            }

                        }
                        else
                        {

                        }
                    }

                    else if (((prof_type == "L")))
                    {

                        if ((Convert.ToInt64(bounding_box_x[0].X) >= 0) && ((Convert.ToInt64(bounding_box_x[1].X)) < output + 1))
                        {


                            TSM.ModelObjectEnumerator boltenum = plate.GetBolts();
                            List<string> string_for_bolt_check = new List<string>();
                            int a = boltenum.GetSize();
                            if (a >= 1)
                            {
                                while (boltenum.MoveNext())
                                {
                                    TSM.BoltGroup bolt = boltenum.Current as TSM.BoltGroup;

                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                    TSG.CoordinateSystem bolt_coord = bolt.GetCoordinateSystem();
                                    TSG.Vector x_vec_of_bolt = bolt_coord.AxisX;
                                    TSG.Vector y_vec_of_bolt = bolt_coord.AxisY;
                                    TSG.Vector z_vec_of_bolt = x_vec_of_bolt.Cross(y_vec_of_bolt);
                                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                    if (Convert.ToInt64(z_vec_of_bolt.Z) != 0)
                                    {
                                        string_for_bolt_check.Add("YES");
                                    }
                                    else
                                    {
                                        string_for_bolt_check.Add("NO");
                                    }
                                }
                                bool check = string_for_bolt_check.Any(x => x.Contains("YES"));
                                if (check == true)
                                {

                                }
                                else
                                {

                                    //string angle_dim = check_for_angle_dim(plate, current_view);

                                    //if (angle_dim == "NEED")
                                    //{

                                    //    List<angle_face_area> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                    //    TSG.Point midpt_of_angle = angle_mid_pt(myreq, current_view);
                                    //    ptlist_for_attachments_top.Add(midpt_of_angle);
                                    //    Guid ID = plate.Identifier.GUID;
                                    //    PARTMARK_TO_RETAIN.Add(ID);
                                    //}



                                    bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                    bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                    if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                    {
                                        if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                        {
                                            //ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint(plate, current_view));
                                            //Guid ID = plate.Identifier.GUID;
                                            //PARTMARK_TO_RETAIN.Add(ID);

                                            List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                            TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                                            ptlist_for_attachments_top.Add(midpt_of_angle);

                                            //ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);

                                        }
                                        else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                        {
                                            //ptlist_for_attachments_bottom.Add(facePointHandler.GetFacePoint(plate, current_view));
                                            //Guid ID = plate.Identifier.GUID;
                                            //PARTMARK_TO_RETAIN.Add(ID);
                                            List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                            TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                                            ptlist_for_attachments_bottom.Add(midpt_of_angle);

                                            //ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);

                                        }

                                    }



                                }

                            }

                            else
                            {

                                bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                {
                                    if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                    {
                                        List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                        TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                                        ptlist_for_attachments_top.Add(midpt_of_angle);

                                        //ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                    else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                    {
                                        List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(plate);
                                        TSG.Point midpt_of_angle = FindAngleMidPoint(myreq, current_view);
                                        ptlist_for_attachments_bottom.Add(midpt_of_angle);
                                        //ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }

                                }
                                //if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                                //{
                                //    ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint(plate, current_view));
                                //    Guid ID = plate.Identifier.GUID;
                                //    PARTMARK_TO_RETAIN.Add(ID);
                                //}
                                //else if ((Convert.ToInt64(bounding_box_y[1].Y) <= -Convert.ToInt64(top_front / 2)))
                                //{
                                //    ptlist_for_attachments_bottom.Add(facePointHandler.GetFacePoint(plate, current_view));
                                //    Guid ID = plate.Identifier.GUID;
                                //    PARTMARK_TO_RETAIN.Add(ID);
                                //}
                                //else
                                //{

                                //}
                            }

                        }

                        else if (Convert.ToInt64(bounding_box_x[0].X) < 0)
                        {

                            TSD.PointList mypt_list_for_angle_top = new TSD.PointList();
                            TSD.PointList mypt_list_for_angle_bottom = new TSD.PointList();
                            mypt_list_for_angle_bottom.Add(new TSG.Point(0, 0, 0));
                            mypt_list_for_angle_top.Add(new TSG.Point(0, 0, 0));
                            if (Convert.ToInt64(bounding_box_x[0].Y) > 0)
                            {

                                mypt_list_for_angle_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if (Convert.ToInt64(bounding_box_x[1].Y) < 0)
                            {
                                mypt_list_for_angle_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[1].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }



                            try
                            {
                                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list_for_angle_top, new TSG.Vector(0, 1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 53, rr);


                            }
                            catch
                            {
                            }

                            try
                            {
                                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;

                                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list_for_angle_bottom, new TSG.Vector(0, -1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 10, rr);
                            }
                            catch
                            {
                            }

                        }

                        else if (Convert.ToInt64(bounding_box_x[1].X) > output)
                        {
                            TSD.PointList mypt_list_for_angle_top = new TSD.PointList();
                            TSD.PointList mypt_list_for_angle_bottom = new TSD.PointList();
                            mypt_list_for_angle_bottom.Add(new TSG.Point(output, 0, 0));
                            mypt_list_for_angle_top.Add(new TSG.Point(output, 0, 0));
                            if (Convert.ToInt64(bounding_box_x[0].Y) > 0)
                            {
                                mypt_list_for_angle_top.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_x[0].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if (Convert.ToInt64(bounding_box_x[1].Y) < 0)
                            {
                                mypt_list_for_angle_bottom.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_x[1].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }



                            try
                            {
                                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list_for_angle_top, new TSG.Vector(0, 1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 53, rr);


                            }
                            catch
                            {
                            }

                            try
                            {
                                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;

                                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, mypt_list_for_angle_bottom, new TSG.Vector(0, -1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 10, rr);
                            }
                            catch
                            {
                            }
                        }






                    }




                    else if (prof_type == "T")
                    {
                        TSM.ModelObjectEnumerator boltenum = plate.GetBolts();

                        int a = boltenum.GetSize();
                        if (a >= 1)
                        {
                            while (boltenum.MoveNext())
                            {
                                TSM.BoltGroup bolt = boltenum.Current as TSM.BoltGroup;

                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                TSG.CoordinateSystem bolt_coord = bolt.GetCoordinateSystem();
                                TSG.Vector x_vec_of_bolt = bolt_coord.AxisX;
                                TSG.Vector y_vec_of_bolt = bolt_coord.AxisY;
                                TSG.Vector z_vec_of_bolt = x_vec_of_bolt.Cross(y_vec_of_bolt);
                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());




                                if (Convert.ToInt64(z_vec_of_bolt.Z) != 0)
                                {
                                }
                            }
                        }

                        else
                        {

                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                            TSG.Vector x_vec_of_hss = plate.GetCoordinateSystem().AxisX;
                            TSG.Vector y_vec_of_hss = plate.GetCoordinateSystem().AxisY;
                            TSG.Vector z_vec_of_hss = x_vec_of_hss.Cross(y_vec_of_hss);
                            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());






                            if (Convert.ToInt64(x_vec_of_hss.X) == 0)
                            {






                                bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                                bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                                if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                                {
                                    if (bounding_box_y[0].Y > 0)
                                    {

                                        List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForTProfile(plate);
                                        TSG.Point midpt_of_angle = facePointHandler.GetTProfileMidPoint(myreq, current_view);


                                        double X = ((bounding_box_y[0].X + bounding_box_y[1].X) / 2);
                                        TSG.Point P1 = new TSG.Point(X, bounding_box_y[1].Y, 0);



                                        //ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint_FOR_T(plate, current_view));
                                        ptlist_for_attachments_top.Add(midpt_of_angle);
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                    else if (bounding_box_y[0].Y < 0)
                                    {
                                        List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForTProfile(plate);
                                        TSG.Point midpt_of_angle = facePointHandler.GetTProfileMidPoint(myreq, current_view);


                                        double X = ((bounding_box_y[0].X + bounding_box_y[1].X) / 2);
                                        TSG.Point P1 = new TSG.Point(X, bounding_box_y[0].Y, 0);


                                        ptlist_for_attachments_bottom.Add(midpt_of_angle);
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);

                                    }
                                }

                            }
                            else
                            {
                                if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                                {

                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);
                                }
                                else if ((Convert.ToInt64(bounding_box_y[1].Y) <= -Convert.ToInt64(top_front / 2)))
                                {

                                    ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);
                                }

                            }


                        }

                    }
                    else if (prof_type == "U")
                    {

                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector x_vec_of_hss = plate.GetCoordinateSystem().AxisX;
                        TSG.Vector y_vec_of_hss = plate.GetCoordinateSystem().AxisY;
                        TSG.Vector z_vec_of_hss = x_vec_of_hss.Cross(y_vec_of_hss);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());






                        if (Convert.ToInt64(x_vec_of_hss.X) == 0)
                        {






                            bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                            bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                            if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                            {
                                if (bounding_box_y[0].Y > 0)
                                {

                                    List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForChannel(plate);
                                    TSG.Point midpt_of_angle = facePointHandler.GetTProfileMidPoint(myreq, current_view);


                                    double X = ((bounding_box_y[0].X + bounding_box_y[1].X) / 2);
                                    TSG.Point P1 = new TSG.Point(X, bounding_box_y[1].Y, 0);



                                    //ptlist_for_attachments_top.Add(facePointHandler.GetFacePoint_FOR_T(plate, current_view));
                                    ptlist_for_attachments_top.Add(midpt_of_angle);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);
                                }
                                else if (bounding_box_y[0].Y < 0)
                                {
                                    List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForChannel(plate);
                                    TSG.Point midpt_of_angle = facePointHandler.GetTProfileMidPoint(myreq, current_view);


                                    double X = ((bounding_box_y[0].X + bounding_box_y[1].X) / 2);
                                    TSG.Point P1 = new TSG.Point(X, bounding_box_y[0].Y, 0);


                                    ptlist_for_attachments_bottom.Add(midpt_of_angle);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                            }

                        }
                        else
                        {
                            if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                            {

                                ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if ((Convert.ToInt64(bounding_box_y[1].Y) <= -Convert.ToInt64(top_front / 2)))
                            {

                                ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }

                        }


                    }
                    else if ((profile.Contains("HSS")) || (profile.Contains("NUT")) || (profile.Contains("PIPE")))
                    {

                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector x_vec_of_hss = plate.GetCoordinateSystem().AxisX;
                        TSG.Vector y_vec_of_hss = plate.GetCoordinateSystem().AxisY;
                        TSG.Vector z_vec_of_hss = x_vec_of_hss.Cross(y_vec_of_hss);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                        bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                        bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                        if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                        {
                            if ((x_vec_of_hss.X != 0) && profile.Contains("HSS"))
                            {
                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                {
                                    ptlist_for_attachments_top.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                                else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                {
                                    ptlist_for_attachments_bottom.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_x[0].Y, 0));
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                            }
                            else
                            {
                                List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForTProfile(plate);
                                TSG.Point midpt_of_angle = facePointHandler.GetTProfileMidPoint(myreq, current_view);



                                if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                                {
                                    TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[1].Y, 0);

                                    ptlist_for_attachments_top.Add(midpt_of_angle);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                                else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                                {
                                    TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[0].Y, 0);
                                    ptlist_for_attachments_bottom.Add(MIDPT);
                                    Guid ID = plate.Identifier.GUID;
                                    PARTMARK_TO_RETAIN.Add(ID);

                                }
                            }



                        }


                    }
                    else if ((plate.GetType().Equals(typeof(TSM.Beam))) && (profile.Contains("STUD")) && plate.Name.Contains("FALLTECH") == false)
                    {

                        bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                        bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                        if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                        {
                            if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                            {
                                TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[1].Y, 0);

                                ptlist_for_attachments_top.Add(MIDPT);
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                            {
                                TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[0].Y, 0);

                                ptlist_for_attachments_bottom.Add(MIDPT);
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                        }




                    }
                    else if (prof_type == "Z")
                    {

                        bool RESULT_FOR_TOP = SKUtility.AlmostEqual(bounding_box_y[0].Y, top_front / 2);
                        bool RESULT_FOR_BOT = SKUtility.AlmostEqual(bounding_box_y[1].Y, top_front / 2);
                        if ((RESULT_FOR_TOP == true) || (RESULT_FOR_BOT == true))
                        {
                            if (Convert.ToInt64(bounding_box_y[0].Y) > 0)
                            {
                                TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[1].Y, 0);

                                ptlist_for_attachments_top.Add(MIDPT);
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                            else if (Convert.ToInt64(bounding_box_y[0].Y) < 0)
                            {
                                TSG.Point MIDPT = new TSG.Point(bounding_box_x[0].X / 2 + bounding_box_x[1].X / 2, bounding_box_x[0].Y, 0);

                                ptlist_for_attachments_bottom.Add(MIDPT);
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                        }

                    }
                    else
                    {
                        TSM.ModelObjectEnumerator boltenum = plate.GetBolts();

                        int a = boltenum.GetSize();
                        if (a >= 1)
                        {
                            while (boltenum.MoveNext())
                            {
                                TSM.BoltGroup bolt = boltenum.Current as TSM.BoltGroup;

                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                TSG.CoordinateSystem bolt_coord = bolt.GetCoordinateSystem();
                                TSG.Vector x_vec_of_bolt = bolt_coord.AxisX;
                                TSG.Vector y_vec_of_bolt = bolt_coord.AxisY;
                                TSG.Vector z_vec_of_bolt = x_vec_of_bolt.Cross(y_vec_of_bolt);
                                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());




                                if (Convert.ToInt64(z_vec_of_bolt.Z) != 0)
                                {
                                    if ((Convert.ToInt16(bounding_box_x[0].X) > Convert.ToInt16(output)))
                                    {
                                        top_platefilter.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[1].Y, 0));

                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);

                                    }
                                    else if (Convert.ToInt16(bounding_box_x[1].X) < 0)
                                    {
                                        bottom_platefilter.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[0].Y, 0));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }

                                }
                                else
                                {
                                    if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                                    {
                                        if (prof_type == "B")
                                        {
                                            ptlist_for_attachments_top.Add(bounding_box_y[0]);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);

                                        }
                                        else
                                        {
                                            ptlist_for_attachments_top.Add(bounding_box_y[1]);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                        }
                                    }
                                    else if ((Convert.ToInt64(bounding_box_y[1].Y) <= -Convert.ToInt64(top_front / 2)))
                                    {
                                        if (prof_type == "B")
                                        {

                                            ptlist_for_attachments_bottom.Add(bounding_box_y[0]);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);

                                        }
                                        else
                                        {

                                            ptlist_for_attachments_bottom.Add(bounding_box_y[1]);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                        }

                                    }
                                    else if ((Convert.ToInt64(bounding_box_x[1].X) > Convert.ToInt64(output)))
                                    {
                                        top_platefilter.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, 0));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);

                                    }
                                    else if (Convert.ToInt64(bounding_box_x[0].X) < 0)
                                    {
                                        bottom_platefilter.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, 0));
                                        Guid ID = plate.Identifier.GUID;
                                        PARTMARK_TO_RETAIN.Add(ID);
                                    }
                                    break;

                                }


                            }
                        }


                        else if ((Convert.ToInt64(bounding_box_x[1].X) > Convert.ToInt64(output)))
                        {
                            top_platefilter.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, 0));
                            Guid ID = plate.Identifier.GUID;
                            PARTMARK_TO_RETAIN.Add(ID);


                        }
                        else if (Convert.ToInt64(bounding_box_x[0].X) < 0)
                        {
                            bottom_platefilter.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, 0));
                            Guid ID = plate.Identifier.GUID;
                            PARTMARK_TO_RETAIN.Add(ID);
                        }




                    }
                }

            }
            double distance = (catalog_values[0]);
            TSD.StraightDimensionSet.StraightDimensionSetAttributes rd = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            rd.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
            rd.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            rd.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            rd.Arrowhead.Head = ArrowheadTypes.FilledArrow;
            rd.Text.Font.Color = DrawingColors.Gray70;
            rd.Color = DrawingColors.Gray70;
            rd.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            TSD.PointList final_ptlist_for_attachments_top = new TSD.PointList();


            double HT2 = (catalog_values[0]);
            double HT = Convert.ToInt64(HT2 / 2);


            TSD.PointList pt_list_z_positive = new TSD.PointList();


            //if (profile_type == "U")
            //{
            //    double WT2 = (catalog_values[1]);
            //    double WT = Convert.ToInt64(WT2 / 2);
            //    foreach (TSG.Point pt in ptlist_for_attachments_top)
            //    {
            //        if (pt != null)
            //        {
            //            if ((Convert.ToInt16(pt.X) > 0) && (Convert.ToInt16(pt.X) < output))
            //            {

            //                if (Convert.ToInt16(pt.Z) > -WT)
            //                {
            //                    pt_list_z_positive.Add(pt);
            //                }
            //            }
            //        }
            //    }

            //}
            //else
            //{
            //    foreach (TSG.Point pt in ptlist_for_attachments_top)
            //    {
            //        if (pt != null)
            //        {
            //            if ((Convert.ToInt16(pt.X) > 0) && (Convert.ToInt16(pt.X) < output))
            //            {

            //                if (Convert.ToInt16(pt.Z) > 0)
            //                {
            //                    pt_list_z_positive.Add(pt);
            //                }
            //            }
            //        }
            //    }
            //}




            foreach (TSG.Point pt in ptlist_for_attachments_top)
            {
                if (pt != null)
                {
                    final_ptlist_for_attachments_top.Add(pt);

                }
            }

            TSD.PointList final_ptlist_for_attachments_bottom = new TSD.PointList();
            foreach (TSG.Point pt in ptlist_for_attachments_bottom)
            {
                if (pt != null)
                {
                    final_ptlist_for_attachments_bottom.Add(pt);

                }
            }

            try
            {
                double MAXY = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);

                double PART_DISTANCE_FOR_OUTSIDE_FLANGE = Math.Abs(MAXY - final_ptlist_for_attachments_top[0].Y);

                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rr.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute;
                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rr.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rr.Text.Font.Color = DrawingColors.Gray70;
                rr.Color = DrawingColors.Gray70;
                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_ptlist_for_attachments_top, new TSG.Vector(0, 1, 0), PART_DISTANCE_FOR_OUTSIDE_FLANGE + 170, rr);


            }
            catch
            {
            }

            try
            {
                double MINY = Math.Abs(current_view.RestrictionBox.MinPoint.Y);

                double PART_DISTANCE_FOR_OUTSIDE_FLANGE = Math.Abs(MINY - final_ptlist_for_attachments_top[0].Y);
                TSD.StraightDimensionSet.StraightDimensionSetAttributes rr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rr.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute;
                rr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rr.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rr.Text.Font.Color = DrawingColors.Gray70;
                rr.Color = DrawingColors.Gray70;
                rr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_ptlist_for_attachments_bottom, new TSG.Vector(0, -1, 0), PART_DISTANCE_FOR_OUTSIDE_FLANGE + 55, rr);
            }
            catch
            {
            }


            try
            {
                //top_platefilter.Add(new TSG.Point(output, top_platefilter[0].Y, 0));
                //TSD.StraightDimensionSet.StraightDimensionSetAttributes ouside = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                //ouside.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Outside;
                //dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, top_platefilter, new TSG.Vector( 0, 1,0), distance + 200, ouside);
            }
            catch
            {
            }
            try
            {
                //bottom_platefilter.Add(new TSG.Point(0, bottom_platefilter[0].Y, 0));
                //TSD.StraightDimensionSet.StraightDimensionSetAttributes ouside = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                //ouside.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Outside;
                //dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, bottom_platefilter, new TSG.Vector( 0,1, 0), distance + 200, ouside);
                //Create_elevation_Dimension(new TSG.Point(0, bottom_platefilter[0].Y, 0), current_view, 500);
            }
            catch
            {
            }



            # region bolt
            TSD.DrawingObjectEnumerator enum_for_boltATT = current_view.GetAllObjects(typeof(TSD.Bolt));
            TSD.PointList rd_point_list_ATT = new TSD.PointList();
            TSG.Matrix top_mat_ATT = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
            while (enum_for_boltATT.MoveNext())
            {
                TSD.Bolt drgbolt = enum_for_boltATT.Current as TSD.Bolt;
                TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                TSM.BoltGroup bolt1 = modelbolt as TSM.BoltGroup;
                int NO_OF_BOLT = bolt1.BoltPositions.Count;
                if (NO_OF_BOLT > 0)
                {


                    if (top_mat_ATT.Transform((bolt1.BoltPositions[0]) as TSG.Point).Y > Convert.ToInt64(top_front / 2))
                    {
                        TSD.Bolt drgbolt1 = drgbolt;
                        TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, current_view);
                        if (POINT_FOR_BOLT_MATRIX != null)
                        {
                            int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                            int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                            for (int i = 0; i < x; i++)
                            {
                                ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                rd_point_list_ATT.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                            }
                        }
                    }
                }
            }
            rd_point_list_ATT.Add(new TSG.Point(0, 0, 0));

            sortingHandler.SortPoints(rd_point_list_ATT);


            // TSD.PointList FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);
            /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
            TSD.StraightDimensionSetHandler bolt_rd_dim_ATTACHMENTS = new TSD.StraightDimensionSetHandler();

            try
            {
                ////////////////////////////////////////////////////dimension distance placing linking ////////////////////////////////////////////////////////////////////////////////////////////////                
                //double distance = Convert.ToDouble(top_front) / 2;
                // TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point);
                // TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                ///////////////////////////////////////////////////rd dimension creation///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rd_att.Text.Font.Color = DrawingColors.Gray70;
                rd_att.Color = DrawingColors.Gray70;
                rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                //FINAL_RD_LIST.Add(new TSG.Point(0, 0, 0));
                //sortingHandler.SortPoints(FINAL_RD_LIST);
                bolt_rd_dim_ATTACHMENTS.CreateDimensionSet(current_view, rd_point_list_ATT, new TSG.Vector(0, 1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 150, rd_att);

            }
            catch
            {
            }
            # endregion

            # region bolt
            TSD.DrawingObjectEnumerator enum_for_boltATT2 = current_view.GetAllObjects(typeof(TSD.Bolt));
            TSD.PointList rd_point_list_ATT2 = new TSD.PointList();
            TSG.Matrix top_mat_ATT2 = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
            while (enum_for_boltATT2.MoveNext())
            {
                TSD.Bolt drgbolt = enum_for_boltATT2.Current as TSD.Bolt;
                TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                TSM.BoltGroup bolt1 = modelbolt as TSM.BoltGroup;

                int NO_OF_BOLT1 = bolt1.BoltPositions.Count;

                if (NO_OF_BOLT1 > 0)
                {
                    if (top_mat_ATT2.Transform((bolt1.BoltPositions[0]) as TSG.Point).Y < -Convert.ToInt64(top_front / 2))
                    {
                        TSD.Bolt drgbolt1 = drgbolt;
                        TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, current_view);
                        if (POINT_FOR_BOLT_MATRIX != null)
                        {
                            int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                            int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                            for (int i = 0; i < x; i++)
                            {
                                ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                rd_point_list_ATT2.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                            }
                        }
                    }
                }
            }
            rd_point_list_ATT2.Add(new TSG.Point(0, 0, 0));

            sortingHandler.SortPoints(rd_point_list_ATT2);


            // TSD.PointList FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);
            /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
            TSD.StraightDimensionSetHandler bolt_rd_dim_ATTACHMENTS2 = new TSD.StraightDimensionSetHandler();

            try
            {
                ////////////////////////////////////////////////////dimension distance placing linking ////////////////////////////////////////////////////////////////////////////////////////////////                
                //double distance = Convert.ToDouble(top_front) / 2;
                // TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point);
                // TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                ///////////////////////////////////////////////////rd dimension creation///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rd_att.Text.Font.Color = DrawingColors.Gray70;
                rd_att.Color = DrawingColors.Gray70;
                rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                //FINAL_RD_LIST.Add(new TSG.Point(0, 0, 0));
                //sortingHandler.SortPoints(FINAL_RD_LIST);
                bolt_rd_dim_ATTACHMENTS2.CreateDimensionSet(current_view, rd_point_list_ATT2, new TSG.Vector(0, -1, 0), Math.Abs(current_view.RestrictionBox.MinPoint.Y) + 150, rd_att);

            }
            catch
            {
            }
            # endregion

        }
    
        private string CheckAngleDimensionNeeded(TSM.Part anglepart, TSD.View currentview)
        {

            TSM.Model mymodel = new TSM.Model();
            List<string> list_of_bolt = new List<string>();
            string angle_dim = "NEED";
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
                if (Convert.ToInt64(zaxis.Z) != 0)
                {
                    angle_dim = "NOT_NEED";
                    break;

                }
 
                //if (Convert.ToInt64(zaxis.Z) != 0)
                //{

                //    list_of_bolt.Add("TRUE");

                //}
                //else
                //{
                //    list_of_bolt.Add("FALSE");

                //}
            }


            //bool RESULT = list_of_bolt.Any(X => X.Equals("TRUE"));
            
            //if (RESULT == true)
            //{
            //    angle_dim = "NOT_NEED";

            //}
            //else
            //{
            //    angle_dim = "NEED";

            //}

            return angle_dim;
        }


        private TSG.Point FindAngleMidPoint(List<AngleFaceArea> myreq, TSD.View current_view)
        {
            TSM.Model mymodel = new TSM.Model();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            myreq.RemoveAll(x => x.VectorType.Equals("X"));
            var myreq1 = (from vector in myreq
                          group vector by vector.VectorType into newlist
                          select new
                          {
                              VectorType = newlist.Key,
                              Face = newlist.ToList()
                          }).ToList();
            List<TSS.Face> myface_list = new List<TSS.Face>();

            for (int h = 0; h < myreq1.Count; h++)
            {
                myface_list.Add((myreq1[h].Face.Find(x => x.Area.Equals(myreq1[h].Face.Max(y => y.Area)))).Face);
            }
            List<TSG.Point> list1 = new List<TSG.Point>();
            List<TSG.Point> list2 = new List<TSG.Point>();

            for (int x = 0; x < myface_list.Count; x++)
            {
                if (x == 0)
                {

                    TSS.LoopEnumerator myloop_enum = myface_list[x].GetLoopEnumerator();
                    while (myloop_enum.MoveNext())
                    {
                        TSS.Loop myloop = myloop_enum.Current as TSS.Loop;
                        TSS.VertexEnumerator myvertex_enum = myloop.GetVertexEnumerator();
                        while (myvertex_enum.MoveNext())
                        {
                            TSG.Point myvertex = myvertex_enum.Current as TSG.Point;
                            list1.Add(myvertex);

                        }
                    }
                }
                else
                {
                    TSS.LoopEnumerator myloop_enum = myface_list[x].GetLoopEnumerator();
                    while (myloop_enum.MoveNext())
                    {
                        TSS.Loop myloop = myloop_enum.Current as TSS.Loop;
                        TSS.VertexEnumerator myvertex_enum = myloop.GetVertexEnumerator();
                        while (myvertex_enum.MoveNext())
                        {
                            TSG.Point myvertex = myvertex_enum.Current as TSG.Point;
                            list2.Add(myvertex);

                        }
                    }

                }

            }

            List<TSG.Point> myedge = list1.Intersect(list2).ToList();
            TSG.Point point = new TSG.Point();
            if (myedge.Count > 0)
            {
                TSG.Matrix global_matrix = mymodel.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                TSG.Point globalpt = global_matrix.Transform(myedge[0]);
                TSG.Point viewpt = toviewmatrix.Transform(globalpt);
                TSG.Point globalpt1 = global_matrix.Transform(myedge[1]);
                TSG.Point viewpt1 = toviewmatrix.Transform(globalpt1);
                point = SKUtility.MidPoint(viewpt, viewpt1);


            }
            return point;

        }

        public List<TSG.Point> mypoint_for_bolt_skew(TSM.Part plate, TSD.View current_view, out TSG.Vector zaxis)
        {
            zaxis = new TSG.Vector();
            List<TSG.Point> BOLT_PTLIST = new List<TSG.Point>();

            TSM.Model mymodel = new TSM.Model();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            TSG.Matrix to_view_matrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            TSM.ModelObjectEnumerator bolt_enum = plate.GetBolts();
            TSG.Point bolt_point = new TSG.Point();
            while (bolt_enum.MoveNext())
            {


                TSM.BoltGroup boltgrp = bolt_enum.Current as TSM.BoltGroup;

                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                TSG.CoordinateSystem boltcheck = boltgrp.GetCoordinateSystem();
                TSG.Vector xaxis = boltcheck.AxisX;
                TSG.Vector yaxis = boltcheck.AxisY;
                zaxis = yaxis.Cross(xaxis);


                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                foreach (TSG.Point PT in boltgrp.BoltPositions)
                {
                    bolt_point = to_view_matrix.Transform(PT);
                    BOLT_PTLIST.Add(bolt_point);
                }



            }
            return BOLT_PTLIST;





        }
        public TSG.Point mypoint_for_bolt_skew1(TSM.Part plate, TSD.View current_view)
        {


            TSM.Model mymodel = new TSM.Model();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            //mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(plate.GetCoordinateSystem()));
            ////mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
            TSG.Matrix to_view_matrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            TSM.ModelObjectEnumerator bolt_enum = plate.GetBolts();
            TSG.Point bolt_point = new TSG.Point();
            while (bolt_enum.MoveNext())
            {
                TSM.BoltGroup mybolt = bolt_enum.Current as TSM.BoltGroup;
                bolt_point = to_view_matrix.Transform((mybolt.BoltPositions[0] as TSG.Point));

            }
            return bolt_point;
        }


        public List<TSG.Point> pts_in_each_face(TSM.Part mypart, TSD.View current_view)
        {

            List<TSG.Point> ALL_PTS_IN_EACH_FACE = new List<TSG.Point>();
            TSM.Model mymodel = new TSM.Model();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));

            TSM.Solid solid = mypart.GetSolid(TSM.Solid.SolidCreationTypeEnum.RAW);
            TSS.FaceEnumerator face_enum = solid.GetFaceEnumerator();
            while (face_enum.MoveNext())
            {
                TSS.Face face = face_enum.Current as TSS.Face;
                TSG.Vector myvector = face.Normal;


                TSS.LoopEnumerator loopenum = face.GetLoopEnumerator();
                while (loopenum.MoveNext())
                {
                    TSS.Loop loop = loopenum.Current as TSS.Loop;
                    TSS.VertexEnumerator myvertex = loop.GetVertexEnumerator();
                    while (myvertex.MoveNext())
                    {
                        TSG.Point mypoint = myvertex.Current as TSG.Point;
                        ALL_PTS_IN_EACH_FACE.Add(mypoint);
                    }
                }

            }
            //ALL_PTS_IN_EACH_FACE.Sort(new sort_by_y_value_asc());
            sortingHandler.SortPoints(ALL_PTS_IN_EACH_FACE, SKSortingHandler.SortBy.Y);
            return ALL_PTS_IN_EACH_FACE;
        }


        public bool check_for_skew_attachments(TSM.Part part, TSM.Part main_part)
        {
            bool result;
            TSM.Model mymodel = new TSM.Model();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(main_part.GetCoordinateSystem()));
            TSG.Vector x_vector = part.GetCoordinateSystem().AxisX;
            TSG.Vector y_vector = part.GetCoordinateSystem().AxisY;
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            x_vector.Normalize();
            y_vector.Normalize();

            if (((Math.Round(x_vector.X, 2) == 1) || (Math.Round(x_vector.Y, 2)) == 1 || (Math.Round(x_vector.Z)) == 1) || (((Math.Round(x_vector.X, 2) == -1) || (Math.Round(x_vector.Y, 2)) == -1 || (Math.Round(x_vector.Z, 2)) == -1)))
            {
                if (((Math.Round(y_vector.X, 2) == 1) || (Math.Round(y_vector.Y, 2)) == 1 || (Math.Round(y_vector.Z, 2)) == 1) || (((Math.Round(y_vector.X, 2) == -1) || (Math.Round(y_vector.Y, 2)) == -1 || (Math.Round(y_vector.Z, 2)) == -1)))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }
    }
}
