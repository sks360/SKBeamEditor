using System;
using System.Collections.Generic;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class FacePointHandler
    {
        private readonly BoundingBoxHandler boundingBoxHandler;
        private readonly string client;

        public FacePointHandler(BoundingBoxHandler boundingBoxHandler, string client)
        {
            this.boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            this.client = client;
        }

        #region Edge Point Methods

        private TSG.Point FindEdgePoint(TSM.Solid solid, TSG.Matrix toViewMatrix, TSD.PointList boundingBox,
            Func<TSG.Point, TSG.Point, int, bool> condition, bool returnStartPoint = true)
        {
            TSS.EdgeEnumerator edgeEnum = solid.GetEdgeEnumerator();
            while (edgeEnum.MoveNext())
            {
                TSS.Edge edge = edgeEnum.Current as TSS.Edge;
                TSG.Point pt1 = toViewMatrix.Transform(edge.StartPoint);
                TSG.Point pt2 = toViewMatrix.Transform(edge.EndPoint);
                int value = Convert.ToInt32(Math.Abs(boundingBox[0].Y - boundingBox[1].Y)); // Default to Y-value
                if (condition(pt1, pt2, value))
                {
                    return returnStartPoint ? pt1 : pt2;
                }
            }
            return null;
        }

        public TSG.Point Get_face_point(TSM.Part part, TSD.View currentView)
        {
            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = boundingBoxHandler.bounding_box_sort_x(part, currentView);

            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, yValue) =>
                Convert.ToInt16(pt1.X) == Convert.ToInt16(pt2.X) &&
                Convert.ToInt16(pt1.Z) == Convert.ToInt16(pt2.Z) &&
                Math.Abs(Convert.ToInt32(pt1.Y - pt2.Y)) == yValue);
        }

        public TSG.Point Get_face_point_for_angle_bothside_weldedlogic(TSM.Part part, TSD.View currentView)
        {
            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = boundingBoxHandler.bounding_box_sort_x(part, currentView);

            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, zValue) =>
                Convert.ToInt16(pt1.X) == Convert.ToInt16(pt2.X) &&
                Convert.ToInt16(pt1.Y) == Convert.ToInt16(pt2.Y) &&
                Math.Abs(Convert.ToInt32(pt1.Z - pt2.Z)) == Convert.ToInt32(Math.Abs(boundingBox[0].Z - boundingBox[1].Z)),
                false);
        }

        public TSG.Point Get_face_point_for_angle_section_view(TSM.Part part, TSD.View currentView)
        {
            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = boundingBoxHandler.bounding_box_sort_x(part, currentView);

            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, xValue) =>
                Convert.ToInt64(pt1.Z) == Convert.ToInt64(pt2.Z) &&
                Convert.ToInt64(pt1.Y) == Convert.ToInt64(pt2.Y) &&
                Math.Abs(Convert.ToInt64(pt1.X - pt2.X)) == Convert.ToInt32(Math.Abs(boundingBox[0].X - boundingBox[1].X)),
                false);
        }

        #endregion

        #region Face Point Methods

        public List<TSG.Point> Get_face_point_for_plate_test(TSM.Part part, TSD.View currentView)
        {
            TSM.Model model = new TSM.Model();
            var workPlaneHandler = model.GetWorkPlaneHandler();
            var originalPlane = workPlaneHandler.GetCurrentTransformationPlane();
            workPlaneHandler.SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.DisplayCoordinateSystem));

            try
            {
                List<TSG.Point> points = new List<TSG.Point>();
                TSM.Solid solid = part.GetSolid();
                TSS.FaceEnumerator faceEnum = solid.GetFaceEnumerator();
                while (faceEnum.MoveNext())
                {
                    TSS.Face currentFace = faceEnum.Current;
                    if (Convert.ToInt64(currentFace.Normal.Z) != 0)
                    {
                        points.AddRange(GetFacePoints(currentFace));
                    }
                }
                return points;
            }
            finally
            {
                workPlaneHandler.SetCurrentTransformationPlane(originalPlane);
            }
        }

        private List<TSG.Point> GetFacePoints(TSS.Face face)
        {
            List<TSG.Point> points = new List<TSG.Point>();
            TSS.LoopEnumerator loopEnum = face.GetLoopEnumerator();
            while (loopEnum.MoveNext())
            {
                TSS.Loop loop = loopEnum.Current as TSS.Loop;
                TSS.VertexEnumerator vertexEnum = loop.GetVertexEnumerator();
                while (vertexEnum.MoveNext())
                {
                    points.Add(vertexEnum.Current as TSG.Point);
                }
            }
            return points;
        }

        #endregion

        #region Face Area Methods

        public List<AngleFaceArea> getface_for_angle(TSM.Part part)
        {
            return GetFaceAreas(part, normal =>
                normal.Y != 0 ? "Y" :
                normal.Z != 0 ? "Z" : "X");
        }

        public List<AngleFaceArea> getface_for_tprofile(TSM.Part part)
        {
            return GetFaceAreas(part, normal => normal.Y != 0 ? "Y" : null);
        }

        public List<AngleFaceArea> getface_for_CHANNEL(TSM.Part part)
        {
            return GetFaceAreas(part, normal => normal.Z != 0 ? "Z" : null);
        }

        private List<AngleFaceArea> GetFaceAreas(TSM.Part part, Func<TSG.Vector, string> vectorTypeSelector)
        {
            List<AngleFaceArea> faceAreas = new List<AngleFaceArea>();
            TSM.Model model = new TSM.Model();
            var workPlaneHandler = model.GetWorkPlaneHandler();
            var originalPlane = workPlaneHandler.GetCurrentTransformationPlane();
            workPlaneHandler.SetCurrentTransformationPlane(new TSM.TransformationPlane(part.GetCoordinateSystem()));

            try
            {
                TSM.Solid solid = part.GetSolid(TSM.Solid.SolidCreationTypeEnum.RAW);
                TSS.FaceEnumerator faceEnum = solid.GetFaceEnumerator();
                while (faceEnum.MoveNext())
                {
                    TSS.Face face = faceEnum.Current as TSS.Face;
                    TSG.Vector normal = face.Normal;
                    string vectorType = vectorTypeSelector(normal);
                    if (vectorType != null)
                    {
                        List<TSG.Point> points = GetFacePoints(face);
                        double area = area_for_each_face(points);
                        faceAreas.Add(new AngleFaceArea { area = area, face = face, vectortype = vectorType });
                    }
                }
                return faceAreas;
            }
            finally
            {
                workPlaneHandler.SetCurrentTransformationPlane(originalPlane);
            }
        }

        public double area_for_each_face(List<TSG.Point> points)
        {
            if (points == null || points.Count < 2) return 0;

            var distances = Enumerable.Range(0, points.Count)
                .Select(i => TSG.Distance.PointToPoint(points[i], points[(i + 1) % points.Count]))
                .Distinct()
                .ToList();
            return distances.Count > 1 ? distances[0] * distances[1] : 0;
        }

        #endregion

        #region Midpoint Method

        public TSG.Point tpro_mid_pt(List<AngleFaceArea> faceAreas, TSD.View currentView)
        {
            TSM.Model model = new TSM.Model();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);

            var maxFace = faceAreas
                .Where(x => x.vectortype != "X")
                .GroupBy(x => x.vectortype)
                .Select(g => g.OrderByDescending(x => x.area).FirstOrDefault())
                .FirstOrDefault()?.face;

            if (maxFace == null) return null;

            List<TSG.Point> points = GetFacePoints(maxFace);
            if (points.Count < 2) return null;

            TSG.Matrix globalMatrix = model.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            TSG.Point viewPt0 = toViewMatrix.Transform(globalMatrix.Transform(points[0]));
            TSG.Point viewPt1 = toViewMatrix.Transform(globalMatrix.Transform(points[1]));

            return client == "SME" ? viewPt1 : SKUtility.MidPoint(viewPt0, viewPt1);
        }

        #endregion
    }

    public class AngleFaceArea
    {
        public double area { get; set; }
        public TSS.Face face { get; set; }
        public string vectortype { get; set; }
    }

}