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


namespace SK.Tekla.Drawing.Automation.Services
{
    public class BeamDrawingCreator
    {
        private SKBoundingBoxHandler boundingBoxHandler;
        private SKFacePointHandler facePointHandler;
        private SKSortingHandler sortingHandler = new SKSortingHandler();

        private SKCatalogHandler catalogHandler = new SKCatalogHandler();

        private DuplicateRemover duplicateRemover;
        private SKDrawingHandler drawingHandler;
        private BoltMatrixHandler boltMatrixHandler;



        

        private SKBottomView skBottomView;

        private SKTopViewBuilderOld sKTopViewBuilder;


        

        private BoltMarkDetailing boltMarkDetailing;

        private SKWeldHandler weldHandler;

        private StreamlineDrawing streamlineDrawing;

    

        private SKBoltHandler skBoltHandler;

        private SKAngleHandler skAngleHandler;

        
        private CustomInputModel _inputModel;

        private SKSectionViewOld skSectionView;

        private SKSectionFlangeBuilder  skSectionFlangeBuilder;

        private SKDimensionHandler skDimensionHandler;

        private readonly List<string> _messageStore = new List<string>();

        private readonly List<string> beamDrawings = new List<string>();

        private static readonly log4net.ILog _logger =
       log4net.LogManager.GetLogger(typeof(BeamDrawingCreator));

        public BeamDrawingCreator(List<string> messageStore, CustomInputModel inputModel)
        {
            _messageStore = messageStore;
            _inputModel = inputModel;
            boundingBoxHandler = new SKBoundingBoxHandler();
            streamlineDrawing = new StreamlineDrawing();
            facePointHandler = new SKFacePointHandler(boundingBoxHandler, _inputModel);

            duplicateRemover = new DuplicateRemover(sortingHandler);
            drawingHandler = new SKDrawingHandler(boundingBoxHandler, catalogHandler, _inputModel);
            boltMatrixHandler = new BoltMatrixHandler(sortingHandler, catalogHandler);
        }

        public void PrepareDrawing(TSM.Model currentModel, ref int proct, ref int errct)
        {
            boltMarkDetailing = new BoltMarkDetailing(catalogHandler, _inputModel);
            
            skBoltHandler = new SKBoltHandler(catalogHandler, boltMatrixHandler, _inputModel);
            weldHandler = new SKWeldHandler(_inputModel);
            skAngleHandler = new SKAngleHandler(boltMatrixHandler);

            skDimensionHandler = new SKDimensionHandler(weldHandler, skBoltHandler, catalogHandler, boltMatrixHandler, boundingBoxHandler,
               sortingHandler, facePointHandler, drawingHandler, duplicateRemover, _inputModel);


            skSectionView = new SKSectionViewOld(catalogHandler, boltMatrixHandler, boundingBoxHandler,
                sortingHandler, facePointHandler, drawingHandler, duplicateRemover, _inputModel, streamlineDrawing,
                skAngleHandler, skBoltHandler);
            skSectionFlangeBuilder = new SKSectionFlangeBuilder(catalogHandler, boltMatrixHandler, boundingBoxHandler,
                sortingHandler, facePointHandler, drawingHandler, duplicateRemover, _inputModel, streamlineDrawing,
                skAngleHandler, skBoltHandler);
            skBottomView = new SKBottomView(sortingHandler, skDimensionHandler.GetFlangeOutDimension(), duplicateRemover, _inputModel);

            sKTopViewBuilder = new SKTopViewBuilderOld(sortingHandler, skDimensionHandler.GetFlangeOutDimension(), duplicateRemover,
                boltMatrixHandler, _inputModel);
            bool tekla_open = TeklaStructures.Connect();
            if (!tekla_open)
            {
                throw new Exception("Connection with Tekla cannot be established!!!");
            }
            string msgCaption = String.Empty;
            {
                _logger.Debug("Tekla Connection established....");
                DateTime s_tm = new DateTime();
                TSM.Model mymodel = new TSM.Model();
                Type type_for_bolt = typeof(TSD.Bolt);
                Type type_for_part = typeof(TSD.Part);
                TSD.DrawingHandler my_handler = new TSD.DrawingHandler();
                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                TSM.UI.Picker picker_for_beam = new TSM.UI.Picker();
                TSM.ModelObjectEnumerator enum_for_beam_pick = null;
                try
                {
                    enum_for_beam_pick =
                         picker_for_beam.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS, "PICK ALL BEAMS");
                }
                catch (ApplicationException ae)
                {
                    MessageBox.Show($"User Interupted during picking up objects. Please try again",
                    msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    System.Environment.Exit(360);
                }

                string skDefaultADFile = "";

                var drg_att_list = ConfigureDrawingData.PrepareDrawingAttributes(mymodel, _inputModel.SheetSelected, ref skDefaultADFile);

                string defaultADFile = "";

                //start showing the progress bar
                //pbar.Maximum = selct + 4;
                //pbar.Value = 0;
                //pbar.Visible = true;
                _logger.Debug($"Total Beams picked up ....{enum_for_beam_pick.GetSize()}");

                //Check for already drawing created
                List<string> SKDrawings = new List<string>();


                while (enum_for_beam_pick.MoveNext())
                {
                    //pbar.Value = pbar.Value + 1;
                    //pbar.Refresh();


                    string DRG_NUMBER = string.Empty;
                    string DRG_REMARK = string.Empty;
                    DateTime start_assy_tm = DateTime.Now;
                    TimeSpan span = new TimeSpan();

                    TSM.ModelObject currentBeam = enum_for_beam_pick.Current;
                    TSM.Assembly currentBeamAsAssembly = currentBeam as TSM.Assembly;
                    if (currentBeamAsAssembly == null)
                    {
                        _logger.Fatal($"Assembly is not selected: {currentBeam}");
                        errct++;
                        continue;
                    }

                    string assembly_pos = SkTeklaDrawingUtility.
                        GetSKReportProperty(currentBeamAsAssembly, "ASSEMBLY_POS");

                    if (assembly_pos.Contains('?'))
                    {
                        _logger.Fatal($"Numbering issue for drawing {assembly_pos}");
                        DRG_REMARK = "Numbering???";
                        errct++;
                        continue;
                    }

                    if (SKDrawings.Contains(assembly_pos.ToUpper()))
                    {
                        _logger.Debug($"Drawing is existing {assembly_pos.ToUpper()} ");
                        continue;
                    }
                    try
                    {

                        SKDrawings.Add(assembly_pos.ToUpper());

                        s_tm = DateTime.Now;
                        TSM.Beam currentBeamMainPart = currentBeamAsAssembly.GetMainPart() as TSM.Beam;
                        if (currentBeamMainPart == null)
                        {
                            _logger.Fatal($"Main Part of the assembly is not beam");
                            DRG_REMARK = "Beam??";
                            errct++;
                            continue;
                        }

                        Console.WriteLine("DRG_NUMBER.....>" + assembly_pos);
                        double output = 0;
                        currentBeamMainPart.GetReportProperty("LENGTH", ref output);

                        //Function will be given by Viswa for layout - Req_attribute

                        //List<req_attribute> required_attribute  =
                        //    drawingHandler.Drawing_create_and_delete_all_dimensions_except_assembly_dim_sheet_check(mymodel, currentBeam, 
                        //    drg_att_list, out currentAssemblyDrawing);

                        SKLayout skLayout = new SKLayout()
                        {
                            attribute = "VBR_BEAM_A1",
                            adFileName = "SK_BEAM_A1",
                            //attribute = "VBR_BEAM_A3",
                            //adFileName = "SK_BEAM_A3",
                            scale = _inputModel.Scale,
                            minLenth = _inputModel.MinLength
                        };
                        _logger.Debug($"Layout - {skLayout.ToString()}");

                        mymodel.GetWorkPlaneHandler().
                         SetCurrentTransformationPlane(new TSM.TransformationPlane(currentBeamMainPart.GetCoordinateSystem()));

                        TSG.Matrix VIEW_MATRIX = TSG.MatrixFactory.ToCoordinateSystem(currentBeamMainPart.GetCoordinateSystem());

                        TSM.Part main_part = currentBeamMainPart;
                        TSM.Assembly ASSEMBLY = currentBeamAsAssembly;

                        string BOTTOM_VIEW;
                        string TOP_VIEW;
                        string BOTTOM_VIEW_TOCREATE;
                        string TOP_VIEW_TOCREATE;
                        

             
                        TSG.Point start_pt_for_section_view_aling = new TSG.Point();



                        double SCALE = skLayout.scale;
                        double MINI_LEN = skLayout.minLenth;
                        defaultADFile = skLayout.adFileName;
                        //VBR_A3

                        List<TSD.View> bottom_view_list = new List<TSD.View>();
                        List<TSD.View> bottom_view_FLANGE_CUT_LIST = new List<TSD.View>();



                        #region Module-2
                        //SCALE = 8;
                        //MINI_LEN = 130;
                        Stopwatch SECTION_CREATION = new Stopwatch();
                        SECTION_CREATION.Start();
                        TSD.AssemblyDrawing currentAssemblyDrawing =
                            drawingHandler.ResetDrawingDimensionsExceptAssemblyDimensions(mymodel,
                            currentBeam, defaultADFile, main_part, output, ASSEMBLY,
                             SCALE, MINI_LEN);

                        List<SectionLocationWithParts> list = drawingHandler.list2;

                        _logger.Debug($"list: {list.Count}");

                        list.ForEach(_logger.Debug);
                        list.ForEach(Console.WriteLine);

                        List<SectionLocationWithParts> list_section_flange = drawingHandler.list_for_flange_section;

                        List<TSM.Part> list_of_parts_for_bottom_view_mark_retain = drawingHandler.list_of_parts_for_bottom_part_mark_retain;

                        List<TSD.RadiusDimension> list_of_radius = drawingHandler.list_of_radius_dim;

                        TSD.StraightDimension OVERALL_DIMENSION = drawingHandler.OVERALL_DIMENSION;
                        SECTION_CREATION.Stop();
                        #endregion
                        double mainpartlength = SkTeklaDrawingUtility.get_report_properties_double(main_part, "LENGTH");
                        string profile_type = "";
                        main_part.GetReportProperty("PROFILE_TYPE", ref profile_type);


                        ////////////////////////////////////////////////////GETTING CATALOG VALUES OF MAINPART////////////////////////////////////////////////////////////////////////           
                        List<double> MAINPART_PROFILE_VALUES = catalogHandler.Getcatalog_values(main_part);
                        double mainPartHeight = MAINPART_PROFILE_VALUES[0];

                        ////////////////////////////////////////////////////CHECKING FOR BOTTOM VIEW CREATION ////////////////////////////////////////////////////////////////////////////////////
                        TSM.Beam main = main_part as TSM.Beam;
                        TSG.Point p1_bottm = null;
                        TSG.Point p2_bottm = null;
                        TSG.Point p3_bottm = null;
                        TSG.Point p4_bottm = null;


                        skBottomView.BOTTOMVIEW_CHECK(main, out BOTTOM_VIEW, mainPartHeight, out p1_bottm, out p2_bottm, output, out BOTTOM_VIEW_TOCREATE);
                        sKTopViewBuilder.TOPVIEW_CHECK(main, out TOP_VIEW, mainPartHeight, out p3_bottm, out p4_bottm, output, out TOP_VIEW_TOCREATE);
                        string TOP_VIEW_needed = 
                            sKTopViewBuilder.TOPVIEW_needed(main, mainPartHeight, output);

                        ////////////////////////////////////////////////////getting views from beam assembly drg ///////////////////////////////////////////////////////////////////////////////////    
                        TSD.DrawingObjectEnumerator enum_for_views = currentAssemblyDrawing.GetSheet().GetAllViews();
                        TSG.Point mml = currentAssemblyDrawing.GetSheet().Origin;

                        TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                        //fixed_attributes.LoadAttributes("SK_BEAM")
                        fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;

                        fixed_attributes.Text.Font.Height =
                            SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, defaultADFile);


                        mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(main_part.GetCoordinateSystem()));


                        List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
                        //ArrayList list_of_secondaries = ASSEMBLY.GetSecondaries();
                        //List<Guid> nearside_parts = new List<Guid>();
                        //List<Guid> farside_parts = new List<Guid>();
                        //double WT1 = 0;
                        //if (profile_type == "U")
                        //{
                        //    TSG.Vector zvector1 = main_part.GetCoordinateSystem().AxisX.Cross(main_part.GetCoordinateSystem().AxisY);
                        //    mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                        //    zvector1.Normalize();
                        //    double WT3 = (catalog_values[1]);
                        //    if (zvector1.Z > 0)
                        //    {
                        //        WT1 = (-WT3 / 2);
                        //    }
                        //    else
                        //    {
                        //        WT1 = (WT3 / 2);
                        //    }
                        //}
                        //foreach (TSM.Part part in list_of_secondaries)
                        //{
                        //    TSD.PointList bbz = boundingBoxHandler.BoundingBoxSort(part, main_part as TSM.Beam, SKSortingHandler.SortBy.Z);

                        //    if (bbz[1].Z < WT1)
                        //    {
                        //        farside_parts.Add(part.Identifier.GUID);
                        //    }
                        //    else if (bbz[0].Z > WT1)
                        //    {
                        //        nearside_parts.Add(part.Identifier.GUID);
                        //    }
                        //}


                        mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        currentAssemblyDrawing.PlaceViews();
                        List<TSD.View> TOP_view_FLANGE_CUT_LIST = new List<TSD.View>();
                        #region codetocopy in drafter tool
                        List<Guid> FRONT_VIEW_PARTMARK_TO_RETAIN = new List<Guid>();
                        List<Guid> TOP_VIEW_PARTMARK_TO_RETAIN = new List<Guid>();
                        List<Guid> FRONT_VIEW_BOLTMARK_TO_RETAIN = new List<Guid>();
                        List<Guid> TOP_VIEW_BOLTMARK_TO_RETAIN = new List<Guid>();

                        ////////////////////////////////////////////////////view enum starts////////////////////////////////////////////////////////////////////////////////////////////////////////
                        while (enum_for_views.MoveNext())
                        {
                            TSD.View current_view = enum_for_views.Current as TSD.View;
                            _logger.Debug($"View in the drawing: {current_view.ViewType}");
                            double SHORTNENING_VALUE_FOR_BOTTOM_VIEW = 0;
                            ///////////////////////////////////////////////////front view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////


                            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                            {
                                #region FRONT_VIEW
                                //////////////////////////////////////////////////bolt rd dimension in front view using matrix function ///////////////////////////////////////////////////////////////////////
                                skDimensionHandler.ConfigureFrontViewBoldDimension(mymodel, currentAssemblyDrawing, main_part, output,
                                defaultADFile, list, list_of_radius, SCALE, MAINPART_PROFILE_VALUES, main,
                                ref FRONT_VIEW_PARTMARK_TO_RETAIN, ref FRONT_VIEW_BOLTMARK_TO_RETAIN, current_view, ref SHORTNENING_VALUE_FOR_BOTTOM_VIEW);

                                #endregion
                                #region bottom_view
                                //////////////////////////////////////////////////////////bottom view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////

                                skBottomView.CreateBottomView(mymodel, main_part, output, BOTTOM_VIEW_TOCREATE, defaultADFile, list_of_parts_for_bottom_view_mark_retain, ref bottom_view_list, ref bottom_view_FLANGE_CUT_LIST,
                                    MAINPART_PROFILE_VALUES, main, p1_bottm, p2_bottm, current_view, SHORTNENING_VALUE_FOR_BOTTOM_VIEW);
                                #endregion
                                #region top_view_creation

                                TOP_view_FLANGE_CUT_LIST = sKTopViewBuilder.CreateTopView(type_for_bolt, output, TOP_VIEW_TOCREATE, TOP_VIEW_needed, defaultADFile, MAINPART_PROFILE_VALUES, main, p3_bottm, p4_bottm, fixed_attributes, current_view);

                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                #endregion
                                ///////////////////////////////////////////////////////////end of front view ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }
                            ///////////////////////////////////////////////top view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////

                            #region TOP_VIEW
                            if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                            {
                                /////////////////////////////////////////////////bolt rd dimension in top view using matrix function /////////////////////////////////////////////////////////////
                                skDimensionHandler.ConfigureTopViewBoltDimensions(mymodel,
                                    currentAssemblyDrawing, main_part, ASSEMBLY, output,
                                    defaultADFile, list, SCALE, MAINPART_PROFILE_VALUES,
                                    main, ref TOP_VIEW_PARTMARK_TO_RETAIN, ref TOP_VIEW_BOLTMARK_TO_RETAIN, current_view);
                            }
                            #endregion

                            ///////////////////////////////////////////////////////////end of top view ///////////////////////////////////////////////////////////////////////////////////////////////////////////


                        }


                        TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height1 = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                        dim_font_height1.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        dim_font_height1.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(_inputModel.Client, _inputModel.FontSize, defaultADFile);


                        List<TSD.View> sectionviews_in_drawing = new List<TSD.View>();

                        #region SECTION_VIEW
                        if (list.Count > 0)
                        {
                            _logger.Debug($"Section View {list.Count}");
                            skSectionView.SectionView(mymodel, my_handler, defaultADFile, main_part, list,
                                SCALE, MAINPART_PROFILE_VALUES, fixed_attributes,
                                catalog_values, sectionviews_in_drawing);
                        }

                        #endregion


                        //list_section_flange


                        #region SECTION_VIEW2
                        if (list_section_flange.Count > 0)
                        {
                            _logger.Debug($"Section Flange {list_section_flange.Count}");
                            skSectionFlangeBuilder.SectionFlange(mymodel, my_handler, defaultADFile, main_part,
                                list_section_flange, SCALE, MAINPART_PROFILE_VALUES,
                                fixed_attributes, catalog_values,  sectionviews_in_drawing);
                        }
                        #endregion


                        sKTopViewBuilder.top_view_check_for_dim(currentAssemblyDrawing, TOP_VIEW_needed);
                        new CreateOrientationMark().orientationmark(currentModel, currentAssemblyDrawing, main_part);
                        skBoltHandler.BOLTMARK_EXACT(currentAssemblyDrawing);


                        //////////////////////////////////////////////////////////////////////////End of ALL views //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        markTest(my_handler, currentAssemblyDrawing, main_part, FRONT_VIEW_PARTMARK_TO_RETAIN, TOP_VIEW_PARTMARK_TO_RETAIN, FRONT_VIEW_BOLTMARK_TO_RETAIN, TOP_VIEW_BOLTMARK_TO_RETAIN);

                        weldHandler.WeldMerge(currentAssemblyDrawing, main_part, my_handler);
                        new SKDrawCenterLine().centre_line(currentAssemblyDrawing, main_part);
                        if (_inputModel.Client == "SME")
                        {
                            new SkConnectionSideMark().connecting_side_mark(currentModel, currentAssemblyDrawing, main_part);
                        }
                        DimensionMarker dimMarker = new DimensionMarker(_inputModel.Client);
                        boltMarkDetailing.bolt_mark_detail(currentModel, currentAssemblyDrawing, main_part, defaultADFile);
                        dimMarker.ATT_SETT(currentAssemblyDrawing, main_part);
                        dimMarker.Dim_Fix(currentAssemblyDrawing);
                        new FallTechDrawing().MarkFallTech(currentModel, currentAssemblyDrawing);

                        currentAssemblyDrawing.PlaceViews();

                        duplicateRemover.DeleteDuplicateSectionViews(currentAssemblyDrawing, my_handler);

                        new ConfigureViewPlacement().view_placement(currentAssemblyDrawing, defaultADFile, start_pt_for_section_view_aling, bottom_view_list, sectionviews_in_drawing, bottom_view_FLANGE_CUT_LIST, TOP_view_FLANGE_CUT_LIST);


                        #endregion
                        my_handler.SaveActiveDrawing();
                        currentAssemblyDrawing.PlaceViews();
                        streamlineDrawing.slot_symbol(currentAssemblyDrawing, main_part);
                        currentAssemblyDrawing.CommitChanges();
                        my_handler.CloseActiveDrawing(true);
                        mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        mymodel.CommitChanges();
                        DRG_REMARK = "Created";

                        span = DateTime.Now.Subtract(start_assy_tm);

                        ////////////////TODO VEERA ///////////COMMENTED
                        //DataGridViewRow MyRow = dgvlog.Rows[dgvlog.Rows.Add()];
                        //MyRow.Cells["drgmark"].Value = assembly_pos;
                        //MyRow.Cells["drgrmk"].Value = span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK;
                        //////////////////////////////////////////
                        proct++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal($"Exception thrown when creating drawing", ex);
                        try
                        {
                            TSD.AssemblyDrawing beam_assembly_drg1 = my_handler.GetActiveDrawing() as TSD.AssemblyDrawing;
                            beam_assembly_drg1.CommitChanges();
                            my_handler.SaveActiveDrawing();
                            my_handler.CloseActiveDrawing(true);
                            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                            mymodel.CommitChanges();
                            errct++;
                            DRG_REMARK = " Error:" + ex.Message;

                            span = DateTime.Now.Subtract(start_assy_tm);

                            //////////////TODO VEERA ///////////COMMENTED
                            //DataGridViewRow MyRow = dgvlog.Rows[dgvlog.Rows.Add()];
                            //MyRow.Cells["drgmark"].Value = assembly_pos;
                            //MyRow.Cells["drgrmk"].Value = span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK;
                            ////////////////////////////////////////
                        }
                        catch
                        {
                        }
                    }
                    finally
                    {
                        _messageStore.Add($"Beam ^{assembly_pos}^ {span.Minutes.ToString()}m {span.Seconds.ToString()}s {DRG_REMARK}");
                    }
                }
                MessageBox.Show("AUTOMATION COMPLETED");


            }
        }

     

   

        public string Execute()
        {
            string statusMessage = String.Empty;


            TSM.Model mymodel = new TSM.Model();
            Type type_for_bolt = typeof(TSD.Bolt);
            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingHandler my_handler = new TSD.DrawingHandler();
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            string skDefaultADFile = "";

            var drg_att_list = ConfigureDrawingData.PrepareDrawingAttributes(mymodel,
                _inputModel.SheetSelected, ref skDefaultADFile);

            string defaultADFile = "";
            PickBeams();
            return statusMessage;
        }

        private bool PickBeams()
        {
            string msgCaption = string.Empty;
            TSM.UI.Picker picker = new TSM.UI.Picker();
            TSM.ModelObjectEnumerator beams = null;
            try
            {
                beams = picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS, "PICK ALL BEAMS");
            }
            catch (ApplicationException ae)
            {
                MessageBox.Show($"User Interupted during picking up objects. Please try again",
                msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Environment.Exit(360);
            }
            if (beams == null || beams.GetSize() == 0)
            {
                MessageBox.Show("Beams are not yet picked");
                return false;
            }
            bool hasError = false;
            int cnt = 0;
            while (beams.MoveNext())
            {
                cnt++;
                TSM.ModelObject currentModelObject = beams.Current;
                string assemblyName = string.Empty;
                var numbered = AssemblyExtension.CheckNumbering(currentModelObject, ref assemblyName);
                if (!numbered)
                {
                    hasError = true;
                    _messageStore.Add($"Beam {numbered} is not numbered appropriately");
                    continue;
                }
                if (beamDrawings.Contains(assemblyName))
                {
                    //_messageStore.Add($"Beam {assemblyName} is already present");
                    continue;
                }
                beamDrawings.Add(assemblyName);
            }
            return hasError;
        }

        public async System.Threading.Tasks.Task PerformOperationAsync(IProgress<string> progress)
        {
            progress.Report("Starting operation...");
            await System.Threading.Tasks.Task.Delay(1000); // Simulate initial work
            progress.Report("Milestone 1 reached.");
            await System.Threading.Tasks.Task.Delay(2000); // Simulate more work
            progress.Report("Milestone 2 reached.");
            await System.Threading.Tasks.Task.Delay(1500); // Simulate final work
            progress.Report("Milestone 3 reached.");
            progress.Report("Operation completed.");
        }

        //add this in form
        //private async void button1_Click(object sender, EventArgs e)
        //{
        //    button1.Enabled = false; // Disable button to prevent multiple clicks
        //    textBox1.Clear(); // Clear previous progress
        //    try
        //    {
        //        var service = new LongRunningService();
        //        var progress = new Progress<string>(message => AppendProgress(message));
        //        await service.PerformOperationAsync(progress);
        //    }
        //    catch (Exception ex)
        //    {
        //        AppendProgress($"Error: {ex.Message}");
        //    }
        //    finally
        //    {
        //        button1.Enabled = true; // Re-enable button
        //    }
        //}

        //private void AppendProgress(string message)
        //{
        //    textBox1.AppendText(message + "\n");
        //    textBox1.SelectionStart = textBox1.Text.Length;
        //    textBox1.ScrollToCaret(); // Scroll to the latest message
        //}


        #region supportMethods
     

        private void markTest(DrawingHandler my_handler, AssemblyDrawing beam_assembly_drg,
            TSM.Part main_part, List<Guid> FRONT_VIEW_PARTMARK_TO_RETAIN,
            List<Guid> TOP_VIEW_PARTMARK_TO_RETAIN, List<Guid> FRONT_VIEW_BOLTMARK_TO_RETAIN,
            List<Guid> TOP_VIEW_BOLTMARK_TO_RETAIN)
        {
            ArrayList PART_MARK_TO_DELETE = new ArrayList();
            TSD.DrawingObjectEnumerator PART_MARK_TEST = beam_assembly_drg.GetSheet().GetAllViews();
            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_MARK = typeof(TSD.Mark);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if ((MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
                {


                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_MARK);
                    while (my_top_view_dimension_check.MoveNext())
                    {

                        TSD.Mark MYMARK = my_top_view_dimension_check.Current as TSD.Mark;

                        TSD.DrawingObjectEnumerator MYBJ = MYMARK.GetRelatedObjects();
                        while (MYBJ.MoveNext())
                        {
                            var mark_part = MYBJ.Current;
                            if (mark_part.GetType().Equals(typeof(TSD.Part)))
                            {
                                TSM.Part modelpart = new TSM.Model().SelectModelObject((mark_part as TSD.Part).ModelIdentifier) as TSM.Part;

                                Guid guid = modelpart.Identifier.GUID;
                                if (guid == main_part.Identifier.GUID)
                                {

                                    MYMARK.Attributes.PreferredPlacing = TSD.PreferredPlacingTypes.LeaderLinePlacingType();
                                    if (MYMARK.Attributes.Frame.Type.Equals(TSD.FrameTypes.None))
                                    {

                                        MYMARK.Attributes.Frame.Type = TSD.FrameTypes.None;
                                    }
                                    MYMARK.Modify();
                                }
                                else
                                {

                                    PART_MARK_TO_DELETE.Add(MYMARK);


                                }
                            }
                            else if (mark_part.GetType().Equals(typeof(TSD.Bolt)))
                            {
                                PART_MARK_TO_DELETE.Add(MYMARK);

                            }

                        }



                    }





                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_MARK);
                    while (my_top_view_dimension_check.MoveNext())
                    {

                        TSD.Mark MYMARK = my_top_view_dimension_check.Current as TSD.Mark;


                        PART_MARK_TO_DELETE.Add(MYMARK);

                    }

                }


            }
            PART_MARK_TEST.Reset();

            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_MARK = typeof(TSD.MarkSet);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if ((MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
                {
                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_MARK);
                    while (my_top_view_dimension_check.MoveNext())
                    {

                        TSD.MarkSet MYMARK = my_top_view_dimension_check.Current as TSD.MarkSet;


                        PART_MARK_TO_DELETE.Add(MYMARK);


                    }


                }
            }

            PART_MARK_TEST.Reset();

            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_PART = typeof(TSD.Part);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if ((MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView)) || (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView)) || (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.BottomView)))
                {
                    my_handler.GetDrawingObjectSelector().SelectObjects(PART_MARK_TO_DELETE, true);
                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();

                }


            }

            ArrayList PARTMARK_TO_BE_PROVIDED_FRONT = new ArrayList();
            ArrayList PARTMARK_TO_BE_PROVIDED_TOP = new ArrayList();
            ArrayList BOLTMARK_TO_BE_PROVIDED_FRONT = new ArrayList();
            ArrayList BOLTMARK_TO_BE_PROVIDED_TOP = new ArrayList();
            ArrayList BOLTMARK_TO_BE_PROVIDED_BOTTOM = new ArrayList();

            PART_MARK_TEST.Reset();

            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_PART = typeof(TSD.Part);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_PART);
                    while (my_top_view_dimension_check.MoveNext())
                    {
                        TSD.Part DRG_PART = my_top_view_dimension_check.Current as TSD.Part;
                        TSM.ModelObject modelpart = new TSM.Model().SelectModelObject(DRG_PART.ModelIdentifier);


                        bool CHECK = FRONT_VIEW_PARTMARK_TO_RETAIN.Any(X => X.Equals(modelpart.Identifier.GUID));

                        if (CHECK == true)
                        {
                            PARTMARK_TO_BE_PROVIDED_FRONT.Add(DRG_PART);

                        }

                    }
                }
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {

                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_PART);
                    while (my_top_view_dimension_check.MoveNext())
                    {
                        TSD.Part KK = my_top_view_dimension_check.Current as TSD.Part;
                        TSM.ModelObject model_bolt = new TSM.Model().SelectModelObject(KK.ModelIdentifier);


                        bool CHECK = TOP_VIEW_PARTMARK_TO_RETAIN.Any(X => X.Equals(model_bolt.Identifier.GUID));
                        if (CHECK == true)
                        {
                            PARTMARK_TO_BE_PROVIDED_TOP.Add(KK);

                        }

                    }
                }


            }

            PART_MARK_TEST.Reset();

            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_BOLT = typeof(TSD.Bolt);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_BOLT);
                    while (my_top_view_dimension_check.MoveNext())
                    {
                        TSD.Bolt DRG_BOLT = my_top_view_dimension_check.Current as TSD.Bolt;
                        TSM.ModelObject modelpart = new TSM.Model().SelectModelObject(DRG_BOLT.ModelIdentifier);


                        bool CHECK = FRONT_VIEW_BOLTMARK_TO_RETAIN.Any(X => X.Equals(modelpart.Identifier.GUID));

                        if (CHECK == true)
                        {
                            BOLTMARK_TO_BE_PROVIDED_FRONT.Add(DRG_BOLT);

                        }

                    }
                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {

                    TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_BOLT);
                    while (my_top_view_dimension_check.MoveNext())
                    {
                        TSD.Bolt DRG_BOLT = my_top_view_dimension_check.Current as TSD.Bolt;
                        TSM.ModelObject model_bolt = new TSM.Model().SelectModelObject(DRG_BOLT.ModelIdentifier);


                        bool CHECK = TOP_VIEW_BOLTMARK_TO_RETAIN.Any(X => X.Equals(model_bolt.Identifier.GUID));
                        if (CHECK == true)
                        {
                            BOLTMARK_TO_BE_PROVIDED_TOP.Add(DRG_BOLT);

                        }

                    }
                }



            }

            PART_MARK_TEST.Reset();

            while (PART_MARK_TEST.MoveNext())
            {
                Type type_for_PART = typeof(TSD.Part);
                TSD.View MYVIEW = PART_MARK_TEST.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    my_handler.GetDrawingObjectSelector().SelectObjects(PARTMARK_TO_BE_PROVIDED_FRONT, true);

                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_FRONT, true);

                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();

                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    my_handler.GetDrawingObjectSelector().SelectObjects(PARTMARK_TO_BE_PROVIDED_TOP, true);

                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_TOP, true);

                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();

                }



            }
        }
        #endregion

    }


}
