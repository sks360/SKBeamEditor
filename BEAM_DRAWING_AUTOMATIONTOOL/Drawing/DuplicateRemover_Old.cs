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
    public class DuplicateRemover_Old
    {
        private SKSortingHandler sortingHandler;

        public DuplicateRemover_Old(SKSortingHandler sortingHandler)
        {
            this.sortingHandler = sortingHandler;
        }

        public TSD.PointList pointlist_remove_duplicate_Xvalues(TSD.PointList ptlist)
        {
            TSD.PointList pointlist_deleted = new TSD.PointList();
            /////////////////////////////////////////sorting the ptlist by x descending/////////////////////////////////////////////////////////////////////////////////////////////////////
            sortingHandler.SortPoints(ptlist, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
            ArrayList list_of_X = new ArrayList();
            /////////////////////////////////////////getting unique X values/////////////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < ptlist.Count; i++)
            {
                double X_value = (ptlist[i] as TSG.Point).X;
                if (i < ptlist.Count - 1)
                {
                    if (Convert.ToInt32((ptlist[i + 1].X)) != Convert.ToInt32((ptlist[i].X)))
                    {
                        list_of_X.Add(ptlist[i].X);
                    }
                }
                else
                {
                    list_of_X.Add(ptlist[i].X);
                }
            }
            TSD.PointList finaldimpts = new TSD.PointList();
            foreach (double Xvalue in list_of_X)
            {
                TSD.PointList allpts = new TSD.PointList();
                for (int i = 0; i < ptlist.Count; i++)
                {
                    if (Convert.ToInt64(ptlist[i].X) == Convert.ToInt64(Xvalue))
                    {
                        allpts.Add(ptlist[i]);
                    }
                }
                sortingHandler.SortPoints(allpts, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
                finaldimpts.Add(allpts[allpts.Count - 1]);
            }
            return finaldimpts;

        }

        public TSD.PointList pointlist_remove_duplicate_Yvalues(TSD.PointList ptlist)
        {
            TSD.PointList pointlist_deleted = new TSD.PointList();
            sortingHandler.SortPoints(ptlist, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
            ArrayList list_of_y = new ArrayList();
            for (int i = 0; i < ptlist.Count; i++)
            {
                double y_value = (ptlist[i] as TSG.Point).Y;

                if (i < ptlist.Count - 1)
                {
                    if (Convert.ToInt32((ptlist[i + 1].Y)) != Convert.ToInt32((ptlist[i].Y)))
                    {
                        list_of_y.Add(ptlist[i].Y);
                    }
                }
                else
                {
                    list_of_y.Add(ptlist[i].Y);
                }

            }
            TSD.PointList finaldimpts = new TSD.PointList();
            foreach (double yvalue in list_of_y)
            {
                TSD.PointList allpts = new TSD.PointList();
                for (int i = 0; i < ptlist.Count; i++)
                {
                    if ((ptlist[i].Y) == yvalue)
                    {
                        allpts.Add(ptlist[i]);

                    }
                }
                sortingHandler.SortPoints(allpts, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                finaldimpts.Add(allpts[0]);
            }
            return finaldimpts;
        }

        public void delete_sec_view_same_dims(TSD.AssemblyDrawing assemblyDrawing, TSD.DrawingHandler my_handler)
        {
            TSD.DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllViews();
            while (drawingObjectEnumerator.MoveNext())
            {
                TSD.View view = drawingObjectEnumerator.Current as TSD.View;
                if (view.ViewType == TSD.View.ViewTypes.SectionView)
                {
                    Type type_for_dimension = typeof(TSD.StraightDimension);
                    ArrayList dim_list = new ArrayList();
                    ArrayList Dims_to_be_deleted = new ArrayList();
                    TSD.DrawingObjectEnumerator dimensions = view.GetAllObjects(type_for_dimension);

                    while (dimensions.MoveNext())
                    {
                        TSD.StraightDimension straightDimension = dimensions.Current as TSD.StraightDimension;

                        if (dim_list.Count == 0)
                        {
                            dim_list.Add(straightDimension);
                        }
                        else
                        {
                            bool found = false;
                            foreach (TSD.StraightDimension temp_dimension in dim_list)
                            {
                                double vect_x_diff = temp_dimension.UpDirection.X - straightDimension.UpDirection.X;
                                double vect_y_diff = temp_dimension.UpDirection.Y - straightDimension.UpDirection.Y;
                                double vect_z_diff = temp_dimension.UpDirection.Z - straightDimension.UpDirection.Z;
                                double start_X_diff = Math.Abs(Math.Round(temp_dimension.StartPoint.X, 2) - Math.Round(straightDimension.StartPoint.X, 2));
                                double start_Y_diff = Math.Abs(Math.Round(temp_dimension.StartPoint.Y, 2) - Math.Round(straightDimension.StartPoint.Y, 2));
                                double end_X_diff = Math.Abs(Math.Round(temp_dimension.EndPoint.X, 2) - Math.Round(straightDimension.EndPoint.X, 2));
                                double end_Y_diff = Math.Abs(Math.Round(temp_dimension.EndPoint.Y, 2) - Math.Round(straightDimension.EndPoint.Y, 2));
                                if (vect_x_diff == 0 && vect_y_diff == 0 && vect_z_diff == 0 && start_X_diff <= 0.5 && start_Y_diff <= 0.5 && end_X_diff <= 0.5 && end_Y_diff <= 0.5)
                                {
                                    found = true;
                                    Dims_to_be_deleted.Add(straightDimension);
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                dim_list.Add(straightDimension);
                            }
                        }
                    }
                    my_handler.GetDrawingObjectSelector().SelectObjects(Dims_to_be_deleted, true);
                    view.Modify();
                    assemblyDrawing.CommitChanges();
                    TSM.Operations.Operation.RunMacro(@"..\drawings\acmd_delete_selected_dr.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    view.Modify();
                }
            }
            assemblyDrawing.CommitChanges();

        }


    }
}
