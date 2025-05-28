using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;
using System.Net.NetworkInformation;
using System.Net;
using Tekla.Structures;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.ObjectPropertiesLibrary;
using System.Diagnostics;
using Tekla.Structures.Drawing;
using ZstdSharp.Unsafe;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Tekla.Structures.DrawingInternal;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.TeleTrust;
using System.IO;
using Tekla.Structures.Model;
using static BEAM_DRAWING_AUTOMATIONTOOL.FormDrawingEditor;
using RenderData;
using Tekla.Structures.Filtering.Categories;
using Tekla.Structures.Filtering;
using SK.Tekla.Drawing.Automation.Services;
using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;
using SK.Tekla.Drawing.Automation.Drawing.Dimensions;
using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Drawing.Views;
using SK.Tekla.Drawing.Automation.Drawing;
using MySqlX.XDevAPI;
//using Security;



namespace BEAM_DRAWING_AUTOMATIONTOOL
{
    public partial class FormDrawingEditor : Form
    {

        public static string skApplicationName = "BEAM DRAWING AUTOMATIONTOOL";
        public static string skApplicationVersion = "2503.10";
        //Control names changed
        //public static string skApplicationVersion = "2503.10";
        //User Option values stored and loaded
        //Log added to cross check the performance for each drawings.
        //public static string skApplicationVersion = "2501.09";
        //Control name modified for understanding
        //Log added
        //
        //KT Provided by Vinoth - 2501.08

        public static bool debugflag = false;
        public static int debugcount = 1;
        //public static string logfile = skWinLib.sklocalpath + skTSLib.SKVersion + "//" + skApplicationName + "_" + skApplicationVersion + "_" + skWinLib.systemname + "_" + skWinLib.username + "_" + DateTime.Now.ToString("dd_mmm_yyyy") + ".vgml";
        public static string msgCaption = "";
        public static Tekla.Structures.Model.Model currentModel;

        private BoundingBoxHandler boundingBoxHandler;
        private FacePointHandler facePointHandler;
        private SKSortingHandler sortingHandler = new SKSortingHandler();

        private SKCatalogHandler catalogHandler = new SKCatalogHandler();

        private DuplicateRemover duplicateRemover;
        private SKDrawingHandler drawingHandler;
        private BoltMatrixHandler boltMatrixHandler;



        private SKFlangeOutDimension flangeOutDimension;

        private SKBottomView skBottomView;

        private SkStudDimensions skStudDimensions;

        private GussetDimension gussetDimension;

        private ElevationDimension elevationDimension;

        private AttachmentDimension attachmentDimension;

        private OutsideAssemblyDimension outsideAssemblyDimension;

        private BoltMarkDetailing boltMarkDetailing;

        private SKWeldHandler weldHandler;

        private StreamlineDrawing streamlineDrawing;

        private SKSlopHandler skSlopHandler;

        private SKBoltHandler skBoltHandler;

        private SKAngleHandler skAngleHandler;

        private CopeDimensions copeDimensions;

        private CustomInputModel _inputModel;

        private FontSizeSelector fontSize = FontSizeSelector.OneBy8;

        private SheetSelector sheetSelector = SheetSelector.UnDefined;

        private string client;

        private double _scale = 0;

        private double _minLength = 0;

        private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(FormDrawingEditor));

        public FormDrawingEditor()
        {
            try
            {
                //try to connect
                currentModel = skTSLib.ModelAccess.ConnectToModel();
                if (currentModel == null)
                {
                    MessageBox.Show($"Problem in establishing connection to the model - " +
                        $"Check if Tekla Structures is open and model is ready to use");
                    System.Environment.Exit(360);
                }
                //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Before-InitializeComponent", "");
                InitializeComponent();
                _logger.Debug("Components initialized....");
                //skWinLib.accesslog(skApplicationName, skApplicationVersion, "After-InitializeComponent", "");
                boundingBoxHandler = new BoundingBoxHandler();
                streamlineDrawing = new StreamlineDrawing();
                facePointHandler = new FacePointHandler(boundingBoxHandler, client);

                duplicateRemover = new DuplicateRemover(sortingHandler);
                drawingHandler = new SKDrawingHandler(boundingBoxHandler, catalogHandler);
                boltMatrixHandler = new BoltMatrixHandler(sortingHandler, catalogHandler);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error@InitializeComponent\n\nOriginal error: {ex.Message} \n {ex.Source} \n ex.StackTrace",
                        msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Environment.Exit(360);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //skWinLib.StoreControlValues(this.Controls, skApplicationName);
            //DateTime s_tm = skTSLib.setStartup(skApplicationName, skApplicationVersion, "Create_BeamDrawings", "", "Please Wait, Creating drawings...", "", lblsbar1, lblsbar2);
            string usermsg = Create_BeamDrawings();
            //skTSLib.setCompletion(skApplicationName, skApplicationVersion, "Complete", "", usermsg, "", lblsbar1, lblsbar2, s_tm);
        }

        private string Create_BeamDrawings()
        {
            ConsumeUserInputs();
            _logger.Debug("In Create Beam Drawing....");
            flangeOutDimension = new SKFlangeOutDimension(catalogHandler, client, fontSize);
            skStudDimensions = new SkStudDimensions(catalogHandler, client, fontSize);
            copeDimensions = new CopeDimensions(catalogHandler, client, fontSize);
            skBottomView = new SKBottomView(sortingHandler, catalogHandler, flangeOutDimension,
                client, fontSize);
            gussetDimension = new GussetDimension(catalogHandler, boltMatrixHandler,
                boundingBoxHandler, sortingHandler, client, fontSize);
            var isEleDim = chkwptxteledim.Checked;
            var isRdConnectMark = chkrdconnmark.Checked;
            elevationDimension = new ElevationDimension(catalogHandler, boltMatrixHandler,
            boundingBoxHandler, sortingHandler, client, fontSize, isEleDim);
            attachmentDimension = new AttachmentDimension(catalogHandler, boltMatrixHandler, boundingBoxHandler,
                sortingHandler, facePointHandler, client, fontSize, drawingHandler, isRdConnectMark, duplicateRemover);
            outsideAssemblyDimension = new OutsideAssemblyDimension(catalogHandler, boltMatrixHandler,
                boundingBoxHandler, sortingHandler, duplicateRemover, client, fontSize);
            boltMarkDetailing = new BoltMarkDetailing(catalogHandler, client, fontSize);
            skSlopHandler = new SKSlopHandler(sortingHandler, client, fontSize);
            skBoltHandler = new SKBoltHandler(catalogHandler, boltMatrixHandler, client, fontSize);
            weldHandler = new SKWeldHandler(client);
            skAngleHandler = new SKAngleHandler(boltMatrixHandler);
            //clear rows
            dgvlog.Rows.Clear();
            string remark = string.Empty;
            int proct = 0;
            int selct = 0;
            int errct = 0;
            List<string> drglog = new List<string>();
            {



                bool tekla_open = TeklaStructures.Connect();

                if (tekla_open == true)
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

                    var drg_att_list = ConfigureDrawingData.PrepareDrawingAttributes(mymodel, sheetSelector, ref skDefaultADFile);

                    string defaultADFile = "";

                    selct = enum_for_beam_pick.GetSize();
                    pbar.Maximum = selct + 4;
                    pbar.Value = 0;
                    pbar.Visible = true;
                    _logger.Debug($"Total Beams picked up ....{selct}");

                    //Check for already drawing created
                    List<string> SKDrawings = new List<string>();


                    while (enum_for_beam_pick.MoveNext())
                    {
                        pbar.Value = pbar.Value + 1;
                        pbar.Refresh();

                        string assembly_pos = string.Empty;
                        string DRG_NUMBER = string.Empty;
                        string DRG_REMARK = string.Empty;
                        DateTime start_assy_tm = DateTime.Now;
                        TimeSpan span = new TimeSpan();
                        try
                        {
                            TSM.ModelObject currentBeam = enum_for_beam_pick.Current;
                            TSM.Assembly currentBeamAsAssembly = currentBeam as TSM.Assembly;
                            if(currentBeamAsAssembly == null)
                            {
                                _logger.Fatal($"Assembly is not selected: {currentBeam}");
                                errct++;
                                continue;
                            }


                            currentBeamAsAssembly.GetReportProperty("ASSEMBLY_POS", ref assembly_pos);

                            bool missingNumbering = assembly_pos.Contains('?');


                            if (missingNumbering)
                            {
                                _logger.Fatal($"Numbering issue for drawing {assembly_pos}");
                                DRG_REMARK = "Numbering???";
                                errct++;
                                continue;
                            }

                            if (SKDrawings.Contains(assembly_pos.ToUpper()) == false)
                            {

                                s_tm = DateTime.Now;
                                TSM.Beam currentBeamMainpart = currentBeamAsAssembly.GetMainPart() as TSM.Beam;
                                //double LENGTH = 0;
                                //mysingleobject.GetReportProperty("LENGTH", ref LENGTH);
                                TSD.AssemblyDrawing currentAssemblyDrawing = null;
                                TSM.Part main_part = null;
                                TSM.Assembly ASSEMBLY;
                                double DIM_DIST;
                                double ACTUAL_DIST;
                                double output;
                                string BOTTOM_VIEW;
                                string TOP_VIEW;
                                string BOTTOM_VIEW_TOCREATE;
                                string TOP_VIEW_TOCREATE;
                                string TOP_VIEW_needed;
                                TSD.StraightDimension overall_dim;
                                ////////////////////////////////////////////////////deleting dimension and getting required information using function//////////////////////////////////////////////////////

                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentBeamMainpart.GetCoordinateSystem()));

                                TSG.Matrix VIEW_MATRIX = TSG.MatrixFactory.ToCoordinateSystem(currentBeamMainpart.GetCoordinateSystem());
                                List<section_loc_with_parts> list = new List<section_loc_with_parts>();
                                List<section_loc_with_parts> list_section_flange = new List<section_loc_with_parts>();
                                List<section_loc_with_parts> list_section_flange_duplicate = new List<section_loc_with_parts>();
                                List<TSM.Part> list_of_parts_for_bottom_view_mark_retain = new List<TSM.Part>();
                                List<Guid> list_of_part_at_end_in_top_view = new List<Guid>();
                                List<TSD.RadiusDimension> list_of_radius = new List<TSD.RadiusDimension>();


                                TSG.Point start_pt_for_section_view_aling = new TSG.Point();

                                Console.WriteLine("DRG_NUMBER.....>" + assembly_pos);
                                //Function will be given by Viswa for layout - Req_attribute

                                //List<req_attribute> required_attribute  =
                                //    drawingHandler.Drawing_create_and_delete_all_dimensions_except_assembly_dim_sheet_check(mymodel, currentBeam, 
                                //    drg_att_list, out currentAssemblyDrawing);

                                SKLayout skLayout = new SKLayout()
                                {
                                    attribute = "VBR_BEAM_A1",
                                    adFileName = "SK_BEAM_A1",
                                    scale = txtscale.Enabled ? Convert.ToDouble(txtscale.Text) : 20,
                                    minLenth = txtminlen.Enabled ? Convert.ToDouble(txtminlen.Text) : 1789.5998
                                };
                                _logger.Debug($"Layout - {skLayout.ToString()}");

                                double SCALE = skLayout.scale;
                                double MINI_LEN = skLayout.minLenth;
                                defaultADFile = skLayout.adFileName;
                                //VBR_A3

                                bool result_for_view_out;
                                List<TSD.View> bottom_view_list = new List<TSD.View>();
                                List<TSD.View> bottom_view_FLANGE_CUT_LIST = new List<TSD.View>();
                                List<TSD.View> TOP_view_FLANGE_CUT_LIST = new List<TSD.View>();


                                #region Module-2
                                //SCALE = 8;
                                //MINI_LEN = 130;
                                TSD.StraightDimension OVERALL_DIMENSION = null;
                                Stopwatch SECTION_CREATION = new Stopwatch();
                                SECTION_CREATION.Start();
                                drawingHandler.Drawing_create_and_delete_all_dimensions_except_assembly_dim(mymodel,
                                    currentBeam, defaultADFile,
                                    out currentAssemblyDrawing, out main_part, out output, out ASSEMBLY,
                                    out overall_dim, out DIM_DIST,
                                    out ACTUAL_DIST, out list, SCALE, MINI_LEN, out list_section_flange,
                                    out list_section_flange_duplicate, out list_of_parts_for_bottom_view_mark_retain,
                                    out list_of_part_at_end_in_top_view, out OVERALL_DIMENSION, out list_of_radius);
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
                                skBottomView.TOPVIEW_CHECK(main, out TOP_VIEW, mainPartHeight, out p3_bottm, out p4_bottm, output, out TOP_VIEW_TOCREATE);
                                skBottomView.TOPVIEW_needed(main, mainPartHeight, output, out TOP_VIEW_needed);

                                ////////////////////////////////////////////////////getting views from beam assembly drg ///////////////////////////////////////////////////////////////////////////////////    
                                TSD.DrawingObjectEnumerator enum_for_views = currentAssemblyDrawing.GetSheet().GetAllViews();
                                TSG.Point mml = currentAssemblyDrawing.GetSheet().Origin;

                                TSD.StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                //fixed_attributes.LoadAttributes("SK_BEAM")
                                fixed_attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                fixed_attributes.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                                if ((defaultADFile == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                {
                                    if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                    {
                                        fixed_attributes.Text.Font.Height = 3.571875;
                                    }
                                    else
                                    {
                                        fixed_attributes.Text.Font.Height = 3.175;
                                    }
                                }
                                else
                                {
                                    fixed_attributes.Text.Font.Height = 2.38125;
                                }

                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(main_part.GetCoordinateSystem()));
                                ArrayList list_of_secondaries = ASSEMBLY.GetSecondaries();
                                List<Guid> nearside_parts = new List<Guid>();
                                List<Guid> farside_parts = new List<Guid>();
                                List<double> catalog_values = catalogHandler.Getcatalog_values(main_part);
                                if (profile_type == "U")
                                {


                                    TSG.Vector zvector1 = main_part.GetCoordinateSystem().AxisX.Cross(main_part.GetCoordinateSystem().AxisY);
                                    mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

                                    zvector1.Normalize();
                                    double WT1 = 0;


                                    double WT3 = (catalog_values[1]);
                                    if (zvector1.Z > 0)
                                    {
                                        WT1 = (-WT3 / 2);
                                    }
                                    else
                                    {
                                        WT1 = (WT3 / 2);
                                    }








                                    foreach (TSM.Part part in list_of_secondaries)
                                    {
                                        TSD.PointList bbz = boundingBoxHandler.bounding_box_sort_z(part, main_part as TSM.Beam);

                                        if (bbz[1].Z < WT1)
                                        {
                                            farside_parts.Add(part.Identifier.GUID);
                                        }
                                        else if (bbz[0].Z > WT1)
                                        {
                                            nearside_parts.Add(part.Identifier.GUID);
                                        }


                                    }
                                }

                                else
                                {
                                    foreach (TSM.Part part in list_of_secondaries)
                                    {
                                        TSD.PointList bbz = boundingBoxHandler.bounding_box_sort_z(part, main_part as TSM.Beam);


                                        if (bbz[1].Z < 0)
                                        {
                                            farside_parts.Add(part.Identifier.GUID);
                                        }
                                        else if (bbz[0].Z > 0)
                                        {
                                            nearside_parts.Add(part.Identifier.GUID);
                                        }


                                    }
                                }
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                currentAssemblyDrawing.PlaceViews();

                                #region codetocopy in drafter tool
                                List<Guid> FRONT_VIEW_PARTMARK_TO_RETAIN = new List<Guid>();
                                List<Guid> TOP_VIEW_PARTMARK_TO_RETAIN = new List<Guid>();
                                List<Guid> FRONT_VIEW_BOLTMARK_TO_RETAIN = new List<Guid>();
                                List<Guid> TOP_VIEW_BOLTMARK_TO_RETAIN = new List<Guid>();

                                ////////////////////////////////////////////////////view enum starts////////////////////////////////////////////////////////////////////////////////////////////////////////
                                while (enum_for_views.MoveNext())
                                {


                                    TSD.View current_view = enum_for_views.Current as TSD.View;
                                    double SHORTNENING_VALUE_FOR_BOTTOM_VIEW = 0;
                                    ///////////////////////////////////////////////////front view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////

                                    #region FRONT_VIEW
                                    //////////////////////////////////////////////////bolt rd dimension in front view using matrix function ///////////////////////////////////////////////////////////////////////
                                    ConfigureFrontViewBoldDimension(mymodel, currentAssemblyDrawing, main_part, output,
                                        defaultADFile, list, list_of_radius, SCALE, MAINPART_PROFILE_VALUES, main, ref FRONT_VIEW_PARTMARK_TO_RETAIN, ref FRONT_VIEW_BOLTMARK_TO_RETAIN, current_view, ref SHORTNENING_VALUE_FOR_BOTTOM_VIEW);

                                    #endregion

                                    ///////////////////////////////////////////////////////////end of front view ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                                    ///////////////////////////////////////////////top view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////

                                    #region TOP_VIEW
                                    /////////////////////////////////////////////////bolt rd dimension in top view using matrix function /////////////////////////////////////////////////////////////
                                    ConfigureTopViewBoltDimensions(mymodel, currentAssemblyDrawing, main_part, ASSEMBLY, output, defaultADFile, list, SCALE, MAINPART_PROFILE_VALUES, main, ref TOP_VIEW_PARTMARK_TO_RETAIN, ref TOP_VIEW_BOLTMARK_TO_RETAIN, current_view);
                                    #endregion

                                    ///////////////////////////////////////////////////////////end of top view ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                                    //////////////////////////////////////////////////////////bottom view filtered//////////////////////////////////////////////////////////////////////////////////////////////////////

                                    #region bottom_view
                                    CreateBottomView(mymodel, main_part, output, BOTTOM_VIEW_TOCREATE, defaultADFile, list_of_parts_for_bottom_view_mark_retain, ref bottom_view_list, ref bottom_view_FLANGE_CUT_LIST, MAINPART_PROFILE_VALUES, main, p1_bottm, p2_bottm, current_view, SHORTNENING_VALUE_FOR_BOTTOM_VIEW);


                                    #endregion

                                    //////////////////////////////////////////////////////////////////////////End of bottom view//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                    #region top_view_creation

                                    TOP_view_FLANGE_CUT_LIST = CreateTopView(type_for_bolt, output, TOP_VIEW_TOCREATE, TOP_VIEW_needed, defaultADFile, TOP_view_FLANGE_CUT_LIST, MAINPART_PROFILE_VALUES, main, p3_bottm, p4_bottm, fixed_attributes, current_view);


                                    #endregion



                                }


                                TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height1 = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                dim_font_height1.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                dim_font_height1.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, defaultADFile);


                                List<TSD.View> sectionviews_in_drawing = new List<TSD.View>();

                                #region SECTION_VIEW
                                /////////////////////////////////////////////////bolt gage dimension in section view using matrix function /////////////////////////////////////////////////////////////
                                for (int z = 0; z < list.Count; z++)
                                {
                                    ///////////////////////////////////////////////////filtering bolts from all parts in section view/////////////////////////////////////////////////////////////////////////////
                                    TSD.View current_view = list[z].myview;

                                    if (current_view != null)
                                    {
                                        Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                                        TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(type_for_dim);
                                        while (dim_drg.MoveNext())
                                        {
                                            var obj = dim_drg.Current;
                                            obj.Delete();

                                        }

                                        if (chksecscale.Checked == true)
                                        {
                                            current_view.Attributes.Scale = SCALE + Convert.ToDouble(cmbsecscale.Text);
                                            current_view.Modify();
                                        }
                                        else
                                        {
                                            current_view.Attributes.Scale = SCALE;
                                            current_view.Modify();
                                        }
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

                                        foreach (var list_of_parts in list[z].req_partlist)
                                        {
                                            TSD.PointList rd_point_list = new TSD.PointList();
                                            TSM.Part mypart = list_of_parts as TSM.Part;

                                            SECTION_VIEW_PARTMARK_TO_RETAIN.Add(mypart.Identifier.GUID);

                                            TSD.PointList angle_hole_locking_check = boundingBoxHandler.bounding_box_sort_y(list_of_parts as TSM.ModelObject, current_view);


                                            TSM.ModelObjectEnumerator enum_for_bolt = list_of_parts.GetBolts();
                                            //ts.DrawingObjectEnumerator enum_for_bolt = current_view.GetAllObjects(type_for_bolt);
                                            string prof_typ = SkTeklaDrawingUtility.get_report_properties(mypart, "PROFILE_TYPE");


                                            if (prof_typ == "L")
                                            {

                                                skAngleHandler.angle_place_check_for_hole_locking(angle_hole_locking_check, out rd_point_list, enum_for_bolt, current_view, ref SECTION_VIEW_PARTMARK_TO_RETAIN, ref SECTION_VIEW_BOLTMARK_TO_RETAIN_POS, ref SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG);
                                                TSD.PointList FINAL_RD_LIST = new TSD.PointList();
                                                FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);

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
                                                inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, defaultADFile);

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
                                                    //bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, -1, 0), distance1 + 75, inside);
                                                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - miny);
                                                    //double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - maxy);
                                                    /////ROSEPLASTIC//////////////
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
                                                FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);

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
                                                inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, defaultADFile);


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
                                                    //bolt_gage_dim.CreateDimensionSet(current_view, FINAL_RD_LIST_below_fl, new TSG.Vector(0, -1, 0), distance1 + 75, inside);
                                                    //double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - miny);
                                                    double distance1 = Math.Abs(Math.Abs(FINAL_RD_LIST_below_fl[0].Y) - maxy);
                                                    /////ROSEPLASTIC//////////////
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
                                        foreach (var list_of_parts in list[z].req_partlist)
                                        {
                                            TSM.Part mypart = list_of_parts as TSM.Part;
                                            double ht = Convert.ToInt64(MAINPART_PROFILE_VALUES[0]);
                                            double height = Convert.ToInt64(ht / 2);
                                            string BOLT_DIM = skBoltHandler.BOLT_IN_VIEW(mypart, current_view);
                                            if (BOLT_DIM == "NEED")
                                            {
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

                                                    else
                                                    {

                                                        foreach (TSG.Point pt in mybolt.BoltPositions)
                                                        {
                                                            TSG.Point p1 = toviewmatrix.Transform(pt);

                                                            list3x3.Add(p1);


                                                            if (p1.X > 0)
                                                            {
                                                                SECTION_VIEW_BOLTMARK_TO_RETAIN_POS.Add(drgbolt.Identifier.GUID);
                                                            }
                                                            else if (p1.X < 0)
                                                            {
                                                                SECTION_VIEW_BOLTMARK_TO_RETAIN_NEG.Add(drgbolt.Identifier.GUID);
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
                                                    FINAL_list3x3_positive.Add(new TSG.Point((MAINPART_PROFILE_VALUES[1]) / 2, MAINPART_PROFILE_VALUES[0] / 2, 0));

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

                                        }


                                        /////////////////END OF 3x3 dimension for SECTION VIEW////////////////////////


                                        //////////////////////////////////////////pour stopper code//////////////////////////////////////////////////

                                        foreach (var list_of_parts in list[z].req_partlist)
                                        {


                                            TSM.Part mypart = list_of_parts as TSM.Part;
                                            TSD.PointList mypt = boundingBoxHandler.bounding_box_sort_z(list_of_parts, current_view);
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

                                                                    List<AngleFaceArea> myreq = facePointHandler.getface_for_angle(mypart);
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
                                                                    pt_lit_for_angle.Add(facePointHandler.Get_face_point_for_angle_section_view(mypart, current_view));
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
                                        foreach (var list_of_parts in list[z].req_partlist)
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



                                                            if (list[z].req_partlist.Any(p => p.Identifier.ID == modelpart.Identifier.ID))
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





                                                    //TSD.DrawingObjectEnumerator enumcheck1 = weldmark.GetObjects();
                                                    Identifier id = weldmark.ModelIdentifier;
                                                    TSM.BaseWeld weld = (new TSM.Model().SelectModelObject(id) as TSM.BaseWeld);
                                                    TSM.Part mainpart = (weld.MainObject as TSM.Part);
                                                    TSM.Part secondary_part = (weld.SecondaryObject as TSM.Part);

                                                    if ((list[z].req_partlist.Any(p => p.Identifier.ID == mainpart.Identifier.ID)) || ((list[z].req_partlist.Any(p => p.Identifier.ID == secondary_part.Identifier.ID))))
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
                                            TSD.PointList bounding_box_x = boundingBoxHandler.bounding_box_sort_x(mm, current_view);
                                            TSD.PointList bounding_box_y = boundingBoxHandler.bounding_box_sort_y(mm, current_view);

                                            TSD.PointList mypt = boundingBoxHandler.bounding_box_sort_z(list_of_parts, current_view);
                                            TSD.PointList mypt_final = new TSD.PointList();
                                            TSD.PointList mypt_final_FOR_LEG = new TSD.PointList();
                                            double height11 = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                                            double h1 = Convert.ToInt64(height11);

                                            //mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
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
                                                        if (POINT_FOR_BOLT_MATRIX == null)
                                                        {
   
                                                            //TSD.PointList p1 = bounding_box_sort_x(mm, current_view);
                                                            TSD.PointList p1 = boundingBoxHandler.bounding_box_sort_y(mm, current_view);


                                                            //TSM.BoltGroup bolt = MODELbolt.Current as TSM.BoltGroup;
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
                                        foreach (var list_of_parts in list[z].req_partlist)
                                        {
                                            List<Guid> MYLISTOF_BOLTS_IN_EACH_PART = new List<Guid>();
                                            TSM.Part MYPART = list_of_parts;
                                            TSM.ModelObjectEnumerator MYENUM = MYPART.GetBolts();
                                            while (MYENUM.MoveNext())
                                            {
                                                TSM.BoltGroup MYBOLT = MYENUM.Current as TSM.BoltGroup;
                                                MYLISTOF_BOLTS_IN_EACH_PART.Add(MYBOLT.Identifier.GUID);



                                            }
                                            List<Guid> POS_LIST = MYLISTOF_BOLTS_IN_EACH_PART.Intersect(FINAL_BOLTMARK_POS).ToList();
                                            List<Guid> NEG_LIST = MYLISTOF_BOLTS_IN_EACH_PART.Intersect(FINAL_BOLTMARK_NEG).ToList();

                                            foreach (var GUID in POS_LIST)
                                            {
                                                Identifier ID = new Identifier(GUID);
                                                TSM.ModelObject MYOBJ = new TSM.Model().SelectModelObject(ID);
                                                BOLTMARK_TO_BE_PROVIDED_SECTION_POS.Add(MYOBJ);

                                            }
                                            foreach (var GUID in NEG_LIST)
                                            {
                                                Identifier ID = new Identifier(GUID);
                                                TSM.ModelObject MYOBJ = new TSM.Model().SelectModelObject(ID);
                                                BOLTMARK_TO_BE_PROVIDED_SECTION_NEG.Add(MYOBJ);

                                            }
                                            my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_SECTION_POS, true);
                                            Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                                            my_handler.GetDrawingObjectSelector().UnselectAllObjects();


                                            my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_SECTION_NEG, true);
                                            Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                                            my_handler.GetDrawingObjectSelector().UnselectAllObjects();


                                        }

                                        foreach (TSD.Part PART_GUID in PARTMARK_TO_BE_PROVIDED_SECTION)
                                        {

                                            my_handler.GetDrawingObjectSelector().SelectObject(PART_GUID);
                                            Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                                            my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                                        }









                                    }













                                }


















                                #endregion


                                //list_section_flange


                                #region SECTION_VIEW2
                                /////////////////////////////////////////////////bolt gage dimension in section view using matrix function /////////////////////////////////////////////////////////////
                                for (int z = 0; z < list_section_flange.Count; z++)
                                {
                                    ///////////////////////////////////////////////////filtering bolts from all parts in section view/////////////////////////////////////////////////////////////////////////////

                                    TSD.View current_view = list_section_flange[z].myview;

                                    if (current_view != null)
                                    {
                                        Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet), typeof(TSD.AngleDimension) };
                                        TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(type_for_dim);
                                        while (dim_drg.MoveNext())
                                        {
                                            var obj = dim_drg.Current;
                                            obj.Delete();

                                        }

                                        if (chksecscale.Checked == true)
                                        {
                                            current_view.Attributes.Scale = SCALE + Convert.ToDouble(cmbsecscale.Text);
                                            current_view.Modify();
                                        }
                                        else
                                        {
                                            current_view.Attributes.Scale = SCALE;
                                            current_view.Modify();
                                        }
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

                                        foreach (var list_of_parts in list_section_flange[z].partlist)
                                        {
                                            TSD.PointList rd_point_list = new TSD.PointList();
                                            TSM.Part mypart = list_of_parts as TSM.Part;
                                            SECTION_VIEW_PARTMARK_TO_RETAIN.Add(mypart.Identifier.GUID);

                                            TSD.PointList angle_hole_locking_check = boundingBoxHandler.bounding_box_sort_y(list_of_parts as TSM.ModelObject, current_view);


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
                                                FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);

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
                                                inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, defaultADFile);


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
                                                FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list);

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
                                                inside.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, defaultADFile);

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
                                        foreach (var list_of_parts in list_section_flange[z].partlist)
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

                                        foreach (var list_of_parts in list_section_flange[z].partlist)
                                        {


                                            TSM.Part mypart = list_of_parts as TSM.Part;
                                            TSD.PointList mypt = boundingBoxHandler.bounding_box_sort_z(list_of_parts, current_view);
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

                                                                    List<AngleFaceArea> myreq = facePointHandler.getface_for_angle(mypart);
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
                                                                    pt_lit_for_angle.Add(facePointHandler.Get_face_point_for_angle_section_view(mypart, current_view));
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
                                        foreach (var list_of_parts in list_section_flange[z].partlist)
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



                                                            if (list_section_flange[z].partlist.Any(p => p.Identifier.ID == modelpart.Identifier.ID))
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

                                                    if ((list_section_flange[z].partlist.Any(p => p.Identifier.ID == mainpart.Identifier.ID)) || ((list_section_flange[z].partlist.Any(p => p.Identifier.ID == secondary_part.Identifier.ID))))
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
                                            TSD.PointList bounding_box_x = boundingBoxHandler.bounding_box_sort_x(mm, current_view);
                                            TSD.PointList bounding_box_y = boundingBoxHandler.bounding_box_sort_y(mm, current_view);

                                            TSD.PointList mypt = boundingBoxHandler.bounding_box_sort_z(list_of_parts, current_view);
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

                                                            TSD.PointList p1 = boundingBoxHandler.bounding_box_sort_y(mm, current_view);



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
                                            Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                                            my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                                        }


                                    }


                                }
                                #endregion





                                skBottomView.top_view_check_for_dim(currentAssemblyDrawing, TOP_VIEW_needed);
                                new CreateOrientationMark().orientationmark(currentModel, currentAssemblyDrawing, main_part);
                                skBoltHandler.BOLTMARK_EXACT(currentAssemblyDrawing);


                                //////////////////////////////////////////////////////////////////////////End of ALL views //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                markTest(my_handler, currentAssemblyDrawing, main_part, FRONT_VIEW_PARTMARK_TO_RETAIN, TOP_VIEW_PARTMARK_TO_RETAIN, FRONT_VIEW_BOLTMARK_TO_RETAIN, TOP_VIEW_BOLTMARK_TO_RETAIN);

                                weldHandler.WeldMerge(currentAssemblyDrawing, main_part, my_handler);
                                new SKDrawCenterLine().centre_line(currentAssemblyDrawing, main_part);
                                if (client == "SME")
                                {
                                    new SkConnectionSideMark().connecting_side_mark(currentModel, currentAssemblyDrawing, main_part);
                                }
                                DimensionMarker dimMarker = new DimensionMarker(client);
                                boltMarkDetailing.bolt_mark_detail(currentModel, currentAssemblyDrawing, main_part, defaultADFile);
                                dimMarker.ATT_SETT(currentAssemblyDrawing, main_part);
                                dimMarker.Dim_Fix(currentAssemblyDrawing);
                                new FallTechDrawing().Fall_Tech(currentModel, currentAssemblyDrawing);

                                currentAssemblyDrawing.PlaceViews();

                                duplicateRemover.delete_sec_view_same_dims(currentAssemblyDrawing, my_handler);

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
                                SKDrawings.Contains(assembly_pos.ToUpper());
                                span = DateTime.Now.Subtract(start_assy_tm);
                                //dgvlog.Rows.Add(assembly_pos, span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK);            
                                DataGridViewRow MyRow = dgvlog.Rows[dgvlog.Rows.Add()];
                                MyRow.Cells["drgmark"].Value = assembly_pos;
                                MyRow.Cells["drgrmk"].Value = span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK;
                                proct++;
                            }



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
                                DRG_REMARK = " Error." + ex.Message;
                                SKDrawings.Contains(assembly_pos.ToUpper());
                                span = DateTime.Now.Subtract(start_assy_tm);
                                //dgvlog.Rows.Add(assembly_pos, span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK);

                                DataGridViewRow MyRow = dgvlog.Rows[dgvlog.Rows.Add()];
                                MyRow.Cells["drgmark"].Value = assembly_pos;
                                MyRow.Cells["drgrmk"].Value = span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s" + DRG_REMARK;
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (SKDrawings.Count >= 1)
                    {
                        string assymark = "\n";
                        foreach (DataGridViewRow dataGridViewRow in dgvlog.Rows)
                        {
                            if (dataGridViewRow.Cells["drgmark"].Value != null)
                                assymark = assymark + "\n" + dataGridViewRow.Cells["drgmark"].Value.ToString();

                        }
                        skWinLib.accesslog(skApplicationName, skApplicationVersion, "Create_BeamDrawings", assymark, "", "");
                        mymodel.CommitChanges();
                    }
                    MessageBox.Show("AUTOMATION COMPLETED");


                }
                else
                {

                    MessageBox.Show("Please open Tekla Structures and a model", "Open Tekla Structures", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FormDrawingEditor.ActiveForm.Close();
                }
            }


            pbar.Value = pbar.Value + 1;
            pbar.Refresh();
            //foreach(string drgdata in drglog)
            //{
            //    string[] split_text = drgdata.Split('|');
            //    dgvlog.Rows.Add(split_text);
            //}

            //skWinLib.updaterowheader(dgvlog);

            pbar.Value = pbar.Value + 1;
            pbar.Refresh();
            dgvlog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

            pbar.Value = pbar.Value + 1;
            pbar.Refresh();
            //this.Text = skWinLib.UpdateHeaderInformation(skApplicationName, skApplicationVersion, skTSLib.ModelName, 0, selct, 0, errct, proct);

            pbar.Value = pbar.Maximum;
            pbar.Refresh();
            pbar.Visible = false;

            return " Created Drawing: " + proct + " Error: " + errct;
        }

        private void ConsumeUserInputs()
        {
            if (chk3by32.Checked)
            {
                fontSize = FontSizeSelector.ThreeBy32;
            }
            else if (chk9by64.Checked)
            {
                fontSize = FontSizeSelector.NineBy64;
            }
            client = cmbclient.Text;
            if (chka0.Checked)
            {
                sheetSelector = SheetSelector.A0;
            }
            else if (chka1.Checked)
            {
                sheetSelector = SheetSelector.A1;
            }
            else if (chka2.Checked)
            {
                sheetSelector = SheetSelector.A2;
            }
            else if (chka3.Checked)
            {
                sheetSelector = SheetSelector.A3;
            }
            if (txtscale.Enabled && txtscale.Text.Length > 0)
            {
                _scale = Convert.ToDouble(txtscale.Text);
            }
            if (txtminlen.Enabled && txtminlen.Text.Length > 0)
            {
                _minLength = Convert.ToDouble(txtminlen.Text);
            }
            _inputModel = new CustomInputModel()
            {
                Client = cmbclient.Text,
                FontSize = fontSize,
                SheetSelected = sheetSelector,
                Scale = _scale,
                MinLength = _minLength
            };
        }

        #region supportMethods
        private void ConfigureFrontViewBoldDimension(Model mymodel, AssemblyDrawing beam_assembly_drg, TSM.Part main_part,
            double output, string drg_att, List<section_loc_with_parts> list, List<RadiusDimension> list_of_radius,
            double SCALE, List<double> MAINPART_PROFILE_VALUES,
            Beam main, ref List<Guid> FRONT_VIEW_PARTMARK_TO_RETAIN,
            ref List<Guid> FRONT_VIEW_BOLTMARK_TO_RETAIN, TSD.View current_view, ref double SHORTNENING_VALUE_FOR_BOTTOM_VIEW)
        {
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
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
                if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                {
                    if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
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

                if (chkcutlen.Checked == true)
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

                    skSlopHandler.slope_bolt_logic(singlebolts1, to_rotate_matrix, current_view, output, MAINPART_PROFILE_VALUES, myvector_for_slope_bolt, yvector_for_slope_bolt, drg_att);
                }
                catch
                {

                }


                try
                {

                    skBoltHandler.bolt_logic(singlebolts, current_view, output, MAINPART_PROFILE_VALUES, drg_att);

                }
                catch
                {

                }
                #endregion
                ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////


                /////////////////END OF 3x3 dimension for FRONT VIEW////////////////////////





                ///////////////// cope dimension for FRONT VIEW////////////////////////

                copeDimensions.CreateCopeDimensions(current_view, main, drg_att);

                /////////////////END OF cope dimension for FRONT VIEW////////////////////////



                gussetDimension.GussetDimensionsWithBolts(main, current_view, ref FRONT_VIEW_PARTMARK_TO_RETAIN, ref FRONT_VIEW_BOLTMARK_TO_RETAIN, drg_att);
                outsideAssemblyDimension.DimensionForPartsOutsideAssembly(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
                attachmentDimension.Dimensions_for_attachments_for_outside_flange(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
                attachmentDimension.Dimensions_for_attachments_for_inside_flange_front(main, current_view, output, ref FRONT_VIEW_PARTMARK_TO_RETAIN, drg_att);
                double DEPTH = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                if (chkeledim.Checked == true)
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

        private void ConfigureTopViewBoltDimensions(Model mymodel, AssemblyDrawing beam_assembly_drg,
            TSM.Part main_part, Assembly ASSEMBLY, double output, string drg_att, List<section_loc_with_parts> list,
            double SCALE, List<double> MAINPART_PROFILE_VALUES, Beam main,
            ref List<Guid> TOP_VIEW_PARTMARK_TO_RETAIN, ref List<Guid> TOP_VIEW_BOLTMARK_TO_RETAIN, TSD.View current_view)
        {
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
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
                OUTSIDE.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

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
                TSD.PointList FINAL_RD_LIST = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list_final);
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
                    rd.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

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
                    TSD.PointList FINAL_RD_LIST11 = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list11);
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
                        rd.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                        bolt_rd_dim.CreateDimensionSet(current_view, FINAL_RD_LIST11, new TSG.Vector(0, 1, 0), maxy + 125, rd);

                    }
                    catch
                    {
                    }



                }


                #endregion

                ////////////////////cope dimension for topview///////////////
                flangeOutDimension.Create_FLANGE_CUT_dimensions_top(current_view, main, drg_att);
                copeDimensions.ProvideFittingCutDimensions(currentModel, current_view, main, drg_att);

                //////////////////// end of cope dimension for topview///////////////
                attachmentDimension.Dimensions_for_attachments_for_inside_flange_top(main, current_view, output, ref TOP_VIEW_PARTMARK_TO_RETAIN, drg_att);
                gussetDimension.GussetDimensionsWithBolts(main, current_view, ref TOP_VIEW_PARTMARK_TO_RETAIN, ref TOP_VIEW_BOLTMARK_TO_RETAIN, drg_att);

                skBoltHandler.partmark_for_bolt_dim_attachments(current_view, ref TOP_VIEW_PARTMARK_TO_RETAIN);

                topViewMatch.Stop();
                Console.WriteLine("top_watch.....>" + topViewMatch.ElapsedMilliseconds.ToString());

            }
        }

        private void CreateBottomView(Model mymodel, TSM.Part main_part, double output,
            string BOTTOM_VIEW_TOCREATE, string drg_att, List<TSM.Part> list_of_parts_for_bottom_view_mark_retain,
            ref List<TSD.View> bottom_view_list, ref List<TSD.View> bottom_view_FLANGE_CUT_LIST,
            List<double> MAINPART_PROFILE_VALUES, Beam main, TSG.Point p1_bottm, TSG.Point p2_bottm,
            TSD.View current_view, double SHORTNENING_VALUE_FOR_BOTTOM_VIEW)
        {
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
            {
                TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                /////////////////BOTTOM VIEW CREATION //////////////////////////////////////

                /////////////////////////////////////////////////////DECLARING VIEW NAME FOR BOTTOM VIEW ///////////////////////////////////////                    
                TSD.View CREATE_BOTTOM_VIEW;
                TSD.View CREATE_BOTTOM_VIEW1;
                List<TSD.View> list_of_bottom_section_views = new List<TSD.View>();

                ////////////////////////////////////////////////////BOTTOM VIEW ON AND OFF CONDITION ///////////////////////////////////////////
                //if (BOTTOM_VIEW == "ON")
                //{
                //////////////////////////////////////////////////BOTTOM VIEW CREATION USING FUNCTION//////////////////////////////////////////
                skBottomView.bottom_view_creation(main, current_view, output, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]), out CREATE_BOTTOM_VIEW, out CREATE_BOTTOM_VIEW1, p1_bottm, p2_bottm, BOTTOM_VIEW_TOCREATE, SHORTNENING_VALUE_FOR_BOTTOM_VIEW, list_of_parts_for_bottom_view_mark_retain, out bottom_view_list, out bottom_view_FLANGE_CUT_LIST, drg_att);
                if (CREATE_BOTTOM_VIEW != null)
                    list_of_bottom_section_views.Add(CREATE_BOTTOM_VIEW);
                if (CREATE_BOTTOM_VIEW1 != null)
                    list_of_bottom_section_views.Add(CREATE_BOTTOM_VIEW1);
                //Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW, main, drg_att);
                //Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW1, main, drg_att);
                //////////////////////////////////////////////////FLANGE CUT DIMENSION ///////////////////////////////////////////////////////
                if (BOTTOM_VIEW_TOCREATE.Contains("ON") || BOTTOM_VIEW_TOCREATE.Contains("RIGHT") || BOTTOM_VIEW_TOCREATE.Contains("LEFT") || BOTTOM_VIEW_TOCREATE.Contains("BOTH"))
                {



                    //Create_FLANGE_CUT_dimensions(CREATE_BOTTOM_VIEW, main);


                    ///////////////////////////////////////////////////////////////////////

                    /////////BOLT RD DIMENSION//////////////
                    ///////////////////////////////////////////////////filtering bolts from all parts in top view/////////////////////////////////////////////////////////////////////////////

                    TSG.Matrix bottom_mat = TSG.MatrixFactory.ToCoordinateSystem(CREATE_BOTTOM_VIEW.ViewCoordinateSystem);
                    TSD.PointList rd_point_list1 = new TSD.PointList();
                    ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////

                    //////////////End of bolt rd dimension FOR BottomVIEW //////////////////////////







                    ///////Copied code for bolt in bottom flange using new logic///////////////////////////////////////////


                    foreach (TSD.View bottom_section_view in list_of_bottom_section_views)
                    {
                        try
                        {

                            List<TSG.Point> singlebolts = new List<TSG.Point>();

                            TSG.Matrix to_view_matrix = TSG.MatrixFactory.ToCoordinateSystem(bottom_section_view.DisplayCoordinateSystem);
                            TSD.StraightDimensionSetHandler bolt_combine_dim = new TSD.StraightDimensionSetHandler();

                            ///////////////////////////////////////////////////getting bolt matrix points for bolt 3x3 dimension///////////////////////////////////////////////////////////////////////////


                            TSM.ModelObjectEnumerator model_bolt_enum = main_part.GetBolts();
                            while (model_bolt_enum.MoveNext())
                            {
                                TSM.BoltGroup boltgrp = model_bolt_enum.Current as TSM.BoltGroup;
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(bottom_section_view.DisplayCoordinateSystem));
                                TSG.CoordinateSystem boltcheck = boltgrp.GetCoordinateSystem();
                                TSG.Vector xaxis = boltcheck.AxisX;
                                TSG.Vector yaxis = boltcheck.AxisY;
                                TSG.Vector zaxis = yaxis.Cross(xaxis);
                                zaxis.Normalize();
                                mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                                if ((zaxis.Z != 0))
                                {
                                    foreach (TSG.Point pt in boltgrp.BoltPositions)
                                    {
                                        TSG.Point conv_pt = to_view_matrix.Transform(pt);
                                        if (conv_pt.Z < 0)
                                        {
                                            singlebolts.Add(conv_pt);
                                        }

                                    }
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



                            for (int i = 0; i < groupedbolts.Count; i++)
                            {
                                rd_point_list1.Add(groupedbolts[i].point_in_group[0]);

                            }



                            TSD.PointList FINAL_RD_LIST2 = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list1);
                            FINAL_RD_LIST2.Insert(0, (new TSG.Point(0, 0, 0)));
                            sortingHandler.SortPoints(FINAL_RD_LIST2);
                            /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
                            TSD.StraightDimensionSetHandler bolt_rd_dim1 = new TSD.StraightDimensionSetHandler();
                            try
                            {
                                TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                                rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                                rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                                rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                                rd_att.Color = DrawingColors.Gray70;
                                rd_att.Text.Font.Color = DrawingColors.Gray70;
                                rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                                rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                                double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                                TSG.Point p1 = (FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point);
                                TSG.Point p2 = new TSG.Point((FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point).X, distance, 0);
                                double distance_value = TSG.Distance.PointToPoint(p1, p2);
                                /////////////////////////////////////////////////////rd dimension creation //////////////////////////////////////////////////////////////////////////////////////////////////////
                                bolt_rd_dim1.CreateDimensionSet(CREATE_BOTTOM_VIEW, FINAL_RD_LIST2, new TSG.Vector(0, 1, 0), CREATE_BOTTOM_VIEW.RestrictionBox.MaxPoint.Y + 75, rd_att);
                            }
                            catch
                            {
                            }



                            double distance_for_bolt_dim = 0;
                            if (groupedbolts.Count > 0)
                            {
                                if (groupedbolts[0].x_dist < 150)
                                {
                                    double REST_BOX_MIN = Math.Abs(bottom_section_view.RestrictionBox.MinPoint.X);

                                    distance_for_bolt_dim = groupedbolts[0].x_dist + REST_BOX_MIN + 75;
                                }
                                else
                                {
                                    distance_for_bolt_dim = 30;
                                }
                            }
                            int h = 1;

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
                                                            h++;

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
                                                            //bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim);
                                                            TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                                            if (ptlist_for_boltdim.Count > 2)
                                                            {
                                                                bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);

                                                            }
                                                            else
                                                            {
                                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                                                bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);



                                                            }
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        h = 1;
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
                                                        // bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim);
                                                        TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                                        if (ptlist_for_boltdim.Count > 2)
                                                        {
                                                            bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);

                                                        }
                                                        else
                                                        {
                                                            REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                                            REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                                            bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);



                                                        }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    h = 1;
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
                                                //bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim);
                                                TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                                if (ptlist_for_boltdim.Count > 2)
                                                {
                                                    bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);

                                                }
                                                else
                                                {
                                                    REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                                    REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                                    bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, REVERSE_OF_PTLIST, new TSG.Vector(-1, 0, 0), distance_for_bolt_dim, dim_font_height);



                                                }
                                            }
                                            catch
                                            {
                                            }
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

                                                    distance_for_bolt_dim = Math.Abs(groupedbolts[i - 1].x_dist - groupedbolts[i].x_dist);
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
                                            //bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new TSG.Vector(1, 0, 0), 175);
                                            TSD.PointList REVERSE_OF_PTLIST = new TSD.PointList();
                                            if (ptlist_for_boltdim.Count > 2)
                                            {
                                                bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, ptlist_for_boltdim, new_vector, distance_for_bolt_dim + 75, dim_font_height);

                                            }
                                            else
                                            {
                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[1]);
                                                REVERSE_OF_PTLIST.Add(ptlist_for_boltdim[0]);
                                                bolt_combine_dim.CreateDimensionSet(bottom_section_view as TSD.ViewBase, REVERSE_OF_PTLIST, new_vector, distance_for_bolt_dim + 75, dim_font_height);



                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }


                                }


                            }
                            ////////////////////end of 3x3 dimension  for top view///////////////////////

                        }
                        catch
                        {
                        }

                    }



                    ///////////////////////////////////////////////////////////////////////

                }
                //else if (BOTTOM_VIEW == "OFF")
                //{
                //}





            }
        }

        private List<TSD.View> CreateTopView(Type type_for_bolt, double output, string TOP_VIEW_TOCREATE,
            string TOP_VIEW_needed, string drg_att, List<TSD.View> TOP_view_FLANGE_CUT_LIST, List<double> MAINPART_PROFILE_VALUES, Beam main, TSG.Point p3_bottm, TSG.Point p4_bottm, StraightDimensionSet.StraightDimensionSetAttributes fixed_attributes, TSD.View current_view)
        {
            if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
            {

                TSD.StraightDimensionSet.StraightDimensionSetAttributes dim_font_height = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                dim_font_height.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                dim_font_height.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                /////////////////BOTTOM VIEW CREATION //////////////////////////////////////

                /////////////////////////////////////////////////////DECLARING VIEW NAME FOR BOTTOM VIEW ///////////////////////////////////////                    
                //TSD.View CREATE_BOTTOM_VIEW;
                //TSD.View CREATE_BOTTOM_VIEW1;


                ////////////////////////////////////////////////////BOTTOM VIEW ON AND OFF CONDITION ///////////////////////////////////////////
                //if (BOTTOM_VIEW == "ON")
                //{
                //////////////////////////////////////////////////BOTTOM VIEW CREATION USING FUNCTION//////////////////////////////////////////

                TSD.View CREATE_BOTTOM_VIEW = null;
                TSD.View CREATE_BOTTOM_VIEW1 = null;
                if (TOP_VIEW_needed != "yes")
                {
                    skBottomView.TOP_view_creation(main, current_view, output, Convert.ToDouble(MAINPART_PROFILE_VALUES[0]), out CREATE_BOTTOM_VIEW, out CREATE_BOTTOM_VIEW1, p3_bottm, p4_bottm, TOP_VIEW_TOCREATE, out TOP_view_FLANGE_CUT_LIST, drg_att);
                    flangeOutDimension.Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW, main, drg_att);
                    flangeOutDimension.Create_FLANGE_CUT_dimensions_top(CREATE_BOTTOM_VIEW1, main, drg_att);
                }

                if ((CREATE_BOTTOM_VIEW != null) || (CREATE_BOTTOM_VIEW1 != null))
                {
                    //////////////////////////////////////////////////FLANGE CUT DIMENSION ///////////////////////////////////////////////////////
                    if (TOP_VIEW_TOCREATE.Contains("ON") || TOP_VIEW_TOCREATE.Contains("RIGHT") || TOP_VIEW_TOCREATE.Contains("LEFT") || TOP_VIEW_TOCREATE.Contains("BOTH"))
                    {


                        //Create_FLANGE_CUT_dimensions(CREATE_BOTTOM_VIEW, main);


                        ///////////////////////////////////////////////////////////////////////

                        /////////BOLT RD DIMENSION//////////////
                        ///////////////////////////////////////////////////filtering bolts from all parts in top view/////////////////////////////////////////////////////////////////////////////
                        TSD.DrawingObjectEnumerator enum_for_bolt2 = CREATE_BOTTOM_VIEW.GetAllObjects(type_for_bolt);
                        TSG.Matrix bottom_mat = TSG.MatrixFactory.ToCoordinateSystem(CREATE_BOTTOM_VIEW.ViewCoordinateSystem);
                        TSD.PointList rd_point_list1 = new TSD.PointList();
                        ///////////////////////////////////////////////////getting bolt matrix points for bolt dimension///////////////////////////////////////////////////////////////////////////
                        while (enum_for_bolt2.MoveNext())
                        {
                            TSD.Bolt drgbolt = enum_for_bolt2.Current as TSD.Bolt;
                            TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                            TSM.BoltArray bolt = modelbolt as TSM.BoltArray;


                            if (bottom_mat.Transform((bolt.BoltPositions[0]) as TSG.Point).Z < (bottom_mat.Transform(main.EndPoint).Z) - Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2)
                            {
                                TSD.Bolt drgbolt1 = drgbolt;
                                TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, CREATE_BOTTOM_VIEW);
                                if (POINT_FOR_BOLT_MATRIX != null)
                                {
                                    int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                    int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                    for (int i = 0; i < x; i++)
                                    {
                                        ///////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                        rd_point_list1.Add(POINT_FOR_BOLT_MATRIX[y - 1, i]);
                                    }
                                }
                            }
                        }
                        /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                        TSD.PointList FINAL_RD_LIST2 = duplicateRemover.pointlist_remove_duplicate_Xvalues(rd_point_list1);
                        FINAL_RD_LIST2.Add(new TSG.Point(0, 0, 0));
                        sortingHandler.SortPoints(FINAL_RD_LIST2);
                        /////////////////////////////////////////////////// inserting bolt rd dimension ////////////////////////////////////////////////////////////////////////////////////////
                        TSD.StraightDimensionSetHandler bolt_rd_dim1 = new TSD.StraightDimensionSetHandler();
                        try
                        {
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                            rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                            rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                            rd_att.Color = DrawingColors.Gray70;
                            rd_att.Text.Font.Color = DrawingColors.Gray70;
                            rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                            rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                            ////////////////////////////////////////////////////dimension distance placing linking /////////////////////////////////////////////////////////////////////////////////////////
                            double distance = Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2;
                            TSG.Point p1 = (FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point);
                            TSG.Point p2 = new TSG.Point((FINAL_RD_LIST2[FINAL_RD_LIST2.Count - 1] as TSG.Point).X, distance, 0);
                            double distance_value = TSG.Distance.PointToPoint(p1, p2);
                            /////////////////////////////////////////////////////rd dimension creation //////////////////////////////////////////////////////////////////////////////////////////////////////
                            bolt_rd_dim1.CreateDimensionSet(CREATE_BOTTOM_VIEW, FINAL_RD_LIST2, new TSG.Vector(0, 1, 0), distance_value, rd_att);
                        }
                        catch
                        {
                        }
                        //////////////End of bolt rd dimension FOR BottomVIEW //////////////////////////






                        //////////////3x3 dimension for top view/////////////////////
                        TSD.DrawingObjectEnumerator enum_for_bolt3 = CREATE_BOTTOM_VIEW.GetAllObjects(type_for_bolt);
                        while (enum_for_bolt3.MoveNext())
                        {
                            TSD.PointList list3x3 = new TSD.PointList();
                            TSD.Bolt drgbolt = enum_for_bolt3.Current as TSD.Bolt;
                            TSM.ModelObject modelbolt = new TSM.Model().SelectModelObject(drgbolt.ModelIdentifier);
                            TSM.BoltArray bolt = modelbolt as TSM.BoltArray;

                            if (bottom_mat.Transform((bolt.BoltPositions[0]) as TSG.Point).Z < (bottom_mat.Transform(main.EndPoint).Z) - Convert.ToDouble(MAINPART_PROFILE_VALUES[0]) / 2)
                            {
                                TSD.Bolt drgbolt1 = drgbolt;
                                TSG.Point[,] POINT_FOR_BOLT_MATRIX = boltMatrixHandler.GetBoltMatrix(drgbolt1, CREATE_BOTTOM_VIEW);
                                if (POINT_FOR_BOLT_MATRIX != null)
                                {
                                    int y = POINT_FOR_BOLT_MATRIX.GetLength(0);
                                    int x = POINT_FOR_BOLT_MATRIX.GetLength(1);
                                    for (int i = 0; i < y; i++)
                                    {
                                        /////////////////////////////////////////////////// condition for getting last row of bolts//////////////////////////////////////////////////////////////////////////////////////////////
                                        list3x3.Add(POINT_FOR_BOLT_MATRIX[i, x - 1]);
                                    }
                                    /////////////////////////////////////////////////// removing duplicate points from boltpoints///////////////////////////////////////////////////////////////////////////////           
                                    TSD.PointList FINAL_list3x3 = duplicateRemover.pointlist_remove_duplicate_Yvalues(list3x3);
                                    FINAL_list3x3.Add(new TSG.Point(list3x3[0].X, 0, 0));
                                    /////////////////////////////////////////////////// inserting bolt 3X3 dimension ////////////////////////////////////////////////////////////////////////////////////////
                                    TSD.StraightDimensionSetHandler dim_3x3 = new TSD.StraightDimensionSetHandler();
                                    try
                                    {

                                        //TSG.Point p1 = (FINAL_list3x3[FINAL_list3x3.Count - 1] as TSG.Point);
                                        //TSG.Point p2 = new TSG.Point((FINAL_list3x3[FINAL_list3x3.Count - 1] as TSG.Point).X, 0, 0);
                                        //double distance_value = TSG.Distance.PointToPoint(p1, p2);
                                        dim_3x3.CreateDimensionSet(CREATE_BOTTOM_VIEW, FINAL_list3x3, new TSG.Vector(1, 0, 0), 200, fixed_attributes);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        ////////////////////end of 3x3 dimension  for top view///////////////////////







                        ///////////////////////////////////////////////////////////////////////

                    }
                }

                //else if (BOTTOM_VIEW == "OFF")
                //{
                //}





            }

            return TOP_view_FLANGE_CUT_LIST;
        }

        private static void markTest(DrawingHandler my_handler, AssemblyDrawing beam_assembly_drg,
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
                    Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
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

                    Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_FRONT, true);

                    Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();

                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    my_handler.GetDrawingObjectSelector().SelectObjects(PARTMARK_TO_BE_PROVIDED_TOP, true);

                    Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    my_handler.GetDrawingObjectSelector().SelectObjects(BOLTMARK_TO_BE_PROVIDED_TOP, true);

                    Tekla.Structures.Model.Operations.Operation.RunMacro(@"..\drawings\acmd_create_marks_selected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();

                }



            }
        }
        #endregion

        #region form events
        private void button2_Click(object sender, EventArgs e)
        {
            dgvlog.Rows.Clear();
            chkrdconnmark.Checked = false;
            chkknockoffdim.Checked = false;
            chkcutlen.Checked = false;
            chkscale.Checked = false;
            chkminlen.Checked = false;
            chkwptxteledim.Checked = false;
            chksecscale.Checked = false;
            chkeledim.Checked = false;
            chka1.Checked = false;
            chka2.Checked = false;
            chka3.Checked = false;
            chkmanualinput.Checked = false;

            cmbsecscale.ResetText();
            cmbclient.ResetText();



        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            client = cmbclient.Text;
            string PROJECT_NAME = client;
            if (PROJECT_NAME.Equals("LIPHART"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkrdconnmark.Checked = true;
                chkknockoffdim.Checked = true;
                chkfontsize.Checked = false;



            }
            if (PROJECT_NAME.Equals("SME"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkrdconnmark.Checked = true;
                chkknockoffdim.Checked = true;
                chkfontsize.Checked = false;



            }
            else if (client.Equals("BENHUR"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkrdconnmark.Checked = true;
                chkknockoffdim.Checked = true;
                chkeledim.Checked = true;
                chkwptxteledim.Checked = true;
                chkfontsize.Checked = false;


            }
            else if (client.Equals("FORD"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkeledim.Checked = true;
                chkfontsize.Checked = false;


            }
            else if (client.Equals("TRINITY"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkeledim.Checked = true;
                chkfontsize.Checked = false;

            }
            else if (client.Equals("STEFFY&SON"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkrdconnmark.Checked = true;
                chkeledim.Checked = true;
                chkfontsize.Checked = false;
            }
            else if (client.Equals("HAMILTON"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chkrdconnmark.Checked = true;
                chkeledim.Checked = true;
                chkfontsize.Checked = false;
            }
            else if (client.Equals("NONE"))
            {
                chkmanualinput.Checked = false;
                chka0.Enabled = false;
                chkrdconnmark.Checked = false;
                chkknockoffdim.Checked = false;
                chkeledim.Checked = false;
                chkwptxteledim.Checked = false;
                chkfontsize.Checked = false;
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

            }


        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

            if (chkscale.Checked == true)
            {

                txtscale.Enabled = true;


            }
            else
            {
                txtscale.Clear();
                txtscale.Enabled = false;

            }



        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

            if (chkminlen.Checked == true)
            {

                txtminlen.Enabled = true;


            }
            else
            {
                txtminlen.Clear();
                txtminlen.Enabled = false;

            }

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (chksecscale.Checked == true)
            {
                cmbsecscale.Enabled = true;
            }
            else
            {
                cmbsecscale.ResetText();
                cmbsecscale.Enabled = false;

            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (chkmanualinput.Checked == true)
            {
                if (client.Equals("TRINITY") || client.Equals("LIPHART") || client.Equals("BENHUR") || client.Equals("HILLSDALE") || client.Equals("HAMILTON"))
                {
                    chka1.Enabled = true;
                }
                if (client.Equals("LIPHART") || client.Equals("BENHUR") || client.Equals("FORD") || client.Equals("STEFFY&SON"))
                {
                    chka2.Enabled = true;
                }
                if (client.Equals("FORD") || client.Equals("STEFFY&SON"))
                {
                    chka3.Enabled = true;
                }
                if (client.Equals("SME"))
                {
                    chka1.Enabled = true;
                    chka3.Enabled = true;
                    chka0.Enabled = true;
                }
                if (client.Equals("NONE"))
                {
                    chka1.Enabled = true;
                    chka2.Enabled = true;
                    chka3.Enabled = true;
                }

                chkscale.Enabled = true;
                chkminlen.Enabled = true;

            }
            else
            {
                chka1.Checked = false;
                chka2.Checked = false;
                chka3.Checked = false;
                chkscale.Checked = false;
                chkminlen.Checked = false;

                chka1.Enabled = false;
                chka2.Enabled = false;
                chka3.Enabled = false;
                chkscale.Enabled = false;
                chkminlen.Enabled = false;


            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtscale.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtscale.Text = txtscale.Text.Remove(txtscale.Text.Length - 1);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtminlen.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtminlen.Text = txtminlen.Text.Remove(txtminlen.Text.Length - 1);
            }

        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (chkfontsize.Checked == true)
            {
                chk1by8.Enabled = true;
                chk3by32.Enabled = true;
                chk9by64.Enabled = true;
            }
            else
            {
                chk1by8.Checked = false;
                chk3by32.Checked = false;
                chk9by64.Checked = false;
                chk1by8.Enabled = false;
                chk3by32.Enabled = false;
                chk9by64.Enabled = false;

            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnpaste_Click(object sender, EventArgs e)
        {
            //check memory
            // Read the clipboard content
            string clipboardText = Clipboard.GetText();

            if (clipboardText.Trim().Length > 1)
            {
                string[] clipboarddata = clipboardText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (clipboarddata.Length > 0)
                {
                    dgvlog.Rows.Clear();

                    //// Parse the clipboard content (assuming it's tab-separated values)
                    //DataTable dataTable = new DataTable();

                    // Add rows
                    for (int i = 0; i < clipboarddata.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(clipboarddata[i]))
                        {
                            //string drgmark = clipboarddata[i].ToString();
                            dgvlog.Rows.Add(clipboarddata[i].ToString());
                        }
                    }
                    skWinLib.updaterowheader(dgvlog);
                }
            }
        }

        private void btnselect_Click(object sender, EventArgs e)
        {


            List<DrawingData> MyDrgList = new List<DrawingData>();
            foreach (DataGridViewRow MyRow in dgvlog.Rows)
            {
                string Assembly_Mark = string.Empty;
                //check whether row have drgmark
                if (MyRow.Cells["drgmark"].Value != null)
                {
                    Assembly_Mark = MyRow.Cells["drgmark"].Value.ToString();
                    if (Assembly_Mark.Trim().Length >= 1)
                    {
                        DrawingData MyDrawingData = new DrawingData();
                        MyDrawingData.Assembly_Mark = Assembly_Mark;
                        MyDrgList.Add(MyDrawingData);

                    }
                }
            }

            TSM.Model MyModel = new TSM.Model();
            BinaryFilterExpression filterExpression1 = new BinaryFilterExpression(new AssemblyFilterExpressions.PositionNumber(), StringOperatorType.IS_EQUAL,
                        new StringConstantFilterExpression(string.Join(" ", MyDrgList.Select(d => d.Assembly_Mark).ToList())));

            BinaryFilterExpression filterExpression2 = new BinaryFilterExpression(new ObjectFilterExpressions.Type(), NumericOperatorType.IS_EQUAL,
               new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.ASSEMBLY));

            var filterDefinition = new BinaryFilterExpressionCollection();
            filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression1, BinaryFilterOperatorType.BOOLEAN_AND));
            filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression2)); ;

            ModelObjectEnumerator MyAssemblys = MyModel.GetModelObjectSelector().GetObjectsByFilter(filterDefinition);
            int Count = MyAssemblys.GetSize();
            Tekla.Structures.Model.Operations.Operation.DisplayPrompt(Count.ToString());

            ArrayList objectsToSelect = new ArrayList();

            foreach (TSM.Assembly MyAssembly in MyAssemblys)
            {
                if (MyAssembly != null)
                {
                    string Assembly_Mark = string.Empty;
                    MyAssembly.GetReportProperty("ASSEMBLY_POS", ref Assembly_Mark);
                    bool check_for_numbering = Assembly_Mark.Contains('?');
                    if (check_for_numbering == false)
                    {
                        //check if already data exist
                        DrawingData MyDrawingData = MyDrgList.FirstOrDefault(d => d.Assembly_Mark == Assembly_Mark);
                        if (MyDrawingData.ProfileType == null)
                        {
                            TSM.ModelObject MyModelObject = MyAssembly.GetMainPart();
                            if (MyModelObject != null)
                            {

                                TSM.Part MainPart = MyModelObject as TSM.Beam;
                                if (MainPart != null)
                                {

                                    double LENGTH = 0;
                                    MainPart.GetReportProperty("LENGTH", ref LENGTH);
                                    string prof_type = string.Empty;
                                    MainPart.GetReportProperty("PROFILE_TYPE", ref prof_type);
                                    int secct = MyAssembly.GetSecondaries().Count;

                                    string Part_Mark = string.Empty;
                                    MyAssembly.GetReportProperty("PART_POS", ref Part_Mark);

                                    string Name = MyAssembly.Name;

                                    string guid = string.Empty;
                                    MyAssembly.GetReportProperty("GUID", ref guid);

                                    //Model Selection
                                    objectsToSelect.Add(MyAssembly);
                                    //Update drawing class
                                    if (MyDrawingData != null)
                                    {
                                        MyDrawingData.Name = Name;
                                        MyDrawingData.Guid = guid;
                                        MyDrawingData.Part_Mark = Part_Mark;
                                        MyDrawingData.Length = LENGTH;
                                        MyDrawingData.Secondary = secct;
                                        MyDrawingData.ProfileType = prof_type;

                                    }
                                }
                            }
                        }

                    }

                }
            }

            //Update Datagrid
            foreach (DataGridViewRow MyRow in dgvlog.Rows)
            {
                string Assembly_Mark = string.Empty;
                //check whether row have drgmark
                if (MyRow.Cells["drgmark"].Value != null)
                {
                    Assembly_Mark = MyRow.Cells["drgmark"].Value.ToString();
                    DrawingData MyDrawingData = MyDrgList.FirstOrDefault(d => d.Assembly_Mark == Assembly_Mark);
                    if (MyDrawingData != null)
                    {
                        if (skTSLib.IsUnitImperial == true)
                        {
                            Tekla.Structures.Datatype.Distance MyDist = new Tekla.Structures.Datatype.Distance(MyDrawingData.Length);
                            MyRow.Cells["drglen"].Value = MyDist.ToFractionalInchesString();
                        }
                        else
                            MyRow.Cells["drglen"].Value = MyDrawingData.Length.ToString("#0.0#");

                        MyRow.Cells["drgname"].Value = MyDrawingData.Name;
                        MyRow.Cells["drgproftype"].Value = MyDrawingData.ProfileType;
                        MyRow.Cells["drgsecs"].Value = MyDrawingData.Secondary;

                    }

                }
            }

            TSM.UI.ModelObjectSelector _selector = new TSM.UI.ModelObjectSelector();
            _selector.Select(objectsToSelect);
        }
        #endregion

    }
}
