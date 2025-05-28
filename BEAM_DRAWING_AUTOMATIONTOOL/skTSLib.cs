//Omm Muruga 2
//Common Function's irrespective of any application, tool, plugin
//Ensure naming of function is meaningfull to understand
//Dont have too many short variables
//If any major changes required in functions please let Vijayakumar know's about this.
//Don't modifiy existing function/class/sub unless you are aware of the consequences
//Use Commentline as much as possible for future reference and other's can understand easily
//Most function's or not tested so ensure its quality at your own risk.
//08Jan22 1502

using SR = System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

// Tekla Structures namespaces
using Tekla.Structures;
using Tekla.Structures.Filtering;
using Tekla.Structures.Filtering.Categories;
using Tekla.Structures.Model;
using Tekla.Structures.Drawing;
using Tekla.Structures.ModelInternal;
using Tekla.Structures.Solid;
using Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSDT = Tekla.Structures.Datatype;
using TSDP = Tekla.Structures.Dialog.ProfileConversion;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSDLG = Tekla.Structures.Dialog;
using TSF= Tekla.Structures.Filtering;
using TSFC = Tekla.Structures.Filtering.Categories;
using Tekla.Structures.Model.Operations;
using TSMO = Tekla.Structures.Model.Operations;

using System.Windows;
//using System.Numerics;
using Vector = Tekla.Structures.Geometry3d.Vector;
using static Tekla.Structures.Filtering.Categories.TaskFilterExpressions;
using static skTSLib;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
//using System.Drawing.Drawing2D;
//using Microsoft.ProjectServer.Client;
//using Mysqlx.Crud;
////using ZstdSharp.Unsafe;

//using static SteelFabNumbering.frmsteelfab;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

//using Microsoft.ProjectServer.Client;
//using Microsoft.Office.Interop.Excel;

public class skTSLib
{
    public static string SKVersion = "2024.0";
    public static TSM.Model MyModel = new TSM.Model();
    public static string ParentPath = string.Empty;
    public static string ModelPath = string.Empty;
    public static string ModelName = string.Empty;
    public static string ProjectNumber = string.Empty;
    public static string SKProjectNumber = string.Empty;
    public static string SKProjectInfo2 = string.Empty;
    public static bool IsSharedmodel = false;
    public static string CurrentUser = string.Empty;
    public static string Version = string.Empty;
    public static string Configuration = string.Empty;

    //public static string ModelPath = MyModel.GetInfo().ModelPath;
    //public static string ModelName = MyModel.GetInfo().ModelName.Replace(".db1", "").Replace(".DB1", "");
    //public static string ProjectNumber = MyModel.GetProjectInfo().ProjectNumber;
    //public static string SKProjectNumber = MyModel.GetProjectInfo().Info1;
    //public static string SKProjectInfo2 = MyModel.GetProjectInfo().Info2;
    //public static bool IsSharedmodel = MyModel.GetInfo().SharedModel;
    //public static string CurrentUser = Tekla.Structures.TeklaStructuresInfo.GetCurrentUser();
    //public static string Version = Tekla.Structures.TeklaStructuresInfo.GetCurrentProgramVersion();  
    //public static string Configuration = Tekla.Structures.ModuleManager.Configuration.ToString();

    public static int esskayappvalidity = skWinLib.esskayappvalidity;
    public static bool IsUnitImperial = true;
    public static TSMUI.GraphicsDrawer drawer = new TSMUI.GraphicsDrawer();
    public static string xspath = string.Empty;
    public static string XS_SYSTEM = string.Empty;
    public static string XSDATADIR = string.Empty;
    public static string XS_AD_ENVIRONMENT = string.Empty;
    public static string XS_DIR = string.Empty;
    public static string XSBIN = string.Empty;
    public static string XS_FIRM = string.Empty;
    public static string SK_CLIENT = string.Empty;
    public static string SK_COMMON = "T:\\Project\\Common";
    public static string XS_TEMPLATE_DIRECTORY = string.Empty;
    public static string XS_PROJECT = string.Empty;

    /* Windows API for getting active MDI view name */
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern IntPtr GetTopWindow(IntPtr hwndParent);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    static extern int GetWindowTextLength(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("User32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    public static TSMUI.Color Red = new TSMUI.Color(1.0, 0.0, 0.0); // Red
    public static TSMUI.Color Green = new TSMUI.Color(0.0, 1.0, 0.0); // Green
    public static TSMUI.Color Yellow = new TSMUI.Color(1.0, 1.0, 0.0); // Yellow
    public static TSMUI.Color Blue = new TSMUI.Color(0.0, 0.0, 1.0); // Blue
    public static TSMUI.Color Magenta = new TSMUI.Color(1.0, 0.0, 1.0); // Magenta
    public static TSMUI.Color Cyan = new TSMUI.Color(0.0, 1.0, 1.0); // Cyan
    public static TSMUI.Color Purple = new TSMUI.Color(0.5, 0.0, 0.5); // Purple
    public static TSMUI.Color Olive = new TSMUI.Color(0.5, 0.5, 0.0); // Olive
    public static TSMUI.Color Teal = new TSMUI.Color(0.0, 0.5, 0.5); // Teal
    public static TSMUI.Color Gray = new TSMUI.Color(0.5, 0.5, 0.5); // Gray   

    public static Dictionary<string, TSMUI.Color> SKColorDictionary = new Dictionary<string, TSMUI.Color>
    {
        { "Red", Red },
        { "Green", Green },
        { "Yellow", Yellow },
        { "Blue", Blue },
        { "Magenta", Magenta },
        { "Cyan", Cyan },
        { "Purple", Purple },
        { "Olive", Olive },
        { "Teal", Teal },
        { "Gray", Gray }
    };
    public class CutData : IEquatable<CutData>
    {
        public string id { get; set; }
        public List<string> UserAttributes { get; set; }
        public double Tolerance { get; set; }
        public double Distance { get; set; }
        public TSG.Point Location { get; set; }

        public bool Equals(CutData Compare)
        {
            return IsEquals(this.Location, Compare.Location, Tolerance, this.Distance, Compare.Distance);
        }
    }


    public class ShopBoltData : IEquatable<ShopBoltData>
    {
        public string id { get; set; }
        public List<string> UserAttributes { get; set; }
        public double Tolerance { get; set; }
        public double Distance { get; set; }
        public double HoleSize { get; set; }
        public TSG.Point Location { get; set; }
        public bool Equals(ShopBoltData Compare)
        {
            return IsEquals(this.Location, Compare.Location, Tolerance, this.Distance, Compare.Distance, this.HoleSize, Compare.HoleSize);
        }
    }

    public class WeldData : IEquatable<WeldData>
    {
        public string id { get; set; }
        public List<string> UserAttributes { get; set; }
        public double Tolerance { get; set; }
        //public double Distance { get; set; }
        public int WeldType { get; set; }
        public double WeldSize { get; set; }
        public TSG.Point Location { get; set; }

        public bool Equals(WeldData Compare)
        {
            return IsEquals(this.Location, Compare.Location, Tolerance, 0, 0, this.WeldType, Compare.WeldType, this.WeldSize, Compare.WeldSize);
        }
    }

    public static bool IsEquals(TSG.Point Location1, TSG.Point Location2, double Tolerance = 0, double CheckValue1a = 0, double CheckValue1b = 0, double CheckValue2a = 0, double CheckValue2b = 0, double CheckValue3a = 0, double CheckValue3b = 0)
    {
        bool result = true;


        //check CheckValue1a & 1b
        result = Math.Abs(CheckValue1a - CheckValue1b) - AssemblyCOGDifference > Tolerance;
        if (result == true) return false;

        //check CheckValue2a & 2b
        result = Math.Abs(CheckValue2a - CheckValue2b) > Tolerance;
        if (result == true) return false;

        //check CheckValue3a & 3b
        result = Math.Abs(CheckValue3a - CheckValue3b) > Tolerance;
        if (result == true) return false;

        //check for location
        if (Location1 != null && Location2 != null)
        {
            result = Math.Abs(TSG.Distance.PointToPoint(Location1, Location2)) - AssemblyCOGDifference > Tolerance;
            if (result == true) return false;
            return true;
        }
        else
            return false;

    }
    public class ConnectionData
    {
        public string UniqueID { get; set; }
        public ArrayList List { get; set; }
    }

    public static Dictionary<string, ConnectionData> ReadConnectionCSVFile(string File, string project, DataGridView MyDataGridView, int uidno)
    {
        Dictionary<string, ConnectionData> MyConnectionData = new Dictionary<string, ConnectionData>();
        //To avoid duplicate unqiue data check whether same item already exists if not then add
        ArrayList MyUnqList = new ArrayList();
        try
        {
            if (System.IO.File.Exists(File) == true)
            {
                using (StreamReader sr = new StreamReader(File))
                {
                    //store column name in datatable
                    System.Data.DataTable MyDataTable = new System.Data.DataTable();
                    MyDataTable.Columns.Clear();

                    //Project,Att.Number,File,Secondary,Primary,Connection Configuration, EndCode,"Shear, Vy(major)","Moment, Mz(major)","Tension, Nt","Compression, Nc","Shear, Vz(minor)","Moment, My(minor)","Tension, Mx",Connection Capacity, UDL code,Conn.Utility Ration, skReserved1, skReserved2, skReserved3, skReserved4, skReserved5, ShearTabThk, number of bolts, Spacing between bolts, Material Name,ShearTabSide
                    //XS_Variable,,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,tpl1,nb,lbd,mat,btab5
                    //XS_Type,,,,,,,,,,,,,,,,,,,,,,double,int,double,string,int
                    //SK1785,146,CSP05 - W14 - W24,W12X16,W14X109,Beam to Column Flange,28.71; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0,,,,,,,,,,,,,,,,NA,3,,,
                    //SK1785,146,CSP05 - W14 - W24,W12X16,W14X283,Beam to Column Flange,28.71; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0,,,,,,,,,,,,,,,,NA,3,,,

                    string line = string.Empty;
                    string[] highlvldata = null;
                    string[] XS_ConnFieldName = null;
                    string[] XS_ConnDataType = null;
                    int lct = 0;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] spltdata = line.Split(new Char[] { ',' });
                        if (lct == 0)
                            highlvldata = spltdata;
                        else if (lct == 1)
                        {
                            XS_ConnFieldName = spltdata;                            
                            for (int i = 0; i < XS_ConnFieldName.Length; ++i)
                            {
                                DataColumn MyDataColumn = new DataColumn();
                                MyDataColumn.ColumnName = (i + 1).ToString() + "-" + XS_ConnFieldName[i];
                                MyDataTable.Columns.Add(MyDataColumn);
                            }
                            //clear columns
                            MyDataGridView.Columns.Clear();
                        }
     
                        else if (lct == 2)
                            XS_ConnDataType = spltdata;
                        else if (lct >= 3)
                        {
                            if (spltdata.Length >= 3)
                            {
                                string uid = spltdata[0] + "|" + spltdata[1] + "|" + spltdata[2] + "|" + spltdata[3];
                                if (uidno == 7 && spltdata.Length >= 7)
                                    uid = uid + "|" + spltdata[4] + "|" + spltdata[5] + "|" + spltdata[6];

                                //Project	+ "|" + Att. Number + "|" + 	File + "|" + 	Secondary + "|" + 	Primary	Connection + "|" +  Configuration	+ "|" +  EndCode
                                //check whether same data exists if "yes" don't add these datas
                                if (MyUnqList.Contains(uid) == false)
                                {
                                    ArrayList ConnExtData = new ArrayList();
                                    bool flag = false;
                                    for (int i = uidno; i < spltdata.Length; i++)
                                    {
                                        string csvcellvalue = spltdata[i];
                                        if (csvcellvalue.Trim().Length >= 1)
                                        {
                                            if (csvcellvalue.ToUpper() != "NA" && csvcellvalue.ToUpper() != "-" && csvcellvalue.ToUpper() != "N/A" && csvcellvalue.ToUpper() != "NIL")
                                            {
                                                //ArrayList tmp = new ArrayList();
                                                //tmp.Add(XS_ConnFieldName[i].ToString() + "|" + XS_ConnDataType[i].ToString() + "|" + csvcellvalue.Trim());
                                                //ConnExtData.Add(tmp);

                                                ConnExtData.Add(XS_ConnFieldName[i].ToString() + "|" + XS_ConnDataType[i].ToString() + "|" + csvcellvalue.Trim());
                                                flag = true;
                                            }
                                        }
                                    }

                                    if (flag == true)
                                    {

                                        ConnectionData EachLineData = new ConnectionData();
                                        EachLineData.UniqueID = uid;
                                        EachLineData.List = ConnExtData;
                                        MyConnectionData.Add(uid, EachLineData);

                                        DataRow row = MyDataTable.NewRow();
                                        row.ItemArray = spltdata;
                                        MyDataTable.Rows.Add(row);
                                        // store uid for avoiding duplicate entry
                                        MyUnqList.Add(uid);

                                    }
                                }
                            }
                        }
                        lct++;

                    }
                    sr.Close();
                    MyDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically; 
                    MyDataGridView.DataSource = MyDataTable;
                    MyDataGridView.Refresh();
                }


            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Actual Error:" + ex.Message, "Error @ getAttributeData");
        }
        return MyConnectionData;

    }
    public static string ImportCustomComponent(string CCFolder)
    {
        TSM.Model MyModel = new TSM.Model();
        if (MyModel.GetConnectionStatus() == true)
        {

            if (System.IO.Directory.Exists(CCFolder) == true)
            {
                new CatalogHandler().ImportCustomComponentItems(CCFolder);
                return "CustomComponent Imported.";
            }
            else
            {
                return CCFolder + " Not Exist!!!";
            }
        }
        else
        {
            return  "GetConnectionStatus Failed.";
        }
    }
    public static double ConvertImperialtoMetric(string ImperialStringValue)
    {
        if (ImperialStringValue.Trim().Length > 0)
        {
            if (ImperialStringValue.IndexOf("'") <= -1)
            {
                if (ImperialStringValue.Substring(0, 1) == "-")
                    ImperialStringValue = "-0'-" + ImperialStringValue.Substring(1);
                else
                    ImperialStringValue = "0'-" + ImperialStringValue;
            }

            Tekla.Structures.Datatype.Distance mydist = Tekla.Structures.Datatype.Distance.FromFractionalFeetAndInchesString(ImperialStringValue, CultureInfo.InvariantCulture, Tekla.Structures.Datatype.Distance.UnitType.Inch);
            return mydist.Millimeters;
        }
        else
            return 0.0;
    }
    public static bool CreateSK_Automation()
    {
        string skpath = skTSLib.ModelPath + "\\SK_Automation\\";
        if (!Directory.Exists(skpath))
        {
            Directory.CreateDirectory(skpath);
            return true;
        }
        return false;
    }


    public static ArrayList SortProfileDepth(ArrayList Secondaries)
    {
        ArrayList myProfile = new ArrayList();
        ArrayList s_depth = new ArrayList();
        ArrayList lSecondaries = Secondaries;
        double chkprofiledepth = 0;

        foreach (Beam mySecondary in lSecondaries)
        {
            if (mySecondary != null)
            {
                mySecondary.GetReportProperty("HEIGHT", ref chkprofiledepth);
                s_depth.Add(chkprofiledepth);
            }
        }
        s_depth.Sort();
        foreach (double myDepth in s_depth)
        {
            int ct = lSecondaries.Count;
            for (int i = 0; i < ct; i++)
            {
                Beam mySecondary = lSecondaries[i] as Beam;
                if (mySecondary != null)
                {
                    mySecondary.GetReportProperty("HEIGHT", ref chkprofiledepth);
                    if (myDepth == chkprofiledepth)
                    {
                        myProfile.Add(mySecondary);
                        lSecondaries.RemoveAt(i);
                        goto skipSecondaryDepth;
                    }
                }
            }

        skipSecondaryDepth:;
        }

        return myProfile;
    }
    public static bool IsNumeric(object Expression)
    {
        double retNum;

        bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        return isNum;
    }


    public static TSMUI.View GetModelActiveView()
    {
        TSMUI.ViewHandler ViewHandler = new TSMUI.ViewHandler();
        TSMUI.ModelViewEnumerator ViewEnum = TSMUI.ViewHandler.GetVisibleViews();

        string activeView = GetModelActiveViewTitle();
        activeView = activeView.Substring(activeView.IndexOf('-') + 2);

        while (ViewEnum.MoveNext())
        {
            TSMUI.View ViewSel = ViewEnum.Current;
            if (ViewSel.Name == activeView)
            {
                return ViewSel;
            }
        }
        return null;
    }

    public static string GetModelActiveViewTitle()
    {
        IntPtr mainWindowHandle = TSDLG.MainWindow.Frame.Handle;
        IntPtr fakeWindow = FindWindowEx(mainWindowHandle, IntPtr.Zero, null, ""); // "MyFakeWindow1"
        IntPtr windowBetween = FindWindowEx(fakeWindow, IntPtr.Zero, null, null); // "AfxMDIFrame1x0"
        IntPtr mdiClient = FindWindowEx(windowBetween, IntPtr.Zero, "MDIClient", null);

        IntPtr onTop = GetTopWindow(mdiClient);

        int max_length = GetWindowTextLength(onTop);
        StringBuilder windowText = new StringBuilder("", max_length + 5);
        GetWindowText(onTop, windowText, max_length + 2);
        return windowText.ToString();
    }

    //public static ArrayList UpdateAttributeData_Profile(string MProject, Dictionary<string, ConnectionData> MyPriSec_Phas_Data, string sConnData, string MPrimaryProfile, string MSecondaryProfile,  ArrayList AttributesList)
    //{

    //    ArrayList MyReturnData = AttributesList;
    //    //Add Additional Data based on Secondary & Primary Profile
    //    //Condition1 for Secondary Profile with Blank Primary Profile
    //    string uid = MProject + "|" + sConnData + "|" + MSecondaryProfile + "|";
    //    skTSLib.ConnectionData Profile_Data = new skTSLib.ConnectionData();
    //    MyPriSec_Prof_Data.TryGetValue(key: uid, out Profile_Data);
    //    if (Profile_Data == null)
    //    {
    //        //Condition2 for Secondary Profile & All Condition
    //        uid = MProject + "|" + MComponent + "|" + MSecondaryProfile + "|All";
    //        MyPriSec_Prof_Data.TryGetValue(key: uid, out Profile_Data);
    //    }
    //    if (Profile_Data == null)
    //    {
    //        //Condition3 for both Secondary Profile & Primary Profile
    //        uid = MProject + "|" + MComponent + "|" + MSecondaryProfile + "|" + MPrimaryProfile;
    //        MyPriSec_Prof_Data.TryGetValue(key: uid, out Profile_Data);
    //    }
    //    if (Profile_Data != null)
    //    {
    //        foreach (string mydata in Profile_Data.List)
    //        {
    //            MyReturnData.Add(mydata);
    //        }
    //    }

    //    return MyReturnData;
    //}
    public static ArrayList UpdateAttributeData_Profile(string MProject, Dictionary<string, ConnectionData> MyProfile_Data, string sConnData, string PrimaryProfile, string SecondaryProfiles, ArrayList AttributesList)
    {
        ArrayList MyReturnData = AttributesList;
        if (MyProfile_Data != null)
        {
            //Condition1 for PrimaryProfile & Blank
            string uid = MProject + "|" + sConnData + "|" + PrimaryProfile + "|";
            ConnectionData Profile_Data = new ConnectionData();
            MyProfile_Data.TryGetValue(key: uid, out Profile_Data);
            if (Profile_Data == null)
            {
                //Condition2 for PrimaryProfile & All Condition
                uid = MProject + "|" + sConnData + "|" + PrimaryProfile + "|All";
                MyProfile_Data.TryGetValue(key: uid, out Profile_Data);
            }
            if (Profile_Data == null)
            {
                //Condition2 for PrimaryProfile and SecondaryProfiles
                uid = MProject + "|" + sConnData + "|" + PrimaryProfile + "|" + SecondaryProfiles;
                MyProfile_Data.TryGetValue(key: uid, out Profile_Data);
            }
            if (Profile_Data != null)
            {
                foreach (string mydata in Profile_Data.List)
                {
                    MyReturnData.Add(mydata);
                }
            }
        }
        

        return MyReturnData;
    }

    public static ArrayList UpdateAttributeData_Phase(string MProject, Dictionary<string, ConnectionData> MyPhase_Data, string sConnData, int PrimaryPhase, string SecondaryPhases, ArrayList AttributesList)
    {
        ArrayList MyReturnData = AttributesList;
        if (MyPhase_Data != null)
        {
            //Condition1 for PrimaryPhase & Blank
            string uid = MProject + "|" + sConnData + "|" + PrimaryPhase + "|";
            ConnectionData Phase_Data = new ConnectionData();
            MyPhase_Data.TryGetValue(key: uid, out Phase_Data);
            if (Phase_Data == null)
            {
                //Condition2 for PrimaryPhase & All Condition
                uid = MProject + "|" + sConnData + "|" + PrimaryPhase + "|All";
                MyPhase_Data.TryGetValue(key: uid, out Phase_Data);
            }
            if (Phase_Data == null)
            {
                //Condition2 for PrimaryPhase and SecondaryPhase
                uid = MProject + "|" + sConnData + "|" + PrimaryPhase + "|" + SecondaryPhases;
                MyPhase_Data.TryGetValue(key: uid, out Phase_Data);
            }
            if (Phase_Data != null)
            {
                foreach (string mydata in Phase_Data.List)
                {
                    MyReturnData.Add(mydata);
                }
            }
        }
        

        return MyReturnData;
    }
    //public static ArrayList UpdateAttributeData_Phase(string MProject, Dictionary<string, ConnectionData> MyPriSec_Phas_Data, string sConnData, int PrimaryPhase, string SecondaryPhase, ArrayList AttributesList)
    //{
    //    ArrayList MyReturnData = AttributesList;
    //    //Check for Primary Phase & Secondary Phase
    //    string uid = MProject + "|" + sConnData + "|" + SecondaryPhase + "|" + PrimaryPhase;
    //    ConnectionData Phase_Data = new ConnectionData();

    //    MyPriSec_Phas_Data.TryGetValue(key: uid, out Phase_Data);
    //    if (Phase_Data != null)
    //    {
    //        foreach (string mydata in Phase_Data.List)
    //        {
    //            MyReturnData.Add(mydata);
    //        }
    //    }


    //    return MyReturnData;
    //}

    public static int InsertConnection(string MProject, Dictionary<string, ConnectionData> MyProfile_Data, Dictionary<string, ConnectionData> MyPhase_Data, string sConnData, string sAttrFile, TSM.ModelObject Primary, ArrayList Secondaries, ArrayList AttributesList, string ComponentClass, bool SortSecondaries, bool Refresh)
    {
        //Check Connection Exists
        //bool chkattfileflag = checkAttributeFile(ModelPath, sAttrFile, sConnData);
        if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, sAttrFile, sConnData) == false)
            return 0;
        if (Refresh == true)
        {
            //Primary.Select();

            ArrayList ObjectsToSelect = new ArrayList();
            ObjectsToSelect.Add(Primary);
            var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
            MOS.Select(ObjectsToSelect);

            TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
            TSM.UI.ModelViewEnumerator visibleViews = TSM.UI.ViewHandler.GetVisibleViews();
            while (visibleViews.MoveNext())
            {
                Tekla.Structures.Model.UI.View visibleView = visibleViews.Current;
                TSM.UI.ViewHandler.RedrawView(visibleView);
            }
        }

        int iApplied = 0;
        bool flag = false;
        string log = string.Empty;
        try
        {

            TSM.Connection MyConnection = new TSM.Connection();
            if (IsNumeric(sConnData) == true)
            {
                MyConnection.Name = sAttrFile;
                MyConnection.Number = Convert.ToInt32(sConnData);
                log = log + "T1";
            }
            else
            {
                MyConnection.Name = sConnData.Replace("p_", "").Replace("P_", "");
                MyConnection.Number = -1;
                log = log + "T2";
            }

            MyConnection.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
            //MyConnection.PositionType = PositionTypeEnum.COLLISION_PLANE;


            flag = MyConnection.SetPrimaryObject(Primary);
            if (flag == true)
                log = log + "PR1";
            else
                log = log + "PR0";

            if (SortSecondaries == true)
            {
                ArrayList sortSecondaries = SortProfileDepth(Secondaries);
                flag = MyConnection.SetSecondaryObjects(sortSecondaries);
            }
            else
            {
                flag = MyConnection.SetSecondaryObjects(Secondaries);
            }
            if (flag == true)
                log = log + "SE1";
            else
                log = log + "SE0";

            flag = MyConnection.LoadAttributesFromFile(sAttrFile);
            if (flag == true)
                log = log + "LF1";
            else
                log = log + "LF0";


            if (skWinLib.IsNumeric(ComponentClass) == true)
            {
                log = log + "CL1";
                MyConnection.Class = Convert.ToInt32(ComponentClass);
            }
            else
            {
                MyConnection.Class = 13;
                log = log + "CL0";
            }



            MyConnection.Code = sAttrFile;
            //flag = MyConnection.Modify();
            //if (flag == true)
            //    log = log + "CM1";
            //else
            //    log = log + "CM0";
            //get primary profile 
            string PrimaryProfile = string.Empty;
            Primary.GetReportProperty("PROFILE", ref PrimaryProfile);

            string mprof = GetMetricProfile(PrimaryProfile);

            //get primary phase 
            Primary.GetPhase(out TSM.Phase PriPh);
            int PriPhase = PriPh.PhaseNumber;
            log = log + "PRPH" + PriPhase;

            string SecProfiles = string.Empty;
            string SecPhases = string.Empty;
            //get secondary profile & phase
            if (Secondaries.Count >= 1)
            {
                SecProfiles = "|";
                SecPhases = "|";
                foreach (TSM.ModelObject MySecondary in Secondaries)
                {
                    TSM.ModelObject Secondary = MySecondary as TSM.ModelObject;
                    if (Secondary != null)
                    {
                        //get secondary profile 
                        string SecondaryProfile = string.Empty;
                        Secondary.GetReportProperty("PROFILE", ref SecondaryProfile);

                        SecProfiles = SecProfiles + "|" + SecondaryProfile;
                        Secondary.GetPhase(out TSM.Phase Sec1Ph);
                        SecPhases = SecPhases + "|" + Sec1Ph.PhaseNumber;
                    }


                }
                SecProfiles = SecProfiles.Replace("||", "");
                SecPhases = SecPhases.Replace("||", "");


            }
            log = log + "SEPH" + SecPhases;
            //check additional connection attribute exists if yes update and change the color
            if (AttributesList.Count >= 1)
                log = log + "MA" + AttributesList.Count;
            //Add Attribute based on profile
            AttributesList = UpdateAttributeData_Profile(MProject, MyProfile_Data, sConnData, PrimaryProfile, SecProfiles, AttributesList);
            if (AttributesList.Count >= 1)
                log = log + "MPR" + AttributesList.Count;
            //Add Attribute based on phase
            AttributesList = UpdateAttributeData_Phase(MProject, MyPhase_Data, sConnData, PriPhase, SecPhases, AttributesList);
            if (AttributesList.Count >= 1)
                log = log + "MPR" + AttributesList.Count;


            string conncode = string.Empty;
            if (AttributesList.Count >= 1)
                flag = UpdateConnectionAttribute(MyConnection, AttributesList, out conncode);
            if (flag == true)
                log = log + "UD1";
            else
                log = log + "UD0";

            if (flag == true)
                MyConnection.Class = 90;

            if (conncode != string.Empty)
            {
                MyConnection.Code = conncode;
                MyConnection.Class = 76;
            }
   

            //update Secondaries AttributesList
            //UpdateAttributeData_Profile()

            //MyConnection.Code = conncode;
            //MyConnection.Name = conncode;

            flag = MyConnection.Insert();
            //For Updation of connection Code
            if (flag == true)
            {
                iApplied = 1;
                log = log + "DS100";
                //if (conncode != string.Empty)
                //{

                //    //For Updation of connection Code              
                //    flag = UpdateConnectionCode(MyConnection.Identifier, conncode);
                //    if (flag == true)
                //        log = log + "DS111";
                //    else
                //        log = log + "DS110";
                //    MyConnection.Class = 76;
                //}
                //else
                //    log = log + "DS100";

            }
            else
                log = log + "DS000";
            Console.WriteLine(log);

            if (Refresh == true)
            {
                MyConnection.Select();                
                flag = MyConnection.Modify();
                if (flag == true)
                    log = log + "CM1";
                else
                    log = log + "CM0";

                TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
            }


            //if (flag == true)
            //    iApplied = 1;
            //iApplied++;

            //if (MyConnection.Number == 11)
            //{
            //    Gussets.Add(MyConnection);
            //}

        }  //end of try loop

        catch (Exception ex)
        {
            MessageBox.Show("Original error:Insert Connections @ " + sConnData + " " + sAttrFile + " " + ex.Message);
        }
        return iApplied;

    }
    //public static int InsertConnection(string MProject, Dictionary<string, ConnectionData> MyProfile_Data, Dictionary<string, ConnectionData> MyPhase_Data,string sConnData, string sAttrFile, TSM.ModelObject Primary, ArrayList Secondaries, ArrayList AttributesList,string ComponentClass, bool SortSecondaries, bool Refresh)
    //{
    //    //Check Connection Exists
    //    //bool chkattfileflag = checkAttributeFile(ModelPath, sAttrFile, sConnData);
    //    if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, sAttrFile, sConnData) == false)
    //        return 0;
    //    if (Refresh == true)
    //    {
    //        //Primary.Select();

    //        ArrayList ObjectsToSelect = new ArrayList();
    //        ObjectsToSelect.Add(Primary);
    //        var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
    //        MOS.Select(ObjectsToSelect);

    //        TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
    //        TSM.UI.ModelViewEnumerator visibleViews = TSM.UI.ViewHandler.GetVisibleViews();
    //        while (visibleViews.MoveNext())
    //        {
    //            Tekla.Structures.Model.UI.View visibleView = visibleViews.Current;
    //            TSM.UI.ViewHandler.RedrawView(visibleView);
    //        }
    //    }
    //    int iApplied = 0;
    //    try
    //    {

    //        TSM.Connection MyConnection = new TSM.Connection();
    //        if (IsNumeric(sConnData) == true)
    //        {

    //            MyConnection.Number = Convert.ToInt32(sConnData);
    //        }
    //        else
    //        {
    //            MyConnection.Name = sConnData.Replace("p_", "").Replace("P_", "");
    //            MyConnection.Number = -1;
    //        }
    //        string thisname = MyConnection.Name;
    //        MyConnection.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
    //        //MyConnection.PositionType = PositionTypeEnum.COLLISION_PLANE;

    //        MyConnection.SetPrimaryObject(Primary);
    //        if (SortSecondaries == true)
    //        {
    //            ArrayList sortSecondaries = SortProfileDepth(Secondaries);
    //            MyConnection.SetSecondaryObjects(sortSecondaries);
    //        }
    //        else
    //        {
    //            MyConnection.SetSecondaryObjects(Secondaries);
    //        }

    //        bool laf = MyConnection.LoadAttributesFromFile(sAttrFile);

    //        MyConnection.Class = Convert.ToInt32(ComponentClass);
    //        MyConnection.Code = sAttrFile;

    //        //get primary profile 
    //        string PrimaryProfile = string.Empty;
    //        Primary.GetReportProperty("PROFILE", ref PrimaryProfile);

    //        string mprof = GetMetricProfile(PrimaryProfile);

    //        //get primary phase 
    //        Primary.GetPhase(out Phase PriPh);
    //        int PriPhase = PriPh.PhaseNumber;

    //        string SecProfiles = string.Empty;
    //        string SecPhases = string.Empty;
    //        //get secondary profile & phase
    //        if (Secondaries.Count >= 1)
    //        {
    //            SecProfiles = "|";
    //            SecPhases = "|";
    //            foreach (TSM.ModelObject MySecondary in Secondaries)
    //            {
    //                TSM.ModelObject Secondary = MySecondary as TSM.ModelObject;
    //                if (Secondary != null)
    //                {
    //                    //get secondary profile 
    //                    string SecondaryProfile = string.Empty;
    //                    Secondary.GetReportProperty("PROFILE", ref SecondaryProfile);

    //                    SecProfiles = SecProfiles + "|" + SecondaryProfile;
    //                    Secondary.GetPhase(out Phase Sec1Ph);
    //                    SecPhases = SecPhases + "|" + Sec1Ph.PhaseNumber;
    //                }


    //            }
    //            SecProfiles = SecProfiles.Replace("||", "");
    //            SecPhases = SecPhases.Replace("||", "");


    //        }
    //        //Add Attribute based on profile
    //        AttributesList = UpdateAttributeData_Profile(MProject, MyProfile_Data, sConnData, PrimaryProfile, SecProfiles, AttributesList);

    //        //Add Attribute based on phase
    //        AttributesList = UpdateAttributeData_Phase(MProject, MyPhase_Data, sConnData, PriPhase, SecPhases, AttributesList);

    //        string conncode = string.Empty;
    //        //check additional connection attribute exists if yes update and change the color
    //        if (AttributesList != null)
    //        {
    //            bool attributeflag = UpdateConnectionAttribute(MyConnection, AttributesList,out conncode);
    //            MyConnection.Class = 9;
    //            //MyConnection.Modify();
    //        }




    //        //update Secondaries AttributesList
    //        //UpdateAttributeData_Profile()
    //        MyConnection.Code = conncode;
    //        MyConnection.Name = conncode;

    //        bool insflag = MyConnection.Insert();
    //        //For Updation of connection Code
    //        if (insflag == true && conncode != string.Empty)
    //        {
    //            //MyConnection.Code = conncode;
    //            //MyConnection.Name = conncode;
    //            bool flag = UpdateConnectionCode(MyConnection.Identifier, conncode);
    //        }
    //        //if (AttributesList != null)
    //        //{
    //        //    if (insflag == true)
    //        //    {
    //        //        bool attributeflag = skTSLib.UpdateConnectionAttribute(MyConnection, AttributesList);
    //        //        MyConnection.Modify();
    //        //    }

    //        //}

    //        if (Refresh == true)
    //        {
    //            MyConnection.Select();
    //            TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
    //        }


    //        if (insflag == true)
    //            iApplied = 1;
    //        //iApplied++;

    //        //if (MyConnection.Number == 11)
    //        //{
    //        //    Gussets.Add(MyConnection);
    //        //}

    //    }  //end of try loop

    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Original error:Insert Connections @ " + sConnData + " " + sAttrFile + " " + ex.Message);
    //    }
    //    return iApplied;

    //}

    //public static int InsertDetail(string MProject,Dictionary<string, ConnectionData> MyPhase_Data, string sConnData, string sAttrFile, TSM.ModelObject Primary, ArrayList AttributesList, string DetailClass, Tekla.Structures.Geometry3d.Point DetailPoint, bool Refresh)
    //{        
    //    return 0;
    //}

    public static bool IsCustomComponentExist(string CustomComponentName)
    {
        CatalogHandler MyCatHan = new CatalogHandler();
        ComponentItemEnumerator MyComponentItemEnumerator = MyCatHan.GetComponentItems();
        while (MyComponentItemEnumerator.MoveNext())
        {
            ComponentItem MyComponentItem = MyComponentItemEnumerator.Current as ComponentItem;
            if (MyComponentItem != null)
            {
                if (MyComponentItem.UIName.ToString().ToUpper().Contains(CustomComponentName.ToUpper().Replace("P_","")) == true)
                    return true;
            }

        }
        return false;
    }
    public static int InsertDetail(string MProject, Dictionary<string, ConnectionData> MyProfile_Data, Dictionary<string, ConnectionData> MyPhase_Data, string sConnData, string sAttrFile, TSM.ModelObject Primary, ArrayList AttributesList, string DetailClass, Tekla.Structures.Geometry3d.Point DetailPoint, bool Refresh)
    {
        //Check Connection Exists
        if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, sAttrFile, sConnData) == false)
            return 0;

        int iApplied = 0;
        bool flag = false;
        string log = string.Empty;
        bool custflag = false;
        try
        {
            Detail MyDetail = new Detail();
            if (sConnData.ToUpper().Substring(0, 2) == "P_")
            {
                custflag = true;
                log = log + "T2";
                MyDetail.Name = sConnData.Replace("p_", "").Replace("P_", "");
                MyDetail.Number = -1;
            }
            else
            {
                if (skWinLib.IsNumeric(sConnData) == true)
                {
                    MyDetail.Name = sAttrFile;
                    MyDetail.Number = Convert.ToInt32(sConnData);
                    log = log + "T1";
                }
                else
                    return -1;
       
            }
            //if (skWinLib.IsNumeric(sConnData) == true)
            //{
            //    MyDetail.Name = sAttrFile;
            //    MyDetail.Number = Convert.ToInt32(sConnData);
            //    log = log + "T1";
            //}
            //else
            //{
            //    log = log + "T2";
            //    MyDetail.Name = sConnData.Replace("p_", "").Replace("P_", "");
            //    MyDetail.Number = -1;
            //}
            //MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
            //MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.END;
            //MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE;

            MyDetail.UpVector = new Vector();
            MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_DETAIL;
            //MyDetail.PositionType = PositionTypeEnum.BOX_PLANE;
            //MyDetail.DetailType = DetailTypeEnum.INTERMEDIATE;
            MyDetail.Code = "";



            flag = MyDetail.SetPrimaryObject(Primary);
            if (flag == true)
                log = log + "PR1";
            else
                log = log + "PR0";
            flag = MyDetail.SetReferencePoint(DetailPoint);
            if (flag == true)
                log = log + "RP1";
            else
                log = log + "RP0";
            if ((custflag == false && sAttrFile.Length >= 1) || (custflag == true && sAttrFile != "-1"))
            {
                flag = MyDetail.LoadAttributesFromFile(sAttrFile);
                MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
                if (flag == true)
                    log = log + "LF1";
                else
                    log = log + "LF0";

            }

            if (skWinLib.IsNumeric(DetailClass) == true)
            {
                log = log + "CL1";
                MyDetail.Class = Convert.ToInt32(DetailClass);
            }
            else
            {
                MyDetail.Class = 13;
                 log = log + "CL0";
            }
            if (custflag == true)
            {
                if (sAttrFile == "-1")
                    MyDetail.Code = sConnData;
                else
                    MyDetail.Code = sAttrFile;
            }
      
            else
                MyDetail.Code = sAttrFile;

            flag = MyDetail.Insert();

            //flag = MyDetail.Modify();
            if (flag == true)                
            {
                iApplied = 1;
                log = log + "DM1";
                //get primary profile 
                string PrimaryProfile = string.Empty;
                Primary.GetReportProperty("PROFILE", ref PrimaryProfile);

                //get primary phase 
                Primary.GetPhase(out TSM.Phase PriPh);
                int PriPhase = PriPh.PhaseNumber;
                log = log + "PH" + PriPhase;

                if (AttributesList.Count >= 1)
                    log = log + "MA" + AttributesList.Count;
                //Add Attribute based on profile
                AttributesList = UpdateAttributeData_Profile(MProject, MyProfile_Data, sConnData, PrimaryProfile, string.Empty, AttributesList);
                if (AttributesList.Count >= 1)
                    log = log + "MPR" + AttributesList.Count;
                //Add Attribute based on phase
                AttributesList = UpdateAttributeData_Phase(MProject, MyPhase_Data, sConnData, PriPhase, string.Empty, AttributesList);
                if (AttributesList.Count >= 1)
                    log = log + "MPH" + AttributesList.Count;
                string conncode = string.Empty;
                if (AttributesList.Count >= 1)
                    flag = UpdateDetailAttribute(MyDetail, AttributesList, out conncode);
                if (flag == true)
                    log = log + "UD1";
                else
                    log = log + "UD0";
                MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE;


                if (flag == true)
                    MyDetail.Class = 90;

                if (conncode != string.Empty)
                {
                    MyDetail.Code = conncode;
                    MyDetail.Class = 76;
                }

                flag = MyDetail.Modify();
                if (flag == true)
                {
                    iApplied = 1;
                    log = log + "DS100";
                    //if (conncode != string.Empty)
                    //{
                    //    //For Updation of connection Code              
                    //    flag = UpdateConnectionCode(MyDetail.Identifier, conncode);
                    //    if (flag == true)
                    //        log = log + "DS111";
                    //    else
                    //        log = log + "DS110";
                    //    MyDetail.Class = 76;
                    //}
                    //else
                    //    log = log + "DS100";

                }
                else
                    log = log + "DS000";
                Console.WriteLine(log);
            }
            else
                log = log + "DM0";
            
            //skWinLib.accesslog(skApplicationName, skapplicationVersion, Task, "Start ;" + Remark, ModelName, Version, Configuration);
        } 

        catch (Exception ex)
        {
            Console.WriteLine("Log:" + log + "\nOriginal error: InsertDetail @ " + sConnData + " " + sAttrFile + " " + ex.Message);
            MessageBox.Show("Log:" + log + "\nOriginal error: InsertDetail @ " + sConnData + " " + sAttrFile + " " + ex.Message);
        }
        return iApplied;
    }
    //public static int InsertDetail(string MProject, Dictionary<string, ConnectionData> MyProfile_Data, Dictionary<string, ConnectionData> MyPhase_Data, string sConnData, string sAttrFile, TSM.ModelObject Primary, ArrayList AttributesList, string DetailClass,  Tekla.Structures.Geometry3d.Point DetailPoint, bool Refresh)
    // {
    //    //Check Connection Exists
    //    //bool chkattfileflag = checkAttributeFile(ModelPath, sAttrFile, sConnData);
    //    if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, sAttrFile, sConnData) == false)
    //        return 0;

    //    int iApplied = 0;
    //    try
    //    {
    //        Detail MyDetail = new Detail();
    //        if (skWinLib.IsNumeric(sConnData) == true)
    //        {
    //            MyDetail.Number = Convert.ToInt32(sConnData);
    //        }
    //        else
    //        {
    //            MyDetail.Name = sConnData.Replace("p_","").Replace("P_", "");
    //            MyDetail.Number = -1;
    //        }
    //        MyDetail.UpVector = new Vector(0, 0, 0);
    //        MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_DETAIL;
    //        MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.END;
    //        //MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE; 

    //        MyDetail.SetPrimaryObject(Primary);
    //        MyDetail.SetReferencePoint(DetailPoint);
    //        bool lafile = MyDetail.LoadAttributesFromFile(sAttrFile);

    //        if (skWinLib.IsNumeric(DetailClass) == true)
    //        {
    //            MyDetail.Class = Convert.ToInt32(DetailClass);
    //        }
    //        MyDetail.Code = sAttrFile;

    //        // MyDetail.DetailType = DetailTypeEnum.END;               
    //        // this format is changed for osha hole 
    //        // skTSLib.UpdateConnectionAttribute(MyDetail, AttributeList);


    //        //get primary profile 
    //        string PrimaryProfile = string.Empty;
    //        Primary.GetReportProperty("PROFILE", ref PrimaryProfile);

    //        //get primary phase 
    //        Primary.GetPhase(out Phase PriPh);
    //        int PriPhase = PriPh.PhaseNumber;


    //        //Add Attribute based on profile
    //        AttributesList = UpdateAttributeData_Profile(MProject, MyProfile_Data, sConnData, PrimaryProfile, string.Empty, AttributesList);

    //        //Add Attribute based on phase
    //        AttributesList = UpdateAttributeData_Phase(MProject, MyPhase_Data, sConnData, PriPhase, string.Empty, AttributesList);


    //        ////get primary phase 
    //        //Primary.GetPhase(out Phase PriPh);
    //        //int PriPhase = PriPh.PhaseNumber;
    //        //string Sec1Phase = string.Empty;
    //        //AttributesList = UpdateAttributeData_Phase(MProject, MyPhase_Data, sConnData, PriPhase, Sec1Phase, AttributesList);


    //        string conncode = string.Empty;


    //        //check whether csv or xl connection attribute exists if yes update and change the color
    //        if (AttributesList != null && AttributesList.Count >=1)
    //        {
    //            bool attributeflag = UpdateDetailAttribute(MyDetail, AttributesList, out conncode);
    //            MyDetail.Class = 76;
    //            //MyConnection.Modify();
    //        }

    //        //MyDetail.Name = "BP";
    //        //MyDetail.Number = -1;
    //        MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE;

    //        bool insflag = MyDetail.Insert();
    //        //For Updation of connection Code
    //        if (insflag == true && conncode != string.Empty)
    //        {

    //            bool flag = UpdateConnectionCode(MyDetail.Identifier, conncode);
    //        }

    //        //skTSLib.Model.UpdateDetailAttribute(MyDetail, AttributeList);
    //        //MyDetail.Modify();

    //        if (insflag == true)
    //            iApplied = 1;
    //    }  //end of try loop

    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Original error: InsertDetail @ " + sConnData + " " + sAttrFile + " " + ex.Message);
    //    }
    //    return iApplied;
    //}
    //public static bool UpdateConnectionCode(Tekla.Structures.Identifier BaseComponentID, string ConnectionCode)
    //{
    //    if (BaseComponentID.IsValid() == true)
    //    {
    //        TSM.ModelObject MyModelObject = MyModel.SelectModelObject(BaseComponentID);
    //        ArrayList ObjectsToSelect = new ArrayList();
    //        Tekla.Structures.Model.UI.ModelObjectSelector MS = new Tekla.Structures.Model.UI.ModelObjectSelector();
    //        ObjectsToSelect.Add(MyModelObject);
    //        MS.Select(ObjectsToSelect);

    //        TSM.BaseComponent BaseComponentObject = MyModelObject as TSM.BaseComponent;
    //        if (BaseComponentObject != null)
    //        {
    //            int attno = BaseComponentObject.Number;
    //            string jntcode = "joint_" + attno;
    //            //establish connection for running macro
    //            //if (TeklaStructures.Connect())
    //            if (TeklaStructures.Connect() == true)
    //            {
    //                MacroBuilder akit = new MacroBuilder();
    //                akit.Callback("acmd_display_selected_object_dialog", "", "main_frame");
    //                akit.ValueChange(jntcode, "joint_code", ConnectionCode);
    //                akit.PushButton("modify_button", jntcode);
    //                akit.PushButton("OK_button", jntcode);
    //                akit.Run();
    //                return true;
    //            }



    //            //MacroBuilder
    //            //Tekla.Structures.MacroBuilder akit = new Tekla.Structures.MacroBuilder();


    //            //if (Selected_part.Checked)
    //            //{
    //            //    if (Tekla.Structures.TeklaStructures.Connect() == true)
    //            //    {
    //            //        akit.Callback("acmdMergeSelectedMarks", "", "View_10 window_1");
    //            //        akit.Run();
    //            //        akit.Callback("acmd_create_marks_selected", "", "View_10 window_1");
    //            //        akit.Run();
    //            //        skTSLib.SKDrawing.SelectDrawingObjects(mytypes);

    //            //    }
    //            //}
    //        }
    //    }

    //    return false;    }

    //public static void Interrupt()
    //{
    //    //establish connection for running macro
    //    if (TeklaStructures.Connect() == true)
    //    {
    //        MacroBuilder akit = new MacroBuilder();
    //        akit.Callback("acmd_interrupt", "", "main_frame");
    //        akit.Callback("acmd_interrupt", "", "main_frame");

    //        //akit.Callback("acmd_interrupt", "", "View_01 window_1");
    //        //wpf.InvokeCommand("CommandRepository", "Misc.SelectionSwitches.SelectAll");
    //        akit.Callback("acmd_fit_workarea", "", "main_frame");
    //        akit.Callback("acmd_redraw_selected_view", "", "main_frame");
    //        akit.Run();
    //    }

        
    //}
    public static bool UpdateConnectionAttribute(Tekla.Structures.Model.BaseComponent Component, ArrayList AttributeList, out string connectioncode)
    {
        bool flag = false;
        string outconncode = string.Empty;
        try
        {
            foreach (string Attribute in AttributeList)
            {
                if (Attribute.IndexOf("|") >= 0)
                {
                    string[] split = Attribute.Split(new Char[] { '|' });
                    if (split.Count() >= 2)
                    {
                        string attribute_fieldname = split[0] as string;
                        string attribute_fieldtype = split[1] as string;
                        string attribute_fieldvalue = split[2].ToString();

                        if (attribute_fieldname != null && attribute_fieldtype != null && attribute_fieldvalue != null && attribute_fieldname != string.Empty && attribute_fieldtype != string.Empty && attribute_fieldvalue != string.Empty)
                        //if (split[2] != "||" && split[2].Trim().Length >= 1)
                        {
                            string attribute_type = attribute_fieldtype.Trim().ToUpper(); //.Substring(0, 2)
                            if (attribute_type == "STRING")
                            {
                                if (attribute_fieldname.ToUpper().Trim() == "JOINT_CODE")
                                {
                                    outconncode = attribute_fieldvalue;
                                    //flag = true;
                                }
                                else
                                {
                                    Component.SetAttribute(attribute_fieldname, attribute_fieldvalue);
                                    flag = true;
                                }
                            }
                            else if (attribute_type == "INT")
                            {
                                Component.SetAttribute(attribute_fieldname, Convert.ToInt32(attribute_fieldvalue));
                                flag = true;
                            }
                            else if (attribute_type == "DOUBLE")
                            {
                                Component.SetAttribute(attribute_fieldname, Convert.ToDouble(attribute_fieldvalue));
                                flag = true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        }
        connectioncode = outconncode;
        return flag;
    }

    
    public static bool UpdateDetailAttribute(Tekla.Structures.Model.Detail MyDetail, ArrayList AttributeList, out string connectioncode)
    {
        bool flag = false;
        string outconncode = string.Empty;
        if (AttributeList != null)
        {
            try
            {
                for (int i = 0; i < AttributeList.Count; i++)
                {
                    ArrayList XSData = AttributeList[i] as ArrayList;
                    if (XSData.Count >= 4)
                    {

                        string attribute_fieldname = XSData[1] as string;
                        string attribute_fieldtype = XSData[2] as string;
                        string attribute_fieldvalue = XSData[3].ToString();


                        if (attribute_fieldname != null && attribute_fieldtype != null && attribute_fieldvalue != null)
                        {
                            string attribute_type = attribute_fieldtype.ToUpper().Trim(); //Substring(0, 2).
                            if (attribute_type == "STRING")
                            {
                                if (attribute_fieldname.ToUpper().Trim() == "JOINT_CODE")
                                {
                                    outconncode = attribute_fieldvalue;
                                }
                                else
                                {
                                    MyDetail.SetAttribute(attribute_fieldname, attribute_fieldvalue);
                                    flag = true;
                                }
             
                            }
                            else if (attribute_type == "INT")
                            {
                                if (skWinLib.IsNumeric(attribute_fieldvalue) == true)
                                {
                                    if (attribute_fieldname.ToUpper().Trim() == "DETAIL_TYPE")
                                    {
                                        int DetailTypeEnum = Convert.ToInt32(attribute_fieldvalue);
                                        if (DetailTypeEnum == 0)
                                            MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.END;
                                        else if (DetailTypeEnum == 1)
                                            MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.INTERMEDIATE;
                                        else if (DetailTypeEnum == 2)
                                            MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.INTERMEDIATE_REVERSE;
                                    }
                                    else
                                        MyDetail.SetAttribute(attribute_fieldname, Convert.ToInt32(attribute_fieldvalue));
                                    flag = true;
                                }
                            }
                            else if (attribute_type == "DOUBLE")
                            {
                                if (skWinLib.IsNumeric(attribute_fieldvalue) == true)
                                {
                                    MyDetail.SetAttribute(attribute_fieldname, Convert.ToDouble(attribute_fieldvalue));
                                    flag = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
            }
        }
        connectioncode = outconncode;
        return flag;
    }

    //public static bool checkAttributeFile(string skmodelpath, string attributefile, string componentnumber)
    //{
    //    bool flag = false;
    //    string sFolder = skmodelpath + "\\attributes";
    //    string sFile = attributefile + ".j" + componentnumber.ToString();

    //    string[] file_Paths = Directory.GetFiles(sFolder, sFile, SearchOption.TopDirectoryOnly);

    //    if (file_Paths.Count() >= 1)
    //        flag = true;
    //    //check for UEL Esskay_186.uel or A01_MICRON.p_Esskay_186
    //    if (componentnumber == "-1")
    //        flag = true;

    //    return flag;
    //}
    //public static TSM.Connection InsertConnection(string ConnName, int ConnNumber, string sAttrFile, TSM.ModelObject Primary, ArrayList Secondaries) //, ArrayList AttributesList
    //{
    //    TSM.Connection MyConnection = new TSM.Connection();
    //    try
    //    {

    //        MyConnection.Name = ConnName;
    //        MyConnection.Number = ConnNumber;

    //        MyConnection.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
    //        //MyConnection.PositionType = PositionTypeEnum.COLLISION_PLANE;

    //        MyConnection.SetPrimaryObject(Primary);
    //        MyConnection.SetSecondaryObjects(Secondaries);
    //        if (System.IO.File.Exists(sAttrFile) == true)
    //            MyConnection.LoadAttributesFromFile(sAttrFile);
    //        //MyConnection.Code = ConnName;
    //        bool flag = MyConnection.Insert();

    //        return MyConnection;

    //    }  //end of try loop

    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Original error:Insert Connections @ " + ConnName + " " + sAttrFile + " " + ex.Message);
    //    }
    //    return null;
    //}


    //public static int InsertDetail(Tekla.Structures.Geometry3d.Point DetailPoint, TSM.ModelObject Primary, string sConnData, string sAttrFile, string sClass, ArrayList AttributeList)
    //{
    //    int iApplied = 0;
    //    try
    //    {
    //        Detail MyDetail = new Detail();
    //        if (skWinLib.IsNumeric(sConnData) == true)
    //        {
    //            MyDetail.Number = Convert.ToInt32(sConnData);
    //        }
    //        else
    //        {
    //            MyDetail.Name = sConnData;
    //            MyDetail.Number = -1;
    //        }

    //        MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
    //        MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.END;
    //        // MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE; 

    //        MyDetail.SetPrimaryObject(Primary);
    //        MyDetail.SetReferencePoint(DetailPoint);
    //        MyDetail.LoadAttributesFromFile(sAttrFile);

    //        if (skWinLib.IsNumeric(sClass) == true)
    //        {
    //            MyDetail.Class = Convert.ToInt32(sClass);
    //        }
    //        MyDetail.Code = sAttrFile;

    //        // MyDetail.DetailType = DetailTypeEnum.END;               
    //        // this format is changed for osha hole 
    //        // skTSLib.UpdateConnectionAttribute(MyDetail, AttributeList);
    //        skTSLib.Model.UpdateDetailAttribute(MyDetail, AttributeList);

    //        bool flag = MyDetail.Insert();

    //        //skTSLib.Model.UpdateDetailAttribute(MyDetail, AttributeList);
    //        //MyDetail.Modify();

    //        if (flag == true)
    //            iApplied = 1;
    //    }  //end of try loop

    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Original error: InsertDetail @ " + sConnData + " " + sAttrFile + " " + ex.Message);
    //    }
    //    return iApplied;
    //}


    public class ModelAccess
    {
        private static Tekla.Structures.Model.Model m_ModelConnection = null;

        /// <summary>
        /// Checks if a model connection is established, and attempts to create one if needed.
        /// </summary>
        /// <returns>true if successfully connected to the  model, false otherwise</returns>
        private static bool ConnectedToModel
        {
            get
            {
                bool connection = false;
                ConnectToModel(out connection);
                return connection;
            }
        }
               

        /// <summary>
        /// Gets a Model connnection
        /// </summary>
        /// <returns>The Model or null if unable to connect</returns>
        
        public static Tekla.Structures.Model.Model ConnectToModel()
        {
            bool connection = false;
            return ConnectToModel(out connection);
        }

        /// <summary>
        /// Gets a model connection
        /// </summary>
        /// <param name="model">the Model connection</param>
        /// <returns>true on success, false otherwise</returns>
        
        public static bool ConnectToModel(out Tekla.Structures.Model.Model model)
        {
            bool connection = false;
            ConnectToModel(out connection);
            model = m_ModelConnection;
            return connection;
        }

        /// <summary>
        /// Gets a Model connection
        /// </summary>
        /// <param name="ConnectedToModel">set to true if a model connection was made, false otherwise.</param>
        /// <returns>The Model or null if unable to connect</returns>
        
        public static Tekla.Structures.Model.Model ConnectToModel(out bool ConnectedToModel)
        {
            ConnectedToModel = false;
            if (m_ModelConnection == null)
            {
                try
                {
                    m_ModelConnection = new Tekla.Structures.Model.Model();
                }
                catch
                {
                    ConnectedToModel = false;
                    m_ModelConnection = null;
                }
            }

            try
            {
                if (m_ModelConnection.GetConnectionStatus())
                {
                    if (m_ModelConnection.GetInfo().ModelName == "")
                    {
                        ConnectedToModel = false;
                        m_ModelConnection = null;
                    }
                    else
                    {
                        ConnectedToModel = true;
                        //TeklaStructuresSettings.GetAdvancedOption("XS_IMPERIAL", ref IsUnitImperial);
                        //ModelPath = MyModel.GetInfo().ModelPath;
                        //ParentPath = System.IO.Directory.GetParent(ModelPath).FullName;
                        //ModelName = MyModel.GetInfo().ModelName.Replace(".db1", "").Replace(".DB1", "");
                        //ProjectNumber = MyModel.GetProjectInfo().ProjectNumber;
                        //SKProjectNumber = MyModel.GetProjectInfo().Info1;
                        //SKProjectInfo2 = MyModel.GetProjectInfo().Info2;
                        //IsSharedmodel = MyModel.GetInfo().SharedModel;
                        //CurrentUser = Tekla.Structures.TeklaStructuresInfo.GetCurrentUser();
                        //Version = Tekla.Structures.TeklaStructuresInfo.GetCurrentProgramVersion();
                        //Configuration = Tekla.Structures.ModuleManager.Configuration.ToString();

                        //Model.TS_GetAdvancedOption();

                        //string version = Tekla.Structures.TeklaStructuresInfo.GetCurrentProgramVersion();
                        //skTSLib.SKVersion = version.ToUpper().Replace("SERVICE", "S").Replace("PACK", "P").Replace(" ", "");
                        //skWinLib.TSVersion = skTSLib.SKVersion;


                    }
                }
                else
                {
                    ConnectedToModel = false;
                    m_ModelConnection = null;
                }
            }
            catch
            {
                ConnectedToModel = false;
                m_ModelConnection = null;
            }

            //if (m_ModelConnection == null)
            //{
            //    System.Windows.Forms.Application.ExitThread();
            //}

            return m_ModelConnection;
        }
    }

    public static double RadianToDegree(double angle)
    {
        return angle * (180.0 / Math.PI);
    }

    public static bool CloseActiveDrawing()
    {
        DrawingHandler drawingHandler = new DrawingHandler();
        if (drawingHandler.GetActiveDrawing() != null)
        {
            drawingHandler.CloseActiveDrawing();
            return true;
        }
        return false;
    }

    public static bool IsDrawingCreatedForAssemblyAndDelete(TSM.Assembly assembly)
    {
        DrawingHandler drawingHandler = new DrawingHandler();
        DrawingEnumerator drawingEnum = drawingHandler.GetDrawings();

        while (drawingEnum.MoveNext())
        {
            AssemblyDrawing assemblyDrawing = drawingEnum.Current as AssemblyDrawing;
            if (assemblyDrawing != null && assemblyDrawing.AssemblyIdentifier.ID == assembly.Identifier.ID)
            {
                // Drawing exists for the assembly, delete it
                assemblyDrawing.Delete();
                return true; // Drawing was found and deleted
            }
        }
        return false; // No drawing found for the assembly
    }
    public static bool IsDrawingCreatedForAssembly(TSM.Assembly assembly)
    {
        DrawingHandler drawingHandler = new DrawingHandler();
        DrawingEnumerator drawingEnum = drawingHandler.GetDrawings();

        while (drawingEnum.MoveNext())
        {
            AssemblyDrawing assemblyDrawing = drawingEnum.Current as AssemblyDrawing;
            if (assemblyDrawing != null && assemblyDrawing.AssemblyIdentifier.ID == assembly.Identifier.ID)
                return true; // Drawing exists for the assembly
        }
        return false; // No drawing found for the assembly
    }
    public static string GetAssemblyNumber(TSD.AssemblyDrawing MyAssyDrawing)
    {
        string assemblyNumber = string.Empty;

        Assembly modelAssembly = new TSM.Model().SelectModelObject(MyAssyDrawing.AssemblyIdentifier) as TSM.Assembly;
        modelAssembly.GetReportProperty("ASSEMBLY_POS", ref assemblyNumber); 
        return assemblyNumber ;
    }

    public static bool TryToGetAssemblyNumber(TSD.Drawing drawing, out string assemblyNumber)
    {
        assemblyNumber = "";
        if (drawing is null)
            return false;

        var parts = drawing.GetSheet().GetAllObjects(typeof(TSD.Part));

        while (parts.MoveNext())
        {
            var part = parts.Current as TSD.Part;
            if (part is null)
                continue;

            var modelPart = new TSM.Model().SelectModelObject(part.ModelIdentifier) as TSM.Part;
            if (modelPart is null)
                continue;

            var assembly = modelPart.GetAssembly();
            if (assembly is null)
                continue;

            assembly.GetReportProperty("ASSEMBLY_POS", ref assemblyNumber);
            return true;
        }

        return false;
    }
    public static bool IsMarkContains(Mark MyMark, string SearchWord)
    { 
        if (MyMark != null)
        {
            TSD.ContainerElement container = MyMark.Attributes.Content;
            foreach (ElementBase item in container)
            {

                TSD.TextElement MyTextElement = item as TSD.TextElement;
                if (MyTextElement != null)
                {
                    if (MyTextElement.Value.ToString().ToUpper().Contains(SearchWord.ToUpper()) == true)
                        return true;
                }
            }
        }
        return false;
    }
    public static List<Tekla.Structures.Identifier> GetRelatedModelParts(List<TSD.Mark> PartMarks)
    {
        List<Tekla.Structures.Identifier> partIds = new List<Tekla.Structures.Identifier>();

        foreach (TSD.Mark mark in PartMarks)
        {
            var objList = mark.GetRelatedObjects();
            while (objList.MoveNext())
            {
                TSD.Part modelObject = objList.Current as TSD.Part;
                if (modelObject != null)
                {
                    partIds.Add(modelObject.ModelIdentifier);
                }
            }
        }

        return partIds;
    }

    public static List<Tekla.Structures.Identifier> GetRelatedModelParts(TSD.Mark PartMark)
    {
        List<Tekla.Structures.Identifier> partIds = new List<Tekla.Structures.Identifier>();

        DrawingObjectEnumerator objList = PartMark.GetRelatedObjects();
        while (objList.MoveNext())
        {
            TSD.Bolt modelObject = objList.Current as TSD.Bolt;
            if (modelObject != null)
            {
                partIds.Add(modelObject.ModelIdentifier);
            }
        }
        return partIds;
    }

    public static List<string> GetRelatedModelPartType(TSD.Mark PartMark)
    {
        List<string> partIds = new List<string>();

        DrawingObjectEnumerator objList = PartMark.GetRelatedObjects();
        while (objList.MoveNext())
        {
            TSD.ModelObject modelObject = objList.Current as TSD.ModelObject;
            if (modelObject != null)
            {
                partIds.Add(modelObject.GetType().Name.ToString());
            }
        }
        return partIds;
    }

    public static TSD.ModelObject GetDrawingObject(TSD.Mark PartMark)
    {
      

        DrawingObjectEnumerator objList = PartMark.GetRelatedObjects();
        while (objList.MoveNext())
        {
            TSD.ModelObject DrawingObject = objList.Current as TSD.ModelObject;
            if (DrawingObject != null)
                return DrawingObject;
        }
        return null;
    }


    public class SKDrawing
    {


        public static ArrayList GetDrawingObjectsByType(Drawing MyDrawing, Type objectType)
        {
            ArrayList ObjectsToBeSelected = new ArrayList();

            foreach (DrawingObject drawingObject in MyDrawing.GetSheet().GetAllObjects())
            {
                if (drawingObject.GetType() == objectType)
                {
                    ObjectsToBeSelected.Add(drawingObject);
                    if (objectType == typeof(TSD.Line))
                    {
                        TSD.Line MyLine = drawingObject as TSD.Line;
                        if (MyLine != null)
                        {
                            MyLine.Attributes.Line.Color = DrawingColors.Red;
                        }
                    }
                    else if (objectType == typeof(TSD.Part))
                    {
                        TSD.Part MyPart = drawingObject as TSD.Part;
                        if (MyPart != null)
                        {
                            MyPart.Attributes.VisibleLines.Color = DrawingColors.Cyan;
                        }
                    }
                    else if (objectType == typeof(TSD.Bolt))
                    {
                        TSD.Bolt MyBolt = drawingObject as TSD.Bolt;
                        if (MyBolt != null)
                        {
                            MyBolt.Attributes.Color = DrawingColors.Red;
                        }
                    }
                    else if (objectType == typeof(TSD.LeaderLine))
                    {
                        TSD.LeaderLine MyLeaderLine = drawingObject as TSD.LeaderLine;
                        if (MyLeaderLine != null)
                        {
                            //MyLeaderLine. . .LoadAttributes. . .Color = DrawingColors.Red;
                        }
                    }
                    else if (objectType == typeof(TSD.Mark))
                    {
                        TSD.Mark MyMark = drawingObject as TSD.Mark;
                        if (MyMark != null)
                        {
                            MyMark.Attributes.Frame.Color = DrawingColors.Red;
                            RectangleBoundingBox MyRectangleBoundingBox = MyMark.GetObjectAlignedBoundingBox();

                            TSD.Line MyLine = new Tekla.Structures.Drawing.Line(MyMark.GetView(), MyRectangleBoundingBox.MinPoint, MyRectangleBoundingBox.MaxPoint, 0.5);
                            //MyLine.StartPoint = MyRectangleBoundingBox.MinPoint;
                            //MyLine.EndPoint = MyRectangleBoundingBox.MaxPoint;
                            MyLine.Attributes.Line.Color = DrawingColors.Magenta;
                            MyLine.Bulge = 0.0;
                            bool lflag = MyLine.Insert();
                            //MyLeaderLine. . .LoadAttributes. . .Color = DrawingColors.Red;
                        }
                    }
                    drawingObject.Modify();
                }
            }

            return ObjectsToBeSelected;
        }
        public static void CopyColud()
        {

            Model model = new Model();
            //TSD.DrawingHandler drawingHandler = new TSD.DrawingHandler();
            //TSD.DrawingObjectEnumerator drawingObjectEnumerator = drawingHandler.GetDrawingObjectSelector().GetSelected();

            ArrayList clouds = new ArrayList();

            // Scale for the different scales of the views
            // Default is 1.0 for the drawing container
            //double scale = 1.0;

            //// If it is a view on the drawing, we get the view scale
            //if (view is Tekla.Structures.Drawing.View)
            //{
            //    Tekla.Structures.Drawing..View vu = view as Tekla.Structures.Drawing..View;
            //    scale = GetViewScale(vu);
            //}


            TSD.DrawingHandler MyDrawingHandler = new TSD.DrawingHandler();
            if (MyDrawingHandler.GetConnectionStatus())
            {
                TSD.Drawing MyDrawing = MyDrawingHandler.GetActiveDrawing();
                TSDUI.Picker MyPicker = MyDrawingHandler.GetPicker();
                //TSD.ViewBase MyViewBase = (TSD.ViewBase)null;
                TSD.ViewBase MyViewBase = null;
                TSG.Point MyPoint = new TSG.Point();
                MyPicker.PickPoint("Pick a Point to Insert Cloud:", out MyPoint, out MyViewBase);
                if (MyPoint != null && MyViewBase != null)
                {
                    TSD.PointList PolygonPoints = new TSD.PointList();
                    //MyPoint is the start point
                    //PolygonPoints to be read from a text file where x, y points are available before. 
                    PolygonPoints.Add(new TSG.Point(10, 10));
                    PolygonPoints.Add(new TSG.Point(250, 10));
                    PolygonPoints.Add(new TSG.Point(350, 450));
                    PolygonPoints.Add(new TSG.Point(300, 550));
                    PolygonPoints.Add(new TSG.Point(-100, 50));
                    PolygonPoints.Add(new TSG.Point(10, 20));

                    TSD.Polygon MyPolygon = new TSD.Polygon(MyViewBase, PolygonPoints);
                    bool cldflg = MyPolygon.Insert();
                    if (cldflg == true)
                    {
                        MyDrawing.CommitChanges();
                        MyDrawingHandler.SaveActiveDrawing();
                    }
                }

            }

        }

        public static void GetViewData(System.Windows.Forms.DataGridView MyDataGridView, System.Windows.Forms.ProgressBar MyProgressBar)
        {
            //pbar1.Visible = true;
            //DateTime stm = skTSLib.setStartup(skApplicationName, skApplicationVersion, "DrawingOpenClose", "", "", "", lblsbar1, lblsbar2);
            //skTSLib.SKDrawing.GetViewData(dgvdrg, pbar1);
            //skTSLib.setCompletion(skApplicationName, skApplicationVersion, "DrawingOpenClose", "", "DrawingCount: " + pbar1.Value.ToString(), "", lblsbar1, lblsbar2, stm);
            //pbar1.Visible = false;+++

            //Loop through all selected document
            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            if (drg_handler.GetConnectionStatus() == true)
            {
                TSD.DrawingEnumerator mydrg_enum = drg_handler.GetDrawingSelector().GetSelected();
                MyProgressBar.Maximum = mydrg_enum.GetSize();
                MyProgressBar.Value = 0;
                MyDataGridView.Rows.Clear();
                while (mydrg_enum.MoveNext())
                {
                    MyProgressBar.Value = MyProgressBar.Value + 1;
                    TSD.Drawing MyDrawing = mydrg_enum.Current as TSD.Drawing;
                    if (MyDrawing != null)
                    {
                        string drgtype = MyDrawing.GetType().Name.ToString();
                        string drgmark = MyDrawing.Name;
                        int vwct = 0;
                        string vwdtls = string.Empty;
                        TSD.ContainerView MyContainerView = MyDrawing.GetSheet();
                        MyDrawing.Select();

                        TSD.DrawingObjectEnumerator AllViews = MyContainerView.GetAllViews();
                        string viewtext = string.Empty;
                        string viewscl = string.Empty;
                        while (AllViews.MoveNext())
                        {
                            string cviewtext = string.Empty;
                            string cviewscl = string.Empty;
                            TSD.View CurrentView = AllViews.Current as TSD.View;
                            if (CurrentView != null)
                            {
                                //Tag A1 location
                                TSD.ViewMarkBasicTagAttributes MyViewMarkTagsAttributes = CurrentView.Attributes.TagsAttributes.TagA1;

                                //Tag A1 Content
                                TSD.ContainerElement MyContainerElement = MyViewMarkTagsAttributes.TagContent;
                                foreach (var item in MyContainerElement)
                                {

                                    if (item.GetType() == typeof(TSD.TextElement))
                                    {
                                        TSD.TextElement MyTextElement = item as TSD.TextElement;
                                        if (MyTextElement != null)
                                            cviewtext = cviewtext + "|" + MyTextElement.Value.ToString();

                                    }

                                }

                                //Tag A2 Content
                                MyViewMarkTagsAttributes = CurrentView.Attributes.TagsAttributes.TagA2;
                                MyContainerElement = MyViewMarkTagsAttributes.TagContent;
                                foreach (var item in MyContainerElement)
                                {
                                    //viewtext = item.value
                                    if (item.GetType() == typeof(TSD.TextElement))
                                    {
                                        TSD.TextElement MyTextElement = item as TSD.TextElement;
                                        if (MyTextElement != null)
                                            cviewtext = cviewtext + "|" + MyTextElement.Value.ToString();

                                    }

                                }

                                //Tag A3 Content
                                MyViewMarkTagsAttributes = CurrentView.Attributes.TagsAttributes.TagA3;
                                MyContainerElement = MyViewMarkTagsAttributes.TagContent;
                                foreach (var item in MyContainerElement)
                                {
                                    //viewtext = item.value
                                    if (item.GetType() == typeof(TSD.TextElement))
                                    {
                                        TSD.TextElement MyTextElement = item as TSD.TextElement;
                                        if (MyTextElement != null)
                                            cviewtext = cviewtext + "|" + MyTextElement.Value.ToString();

                                    }

                                }

                                //Tag A4 Content
                                MyViewMarkTagsAttributes = CurrentView.Attributes.TagsAttributes.TagA4;
                                MyContainerElement = MyViewMarkTagsAttributes.TagContent;
                                foreach (var item in MyContainerElement)
                                {
                                    //viewtext = item.value
                                    if (item.GetType() == typeof(TSD.TextElement))
                                    {
                                        TSD.TextElement MyTextElement = item as TSD.TextElement;
                                        if (MyTextElement != null)
                                            cviewtext = cviewtext + "|" + MyTextElement.Value.ToString();

                                    }

                                }

                                //Tag A5 Content
                                MyViewMarkTagsAttributes = CurrentView.Attributes.TagsAttributes.TagA5;
                                MyContainerElement = MyViewMarkTagsAttributes.TagContent;
                                foreach (var item in MyContainerElement)
                                {
                                    //viewtext = item.value
                                    if (item.GetType() == typeof(TSD.TextElement))
                                    {
                                        TSD.TextElement MyTextElement = item as TSD.TextElement;
                                        if (MyTextElement != null)
                                            cviewtext = cviewtext + "|" + MyTextElement.Value.ToString();

                                    }

                                }
                                double cvscl = CurrentView.Attributes.Scale;
                                cviewscl = cvscl.ToString();
                                vwct++;
                            }
                            viewtext = viewtext + "[" + cviewtext + "]";
                            viewscl = viewscl + "[" + cviewscl + "]";
                        }
                        drg_handler.CloseActiveDrawing();
                        MyDataGridView.Rows.Add(drgtype, drgmark, vwct, viewtext.Replace("[|", "["), viewscl);
                    }

                }

                skWinLib.DataGridView_Setting_After(MyDataGridView);
                skWinLib.updaterowheader(MyDataGridView);
            }
        }


        public enum Dominating_Corner
        {
            LeftBottomCorner, RightBottomCorner, LeftTopCorner, RightTopCorner,
        }
        private void CreateNewDimensions(TSM.Model Model, TSD.Drawing currentDrawing, Dominating_Corner dimensionCorner)
        {


            DrawingObjectEnumerator drawingObjectEnumerator = currentDrawing.GetSheet().GetAllObjects();
            List<DrawingObject> dwgObjct = new List<DrawingObject>();

            foreach (DrawingObject currentObject in drawingObjectEnumerator)
            {
                dwgObjct.Add(currentObject);
            }

            TSD.Part platePart = dwgObjct.Where(x => x is TSD.Part).ToList().FirstOrDefault() as TSD.Part;
            Bolt plateBolt = dwgObjct.Where(x => x is Bolt).ToList().FirstOrDefault() as Bolt;

            TSM.ModelObject Plate = Model.SelectModelObject(platePart.ModelIdentifier);
            TSM.ModelObject boltobjct = Model.SelectModelObject(plateBolt.ModelIdentifier);

            TSD.View View = platePart.GetView() as TSD.View;
            TransformationPlane SavePlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(View.DisplayCoordinateSystem));
            try
            {
                GeometricPlane nutralPlane = new GeometricPlane();
                Plate.Select();
                boltobjct.Select();
                Beam plateBeam = Plate as Beam;
                BoltArray boltArray = boltobjct as BoltArray;
                TSG.Point sp = plateBeam.StartPoint;
                TSG.Point ep = plateBeam.EndPoint;
                double width = Math.Abs(TSG.Projection.PointToPlane((Plate as Beam).GetSolid().MinimumPoint, nutralPlane).Y);
                double length = Distance.PointToPoint(sp, ep);

                Vector plateX = plateBeam.GetCoordinateSystem().AxisX.GetNormal();
                Vector platey = plateBeam.GetCoordinateSystem().AxisY.GetNormal();

                TSG.Point p00 = sp - platey * width;
                TSG.Point p01 = sp + platey * width;
                TSG.Point p10 = ep - platey * width;
                TSG.Point p11 = ep + platey * width;


                TSG.LineSegment leftLineseg = new TSG.LineSegment(p00, p01);
                TSG.LineSegment rightLineseg = new TSG.LineSegment(p10, p11);
                TSG.LineSegment bottomLineseg = new TSG.LineSegment(p00, p10);
                TSG.LineSegment topLineseg = new TSG.LineSegment(p01, p11);
                TSG.Line leftLine = new TSG.Line(leftLineseg);
                TSG.Line rightLine = new TSG.Line(rightLineseg);
                TSG.Line bottomLine = new TSG.Line(bottomLineseg);
                TSG.Line topLine = new TSG.Line(topLineseg);

                List<TSG.Point> boltPoints = new List<TSG.Point>();
                var positions = boltArray.BoltPositions;
                foreach (var item in positions)
                {
                    boltPoints.Add(item as TSG.Point);
                }
                LineSegment verticalLineSegment = new LineSegment();
                LineSegment horizontalLineSegment = new LineSegment();

                TSG.Point boltMarkPoint = new TSG.Point();
                TSG.Point plateMarkPoint = new TSG.Point(0.5 * (p01.X + p11.X), 0.5 * (p01.Y + p11.Y), 0.5 * (p01.Z + p11.Z)) + new Vector(0, 30, 0);

                if (dimensionCorner == Dominating_Corner.RightTopCorner)
                {
                    verticalLineSegment = rightLineseg;
                    horizontalLineSegment = topLineseg;
                    boltMarkPoint = p00 + new Vector(-100, -100, 0);
                }
                else if (dimensionCorner == Dominating_Corner.RightBottomCorner)
                {
                    verticalLineSegment = rightLineseg;
                    horizontalLineSegment = bottomLineseg;
                    boltMarkPoint = p01 + new Vector(-100, 100, 0);
                }
                else if (dimensionCorner == Dominating_Corner.LeftTopCorner)
                {
                    verticalLineSegment = leftLineseg;
                    horizontalLineSegment = topLineseg;
                    boltMarkPoint = p10 + new Vector(100, -100, 0);
                }
                else if (dimensionCorner == Dominating_Corner.LeftBottomCorner)
                {
                    verticalLineSegment = leftLineseg;
                    horizontalLineSegment = bottomLineseg;
                    boltMarkPoint = p11 + new Vector(100, 100, 0);
                }

                Mark plateMark = new Mark(platePart);
                plateMark.Attributes.Content.Clear();
                plateMark.Attributes.Content.Add(new TextElement(plateBeam.Profile.ProfileString));
                plateMark.InsertionPoint = plateMarkPoint;
                plateMark.Insert();

                Mark boltMark = new Mark(plateBolt);
                boltMark.Attributes.Content.Clear();
                boltMark.Attributes.Content.Add(new TextElement(boltArray.BoltSize + boltArray.BoltStandard));
                boltMark.InsertionPoint = boltMarkPoint;
                boltMark.Insert();
                PlacingDimensionBasedOnCorner(verticalLineSegment, horizontalLineSegment, boltPoints, platePart, currentDrawing, dimensionCorner);
            }
            finally
            {
                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(SavePlane);
            }
        }

        private void PlacingDimensionBasedOnCorner(LineSegment verticalLineSegment, LineSegment horizontalLineSegment, List<TSG.Point> boltPoints, TSD.Part platePart, TSD.Drawing currentDrawing, Dominating_Corner dimensionCorner)
        {
            TSD.PointList horizontalPointList = new TSD.PointList();
            TSD.PointList majorhorizontalPointList = new TSD.PointList();
            TSD.PointList verticalPointList = new TSD.PointList();
            TSD.PointList majorverticalPointList = new TSD.PointList();
            TSD.PointList verticalPoints = new TSD.PointList();

            verticalPoints = FindBoltProjectedPoints(boltPoints, verticalLineSegment);
            verticalPointList.Add(verticalLineSegment.StartPoint);
            majorverticalPointList.Add(verticalLineSegment.StartPoint);
            verticalPointList.AddRange(verticalPoints);
            verticalPointList.Add(verticalLineSegment.EndPoint);
            majorverticalPointList.Add(verticalLineSegment.EndPoint);


            TSD.PointList horizontalPoints = new TSD.PointList();
            horizontalPoints = FindBoltProjectedPoints(boltPoints, horizontalLineSegment);
            horizontalPointList.Add(horizontalLineSegment.StartPoint);
            majorhorizontalPointList.Add(horizontalLineSegment.StartPoint);
            horizontalPointList.AddRange(horizontalPoints);
            horizontalPointList.Add(horizontalLineSegment.EndPoint);
            majorhorizontalPointList.Add(horizontalLineSegment.EndPoint);


            Vector horizontalDimensionVector = (dimensionCorner == Dominating_Corner.RightTopCorner || dimensionCorner == Dominating_Corner.LeftTopCorner) ? new Vector(0, 1, 0) : new Vector(0, -1, 0);
            Vector verticalDimentionVector = (dimensionCorner == Dominating_Corner.RightTopCorner || dimensionCorner == Dominating_Corner.RightBottomCorner) ? new Vector(1, 0, 0) : new Vector(-1, 0, 0);


            ViewBase ViewBase = platePart.GetView();
            StraightDimensionSet.StraightDimensionSetAttributes attr = new StraightDimensionSet.StraightDimensionSetAttributes(platePart);

            StraightDimensionSet xDimensions = new StraightDimensionSetHandler().CreateDimensionSet(ViewBase, horizontalPointList, horizontalDimensionVector, 100, attr);
            StraightDimensionSet majorxDimensions = new StraightDimensionSetHandler().CreateDimensionSet(ViewBase, majorhorizontalPointList, horizontalDimensionVector, 200, attr);
            StraightDimensionSet yDimensions = new StraightDimensionSetHandler().CreateDimensionSet(ViewBase, verticalPointList, verticalDimentionVector, 100, attr);
            StraightDimensionSet majoryDimensions = new StraightDimensionSetHandler().CreateDimensionSet(ViewBase, majorverticalPointList, verticalDimentionVector, 200, attr);



            currentDrawing.CommitChanges();

        }

        private TSD.PointList FindBoltProjectedPoints(List<TSG.Point> boltPoints, TSG.LineSegment lineSegment)
        {
            TSD.PointList points = new TSD.PointList();
            foreach (TSG.Point item in boltPoints)
            {
                TSG.Line relevantLine = new TSG.Line(lineSegment.StartPoint, lineSegment.EndPoint);
                TSG.Point temppt = TSG.Projection.PointToLine(item, relevantLine);
                if (!CheckPointContainsIntheList(points, temppt))
                {
                    points.Add(temppt);
                }
            }
            return points;
        }

        private bool CheckPointContainsIntheList(TSD.PointList points, TSG.Point temppt)
        {
            foreach (TSG.Point point in points)
            {
                if (temppt.X == point.X && temppt.Y == point.Y && temppt.Z == point.Z)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteDrawingObjects(TSD.Drawing currentDrawing)
        {
            #region Delete Existing parts
            List<TSD.View> frontViews = new List<TSD.View>();
            List<TSD.View> otherView = new List<TSD.View>();
            DrawingObjectEnumerator viewCollection = currentDrawing.GetSheet().GetAllViews();
            List<TSD.Part> tsdParts = new List<TSD.Part>();
            foreach (DrawingObject dwgObj in viewCollection)
            {
                if (dwgObj is TSD.View)
                {
                    TSD.View currentView = (TSD.View)dwgObj;
                    if (currentView.ViewType == TSD.View.ViewTypes.FrontView)
                    {
                        frontViews.Add(currentView);
                    }
                    else
                    {
                        otherView.Add(currentView);
                    }
                }

            }
            foreach (TSD.View currentView in otherView)
            {
                currentView.Delete();
            }
            TSD.View frontView = frontViews.FirstOrDefault();
            DrawingObjectEnumerator allObjects = frontView.GetAllObjects();
            List<DrawingObject> drawingObjects = new List<DrawingObject>();
            List<StraightDimensionSet> straightDimensionSets = new List<StraightDimensionSet>();
            List<Mark> marks = new List<Mark>();
            while (allObjects.MoveNext())
            {
                DrawingObject currentObject = allObjects.Current;
                if (currentObject is StraightDimensionSet)
                {
                    straightDimensionSets.Add(currentObject as StraightDimensionSet);
                }
                else if (currentObject is Mark)
                {
                    marks.Add(currentObject as Mark);
                }
            }
            foreach (StraightDimensionSet item in straightDimensionSets)
            {
                item.Delete();
            }
            foreach (Mark mark in marks)
            {
                mark.Delete();
            }
            currentDrawing.CommitChanges();
            #endregion
        }

        public void DimensionCreation(TSM.Model Model)
        {
            try
            {
                DrawingHandler DrawingHandler = new DrawingHandler();

                if (DrawingHandler.GetConnectionStatus())
                {
                    TSD.Drawing CurrentDrawing = DrawingHandler.GetActiveDrawing();
                    if (CurrentDrawing != null)
                    {
                        DrawingObjectEnumerator DrawingObjectEnumerator = CurrentDrawing.GetSheet().GetAllObjects(typeof(TSD.Part));

                        foreach (TSD.Part myPart in DrawingObjectEnumerator)
                        {
                            TSD.View View = myPart.GetView() as TSD.View;
                            TransformationPlane SavePlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(View.DisplayCoordinateSystem));

                            Identifier Identifier = myPart.ModelIdentifier;
                            TSM.ModelObject ModelSideObject = Model.SelectModelObject(Identifier);

                            TSD.PointList PointList = new TSD.PointList();
                            Beam myBeam = new Beam();
                            if (ModelSideObject.GetType().Equals(typeof(Beam)))
                            {
                                myBeam.Identifier = Identifier;
                                myBeam.Select();

                                PointList.Add(myBeam.StartPoint);
                                PointList.Add(myBeam.EndPoint);
                            }

                            ViewBase ViewBase = myPart.GetView();
                            StraightDimensionSet.StraightDimensionSetAttributes attr = new StraightDimensionSet.StraightDimensionSetAttributes(myPart);

                            if (myBeam.StartPoint.X != myBeam.EndPoint.X)
                            {
                                StraightDimensionSet XDimensions = new StraightDimensionSetHandler().CreateDimensionSet(ViewBase, PointList, new Vector(0, -100, 0), 200, attr);
                                CurrentDrawing.CommitChanges();
                            }

                             Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(SavePlane);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
    
        public static ArrayList SelectDrawingObjects(Type[] FilterType)
        {
            ArrayList MyData = new ArrayList();
            TSD.DrawingHandler DH = new TSD.DrawingHandler();
            if (DH.GetConnectionStatus() == true)
            {
                TSD.Drawing CurrentDrawing = DH.GetActiveDrawing();
                if (CurrentDrawing != null)
                {
                    //Get All Views
                    TSD.DrawingObjectEnumerator DrgViews = CurrentDrawing.GetSheet().GetViews();
                    while (DrgViews.MoveNext())
                    {
                        //Loop through each views
                        Tekla.Structures.Drawing.View CurrentView = DrgViews.Current as Tekla.Structures.Drawing.View;
                        if (CurrentView != null)
                        {
                            //Get Filter Objects
                            Tekla.Structures.Drawing.DrawingObjectEnumerator DrgObjects = CurrentView.GetObjects(FilterType);
                            while (DrgObjects.MoveNext())
                            {
                                if (DrgObjects.Current != null)
                                    MyData.Add(DrgObjects.Current);

                            }
                        }
                    }

                    //Select
                    DH.GetDrawingObjectSelector().SelectObjects(MyData, false);
                }
            }
            return MyData;
        }
        public static TSD.PointList Get_plate_corner_points(TSD.PointList x_sorted_bounding_box, TSD.PointList y_sorted_bounding_box)
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

        public static TSD.PointList sorting_points_by_x_asc(TSD.PointList list_of_points)
        {

            for (int i = 0; i < list_of_points.Count; i++)
            {

                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).X < (list_of_points[j] as TSG.Point).X)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;
                    }
                }
            }
            return list_of_points;
        }


        public static TSD.PointList sorting_points_by_y_asc(TSD.PointList list_of_points)
        {
            for (int i = 0; i < list_of_points.Count; i++)
            {
                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).Y < (list_of_points[j] as TSG.Point).Y)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;

                    }

                }
            }
            return list_of_points;

        }


        //function converts ArrayList to PointList and then Sort
        public static TSD.PointList GetSortedPointList(ArrayList MyArrayList, string SortByXYZ, bool SortByAscending)
        {
            TSD.PointList MyPointLists = new TSD.PointList();
            foreach (TSG.Point MyPoint in MyArrayList)
            {
                if (MyPoint != null)
                    MyPointLists.Add(MyPoint);
            }


            return GetSortedPointList(MyPointLists, SortByXYZ, SortByAscending);

        }

        

        public static TSD.PointList GetSortedPointList(TSD.PointList MyPointLists, string SortByXYZ, bool SortByAscending)
        {
            //ArrayList MyBoltPositions = MyBoltArray.BoltPositions;
            //foreach (TSG.Point MyBoltPosition in MyBoltPositions)
            //{

            for (int i = 0; i < MyPointLists.Count; i++)
            {
                TSG.Point tmp_i = new TSG.Point((MyPointLists[i] as TSG.Point).X, (MyPointLists[i] as TSG.Point).Y, (MyPointLists[i] as TSG.Point).Z);
                for (int j = 0; j < MyPointLists.Count; j++)
                {                    
                    TSG.Point tmp_j = new TSG.Point((MyPointLists[j] as TSG.Point).X, (MyPointLists[j] as TSG.Point).Y, (MyPointLists[j] as TSG.Point).Z);
                    bool flag = false;
                    if (SortByXYZ.Trim().ToUpper() == "X")
                    {
                        if (SortByAscending == true)
                        {
                            if ((MyPointLists[i] as TSG.Point).X < (MyPointLists[j] as TSG.Point).X)
                                flag = true;         
                        }
                        else
                        {
                            if ((MyPointLists[i] as TSG.Point).X > (MyPointLists[j] as TSG.Point).X)
                                flag = true;
                        }

                    }
                    else if (SortByXYZ.Trim().ToUpper() == "Y")
                    {
                        if (SortByAscending == true)
                        {
                            if ((MyPointLists[i] as TSG.Point).Y < (MyPointLists[j] as TSG.Point).Y)
                                flag = true;
                        }
                        else
                        {
                            if ((MyPointLists[i] as TSG.Point).Y > (MyPointLists[j] as TSG.Point).Y)
                                flag = true;
                        }
                    }
                    else if (SortByXYZ.Trim().ToUpper() == "Z")
                    {
                        if (SortByAscending == true)
                        {
                            if ((MyPointLists[i] as TSG.Point).Z < (MyPointLists[j] as TSG.Point).Z)
                                flag = true;
                        }
                        else
                        {
                            if ((MyPointLists[i] as TSG.Point).Z > (MyPointLists[j] as TSG.Point).Z)
                                flag = true;
                        }
                    }
                    if (flag == true)
                    {
                        //swap the points
                        MyPointLists[i] = tmp_j;
                        MyPointLists[j] = tmp_i;
                    }


                }
            }
            return MyPointLists;

        }

        public static bool IsDrawingObjectPartValid(TSD.DrawingObject MyDrawingObject, string CheckLineType = "SolidLine", string IgnoreColor = "Invisible")
        {
            TSD.Part MyPart = MyDrawingObject as TSD.Part;
            //check whether DrawingObject is a valid Part
            if (MyPart != null)
            {

                //MyPart.Attributes.
                //if (MyPart.Attributes.Color.ToString().ToUpper() == "Invisible".ToUpper())
                {
                    LineTypeAttributes MyLineTypeAttributes = MyPart.Attributes.VisibleLines;
                    //get LineTypeAttributes for that part and check whether LineTypeAttributes is valid
                    if (MyLineTypeAttributes != null)
                    {
                        LineTypes MyLineTypes = MyLineTypeAttributes.Type;
                        //get LineTypes
                        //check whether LineTypes is solid and color not invisible 
                        if (MyLineTypes.ToString().ToUpper() == CheckLineType.ToUpper() && MyLineTypeAttributes.Color.ToString().ToUpper() != IgnoreColor.ToUpper())
                            return true;
                    }
                }

            }
            return false;
        }
       
        public static bool SetStraightDimensionSetTextColor(DrawingObject SelectedObject, DrawingColors Color)
        {
            bool flag = false;
            StraightDimensionSet mysds = SelectedObject as StraightDimensionSet;
            DrawingObjectEnumerator mydrgobjen = mysds.GetObjects();

            while (mydrgobjen.MoveNext())
            {
                StraightDimension mysd = mydrgobjen.Current as StraightDimension;
                ContainerElement myce = mysd.Value;
                var values = myce.GetEnumerator();
                while (values.MoveNext())
                {                  
                    TextElement MyTxtEl = values.Current as TextElement;
                    MyTxtEl.Font.Color = Color;
                    flag = mysd.Modify();
                }
            }
            return flag;
        }

        #region Model object child part fetching
        /// <summary>
        /// Gets list of assembly parts 
        /// </summary>
        /// <param name="SelectedModelObjects"></param>
        /// <returns></returns>
        public static ArrayList GetAssemblyParts(Assembly assembly)
        {
            ArrayList Parts = new ArrayList();
            IEnumerator AssemblyChildren = (assembly).GetSecondaries().GetEnumerator();

            Parts.Add((assembly).GetMainPart().Identifier);

            while (AssemblyChildren.MoveNext())
                Parts.Add((AssemblyChildren.Current as Tekla.Structures.Model.ModelObject).Identifier);

            return Parts;
        }

        /// <summary>
        /// Gets list of component parts
        /// </summary>
        /// <param name="SelectedModelObjects"></param>
        /// <returns></returns>
        public static ArrayList GetComponentParts(BaseComponent component)
        {
            ArrayList Parts = new ArrayList();
            IEnumerator myChildren = component.GetChildren();

            while (myChildren.MoveNext())
                Parts.Add((myChildren.Current as Tekla.Structures.Model.ModelObject).Identifier);

            return Parts;
        }

        /// <summary>
        /// Gets list of task parts
        /// </summary>
        /// <param name="TaskMembers"></param>
        /// <returns></returns>
        public static ArrayList GetTaskParts(Tekla.Structures.Model.Task task)
        {
            ArrayList Parts = new ArrayList();

            ModelObjectEnumerator myMembers = task.GetChildren();

            while (myMembers.MoveNext())
            {
                if (myMembers.Current is Tekla.Structures.Model.Task)
                    Parts.AddRange(GetTaskParts(myMembers.Current as Tekla.Structures.Model.Task));
                else if (myMembers.Current is Tekla.Structures.Model.Part)
                    Parts.Add(myMembers.Current.Identifier);
            }

            return Parts;
        }

        #endregion

        #region Coordinate system calculations

        //public static Tekla.Structures.Geometry3d.Vector UpDirection = new Tekla.Structures.Geometry3d.Vector(0.0, 0.0, 1.0);

        /// <summary>
        /// Add one basic view
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="MyDrawing"></param>
        /// <param name="Parts"></param>
        /// <param name="CoordinateSystem"></param>
        public static Tekla.Structures.Drawing.View AddView(String Name, TSD.Drawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            Tekla.Structures.Drawing.View MyView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(),
                                                                                     CoordinateSystem,
                                                                                     CoordinateSystem,
                                                                                     Parts);

            MyView.Name = Name;
            MyView.Insert();

            return MyView;

        }
        /// <summary>
        /// Add rotated view
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="MyDrawing"></param>
        /// <param name="Parts"></param>
        /// <param name="CoordinateSystem"></param>
        public static Tekla.Structures.Drawing.View AddRotatedView(String Name,TSD.Drawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem displayCoordinateSystem = new Tekla.Structures.Geometry3d.CoordinateSystem();

            Tekla.Structures.Geometry3d.Matrix RotationAroundX = Tekla.Structures.Geometry3d.MatrixFactory.Rotate(20.0 * Math.PI * 2.0 / 360.0, CoordinateSystem.AxisX);
            Tekla.Structures.Geometry3d.Matrix RotationAroundZ = Tekla.Structures.Geometry3d.MatrixFactory.Rotate(30.0 * Math.PI * 2.0 / 360.0, CoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Matrix Rotation = RotationAroundX * RotationAroundZ;

            displayCoordinateSystem.AxisX = new Tekla.Structures.Geometry3d.Vector(Rotation.Transform(new Tekla.Structures.Geometry3d.Point(CoordinateSystem.AxisX)));
            displayCoordinateSystem.AxisY = new Tekla.Structures.Geometry3d.Vector(Rotation.Transform(new Tekla.Structures.Geometry3d.Point(CoordinateSystem.AxisY)));

            Tekla.Structures.Drawing.View FrontView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(),
                                                                                        CoordinateSystem,
                                                                                        displayCoordinateSystem,
                                                                                        Parts);

            FrontView.Name = Name;
            FrontView.Insert();

            return FrontView;
        }

        /// <summary>
        /// Gets part default front view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        /// <param name="objectCoordinateSystem"></param>
        /// <returns></returns>
       
        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicViewsCoordinateSystemForFrontView(Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem, Tekla.Structures.Geometry3d.Vector UpDirection)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();

            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection).GetNormal();
            result.AxisY = UpDirection.GetNormal();

            return result;
        }

        /// <summary>
        /// Gets part default top view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        /// <param name="objectCoordinateSystem"></param>
        /// <returns></returns>
        
        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicViewsCoordinateSystemForTopView(Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem, Tekla.Structures.Geometry3d.Vector UpDirection)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();

            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection);
            result.AxisY = tempVector;

            return result;
        }

        /// <summary>
        /// Gets part default end view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        /// <param name="objectCoordinateSystem"></param>
        /// <returns></returns>
        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicViewsCoordinateSystemForEndView(Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem, Tekla.Structures.Geometry3d.Vector UpDirection)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();

            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector;
            result.AxisY = UpDirection;

            return result;
        }

        #endregion
    }
	public static double get_perimeter(TSM.Part part)
    {
        List<TSG.Point> points = new List<TSG.Point>();
        TSG.Point prev_point = null;
        int i = 1;
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(part.GetCoordinateSystem()));
        Matrix matrix = MatrixFactory.FromCoordinateSystem(part.GetCoordinateSystem());
        MyModel.CommitChanges();
        EdgeEnumerator edgeEnumerator = part.GetSolid().GetEdgeEnumerator();
        double perimeter = 0;
        while (edgeEnumerator.MoveNext())
        {
            points.Add((edgeEnumerator.Current as Edge).StartPoint);
            points.Add((edgeEnumerator.Current as Edge).EndPoint);
        }

        List<TSG.Point> unique_points = points.OrderBy(x => x.Z).Distinct().ToList();
        Dictionary<double, int> z_count = new Dictionary<double, int>();
        foreach (TSG.Point point in unique_points)
        {
            if (prev_point == null)
            {
                prev_point = point;

            }
            else
            {
                if (Math.Abs(prev_point.Z - point.Z) <= 0.5)
                {
                    i++;
                    prev_point = point;
                }
                else
                {
                    z_count.Add(prev_point.Z, i);
                    i = 1;
                    prev_point = point;

                }
                if (unique_points.IndexOf(point) == unique_points.Count - 1)
                {
                    z_count.Add(prev_point.Z, i);
                }
            }
        }

        KeyValuePair<double,int> max_z_count = z_count.OrderByDescending(x => x.Value).First();

        edgeEnumerator.Reset();
        while(edgeEnumerator.MoveNext())
        {
            Edge edge = edgeEnumerator.Current as Edge;
            if(edge!=null)
            {
                if(Math.Abs(edge.StartPoint.Z-max_z_count.Key)<0.5 && Math.Abs(edge.EndPoint.Z - max_z_count.Key)< 0.5)
                {
                    perimeter += Tekla.Structures.Geometry3d.Distance.PointToPoint(edge.StartPoint,edge.EndPoint);
                }
            }
        }
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        return perimeter;
    }








    //public static List<TSG.Point> get_unique_edge_points(TSM.Part part)
    //{
    //    List<TSG.Point> points = new List<TSG.Point>();
    //    TSG.Point prev_point = null;
    //    int i = 1;
    //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
    //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(part.GetCoordinateSystem()));
    //    Matrix matrix = MatrixFactory.FromCoordinateSystem(part.GetCoordinateSystem());
    //    MyModel.CommitChanges();
    //    EdgeEnumerator edgeEnumerator =  part.GetSolid().GetEdgeEnumerator();
    //    while(edgeEnumerator.MoveNext())
    //    {
    //        points.Add((edgeEnumerator.Current as Edge).StartPoint);
    //        points.Add((edgeEnumerator.Current as Edge).EndPoint);
    //    }

    //    List<TSG.Point> unique_points = points.OrderBy(x => x.Z).Distinct().ToList();
    //    Dictionary<double, int> z_count = new Dictionary<double, int>();
    //    foreach(TSG.Point point in unique_points)
    //    {
    //        if(prev_point==null)
    //        {
    //            prev_point = point;
                
    //        }
    //        else
    //        {
    //            if(Math.Abs(prev_point.Z-point.Z)<=0.5)
    //            {
    //                i++;
    //                prev_point = point;
    //            }
    //            else
    //            {
    //                z_count.Add(prev_point.Z, i);
    //                i = 1;
    //                prev_point = point;

    //            }
    //            if(unique_points.IndexOf(point) == unique_points.Count-1)
    //            {
    //                z_count.Add(prev_point.Z, i);
    //            }
    //        }
    //    }

    //    KeyValuePair<double, int> max_Z = z_count.MaxBy(x=>x.Value);



    //    return points;
    //}

    public class Model
    {

        //TSM.Model MyModel. = new TSM.Model();
        //220503

        public static string[] TS_GetAdvancedOption()
        {

            // List<string> xspath = List<string>;
            // string xspath = string.Empty;
            // string XS_SYSTEM = string.Empty;
            // string XSDATADIR = string.Empty;
            // string XS_AD_ENVIRONMENT = string.Empty;
            // string XS_DIR = string.Empty;
            // string XSBIN = string.Empty;
            // string XS_FIRM = string.Empty;
            // string XS_PROJECT = string.Empty;


            TeklaStructuresSettings.GetAdvancedOption("XS_SYSTEM", ref XS_SYSTEM);
            TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref XSDATADIR);
            TeklaStructuresSettings.GetAdvancedOption("XS_AD_ENVIRONMENT", ref XS_AD_ENVIRONMENT);
            TeklaStructuresSettings.GetAdvancedOption("XS_DIR", ref XS_DIR);
            TeklaStructuresSettings.GetAdvancedOption("XSBIN", ref XSBIN);
            TeklaStructuresSettings.GetAdvancedOption("XS_FIRM", ref XS_FIRM);
            TeklaStructuresSettings.GetAdvancedOption("XS_PROJECT", ref XS_PROJECT);
            TeklaStructuresSettings.GetAdvancedOption("XS_TEMPLATE_DIRECTORY", ref XS_TEMPLATE_DIRECTORY);   


            if (XS_SYSTEM.Length >= 1)
                xspath = xspath + ";" + XS_SYSTEM;

            if (XSDATADIR.Length >= 1)
                xspath = xspath + ";" + XSDATADIR;

            //if (XS_AD_ENVIRONMENT.Length >= 1)
            //    xspath = xspath + ";" + XS_AD_ENVIRONMENT;

            if (XS_DIR.Length >= 1)
                xspath = xspath + ";" + XS_DIR;

            if (XSBIN.Length >= 1)
                xspath = xspath + ";" + XSBIN;

            if (XS_FIRM.Length >= 1)
                xspath = xspath + ";" + XS_FIRM;

            if (XS_PROJECT.Length >= 1)
                xspath = xspath + ";" + XS_PROJECT;

            if(XS_PROJECT!="")
            {
                string[] split = XS_PROJECT.Split('\\');
                if (SK_CLIENT == "")
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        if (i == 2)
                        {
                            SK_CLIENT += split[i];
                        }
                        else
                        {
                            SK_CLIENT += split[i] + "\\";
                        }
                    }
                }
            }
            string[] spltxspath = xspath.Split(new Char[] { ';' });
            return spltxspath;
        }

        //public static TSM.Connection InsertConnection(string ConnName, int ConnNumber, string sAttrFile, TSM.ModelObject Primary, ArrayList Secondaries) //, ArrayList AttributesList
        //{
        //    TSM.Connection MyConnection = new TSM.Connection();
        //    try
        //    {                

        //        MyConnection.Name = ConnName;
        //        MyConnection.Number = ConnNumber;

        //        MyConnection.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE;
        //        //MyConnection.PositionType = PositionTypeEnum.COLLISION_PLANE;

        //        MyConnection.SetPrimaryObject(Primary);
        //        MyConnection.SetSecondaryObjects(Secondaries);
        //        if (System.IO.File.Exists(sAttrFile) == true)
        //            MyConnection.LoadAttributesFromFile(sAttrFile);
        //        //MyConnection.Code = ConnName;
        //        bool flag = MyConnection.Insert();

        //        return MyConnection;

        //    }  //end of try loop

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Original error:Insert Connections @ " + ConnName + " " + sAttrFile + " " + ex.Message);
        //    }
        //    return null;
        //}


        //public static int InsertDetail(Tekla.Structures.Geometry3d.Point DetailPoint, TSM.ModelObject Primary, string sConnData, string sAttrFile, string sClass, ArrayList AttributeList )
        //{
        //    int iApplied = 0;
        //    try
        //    {
        //        Detail MyDetail = new Detail();
        //        if (skWinLib.IsNumeric(sConnData) == true)
        //        {
        //            MyDetail.Number = Convert.ToInt32(sConnData);
        //        }
        //        else
        //        {
        //            MyDetail.Name = sConnData;
        //            MyDetail.Number = -1;
        //        }

        //        MyDetail.AutoDirectionType = AutoDirectionTypeEnum.AUTODIR_FROM_ATTRIBUTE_FILE; 
        //        MyDetail.DetailType = Tekla.Structures.DetailTypeEnum.END; 
        //        // MyDetail.PositionType = PositionTypeEnum.COLLISION_PLANE; 

        //        MyDetail.SetPrimaryObject(Primary);
        //        MyDetail.SetReferencePoint(DetailPoint);
        //        MyDetail.LoadAttributesFromFile(sAttrFile);

        //        if (skWinLib.IsNumeric(sClass) == true)
        //        {
        //            MyDetail.Class = Convert.ToInt32(sClass);
        //        }
        //        MyDetail.Code = sAttrFile;

        //        // MyDetail.DetailType = DetailTypeEnum.END;               
        //        // this format is changed for osha hole 
        //        // skTSLib.UpdateConnectionAttribute(MyDetail, AttributeList);
        //        skTSLib.Model.UpdateDetailAttribute(MyDetail, AttributeList);

        //        bool flag = MyDetail.Insert();

        //        //skTSLib.Model.UpdateDetailAttribute(MyDetail, AttributeList);
        //        //MyDetail.Modify();

        //        if (flag == true)
        //            iApplied = 1;
        //    }  //end of try loop

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Original error: InsertDetail @ " + sConnData + " " + sAttrFile + " " + ex.Message);
        //    }
        //    return iApplied;
        //}

        public static ArrayList GetBoltStandardandSize()
        {
            ArrayList MyBolt = new ArrayList();
            ArrayList MyBoltStandard = new ArrayList();
            ArrayList MyBoltSizeStandard = new ArrayList();
            CatalogHandler CatalogHandler = new CatalogHandler();

            if (CatalogHandler.GetConnectionStatus())
            {
                BoltItemEnumerator BoltItemEnumerator = CatalogHandler.GetBoltItems();

                while (BoltItemEnumerator.MoveNext())
                {
                    BoltItem _BoltItem = BoltItemEnumerator.Current as BoltItem;
                    MyBoltStandard.Add(_BoltItem.Standard.ToString());
                    MyBoltSizeStandard.Add(_BoltItem.Size.ToString());
                }
            }
            MyBolt.Add(MyBoltStandard);
            MyBolt.Add(MyBoltSizeStandard);
            return MyBolt;
        }


        public static bool IsParallel(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {

            Tekla.Structures.Geometry3d.Line Line1 = new Tekla.Structures.Geometry3d.Line(Beam1.StartPoint, Beam1.EndPoint);
            Tekla.Structures.Geometry3d.Line Line2 = new Tekla.Structures.Geometry3d.Line(Beam2.StartPoint, Beam2.EndPoint);
            
            return Tekla.Structures.Geometry3d.Parallel.LineToLine(Line1, Line2);
        }


        public static bool IsParallelBeams(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {

            LineSegment Line1 = new LineSegment(Beam1.StartPoint, Beam1.EndPoint);
            LineSegment Line2 = new LineSegment(Beam2.StartPoint, Beam2.EndPoint);

            return Tekla.Structures.Geometry3d.Parallel.LineSegmentToLineSegment(Line1, Line2);
        }


        public static string GetFramingCondition(Tekla.Structures.Geometry3d.Point MyPoint, Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, System.Windows.Forms.CheckBox CheckWrapAroundFlag, double nodeTolerance)
        {
            string sFramingCondition = "";

            if (CheckWrapAroundFlag.Checked == true)
            {
                if (IsBrace(Secondary) == true)
                {
                    double dAngle = GetAngle(Primary, Secondary);

                    if (dAngle > 20 && dAngle < 70)
                    {
                        Tekla.Structures.Model.Beam Primary3 = GetWebPrimary(Primary, Secondary, Secondaries, nodeTolerance);
                        Tekla.Structures.Model.Beam Primary2 = GetFlangePrimary(Primary, Secondary, Secondaries, nodeTolerance);

                        if (Primary2 != null)
                        {
                            if (Is2Flange(Primary, Secondary) == true)
                            {
                                sFramingCondition = "Wrap Around @ Flange";
                            }
                        }
                        if (Primary3 != null)
                        {
                            if (Is2Flange(Primary, Secondary) == false)
                            {
                                sFramingCondition = "Wrap Around @ Web";
                            }
                        }
                    }
                    else if (dAngle > 85)
                    {

                        Tekla.Structures.Model.Beam Primary3 = GetWebPrimary(Primary, Secondary, Secondaries, nodeTolerance);
                        Tekla.Structures.Model.Beam Primary2 = GetFlangePrimary(Primary, Secondary, Secondaries, nodeTolerance);

                        if (Primary2 != null && Primary3 != null)
                        {
                            if (IsColumn(Primary) == true)
                            {
                                sFramingCondition = "Wrap Around @ Column";
                            }
                            else if (IsBeam(Primary) == true)
                            {
                                sFramingCondition = "Wrap Around @ Beam";
                            }
                        }
                    }
                }
            }

            //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
            //double nodeTolerance = Convert.ToDouble(txtnodetolerance.Text);
            if (sFramingCondition == "")
            {
                if (IsSplice(Primary, Secondary) == true)
                {
                    if (Distance.PointToPoint(Primary.StartPoint, MyPoint) < nodeTolerance || Distance.PointToPoint(Primary.EndPoint, MyPoint) < nodeTolerance)
                    {
                        if (Distance.PointToPoint(Secondary.StartPoint, MyPoint) < nodeTolerance || Distance.PointToPoint(Secondary.EndPoint, MyPoint) < nodeTolerance)
                        {
                            if (IsColumn(Primary) == true && IsColumn(Secondary) == true)
                            {
                                sFramingCondition = "Column to Column Splice";
                            }
                            else
                            {
                                sFramingCondition = "Beam to Beam Splice";
                            }
                        }
                    }
                }
            }

            if (sFramingCondition == "")
            {
                if (IsParallelBeams(Primary, Secondary) == true)
                {
                    sFramingCondition = "Parallel Members";
                }
            }
            if (sFramingCondition == "")
            {
                if (IsColumn(Primary) == true)
                {
                    if (Is2Flange(Primary, Secondary) == true)
                    {
                        sFramingCondition = "Beam to Column Flange";
                    }
                    else
                    {
                        sFramingCondition = "Beam to Column Web";
                    }
                }
                else if (IsColumn(Secondary) == true)
                {
                    if (Is2Flange(Primary, Secondary) == true)
                    {
                        sFramingCondition = "Column to Beam Flange";
                    }
                    else
                    {
                        sFramingCondition = "Column to Beam Web";
                    }
                }
                else if (IsBeam(Primary) == true)
                {
                    if (Is2Flange(Primary, Secondary) == true)
                    {
                        sFramingCondition = "Beam to Beam Flange";
                    }
                    else
                    {
                        sFramingCondition = "Beam to Beam Web";
                    }
                }

                else if (skTSLib.GetDistance(MyPoint, Primary) < nodeTolerance && skTSLib.GetDistance(MyPoint, Secondary) < nodeTolerance)
                {
                    sFramingCondition = "Connected Ends";
                }
            }
            return sFramingCondition;
        }


        public static string GetSingleCondition(Tekla.Structures.Geometry3d.Point MyPoint, Tekla.Structures.Model.Beam Secondary, double Tolerance)
        {
            string sFramingCondition = "";
            //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
            //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);


            if (IsColumn(Secondary) == true)
            {
                if (Secondary.StartPoint.Z - MyPoint.Z > Tolerance || Secondary.EndPoint.Z - MyPoint.Z > Tolerance)
                {
                    sFramingCondition = "Base Plate";
                }
                else
                {
                    sFramingCondition = "Cap Plate";
                }
            }
            else
            {
                sFramingCondition = "Free End";
            }
            return sFramingCondition;
        }

        public static string GetHittingFlange(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, double nodeTolerance)
        {
            string sFramingCondition = "";

            if (Is2TopSide(Primary, Secondary, nodeTolerance) == true)
            {
                sFramingCondition = "Top";
            }
            else
            {
                sFramingCondition = "Bottom";
            }
            return sFramingCondition;
        }

        public static Tekla.Structures.Geometry3d.Point GetMidPoint(Tekla.Structures.Geometry3d.Point Point1, Tekla.Structures.Geometry3d.Point Point2)
        {
            Double X1 = Point1.X;
            Double Y1 = Point1.Y;
            Double Z1 = Point1.Z;
            Double X2 = Point2.X;
            Double Y2 = Point2.Y;
            Double Z2 = Point2.Z;

            Double X = (X2 + X1) / 2.0;
            Double Y = (Y2 + Y1) / 2.0;
            Double Z = (Z2 + Z1) / 2.0;

            return  new Tekla.Structures.Geometry3d.Point(X, Y ,Z);
        }

        private static double CalculateAngleBetweenVectors(Vector v1, Vector v2)
        {
            double dotProduct = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
            double magnitudeV1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y + v1.Z * v1.Z);
            double magnitudeV2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y + v2.Z * v2.Z);

            double angle = Math.Acos(dotProduct / (magnitudeV1 * magnitudeV2));

            // Convert angle from radians to degrees
            angle = angle * (180.0 / Math.PI);

            return angle;
        }

        public static double GetAngleFromPolyBeam(PolyBeam MyPolyBeam)
        {
            if (MyPolyBeam == null || MyPolyBeam.Contour.ContourPoints.Count < 3)
            {
                return -1;
            }

            // Get the first three contour points
            ContourPoint point1 = MyPolyBeam.Contour.ContourPoints[0] as ContourPoint;
            ContourPoint point2 = MyPolyBeam.Contour.ContourPoints[1] as ContourPoint;
            ContourPoint point3 = MyPolyBeam.Contour.ContourPoints[2] as ContourPoint;

            // Calculate vectors
            Vector vector1 = new Vector(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
            Vector vector2 = new Vector(point3.X - point2.X, point3.Y - point2.Y, point3.Z - point2.Z);

            // Calculate the angle between the vectors
            double angle = CalculateAngleBetweenVectors(vector1, vector2);

            return angle;

        }

        public static double GetAngle(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {
            Beam1.Select();
            Beam2.Select();

            Vector Beam1Vector = new Vector(Beam1.GetCoordinateSystem().AxisX);
            Vector Beam2Vector = new Vector(Beam2.GetCoordinateSystem().AxisX);

            double angle = 0.0;
            angle = Beam1Vector.GetAngleBetween(Beam2Vector);
            angle = angle * 180 / Math.PI;
            if (angle > 90)
            {
                angle = 180 - angle;
            }

            return angle;
        }

        public static Tekla.Structures.Model.Beam GetFlangePrimary(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, double nodeTolerance)
        {
            Tekla.Structures.Model.Beam Primary2 = new Tekla.Structures.Model.Beam();
            bool Available = false;
            bool bTop = Is2TopSide(Primary, Secondary, nodeTolerance);


            foreach (Tekla.Structures.Model.Beam MyBeam in Secondaries)
            {
                if (MyBeam != Primary && MyBeam != Secondary)
                {
                    if (Is2TopSide(Primary, MyBeam, nodeTolerance) == bTop && Is2Flange(Primary, MyBeam) == true)
                    {
                        if (IsColumn(MyBeam) == true || IsBeam(MyBeam) == true)
                        {
                            Primary2 = MyBeam;
                            Available = true;
                        }
                    }
                }
            }

            if (Available == true)
            {
                return Primary2;
            }
            else
            {
                return null;
            }
        }

        public static Tekla.Structures.Model.Beam GetWebPrimary(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, double nodeTolerance)
        {
            Tekla.Structures.Model.Beam Primary2 = new Tekla.Structures.Model.Beam();
            bool Available = false;
            bool bFront = Is2FrontSide(Primary, Secondary, nodeTolerance);

            foreach (Tekla.Structures.Model.Beam MyBeam in Secondaries)
            {
                if (MyBeam != Primary && MyBeam != Secondary)
                {
                    if (Is2FrontSide(Primary, MyBeam, nodeTolerance) == bFront && Is2Flange(Primary, MyBeam) == false)
                    {
                        if (IsColumn(MyBeam) == true || IsBeam(MyBeam) == true)
                        {
                            Primary2 = MyBeam;
                            Available = true;
                        }
                    }
                }
            }

            if (Available == true)
            {
                return Primary2;
            }
            else
            {
                return null;
            }
        }


        public static bool IsRightSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, Tekla.Structures.Geometry3d.Point MyPoint, double nodeTolerance)
        {
            //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
            //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
            bool condition = false;
            try
            {  // model connection is not necessary
               //Set up model Component w/TS

                //MyModel = new TSM.Model();
                //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {
                    TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();

                    if (Beam2.StartPoint.X - MyPoint.X > nodeTolerance || Beam2.EndPoint.X - MyPoint.X > nodeTolerance)
                    {
                        condition = true;
                    }

                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();
                } // end of if that checks to see if there is a model and one is open
                else
                {
                    //If no Component to model is possible show error message
                    MessageBox.Show("Tekla Structures not open or model not open.",
                        "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }  //end of try loop

            catch (Exception ex)
            {
                MessageBox.Show("Original error: IsRightSide " + ex.Message);
            }

            return condition;
        }

        public static bool Is2FrontSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, double nodeTolerance)
        {
            //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
            //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
            bool condition = false;
            try
            {  // model connection is not necessary
               //Set up model Component w/TS
               //MyModel = new TSM.Model();
               //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {
                    //240719
                    TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();

                    if (Beam2.StartPoint.Z > nodeTolerance || Beam2.EndPoint.Z > nodeTolerance)
                    {
                        condition = true;
                    }

                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();
                } // end of if that checks to see if there is a model and one is open
                else
                {
                    //If no Component to model is possible show error message
                    MessageBox.Show("Tekla Structures not open or model not open.",
                        "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }  //end of try loop

            catch (Exception ex)
            {
                MessageBox.Show("Original error: IS2FRONTSIDE " + ex.Message);
            }

            return condition;

        }



        public static bool Is2TopSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, double nodeTolerance)
        {
            //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
            //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
            bool condition = false;
            try
            {  // model connection is not necessary
               //Set up model Component w/TS
                //MyModel = new TSM.Model();
                //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {
                    TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();

                    if (Beam2.StartPoint.Y > nodeTolerance || Beam2.EndPoint.Y > nodeTolerance)
                    {
                        condition = true;
                    }
                    // i did it because the current is changes the nodepoint
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                    TSMUI.ViewHandler.RedrawWorkplane();
                    Beam2.Select();

                } // end of if that checks to see if there is a model and one is open
                else
                {
                    //If no Component to model is possible show error message
                    MessageBox.Show("Tekla Structures not open or model not open.",
                        "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }  //end of try loop

            catch (Exception ex)
            {
                MessageBox.Show("Original error: IS2TOPSIDE" + ex.Message);
            }

            return condition;

        }

        public static bool IsSecondaryLower(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {
            double ZValue = 0;

            if (Beam1.StartPoint.Z > Beam1.EndPoint.Z)
            {
                ZValue = Beam1.StartPoint.Z;
            }
            else
            {
                ZValue = Beam1.EndPoint.Z;
            }

            if (Beam2.StartPoint.Z < ZValue || Beam2.EndPoint.Z < ZValue)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsSplice(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {
            if (Beam1 != null && Beam2 != null)
            {
                CoordinateSystem cs1 = Beam2.GetCoordinateSystem();
                CoordinateSystem cs2 = Beam1.GetCoordinateSystem();  // for primary

                double dot = Vector.Dot(cs1.AxisX.GetNormal(), cs2.AxisX.GetNormal());

                bool Connected = false;

                if (Distance.PointToPoint(Beam1.StartPoint, Beam2.StartPoint) < 10)
                {
                    Connected = true;
                }
                else if (Distance.PointToPoint(Beam1.StartPoint, Beam2.EndPoint) < 10)
                {
                    Connected = true;
                }
                else if (Distance.PointToPoint(Beam1.EndPoint, Beam2.EndPoint) < 10)
                {
                    Connected = true;
                }
                else if (Distance.PointToPoint(Beam1.EndPoint, Beam2.StartPoint) < 10)
                {
                    Connected = true;
                }

                if (Math.Abs(dot) > 0.95 && Connected == true)  // Hameed you can reduce for commercial jobs
                {
                    return true;
                }
            }
            return false;

        }

        public static bool IsWebPerpendicular(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {
            if (Beam1 != null && Beam2 != null)
            {

                CoordinateSystem cs1 = Beam1.GetCoordinateSystem();  // for primary
                CoordinateSystem cs2 = Beam2.GetCoordinateSystem();

                double dot = Vector.Dot(cs2.AxisY.GetNormal(), cs1.AxisX.GetNormal());

                if (Math.Abs(dot) < 0.5)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Is2Flange(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
        {
            if (Beam1 != null && Beam2 != null)
            {
                CoordinateSystem cs1 = Beam1.GetCoordinateSystem();  // for primary
                CoordinateSystem cs2 = Beam2.GetCoordinateSystem();
   
                double dot = Vector.Dot(cs2.AxisX.GetNormal(), cs1.AxisY.GetNormal());

                if (Math.Abs(dot) > 0.2)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsBrace(Tekla.Structures.Model.Beam MyBeam)
        {
            if (IsColumn(MyBeam) == false && IsBeam(MyBeam) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsBeam(Tekla.Structures.Model.Beam MyBeam)
        {
            MyBeam.Select();

            Double douXS = MyBeam.StartPoint.X;
            Double douYS = MyBeam.StartPoint.Y;
            Double douZS = MyBeam.StartPoint.Z;
            Double douXE = MyBeam.EndPoint.X;
            Double douYE = MyBeam.EndPoint.Y;
            Double douZE = MyBeam.EndPoint.Z;

            if (Math.Abs(douXS - douXE) < 10 && Math.Abs(douZS - douZE) < 10 || Math.Abs(douYS - douYE) < 10 && Math.Abs(douZS - douZE) < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool IsColumn(Tekla.Structures.Model.Beam MyBeam)
        {
            MyBeam.Select();

            Double douXS = MyBeam.StartPoint.X;
            Double douYS = MyBeam.StartPoint.Y;
            Double douZS = MyBeam.StartPoint.Z;
            Double douXE = MyBeam.EndPoint.X;
            Double douYE = MyBeam.EndPoint.Y;
            Double douZE = MyBeam.EndPoint.Z;

            if (Math.Abs(douYS - douYE) < 10 && Math.Abs(douXS - douXE) < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsDetailAvailable(Tekla.Structures.Geometry3d.Point MyPoint, string sComponent)
        {
            bool Available = false;
            try
            {
                //Set up model Component w/TS
                //MyModel = new TSM.Model();
                //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {

                    Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                    Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                    MinP.X = MinP.X - 50;
                    MinP.Y = MinP.Y - 50;
                    MinP.Z = MinP.Z - 50;

                    MaxP.X = MaxP.X + 50;
                    MaxP.Y = MaxP.Y + 50;
                    MaxP.Z = MaxP.Z + 50;

                    ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                    while (CrossEnum.MoveNext())
                    {
                        Detail Crossing = CrossEnum.Current as Detail;

                        if (Crossing != null && Crossing.Name == sComponent)
                        {
                            Available = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ISDETAILAVAILABLE. Original error: " + ex.Message);
            }
            return Available;
        }

        public static Tekla.Structures.Model.Beam GetPrimary(Tekla.Structures.Geometry3d.Point MyPoint, List<Tekla.Structures.Model.Beam> MyBeams)
        {

            if (MyBeams != null)
            {
                if (MyBeams.Count == 1)
                    return MyBeams[0];
                else
                {
                    Tekla.Structures.Model.Beam MyreturnBeam  = MyBeams[0];
                    double ht = 0;

                    foreach (Tekla.Structures.Model.Beam MyBeam in MyBeams)
                    {
                        //check for column
                        if (IsColumn(MyBeam) == true)
                            MyreturnBeam = MyBeam;
                        double distToStartPoint = Distance.PointToPoint(MyBeam.StartPoint, MyPoint);
                        double distToEndPoint = Distance.PointToPoint(MyBeam.EndPoint, MyPoint);
                        if ((distToStartPoint > 20.0) && (distToEndPoint > 20.0))
                            return MyBeam;

                        //in case both member joins at corner heigher height beam is considered as primary
                        double bmht = 0;
                        MyBeam.GetReportProperty("HEIGHT", ref bmht);
                        if (bmht > ht )
                        {
                            ht = bmht;
                            MyreturnBeam = MyBeam;
                        }
                    }
                    return MyreturnBeam;
                }
            }

            return null;
        }

        //  public static List<Tekla.Structures.Model.Beam> GetCrossParts(Tekla.Structures.Geometry3d.Point MyPoint, double Tolerance, string sIgnoreClass, string sIgnoreName)
        public static List<Beam> GetCrossParts(Tekla.Structures.Geometry3d.Point MyPoint, double Tolerance, string sIgnoreClass, string sIgnoreName, TSMUI.View ActiveView = null)
        {
            //change PoBm
            List<Beam> CrossParts = new List<Beam>();
            try
            {

                //Set up model Component w/TS
                //MyModel = new TSM.Model();
                //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {

                    Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                    Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                    MinP.X = MinP.X - Tolerance;
                    MinP.Y = MinP.Y - Tolerance;
                    MinP.Z = MinP.Z - Tolerance;

                    MaxP.X = MaxP.X + Tolerance;
                    MaxP.Y = MaxP.Y + Tolerance;
                    MaxP.Z = MaxP.Z + Tolerance;
                    
                    ModelObjectEnumerator CrossEnum = null;;
                    if (ActiveView == null)
                        CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
                    else
                    {
                        //TSM.UI.ModelObjectSelector _selector = new TSM.UI.ModelObjectSelector().GetObjectsByBoundingBox;
                        CrossEnum = new TSM.UI.ModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP, ActiveView);

                    }
           

                    //drawer.DrawText(MaxP, "Max", new TSMUI.Color(0, 1, 0));
                    //drawer.DrawText(MinP, "Min", new TSMUI.Color(0, 1, 0));

                    //if (debugflag == true)
                    //{
                    //    //drawer.DrawText(MaxP, "Max", new TSMUI.Color(0, 1, 0));
                    //    //drawer.DrawText(MinP, "Min", new TSMUI.Color(0, 1, 0));
                    //}


                    while (CrossEnum.MoveNext())
                    {
                        Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                        if (CrossBeam != null)
                        {
                            string PProfileType = "";
                            CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);
                            if (PProfileType != "B" && sIgnoreClass.Contains(CrossBeam.Class) == false && sIgnoreName.Trim().ToUpper().Replace(" ", "").Contains(CrossBeam.Name.Trim().ToUpper().Replace(" ", "")) == false)
                            //if (PProfileType != "B")
                            {
                                CrossParts.Add(CrossBeam);
                            }
                        }
                        else
                        {
                            Tekla.Structures.Model.PolyBeam CrossPolyBeam = CrossEnum.Current as Tekla.Structures.Model.PolyBeam;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetCrossParts. Original error: " + ex.Message);
            }
            return CrossParts;
        }


        public static ArrayList GetCrossPartsID(Tekla.Structures.Geometry3d.Point MyPoint, Double Tolerance, TSMUI.View ActiveView = null)
        {
            ArrayList CrossParts = new ArrayList();

            try
            {
                // Set up model Component w/TS
                //MyModel = new TSM.Model();
                //ModelPath = MyModel.GetInfo().ModelPath;
                if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
                {

                    Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                    Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                    MinP.X = MinP.X - Tolerance;
                    MinP.Y = MinP.Y - Tolerance;
                    MinP.Z = MinP.Z - Tolerance;

                    MaxP.X = MaxP.X + Tolerance;
                    MaxP.Y = MaxP.Y + Tolerance;
                    MaxP.Z = MaxP.Z + Tolerance;


                    ModelObjectEnumerator CrossEnum = null; ;
                    if (ActiveView == null)
                        CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
                    else
                    {
                        //TSM.UI.ModelObjectSelector _selector = new TSM.UI.ModelObjectSelector().GetObjectsByBoundingBox;
                        CrossEnum = new TSM.UI.ModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP, ActiveView);

                    }
                    //ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                    while (CrossEnum.MoveNext())
                    {
                        Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                        if (CrossBeam != null)
                        {
                            string PProfileType = "";
                            CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);

                            if (PProfileType != "B")
                            {
                                CrossParts.Add(CrossBeam.Identifier.ID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetCrossPartsID. Original error: " + ex.Message);
            }
            return CrossParts;
        }

        //public static bool UpdateConnectionAttribute(Tekla.Structures.Model.BaseComponent Component, ArrayList AttributeList)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        foreach (string Attribute in AttributeList)
        //        {
        //            if (Attribute.IndexOf("|") >= 0)
        //            {
        //                string[] split = Attribute.Split(new Char[] { '|' });
        //                if (split.Count() >= 2)
        //                {
        //                    string attribute_type = split[1].Trim().Substring(0, 2).ToUpper();
        //                    if (attribute_type == "ST")
        //                    {
        //                        Component.SetAttribute(split[0], split[2]);
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "IN")
        //                    {
        //                        Component.SetAttribute(split[0], Convert.ToInt32(split[2]));
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "DO")
        //                    {
        //                        Component.SetAttribute(split[0], Convert.ToDouble(split[2]));
        //                        flag = true;
        //                    }

        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        //    }
        //    return flag;
        //}
        //public static bool UpdateConnectionAttribute(Tekla.Structures.Model.BaseComponent Component, ArrayList AttributeList)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        foreach (string Attribute in AttributeList)
        //        {
        //            if (Attribute.IndexOf("|") >= 0)
        //            {
        //                string[] split = Attribute.Split(new Char[] { '|' });
        //                if (split.Count() >= 2)
        //                {
        //                    string attribute_type = split[1].Trim().Substring(0, 2).ToUpper();
        //                    if (attribute_type == "ST")
        //                    {
        //                        Component.SetAttribute(split[0], split[2]);
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "IN")
        //                    {
        //                        Component.SetAttribute(split[0], Convert.ToInt32(split[2]));
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "DO")
        //                    {
        //                        Component.SetAttribute(split[0], Convert.ToDouble(split[2]));
        //                        flag = true;
        //                    }

        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        //    }
        //    return flag;
        //}

        //public static bool UpdateDetailAttribute(Tekla.Structures.Model.Detail MyDetail, ArrayList AttributeList)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        foreach (string Attribute in AttributeList)
        //        {
        //            if (Attribute.IndexOf("|") >= 0)
        //            {
        //                string[] split = Attribute.Split(new Char[] { '|' });
        //                if (split.Count() >= 2)
        //                {
        //                    string attribute_type = split[1].Trim().Substring(0, 2).ToUpper();
        //                    if (attribute_type == "ST")
        //                    {
        //                        MyDetail.SetAttribute(split[0], split[2]);
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "IN")
        //                    {
        //                        MyDetail.SetAttribute(split[0], Convert.ToInt32(split[2]));
        //                        flag = true;
        //                    }
        //                    else if (attribute_type == "DO")
        //                    {
        //                        MyDetail.SetAttribute(split[0], Convert.ToDouble(split[2]));
        //                        flag = true;
        //                    }

        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        //    }
        //    return flag;
        //}




        //public static bool UpdateConnectionAttribute(Tekla.Structures.Model.BaseComponent MyComponent, ArrayList AttributeList)
        //{
        //    bool flag = false;
        //    if (AttributeList != null)
        //    {
        //        try
        //        {
        //            for (int i = 7; i < AttributeList.Count; i++)
        //            {
        //                ArrayList XSData = AttributeList[i] as ArrayList;
        //                if (XSData.Count >= 4)
        //                {
        //                    string attribute_fieldname = XSData[1] as string;
        //                    string attribute_fieldtype = XSData[2] as string;
        //                    string attribute_fieldvalue = XSData[3] as string;
        //                    if (attribute_fieldvalue != "||" && attribute_fieldvalue.Trim().Length >= 1)
        //                    {
        //                        string attribute_type = attribute_fieldtype.Substring(0, 2).ToUpper().Trim();
        //                        if (attribute_type == "ST")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, attribute_fieldvalue);
        //                            flag = true;
        //                        }
        //                        else if (attribute_type == "IN")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, Convert.ToInt32(attribute_fieldvalue));
        //                            flag = true;
        //                        }
        //                        else if (attribute_type == "DO")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, Convert.ToDouble(attribute_fieldvalue));
        //                            flag = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        MyComponent.SetAttribute(attribute_fieldname, attribute_fieldvalue);
        //                        flag = true;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        //        }
        //    }

        //    return flag;
        //}
        //public static bool UpdateConnectionAttribute(Tekla.Structures.Model.BaseComponent MyComponent, ArrayList AttributeList)
        //{
        //    bool flag = false;
        //    if (AttributeList !=null)
        //    {
        //        try
        //        {
        //            for (int i = 7; i < AttributeList.Count; i++)
        //            {
        //                ArrayList XSData = AttributeList[i] as ArrayList;
        //                if (XSData.Count >= 4)
        //                {
        //                    string attribute_fieldname = XSData[1] as string;
        //                    string attribute_fieldtype = XSData[2] as string;
        //                    string attribute_fieldvalue = XSData[3] as string;
        //                    if (attribute_fieldvalue != "||" && attribute_fieldvalue.Trim().Length >= 1)
        //                    {
        //                        string attribute_type = attribute_fieldtype.Substring(0, 2).ToUpper().Trim();
        //                        if (attribute_type == "ST")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, attribute_fieldvalue);
        //                            flag = true;
        //                        }
        //                        else if (attribute_type == "IN")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, Convert.ToInt32(attribute_fieldvalue));
        //                            flag = true;
        //                        }
        //                        else if (attribute_type == "DO")
        //                        {
        //                            MyComponent.SetAttribute(attribute_fieldname, Convert.ToDouble(attribute_fieldvalue));
        //                            flag = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        MyComponent.SetAttribute(attribute_fieldname, attribute_fieldvalue);
        //                        flag = true;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Actual Error: " + ex.ToString(), "Connection Attribute Excel Error");
        //        }
        //    }

        //    return flag;
        //}

        //public static bool CheckAutoConnectionAttribute(string modelpath, string sAttrFile, string sConnData)
        //{

        //    bool flag = false;
        //    string sFolder = modelpath + "\\attributes";
        //    //check wether + or [ exists

        //    //other
        //    if ((!sConnData.Contains("+") == true) && (!sAttrFile.Contains("+") == true))
        //    {
        //        if ((sConnData.Contains("][") == false) && (sAttrFile.Contains("][") == false))
        //        {

        //            if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, sAttrFile, sConnData) == false)
        //            {
        //                lvItem.SubItems[6].BackColor = Color.Red;
        //                lvItem.SubItems[7].BackColor = Color.Red;
        //            }
        //            else
        //            {
        //                lvItem.SubItems[6].BackColor = Color.Green;
        //                lvItem.SubItems[7].BackColor = Color.Green;
        //                chkct++;
        //            }
        //            goto nextItem;
        //        }

        //        //check for simple condition example [146][146] , [Rhs][Lhs]
        //        if ((sConnData.Contains("][") == true) && (sAttrFile.Contains("][") == true))
        //        {
        //            //for two
        //            string[] splitConn = sConnData.Split(new string[] { "][" }, StringSplitOptions.None);
        //            string[] splitAttr = sAttrFile.Split(new string[] { "][" }, StringSplitOptions.None);
        //            int connct = splitConn.Count();
        //            int attct = splitAttr.Count();
        //            if (connct == 2 && attct == 2)
        //            {
        //                //sConnData = sConnData.Replace("[", "");
        //                //sConnData = sConnData.Replace("]", "");
        //                //sConnData = sConnData.Replace("}", "");
        //                //sConnData = sConnData.Replace("{", "");
        //                //sConnData = sConnData.Replace("&", "+");
        //                //sConnData = sConnData.Replace(" +", "+");
        //                //sConnData = sConnData.Replace("+ ", "+");
        //                //Check Front Condition 
        //                if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, ReplaceAttribute(splitAttr[1]), ReplaceAttribute(splitConn[1])) == false)
        //                {
        //                    lvItem.SubItems[6].BackColor = Color.Red;
        //                    lvItem.SubItems[7].BackColor = Color.Red;
        //                    goto nextItem;
        //                }
        //                //Check Rear Condition 
        //                if (skTSLib.Model.checkAttributeFile(skTSLib.ModelPath, ReplaceAttribute(splitAttr[0]), ReplaceAttribute(splitConn[0])) == false)
        //                {
        //                    lvItem.SubItems[6].BackColor = Color.Red;
        //                    lvItem.SubItems[7].BackColor = Color.Red;
        //                    goto nextItem;
        //                }
        //                lvItem.SubItems[6].BackColor = Color.Green;
        //                lvItem.SubItems[7].BackColor = Color.Green;
        //                chkct++;
        //                chkct++;
        //                goto nextItem;

        //            }
        //        }


        //    }

        //    /*
        //    if ((sConnData.Contains("+") == true) && (sAttrFile.Contains("+") == true))
        //    {
        //        //for two or more
        //        string[] splitConn = sConnData.Split(new string[] { "+" }, StringSplitOptions.None);
        //        string[] splitAttr = sAttrFile.Split(new string[] { "+" }, StringSplitOptions.None);
        //        int connct = splitConn.Count();
        //        int attct = splitAttr.Count();
        //        if (connct == attct)
        //        {
        //            for (int chk = 0; chk < connct; chk++)
        //            {
        //                if (checkAttributeFile(skTSLib.ModelPath, splitAttr[chk], splitConn[chk])==false)
        //                {
        //                    lvItem.SubItems[6].BackColor = Color.Red;
        //                    lvItem.SubItems[7].BackColor = Color.Red;
        //                    goto nextItem;
        //                }
        //            }

        //        }
        //        if (connct != attct)
        //        {
        //            for (int chk = 0; chk < connct; chk++)
        //            {
        //                if (checkAttributeFile(skTSLib.ModelPath, splitAttr[chk], splitConn[chk]) == false)
        //                {
        //                    lvItem.SubItems[6].BackColor = Color.Red;
        //                    lvItem.SubItems[7].BackColor = Color.Red;
        //                    goto nextItem;
        //                }
        //            }

        //        }

        //    }
        //    else if ((sConnData.Contains("+") == true) && (sAttrFile.Contains("+") == false))
        //    {
        //        string[] splitConn = sConnData.Split(new string[] { "+" }, StringSplitOptions.None);
        //        int connct = splitConn.Count();
        //        for (int chk = 0; chk < connct; chk++)
        //        {
        //            CheckAttribute(splitConn[chk], sAttrFile, lvItem);
        //            if (lvItem.SubItems[6].BackColor == Color.Red || lvItem.SubItems[7].BackColor == Color.Red)
        //                goto nextItem;
        //        }


        //    }
        //    else if ((sConnData.Contains("+") == false) && (sAttrFile.Contains("+") == true))
        //    {
        //        string[] splitAttr = sAttrFile.Split(new string[] { "+" }, StringSplitOptions.None);
        //        int attct = splitAttr.Count();
        //        for (int chk = 0; chk < attct; chk++)
        //        {
        //            CheckAttribute(sConnData, splitAttr[chk], lvItem);
        //            if (lvItem.SubItems[6].BackColor == Color.Red || lvItem.SubItems[7].BackColor == Color.Red)
        //                goto nextItem;
        //        }
        //    }
        //    else if ((sConnData.Contains("+") == false) && (sAttrFile.Contains("+") == false))
        //    {
        //            CheckAttribute(sConnData, sAttrFile, lvItem);
        //            if (lvItem.SubItems[6].BackColor == Color.Red || lvItem.SubItems[7].BackColor == Color.Red)
        //                goto nextItem;
        //    }
        //    */
        //    //for [186+11+11][143+11], [att2+at2][stt2+sss3] to be modified
        //    //for [186][186], [att2]
        //    string sFile = attributefile + ".j" + componentnumber.ToString();

        //    string[] file_Paths = Directory.GetFiles(sFolder, sFile, SearchOption.TopDirectoryOnly);

        //    if (file_Paths.Count() >= 1)
        //        flag = true;

        //    return flag;
        //}
        public static bool checkAttributeFile(string modelpath, string CheckFile)
        {

            string CheckFolder = modelpath + "\\attributes";
            if (System.IO.Directory.Exists(CheckFolder) == true)
            {
                string[] GetFiles = Directory.GetFiles(CheckFolder, CheckFile, SearchOption.TopDirectoryOnly);

                if (GetFiles.Count() >= 1)
                    return true;
            }

            return false;
        }
        public static bool checkAttributeFile(string modelpath, string ConnectionFile, string ConnectionNumber)
        {

            if (ConnectionFile.Trim().Length >=1 && ConnectionNumber.Trim().Length >= 2)
            {
                string CheckFolder = modelpath + "\\attributes";
                if (System.IO.Directory.Exists(CheckFolder) == true)
                {

                    string CheckFile = string.Empty;
                    if (ConnectionNumber.ToUpper().Substring(0, 2) == "P_")
                    {
                        if (IsCustomComponentExist(ConnectionNumber) == true)
                        {
                            if (ConnectionFile == "-1")
                                return true;
                            else
                                CheckFile = ConnectionFile + "." + ConnectionNumber.ToString(); //check for custom component
                        }
                        else
                            return false;
                        
                    }
                    else
                        CheckFile = ConnectionFile + ".j" + ConnectionNumber.ToString();

                    string[] GetFiles = Directory.GetFiles(CheckFolder, CheckFile, SearchOption.TopDirectoryOnly);
                    if (GetFiles.Count() >= 1)
                        return true;
                    else
                    {
                        TeklaStructuresFiles MyTeklaStructuresFiles = new TeklaStructuresFiles(modelpath);
                        List<string> Files = MyTeklaStructuresFiles.GetMultiDirectoryFileList(CheckFile, false);
                        if (Files.Count() >= 1)
                            return true;
                    }

                }
            }
            return false;
        }
        public static LineSegment GetIntersection(Beam Beam1, Beam Beam2)
        {

            Tekla.Structures.Geometry3d.Line Line1 = new Tekla.Structures.Geometry3d.Line(Beam1.StartPoint, Beam1.EndPoint);
            Tekla.Structures.Geometry3d.Line Line2 = new Tekla.Structures.Geometry3d.Line(Beam2.StartPoint, Beam2.EndPoint);
            LineSegment Interseg = null;

            if (Tekla.Structures.Geometry3d.Parallel.LineToLine(Line1, Line2) == false)
            {
                Interseg = Intersection.LineToLine(Line1, Line2);
            }
            return Interseg;
        }

        public static Tekla.Structures.Geometry3d.Point GetIntersectionPoint(Tekla.Structures.Geometry3d.Point A, Tekla.Structures.Geometry3d.Point B, Tekla.Structures.Geometry3d.Point C, Tekla.Structures.Geometry3d.Point D)
        {
            //// Line AB represented as a1x + b1y = c1
            //double x1 = A.X;
            //double y1 = A.Y;

            //double x2 = B.X;
            //double y2 = B.Y;

            //double x3 = C.X;
            //double y3 = C.Y;

            //double x4 = D.X;
            //double y4 = D.Y;

            //double determinant = ((x1 - x2) * (y3 - y4)) - ((y1 - y4)*(x3 - x4));
            //double xab1 = ((x1 * y2) - (y1*x2)) * (x3 - x4);
            //double xab2 = ((x1 - x2) * (x3*y4) - (y3*x4));

            //double yab1 = ((x1 * y2) - (y1 * x2)) * (y3 - y4);
            //double yab2 = ((y1 - y2) * (x3 * y4) - (y3 * x4));

            //double X = (xab1 - xab2) / determinant;

            //double Y = (yab1 - yab2) / determinant;
            //if (determinant != 0)
            //    return new Tekla.Structures.Geometry3d.Point(X, Y);
            //else
            //    return new Tekla.Structures.Geometry3d.Point(double.MaxValue, double.MaxValue);
            //// Line AB represented as a1x + b1y = c1
            //double a1 = B.Y - A.Y;
            //double b1 = A.X - B.X;
            //double c1 = a1 * (A.X) + b1 * (A.Y);

            //// Line CD represented as a2x + b2y = c2
            //double a2 = D.Y - C.Y;
            //double b2 = C.X - D.X;
            //double c2 = a2 * (C.X) + b2 * (C.Y);

            //double determinant = a1 * b2 - a2 * b1;

            //if (determinant != 0)
            //{
            //    double x = (b2 * c1 - b1 * c2) / determinant;
            //    double y = (a1 * c2 - a2 * c1) / determinant;
            //    return new Tekla.Structures.Geometry3d.Point(x, y);
            //}
            //return new Tekla.Structures.Geometry3d.Point(double.MaxValue, double.MaxValue);

            /// define P1 and P2 as points for line 1;
            //                    //define P3 and P4 as points for line 2;
            //                    Point P1 =
            //                    Point P2 =
            //                    Point P3 =
            //                    Point P4 =

            //                    //vector Line1 = P1+a*r1
            //                    //vector Line2 = P3+a*r1 - also can be P4-a*r1 if intersection is at P3
            //                    //use boundary conditions and direct vectors appropriately for your use
            Vector r1 = new Vector(B - A);
            Vector r2 = new Vector(D - C);
            Vector r3 = new Vector(C - A);
            Vector rr = r1.Cross(r2);
            Vector rrp = r3.Cross(r2);
            Vector rrn = rr.Cross(rrp);
            double a = 0;

            //when two lines intersect vector rrn is a zero vector meaning the rr and rrp are parallel,
            //and vector rr cannot be a 0 vector
            //if (rrn.Equals(0) && !rr.Equals(0))
            if (rrn == new Vector(0,0,0)  && rr != new Vector(0, 0, 0))
            {
                double v1 = Math.Sqrt(Math.Pow(rrp.X, 2) + Math.Pow(rrp.Y, 2) + Math.Pow(rrp.Z, 2));
                double v2 = Math.Sqrt(Math.Pow(rr.X, 2) + Math.Pow(rr.Y, 2) + Math.Pow(rr.Z, 2));
                a = v1 / v2;

                //if rrp is opposite of rr then value of a = -a
                //if (rrp.X.Equals(rr.X * -1) & rrp.Y.Equals(rr.Y * -1) & rrp.Z.Equals(rr.Z * -1))
                if (rrp.X == (rr.X * -1) && rrp.Y == (rr.Y * -1) && rrp.Z == (rr.Z * -1))
                {
                    a = a * -1;
                }
                Tekla.Structures.Geometry3d.Point intpt1 = A + (a * r1);
                Tekla.Structures.Geometry3d.Point r2pt = new Tekla.Structures.Geometry3d.Point(r2.X, r2.Y, r2.Z);
                Tekla.Structures.Geometry3d.Point bpt = (intpt1 - D);
                double b1 = Math.Sqrt(Math.Pow(bpt.X, 2) + Math.Pow(bpt.Y, 2) + Math.Pow(bpt.Z, 2));
                double b2 = Math.Sqrt(Math.Pow(r2.X, 2) + Math.Pow(r2.Y, 2) + Math.Pow(r2.Z, 2));
                double b = b1 / b2;
                Tekla.Structures.Geometry3d.Point intpt2 = D - (b * r2);

                //here are the boundary conditions, I used 1.05 just in case one of 
                //the lines is really close and they aren't actually intersecting
                //intpt1 is the intersection point determined using variable a and vector line equation L1=P1+a*r1
                //intpt1 must equal intpt2 and the parameters a & b must be smaller than 1 (indicating that the intersection
                //actually happens on the lines and not projected outside the lines)

                //if (intpt1 == C && intpt2 == C && a <= 1.05 && b <= 1.05)

                if (a <= 1.05 && b <= 1.05)
                {
                    //IntersectedBeam = BM2;
                    return intpt2;
                }
                else
                    return null;

            }
            return new Tekla.Structures.Geometry3d.Point(double.MaxValue, double.MaxValue);
        }

        public static double GetDistance(Tekla.Structures.Geometry3d.Point refPoint, Beam Beam2)
        {
            LineSegment Lineseg = new LineSegment();

            Lineseg.Point1 = Beam2.StartPoint;
            Lineseg.Point2 = Beam2.EndPoint;

            double intDistance = Distance.PointToLineSegment(refPoint, Lineseg);

            return intDistance;
        }

    }
    public static bool SetBackColor_Viewer_Drafter(System.Windows.Forms.Button MyButton, System.Drawing.Color MyColor)
    {
        if (skTSLib.Configuration.ToUpper().Contains("VIEWER") == true || skTSLib.Configuration.ToUpper().Contains("DRAFT") == true || skTSLib.Configuration.ToUpper().Contains("CARBON") == true)
        {
            MyButton.BackColor = MyColor;
            return true;
        }
        return false;
    }

//220503

public static string GetUserFieldValue(Beam MyBeam, string AttributeName)
    {
        string sValue = string.Empty;
        MyBeam.GetReportProperty(AttributeName, ref sValue);
        if (sValue.Trim().Length >= 1)
            return sValue;
        else
        {
            //check for double
            double dValue = -2147483648.000000;
            MyBeam.GetReportProperty(AttributeName, ref dValue);
            if (dValue != -2147483648.000000)
            {
                double kipchange = 4448.2222;
                return (dValue / kipchange).ToString("#0.0#");
            }
            else
            {
                //check for int
                int iValue = -2147483648;
                MyBeam.GetReportProperty(AttributeName, ref iValue);
                if (iValue != -2147483648)
                    return iValue.ToString();
            }
        }
        return "";
    }
    public static bool CheckCondition(string Profile, string SearchCondition)
    {
        //replace any space and trim
        string chkProfile = Profile.ToUpper().Trim().Replace(" ", "");
        string chkSearchCondition = SearchCondition.ToUpper().Trim().Replace(" ", "");

        //check1
        //Profile W16X40
        //SearchCondition W16X40
        if (chkProfile == chkSearchCondition)
            return true;

        //check2
        //Profile W16X40
        //SearchCondition1 ie. W16* ,W18*, W21* multiple
        if (chkSearchCondition.IndexOf(",") >= 1)
        {
            string[] splitsearch = chkSearchCondition.Split(new Char[] { ',' }); ;
            for (int i = 0; i < splitsearch.Length; i++)
            {
                if (splitsearch[i].IndexOf("*") >= 1)
                {
                    string tmp = splitsearch[i].Replace("*", "");
                    if (chkProfile.Contains(tmp))
                        return true;
                }
                else
                {
                    if (chkProfile == splitsearch[i])
                        return true;
                }
            }
        }

        //check3
        //Profile W16X40
        //SearchCondition2 W10* (single)
        if (chkSearchCondition.IndexOf("*") >= 1)
        {
            string tmp = chkSearchCondition.Replace("*", "");
            if (chkProfile.Contains(tmp))
                return true;
        }
        else
        {
            if (chkProfile == chkSearchCondition)
                return true;
        }

        //check4
        //Profile W16X40
        //SearchCondition ""
        if (chkSearchCondition.Length == 0)
            return true;

        //check5
        //Profile W16X40
        //SearchCondition "All"
        if (chkSearchCondition == "ALL")
            return true;

        return false;
    }

    public static bool CheckMultipleConnectionCondition(string MProject, string MSecondaryProfile, string MPrimaryProfile, string MConnConfiguration, string MConnCode, string MComponent, string MAttributeFile, string XLComponent, string XLProject, string XLAttributeFile, string XLSecondaryProfile, string XLPrimaryProfile, string XLConnConfiguration, string XLConnCode)
    {
        //Check the condition and get the attribute data
        if (
            (skTSLib.CheckCondition(MProject, XLProject) == true)
            && (skTSLib.CheckCondition(MSecondaryProfile, XLSecondaryProfile) == true)
            && (skTSLib.CheckCondition(MPrimaryProfile, XLPrimaryProfile) == true)
            && (skTSLib.CheckCondition(MConnConfiguration, XLConnConfiguration) == true)
            && (skTSLib.CheckCondition(MConnCode, XLConnCode) == true)
            && (skTSLib.CheckCondition(MComponent, XLComponent) == true)
            && (skTSLib.CheckCondition(MAttributeFile, XLAttributeFile) == true)
         )
            return true;
        else
            return false;

    }

   
    public static ArrayList getAttributeData(ArrayList AutoConnectionData, string MProject, string MSecondaryProfile, string MPrimaryProfile, string MConnConfiguration, string MConnCode, string MComponent, string MAttributeFile)
    {
        
        ArrayList AttributeData = new ArrayList();
        try
        {
            for (int i = 0; i < AutoConnectionData.Count; i++)
            {
                ArrayList mydata = AutoConnectionData[i] as ArrayList;
                if (mydata.Count >= 6)
                {
                    string XLComponent = mydata[0] as string;
                    string XLProject = mydata[1] as string;
                    string XLAttributeFile = mydata[2] as string;
                    string XLSecondaryProfile = mydata[3] as string;
                    string XLPrimaryProfile = mydata[4] as string;
                    string XLConnConfiguration = mydata[5] as string;
                    string XLConnCode = mydata[6] as string;
                    //Check the condition and get the attribute data
                    if (CheckMultipleConnectionCondition(MProject, MSecondaryProfile, MPrimaryProfile, MConnConfiguration, MConnCode, MComponent, MAttributeFile, XLComponent, XLProject, XLAttributeFile, XLSecondaryProfile, XLPrimaryProfile, XLConnConfiguration, XLConnCode) == true)
                    {
                        return mydata;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Actual Error:" + ex.Message, "Error @ getAttributeData");
        }
        return AttributeData;
    }
    //public static ArrayList getAttributeData(ArrayList AutoConnectionData, string MProject, string MSecondaryProfile, string MPrimaryProfile, string MConnConfiguration, string MConnCode, string MComponent, string MAttributeFile)
    //{
    //    ArrayList AttributeData = new ArrayList();
    //    try
    //    {
    //        for (int i = 0; i < AutoConnectionData.Count; i++)
    //        {
    //            ArrayList mydata = AutoConnectionData[i] as ArrayList;
    //            if (mydata.Count >= 6)
    //            {
    //                string XLComponent = mydata[0] as string;
    //                string XLProject = mydata[1] as string;
    //                string XLAttributeFile = mydata[2] as string;
    //                string XLSecondaryProfile = mydata[3] as string;
    //                string XLPrimaryProfile = mydata[4] as string;
    //                string XLConnConfiguration = mydata[5] as string;
    //                string XLConnCode = mydata[6] as string;
    //                //Check the condition and get the attribute data
    //                if (
    //                    (skTSLib.CheckCondition(MProject, XLProject) == true)
    //                    && (skTSLib.CheckCondition(MSecondaryProfile, XLSecondaryProfile) == true)
    //                    && (skTSLib.CheckCondition(MPrimaryProfile, XLPrimaryProfile) == true)
    //                    && (skTSLib.CheckCondition(MConnConfiguration, XLConnConfiguration) == true)
    //                    && (skTSLib.CheckCondition(MConnCode, XLConnCode) == true)
    //                    && (skTSLib.CheckCondition(MComponent, XLComponent) == true)
    //                    && (skTSLib.CheckCondition(MAttributeFile, XLAttributeFile) == true)
    //                 )
    //                {
    //                    return mydata;
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Actual Error:" + ex.Message, "Error @ getAttributeData");
    //    }
    //    return AttributeData;
    //}
    public static void AutoSizeListView(ListView myListView, string HeaderName, System.Windows.Forms.CheckBox CheckItem)
    {
        if (CheckItem.Checked == true)
        {
            int colct = myListView.Columns.Count;
            string chkHeaderName = HeaderName;
            chkHeaderName = chkHeaderName.Replace(" ", "").ToUpper();
            for (int i = 0; i < colct; i++)
            {
                string colname = myListView.Columns[i].Text;
                colname = colname.Replace(" ", "").ToUpper();
                if (chkHeaderName.Contains("," + colname + ","))
                    myListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
            }

        }

    }

    public static bool setComponent(Tekla.Structures.Model.BaseComponent Component, String SetAttributeName, String SetAttributeValue)
    {
        bool flag = false;
        try
        {
            //check whether the attribute parameter is string
            string s_Value = "-1";
            Component.GetAttribute(SetAttributeName, ref s_Value);
            if ((s_Value != "-1") && (s_Value != SetAttributeValue))
            {
                Component.SetAttribute(SetAttributeName, SetAttributeValue);
                return true;
            }
            //check whether the attribute parameter is integer
            int i_Value = 1312810155;
            Component.GetAttribute(SetAttributeName, ref i_Value);
            if ((i_Value != 1312810155) && (i_Value != Convert.ToInt32(SetAttributeValue)))
            {
                Component.SetAttribute(SetAttributeName, Convert.ToInt32(SetAttributeValue));
                return true;
            }
            //check whether the attribute parameter is double
            double d_Value = 1312810155.200420052007;
            Component.GetAttribute(SetAttributeName, ref d_Value);
            if ((d_Value != 1312810155.200420052007) && (d_Value != Convert.ToDouble(SetAttributeValue)))
            {
                Component.SetAttribute(SetAttributeName, Convert.ToDouble(SetAttributeValue));
                return true;
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show("Actual Error: " + ex.ToString());
        }

        return flag;
    }
    public static string getComponent(Tekla.Structures.Model.BaseComponent Component, String GetAttributeName)
    {
        string flag = "||";
        try
        {

            //check whether the attribute parameter is string
            string s_Value = "-1";
            //lblsbar2.Text = "s_Value";
            Component.GetAttribute(GetAttributeName, ref s_Value);
            if (s_Value != "-1")
                return s_Value;

            //check whether the attribute parameter is integer
            int i_Value = 1312810155;
            //lblsbar2.Text = "i_Value";
            Component.GetAttribute(GetAttributeName, ref i_Value);
            if (i_Value != 1312810155)
                return i_Value.ToString();

            //check whether the attribute parameter is double
            double d_Value = 1312810155.200420052007;
            //lblsbar2.Text = "d_Value";
            Component.GetAttribute(GetAttributeName, ref d_Value);
            if (d_Value != 1312810155.200420052007)
                return d_Value.ToString();

        }
        catch (Exception ex)
        {
            MessageBox.Show("Actual Error: " + ex.ToString());
        }

        return flag;
    }
    private static bool IsBeam(Beam MyBeam)
    {
        string IDstring = Convert.ToString(MyBeam.Identifier.ID);

        Double strXS = MyBeam.StartPoint.X;
        Double strYS = MyBeam.StartPoint.Y;
        Double strZS = MyBeam.StartPoint.Z;
        Double strXE = MyBeam.EndPoint.X;
        Double strYE = MyBeam.EndPoint.Y;
        Double strZE = MyBeam.EndPoint.Z;


        if (Math.Abs(strZS - strZE) < 1)
        {
            return true;
        }
        else
        {
            return false;

        }
    }

    private TSM.Beam GetBeam(int CurrentID)
    {
        //lblsbar2.Text = "";
        Beam MyBeam = new Beam();
        try
        {
            //Set up model connection w/TS
            //Model MyModel. = new TSM.Model();
            //MyModel = new TSM.Model();
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
                TSMUI.ViewHandler.RedrawWorkplane();
                Identifier PartID = new Identifier(CurrentID);
                TSM.ModelObject MOB = MyModel.SelectModelObject(PartID);

                MyBeam = MOB as Beam;

                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();
            } // end of if that checks to see if there is a model and one is open

            else
            {
                //If no connection to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch { }
        //lblsbar2.Text = "";

        return MyBeam;

    }
    private TSM.Beam GetBeam(int CurrentID, bool Status)
    {
        //lblsbar2.Text = "";
        Beam MyBeam = new Beam();
        try
        {
            //Set up model connection w/TS
            //Model skTSLibModel = new Model();
            //MyModel = new TSM.Model();
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                if (Status == true)
                {
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
                    TSMUI.ViewHandler.RedrawWorkplane();
                }
                Identifier PartID = new Identifier(CurrentID);
                TSM.ModelObject MOB = MyModel.SelectModelObject(PartID);

                MyBeam = MOB as Beam;

                if (Status == true)
                {
                    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                    TSMUI.ViewHandler.RedrawWorkplane();
                }
            } // end of if that checks to see if there is a model and one is open

            else
            {
                //If no connection to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch { }
        //lblsbar2.Text = "";

        return MyBeam;

    }
    public static ArrayList GetPhaseName(string GUID)
    {
        ArrayList returndata = new ArrayList();
        string myObjectName = "-1";
        string PhaseName = string.Empty;
        try
        {
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                Identifier ID = new Identifier(GUID);
                if (ID.IsValid() == true)
                {
                    TSM.ModelObject MOB = MyModel.SelectModelObject(ID);
                    MOB.GetPhase(out TSM.Phase ph);
                    myObjectName = MOB.GetType().ToString();
                    PhaseName =  ph.PhaseName;
                }
                else
                    myObjectName = "Invalid";
                     
            } 

            else
                PhaseName = "Tekla Structures not open or model not open.";        
        }  //end of try loop
        catch (Exception ex)
        {
            PhaseName = "Phase Error:" + ex.Message.ToString();
        }
        returndata.Add(myObjectName);
        returndata.Add(PhaseName);
        return returndata;
    }
    public static void export_screw_database()
    {
        string catalogName = "export_sk_screwdb.cs";
        string XS_MACRO_DIRECTORY = string.Empty;
        Tekla.Structures.TeklaStructuresSettings.GetAdvancedOption("XS_MACRO_DIRECTORY", ref XS_MACRO_DIRECTORY);



        if (XS_MACRO_DIRECTORY.IndexOf(';') > 0)
            XS_MACRO_DIRECTORY = XS_MACRO_DIRECTORY.Remove(XS_MACRO_DIRECTORY.IndexOf(';'));

        string screwdbscript = "namespace Tekla.Technology.Akit.UserScript{" +
        "public class Script {" +
        "public static void Run(Tekla.Technology.Akit.IScript akit){" +
        "akit.Callback(\"acmd_export_screw_database\", \"\", \"main_frame\"); }}};";

        File.WriteAllText(Path.Combine(XS_MACRO_DIRECTORY, catalogName), screwdbscript);
        Tekla.Structures.Model.Operations.Operation.RunMacro("..\\" + catalogName);

    }

    public static bool IsParallel(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {

        Tekla.Structures.Geometry3d.Line Line1 = new Tekla.Structures.Geometry3d.Line(Beam1.StartPoint, Beam1.EndPoint);
        Tekla.Structures.Geometry3d.Line Line2 = new Tekla.Structures.Geometry3d.Line(Beam2.StartPoint, Beam2.EndPoint);

        return Tekla.Structures.Geometry3d.Parallel.LineToLine(Line1, Line2);
    }

    public static string GetSKTeklaSupportFile(string SearchFile, bool IsTemplate)
    {
        //IsTemplate is true will check only Template Folder
        string check = null;
        if (IsTemplate == true)
        {
            check = skTSLib.XS_TEMPLATE_DIRECTORY + "\\" + SearchFile;
            if (System.IO.File.Exists(check) == false)
                return null;
            else
                return check;
        }
        else
        {
            check = skTSLib.ModelPath + "\\" + SearchFile;
            if (System.IO.File.Exists(check) == false)
            {

                //Check fltprops.inp XS_PROJECT
                check = skTSLib.XS_PROJECT + "\\" + SearchFile;
                if (System.IO.File.Exists(check) == false)
                {

                    //Check fltprops.inp in XS_FIRM
                    check = skTSLib.XS_FIRM + "\\" + SearchFile;
                    if (System.IO.File.Exists(check) == false)
                    {
                        //Check fltprops.inp in XS_TEMPLATE_DIRECTORY
                        check = skTSLib.XS_TEMPLATE_DIRECTORY + "\\" + SearchFile;
                        if (System.IO.File.Exists(check) == false)
                            return null;
                        else
                            return check;
                    }
                    else
                        return check;
                }
                else
                    return check;
            }
            else
                return check;
        }
        
    }

    public static bool IsMultipleHalfInch(double thickness, double tolerance = 1.0 / 32.0)
    {
        //Get Remainder
        double remainder = thickness % 12.7;

        // Check if the remainder is within the tolerance range
        return Math.Abs(remainder) <= tolerance || Math.Abs(remainder - 12.7) <= tolerance;
    }
    public static double getflangethickness(string profile)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C)
        {
            return (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
        {
            return  (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
        }
        return result;
    }
    public static double getwebthickness(string profile)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C)
        {
            return (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
        }
        return result;
    }

    public static double get_thickness(string profile, string boltcat)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        //BoltItem.BoltItemTypeEnum teklaProfileType = libProfile.;
        return result;
    }

    public static double getheight(string profile)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
        {
            return (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
        }
        //else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_P)
        //{
        //    double val = (libProfile.aProfileItemParameters[4] as ProfileItemParameter).Value;
        //    result = val;
        //}

        return result;
    }

    public static double getwidth(string profile)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
        {
            return (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
        }
        //else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_P)
        //{
        //    double val = (libProfile.aProfileItemParameters[4] as ProfileItemParameter).Value;
        //    result = val;
        //}

        return result;
    }
    public static double getrootradious(string profile)
    {
        double result = 0;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C || teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
        {
            return  (libProfile.aProfileItemParameters[4] as ProfileItemParameter).Value;
        }
        //else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_P)
        //{
        //    double val = (libProfile.aProfileItemParameters[4] as ProfileItemParameter).Value;
        //    result = val;
        //}

        return result;
    }

    public static double getKvalue(string profile)
    {
        double result = -1;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        //get k value directly
        foreach (ProfileItemParameter userparam in libProfile.aProfileItemUserParameters)
        {
            string getkey = userparam.StringValue;
            string getsym = userparam.Symbol;
            if (getkey.ToUpper().Trim() == "K")
            {
                return userparam.Value;
            }
            if (getsym.ToUpper().Trim() == "K")
            {
                return userparam.Value;
            }
        }
        return result;
    }
    //
    //copy "x:\firm_folder\KeyboardShortcuts.xml" "%LOCALAPPDATA%\Trimble\TeklaStructures\2017i\Settings\"
    public static double getK1value(string profile)
    {
        double result = -1;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);
        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
        //get k value directly
        foreach (ProfileItemParameter userparam in libProfile.aProfileItemUserParameters)
        {
            string getkey = userparam.StringValue;
            string getsym = userparam.Symbol;
            if (getkey.ToUpper().Trim() == "K1")
            {
                return userparam.Value;
            }
            if (getsym.ToUpper().Trim() == "K1")
            {
                return userparam.Value;
            }
        }
        return result;
    }

    public static string GetImperialProfile(string MetricProfile)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        bool flag = libProfile.Select(MetricProfile);
        if (flag == true)
        {

            //ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;
            foreach (ProfileItemParameter userparam in libProfile.aProfileItemUserParameters)
            {
                string chktype = userparam.Property;
                if (chktype.ToUpper().Trim() == "IMPERIAL_EQ_NAME")
                    return userparam.StringValue;

            }
        }

        return string.Empty;
    }

    public static string GetMetricProfile(string ImperialProfile)
    {    
        LibraryProfileItem MyLibraryProfileItem = new LibraryProfileItem();
        bool flag = MyLibraryProfileItem.Select(ImperialProfile);
        if (flag == true)
        {            
 
            foreach (ProfileItemParameter MyProfileItemParameter in MyLibraryProfileItem.aProfileItemUserParameters)
            {
                string chktype = MyProfileItemParameter.Property;
                if (chktype.ToUpper().Trim() == "METRIC_EQ_NAME")
                    return MyProfileItemParameter.StringValue;

            }
        }
        return string.Empty;
    }

    
    public static double getProfileParameter(string Profile, string Property = "", string Symbol = "", string ProfileItem = "ProfileItemParameters")
    {
        double result = -1;
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(Profile);
        if (ProfileItem.ToUpper().Trim() == "ProfileItemParameters".ToUpper().Trim())
        {
            //ProfileItemParameter Parameters;
            //Parameters.Property. = Property.ToUpper().Trim();
            //bool dd = libProfile.aProfileItemParameters.Contains(Property);
            foreach (ProfileItemParameter Parameters in libProfile.aProfileItemParameters)
            {
                if (Property != "")
                {
                    if (Parameters.Property.ToUpper().Trim() == Property.ToUpper().Trim())
                    {
                        return Parameters.Value;
                    }
                }
                if (Symbol != "")
                {
                    if (Parameters.Symbol.ToUpper().Trim() == Symbol.ToUpper().Trim())
                    {
                        return Parameters.Value;
                    }
                }
            }
        }
        else if (ProfileItem.ToUpper().Trim() == "ProfileItemUserParameters".ToUpper().Trim())
        {
            foreach (ProfileItemParameter Parameters in libProfile.aProfileItemUserParameters)
            {
                if (Property != "")
                {
                    if (Parameters.Property.ToUpper().Trim() == Property.ToUpper().Trim())
                    {
                        return Parameters.Value;
                    }
                }
                if (Symbol != "")
                {
                    if (Parameters.Symbol.ToUpper().Trim() == Symbol.ToUpper().Trim())
                    {
                        return Parameters.Value;
  
                    }
                }

            }
        }
        else if (ProfileItem.ToUpper().Trim() == "ProfileItemAnalysisParameters".ToUpper().Trim())
        {
            foreach (ProfileItemParameter Parameters in libProfile.aProfileItemAnalysisParameters)
            {
                if (Property != "")
                {
                    if (Parameters.Property.ToUpper().Trim() == Property.ToUpper().Trim())
                    {
                        return Parameters.Value;
                    }
                }
                if (Symbol != "")
                {
                    if (Parameters.Symbol.ToUpper().Trim() == Symbol.ToUpper().Trim())
                    {
                        return Parameters.Value;

                    }
                }

            }
        }


        return result;
    }

    public static double GetPlateDimensions(string profile, ref double height, ref double thickness)
    {
        string sysProfileString = null;
        Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);
        
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(sysProfileString);
        //libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PL)
            {
                double dim1 = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                double dim2 = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;

                thickness = Math.Min(dim1, dim2);
                height = Math.Max(dim1, dim2);
            }
        }

        return 0.0;
    }

    public static void GetAngleDimensions(string profile, ref double flange, ref double web, ref double thickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
        {
            web = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            flange = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
            thickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
            {
                web = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                flange = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
                thickness = (paraProfileItem.aProfileItemParameters[2] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetWideFlangeDimensions(string profile, ref double flange, ref double web, ref double flgThickness, ref double webThickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_I)
        {
            web = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            flange = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
            webThickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            flgThickness = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_I)
            {
                web = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                flange = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
                webThickness = (paraProfileItem.aProfileItemParameters[2] as ProfileItemParameter).Value;
                flgThickness = (paraProfileItem.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetWTDimensions(string profile, ref double flange, ref double web, ref double flgThickness, ref double webThickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_T)
        {
            web = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            flange = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
            webThickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            flgThickness = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_T)
            {
                web = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                flange = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
                webThickness = (paraProfileItem.aProfileItemParameters[2] as ProfileItemParameter).Value;
                flgThickness = (paraProfileItem.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetTubeDimensions(string profile, ref double width, ref double height, ref double thickness, ref double roundingRadius)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_P)
        {
            if (libProfile.aProfileItemParameters.Count < 4)
            {
                height = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
                width = height;
                thickness = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
                roundingRadius = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            }
            else
            {
                height = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
                width = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
                thickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
                roundingRadius = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (libProfile.aProfileItemParameters.Count < 4)
            {
                height = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
                width = height;
                thickness = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
                roundingRadius = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            }
            else
            {
                height = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
                width = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
                thickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
                roundingRadius = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
    }

    public static double ConvertWithPrecision(double value, double precision)
    {

        //double value = 1220.1547719858884;
        //double precision = 0.0625;
        //double convertedValue = ConvertWithPrecision(value, precision);
        //Console.WriteLine($"Converted value: {convertedValue}");

        return Math.Round(value / precision) * precision;
    }


    //LINQ method
    public static List<TSG.Point> MaximumDistancePoints(List<TSG.Point> points)
    {
        return points.SelectMany((p1, a) => points.Skip(a + 1).Select(p2 => new { Pair = new List<TSG.Point> { p1, p2 }, Distance = Distance.PointToPoint(p1, p2) }))
                     .OrderByDescending(x => x.Distance)
                     .FirstOrDefault(x => x.Distance - points.SelectMany((p1, i) => points.Skip(i + 1).Select(p2 => Distance.PointToPoint(p1, p2))).Max() < 0.3)?.Pair;
    }

    public static void GetPipeDimensions(string profile, ref double diameter, ref double thickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_PD)
        {
            diameter = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            thickness = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PD)
            {
                diameter  = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                thickness = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetRoundDimensions(string profile, ref double diameter)
    {
        string sysProfileString = null;
        Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(sysProfileString);

        if (libProfile.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_D)
            {
                diameter = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetChannelDimensions(string profile, ref double flange, ref double web, ref double flgThickness, ref double webThickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_C)
        {
            web = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            flange = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
            webThickness = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            flgThickness = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_C)
            {
                web = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                flange = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
                webThickness = (paraProfileItem.aProfileItemParameters[2] as ProfileItemParameter).Value;
                flgThickness = (paraProfileItem.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
    }

    public static void GetCSectionDimensions(string profile, ref double flange, ref double web, ref double foldDist, ref double thickness)
    {
        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_CC)
        {
            web = (libProfile.aProfileItemParameters[0] as ProfileItemParameter).Value;
            thickness = (libProfile.aProfileItemParameters[1] as ProfileItemParameter).Value;
            foldDist = (libProfile.aProfileItemParameters[2] as ProfileItemParameter).Value;
            flange = (libProfile.aProfileItemParameters[3] as ProfileItemParameter).Value;
        }
        else if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            string sysProfileString = null;
            Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_CC)
            {
                web = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                thickness = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;
                foldDist = (paraProfileItem.aProfileItemParameters[2] as ProfileItemParameter).Value;
                flange = (paraProfileItem.aProfileItemParameters[3] as ProfileItemParameter).Value;
            }
        }
    }

    public static double GetPlateThickness(string profile)
    {
        string sysProfileString = null;
        Tekla.Structures.Dialog.ProfileConversion.ConvertFromCurrentUnits(profile, ref sysProfileString);

        LibraryProfileItem libProfile = new LibraryProfileItem();
        libProfile.Select(sysProfileString);
        //libProfile.Select(profile);

        ProfileItem.ProfileItemTypeEnum teklaProfileType = libProfile.ProfileItemType;

        if (teklaProfileType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
        {
            ParametricProfileItem paraProfileItem = new ParametricProfileItem();
            paraProfileItem.Select(sysProfileString);
            //paraProfileItem.Select(profile);

            if (paraProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PL)
            {
                double dim1 = (paraProfileItem.aProfileItemParameters[0] as ProfileItemParameter).Value;
                double dim2 = (paraProfileItem.aProfileItemParameters[1] as ProfileItemParameter).Value;

                return Math.Min(dim1, dim2);
            }
        }

        return 0.0;
    }
    /*
     * 240925 1057
    */

    public static List<Tekla.Structures.Drawing.WeldMark> getWeldmarksList(Tekla.Structures.Drawing.SinglePartDrawing MyAssyDrawing)
    {
        Type[] mytypes = { typeof(Tekla.Structures.Drawing.WeldMark) };
        List<Tekla.Structures.Drawing.WeldMark> weldMarksList = new List<Tekla.Structures.Drawing.WeldMark>();
        Tekla.Structures.Drawing.DrawingObjectEnumerator viewsEnum = MyAssyDrawing.GetSheet().GetViews();
        while (viewsEnum.MoveNext())
        {
            Tekla.Structures.Drawing.View view = viewsEnum.Current as Tekla.Structures.Drawing.View;
            if (view != null)
            {
                TSG.GeometricPlane viewplane = new TSG.GeometricPlane(view.ViewCoordinateSystem);
                Tekla.Structures.Drawing.DrawingObjectEnumerator weldMarksEnum = view.GetObjects(mytypes);
                while (weldMarksEnum.MoveNext())
                {
                    Tekla.Structures.Drawing.WeldMark currentWeldMark = weldMarksEnum.Current as Tekla.Structures.Drawing.WeldMark;
                    if (currentWeldMark != null)
                    {
                        weldMarksList.Add(currentWeldMark);
                    }
                }
            }
        }
        return weldMarksList;
    }


    public static bool IsPointInSolid(Tekla.Structures.Model.Part part, Tekla.Structures.Geometry3d.Point p1)
    {
        bool pointIsInSolid = false;
        Tekla.Structures.Model.Solid mysolid = part.GetSolid();
        Tekla.Structures.Geometry3d.Point minPoint = mysolid.MinimumPoint;
        Tekla.Structures.Geometry3d.Point maxPoint = mysolid.MaximumPoint;
        Tekla.Structures.Geometry3d.AABB boundingBox = new Tekla.Structures.Geometry3d.AABB(minPoint, maxPoint);

        // First check if the point is in the bounding box
        if (boundingBox.IsInside(p1))
        {
            Tekla.Structures.Geometry3d.Vector myvector = new Tekla.Structures.Geometry3d.Vector(maxPoint - minPoint);
            var p2 = p1 + (2 * myvector);
            ArrayList intersectionPoints = mysolid.Intersect(new Tekla.Structures.Geometry3d.LineSegment(p1, p2));
            if (intersectionPoints.Count % 2 == 1)
            {
                pointIsInSolid = true;
            }
        }

        return pointIsInSolid;
    }
    public static bool clashCheck(Tekla.Structures.Model.Beam priBeam, Tekla.Structures.Model.Beam secBeam)
    {
        #region check Clash

        bool result = false;

        ArrayList objectsToSelect = new ArrayList();
        objectsToSelect.Add(priBeam);
        objectsToSelect.Add(secBeam);

        TSM.UI.ModelObjectSelector _selector = new TSM.UI.ModelObjectSelector();
        _selector.Select(objectsToSelect);
        MyModel.CommitChanges();

        Tekla.Structures.Model.ClashCheckHandler ClashCheck = MyModel.GetClashCheckHandler();

        result = ClashCheck.RunClashCheck();
        
        ClashCheck.StopClashCheck();

        return result;

        #endregion
    }

    
    
    public static ArrayList GetBooleanObjectsByBoundingBox(TSM.Beam CurrentSelectedBeam)
    {
        ArrayList MySelectedPart = new ArrayList();
        if (CurrentSelectedBeam != null)
        {
            ModelObjectEnumerator MyAllBooleans = CurrentSelectedBeam.GetBooleans();
            while (MyAllBooleans.MoveNext())
            {
                BooleanPart MyCutPart = MyAllBooleans.Current as BooleanPart;
                if (MyCutPart != null)
                {
                    if (MyCutPart.Type == BooleanPart.BooleanTypeEnum.BOOLEAN_CUT)
                    {
                        TSM.Part MyOpPrt = MyCutPart.OperativePart;
                        Solid MySolid = MyOpPrt.GetSolid();
                        ModelObjectEnumerator SurroudingObjectAll = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MySolid.MinimumPoint, MySolid.MaximumPoint);

                        foreach (TSM.ModelObject SurroudingObject in SurroudingObjectAll)
                        {
                            Beam SurroudingBeam = SurroudingObject as Beam;
                            if (SurroudingBeam != null)
                            {
                                if (CurrentSelectedBeam.Identifier.GUID != SurroudingBeam.Identifier.GUID)
                                {
                                    if (!MySelectedPart.Contains(SurroudingBeam))
                                    {
                                        MySelectedPart.Add(SurroudingBeam);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return MySelectedPart;
    }


    public static void RemoveDuplicateWeldMarks(Tekla.Structures.Drawing.SinglePartDrawing MyAssyDrawing)
    {
        Type[] mytypes = { typeof(Tekla.Structures.Drawing.WeldMark) };
        TSM.Model MyModel = new TSM.Model();
        Tekla.Structures.Drawing.DrawingObjectEnumerator viewsEnum = MyAssyDrawing.GetSheet().GetViews();
        List<Tekla.Structures.Drawing.WeldMark> weldMarksList = skTSLib.getWeldmarksList(MyAssyDrawing);
        viewsEnum.Reset();

        // delete unnecessary weld marks
        while (viewsEnum.MoveNext())
        {
            Tekla.Structures.Drawing.View view = viewsEnum.Current as Tekla.Structures.Drawing.View;
            if (view != null)
            {
                TSG.GeometricPlane viewplane = new TSG.GeometricPlane(view.ViewCoordinateSystem);
                Tekla.Structures.Drawing.DrawingObjectEnumerator weldMarksEnum = view.GetObjects(mytypes);
                while (weldMarksEnum.MoveNext())
                {
                    Tekla.Structures.Drawing.WeldMark currentWeldMark = weldMarksEnum.Current as Tekla.Structures.Drawing.WeldMark;
                    if (currentWeldMark != null)
                    {
                        var myWeld = MyModel.SelectModelObject(currentWeldMark.ModelIdentifier);
                        if (myWeld != null)
                        {
                            TSG.GeometricPlane weldplane = new TSG.GeometricPlane(myWeld.GetCoordinateSystem());
                            if (!TSG.Parallel.PlaneToPlane(weldplane, viewplane))
                            {
                                List<Tekla.Structures.Drawing.WeldMark> listofSimilarweldMarks = weldMarksList.Where(i => i.ModelIdentifier.ID == myWeld.Identifier.ID).ToList();
                                if (listofSimilarweldMarks.Count > 1)
                                {
                                    currentWeldMark.Delete();
                                    weldMarksList.RemoveAt(weldMarksList.FindIndex(i => i.ModelIdentifier.ID == myWeld.Identifier.ID));
                                }
                            }
                        }
                    }
                }
            }
        }

        // *
        viewsEnum.Reset();

        // check and delete repeated weld marks

        while (viewsEnum.MoveNext())
        {
            Tekla.Structures.Drawing.View view = viewsEnum.Current as Tekla.Structures.Drawing.View;
            if (view != null)
            {
                Tekla.Structures.Drawing.DrawingObjectEnumerator weldMarksEnum = view.GetObjects(mytypes);
                while (weldMarksEnum.MoveNext())
                {
                    Tekla.Structures.Drawing.WeldMark currentWeldMark = weldMarksEnum.Current as Tekla.Structures.Drawing.WeldMark;
                    if (currentWeldMark != null)
                    {
                        var myWeld = MyModel.SelectModelObject(currentWeldMark.ModelIdentifier);
                        if (myWeld != null)
                        {
                            List<Tekla.Structures.Drawing.WeldMark> listofSimilarweldMarks = weldMarksList.Where(i => i.ModelIdentifier.ID == myWeld.Identifier.ID).ToList();
                            if (listofSimilarweldMarks.Count > 1)
                            {
                                currentWeldMark.Delete();
                                weldMarksList.RemoveAt(weldMarksList.FindIndex(i => i.ModelIdentifier.ID == myWeld.Identifier.ID));
                            }
                        }
                    }
                }
            }
        }
    }
    public static string getDataGrid_GUID(DataGridViewRow row)
    {
        string myguid = string.Empty;
        //check whether row have guid
        if (row.Cells["GUID"].Value != null)
        {
            myguid = row.Cells["GUID"].Value.ToString();

        }
        //replace if guid is there
        return myguid.ToUpper().Replace("GUID:", "").Replace("GUID", "").Trim();
    }

    /// <summary>
    /// Sets the Object representation settings and returns the current view back to its original camera location.
    /// </summary>
    /// <param name="SettingsName">Saved object representation settings.</param>
    /// <returns></returns>
    public static bool SetObjectRepresentation(string SettingsName)
    {

        TSMUI.ModelViewEnumerator TheseViews = TSMUI.ViewHandler.GetSelectedViews();
        TSMUI.View ThisView = null;
        if (TheseViews.Count == 0)
        {
            TheseViews = TSMUI.ViewHandler.GetVisibleViews();
            if (TheseViews.Count > 0)
            {
                TheseViews.MoveNext();
                ThisView = TheseViews.Current as TSMUI.View;
            }
        }
        else
        {
            TheseViews.MoveNext();
            ThisView = TheseViews.Current as TSMUI.View;
        }


        bool SetRep = false;
        if (ThisView != null)
        {
            TSMUI.ViewCamera Camera = new TSMUI.ViewCamera();
            Camera.View = ThisView;
            Camera.Select();
            SetRep = TSMUI.ViewHandler.SetRepresentation(SettingsName);
            Camera.Modify();
        }

        return SetRep;
    }

    private void set_representation(ArrayList szString)
    {
        Model MyModel = new Model();
        string fname = skTSLib.ModelPath + "\\attributes\\Smart Checks.rep";
        int iTotal = szString.Count + 1;

        TextWriter stringWriter = new StringWriter();
        using (TextWriter streamWriter =
            new StreamWriter(fname))
        {
            streamWriter.WriteLine("REPRESENTATIONS ");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("    Version= 1.04 ");
            streamWriter.WriteLine("    Count= " + iTotal.ToString() + " ");

            for (int i = 0; i < szString.Count; i++)
            {

                var values = Guid.NewGuid().ToByteArray().Select(b => (int)b);
                int red = values.Take(5).Sum() % 255;
                int green = values.Skip(5).Take(5).Sum() % 255;
                int blue = values.Skip(10).Take(5).Sum() % 255;

                streamWriter.WriteLine("    SECTION_UTILITY_LIMITS ");
                streamWriter.WriteLine("    {");
                streamWriter.WriteLine("        0.5");
                streamWriter.WriteLine("        0.9");
                streamWriter.WriteLine("        1");
                streamWriter.WriteLine("        1.2");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("    SECTION_OBJECT_REP ");
                streamWriter.WriteLine("    {");
                streamWriter.WriteLine("        " + szString[i] + " ");
                streamWriter.WriteLine("        16711780 ");
                streamWriter.WriteLine("        10");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("    SECTION_OBJECT_REP_BY_ATTRIBUTE ");
                streamWriter.WriteLine("    {");
                streamWriter.WriteLine("        (SETTINGNOTDEFINED) ");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("    SECTION_OBJECT_REP_RGB_VALUE ");
                streamWriter.WriteLine("    {");
                streamWriter.WriteLine("        " + red.ToString());
                streamWriter.WriteLine("        " + green.ToString());
                streamWriter.WriteLine("        " + blue.ToString());
                streamWriter.WriteLine("        }");

            }
            streamWriter.WriteLine("    SECTION_UTILITY_LIMITS ");
            streamWriter.WriteLine("    {");
            streamWriter.WriteLine("        0.5");
            streamWriter.WriteLine("        0.9");
            streamWriter.WriteLine("        1");
            streamWriter.WriteLine("        1.2");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("    SECTION_OBJECT_REP ");
            streamWriter.WriteLine("    {");
            streamWriter.WriteLine("        All ");
            streamWriter.WriteLine("        100");
            streamWriter.WriteLine("        10");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("    SECTION_OBJECT_REP_BY_ATTRIBUTE ");
            streamWriter.WriteLine("    {");
            streamWriter.WriteLine("        (SETTINGNOTDEFINED) ");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("    SECTION_OBJECT_REP_RGB_VALUE ");
            streamWriter.WriteLine("    {");
            streamWriter.WriteLine("        0");
            streamWriter.WriteLine("        0");
            streamWriter.WriteLine("        0");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("    }");

            streamWriter.Close();
        }
    }

    private void set_filter(string stype, int sequence, string szString)
    {
        Model MyModel = new Model();
        string fname = skTSLib.ModelPath + "\\attributes\\" + stype + sequence.ToString() + ".PObjGrp";


        TextWriter stringWriter = new StringWriter();
        using (TextWriter streamWriter =
            new StreamWriter(fname))
        {
            streamWriter.WriteLine("TITLE_OBJECT_GROUP ");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("    Version= 1.05 ");
            streamWriter.WriteLine("    Count= 1 ");
            streamWriter.WriteLine("    SECTION_OBJECT_GROUP ");
            streamWriter.WriteLine("    {");
            streamWriter.WriteLine("        0 ");
            streamWriter.WriteLine("        1");
            streamWriter.WriteLine("        co_part ");

            //foreach (ListViewItem lvItem in listView5.Items)
            //{
            //    if (lvItem.SubItems[1].Text == stype)
            //    {
            //        streamWriter.WriteLine("        " + lvItem.SubItems[2].Text + " ");
            //        streamWriter.WriteLine("        " + lvItem.SubItems[3].Text + " ");
            //    }
            //}

            streamWriter.WriteLine("         11 ");
            streamWriter.WriteLine("         12 ");
            streamWriter.WriteLine("        == ");
            streamWriter.WriteLine("        albl_Equals ");
            streamWriter.WriteLine("        " + szString + " ");
            streamWriter.WriteLine("        0 ");
            streamWriter.WriteLine("        Empty ");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("    }");

            streamWriter.Close();
        }
    }


    public static void SKModelObjectVisualization_AllRow(DataGridView dgvcheckncfile)
    {
        //Tekla
        List<Identifier> myListIdentifier = new List<Identifier>();
        ArrayList ObjectsToSelect = new ArrayList();

        try
        {
            foreach (DataGridViewRow row in dgvcheckncfile.Rows)
            {

                string myguid = getDataGrid_GUID(row);
                if (myguid.Length >= 1)
                {
                    Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier();
                    myIdentifier = new Tekla.Structures.Identifier(myguid);
                    //bool flag = myIdentifier.IsValid();
                    if (myIdentifier.IsValid() == true && myIdentifier  != null)
                    {
                        Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);
                        if (ModelSideObject.Identifier.IsValid() == true && ModelSideObject != null)
                        {
                            if (!ObjectsToSelect.Contains(ModelSideObject))
                                ObjectsToSelect.Add(ModelSideObject);


                            if (!myListIdentifier.Contains(myIdentifier))
                                myListIdentifier.Add(myIdentifier);
                        }
                    }
                }

            }

            var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
            MOS.Select(ObjectsToSelect);
            TSMUI.Color tsgrey = new TSM.UI.Color(0.0, 0.0, 1.0);
            TSMUI.Color tsgreen = new TSM.UI.Color(0.0, 1.0, 0.0);
            SKModelObjectVisualization(myListIdentifier, tsgrey, tsgreen);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Representation Error: " + ex.Message);
        }

    }

    public static void SKModelObjectVisualization_SelectedRow(DataGridView dgvcheckncfile)
    {
        //Tekla
        List<Identifier> myListIdentifier = new List<Identifier>();
        ArrayList ObjectsToSelect = new ArrayList();

        try
        {      
            foreach (DataGridViewRow row in dgvcheckncfile.SelectedRows)
            {
               
                string myguid = getDataGrid_GUID(row);
                if (myguid.Length >=1)
                {
                    Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier();
                    myIdentifier = new Tekla.Structures.Identifier(myguid);
                    bool flag = myIdentifier.IsValid();
                    if (flag)
                    {
                        Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);
                        if (ModelSideObject.Identifier.IsValid())
                        {
                            if (!ObjectsToSelect.Contains(ModelSideObject))
                                ObjectsToSelect.Add(ModelSideObject);

                            if (!myListIdentifier.Contains(myIdentifier))
                                myListIdentifier.Add(myIdentifier);
                        }
                    }
                }                    
            }

            var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
            MOS.Select(ObjectsToSelect);
            TSMUI.Color tsgrey = new TSM.UI.Color(0.0, 0.0, 1.0);
            TSMUI.Color tsgreen = new TSM.UI.Color(0.0, 1.0, 0.0);
            SKModelObjectVisualization(myListIdentifier, tsgrey, tsgreen);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Representation Error: " + ex.Message);
        }

    }


    public static void SKModelObjectVisualization(List<Identifier> myListIdentifier, TSMUI.Color SetTemporaryStateColorForAll, TSMUI.Color SetTemporaryStateColorSelected)
    {
        TSMUI.ModelObjectVisualization.ClearAllTemporaryStates();
        TSMUI.ModelObjectVisualization.SetTemporaryStateForAll(SetTemporaryStateColorForAll);
        TSMUI.ModelObjectVisualization.SetTemporaryState(myListIdentifier, SetTemporaryStateColorSelected);
    }
    public static void SKModelObjectVisualization(List<Identifier>[] myListIdentifierList, TSMUI.Color SetTemporaryStateColorForAll)
    {
        TSMUI.ModelObjectVisualization.ClearAllTemporaryStates();
        TSMUI.ModelObjectVisualization.SetTemporaryStateForAll(SetTemporaryStateColorForAll);
        double myred = 1.0;
        double mygreen= 1.0;
        double myblue  = 1.0;

        foreach (List<Identifier> myListIdentifier in myListIdentifierList)
        {
            TSMUI.Color SetTemporaryStateColorSelected = new TSM.UI.Color(myred, mygreen, myblue);
            TSMUI.ModelObjectVisualization.SetTemporaryState(myListIdentifier, SetTemporaryStateColorSelected);
            myred++;
            mygreen++;
            myblue++;
        } 
    }

    public static void selectDataGridAllRows(DataGridView mydatagrid, int colid = 0, string idtype = "GUID", bool zoom = true)
    {
        try
        {
            //int rowindex = e.RowIndex;
            //DataGridViewRow row = mydatagrid.Rows[rowindex];
            ArrayList ObjectsToSelect = new ArrayList();
            int i = 0;
            foreach (DataGridViewRow row in mydatagrid.Rows)
            {
                //DataGridViewRow row = mydatagrid.SelectedRows[i]; 
                Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier();
                if (idtype.ToUpper().Trim() == "GUID")
                {
                    string myguid = row.Cells[colid].Value.ToString();
                    myguid = myguid.ToUpper().Replace("GUID:", "").Replace("GUID", "").Trim();
                    myIdentifier = new Tekla.Structures.Identifier(myguid);
                }
                else if (idtype.ToUpper().Trim() == "ID")
                {
                    string s_id = row.Cells[colid].Value.ToString();
                    s_id = s_id.ToUpper().Replace("ID:", "").Trim();
                    int id = Convert.ToInt32(s_id);
                    myIdentifier = new Tekla.Structures.Identifier(id);
                }

                bool flag = myIdentifier.IsValid();

                if (flag)
                {
                    Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);

                    if (ModelSideObject.Identifier.IsValid())
                    {
                        //ModelSideObject.Select();
                        if (!ObjectsToSelect.Contains(ModelSideObject))
                            ObjectsToSelect.Add(ModelSideObject);
                    }



                }
                i++;

            }

            var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
            MOS.Select(ObjectsToSelect);
            //if (zoom == true) TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
            if (zoom == true)
                ZoomSelected();



        }
        catch (Exception ex)
        {
            MessageBox.Show("Select/Zoom: " + ex.Message);
        }


    }

    public static void ZoomSelected()
    {
        ////Yearly Renewal required
        //if (Convert.ToInt32(DateTime.Now.ToString("yyyy")) != esskayappvalidity)
        //{
        //    //skWinLib.DebugLog(debugflag, debugcount++, logfile, "ConnectToModel-Failed");
        //    skWinLib.worklog("", "", "ZoomSelected", Convert.ToInt32(DateTime.Now.ToString("yyyy")).ToString(), "", "", "");
        //    System.Environment.Exit(0);
        //}
        //var akit = new MacroBuilder();
        //akit.Callback("acmdZoomToSelected", "", "main_frame");
        //TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
        Tekla.Structures.ModelInternal.Operation.dotStartAction("ZoomToSelected", "");


    
    }

    public static void ZoomSelected(TSM.ModelObject MyModelObject)
    {

        Solid MySolid = null;
        if (MyModelObject is TSM.Part MyPart)
            MySolid = MyPart.GetSolid();
        else if (MyModelObject is TSM.Reinforcement MyReinforcement)
            MySolid = MyReinforcement.GetSolid();
        else if (MyModelObject is TSM.ContourPlate MyContourPlate)
            MySolid = MyContourPlate.GetSolid();
        else if (MyModelObject is TSM.PolyBeam MyPolyBeam)
            MySolid = MyPolyBeam.GetSolid();
        else if (MyModelObject is TSM.Beam MyBeam)
            MySolid = MyBeam.GetSolid();
        else if (MyModelObject is TSM.BaseWeld MyBaseWeld)
            MySolid = MyBaseWeld.GetSolid();
        else if (MyModelObject is TSM.BoltGroup MyBoltGroup)
            MySolid = MyBoltGroup.GetSolid();

        if (MySolid != null)
        {
            AABB MyAABB = new AABB();
            MyAABB.MaxPoint = MySolid.MaximumPoint;
            MyAABB.MinPoint = MySolid.MinimumPoint;
            TSM.UI.ViewHandler.ZoomToBoundingBox(MyAABB);
        }

    }
    public static Tekla.Structures.Model.ModelObject GetModelObject_FromGUIDID(string guid)
    {
        Tekla.Structures.Model.ModelObject MyModelObject = null;
        string myguid = guid.ToUpper().Replace("GUID:", "").Replace("GUID", "").Trim();
        Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier(myguid);
        bool flag = myIdentifier.IsValid();
        if (flag == true)
        {
            MyModelObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);
        }
        return MyModelObject;
    }

    public static Tekla.Structures.Model.ModelObject GetModelObject_FromGUIDID(int id)
    {
        Tekla.Structures.Model.ModelObject MyModelObject = null;
        Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier(id);
        bool flag = myIdentifier.IsValid();
        if (flag == true)
        {
            MyModelObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);
        }
        return MyModelObject;
    }

    public static void ModelObject_List_To_ArrayList(List<TSM.ModelObject> MyModelObjects)
    {
        ArrayList objList = new ArrayList();
        var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
        foreach (TSM.ModelObject MyModelObject in MyModelObjects)
        {
            objList.Add(MyModelObject);
        }
        MOS.Select(objList);
    }


    public static void selectDataGridselectedrows(Tekla.Structures.Model.Model MyModel, DataGridView mydatagrid, int colid = 0, string idtype = "GUID", bool zoom = true)
    {
        var guidList = new List<string>();
        var idList = new List<long>(); 
        try
        {
            //int rowindex = e.RowIndex;
            //DataGridViewRow row = mydatagrid.Rows[rowindex];
            ArrayList ObjectsToSelect = new ArrayList();
            //int i = 0;
            foreach (DataGridViewRow row in mydatagrid.SelectedRows)
            {
  
                if (idtype.ToUpper().Trim().Contains("GUID"))
                {
                    if (row.Cells[colid].Value != null)
                    {
                        string myguid = row.Cells[colid].Value.ToString();
                        if (myguid.Trim().Length >=1)
                        {
                            myguid = myguid.Replace("GUID:", "").Replace("GUID", "").Replace("guid:", "").Replace("guid", "").Trim();
                            //myguid = myguid.ToUpper().Replace("GUID:", "").Replace("GUID", "").Trim();
                            if (myguid.Contains(";") == true)
                            {
                                string[] spltguids = myguid.Split(";".ToCharArray());
                                foreach (string guid in spltguids)
                                {
                                    guidList.Add(guid);
                                }
                            }
                            else
                                guidList.Add(myguid);
                        }
                    }
                }
                else if (idtype.ToUpper().Trim() == "ID")
                {
                    if (row.Cells[colid].Value != null)
                    {
                        string s_id = row.Cells[colid].Value.ToString();
                        if (s_id.Trim().Length >= 1)
                        {
                            s_id = s_id.ToUpper().Replace("ID:", "").Trim();
                            int id = Convert.ToInt32(s_id);
                            guidList.Add(MyModel.GetGUIDByIdentifier(new Identifier(id)));
                        }
                        
                    }
                }
            }

            if (MyModel.GetConnectionStatus() ==  true)
            {
                var MyModelObjects = MyModel.FetchModelObjects(guidList, false);
                //List<TSM.ModelObject> MyModelObjects = MyModel.FetchModelObjects(guidList, false);
                //int t = objList.Count;
                //this if failure other wise objects are selected

                ModelObject_List_To_ArrayList(MyModelObjects);

                if (zoom == true)
                    ZoomSelected();
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show("Error@selectDataGridselectedrows1 " + ex.Message);
        }

    }

    public static void selectDataGridselectedrows(DataGridView mydatagrid, int colid = 0, string idtype = "GUID", bool zoom = true)
    {

        string gdname = mydatagrid.Name;
        int cnt = mydatagrid.SelectedRows.Count;

        try
        {

            //int rowindex = e.RowIndex;
            //DataGridViewRow row = mydatagrid.Rows[rowindex];

            ArrayList ObjectsToSelect = new ArrayList();
            int i = 0;
            foreach (DataGridViewRow row in mydatagrid.SelectedRows)
            {
                if (row.Visible == true)
                {
                    //DataGridViewRow row12 = mydatagrid.SelectedRows[i];
                    //var row1 = row.Cells[0].Value.ToString();
                    //var row2 = row.Cells[1].Value.ToString();
                    //var row3 = row.Cells[2].Value.ToString();

                    Tekla.Structures.Identifier myIdentifier = new Tekla.Structures.Identifier();
                    string myguid = string.Empty;
                    string s_id = string.Empty;
                    if (idtype.ToUpper().Trim().Contains("GUID") == true)
                    {
                        if (row.Cells[colid].Value != null)
                        {
                            myguid = row.Cells[colid].Value.ToString();
                            myguid = myguid.Replace("GUID:", "").Replace("GUID", "").Replace("guid:", "").Replace("guid", "").Trim();
                            myIdentifier = new Tekla.Structures.Identifier(myguid);

                        }
                    }

                    else if (idtype.ToUpper().Contains("ID") == true)
                    {
                        if (row.Cells[colid].Value != null)
                        {
                            s_id = row.Cells[colid].Value.ToString();
                            s_id = s_id.Replace("ID:", "").Replace("ID", "").Replace("id:", "").Replace("id", "").Trim();
                            int id = Convert.ToInt32(s_id);
                            myIdentifier = new Tekla.Structures.Identifier(id);
                        }
                    }
                    if (myIdentifier.IsValid() == true)
                    {
                        Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(myIdentifier);

                        if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                        {
                            //ModelSideObject.Select();
                            if (!ObjectsToSelect.Contains(ModelSideObject))
                                ObjectsToSelect.Add(ModelSideObject);
                        }
                    }
                    else
                    {
                        List<string> mymo = new List<string>();
                        //check for split
                        if (myguid != string.Empty)
                        {
                            //split based on PIE
                            string[] spltdata = myguid.Split("|".ToCharArray());
                            if (spltdata.Length > 1)
                            {
                                ArrayList tmp = new ArrayList();
                                foreach (string guid in spltdata)
                                {
                                    if (guid.Trim().Length >=1)
                                    {
                                        Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(new Tekla.Structures.Identifier(guid));
                                        if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                                        {
                                            //ModelSideObject.Select();
                                            if (!ObjectsToSelect.Contains(ModelSideObject))
                                                ObjectsToSelect.Add(ModelSideObject);
                                        }
                                    }
                
                                }
                 

                                //List<TSM.ModelObject> MyModelObjects = MyModel.FetchModelObjects(spltdata.ToList(), false);
                                //ObjectsToSelect.AddRange(MyModelObjects);
                            }
                            else
                            {
                                spltdata = myguid.Split(";".ToCharArray());
                                if (spltdata.Length > 1)
                                {
                                    //ArrayList tmp = new ArrayList();
                                    foreach (string guid in spltdata)
                                    {
                                        if (guid.Trim().Length >= 1)
                                        {
                                            Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(new Tekla.Structures.Identifier(guid));

                                            if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                                            {
                                                //ModelSideObject.Select();
                                                if (!ObjectsToSelect.Contains(ModelSideObject))
                                                    ObjectsToSelect.Add(ModelSideObject);
                                            }
                                        }
                                    }

                                    //mymo.AddRange(spltdata.ToArray());
                                    //List<TSM.ModelObject> MyModelObjects = MyModel.FetchModelObjects(mymo, false);
                                    //ObjectsToSelect.AddRange(MyModel.FetchModelObjects(spltdata.ToList(), false));
                                }
                                else
                                {
                                    spltdata = myguid.Split(",".ToCharArray());
                                    if (spltdata.Length > 1)
                                    {
                                        foreach (string guid in spltdata)
                                        {
                                            if (guid.Trim().Length >= 1)
                                            {
                                                Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(new Tekla.Structures.Identifier(guid));

                                                if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                                                {
                                                    //ModelSideObject.Select();
                                                    if (!ObjectsToSelect.Contains(ModelSideObject))
                                                        ObjectsToSelect.Add(ModelSideObject);
                                                }
                                            }
                                            //List<TSM.ModelObject> MyModelObjects = MyModel.FetchModelObjects(spltdata.ToList(), false);
                                            //ObjectsToSelect.AddRange(MyModelObjects);
                                        }
                                    }
                
                                }

                            }

                        }
                        
                    }
                }
                
                i++;

            }

            var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
            MOS.Select(ObjectsToSelect);
            //if (zoom == true) TSM.Operations.Operation.RunMacro("ZoomSelected.cs");
            //BA091F75-0A06-41FD-9C96-C06B5A4E3E5B
            if (zoom == true)
                ZoomSelected();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error@selectDataGridselectedrows2 " + ex.Message);
        }


    }

    public static void SelectZoomGuids(string Guids, bool zoom)
    {
        ArrayList ObjectsToSelect = new ArrayList();

        //check for split
        if (Guids != string.Empty)
        {
            string myguids = Guids.Replace(",", "|").Replace(";", "|");
            //split based on PIE
            string[] spltdata = myguids.Split("|".ToCharArray());
            if (spltdata.Length > 1)
            {

                foreach (string guid in spltdata)
                {
                    if (guid.Trim().Length >= 1)
                    {
                        Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(new Tekla.Structures.Identifier(guid));
                        if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                        {
                            if (!ObjectsToSelect.Contains(ModelSideObject))
                                ObjectsToSelect.Add(ModelSideObject);
                        }
                    }

                }
            }
            else
            {
                Tekla.Structures.Model.ModelObject ModelSideObject = new Tekla.Structures.Model.Model().SelectModelObject(new Tekla.Structures.Identifier(Guids));

                if (ModelSideObject != null && ModelSideObject.Identifier.IsValid() == true)
                {
                    //ModelSideObject.Select();
                    if (!ObjectsToSelect.Contains(ModelSideObject))
                        ObjectsToSelect.Add(ModelSideObject);
                }
            }

        }

        var MOS = new Tekla.Structures.Model.UI.ModelObjectSelector();
        MOS.Select(ObjectsToSelect);

        if (zoom == true)
            ZoomSelected();

    }
    public static bool IsPointNearRootRadius(TSG.Point ClashCheckPart_Point, TSG.Point Member_TopMiddleEndPoint,double width, double rootradius, double flangethickness, double webthickness)
    {
        double chkx = ClashCheckPart_Point.X;
        double chky = Math.Abs(ClashCheckPart_Point.Y);
        double chkz = Math.Abs(ClashCheckPart_Point.Z);

        double memx = Member_TopMiddleEndPoint.X;
        double memy = Member_TopMiddleEndPoint.Y;
        double memz = Member_TopMiddleEndPoint.Z;

        double flgthkrootradius = flangethickness + rootradius;
        double webthk2rootradius = (webthickness / 2) + rootradius;

        //check weather clash part is below start point x  and greater than end point x
        if ((chkx < 0) || (chkx > memx))
            return false;


        //check weather clash part isgreater than end point y
        if (chky > memy)
            return false;

        //check weather clash part is greater than end point z
        if (chkz > (width / 2.0))
            return false;

        double chkdist = TSG.Distance.PointToPoint(new TSG.Point(memz + webthk2rootradius, memy - flgthkrootradius, 0), new TSG.Point(chkz, chky, 0));
            
        if (chkdist > flgthkrootradius && flgthkrootradius > webthk2rootradius)
            return false;
        else if (chkdist > webthk2rootradius && flgthkrootradius < webthk2rootradius)
            return false;

        return true;
    }

    public static ArrayList getRootRadiusNearPoint(Tekla.Structures.Solid.FaceEnumerator faces, TSG.Point Member_TopMiddleEndPoint, double width, double rootradius, double flangethickness, double webthickness)
    {
        ArrayList result = new ArrayList();
        result.Add("Esskay");
        while (faces.MoveNext())
        {
            if (faces.Current is Tekla.Structures.Solid.Face face)
            {
                Tekla.Structures.Solid.LoopEnumerator Loops = face.GetLoopEnumerator();
                while (Loops.MoveNext())
                {
                    if (Loops.Current is Tekla.Structures.Solid.Loop Loop)
                    {
                        Tekla.Structures.Solid.VertexEnumerator CornerPoints = Loop.GetVertexEnumerator();
                        while (CornerPoints.MoveNext())
                        {
                            if (CornerPoints.Current is TSG.Point myPoint)
                            {
                                bool isnear = IsPointNearRootRadius(myPoint, Member_TopMiddleEndPoint, width, rootradius, flangethickness, webthickness);
                                if (isnear == true)
                                {
                                    if (!result.Contains(myPoint))
                                    {
                                         result.Add(myPoint);

                                        // TSM.ControlPoint test = new TSM.ControlPoint (myPoint);
                                        // bool flag = test.Insert();

                                    }
                                } 
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    public static LineSegment GetCrossPartsVertext(Tekla.Structures.Geometry3d.Point MyPoint, Double Tolerance)
    {
        ArrayList CrossParts = new ArrayList();
        LineSegment MyLineSegment = new LineSegment();
        //string ids = "";

        try
        {

            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;

                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                while (CrossEnum.MoveNext())
                {

                    //Tekla.Structures.Model.ModelObject myObject = CrossEnum.Current as Tekla.Structures.Model.ModelObject;
                    //ids = ids + "\n" + myObject.Identifier.ID.ToString();

                    Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                    if (CrossBeam != null)
                    {
                        string PProfileType = "";
                        CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);
                        if (PProfileType != "B")
                        {
                            CrossParts.Add(CrossBeam.Identifier.ID);
                        }
                    }
                }
                foreach(Tekla.Structures.Model.Beam CrossBeam in CrossParts)
                {

                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossPartsID. Original error: " + ex.Message);
        }

        return MyLineSegment;

    }

    //public static ArrayList GetCrossPartPolygonCutLineSegement(Tekla.Structures.Model.Model MyModel, Tekla.Structures.Model.Part MyBeam)
    //{
    //    ArrayList result = new ArrayList();
    //    ArrayList sptcheck = new ArrayList();
    //    ArrayList eptcheck = new ArrayList();
    //    result.Add("Esskay");

    //    //get current plane for resetting at last
    //    TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(MyBeam.GetCoordinateSystem()));



    //    TSMUI.GraphicsDrawer drawer = new TSMUI.GraphicsDrawer();
    //    ModelObjectEnumerator MyBeamBoolObjects = MyBeam.GetBooleans();
    //    double Xmax = MyBeam.GetSolid().MaximumPoint.X;
    //    double Xmin = MyBeam.GetSolid().MinimumPoint.X;
    //    double Xmid = (Xmax - Xmin) / 2.0;
    //    foreach (Tekla.Structures.Model.ModelObject  IsboolObject in MyBeamBoolObjects)
    //    {
    //        Tekla.Structures.Model.BooleanPart BoolenPartAll = IsboolObject as Tekla.Structures.Model.BooleanPart;
    //        if (BoolenPartAll != null)
    //        {
    //            Tekla.Structures.Model.Part MyBoolenPart = BoolenPartAll.OperativePart;
    //            if (MyBoolenPart != null)
    //            {
    //                if (MyBeam.Identifier.ID != MyBoolenPart.Identifier.ID)
    //                {
    //                    string PProfileType = "";
    //                    MyBoolenPart.GetReportProperty("PROFILE_TYPE", ref PProfileType);
    //                    if (PProfileType != "B")
    //                    {
    //                        ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MyBoolenPart.GetSolid().MinimumPoint, MyBoolenPart.GetSolid().MaximumPoint);
    //                        while (CrossEnum.MoveNext())
    //                        {
    //                            Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

    //                            if (CrossBeam != null)
    //                            {
    //                                if (MyBeam.Identifier.ID != CrossBeam.Identifier.ID)
    //                                {
    //                                    PProfileType = "";
    //                                    CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);
    //                                    if (PProfileType != "B")
    //                                    {
    //                                        result.Add(CrossBeam.Identifier.ID);
    //                                        if (MyBoolenPart.GetSolid().MaximumPoint.X <= Xmid)
    //                                        {
    //                                            sptcheck.Add(CrossBeam);
    //                                            //drawer.DrawText(MyBoolenPart.GetSolid().MaximumPoint, count++.ToString(), new TSMUI.Color(1, 0, 0));
    //                                        }
    //                                        else
    //                                        {
    //                                            eptcheck.Add(CrossBeam);
    //                                            //drawer.DrawText(MyBoolenPart.GetSolid().MaximumPoint, count++.ToString(), new TSMUI.Color(0, 1, 0));
    //                                        }
    //                                    }
    //                                }

    //                            }
    //                        }
    //                    }
    //                }

    //            }
    //        }
            
    //    }

           


    //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
    //    result.Clear();
    //    result.Add(sptcheck);
    //    result.Add(eptcheck);
    //    return result;
    //}
    //public static ArrayList GetPointforCopeCheck(Tekla.Structures.Model.Model MyModel, Tekla.Structures.Model.Part MyBeam, bool debug = false)
    //{
    //    ArrayList result = new ArrayList();
    //    ArrayList sptcheck = new ArrayList();
    //    ArrayList eptcheck = new ArrayList();
    //    result.Add("Esskay");

    //    ////get current plane for resetting at last
    //    //TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //    //MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(MyBeam.GetCoordinateSystem()));


    //    double tw = skTSLib.getwebthickness(MyBeam.Profile.ProfileString);
    //    double  Zmax = tw / 2.0;
    //    double Zmin = tw / -2.0;
    //    TSMUI.GraphicsDrawer drawer = new TSMUI.GraphicsDrawer();
    //    int count = 1;
    //    //TSM.Solid MySolid = MyBeam.GetSolid(Solid.SolidCreationTypeEnum.HIGH_ACCURACY);
    //    TSM.Solid MySolid = MyBeam.GetSolid();


    //    if (MySolid != null)
    //    {
    //        double Xmax = MySolid.MaximumPoint.X;
    //        double Xmin = MySolid.MinimumPoint.X;
    //        double Xmid = (Xmax - Xmin) / 2.0;

    //        double Ymax = MySolid.MaximumPoint.Y;
    //        double Ymin = MySolid.MinimumPoint.Y;
    //        Tekla.Structures.Solid.FaceEnumerator MyFaceEnum = MySolid.GetFaceEnumerator();
    //        while (MyFaceEnum.MoveNext())
    //        {
    //            if (MyFaceEnum.Current is Tekla.Structures.Solid.Face face)
    //            {
    //                Tekla.Structures.Solid.LoopEnumerator Loops = face.GetLoopEnumerator();
    //                while (Loops.MoveNext())
    //                {
    //                    if (Loops.Current is Tekla.Structures.Solid.Loop Loop)
    //                    {
    //                        Tekla.Structures.Solid.VertexEnumerator CornerPoints = Loop.GetVertexEnumerator();
    //                        while (CornerPoints.MoveNext())
    //                        {
    //                            if (CornerPoints.Current is TSG.Point myPoint)
    //                            {
    //                                //drawer.DrawText(myPoint, myPoint.X.ToString() + "," + myPoint.Y.ToString() + "," + myPoint.Z.ToString(), new TSMUI.Color(0, 0, 1));
    //                                TSG.Point chkPoint = new TSG.Point(myPoint.X, myPoint.Y, 0);
    //                                //check for only web cut no other cuts can be within zmin to zmax
    //                                double chkZ = Math.Abs(myPoint.Z) - Zmax;
    //                                //if (myPoint.Z >= Zmin && myPoint.Z <= Zmax )
    //                                if (chkZ <= ((1/32.0) * 25.4))
    //                                {

    //                                    //ignore outermost point voided considering clearance is must
    //                                    //if ((myPoint.X != Xmax && myPoint.X != Xmin) && (myPoint.Y != Ymax && myPoint.Y != Ymin))
    //                                    {
    //                                        if (!result.Contains(chkPoint))
    //                                        {
    //                                            result.Add(chkPoint);
    //                                            if (myPoint.X <= Xmid)
    //                                            {
    //                                                sptcheck.Add(chkPoint);
    //                                                if (debug == true)
    //                                                    drawer.DrawText(chkPoint, count++.ToString(), new TSMUI.Color(0, 1, 0));
    //                                            }

    //                                            else
    //                                            {
    //                                                eptcheck.Add(chkPoint);
    //                                                if (debug == true)
    //                                                    drawer.DrawText(chkPoint, count++.ToString(), new TSMUI.Color(0, 1, 0));
    //                                            }

    //                                           // drawer.DrawText(chkPoint, count++.ToString(), new TSMUI.Color(1, 0, 0));
    //                                            //TSM.ControlPoint test = new TSM.ControlPoint (myPoint);
    //                                            //bool flag = test.Insert();

    //                                        }
    //                                    }
    //                                }                                   

    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    //MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
    //    result.Clear();
    //    result.Add(sptcheck);
    //    result.Add(eptcheck);
    //    return result;
    //}
    public static ArrayList getUniqueFacePoints(Tekla.Structures.Solid.FaceEnumerator faces)
    {
        ArrayList result = new ArrayList();
        result.Add("Esskay");
        int count = 0;
        TSMUI.GraphicsDrawer drawer = new TSMUI.GraphicsDrawer();
        while (faces.MoveNext())
        {
            if (faces.Current is Tekla.Structures.Solid.Face face)
            {
                Tekla.Structures.Solid.LoopEnumerator Loops = face.GetLoopEnumerator();
               
                while (Loops.MoveNext())
                {
                    if (Loops.Current is Tekla.Structures.Solid.Loop Loop)
                    {
                        Tekla.Structures.Solid.VertexEnumerator CornerPoints = Loop.GetVertexEnumerator();
                        while (CornerPoints.MoveNext())
                        {
                            if (CornerPoints.Current is TSG.Point myPoint)
                            {
                                if (!result.Contains(myPoint))
                                {
                                    result.Add(myPoint);
                                    drawer.DrawText(myPoint, count++.ToString(), new TSMUI.Color(0, 1, 0));
                                    //TSM.ControlPoint test = new TSM.ControlPoint (myPoint);
                                    //bool flag = test.Insert();

                                }
                                       
                            }
                        }
                    }
                }
            }
        }
        return result;
    }


    public static string FormatPointCoordinates(TSG.Point MyPoint)
    {
        double xx = MyPoint.X;
        double yy = MyPoint.Y;
        double zz = MyPoint.Z;
        return xx.ToString("#0.0#") + "," + yy.ToString("#0.0#") + "," + zz.ToString("#0.0#");
    }
    public static ArrayList getUniqueFacePoints(Tekla.Structures.Model.Solid MySolid, bool ignore_x, bool ignore_y, bool ignore_z, bool debug = false)
    {
        ArrayList result = new ArrayList();
        //result.Add("Esskay");

        //int count = 0;
        TSMUI.GraphicsDrawer drawer = new TSMUI.GraphicsDrawer();

        if (MySolid != null)
        {
            double Xmax = MySolid.MaximumPoint.X;
            double Xmin = MySolid.MinimumPoint.X;
            double Xmid = (Xmax - Xmin) / 2.0;

            double Ymax = MySolid.MaximumPoint.Y;
            double Ymin = MySolid.MinimumPoint.Y;
            Tekla.Structures.Solid.FaceEnumerator faces = MySolid.GetFaceEnumerator();
            while (faces.MoveNext())
            {
                if (faces.Current is Tekla.Structures.Solid.Face face)
                {
                    Tekla.Structures.Solid.LoopEnumerator Loops = face.GetLoopEnumerator();
                    while (Loops.MoveNext())
                    {
                        if (Loops.Current is Tekla.Structures.Solid.Loop Loop)
                        {
                            Tekla.Structures.Solid.VertexEnumerator CornerPoints = Loop.GetVertexEnumerator();
                            while (CornerPoints.MoveNext())
                            {
                                if (CornerPoints.Current is TSG.Point myPoint)
                                {
                                    double xx = myPoint.X;
                                    double yy = myPoint.Y;
                                    double zz = myPoint.Z;
                                    if (ignore_x == true)
                                        xx = 0;
                                    if (ignore_y == true)
                                        yy = 0;
                                    if (ignore_z == true)
                                        zz = 0;

                                    TSG.Point chkPoint = new TSG.Point(xx, yy, zz);
                                    if (!result.Contains(chkPoint))
                                    {
                                        result.Add(chkPoint);
                                        if (debug == true)
                                            drawer.DrawText(chkPoint, result.Count.ToString(), new TSMUI.Color(0, 0, 1));
                                    }

                                }
                            }
                        }
                    }
                }
            }
  
        }
        return result;
    }

    class ListViewItemComparer : IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }
        public ListViewItemComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }

    //new class node added on 11th Jan 2023
    //Credits Mr. Abdul Hammed
    public class Nodes
    {
        public List<Node> NodesList = new List<Node>();
        public double Tolerance { get; set; }
        private Tekla.Structures.Geometry3d.Point Origin = new Tekla.Structures.Geometry3d.Point();



        //public void AddNode(Node NewNode)
        //{
        //    List<Node> CopyNodes = NodesList;
        //    bool NodeAvailable = false;
        //    foreach (Node OldNode in CopyNodes)
        //    {
        //        double dDistance = Distance.PointToPoint(OldNode.NodePoint, NewNode.NodePoint);
        //        if (dDistance < Tolerance)
        //        {
        //            NodeAvailable = true;
        //        }
        //    }
        //    if (NodesList.Count == 0 || NodeAvailable == false)
        //    {
        //        CopyNodes.Add(NewNode);
        //    }
        //    NodesList = CopyNodes;
        //}

        public void AddNode(Node NewNode, string sMember)
        {
            if (NodesList.Count == 0)  // mainly for no nodes
            {
                NodesList.Add(NewNode);
            }
            else
            {
                double dNew = Distance.PointToPoint(Origin, NewNode.NodePoint);

                int iMin = 0;
                int iMax = NodesList.Count - 1;
                int iRef = Convert.ToInt32((iMax + iMin) * 0.5);

                Tekla.Structures.Geometry3d.Point MinPoint = NodesList[iMin].NodePoint;
                Tekla.Structures.Geometry3d.Point MaxPoint = NodesList[iMax].NodePoint;
                Tekla.Structures.Geometry3d.Point RefPoint = NodesList[iRef].NodePoint;

                int dRef = Convert.ToInt32(Distance.PointToPoint(Origin, RefPoint));  // use max to compare

                while (iMax - iMin > 1)  // for getting the 2 nodes
                {
                    if (dRef <= dNew)
                    {
                        iMin = iRef;
                        iRef = Convert.ToInt32((iMax + iMin) * 0.5);
                        RefPoint = NodesList[iRef].NodePoint;
                        dRef = Convert.ToInt32(Distance.PointToPoint(Origin, RefPoint));
                    }
                    if (dRef >= dNew)
                    {
                        iMax = iRef;
                        iRef = Convert.ToInt32((iMax + iMin) * 0.5);
                        RefPoint = NodesList[iRef].NodePoint;
                        dRef = Convert.ToInt32(Distance.PointToPoint(Origin, RefPoint));
                    }
                }

                int iMinE = iMin - 5;
                if (iMinE < 0)
                {
                    iMinE = 0;
                }
                int iMaxE = iMax + 5;
                if (iMaxE > NodesList.Count - 1)
                {
                    iMaxE = NodesList.Count - 1;
                }

                bool Available = false;
                MinPoint = NodesList[iMin].NodePoint;
                MaxPoint = NodesList[iMax].NodePoint;
                int dMin = Convert.ToInt32(Distance.PointToPoint(Origin, MinPoint));
                int dMax = Convert.ToInt32(Distance.PointToPoint(Origin, MaxPoint));


                for (int i = iMinE; i <= iMaxE; i++) // for merging purpose USING E FOR EXTRA 5 + 5
                { 
                    RefPoint = NodesList[i].NodePoint;
                    dRef = Convert.ToInt32(Distance.PointToPoint(NewNode.NodePoint, RefPoint));  // for verifying the tolerance
                    if (dRef < Tolerance) 
                    {
                        if (sMember == "Main")
                        {
                            NodesList[i].NodePoint = NewNode.NodePoint; 
                        }
                        Available = true;
                    }
                }

                //insert new node
                if (Available == false)
                {
                    if (dMin > dNew)
                    {
                        NodesList.Insert(iMin, NewNode);
                    }
                    else if (dMax < dNew)
                    {
                        NodesList.Insert(iMax + 1, NewNode);
                    }
                    else
                    {
                        NodesList.Insert(iMin + 1, NewNode);
                    }
                }
            }
        }


        public List<Node> GetNodes()
        {
            return NodesList;
        }
    }

    public class Node
    {
        public Tekla.Structures.Geometry3d.Point NodePoint;
        public List<Component> Components = new List<Component>();

        public void ChangeNodePoint(Tekla.Structures.Geometry3d.Point NewPoint)
        {
            NodePoint = NewPoint;
        }
    }

    public class Component
    {
        public string FramingCondition;
        public string HittingFlange = "";
        public string WebAlignment = "";
        public Beam Primary;
        public Beam Primary2;
        public Beam Primary3;
        public List<Beam> Secondary = new List<Beam>();
    }

    public static bool IsMainPart(Tekla.Structures.Model.ModelObject part)
    {
        bool res = false;
        int ismain = 0;
        part.GetReportProperty("MAIN_PART", ref ismain);
        if (ismain == 1)
            res = true;
        return res;
    }

    //public static ArrayList getChilderenObject(TSM.ModelObject ModelObj)
    //{
    //    ArrayList result = new ArrayList();
    //    result.Add("Esskay");
    //    if (ModelObj is SinglePart assmbly)
    //    {
    //        List<ModelObject> concreteparts = new List<ModelObject>();

    //        var main = assmbly.GetMainPart();
    //        result.Add( assmbly.GetSecondaries());
    //        var subs = assmbly.GetSubAssemblies()
    //    }

    //    return result;

    //}

    public static bool IsParallelBeams(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {

        LineSegment Line1 = new LineSegment(Beam1.StartPoint, Beam1.EndPoint);
        LineSegment Line2 = new LineSegment(Beam2.StartPoint, Beam2.EndPoint);

        return Tekla.Structures.Geometry3d.Parallel.LineSegmentToLineSegment(Line1, Line2);
    }
   
    
    public static string GetFramingCondition(Tekla.Structures.Geometry3d.Point MyPoint, Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, System.Windows.Forms.CheckBox CheckWrapAroundFlag, double nodeTolerance)
    {
        string sFramingCondition = "";

        if (CheckWrapAroundFlag.Checked == true)
        {
            if (IsBrace(Secondary) == true)
            {
                double dAngle = GetAngle(Primary, Secondary);

                if (dAngle > 20 && dAngle < 70)
                {
                    Tekla.Structures.Model.Beam Primary3 = GetWebPrimary(Primary, Secondary, Secondaries, nodeTolerance);
                    Tekla.Structures.Model.Beam Primary2 = GetFlangePrimary(Primary, Secondary, Secondaries, nodeTolerance );

                    if (Primary2 != null)
                    {
                        if (Is2Flange(Primary, Secondary) == true)
                        {
                            sFramingCondition = "Wrap Around @ Flange";
                        }
                    }
                    if (Primary3 != null)
                    {
                        if (Is2Flange(Primary, Secondary) == false)
                        {
                            sFramingCondition = "Wrap Around @ Web";
                        }
                    }
                }
                else if (dAngle > 85)
                {

                    Tekla.Structures.Model.Beam Primary3 = GetWebPrimary(Primary, Secondary, Secondaries, nodeTolerance);
                    Tekla.Structures.Model.Beam Primary2 = GetFlangePrimary(Primary, Secondary, Secondaries,nodeTolerance);

                    if (Primary2 != null && Primary3 != null)
                    {
                        if (IsColumn(Primary) == true)
                        {
                            sFramingCondition = "Wrap Around @ Column";
                        }
                        else if (IsBeam(Primary) == true)
                        {
                            sFramingCondition = "Wrap Around @ Beam";
                        }
                    }
                }
            }
        }

        //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
        //double nodeTolerance = Convert.ToDouble(txtnodetolerance.Text);
        if (sFramingCondition == "")
        {
            if (IsSplice(Primary, Secondary) == true)
            {
                if (Distance.PointToPoint(Primary.StartPoint, MyPoint) < nodeTolerance || Distance.PointToPoint(Primary.EndPoint, MyPoint) < nodeTolerance)
                {
                    if (Distance.PointToPoint(Secondary.StartPoint, MyPoint) < nodeTolerance || Distance.PointToPoint(Secondary.EndPoint, MyPoint) < nodeTolerance)
                    {
                        if (IsColumn(Primary) == true && IsColumn(Secondary) == true)
                        {
                            sFramingCondition = "Column to Column Splice";
                        }
                        else
                        {
                            sFramingCondition = "Beam to Beam Splice";
                        }
                    }
                }
            }
        }
        if (sFramingCondition == "")
        {
            if (IsParallelBeams(Primary, Secondary) == true)
            {
                sFramingCondition = "Parallel Members";
            }
        }
        if (sFramingCondition == "")
        {
            if (IsColumn(Primary) == true)
            {
                if (Is2Flange(Primary, Secondary) == true)
                {
                    sFramingCondition = "Beam to Column Flange";
                }
                else
                {
                    sFramingCondition = "Beam to Column Web";
                }
            }
            else if (IsColumn(Secondary) == true)
            {
                if (Is2Flange(Primary, Secondary) == true)
                {
                    sFramingCondition = "Column to Beam Flange";
                }
                else
                {
                    sFramingCondition = "Column to Beam Web";
                }
            }
            else if (IsBeam(Primary) == true)
            {
                if (Is2Flange(Primary, Secondary) == true)
                {
                    sFramingCondition = "Beam to Beam Flange";
                }
                else
                {
                    sFramingCondition = "Beam to Beam Web";
                }
            }
            else if (skTSLib.GetDistance(MyPoint, Primary) < nodeTolerance && skTSLib.GetDistance(MyPoint, Secondary) < nodeTolerance)
            {
                sFramingCondition = "Connected Ends";
            }
        }
        return sFramingCondition;
    }


    public static string GetSingleCondition(Tekla.Structures.Geometry3d.Point MyPoint, Tekla.Structures.Model.Beam Secondary, double Tolerance)
    {
        string sFramingCondition = "";
        //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
        //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);


        if (IsColumn(Secondary) == true)
        {
            if (Secondary.StartPoint.Z - MyPoint.Z > Tolerance || Secondary.EndPoint.Z - MyPoint.Z > Tolerance)
            {
                sFramingCondition = "Base Plate";
            }
            else
            {
                sFramingCondition = "Cap Plate";
            }
        }
        else
        {
            sFramingCondition = "Free End";
        }
        return sFramingCondition;
    }

    public static string GetHittingFlange(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary,double nodeTolerance)
    {
        string sFramingCondition = "";

        if (Is2TopSide(Primary, Secondary, nodeTolerance) == true)
        {
            sFramingCondition = "Top";
        }
        else
        {
            sFramingCondition = "Bottom";
        }
        return sFramingCondition;
    }
    public static double GetAngle_X(Tekla.Structures.Model.Part Part1, Tekla.Structures.Model.Part Part2, bool Is360 = false)
    {
        Part1.Select();
        Part2.Select();

        Vector Part1Vector = new Vector(Part1.GetCoordinateSystem().AxisX);
        Vector Part2Vector = new Vector(Part2.GetCoordinateSystem().AxisX);

        double angle = 0.0;
        angle = Part1Vector.GetAngleBetween(Part2Vector);
        angle = angle * 180 / Math.PI;
        if (Is360 == false)
        {
            if (angle > 90)
            {
                angle = 180 - angle;
            }
        }


        return angle;
    }

    public static double GetAngle_Y(Tekla.Structures.Model.Part Part1, Tekla.Structures.Model.Part Part2, bool Is360 = false)
    {
        Part1.Select();
        Part2.Select();

        Vector Part1Vector = new Vector(Part1.GetCoordinateSystem().AxisY);
        Vector Part2Vector = new Vector(Part2.GetCoordinateSystem().AxisY);

        double angle = 0.0;
        angle = Part1Vector.GetAngleBetween(Part2Vector);
        angle = angle * 180 / Math.PI;
        if (Is360 == false)
        {
            if (angle > 90)
            {
                angle = 180 - angle;
            }
        }

        return angle;
    }

    public static double GetAngle(TSG.Point StartPoint, TSG.Point EndPoint)
    {
        LineSegment LS1 = new LineSegment(StartPoint, EndPoint);
        Vector VC1 = LS1.GetDirectionVector();

        LineSegment LS2 = new LineSegment(StartPoint, new TSG.Point(EndPoint.X + StartPoint.Y, StartPoint.Y, StartPoint.Z));
        Vector VC2 = LS2.GetDirectionVector();

        double deg = (180 / Math.PI) * VC1.GetAngleBetween(VC2);
        //based on z(plan) vector change 
        double chkvecz = new Vector(1, 0, 0).Cross(VC1).Z;
        if (chkvecz < 0.0)
            deg = 360.0 - deg;

        //TSG.Point MidPt = skTSLib.GetMidPoint(StartPoint, EndPoint);
        //skTSLib.drawer.DrawText(MidPt, deg.ToString("#0.0#"), new TSMUI.Color(0, 0, 1));

        return deg;
    }
    public static double GetAngle(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        Beam1.Select();
        Beam2.Select();

        Vector Beam1Vector = new Vector(Beam1.GetCoordinateSystem().AxisX);
        Vector Beam2Vector = new Vector(Beam2.GetCoordinateSystem().AxisX);

        double angle = 0.0;
        angle = Beam1Vector.GetAngleBetween(Beam2Vector);
        angle = angle * 180 / Math.PI;
        if (angle > 90)
        {
            angle = 180 - angle;
        }

        return angle;
    }



    public static double GetAngleAxisX(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        return GetAngle(Beam1, Beam2);
    }

    public static double GetAngleAxisY(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        Beam1.Select();
        Beam2.Select();

        Vector Beam1Vector = new Vector(Beam1.GetCoordinateSystem().AxisY);
        Vector Beam2Vector = new Vector(Beam2.GetCoordinateSystem().AxisY);

        double angle = 0.0;
        angle = Beam1Vector.GetAngleBetween(Beam2Vector);
        angle = angle * 180 / Math.PI;
        if (angle > 90)
        {
            angle = 180 - angle;
        }

        return angle;
    }

    public static string GetAngleAxis(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        try
        {
            return "Slope:" + GetAngleAxisX(Beam1, Beam2).ToString("#0.#") + ";Skew:" + GetAngleAxisY(Beam1, Beam2).ToString("#0.#");
        }
        catch
        {
            return string.Empty;
        }

    }

    public static string GetAngleAxis(Tekla.Structures.Model.ModelObject ModelObject1, Tekla.Structures.Model.ModelObject ModelObject2)
    {
        Tekla.Structures.Model.Beam Beam1 = ModelObject1 as Tekla.Structures.Model.Beam;
        Tekla.Structures.Model.Beam Beam2 = ModelObject2 as Tekla.Structures.Model.Beam;
        return "Slope:" + GetAngleAxisX(Beam1, Beam2).ToString("#0.#") + ";Skew:" + GetAngleAxisY(Beam1, Beam2).ToString("#0.#");
    }


    public static Tekla.Structures.Model.Beam GetFlangePrimary(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, double nodeTolerance)
    {
        Tekla.Structures.Model.Beam Primary2 = new Tekla.Structures.Model.Beam();
        bool Available = false;
        bool bTop = Is2TopSide(Primary, Secondary, nodeTolerance);

        foreach (Tekla.Structures.Model.Beam MyBeam in Secondaries)
        {
            if (MyBeam != Primary && MyBeam != Secondary)
            {
                if (Is2TopSide(Primary, MyBeam, nodeTolerance) == bTop && Is2Flange(Primary, MyBeam) == true)
                {
                    if (IsColumn(MyBeam) == true || IsBeam(MyBeam) == true)
                    {
                        Primary2 = MyBeam;
                        Available = true;
                    }
                }
            }
        }
        if (Available == true)
        {
            return Primary2;
        }
        else
        {
            return null;
        }
    }

    public static Tekla.Structures.Model.Beam GetWebPrimary(Tekla.Structures.Model.Beam Primary, Tekla.Structures.Model.Beam Secondary, List<Tekla.Structures.Model.Beam> Secondaries, double nodeTolerance)
    {
        Tekla.Structures.Model.Beam Primary2 = new Tekla.Structures.Model.Beam();
        bool Available = false;
        bool bFront = Is2FrontSide(Primary, Secondary, nodeTolerance);

        foreach (Tekla.Structures.Model.Beam MyBeam in Secondaries)
        {
            if (MyBeam != Primary && MyBeam != Secondary)
            {
                if (Is2FrontSide(Primary, MyBeam, nodeTolerance) == bFront && Is2Flange(Primary, MyBeam) == false)
                {
                    if (IsColumn(MyBeam) == true || IsBeam(MyBeam) == true)
                    {
                        Primary2 = MyBeam;
                        Available = true;
                    }
                }
            }
        }
        if (Available == true)
        {
            return Primary2;
        }
        else
        {
            return null;
        }
    }


    public static bool IsRightSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, Tekla.Structures.Geometry3d.Point MyPoint, double nodeTolerance)
    {
        //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
        //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
        bool condition = false;
        try
        {  // model connection is not necessary
           //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();

                if (Beam2.StartPoint.X - MyPoint.X > nodeTolerance || Beam2.EndPoint.X - MyPoint.X > nodeTolerance)
                {
                    condition = true;
                }

                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();
            } // end of if that checks to see if there is a model and one is open
            else
            {
                //If no Component to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.",
                    "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch (Exception ex)
        {
            MessageBox.Show("Original error: IsRightSide " + ex.Message);
        }


        return condition;


    }
    public static bool Is2FrontSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, double nodeTolerance)
    {
        //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
        //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
        bool condition = false;
        try
        {  // model connection is not necessary
           //Set up model Component w/TS
           //MyModel = new TSM.Model();
           //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();

                if (Beam2.StartPoint.Z > nodeTolerance || Beam2.EndPoint.Z > nodeTolerance)
                {
                    condition = true;
                }

                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();
            } // end of if that checks to see if there is a model and one is open
            else
            {
                //If no Component to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.",
                    "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch (Exception ex)
        {
            MessageBox.Show("Original error: IS2FRONTSIDE " + ex.Message);
        }


        return condition;


    }

    public static TSG.Point GetMidPoint(TSG.Point Point1, TSG.Point Point2)
    {
        return new TSG.Point((Point1.X + Point2.X) / 2.0, (Point1.Y + Point2.Y) / 2.0, (Point1.Z + Point2.Z) / 2.0);
    }

    public static TSG.LineSegment GetSolid_TopEnd1End2MidPoint(Tekla.Structures.Model.Solid MySolid, string PROFILE_TYPE = "I")
    {
        TSG.Point MaxPt = MySolid.MaximumPoint;
        TSG.Point MinPt = MySolid.MinimumPoint;
        return new TSG.LineSegment(new TSG.Point(MinPt.X, MaxPt.Y, 0), new TSG.Point(MaxPt.X, MaxPt.Y, 0));
    }

    public static TSG.LineSegment GetSolid_BottomEnd1End2MidPoint(Tekla.Structures.Model.Solid MySolid, string PROFILE_TYPE = "I")
    {
        TSG.Point MaxPt = MySolid.MaximumPoint;
        TSG.Point MinPt = MySolid.MinimumPoint;
        return new TSG.LineSegment(new TSG.Point(MinPt.X, MinPt.Y, 0), new TSG.Point(MaxPt.X, MinPt.Y, 0));
    }

    public static TSG.LineSegment GetSolid_CenterEnd1End2Point(Tekla.Structures.Model.Solid MySolid)
    {
        TSG.Point MaxPt = MySolid.MaximumPoint;
        TSG.Point MinPt = MySolid.MinimumPoint;
        //double XDeleta = (MaxPt.X - MinPt.X) / 2.0;
        double YDeleta = (MaxPt.Y - MinPt.Y) / 2.0;
        double ZDeleta = (MaxPt.Z - MinPt.Z) / 2.0;

        return new TSG.LineSegment(new TSG.Point(MinPt.X, YDeleta, ZDeleta), new TSG.Point(MaxPt.X, YDeleta, ZDeleta));
    }

    public static bool Is2TopSide(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2, double nodeTolerance)
    {
        //int Tolerance = Convert.ToInt32(txtnodetolerance.Text);
        //double Tolerance = Convert.ToDouble(txtnodetolerance.Text);
        bool condition = false;
        try
        {  // model connection is not necessary
            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(Beam1.GetCoordinateSystem()));
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();

                if (Beam2.StartPoint.Y > nodeTolerance || Beam2.EndPoint.Y > nodeTolerance)
                {
                    condition = true;
                }
                // i did it because the current is changes the nodepoint
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();
                Beam2.Select();

            } // end of if that checks to see if there is a model and one is open
            else
            {
                //If no Component to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.",
                    "Component Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch (Exception ex)
        {
            MessageBox.Show("Original error: IS2TOPSIDE" + ex.Message);
        }


        return condition;


    }

    public static bool IsSecondaryLower(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        double ZValue = 0;

        if (Beam1.StartPoint.Z > Beam1.EndPoint.Z)
        {
            ZValue = Beam1.StartPoint.Z;
        }
        else
        {
            ZValue = Beam1.EndPoint.Z;
        }

        if (Beam2.StartPoint.Z < ZValue || Beam2.EndPoint.Z < ZValue)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public static bool IsSplice(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {

        CoordinateSystem cs1 = Beam2.GetCoordinateSystem();
        CoordinateSystem cs2 = Beam1.GetCoordinateSystem();  // for primary

        double dot = Vector.Dot(cs1.AxisX.GetNormal(), cs2.AxisX.GetNormal());



        bool Connected = false;

        if (Distance.PointToPoint(Beam1.StartPoint, Beam2.StartPoint) < 10)
        {
            Connected = true;
        }
        else if (Distance.PointToPoint(Beam1.StartPoint, Beam2.EndPoint) < 10)
        {
            Connected = true;
        }
        else if (Distance.PointToPoint(Beam1.EndPoint, Beam2.EndPoint) < 10)
        {
            Connected = true;
        }
        else if (Distance.PointToPoint(Beam1.EndPoint, Beam2.StartPoint) < 10)
        {
            Connected = true;
        }


        if (Math.Abs(dot) > 0.95 && Connected == true) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsWebPerpendicular(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {

        CoordinateSystem cs1 = Beam1.GetCoordinateSystem();  // for primary
        CoordinateSystem cs2 = Beam2.GetCoordinateSystem();

        double dot = Vector.Dot(cs2.AxisY.GetNormal(), cs1.AxisX.GetNormal());

        if (Math.Abs(dot) < 0.5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Is2Flange(Tekla.Structures.Model.Beam Beam1, Tekla.Structures.Model.Beam Beam2)
    {
        CoordinateSystem cs1 = Beam1.GetCoordinateSystem();  // for primary
        CoordinateSystem cs2 = Beam2.GetCoordinateSystem();

        double dot = Vector.Dot(cs2.AxisX.GetNormal(), cs1.AxisY.GetNormal());

        if (Math.Abs(dot) > 0.2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsBrace(Tekla.Structures.Model.Beam MyBeam)
    {
        if (IsColumn(MyBeam) == false && IsBeam(MyBeam) == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public static bool IsColumn(Tekla.Structures.Model.Beam MyBeam)
    {
        //if not a beam properites then return false since column possiblity of contn
        if (MyBeam != null)
        {
            MyBeam.Select();

            Double douXS = MyBeam.StartPoint.X;
            Double douYS = MyBeam.StartPoint.Y;
            Double douZS = MyBeam.StartPoint.Z;
            Double douXE = MyBeam.EndPoint.X;
            Double douYE = MyBeam.EndPoint.Y;
            Double douZE = MyBeam.EndPoint.Z;

            if (Math.Abs(douYS - douYE) < 10 && Math.Abs(douXS - douXE) < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }
    public static bool IsColumn(Tekla.Structures.Model.Part MyPart)
    {
        MyPart.Select();
        TSM.Beam MyBeam = MyPart as TSM.Beam;
        if (MyBeam != null)
            return IsColumn(MyBeam);
        else
            return false;
    }

    public static bool IsDetailAvailable(Tekla.Structures.Geometry3d.Point MyPoint, string sComponent)
    {
        bool Available = false;
        try
        {
            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                MinP.X = MinP.X - 50;
                MinP.Y = MinP.Y - 50;
                MinP.Z = MinP.Z - 50;

                MaxP.X = MaxP.X + 50;
                MaxP.Y = MaxP.Y + 50;
                MaxP.Z = MaxP.Z + 50;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                while (CrossEnum.MoveNext())
                {
                    Detail Crossing = CrossEnum.Current as Detail;

                    if (Crossing != null && Crossing.Name == sComponent)
                    {
                        Available = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("ISDETAILAVAILABLE. Original error: " + ex.Message);
        }
        return Available;
    }
    public static List<string> GetModelObjectGuids(List<TSM.Beam> NodeBeams)
    {
        List<string> MyMdlSelGuids = new List<string>();
        foreach(TSM.Beam MyBeam in NodeBeams)
        {
            if (MyBeam != null)
                MyMdlSelGuids.Add(MyBeam.Identifier.GUID.ToString());
        }
        return MyMdlSelGuids;
    }
    public static List<string> GetModelObjectGuids()
    {
        List<string> MyMdlSelGuids = new List<string>();
        Tekla.Structures.Model.ModelObjectEnumerator MyMOEnum = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
        while (MyMOEnum.MoveNext())
        {
            Tekla.Structures.Model.ModelObject MyMO = MyMOEnum.Current as Tekla.Structures.Model.ModelObject;
            MyMdlSelGuids.Add(MyMO.Identifier.GUID.ToString());

        }
        return MyMdlSelGuids;
    }

    //public static ArrayList GetModelObjectGuids()
    //{
    //    ArrayList MyMdlSelGuids = new ArrayList();
    //    Tekla.Structures.Model.ModelObjectEnumerator MyMOEnum = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
    //    while (MyMOEnum.MoveNext())
    //    {
    //        Tekla.Structures.Model.ModelObject MyMO = MyMOEnum.Current as Tekla.Structures.Model.ModelObject;
    //        MyMdlSelGuids.Add(MyMO.Identifier.GUID.ToString());

    //    }
    //    return MyMdlSelGuids;
    //}

    public static List<Tekla.Structures.Model.Part> GetCrossPartsatBothEnd(Tekla.Structures.Model.Beam MyBeam, double Tolerance)
    {
        List<Tekla.Structures.Model.Part> CrossParts = new List<Tekla.Structures.Model.Part>();
        try
        {

            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();

                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(MyBeam.GetCoordinateSystem()));
                TSMUI.ViewHandler.RedrawWorkplane();
                MyBeam.Select();
                bool isupdateflag = MyBeam.IsUpToDate;

                Tekla.Structures.Geometry3d.Point MinP = new TSG.Point(MyBeam.StartPoint.X, MyBeam.StartPoint.Y, MyBeam.StartPoint.Z);
                Tekla.Structures.Geometry3d.Point MaxP = new TSG.Point(MyBeam.StartPoint.X, MyBeam.StartPoint.Y, MyBeam.StartPoint.Z);
                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;
                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
                int ct = CrossEnum.GetSize();
                if (ct == 0)
                {
                    //ct can't be zero
                    MyBeam.Modify();
                    MyBeam.Select();
                    skTSLib.ZoomSelected();
                }
                while (CrossEnum.MoveNext())
                {
                    Tekla.Structures.Model.Part CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Part;
                    if (CrossBeam != null)
                    {
                        if (MyBeam.Identifier.GUID != CrossBeam.Identifier.GUID)
                        {
                            if (!CrossParts.Contains(CrossBeam))
                            {
                                CrossParts.Add(CrossBeam);
                            }
                        }
                    }    
                }

                //End Point
                MinP = new TSG.Point(MyBeam.EndPoint.X, MyBeam.EndPoint.Y, MyBeam.EndPoint.Z);
                MaxP = new TSG.Point(MyBeam.EndPoint.X, MyBeam.EndPoint.Y, MyBeam.EndPoint.Z);
                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;
                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                //if (debugflag == true)
                //{
                //    drawer.DrawText(MinP, "MinP", new TSMUI.Color(0, 0.5, 0));
                //    drawer.DrawText(MaxP, "MaxP", new TSMUI.Color(0, 0.5, 0));
                //}

                CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
                ct = CrossEnum.GetSize();
                if (ct == 0)
                {
                    //ct can't be zero
                    MyBeam.Modify();
                    MyBeam.Select();
                    skTSLib.ZoomSelected();
                }
                while (CrossEnum.MoveNext())
                {
                    Tekla.Structures.Model.Part CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Part;
                    if (CrossBeam != null)
                    {
                        if (MyBeam.Identifier.GUID != CrossBeam.Identifier.GUID)
                        {
                            if (!CrossParts.Contains(CrossBeam))
                            {
                                CrossParts.Add(CrossBeam);
                            }
                        }

                    }

                }

                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossPartsatBothEnd Original error: " + ex.Message);
        }
        return CrossParts;
    }
    //public static List<Tekla.Structures.Model.Part> GetCrossParts(TSG.Point CrossPoint, double Tolerance)
    //{
    //    List<Tekla.Structures.Model.Part> CrossParts = new List<Tekla.Structures.Model.Part>();
    //    try
    //    {
    //        Tekla.Structures.Geometry3d.Point MinP = new TSG.Point(CrossPoint.X, CrossPoint.Y, CrossPoint.Z);
    //        Tekla.Structures.Geometry3d.Point MaxP = new TSG.Point(CrossPoint.X, CrossPoint.Y, CrossPoint.Z);
    //        MinP.X = MinP.X - Tolerance;
    //        MinP.Y = MinP.Y - Tolerance;
    //        MinP.Z = MinP.Z - Tolerance;
    //        MaxP.X = MaxP.X + Tolerance;
    //        MaxP.Y = MaxP.Y + Tolerance;
    //        MaxP.Z = MaxP.Z + Tolerance;

    //        ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
    //        while (CrossEnum.MoveNext())
    //        {
    //            Tekla.Structures.Model.Part CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Part;
    //            if (CrossBeam != null)
    //            {   
    //                CrossParts.Add(CrossBeam);
    //            }
    //        }
    //    }

    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("GetCrossPartsatBothEnd Original error: " + ex.Message);
    //    }
    //    return CrossParts;
    //}

    public static List<Tekla.Structures.Model.ModelObject> GetCrossModelObject(Tekla.Structures.Geometry3d.Point MyPoint, double Tolerance, string sIgnoreClass, string sIgnoreName)
    {
        List<Tekla.Structures.Model.ModelObject> CrossParts = new List<Tekla.Structures.Model.ModelObject>();
        try
        {
            // Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {
                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);

                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;

                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
                while (CrossEnum.MoveNext())
                {
                    Tekla.Structures.Model.ModelObject CrossBeam = CrossEnum.Current as Tekla.Structures.Model.ModelObject;

                    if (CrossBeam != null)
                    {
                        CrossParts.Add(CrossBeam);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossParts1. Original error: " + ex.Message);
        }
        return CrossParts;
    }

    public static List<Tekla.Structures.Model.Beam> GetCrossParts(Tekla.Structures.Geometry3d.Point MyPoint, double Tolerance, string sIgnoreClass, string sIgnoreName)
    {
        List<Tekla.Structures.Model.Beam> CrossParts = new List<Tekla.Structures.Model.Beam>();

        try
        {

            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);


                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;

                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                while (CrossEnum.MoveNext())
                {
                    Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                    if (CrossBeam != null)
                    {
                        string guid = "";
                        CrossBeam.GetReportProperty("GUID", ref guid);
                        double CheckLength = 0.0;
                        CrossBeam.GetReportProperty("LENGTH", ref CheckLength);

                        string name = "";
                        CrossBeam.GetReportProperty("NAME", ref name);
                        double chkLength = TSG.Distance.PointToPoint(CrossBeam.StartPoint, CrossBeam.EndPoint);
                        double chkStartPointdist = TSG.Distance.PointToPoint(CrossBeam.StartPoint, MyPoint);
                        double chkEndPointdist = TSG.Distance.PointToPoint(CrossBeam.EndPoint, MyPoint);
                        //CheckLength = Math.Abs(CheckLength - (chkStartPointdist + chkEndPointdist));
                        CheckLength = Math.Abs(chkLength - (chkStartPointdist + chkEndPointdist));
                        if ((chkStartPointdist <= Tolerance) || (chkEndPointdist <= Tolerance) || CheckLength <= Tolerance)
                        {
                            string PProfileType = "";
                            CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);
                            if (PProfileType != "B" && sIgnoreClass.Contains(CrossBeam.Class) == false && sIgnoreName.Trim().ToUpper().Replace(" ", "").Contains(CrossBeam.Name.Trim().ToUpper().Replace(" ", "")) == false)
                            //if (PProfileType != "B")
                            {
                                //"\"e4b780d0-ef6b-4f28-83e9-fced34579ccf\" 9cd8824a-1412-4663-badc-e1f3b4e94b8b\" \"8faf8456-519b-4670-9aba-d3e1172fd5b6\""
                                CrossParts.Add(CrossBeam);
                            }
                        }
               
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossParts1. Original error: " + ex.Message);
        }
        return CrossParts;
    }


    public static List<Tekla.Structures.Model.Beam> GetCrossParts(Tekla.Structures.Geometry3d.Point MyPoint1, Tekla.Structures.Geometry3d.Point MyPoint2, double Tolerance, string sIgnoreClass, string sIgnoreName)
    {
        List<Tekla.Structures.Model.Beam> CrossParts = new List<Tekla.Structures.Model.Beam>();

        try
        {

            // MyModel = new TSM.Model();
            // ModelPath = MyModel.GetInfo().ModelPath;

            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MyPoint1, MyPoint2);

                while (CrossEnum.MoveNext())
                {
                    Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                    if (CrossBeam != null)
                    {
                        string PProfileType = "";
                        CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);
                        if (PProfileType != "B" && sIgnoreClass.Contains(CrossBeam.Class) == false && sIgnoreName.Trim().ToUpper().Replace(" ", "").Contains(CrossBeam.Name.Trim().ToUpper().Replace(" ", "")) == false)
                            {
                            CrossParts.Add(CrossBeam);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossParts2. Original error: " + ex.Message);
        }
        return CrossParts;
    }

    public static ArrayList GetCrossPartsID(Tekla.Structures.Geometry3d.Point MyPoint, Double Tolerance)
    {
        ArrayList CrossParts = new ArrayList();
        //string ids = "";

        try
        {

            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {


                //Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                //Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);

                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);

                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;

                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;

                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);
               
                while (CrossEnum.MoveNext())
                {
                       
                    //Tekla.Structures.Model.ModelObject myObject = CrossEnum.Current as Tekla.Structures.Model.ModelObject;
                    //ids = ids + "\n" + myObject.Identifier.ID.ToString();

                    Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;

                    if (CrossBeam != null)
                    {
                        string PProfileType = "";
                        CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);

                        if (PProfileType != "B")
                        {
                            CrossParts.Add(CrossBeam.Identifier.ID);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossPartsID. Original error: " + ex.Message);
        }
        return CrossParts;
    }


    //added newly for including contour plates
    public static ArrayList GetCrossParts(Tekla.Structures.Geometry3d.Point MyPoint, Double Tolerance)
    {
        ArrayList CrossParts = new ArrayList();
        string ids = "";

        try
        {

            //Set up model Component w/TS
            //MyModel = new TSM.Model();
            //ModelPath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && MyModel.GetInfo().ModelPath != string.Empty)
            {

                //Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                //Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint);
                Tekla.Structures.Geometry3d.Point MinP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);
                Tekla.Structures.Geometry3d.Point MaxP = new Tekla.Structures.Geometry3d.Point(MyPoint.X, MyPoint.Y, MyPoint.Z);

                MinP.X = MinP.X - Tolerance;
                MinP.Y = MinP.Y - Tolerance;
                MinP.Z = MinP.Z - Tolerance;

                MaxP.X = MaxP.X + Tolerance;
                MaxP.Y = MaxP.Y + Tolerance;
                MaxP.Z = MaxP.Z + Tolerance;


                //ModelObjectEnumerator.AutoFetch = true;
                ModelObjectEnumerator CrossEnum = MyModel.GetModelObjectSelector().GetObjectsByBoundingBox(MinP, MaxP);

                //if (debugflag == true)
                //{
                //    drawer.DrawText(MinP, "MinP", new TSMUI.Color(0, 0.5, 0));
                //    drawer.DrawText(MaxP, "MaxP", new TSMUI.Color(0, 0.5, 0));
                //}

                while (CrossEnum.MoveNext())
                {

                    Tekla.Structures.Model.ModelObject myObject = CrossEnum.Current as Tekla.Structures.Model.ModelObject;
                    ids = ids + "\n" + myObject.Identifier.ID.ToString();

                    Tekla.Structures.Model.Beam CrossBeam = CrossEnum.Current as Tekla.Structures.Model.Beam;
                    //string PProfileType = "";
                    if (CrossBeam != null)
                    {
                        //CrossBeam.GetReportProperty("PROFILE_TYPE", ref PProfileType);

                        //if (PProfileType != "B")
                        //{
                            CrossParts.Add(CrossBeam);
                                
                        //}
                    }
                    Tekla.Structures.Model.ContourPlate CrossPlate = CrossEnum.Current as Tekla.Structures.Model.ContourPlate;
                    if (CrossPlate != null)
                    {

                        //CrossPlate.GetReportProperty("PROFILE_TYPE", ref PProfileType);

                        //if (PProfileType == "B")
                        //{
                            CrossParts.Add(CrossPlate);
                        //}
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("GetCrossParts3. Original error: " + ex.Message);
        }
        return CrossParts;
    }

//for (int i = 0; i<AttributeList.Count; i++)
//{
//string Attribute = AttributeList[i] as string;
//}

   

    public static LineSegment GetIntersection(Beam Beam1, Beam Beam2)
    {

        Tekla.Structures.Geometry3d.Line Line1 = new Tekla.Structures.Geometry3d.Line(Beam1.StartPoint, Beam1.EndPoint);
        Tekla.Structures.Geometry3d.Line Line2 = new Tekla.Structures.Geometry3d.Line(Beam2.StartPoint, Beam2.EndPoint);
        LineSegment Interseg = null;

        if (Tekla.Structures.Geometry3d.Parallel.LineToLine(Line1, Line2) == false)
        {
            Interseg = Intersection.LineToLine(Line1, Line2);
        }
        return Interseg;
    }

    public static double GetDistance(Tekla.Structures.Geometry3d.Point refPoint, Beam Beam2)
    {
        LineSegment Lineseg = new LineSegment();

        Lineseg.Point1 = Beam2.StartPoint;
        Lineseg.Point2 = Beam2.EndPoint;

        double intDistance = Distance.PointToLineSegment(refPoint, Lineseg);

        return intDistance;
    }

    public static void ObjectVisualization(List<Identifier> FailedList, List<Identifier> PassedList)
    {
        //clear all
        TSMUI.ModelObjectVisualization.ClearAllTemporaryStates();

        //all objects set as grey
        TSMUI.Color tsgrey = new TSM.UI.Color(0.75, 0.75, 0.75);
        TSMUI.ModelObjectVisualization.SetTemporaryStateForAll(tsgrey);

        //all objects which are error to be displayed as red
        TSMUI.Color tsred = new TSM.UI.Color(1.0, 0.0, 0.0);
        TSMUI.ModelObjectVisualization.SetTemporaryState(FailedList, tsred);

        //all objects which are correct/passed the condition to be displayed as green 
        TSMUI.Color tsgreen = new TSM.UI.Color(0.0, 1.0, 0.0); 
        TSMUI.ModelObjectVisualization.SetTemporaryState(PassedList, tsgreen); 
    }

    public static void ObjectVisualization(List<Identifier> ObjectList)
    {
        //clear all
        TSMUI.ModelObjectVisualization.ClearAllTemporaryStates();

        //all objects set as grey
        TSMUI.Color tsgrey = new TSM.UI.Color(0.75, 0.75, 0.75);
        TSMUI.ModelObjectVisualization.SetTemporaryStateForAll(tsgrey);

        //all objects which are error to be displayed as red
        TSMUI.Color tsblue= new TSM.UI.Color(0.0, 0.0, 1.0);
        TSMUI.ModelObjectVisualization.SetTemporaryState(ObjectList, tsblue);
    }

    public static DateTime setStartup(string skApplicationName, string skapplicationVersion, string Task, string Remark)
    {
        // Yearly Renewal required
        if (Convert.ToInt32(DateTime.Now.ToString("yyyy")) != esskayappvalidity)
        {
            if (Convert.ToInt32(DateTime.Now.ToString("MMdd")) >= 1226 && Convert.ToInt32(DateTime.Now.ToString("MMdd")) <= 1231)
            {

            }
            else
            {
                //skWinLib.DebugLog(debugflag, debugcount++, logfile, "ConnectToModel-Failed");
                skWinLib.worklog(skApplicationName, skapplicationVersion, "Esskay_Tool_Validation", "Start ;" + Convert.ToInt32(DateTime.Now.ToString("yyyy")).ToString(), ModelName, Version, Configuration);
                //skWinLib.accesslog(skApplicationName, skApplicationVersion, "Esskay_Tool_Validation", "YearlyValidation -" + Convert.ToInt32(DateTime.Now.ToString("yyyy")).ToString());
                System.Environment.Exit(0);
            }             
        }

        //12-apr-2022
        DateTime mytime = DateTime.Now;
        //lblsbar1.Visible = true;
        //lblsbar2.Visible = true;
        skWinLib.accesslog(skApplicationName, skapplicationVersion, Task, "Start ;" + Remark, ModelName, Version, Configuration);
        //Cursor.Current = Cursors.WaitCursor;
        return mytime;

    }

    public static void setCompletion(string skApplicationName, string skapplicationVersion, string Task, string Remark)
    {
        //12-apr-2022
        //lblsbar1.Visible = true;
        //lblsbar2.Visible = false;        
        skWinLib.worklog(skApplicationName, skapplicationVersion, Task, "Complete ;" + Remark, ModelName, Version, Configuration);
        Cursor.Current = Cursors.Default;

    }

        public static TimeSpan setCompletion(string skApplicationName, string skapplicationVersion, string Task, string Remark,DateTime startTime)
        {
            //12-apr-2022
            //lblsbar1.Visible = true;
            //lblsbar2.Visible = false;        
            skWinLib.worklog(skApplicationName, skapplicationVersion, Task, "Complete ;" + Remark, ModelName, Version, Configuration);
            Cursor.Current = Cursors.Default;
            TimeSpan span = DateTime.Now.Subtract(startTime);
            return span;
        }

        public static DateTime setStartup(string skApplicationName, string skapplicationVersion, string Task, string Remark, string statusbar1, string statusbar2, System.Windows.Forms.Label lblsbar1, System.Windows.Forms.Label lblsbar2)
        {
            // Yearly Renewal required
            if (Convert.ToInt32(DateTime.Now.ToString("yyyy")) != esskayappvalidity)
            {
                if (Convert.ToInt32(DateTime.Now.ToString("MMdd")) >= 1226 && Convert.ToInt32(DateTime.Now.ToString("MMdd")) <= 1231)
                {

                }
                else
                {
                    // skWinLib.DebugLog(debugflag, debugcount++, logfile, "ConnectToModel-Failed");
                    skWinLib.worklog(skApplicationName, skapplicationVersion, "Esskay_Tool_Validation", "Start ;" + Convert.ToInt32(DateTime.Now.ToString("yyyy")).ToString(), ModelName, Version, Configuration);
                    //skWinLib.worklog(skApplicationName, skapplicationVersion, Task, "Start ;" + Convert.ToInt32(DateTime.Now.ToString("yyyy")).ToString(), ModelName, Version, Configuration);
                    System.Environment.Exit(0);
                }

         
            }

            // 12-apr-2022
            DateTime mytime = DateTime.Now;
            lblsbar1.Visible = true;
            lblsbar2.Visible = true;
            if (statusbar2.Trim().Length >=1)
                lblsbar2.Text = statusbar2;
            lblsbar1.Text = statusbar1 + " < " + mytime.ToString("h:mm:ss") + " >...";
            if (skTSLib.MyModel != null)
            {
                try { Tekla.Structures.Model.Operations.Operation.DisplayPrompt(lblsbar1.Text); }
                catch { };
            }
            skWinLib.accesslog(skApplicationName, skapplicationVersion, Task, "Start ;" + Remark, ModelName, Version, Configuration);
            Cursor.Current = Cursors.WaitCursor;
            lblsbar1.Refresh();
            lblsbar2.Refresh();
            return mytime;          
        }

        public static void setCompletion(string skApplicationName, string skapplicationVersion, string Task, string Remark, string statusbar1, string statusbar2, System.Windows.Forms.Label lblsbar1, System.Windows.Forms.Label lblsbar2, DateTime startTime)
        {
            //12-apr-2022
            //lblsbar1.Visible = true;
            //lblsbar2.Visible = false;        
            skWinLib.worklog(skApplicationName, skapplicationVersion, Task, "Complete ;" + Remark,ModelName , Version,Configuration );
            Cursor.Current = Cursors.Default;
            TimeSpan span = DateTime.Now.Subtract(startTime);
            lblsbar1.Text = "Ready. " + statusbar1 + " <" + span.Minutes.ToString() + ":" + span.Seconds.ToString() + ">";
            if (statusbar2.Trim().Length >= 1)
                lblsbar2.Text = statusbar2;
            if (skTSLib.MyModel != null)
            {
                try { Tekla.Structures.Model.Operations.Operation.DisplayPrompt(lblsbar1.Text);}
                catch {};                      
            }
          

            lblsbar1.Refresh();
            lblsbar2.Refresh();
        }

        public static bool UpdatePartUserField(TSM.Part MyPart, string UserFieldAttribute, string SetUserFieldValue)
        {
            if (MyPart != null)
            {
                 if (UserFieldAttribute.ToUpper() == "FINISH")
                {
                    if (MyPart.Finish != SetUserFieldValue)
                    {
                        MyPart.Finish = SetUserFieldValue;
                        return true;
                    }
                }
                else
                {
                    string GetUserFieldValue = string.Empty;
                    MyPart.GetUserProperty(UserFieldAttribute, ref GetUserFieldValue);
                    if (GetUserFieldValue != SetUserFieldValue)
                    {
                        MyPart.SetUserProperty(UserFieldAttribute, SetUserFieldValue);
                        return true;
                    }
                }
            }
            return false;
        }



        public static string GetAssemblyFieldValue(TSM.Assembly MyAssembly, string GetAssemblyField)
        {
          if (MyAssembly != null)
           {
             TSM.ModelObject AssyMainPart = MyAssembly.GetMainPart();
             TSM.Part MyMainPart = AssyMainPart as TSM.Part;
             if (GetAssemblyField.ToUpper() == "FINISH")
                return MyMainPart.Finish;
             else /*if ((GetAssemblyField.ToUpper() == "FABRICATION_CODE") || (GetAssemblyField.ToUpper() == "COST_CODE"))*/
             {
                string GetUserFieldValue = string.Empty;
                MyMainPart.GetUserProperty(GetAssemblyField, ref GetUserFieldValue);
                return GetUserFieldValue;
             }
          }
            return "";
        }

        public static string GetSinglePartMainPartAttribute(TSM.Assembly MySinglePart, string GetSinglePartAttribute)
        {
            if (MySinglePart != null)
            {
                TSM.ModelObject AssyMainPart = MySinglePart.GetMainPart();
                TSM.Part MyMainPart = AssyMainPart as TSM.Part;
                if (GetSinglePartAttribute.ToUpper() == "FINISH")
                    return MyMainPart.Finish;
                else //if ((GetSinglePartAttribute.ToUpper() == "FABRICATION_CODE") || (GetSinglePartAttribute.ToUpper() == "COST_CODE"))
                {
                    string GetUserFieldValue = string.Empty;
                    MyMainPart.GetUserProperty(GetSinglePartAttribute, ref GetUserFieldValue);
                    return GetUserFieldValue;
                }
            }
            return "";
        }


        public static bool IsUpperFlangeCut(TSM.Beam MyBeam) 
        {
            //work only for I Shape or C Shape profile type
            double memlen = 0;
            double flglen = 0;
            MyBeam.Select();
            MyBeam.GetReportProperty("LENGTH", ref memlen);
            MyBeam.GetReportProperty("FLANGE_LENGTH_U", ref flglen);
            if (flglen < memlen)
                return true;
            else
                return false;
        }

        public static bool IsBottomFlangeCut(TSM.Beam MyBeam)
        {
            //work only for I Shape or C Shape profile type
            double memlen = 0;
            double flglen = 0;
            MyBeam.Select();
            MyBeam.GetReportProperty("LENGTH", ref memlen);
            MyBeam.GetReportProperty("FLANGE_LENGTH_B", ref flglen);
            if (flglen < memlen)
                return true;
            else
                return false;
        }

    public static string IsFlangeCut(TSM.Beam MyBeam)
    {
        //work only for I Shape or C Shape profile type
        double memlen = 0;
        double flglenu = 0;
        double flglenb = 0;
        MyBeam.Select();
        MyBeam.GetReportProperty("LENGTH", ref memlen);
        MyBeam.GetReportProperty("FLANGE_LENGTH_U", ref flglenu);
        MyBeam.GetReportProperty("FLANGE_LENGTH_B", ref flglenb);
        if (flglenu < memlen)        
        {
            if (flglenb < memlen)
                return "Cut@Top&Bottom";
            else
                return "Cut@Toponly";
        }
        else
        {
            if (flglenb < memlen)
                return "Cut@BottomOnly";
            else
                return "";
        }
    }
    

    public static double GetAngleBetweenVectorsinDegrees(TSG.Vector AxisX, TSG.Vector AxisY)
    {
        //using Tekla.Structures.Model;
        //using Tekla.Structures.ModelInternal;
        //using Tekla.Structures.Solid;
        //using Tekla.Structures.Geometry3d;
        //using TSM = Tekla.Structures.Model;
        //using TSG = Tekla.Structures.Geometry3d;
        //using TSMUI = Tekla.Structures.Model.UI;
        //using Tekla.Structures.Catalogs;
        //using System.Windows;
        ////using System.Numerics;
        //using Vector = Tekla.Structures.Geometry3d.Vector;

        double AngleDiff = 0;

        AngleDiff = (180 / 3.14159) * AxisX.GetAngleBetween(AxisY);
        AngleDiff = Math.Round(AngleDiff, 2);

        return AngleDiff;
    }

    //public static void ChangeSectionMarkName_Macros(string sectionname)
    //{
    //    new MacroBuilder().
    //        Callback("acmd_display_selected_drawing_object_dialog", "", "View_10 window_1").
    //        PushButton("csym_on_off", "csym_dial").
    //        TabChange("csym_dial", "tabbedwindow_5665", "tabbeditem_5644").
    //        ValueChange("csym_dial", "csym_label", sectionname).
    //        ValueChange("csym_dial", "csym_label_enable", "1").
    //        ValueChange("csym_dial", "SectionViewMarkA1TextContent_en", "1").
    //        PushButton("csym_modify", "csym_dial").
    //        PushButton("csym_cancel", "csym_dial").
    //        Run();
    //}

    private void MoveSectionMark(TSD.SectionMark section, int side, TSG.Point pick_point, TSD.View view)
    {
        double delta = 0;
        TSG.Point A = new TSG.Point();
        TSG.Point B = new TSG.Point();
        switch (side)
        {
            case 0: //eSide.Left:
                A = section.LeftPoint;
                B = section.RightPoint;
                delta = GetdeltaforSectionMark(A, B, pick_point, view);
                if (delta == double.MinValue)
                    break;
                section.Attributes.LineWidthOffsetLeft = delta;
                break;
            case 1: //eSide.Right:
                A = section.RightPoint;
                B = section.LeftPoint;
                delta = GetdeltaforSectionMark(A, B, pick_point, view);
                if (delta == double.MinValue)
                    break;
                section.Attributes.LineWidthOffsetRight = delta;
                break;
            default:
                break;
        }
        section.Modify();
    }
    public static double GetdeltaforSectionMark(TSG.Point A, TSG.Point B, TSG.Point pick_point, TSD.View  view)
    {
        
        double delta = 0;
        double scale = view.Attributes.Scale;

        if (Math.Round(B.Y - A.Y, 0) == 0)
        {
            delta = pick_point.X - A.X;
            if (delta < 0)
            {
                delta = A.X - pick_point.X;
            }
        }
        else if (Math.Round(B.X - A.X, 0) == 0)
        {
            delta = pick_point.Y - A.Y;
            if (delta < 0)
            {
                delta = A.Y - pick_point.Y;
            }
        }
        else
        {
            //DisplayException();
            return double.MinValue;
        }
        TSG.Vector AB = new TSG.Vector(B - A);
        TSG.Vector A2point = new TSG.Vector(pick_point - A);
        double angle4A = GetAngleBetweenVectorsinDegrees(AB, A2point);
        delta = (angle4A < 90) ? delta * (-1) : delta;
        delta = delta / scale;

        return delta;
    }
    //public static void DisplayException()
    //{
    //    string grating_message = "Alignment is only available when \nsection line is perpendicular to vector(x,0) or vector(0,y)";
    //    string caption1 = "Info";
    //    var x1 = MessageBox.Show(grating_message, caption1, MessageBoxButtons.OK, MessageBoxIcon.Information);
    //    //TSM.Operations.Operation.DisplayPrompt("alignment is only available when section line is perpendicular to vector(x,0) and vector(0,y)");
    //}
    //public static double GetAngleBetweenVectorsinDegrees(TSG.Vector AxisX, TSG.Vector AxisY)
    //{
    //    double AngleDiff = 0;

    //    AngleDiff = (180 / 3.14159) * AxisX.GetAngleBetween(AxisY);
    //    AngleDiff = Math.Round(AngleDiff, 2);

    //    return AngleDiff;
    //}

    public static List<int> TransferAssemblyInfotoSecondary(string FieldAttribute, System.Windows.Forms.ProgressBar MyProgressBar = null)
    {
        //Simple Macro to copy finsih of assembly (main part) to parts
        List<int> MyReturnData = new List<int>();

        //getting information from object selected in model
        TSMUI.ModelObjectSelector modelObjectSelector = new TSMUI.ModelObjectSelector();
        TSM.ModelObjectEnumerator modelObjectEnumerator = modelObjectSelector.GetSelectedObjects();

        ArrayList uniquemobj = new ArrayList();
        //string FieldAttribute = "FINISH";
        int totct = modelObjectEnumerator.GetSize();
        int updct = 0;
        int errct = 0;
        if (MyProgressBar != null && totct >= 1)
        {
            MyProgressBar.Value = 0;
            MyProgressBar.Maximum = totct;
            MyProgressBar.Visible = true;
        }

        //converting part to assembly finish
        while (modelObjectEnumerator.MoveNext())
        {
            if (MyProgressBar != null)
                MyProgressBar.Value = MyProgressBar.Value + 1;

            TSM.Part mypart = modelObjectEnumerator.Current as TSM.Part;
            if (mypart != null)
            {
                //for speeding up unique added now
                if (!uniquemobj.Contains(mypart))
                {

                    TSM.Assembly MyAssembly = mypart.GetAssembly();

                    string AssyInfo = skTSLib.GetAssemblyFieldValue(MyAssembly, FieldAttribute);
                    if (AssyInfo.Trim().Length >= 1)
                    {
            
                        //Updating assembly information to part user field
                        foreach (var MyPart in MyAssembly.GetSecondaries())
                        {
                            TSM.Part MyPartData = MyPart as TSM.Part;
                            if (skTSLib.UpdatePartUserField(MyPartData, FieldAttribute, AssyInfo) == true)
                            {
                                MyPartData.Modify();
                                updct++;
                            }
                            else
                                errct++;

                            uniquemobj.Add(MyPartData);

                        }
                    }
                }
            }
        }

        //MessageBoxIcon myMessageBoxIcon = MessageBoxIcon.None;
        string msg = "\n\n\nTotal Assemby Processed: " + totct;
        if (updct == 0 && errct == 0)
        {
            msg = msg + "\nNothing to update";
            //myMessageBoxIcon = MessageBoxIcon.Asterisk;
        }
        else
        {
            if (updct != 0 && errct != 0)
            {
                msg = msg + "\n\nPart(s) updated: " + updct + "\nPart(s) already updated: " + errct;
                //myMessageBoxIcon = MessageBoxIcon.Error;
            }
            else
            {
                if (updct != 0)
                {
                    msg = msg + "\nPart(s) updated: " + updct;
                    //myMessageBoxIcon = MessageBoxIcon.Warning;
                }
                else
                {
                    msg = msg + "\nPart(s) already updated: " + errct;
                    //myMessageBoxIcon = MessageBoxIcon.Exclamation;
                }
            }

        }


        if (MyProgressBar != null)
            MyProgressBar.Visible = false;


        MyReturnData.Add(errct);
        MyReturnData.Add(totct);
        MyReturnData.Add(0);
        MyReturnData.Add(0);
        MyReturnData.Add(0);
        MyReturnData.Add(updct);


        return MyReturnData;

    }


    private void Example_Filter_Assembly()
    {
        TSM.Model MyModel = new TSM.Model();
        BinaryFilterExpression filterExpression1 = new BinaryFilterExpression(new AssemblyFilterExpressions.PositionNumber(), StringOperatorType.IS_EQUAL,
                            new StringConstantFilterExpression(string.Join(" ", new List<string> { "4002B", "11002C" })));

        BinaryFilterExpression filterExpression2 = new BinaryFilterExpression(new ObjectFilterExpressions.Type(), NumericOperatorType.IS_EQUAL,
           new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.ASSEMBLY));

        var filterDefinition = new BinaryFilterExpressionCollection();
        filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression1, BinaryFilterOperatorType.BOOLEAN_AND));
        filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression2)); ;

        ModelObjectEnumerator moe = MyModel.GetModelObjectSelector().GetObjectsByFilter(filterDefinition);
        int moecount = moe.GetSize();           

        if (skTSLib.MyModel != null)
        {
            try { Tekla.Structures.Model.Operations.Operation.DisplayPrompt(moecount.ToString()); }
            catch { };
        }

    }


    //231027 Read File from "T:\SupportFiles\SK_Angle_Gage.dat" and return string[] which contain AngleGageData [AngleLegLength & Gage] in metric units
    public static string[] GetAngleGageList()
    {
        string AngleGageFile = @"T:\SupportFiles\SK_Angle_Gage.dat";
        if (System.IO.File.Exists(AngleGageFile) == true)
        {
            return System.IO.File.ReadAllLines(AngleGageFile);
        }

        return null;
    }

    public static string[] GetEnroachmentvalue()
    {
        string EncroachmentFile = @"T:\SupportFiles\SK_KValue_Ench.dat";
        if (System.IO.File.Exists(EncroachmentFile) == true)
        {
            return System.IO.File.ReadAllLines(EncroachmentFile);
        }

        return null;
    }

    //231027 AngleGageData and AngleLegLength when inputed will return Gage in metric units
    public static double GetAngleGage(string[] AngleGageData, double AngleLegLength)
    {
        Tekla.Structures.Datatype.Distance mydist = new Tekla.Structures.Datatype.Distance();
        for (int i = 1; i < AngleGageData.Count() - 1; i++)
        {
            //Loop through each angle line            
            string[] spltdata = AngleGageData[i].Split(new Char[] { ',' });
            if (spltdata.Length >= 1)
            {
                string sleglength = spltdata[0];
                mydist = Tekla.Structures.Datatype.Distance.FromFractionalFeetAndInchesString(sleglength, CultureInfo.InvariantCulture, Tekla.Structures.Datatype.Distance.UnitType.Inch);
                double dleglength = mydist.Millimeters;

                //check angle leg length and db length are same
                if (Math.Abs(AngleLegLength - dleglength) <= 0.25)
                {
                    string sboltgage = spltdata[1];
                    mydist = Tekla.Structures.Datatype.Distance.FromFractionalFeetAndInchesString(sboltgage, CultureInfo.InvariantCulture, Tekla.Structures.Datatype.Distance.UnitType.Inch);
                    return mydist.Millimeters;

                }
            }
        }
        return -1;
    }
    
    //231027 AngleGageData, AngleLegLength and HoleLocation when inputed will check hole location is greater than or equal to Gage. Return true if condition passed else false. 
    public static bool CheckAngleGage(string[] AngleGageData, double AngleLegLength, double HoleLocation)
    {
        double dHoleLocation = GetAngleGage(AngleGageData, AngleLegLength);
        if (dHoleLocation != -1)
        {
            double clearence = dHoleLocation - HoleLocation;
            double tol = 0.5;
            if (Math.Abs(clearence) < tol)
                return true;
        }
        return false;

    }

    public static List<double> getLowerHigherBoltLength(string name, double diameter, double length, string din)
    {
        List<double> boltlength = new List<double>();
        //ArrayList result = new ArrayList();
        //result.Add("Esskay");


        List<double> returnboltlength = new List<double>();
        returnboltlength.Add(0.0);

        System.IO.StreamReader screwdblis = new System.IO.StreamReader(skTSLib.ModelPath + "\\screwdb.lis");
        string line;
        double mytolerance = 0.009;
        while ((line = screwdblis.ReadLine()) != null)
        {
            string[] split = line.Split(new Char[] { ',' });
            if (split.Count() >= 3)
            {
                string chkname = split[0];
                string chkdin = split[3];
                if (skWinLib.IsNumeric(split[1]))
                {
                    double chkdiameter = Convert.ToDouble(split[1]);
                    if (chkname.ToUpper().Contains(name.ToUpper()) == true)
                    {
                        if (Math.Abs(chkdiameter - diameter) < mytolerance)
                        {
                            if (din.ToUpper().Trim().Contains(chkdin.ToUpper().Trim()) == true)
                            {
                                double screwdblength = Convert.ToDouble(split[2]);
                                boltlength.Add(screwdblength);
                            }
                        }

                    }
                }

            }
            //Console.WriteLine(line);
        }
        screwdblis.Close();
        boltlength.Sort();

        for (int i = 0; i < boltlength.Count; i++)
        {
            if (boltlength[i] >= length)
            {
                if (length == boltlength[i])
                {
                    returnboltlength.Add(boltlength[i]);
                    if (i + 1 < boltlength.Count)
                        returnboltlength.Add(boltlength[i + 1]);
                    else
                        returnboltlength.Add(-1); //last bolt

                    return returnboltlength;
                }
                else
                {
                    if (i != 0)
                        returnboltlength.Add(boltlength[i - 1]);
                    else
                        returnboltlength.Add(-1); //first bolt
                    returnboltlength.Add(boltlength[i]);

                    return returnboltlength;

                }

            }
        }

        return returnboltlength;
    }

    public static double getStickout(double boltdiameter)
    {
        double Stickout = 0;

        if (boltdiameter <= 12.70)
            Stickout = 3.0 / 16.0;
        else if (boltdiameter <= 25.40)
            Stickout = 1.0 / 4.0;
        else if (boltdiameter > 25.40)
            Stickout = 3.0 / 8.0;
        //else if (boltdiameter <= 38.1)
        //    Stickout = 3 / 8.0;
        //else if (boltdiameter > 38.1)
        //    Stickout = 3 / 8.0;

        return Stickout * 25.4;
    }

    public static List<TSD.Part> GetViewParts(TSD.View view)
    {
        TSM.TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane()); //must set the global workplane first     
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(view.DisplayCoordinateSystem));   //set the workplane to viewplane
        TSMUI.ViewHandler.RedrawWorkplane();
        var tsdparts = view.GetAllObjects(typeof(TSD.Part));
        List<TSD.Part> list = new List<TSD.Part>();
        while (tsdparts.MoveNext())
        {
            TSD.Part tsdPart = tsdparts.Current as TSD.Part;
            TSM.ModelObject MyModelObject = MyModel.SelectModelObject(tsdPart.ModelIdentifier);
            if (MyModelObject != null)
            {
                TSM.Part modelPart = MyModelObject as TSM.Part;
                if (modelPart != null)
                {
                    Solid MySolid = modelPart.GetSolid();

                    var minPt = MySolid.MinimumPoint;
                    var maxPt = MySolid.MaximumPoint;
                    if (minPt.X < view.RestrictionBox.MinPoint.X) continue;
                    if (minPt.Y < view.RestrictionBox.MinPoint.Y) continue;
                    if (minPt.Z < view.RestrictionBox.MinPoint.Z) continue;

                    if (maxPt.X > view.RestrictionBox.MaxPoint.X) continue;
                    if (maxPt.Y > view.RestrictionBox.MaxPoint.Y) continue;
                    if (maxPt.Z > view.RestrictionBox.MaxPoint.Z) continue;
                    list.Add(tsdPart);
                }
            }
        }
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
        TSMUI.ViewHandler.RedrawWorkplane();
        return list;
    }

    public static void CheckBoltLength(DataGridView MyDataGridView, ProgressBar MyProgressBar = null, double Tolerance = 0.0)
    {

        if (MyProgressBar != null)
            MyProgressBar.Visible = true;

        int totct = 0;
        int ct = 0;
        int ect = 0;
        try
        {

            //export_catalog_modelfolder("getScrewdb");
            //export_catalog_modelfolder("getassdb");
            skTSLib.export_screw_database();
            //export_ass_database();
            //Set up model connection w/TS
            Tekla.Structures.Model.Model MyModel = new Tekla.Structures.Model.Model();
            //if (MyModel.GetConnectionStatus() == true)
            //{
            //    Tekla.Structures.Model.ModelObjectEnumerator MdlObjAssy = MyModel.GetModelObjectSelector().GetAllObjects();
            //}

            string modelpath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && modelpath != string.Empty)
            {
                TSM.TransformationPlane CurrentTP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                TSMUI.ViewHandler.RedrawWorkplane();
                          
                TSM.ModelObjectEnumerator MyEnum = (new TSM.UI.ModelObjectSelector()).GetSelectedObjects();
                string Phase = "";
                //string mytype = "";
                MyDataGridView.Rows.Clear();

                if (MyProgressBar != null)
                {
                    MyProgressBar.Maximum = MyEnum.GetSize();
                    MyProgressBar.Value = 0;
                }


                //add required column in datagrid
                MyDataGridView.Columns.Clear();
                MyDataGridView.Columns.Add("guid", "GUID");
                MyDataGridView.Columns.Add("check", "check");
                MyDataGridView.Columns.Add("Phase", "Phase");
                MyDataGridView.Columns.Add("Dia", "Dia");
                MyDataGridView.Columns.Add("Std", "Std");
                MyDataGridView.Columns.Add("CalcLen", "CalcLen");
                MyDataGridView.Columns.Add("BoltLength", "BoltLength");
                MyDataGridView.Columns.Add("ExLen", "ExLen");
                MyDataGridView.Columns.Add("SKLen", "SKLen");
                MyDataGridView.Columns.Add("m1by32", "-1/32");
                MyDataGridView.Columns.Add("p1by16", "+1/16");
                MyDataGridView.Columns.Add("WashThk", "WashThk");
                MyDataGridView.Columns.Add("StickLen", "StickLen");
                MyDataGridView.Columns.Add("StickRmk", "StickRmk");
                MyDataGridView.Columns.Add("GripLen", "GripLen");
                MyDataGridView.Columns.Add("Plys", "Plys");
                MyDataGridView.Columns.Add("ThreadLen", "ThreadLen");
                MyDataGridView.Columns.Add("ShankLen", "ShankLen");
                MyDataGridView.Columns.Add("ShankRmk", "ShankRmk");

                while (MyEnum.MoveNext())
                {
                    TSM.ModelObject current = MyEnum.Current;
                    var guid = current.Identifier.GUID;
                    Console.WriteLine(guid);
                    totct++;
                    if (current is TSM.BoltGroup)
                    {
                        ct++;
                        TSM.BoltGroup myBoltGroup = current as TSM.BoltGroup;
                        //if (myBoltGroup.BoltStandard.ToString().Trim().ToUpper().Contains(boltstandard.Trim().ToUpper()))
                        {
                            bool boltflag = myBoltGroup.Bolt;
                            if (boltflag == true)
                            {
                                CoordinateSystem MyCoordinateSystem = myBoltGroup.GetCoordinateSystem();
                                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(MyCoordinateSystem));
                                TSMUI.ViewHandler.RedrawWorkplane();
                                myBoltGroup.Select();


                                myBoltGroup.GetPhase(out TSM.Phase ph);
                                Phase = ph.PhaseName;


                                double bltdiameter = myBoltGroup.BoltSize;
                                string bltdin = myBoltGroup.BoltStandard;

                                double bltlen = 0;
                                myBoltGroup.GetReportProperty("LENGTH", ref bltlen);

                                double grip = 0;
                                myBoltGroup.GetReportProperty("BOLT_MATERIAL_LENGTH", ref grip);

                                double thrdlen = 0;
                                myBoltGroup.GetReportProperty("BOLT_THREAD_LENGTH", ref thrdlen);


          

                                bool nut1 = myBoltGroup.Nut1;
                                double nutthick1 = 0.0;
                                string nutname1 = string.Empty;
                                string nuttype1 = string.Empty;

                                if (nut1 == true)
                                {
                                    myBoltGroup.GetReportProperty("NUT.NAME", ref nutname1);
                                    myBoltGroup.GetReportProperty("NUT.TYPE1", ref nuttype1);
                                    myBoltGroup.GetReportProperty("NUT.THICKNESS1", ref nutthick1);
                                }
                                bool nut2 = myBoltGroup.Nut2;
                                double nutthick2 = 0.0;
                                string nutname2 = string.Empty;
                                string nuttype2 = string.Empty;
                                if (nut2 == true)
                                {
                                    myBoltGroup.GetReportProperty("NUT.NAME", ref nutname2);
                                    myBoltGroup.GetReportProperty("NUT.TYPE2", ref nuttype2);
                                    myBoltGroup.GetReportProperty("NUT.THICKNESS2", ref nutthick2);
                                }

                                double nutthickness = nutthick1 + nutthick2;

                                bool wash1 = myBoltGroup.Washer1;
                                double washerthick1 = 0.0;
                                string swashtype1 = string.Empty;
                                if (wash1 == true)
                                {

                                    myBoltGroup.GetReportProperty("WASHER.THICKNESS1", ref washerthick1);
                                    myBoltGroup.GetReportProperty("WASHER.TYPE1", ref swashtype1);
                                }
                                bool wash2 = myBoltGroup.Washer2;
                                double washerthick2 = 0.0;
                                string swashtype2 = string.Empty;
                                if (wash2 == true)
                                {
                                    myBoltGroup.GetReportProperty("WASHER.THICKNESS2", ref washerthick2);
                                    myBoltGroup.GetReportProperty("WASHER.TYPE2", ref swashtype2);
                                }
                                bool wash3 = myBoltGroup.Washer3;
                                double washerthick3 = 0.0;
                                string swashtype3 = string.Empty;
                                if (wash3 == true)
                                {

                                    myBoltGroup.GetReportProperty("WASHER.THICKNESS3", ref washerthick3);
                                    myBoltGroup.GetReportProperty("WASHER.TYPE3", ref swashtype3);
                                }

                                double washerthickness = washerthick1 + washerthick2 + washerthick3;
                                double stickout = getStickout(bltdiameter);
                                double skcalclength = grip + nutthickness + washerthickness + stickout;
                                double plynutwash = (grip + nutthickness + washerthickness);
                                double tolnegative = (1.0 / 32.0) * 25.4;
                                //double tolpositive = (1.0 / 16.0) * 25.4;

                                double sklength_neg = -1;
                                double sklength_pos = -1;

                                List<double> skboltlengthlist = getLowerHigherBoltLength("BOLT", bltdiameter, skcalclength, bltdin);
                                if (skboltlengthlist.Count >= 2)
                                {
                                    sklength_neg = skboltlengthlist[1];
                                    sklength_pos = skboltlengthlist[2];
                                }

                                string chkflag1 = "ok";
                                //string sklengthinch = "";
                                System.Drawing.Color chkcolor = System.Drawing.Color.Green;
                                System.Drawing.Color negcolor = System.Drawing.Color.Green;
                                System.Drawing.Color poscolor = System.Drawing.Color.Green;

                                double chkflagmin = skcalclength - sklength_neg;
                                double chkflagmax = sklength_pos - skcalclength;

                                double boltsticklength = 0.0;

                                if (chkflagmin > tolnegative)
                                {
                                    boltsticklength = sklength_pos;
                                    negcolor = System.Drawing.Color.Red;
                                }

                                else
                                {
                                    boltsticklength = sklength_neg;
                                    poscolor = System.Drawing.Color.Red;
                                }


                                if (Math.Abs(boltsticklength - bltlen) > 2.0)
                                {
                                    //chkflag1 = "Check";
                                    ect++;
                                    chkcolor = System.Drawing.Color.Red;
                                }




                                double extralen = 0.0;
                                myBoltGroup.GetReportProperty("EXTRA_LENGTH", ref extralen);

                                //Tekla.Structures.Datatype.Distance distance4 = Tekla.Structures.Datatype.Distance.FromFractionalFeetAndInchesString("1'-10", CultureInfo.InvariantCulture, Tekla.Structures.Datatype.Distance.UnitType.Inch);
                                Tekla.Structures.Datatype.Distance bltdiamm = new Tekla.Structures.Datatype.Distance(bltdiameter);
                                string bldiainch = bltdiamm.ToFractionalInchesString();
                                //if (bldiainch.Substring(0,2) == "0\\")
                                //    bldiainch = bldiainch.Replace("0", "");

                                Tekla.Structures.Datatype.Distance griplenmm = new Tekla.Structures.Datatype.Distance(grip + washerthickness);
                                string griplenin = griplenmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance skcalclengthmm = new Tekla.Structures.Datatype.Distance(skcalclength);
                                string skcalclengthinch = skcalclengthmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance bltlenmm = new Tekla.Structures.Datatype.Distance(bltlen);
                                string bltleninch = bltlenmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance extralenmm = new Tekla.Structures.Datatype.Distance(extralen);
                                string extraleninch = extralenmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance negtolmm = new Tekla.Structures.Datatype.Distance(sklength_neg);
                                string negtolinch = negtolmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance postolmm = new Tekla.Structures.Datatype.Distance(sklength_pos);
                                string postolmminch = postolmm.ToFractionalInchesString();

                                Tekla.Structures.Datatype.Distance boltsticklengthmm = new Tekla.Structures.Datatype.Distance(boltsticklength);
                                string sklengthinch = boltsticklengthmm.ToFractionalInchesString();




                                //actual stick through is tekla bolt length minum plynutwash
                                double stickdiff = bltlen - plynutwash;
                                //double stickdiff = boltsticklength - plynutwash;

                                string stickrmk = string.Empty;

                                //Check for Bolt Diameter upto 1 1/8" 
                                if (bltdiameter <= ((18 / 16.0) * 25.4))
                                {
                                    if (stickdiff < ((1 / 4.0) * 25.4))
                                    {
                                        stickrmk = "<1/4";
                                        chkflag1 = "Stick Length Error";
                                        chkcolor = System.Drawing.Color.Red;
                                        negcolor = System.Drawing.Color.Red;
                                        poscolor = System.Drawing.Color.Green;
                                    }
                                    else if (stickdiff >= ((1 / 2.0) * 25.4))
                                    {
                                        stickrmk = ">=1/2";
                                        chkflag1 = "Stick Length Error";
                                        chkcolor = System.Drawing.Color.Red;
                                        negcolor = System.Drawing.Color.Red;
                                        poscolor = System.Drawing.Color.Green;
                                    }
                                }
                                else
                                {
                                    //for bolt greter than 1 1/8"
                                    if (stickdiff < ((3 / 8.0) * 25.4))
                                    {
                                        stickrmk = "<3/8";
                                        chkflag1 = "Stick Length Error";
                                        chkcolor = System.Drawing.Color.Red;
                                        negcolor = System.Drawing.Color.Red;
                                        poscolor = System.Drawing.Color.Green;
                                    }
                                    else if (stickdiff >= ((3 / 4.0) * 25.4))
                                    {
                                        stickrmk = ">=3/4";
                                        chkflag1 = "Stick Length Error";
                                        chkcolor = System.Drawing.Color.Red;
                                        negcolor = System.Drawing.Color.Red;
                                        poscolor = System.Drawing.Color.Green;
                                    }
                                }

                                //Check for Threads in  Shear Plane
                                Tekla.Structures.Datatype.Distance thrdlenmm = new Tekla.Structures.Datatype.Distance(thrdlen);
                                string thrdleninch = thrdlenmm.ToFractionalInchesString();

                                double shanklen = bltlen - thrdlen;
                                Tekla.Structures.Datatype.Distance shanklenmm = new Tekla.Structures.Datatype.Distance(shanklen);
                                string shankleninch = shanklenmm.ToFractionalInchesString();



                                //ModelObjectEnumerator MyModelObjectEnumerator = myBoltGroup.GetChildren();
                                //int plyct = MyModelObjectEnumerator.GetSize();



                                //ArrayList BoltPositions = myBoltGroup.BoltPositions;
                                int plyct = 0;
                                TSM.Part MyPartToBeBolted = myBoltGroup.PartToBeBolted;
                                TSM.Part MyPartToBoltTo = myBoltGroup.PartToBoltTo;
                                if (MyPartToBeBolted != null)                    
                                    plyct++;
                                if (MyPartToBoltTo != null)
                                    plyct++;
                                ArrayList MyOtherPartsToBolts = myBoltGroup.OtherPartsToBolt;
                                plyct = plyct + MyOtherPartsToBolts.Count;
                                //int bct = BoltPositions.Count;
                                //int bltposid = 1;
                                int bltthrnlenerr = 0;
                                string shankrmk = string.Empty;
                                foreach (TSG.Point BoltPosition in myBoltGroup.BoltPositions)
                                {
                                    //skTSLib.drawer.DrawText(BoltPosition, bltposid++.ToString(), new TSMUI.Color(0, 0, 1));
                                    string result = CheckThreadsinShearPlane(myBoltGroup, BoltPosition, shanklen, washerthick1, Tolerance);
                                    if (result.ToUpper() != "OK" && result.ToUpper() != "")
                                    {
                                        bltthrnlenerr++;
                                        shankrmk = result;
                                        ect++;
                                    }
                                }



                                if (chkflag1 == "Stick Length Error" && shankrmk != string.Empty)
                                    chkflag1 = "Stick Length Error & Threads in Shear Plane";
                                else if (chkflag1 == "ok" && shankrmk != string.Empty)
                                    chkflag1 = "Error Threads in Shear Plane";


                                //stick convertion
                                Tekla.Structures.Datatype.Distance stickdiffmm = new Tekla.Structures.Datatype.Distance(stickdiff);
                                string stickdiffinch = stickdiffmm.ToFractionalInchesString();

                                //Washer thick converion
                                Tekla.Structures.Datatype.Distance washerthicknessmm = new Tekla.Structures.Datatype.Distance(washerthickness);
                                string swasherthicknessinch = washerthicknessmm.ToFractionalInchesString();

                                //string sklength = sklength_neg;


                                //if ((chkdtl.Checked == true) || chkflag1 == "Check")
                                //if (chkflag1 == "Check")
                                {
                                    MyDataGridView.Rows.Add(guid.ToString(), chkflag1, Phase, bldiainch, myBoltGroup.BoltStandard.ToString(), skcalclengthinch, bltleninch, extraleninch, sklengthinch, negtolinch, postolmminch, swasherthicknessinch, stickdiffinch, stickrmk, griplenin, plyct, thrdleninch, shankleninch, shankrmk);
                                    //MyDataGridView.Rows[dgvstickoutcheck.Rows.Count - 1].HeaderCell.Value = (dgvstickoutcheck.Rows.Count).ToString();
                                    //if(chkflag1 == "Check")                                            
                                    //if (stickrmk != "")
                                    //{
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[1].Style.BackColor = chkcolor;
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[9].Style.BackColor = negcolor;
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[10].Style.BackColor = poscolor;
                                    //}
                                    //else
                                    //{
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[1].Style.BackColor = System.Drawing.Color.Green;
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[9].Style.BackColor = System.Drawing.Color.Green;
                                    //    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[10].Style.BackColor = System.Drawing.Color.Green;
                                    //}

                                    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[1].Style.BackColor = chkcolor;
                                    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[9].Style.BackColor = negcolor;
                                    MyDataGridView.Rows[MyDataGridView.Rows.Count - 1].Cells[10].Style.BackColor = poscolor;

                                    //dgvbolt.Rows[dgvbolt.Rows.Count - 1].Cells[1].Style.BackColor = System.Drawing.Color.Red;
                                }

                            }

                        }


                    }
                    if (MyProgressBar != null)
                        MyProgressBar.Value = MyProgressBar.Value + 1;
                }
                MyDataGridView.AutoResizeColumns();
                //lblsbar1.Text = "[Error:" + ect.ToString() + "]. " + totct.ToString() + " / " + ct.ToString() + "Bolt processed.";

                MyDataGridView.Tag = ";Total:" + MyDataGridView.Rows.Count.ToString() + "; Error:" + ect;


                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(CurrentTP);
                TSMUI.ViewHandler.RedrawWorkplane();



                //label2.Text = listView1.Items.Count.ToString() + " Object(s) listed";
            } // end of if that checks to see if there is a model and one is open

            else
            {
                //If no connection to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch
        {
            MessageBox.Show("1");
        }

        if (MyProgressBar != null)
            MyProgressBar.Visible = false;

    }

    public static string CheckThreadsinShearPlane(TSM.BoltGroup myBoltGroup, TSG.Point BoltPosition, double ShankLength, double HeadWasherThick, double Tolerance)
    {

        TSM.Solid MySolid = myBoltGroup.GetSolid();
        //skTSLib.drawer.DrawText(MySolid.MaximumPoint, "MAX", new TSMUI.Color(1, 0, 0));
        //skTSLib.drawer.DrawText(MySolid.MinimumPoint, "MIN", new TSMUI.Color(1, 0, 0));


        //double dx = MySolid.MaximumPoint.X - MySolid.MinimumPoint.X;
        //double dy = MySolid.MaximumPoint.Y - MySolid.MinimumPoint.Y;
        double dz = MySolid.MaximumPoint.Z - MySolid.MinimumPoint.Z;

        TSG.Point BoltHeadPoint = new TSG.Point(BoltPosition.X, BoltPosition.Y, BoltPosition.Z + dz);
        TSG.Point BoltNutPoint = new TSG.Point(BoltPosition.X, BoltPosition.Y, BoltPosition.Z - dz);
        //skTSLib.drawer.DrawText(BoltHeadPoint, "HEAD", new TSMUI.Color(1, 0, 0));
        //skTSLib.drawer.DrawText(BoltNutPoint, "NUT", new TSMUI.Color(1, 0, 0));

        //int ict = 1;
        int plyct = 0;
        TSM.Part MyPartToBeBolted = myBoltGroup.PartToBeBolted;
        TSM.Part MyPartToBoltTo = myBoltGroup.PartToBoltTo;

        //ArrayList MyBoltIntPnts = new ArrayList();
        List<TSG.Point> MyBoltIntPnts = new List<TSG.Point>();
        if (MyPartToBeBolted != null)
        {
            plyct++;
            ArrayList MyIntPnts = MyPartToBeBolted.GetSolid().Intersect(BoltHeadPoint, BoltNutPoint);
            foreach (TSG.Point MyIntPnt in MyIntPnts)
            {
                if (MyBoltIntPnts.Contains(MyIntPnt) == false)
                    MyBoltIntPnts.Add(MyIntPnt);
            }
        }
        if (MyPartToBoltTo != null)
        {
            plyct++;
            ArrayList MyIntPnts = MyPartToBoltTo.GetSolid().Intersect(BoltHeadPoint, BoltNutPoint);
            foreach (TSG.Point MyIntPnt in MyIntPnts)
            {
                if (MyBoltIntPnts.Contains(MyIntPnt) == false)
                    MyBoltIntPnts.Add(MyIntPnt);
            }
        }
        ArrayList MyOtherPartsToBolts = myBoltGroup.OtherPartsToBolt;

        foreach (TSM.Part MyOtherPartsToBolt in MyOtherPartsToBolts)
        {
            if (MyOtherPartsToBolt != null)
            {
                plyct++;
                ArrayList MyIntPnts = MyOtherPartsToBolt.GetSolid().Intersect(BoltHeadPoint, BoltNutPoint);
                foreach (TSG.Point MyIntPnt in MyIntPnts)
                {
                    if (MyBoltIntPnts.Contains(MyIntPnt) == false)
                        MyBoltIntPnts.Add(MyIntPnt);
                }
            }
        }

        //sort based on z
        //MyBoltIntPnts.Sort(2);
        // Sort the points based on the z-coordinate asc           
        MyBoltIntPnts.Sort((a, b) => b.Z.CompareTo(a.Z));
        // Sort the points based on the z-coordinate desc
        //MyBoltIntPnts.Sort((a, b) => a.Z.CompareTo(b.Z));

        //foreach (TSG.Point MyIntPnt in MyBoltIntPnts)
        //{
        //    skTSLib.drawer.DrawText(MyIntPnt, ict++.ToString(), new TSMUI.Color(0, 0, 1));

        //}
        ArrayList MyDists = new ArrayList();
        double chkshearplanegrip = HeadWasherThick;
        string shankrmk = string.Empty;

        for (int i = 1; i + 1 < MyBoltIntPnts.Count; i++)
        {
            double dist = TSG.Distance.PointToPoint(MyBoltIntPnts[i - 1], MyBoltIntPnts[i]);
            MyDists.Add(dist);
            chkshearplanegrip = chkshearplanegrip + dist;

            //check the difference between shank len and each shear plane
            double shankdiff = ShankLength - chkshearplanegrip;

            //Tekla.Structures.Datatype.Distance shankdifflenmm = new Tekla.Structures.Datatype.Distance(shankdiff);
            //string shankdiffleninch = shankdifflenmm.ToFractionalInchesString();
            //string shankrmk = shankdiffleninch;// shankdiff.ToString("#0.0#");

            //double tol = (1 / 32.0) * -25.4;

            if (shankdiff < Tolerance)
            {
                Tekla.Structures.Datatype.Distance distmm = new Tekla.Structures.Datatype.Distance(chkshearplanegrip - ShankLength);
                string distin = distmm.ToFractionalInchesString();
                //chkcolor = System.Drawing.Color.Red;
                shankrmk = distin + "[" + (i + 1) + "] " + shankrmk;
                skTSLib.drawer.DrawText(MyBoltIntPnts[i], (i + 1).ToString() + "-" + distin, new TSMUI.Color(1, 0, 0));
            }

        }
        return shankrmk;
    }

    public static void CheckMarkWithDifferentProfile(DataGridView MyDataGridView, ProgressBar MyProgressBar = null)
    {

        if (MyProgressBar != null)
            MyProgressBar.Visible = true;

        int totct = 0;
        int ct = 0;
        int ect = 0;
        try
        {
           
            //Set up model connection w/TS
            TSM.Model MyModel = new TSM.Model();
            string modelpath = MyModel.GetInfo().ModelPath;
            if (MyModel.GetConnectionStatus() && modelpath != string.Empty)
            {


                TSM.ModelObjectEnumerator MyEnum = (new TSM.UI.ModelObjectSelector()).GetSelectedObjects();

                bool IsUnitImperial = skTSLib.IsUnitImperial;

                MyDataGridView.Rows.Clear();

                if (MyProgressBar != null)
                {
                    MyProgressBar.Maximum = MyEnum.GetSize() * 2;
                    MyProgressBar.Value = 0;
                }


                //add required column in datagrid
                MyDataGridView.Columns.Clear();
                MyDataGridView.Columns.Add("guid", "GUID");
                MyDataGridView.Columns.Add("assypos", "Assembly Mark");
                MyDataGridView.Columns.Add("partpos", "Part Mark");
                MyDataGridView.Columns.Add("Profile", "Profile");
                MyDataGridView.Columns.Add("length", "Length");
                MyDataGridView.Columns.Add("Phase", "Phase");
                MyDataGridView.Columns.Add("Remark", "Remark");
                ArrayList MyUnqPosition = new ArrayList();
                ArrayList MyUnqProfile = new ArrayList();
                ArrayList MyUnqLength = new ArrayList();
                ArrayList ErrorPosition = new ArrayList();
                ArrayList ErrorRemark = new ArrayList();

                while (MyEnum.MoveNext())
                {
                    totct++;
                    TSM.ModelObject MyModelObject = MyEnum.Current as TSM.ModelObject;
                    string guid = MyEnum.Current.Identifier.GUID.ToString();
                    string AssyMark = string.Empty;
                    string PartMark = string.Empty;
                    string Profile = string.Empty;
                    string Length = string.Empty;
                    string Phase = string.Empty;
                    string Remark = string.Empty;

          
                    Console.WriteLine(guid);
                    if (MyModelObject != null)
                    {
                        string motype = MyModelObject.GetType().Name.ToString().ToUpper();
                        if (motype == "PART" || motype == "BEAM" || motype == "POLYBEAM" || motype == "CURVEDBEAM" || motype == "CONTOURPLATE")
                        {
                            ct++;
                            MyModelObject.GetPhase(out TSM.Phase ph);
                            Phase = ph.PhaseName;
                            MyModelObject.GetReportProperty("ASSEMBLY_POS", ref AssyMark);
                            MyModelObject.GetReportProperty("PART_POS", ref PartMark);
                            MyModelObject.GetReportProperty("PROFILE", ref Profile);
                            double MOLen = 0;
                            MyModelObject.GetReportProperty("LENGTH", ref MOLen);
                            if (IsUnitImperial == true)
                            {
                                Tekla.Structures.Datatype.Distance MyDist = new Tekla.Structures.Datatype.Distance(MOLen);
                                Length = MyDist.ToFractionalInchesString();
                            }
                            else
                            {
                                Length = MOLen.ToString();
                            }
                            int idx = MyUnqPosition.IndexOf(PartMark);
                            if (idx == -1)
                            {
                                MyUnqPosition.Add(PartMark);
                                MyUnqProfile.Add(Profile);
                                MyUnqLength.Add(Length);
                            }
                            else
                            {
                                string lastprofile = MyUnqProfile[idx].ToString();
                                string lastlength = MyUnqLength[idx].ToString();
                                if (lastlength != Length && lastprofile != Profile)
                                    Remark = "Profile & Length Mismatch";
                                else if(lastprofile != Profile)
                                    Remark = "Profile Mismatch";
                                else if (lastlength != Length)
                                    Remark = "Length Mismatch";
                                if (Remark != "")
                                {
                                    ErrorPosition.Add(PartMark);
                                    ErrorRemark.Add(Remark);
                                    ect++; 
                                }

                            }
                            
                        }
                        else
                        {
                            Remark = "Not a Valid Type: " + motype;
                        }
                        MyDataGridView.Rows.Add(guid.ToString(), AssyMark, PartMark, Profile, Length, Phase, Remark);

                    }

                    


                    if (MyProgressBar != null)
                        MyProgressBar.Value = MyProgressBar.Value + 1;
                }
                //MyProgressBar.Value = MyProgressBar.Value + ((MyProgressBar.Maximum - MyProgressBar.Value) -(MyDataGridView.Rows.Count +1));
                foreach (DataGridViewRow MyDataGridViewRow in MyDataGridView.Rows)
                {
                    string rmk = MyDataGridViewRow.Cells[6].Value.ToString();
                    if (rmk == string.Empty)
                    {
                        string partpos = MyDataGridViewRow.Cells[2].Value.ToString();
                        int idx = ErrorPosition.IndexOf(partpos);
                        if (idx != -1)
                        {
                            ect++;
                            MyDataGridViewRow.Cells[6].Value = ErrorRemark[idx].ToString();
                        }
                    }
                    if (MyProgressBar != null)
                        MyProgressBar.Value = MyProgressBar.Value + 1;


                }
                //for(int i=0; i < MyUnqPosition.Count; i ++ )
                //{
                //    MyDataGridView.Rows.Add(guid.ToString(), AssyMark, PartMark, Profile, Length, Phase, Remark);
                //}
                

                MyDataGridView.Tag = ";Total:" + MyDataGridView.Rows.Count.ToString() + "; Error:" + ect;

            }
            else
            {
                //If no connection to model is possible show error message
                MessageBox.Show("Tekla Structures not open or model not open.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  //end of try loop

        catch 
        {
            MessageBox.Show("1");
        }

        if (MyProgressBar != null)
            MyProgressBar.Visible = false;

    }

    public static Dictionary<Plane, Face> GetGeometricPlanes(Solid solid)
    {
        var faceEnum = solid.GetFaceEnumerator();
        var planes = new Dictionary<Plane, Face>();
        while (faceEnum.MoveNext())
        {
            var planeVertexes = new List<TSG.Point>();
            var face = faceEnum.Current as Face;
            var loops = face.GetLoopEnumerator();
            while (loops.MoveNext())
            {
                var loop = loops.Current as Loop;
                var vertexes = loop.GetVertexEnumerator();

                while (vertexes.MoveNext())
                {
                    var vertex = vertexes.Current as TSG.Point;
                    if (!planeVertexes.Contains(vertex))
                    {
                        //Three points form a plane and they cannot be aligned.
                        if (planeVertexes.Count != 3 ||
                            (planeVertexes.Count == 3 && !ArePointAligned(planeVertexes[0], planeVertexes[1], vertex)))
                            planeVertexes.Add(vertex);

                        if (planeVertexes.Count == 3)
                        {
                            var vector1 = new Vector(planeVertexes[1].X - planeVertexes[0].X, planeVertexes[1].Y - planeVertexes[0].Y, planeVertexes[1].Z - planeVertexes[0].Z);
                            var vector2 = new Vector(planeVertexes[2].X - planeVertexes[0].X, planeVertexes[2].Y - planeVertexes[0].Y, planeVertexes[2].Z - planeVertexes[0].Z);

                            var plane = new Plane
                            {
                                Origin = planeVertexes[0],
                                AxisX = vector1,
                                AxisY = vector2
                            };
                            planes.Add(plane, face);
                            break;
                        }
                    }
                }
                break;
            }
        }
        return planes;
    }
    public static bool ArePointAligned(TSG.Point point1, TSG.Point point2, TSG.Point point3)
    {
        var vector1 = new Vector(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
        var vector2 = new Vector(point3.X - point1.X, point3.Y - point1.Y, point3.Z - point1.Z);
        return TSG.Parallel.VectorToVector(vector1, vector2);
    }
    public static List<List<TSG.Point>> GetFacePolygons(Face face)
    {
        var polygons = new List<List<TSG.Point>>();
        var loops = face.GetLoopEnumerator();
        while (loops.MoveNext())
        {
            var polygon = new List<TSG.Point>();
            var loop = loops.Current as Loop;
            var vertexes = loop.GetVertexEnumerator();

            while (vertexes.MoveNext())
            {
                var vertex = vertexes.Current as TSG.Point;
                polygon.Add(vertex);
            }

            polygons.Add(polygon);
        }

        return polygons;
    }

    public static void Example_SaveFilter_PartOnly(string filtername)
    {
        string AttributesPath = Path.Combine(skTSLib.ModelPath, "attributes");
        string FilterName = Path.Combine(AttributesPath, filtername);

        TSF.Filter Filter = new TSF.Filter(Example_Filter_PartOnly());
        // Generates the filter file
        Filter.CreateFile(FilterExpressionFileType.OBJECT_GROUP_SELECTION, FilterName);

    }

    public static BinaryFilterExpressionCollection Example_Filter_PartOnly()
    {
        //Set Filter
        var partExp = new BinaryFilterExpression(new ObjectFilterExpressions.Type(), NumericOperatorType.IS_EQUAL, new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.PART));
        return new BinaryFilterExpressionCollection { new BinaryFilterExpressionItem(partExp, BinaryFilterOperatorType.EMPTY) };
    }


    //private void Example_Filter_Assembly()
    //{
    //    TSM.Model MyModel = new TSM.Model();
    //    BinaryFilterExpression filterExpression1 = new BinaryFilterExpression(new AssemblyFilterExpressions.PositionNumber(), StringOperatorType.IS_EQUAL,
    //                        new StringConstantFilterExpression(string.Join(" ", new List<string> { "4002B", "11002C" })));

    //    BinaryFilterExpression filterExpression2 = new BinaryFilterExpression(new ObjectFilterExpressions.Type(), NumericOperatorType.IS_EQUAL,
    //       new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.ASSEMBLY));

    //    var filterDefinition = new BinaryFilterExpressionCollection();
    //    filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression1, BinaryFilterOperatorType.BOOLEAN_AND));
    //    filterDefinition.Add(new BinaryFilterExpressionItem(filterExpression2)); ;

    //    ModelObjectEnumerator moe = MyModel.GetModelObjectSelector().GetObjectsByFilter(filterDefinition);
    //    int moecount = moe.GetSize();
    //    Tekla.Structures.Model.Operations.Operation.DisplayPrompt(moecount.ToString());

    //}

    //public static float GetAngleBetweenPoints(TSG.Point Point1, TSG.Point Point2, TSG.Point Point3)
    //{
    //    float lengthA = Math.Sqrt(Math.Pow(Point2.X- Point1.X, 2) + Math.Pow(Point2.Y - Point1.y, 2));
    //    float lengthB = Math.Sqrt(Math.Pow(Point3.X- Point2.X, 2) + Math.Pow(Point3.y - Point2.y, 2));

    //    // Calculate the angle using trigonometry
    //    float angleInRadians = Math.Acos((Point2.X- Point1.X) * (Point3.X- Point2.X) + (Point2.y - Point1.y) * (Point3.y - Point2.y) / (lengthA * lengthB));

    //    // Convert radians to degrees
    //    float angleInDegrees = angleInRadians * (180f / Math.PI);

    //    return angleInDegrees;
    //    .x
    //        .y
    //        .z
    //}

    public static ArrayList SKCompareData(ArrayList cNewdata, ArrayList cOlddata, bool debug = false, bool errflag = false)
    {
        ArrayList MyReturnData = new ArrayList();
        if (cNewdata != null && cOlddata != null)
        {
            ArrayList NewDatas = new ArrayList();
            ArrayList OldDatas = new ArrayList();
            for (int i = 5; i < cNewdata.Count; i++)
                NewDatas.Add(cNewdata[i]);

            for (int i = 5; i < cOlddata.Count; i++)
                OldDatas.Add(cOlddata[i]);


            if (OldDatas.Count >=1 && NewDatas.Count >=1)
            {
                //Later for speeding up if profile type can be ignored when change in type
                //string newproftype = string.Empty;
                //string oldproftype = string.Empty;
                //if (cNewdata.Count > 3)
                //    newproftype = cNewdata[2].ToString();

                //if (cOlddata.Count > 3)
                //    oldproftype = cOlddata[2].ToString();

                //if (newproftype != oldproftype)
                //{
                //    MyReturnData.Add("ProfileType : " + newproftype + "!=" + oldproftype);
                //    return MyReturnData;
                //}

                string solrmk = string.Empty;
                string bltrmk = string.Empty;
                string wldrmk = string.Empty;
                string Olderr = string.Empty;
                string Newerr = string.Empty;



         






                double Tolerance = (1 / 64.0) * 25.4;
                //AssemblyCOGDifference = Tekla.Structures.Geometry3d.Distance.PointToPoint(OldCOG, NewCOG);
                ArrayList MyErrorData = new ArrayList();
                //Compare Model1 & Model2 Assembly remove no error
                for (int i = 0; i < OldDatas.Count; i++)
                {
                    //var Oldcog = OldDatas[1];
                    ArrayList ChkSecPart1 = OldDatas[i] as ArrayList;
                    if (ChkSecPart1 != null)
                    {
                        ArrayList ChkSecPart1Data = ChkSecPart1[2] as ArrayList;
                        List<CutData> ChkCut1 = ChkSecPart1Data[0] as List<CutData>;
                        List<ShopBoltData> ChkBolt1 = ChkSecPart1Data[1] as List<ShopBoltData>;
                        List<WeldData> ChkWeld1 = ChkSecPart1Data[2] as List<WeldData>;
                        for (int j = 0; j < NewDatas.Count; j++)
                        {
                            ArrayList ChkSecPart2 = NewDatas[j] as ArrayList;
                            //var Newcog = NewDatas[1];
                            //AssemblyCOGDifference = ((double)Oldcog - (double)Newcog) / 2.0; ;
                            if (ChkSecPart2 != null)
                            {
                                ArrayList ChkSecPart2Data = ChkSecPart2[2] as ArrayList;
                                List<CutData> ChkCut2 = ChkSecPart2Data[0] as List<CutData>;
                                List<ShopBoltData> ChkBolt2 = ChkSecPart2Data[1] as List<ShopBoltData>;
                                List<WeldData> ChkWeld2 = ChkSecPart2Data[2] as List<WeldData>;
                                if (ChkCut1 != null && ChkCut2 != null)
                                    solrmk = CompareCutData(ChkCut1, ChkCut2, Tolerance);
                                if (ChkBolt1 != null && ChkBolt2 != null)
                                    bltrmk = CompareBoltData(ChkBolt1, ChkBolt2, Tolerance);
                                if (ChkWeld1 != null && ChkWeld2 != null)
                                    wldrmk = CompareWeldData(ChkWeld1, ChkWeld2, Tolerance);

                                if (solrmk == "Cut Ok" && bltrmk == "Bolt Ok" && wldrmk == "Weld Ok")
                                {
                                    OldDatas.RemoveAt(i);
                                    NewDatas.RemoveAt(j);
                                    i--;
                                    j--;
                                    break;
                                }
                            }

                        }
                    }
                }

                if (OldDatas.Count >= 1 || NewDatas.Count >= 1)
                {
                    ArrayList NewErrors = GetErrorData(NewDatas);
                    ArrayList OldErrors = GetErrorData(OldDatas);
                    MyReturnData.Add(NewErrors);
                    MyReturnData.Add(OldErrors);
                    return MyReturnData;
                }
                else
                    return null;
            }
            else
            {
                MyReturnData.Add("E1");
                return MyReturnData;
            }


           





            

        }
        return null;
    }

    public static ArrayList GetErrorData(ArrayList ErrorDatas)
    {
        ArrayList MyReturnData = new ArrayList();
        List<CutData> CurErrors = new List<CutData>();
        List<ShopBoltData> BltErrors = new List<ShopBoltData>();
        List<WeldData> WldErrors = new List<WeldData>();

        for (int i = 0; i < ErrorDatas.Count; i++)
        {
            ArrayList PartData = ErrorDatas[i] as ArrayList;
            if (PartData != null)
            {
                ArrayList ChkPartData = PartData[2] as ArrayList;
                List<CutData> CutData = ChkPartData[0] as List<CutData>;
                List<ShopBoltData> BoltData = ChkPartData[1] as List<ShopBoltData>;
                List<WeldData> WeldData = ChkPartData[2] as List<WeldData>;
                if (CutData.Count >= 1)
                    CurErrors.AddRange(CutData);
                if (BoltData.Count >= 1)
                    BltErrors.AddRange(BoltData);
                if (WeldData.Count >= 1)
                    WldErrors.AddRange(WeldData);
            }
        }

        if (CurErrors.Count == 0 && BltErrors.Count == 0 && WldErrors.Count == 0)
            return null;
        else
        {
            if (CurErrors.Count >= 1)
                MyReturnData.Add(CurErrors);
            else
                MyReturnData.Add(null);

            if (BltErrors.Count >= 1)
                MyReturnData.Add(BltErrors);
            else
                MyReturnData.Add(null);

            if (WldErrors.Count >= 1)
                MyReturnData.Add(WldErrors);
            else
                MyReturnData.Add(null);
        }




        return MyReturnData;
    }


    //public static ArrayList GetErrorData(ArrayList ErrorDatas)
    //{
    //    ArrayList MyReturnData = new ArrayList();
    //    ArrayList CurErrors = new ArrayList();
    //    ArrayList BltErrors = new ArrayList();
    //    ArrayList WldErrors = new ArrayList();

    //    for (int i = 0; i < ErrorDatas.Count; i++)
    //    {
    //        ArrayList PartData = ErrorDatas[i] as ArrayList;
    //        if (PartData != null)
    //        {
    //            ArrayList ChkPartData = PartData[2] as ArrayList;
    //            List<CutData> CutData = ChkPartData[0] as List<CutData>;
    //            List<ShopBoltData> BoltData = ChkPartData[1] as List<ShopBoltData>;
    //            List<WeldData> WeldData = ChkPartData[2] as List<WeldData>;
    //            if (CutData.Count >= 1)
    //                CurErrors.AddRange(CutData);
    //            if (BoltData.Count >= 1)
    //                BltErrors.AddRange(BoltData);
    //            if (WeldData.Count >= 1)
    //                WldErrors.AddRange(WeldData);
    //        }
    //    }

    //    if (CurErrors.Count == 0 && BltErrors.Count == 0 && WldErrors.Count == 0)
    //        return null;
    //    else
    //    {
    //        if (CurErrors.Count >= 1)
    //            MyReturnData.Add(CurErrors);
    //        else
    //            MyReturnData.Add(null);

    //        if (BltErrors.Count >= 1)
    //            MyReturnData.Add(BltErrors);
    //        else
    //            MyReturnData.Add(null);

    //        if (WldErrors.Count >= 1)
    //            MyReturnData.Add(WldErrors);
    //        else
    //            MyReturnData.Add(null);
    //    }




    //    return MyReturnData;
    //}
    //public static ArrayList SKCompareData(ArrayList cNewdata, ArrayList cOlddata, bool debug = false, bool errflag = false)
    //{

    //    ArrayList MyReturnData = new ArrayList();

    //    //ArrayList cNewdata = (ArrayList)Newdata.Clone();
    //    //ArrayList cOlddata = (ArrayList)Olddata.Clone();

    //    ArrayList NewDatas = (ArrayList)cNewdata.Clone();
    //    ArrayList OldDatas = (ArrayList)cOlddata.Clone();


    //    string solrmk = string.Empty;
    //    string bltrmk = string.Empty;
    //    string wldrmk = string.Empty;
    //    string Olderr = string.Empty;
    //    string Newerr = string.Empty;
    //    string mo1proftype = string.Empty;
    //    string mo2proftype = string.Empty;
    //    //OldMainPart.GetReportProperty("PROFILE_TYPE", ref mo1proftype);
    //    //NewMainPart.GetReportProperty("PROFILE_TYPE", ref mo2proftype);
    //    TSG.Point OldCOG = new TSG.Point();
    //    //AssemblyObject1.GetReportProperty("COG_X", ref OldCOG.X);
    //    //AssemblyObject1.GetReportProperty("COG_Y", ref OldCOG.Y);
    //    //AssemblyObject1.GetReportProperty("COG_Z", ref OldCOG.Z);

    //    TSG.Point NewCOG = new TSG.Point();
    //    //AssemblyObject2.GetReportProperty("COG_X", ref NewCOG.X);
    //    //AssemblyObject2.GetReportProperty("COG_Y", ref NewCOG.Y);
    //    //AssemblyObject2.GetReportProperty("COG_Z", ref NewCOG.Z);

    //    if (mo1proftype != mo2proftype)
    //    {
    //        MyReturnData.Add("ProfileType : " + mo1proftype + "!=" + mo2proftype);
    //        return MyReturnData;
    //    }

    //    string proftmp = string.Empty;
    //    //OldMainPart.GetReportProperty("PROFILE", ref proftmp);

    //    //ArrayList OldDatas = GetAssemblyData(AssemblyObject1, debug);
    //    //ArrayList NewDatas = GetAssemblyData(AssemblyObject2, debug);

    //    //TSM.ModelObject OldMainPart = AssemblyObject1.GetMainPart();
    //    //TSM.ModelObject NewMainPart = AssemblyObject2.GetMainPart();
    //    if (OldDatas != null && NewDatas != null)
    //    {
    //        TransformationPlane M1TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //        //NewDatas = GetAssemblyData(AssemblyObject2, debug);
    //        TransformationPlane M2TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();

    //        if (OldDatas.Count == 0 || NewDatas.Count == 0)
    //        {
    //            MyReturnData.Add("E1");
    //            return MyReturnData;
    //        }




    //        double Tolerance = (1 / 64.0) * 25.4;
    //        //AssemblyCOGDifference = Tekla.Structures.Geometry3d.Distance.PointToPoint(OldCOG, NewCOG);
    //        ArrayList MyErrorData = new ArrayList();
    //        //Compare Model1 & Model2 Assembly remove no error
    //        for (int i = 5; i < OldDatas.Count; i++)
    //        {
    //            var Oldcog = OldDatas[1];
    //            ArrayList ChkSecPart1 = OldDatas[i] as ArrayList;
    //            if (ChkSecPart1 != null)
    //            {
    //                ArrayList ChkSecPart1Data = ChkSecPart1[2] as ArrayList;
    //                List<CutData> ChkCut1 = ChkSecPart1Data[0] as List<CutData>;
    //                List<ShopBoltData> ChkBolt1 = ChkSecPart1Data[1] as List<ShopBoltData>;
    //                List<WeldData> ChkWeld1 = ChkSecPart1Data[2] as List<WeldData>;
    //                for (int j = 5; j < NewDatas.Count; j++)
    //                {
    //                    ArrayList ChkSecPart2 = NewDatas[j] as ArrayList;
    //                    //var Newcog = NewDatas[1];
    //                    //AssemblyCOGDifference = ((double)Oldcog - (double)Newcog) / 2.0; ;
    //                    if (ChkSecPart2 != null)
    //                    {
    //                        ArrayList ChkSecPart2Data = ChkSecPart2[2] as ArrayList;
    //                        List<CutData> ChkCut2 = ChkSecPart2Data[0] as List<CutData>;
    //                        List<ShopBoltData> ChkBolt2 = ChkSecPart2Data[1] as List<ShopBoltData>;
    //                        List<WeldData> ChkWeld2 = ChkSecPart2Data[2] as List<WeldData>;
    //                        if (ChkCut1 != null && ChkCut2 != null)
    //                            solrmk = CompareCutData(ChkCut1, ChkCut2, Tolerance);
    //                        if (ChkBolt1 != null && ChkBolt2 != null)
    //                            bltrmk = CompareBoltData(ChkBolt1, ChkBolt2, Tolerance);
    //                        if (ChkWeld1 != null && ChkWeld2 != null)
    //                            wldrmk = CompareWeldData(ChkWeld1, ChkWeld2, Tolerance);

    //                        if (solrmk == "Cut Ok" && bltrmk == "Bolt Ok" && wldrmk == "Weld Ok")
    //                        {
    //                            OldDatas.RemoveAt(i);
    //                            NewDatas.RemoveAt(j);
    //                            i--;
    //                            j--;
    //                            break;
    //                        }
    //                    }

    //                }
    //            }




    //        }

    //        if (OldDatas.Count > 5 || NewDatas.Count > 5)
    //        {
    //            MyReturnData.Add(M1TP);
    //            MyReturnData.Add(OldDatas);
    //            MyReturnData.Add(M2TP);
    //            MyReturnData.Add(NewDatas);
    //            return MyReturnData;
    //        }
    //        else
    //            return null;


    //    }
    //    else
    //    {
    //        MyReturnData.Add("Both / Either one MainPart is Null.");
    //        return MyReturnData;
    //    }

    //}

    //public static ArrayList SKCompareAssembly(ArrayList Assy1Datas, ArrayList Assy2Datas, bool debug = false, bool errflag = false)
    //{

    //    ArrayList MyReturnData = new ArrayList();
    //    string solrmk = string.Empty;
    //    string bltrmk = string.Empty;
    //    string wldrmk = string.Empty;
    //    string assy1err = string.Empty;
    //    string assy2err = string.Empty;
    //    string mo1proftype = string.Empty;
    //    string mo2proftype = string.Empty;
    //    //Assy1MainPart.GetReportProperty("PROFILE_TYPE", ref mo1proftype);
    //    //Assy2MainPart.GetReportProperty("PROFILE_TYPE", ref mo2proftype);
    //    TSG.Point Assy1COG = new TSG.Point();
    //    //AssemblyObject1.GetReportProperty("COG_X", ref Assy1COG.X);
    //    //AssemblyObject1.GetReportProperty("COG_Y", ref Assy1COG.Y);
    //    //AssemblyObject1.GetReportProperty("COG_Z", ref Assy1COG.Z);

    //    TSG.Point Assy2COG = new TSG.Point();
    //    //AssemblyObject2.GetReportProperty("COG_X", ref Assy2COG.X);
    //    //AssemblyObject2.GetReportProperty("COG_Y", ref Assy2COG.Y);
    //    //AssemblyObject2.GetReportProperty("COG_Z", ref Assy2COG.Z);

    //    if (mo1proftype != mo2proftype)
    //    {
    //        MyReturnData.Add("ProfileType : " + mo1proftype + "!=" + mo2proftype);
    //        return MyReturnData;
    //    }

    //    string proftmp = string.Empty;
    //    //Assy1MainPart.GetReportProperty("PROFILE", ref proftmp);

    //    //ArrayList Assy1Datas = GetAssemblyData(AssemblyObject1, debug);
    //    //ArrayList Assy2Datas = GetAssemblyData(AssemblyObject2, debug);

    //    //TSM.ModelObject Assy1MainPart = AssemblyObject1.GetMainPart();
    //    //TSM.ModelObject Assy2MainPart = AssemblyObject2.GetMainPart();
    //    if (Assy1Datas != null && Assy2Datas != null)
    //    {
    //        TransformationPlane M1TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //        //Assy2Datas = GetAssemblyData(AssemblyObject2, debug);
    //        TransformationPlane M2TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();

    //        if (Assy1Datas.Count == 0 || Assy2Datas.Count == 0)
    //        {
    //            MyReturnData.Add("E1");
    //            return MyReturnData;
    //        }


    //        double Tolerance = (1 / 64.0) * 25.4;
    //        //AssemblyCOGDifference = Tekla.Structures.Geometry3d.Distance.PointToPoint(Assy1COG, Assy2COG);
    //        ArrayList MyErrorData = new ArrayList();
    //        //Compare Model1 & Model2 Assembly remove no error
    //        for (int i = 5; i < Assy1Datas.Count; i++)
    //        {
    //            var assy1cog = Assy1Datas[1];
    //            ArrayList ChkSecPart1 = Assy1Datas[i] as ArrayList;
    //            if (ChkSecPart1 != null)
    //            {
    //                ArrayList ChkSecPart1Data = ChkSecPart1[2] as ArrayList;
    //                List<CutData> ChkCut1 = ChkSecPart1Data[0] as List<CutData>;
    //                List<ShopBoltData> ChkBolt1 = ChkSecPart1Data[1] as List<ShopBoltData>;
    //                List<WeldData> ChkWeld1 = ChkSecPart1Data[2] as List<WeldData>;
    //                for (int j = 5; j < Assy2Datas.Count; j++)
    //                {
    //                    ArrayList ChkSecPart2 = Assy2Datas[j] as ArrayList;
    //                    //var assy2cog = Assy2Datas[1];
    //                    //AssemblyCOGDifference = ((double)assy1cog - (double)assy2cog) / 2.0; ;
    //                    if (ChkSecPart2 != null)
    //                    {
    //                        ArrayList ChkSecPart2Data = ChkSecPart2[2] as ArrayList;
    //                        List<CutData> ChkCut2 = ChkSecPart2Data[0] as List<CutData>;
    //                        List<ShopBoltData> ChkBolt2 = ChkSecPart2Data[1] as List<ShopBoltData>;
    //                        List<WeldData> ChkWeld2 = ChkSecPart2Data[2] as List<WeldData>;
    //                        if (ChkCut1 != null && ChkCut2 != null)
    //                            solrmk = CompareCutData(ChkCut1, ChkCut2, Tolerance);
    //                        if (ChkBolt1 != null && ChkBolt2 != null)
    //                            bltrmk = CompareBoltData(ChkBolt1, ChkBolt2, Tolerance);
    //                        if (ChkWeld1 != null && ChkWeld2 != null)
    //                            wldrmk = CompareWeldData(ChkWeld1, ChkWeld2, Tolerance);

    //                        if (solrmk == "Cut Ok" && bltrmk == "Bolt Ok" && wldrmk == "Weld Ok")
    //                        {
    //                            Assy1Datas.RemoveAt(i);
    //                            Assy2Datas.RemoveAt(j);
    //                            i--;
    //                            j--;
    //                            break;
    //                        }
    //                    }

    //                }
    //            }




    //        }

    //        MyReturnData.Add(M1TP);
    //        MyReturnData.Add(Assy1Datas);
    //        MyReturnData.Add(M2TP);
    //        MyReturnData.Add(Assy2Datas);
    //        return MyReturnData;
    //    }
    //    else
    //    {
    //        MyReturnData.Add("Both / Either one MainPart is Null.");
    //        return MyReturnData;
    //    }

    //}
    //public static ArrayList SKCompareAssembly(TSM.Assembly AssemblyObject1, TSM.Assembly AssemblyObject2, bool debug = false, bool errflag = false)
    //{

    //    ArrayList MyReturnData = new ArrayList();
    //    string solrmk = string.Empty;
    //    string bltrmk = string.Empty;
    //    string wldrmk = string.Empty;
    //    string assy1err = string.Empty;
    //    string assy2err = string.Empty;
    //    ArrayList Assy1Datas = new ArrayList();
    //    ArrayList Assy2Datas = new ArrayList();
    //    //ArrayList Assy1Datas = GetAssemblyData(AssemblyObject1, debug);
    //    //ArrayList Assy2Datas = GetAssemblyData(AssemblyObject2, debug);
    //    if (AssemblyObject1 != null && AssemblyObject2 != null)
    //    {
    //        TSM.ModelObject Assy1MainPart = AssemblyObject1.GetMainPart();
    //        TSM.ModelObject Assy2MainPart = AssemblyObject2.GetMainPart();
    //        if (Assy1MainPart != null && Assy2MainPart != null)
    //        {
    //            string mo1proftype = string.Empty;
    //            string mo2proftype = string.Empty;
    //            Assy1MainPart.GetReportProperty("PROFILE_TYPE", ref mo1proftype);
    //            Assy2MainPart.GetReportProperty("PROFILE_TYPE", ref mo2proftype);

    //            TSG.Point Assy1COG = new TSG.Point();
    //            AssemblyObject1.GetReportProperty("COG_X", ref Assy1COG.X);
    //            AssemblyObject1.GetReportProperty("COG_Y", ref Assy1COG.Y);
    //            AssemblyObject1.GetReportProperty("COG_Z", ref Assy1COG.Z);

    //            TSG.Point Assy2COG = new TSG.Point();
    //            AssemblyObject2.GetReportProperty("COG_X", ref Assy2COG.X);
    //            AssemblyObject2.GetReportProperty("COG_Y", ref Assy2COG.Y);
    //            AssemblyObject2.GetReportProperty("COG_Z", ref Assy2COG.Z);

    //            if (mo1proftype != mo2proftype)
    //            {
    //                MyReturnData.Add("ProfileType : " + mo1proftype + "!=" + mo2proftype);
    //                return MyReturnData;
    //            }

    //            string proftmp = string.Empty;
    //            Assy1MainPart.GetReportProperty("PROFILE", ref proftmp);
    //            //string chk = GetqMetricProfile(proftmp);

    //            Assy1Datas = GetAssemblyData(AssemblyObject1, debug);
    //            TransformationPlane M1TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //            Assy2Datas = GetAssemblyData(AssemblyObject2, debug);
    //            TransformationPlane M2TP = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
    //            if (Assy1Datas.Count == 0 || Assy2Datas.Count == 0)
    //            {
    //                MyReturnData.Add("E1");
    //                return MyReturnData;
    //            }


    //            double Tolerance = (1 / 64.0) * 25.4;
    //            //AssemblyCOGDifference = Tekla.Structures.Geometry3d.Distance.PointToPoint(Assy1COG, Assy2COG);
    //            ArrayList MyErrorData = new ArrayList();
    //            //Compare Model1 & Model2 Assembly remove no error
    //            for (int i = 2; i < Assy1Datas.Count; i++)
    //            {
    //                var assy1cog = Assy1Datas[1];
    //                ArrayList ChkSecPart1 = Assy1Datas[i] as ArrayList;
    //                if (ChkSecPart1 != null)
    //                {
    //                    ArrayList ChkSecPart1Data = ChkSecPart1[2] as ArrayList;
    //                    List<CutData> ChkCut1 = ChkSecPart1Data[0] as List<CutData>;
    //                    List<ShopBoltData> ChkBolt1 = ChkSecPart1Data[1] as List<ShopBoltData>;
    //                    List<WeldData> ChkWeld1 = ChkSecPart1Data[2] as List<WeldData>;
    //                    for (int j = 2; j < Assy2Datas.Count; j++)
    //                    {
    //                        ArrayList ChkSecPart2 = Assy2Datas[j] as ArrayList;
    //                        //var assy2cog = Assy2Datas[1];
    //                        //AssemblyCOGDifference = ((double)assy1cog - (double)assy2cog) / 2.0; ;
    //                        if (ChkSecPart2 != null)
    //                        {
    //                            ArrayList ChkSecPart2Data = ChkSecPart2[2] as ArrayList;
    //                            List<CutData> ChkCut2 = ChkSecPart2Data[0] as List<CutData>;
    //                            List<ShopBoltData> ChkBolt2 = ChkSecPart2Data[1] as List<ShopBoltData>;
    //                            List<WeldData> ChkWeld2 = ChkSecPart2Data[2] as List<WeldData>;
    //                            if (ChkCut1 != null && ChkCut2 != null)
    //                                solrmk = CompareCutData(ChkCut1, ChkCut2, Tolerance);
    //                            if (ChkBolt1 != null && ChkBolt2 != null)
    //                                bltrmk = CompareBoltData(ChkBolt1, ChkBolt2, Tolerance);
    //                            if (ChkWeld1 != null && ChkWeld2 != null)
    //                                wldrmk = CompareWeldData(ChkWeld1, ChkWeld2, Tolerance);

    //                            if (solrmk == "Cut Ok" && bltrmk == "Bolt Ok" && wldrmk == "Weld Ok")
    //                            {
    //                                Assy1Datas.RemoveAt(i);
    //                                Assy2Datas.RemoveAt(j);
    //                                i--;
    //                                j--;
    //                                break;
    //                            }
    //                        }

    //                    }
    //                }




    //            }

    //            MyReturnData.Add(M1TP);
    //            MyReturnData.Add(Assy1Datas);
    //            MyReturnData.Add(M2TP);
    //            MyReturnData.Add(Assy2Datas);
    //            return MyReturnData;
    //        }
    //        else
    //        {
    //            MyReturnData.Add("Both / Either one MainPart is Null.");
    //            return MyReturnData;
    //        }
    //    }
    //    else
    //    {
    //        MyReturnData.Add("Both / Either one Assembly is Null.");
    //        return MyReturnData;
    //    }
    //}

    public static ArrayList GetPartData(TSM.Part MyPart, int ModelTrackID, bool debug)
    {
        ArrayList MyReturnData = new ArrayList();
        if (MyPart == null)
            MyReturnData = null;


        CoordinateSystem MOCS = MyPart.GetCoordinateSystem();
        TSM.Solid M1Sol = MyPart.GetSolid(Solid.SolidCreationTypeEnum.HIGH_ACCURACY);
        TSG.Point MinPt = M1Sol.MinimumPoint;
        TSG.Point MaxPt = M1Sol.MaximumPoint;
        TSG.Point MidPt = skTSLib.GetMidPoint(MinPt, MaxPt);
        MOCS.Origin = MidPt;

        //Set the Plane for AssyMainPart
        
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(MOCS));
        TSMUI.ViewHandler.RedrawWorkplane();
        MyPart.Select();

        TransformationPlane SavePlane = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();


        //Save Guid
        MyReturnData.Add(MyPart.Identifier.GUID.ToString());

        //Save ASSEMBLY_POS
        string AssyPos = string.Empty;
        MyPart.GetReportProperty("PART_POS", ref AssyPos);
        MyReturnData.Add(AssyPos);

        //Save Profile Type
        string proftype = string.Empty;
        MyPart.GetReportProperty("PROFILE_TYPE", ref proftype);
        MyReturnData.Add(proftype);

        //Save Plane
        MyReturnData.Add(SavePlane);
        //MyReturnData.Add(new TransformationPlane(MOCS));

        //Save Length
        double assylength = 0.0;
        MyPart.GetReportProperty("LENGTH", ref assylength);
        MyReturnData.Add(assylength);

        //Save MainPartData
        ArrayList PartData = GetModelObjectData(MyPart, ModelTrackID, debug);
        MyReturnData.Add(PartData);




        return MyReturnData;
    }
    public static ArrayList GetAssemblyData(TSM.Assembly MyAssembly, int ModelTrackID, bool debug)
    {
        ArrayList MyReturnData = new ArrayList();
        if (MyAssembly == null)
            MyReturnData = null;

        TSM.ModelObject AssyMainPart = MyAssembly.GetMainPart();
        CoordinateSystem MOCS = AssyMainPart.GetCoordinateSystem();
        if ((AssyMainPart as TSM.Part) != null)
        {
            TSM.Solid M1Sol = ((TSM.Part)AssyMainPart).GetSolid(Solid.SolidCreationTypeEnum.HIGH_ACCURACY);
            TSG.Point MinPt = M1Sol.MinimumPoint;
            TSG.Point MaxPt = M1Sol.MaximumPoint;
            TSG.Point MidPt = skTSLib.GetMidPoint(MinPt, MaxPt);
            MOCS.Origin = MidPt;

        }
        //Set the Plane for AssyMainPart
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(MOCS));
        TSMUI.ViewHandler.RedrawWorkplane();
        AssyMainPart.Select();
        TransformationPlane SavePlane = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();

        //Save Guid
        MyReturnData.Add(AssyMainPart.Identifier.GUID.ToString());

        //Save ASSEMBLY_POS
        string AssyPos = string.Empty;
        AssyMainPart.GetReportProperty("ASSEMBLY_POS", ref AssyPos);
        MyReturnData.Add(AssyPos);

        //Save Profile Type
        string proftype = string.Empty;
        AssyMainPart.GetReportProperty("PROFILE_TYPE", ref proftype);
        MyReturnData.Add(proftype);

        //Save Plane
        MyReturnData.Add(SavePlane);

        //Save Length
        double assylength = 0.0;
        AssyMainPart.GetReportProperty("LENGTH", ref assylength);
        MyReturnData.Add(assylength);

        //Save MainPartData
        ArrayList PartData = GetModelObjectData(AssyMainPart, ModelTrackID, debug);
        MyReturnData.Add(PartData);

        ArrayList MySecondaries = MyAssembly.GetSecondaries();
        foreach (TSM.ModelObject ModelObject in MySecondaries)
        {
            //Save SecondaryPartData
            ArrayList MySecondary = GetModelObjectData(ModelObject, ModelTrackID, debug);
            MyReturnData.Add(MySecondary);

            //AssyCt = AssyCt + cutbltwldct;
        }


        return MyReturnData;
    }

    public static string CompareCutData(List<CutData> CutData1, List<CutData> CutData2, double Tolerance = 0.0)
    {
        //Compare Cut Data 
        for (int i = 0; i < CutData1.Count; i++)
        {
            CutData Chk1 = CutData1[i];
            Chk1.Tolerance = Tolerance;
            //skTSLib.drawer.DrawText(Chk1.Location, i+1.ToString(), new TSMUI.Color(0, 0, 1));

            for (int j = 0; j < CutData2.Count; j++)
            {
                CutData Chk2 = CutData2[j];
                Chk2.Tolerance = Tolerance;

                if (Chk1.Equals(Chk2) == true)
                {
                    CutData1.RemoveAt(i);
                    CutData2.RemoveAt(j);
                    i--;
                    j--;
                    break;
                }
            }
        }
        if (CutData1.Count == 0 && CutData2.Count == 0)
            return "Cut Ok";
        else
            return "Cut Wrong";
    }

    public static string CompareBoltData(List<ShopBoltData> BoltData1, List<ShopBoltData> BoltData2, double Tolerance = 0.0)
    {
        //Compare for Bolt Data 
        for (int i = 0; i < BoltData1.Count; i++)
        {
            ShopBoltData Chk1 = BoltData1[i];
            Chk1.Tolerance = Tolerance;
            for (int j = 0; j < BoltData2.Count; j++)
            {
                ShopBoltData Chk2 = BoltData2[j];

                Chk2.Tolerance = Tolerance;
                if (Chk1.Equals(Chk2) == true)
                {
                    BoltData1.RemoveAt(i);
                    BoltData2.RemoveAt(j);
                    i--;
                    j--;
                    break;
                }
            }
        }
        if (BoltData1.Count == 0 && BoltData2.Count == 0)
            return "Bolt Ok";
        else
            return "Bolt Wrong";
    }
    public static string CompareWeldData(List<WeldData> WeldData1, List<WeldData> WeldData2, double Tolerance = 0.0)
    {
        //Compare Weld Data
        for (int i = 0; i < WeldData1.Count; i++)
        {
            WeldData Chk1 = WeldData1[i];
            Chk1.Tolerance = Tolerance;
            for (int j = 0; j < WeldData2.Count; j++)
            {
                WeldData Chk2 = WeldData2[j];

                Chk2.Tolerance = Tolerance;
                if (Chk1.Equals(Chk2) == true)
                {
                    WeldData1.RemoveAt(i);
                    WeldData2.RemoveAt(j);
                    i--;
                    j--;
                    break;
                }
            }
        }
        if (WeldData1.Count == 0 && WeldData2.Count == 0)
            return "Weld Ok";
        else
            return "Weld Wrong";
    }

    public static ArrayList GetModelObjectData(TSM.ModelObject ModelObject, int ModelTrackID, bool debug, bool errflag = false)
    {
        ArrayList MyReturnData = new ArrayList();
        List<CutData> CutData = GetModelObjectCutData(ModelObject, ModelTrackID, debug);
        List<ShopBoltData> BoltData = GetModelObjectBoltData(ModelObject, ModelTrackID, debug);
        List<WeldData> WeldData = GetModelObjectWeldData(ModelObject, ModelTrackID, debug);

        ArrayList chld = new ArrayList();
        chld.Add(CutData);
        chld.Add(BoltData);
        chld.Add(WeldData);

        int cutbltwldct = 0;
        if (CutData != null)
            cutbltwldct = cutbltwldct + CutData.Count;
        if (BoltData != null)
            cutbltwldct = cutbltwldct + BoltData.Count;
        if (WeldData != null)
            cutbltwldct = cutbltwldct + WeldData.Count;
        if (cutbltwldct == 0)
            return null;

        MyReturnData.Add(ModelObject.Identifier.GUID.ToString());
        string PART_POS = string.Empty;
        ModelObject.GetReportProperty("PART_POS", ref PART_POS);
        MyReturnData.Add(cutbltwldct + "|" + ModelObject.GetType().Name + "|" + PART_POS);
        MyReturnData.Add(chld);
        return MyReturnData;
    }

    public static List<CutData> GetModelObjectCutData(TSM.ModelObject MyModelObject, int ModelTrackID, bool debug = false, bool errflag = false)
    {

        //is the object already exist
        string guid = "Cut" + ModelTrackID + MyModelObject.Identifier.GUID.ToString();
        if (IsExist(guid) == true)
            return null;

        ObjectGuids.Add(guid);

        List<CutData> MyReturnCutData = new List<CutData>();
        ArrayList ChkCutPoints = new ArrayList();
        TSM.Solid MySolid = null;
        TSM.Beam MyBeam = MyModelObject as TSM.Beam;
        if (MyBeam != null)
        {
            MySolid = MyBeam.GetSolid();
            goto SkipGetSolid;
        }
        TSM.Part MyPart = MyModelObject as TSM.Part;
        if (MyPart != null)
        {
            MySolid = MyPart.GetSolid();
            goto SkipGetSolid;
        }

        TSM.PolyBeam MyPolyBeam = MyModelObject as TSM.PolyBeam;
        if (MyPolyBeam != null)
        {
            MySolid = MyPolyBeam.GetSolid();
            goto SkipGetSolid;
        }
        return MyReturnCutData;
    SkipGetSolid:;



        EdgeEnumerator MyEdgeEnumerator = MySolid.GetEdgeEnumerator();
        while (MyEdgeEnumerator.MoveNext())
        {
            Edge MyEdge = MyEdgeEnumerator.Current as Edge;
            if (MyEdge != null)
            {
                TSG.Point CheckPoint = MyEdge.StartPoint;
                //is CheckPoint already exist                 
                if (ChkCutPoints.Contains(CheckPoint) == false)
                {
                    ChkCutPoints.Add(CheckPoint);
                    CutData MyData = new CutData();
                    MyData.id = guid;// MyModelObject.Identifier.GUID.ToString();
                    MyData.Distance = Math.Round(TSG.Distance.PointToPoint(new TSG.Point(), CheckPoint), 2);
                    MyData.Location = CheckPoint;
                    MyReturnCutData.Add(MyData);
                    if (debug == true)
                        skTSLib.drawer.DrawText(CheckPoint, MyData.Distance.ToString("#0.0#"), new TSMUI.Color(0, 0, 1));
                }
                //is CheckPoint already exist 
                CheckPoint = MyEdge.EndPoint;
                if (ChkCutPoints.Contains(CheckPoint) == false)
                {
                    ChkCutPoints.Add(CheckPoint);
                    CutData MyData = new CutData();
                    MyData.id = guid;// MyModelObject.Identifier.GUID.ToString();
                    MyData.Distance = Math.Round(TSG.Distance.PointToPoint(new TSG.Point(), CheckPoint), 2);
                    MyData.Location = CheckPoint;
                    MyReturnCutData.Add(MyData);
                    if (debug == true)
                        skTSLib.drawer.DrawText(CheckPoint, MyData.Distance.ToString("#0.0#"), new TSMUI.Color(0, 0, 1));
                }
            }
        }
        return MyReturnCutData;
    }
    public static List<ShopBoltData> GetModelObjectBoltData(TSM.ModelObject MyModelObject, int ModelTrackID, bool debug = false)
    {
        List<ShopBoltData> MyShopBoltData = new List<ShopBoltData>();

        TSM.Part MyPart = MyModelObject as TSM.Part;
        if (MyPart != null)
        {
            ModelObjectEnumerator MyBolts = MyPart.GetBolts();
            while (MyBolts.MoveNext())
            {
                TSM.BoltGroup MyBoltGroup = MyBolts.Current as TSM.BoltGroup;
                //if (IgnoreSiteBolt == false)
                //{
                //    //BoltTypeEnum MyBoltTypeEnum = MyBoltGroup.BoltType;
                //    if (BoltTypeEnum.BOLT_TYPE_SITE == MyBoltGroup.BoltType
                //}                    
                if (MyBoltGroup != null)
                {
                    //is the object already exist
                    string guid = "Bolt" + ModelTrackID + MyBoltGroup.Identifier.GUID.ToString();
                    if (IsExist(guid) == true)
                        break;

                    ObjectGuids.Add(guid);
                    ArrayList MyBoltPositions = MyBoltGroup.BoltPositions;
                    foreach (TSG.Point BoltPosition in MyBoltPositions)
                    {
                        ShopBoltData MyData = new ShopBoltData();
                        MyData.id = guid;
                        MyData.HoleSize = MyBoltGroup.BoltSize + MyBoltGroup.Tolerance;
                        MyData.Location = BoltPosition;
                        MyShopBoltData.Add(MyData);
                    }

                }
            }
        }
        return MyShopBoltData;
    }
    public static List<WeldData> GetModelObjectWeldData(TSM.ModelObject MyModelObject, int ModelTrackID, bool debug = false)
    {


        List<WeldData> MyReturnWeldData = new List<WeldData>();
        ArrayList ChkWeldPoints = new ArrayList();
        TSM.Solid MySolid = null;

        TSM.Part MyPart = MyModelObject as TSM.Part;
        if (MyPart != null)
        {
            ModelObjectEnumerator MyWelds = MyPart.GetWelds();
            int wldct = MyWelds.GetSize();
            while (MyWelds.MoveNext())
            {
                TSM.BaseWeld MyBaseWelds = MyWelds.Current as TSM.BaseWeld;
                if (MyBaseWelds != null)
                {
                    //is the object already exist
                    string guid = "Weld" + ModelTrackID +  MyBaseWelds.Identifier.GUID.ToString();
                    if (IsExist(guid) == true)
                        break;

                    ObjectGuids.Add(guid);
                    MySolid = MyBaseWelds.GetSolid();
                    //ArrayList MyWeldGeometries = MyBaseWelds.GetWeldGeometries();

                    if (ChkWeldPoints.Contains(MySolid.MinimumPoint) == false)
                    {
                        ChkWeldPoints.Add(MySolid.MinimumPoint);
                        WeldData MyData = new WeldData();
                        MyData.id = guid;
                        if (MyBaseWelds.ShopWeld == false)
                            MyData.WeldType = 0;
                        else
                            MyData.WeldType = 1;
                        int wldtype = MyBaseWelds.TypeAbove.GetHashCode() + MyBaseWelds.TypeBelow.GetHashCode() + MyBaseWelds.Preparation.GetHashCode() + MyBaseWelds.ProcessType.GetHashCode();
                        MyData.WeldSize = wldtype + MyBaseWelds.ElectrodeCoefficient + MyBaseWelds.ElectrodeStrength + MyBaseWelds.EffectiveThroatAbove + MyBaseWelds.EffectiveThroatBelow + MyBaseWelds.AngleAbove + MyBaseWelds.AngleBelow + MyBaseWelds.AdditionalSizeAbove + +MyBaseWelds.AdditionalSizeBelow + MyBaseWelds.RootFaceAbove + MyBaseWelds.RootFaceBelow + MyBaseWelds.RootFaceAbove + MyBaseWelds.RootFaceBelow + MyBaseWelds.SizeAbove + MyBaseWelds.SizeBelow + MyBaseWelds.LengthAbove + MyBaseWelds.LengthBelow + MyBaseWelds.PitchAbove + MyBaseWelds.PitchBelow;
                        MyData.Location = MySolid.MinimumPoint;

                        MyReturnWeldData.Add(MyData);
                        if (debug == true)
                            skTSLib.drawer.DrawText(MySolid.MinimumPoint, "WMin", new TSMUI.Color(0, 0, 1));
                    }

                    if (ChkWeldPoints.Contains(MySolid.MaximumPoint) == false)
                    {
                        ChkWeldPoints.Add(MySolid.MaximumPoint);
                        WeldData MyData = new WeldData();
                        MyData.id = guid;

                        if (MyBaseWelds.ShopWeld == false)
                            MyData.WeldType = 0;
                        else
                            MyData.WeldType = 1;

                        int wldtype = MyBaseWelds.TypeAbove.GetHashCode() + MyBaseWelds.TypeBelow.GetHashCode() + MyBaseWelds.Preparation.GetHashCode() + MyBaseWelds.ProcessType.GetHashCode();
                        MyData.WeldSize = wldtype + MyBaseWelds.ElectrodeCoefficient + MyBaseWelds.ElectrodeStrength + MyBaseWelds.EffectiveThroatAbove + MyBaseWelds.EffectiveThroatBelow + MyBaseWelds.AngleAbove + MyBaseWelds.AngleBelow + MyBaseWelds.AdditionalSizeAbove + +MyBaseWelds.AdditionalSizeBelow + MyBaseWelds.RootFaceAbove + MyBaseWelds.RootFaceBelow + MyBaseWelds.RootFaceAbove + MyBaseWelds.RootFaceBelow + MyBaseWelds.SizeAbove + MyBaseWelds.SizeBelow + MyBaseWelds.LengthAbove + MyBaseWelds.LengthBelow + MyBaseWelds.PitchAbove + MyBaseWelds.PitchBelow;
                        MyData.Location = MySolid.MaximumPoint;

                        MyReturnWeldData.Add(MyData);
                        if (debug == true)
                            skTSLib.drawer.DrawText(MySolid.MaximumPoint, "WMax", new TSMUI.Color(0, 0, 1));
                    }


                }
            }
        }
        return MyReturnWeldData;
    }
    public static string DisplayError(ArrayList ErrorDatas, TransformationPlane MyTransformationPlane, string Message, bool debug = false, bool errflag = false)
    {

        string MyReturnData = string.Empty;

        string cutrmk = string.Empty;
        string bltrmk = string.Empty;
        string wldrmk = string.Empty;

        //error count    
        int cuterrct = 0;
        int blterrct = 0;
        int wlderrct = 0;

        //Set the Plane of Model Object TransformationPlane
        MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(MyTransformationPlane);
        TSMUI.ViewHandler.RedrawWorkplane();

        if (ErrorDatas != null)
        {
    
            List<CutData> ChkCut = ErrorDatas[0] as List<CutData>;
            List<ShopBoltData> ChkBolt = ErrorDatas[1] as List<ShopBoltData>;
            List<WeldData> ChkWeld = ErrorDatas[2] as List<WeldData>; ;

            if (ChkCut != null)
            {
                cuterrct = cuterrct + DisplayError(ChkCut, "X " + Message, debug, errflag);
            }
            if (ChkBolt != null)
            {
                blterrct = blterrct + DisplayError(ChkBolt, "B " + Message, debug, errflag);
            }
            if (ChkWeld != null)
            {
                wlderrct = wlderrct + DisplayError(ChkWeld, "W " + Message, debug, errflag);
            }

            
            
            if (cuterrct >= 1)
                cutrmk = cuterrct + " Part. ";
            if (blterrct >= 1)
                bltrmk = blterrct + " Bolt. ";
            if (wlderrct >= 1)
                wldrmk = wlderrct + " Weld. ";
            MyReturnData = cutrmk + bltrmk + wldrmk;

        }
        return MyReturnData;
    }
    //public static string DisplayError(ArrayList AssemblyErrorDatas, TransformationPlane AssemblyObjectTP, string Message, bool debug = false, bool errflag = false)
    //{

    //    string MyReturnData = string.Empty;

    //    string cutrmk = string.Empty;
    //    string bltrmk = string.Empty;
    //    string wldrmk = string.Empty;

    //    //error count    
    //    int cuterrct = 0;
    //    int blterrct = 0;
    //    int wlderrct = 0;

    //    //Set the Plane of Model Object TransformationPlane
    //    MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(AssemblyObjectTP);
    //    TSMUI.ViewHandler.RedrawWorkplane();

    //    for (int j = 5; j < AssemblyErrorDatas.Count; j++)
    //    {
    //        ArrayList ChkSecPart = AssemblyErrorDatas[j] as ArrayList;
    //        if (ChkSecPart != null)
    //        {
    //            ArrayList ChkSecPartData = ChkSecPart[2] as ArrayList;
    //            List<CutData> ChkCut = ChkSecPartData[0] as List<CutData>;
    //            List<ShopBoltData> ChkBolt = ChkSecPartData[1] as List<ShopBoltData>;
    //            List<WeldData> ChkWeld = ChkSecPartData[2] as List<WeldData>; ;

    //            cuterrct = cuterrct + DisplayError(ChkCut, "Part " + Message, debug, errflag);
    //            blterrct = blterrct + DisplayError(ChkBolt, "Bolt " + Message, debug, errflag);
    //            wlderrct = wlderrct + DisplayError(ChkWeld, "Weld " + Message, debug, errflag);
    //            //if (ChkCut.Count >= 1)
    //            //    cuterrct = cuterrct + 1;
    //            //if (ChkBolt.Count >= 1)
    //            //    blterrct = blterrct + 1;
    //            //if (ChkWeld.Count >= 1)
    //            //    wlderrct = wlderrct + 1;
    //            //cuta1errct = blta1errct + ChkCut2.Count;
    //            //blta1errct = blta1errct + ChkBolt2.Count;
    //            //wlda1errct = wlda1errct + ChkWeld2.Count;
    //        }


    //    }
    //    if (AssemblyErrorDatas.Count >= 1)
    //    {
    //        if (cuterrct >= 1)
    //            cutrmk = cuterrct + " Part. ";
    //        if (blterrct >= 1)
    //            bltrmk = blterrct + " Bolt. ";
    //        if (wlderrct >= 1)
    //            wldrmk = wlderrct + " Weld. ";
    //        MyReturnData = cutrmk + bltrmk + wldrmk;
    //    }
    //    return MyReturnData;
    //}

    public static int DisplayError(List<CutData> CutDatas, string DisplayMessage, bool debugflag, bool errflag)
    {
        ArrayList UniqueID = new ArrayList();
        for (int i = 0; i < CutDatas.Count; i++)
        {
            CutData chk = CutDatas[i];
            string id = chk.id;
            if (UniqueID.Contains(id) == false)
                UniqueID.Add(id);
            DisplayError(chk.Location, DisplayMessage, debugflag, errflag);
        }
        return UniqueID.Count;
    }
    public static int DisplayError(List<ShopBoltData> ShopBoltDatas, string DisplayMessage, bool debugflag, bool errflag)
    {
        ArrayList UniqueID = new ArrayList();
        for (int i = 0; i < ShopBoltDatas.Count; i++)
        {
            ShopBoltData chk = ShopBoltDatas[i];
            string id = chk.id;
            if (UniqueID.Contains(id) == false)
                UniqueID.Add(id);
            DisplayError(chk.Location, DisplayMessage, debugflag, errflag);
        }
        return UniqueID.Count;
    }
    public static int DisplayError(List<WeldData> WeldDatas, string DisplayMessage, bool debugflag, bool errflag)
    {
        ArrayList UniqueID = new ArrayList();
        for (int i = 0; i < WeldDatas.Count; i++)
        {
            WeldData chk = WeldDatas[i];
            string id = chk.id;
            if (UniqueID.Contains(id) == false)
                UniqueID.Add(id);
            DisplayError(chk.Location, DisplayMessage, debugflag, errflag);
        }
        return UniqueID.Count;
    }
    public static void DisplayError(TSG.Point DisplayPoint, string DisplayMessage, bool debugflag, bool errflag)
    {
        if (debugflag == true || errflag == true)
            skTSLib.drawer.DrawText(DisplayPoint, DisplayMessage, new TSMUI.Color(1, 0, 0));

    }


    public static bool IsPointAreEqual(TSG.Point Point1, TSG.Point Point2, double Tolerance)
    {
        bool result = false;
        if (Point1 != null && Point2 != null)
        {
            result = Math.Abs(Point1.X - Point2.X) < Tolerance && Math.Abs(Point1.Y - Point2.Y) < Tolerance && Math.Abs(Point1.Z - Point2.Z) < Tolerance;
        }

        return result;
    }

    public static bool IsShopBoltDataAreEqual(ShopBoltData ShopBoltData1, ShopBoltData ShopBoltData2, double Tolerance)
    {
        if (ShopBoltData1 != null && ShopBoltData1 != null)
        {

            bool chk = ShopBoltData1.HoleSize == ShopBoltData1.HoleSize;
            if (chk == false)
                return false;
            return Math.Abs(ShopBoltData1.Location.X - ShopBoltData2.Location.X) < Tolerance && Math.Abs(ShopBoltData1.Location.Y - ShopBoltData2.Location.Y) < Tolerance && Math.Abs(ShopBoltData1.Location.Z - ShopBoltData2.Location.Z) < Tolerance;

        }

        return false;
    }

    public static List<string> ObjectGuids = new List<string>();
    public static bool IsExist(string CheckID)
    {
        return ObjectGuids.Contains(CheckID);
    }
    public static double AssemblyCOGDifference = 0.0;


    public static string GetDrawingRevision(TSD.DrawingEnumerator MyDrawings)
    {
        //skWinLib.DebugLog(debugflag, debugcount++, logfile, "GetDrawingRevision");

        string drgrev = string.Empty;
        System.Reflection.PropertyInfo pi = MyDrawings.Current.GetType().GetProperty("Identifier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        object value = pi.GetValue(MyDrawings.Current, null);
        Identifier Identifier = (Identifier)value;

        TSM.Beam fakebeam = new TSM.Beam();
        fakebeam.Identifier = Identifier;

        bool rev = fakebeam.GetReportProperty("REVISION.MARK", ref drgrev);
        return drgrev;
    }
    public static string GetDPMPrinterCommand()
    {
        //string printerLocation = @"C:\TeklaStructures\2022.0\bin\applications\Tekla\Model\DPMPrinter\DPMPrinterCommand.exe";
        var dpmPath = @"applications\Tekla\Model\DPMPrinter\DPMPrinterCommand.exe";
        var binPath = string.Empty;
        TeklaStructuresSettings.GetAdvancedOption("XSBIN", ref binPath);

        return Path.Combine(binPath.Replace(@"\\", @"\"), dpmPath);
    }

    public static string GetPrinterArgs(string printerTemplate)
    {
        StringBuilder arg = new StringBuilder();
        arg.Append("printActive:true ");
        arg.Append("printer:pdf ");
        arg.AppendFormat(@"settingsFile:""{0}"" ", printerTemplate);

        return arg.ToString();
    }

    public static string PrintPdf(TSD.Drawing currentDrawing, bool includeRevisionMarkToFilename, string path, string printerTemplate)
    {
        var binPath = GetDPMPrinterCommand();
        var args = GetPrinterArgs(printerTemplate);

        // Add some views.
        TSD.DrawingHandler DrawingHandler = new TSD.DrawingHandler();

        DrawingHandler.SetActiveDrawing(currentDrawing, false);

        string filename = string.Format("{0}.pdf", currentDrawing.GetPlotFileName(includeRevisionMarkToFilename));
        string fullname = Path.Combine(path, filename);
        string arg = args + string.Format(@"out:""{0}""", fullname);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = binPath;
        startInfo.Arguments = arg;

        Process proc = new Process() { StartInfo = startInfo };
        proc.Start();
        proc.WaitForExit();

        DrawingHandler.CloseActiveDrawing();
        return fullname;
    }

    public static void HighLightDrawingObjects(ArrayList selectedObjectsArray)
    {
        DrawingHandler drawingHandler = new DrawingHandler();
        drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();
        var methodInfo = drawingHandler.GetDrawingObjectSelector().GetType().GetMethod("SelectObjectsWithHighLight",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        methodInfo.Invoke(drawingHandler.GetDrawingObjectSelector(), new object[] { selectedObjectsArray, true });
    }
    public static void HighLightDrawingObjects()
    {
        DrawingHandler drawingHandler = new DrawingHandler();
        var doe = drawingHandler.GetDrawingObjectSelector().GetSelected();
        ArrayList selectedObjectsArray = new ArrayList();

        while (doe.MoveNext())
        {
            selectedObjectsArray.Add(doe.Current);
        }
        drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();
        var methodInfo = drawingHandler.GetDrawingObjectSelector().GetType().GetMethod("SelectObjectsWithHighLight",
        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        methodInfo.Invoke(drawingHandler.GetDrawingObjectSelector(), new object[] { selectedObjectsArray, true });
    }

    public static void Display_CONN_CODE()
    {
        Tekla.Structures.Model.ModelObjectEnumerator MyMOEnum = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
        while (MyMOEnum.MoveNext())
        {
            
            string CONN_CODE_END1 = string.Empty;
            string CONN_CODE_END2 = string.Empty;
            TSG.Point StartPoint = new TSG.Point();
            TSG.Point EndPoint = new TSG.Point();
            Tekla.Structures.Model.Part MyPart = MyMOEnum.Current as Tekla.Structures.Model.Part;
            if (MyPart != null)
            {
                //get end code
                MyPart.GetReportProperty("CONN_CODE_END1", ref CONN_CODE_END1);
                MyPart.GetReportProperty("CONN_CODE_END2", ref CONN_CODE_END2);
                if (CONN_CODE_END1.Trim().Length  == 0 && CONN_CODE_END2.Trim().Length == 0)
                {

                }
                else
                {
                    Tekla.Structures.Model.Beam MyBeam = MyMOEnum.Current as Tekla.Structures.Model.Beam;
                    if (MyBeam != null)
                    {

                        //get start and end point
                        StartPoint = MyBeam.StartPoint;
                        EndPoint = MyBeam.EndPoint;
                    }
                    else
                    {
                        Tekla.Structures.Model.PolyBeam MyPolyBeam = MyMOEnum.Current as Tekla.Structures.Model.PolyBeam;
                        if (MyPolyBeam != null)
                        {
                            ArrayList MyContourPoints = MyPolyBeam.Contour.ContourPoints;

                            //get start and end point
                            StartPoint = MyContourPoints[0] as TSG.Point;
                            EndPoint = MyContourPoints[MyContourPoints.Count - 1] as TSG.Point;
                        }
                        else
                        {
                            Tekla.Structures.Model.ContourPlate MyContourPlate = MyMOEnum.Current as Tekla.Structures.Model.ContourPlate;
                            if (MyContourPlate != null)
                            {
                                ArrayList MyContourPoints = MyContourPlate.Contour.ContourPoints;

                                //get start and end point
                                StartPoint = MyContourPoints[0] as TSG.Point;
                                EndPoint = MyContourPoints[MyContourPoints.Count - 1] as TSG.Point;
                            }
                            else
                            {

                            }
                        }
                    }
                }

                
            }

            
            if (StartPoint != null && EndPoint != null)
            {
                //display end code
                new Tekla.Structures.Model.UI.GraphicsDrawer().DrawText(StartPoint, CONN_CODE_END1, new Tekla.Structures.Model.UI.Color(0, 0, 1));
                new Tekla.Structures.Model.UI.GraphicsDrawer().DrawText(EndPoint, CONN_CODE_END2, new Tekla.Structures.Model.UI.Color(0, 0, 1));

            }


        }

    }

    public static string GetNodeBeamEnd(TSM.Beam MyBeam, TSG.Point NodePoint)
    {
        string MyReturnData = string.Empty;
        if (MyBeam != null)
        {
            //TSG.Point StartPoint = new TSG.Point();
            //TSG.Point EndPoint = new TSG.Point();
            double CheckStartNodePointDist = Distance.PointToPoint(MyBeam.StartPoint, NodePoint);
            double CheckEndNodePointDist = Distance.PointToPoint(MyBeam.EndPoint, NodePoint);
            if (CheckStartNodePointDist < CheckEndNodePointDist)
                MyReturnData = "START";
            else
                MyReturnData = "END";
        }        
        return MyReturnData;
    }
    public static string GetBeamConnectionCode(TSM.Beam MyBeam, TSG.Point NodePoint, string ConnectionCode)
    {
        string MyReturnData = string.Empty;
        if (MyBeam != null)
        {
            if (GetNodeBeamEnd(MyBeam, NodePoint) == "START")
                MyBeam.GetReportProperty(ConnectionCode + "1", ref MyReturnData);
            else
                MyBeam.GetReportProperty(ConnectionCode + "2", ref MyReturnData);
        }        
        return MyReturnData;
    }

    public static double GetAngle_X360(TSG.Point StartPoint, TSG.Point EndPoint)
    {
        LineSegment LS1 = new LineSegment(StartPoint, EndPoint);
        Vector VC1 = LS1.GetDirectionVector();

        LineSegment LS2 = new LineSegment(StartPoint, new TSG.Point(EndPoint.X + StartPoint.Y + 36076840705, StartPoint.Y, StartPoint.Z));
        Vector VC2 = LS2.GetDirectionVector();

        double deg = (180 / Math.PI) * VC1.GetAngleBetween(VC2);
        //based on z(plan) vector change 
        double chkvecx = new Vector(1, 0, 0).Cross(VC1).X;
        double chkvecy = new Vector(1, 0, 0).Cross(VC1).Y;
        double chkvecz = new Vector(1, 0, 0).Cross(VC1).Z;

        //if (chkvecz < 0.0)
        //    deg = 360.0 - deg;
        if (chkvecz < 0.0)
            return 360.0 - deg;  //return (deg + 360) % 360;
        else
            return deg;
        //TSG.Point MidPt = skTSLib.GetMidPoint(StartPoint, EndPoint);
        //skTSLib.drawer.DrawText(MidPt, deg.ToString("#0.0#"), new TSMUI.Color(0, 0, 1));

    }
    public static double CalculateAngle(TSG.Point xyz1, TSG.Point xyz2, TSG.Point xyz3, string plane)
    {
        double angle = 0;
        double x1 = xyz1.X;
        double y1 = xyz1.Y;
        double z1 = xyz1.Z;

        double x2 = xyz2.X;
        double y2 = xyz2.Y;
        double z2 = xyz2.Z;

        double x3 = xyz3.X;
        double y3 = xyz3.Y;
        double z3 = xyz3.Z;

        switch (plane)
        {
            case "XY":
                angle = Math.Atan2((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1), (x2 - x1) * (x3 - x1) + (y2 - y1) * (y3 - y1));
                break;
            case "XZ":
                angle = Math.Atan2((z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1), (x2 - x1) * (x3 - x1) + (z2 - z1) * (z3 - z1));
                break;
            case "YZ":
                angle = Math.Atan2((z2 - z1) * (y3 - y1) - (z3 - z1) * (y2 - y1), (y2 - y1) * (y3 - y1) + (z2 - z1) * (z3 - z1));
                break;
        }
        angle = angle * (180.0 / Math.PI); // Convert to degrees
        return angle;
        //return (angle + 360) % 360; // Ensure 360-degree range
    }

    public static bool IsViewInsideLayout(TSD.Drawing MyDrawing, TSD.View MyView)
    {

        double sheet_width = MyDrawing.Layout.SheetSize.Width;
        double sheet_height = MyDrawing.Layout.SheetSize.Height;
        Tekla.Structures.Geometry3d.Point extrema_min_pt_gl = MyDrawing.GetSheet().Origin;
        Tekla.Structures.Geometry3d.Point extrema_max_pt_gl = new Tekla.Structures.Geometry3d.Point(extrema_min_pt_gl.X + sheet_width, extrema_min_pt_gl.Y + sheet_height);
        TSD.RectangleBoundingBox rectangleBoundingBox = MyView.GetAxisAlignedBoundingBox();

        TSG.Point view_min_pt = rectangleBoundingBox.MinPoint;
        if (view_min_pt.X >= extrema_min_pt_gl.X && view_min_pt.X <= extrema_max_pt_gl.X
        && view_min_pt.Y >= extrema_min_pt_gl.Y && view_min_pt.Y <= extrema_max_pt_gl.Y)
            return true;
        else
            return false;
    }



    
}
