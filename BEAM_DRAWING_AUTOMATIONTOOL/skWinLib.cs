//Omm Muruga 3
//Common Function's irrespective of any application, tool, plugin
//Ensure naming of function is meaningfull to understand
//Dont have too many short variables
//If any major changes required in functions please let Vijayakumar know's about this.
//Don't modifiy existing function/class/sub unless you are aware of the consequences {invalid data can cause unexpected behaviour and potential data loss}
//Use Commentline as much as possible for future reference and other's can understand easily
//Most function's or not tested so ensure its quality at your own risk.
//08Jan22 1502
//23Feb22 1859 SharePoint Two Nuget Package added
//MyEnum.SelectInstances = false; // Set the "SelectInstances" to false to speed up the enquiry; possible because only report properties are asked
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
//using System.Net.Http;


using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.VisualBasic;
using Microsoft.SharePoint.Client;
using System.Net.Mail;

using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using File = System.IO.File;
using System.Management;

//using static ABMLengthVSLiveLength.Form1;

//using OpenQA.Selenium;
//Imports System.DirectoryServices
//Imports System.Net.NetworkInformation

public class skWinLib
{

    //private static readonly HttpClient MyHttpClient = new HttpClient();
    private const string LogURL = "http://logs-01.loggly.com/inputs/a3c096b3-8fd8-47f8-ac1a-ff3d90dcfcf3/tag/http/";


    public static MySqlConnection myconn = new MySqlConnection(skWinLib.connString);
    public static MySqlCommand mycmd = new MySqlCommand();
    public static string mysql = "";
    public static MySqlDataReader myrdr = null;
    public static ArrayList projectdata = new ArrayList();
    public static ArrayList  empdata = new ArrayList();
    public static List<string> MyEmployeeData = new List<string>();
    public static List<string> MyProjectData = new List<string>();

    //Version 2309.05
    

    //version 22Dec21 1058
    public static string username = System.Environment.UserName;
    public static string systemname = System.Environment.MachineName;
    public static bool Is64BitOS = System.Environment.Is64BitOperatingSystem;
    public static string OSVersion = System.Environment.OSVersion.ToString();
    public static string DomainName = System.Environment.UserDomainName;
    public static int ProcessorCount = System.Environment.ProcessorCount;

    public static bool WFH = false;
    //these user will be ignored for accesslog, worklog since part of development team
    //Based on Office365 Migration Lap user login id changed

    public static string esskayignoreuser = "|SKS138|VishwaramP|SKS282|NaveenKumarS|SKS360|VijayakumarP|SKS413|SKS387|";
    //public static string esskayignoreuser = "|SKS413|SKS287|SKS282|SKS138|admin|NaveenKumarS|ShandoJoseLeen|VishwaramP|"; // ""; //
    //public static string esskayignoreuser = "|SKS413|SKS360|SKS138|admin|VijayakumarP|VishwaramP|SKS006C|";

    //public static string esskayignoreuser = "|SKS413|SKS360|SKS287|SKS282|SKS138|admin|VijayakumarP|NaveenKumarS|ShandoJoseLeen|VishwaramP|"; // ""; // 
    public static int esskayappvalidity = 2025;

    public static string TSVersion = "2024.0";
    public static string sklogfilepath = "D:\\teklasupport\\" + DateTime.Now.ToString("yyyy") + "\\log\\" + DateTime.Now.ToString("MMM") + "\\";
    public static string sklocalpath = "d:\\esskay\\";
    public static string skserverpath = "T:\\";
    public static string skdomainname = "esskaystructures.com";
    public static string Email_Automation = "vijayakumar@esskaystructures.com";
    public static string Email_DatabaseSupport = "vishwa@esskaystructures.com";
    public static string Email_IT = "jagadeesh@esskaystructures.com";

    public static string sklogin = getPayanarPeyar();
    public static string skpassword = getKadavuSoll();


    public static List<string> skVersion = new List<string>() { "2019.1", "2020.0", "2021.0", "2022.0", "2023.0", "2024.0" };
    public static List<string> SK_XSServicePack = new List<string>() { "2019i", "2020", "2021", "2022", "2023", "2024" };
    //const dbname = "T:\\";

   // public static string connString = string.Empty;
    public static string connString = string.Empty;

    

    //get domainname
    //public static string DomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
    //public static string DomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
    //System.DirectoryServices.ActiveDirectory.Domain
    //Domain domain = Domain.GetComputerDomain();
    //Console.WriteLine(domain.Name );
    //System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
    //registry 14Dec21

    public static string userRoot = "HKEY_CURRENT_USER\\SOFTWARE\\ESSKAY";
    public static string SKLastUsedApplication1 = "SKLastUsedApplication1";
    public static string SKLastUsedApplication2 = "SKLastUsedApplication2";

    //private static readonly string apiUrl = "https://log.esskaydesign.com/";

    public static string address { get; set; }
    public static string server { get; set; }
    public static string conpassword { get; set; }
    public static string condatbase { get; set; }

    

    public static string serverpath = "";
    //public class Application
    //{
    //public static CreateFileWithServerInfo(Tuple<string, string> licenseInfo)
    //{
    //    // http://stackoverflow.com/questions/1128655/c-query-flexlm-license-manager

    //    var currentFolder = Path.GetDirectoryName(System.Reflection.SinglePart.GetExecutingSinglePart().Location);
    //    var args = String.Format("lmstat -c {0} -a -i", licenseInfo.Item1);
    //    var info = new ProcessStartInfo(String.Concat(currentFolder, @"\lmutil.exe"), args)
    //    {
    //        WindowStyle = ProcessWindowStyle.Hidden,
    //        UseShellExecute = false,
    //        RedirectStandardOutput = true
    //    };

    //    using (var process = Process.Start(info))
    //    {
    //        if (process == null) return;
    //        string output = process.StandardOutput.ReadToEnd();

    //        // standard output must be read first; wait max 5 minutes
    //        if (process.WaitForExit(300000))
    //        {
    //            process.WaitForExit(); // per MSDN guidance: Process.WaitForExit Method 
    //        }
    //        else
    //        {
    //            // kill the lmstat instance and move on
    //            //log.Warn("lmstat did not exit within timeout period; killing");
    //            process.Kill();
    //            process.WaitForExit(); // Process.Kill() is asynchronous; wait for forced quit
    //        }
    //        File.WriteAllText(String.Concat(currentFolder, @"\", licenseInfo.Item1, ".lmout"), output);
    //    }
    //}

    public static bool IsApplicationExecutedFrom_CorD_Drive()
    {
        string myapppath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().ToUpper().Trim();
        if (myapppath.Length > 3)
        {
            if (myapppath.Substring(0, 2) == "C:" || myapppath.Substring(0, 2) == "D:")
                return true;
        }
        return false;
    }

    public static void ExecutedApplication_CorD_Drive_UserNotification()
    {
        string myapppath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().ToUpper().Trim();
        string targetFolderPath = @"D:\esskay\SupportFiles";
        Random random = new Random();

        string filePath = Path.Combine(targetFolderPath, DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MMM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + (random.Next() * 360) .ToString() +   "checkDriveexe.txt");
        if (!Directory.Exists(targetFolderPath))
        {
            Directory.CreateDirectory(targetFolderPath);
        }

        File.WriteAllText(filePath, "All tools to be executed/run from local drive (C: or D:).\n\n\nYour Application Path:" + myapppath + "\n\n\nClick Autoupate @T:\\SKTSAppAutoUpdateFold\\SKTSAppAutoUpdate.exe");
        
        // Open the Notepad file
        Process.Start("notepad.exe", filePath);
        Console.WriteLine("File created and opened: " + filePath);

    }


    public static bool IsModelPathValid(string XSPath)
    {
        string XSModelName = Path.GetFileName(XSPath.TrimEnd(Path.DirectorySeparatorChar));
        string XSModelPath = XSPath + @"\" + XSModelName + ".db1";
        if (System.IO.File.Exists(XSModelPath) == true)
            return true;
        else
            return false;
    }

    public static string GetConnectionString()
    {
        ArrayList convalre = new ArrayList();

        //string pwvalue = "";
        // string path1 = @"T:\SupportFiles\qcConnstring";
        string path1 = @"T:\SupportFiles\Connstring";
        //string path1 = @"T:\SupportFiles\Connstring - prod";
        //string path2 = path1 + "\\checkLdrive";
        string[] FilesPath1 = Directory.GetFiles(path1);

        string[] lines = File.ReadAllLines(FilesPath1[0]);
        //string pass = lines[1];

        foreach (string line in lines)
        {
            string lrive = "";
            string rmvline = line.Substring(3);

            for (int i = 0; i < rmvline.Length; i++)
            {
                if (i % 2 != 0)
                {
                    string c1 = rmvline[i].ToString();
                    //string check1 = pass.Replace(c1, "");
                    lrive = lrive + c1;
                }
            }

            convalre.Add(lrive);
        }

        address = convalre[0].ToString();
        server = convalre[1].ToString();
        conpassword = convalre[2].ToString();
        condatbase = convalre[3].ToString();

        //return convalre;
        return "server=(" + address + ",priority=100);user id = " + server + "; password=" + conpassword + ";database=" + condatbase;
    }

    //public static string GetConnectionStringOldQC()
    //{
    //    try
    //    {
    //        ArrayList convalre = new ArrayList();

    //        //string pwvalue = "";
    //        string path1 = @"T:\SupportFiles\qcConnstring";
    //       // string path1 = @"T:\SupportFiles\Connstring";
    //        //string path2 = path1 + "\\checkLdrive";
    //        string[] FilesPath1 = Directory.GetFiles(path1);

    //        string[] lines = File.ReadAllLines(FilesPath1[0]);
    //        //string pass = lines[1];

    //        foreach (string line in lines)
    //        {
    //            string lrive = "";
    //            string rmvline = line.Substring(3);

    //            for (int i = 0; i < rmvline.Length; i++)
    //            {
    //                if (i % 2 != 0)
    //                {
    //                    string c1 = rmvline[i].ToString();
    //                    //string check1 = pass.Replace(c1, "");
    //                    lrive = lrive + c1;
    //                }
    //            }

    //            convalre.Add(lrive);
    //        }

    //        address = convalre[0].ToString();
    //        server = convalre[1].ToString();`
    //        conpassword = convalre[2].ToString();
    //        condatbase = convalre[3].ToString();

    //        //return convalre;
    //        return "server=(" + address + ",priority=100);user id = " + server + "; password=" + conpassword + ";database=" + condatbase;
    //    }
    //    catch
    //    {

    //    }
    //    return "server=(" + address + ",priority=100);user id = " + server + "; password=" + conpassword + ";database=" + condatbase;
    //}

    //public static string connString = "server=(" + address + ",priority=100);user id = +" + server + "; password=" + conpassword + ";database=" + condatbase;
    //public static async System.Threading.Tasks.Task MyAppLog(string message)
    //{
    //    //var content = new StringContent($"{{\"message\":\"{message}\"}}", Encoding.UTF8, "application/json");
    //    //var response = await MyHttpClient.PostAsync(LogURL, content);
    //}

    

    public static string getKadavuSoll()
    {
        //string pwvalue = "";

        string path1 = @"T:\SupportFiles\RH";
        string path2 = path1 + "\\checkLdrive";
        string[] FilesPath1 = Directory.GetFiles(path2);
        int COUNT1 = FilesPath1.Count();

        string tpath = path2;
        string[] lines = File.ReadAllLines(FilesPath1[0]);
        string pass1 = lines[1];

        string path3 = @"L:\SupportFiles";
        string path4 = path3 + "\\checkLdrive";
        string[] FilesPath = Directory.GetFiles(path4);
        int COUNT = FilesPath.Count();

        string Lpath = path4;
        string[] lines2 = File.ReadAllLines(FilesPath[0]);
        string pass2 = lines2[1]; 

        pass1 = pass1.Substring(3);
        pass2 = pass2.Substring(0, pass2.Length - 3);

        string lrive = "";
        string drive = "";

        for (int i = 0; i < pass1.Length; i++)
        {
            if (i % 2 != 0)
            {
                string c1 = pass1[i].ToString();              
                lrive = lrive + c1;            
            }
        }

        for (int i = 0; i < pass2.Length; i++)
        {
            if (i % 2 != 0)
            {
                string c2 = pass2[i].ToString();               
                drive = drive + c2;              
            }
        }

        if (lrive == drive)
        {
           // MessageBox.Show("password : " + lrive);
        }       

        return lrive;
    }

    public static string getPayanarPeyar()
    {
        //string Lvalue = "";

        //string pwvalue = "";

        string path1 = @"T:\SupportFiles\RH";
        string path2 = path1 + "\\checkLdrive";
        string[] FilesPath1 = Directory.GetFiles(path2);
        int COUNT1 = FilesPath1.Count();

        string tpath = path2;
        string[] lines = File.ReadAllLines(FilesPath1[0]);
        string pass1 = lines[2];

        string path3 = @"L:\SupportFiles";
        string path4 = path3 + "\\checkLdrive";
        string[] FilesPath = Directory.GetFiles(path4);
        int COUNT = FilesPath.Count();

        string Lpath = path4;
        string[] lines2 = File.ReadAllLines(FilesPath[0]);
        string pass2 = lines2[2];

        pass1 = pass1.Substring(3);
        pass2 = pass2.Substring(0, pass2.Length - 3);

        string lrive = "";
        string drive = "";

        for (int i = 0; i < pass1.Length; i++)
        {
            if (i % 2 != 0)
            {
                string c1 = pass1[i].ToString();
                lrive = lrive + c1;
            }
        }

        for (int i = 0; i < pass2.Length; i++)
        {
            if (i % 2 != 0)
            {
                string c2 = pass2[i].ToString();
                drive = drive + c2;
            }
        }

        if (lrive == drive)
        {
            // MessageBox.Show("password : " + lrive);
        }

        return lrive;


       
    }

    public static void DriveMappingErrorMail(string ToMail, string Username)
    {
        string sklogin = getPayanarPeyar();
        string skpassword = getKadavuSoll();

        //bool errflag = false;
        try
        {
            // SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = false;

            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = false;
            // mySmtpClient.d

            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            string From_mail = sklogin;
            string To_mail = ToMail;
            

            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
            MailAddress to = new MailAddress(To_mail, "QC");
            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            // add ReplyTo
            MailAddress replyTo = new MailAddress(sklogin);
            myMail.ReplyToList.Add(replyTo);


            //  public static void SendEmail(string myversion, string modelname, string reportname, string skempname, string record, string process)

            //string version_login_system_empname = myversion + " / " + "systemname" + " / " + "username" + " / " + skempname + " ]";
            // set subject and encoding
            myMail.Subject = "Automatic Mail: QC";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;
            // set body-message and encoding
            //myMail.Body = "<b>" + reportname + "</b><br>using <b>HTML</b>.";
            myMail.Body = "<p class=MsoNormal><span class=MsoIntenseEmphasis>Greetings,<o:p></o:p></span></p> " +
                //"<p class=MsoNormal><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
                "<p class=MsoNormal><span class=MsoIntenseEmphasis>This is system generated mail for below user's machine which is not connected to L and T Drive </br> Please make the neccessary step accordingly  " + "" + "<o:p></o:p></span></p> " +
                "<p class=MsoNormal style = 'text-align:justify'><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
"<div style = 'mso-element:para-border-div;border-top:solid #5B9BD5 1.0pt; " +
"mso -border-top-themecolor:accent1;border-left:none;border-bottom:solid #5B9BD5 1.0pt; " +
"mso -border-bottom-themecolor:accent1;border-right:none;mso-border-top-alt:solid #5B9BD5 .5pt; " +
"mso -border-top-themecolor:accent1;mso-border-bottom-alt:solid #5B9BD5 .5pt; " +
"mso -border-bottom-themecolor:accent1;padding:10.0pt 0cm 10.0pt 0cm;margin-left:43.2pt;margin-right:43.2pt'> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>User<span style='mso-tab-count:1'>  " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + Username + "<o:p></o:p></span></span></p> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>Machine Name<span style='mso-tab-count:1'> " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + Environment.MachineName + "<o:p></o:p></span></span></p> " +

//"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
//"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
//"style='font-style:normal'>Record(s)<span style='mso-tab-count:1'> " +
//"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + record + "<o:p></o:p></span></span></p> " +

//"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
//"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
//"style='font-style:normal'>Process<span style='mso-tab-count:1'> " +
//"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + process + "<o:p></o:p></span></span></p> " +

"</div> " +

"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Regards,<o:p></o:p></span></p> " +
"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Esskay Automation Team.<o:p></o:p></span></p> " +
"<p class=MsoNormal><o:p>&nbsp;</o:p></p> ";
            //"</div> " +
            //"</body>";

            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html
            myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            //errflag = true;            
            MessageBox.Show("SmtpException has occured: " + ex.Message);
        }
    }

    //public static async System.Threading.Tasks.Task wbcMain()
    //{
    //    HttpClient client = new HttpClient();

    //    try
    //    {
    //        HttpResponseMessage response = await client.GetAsync(apiUrl);
    //        response.EnsureSuccessStatusCode();

    //        string responseBody = await response.Content.ReadAsStringAsync();

    //        // Parse the JSON response using Newtonsoft.Json
    //        JArray posts = JArray.Parse(responseBody);

    //        foreach (var post in posts)
    //        {
    //            // Extract specific fields, e.g., title and content
    //            string title = post["title"]["rendered"].ToString();
    //            string content = post["content"]["rendered"].ToString();

    //            // Output post title and content
    //            Console.WriteLine($"Title: {title}");
    //            Console.WriteLine($"Content: {content}");
    //            Console.WriteLine("--------------------------------------------");
    //        }
    //    }
    //    catch (HttpRequestException e)
    //    {
    //        Console.WriteLine($"Request error: {e.Message}");
    //    }
    //}


    public static string GetHashtablestringValues(Hashtable rValues, string key)
    {
        try
        {
            if (rValues.Contains(key) == true)
                return rValues[key].ToString();
        }
        catch
        {

        }
        return null;
    }
    public static double GetHashtabledoubleValues(Hashtable rValues, string key)
    {
        try
        {
            if (rValues.Contains(key) == true)
                return double.Parse(rValues[key].ToString());
        }
        catch
        {

        }
        return -1360;
    }
    public static int GetHashtableintValues(Hashtable rValues, string key)
    {
        try
        {
            if (rValues.Contains(key) == true)
                return int.Parse(rValues[key].ToString());
        }
        catch
        {

        }

        return -1360;
    }



    public static ArrayList GetProjectInformation(string skApplicationName)
    {
        ArrayList MyUserProject = new ArrayList();
        ArrayList ModelData = new ArrayList();
        string regkey = @"Software\\Esskay\\" + skApplicationName;
        RegistryKey MyRegModel = Registry.CurrentUser.OpenSubKey(regkey, true);
        if (MyRegModel != null) {
            if (MyRegModel.SubKeyCount >= 1)
            {
                string[] MyModelData = MyRegModel.GetSubKeyNames();
                ArrayList MyProjects = new ArrayList();
                foreach (string ModelName in MyModelData)
                {
                    ArrayList MyProject = new ArrayList();
                    RegistryKey MyRegModelPath = Registry.CurrentUser.OpenSubKey(regkey + "\\" + ModelName, true);
                    if (MyRegModelPath != null)
                    {
                        string[] MyModelPath = MyRegModelPath.GetSubKeyNames();

                        foreach (string ModelPath in MyModelPath)
                        {
                            MyProject.Add(GetMyRegistryKeyData(skApplicationName, ModelName, ModelPath));
                        }
                    }
                    MyUserProject.Add(MyProject);
                }
                // MyUserProject.Add(MyProjects);

            }
            else
                return null;
        }
        return MyUserProject;
    }

    public static ArrayList GetMyRegistryKeyData(string skApplicationName, string ModelName, string ModelPath)
    {
        ArrayList MyData = new ArrayList();
        string modelpath = ModelPath.Replace("\\", "|");
        string chkreg = skApplicationName + "\\" + ModelName + "\\" + modelpath;
        string client = skWinLib.GetRegistryValue(chkreg, "client");
        string version = skWinLib.GetRegistryValue(chkreg, "version");
        string project = skWinLib.GetRegistryValue(chkreg, "project");
        MyData.Add(ModelName);
        MyData.Add(ModelPath);
        MyData.Add(client);
        MyData.Add(version);
        MyData.Add(project);
        return MyData;
    }

    public static bool DebugFlag(string skApplicationName = "")
    {

        // get old entry stored earlier and update UI
        string keyName = @"Software\\Esskay\\" + skApplicationName;

        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName);
        if (registryKey != null)
        {
            string skdebug = (string)registryKey.GetValue("skdebug");
            if (skdebug != null)
            {
                if (skdebug.ToUpper().Trim() == "TRUE")
                    return true;
            }
        }
        return false;
    }


    public static void OpenHelpFile(string currentapp, string path, string msgCaption, bool flagvideo)
    {
        try
        {
            // call the other application
            // System.Diagnostics.Process.Start("\\\\192.168.2.3\\Application & Tool\\SKTSAppAutoUpdate.exe");
            System.Diagnostics.Process myprocess = new System.Diagnostics.Process();

            if (currentapp.ToUpper().Contains(" VER.") == true)
                currentapp = currentapp.Substring(0, currentapp.IndexOf(" VER."));

            string mycurrentapp = currentapp + ".pdf";
            bool hlpflagpdf = false;

            if (System.IO.File.Exists(path + @"\" + mycurrentapp) == true)
            {
                hlpflagpdf = true;
                myprocess.StartInfo.FileName = path + @"\" + mycurrentapp;
                myprocess.Start();
            }

            bool hlpflagvid = false;
            mycurrentapp = currentapp + ".webm";
            if (System.IO.File.Exists(path + @"\" + mycurrentapp) == true)
            {
                hlpflagvid = true;
                myprocess.StartInfo.FileName = path + @"\" + mycurrentapp;
                myprocess.Start();
            }
            if (hlpflagpdf == false && hlpflagvid == false)
            {
                MessageBox.Show(currentapp + "\n\n\nHelp documention under construction", msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        catch (Exception ex)
        {
            MessageBox.Show(currentapp + "\n\n\nHelp documention under construction.\n" + ex.Source, msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }


    /// <summary>
        public static void StoreControlValues(Control.ControlCollection MyControlCollection, string skApplicationName)
        {
            foreach (Control c in MyControlCollection)
            {
                if (c is TextBox)
                {
                    skWinLib.SetRegistryValue(skApplicationName, c.Name, ((TextBox) c).Text);
                }
                else if (c is ComboBox)
                {
                    skWinLib.SetRegistryValue(skApplicationName, c.Name, ((ComboBox)c).Text.ToString());
                }
                else if (c is CheckBox)
                {
                    skWinLib.SetRegistryValue(skApplicationName, c.Name, ((CheckBox)c).Checked.ToString());
                }
                else if (c is RadioButton)
                {
                    skWinLib.SetRegistryValue(skApplicationName, c.Name, ((RadioButton)c).Checked.ToString());
                }
                else if (c is RichTextBox)
                {
                    skWinLib.SetRegistryValue(skApplicationName, c.Name, ((RichTextBox)c).Text);
                }
                if (c.HasChildren)
                {
                    StoreControlValues(c.Controls, skApplicationName);
                }
            }
        }


  

    public static void GetControlValues(Control.ControlCollection MyControlCollection, string skApplicationName, bool IsDefaultText) //Control control
        {
        
            foreach (Control c in MyControlCollection)
            {
            if (c is TextBox)
            {

                //string tmp = skWinLib.GetRegistryValue(skApplicationName, c.Name) ?? string.Empty;
                //               if (tmp.Trim().Length >= 1 || IsDefaultText == true)
                string tmp = skWinLib.GetRegistryValue(skApplicationName, c.Name);
                if (tmp != null)      
                {
                    if (IsDefaultText == true)
                        ((TextBox)c).Text = tmp;

                }

            }
            else if (c is ComboBox)
            {
                string tmp = skWinLib.GetRegistryValue(skApplicationName, c.Name);
                if (tmp != null)
                {
                    if (IsDefaultText == true)
                    {
                        if (((ComboBox)c).Items.Contains(tmp) == true)
                            ((ComboBox)c).Text = tmp;
                    }
               

                }
            }

            else if (c is CheckBox)
            {
                bool checkBoxValue;
                bool.TryParse(skWinLib.GetRegistryValue(skApplicationName, c.Name), out checkBoxValue);
                ((CheckBox)c).Checked = checkBoxValue;
            }
            else if (c is RadioButton)
            {
                bool radioButtonValue;
                bool.TryParse(skWinLib.GetRegistryValue(skApplicationName, c.Name), out radioButtonValue);
                ((RadioButton)c).Checked = radioButtonValue;

            }
            else if (c is RichTextBox)
            {
                ((RichTextBox)c).Text = skWinLib.GetRegistryValue(skApplicationName, c.Name) ?? string.Empty;
            }
            if (c.HasChildren)
                {
                    GetControlValues(c.Controls, skApplicationName, IsDefaultText);
                }
            }
        }



    //private void StoreUserEntry()
    //{
    //    StoreControlValues(this);
    //}

    public static bool SetRegistryValue(string skApplicationName, string keyName, string keyvalue)
    {
        string regkey = userRoot + "\\" + skApplicationName;
        try
        {
            Registry.SetValue(regkey, keyName, keyvalue);
        }
        catch
        {
            return false;
        }
        return true;
    }






    public static string GetRegistryValue(string skApplicationName, string keyName)
    {

        //string keyName = skWinLib.userRoot + "\\" + skApplicationName;
        //keyName = @"Software\\Esskay\\" + skApplicationName;
        //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName);
        //if (registryKey != null)
        //{
        //    //SK-TR-01
        //    string Tran_Project = (string)registryKey.GetValue("Tran_Project");
        //    if (Tran_Project != null)
        //        txttranproject.Text = Tran_Project;
        //string regkey = @userRoot + "\\" + skApplicationName;

        string regkey = @"Software\\Esskay\\" + skApplicationName;
        try
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regkey);
            if (registryKey != null)
            {
                return (string)registryKey.GetValue(keyName);
            }
        }
        catch
        {
            return "";
        }

        return "";
    }

    //public static bool GetBoolRegistryValue(string skApplicationName, string keyName)
    //{
    //    string flag = skWinLib.GetRegistryValue(skApplicationName, keyName);
    //    if (flag != null)
    //    {
    //        if (flag.Trim().ToUpper() == "TRUE")
    //            return true;
    //        else if (flag.Trim().ToUpper() == "FALSE")
    //            return false;
    //    }
    //    return false;
    //}

    public static void GetBoolRegistryValue(CheckBox MyItem, string skApplicationName, string keyName)
    {
        string flag = skWinLib.GetRegistryValue(skApplicationName, keyName);
        if (flag != null)
        {
            if (flag.Trim().ToUpper() == "TRUE")
                MyItem.Checked = true;
            else if (flag.Trim().ToUpper() == "FALSE")
                MyItem.Checked = false;
        }
    }

    public static void GetBoolRegistryValue(RadioButton MyItem, string skApplicationName, string keyName)
    {
        string flag = skWinLib.GetRegistryValue(skApplicationName, keyName);
        if (flag != null)
        {
            if (flag.Trim().ToUpper() == "TRUE")
                MyItem.Checked = true;
            else if (flag.Trim().ToUpper() == "FALSE")
                MyItem.Checked = false;
        }
    }

    //public static string GetRegistryValue(string skApplicationName, string keyName, string keyitem)
    //{
    //    string regkey = userRoot + "\\" + skApplicationName + "\\" + keyName;
    //    try
    //    {
    //        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regkey);
    //        if (registryKey != null)
    //        {
    //            return (string)registryKey.GetValue(keyitem);
    //        }
    //    }
    //    catch
    //    {
    //        return "";
    //    }
    //    return "";
    //}

    //public static bool GetBoolRegistryValue(string skApplicationName, string keyName, string keyitem)
    //{
    //    string flag = skWinLib.GetRegistryValue(skApplicationName, keyName, keyitem);
    //    if (flag.Trim().ToUpper() == "TRUE")
    //        return true;
    //    else
    //        return false;
    //}



    public static bool CreateSK_Automation(string skpath)
    {
        //string skpath = skTSLib.ModelPath + "\\SK_Automation\\" + myapplication;
        if (!Directory.Exists(skpath))
        {
            Directory.CreateDirectory(skpath);
            return true;
        }
        return false;
    }


    public static bool CheckAndCreateFolder(string skpath)
    {
        //string skpath = skTSLib.ModelPath + "\\SK_Automation\\" + myapplication;
        if (!Directory.Exists(skpath))
        {
            Directory.CreateDirectory(skpath);
            return true;
        }
        return false;
    }

    public static string GetXS_ServicePackVersion(string XS_Version)
    {
        //SK_XSServicePack from skVersion
        try
        {
            int idx = skVersion.IndexOf(XS_Version);
            if (idx >= 0)
                return SK_XSServicePack[idx];

        }
        catch
        {

        }
        return null;
    }



    public static bool CreateEsskayFolder()
    {
        string skpath = "d://esskay";
        bool flag1 = CheckAndCreateFolder(skpath);

        // create folder for each version
        foreach (string TSVersion in skVersion)
        {
            skpath = "d://esskay//" + TSVersion;
            bool flag2 = CheckAndCreateFolder(skpath);
        }

        skpath = "d://esskay//SupportFiles";
        bool flag7 = CheckAndCreateFolder(skpath);

        //Starting from 2024 Tekla Model sharing tool Released since model sharing part of diamond, carbon and graphite named users
        //skpath = "d://esskay//SupportFiles//SK_TMSL";
        //bool flag7a = CheckAndCreateFolder(skpath);

        skpath = "d://esskay//model";
        bool flag8 = CheckAndCreateFolder(skpath);

        return true;
    }

    //public static bool CreateEsskayFolder()
    //{
    //    string skpath = "d://esskay";
    //    bool flag1 = CheckAndCreateFolder(skpath);


    //        skpath = "d://esskay//2019.1";
    //        bool flag2 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//2020.0";
    //        bool flag3 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//2021.0";
    //        bool flag4 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//2022.0";
    //        bool flag5 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//2023.0";
    //        bool flag6 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//SupportFiles";
    //        bool flag7 = CheckAndCreateFolder(skpath);

    //        skpath = "d://esskay//model";
    //        bool flag8 = CheckAndCreateFolder(skpath);

    //        return true;
    //  }
    //public static bool CreateEsskayFolder()
    //{
    //    string skpath = "d://esskay";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//2019.1";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//2020.0";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//2021.0";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//2022.0";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//2023.0";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//SupportFiles";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }

    //    skpath = "d://esskay//model";
    //    if (!Directory.Exists(skpath))
    //    {
    //        Directory.CreateDirectory(skpath);
    //    }
    //    return true;

    //}


    public static bool DebugLog(bool flag, int debugcount, string logfile, string log)
    {
        if (flag == true)
        {
            //check and create d:\esskay\2019.1, 2020, 2021, 2022, 2023, 2024
            CreateEsskayFolder();
            try
            {
                // write to file
                using (TextWriter streamWriter = new StreamWriter(logfile, true))
                {
                    streamWriter.WriteLine(debugcount++.ToString() + "|" + DateTime.Now.ToString("HH:mm:ss:ff") + "|" + log);
                    streamWriter.Close();
                    return true;
                }
            }

            catch
            {
                // write to error handling file
                using (TextWriter streamWriter = new StreamWriter(logfile.Replace(".vgml", "-err01.vgml"), true))
                {
                    streamWriter.WriteLine(debugcount++.ToString() + "|" + DateTime.Now.ToString("HH:mm:ss:ff") + "|" + log);
                    streamWriter.Close();
                    return true;
                }
            }
        }
        return false;
    }


    

    public static string mysql_replace_quotecomma(string inputstring)
    {
        //to avoid mysql error during insert or update replace quote or comma char
        string mystring = inputstring;

        //replace single quote with "small g ALT+255 big V";
        mystring = mystring.Replace("'", "g" + (char)160 + "V");

        //replace comma with "small v ALT+255 big G";
        mystring = mystring.Replace(",", "v" + (char)160 + "G");

        ////replace \\ error with "small v ALT+255 big G";
        //mystring = mystring.Replace("\\", "v" + (char)160 + "G");

        return mystring;
    }

    public static string mysql_set_quotecomma(string inputstring)
    {
        //read mysql dbvalue and update quote or comma char 
        string mystring = inputstring;

        //for quote
        mystring = mystring.Replace("g" + (char)160 + "V", "'");
        //for comma
        mystring = mystring.Replace("v" + (char)160 + "G", ",");

        return mystring;
    }

    public static void accesslog(string skApplicationName, string skApplicationVersion, string task, string skremarks, string XS_ModelName = "", string XS_Version = "", string XS_Configuration = "")
    {
        //check whether l drive exist
        DriveInfo MyDrive = new System.IO.DriveInfo("L:");
        if (MyDrive.IsReady == true)
        {
            //logfolder
            string logfolder = "l://teklasupport//" + DateTime.Now.Year + "//log//" + DateTime.Now.ToString("MMM");
            if (!System.IO.Directory.Exists(logfolder) == true)
                System.IO.Directory.CreateDirectory(logfolder);
            try
            {

                string logfile = logfolder + "//" + System.Environment.UserName + "_" + DateTime.Now.Day + ".log";
                Stream mystream = new FileStream(logfile, FileMode.Append, FileAccess.Write, FileShare.None);
                TextWriter streamWriter = new StreamWriter(mystream, Encoding.Unicode);
                streamWriter.WriteLine(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + ";" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "|" + systemname + "|" + username + "|" + skApplicationName + "|" + skApplicationVersion + "|" + task + "|" + skremarks + "|" + "" + "|" + "" + "|" + XS_ModelName + "|" + XS_Version + "|" + XS_Configuration);
                streamWriter.Close();
                mystream.Close();

            }
            catch (Exception Ex)
            {


                int retries = 5;
                string errlogfile = logfolder + "//" + System.Environment.UserName + "_" + DateTime.Now.Day + ".errlog";

                //while (retries > 0)
                for(int i = 0; i < retries; i++)
                {
                    try
                    {

                        Stream mystream = new FileStream(errlogfile, FileMode.Append, FileAccess.Write, FileShare.None);
                        TextWriter streamWriter = new StreamWriter(mystream, Encoding.Unicode);
                        streamWriter.WriteLine(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + ";" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "|" + systemname + "|" + username + "|" + skApplicationName + "|" + skApplicationVersion + "|" + task + "|" + skremarks + ";" + Ex.Message + ";" + Ex.Source + ";" + Ex.StackTrace + "|" + "" + "|" + "" + "|" + XS_ModelName + "|" + XS_Version + "|" + XS_Configuration);
                        streamWriter.Close();
                        mystream.Close();
                        break;
                    }
                    catch (IOException)
                    {
              
                        errlogfile = logfolder + "//" + System.Environment.UserName + "_" + DateTime.Now.Day + retries +  ".errlog";
                        System.Threading.Thread.Sleep(1000); // Wait for 1 second before retrying
                        retries--;
                    }
                }

                // worklog("accesslog;", skremarks + ";" + Ex.Message + ";" + Ex.Source + ";" + Ex.StackTrace);
            }
        }
    }

    public static void worklog(string skApplicationName, string skApplicationVersion, string task, string skremarks, string XS_ModelName = "", string XS_Version = "", string XS_Configuration = "")
    {
        //Same as AccessLog
        skWinLib.accesslog(skApplicationName, skApplicationVersion, task, skremarks, XS_ModelName, XS_Version, XS_Configuration);
    }
    
    public static void ExecuteLastSKApplication()
    {
        //load the earlier or last used application
        string keyName = @"Software\\Esskay\\";
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName);
        if (registryKey != null)
        {
            string skdebug = (string)registryKey.GetValue(SKLastUsedApplication1);
            if (skdebug != null)
            {
                //if (!skdebug.ToUpper().Contains("SKTeklaProject.exe".ToUpper()))
                if (skdebug.ToUpper().Contains("SKTeklaProject".ToUpper()) == false && skdebug.ToUpper().Contains("AutoUpdate".ToUpper()) == false && skdebug.ToUpper().Contains("Admin".ToUpper()) == false)
                {
                    if (System.IO.File.Exists(skdebug) == true)
                        System.Diagnostics.Process.Start(skdebug);
                }
            }
        }
    }

    
   

    public static string GetTeklaInstalledPath(string TeklaVersion)
    {
        //Function changed on 2503.25 17:19 to add Program Files
        //"C:\Program Files\Tekla Structures\2019.1\nt\bin\TeklaStructures.exe"
        string tspathchange = @"\nt\bin\";
        double dTeklaVersion = Convert.ToDouble(TeklaVersion);
        if (dTeklaVersion >= 2022.0)
            tspathchange = @"\bin\";

        //Check1 -  C:\TeklaStructures\
        string TSApplication = @"C:\TeklaStructures\" + TeklaVersion + tspathchange + "TeklaStructures.exe";
        if (System.IO.File.Exists(TSApplication) == true)
            return TSApplication;
        //Check2 -  D:\TeklaStructures\
        TSApplication = @"D:\TeklaStructures\" + TeklaVersion + tspathchange + "TeklaStructures.exe";
        if (System.IO.File.Exists(TSApplication) == true)
            return TSApplication;
        //Check3 -  C:\Program Files\Tekla Structures\
        TSApplication = @"C:\Program Files\Tekla Structures\" + TeklaVersion + tspathchange + "TeklaStructures.exe";
        if (System.IO.File.Exists(TSApplication) == true)
            return TSApplication;
        //Check4 -  D:\Program Files\Tekla Structures\
        TSApplication = @"D:\Program Files\Tekla Structures\" + TeklaVersion + tspathchange + "TeklaStructures.exe";
        if (System.IO.File.Exists(TSApplication) == true)
            return TSApplication;
        return "";
    }
    public static ArrayList GetTeklaInstalledVersion()
    {
        ArrayList MyVersion = new ArrayList();
        string TSApplication = string.Empty;

        TSApplication = "2019.1";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        TSApplication = "2020.0";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        TSApplication = "2021.0";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        TSApplication = "2022.0";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        TSApplication = "2023.0";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        TSApplication = "2024.0";
        if (GetTeklaInstalledPath(TSApplication).Trim().Length >= 1)
            MyVersion.Add(TSApplication.ToUpper());

        return MyVersion;
    }

    public static string GetTeklaServicePack(string XS_Version)
    { 
        try
        {

            string TSApplication = GetTeklaInstalledPath(XS_Version);
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(TSApplication);

            if (fileVersionInfo != null) 
                return fileVersionInfo.FileVersion;
           
        }
        catch
        {
        }
        return null;
    }

    //public static string GetTeklaServicePack(string XS_Version)
    //{
    //ArrayList MyList = GetTeklaInstalledVersion();
    //foreach (string XS_Version in MyList)
    //{
    //    //string XS_Version = "2024.0";
    //    string TSApplication = GetTeklaInstalledPath(XS_Version);
    //    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(TSApplication);

    //    //Console.WriteLine("File: " + fileVersionInfo.FileName);
    //    Console.WriteLine("Version: " + fileVersionInfo.FileVersion);
    //}
    //    try
    //    {
    //        string xs_sp_version = skWinLib.GetXS_ServicePackVersion(XS_Version);
    //        string query = "SELECT * FROM Win32_Product WHERE Name like '%Tekla Structures " + xs_sp_version + " Service Pack%'";
    //        ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
    //        foreach (ManagementObject mo in mos.Get())
    //        {
    //            string applicationName = mo["Name"] as string;
    //            if (!string.IsNullOrEmpty(applicationName))
    //                return applicationName;
    //        }
    //    }
    //    catch
    //    {
    //    }
    //    return null;
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

    //public static DataGridView FileToMyDataGridView(string fileName, string separator, DataGridView MyDataGridView)
    //{

    //}

    //public static DataTable FileToTable(String fileName, string separator = "|")
    //{
    //    DataTable result = new DataTable();

    //    foreach (var line in System.IO.File.ReadLines(fileName))
    //    {
    //        DataRow row = result.NewRow();

    //        String[] items = line.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //        //TODO: you may want tabulation ('\t') separator as well as space
    //        //String[] items = line.Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    //        // Columns adjusting: you don't need exactly 3 columns  
    //        for (int i = result.Columns.Count; i < items.Length; ++i)
    //            result.Columns.Add(String.Format("COL {0}", i + 1));

    //        row.ItemArray = items;
    //        result.Rows.Add(row);
    //    }

    //    return result;
    //}

    public static DataTable ArrayListToTable(ArrayList MyArrayList, string separator = "|")
    {
        DataTable result = new DataTable();

        foreach (string line in MyArrayList)
        {
            DataRow row = result.NewRow();

            String[] items = line.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //TODO: you may want tabulation ('\t') separator as well as space
            //String[] items = line.Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            // Columns adjusting: you don't need exactly 3 columns  
            for (int i = result.Columns.Count; i < items.Length; ++i)
                result.Columns.Add(String.Format("COL {0}", i + 1));

            row.ItemArray = items;
            result.Rows.Add(row);
        }

        return result;
    }


    public static void Import_DataGridView(string ApplicationName, string ApplicationVersion, string ModelPath, DataGridView mydatagridview)
    {
        try
        {
            string filename = ModelPath + "\\SK_Automation\\" + ApplicationName + "_" + ApplicationVersion + "_" + mydatagridview.Name + "_" + skWinLib.username + ".dgei";
            if (System.IO.File.Exists(filename) == true)
            {
                //read file if exists
                char[] splitchar = new Char[] { '|' };
                ArrayList dgv = ReadFile(filename, "//", "");
                int lnct = dgv.Count;
                int maxcol = mydatagridview.ColumnCount;
                if (lnct > 1)
                {
                    string[] splitcoldata = dgv[0].ToString().Split(splitchar);
                    mydatagridview.Rows.Clear();
                    ////mydatagridview.Columns.Clear();
                    //int colct = splitcoldata.Length;
                    //for (int i = 0; i < colct; i++)
                    //{
                    //    // MyDataGridViewColumn.Visible + "#" + MyDataGridViewColumn.Name + "#" + MyDataGridViewColumn.HeaderText;
                    //    string coldata = splitcoldata[i];
                    //    int chk1 = coldata.IndexOf("#");
                    //    int chk2 = coldata.IndexOf("#", chk1+1);
                    //    if (chk1 >=1 && chk2 >2)
                    //    {
                    //        bool flag = true;
                    //        if (coldata.Substring(0,chk1).ToUpper().Contains("FALSE") ==  true)
                    //            flag = false;

                    //        string colname = coldata.Substring(chk1+1, chk2-chk1-1);
                    //        string colhdr = coldata.Substring(chk2+1);

                    //        DataGridViewColumn MyCol = new DataGridViewColumn();
                    //        MyCol.Name = colname;
                    //        MyCol.HeaderText = colhdr;
                    //        MyCol.Visible = flag;
                    //        //MyCol.CellTemplate = ;
                    //        mydatagridview.Columns.Add(MyCol);
                    //    }

                    //}

                    for (int i = 1; i < lnct; i++)
                    {
                        string[] splitrowdata = dgv[i].ToString().Split(splitchar);
                        mydatagridview.Rows.Add(splitrowdata.Take(maxcol).ToArray());

                    }
                    updaterowheader(mydatagridview);
                }

            }
        }
        catch
        {

        }
    }
    public static string Export_DataGridView(string ApplicationName, string ApplicationVersion, string ModelPath, DataGridView mydatagridview, string fileseperator = "|")
    {

        int rowct = mydatagridview.Rows.Count;
        int colct = mydatagridview.Columns.Count;
        ArrayList dgv = new ArrayList();
        //string fileseperator = "|";

        for (int i = 0; i < rowct; i++)
        {
            DataGridViewRow row = mydatagridview.Rows[i];
            string dgvexportdata = fileseperator;
            for (int j = 0; j < colct; j++)
            {
                DataGridViewCell ox = mydatagridview.Rows[i].Cells[j];
                if (j == 0)
                    dgvexportdata = ox.EditedFormattedValue.ToString();
                else
                    dgvexportdata = dgvexportdata + fileseperator + ox.EditedFormattedValue.ToString();
            }
            if (dgvexportdata.Replace(fileseperator, "").Length >= 1)
                dgv.Add(dgvexportdata.Replace("\n", ""));
        }

        //write the column name
        string mycdgcolumnheader = fileseperator;
        for (int i = 0; i < mydatagridview.Columns.Count; i++)
        {
            DataGridViewColumn MyDataGridViewColumn = mydatagridview.Columns[i];
            mycdgcolumnheader = mycdgcolumnheader + fileseperator + MyDataGridViewColumn.Name;

        }

        mycdgcolumnheader = mycdgcolumnheader.Replace(fileseperator + fileseperator, "");
        string folderpath = ModelPath + "\\SK_Automation\\";
        skWinLib.CheckAndCreateFolder(folderpath);
        string filename = folderpath + ApplicationName + "_" + ApplicationVersion + "_" + mydatagridview.Name + "_" + skWinLib.username + ".dgei";
        //write to file
        using (TextWriter streamWriter = new StreamWriter(filename))
        {
            //write the instruction in the header
            streamWriter.WriteLine("//Don't modify this file unless you are aware.");
            streamWriter.WriteLine("//For issue or help contact Automation team.");
            streamWriter.WriteLine("//Export Version: 240613");
            //write the column name
            streamWriter.WriteLine(mycdgcolumnheader);
            foreach (string linedata in dgv)
                streamWriter.WriteLine(linedata);
            streamWriter.Close();
        }
        return filename.Replace(ModelPath, @".\");
    }
    public static void Export_DataGridView(DataGridView mydatagridview, string filename, string myFormat, string myHideColumns, bool append = false)
    {

        int rowct = mydatagridview.Rows.Count;
        int colct = mydatagridview.Columns.Count;
        ArrayList dgv = new ArrayList();
        string fileseperator = ",";

        if (myFormat == "CSV")
            fileseperator = ",";
        else if (myFormat == "PIE")
            fileseperator = "|";
        for (int i = 0; i < rowct; i++)
        {
            DataGridViewRow row = mydatagridview.Rows[i];
            string dgvexportdata = fileseperator;
            for (int j = 0; j < colct; j++)
            {
                DataGridViewCell ox = mydatagridview.Rows[i].Cells[j];
                if (j == 0)
                    dgvexportdata = ox.EditedFormattedValue.ToString();
                else
                    dgvexportdata = dgvexportdata + fileseperator + ox.EditedFormattedValue.ToString();
            }
            if (dgvexportdata.Replace(fileseperator, "").Length >= 1)
                dgv.Add(dgvexportdata.Replace("\n", ""));
        }

        //write the column name
        string mycdgcolumnheader = fileseperator;
        for (int i = 0; i < mydatagridview.Columns.Count; i++)
        {
            DataGridViewColumn MyDataGridViewColumn = mydatagridview.Columns[i];
            //if (MyDataGridViewColumn.Visible == true)
            {
                mycdgcolumnheader = mycdgcolumnheader + fileseperator + MyDataGridViewColumn.Name;
            }
        }

        mycdgcolumnheader = mycdgcolumnheader.Replace(fileseperator + fileseperator, "");
        //write to file
        using (TextWriter streamWriter = new StreamWriter(filename, append))
        {
            //write the instruction in the header
            streamWriter.WriteLine("//Don't Modify this file unless you are aware.");
            streamWriter.WriteLine("//For issue or help contact Automation team.");
            streamWriter.WriteLine("//Export Version: 221208");
            //write the column name
            streamWriter.WriteLine("//" + mycdgcolumnheader);

            foreach (string linedata in dgv)
                streamWriter.WriteLine(linedata);

            streamWriter.Close();
        }
    }

    //read the file with ignore begin with and ignore words
    public static ArrayList ReadFile(string filename, string ignorebeginwith = "//", string ignoreword = "")
    {
        ArrayList MyArrayList = new ArrayList();
        if (System.IO.File.Exists(filename) == true)
        {
            try
            {
                System.IO.StreamReader srdgv = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
                string line = " ";
                while ((line = srdgv.ReadLine()) != null)
                {
                    if (ignorebeginwith != string.Empty)
                    {
                        if (line != string.Empty)
                        {
                            if (ignorebeginwith != line.Substring(0, ignorebeginwith.Length))
                                MyArrayList.Add(line.ToUpper());
                        }

                    }
                    //else if (ignoreword != string.Empty && ignoreword.Trim().Length >= 1)
                    //{
                    //    if (line.ToUpper().Contains(ignoreword.ToUpper()) == false)
                    //        MyArrayList.Add(line);
                    //}

                    //else
                    //    MyArrayList.Add(line);
                }
                srdgv.Close();

            }
            catch (Exception Ex)
            {
                MyArrayList.Add(Ex.Message + "\n" + Ex.Source);
            }
        }
        return MyArrayList;
    }

    public static void Import_DataGridView(DataGridView mydatagridview, string filename, string myFormat, string myHideColumns, string ignorebeginwith = "//", bool isrowclear = true)
    {
        try
        {
            if (System.IO.File.Exists(filename) == true)
            {
                //read file if exists
                ArrayList dgv = new ArrayList();
                //System.IO.StreamReader srdgv = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
                //string line = " ";
                //while ((line = srdgv.ReadLine()) != null)
                //{
                //    if (ignorebeginwith != string.Empty)
                //    {
                //        if (ignorebeginwith != line.Substring(0, ignorebeginwith.Length))
                //            dgv.Add(line);
                //    }
                //    else
                //        dgv.Add(line);
                //}
                //srdgv.Close();
                dgv = ReadFile(filename, ignorebeginwith, "");

                char[] splitchar = new Char[] { ',' };
                if (myFormat == "CSV")
                    splitchar = new Char[] { ',' };
                else if (myFormat == "PIE")
                    splitchar = new Char[] { '|' };

                //update data grid view
                if (isrowclear == true)
                    mydatagridview.Rows.Clear();

                bool addflag = mydatagridview.AllowUserToAddRows;
                foreach (string linedata in dgv)
                {
                    string[] splitrowdata = linedata.ToString().Split(splitchar);
                    int colct = splitrowdata.Length - mydatagridview.Columns.Count;
                    for (int i = 0; i < colct; i++)
                    {
                        mydatagridview.Columns.Add((i + 1).ToString(), (i + 1).ToString());
                        //mydatagridview.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    mydatagridview.Rows.Add(splitrowdata);
                    //if (addflag == true)
                    //    mydatagridview.Rows[mydatagridview.Rows.Count-2].HeaderCell.Value = (mydatagridview.Rows.Count-1).ToString();
                    //else
                    //    mydatagridview.Rows[mydatagridview.Rows.Count - 1].HeaderCell.Value = (mydatagridview.Rows.Count).ToString();
                }
                updaterowheader(mydatagridview);
            }
        }
        catch
        {

        }

    }

    public static string GetCellValue(DataGridViewRow MyRow, string dgColName)
    {
        if (MyRow.Cells[dgColName].Value != null)
            return MyRow.Cells[dgColName].Value.ToString();

        return string.Empty;
    }

    public static string SetCellValue(DataGridViewRow MyRow, string dgColName, string CellValue, bool IsAppend = false, bool AvoidDuplicateValue = false)
    {
        string MyCellValue = GetCellValue(MyRow, dgColName);
        //check whether existing value is empty
        if (MyCellValue.Trim().Length == 0)
            MyCellValue = CellValue;
        else
        {
            bool flag = MyCellValue.Contains(CellValue);
            if (flag == true)
            {
                if (AvoidDuplicateValue == true)
                    MyCellValue = "";
            }
            if (IsAppend == true)
                MyCellValue = MyCellValue + CellValue;
            else
                MyCellValue = CellValue;

        }


        //set cell value
        MyRow.Cells[dgColName].Value = MyCellValue;

        return MyCellValue;
    }


    public static void updaterowheader(DataGridView myDataGridView)
    {
        //provide row header title
        myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        myDataGridView.Visible = false;
        if (myDataGridView.Rows.Count >= 1)
        {
            for (int j = 0; j < myDataGridView.Rows.Count; j++)
            {
                myDataGridView.Rows[j].HeaderCell.Value = (j + 1).ToString();
            }
        }

        myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        myDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
        //myDataGridView.Refresh();
        myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        myDataGridView.Visible = true;
        //    dataGridView1.RowHeadersWidthSizeMode =
        //    DataGridViewRowHeadersWidthSizeMode.DisableResizing;

       
        //    if (myDataGridView.Rows.Count >=1)
        //    {
        //        myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        //        myDataGridView.RowHeadersVisible = true;
        //        myDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
        //        myDataGridView.Refresh();
        //   }

    }

    public static bool IsCellValueUnique(DataGridView MyDataGridView, string columnName, string cellValue)
    {
        var cellValues = MyDataGridView.Rows
            .OfType<DataGridViewRow>()
            .Where(row => row.Cells[columnName].Value != null)
            .Select(row => row.Cells[columnName].Value.ToString());

        return cellValues.Count(value => value == cellValue) == 1;
    }

    public static void SetColumnnVisible(DataGridView MyDataGridView, List<bool> Flag)
    {
        if (MyDataGridView.Columns.Count == Flag.Count)
        {
            for (int i = 0; i < MyDataGridView.Columns.Count; i++)
            {
                MyDataGridView.Columns[i].Visible = Flag[i];
            }
        }
    }

    public static void SetColumnnHeaderText(DataGridView MyDataGridView, List<string> HeaderText)
    {
        if (MyDataGridView.Columns.Count == HeaderText.Count)
        {
            for (int i = 0; i < MyDataGridView.Columns.Count; i++)
            {
                MyDataGridView.Columns[i].HeaderText = HeaderText[i];
            }
        }

    }

    public static List<bool> GetColumnnVisible(DataGridView MyDataGridView)
    {
        List<bool> ColName = new List<bool>();
        for (int i = 0; i < MyDataGridView.Columns.Count; i++)
        {
            ColName.Add(MyDataGridView.Columns[i].Visible);
        }
        return ColName;
    }

    public static List<string> GetColumnnName(DataGridView MyDataGridView)
    {
        List<string> ColName = new List<string>();
        for (int i = 0; i < MyDataGridView.Columns.Count; i++)
        {
            ColName.Add(MyDataGridView.Columns[i].Name);
        }
        return ColName;
    }

    public static List<string> GetColumnnHeaderText(DataGridView MyDataGridView)
    {
        List<string> HeaderText = new List<string>();
        for (int i = 0; i < MyDataGridView.Columns.Count; i++)
        {
            HeaderText.Add(MyDataGridView.Columns[i].HeaderText);
        }
        return HeaderText;
    }

    public static void DataGridView_Setting_After(DataGridView myDataGridView)
    {
        //myDataGridView.VirtualMode = false;
        //myDataGridView.AutoSize = false;
        //myDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        //myDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        //myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        //myDataGridView.RowHeadersVisible = true;
        //myDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
        myDataGridView.AutoResizeColumns();
        //myDataGridView.AutoResizeRows();

        myDataGridView.Visible = true;
        myDataGridView.Refresh();
    }

    public static void DataGridView_Setting_Before(DataGridView myDataGridView)
    {
        //foreach (DataGridViewColumn c in myDataGridView.Columns)
        //{
        //    c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        //}

        //myDataGridView.Rows.Clear();
        //myDataGridView.Dock = DockStyle.Fill;
        //myDataGridView.VirtualMode = true;
        //myDataGridView.Enabled = false;
        myDataGridView.AutoSize = false;
        myDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        myDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        //myDataGridView.RowHeadersVisible = false;

        //myDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        // or even better, use .DisableResizing. Most time consuming enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders

        // set it to false if not needed
        //myDataGridView.RowHeadersVisible = false;

        myDataGridView.Visible = false;


    }

    public static DateTime setStartup(string skApplicationName, string skapplicationVersion, string Task, string Remark, string statusbar1, string statusbar2, Label lblsbar1, Label lblsbar2)
    {
        //12-apr-2022
        DateTime mytime = DateTime.Now;
        //lblsbar1.Visible = true;
        //lblsbar2.Visible = true;
        lblsbar2.Text = statusbar2;
        lblsbar1.Text = statusbar1 + " < " + mytime.ToString("h:mm:ss") + " >...";
        skWinLib.accesslog(skApplicationName, skapplicationVersion, Task, "Start ;" + Remark);
        Cursor.Current = Cursors.WaitCursor;
        lblsbar1.Refresh();
        lblsbar2.Refresh();
        return mytime;

    }

    public static void setCompletion(string skApplicationName, string skapplicationVersion, string Task, string Remark, string statusbar1, string statusbar2, Label lblsbar1, Label lblsbar2, DateTime startTime)
    {
        //12-apr-2022
        //lblsbar1.Visible = true;
        //lblsbar2.Visible = false;        
        skWinLib.worklog(skApplicationName, skapplicationVersion, Task, "Complete ;" + Remark);
        Cursor.Current = Cursors.Default;
        TimeSpan span = DateTime.Now.Subtract(startTime);
        lblsbar1.Text = "Ready." + span.Minutes.ToString() + "m " + span.Seconds.ToString() + "s ." + statusbar1;
        lblsbar2.Text = statusbar2;
        lblsbar1.Refresh();
        lblsbar2.Refresh();
    }

    public static System.Boolean IsNumeric(System.Object Expression)
    {
        if (Expression == null || Expression is DateTime)
            return false;

        if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is System.Boolean)
            return true;

        try
        {
            if (Expression is string)
                Double.Parse(Expression as string);
            else
                Double.Parse(Expression.ToString());
            return true;
        }
        catch { } // just dismiss errors but return false
        return false;
    }
    public static void SendEmail(string myapplication, string myversion, List<string> To_mails, List<string> CC_mails, string MailSubject, string MailBody)
    {
        //bool errflag = false;
        try
        {
            //SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = false;

            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = false;
            //mySmtpClient.d

            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            string From_mail = sklogin;
            //string CC_mail = "vijayakumar@esskaystructures.com";
            //string To_mail = "jagadeesh@esskaystructures.com";


            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
            MailMessage myMail = new MailMessage();
            myMail.From = from;
            MailAddress replyTo = new MailAddress(sklogin);
            myMail.ReplyToList.Add(replyTo);
            // List of "To" email addresses
            foreach (string email in To_mails)
            {
                myMail.To.Add(new MailAddress(email));
            }

            // List of "CC" email addresses
            foreach (string email in CC_mails)
            {
                myMail.CC.Add(new MailAddress(email));
            }

            //MailAddress to = new MailAddress(To_mail, To_mail);
            //MailAddress cc = new MailAddress(CC_mail, CC_mail);
            //MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            //add cc
            //myMail.CC.Add(cc);
            // add ReplyTo


            // set subject and encoding
            myMail.Subject = "Automatic Mail: " + MailSubject + " [ " + myapplication + " / " + myversion + " / " + skWinLib.systemname + " / " + skWinLib.username + " ]";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            myMail.Body = MailBody;
             // text or html
             myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            //errflag = true;            
            MessageBox.Show("SmtpException has occured@ SendEmail. Orginal Error \n\n" + ex.Message);
        }

    }
    public static void SendEmail(string myapplication, string myversion, List<string> To_mails, List<string> CC_mails, string MailSubject)
    {
        //bool errflag = false;
        try
        {
            //SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = false;

            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = false;
            //mySmtpClient.d

            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            string From_mail = sklogin;
            //string CC_mail = "vijayakumar@esskaystructures.com";
            //string To_mail = "jagadeesh@esskaystructures.com";


            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
            MailMessage myMail = new MailMessage();
            myMail.From = from;
            MailAddress replyTo = new MailAddress(sklogin);
            myMail.ReplyToList.Add(replyTo);
            // List of "To" email addresses
            foreach (string email in To_mails)
            {
                myMail.To.Add(new MailAddress(email));
            }

            // List of "CC" email addresses
            foreach (string email in CC_mails)
            {
                myMail.CC.Add(new MailAddress(email));
            }           

            //MailAddress to = new MailAddress(To_mail, To_mail);
            //MailAddress cc = new MailAddress(CC_mail, CC_mail);
            //MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            //add cc
            //myMail.CC.Add(cc);
            // add ReplyTo


            // set subject and encoding
            myMail.Subject = "Automatic Mail: " + MailSubject + " [ " + myapplication + " / " + myversion + " / " + skWinLib.systemname + " / " + skWinLib.username + " ]";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html
            myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            //errflag = true;            
            MessageBox.Show("SmtpException has occured@ SendEmail. Orginal Error \n\n" + ex.Message);
        }

    }
    public static void SendEmail(string myapplication, string myversion, string MailSubject)
    {
        //bool errflag = false;
        try
        {
            //SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = false;

            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = false;
            //mySmtpClient.d

            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            string From_mail = sklogin;
            string CC_mail = skWinLib.Email_Automation;// "vijayakumar@esskaystructures.com";
            string To_mail = skWinLib.Email_Automation; //"jagadeesh@esskaystructures.com";


            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
            MailAddress to = new MailAddress(To_mail, To_mail);
            MailAddress cc = new MailAddress(CC_mail, CC_mail);
            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            //add cc
            myMail.CC.Add(cc);
            // add ReplyTo
            MailAddress replyTo = new MailAddress(sklogin);
            myMail.ReplyToList.Add(replyTo);

            // set subject and encoding
            myMail.Subject = "Automatic Mail: " + MailSubject + " [ " + myapplication + " / " + myversion + " / " + skWinLib.systemname + " / " + skWinLib.username + " ]";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;           

            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html
            myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            //errflag = true;            
            MessageBox.Show("SmtpException has occured@ SendEmail. Orginal Error \n\n" + ex.Message);
        }

    }


    public static void SendEmail(string myversion, string modelname, string reportname, string skempname, string record, string process)
    {
        //bool errflag = false;
        try
        {
            //SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

            // set smtp-client with basicAuthentication
            mySmtpClient.UseDefaultCredentials = false;

            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = true;
            mySmtpClient.UseDefaultCredentials = false;
            //mySmtpClient.d

            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
            mySmtpClient.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            string From_mail = sklogin;
            string To_mail = "vijayakumar@esskaystructures.com";

            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
            MailAddress to = new MailAddress(To_mail, "QC");
            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            // add ReplyTo
            MailAddress replyTo = new MailAddress(sklogin);
            myMail.ReplyToList.Add(replyTo);

            string version_login_system_empname = myversion + " / " + skWinLib.systemname + " / " + skWinLib.username + " / " + skempname + " ]";
            // set subject and encoding
            myMail.Subject = "Automatic Mail: QC";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;
            // set body-message and encoding
            //myMail.Body = "<b>" + reportname + "</b><br>using <b>HTML</b>.";
            myMail.Body = "<p class=MsoNormal><span class=MsoIntenseEmphasis>Greetings,<o:p></o:p></span></p> " +
                //"<p class=MsoNormal><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
                "<p class=MsoNormal><span class=MsoIntenseEmphasis>This is system generated mail for QC-Tool [ " + version_login_system_empname + "<o:p></o:p></span></p> " +
                "<p class=MsoNormal style = 'text-align:justify'><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
"<div style = 'mso-element:para-border-div;border-top:solid #5B9BD5 1.0pt; " +
"mso -border-top-themecolor:accent1;border-left:none;border-bottom:solid #5B9BD5 1.0pt; " +
"mso -border-bottom-themecolor:accent1;border-right:none;mso-border-top-alt:solid #5B9BD5 .5pt; " +
"mso -border-top-themecolor:accent1;mso-border-bottom-alt:solid #5B9BD5 .5pt; " +
"mso -border-bottom-themecolor:accent1;padding:10.0pt 0cm 10.0pt 0cm;margin-left:43.2pt;margin-right:43.2pt'> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>Model<span style='mso-tab-count:1'>  " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + modelname + "<o:p></o:p></span></span></p> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>Report<span style='mso-tab-count:1'> " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + reportname + "<o:p></o:p></span></span></p> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>Record(s)<span style='mso-tab-count:1'> " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + record + "<o:p></o:p></span></span></p> " +

"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
"style='font-style:normal'>Process<span style='mso-tab-count:1'> " +
"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + process + "<o:p></o:p></span></span></p> " +

"</div> " +

"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Regards,<o:p></o:p></span></p> " +
"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Esskay Automation Team.<o:p></o:p></span></p> " +
"<p class=MsoNormal><o:p>&nbsp;</o:p></p> ";
//"<!DOCTYPE html>\r\n<html lang=\"en\" xmlns:th=\"http://www.thymeleaf.org\">\r\n<head>\r\n<meta charset=\"UTF-8\" />\r\n<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />\r\n<title>ESSKAY MANAGEMENT SYSTEM (EMS)</title>\r\n<style>\r\n      body {\r\n        margin: 0;\r\n        padding: 0;\r\n        width: 100%;\r\n        font-family: \"Calibri\", Arial, sans-serif;\r\n      }\r\n      .logo-container {\r\n        text-align: center;\r\n        margin: 10px 0;\r\n      }\r\n      table {\r\n        width: 100%;\r\n        border-collapse: collapse;\r\n        font-size: 12px;\r\n        overflow: auto;\r\n      }\r\n \r\n      table thead th {\r\n        background-color: rgb(69, 155, 69);\r\n        color: white;\r\n      }\r\n      td {\r\n        padding: 8px;\r\n        text-align: left;\r\n      }\r\n</style>\r\n</head>\r\n<body>\r\n<div class=\"logo-container\">\r\n<img th:src=\"@{cid:logoImage}\" alt=\"Logo\" />\r\n</div>\r\n<p\r\n      th:text=\"'The following items have Created / Completed in EMS for your further action'\"\r\n></p>\r\n<div style=\"overflow-x:auto;\">\r\n<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\">\r\n<thead>\r\n<tr>\r\n<th>Sk Project #</th>\r\n<th>Project Name</th>\r\n<th>Client Name</th>\r\n<th>Client Project #</th>\r\n<th>Branch</th>\r\n<th>Project Manager</th>\r\n<th>WBS</th>\r\n<th>SEQ#</th>\r\n<th>Actual Date</th>\r\n<th>Status</th>\r\n<th>Remarks</th>\r\n</tr>\r\n</thead>\r\n<tbody>\r\n<!-- Loop through the list of data -->\r\n<tr th:each=\"item : ${dataList}\">\r\n<td th:text=\"${item.projectNumber}\"></td>\r\n<td th:text=\"${item.projectName}\"></td>\r\n<td th:text=\"${item.clientName}\"></td>\r\n<td th:text=\"${item.clientNumber}\"></td>\r\n<td th:text=\"${item.branch}\"></td>\r\n<td th:text=\"${item.projectManage}\"></td>\r\n<td th:text=\"${item.wbs}\"></td>\r\n<td th:text=\"${item.sequence}\"></td>\r\n<td th:text=\"${item.actualDate}\"></td>\r\n<td th:text=\"${item.status}\"></td>\r\n<td th:text=\"${item.remarks}\"></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</div>\r\n<div th:utext=\"${bodyContent}\"></div>\r\n<p th:utext=\"'Thanks and Regards,<br/>' + ${regards}\"></p>\r\n</body>\r\n</html>";

            //"</div> " +
            //"</body>";

            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html
            myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            //errflag = true;            
            MessageBox.Show("SmtpException has occured@ SendEmail. Orginal Error \n\n" + ex.Message);
        }

    }

    //    public static void SendEmail(string myversion, string modelname, string reportname, string skempname, string record, string process)
    //    {
    //        //bool errflag = false;
    //        try
    //        {
    //            //SmtpClient mySmtpClient = new SmtpClient("smtp.1and1.com");
    //            SmtpClient mySmtpClient = new SmtpClient("smtp.office365.com");

    //            // set smtp-client with basicAuthentication
    //            mySmtpClient.UseDefaultCredentials = false;

    //            mySmtpClient.Port = 587;
    //            mySmtpClient.EnableSsl = true;
    //            mySmtpClient.UseDefaultCredentials = false;
    //            //mySmtpClient.d

    //            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(sklogin, skpassword);
    //            mySmtpClient.Credentials = basicAuthenticationInfo;

    //            // add from,to mailaddresses
    //            string From_mail = sklogin;
    //            string To_mail = "vijayakumar@esskaystructures.com";

    //            MailAddress from = new MailAddress(From_mail, "Esskay Automation");
    //            MailAddress to = new MailAddress(To_mail, "QC");
    //            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

    //            // add ReplyTo
    //            MailAddress replyTo = new MailAddress(sklogin);
    //            myMail.ReplyToList.Add(replyTo);

    //            string version_login_system_empname = myversion + " / " + skWinLib.systemname + " / " + skWinLib.username + " / " + skempname + " ]";
    //            // set subject and encoding
    //            myMail.Subject = "Automatic Mail: QC";
    //            myMail.SubjectEncoding = System.Text.Encoding.UTF8;
    //            // set body-message and encoding
    //            //myMail.Body = "<b>" + reportname + "</b><br>using <b>HTML</b>.";
    //            myMail.Body = "<p class=MsoNormal><span class=MsoIntenseEmphasis>Greetings,<o:p></o:p></span></p> " +
    //                //"<p class=MsoNormal><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
    //                "<p class=MsoNormal><span class=MsoIntenseEmphasis>This is system generated mail for QC-Tool [ " + version_login_system_empname + "<o:p></o:p></span></p> " +
    //                "<p class=MsoNormal style = 'text-align:justify'><span class=MsoIntenseEmphasis><o:p>&nbsp;</o:p></span></p> " +
    //"<div style = 'mso-element:para-border-div;border-top:solid #5B9BD5 1.0pt; " +
    //"mso -border-top-themecolor:accent1;border-left:none;border-bottom:solid #5B9BD5 1.0pt; " +
    //"mso -border-bottom-themecolor:accent1;border-right:none;mso-border-top-alt:solid #5B9BD5 .5pt; " +
    //"mso -border-top-themecolor:accent1;mso-border-bottom-alt:solid #5B9BD5 .5pt; " +
    //"mso -border-bottom-themecolor:accent1;padding:10.0pt 0cm 10.0pt 0cm;margin-left:43.2pt;margin-right:43.2pt'> " +

    //"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
    //"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
    //"style='font-style:normal'>Model<span style='mso-tab-count:1'>  " +
    //"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + modelname + "<o:p></o:p></span></span></p> " +

    //"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
    //"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
    //"style='font-style:normal'>Report<span style='mso-tab-count:1'> " +
    //"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + reportname + "<o:p></o:p></span></span></p> " +

    //"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
    //"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
    //"style='font-style:normal'>Record(s)<span style='mso-tab-count:1'> " +
    //"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + record + "<o:p></o:p></span></span></p> " +

    //"<p class=MsoIntenseQuote style = 'margin-top:18.0pt;margin-right:0cm;margin-bottom: " +
    //"18.0pt;margin-left:0cm;text-align:justify'><span class=MsoIntenseEmphasis><span " +
    //"style='font-style:normal'>Process<span style='mso-tab-count:1'> " +
    //"       </span>:<span style = 'mso-tab-count:1' > </ span >  " + process + "<o:p></o:p></span></span></p> " +

    //"</div> " +

    //"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Regards,<o:p></o:p></span></p> " +
    //"<p class=MsoNormal style = 'text-align:justify' ><span class=MsoIntenseEmphasis>Esskay Automation Team.<o:p></o:p></span></p> " +
    //"<p class=MsoNormal><o:p>&nbsp;</o:p></p> ";
    //            //"</div> " +
    //            //"</body>";

    //            myMail.BodyEncoding = System.Text.Encoding.UTF8;
    //            // text or html
    //            myMail.IsBodyHtml = true;

    //            mySmtpClient.Send(myMail);
    //        }
    //        catch (SmtpException ex)
    //        {
    //            //errflag = true;            
    //            MessageBox.Show("SmtpException has occured: " + ex.Message);
    //        }

    //    }

    public static System.Double GetNumeric(System.Object Expression)
    {
        if (Expression == null || Expression is DateTime)
            return -1;

        if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is System.Boolean)
            return Double.Parse(Expression.ToString());


        try
        {
            if (Expression is string)
            {
                string str_double = string.Empty;
                string chkstring = Expression.ToString();
                foreach (char c in chkstring)
                {
                    if ((int)c >= 48 && (int)c <= 57)
                        str_double = str_double + c.ToString();

                }
                if (str_double != string.Empty)
                    return Double.Parse(str_double);
                else
                    return -1;
            }
            return Double.Parse(Expression.ToString());


        }
        catch { } // just dismiss errors but return false


        return -1;
    }

    public static ArrayList GetActiveTeklaModels()
    {
        //230419 1732
        //230417 1914
        ArrayList TSModelList = new ArrayList();
        Process[] TSprocesslist = Process.GetProcessesByName("TeklaStructures");
        foreach (Process CurrentProcess in TSprocesslist)
        {
            //if (CurrentProcess.MainWindowTitle.Trim().Length >=1)
            {
                //CurrentProcess.MainWindowTitle is valid 
                //get TeklaStructures Title
                //TMSL_Log("CurrentProcess.MainWindowTitle: " + CurrentProcess.MainWindowTitle);
                //get CurrentProcessPath
                string ProcessPath = GetCurrentProcessPath(CurrentProcess.MainWindowTitle);

                ////get TeklaStructures Title
                //TMSL_Log("ProcessPath: " + ProcessPath);
                //string ttt = CurrentProcess.MainModule.FileName;
                ArrayList Mydata = new ArrayList();
                Mydata.Add(ProcessPath);
                Mydata.Add(CurrentProcess.StartTime);
                Mydata.Add(CurrentProcess.Id);


                //store model information
                TSModelList.Add(Mydata);
            }


        }
        return TSModelList;

    }
    public static string GetPathName(string PathFileName)
    {
        //if (System.IO.File.Exists(PathFileName) == true)
        if (PathFileName.Contains('\\') == true)
        {
            string filename = GetFileName(PathFileName);
            if (filename.Length >= 1)
                return PathFileName.Substring(0, PathFileName.Length - filename.Length);

        }
        return string.Empty;
    }
    public static string GetFileName(string PathFileName)
    {
        //if (System.IO.File.Exists(PathFileName) == true)
        if (PathFileName.Contains('\\') == true)
        {
            string[] chkspltpath = PathFileName.Split('\\');
            if (chkspltpath.Length >= 1)
                return chkspltpath[chkspltpath.Length - 1];

        }
        return string.Empty;
    }

    public static string GetCurrentProcessPath(string MainWindowTitle)
    {
        //230424 1412
        string modelpath = MainWindowTitle;
        if (modelpath.Trim().Length >= 1)
        {
            int id = MainWindowTitle.IndexOf(':');
            string chkpath = string.Empty;

            //check1 based on Drive : since this must exists
            //Tekla Structures - D:\\Vijayakumar\\02_Model\\2021\\Single Part POC  - A   [C3]"
            if (id >= 2)
            {
                modelpath = MainWindowTitle.Substring(id - 1);
                //Check1
                int idx = modelpath.IndexOf(" - ");
                if (idx >= 0)
                {
                    chkpath = modelpath.Substring(0, idx).Trim();
                    if (System.IO.Directory.Exists(chkpath) == true)
                    {
                        string[] chkspltpath = chkpath.Split('\\');
                        if (chkspltpath.Length >= 1)
                        {
                            string model_name = chkspltpath[chkspltpath.Length - 1];
                            string model_dbfile = chkpath + @"\" + model_name + ".db1";
                            bool modelflag = System.IO.File.Exists(model_dbfile);

                            if (modelflag == true)
                                return chkpath;
                        }

                    }

                }

                //Check2
                string[] spltpath = modelpath.Split('\\');
                foreach (string mypath in spltpath)
                {
                    string myfolder = string.Empty;
                    foreach (char mychar in mypath)
                    {
                        myfolder = myfolder + mychar;
                        string model_dbfile = chkpath + @"\" + myfolder + @"\" + myfolder + ".db1";
                        bool modelflag = System.IO.File.Exists(model_dbfile);

                        if (modelflag == true)
                            return chkpath + @"\" + myfolder;

                    }
                    //set the path for checking
                    if (chkpath == string.Empty)
                        chkpath = mypath;
                    else
                        chkpath = chkpath + @"\" + mypath;

                }

            }
            //else
            //{
            //    CreateErrorLog("':' Missing Check:", MainWindowTitle);
            //}
            ////ignore for cancel since tekla model is closing
            ////if ((MainWindowTitle.ToUpper().Contains("CANCEL") == false)) //&& (MainWindowTitle.Trim().Length >= 3)
            //if ((MainWindowTitle.ToUpper().Trim() != ("Tekla Structures - Cancel".ToUpper().Trim())) && (MainWindowTitle.ToUpper().Trim() != ("Tekla Structures ".ToUpper().Trim())))
            //    CreateErrorLog("GetCurrentProcessPath:", MainWindowTitle);
        }
        return modelpath;
    }


    //this function is used to check whether the L & T drive is connected or not
    public bool check_TL_drive()
    {
        bool check = true;
        try
        {
            bool checkT = System.IO.Directory.Exists("T:\\");
            bool checkL = System.IO.Directory.Exists("L:\\");

            if (checkT == true && checkL == true)
            {
                check = true;
            }
            else
            {
                check = false;
            }

        }
        catch
        {
            check = false;
        }

        return check;

    }
    public enum AssemblyEnum
    {
        Guid = 0,
        Name,
        Position,
        Quantity,
        Profile_imp,
        Material,
        Finish,
        PhaseNumber,
        PhaseName,
        Width_ftfrac,
        Height_ftfrac,
        Length_ftfrac,
        NetWeight_lbs,
        GrossWeight_lbs,
        Area_sqft,
        Profile_mm,
        Width_mm,
        Height_mm,
        Length_mm,
        NetWeight_kg,
        GrossWeight_kg,
        Area_mm2,
        User_Field_1,
        User_Field_2,
        User_Field_3,
        User_Field_4,
        User_Field_Comment,
        User_Field_Phase,
        User_Field_Issue
    }
    //public enum AssemblyEnum
    //{
    //    guid = 0,
    //    AssemblyPosition,
    //    Profile,
    //    Material,
    //    Finish,
    //    Name,
    //    PhaseNumber,
    //    PhaseName,
    //    NetLength,
    //    NetWeight,
    //    GrossLength,
    //    GrossWeight,
    //    Quanity,
    //    UDA,
    //    Remark


    //}
    public class AssemblyData
    {
        public string guid { get; set; }
        public string Position { get; set; }
        public string Profile_imp { get; set; }
        public string Profile_mm { get; set; }
        public string Material { get; set; }
        public string Finish { get; set; }
        public string Name { get; set; }
        public int PhaseNumber { get; set; }
        public string PhaseName { get; set; }
        public string Length_ftfrac { get; set; }
        public double NetWeight_lbs { get; set; }
        public double GrossWeight_lbs { get; set; }
        public double Length_mm { get; set; }
        public double NetWeight_kg { get; set; }
        public double GrossWeight_kg { get; set; }
        public int Quanity { get; set; }
        public string UDA { get; set; }
        public string Remark { get; set; }

    }
    //public static List<AssemblyData> GetSKRFIList(string filename)
    //{
    //    List<AssemblyData> MyAssemblyData = new List<AssemblyData>();
    //    try
    //    {
    //        //foreach (DataGridViewRow MyRow in MyDataGridView.Rows)
    //        //{
    //        //    string RFI_ID = string.Empty;
    //        //    string Client_RFI_ID = string.Empty;
    //        //    string Description = string.Empty;
    //        //    string Reply = string.Empty;
    //        //    bool Status = false;
    //        //    string guid = string.Empty;

    //        //    AssemblyData MyAssemblyData = new AssemblyData();
    //        //    if (MyRow.Cells["rfiregskno"].Value != null)
    //        //        RFI_ID = MyRow.Cells["rfiregskno"].Value.ToString();
    //        //    if (MyRow.Cells["rfiregclientno"].Value != null)
    //        //        Client_RFI_ID = MyRow.Cells["rfiregclientno"].Value.ToString();
    //        //    if (MyRow.Cells["rfiregdesc"].Value != null)
    //        //        Description = MyRow.Cells["rfiregdesc"].Value.ToString();
    //        //    if (MyRow.Cells["rfiregans"].Value != null)
    //        //        Reply = MyRow.Cells["rfiregans"].Value.ToString();
    //        //    if (MyRow.Cells["rfimdlguid"].Value != null)
    //        //        guid = MyRow.Cells["rfimdlguid"].Value.ToString();

    //        //    if (MyRow.Cells["rfiregstatus"].Value != null)
    //        //    {
    //        //        if (MyRow.Cells["rfiregstatus"].Value.ToString().Trim().ToUpper() != "FALSE")
    //        //            Status = true;
    //        //    }


    //        //    MySKRFI.RFI_ID = RFI_ID;
    //        //    MySKRFI.Client_RFI_ID = Client_RFI_ID;
    //        //    MySKRFI.Description = Description;
    //        //    MySKRFI.Client_Reply = Reply;
    //        //    MySKRFI.Status = Status;
    //        //    MySKRFI.guid = guid;
    //        //    SKRFIList.Add(MySKRFI);


    //        //}


    //    }
    //    catch (Exception ex)
    //    {
    //        //MessageBox.Show("Please Check the File selected");
    //    }

    //    return MyAssemblyData;
    //}

    public class SKRFI
    {
        public string RFI_ID { get; set; }
        public string Client_RFI_ID { get; set; }
        public string Description { get; set; }
        public string Client_Reply { get; set; }
        public bool Status { get; set; }
        public string guid { get; set; }

    }



    public static List<SKRFI> GetSKRFIList(DataGridView MyDataGridView)
    {
        List<SKRFI> SKRFIList = new List<SKRFI>();
        try
        {
            foreach (DataGridViewRow MyRow in MyDataGridView.Rows)
            {
                string RFI_ID = string.Empty;
                string Client_RFI_ID = string.Empty;
                string Description = string.Empty;
                string Reply = string.Empty;
                bool Status = false;
                string guid = string.Empty;

                SKRFI MySKRFI = new SKRFI();
                if (MyRow.Cells["rfiregskno"].Value != null)
                    RFI_ID = MyRow.Cells["rfiregskno"].Value.ToString();
                if (MyRow.Cells["rfiregclientno"].Value != null)
                    Client_RFI_ID = MyRow.Cells["rfiregclientno"].Value.ToString();
                if (MyRow.Cells["rfiregdesc"].Value != null)
                    Description = MyRow.Cells["rfiregdesc"].Value.ToString();
                if (MyRow.Cells["rfiregans"].Value != null)
                    Reply = MyRow.Cells["rfiregans"].Value.ToString();

                //if (MyRow.Cells["rfimdlguid"].Value != null)
                //    guid = MyRow.Cells["rfimdlguid"].Value.ToString();

                if (MyRow.Cells["rfiregstatus"].Value != null)
                {
                    if (MyRow.Cells["rfiregstatus"].Value.ToString().Trim().ToUpper() != "FALSE")
                        Status = true;
                }


                MySKRFI.RFI_ID = RFI_ID;
                MySKRFI.Client_RFI_ID = Client_RFI_ID;
                MySKRFI.Description = Description;
                MySKRFI.Client_Reply = Reply;
                MySKRFI.Status = Status;
                //MySKRFI.guid = guid;
                SKRFIList.Add(MySKRFI);


            }


        }
        catch (Exception ex)
        {
            string tmp = ex.ToString();
            //MessageBox.Show("Please Check the File selected");
        }

        return SKRFIList;
    }
    public static List<SKRFI> GetRFIDetails(List<SKRFI> RFI_Data, string RFINumbers, string guid)
    {
        List<SKRFI> SKRFIList = new List<SKRFI>();

        try
        {
            if (RFINumbers.IndexOf(',') >= 0)
            {
                string[] spltSK_RFI = RFINumbers.Split(',');
                foreach (string RFI in spltSK_RFI)
                {
                    SKRFI MySKRFI = new SKRFI();
                    MySKRFI = RFI_Data.FirstOrDefault(i => i.RFI_ID == RFI);
                    if (MySKRFI != null)
                    {
                        //MySKRFI.guid = guid;
                        SKRFIList.Add(MySKRFI);

                    }

                }
            }
            else
            {
                SKRFI MySKRFI = new SKRFI();
                MySKRFI = RFI_Data.FirstOrDefault(i => i.RFI_ID == RFINumbers);
                if (MySKRFI != null)
                {
                    //MySKRFI.guid = guid;
                    SKRFIList.Add(MySKRFI);

                }
            }

        }
        catch (Exception ex)
        {
            string tmp = ex.ToString();
            //MessageBox.Show("Please Check the File selected");
        }

        return SKRFIList;
    }
    public static List<SKRFI> GetRFIDetails(List<SKRFI> RFI_Data, string RFINumbers)
    {
        List<SKRFI> SKRFIList = new List<SKRFI>();
        SKRFI MySKRFI = new SKRFI();
        try
        {
            if (RFINumbers.IndexOf(',') >= 0)
            {
                string[] spltSK_RFI = RFINumbers.Split(',');
                foreach (string RFI in spltSK_RFI)
                {
                    MySKRFI = RFI_Data.FirstOrDefault(i => i.RFI_ID == RFI);
                    if (MySKRFI != null)
                        SKRFIList.Add(MySKRFI);

                }
            }
            else
            {

                MySKRFI = RFI_Data.FirstOrDefault(i => i.RFI_ID == RFINumbers);
                if (MySKRFI != null)
                    SKRFIList.Add(MySKRFI);
            }

        }
        catch (Exception ex)
        {
            string tmp = ex.ToString();
            //MessageBox.Show("Please Check the File selected");
        }

        return SKRFIList;
    }
    public static void CreateDirectoryIfNotExsists(string directoryPath)
    {
        if (System.IO.Directory.Exists(directoryPath) == false)
            System.IO.Directory.CreateDirectory(directoryPath);
    }

    private static readonly byte[] sk = Encoding.UTF8.GetBytes("Essk@y2010360%XS");
    private static readonly byte[] ks = { 0x01, 0x02, 0x05, 0x16, 0x15, 0x12, 0x08, 0x07, 0x02, 0x04, 0x03, 0x14, 0x12, 0x13, 0x11, 0x02 };
    public static string kuriyaakku(string ulleedu, int flag = 128)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = sk;
            aesAlg.IV = ks;


            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(ulleedu);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string maraikuriyaakkam(string ulleedu, int flag = 16)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = sk;
            aesAlg.IV = ks;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(ulleedu)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    public static string UpdateHeaderInformation(string skApplicationName, string skApplicationVersion, string mdlname, int LoadCount = 0, int selectcount = 0, int updatecount = 0, int errorcount = 0, int processcount = 0)
    {

        int e = UpdateUsageCount(skApplicationName, "Error", errorcount);
        string MyUsage = "E:" + GetAbbCount(e);

        int s = UpdateUsageCount(skApplicationName, "Select", selectcount);
        MyUsage = MyUsage + " S:" + GetAbbCount(s);

        int p = UpdateUsageCount(skApplicationName, "Process", processcount);
        MyUsage = MyUsage + " S:" + GetAbbCount(p);

        int a = UpdateUsageCount(skApplicationName, "Load", LoadCount);
        int y = UpdateUsageCount(skApplicationName, "Update", updatecount);

        MyUsage = MyUsage + " K:" + GetAbbCount(e + s + p + a + y) + " A:" + GetAbbCount(a) + " Y:" + GetAbbCount(y);

        if (mdlname != string.Empty)
            return skApplicationName + " Ver. " + skApplicationVersion + " [" + MyUsage + "] <" + TSVersion + " / " + mdlname.Replace(".DB1", "").Replace(".db1", "") + ">";
        else
            return skApplicationName + " Ver. " + skApplicationVersion + " [" + MyUsage + "]";
    }
    private static string GetAbbCount (int Count)
    {
        int result = 0;
        string abb = string.Empty;

        if (Count > 1000000000)
        {
            result = Count / 1000000000;
            int div = Count % 10;
            abb = "HC*" + div;
        }
        else if (Count > 100000000)
        {
            result = Count / 100000000;
            int div = Count % 10;
            abb = "TC*" + div;
        }
        else if (Count > 10000000)
        {
            result = Count / 10000000;
            int div = Count % 100;
            abb = "C*" + div;
        }
        else if (Count > 1000000)
        {
            result = Count / 1000000;
            int div = Count % 10;
            abb = "TL*" + div; ;
        }
        else if (Count > 100000)
        {
            result = Count / 100000;
            int div = Count % 100;
            abb = "L*" + div;
        }
        else if (Count > 10000)
        {
            result = Count / 10000;
            int div = Count % 10;
            abb = "TT*" + div;
        }
        else if (Count > 1000)
        {
            result = Count / 1000;
            abb = "T";
        }
        else if (Count > 100)
        {
            result = Count / 100;
            abb = "H";
        }
        else if (Count >= 0)
        {
            return Count.ToString();
        }
        else
        {
            return "i!!!";
        }
        return result + abb + "+";
    }
    //public static string UpdateHeaderInformation(string skApplicationName, string skApplicationVersion, string mdlname, int LoadCount = 0, int selectcount = 0, int updatecount = 0, int errorcount = 0, int processcount = 0)
    //{



    //    int e = UpdateUsageCount("Error", errorcount);
    //    string MyUsage = "E:" + e;

    //    int s = UpdateUsageCount("Select", selectcount);
    //    MyUsage = MyUsage + " S:" + s;

    //    int p = UpdateUsageCount("Process", processcount);
    //    MyUsage = MyUsage + " S:" + p;

    //    int a = UpdateUsageCount("Load", LoadCount);
    //    int y = UpdateUsageCount("Update", updatecount);

    //    MyUsage = MyUsage + " K:" + (e + s + p + a + y) + " A:" + a + " Y:" + y;

    //    string mdlname = MyModel.GetInfo().ModelName;
    //    string myreturn = skApplicationName + " Ver. " + skApplicationVersion + " [" + MyUsage + "]<" + skWinLib.TSVersion + " / " + mdlname.Replace(".DB1", "").Replace(".db1", "") + ">";
    //    return myreturn;

    //}
    public static int UpdateUsageCount(string skApplicationName, string Key, int Count)
    {
        int usgcount = 0;

        //Get Last User Usage Count
        string KeyValue = skWinLib.GetRegistryValue(skApplicationName, "cT" + Key);
        if (KeyValue != string.Empty && KeyValue != null)
            int.TryParse(KeyValue, out usgcount);


        usgcount = usgcount + Count;

        if (usgcount >= 1)
        {
            //Update User Usage Count
            skWinLib.SetRegistryValue(skApplicationName, "cT" + Key, usgcount.ToString());
        }

        return usgcount;

    }

    //Use PDF sharp library to combine after all the individual drawings are created.
    public static void MergePDFs(string targetPath, params string[] pdfs)
    {
        //using (PdfDocument targetDoc = new PdfDocument())
        //{
        //    foreach (string pdf in pdfs)
        //    {
        //        using (PdfDocument pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
        //        {
        //            for (int i = 0; i < pdfDoc.PageCount; i++)
        //            {
        //                targetDoc.AddPage(pdfDoc.Pages[i]);
        //            }
        //        }
        //    }
        //    if (targetDoc.PageCount > 0)
        //        targetDoc.Save(targetPath);
        //}
    }

    //public static int GetColumnIndexByName(DataGridView dgv, string columnName)
    //{
    //    foreach (DataGridViewColumn column in dgv.Columns)
    //    {
    //        if (column.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return column.Index;
    //        }
    //    }
    //    return -1; // Return -1 if the column is not found
    //}
    public static int GetColumnIndexByName(DataGridView dgv, string columnName)
    {
        // Use LINQ to find the index of the column with the name "GUID"
        int guidColumnIndex = dgv.Columns
            .Cast<DataGridViewColumn>()
            .Select((column, index) => new { column, index })
            .Where(x => x.column.Name == columnName)
            .Select(x => x.index)
            .FirstOrDefault();
        return guidColumnIndex; // Return -1 if the column is not found
    }


    public class SKEmployeeData
    {
        public int dbid { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Branch { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }        

    }

    public static List<SKEmployeeData> MyEmployeeList = new List<SKEmployeeData>();
    public static void GetEmployeeData()
    {

        try
        {
            //Clear
            MyEmployeeList.Clear();

            //fetch Employee information and store in empdata;
            empdata.Clear();

            mysql = "SELECT id, branch_id, employee_code, first_name, middle_name, last_name, user_name, login_name, email FROM `employee` where is_active = 1 ORDER BY id";// UPPER(employee_code) = ('" + LoginName + "') or UPPER(REPLACE(login_name, ' ', '')) = UPPER('" + LoginName + "')";
            myconn = new MySqlConnection(skWinLib.connString);
            if (myconn.State == ConnectionState.Closed)
                myconn.Open();
            mycmd.CommandText = mysql;
            mycmd.Connection = myconn;
            myrdr = mycmd.ExecuteReader();

            while (myrdr.Read())
            {
                SKEmployeeData MyData = new SKEmployeeData();
                MyData.dbid =  Convert.ToInt32(myrdr["id"].ToString());
                MyData.Branch = Convert.ToInt32(myrdr["branch_id"].ToString());
                MyData.Code = myrdr["employee_code"].ToString();
                MyData.FirstName = myrdr["first_name"].ToString();
                MyData.MiddleName = myrdr["middle_name"].ToString();
                MyData.LastName = myrdr["last_name"].ToString();
                MyData.Login = myrdr["login_name"].ToString(); //VijayaKumar P
                MyData.Email = myrdr["email"].ToString(); //change this in future
                MyEmployeeList.Add(MyData);

                //159|SKS360|VIJAYAKUMAR P|vijayakumar@esskaystructures.com
                empdata.Add(MyData.dbid + "|" + MyData.Code + "|" + MyData.FirstName + "" + MyData.MiddleName + "" + MyData.LastName + ""  + "|" + MyData.Email);
            }
            myrdr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static List<string> GetEmployeeData(string LoginName)
    {
        List<string> MyData = new List<string>();
        try
        {
            mysql = "SELECT id, branch_id, employee_code, first_name, middle_name, last_name, user_name, login_name, email FROM `employee` where UPPER(employee_code) = ('" + LoginName + "') or UPPER(REPLACE(login_name, ' ', '')) = UPPER('" + LoginName + "') and is_active = 1 ORDER BY id";
            myconn = new MySqlConnection(skWinLib.connString);         
            if (myconn.State == ConnectionState.Closed)
                myconn.Open();
            mycmd.CommandText = mysql;
            mycmd.Connection = myconn;
            myrdr = mycmd.ExecuteReader();

            while (myrdr.Read())
            {
                string skempcode = myrdr["employee_code"].ToString(); //SKS360

                string FirstName = myrdr["first_name"].ToString();
                string MiddleName = myrdr["middle_name"].ToString();
                string LastName = myrdr["last_name"].ToString();

                string skempname = FirstName + MiddleName + LastName;
                string skid = myrdr["id"].ToString(); // 159
                string skemail = myrdr["email"].ToString();
                string branch_id = myrdr["branch_id"].ToString();
                MyData.Add(skempcode);
                MyData.Add(skempname);
                MyData.Add(skid);
                MyData.Add(skemail);
                if (branch_id == "1")
                    MyData.Add("Chennai");
                else if(branch_id == "2")
                    MyData.Add("Trichy");
                else
                    MyData.Add(branch_id);

            }
            myrdr.Close();
        }
        catch (Exception ex)
        {
            myrdr.Close();
            Console.WriteLine(ex.ToString());
        }
        return MyData;
    }

    public static string GetSKSCode(string LoginName)
    {
        string MyData = string.Empty;
        try
        {
            mysql = "SELECT id, employee_code FROM `employee` where UPPER(employee_code) = ('" + LoginName + "') or UPPER(REPLACE(login_name, ' ', '')) = UPPER('" + LoginName + "') and is_active = 1 ORDER BY id";
            myconn = new MySqlConnection(skWinLib.connString);
            if (myconn.State == ConnectionState.Closed)
                myconn.Open();
            mycmd.CommandText = mysql;
            mycmd.Connection = myconn;
            myrdr = mycmd.ExecuteReader();

            while (myrdr.Read())
            {
                MyData = myrdr["id"].ToString(); //159
                MyData =  myrdr["employee_code"].ToString(); //SKS360
            }
            myrdr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return MyData;
    }

    //get EmployeeData based on LoginName / System Name
    //public static List<string> GetEmployeeData(string LoginName)
    //{
    //    List<string> MyData = new List<string>();
    //    try
    //    {
    //        //fetch Employee information and store in empdata;
    //        empdata.Clear();
    //        // toolusers.id,toolusers.username,toolusers.name,toolusers.email
    //        mysql = "SELECT id,employee_code,login_name,email FROM `employee` where UPPER(employee_code) = ('" + LoginName + "') or UPPER(REPLACE(login_name, ' ', '')) = UPPER('" + LoginName + "')";
    //        myconn = new MySqlConnection(skWinLib.connString);
    //        if (myconn.State == ConnectionState.Closed)
    //            myconn.Open();
    //        mycmd.CommandText = mysql;
    //        mycmd.Connection = myconn;
    //        myrdr = mycmd.ExecuteReader();

    //        while (myrdr.Read())
    //        {
    //            string skempcode = myrdr["employee_code"].ToString(); //SKS360
    //            string skempname = myrdr["login_name"].ToString(); //Vijayakumar P
    //            string skid = myrdr["id"].ToString(); // 155
    //            string skempid = myrdr["employee_id"].ToString(); // 159
    //            string skemail = string.Empty;
    //            MyData.Add(skempcode);
    //            MyData.Add(skempname);
    //            MyData.Add(skempid);
    //            MyData.Add(skemail);
    //            MyData.Add(skid);

    //            //SKS360| VIJAYAKUMAR P|297|vijayakumar@esskaystructures.com
    //            empdata.Add(skempcode + "|" + skempname + "|" + skempid + "|" + skemail + "|" + skid);

    //        }
    //        myrdr.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.ToString());
    //    }
    //    return MyData;
    //}

    //get ProjectData based on ProjectName
    public static List<string> GetProjectData(string SKProjectNumber) //SK#1889
    {
        List<string> MyData = new List<string>();
        try
        {
            //fetch Employee information and store in projectdata;
            projectdata.Clear();

            mysql = "select id, client_job_number, job_name from project where ess_kay_job_number = '" + SKProjectNumber + "'";
            myconn = new MySqlConnection(skWinLib.connString);
            if (myconn.State == ConnectionState.Closed)
                myconn.Open();
            mycmd.CommandText = mysql;
            mycmd.Connection = myconn;
            myrdr = mycmd.ExecuteReader();
            while (myrdr.Read())
            {

                string client_job_number = myrdr["client_job_number"].ToString(); //SKS360
                string job_name = myrdr["job_name"].ToString(); //Vijayakumar P
                string id = myrdr["id"].ToString(); //

                MyData.Add(id);
                MyData.Add(string.Empty); //Lipart Steel Future get the Client name using JOIN query
                MyData.Add(SKProjectNumber); //SK#1498
                MyData.Add(client_job_number); //19-04
                MyData.Add(job_name); //American Saving Bank 83296



                projectdata.Add(id); //86
                projectdata.Add(string.Empty); //Lipart Steel Future get the Client name using JOIN query
                projectdata.Add(SKProjectNumber);  //SK#1889
                projectdata.Add(client_job_number); //24-1097
                projectdata.Add(job_name); //S01-Second Avenue Over M-10
            }
            myrdr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return MyData;
    }

    public static List<string> GetEmployeeProjectData(int empid) //SK#1889
    {
        List<string> MyData = new List<string>();
        try
        {
            //fetch Employee information and store in projectdata;
            projectdata.Clear();

            mysql = "SELECT p.id AS project_id, CONCAT(p.ess_kay_job_number,'-',p.job_name) AS project_name FROM project p JOIN project_subgroup_team pst ON p.id = pst.project_id JOIN employee_team_mapping etm ON pst.team_id = etm.team_id JOIN employee e ON etm.employee_code = e.id WHERE e.id = " + empid + " ORDER BY project_name";
            myconn = new MySqlConnection(skWinLib.connString);
            if (myconn.State == ConnectionState.Closed)
                myconn.Open();
            mycmd.CommandText = mysql;
            mycmd.Connection = myconn;
            myrdr = mycmd.ExecuteReader();
            while (myrdr.Read())
            {
                string project_id = myrdr[0].ToString(); //Project ID
                //string project_id = myrdr["id"].ToString(); //Project ID
                MyData.Add(project_id); //Project ID
            }
            myrdr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return MyData;
    }

    public static long CreateModelSharingLicenseRequest(string trimbleid, string licenserequest)
    {


        skWinLib.mycmd.CommandText = "INSERT INTO xsmodelsharing_lic_mgmt(sysname,login,trimbleid,startdatetime,request,remark) VALUES(@sysname, @login, @trimbleid, @startdatetime, @request, @remark)";
        skWinLib.mycmd.Parameters.Clear();
        skWinLib.mycmd.Parameters.Add("@sysname", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@login", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@trimbleid", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@startdatetime", MySqlDbType.Int64);
        skWinLib.mycmd.Parameters.Add("@request", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@remark", MySqlDbType.VarChar);

        skWinLib.mycmd.Parameters["@sysname"].Value = skWinLib.systemname;
        skWinLib.mycmd.Parameters["@login"].Value = skWinLib.username;
        skWinLib.mycmd.Parameters["@trimbleid"].Value = trimbleid;
        long mydatetime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        skWinLib.mycmd.Parameters["@startdatetime"].Value = mydatetime;
        skWinLib.mycmd.Parameters["@request"].Value = licenserequest;
        skWinLib.mycmd.Parameters["@remark"].Value = string.Empty;


        //Check and create Connection
        if (skWinLib.myconn.State == ConnectionState.Closed)
            skWinLib.myconn.Open();

        //Set Connection
        skWinLib.mycmd.Connection = skWinLib.myconn;


        //insert new record
        int updct = skWinLib.mycmd.ExecuteNonQuery();

        //close the connection
        skWinLib.myconn.Close();
        return mydatetime;
    }

    public static bool UpdateModelSharingLicenseRequest(string dbid, string remark)
    {

        skWinLib.mycmd.CommandText = "UPDATE xsmodelsharing_lic_mgmt SET upsysname = @upsysname,  uplogin = @uplogin, updatetime = @updatetime, remark = @remark  WHERE id = " + dbid;

        skWinLib.mycmd.Parameters.Clear();
        skWinLib.mycmd.Parameters.Add("@upsysname", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@uplogin", MySqlDbType.VarChar);
        skWinLib.mycmd.Parameters.Add("@updatetime", MySqlDbType.Int64);
        skWinLib.mycmd.Parameters.Add("@remark", MySqlDbType.VarChar);

        // Set the values for the parameters
        skWinLib.mycmd.Parameters["@upsysname"].Value = skWinLib.systemname;
        skWinLib.mycmd.Parameters["@uplogin"].Value = skWinLib.username;
        long mydatetime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        skWinLib.mycmd.Parameters["@updatetime"].Value = mydatetime;
        skWinLib.mycmd.Parameters["@remark"].Value = remark;
        //Check and create Connection
        if (skWinLib.myconn.State == ConnectionState.Closed)
            skWinLib.myconn.Open();

        //Set Connection
        skWinLib.mycmd.Connection = skWinLib.myconn;


        //insert new record
        int updct = skWinLib.mycmd.ExecuteNonQuery();

        //close the connection
        skWinLib.myconn.Close();

        if (updct > 0)
            return true;
        else
            return false;
    }

    public static List<string> GetModelSharingLicenseDetail(long id, string trimbleid)
    {
        List<string> MyData = new List<string>();
        skWinLib.mycmd.CommandText = "select id, trimbleid, startdatetime, request, remark from xsmodelsharing_lic_mgmt where startdatetime = " + id + " and trimbleid = '" + trimbleid + "'"; ;


        //Check and create Connection
        if (skWinLib.myconn.State == ConnectionState.Closed)
            skWinLib.myconn.Open();

        //Set Connection
        skWinLib.mycmd.Connection = skWinLib.myconn;

        // get database values 
        skWinLib.myrdr = skWinLib.mycmd.ExecuteReader();
        while (skWinLib.myrdr.Read())
        {

            MyData.Add(skWinLib.myrdr["id"].ToString());  //auto number db
            MyData.Add(skWinLib.myrdr["trimbleid"].ToString());  //vijayakumar@esskaystrucutures.com
            MyData.Add(skWinLib.myrdr["startdatetime"].ToString()); //UTC Milliseconds
            MyData.Add(skWinLib.myrdr["request"].ToString()); //Assign or Release
            if (skWinLib.myrdr["remark"].ToString().Trim().Length >=2)
                MyData.Add(skWinLib.myrdr["remark"].ToString());  //Assign or Released

        }

        //close the return data
        skWinLib.myrdr.Close();

        //close the connection
        skWinLib.myconn.Close();

        return MyData;
    }

    public static List<string> GetPendingModelSharingLicense(long dbid)
    {
        List<string> MyData = new List<string>();
        //long mydatetime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        skWinLib.mycmd.CommandText = "select id, trimbleid, startdatetime, request, remark from xsmodelsharing_lic_mgmt where id > " + dbid + " and (remark is null or remark = '')";


        //Check and create Connection
        if (skWinLib.myconn.State == ConnectionState.Closed)
            skWinLib.myconn.Open();

        //Set Connection
        skWinLib.mycmd.Connection = skWinLib.myconn;


        // get database values 
        skWinLib.myrdr = skWinLib.mycmd.ExecuteReader();
        while (skWinLib.myrdr.Read())
        {

            string id = skWinLib.myrdr["id"].ToString(); //auto number db
            string trimbleid = skWinLib.myrdr["trimbleid"].ToString(); //vijayakumar@esskaystrucutures.com
            string datetime = skWinLib.myrdr["startdatetime"].ToString(); //UTC Milliseconds
            string request = skWinLib.myrdr["request"].ToString(); ///Need or Release
            string remark = skWinLib.myrdr["remark"].ToString(); //auto number db

            MyData.Add(id + "|" + trimbleid + "|" + datetime + "|" + request + "|" + remark);  //Assign or Released


        }

        //close the return data
        skWinLib.myrdr.Close();

        //close the connection
        skWinLib.myconn.Close();

        return MyData;
    }

    public static long GetModelSharingDatabase_LastID()
    {
        long MyData = 0;

        skWinLib.mycmd.CommandText = "select max(id) from xsmodelsharing_lic_mgmt";

        //Check and create Connection
        if (skWinLib.myconn.State == ConnectionState.Closed)
            skWinLib.myconn.Open();

        //Set Connection
        skWinLib.mycmd.Connection = skWinLib.myconn;

        // get database values 
        object result = skWinLib.mycmd.ExecuteScalar();
        if (result != DBNull.Value)
        {
            MyData = Convert.ToInt64(result);
            Console.WriteLine("Last ID: " + MyData);
        }
        else
        {
            Console.WriteLine("No records found.");
        }


        //// get database values 
        //skWinLib.myrdr = skWinLib.mycmd.ExecuteReader();
        //while (skWinLib.myrdr.Read())
        //{
        //    int rct = myrdr.FieldCount;
        //    MyData = Convert.ToInt64(myrdr);

        //    //string idd = skWinLib.myrdr[0].ToString(); //auto number db
        //    //long.TryParse(idd, out MyData);
        //}

        //close the return data
        skWinLib.myrdr.Close();

        //close the connection
        skWinLib.myconn.Close();

        return MyData;
    }
}

