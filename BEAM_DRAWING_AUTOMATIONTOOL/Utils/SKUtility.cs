using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
namespace SK.Tekla.Drawing.Automation.Utils
{
    public static class SKUtility
    {

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static bool AlmostEqual(double x, double y)
        {
            var epsilon = Math.Abs(Math.Abs(x) - Math.Abs(y));
            return epsilon <= 10;
        }

        public static string DateandTime()
        {
            DateTime dateTime = System.DateTime.Now;
            return dateTime.ToString("g").Replace(":", "_").Replace('/', '_');
        }

        public static TSG.Point MidPoint(TSG.Point pt1, TSG.Point pt2)
        {
            TSG.Point point = new TSG.Point((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0, (pt1.Z + pt2.Z) / 2.0);
            return point;
        }
    }
}
