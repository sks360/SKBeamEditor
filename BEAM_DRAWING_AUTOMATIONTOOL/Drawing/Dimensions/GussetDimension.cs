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
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class GussetDimension
    {
        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly BoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        public GussetDimension(SKCatalogHandler catalogHandler,
            BoltMatrixHandler boltMatrixHandler, BoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
            string client,
            FontSizeSelector fontSize)
        {
            this.catalogHandler = catalogHandler;
            this.boltMatrixHandler = boltMatrixHandler;
            this.boundingBoxHandler = boundingBoxHandler;
            this.sortingHandler = sortingHandler;
            this.client = client;
            this.fontSize = fontSize;
        }

        public void Gusset_Dimensions_with_bolts_reworked(TSM.Beam main_part, TSD.View current_view,
            ref List<Guid> PARTMARK_TO_RETAIN, ref List<Guid> BOLTMARK_TO_RETAIN, string drg_att)
        {

            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height =
                new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);


            TSD.AngleDimensionAttributes angle_dim_font_height = new TSD.AngleDimensionAttributes();
            angle_dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            angle_dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes =
                new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            fixed_attributes.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
            double top_front;
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))

            ///////////////////Values based on which gusset plates are filtered, Outside flange or Web based on view type///////////////////
            {
                top_front = (catalog_values[0]);
            }
            else
            {
                top_front = (catalog_values[2]);
            }

            TSM.Model model = new TSM.Model();
            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            TSD.StraightDimensionSetHandler dim_set_handler = new TSD.StraightDimensionSetHandler();
            TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(typeof(TSD.Part));

            TSD.PointList sorted_pt_y_list_top = new TSD.PointList();
            TSD.PointList sorted_pt_y_list_bottom = new TSD.PointList();

            TSD.PointList final_pt_list_for_rd_top = new TSD.PointList();
            TSD.PointList final_pt_list_for_rd_bottom = new TSD.PointList();

            TSD.PointList final_pt_list_for_rd_top_without_bolt = new TSD.PointList();
            TSD.PointList final_pt_list_for_rd_bottom_without_bolt = new TSD.PointList();
            final_pt_list_for_rd_top.Add(new TSG.Point(0, 0, 0));
            final_pt_list_for_rd_bottom.Add(new TSG.Point(0, 0, 0));
            final_pt_list_for_rd_top_without_bolt.Add(new TSG.Point(0, 0, 0));
            final_pt_list_for_rd_bottom_without_bolt.Add(new TSG.Point(0, 0, 0));

            TSD.StraightDimensionSet.StraightDimensionSetAttributes rdattr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            rdattr.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
            rdattr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
            rdattr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            rdattr.Arrowhead.Head = ArrowheadTypes.FilledArrow;
            rdattr.Text.Font.Color = DrawingColors.Gray70;
            rdattr.Color = DrawingColors.Gray70;
            rdattr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

            ///////////////////////////////Getting all parts/////////////////////////////////////////////////
            while (enum_for_parts_drg.MoveNext())
            {
                TSD.Part mypart = enum_for_parts_drg.Current as TSD.Part;
                TSD.PointList bolt_first_points_list_top_flange = new TSD.PointList();
                TSD.PointList bolt_first_points_list_bottom_flange = new TSD.PointList();
                ArrayList top_left_bolts_list = new ArrayList();
                ArrayList top_right_bolts_list = new ArrayList();
                ArrayList bottom_left_bolts_list = new ArrayList();
                ArrayList bottom_right_bolts_list = new ArrayList();
                ArrayList vertical_bolt_top_list = new ArrayList();
                ArrayList vertical_bolt_bottom_list = new ArrayList();
                TSM.Part plate = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.Part;


                string prof_type = "";

                plate.GetReportProperty("PROFILE_TYPE", ref prof_type);
                //////////////////////////////////Filtering all the plates////////////////////////////////
                if (prof_type == "B")
                {

                    ///////////////////////Converting to view coordinate system and getting plate z vector for filtering//////////////////
                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                    TSG.Vector x_vector_plate = plate.GetCoordinateSystem().AxisX;
                    TSG.Vector y_vector_plate = plate.GetCoordinateSystem().AxisY;
                    TSG.Vector z_vector_plate = TSG.Vector.Cross(x_vector_plate, y_vector_plate);
                    model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


                    TSD.PointList bounding_box_x = boundingBoxHandler.bounding_box_sort_x(plate, current_view);
                    TSD.PointList bounding_box_y = boundingBoxHandler.bounding_box_sort_y(plate, current_view);

                    TSD.PointList pointlist_plate_corner_points = Get_plate_corner_points(bounding_box_x, bounding_box_y);

                    TSM.ContourPlate contourplate = plate as TSM.ContourPlate;
                    ///////////////////////Filtering the plates whichis normal to the view coordinate system///////////////////
                    if ((Convert.ToInt32(z_vector_plate.Z) != 0))
                    {

                        ///////Filtering for plates which are on the positive side of flange///////////////
                        if ((Convert.ToInt64(bounding_box_y[0].Y) >= Convert.ToInt64(top_front / 2)))
                        {

                            TSM.ModelObjectEnumerator bolt_enum = plate.GetBolts();
                            int a = bolt_enum.GetSize();
                            TSG.Vector vector_for_dim = new TSG.Vector();


                            ////Bolt enum for each gusset/////////////////////////////////

                            if (a > 0)
                            {
                                while (bolt_enum.MoveNext())
                                {
                                    TSM.BoltGroup bolt = bolt_enum.Current as TSM.BoltGroup;
                                    if (bolt.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE))
                                    {

                                        //////////Getting point matrix for each bolts/////////////////////////
                                        TSG.Point[,] point_matrix = boltMatrixHandler.Get_Bolt_properties_matrix_for_gusset(bolt, current_view, "x_asc");

                                        ////////////////////////////Transforming to current view and getting the x vector for bolt//////////////
                                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                        TSG.Vector vector_for_sep = new TSG.Vector(bolt.GetCoordinateSystem().AxisX);
                                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                        /////////Filtering the bolts into four quadrants... Top right, top left, bottom right and bottom left//////////////////////
                                        if (((vector_for_sep.X < 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X > 0) && (vector_for_sep.Y > 0)))
                                        {
                                            top_right_bolts_list.Add(bolt);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);

                                        }
                                        else if (((vector_for_sep.X > 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X < 0) && (vector_for_sep.Y > 0)))
                                        {
                                            top_left_bolts_list.Add(bolt);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);

                                        }
                                        else
                                        {
                                            vertical_bolt_top_list.Add(bolt);

                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);
                                        }
                                    }
                                }
                            }

                            else
                            {
                                if (plate.GetType().Equals(typeof(TSM.ContourPlate)))
                                {


                                    TSD.PointList list_of_points = sortingHandler.SortPoints(Get_Gusset_plate_points(contourplate, current_view));
                                    final_pt_list_for_rd_top_without_bolt.Add(list_of_points[0]);

                                    if (list_of_points.Count == 6)
                                    {
                                        TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                        fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                        fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                        TSD.PointList ptlist_1_gusset_dim = new TSD.PointList();

                                        ptlist_1_gusset_dim.Add(list_of_points[0]);
                                        ptlist_1_gusset_dim.Add(list_of_points[1]);
                                        TSD.PointList ptlist_2_gusset_dim = new TSD.PointList();
                                        ptlist_2_gusset_dim.Add(list_of_points[4]);
                                        ptlist_2_gusset_dim.Add(list_of_points[5]);


                                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_1_gusset_dim, new TSG.Vector(-1, 0, 0), 100, fixe);
                                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_2_gusset_dim, new TSG.Vector(1, 0, 0), 100, fixe);

                                    }

                                }

                            }

                        }

                        ///////Filtering for plates which are on the bottom flange///////////////
                        else if ((Convert.ToInt64(bounding_box_y[1].Y)) <= -Convert.ToInt64(top_front / 2))
                        {
                            TSM.ModelObjectEnumerator bolt_enum1 = plate.GetBolts();
                            TSG.Vector vector_for_dim = new TSG.Vector();
                            int b = bolt_enum1.GetSize();
                            if (b > 0)
                            {


                                while (bolt_enum1.MoveNext())
                                {
                                    TSM.BoltGroup bolt = bolt_enum1.Current as TSM.BoltGroup;
                                    if (bolt.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE))
                                    {

                                        //////////Getting point matrix for each bolts/////////////////////////
                                        TSG.Point[,] point_matrix = boltMatrixHandler.Get_Bolt_properties_matrix_for_gusset(bolt, current_view, "x_asc");

                                        ////////////////////////////Transforming to current view and getting the x vector for bolt//////////////
                                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                                        TSG.Vector vector_for_sep = new TSG.Vector(bolt.GetCoordinateSystem().AxisX);
                                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                        /////////Filtering the bolts into four quadrants... Top right, top left, bottom right and bottom left//////////////////////
                                        if (((vector_for_sep.X < 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X > 0) && (vector_for_sep.Y > 0)))
                                        {
                                            bottom_left_bolts_list.Add(bolt);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);

                                        }
                                        else if (((vector_for_sep.X > 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X < 0) && (vector_for_sep.Y > 0)))
                                        {
                                            bottom_right_bolts_list.Add(bolt);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);
                                        }
                                        else
                                        {
                                            vertical_bolt_bottom_list.Add(bolt);
                                            Guid ID = plate.Identifier.GUID;
                                            PARTMARK_TO_RETAIN.Add(ID);
                                            Guid BOLT_ID = bolt.Identifier.GUID;
                                            BOLTMARK_TO_RETAIN.Add(BOLT_ID);
                                        }

                                    }



                                }
                            }

                            else
                            {
                                if (plate.GetType().Equals(typeof(TSM.ContourPlate)))
                                {
                                    TSD.PointList list_of_points = sortingHandler.SortPoints(Get_Gusset_plate_points(contourplate, current_view));
                                    final_pt_list_for_rd_bottom_without_bolt.Add(list_of_points[0]);

                                    if (list_of_points.Count == 6)
                                    {
                                        TSD.PointList ptlist_1_gusset_dim = new TSD.PointList();
                                        TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                        fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                        fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                        ptlist_1_gusset_dim.Add(list_of_points[0]);
                                        ptlist_1_gusset_dim.Add(list_of_points[1]);
                                        TSD.PointList ptlist_2_gusset_dim = new TSD.PointList();
                                        ptlist_2_gusset_dim.Add(list_of_points[4]);
                                        ptlist_2_gusset_dim.Add(list_of_points[5]);
                                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_1_gusset_dim, new TSG.Vector(-1, 0, 0), 100, fixe);
                                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_2_gusset_dim, new TSG.Vector(1, 0, 0), 100, fixe);

                                    }
                                }
                            }

                        }



                    }
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixedattr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixedattr.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixedattr.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixedattr.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                    TSD.StraightDimensionSet.StraightDimensionSetAttributes OUSIDE = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    OUSIDE.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    OUSIDE.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside;
                    OUSIDE.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    OUSIDE.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                    ////////Declaring pointlists for each quadrant dimenrions and for RD///////////////////
                    TSD.PointList point_for_dim_top_right = new TSD.PointList();
                    TSD.PointList point_for_dim_top_left = new TSD.PointList();
                    TSD.PointList point_for_dim_bottom_right = new TSD.PointList();
                    TSD.PointList point_for_dim_bottom_left = new TSD.PointList();
                    TSD.PointList point_for_vertical_dim_top_right = new TSD.PointList();
                    TSD.PointList point_for_vertical_dim_bottom_right = new TSD.PointList();
                    TSD.PointList point_for_vertical_dim_top_left = new TSD.PointList();
                    TSD.PointList point_for_vertical_dim_bottom_left = new TSD.PointList();


                    double view_scale = current_view.Attributes.Scale;


                    ///////////////////////top_right_bolts_list///////////////////////////////
                    //try
                    //{

                    TopRightBoltList(current_view, drg_att, angle_dim_font_height,
                        top_front, model, dim_set_handler, final_pt_list_for_rd_top,
                        final_pt_list_for_rd_bottom, top_right_bolts_list,
                        bounding_box_x, bounding_box_y, pointlist_plate_corner_points,
                        fixedattr, OUSIDE, point_for_dim_top_right,
                        point_for_vertical_dim_top_right, view_scale);




                    //////////////////////////////top_left_bolts_list//////////////////////////////////////////////////////////

                    TopLeftBoldList(current_view, drg_att, angle_dim_font_height, top_front, model, dim_set_handler, final_pt_list_for_rd_top, final_pt_list_for_rd_bottom, top_left_bolts_list, bounding_box_x, bounding_box_y, pointlist_plate_corner_points, fixedattr, OUSIDE, point_for_dim_top_left, point_for_dim_bottom_left, point_for_vertical_dim_top_left, view_scale);
                    /////////////////////////////bottom_left_bolts_list////////////////////////////////////
                    BottomLeftBoltlist(current_view, drg_att, top_front, model, dim_set_handler, final_pt_list_for_rd_top, final_pt_list_for_rd_bottom, bottom_left_bolts_list, bounding_box_y, pointlist_plate_corner_points, fixedattr, OUSIDE, point_for_dim_bottom_right, point_for_vertical_dim_bottom_right, view_scale);
                    //////////////negative_down_bolts_list/////////////////////////////////////////////
                    BottomRightBoltList(current_view, drg_att, top_front, model, dim_set_handler, final_pt_list_for_rd_top, final_pt_list_for_rd_bottom, bottom_right_bolts_list, bounding_box_x, bounding_box_y, pointlist_plate_corner_points, fixedattr, OUSIDE, point_for_dim_bottom_left, point_for_vertical_dim_bottom_left, view_scale);

                }




            }

            //TSD.PointList assyboundingbox = bounding_box_sort_x(main_part.GetAssembly(), current_view);

            // double distance_for_rd = assyboundingbox[1].Y + 500;
            double distance_for_rd = 500;
            ///////////Creating RD dimension for all the gussets - both top and bottom//////////////////////////////////////////////////////
            try
            {
                double distance = Math.Abs(Math.Abs(final_pt_list_for_rd_top[0].Y) - current_view.RestrictionBox.MaxPoint.Y);
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_pt_list_for_rd_top, new TSG.Vector(0, 1, 0), distance + 200, rdattr);

            }
            catch
            {
            }
            try
            {
                double distance = Math.Abs(Math.Abs(final_pt_list_for_rd_top[0].Y) - current_view.RestrictionBox.MinPoint.Y);
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_pt_list_for_rd_bottom, new TSG.Vector(0, -1, 0), distance + 100, rdattr);
            }
            catch
            {
            }
            try
            {
                //sortingHandler.SortPoints(final_pt_list_for_rd_bottom_without_bolt);
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_pt_list_for_rd_bottom_without_bolt, new TSG.Vector(0, -1, 0), distance_for_rd + 100, rdattr);
            }
            catch
            {
            }
            try

            {
                //sortingHandler.SortPoints(final_pt_list_for_rd_top_without_bolt);
                dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, final_pt_list_for_rd_top_without_bolt, new TSG.Vector(0, 1, 0), distance_for_rd + 100, rdattr);
            }
            catch
            {
            }
        }

        private void BottomRightBoltList(TSD.View current_view, string drg_att, double top_front, Model model, StraightDimensionSetHandler dim_set_handler, PointList final_pt_list_for_rd_top, PointList final_pt_list_for_rd_bottom, ArrayList bottom_right_bolts_list, PointList bounding_box_x, PointList bounding_box_y, PointList pointlist_plate_corner_points, StraightDimensionSet.StraightDimensionSetAttributes fixedattr, StraightDimensionSet.StraightDimensionSetAttributes OUSIDE, PointList point_for_dim_bottom_left, PointList point_for_vertical_dim_bottom_left, double view_scale)
        {
            if (bottom_right_bolts_list.Count > 0)
            {
                ArrayList distances = new ArrayList();
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                int lm = 0;
                foreach (TSM.BoltGroup boltarray in bottom_right_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");

                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];

                    //point_matrix1 = point_matrix;
                    ///Flipping bolt matrix left right vs right left in case of top right and bottom left only; d>1 condition-flipping only in case of more than one rows////////////////
                    if (d > 1)
                    {





                    }



                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X < point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }

                    if (c > 1)
                    {

                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);


                        }
                        double dist_for_dim = 100;
                        if (d > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }

                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;
                        if (c > 1)
                        {
                            distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                        }
                        else
                        {
                            distances.Add(20);

                        }
                        try
                        {
                            TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                            fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                            for (int l = 0; l < d; l++)
                            {
                                if (lm == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, fi);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[0, 0] - point_matrix1[c - 1, 0]), new TSG.Vector(0, 0, 1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, OUSIDE);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                        point_for_dim_bottom_left.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] + new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 - new TSG.Point(0, y_value, 0);
                        double distance = 200 / view_scale;

                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);

                        angledim1.Insert();
                    }

                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    for (int j = 0; j < point_matrix1.GetLength(1); j++)
                    {
                        point_for_dim_bottom_left.Add(point_matrix1[0, j]);

                    }

                    lm++;

                }
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);

                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.Y);
                if (point_for_dim_bottom_left.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_bottom_left[0] - point_for_dim_bottom_left[point_for_dim_bottom_left.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_bottom_left, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 300, fixedattr);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.Y);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_bottom_left[0].Y > 0)
                {

                    final_pt_list_for_rd_top.Add(point_for_dim_bottom_left[0]);
                }
                else
                {

                    final_pt_list_for_rd_bottom.Add(point_for_dim_bottom_left[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_bottom_left.Add(point_for_dim_bottom_left[0]);
                point_for_vertical_dim_bottom_left.Add(new TSG.Point(point_for_dim_bottom_left[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_dim_bottom_left[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[0]);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_bottom_left[0].Y - bounding_box_y[1].Y);



                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_bottom_left[0].X - bounding_box_x[1].X);
                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_bottom_left, new TSG.Vector(1, 0, 0), dist_for_vertical_dim + 300, fixedattr);
                }
                catch
                {
                }
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, -1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {
                }

            }
        }

        private void BottomLeftBoltlist(TSD.View current_view, string drg_att, double top_front, Model model, StraightDimensionSetHandler dim_set_handler, PointList final_pt_list_for_rd_top, PointList final_pt_list_for_rd_bottom, ArrayList bottom_left_bolts_list, PointList bounding_box_y, PointList pointlist_plate_corner_points, StraightDimensionSet.StraightDimensionSetAttributes fixedattr, StraightDimensionSet.StraightDimensionSetAttributes OUSIDE, PointList point_for_dim_bottom_right, PointList point_for_vertical_dim_bottom_right, double view_scale)
        {
            if (bottom_left_bolts_list.Count > 0)
            {
                ArrayList distances = new ArrayList();
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                int kl = 0;
                foreach (TSM.BoltGroup boltarray in bottom_left_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.Get_Bolt_properties_matrix_for_gusset(boltarray, current_view, "x_asc");
                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];
                    ///Adding first row points of all bolt enum in the pointlist//////////////////////




                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X < point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {
                        for (int j = 0; j < point_matrix1.GetLength(1); j++)
                        {
                            point_for_dim_bottom_right.Add(point_matrix1[0, j]);

                        }

                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (point_matrix.GetLength(1) > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        if (c > 1)
                        {
                            distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                        }
                        else
                        {
                            distances.Add(20);

                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;
                        try
                        {
                            TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                            fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                            for (int l = 0; l < d; l++)
                            {
                                if (kl == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, fi);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[1, 0] - point_matrix1[0, 0]), new TSG.Vector(0, 0, -1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, OUSIDE);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_bottom_right.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(-1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] - new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 - new TSG.Point(0, y_value, 0);

                        double distance = 200 / view_scale;


                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);

                        angledim1.Insert();
                    }


                    kl++;

                }

                sortingHandler.SortPoints(point_for_dim_bottom_right, SKSortingHandler.SortBy.Y);
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_bottom_right.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_bottom_right[0] - point_for_dim_bottom_right[point_for_dim_bottom_right.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_bottom_right, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 300, fixedattr);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_bottom_right, SKSortingHandler.SortBy.Y);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_bottom_right[0].Y > 0)
                {

                    final_pt_list_for_rd_top.Add(point_for_dim_bottom_right[0]);
                }
                else
                {

                    final_pt_list_for_rd_bottom.Add(point_for_dim_bottom_right[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////

                point_for_vertical_dim_bottom_right.Add(point_for_dim_bottom_right[0]);
                point_for_vertical_dim_bottom_right.Add(new TSG.Point(point_for_dim_bottom_right[0].X, top_front / 2, 0));
                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_bottom_right[0].X - bounding_box_y[0].X);
                pointlist_for_lock.Add(point_for_vertical_dim_bottom_right[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[1]);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_bottom_right[0].Y - bounding_box_y[0].Y);

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_bottom_right, new TSG.Vector(-1, 0, 0), dist_for_vertical_dim + 200, fixe);
                }
                catch
                {

                }

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, -1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {

                }
            }
        }

        private void TopLeftBoldList(TSD.View current_view, string drg_att, AngleDimensionAttributes angle_dim_font_height, double top_front, Model model, StraightDimensionSetHandler dim_set_handler, PointList final_pt_list_for_rd_top, PointList final_pt_list_for_rd_bottom, ArrayList top_left_bolts_list, PointList bounding_box_x, PointList bounding_box_y, PointList pointlist_plate_corner_points, StraightDimensionSet.StraightDimensionSetAttributes fixedattr, StraightDimensionSet.StraightDimensionSetAttributes OUSIDE, PointList point_for_dim_top_left, PointList point_for_dim_bottom_left, PointList point_for_vertical_dim_top_left, double view_scale)
        {
            if (top_left_bolts_list.Count > 0)
            {
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                ArrayList distances = new ArrayList();
                int jk = 0;
                foreach (TSM.BoltGroup boltarray in top_left_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");
                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];



                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X > point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {

                        for (int j = 0; j < point_matrix1.GetLength(1); j++)
                        {
                            point_for_dim_top_left.Add(point_matrix1[0, j]);

                        }
                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (point_matrix.GetLength(1) > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;

                        try
                        {
                            for (int l = 0; l < d; l++)
                            {
                                if (jk == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, angle_dim_font_height);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[1, 0] - point_matrix1[0, 0]), new TSG.Vector(0, 0, 1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, OUSIDE);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_top_left.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(-1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] - new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 + new TSG.Point(0, y_value, 0);

                        double distance = 200 / view_scale;


                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);




                        //TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0],new TSG.Vector(1, 0, 0), vector_x,  200, fi);
                        angledim1.Insert();
                    }




                    //point_for_dim_top_left.Add(point_matrix[0, 0]);
                    if (c > 1)
                    {
                        distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                    }
                    else
                    {
                        distances.Add(20);

                    }
                    jk++;

                }
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_top_left.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_top_left[0] - point_for_dim_top_left[point_for_dim_top_left.Count - 1]).Cross(new TSG.Vector(0, 0, 1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_top_left, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 100, fixedattr);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_top_left, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_top_left[0].Y > 0)
                {

                    final_pt_list_for_rd_top.Add(point_for_dim_top_left[0]);
                }
                else
                {

                    final_pt_list_for_rd_bottom.Add(point_for_dim_top_left[0]);
                }


                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_top_left.Add(point_for_dim_top_left[0]);
                point_for_vertical_dim_top_left.Add(new TSG.Point(point_for_dim_top_left[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_dim_top_left[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[2]);

                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_top_left[0].X - bounding_box_x[0].X);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_top_left[0].Y - bounding_box_y[0].Y);
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fix = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fix.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fix.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fix.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_top_left, new TSG.Vector(-1, 0, 0), dist_for_vertical_dim + 300, fix);
                }
                catch
                {

                }

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, 1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {
                }

            }
        }

        private void TopRightBoltList(TSD.View current_view, string drg_att, AngleDimensionAttributes angle_dim_font_height, double top_front, Model model, StraightDimensionSetHandler dim_set_handler, PointList final_pt_list_for_rd_top, PointList final_pt_list_for_rd_bottom, ArrayList top_right_bolts_list, PointList bounding_box_x, PointList bounding_box_y, PointList pointlist_plate_corner_points, StraightDimensionSet.StraightDimensionSetAttributes fixedattr, StraightDimensionSet.StraightDimensionSetAttributes OUSIDE, PointList point_for_dim_top_right, PointList point_for_vertical_dim_top_right, double view_scale)
        {
            if (top_right_bolts_list.Count > 0)
            {
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                ArrayList distances = new ArrayList();
                int ij = 0;
                foreach (TSM.BoltGroup boltarray in top_right_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.Get_Bolt_properties_matrix_for_gusset(boltarray, current_view, "x_asc");

                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);

                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];




                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X > point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {
                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (d > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;

                        try
                        {

                            for (int l = 0; l < d; l++)
                            {
                                if (ij == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, angle_dim_font_height);
                                    angledim1.Insert();
                                }
                            }

                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[0, 0] - point_matrix1[1, 0]), new TSG.Vector(0, 0, -1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, -dist_for_dim, OUSIDE);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_top_right.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] + new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 + new TSG.Point(0, y_value, 0);
                        double distance = 200 / view_scale;

                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);




                        //TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], vector_y, new TSG.Vector(1, 0, 0), 200);
                        angledim1.Insert();
                    }




                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    for (int j = 0; j < point_matrix1.GetLength(1); j++)
                    {
                        point_for_dim_top_right.Add(point_matrix1[0, j]);

                    }

                    if (c > 1)
                    {
                        distances.Add(Convert.ToInt16(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0])));
                    }
                    else
                    {
                        distances.Add(20);

                    }
                    ij++;
                }
                sortingHandler.SortPoints(point_for_dim_top_right, SKSortingHandler.SortBy.Y,
                    SKSortingHandler.SortOrder.Descending);
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_top_right.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_top_right[0] - point_for_dim_top_right[point_for_dim_top_right.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_top_right, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 250, fixedattr);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////



                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_top_right[0].Y > 0)
                {

                    final_pt_list_for_rd_top.Add(point_for_dim_top_right[0]);
                }
                else
                {

                    final_pt_list_for_rd_bottom.Add(point_for_dim_top_right[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_top_right.Add(point_for_dim_top_right[0]);
                point_for_vertical_dim_top_right.Add(new TSG.Point(point_for_dim_top_right[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_vertical_dim_top_right[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[3]);
                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_top_right[0].X - bounding_box_x[1].X);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_top_right[0].Y - bounding_box_y[1].Y);
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_top_right, new TSG.Vector(1, 0, 0), dist_for_vertical_dim + 300, fixe);
                }
                catch
                {
                }
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, 1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {
                }

            }
        }

        public TSD.PointList Get_plate_corner_points(TSD.PointList x_sorted_bounding_box,
            TSD.PointList y_sorted_bounding_box)
        {
            TSD.PointList corner_pts = new TSD.PointList();

            TSG.Point pt1 = new TSG.Point(x_sorted_bounding_box[1].X, y_sorted_bounding_box[1].Y);
            TSG.Point pt2 = new TSG.Point(x_sorted_bounding_box[0].X, y_sorted_bounding_box[1].Y);
            TSG.Point pt3 = new TSG.Point(x_sorted_bounding_box[0].X, y_sorted_bounding_box[0].Y);
            TSG.Point pt4 = new TSG.Point(x_sorted_bounding_box[1].X, y_sorted_bounding_box[0].Y);

            corner_pts.Add(pt1);
            corner_pts.Add(pt2);
            corner_pts.Add(pt3);
            corner_pts.Add(pt4);
            return corner_pts;
        }
        public TSD.PointList Get_Gusset_plate_points(TSM.ContourPlate gusset, TSD.View current_view)
        {
            TSD.PointList pt_list_gusset_pts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            foreach (TSG.Point pt in gusset.Contour.ContourPoints)
            {
                pt_list_gusset_pts.Add(toviewmatrix.Transform(pt));
            }
            return pt_list_gusset_pts;
        }


    }
}
