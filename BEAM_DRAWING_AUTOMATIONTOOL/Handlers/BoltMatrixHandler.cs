using System;
using System.Collections.Generic;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using Google.Protobuf.WellKnownTypes;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class BoltMatrixHandler
    {
        private readonly SKSortingHandler _sortingHandler;
        private readonly SKCatalogHandler _catalogHandler;

        public BoltMatrixHandler(SKSortingHandler sortingHandler, SKCatalogHandler catalogHandler)
        {
            _sortingHandler = sortingHandler ?? throw new ArgumentNullException(nameof(sortingHandler));
            _catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
        }

        public SKSortingHandler GetSortingHandler() => _sortingHandler;

        /// <summary>
        /// Gets a matrix of bolt positions for a drawing bolt, transformed to the view's coordinate system.
        /// </summary>
        public TSG.Point[,] GetBoltMatrix(TSD.Bolt drawingBolt, TSD.View currentView, TSM.Beam mainPart = null)
        {
            if (drawingBolt == null || currentView == null)
                return null;

            var model = new TSM.Model();
            var modelBolt = model.SelectModelObject(drawingBolt.ModelIdentifier);
            return CreateBoltMatrix(modelBolt as TSM.BoltArray, currentView, mainPart);
        }

        /// <summary>
        /// Gets a matrix of bolt positions for a model bolt group, transformed to the view's coordinate system.
        /// </summary>
        public TSG.Point[,] GetBoltMatrix(TSM.BoltGroup modelBolt, TSD.View currentView, bool forSection = false)
        {
            if (modelBolt == null || currentView == null)
                return null;

            return forSection
                ? CreateBoltMatrixForSection(modelBolt as TSM.BoltArray, currentView)
                : CreateBoltMatrix(modelBolt as TSM.BoltArray, currentView);
        }

        /// <summary>
        /// Gets a matrix of bolt positions for a gusset, with optional sorting condition (which is not used) //TODO.
        /// </summary>
        public TSG.Point[,] GetBoltMatrixForGusset(TSM.BoltGroup modelBolt, TSD.View currentView, string sortedCondition = null)
        {
            if (modelBolt == null || currentView == null)
                return null;

            var boltArray = modelBolt as TSM.BoltArray;
            if (boltArray == null)
                return null;

            var model = new TSM.Model();
            var toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            var toBoltMatrix = TSG.MatrixFactory.ToCoordinateSystem(boltArray.GetCoordinateSystem());
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.ViewCoordinateSystem));

            var boltCoord = boltArray.GetCoordinateSystem();
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            if (Math.Abs(boltCoord.AxisX.Z) > double.Epsilon || Math.Abs(boltCoord.AxisY.Z) > double.Epsilon)
                return null;

            var numberOfRows = boltArray.GetBoltDistXCount() + (boltArray.GetBoltDistX(0) != 0 ? 1 : 0);
            var numberOfColumns = boltArray.GetBoltDistYCount() + (boltArray.GetBoltDistY(0) != 0 ? 1 : 0);
            numberOfRows = numberOfRows == 0 ? 1 : numberOfRows;
            numberOfColumns = numberOfColumns == 0 ? 1 : numberOfColumns;

            var pointArray = new TSG.Point[numberOfRows, numberOfColumns];
            var boltPoints = GetBoltPoints(boltArray, currentView, toBoltMatrix);

            if (boltPoints.Count == 0)
                return pointArray;

            _sortingHandler.SortPoints(boltPoints);  //sort by X asc

            //var uniqueXValues = boltPoints
            //    .Select(p => (long)p.X)
            //    .Distinct()
            //    .OrderBy(x => x)
            //    .ToList();
            var uniqueXValues = new ArrayList();
            for (int i = 0; i < boltPoints.Count; i++)
            {
                double x_value = (boltPoints[i] as TSG.Point).X;

                if (i < boltPoints.Count - 1)
                {
                    if (!SKUtility.AlmostEqual(boltPoints[i + 1].X, boltPoints[i].X))
                    {
                        uniqueXValues.Add(Convert.ToInt64(boltPoints[i].X));
                    }
                }
                else
                {
                    uniqueXValues.Add(Convert.ToInt64(boltPoints[i].X));
                }

            }
            for (int b = 0; b < uniqueXValues.Count; b++)
            {
                long xValue = (long) uniqueXValues[b];
                TSD.PointList pointListInRow = new TSD.PointList();
                for (int i = 0; i < pointListInRow.Count; i++)
                {
                    double difference = Math.Abs(xValue - Convert.ToInt64(boltPoints[i].X));
                    if (difference <= 1)
                    {
                        pointListInRow.Add(boltPoints[i]);
                    }
                }

                //var pointsInRow = boltPoints.ToArray()
                //    .Where(p => Math.Abs(p.X - xValue) <= 1)
                //    .ToList();

                pointListInRow = _sortingHandler.SortPoints(pointListInRow, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);

                for (int a = 0; a < pointListInRow.Count && a < numberOfColumns; a++)
                {
                    var fromBoltToGlobal = TSG.MatrixFactory.FromCoordinateSystem(boltArray.GetCoordinateSystem());
                    pointArray[b, a] = toViewMatrix.Transform(fromBoltToGlobal.Transform(pointListInRow[a]));
                }
            }

            return AdjustGussetMatrixOrientation(pointArray, boltCoord.AxisX);
        }

        private TSG.Point[,] CreateBoltMatrix(TSM.BoltArray boltArray, TSD.View currentView, TSM.Beam mainPart = null)
        {
            if (boltArray == null || currentView == null)
                return null;

            var model = new TSM.Model();
            var toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.ViewCoordinateSystem));

            var boltCoord = boltArray.GetCoordinateSystem();
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            if (Math.Abs(boltCoord.AxisX.Z) > double.Epsilon || Math.Abs(boltCoord.AxisY.Z) > double.Epsilon)
                return null;

            var angleCheck = SKUtility.RadianToDegree(boltCoord.AxisX.GetAngleBetween(new TSG.Vector(1, 0, 0)));
            if (!IsValidAngle(angleCheck))
                return null;

            if (mainPart != null)
            {
                var profileValues = _catalogHandler.Getcatalog_values(mainPart);
                var halfProfile = profileValues[0] / 2;
                var firstBoltY = toViewMatrix.Transform(boltArray.BoltPositions[0] as TSG.Point).Y;
                if (firstBoltY < -halfProfile || firstBoltY > halfProfile)
                    return null;
            }

            return BuildPointMatrix(boltArray, currentView, toViewMatrix);
        }

        private TSG.Point[,] CreateBoltMatrixForSection(TSM.BoltArray boltArray, TSD.View currentView)
        {
            if (boltArray == null || currentView == null)
                return null;

            var model = new TSM.Model();
            var toViewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.ViewCoordinateSystem));

            var boltCoord = boltArray.GetCoordinateSystem();
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());

            var zAxis = boltCoord.AxisX.Cross(boltCoord.AxisY);
            if (Math.Abs(zAxis.X) < double.Epsilon)
                return null;

            return BuildPointMatrix(boltArray, currentView, toViewMatrix);
        }

        private TSG.Point[,] BuildPointMatrix(TSM.BoltArray boltArray, TSD.View currentView, TSG.Matrix toViewMatrix)
        {
            var boltPoints = GetBoltPoints(boltArray, currentView, toViewMatrix);
            if (boltPoints.Count == 0)
                return new TSG.Point[1, 1];

            _sortingHandler.SortPoints(boltPoints, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
            var uniqueYValues = new ArrayList();
            //var uniqueYValues = boltPoints
            //    .Select(p => (long)p.Y)
            //    .Distinct()
            //    .OrderByDescending(y => y)
            //    .ToList();
            for (int i = 0; i < boltPoints.Count; i++)
            {
                double y_value = (boltPoints[i] as TSG.Point).Y;
                if (i < boltPoints.Count - 1)
                {
                    if (Convert.ToInt32((boltPoints[i + 1].Y)) != Convert.ToInt32((boltPoints[i].Y)))
                    {
                        uniqueYValues.Add(boltPoints[i].Y);
                    }
                }
                else
                {
                    uniqueYValues.Add(boltPoints[i].Y);
                }
            }
            var numberOfRows = uniqueYValues.Count;
            var numberOfColumns = boltPoints.ToArray()
                .GroupBy(p => (long)p.Y)
                .Max(g => g.Count());

            var pointArray = new TSG.Point[numberOfRows, numberOfColumns];

            for (int b = 0; b < uniqueYValues.Count; b++)
            {
                double yValue = (double)uniqueYValues[b];
                var pointsInRow = boltPoints.ToArray()
                    .Where(p => Math.Abs(p.Y - yValue) <= double.Epsilon)
                    .ToList();
                //TODO this sorting is not bubble sort!!!!
                _sortingHandler.SortPoints(pointsInRow, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);

                for (int a = 0; a < pointsInRow.Count; a++)
                {
                    pointArray[b, a] = pointsInRow[a];
                }
            }

            return pointArray;
        }

        private TSG.Point[,] AdjustGussetMatrixOrientation(TSG.Point[,] pointArray, TSG.Vector axisX)
        {
            if (pointArray.GetLength(0) <= 1)
                return pointArray;

            var c = pointArray.GetLength(0);
            var d = pointArray.GetLength(1);
            var adjustedArray = new TSG.Point[c, d];

            if (pointArray[0, 0].Y > 0 && ((axisX.X > 0 && axisX.Y > 0) || (axisX.X < 0 && axisX.Y > 0)) ||
                pointArray[0, 0].Y < 0 && ((axisX.X < 0 && axisX.Y < 0) || (axisX.X > 0 && axisX.Y < 0)))
            {
                for (int i = 0; i < c; i++)
                {
                    for (int j = 0; j < d; j++)
                    {
                        adjustedArray[i, j] = pointArray[c - i - 1, j];
                    }
                }
                return adjustedArray;
            }

            return pointArray;
        }

        public TSD.PointList GetBoltPoints(TSM.BoltGroup boltGroup, TSD.View currentView, TSG.Matrix transformMatrix = null)
        {
            var boltPoints = new TSD.PointList();
            if (boltGroup == null || boltGroup.BoltPositions == null)
                return boltPoints;

            transformMatrix = transformMatrix ?? 
            TSG.MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);

            foreach (TSG.Point point in boltGroup.BoltPositions)
            {
                if (point != null)
                {
                    boltPoints.Add(transformMatrix.Transform(point));
                }
            }

            return boltPoints;
        }

        private static bool IsValidAngle(double angle)
        {
            return Math.Abs(angle - 0) < double.Epsilon ||
                   Math.Abs(angle - 90) < double.Epsilon ||
                   Math.Abs(angle - 180) < double.Epsilon ||
                   Math.Abs(angle - 270) < double.Epsilon ||
                   Math.Abs(angle - 360) < double.Epsilon;
        }
    }
}