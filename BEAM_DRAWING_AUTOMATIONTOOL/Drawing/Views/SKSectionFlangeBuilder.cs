using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using System.Windows.Forms;
using SK.Tekla.Drawing.Automation.Support;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SK.Tekla.Drawing.Automation.Drawing;
using SK.Tekla.Drawing.Automation.Models;
using System.Reflection;
using SK.Tekla.Drawing.Automation.Drawing.Dimensions;
using SK.Tekla.Drawing.Automation.Drawing.Views;
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures;
using System.Diagnostics;
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using System.Web.UI.WebControls.WebParts;
namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    public class SKSectionFlangeBuilder
    {
        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private readonly SKFacePointHandler facePointHandler;

        private readonly SKDrawingHandler drawingHandler;

        private readonly DuplicateRemover duplicateRemover;

        private readonly CustomInputModel _inputModel;

        private readonly StreamlineDrawing streamlineDrawing;

        private readonly SKAngleHandler skAngleHandler;

        private readonly SKBoltHandler skBoltHandler;

        public SKSectionFlangeBuilder(SKCatalogHandler catalogHandler,
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
            SKFacePointHandler facePointHandler,
             SKDrawingHandler drawingHandler, DuplicateRemover duplicateRemover, CustomInputModel inputModel,
            StreamlineDrawing streamlineDrawing, SKAngleHandler skAngleHandler, SKBoltHandler skBoltHandler)
        {

            this.catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            this.boltMatrixHandler = boltMatrixHandler ?? throw new ArgumentNullException(nameof(boltMatrixHandler));
            this.boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            this.sortingHandler = sortingHandler ?? throw new ArgumentNullException(nameof(sortingHandler));
            this.facePointHandler = facePointHandler ?? throw new ArgumentNullException(nameof(facePointHandler));
            this.drawingHandler = drawingHandler ?? throw new ArgumentNullException(nameof(drawingHandler));
            this.duplicateRemover = duplicateRemover ?? throw new ArgumentNullException(nameof(duplicateRemover));
            _inputModel = inputModel ?? throw new ArgumentNullException(nameof(inputModel));
            this.streamlineDrawing = streamlineDrawing ?? throw new ArgumentNullException(nameof(streamlineDrawing));
            this.skAngleHandler = skAngleHandler ?? throw new ArgumentNullException(nameof(skAngleHandler));
            this.skBoltHandler = skBoltHandler ?? throw new ArgumentNullException(nameof(skBoltHandler));
        }

        public void SectionFlange(Model mymodel, DrawingHandler my_handler, string defaultADFile,
       TSM.Part main_part, List<SectionLocationWithParts> list_section_flange,
       double SCALE, List<double> MAINPART_PROFILE_VALUES,
       StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes,
       List<double> catalog_values,
       List<TSD.View> sectionviews_in_drawing)
        {
            TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height1 = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
            dim_font_height1.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
            dim_font_height1.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, defaultADFile);

            /////////////////////////////////////////////////bolt gage dimension in section view using matrix function /////////////////////////////////////////////////////////////
            for (int z = 0; z < list_section_flange.Count; z++)
            {
                ///////////////////////////////////////////////////filtering bolts from all parts in section view/////////////////////////////////////////////////////////////////////////////

                TSD.View current_view = list_section_flange[z].MyView;

                if (current_view != null)
                {
                    Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                    TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(type_for_dim);
                    while (dim_drg.MoveNext())
                    {
                        var obj = dim_drg.Current;
                        obj.Delete();

                    }

                    current_view.Attributes.Scale = SCALE + _inputModel.SecScale;
                    current_view.Modify();
                    List<Guid> SECTION_VIEW_BOLTMARK_TO_RETAIN_POS = new List<Guid>();
                    List<Guid> SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG = new List<Guid>();

                    List<Guid> SECTION_VIEW_PARTMARK_TO_RETAIN = new List<Guid>();
                    current_view.Modify();
                    sectionviews_in_drawing.Add(current_view);
                    streamlineDrawing.SECTION_VIEW_PART_MARK_DELETE(current_view, my_handler);
                    TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                    List<TSD.Bolt> bolt_list = new List<TSD.Bolt>();

                    double minx = current_view.RestrictionBox.MinPoint.X;
                    double miny = current_view.RestrictionBox.MinPoint.Y;
                    double maxx = current_view.RestrictionBox.MaxPoint.X;
                    double maxy = current_view.RestrictionBox.MaxPoint.Y;

                    foreach (var list_of_parts in list_section_flange[z].PartList)
                    {
                        TSD.PointList rd_point_list = new TSD.PointList();
                        TSM.Part mypart = list_of_parts as TSM.Part;
                        SECTION_VIEW_PARTMARK_TO_RETAIN.Add(mypart.Identifier.GUID);

                        TSD.PointList angle_hole_locking_check = boundingBoxHandler.BoundingBoxSort(list_of_parts as TSM.ModelObject, current_view, SKSortingHandler.SortBy.Y);


                        TSM.ModelObjectEnumerator enum_for_bolt = list_of_parts.GetBolts();
                        //ts.DrawingObjectEnumerator enum_for_bolt = current_view.GetAllObjects(type_for_bolt);
                        string prof_typ = SkTeklaDrawingUtility.get_report_properties(mypart, "PROFILE_TYPE");

                        if (list_of_parts.Identifier.ID.Equals(670665))
                        {
                        }

                        if (prof_typ == "L")
                        {

                            skAngleHandler.angle_place_check_for_hole_locking(angle_hole_locking_check, out rd_point_list, enum_for_bolt, current_view, ref SECTION_VIEW_PARTMARK_TO_RETAIN, ref SECTION_VIEW_BOLTMARK_TO_RETAIN_POS, ref SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG);
                            /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                            TSD.PointList FINAL_RD_LIST = new TSD.PointList();
                            FINAL_RD_LIST = duplicateRemover.RemoveDuplicateXValues(rd_point_list);

                            /////////////////////////////////////////////////// inserting bolt gage dimension ////////////////////////////////////////////////////////////////////////////////////////
                            double ht = Convert.ToInt64(MAINPART_PROFILE_VALUES[0]);
                            double height = Convert.ToInt64(ht / 2);

                            TSD.PointList FINAL_RD_LIST_within_ht = new TSD.PointList();

                            TSD.PointList FINAL_RD_LIST_above_fl = new TSD.PointList();
                            TSD.PointList FINAL_RD_LIST_below_fl = new TSD.PointList();


                            string profile_type_for_section = "";
                            main_part.GetReportProperty("PROFILE_TYPE", ref profile_type_for_section);

                            if (profile_type_for_section == "U")
                            {
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                                TSG.Vector zvector = main_part.GetCoordinateSystem().AxisX.Cross(main_part.GetCoordinateSystem().AxisY);
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                zvector.Normalize();
                                double WT = 0;


                                double WT2 = (catalog_values[1]);
                                if (zvector.X > 0)
                                {
                                    WT = (-WT2 / 2);
                                }
                                else
                                {
                                    WT = (WT2 / 2);
                                }
                                FINAL_RD_LIST_within_ht.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_above_fl.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_below_fl.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));

                            }
                            else
                            {

                                FINAL_RD_LIST_within_ht.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_above_fl.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_below_fl.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                            }
                            foreach (TSG.Point PT in FINAL_RD_LIST)
                            {
                                if ((Convert.ToInt64(PT.Y) < height) && (Convert.ToInt64(PT.Y) > -height))
                                {
                                    FINAL_RD_LIST_within_ht.Add(PT);

                                }
                                else if (Convert.ToInt64(PT.Y) > height)
                                {
                                    FINAL_RD_LIST_above_fl.Add(PT);

                                }
                                else if (Convert.ToInt64(PT.Y) < -height)
                                {
                                    FINAL_RD_LIST_below_fl.Add(PT);

                                }
                            }




                            sortingHandler.SortPoints(FINAL_RD_LIST_within_ht, SKSortingHandler.SortBy.Y,
SKSortingHandler.SortOrder.Descending);
                            sortingHandler.SortPoints(FINAL_RD_LIST_above_fl, SKSortingHandler.SortBy.Y);
                            sortingHandler.SortPoints(FINAL_RD_LIST_below_fl, SKSortingHandler.SortBy.Y,
SKSortingHandler.SortOrder.Descending);
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes inside = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            inside.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, defaultADFile);


                            //inside.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside;

                            try
                            {
                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                                //double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                                //TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 2] as TSG.Point);
                                //TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                                //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_within_ht[0].Y) - maxy);
                                //////////////////////////////////////////////////// rd dimension creation////////////////////////////////////////////////////////////////////////////////////////////////////////
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_within_ht, new TSG.Vector(0, 1, 0), distance1 + 75, inside);
                            }
                            catch
                            {
                            }


                            try
                            {
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl[0].Y) - maxy);
                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl, new TSG.Vector(0, 1, 0), distance1 + 150, inside);
                            }
                            catch
                            {
                            }
                            try
                            {

                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                ///////ROSEPLASTIC
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - miny);
                                //bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, -1, 0), distance1 + 75, inside);
                                //double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - maxy);
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, -1, 0), distance1 + 75, inside);
                            }
                            catch
                            {
                            }
                        }
                        ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////


                        else
                        {


                            ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////

                            while (enum_for_bolt.MoveNext())
                            {
                                //TSD.Bolt drgbolt = enum_for_bolt.Current as TSD.Bolt;
                                TSM.BoltGroup drgbolt = enum_for_bolt.Current as TSM.BoltGroup;
                                TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);


                                ///////////////////////////////////////////////////condition for depth adjustment////////////////////////////////////////////////////////////////////////////////////////////
                                //////////////////////////////////////////////////// filtering bolts which lies within the section depth/////////////////////////////////////////////////////////////////////                       
                                double upper_limit = current_view.RestrictionBox.MaxPoint.Z;
                                double lower_limit = current_view.RestrictionBox.MinPoint.Z;
                                if (POINT_FOR_BOLT_MATRIX != null)
                                {
                                    if ((POINT_FOR_BOLT_MATRIX[0, 0].Z > lower_limit) && (POINT_FOR_BOLT_MATRIX[0, 0].Z < upper_limit))
                                    {
                                        int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                        int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                        for (int i = 0; i < x; i++)
                                        {
                                            //////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                            rd_point_list.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                                        }
                                        if (POINT_FOR_BOLT_MATRIX[0, 0].X > 0)
                                        {
                                            SECTION_VIEW_BOLTMARK_TO_RETAIN_POS.Add(drgbolt.Identifier.GUID);
                                        }
                                        if (POINT_FOR_BOLT_MATRIX[0, 0].X < 0)
                                        {
                                            SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG.Add(drgbolt.Identifier.GUID);
                                        }
                                    }
                                }
                            }
                            /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                            TSD.PointList FINAL_RD_LIST = new TSD.PointList();
                            FINAL_RD_LIST = duplicateRemover.RemoveDuplicateXValues(rd_point_list);

                            /////////////////////////////////////////////////// inserting bolt gage dimension ////////////////////////////////////////////////////////////////////////////////////////
                            double ht = Convert.ToInt64(MAINPART_PROFILE_VALUES[0]);
                            double height = Convert.ToInt64(ht / 2);

                            TSD.PointList FINAL_RD_LIST_within_ht = new TSD.PointList();

                            TSD.PointList FINAL_RD_LIST_above_fl = new TSD.PointList();
                            TSD.PointList FINAL_RD_LIST_below_fl = new TSD.PointList();


                            string profile_type_for_section = "";
                            main_part.GetReportProperty("PROFILE_TYPE", ref profile_type_for_section);

                            if (profile_type_for_section == "U")
                            {
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
                                TSG.Vector zvector = main_part.GetCoordinateSystem().AxisX.Cross(main_part.GetCoordinateSystem().AxisY);
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                zvector.Normalize();
                                double WT = 0;


                                double WT2 = (catalog_values[1]);
                                if (zvector.X > 0)
                                {
                                    WT = (-WT2 / 2);
                                }
                                else
                                {
                                    WT = (WT2 / 2);
                                }
                                FINAL_RD_LIST_within_ht.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_above_fl.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_below_fl.Add(new TSG.Point(WT, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));

                            }
                            else
                            {

                                FINAL_RD_LIST_within_ht.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_above_fl.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                                FINAL_RD_LIST_below_fl.Add(new TSG.Point(0, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));
                            }
                            foreach (TSG.Point PT in FINAL_RD_LIST)
                            {
                                if ((Convert.ToInt64(PT.Y) < height) && (Convert.ToInt64(PT.Y) > -height))
                                {
                                    FINAL_RD_LIST_within_ht.Add(PT);

                                }
                                else if (Convert.ToInt64(PT.Y) > height)
                                {
                                    FINAL_RD_LIST_above_fl.Add(PT);

                                }
                                else if (Convert.ToInt64(PT.Y) < -height)
                                {
                                    FINAL_RD_LIST_below_fl.Add(PT);

                                }
                            }




                            sortingHandler.SortPoints(FINAL_RD_LIST_within_ht, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
                            sortingHandler.SortPoints(FINAL_RD_LIST_above_fl, SKSortingHandler.SortBy.Y);
                            sortingHandler.SortPoints(FINAL_RD_LIST_below_fl, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes inside = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            inside.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, defaultADFile);

                            //inside.ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside;

                            try
                            {
                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                                //double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                                //TSG.Point p1 = (FINAL_RD_LIST[FINAL_RD_LIST.Count - 2] as TSG.Point);
                                //TSG.Point p2 = new TSG.Point((FINAL_RD_LIST[FINAL_RD_LIST.Count - 1] as TSG.Point).X, distance, 0);
                                //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_within_ht[0].Y) - maxy);
                                //////////////////////////////////////////////////// rd dimension creation////////////////////////////////////////////////////////////////////////////////////////////////////////
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_within_ht, new TSG.Vector(0, 1, 0), distance1 + 75, inside);
                            }
                            catch
                            {
                            }


                            try
                            {
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_above_fl[0].Y) - maxy);
                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_above_fl, new TSG.Vector(0, 1, 0), distance1 + 150, inside);
                            }
                            catch
                            {
                            }
                            try
                            {

                                TSD.StraightDimensionSetHandler bolt_gage_dim = new TSD.StraightDimensionSetHandler();
                                ///////ROSEPLASTIC
                                //double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - miny);
                                //bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, -1, 0), distance1 + 75, inside);
                                double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - maxy);
                                bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, 1, 0), distance1 + 75, inside);
                            }
                            catch
                            {
                            }


                        }



                    }
                    ///////////////////END OF RD DIMENSION//////////////////////
                    /////////////////3x3 dimension////////////////////////
                    ///////////////////////////////////////////////////filtering bolts from all parts in front view/////////////////////////////////////////////////////////////////////////////

                    TSD.StraightDimensionSetHandler dim_3x3 = new TSD.StraightDimensionSetHandler();
                    double x1 = 75;
                    double x2 = 75;
                    foreach (var list_of_parts in list_section_flange[z].PartList)
                    {
                        TSM.Part mypart = list_of_parts as TSM.Part;
                        double ht = Convert.ToInt64(MAINPART_PROFILE_VALUES[0]);
                        double height = Convert.ToInt64(ht / 2);


                        TSD.PointList list3x3 = new TSD.PointList();
                        TSM.ModelObjectEnumerator enum_for_bolt1 = list_of_parts.GetBolts();
                        //TSD.DrawingObjectEnumerator enum_for_bolt1 = current_view.GetAllObjects(type_for_bolt);

                        ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////
                        while (enum_for_bolt1.MoveNext())
                        {
                            TSM.BoltGroup drgbolt = enum_for_bolt1.Current as TSM.BoltGroup;
                            //TSD.Bolt drgbolt = enum_for_bolt1.Current as TSD.Bolt;
                            TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);
                            double upper_limit = current_view.RestrictionBox.MaxPoint.Z;
                            double lower_limit = current_view.RestrictionBox.MinPoint.Z;
                            //TSM.ModelObject mymodel_ = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                            TSM.BoltGroup mybolt = drgbolt;
                            if (POINT_FOR_BOLT_MATRIX != null)
                            {
                                if ((POINT_FOR_BOLT_MATRIX[0, 0].Z > lower_limit) && (POINT_FOR_BOLT_MATRIX[0, 0].Z < upper_limit))
                                {
                                    int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                    int x = POINT_FOR_BOLT_MATRIX.GetLength(1);

                                    if ((POINT_FOR_BOLT_MATRIX[0, 0].X < 0) && (POINT_FOR_BOLT_MATRIX[0, x - 1].X > 0) || (POINT_FOR_BOLT_MATRIX[0, 0].X > 0) && (POINT_FOR_BOLT_MATRIX[0, x - 1].X < 0))
                                    {


                                        foreach (TSG.Point pt in mybolt.BoltPositions)
                                        {
                                            TSG.Point p1 = toviewmatrix.Transform(pt);

                                            list3x3.Add(p1);
                                        }



                                    }

                                    else if ((POINT_FOR_BOLT_MATRIX[0, 0].X > 0))
                                    {


                                        for (int i = 0; i < y; i++)
                                        {
                                            /////////////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                            list3x3.Add(POINT_FOR_BOLT_MATRIX[i, x - 1]);
                                        }

                                    }
                                    else if ((POINT_FOR_BOLT_MATRIX[0, 0].X < 0))
                                    {


                                        for (int i = 0; i < y; i++)
                                        {
                                            /////////////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                            list3x3.Add(POINT_FOR_BOLT_MATRIX[i, 0]);
                                        }

                                    }
                                }
                            }
                        }

                        /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                        //  TSD.PointList FINAL_list3x3 = duplicateRemover.pointlist_remove_duplicate_Yvalues(list3x3);
                        TSD.PointList FINAL_list3x3 = list3x3;
                        TSD.PointList FINAL_list3x3_positive = new TSD.PointList();
                        TSD.PointList FINAL_list3x3_negative = new TSD.PointList();
                        TSG.Vector VECTOR1 = new TSG.Vector();
                        TSG.Vector VECTOR2 = new TSG.Vector();
                        /////////////////////////////////////////////////// ASSIGNING VECTOR AND ADDING TOP POINT IN 3X3 DIMENSION(BEAM TOP FLANGE)//////////////////////////////////////////
                        try
                        {
                            foreach (TSG.Point pt in FINAL_list3x3)
                            {
                                if (Convert.ToInt64(pt.X) > 0)
                                {
                                    FINAL_list3x3_positive.Add(pt);



                                }
                                else if (Convert.ToInt64(pt.X) < 0)
                                {
                                    FINAL_list3x3_negative.Add(pt);


                                }
                            }
                        }
                        catch
                        {
                        }

                        string prof_typ = SkTeklaDrawingUtility.get_report_properties(mypart, "PROFILE_TYPE");

                        if (prof_typ == "L")
                        {
                            skBoltHandler.ANGLE_BOLT_DIM(FINAL_list3x3_positive, FINAL_list3x3_negative, height, current_view, MAINPART_PROFILE_VALUES, maxx, minx, defaultADFile);
                        }
                        else
                        {
                            FINAL_list3x3_positive.Add(new TSG.Point((Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));

                            VECTOR1 = new TSG.Vector(1, 0, 0);

                            FINAL_list3x3_negative.Add(new TSG.Point(-(Convert.ToDouble(MAINPART_PROFILE_VALUES[1])) / 2, (Convert.ToDouble(MAINPART_PROFILE_VALUES[0])) / 2, 0));

                            VECTOR2 = new TSG.Vector(-1, 0, 0);


                            /////////////////////////////////////////////////// inserting bolt 3X3 dimension ////////////////////////////////////////////////////////////////////////////////////////

                            try
                            {
                                double distance1 = Math.Abs(Math.Abs(FINAL_list3x3_positive[0].X) - Math.Abs(maxx));
                                dim_3x3.CreateDimensionSet(current_view, FINAL_list3x3_positive, VECTOR1, distance1 + x1, fixed_attributes);
                                x1 = x1 + 50;
                            }
                            catch
                            {
                            }

                            try
                            {
                                double distance1 = Math.Abs(Math.Abs(FINAL_list3x3_negative[0].X) - Math.Abs(minx));
                                dim_3x3.CreateDimensionSet(current_view, FINAL_list3x3_negative, VECTOR2, distance1 + x2, fixed_attributes);
                                x2 = x2 + 50;
                            }
                            catch
                            {
                            }
                        }


                    }


                    /////////////////END OF 3x3 dimension for SECTION VIEW////////////////////////


                    //////////////////////////////////////////pour stopper code//////////////////////////////////////////////////

                    foreach (var list_of_parts in list_section_flange[z].PartList)
                    {


                        TSM.Part mypart = list_of_parts as TSM.Part;
                        TSD.PointList mypt = boundingBoxHandler.BoundingBoxSort(list_of_parts, current_view, SKSortingHandler.SortBy.Z);
                        TSD.PointList mypt_final = new TSD.PointList();
                        TSD.PointList mypt_finalFOR_LEG = new TSD.PointList();
                        double height11 = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]);
                        double h1 = Convert.ToInt64(height11 / 2);
                        string prof_type = "";
                        mypart.GetReportProperty("PROFILE_TYPE", ref prof_type);
                        mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.CoordinateSystem angle_cood = list_of_parts.GetCoordinateSystem();
                        mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        if (!mypart.Name.Contains("STUD") && !mypart.Equals(main_part) && !mypart.Name.Contains("GUSSET"))
                        {

                            if ((Convert.ToInt64(mypt[0].Y) >= h1) && (Convert.ToInt64(mypt[1].Y) >= h1))
                            {
                                TSM.ModelObjectEnumerator enum_for_bolt = mypart.GetBolts();
                                int s = enum_for_bolt.GetSize();
                                if (s > 0)
                                {
                                    //if (!angle_cood.AxisX.X.Equals(0))
                                    //{
                                    List<TSG.Point[,]> check_for_bolt_in_part = new List<TSG.Point[,]>();
                                    while (enum_for_bolt.MoveNext())
                                    {
                                        TSM.BoltGroup drgbolt = enum_for_bolt.Current as TSM.BoltGroup;
                                        TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);
                                        check_for_bolt_in_part.Add(POINT_FOR_BOLT_MATRIX);
                                    }
                                    bool result_for_check_for_bolt_in_part = check_for_bolt_in_part.All(x => x == null);
                                    if ((result_for_check_for_bolt_in_part == true) && (prof_type != "L"))
                                    {
                                        if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {


                                                mypt_final.Add(mypt[1]);


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }
                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }




                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {
                                                mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }




                                                ///////////////   face_of_angle_logic_need_to_be_copied////////

                                                List<AngleFaceArea> myreq = facePointHandler.GetFaceAreasForAngle(mypart);
                                                TSD.PointList p1 = skAngleHandler.angle_pts_for_section(myreq, current_view);

                                                if (mypt[1].X > mypt[0].X)
                                                {
                                                    mypt_finalFOR_LEG.Add(new TSG.Point(mypt[0].X, mypt[0].Y, mypt[1].Z));
                                                    mypt_finalFOR_LEG.Add(new TSG.Point(mypt[0].X, mypt[1].Y, mypt[0].Z));

                                                }
                                                else
                                                {
                                                    mypt_finalFOR_LEG.Add(mypt[0]);
                                                    mypt_finalFOR_LEG.Add(mypt[1]);
                                                }
                                                try
                                                {
                                                    TSG.Vector myvector = new TSG.Vector();
                                                    double distance1 = 0;
                                                    if (mypt_finalFOR_LEG[0].X < 0)
                                                    {
                                                        myvector = new TSG.Vector(-1, 0, 0);
                                                        //distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(minx));
                                                        distance1 = Math.Abs(minx) - Math.Abs(mypt_finalFOR_LEG[0].X);
                                                        distance1 = distance1 + 150;
                                                    }
                                                    else
                                                    {

                                                        myvector = new TSG.Vector(1, 0, 0);
                                                        distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                        distance1 = distance1 + 150;
                                                    }


                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, myvector, distance1, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }


                                            }

                                        }



                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }

                                    }
                                    else
                                    {
                                        if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {


                                                mypt_final.Add(mypt[1]);


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }
                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }




                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                ///////////////////////2018*///////////////////
                                                TSG.Vector VECTOR_FOR_ANGLE = new TSG.Vector();
                                                if ((mypt[0].X > 0) && (mypt[1].X > 0))
                                                {
                                                    VECTOR_FOR_ANGLE = new TSG.Vector(1, 0, 0);

                                                }
                                                else
                                                {

                                                    VECTOR_FOR_ANGLE = new TSG.Vector(-1, 0, 0);

                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(new TSG.Point(mypt[0].X, mypt[1].Y, mypt[1].Z));
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, VECTOR_FOR_ANGLE, distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }


                                            }

                                        }



                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                    //////////////////WHY/////////////////
                                                    //dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }
                                                ///////////////////////////////////2018/////////////////////////////////////////////////////
                                                TSG.Vector VECTOR_FOR_ANGLE = new TSG.Vector();
                                                if ((mypt[0].X > 0) && (mypt[1].X > 0))
                                                {
                                                    VECTOR_FOR_ANGLE = new TSG.Vector(1, 0, 0);

                                                }
                                                else
                                                {

                                                    VECTOR_FOR_ANGLE = new TSG.Vector(-1, 0, 0);

                                                }
                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, VECTOR_FOR_ANGLE, distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }

                                    }

                                    //}
                                    //else
                                    //{ 
                                    //}
                                }


                                else
                                {


                                    if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {



                                            mypt_final.Add(mypt[1]);


                                            mypt_final.Add(new TSG.Point(0, 0, 0));

                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }
                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }



                                        }

                                    }




                                    if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {

                                            mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                            mypt_final.Add(new TSG.Point(0, 0, 0));

                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }

                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }


                                        }

                                    }



                                    if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {

                                            mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));


                                            mypt_final.Add(new TSG.Point(0, 0, 0));

                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(maxy));
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }

                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }



                                        }

                                    }


                                }

                            }

                            else if ((Convert.ToInt64(mypt[0].Y) <= -h1) && (Convert.ToInt64(mypt[1].Y) >= -h1))
                            {
                                TSM.ModelObjectEnumerator enum_for_bolt = mypart.GetBolts();
                                int s = enum_for_bolt.GetSize();
                                if (s > 0)
                                {
                                    List<TSG.Point[,]> check_for_bolt_in_part = new List<TSG.Point[,]>();
                                    while (enum_for_bolt.MoveNext())
                                    {
                                        TSM.BoltGroup drgbolt = enum_for_bolt.Current as TSM.BoltGroup;
                                        TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt, current_view);
                                        check_for_bolt_in_part.Add(POINT_FOR_BOLT_MATRIX);
                                    }
                                    bool result_for_check_for_bolt_in_part = check_for_bolt_in_part.All(x => x == null);

                                    if ((result_for_check_for_bolt_in_part == true) && (prof_type != "L"))
                                    {
                                        if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {



                                                mypt_final.Add(mypt[1]);

                                                mypt_final.Add(new TSG.Point(0, 0, 0));


                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }


                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }




                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }



                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                if ((Convert.ToInt64(mypt[0].Y) < 0) && (Convert.ToInt64(mypt[1].Y) < 0))
                                                {

                                                    mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                    mypt_final.Add(new TSG.Point(0, 0, 0));



                                                    TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                        dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                    mypt_finalFOR_LEG.Add(mypt[0]);
                                                    mypt_finalFOR_LEG.Add(mypt[1]);
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                        dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                }


                                                if ((Convert.ToInt64(mypt[0].Y) > 0) && (Convert.ToInt64(mypt[1].Y) > 0))
                                                {
                                                    mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                    mypt_final.Add(new TSG.Point(0, 0, 0));



                                                    TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                        dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                    mypt_finalFOR_LEG.Add(mypt[0]);
                                                    mypt_finalFOR_LEG.Add(mypt[1]);
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                        dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                }

                                            }

                                        }


                                    }
                                    else
                                    {

                                        if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(0, 0, 0));
                                                mypt_final.Add(mypt[1]);




                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }


                                                TSG.Vector VECTOR_FOR_ANGLE = new TSG.Vector();
                                                if ((mypt[0].X > 0) && (mypt[1].X > 0))
                                                {
                                                    VECTOR_FOR_ANGLE = new TSG.Vector(1, 0, 0);

                                                }
                                                else
                                                {

                                                    VECTOR_FOR_ANGLE = new TSG.Vector(-1, 0, 0);

                                                }
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                mypt_finalFOR_LEG.Add(new TSG.Point(mypt[1].X, mypt[0].Y, mypt[0].Z));


                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, VECTOR_FOR_ANGLE, distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }




                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                                mypt_final.Add(new TSG.Point(0, 0, 0));

                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    ///1//////
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(new TSG.Point(mypt[0].X, mypt[1].Y, mypt[1].Z));
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(-1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }



                                            }

                                        }



                                        if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                        {


                                            if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                            {

                                                if ((Convert.ToInt64(mypt[0].Y) < 0) && (Convert.ToInt64(mypt[1].Y) < 0))
                                                {

                                                    mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                    mypt_final.Add(new TSG.Point(0, 0, 0));



                                                    TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));

                                                        ////////////////////////WHY////////////////
                                                        //dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    ////////////////////////////////////////////////2018////////////////////////////////////////

                                                    TSG.Vector VECTOR_FOR_ANGLE = new TSG.Vector();
                                                    if ((mypt[0].X > 0) && (mypt[1].X > 0))
                                                    {
                                                        VECTOR_FOR_ANGLE = new TSG.Vector(1, 0, 0);

                                                    }
                                                    else
                                                    {

                                                        VECTOR_FOR_ANGLE = new TSG.Vector(-1, 0, 0);

                                                    }

                                                    mypt_finalFOR_LEG.Add(mypt[0]);
                                                    mypt_finalFOR_LEG.Add(mypt[1]);
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                        dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, VECTOR_FOR_ANGLE, distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                }


                                                if ((Convert.ToInt64(mypt[0].Y) > 0) && (Convert.ToInt64(mypt[1].Y) > 0))
                                                {
                                                    mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                    mypt_final.Add(new TSG.Point(0, 0, 0));



                                                    TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                        dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                    mypt_finalFOR_LEG.Add(mypt[0]);
                                                    mypt_finalFOR_LEG.Add(mypt[1]);
                                                    try
                                                    {
                                                        double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                        dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                }

                                            }

                                        }


                                    }


                                }
                                else
                                {


                                    if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {


                                            mypt_final.Add(mypt[1]);

                                            mypt_final.Add(new TSG.Point(0, 0, 0));


                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }


                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }



                                        }

                                    }




                                    if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) < 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {


                                            mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));


                                            mypt_final.Add(new TSG.Point(0, 0, 0));

                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }

                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(-1, 0, 0), distance1 + 150, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }



                                        }

                                    }



                                    if ((Convert.ToInt64(mypt[0].X) < 0) && (Convert.ToInt64(mypt[1].X) > 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {

                                            if ((Convert.ToInt64(mypt[0].Y) < 0) && (Convert.ToInt64(mypt[1].Y) < 0))
                                            {


                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                mypt_final.Add(new TSG.Point(0, 0, 0));



                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                            }


                                            if ((Convert.ToInt64(mypt[0].Y) > 0) && (Convert.ToInt64(mypt[1].Y) > 0))
                                            {
                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                                mypt_final.Add(new TSG.Point(0, 0, 0));



                                                TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                    dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, -1, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                                mypt_finalFOR_LEG.Add(mypt[0]);
                                                mypt_finalFOR_LEG.Add(mypt[1]);
                                                try
                                                {
                                                    double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                    dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, new TSG.Vector(1, 0, 0), distance1 + 150, fixed_attributes);
                                                }
                                                catch
                                                {
                                                }

                                            }

                                        }

                                    }

                                }

                            }
                            else
                            {
                                if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                {
                                    TSM.ModelObjectEnumerator B = mypart.GetBolts();
                                    int C = B.GetSize();
                                    if (C == 0)
                                    {
                                        if (!prof_type.Equals("B"))
                                        {
                                            mypt_finalFOR_LEG.Add(mypt[0]);
                                            mypt_finalFOR_LEG.Add(mypt[1]);
                                            if (mypart.Position.Rotation.Equals(TSM.Position.RotationEnum.BACK))
                                            {

                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                            }
                                            if ((mypart.Position.Rotation.Equals(TSM.Position.RotationEnum.FRONT)) && (mypart.Position.Plane.Equals(TSM.Position.PlaneEnum.LEFT)))
                                            {
                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[0].Y, 0));
                                            }
                                            if ((mypart.Position.Rotation.Equals(TSM.Position.RotationEnum.FRONT)) && (mypart.Position.Plane.Equals(TSM.Position.PlaneEnum.RIGHT)))
                                            {
                                                mypt_final.Add(new TSG.Point(mypt[1].X, mypt[0].Y, 0));
                                            }
                                            //if ((mypart.Position.Rotation.Equals(TSM.Position.RotationEnum.TOP)) && (mypart.Position.Plane.Equals(TSM.Position.PlaneEnum.RIGHT)))
                                            //{
                                            //    mypt_final.Add(new TSG.Point(mypt[1].X, mypt[1].Y, 0));
                                            //}
                                            TSG.Vector MYVECTOR = new TSG.Vector();
                                            if (mypt[1].X > 0)
                                            {
                                                MYVECTOR = new TSG.Vector(1, 0, 0);
                                            }
                                            else if (mypt[1].X < 0)
                                            {
                                                MYVECTOR = new TSG.Vector(-1, 0, 0);
                                            }


                                            mypt_final.Add(new TSG.Point(0, h1, 0));
                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_final[0].Y) - Math.Abs(miny));
                                                dim.CreateDimensionSet(current_view, mypt_final, MYVECTOR, distance1 + 150, fixed_attributes);

                                            }
                                            catch
                                            {
                                            }

                                            try
                                            {
                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, mypt_finalFOR_LEG, MYVECTOR, distance1 + 150, fixed_attributes);

                                            }
                                            catch
                                            {
                                            }

                                            ////////////////////////////////2018/////////////////////////////////////////////////////
                                            //if (Convert.ToInt64(angle_cood.AxisX.X) != 0)
                                            //{
                                            try
                                            {

                                                TSD.PointList pt_lit_for_angle = new TSD.PointList();
                                                pt_lit_for_angle.Add(facePointHandler.GetFacePointForAngleSectionView(mypart, current_view));
                                                pt_lit_for_angle.Add(new TSG.Point(0, h1, 0));

                                                double distance1 = Math.Abs(Math.Abs(mypt_finalFOR_LEG[0].X) - Math.Abs(maxx));
                                                dim.CreateDimensionSet(current_view, pt_lit_for_angle, MYVECTOR, distance1 + 200, fixed_attributes);

                                            }
                                            catch
                                            {
                                            }
                                            //}


                                        }
                                    }
                                }
                            }

                        }

                    }

                    //////////////////////////////////////////end of pour stopper////////////////////////////////////////////////////////





                    Type[] type_for_mark = new Type[] { typeof(TSD.Mark), typeof(TSD.WeldMark) };

                    //////////////////////////////////////////seating angle code//////////////////////////////////////////////////
                    foreach (var list_of_parts in list_section_flange[z].PartList)
                    {



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



                                        if (list_section_flange[z].PartList.Any(p => p.Identifier.ID == modelpart.Identifier.ID))
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




                                ArrayList MERGE_WELD = new ArrayList();
                                //TSD.DrawingObjectEnumerator enumcheck1 = weldmark.GetObjects();
                                Identifier id = weldmark.ModelIdentifier;
                                TSM.BaseWeld weld = (new TSM.Model().SelectModelObject(id) as TSM.BaseWeld);
                                TSM.Part mainpart = (weld.MainObject as TSM.Part);
                                TSM.Part secondary_part = (weld.SecondaryObject as TSM.Part);

                                if ((list_section_flange[z].PartList.Any(p => p.Identifier.ID == mainpart.Identifier.ID)) || ((list_section_flange[z].PartList.Any(p => p.Identifier.ID == secondary_part.Identifier.ID))))
                                {

                                }
                                else
                                {
                                    weldmark.Delete();
                                }
                            }
                        }
                        TSM.Part mm = list_of_parts as TSM.Part;
                        string prof_type = "";
                        mm.GetReportProperty("PROFILE_TYPE", ref prof_type);
                        TSD.PointList bounding_box_x = boundingBoxHandler.BoundingBoxSort(mm, current_view);
                        TSD.PointList bounding_box_y = boundingBoxHandler.BoundingBoxSort(mm, current_view, SKSortingHandler.SortBy.Y);

                        TSD.PointList mypt = boundingBoxHandler.BoundingBoxSort(list_of_parts, current_view, SKSortingHandler.SortBy.Z);
                        TSD.PointList mypt_final = new TSD.PointList();
                        TSD.PointList mypt_final_FOR_LEG = new TSD.PointList();
                        double height11 = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                        double h1 = Convert.ToInt64(height11);
                        TSG.CoordinateSystem PLATE_COORD = mm.GetCoordinateSystem();
                        TSG.Vector PLATE_X_VECTOR = PLATE_COORD.AxisX;
                        TSG.Vector PLATE_Y_VECTOR = PLATE_COORD.AxisY;
                        TSG.Vector PLATE_Z_VECTOR = PLATE_X_VECTOR.Cross(PLATE_Y_VECTOR);
                        PLATE_Z_VECTOR.Normalize();
                        if ((prof_type == "B") && (PLATE_Z_VECTOR.Z != 0))
                        {
                            TSM.ModelObjectEnumerator myplate_check_for_bolts = mm.GetBolts();




                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            int a = myplate_check_for_bolts.GetSize();

                            if (a == 0)
                            {


                                ArrayList PROFILE = catalogHandler.Getcatalog_values_WITH_FLANGE_THICK(main_part);
                                double HEIGHT = Math.Abs(Convert.ToInt64(mypt[0].Y) - Convert.ToInt64(mypt[1].Y));
                                double W_HT = Convert.ToInt16(PROFILE[0]);
                                double THICK = Convert.ToInt16(PROFILE[3]);
                                double FULL_HT = W_HT - (2 * THICK);
                                if (HEIGHT + 5 < FULL_HT)
                                {
                                    if ((Convert.ToInt64(mypt[0].X) <= 0) && (Convert.ToInt64(mypt[1].X) <= 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {


                                            mypt_final.Add(new TSG.Point(mypt[0].X, mypt[1].Y, 0));


                                            mypt_final.Add(new TSG.Point(0, height11, 0));


                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();

                                            try
                                            {
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(0, 1, 0), 200, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }
                                            try
                                            {
                                                dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(-1, 0, 0), 200, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }


                                        }

                                    }




                                    if ((Convert.ToInt64(mypt[0].X) >= 0) && (Convert.ToInt64(mypt[1].X) >= 0))
                                    {


                                        if ((current_view.RestrictionBox.MaxPoint.X > mypt[0].Z) && (current_view.RestrictionBox.MinPoint.X < mypt[1].Z))
                                        {
                                            mypt_final.Add(mypt[1]);


                                            mypt_final.Add(new TSG.Point(0, height11, 0));

                                            TSD.StraightDimensionSetHandler dim = new TSD.StraightDimensionSetHandler();
                                            try
                                            {
                                                //dim.CreateDimensionSet(current_view, mypt_final, new TSG.Vector(1, 0, 0), 200, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }

                                            mypt_final_FOR_LEG.Add(mypt[0]);
                                            mypt_final_FOR_LEG.Add(mypt[1]);
                                            try
                                            {
                                                //dim.CreateDimensionSet(current_view, mypt_final_FOR_LEG, new TSG.Vector(1, 0, 0), 200, fixed_attributes);
                                            }
                                            catch
                                            {
                                            }



                                        }



                                    }





                                }




                            }



                            else
                            {
                                ArrayList MM = new ArrayList();
                                while (myplate_check_for_bolts.MoveNext())
                                {
                                    TSD.PointList PLATE_CONNECTING_SIDE_POINTS = new TSD.PointList();

                                    TSM.BoltGroup MODELbolt = myplate_check_for_bolts.Current as TSM.BoltGroup;
                                    TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(MODELbolt, current_view);
                                    ///////////////////////////////////////////////////condition for depth adjustment////////////////////////////////////////////////////////////////////////////////////////////
                                    //////////////////////////////////////////////////// filtering bolts which lies within the section depth/////////////////////////////////////////////////////////////////////                       
                                    double upper_limit = current_view.RestrictionBox.MaxPoint.Z;
                                    double lower_limit = current_view.RestrictionBox.MinPoint.Z;
                                    if (POINT_FOR_BOLT_MATRIX != null)
                                    {
                                    }
                                    else
                                    {

                                        TSD.PointList p1 = boundingBoxHandler.BoundingBoxSort(mm, current_view, SKSortingHandler.SortBy.Y);



                                        TSG.CoordinateSystem m = MODELbolt.GetCoordinateSystem();
                                        TSM.Part mw = MODELbolt.PartToBeBolted;
                                        TSM.Part mw1 = MODELbolt.PartToBoltTo;
                                        ArrayList mw2 = MODELbolt.OtherPartsToBolt;
                                        if (bounding_box_x[0].X > 0)
                                        {
                                            if (!mw.Identifier.ID.Equals(mm.Identifier.ID))
                                            {
                                                TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                TSG.Point pz = tokkk.Transform(kl.Origin);
                                                double Y_value = pz.Y;

                                                if (Convert.ToInt64(Y_value) > Convert.ToInt64(p1[1].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));

                                                }
                                                else if (Convert.ToInt64(Y_value) < Convert.ToInt64(p1[0].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));

                                                }


                                            }
                                            if (!mw1.Identifier.ID.Equals(mm.Identifier.ID))
                                            {
                                                TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();

                                                TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                TSG.Point pz = tokkk.Transform(kl.Origin);
                                                double Y_value = pz.Y;

                                                if (Convert.ToInt64(Y_value) > Convert.ToInt64(p1[1].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));

                                                }
                                                else if (Convert.ToInt64(Y_value) < Convert.ToInt64(p1[0].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));

                                                }
                                            }
                                            double DISTANCE = Math.Abs(current_view.RestrictionBox.MaxPoint.X);


                                            PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(0, height11, 0));

                                            try
                                            {
                                                dim_3x3.CreateDimensionSet(current_view as TSD.ViewBase, PLATE_CONNECTING_SIDE_POINTS, new TSG.Vector(-1, 0, 0), DISTANCE + 100, dim_font_height1);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        if (bounding_box_x[0].X < 0)
                                        {
                                            if (!mw.Identifier.ID.Equals(mm.Identifier.ID))
                                            {
                                                TSG.CoordinateSystem kl = mw.GetCoordinateSystem();

                                                TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                TSG.Point pz = tokkk.Transform(kl.Origin);
                                                double Y_value = pz.Y;

                                                if (Convert.ToInt64(Y_value) > Convert.ToInt64(p1[1].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));

                                                }
                                                else if (Convert.ToInt64(Y_value) < Convert.ToInt64(p1[0].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[0].Y, bounding_box_y[0].Z));

                                                }


                                            }
                                            if (!mw1.Identifier.ID.Equals(mm.Identifier.ID))
                                            {
                                                TSG.CoordinateSystem kl = mw1.GetCoordinateSystem();

                                                TSG.Matrix tokkk = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                                                TSG.Point pz = tokkk.Transform(kl.Origin);
                                                double Y_value = pz.Y;

                                                if (Convert.ToInt64(Y_value) > Convert.ToInt64(p1[1].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[1].X, bounding_box_y[1].Y, bounding_box_y[0].Z));

                                                }
                                                else if (Convert.ToInt64(Y_value) < Convert.ToInt64(p1[0].Y))
                                                {
                                                    PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(bounding_box_x[0].X, bounding_box_y[0].Y, bounding_box_y[0].Z));

                                                }
                                            }

                                            double DISTANCE = current_view.RestrictionBox.MaxPoint.X;

                                            PLATE_CONNECTING_SIDE_POINTS.Add(new TSG.Point(0, height11, 0));
                                            try
                                            {
                                                dim_3x3.CreateDimensionSet(current_view as TSD.ViewBase, PLATE_CONNECTING_SIDE_POINTS, new TSG.Vector(1, 0, 0), DISTANCE + 100, dim_font_height1);
                                            }
                                            catch
                                            {
                                            }
                                        }






                                    }
                                }
                            }

                        }
                    }
                    //////////////////////////////////////////end of seating angle code////////////////////////////////////////////////////////
                    List<Guid> FINAL_BOLTMARK_POS = new List<Guid>();
                    List<Guid> FINAL_BOLTMARK_NEG = new List<Guid>();

                    ArrayList PARTMARK_TO_BE_PROVIDED_SECTION = new ArrayList();
                    ArrayList BOLTMARK_TO_BE_PROVIDED_SECTION_POS = new ArrayList();
                    ArrayList BOLTMARK_TO_BE_PROVIDED_SECTION_NEG = new ArrayList();

                    Type type_for_PART = typeof(TSD.Part);
                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = current_view.GetAllObjects(type_for_PART);
                    while (my_top_view_dimension_check.MoveNext())
                    {
                        TSD.Part DRG_PART = my_top_view_dimension_check.Current as TSD.Part;
                        TSM.ModelObject modelpart = new TSM.Model().SelectModelObject(DRG_PART.ModelIdentifier);


                        bool CHECK = SECTION_VIEW_PARTMARK_TO_RETAIN.Any(X => X.Equals(modelpart.Identifier.GUID));

                        if (CHECK == true)
                        {
                            PARTMARK_TO_BE_PROVIDED_SECTION.Add(DRG_PART);

                        }

                    }
                    Type type_for_BOLT = typeof(TSD.Bolt);
                    TSD.DrawingObjectEnumerator SECTION_VIEW_BOLTMARK_ENUM = current_view.GetAllObjects(type_for_BOLT);
                    while (SECTION_VIEW_BOLTMARK_ENUM.MoveNext())
                    {
                        TSD.Bolt DRG_BOLT = SECTION_VIEW_BOLTMARK_ENUM.Current as TSD.Bolt;
                        TSM.ModelObject modelpart = new TSM.Model().SelectModelObject(DRG_BOLT.ModelIdentifier);

                        bool CHECK = SECTION_VIEW_BOLTMARK_TO_RETAIN_POS.Any(X => X.Equals(modelpart.Identifier.GUID));
                        bool CHECK1 = SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG.Any(X => X.Equals(modelpart.Identifier.GUID));

                        if (CHECK == true)
                        {
                            BOLTMARK_TO_BE_PROVIDED_SECTION_POS.Add(DRG_BOLT);
                            FINAL_BOLTMARK_POS.Add(DRG_BOLT.ModelIdentifier.GUID);


                        }
                        if (CHECK1 == true)
                        {
                            BOLTMARK_TO_BE_PROVIDED_SECTION_NEG.Add(DRG_BOLT);
                            FINAL_BOLTMARK_NEG.Add(DRG_BOLT.ModelIdentifier.GUID);

                        }

                    }
                    foreach (TSD.Part PART_GUID in PARTMARK_TO_BE_PROVIDED_SECTION)
                    {

                        my_handler.GetDrawingObjectSelector().SelectObject(PART_GUID);
                        TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                        my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    }


                }


            }
        }
    }
}
