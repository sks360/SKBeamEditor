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
using System.Diagnostics;
using Tekla.Structures.Model;

namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class SKDimensionHandler
    {
        private readonly CustomInputModel _inputModel;

        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private readonly SKFacePointHandler facePointHandler;

        private readonly SKDrawingHandler drawingHandler;

        private readonly SKWeldHandler weldHandler;

        private readonly DuplicateRemover duplicateRemover;

        private readonly CopeDimensions copeDimensions;


        private readonly SkStudDimensions skStudDimensions;

        private readonly SKGussetDimension gussetDimension;

        private readonly ElevationDimension elevationDimension;

        private readonly AttachmentDimension attachmentDimension;

        private readonly OutsideAssemblyDimension outsideAssemblyDimension;

        private SKSlopHandler skSlopHandler;

        private readonly SKBoltHandler skBoltHandler;
        private SKFlangeOutDimension flangeOutDimension;

        private static readonly log4net.ILog _logger =
     log4net.LogManager.GetLogger(typeof(SKDimensionHandler));

        public SKDimensionHandler(  
            SKWeldHandler weldHandler, SKBoltHandler skBoltHandler,
            SKCatalogHandler catalogHandler,
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
            SKFacePointHandler facePointHandler,
             SKDrawingHandler drawingHandler, DuplicateRemover duplicateRemover, CustomInputModel userInput)
        {
            this.weldHandler = weldHandler;
            this.skBoltHandler = skBoltHandler;
            this.catalogHandler = catalogHandler;
            this.boltMatrixHandler = boltMatrixHandler;
            this.boundingBoxHandler = boundingBoxHandler;
            this.sortingHandler = sortingHandler;
            _inputModel = userInput;
            this.client = _inputModel.Client;
            this.fontSize = _inputModel.FontSize;
            this.facePointHandler = facePointHandler;
            this.drawingHandler = drawingHandler;
            this.duplicateRemover = duplicateRemover;
            skStudDimensions = new SkStudDimensions(catalogHandler, _inputModel);
            copeDimensions = new CopeDimensions(catalogHandler, _inputModel);
            gussetDimension = new SKGussetDimension(catalogHandler, boltMatrixHandler,
                boundingBoxHandler, sortingHandler, _inputModel);

            elevationDimension = new ElevationDimension(_inputModel);
            attachmentDimension = new AttachmentDimension(catalogHandler, boltMatrixHandler, boundingBoxHandler,
                sortingHandler, facePointHandler, drawingHandler, duplicateRemover, _inputModel);
            outsideAssemblyDimension = new OutsideAssemblyDimension(catalogHandler, boltMatrixHandler,
                boundingBoxHandler, sortingHandler, duplicateRemover, _inputModel);

            skSlopHandler = new SKSlopHandler(sortingHandler, _inputModel);

            flangeOutDimension = new SKFlangeOutDimension(catalogHandler, _inputModel);
        }


        public SKFlangeOutDimension GetFlangeOutDimension()
        {
            return flangeOutDimension;
        }
        public void ConfigureTopViewBoltDimensions(Model mymodel, AssemblyDrawing beam_assembly_drg,
            TSM.Part main_part, TSM.Assembly ASSEMBLY, double output, string drg_att, List<SectionLocationWithParts> list,
            double SCALE, List<double> MAINPART_PROFILE_VALUES, Beam main,
            ref List<Guid> TOP_VIEW_PARTMARK_TO_RETAIN, ref List<Guid> TOP_VIEW_BOLTMARK_TO_RETAIN, TSD.View current_view)
        {
            Stopwatch topViewMatch = new Stopwatch();
            topViewMatch.Start();
            current_view.Attributes.Scale = SCALE;
            current_view.Modify();
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

            double maxy = current_view.RestrictionBox.MaxPoint.Y;



            TSD.PointList FINAL_RD_DIST_CHECK = new TSD.PointList();
            try
            {
                weldHandler.WeldDelete(current_view, list, beam_assembly_drg);
            }
            catch
            {
            }

            /////////BOLT RD DIMENSION//////////////
            ///////////////////////////////////////////////////filtering bolts from all parts in top view/////////////////////////////////////////////////////////////////////////////

            TSD.PointList rd_point_list = new TSD.PointList();
            TSG.Matrix top_mat = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////Copied part from front view for bolt dim using new logic/////////////////////////////////////
            List<TSG.Point> singlebolts = new List<TSG.Point>();

            TSG.Matrix to_view_matrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
            TSD.StraightDimensionSetHandler bolt_combine_dim = new TSD.StraightDimensionSetHandler();

            ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////


            TSM.ModelObjectEnumerator model_bolt_enum = main_part.GetBolts();
            while (model_bolt_enum.MoveNext())
            {
                TSM.BoltGroup boltgrp = model_bolt_enum.Current as TSM.BoltGroup;
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                TSG.CoordinateSystem boltcheck = boltgrp.GetCoordinateSystem();
                TSG.Vector xaxis = boltcheck.AxisX;
                TSG.Vector yaxis = boltcheck.AxisY;
                TSG.Vector zaxis = yaxis.Cross(xaxis);
                zaxis.Normalize();
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                if ((Convert.ToInt64(zaxis.Z) != 0))
                {
                    foreach (TSG.Point pt in boltgrp.BoltPositions)
                    {
                        TSG.Point conv_pt = to_view_matrix.Transform(pt);
                        if (Convert.ToInt64(conv_pt.Z) > 0)
                        {
                            singlebolts.Add(conv_pt);
                        }
                    }
                    TOP_VIEW_BOLTMARK_TO_RETAIN.Add(boltgrp.Identifier.GUID);
                }
            }



            var groupedbolts = (from points in singlebolts
                                group points by Convert.ToInt64(points.X) into newlist
                                orderby newlist.Key ascending
                                select new
                                {
                                    x_dist = newlist.Key,
                                    point_in_group = (newlist.OrderBy(y => y.Y).ToList())

                                }).ToList();

            TSD.StraightDimensionSet.StraightDimensionSetAttributes OUTSIDE = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            OUTSIDE.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Outside;
            OUTSIDE.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            OUTSIDE.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, drg_att);

            for (int i = 0; i < groupedbolts.Count; i++)
            {

                rd_point_list.Add(groupedbolts[i].point_in_group[0]);


            }

            TSD.PointList rd_point_list_final = new TSD.PointList();

            foreach (TSG.Point pt in rd_point_list)
            {
                if (Convert.ToInt64(pt.Z) > 0)
                {
                    rd_point_list_final.Add(pt);

                }
            }


            TSD.StraightDimensionSetHandler bolt_rd_dim = new TSD.StraightDimensionSetHandler();
            TSD.PointList FINAL_RD_LIST = duplicateRemover.RemoveDuplicateXValues(rd_point_list_final);
            FINAL_RD_DIST_CHECK = FINAL_RD_LIST;
            FINAL_RD_LIST.Add(new TSG.Point(0, 0, 0));
            sortingHandler.SortPoints(FINAL_RD_LIST);
            /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////

            try
            {
                ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point);
                TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                double distance_value = TSG.Distance.PointToPoint(p1, p2);
                /////////////////////////////////////////////////////rd dimension creation////////////////////////////////////////////////////////////////////////////////////////////////////
                TSD.StraightDimensionSet.StraightDimensionSetAttributes rd = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                rd.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                rd.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                rd.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                rd.Color = DrawingColors.Gray70;
                rd.Text.Font.Color = DrawingColors.Gray70;
                rd.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                rd.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, drg_att);

                bolt_rd_dim.CreateDimensionSet(current_view, FINAL_RD_LIST, new TSG.Vector(0, 1, 0), maxy + 100, rd);


            }
            catch
            {
            }












            double distance_for_bolt_dim = 0;




            if (groupedbolts.Count > 1)
            {
                if (groupedbolts[0].x_dist < 150)
                {
                    double REST_BOX_MIN = Math.Abs(current_view.RestrictionBox.MinPoint.X);

                    distance_for_bolt_dim = groupedbolts[0].x_dist + REST_BOX_MIN + 75;
                }
                else
                {
                    distance_for_bolt_dim = 30;
                }
            }


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
                                    int threshold_value_for_boltdim_combine = 140;
                                    long x_value_current = Convert.ToInt64(groupedbolts[i].point_in_group[j].X);
                                    long x_value_next = Convert.ToInt64(groupedbolts[i + 1].point_in_group[j].X);
                                    long difference = Math.Abs(x_value_current - x_value_next);
                                    if (difference < threshold_value_for_boltdim_combine)
                                    {
                                        if (j == number_of_bolts_current - 1)
                                        {
                                            distance_for_bolt_dim = distance_for_bolt_dim + difference;
                                        }
                                        ptlist_for_boltdim_rd.Add(groupedbolts[i].point_in_group[j]);

                                        //boltmatrix = new TSG.Point[number_of_bolts_current,1];

                                        //grouped_bolts_matrix_form[h][number_of_bolts_current] = groupedbolts[i].point_in_group[j];

                                    }
                                    else
                                    {
                                        foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                        {
                                            ptlist_for_boltdim.Add(pt);

                                        }
                                        //if (j == number_of_bolts_current - 1)
                                        //{
                                        ptlist_for_boltdim.Add(new TSG.Point(groupedbolts[i].point_in_group[j].X, 0, 0));
                                        //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_rd, new TSG.Vector(0, 1, 0), 500);
                                        //}

                                        try
                                        {
                                            TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                            if (ptlist_for_boltdim.Count > 2)
                                            {
                                                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);

                                            }
                                            else
                                            {
                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);



                                            }


                                        }
                                        catch
                                        {
                                        }

                                        distance_for_bolt_dim = 30;
                                        break;
                                    }
                                }
                                else
                                {

                                    foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                    {
                                        ptlist_for_boltdim.Add(pt);
                                    }
                                    //if (j == number_of_bolts_current - 1)
                                    //{
                                    ptlist_for_boltdim.Add(new TSG.Point(groupedbolts[i].point_in_group[j].X, 0, 0));
                                    //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_rd, new TSG.Vector(0, 1, 0), 500);
                                    //}
                                    try
                                    {


                                        //   bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);
                                        TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                        if (ptlist_for_boltdim.Count > 2)
                                        {
                                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);

                                        }
                                        else
                                        {
                                            REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                            REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                            bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);



                                        }

                                    }
                                    catch
                                    {
                                    }

                                    distance_for_bolt_dim = 30;
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
                            ptlist_for_boltdim.Add(new TSG.Point(ptlist_for_boltdim[0].X, 0, 0));
                            //bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim_rd, new TSG.Vector(0, 1, 0), 500);
                            try
                            {
                                //    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);

                                if (ptlist_for_boltdim[0].X < 150)
                                {
                                    //distance_for_bolt_dim = 200;

                                }


                                TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                if (ptlist_for_boltdim.Count > 2)
                                {
                                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);

                                }
                                else
                                {
                                    REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                    REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                    bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, OUTSIDE);



                                }

                            }
                            catch
                            {
                            }
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

                                foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                                {
                                    ptlist_for_boltdim.Add(pt);

                                }

                                if (groupedbolts[i].x_dist > output - 150)
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
                                foreach (TSG.Point pt in groupedbolts[i - 1].point_in_group)
                                {
                                    ptlist_for_boltdim.Add(pt);

                                }
                                if (groupedbolts[i].x_dist > output - 150)
                                {
                                    double REST_BOX_MAX = Math.Abs(current_view.RestrictionBox.MaxPoint.X);
                                    distance_for_bolt_dim = Math.Abs(groupedbolts[i - 1].x_dist - REST_BOX_MAX);
                                }
                                else
                                {

                                    //distance_for_bolt_dim = Math.Abs(groupedbolts[i - 1].x_dist - groupedbolts[i].x_dist);
                                    distance_for_bolt_dim = 0;
                                }
                            }
                        }
                        else
                        {
                            foreach (TSG.Point pt in groupedbolts[i].point_in_group)
                            {
                                ptlist_for_boltdim.Add(pt);

                            }

                            if (groupedbolts[i].x_dist > output - 150)
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
                        if (groupedbolts[i].x_dist > output - 150)
                        {

                            new_vector = new TSG.Vector(1, 0, 0);
                        }
                        else
                        {

                            new_vector = new TSG.Vector(1, 0, 0);
                        }

                        ptlist_for_boltdim.Add(new TSG.Point(ptlist_for_boltdim[0].X, 0, 0));
                        try
                        {

                            TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                            if (ptlist_for_boltdim.Count > 2)
                            {
                                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, ptlist_for_boltdim, new_vector, distance_for_bolt_dim + 75, OUTSIDE);

                            }
                            else
                            {
                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                bolt_combine_dim.CreateDimensionSet(current_view as TSD.ViewBase, REVERSE_OF_PTLIST, new_vector, distance_for_bolt_dim + 75, OUTSIDE);



                            }


                        }
                        catch
                        {
                        }
                    }

                }

            }
            //////////////End of bolt rd dimension FOR TOPVIEW //////////////////////////
            #region boltwithinflange
            TSG.Matrix top_mat11 = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            ArrayList secondary_parts = ASSEMBLY.GetSecondaries();

            foreach (TSM.Part mypart in secondary_parts)
            {

                //TSD.DrawingObjectEnumerator enum_for_bolt11 = current_view.GetAllObjects(type_for_bolt);
                TSM.ModelObjectEnumerator enum_for_bolt11 = mypart.GetBolts();
                int size = enum_for_bolt11.GetSize();
                TSD.PointList rd_point_list11 = new TSD.PointList();
                if (size > 0)
                {
                    ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
                    while (enum_for_bolt11.MoveNext())
                    {


                        TSM.BoltGroup bolt = enum_for_bolt11.Current as TSM.BoltGroup;
                        if (!bolt.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP))
                        {


                            if ((top_mat11.Transform((bolt.BoltPositions[0]) as TSG.Point).Z > 0) && (top_mat11.Transform((bolt.BoltPositions[0]) as TSG.Point).Z < Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2))
                            {
                                //TSD.Bolt drgbolt1 = drgbolt11;
                                TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(bolt, current_view);
                                if (POINT_FOR_BOLT_MATRIX != null)
                                {
                                    int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                    int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                    for (int i = 0; i < x; i++)
                                    {
                                        ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                        rd_point_list11.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                                    }
                                }
                            }
                        }

                    }
                }

                /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                TSD.PointList FINAL_RD_LIST11 = duplicateRemover.RemoveDuplicateXValues(rd_point_list11);
                FINAL_RD_LIST11.Insert(0, (new TSG.Point(0, 0, 0)));
                sortingHandler.SortPoints(FINAL_RD_LIST11);
                /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                    double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                    TSG.Point p1 = (FINAL_RD_LIST11[FINAL_RD_LIST11.Count - 1] as TSG.Point);
                    TSG.Point p2 = new TSG.Point((FINAL_RD_LIST11[FINAL_RD_LIST11.Count - 1] as TSG.Point).X, distance, 0);
                    double distance_value = TSG.Distance.PointToPoint(p1, p2);
                    /////////////////////////////////////////////////////rd dimension creation////////////////////////////////////////////////////////////////////////////////////////////////////
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes rd = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    rd.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                    rd.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    rd.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    rd.Color = DrawingColors.Gray70;
                    rd.Text.Font.Color = DrawingColors.Gray70;
                    rd.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                    rd.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, drg_att);
                    bolt_rd_dim.CreateDimensionSet(current_view, FINAL_RD_LIST11, new TSG.Vector(0, 1, 0), maxy + 125, rd);

                }
                catch
                {
                }



            }


            #endregion

            ////////////////////cope dimension for topview///////////////
            flangeOutDimension.Create_FLANGE_CUT_dimensions_top(current_view, main, drg_att);
            copeDimensions.ProvideFittingCutDimensions(mymodel, current_view, main, drg_att);

            //////////////////// end of cope dimension for topview///////////////
            attachmentDimension.CreateDimensionInsideFlangeTop(main, current_view, output, ref TOP_VIEW_PARTMARK_TO_RETAIN, drg_att);
            gussetDimension.GussetDimensionsWithBolts(main, current_view, ref TOP_VIEW_PARTMARK_TO_RETAIN, ref TOP_VIEW_BOLTMARK_TO_RETAIN, drg_att);

            skBoltHandler.partmark_for_bolt_dim_attachments(current_view, ref TOP_VIEW_PARTMARK_TO_RETAIN);

            topViewMatch.Stop();
            Console.WriteLine("top_watch.....>" + topViewMatch.ElapsedMilliseconds.ToString());


        }

        public void ConfigureFrontViewBoldDimension(Model mymodel, AssemblyDrawing beam_assembly_drg, TSM.Part main_part,
         double output, string drg_att, List<SectionLocationWithParts> list, List<RadiusDimension> list_of_radius,
         double SCALE, List<double> MAINPART_PROFILE_VALUES,
         Beam main, ref List<Guid> FRONT_VIEW_PARTMARK_TO_RETAIN,
         ref List<Guid> FRONT_VIEW_BOLTMARK_TO_RETAIN, TSD.View current_view, ref double SHORTNENING_VALUE_FOR_BOTTOM_VIEW)
        {
            Stopwatch front_view_watch = new Stopwatch();
            front_view_watch.Start();
            current_view.Attributes.Scale = SCALE;
            current_view.Modify();
            double VIEW_sCALE = current_view.Attributes.Scale;
            double RAD_DIST = 180 / VIEW_sCALE;
            foreach (TSD.RadiusDimension myrad in list_of_radius)
            {

                myrad.Distance = RAD_DIST;
                myrad.Modify();
            }
            TSD.PointList FINAL_RD_DIST_CHECK = new TSD.PointList();

            SHORTNENING_VALUE_FOR_BOTTOM_VIEW = current_view.Attributes.Shortening.MinimumLength;


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

            double Y_POS = current_view.RestrictionBox.MaxPoint.Y;
            //double Y_NEG = current_view.RestrictionBox.MinPoint.Y;
            current_view.RestrictionBox.MaxPoint.Y = Y_POS + 100;
            current_view.Modify();


            try
            {
                weldHandler.WeldDelete(current_view, list, beam_assembly_drg);
            }
            catch
            {
            }


            //////////BOLT RD DIMENSION/////////////                                                                                                                                                    
            ///////////////////////////////////////////////////filtering bolts from all parts in front view/////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
            TSD.StraightDimensionSetHandler bolt_rd_dim = new TSD.StraightDimensionSetHandler();
            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            if ((drg_att == "SK_BEAM_A1") || (_inputModel.FontSize == FontSizeSelector.OneBy8))
            {
                if ((_inputModel.Client.Equals("HILLSDALE")) || (_inputModel.FontSize == FontSizeSelector.NineBy64))
                {
                    dim_font_height.Text.Font.Height = 3.571875;
                }
                else
                {
                    dim_font_height.Text.Font.Height = 3.175;
                }
            }
            else
            {
                dim_font_height.Text.Font.Height = 2.38125;


            }




            List<double> list_of_catalog = catalogHandler.Getcatalog_values(main_part);

            double heightBy2 = list_of_catalog[0] / 2;

            if (_inputModel.NeedCutLength)
            {

                TSD.PointList overall_dim_for_beam = new TSD.PointList();
                overall_dim_for_beam.Add(new TSG.Point(0, -heightBy2, 0));
                overall_dim_for_beam.Add(new TSG.Point(output, -heightBy2, 0));
                bolt_rd_dim.CreateDimensionSet(current_view, overall_dim_for_beam, new TSG.Vector(0, -1, 0), Math.Abs(current_view.RestrictionBox.MinPoint.Y) + 75, dim_font_height);
            }

            //bolt_logic

            /////////////////END OF BOLT RD DIMENSION for FRONT VIEW  ///////////////



            /////////////////3x3 dimension////////////////////////
            ///////////////////////////////////////////////////filtering bolts from all parts in front view/////////////////////////////////////////////////////////////////////////////


            #region bolt_vertical_dimension
            List<TSG.Point> singlebolts = new List<TSG.Point>();
            List<TSG.Point> singlebolts1 = new List<TSG.Point>();
            TSG.Matrix to_view_matrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
            TSD.StraightDimensionSetHandler bolt_combine_dim = new TSD.StraightDimensionSetHandler();
            TSG.Vector myvector_for_slope_bolt = new TSG.Vector();

            ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////
            Dictionary<int, Guid> MY_DICTIONARY = new Dictionary<int, Guid>();
            List<TSM.BoltGroup> SLOPE_BOLT_GROUP = new List<TSM.BoltGroup>();
            TSM.ModelObjectEnumerator model_bolt_enum = main_part.GetBolts();
            TSG.Vector zaxis_for_slope = new TSG.Vector();
            while (model_bolt_enum.MoveNext())
            {
                TSM.BoltGroup boltgrp = model_bolt_enum.Current as TSM.BoltGroup;
                MY_DICTIONARY.Add(boltgrp.Identifier.ID, boltgrp.Identifier.GUID);
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                TSG.CoordinateSystem boltcheck = boltgrp.GetCoordinateSystem();
                TSG.Vector xaxis = boltcheck.AxisX;
                TSG.Vector yaxis = boltcheck.AxisY;
                TSG.Vector zaxis = yaxis.Cross(xaxis);
                zaxis.Normalize();
                zaxis_for_slope = zaxis;
                zaxis_for_slope.Normalize();
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                double angle_check_FOR_NOT_IN_VIEW = Math.Abs(SKUtility.RadianToDegree((zaxis.GetAngleBetween(new TSG.Vector(1, 0, 0)))));
                double angle_check_FOR_SLOPE_AND_NORMAL = Math.Abs(SKUtility.RadianToDegree((zaxis.GetAngleBetween(new TSG.Vector(0, 1, 0)))));
                double angle_check_FOR_SLOPE_AND_NORMAL1 = Math.Abs(SKUtility.RadianToDegree((xaxis.GetAngleBetween(new TSG.Vector(1, 0, 0)))));

                if (Convert.ToInt64(angle_check_FOR_NOT_IN_VIEW) == 90)
                {
                    if ((Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL) == 90))
                    {
                        if ((Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL1) == 90) || (Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL1) == 0) || (Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL1) == 180) || (Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL1) == 270))
                        {
                            foreach (TSG.Point pt in boltgrp.BoltPositions)
                            {

                                singlebolts.Add(to_view_matrix.Transform(pt));

                            }
                            FRONT_VIEW_BOLTMARK_TO_RETAIN.Add(boltgrp.Identifier.GUID);
                        }
                        else
                        {
                            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));

                            myvector_for_slope_bolt = skSlopHandler.VectorForSlope(boltgrp, current_view);
                            if (myvector_for_slope_bolt != new TSG.Vector(0, 0, 0))
                            {
                                SLOPE_BOLT_GROUP.Add(boltgrp);
                            }
                            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                        }
                    }
                    else if ((Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL) == 0) || (Convert.ToInt64(angle_check_FOR_SLOPE_AND_NORMAL) == 180))
                    {

                    }

                }
            }
            TSG.Vector yvector_for_slope_bolt = new TSG.Vector();

            TSG.Matrix to_rotate_matrix = new TSG.Matrix();
            if (myvector_for_slope_bolt != new TSG.Vector(0, 0, 0))
            {
                yvector_for_slope_bolt = myvector_for_slope_bolt.Cross(zaxis_for_slope);
                TSG.CoordinateSystem rotated_coord = new TSG.CoordinateSystem(new TSG.Point(0, 0, 0), myvector_for_slope_bolt, yvector_for_slope_bolt);
                //mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(rotated_coord));
                model_bolt_enum.Reset();
                to_rotate_matrix = TSG.MatrixFactory.ToCoordinateSystem(rotated_coord);

                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                foreach (TSM.BoltGroup boltgrp in SLOPE_BOLT_GROUP)
                {

                    foreach (TSG.Point pt in boltgrp.BoltPositions)
                    {

                        singlebolts1.Add((to_view_matrix.Transform(pt)));

                    }
                    FRONT_VIEW_BOLTMARK_TO_RETAIN.Add(boltgrp.Identifier.GUID);

                }






            }


            try
            {
                if (singlebolts1.Count > 0)
                {
                    skSlopHandler.slope_bolt_logic(singlebolts1, to_rotate_matrix,
                        current_view, output, MAINPART_PROFILE_VALUES, myvector_for_slope_bolt,
                        yvector_for_slope_bolt, drg_att);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Problem in handling slope blots", e);
            }


            try
            {
                if (singlebolts.Count > 0)
                {
                    skBoltHandler.bolt_logic(singlebolts, current_view, output, MAINPART_PROFILE_VALUES, drg_att);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Problem in handling single blots", e);
            }
            #endregion
            ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////


            /////////////////END OF 3x3 dimension for FRONT VIEW////////////////////////





            ///////////////// cope dimension for FRONT VIEW////////////////////////

            copeDimensions.CreateCopeDimensions(current_view, main, drg_att);

            /////////////////END OF cope dimension for FRONT VIEW////////////////////////



            gussetDimension.GussetDimensionsWithBolts(main, current_view, ref FRONT_VIEW_PARTMARK_TO_RETAIN, ref FRONT_VIEW_BOLTMARK_TO_RETAIN, drg_att);
            outsideAssemblyDimension.DimensionForPartsOutsideAssembly(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
            attachmentDimension.CreateDimensionOutsideFlange(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
            attachmentDimension.CreateDimensionInsideFlangeFront(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
            double DEPTH = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
            if (_inputModel.NeedEleDimension)
            {
                elevationDimension.CreateElevationDimension(new TSG.Point(0, DEPTH, 0), current_view, Math.Abs(current_view.RestrictionBox.MinPoint.X) + 250, drg_att);
            }
            skStudDimensions.Create_stud_dimensions(current_view, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
            skBoltHandler.partmark_for_bolt_dim_attachments(current_view, ref FRONT_VIEW_PARTMARK_TO_RETAIN);
            FRONT_VIEW_PARTMARK_TO_RETAIN.Add(main_part.Identifier.GUID);
            front_view_watch.Stop();
            Console.WriteLine("front_view_watch.....>" + front_view_watch.ElapsedMilliseconds.ToString());
        }

    }
}
