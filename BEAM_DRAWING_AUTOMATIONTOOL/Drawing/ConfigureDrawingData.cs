using SK.Tekla.Drawing.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Microsoft.SharePoint.Client.Discovery;

namespace SK.Tekla.Drawing.Automation.Drawing
{
    public static class ConfigureDrawingData
    {

        public static List<string> PrepareDrawingAttributes(TSM.Model currentModel, SheetSelector sheetSelector,
            ref string defaultADFile )
        {
            string file_path = currentModel.GetInfo().ModelPath;
            TeklaStructuresFiles files = new TeklaStructuresFiles(file_path);

            List<string> allADFiles = files.GetMultiDirectoryFileList("ad", false);
            string sheet = sheetSelector == SheetSelector.UnDefined? string.Empty: 
                        sheetSelector.ToString();
            List<string> sheetBasedList = allADFiles.Where(X => X.Contains($"VBR_BEAM_A{sheet}")).ToList();
            List<string> skSheetBasedList = allADFiles.Where(X => X.Contains($"SK_BEAM_A{sheet}")).ToList();

            sheetBasedList.Sort();
            sheetBasedList.Reverse();
            skSheetBasedList.Sort();
            skSheetBasedList.Reverse();
            defaultADFile = skSheetBasedList.Count > 0? skSheetBasedList[0]: null;
            return sheetBasedList;
        }
    }
}
