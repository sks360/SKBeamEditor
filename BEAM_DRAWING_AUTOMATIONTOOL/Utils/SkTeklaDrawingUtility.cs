using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Models;

namespace SK.Tekla.Drawing.Automation.Utils
{
    public static class SkTeklaDrawingUtility
    {
        public static string get_report_properties(TSM.Part part, string property)
        {

            string output = "";
            part.GetReportProperty(property, ref output);
            return output;
        }
        public static string GetSKReportProperty(TSM.Assembly part, string property)
        {
            string output = "";
            part.GetReportProperty(property, ref output);
            return output;
        }
        public static double get_report_properties_double(TSM.Part part, string property)
        {
            double output = 0;
            part.GetReportProperty(property, ref output);
            return Math.Round((output), 2);
        }

        public static List<TSD.DrawingObject> drg_object_from_enumerator(TSD.DrawingObjectEnumerator drgobjectenum)
        {
            List<TSD.DrawingObject> drgpartlist = new List<TSD.DrawingObject>();
            while (drgobjectenum.MoveNext())
            {

                TSD.DrawingObject drgpart = drgobjectenum.Current as TSD.DrawingObject;
                drgpartlist.Add(drgpart);
            }
            return drgpartlist;
        }

        public static double GetFontHeight(string client, FontSizeSelector fontSize, string drawAttrib)
        {
            if ((drawAttrib == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
            {
                if (client.Equals("HILLSDALE") || (fontSize == FontSizeSelector.NineBy64))
                {
                    return 3.571875;
                }
                else
                {
                    return 3.175;
                }
            }
            return 2.38125;
        }






    }
}
