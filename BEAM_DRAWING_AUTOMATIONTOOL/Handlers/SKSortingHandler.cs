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
using Tekla.Structures.Geometry3d;



namespace SK.Tekla.Drawing.Automation.Handlers
{
    /// <summary>
    /// Sorts the 3D pointlist 
    /// </summary>
    public class SKSortingHandler
    {
        private static readonly log4net.ILog _logger =
       log4net.LogManager.GetLogger(typeof(SKSortingHandler));

        public SKSortingHandler() { }
        public enum SortBy
        {
            X,
            Y,
            Z
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }

        public TSD.PointList SortPoints(ArrayList arrayList, TSD.View view, SortBy sortBy = SortBy.X,
       SortOrder sortOrder = SortOrder.Ascending)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(view.ViewCoordinateSystem);
            foreach (TSG.Point pt in arrayList)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
            if (sortBy == SortBy.X)
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    for (int i = 0; i < boltpts.Count; i++)
                    {
                        for (int j = 0; j < boltpts.Count; j++)
                        {
                            if ((boltpts[i] as TSG.Point).X < (boltpts[j] as TSG.Point).X)
                            {
                                TSG.Point temp_point = (boltpts[i] as TSG.Point);
                                boltpts[i] = boltpts[j];
                                boltpts[j] = temp_point;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boltpts.Count; i++)
                    {
                        for (int j = 0; j < boltpts.Count; j++)
                        {
                            if ((boltpts[i] as TSG.Point).X > (boltpts[j] as TSG.Point).X)
                            {
                                TSG.Point temp_point = (boltpts[i] as TSG.Point);
                                boltpts[i] = boltpts[j];
                                boltpts[j] = temp_point;
                            }
                        }
                    }
                }
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    for (int i = 0; i < boltpts.Count; i++)
                    {
                        for (int j = 0; j < boltpts.Count; j++)
                        {
                            if ((boltpts[i] as TSG.Point).Y < (boltpts[j] as TSG.Point).Y)
                            {
                                TSG.Point temp_point = (boltpts[i] as TSG.Point);
                                boltpts[i] = boltpts[j];
                                boltpts[j] = temp_point;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boltpts.Count; i++)
                    {
                        for (int j = 0; j < boltpts.Count; j++)
                        {
                            if ((boltpts[i] as TSG.Point).Y > (boltpts[j] as TSG.Point).Y)
                            {
                                TSG.Point temp_point = (boltpts[i] as TSG.Point);
                                boltpts[i] = boltpts[j];
                                boltpts[j] = temp_point;
                            }
                        }
                    }
                }
            }
            return boltpts;
        }

        //public TSD.PointList SortPoints(ArrayList arrayList, TSD.View view, SortBy sortBy = SortBy.X,
        //    SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    var points = ArrayListConverter.ToPointEnumerable(arrayList);
        //    return SortPoints(points, view, sortBy, sortOrder);
        //}

        private TSD.PointList SortPoints(IEnumerable<Point> points, TSD.View view, 
            SortBy sortBy = SortBy.X, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (points == null || view == null)
                throw new ArgumentNullException("Points or view cannot be null.");

            var pointsList = points.ToList();
            if (!pointsList.Any())
                return new TSD.PointList();

            var toViewMatrix = MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
            var transformedPoints = pointsList.Select(p => toViewMatrix.Transform(p));

            var sortedPoints = sortOrder == SortOrder.Ascending
                ? sortBy == SortBy.X
                    ? transformedPoints.OrderBy(p => p.X)
                    : transformedPoints.OrderBy(p => p.Y)
                : sortBy == SortBy.X
                    ? transformedPoints.OrderByDescending(p => p.X)
                    : transformedPoints.OrderByDescending(p => p.Y);

            var pointList = new TSD.PointList();
            foreach (var point in sortedPoints)
            {
                pointList.Add(point);
            }

            return pointList;
        }


        //public TSD.PointList SortPoints(ArrayList arrayList, SortBy sortBy = SortBy.X,
        //  SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    var points = ArrayListConverter.ToPointEnumerable(arrayList);
        //    if (points == null)
        //        throw new ArgumentNullException("Points or view cannot be null.");

        //    return SortPoints(points.ToList(), sortBy, sortOrder);
        //}
        public void SortPoints(List<Point> pointsList,
           SortBy sortBy = SortBy.X, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (!pointsList.Any())
                return;

            var sortedPoints = sortOrder == SortOrder.Ascending
                ? sortBy == SortBy.X
                    ? pointsList.OrderBy(p => p.X)
                    : pointsList.OrderBy(p => p.Y)
                : sortBy == SortBy.X
                    ? pointsList.OrderByDescending(p => p.X)
                    : pointsList.OrderByDescending(p => p.Y);


            pointsList = sortedPoints.ToList();
        }

        public TSD.PointList SortPoints(TSD.PointList points, SortBy sortBy = SortBy.X,
    SortOrder sortOrder = SortOrder.Ascending)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (sortBy == SortBy.X)
            {
                if (sortOrder == SortOrder.Ascending)
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
                else
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            if ((points[i] as TSG.Point).X > (points[j] as TSG.Point).X)
                            {
                                TSG.Point temp_point = (points[i] as TSG.Point);
                                points[i] = points[j];
                                points[j] = temp_point;
                            }
                        }
                    }
                }
            }
            else
            {
                if (sortOrder == SortOrder.Ascending)
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
                else
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            if ((points[i] as TSG.Point).Y > (points[j] as TSG.Point).Y)
                            {
                                TSG.Point temp_point = (points[i] as TSG.Point);
                                points[i] = points[j];
                                points[j] = temp_point;
                            }
                        }
                    }
                }
            }
            return points;
            //var pointsList = points.Cast<Point>().ToList();
            //return SortPoints(pointsList, sortBy, sortOrder);
        }

        //public TSD.PointList SortPoints(TSD.PointList points, SortBy sortBy = SortBy.X, 
        //    SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    if (points == null)
        //        throw new ArgumentNullException(nameof(points));

        //    var pointsList = points.Cast<Point>().ToList();
        //    return SortPoints(pointsList,sortBy,sortOrder);
        //}

        //public List<double> SortDoublesDescending(List<double> values)
        //{
        //    if (values == null)
        //        throw new ArgumentNullException(nameof(values));

        //    return values.OrderByDescending(v => v).ToList();
        //}
    }

    public static class ArrayListConverter
    {
        public static IEnumerable<Point> ToPointEnumerable(ArrayList arrayList)
        {
            if (arrayList == null)
                throw new ArgumentNullException(nameof(arrayList));

            foreach (var item in arrayList)
            {
                if (item is Point point)
                {
                    yield return point;
                }
                else
                {
                    throw new InvalidCastException($"Element of type {item?.GetType().Name} cannot be cast to Point.");
                }
            }
        }
    }
}
