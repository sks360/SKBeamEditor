using SK.Tekla.Drawing.Automation.Handlers;
using System;
using System.Collections;
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
using Tekla.Structures.Model;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;

using Tekla.Structures.Drawing.Operations;


namespace SK.Tekla.Drawing.Automation.Drawing
{
    public class DuplicateRemover
    {
        private static readonly log4net.ILog _logger =
       log4net.LogManager.GetLogger(typeof(DuplicateRemover));

        private SKSortingHandler sortingHandler;

        public DuplicateRemover(SKSortingHandler sortingHandler)
        {
            this.sortingHandler = sortingHandler;
        }

        public TSD.PointList RemoveDuplicateXValues(TSD.PointList ptList)
        {
            if (ptList == null || ptList.Count == 0)
            {
                return ptList;
            }

            var points = ptList.Cast<TSG.Point>()
                               .GroupBy(p => (long)p.X)
                               .Select(g => g.OrderBy(p => p.Y).First())
                               .ToList();

            TSD.PointList uniquePoints = new TSD.PointList();
            foreach (var point in points)
            {
                uniquePoints.Add(point);
            }
            _logger.Info(uniquePoints.ToString());
            return uniquePoints;

            //if(ptlist == null || ptlist.Count == 0)
            //{
            //    return ptlist;
            //}
            //TSD.PointList pointlist_deleted = new TSD.PointList();
            ///////////////////////////////////////////sorting the ptlist by x descending/////////////////////////////////////////////////////////////////////////////////////////////////////
            //sortingHandler.SortPoints(ptlist, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
            //ArrayList list_of_X = new ArrayList();
            ///////////////////////////////////////////getting unique X values/////////////////////////////////////////////////////////////////////////////////////////////////////
            //for (int i = 0; i < ptlist.Count; i++)
            //{
            //    double X_value = (ptlist[i] as TSG.Point).X;
            //    if (i < ptlist.Count - 1)
            //    {
            //        if (Convert.ToInt32((ptlist[i + 1].X)) != Convert.ToInt32((ptlist[i].X)))
            //        {
            //            list_of_X.Add(ptlist[i].X);
            //        }
            //    }
            //    else
            //    {
            //        list_of_X.Add(ptlist[i].X);
            //    }
            //}
            //TSD.PointList finaldimpts = new TSD.PointList();
            //foreach (double Xvalue in list_of_X)
            //{
            //    TSD.PointList allpts = new TSD.PointList();
            //    for (int i = 0; i < ptlist.Count; i++)
            //    {
            //        if (Convert.ToInt64(ptlist[i].X) == Convert.ToInt64(Xvalue))
            //        {
            //            allpts.Add(ptlist[i]);
            //        }
            //    }
            //    sortingHandler.SortPoints(allpts, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
            //    finaldimpts.Add(allpts[allpts.Count - 1]);
            //}
            //_logger.Info(finaldimpts.ToString());
            //return finaldimpts;

        }

        public TSD.PointList RemoveDuplicateYValues(TSD.PointList ptList)
        {
            if (ptList == null || ptList.Count == 0)
            {
                return ptList;
            }

            var points = ptList.Cast<TSG.Point>()
                               .GroupBy(p => (long)p.Y)
                               .Select(g => g.OrderByDescending(p => p.X).First())
                               .ToList();

            TSD.PointList uniquePoints = new TSD.PointList();
            foreach (var point in points)
            {
                uniquePoints.Add(point);
            }
            return uniquePoints;

            //TSD.PointList pointlist_deleted = new TSD.PointList();
            //sortingHandler.SortPoints(ptlist, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
            //ArrayList list_of_y = new ArrayList();
            //for (int i = 0; i < ptlist.Count; i++)
            //{
            //    double y_value = (ptlist[i] as TSG.Point).Y;

            //    if (i < ptlist.Count - 1)
            //    {
            //        if (Convert.ToInt32((ptlist[i + 1].Y)) != Convert.ToInt32((ptlist[i].Y)))
            //        {
            //            list_of_y.Add(ptlist[i].Y);
            //        }
            //    }
            //    else
            //    {
            //        list_of_y.Add(ptlist[i].Y);
            //    }

            //}
            //TSD.PointList finaldimpts = new TSD.PointList();
            //foreach (double yvalue in list_of_y)
            //{
            //    TSD.PointList allpts = new TSD.PointList();
            //    for (int i = 0; i < ptlist.Count; i++)
            //    {
            //        if ((ptlist[i].Y) == yvalue)
            //        {
            //            allpts.Add(ptlist[i]);

            //        }
            //    }
            //    sortingHandler.SortPoints(allpts, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
            //    finaldimpts.Add(allpts[0]);
            //}
            //return finaldimpts;
        }

        /// <summary>
        /// Deletes duplicate straight dimensions in section views of an assembly drawing.
        /// </summary>
        /// <param name="assemblyDrawing">The assembly drawing to process.</param>
        /// <param name="myHandler">The drawing handler for selection and modification.</param>
        public void DeleteDuplicateSectionViews(TSD.AssemblyDrawing assemblyDrawing, TSD.DrawingHandler myHandler)
        {
            if (assemblyDrawing == null || myHandler == null)
            {
                throw new ArgumentNullException("Assembly drawing or drawing handler cannot be null.");
            }

            TSD.DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllViews();
            while (drawingObjectEnumerator.MoveNext())
            {
                TSD.View view = drawingObjectEnumerator.Current as TSD.View;
                if (view?.ViewType != TSD.View.ViewTypes.SectionView)
                {
                    continue;
                }
                Type dimensionType = typeof(TSD.StraightDimension);
                List<TSD.StraightDimension> dimList = new List<TSD.StraightDimension>();
                List<TSD.StraightDimension> dimsToBeDeleted = new List<TSD.StraightDimension>();
                TSD.DrawingObjectEnumerator dimensions = view.GetAllObjects(dimensionType);

                while (dimensions.MoveNext())
                {
                    TSD.StraightDimension straightDimension = dimensions.Current as TSD.StraightDimension;
                    if (straightDimension == null) continue;

                    if (dimList.Any(temp => AreDimensionsSimilar(temp, straightDimension)))
                    {
                        dimsToBeDeleted.Add(straightDimension);
                    }
                    else
                    {
                        dimList.Add(straightDimension);
                    }
                }
                ArrayList dimArr = new ArrayList();
                dimArr.AddRange(dimsToBeDeleted);
                myHandler.GetDrawingObjectSelector().SelectObjects(dimArr, true);
                view.Modify();
                assemblyDrawing.CommitChanges();
                TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
                myHandler.GetDrawingObjectSelector().UnselectAllObjects();
                view.Modify();
            }
            assemblyDrawing.CommitChanges();
        }

        /// <summary>
        /// Determines if two straight dimensions are similar based on up direction and proximity of start/end points.
        /// </summary>
        /// <param name="d1">First dimension to compare.</param>
        /// <param name="d2">Second dimension to compare.</param>
        /// <returns>True if dimensions are similar, false otherwise.</returns>
        private bool AreDimensionsSimilar(TSD.StraightDimension d1, TSD.StraightDimension d2)
        {
            double vectXDiff = d1.UpDirection.X - d2.UpDirection.X;
            double vectYDiff = d1.UpDirection.Y - d2.UpDirection.Y;
            double vectZDiff = d1.UpDirection.Z - d2.UpDirection.Z;
            double startXDiff = Math.Abs(Math.Round(d1.StartPoint.X, 2) - Math.Round(d2.StartPoint.X, 2));
            double startYDiff = Math.Abs(Math.Round(d1.StartPoint.Y, 2) - Math.Round(d2.StartPoint.Y, 2));
            double endXDiff = Math.Abs(Math.Round(d1.EndPoint.X, 2) - Math.Round(d2.EndPoint.X, 2));
            double endYDiff = Math.Abs(Math.Round(d1.EndPoint.Y, 2) - Math.Round(d2.EndPoint.Y, 2));
            return vectXDiff == 0 && vectYDiff == 0 && vectZDiff == 0 &&
                   startXDiff <= 0.5 && startYDiff <= 0.5 &&
                   endXDiff <= 0.5 && endYDiff <= 0.5;
        }

        //public void delete_sec_view_same_dims(TSD.AssemblyDrawing assemblyDrawing, TSD.DrawingHandler my_handler)
        //{
        //    TSD.DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllViews();
        //    while (drawingObjectEnumerator.MoveNext())
        //    {
        //        TSD.View view = drawingObjectEnumerator.Current as TSD.View;
        //        if (view.ViewType == TSD.View.ViewTypes.SectionView)
        //        {
        //            Type type_for_dimension = typeof(TSD.StraightDimension);
        //            ArrayList dim_list = new ArrayList();
        //            ArrayList Dims_to_be_deleted = new ArrayList();
        //            TSD.DrawingObjectEnumerator dimensions = view.GetAllObjects(type_for_dimension);

        //            while (dimensions.MoveNext())
        //            {
        //                TSD.StraightDimension straightDimension = dimensions.Current as TSD.StraightDimension;

        //                if (dim_list.Count == 0)
        //                {
        //                    dim_list.Add(straightDimension);
        //                }
        //                else
        //                {
        //                    bool found = false;
        //                    foreach (TSD.StraightDimension temp_dimension in dim_list)
        //                    {
        //                        double vect_x_diff = temp_dimension.UpDirection.X - straightDimension.UpDirection.X;
        //                        double vect_y_diff = temp_dimension.UpDirection.Y - straightDimension.UpDirection.Y;
        //                        double vect_z_diff = temp_dimension.UpDirection.Z - straightDimension.UpDirection.Z;
        //                        double start_X_diff = Math.Abs(Math.Round(temp_dimension.StartPoint.X, 2) - Math.Round(straightDimension.StartPoint.X, 2));
        //                        double start_Y_diff = Math.Abs(Math.Round(temp_dimension.StartPoint.Y, 2) - Math.Round(straightDimension.StartPoint.Y, 2));
        //                        double end_X_diff = Math.Abs(Math.Round(temp_dimension.EndPoint.X, 2) - Math.Round(straightDimension.EndPoint.X, 2));
        //                        double end_Y_diff = Math.Abs(Math.Round(temp_dimension.EndPoint.Y, 2) - Math.Round(straightDimension.EndPoint.Y, 2));
        //                        if (vect_x_diff == 0 && vect_y_diff == 0 && vect_z_diff == 0 && start_X_diff <= 0.5 && start_Y_diff <= 0.5 && end_X_diff <= 0.5 && end_Y_diff <= 0.5)
        //                        {
        //                            found = true;
        //                            Dims_to_be_deleted.Add(straightDimension);
        //                            break;
        //                        }
        //                    }
        //                    if (found == false)
        //                    {
        //                        dim_list.Add(straightDimension);
        //                    }
        //                }
        //            }
        //            my_handler.GetDrawingObjectSelector().SelectObjects(Dims_to_be_deleted, true);
        //            view.Modify();
        //            assemblyDrawing.CommitChanges();
        //            TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
        //            my_handler.GetDrawingObjectSelector().UnselectAllObjects();
        //            view.Modify();
        //        }
        //    }
        //    assemblyDrawing.CommitChanges();

        //}

    }
}
