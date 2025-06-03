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
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;
using Tekla.Structures.Model;
using Tekla.Structures.Drawing;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKBoundingBoxHandler
    {

        private static readonly log4net.ILog _logger =
                            log4net.LogManager.GetLogger(typeof(SKBoundingBoxHandler));
        public SKBoundingBoxHandler() { }

        /// <summary>
        /// Sorts a list of points along the specified axis and returns a TSD.PointList.
        /// </summary>
        private void SortPointsByAxis(TSD.PointList points, SortBy sortBy)
        {
            if (sortBy == SortBy.X)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        if ((points[i] as TSG.Point).X < (points[j] as TSG.Point).X)
                        {
                            TSG.Point temp_point = (points[i] as TSG.Point);
                            points[i] = points[j];
                            points[j] = temp_point;
                        }
                    }
                }
            }
            else if (sortBy == SortBy.Y)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        if ((points[i] as TSG.Point).Y < (points[j] as TSG.Point).Y)
                        {
                            TSG.Point temp_point = (points[i] as TSG.Point);
                            points[i] = points[j];
                            points[j] = temp_point;
                        }
                    }
                }
            }
            else if (sortBy == SortBy.Z)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        if ((points[i] as TSG.Point).Z < (points[j] as TSG.Point).Z)
                        {
                            TSG.Point temp_point = (points[i] as TSG.Point);
                            points[i] = points[j];
                            points[j] = temp_point;
                        }
                    }
                }
            }
        }

        public TSD.PointList BoundingBoxSort(TSM.ModelObject modelObject, TSD.View currentView, SortBy sortBy = SortBy.X)
        {
            TSD.PointList boundingBoxPoints = new TSD.PointList();
            GetSolidPoints(modelObject, currentView.DisplayCoordinateSystem, boundingBoxPoints);
            SortPointsByAxis(boundingBoxPoints, sortBy);
            return boundingBoxPoints;
        }

 
        public TSD.PointList BoundingBoxSort(TSM.ModelObject modelObject, TSM.Beam mainPart, SortBy sortBy = SortBy.X)
        {
            TSD.PointList boundingBoxPoints = new TSD.PointList();
            GetSolidPoints(modelObject, mainPart.GetCoordinateSystem(), boundingBoxPoints);
            SortPointsByAxis(boundingBoxPoints, sortBy);
            return boundingBoxPoints;

        }

        private static void GetSolidPoints(TSM.ModelObject modelObject,TSG.CoordinateSystem coordinateSystem, PointList boundingBoxPoints)
        {
            TSM.Model model = new TSM.Model();
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(coordinateSystem));
            TSM.Solid solid = (modelObject as TSM.Part).GetSolid();
            boundingBoxPoints.Add(solid.MaximumPoint);
            boundingBoxPoints.Add(solid.MinimumPoint);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
        }



        /// <summary>
        /// Get the bounding box points
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public TSD.PointList BoundingBoxForDimension(TSM.ModelObject modelObject)
        {
            double x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
            modelObject.GetReportProperty("BOUNDING_BOX_MIN_X", ref x1);
            modelObject.GetReportProperty("BOUNDING_BOX_MIN_Y", ref y1);
            modelObject.GetReportProperty("BOUNDING_BOX_MIN_Z", ref z1);
            modelObject.GetReportProperty("BOUNDING_BOX_MAX_X", ref x2);
            modelObject.GetReportProperty("BOUNDING_BOX_MAX_Y", ref y2);
            modelObject.GetReportProperty("BOUNDING_BOX_MAX_Z", ref z2);
            TSD.PointList boundingBoxPts = new TSD.PointList()
            {
                new TSG.Point(x1, y1, z1),
                new TSG.Point(x2 + 50, y2, z2)
            };
            return boundingBoxPts;
        }

    }
}
