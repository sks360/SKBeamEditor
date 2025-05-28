using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Catalogs;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;
using System.Collections;
using SK.Tekla.Drawing.Automation.Support;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKCatalogHandler
    {
        public SKCatalogHandler() { }

        public List<double> Getcatalog_values(TSM.Part main_part)
        {
            LibraryProfileItem mainpro = new LibraryProfileItem { ProfileName = main_part.Profile.ProfileString };
            mainpro.Select();
            ArrayList parameters = mainpro.aProfileItemParameters;

            if (parameters.Count < 3)
            {
                throw new InvalidOperationException("Insufficient parameters");
            }

            var param0 = parameters[0] as ProfileItemParameter;
            var param1 = parameters[1] as ProfileItemParameter;
            var param2 = parameters[2] as ProfileItemParameter;

            if (param0 == null || param1 == null || param2 == null)
            {
                throw new InvalidOperationException("Invalid parameter type");
            }

            return new List<double> { param0.Value, param1.Value, param2.Value };


            //ArrayList values = new ArrayList();
            //double size1_m = 0, size3_m = 0, size2_m = 0;
            //LibraryProfileItem mainpro = new LibraryProfileItem { ProfileName = main_part.Profile.ProfileString };
            //mainpro.Select();
            //ArrayList parameters_for_main = mainpro.aProfileItemParameters;
            //ProfileItemParameter bm = parameters_for_main[0] as ProfileItemParameter;
            //ProfileItemParameter cm = parameters_for_main[2] as ProfileItemParameter;
            //ProfileItemParameter dm = parameters_for_main[1] as ProfileItemParameter;
            //size1_m = bm.Value;
            //size3_m = cm.Value;
            //size2_m = dm.Value;
            //values.Add(size1_m);
            //values.Add(size2_m);
            //values.Add(size3_m);
            //return values;
        }

        public ArrayList Getcatalog_values_WITH_FLANGE_THICK(TSM.Part main_part)
        {
            ArrayList values = new ArrayList();
            double size1_m = 0, size3_m = 0, size2_m = 0, size4_m = 0;
            LibraryProfileItem mainpro = new LibraryProfileItem { ProfileName = main_part.Profile.ProfileString };
            mainpro.Select();
            ArrayList parameters_for_main = mainpro.aProfileItemParameters;
            ProfileItemParameter bm = parameters_for_main[0] as ProfileItemParameter;
            ProfileItemParameter cm = parameters_for_main[2] as ProfileItemParameter;
            ProfileItemParameter dm = parameters_for_main[1] as ProfileItemParameter;
            ProfileItemParameter dEm = parameters_for_main[3] as ProfileItemParameter;
            size1_m = bm.Value;
            size3_m = cm.Value;
            size2_m = dm.Value;
            size4_m = dEm.Value;
            values.Add(size1_m);
            values.Add(size2_m);
            values.Add(size3_m);
            values.Add(size4_m);
            return values;
        }

    }
}
