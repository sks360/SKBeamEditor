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
        public static string skApplicationVersion = "2506.0.0.0";
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


        private CustomInputModel _inputModel;

        private FontSizeSelector fontSize = FontSizeSelector.OneBy8;

        private SheetSelector sheetSelector = SheetSelector.UnDefined;

        private string client;

        private double _scale = 10;

        private double _minLength = 1666.24; //A3 sheet

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
            
            //clear rows
            dgvlog.Rows.Clear();
            string remark = string.Empty;
            int proct = 0;
            int errct = 0;
            List<string> drglog = new List<string>();
            BeamDrawingCreator beamDrawingCreator = new BeamDrawingCreator(drglog,_inputModel);
            beamDrawingCreator.PrepareDrawing(currentModel, ref proct, ref errct);

            foreach (string drgdata in drglog)
            {
                DataGridViewRow MyRow = dgvlog.Rows[dgvlog.Rows.Add()];
                MyRow.Cells["drgmark"].Value = drgdata.Substring(0,drgdata.LastIndexOf("^")+1);
                MyRow.Cells["drgrmk"].Value = drgdata;
            }
            dgvlog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

            ////////////////COMMENTED VEERA/////////////////////
            //pbar.Value = pbar.Value + 1;
            //pbar.Refresh();

            ////skWinLib.updaterowheader(dgvlog);

            //pbar.Value = pbar.Value + 1;
            //pbar.Refresh();
            dgvlog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

            //pbar.Value = pbar.Value + 1;
            //pbar.Refresh();


            //pbar.Value = pbar.Maximum;
            //pbar.Refresh();
            //pbar.Visible = false;
            ////////////////COMMENTED VEERA/////////////////////
            ///
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
                MinLength = _minLength,
                NeedEleDimension = chkwptxteledim.Checked,
                NeedRDConnectionMark = chkrdconnmark.Checked,
                NeedCutLength = chkcutlen.Checked,
                KnockOffDimension = chkknockoffdim.Checked,
                SecScale = chksecscale.Checked && cmbsecscale.Text.Length > 0 ? Convert.ToDouble(cmbsecscale.Text) : 0
            };
        }

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
