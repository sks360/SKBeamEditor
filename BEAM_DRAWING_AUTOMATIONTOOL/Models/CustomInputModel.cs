using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.Tekla.Drawing.Automation.Models
{
    public class CustomInputModel
    {
        public string Client { get; set; }
        public FontSizeSelector FontSize = FontSizeSelector.OneBy8;

        public SheetSelector SheetSelected = SheetSelector.UnDefined;

        public double Scale { get; set; }

        public double MinLength { get; set; }

        public CustomInputModel() { }

        public bool NeedEleDimension { get; set; }

        public bool NeedRDConnectionMark { get; set; }

        public bool NeedCutLength { get; set; }


        public double SecScale { get; set; }
    }
}
