using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace SK.Tekla.Drawing.Automation.Support
{
    public static class AssemblyExtension
    {
        /// <summary>
        /// Check if the numbering configured correctly
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public static bool CheckNumbering(TSM.ModelObject modelObject, ref string assemblyName)
        {
            TSM.Assembly assembly = modelObject as TSM.Assembly;
            string assemblyPos = string.Empty;

            assembly.GetReportProperty("ASSEMBLY_POS", ref assemblyPos);

            return assemblyPos.Contains('?');
        }
    }
}
