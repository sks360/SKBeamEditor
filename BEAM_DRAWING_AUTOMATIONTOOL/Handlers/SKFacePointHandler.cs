using System;
using System.Collections.Generic;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using SK.Tekla.Drawing.Automation.Models;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKFacePointHandler
    {
        private readonly SKBoundingBoxHandler _boundingBoxHandler;
        private readonly CustomInputModel _userInput;
        private readonly string _client;

        public SKFacePointHandler(SKBoundingBoxHandler boundingBoxHandler, CustomInputModel userInput)
        {
            _boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
            _client = _userInput.Client;
        }
        #region Edge Point Methods

        private TSG.Point FindEdgePoint(TSM.Solid solid, TSG.Matrix toViewMatrix, TSD.PointList boundingBox,
           Func<TSG.Point, TSG.Point, double, bool> condition, bool returnStartPoint = true)
        {
            TSS.EdgeEnumerator edgeEnum = solid.GetEdgeEnumerator();
            while (edgeEnum.MoveNext())
            {
                TSS.Edge edge = edgeEnum.Current as TSS.Edge;
                TSG.Point pt1 = toViewMatrix.Transform(edge.StartPoint);
                TSG.Point pt2 = toViewMatrix.Transform(edge.EndPoint);
                double value = Math.Abs(boundingBox[0].Y - boundingBox[1].Y); // Default to Y-value
                if (condition(pt1, pt2, value))
                {
                    return returnStartPoint ? pt1 : pt2;
                }
            }
            return null;
        }

        public TSG.Point GetFacePoint(TSM.Part part, TSD.View currentView)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (currentView == null) throw new ArgumentNullException(nameof(currentView));

            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = _boundingBoxHandler.BoundingBoxSort(part, currentView);

            //TODO VEERA: Test if the edge points are same, otherwise use the OLD code that remains commented
            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, yValue) =>
               Convert.ToInt16(pt1.X) == Convert.ToInt16(pt2.X) &&
               Convert.ToInt16(pt1.Z) == Convert.ToInt16(pt2.Z) &&
               Math.Abs(Convert.ToInt32(pt1.Y - pt2.Y)) == yValue);

            //return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, yValue) =>
            //    Math.Abs(pt1.X - pt2.X) < 1e-6 &&
            //    Math.Abs(pt1.Z - pt2.Z) < 1e-6 &&
            //    Math.Abs(Math.Abs(pt1.Y - pt2.Y) - yValue) < 1e-6);
        }

        public TSG.Point GetFacePointForAngleBothsideWeldedLogic(TSM.Part part, TSD.View currentView)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (currentView == null) throw new ArgumentNullException(nameof(currentView));

            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = _boundingBoxHandler.BoundingBoxSort(part, currentView);
           

            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, zValue) =>
                Convert.ToInt16(pt1.X) == Convert.ToInt16(pt2.X) &&
                Convert.ToInt16(pt1.Y) == Convert.ToInt16(pt2.Y) &&
                Math.Abs(Convert.ToInt32(pt1.Z - pt2.Z)) == Convert.ToInt32(Math.Abs(boundingBox[0].Z - boundingBox[1].Z)),
                false);
            //double zValue = Math.Abs(boundingBox[0].Z - boundingBox[1].Z);
            //return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, _) =>
            //    Math.Abs(pt1.X - pt2.X) < 1e-6 &&
            //    Math.Abs(pt1.Y - pt2.Y) < 1e-6 &&
            //    Math.Abs(Math.Abs(pt1.Z - pt2.Z) - zValue) < 1e-6, false);
        }
        public TSG.Point GetFacePointForAngleSectionView(TSM.Part part, TSD.View currentView)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (currentView == null) throw new ArgumentNullException(nameof(currentView));

            TSM.Solid solid = part.GetSolid();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            TSD.PointList boundingBox = _boundingBoxHandler.BoundingBoxSort(part, currentView);

            //double xValue = Math.Abs(boundingBox[0].X - boundingBox[1].X);
            //return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, _) =>
            //    Math.Abs(pt1.Z - pt2.Z) < 1e-6 &&
            //    Math.Abs(pt1.Y - pt2.Y) < 1e-6 &&
            //    Math.Abs(Math.Abs(pt1.X - pt2.X) - xValue) < 1e-6, false);
            return FindEdgePoint(solid, toViewMatrix, boundingBox, (pt1, pt2, xValue) =>
               Convert.ToInt64(pt1.Z) == Convert.ToInt64(pt2.Z) &&
               Convert.ToInt64(pt1.Y) == Convert.ToInt64(pt2.Y) &&
               Math.Abs(Convert.ToInt64(pt1.X - pt2.X)) == Convert.ToInt32(Math.Abs(boundingBox[0].X - boundingBox[1].X)),
               false);
        }
        #endregion

        #region Face Point Methods

        public List<TSG.Point> GetFacePointsForPlateTest(TSM.Part part, TSD.View currentView)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (currentView == null) throw new ArgumentNullException(nameof(currentView));

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
                    //if (Math.Abs(currentFace.Normal.Z) > 1e-6)
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

        public List<AngleFaceArea> GetFaceAreasForAngle(TSM.Part part)
        {
            //return GetFaceAreas(part, normal =>
            //    Math.Abs(normal.Y) > 1e-6 ? "Y" :
            //    Math.Abs(normal.Z) > 1e-6 ? "Z" : "X");
            return GetFaceAreas(part, normal =>
               normal.Y != 0 ? "Y" :
               normal.Z != 0 ? "Z" : "X");
        }

        public List<AngleFaceArea> GetFaceAreasForTProfile(TSM.Part part)
        {
            return GetFaceAreas(part, normal => Math.Abs(normal.Y) > 1e-6 ? "Y" : null);
        }

        public List<AngleFaceArea> GetFaceAreasForChannel(TSM.Part part)
        {
            return GetFaceAreas(part, normal => Math.Abs(normal.Z) > 1e-6 ? "Z" : null);
        }

        private List<AngleFaceArea> GetFaceAreas(TSM.Part part, Func<TSG.Vector, string> vectorTypeSelector)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

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
                        double area = CalculateFaceArea(points);
                        faceAreas.Add(new AngleFaceArea { Area = area, Face = face, VectorType = vectorType });
                    }
                }
                return faceAreas;
            }
            finally
            {
                workPlaneHandler.SetCurrentTransformationPlane(originalPlane);
            }
        }

        private double CalculateFaceArea(List<TSG.Point> points)
        {
            if (points == null || points.Count < 2) return 0;

            var distances = points
                .Select((p, i) => TSG.Distance.PointToPoint(p, points[(i + 1) % points.Count]))
                .Distinct()
                .ToList();
            return distances.Count > 1 ? distances[0] * distances[1] : 0;
        }

        #endregion

        #region Midpoint Method

        public TSG.Point GetTProfileMidPoint(List<AngleFaceArea> faceAreas, TSD.View currentView)
        {
            if (faceAreas == null || faceAreas.Count == 0) return null;
            if (currentView == null) throw new ArgumentNullException(nameof(currentView));

            TSM.Model model = new TSM.Model();
            TSG.Matrix toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);

            var maxFace = faceAreas
                .Where(x => x.VectorType != "X")
                .GroupBy(x => x.VectorType)
                .Select(g => g.OrderByDescending(x => x.Area).FirstOrDefault())
                .FirstOrDefault()?.Face;

            if (maxFace == null) return null;

            List<TSG.Point> points = GetFacePoints(maxFace);
            if (points.Count < 2) return null;

            TSG.Matrix globalMatrix = model.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            TSG.Point viewPt0 = toViewMatrix.Transform(globalMatrix.Transform(points[0]));
            TSG.Point viewPt1 = toViewMatrix.Transform(globalMatrix.Transform(points[1]));

            return _client == "SME" ? viewPt1 : SKUtility.MidPoint(viewPt0, viewPt1);
        }

        #endregion
    }

    public class AngleFaceArea
    {
        public double Area { get; set; }
        public TSS.Face Face { get; set; }
        public string VectorType { get; set; }
    }
}
