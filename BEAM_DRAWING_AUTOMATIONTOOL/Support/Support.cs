using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Org.BouncyCastle.Asn1.Cms;

namespace SK.Tekla.Drawing.Automation.Support
{
    public class Support
    {
    }

    public class DrawingData
    {
        public string Assembly_Mark { get; set; }
        public string Part_Mark { get; set; }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string ProfileType { get; set; }
        public double Length { get; set; }
        public int Secondary { get; set; }

    }


    public class SKLayout
    {
        public string attribute;
        public string adFileName;
        public double scale;
        public double minLenth;

        public override string ToString()
        {
            return "attribute: " + attribute + " AD: " + adFileName + " scale: " + scale + " minLenth: " + minLenth;
        }
    }

    public class DimensionWithDifference
    {
        public TSD.StraightDimension StDimen;
        public double Difference;
        public TSG.Vector MyVector;

    }

    public class req_pts
    {
        public double distance;
        public double distance_for_y;
        public double distance_for_Z;
        public TSD.PointList list_of_points;
        public TSM.Part part;
        public string PART_MARK;
    }

    public class slope_bolt_class
    {
        public TSG.Point original_pt;
        public TSG.Point converted_pts;
        public double x_dist_of_rotated;
        public int value;
        public List<TSG.Point> mypoint_list_original;
    }



    public class angle_dim_3_5
    {
        public TSG.Point pt1;
        public TSG.Point pt2;
        public TSG.Vector x_axis;
        public TSD.PointList myptlist;
        public TSM.Part angle;
    }



    public class REMOVING_DUPLICATE_Z_VALUE_IN_CURRENT_VIEW : IEqualityComparer<TSG.Point>
    {
        public bool Equals(TSG.Point a, TSG.Point b)
        {


            if ((Convert.ToInt64(a.X) == Convert.ToInt64(b.X)) && (Convert.ToInt64(a.Y) == Convert.ToInt64(b.Y)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int GetHashCode(TSG.Point codeh)
        {
            return codeh.X.GetHashCode() ^ codeh.Y.GetHashCode();
        }
    }

    public class REMOVING_DUPLICATE_angle_VALUE_IN_CURRENT_VIEW : IEqualityComparer<angle_dim_3_5>
    {
        public bool Equals(angle_dim_3_5 a, angle_dim_3_5 b)
        {
            bool result_for_x_1 = (Convert.ToInt64(a.pt1.X) == Convert.ToInt64(b.pt1.X));
            bool result_for_y_1 = (Convert.ToInt64(a.pt1.Y) == Convert.ToInt64(b.pt1.Y));
            bool result_for_x_2 = (Convert.ToInt64(a.pt2.X) == Convert.ToInt64(b.pt2.X));
            bool result_for_y_2 = (Convert.ToInt64(a.pt2.Y) == Convert.ToInt64(b.pt2.Y));


            if ((result_for_x_1 == true) && (result_for_y_1 == true) && (result_for_x_2 == true) && (result_for_y_2 == true))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int GetHashCode(angle_dim_3_5 codeh)
        {
            //return codeh.X.GetHashCode() ^ codeh.Y.GetHashCode();
            return codeh.pt1.X.GetHashCode() ^ codeh.pt1.Y.GetHashCode() ^ codeh.pt2.X.GetHashCode() ^ codeh.pt2.Y.GetHashCode();
        }
    }

    public class SectionLocationWithParts
    {
        public List<TSM.Part> PartList { get; set; } = new List<TSM.Part>();
        public double Distance { get; set; }
        public string SectionViewNeeded { get; set; }
        public int IndexOfSameSection { get; set; }
        public TSD.View MyView { get; set; }
        public List<TSM.Part> RequiredPartList  { get; set; } = new List<TSM.Part>();

        public override string ToString()
        {
            return $"PartList: {PartList.Count}  RequiredPartList: {RequiredPartList.Count} " +
                $"Distance: {Distance} SectionViewNeeded: {SectionViewNeeded} MyView: {MyView.Name}";
        }

    }

    public class RequiredPoints
    {
        public double Distance { get; set; }
        public PointList Points { get; set; }
        public double DistanceForY { get; set; }
        public Part Part { get; set; }
        public string PartMark { get; set; }
        public double DistanceForZ { get; set; }
    }

    //public class DimensionWithDifference
    //{
    //    public StraightDimension Dimension { get; set; }
    //    public double Difference { get; set; }
    //    public Vector Vector { get; set; }
    //}
}
