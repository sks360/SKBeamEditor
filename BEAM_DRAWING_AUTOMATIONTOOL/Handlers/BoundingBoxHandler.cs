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
namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class BoundingBoxHandler
    {


        public BoundingBoxHandler() { }

        public TSD.PointList bounding_box_sort_z(TSM.ModelObject mymodel_object, TSD.View current_view)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();
            //TSM.ModelObject assembly_for_check = mymodel_object;
            ////////////////////////////////////////////////assembly bounding box logic//////////////////////////////////////////////////////////////////
            //double x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_X", ref x1);
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_Y", ref y1);
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_Z", ref z1);
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_X", ref x2);
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_Y", ref y2);
            //assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_Z", ref z2);
            //TSG.Point workpointst_1 = new TSG.Point(x1, y1, z1);
            //TSG.Point workpointend_1 = new TSG.Point(x2, y2, z2);
            //TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);

            //TSG.Point workpointst2 = toviewmatrix.Transform(workpointst_1);

            //TSG.Point workpointend2 = toviewmatrix.Transform(workpointend_1);
            //bounding_box_pts.Add(workpointst2);
            //bounding_box_pts.Add(workpointend2);
            TSM.Model MYMODEL = new TSM.Model();
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
            TSM.Solid MYSOLID = (mymodel_object as TSM.Part).GetSolid();
            TSG.Point MAXPT = MYSOLID.MaximumPoint;
            TSG.Point MINPT = MYSOLID.MinimumPoint;
            bounding_box_pts.Add(MAXPT);
            bounding_box_pts.Add(MINPT);
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            for (int i = 0; i < bounding_box_pts.Count; i++)
            {

                for (int j = 0; j < bounding_box_pts.Count; j++)
                {
                    if ((bounding_box_pts[i] as TSG.Point).Z < (bounding_box_pts[j] as TSG.Point).Z)
                    {
                        TSG.Point temp_point = (bounding_box_pts[i] as TSG.Point);
                        bounding_box_pts[i] = bounding_box_pts[j];
                        bounding_box_pts[j] = temp_point;

                    }

                }
            }

            return (bounding_box_pts);


        }

        public TSD.PointList bounding_box_sort_z(TSM.ModelObject mymodel_object, TSM.Beam mainpart)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();

            TSM.Model MYMODEL = new TSM.Model();
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(mainpart.GetCoordinateSystem()));
            TSM.Solid MYSOLID = (mymodel_object as TSM.Part).GetSolid();
            TSG.Point MAXPT = MYSOLID.MaximumPoint;
            TSG.Point MINPT = MYSOLID.MinimumPoint;
            bounding_box_pts.Add(MAXPT);
            bounding_box_pts.Add(MINPT);
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            for (int i = 0; i < bounding_box_pts.Count; i++)
            {

                for (int j = 0; j < bounding_box_pts.Count; j++)
                {
                    if ((bounding_box_pts[i] as TSG.Point).Z < (bounding_box_pts[j] as TSG.Point).Z)
                    {
                        TSG.Point temp_point = (bounding_box_pts[i] as TSG.Point);
                        bounding_box_pts[i] = bounding_box_pts[j];
                        bounding_box_pts[j] = temp_point;

                    }

                }
            }

            return (bounding_box_pts);
        }

        public TSD.PointList bounding_box_sort_x(TSM.ModelObject mymodel_object, TSD.View current_view)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();

            TSM.Model MYMODEL = new TSM.Model();
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
            TSM.Solid MYSOLID = (mymodel_object as TSM.Part).GetSolid();
            TSG.Point MAXPT = MYSOLID.MaximumPoint;
            TSG.Point MINPT = MYSOLID.MinimumPoint;
            bounding_box_pts.Add(MAXPT);
            bounding_box_pts.Add(MINPT);
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            for (int i = 0; i < bounding_box_pts.Count; i++)
            {

                for (int j = 0; j < bounding_box_pts.Count; j++)
                {
                    if ((bounding_box_pts[i] as TSG.Point).X < (bounding_box_pts[j] as TSG.Point).X)
                    {
                        TSG.Point temp_point = (bounding_box_pts[i] as TSG.Point);
                        bounding_box_pts[i] = bounding_box_pts[j];
                        bounding_box_pts[j] = temp_point;

                    }

                }
            }

            return (bounding_box_pts);


        }

        public TSD.PointList bounding_box_sort_x(TSM.ModelObject mymodel_object, TSM.Beam mainpart)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();

            TSM.Model MYMODEL = new TSM.Model();
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(mainpart.GetCoordinateSystem()));
            TSM.Solid MYSOLID = (mymodel_object as TSM.Part).GetSolid();
            TSG.Point MAXPT = MYSOLID.MaximumPoint;
            TSG.Point MINPT = MYSOLID.MinimumPoint;
            bounding_box_pts.Add(MAXPT);
            bounding_box_pts.Add(MINPT);
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            for (int i = 0; i < bounding_box_pts.Count; i++)
            {

                for (int j = 0; j < bounding_box_pts.Count; j++)
                {
                    if ((bounding_box_pts[i] as TSG.Point).X < (bounding_box_pts[j] as TSG.Point).X)
                    {
                        TSG.Point temp_point = (bounding_box_pts[i] as TSG.Point);
                        bounding_box_pts[i] = bounding_box_pts[j];
                        bounding_box_pts[j] = temp_point;

                    }

                }
            }

            return (bounding_box_pts);


        }
      
        public TSD.PointList bounding_box_sort_y(TSM.ModelObject mymodel_object, TSD.View current_view)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();

            TSM.Model MYMODEL = new TSM.Model();
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.DisplayCoordinateSystem));
            TSM.Solid MYSOLID = (mymodel_object as TSM.Part).GetSolid();
            TSG.Point MAXPT = MYSOLID.MaximumPoint;
            TSG.Point MINPT = MYSOLID.MinimumPoint;
            bounding_box_pts.Add(MAXPT);
            bounding_box_pts.Add(MINPT);
            MYMODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            for (int i = 0; i < bounding_box_pts.Count; i++)
            {

                for (int j = 0; j < bounding_box_pts.Count; j++)
                {
                    if ((bounding_box_pts[i] as TSG.Point).Y < (bounding_box_pts[j] as TSG.Point).Y)
                    {
                        TSG.Point temp_point = (bounding_box_pts[i] as TSG.Point);
                        bounding_box_pts[i] = bounding_box_pts[j];
                        bounding_box_pts[j] = temp_point;

                    }

                }
            }

            return (bounding_box_pts);


        }


        public TSD.PointList bounding_box_FOR_DIM(TSM.ModelObject mymodel_object)
        {
            TSD.PointList bounding_box_pts = new TSD.PointList();
            TSM.ModelObject assembly_for_check = mymodel_object;
            //////////////////////////////////////////////assembly bounding box logic//////////////////////////////////////////////////////////////////
            double x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_X", ref x1);
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_Y", ref y1);
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MIN_Z", ref z1);
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_X", ref x2);
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_Y", ref y2);
            assembly_for_check.GetReportProperty("BOUNDING_BOX_MAX_Z", ref z2);
            TSG.Point workpointst_1 = new TSG.Point(x1, y1, z1);
            TSG.Point workpointend_1 = new TSG.Point(x2 + 50, y2, z2);
            bounding_box_pts.Add(workpointst_1);
            bounding_box_pts.Add(workpointend_1);

            return (bounding_box_pts);
        }



    }
}
