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

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SortingHandler
    {
        public SortingHandler() { }

        public TSD.PointList sorting_points_by_x_asc(ArrayList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_x_des(ArrayList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_x_des(TSD.PointList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_y_asc(ArrayList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_y_des(ArrayList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_y_des(TSD.PointList list_of_points, TSD.View currentview)
        {
            TSD.PointList boltpts = new TSD.PointList();
            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(currentview.ViewCoordinateSystem);
            foreach (TSG.Point pt in list_of_points)
            {
                boltpts.Add(toviewmatrix.Transform(pt));
            }
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
            return boltpts;
        }

        public TSD.PointList sorting_points_by_y_des(TSD.PointList list_of_points)
        {
            for (int i = 0; i < list_of_points.Count; i++)
            {
                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).Y > (list_of_points[j] as TSG.Point).Y)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;
                    }
                }
            }
            return list_of_points;
        }

        public TSD.PointList sorting_points_by_x_des(TSD.PointList list_of_points)
        {
            for (int i = 0; i < list_of_points.Count; i++)
            {
                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).X > (list_of_points[j] as TSG.Point).X)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;
                    }
                }
            }
            return list_of_points;
        }
        public TSD.PointList sorting_points_by_x_asc(TSD.PointList list_of_points)
        {


            for (int i = 0; i < list_of_points.Count; i++)
            {

                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).X < (list_of_points[j] as TSG.Point).X)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;

                    }

                }
            }
            return list_of_points;

        }
        public TSD.PointList sorting_points_by_y_asc(TSD.PointList list_of_points)
        {


            for (int i = 0; i < list_of_points.Count; i++)
            {

                for (int j = 0; j < list_of_points.Count; j++)
                {
                    if ((list_of_points[i] as TSG.Point).Y < (list_of_points[j] as TSG.Point).Y)
                    {
                        TSG.Point temp_point = (list_of_points[i] as TSG.Point);
                        list_of_points[i] = list_of_points[j];
                        list_of_points[j] = temp_point;

                    }

                }
            }
            return list_of_points;

        }

        public void sorting_points_by_x_des(ArrayList value_of_no)
        {
            for (int i = 0; i < value_of_no.Count; i++)
            {
                for (int j = 0; j < value_of_no.Count; j++)
                {
                    if (Convert.ToDouble(value_of_no[i]) > (Convert.ToDouble(value_of_no[j])))
                    {
                        double temp_point = Convert.ToDouble(value_of_no[i]);
                        value_of_no[i] = value_of_no[j];
                        value_of_no[j] = temp_point;
                    }
                }
            }
            //return ;
        }


        public class sort_by_y_value_asc : IComparer<TSG.Point>
        {
            public int Compare(TSG.Point a, TSG.Point b)
            {
                if (a.Y > b.Y)
                {
                    return 1;
                }
                if (a.Y < b.Y)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public class sort_by_x_value_max : IComparer<TSG.Point>
        {
            public int Compare(TSG.Point a, TSG.Point b)
            {
                if (a.X > b.X)
                {
                    return 1;
                }
                if (a.X < b.X)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public class sort_by_x_value_min : IComparer<TSG.Point>
        {
            public int Compare(TSG.Point a, TSG.Point b)
            {


                if (a.X < b.X)
                {
                    return 1;
                }
                if (a.X > b.X)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public class sort_by_x_value_DOUBLE : IComparer<double>
        {
            public int Compare(double a, double b)
            {


                if (a < b)
                {
                    return 1;
                }
                if (a > b)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
