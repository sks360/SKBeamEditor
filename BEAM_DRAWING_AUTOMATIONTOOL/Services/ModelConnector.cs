using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures;

namespace SK.Tekla.Drawing.Automation.Services
{
    /// <summary>
    /// Establish connection with Tekla Model
    /// </summary>
    public class ModelConnector
    {
        public bool ValidateConnectivity()
        {
            var connected = false;
            
            var currentModel = skTSLib.ModelAccess.ConnectToModel();
            if (currentModel != null)
            {
                //try establishing conneciton to Tekla
                connected = TeklaStructures.Connect();
            }
            return connected;
        }

        //public FormDrawingEditor()
        //{
        //    //2409.18
        //    try
        //    {
        //        // Set the debugflag based on current user registry 
        //        //debugflag = skWinLib.DebugFlag(skApplicationName);
        //        //skWinLib.username = "SKS158";
        //        //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Before-InitializeComponent", "");
        //        //msgCaption = skApplicationName + " Ver. " + skApplicationVersion;
        //        InitializeComponent();
        //        //skWinLib.accesslog(skApplicationName, skApplicationVersion, "After-InitializeComponent", "");

        //        //Validation of L & T Drive
        //        //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Validate Esskay Mapping", "");
        //        //bool IsEsskayMappingValid = skWinLib.IsDrivesMapped();
        //        //if (IsEsskayMappingValid == false)
        //        //{
        //        //    skWinLib.accesslog(skApplicationName, skApplicationVersion, "Esskay Mapping (L: & T: Drive Failed", "Esskay Mapping (L: & T: Drive Failed [ " + skWinLib.username + " / " + skWinLib.systemname + "  ].", "", "", "");
        //        //    MessageBox.Show("Esskay Mapping (L: & T: Drive Failed", msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        //    System.Environment.Exit(360);
        //        //}


        //        //Validation for Esskay Domain User
        //        //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Validate Esskay Domain User", "YearlyValidation - MMM/dd/yy" + DateTime.Now.ToString("MMM") + "/" + DateTime.Now.ToString("dd") + "/" + DateTime.Now.ToString("yy"));
        //        //skWinLib.Esskay_Tool_Validation(skApplicationName, skApplicationVersion);

        //        //string mysysid = skWinLib.systemname;
        //        //string myuser = skWinLib.username;

        //        //Validation for Esskay EmpData
        //        //skWinLib.MyEmployeeData = skWinLib.GetEmployeeData("SKS218");
        //        //skWinLib.MyEmployeeData = skWinLib.GetEmployeeData(skWinLib.username);
        //        //if (skWinLib.MyEmployeeData == null || skWinLib.MyEmployeeData.Count == 0)
        //        //{
        //        //    skWinLib.accesslog(skApplicationName, skApplicationVersion, "Validation for Esskay User Failed", "Validation for Esskay User Failed [ " + skWinLib.TSVersion + " ].", "", "", "");
        //        //    MessageBox.Show("Validation for Esskay User Failed", msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        //    System.Environment.Exit(360);
        //        //}
        //        //else
        //        //    lblsbar2.Text = skWinLib.MyEmployeeData[1] + "/" + skWinLib.MyEmployeeData[2];


        //        //Establish connection with Tekla Model
        //        MyModel = skTSLib.ModelAccess.ConnectToModel();
        //        if (MyModel == null)
        //        {
        //            //skWinLib.accesslog(skApplicationName, skApplicationVersion, "ConnectToModel-Failed", "Could not establish connection with Tekla [ " + skTSLib.SKVersion + " ].", "", "", "");
        //            MessageBox.Show("Could not establish connection with Tekla [ " + skTSLib.SKVersion + " ].\n\nEnsure Tekla [ " + skTSLib.SKVersion + " ] model is open,if still issue's contact support team.", msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        //            System.Environment.Exit(360);
        //        }
        //        else
        //        {
        //            //skTSLib.setStartup(skApplicationName, skApplicationVersion, "ConnectToModel-Success", "");
        //            //msgCaption = skApplicationName + " Ver. " + skApplicationVersion + " [ " + skTSLib.SKVersion + " / " + skTSLib.ModelName + " ] ";
        //            this.Text = msgCaption;

        //            //lblsbar2.Text = skWinLib.username;


        //            TeklaStructures.Connect();
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Error@InitializeComponent ", "Error@InitializeComponent\n\nOriginal error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
        //        MessageBox.Show("Error@InitializeComponent\n\nOriginal error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        //        System.Environment.Exit(360);
        //    }
        //}


        //private void Form1_Load_1(object sender, EventArgs e)
        //{
        //    //Ipcheck mysec = new Ipcheck();
        //    //if (!mysec.domaincheck())
        //    //{
        //    //    MessageBox.Show("Please contact Tekla admin");
        //    //    this.BeginInvoke(new MethodInvoker(this.Close));
        //    //}
        //    //GetLastUserEntry();
        //    //skWinLib.GetControlValues(this.Controls, skApplicationName, true);
        //    //this.Text = skWinLib.UpdateHeaderInformation(skApplicationName, skApplicationVersion, MyModel.GetInfo().ModelName, 1, 0, 0, 0, 0);
        //}
    }
}
