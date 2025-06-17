using SK.Tekla.Drawing.Automation.Handlers;
using SK.Tekla.Drawing.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;
using TSS = Tekla.Structures.Solid;

using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class SKGussetDimension
    {
        private readonly CustomInputModel _userInput;

        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private const double DimDistance = 100;
        private const double AngleDimDistance = 200;
        private const double ViewScaleFactor = 1.0; // Adjust based on actual view scale
        private const double Tolerance = 1e-6;

        public SKGussetDimension(SKCatalogHandler catalogHandler, 
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler;
            this.boltMatrixHandler = boltMatrixHandler;
            this.boundingBoxHandler = boundingBoxHandler;
            this.sortingHandler = sortingHandler;
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }

        public void GussetDimensionsWithBolts(
              Beam mainPart,
              TSD.View currentView,
              ref List<Guid> partMarkToRetain,
              ref List<Guid> boltMarkToRetain,
              string drawingAttributes)
        {
            if (mainPart == null || currentView == null)
                throw new ArgumentNullException("mainPart or currentView cannot be null.");

            StraightDimensionSet.StraightDimensionSetAttributes dimAttributes =
                new StraightDimensionSet.StraightDimensionSetAttributes
                {
                    Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed },
                    Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
                };

            AngleDimensionAttributes angleDimAttributes = new AngleDimensionAttributes
            {
                Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed },
                Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
            };

            StraightDimensionSet.StraightDimensionSetAttributes outsideAttributes =
                new StraightDimensionSet.StraightDimensionSetAttributes
                {
                    Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed },
                    ExtensionLine = DimensionSetBaseAttributes.ExtensionLineTypes.No,
                    ShortDimension = TSD.DimensionSetBaseAttributes.ShortDimensionTypes.Inside,
                    Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
                };

            StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes =
                new StraightDimensionSet.StraightDimensionSetAttributes
                {
                    Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed },
                    ExtensionLine = DimensionSetBaseAttributes.ExtensionLineTypes.No,
                    Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
                };

            List<double> catalogValues = catalogHandler.Getcatalog_values(mainPart);
            double topFront = currentView.ViewType == TSD.View.ViewTypes.FrontView ? catalogValues[0] : catalogValues[2];

            TSM.Model model = new TSM.Model();
            DrawingHandler drawingHandler = new DrawingHandler();
            StraightDimensionSetHandler dimSetHandler = new StraightDimensionSetHandler();
            GussetBoltPoints gussetBoltPoints = new GussetBoltPoints();
            DrawingObjectEnumerator partEnumerator = currentView.GetAllObjects(typeof(TSD.Part));
            while (partEnumerator.MoveNext())
            {
                GussetBoltList gussetBoltList = new GussetBoltList();
                
                TSD.Part myPart = partEnumerator.Current as TSD.Part;
                if (myPart == null) continue;

                TSM.Part plate = model.SelectModelObject(myPart.ModelIdentifier) as TSM.Part;
                if (plate == null) continue;

                string profileType = "";
                plate.GetReportProperty("PROFILE_TYPE", ref profileType);
                if (profileType != "B") continue;

                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(currentView.ViewCoordinateSystem));
                Vector xVectorPlate = plate.GetCoordinateSystem().AxisX;
                Vector yVectorPlate = plate.GetCoordinateSystem().AxisY;
                Vector zVectorPlate = Vector.Cross(xVectorPlate, yVectorPlate);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());

                PointList boundingBoxX = boundingBoxHandler.BoundingBoxSort(plate, currentView);
                PointList boundingBoxY = boundingBoxHandler.BoundingBoxSort(plate, currentView, SKSortingHandler.SortBy.Y);

                PointList plateCornerPoints = GetPlateCornerPoints(boundingBoxX, boundingBoxY);

                //////////////////////Filtering the plates whichis normal to the view coordinate system////
                // if (Math.Abs(zVectorPlate.Z) > Tolerance) 
                if (zVectorPlate.Z != 0) //TODO check if this condition is needed instead of Tolerance!
                {
                    if (boundingBoxY[0].Y >= Convert.ToInt64(topFront / 2))
                    {
                        ///////Filtering for plates which are on the positive side of flange///////////////
                        ProcessGussetPlate(gussetBoltList, plate, currentView, dimSetHandler,
                            angleDimAttributes, fixedAttributes, topFront, true,
                            ref partMarkToRetain, ref boltMarkToRetain);
                    }
                    else if (boundingBoxY[1].Y <= -Convert.ToInt64(topFront / 2))
                    {
                        ///////Filtering for plates which are on the bottom flange///////////////
                        ProcessGussetPlate(gussetBoltList,plate, currentView, dimSetHandler, angleDimAttributes, 
                            fixedAttributes, topFront, false, ref partMarkToRetain, ref boltMarkToRetain);
                    }
                }
                ////////Declaring pointlists for each quadrant dimenrions and for RD///////////////////
                TSD.PointList point_for_dim_top_right = new TSD.PointList();
                TSD.PointList point_for_dim_top_left = new TSD.PointList();
                TSD.PointList point_for_dim_bottom_right = new TSD.PointList();
                TSD.PointList point_for_dim_bottom_left = new TSD.PointList();
                TSD.PointList point_for_vertical_dim_top_right = new TSD.PointList();
                TSD.PointList point_for_vertical_dim_bottom_right = new TSD.PointList();
                TSD.PointList point_for_vertical_dim_top_left = new TSD.PointList();
                TSD.PointList point_for_vertical_dim_bottom_left = new TSD.PointList();


                TopRightBoltList(gussetBoltList,gussetBoltPoints, currentView, drawingAttributes, angleDimAttributes,
               topFront, model, dimSetHandler, 
               boundingBoxX, boundingBoxY, GetPlateCornerPoints(boundingBoxX, boundingBoxY),
               fixedAttributes, outsideAttributes, point_for_dim_top_right,
               point_for_vertical_dim_top_right, currentView.Attributes.Scale);

                TopLeftBoltList(gussetBoltList, gussetBoltPoints, currentView,drawingAttributes,angleDimAttributes,
                    topFront, model, dimSetHandler,
                    boundingBoxX, boundingBoxY, GetPlateCornerPoints(boundingBoxX, boundingBoxY),
                    fixedAttributes,outsideAttributes,point_for_dim_top_left,point_for_dim_bottom_left,
                    point_for_vertical_dim_top_left, currentView.Attributes.Scale);
                /////////////////////////////bottom_left_bolts_list////////////////////////////////////
                BottomLeftBoltlist(gussetBoltList, gussetBoltPoints,
                    currentView, drawingAttributes, topFront, model, dimSetHandler,
                    boundingBoxX, boundingBoxY, GetPlateCornerPoints(boundingBoxX, boundingBoxY),
                    fixedAttributes, outsideAttributes, point_for_dim_bottom_right, 
                    point_for_vertical_dim_bottom_right, currentView.Attributes.Scale);
                //////////////negative_down_bolts_list/////////////////////////////////////////////
                BottomRightBoltList(gussetBoltList, gussetBoltPoints,
                    currentView, drawingAttributes, topFront, model, dimSetHandler,
                    boundingBoxX, boundingBoxY, GetPlateCornerPoints(boundingBoxX, boundingBoxY),
                    fixedAttributes, outsideAttributes, point_for_dim_bottom_left, 
                    point_for_vertical_dim_bottom_left, currentView.Attributes.Scale);

            }


            CreateRDDimensions(gussetBoltPoints, currentView, dimAttributes, dimSetHandler);
        }

        private void CreateRDDimensions(GussetBoltPoints gussetBoltPoints, TSD.View currentView, 
            StraightDimensionSet.StraightDimensionSetAttributes dimAttributes, 
            StraightDimensionSetHandler dimSetHandler)
        {
            // Create RD dimensions
            try
            {
                PointList finalPtListForRdTop = gussetBoltPoints.finalPtListForRdTop;
                double distanceTop = Math.Abs(Math.Abs(finalPtListForRdTop[0].Y) - currentView.RestrictionBox.MaxPoint.Y);
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdTop, new Vector(0, 1, 0), distanceTop + 200, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for top: {ex.Message}");
            }

            try
            {
                PointList finalPtListForRdBottom = gussetBoltPoints.finalPtListForRdBottom;
                double distanceBottom = Math.Abs(Math.Abs(finalPtListForRdBottom[0].Y) - currentView.RestrictionBox.MinPoint.Y);
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdBottom, new Vector(0, -1, 0), distanceBottom + 100, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for bottom: {ex.Message}");
            }

            try
            {
                PointList finalPtListForRdTopNoBolt = gussetBoltPoints.finalPtListForRdTopNoBolt;
                double distance = 500;
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdTopNoBolt, new Vector(0, -1, 0), distance + 100, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for top with no bolts: {ex.Message}");
            }

            try
            {
                PointList finalPtListForRdBottomNoBolt = gussetBoltPoints.finalPtListForRdBottomNoBolt;
                double distance = 500;
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdBottomNoBolt, new Vector(0, -1, 0), distance + 100, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for bottom with no bolts: {ex.Message}");
            }
        }

        private void ProcessGussetPlate(
            GussetBoltList gussetBoltList,
            TSM.Part plate,
            TSD.View currentView,
            StraightDimensionSetHandler dimSetHandler,
            AngleDimensionAttributes angleDimAttributes,
            StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes,
            double topFront,
            bool isTopFlange,
            ref List<Guid> partMarkToRetain,
            ref List<Guid> boltMarkToRetain)
        {
            TSM.Model model = new TSM.Model();
            TSM.ModelObjectEnumerator boltEnumerator = plate.GetBolts();
            if (boltEnumerator.GetSize() > 0)
            {
                while (boltEnumerator.MoveNext())
                {
                    BoltGroup bolt = boltEnumerator.Current as BoltGroup;
                    if (bolt != null && bolt.BoltType == BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE)
                    {
                        Point[,] pointMatrix = boltMatrixHandler.
                            GetBoltMatrixForGusset(bolt, currentView, "x_asc");
                        
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(currentView.ViewCoordinateSystem));
                        TSG.Vector vector_for_sep = new TSG.Vector(bolt.GetCoordinateSystem().AxisX);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        /////////Filtering the bolts into four quadrants... Top right, top left, bottom right and bottom left///////
                        if (((vector_for_sep.X < 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X > 0) && (vector_for_sep.Y > 0)))
                        {
                            if (isTopFlange)
                            {
                                gussetBoltList.top_right_bolts_list.Add(bolt);
                            }
                            else
                            {
                                gussetBoltList.bottom_left_bolts_list.Add(bolt);
                            }
                                
                        }
                        else if (((vector_for_sep.X > 0) && (vector_for_sep.Y < 0)) || ((vector_for_sep.X < 0) && (vector_for_sep.Y > 0)))
                        {
                            if (isTopFlange)
                            {
                                gussetBoltList.top_left_bolts_list.Add(bolt);
                            }
                            else
                            {
                                gussetBoltList.bottom_right_bolts_list.Add(bolt);
                            }
                        }
                        else
                        {
                            if (isTopFlange)
                            {
                                gussetBoltList.vertical_bolt_top_list.Add(bolt);
                            }
                            else
                            {
                                gussetBoltList.vertical_bolt_bottom_list.Add(bolt);
                            }
                        }
                      

                        partMarkToRetain.Add(plate.Identifier.GUID);
                        boltMarkToRetain.Add(bolt.Identifier.GUID);
                    }
                }
            }
            else if (plate is TSM.ContourPlate contourPlate)
            {
                PointList listOfPoints = sortingHandler.SortPoints(GetGussetPlatePoints(contourPlate, currentView));
                //TODO Not clear on the count
                if (listOfPoints.Count == 6)
                {
                    PointList ptList1 = new PointList { listOfPoints[0], listOfPoints[1] };
                    PointList ptList2 = new PointList { listOfPoints[4], listOfPoints[5] };
                    dimSetHandler.CreateDimensionSet(currentView, ptList1, new Vector(-1, 0, 0), DimDistance, fixedAttributes);
                    dimSetHandler.CreateDimensionSet(currentView, ptList2, new Vector(1, 0, 0), DimDistance, fixedAttributes);
                }
            }
        }




        private void TopRightBoltList(GussetBoltList gussetBoltList,
            GussetBoltPoints gussetBoltPoints,
            TSD.View current_view, string drg_att, 
            AngleDimensionAttributes angle_dim_font_height, double top_front, 
            Model model, StraightDimensionSetHandler dim_set_handler, 
           
            PointList bounding_box_x, 
            PointList bounding_box_y, PointList pointlist_plate_corner_points, 
            StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes, 
            StraightDimensionSet.StraightDimensionSetAttributes outSide, 
            PointList point_for_dim_top_right, PointList point_for_vertical_dim_top_right, double view_scale)
        {
            if (gussetBoltList.top_right_bolts_list.Count > 0)
            {
                 
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                ArrayList distances = new ArrayList();
                int ij = 0;
                foreach (TSM.BoltGroup boltarray in gussetBoltList.top_right_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");

                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);

                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];




                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X > point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {
                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (d > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;

                        try
                        {

                            for (int l = 0; l < d; l++)
                            {
                                if (ij == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, angle_dim_font_height);
                                    angledim1.Insert();
                                }
                            }

                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[0, 0] - point_matrix1[1, 0]), new TSG.Vector(0, 0, -1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, -dist_for_dim, outSide);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_top_right.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] + new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 + new TSG.Point(0, y_value, 0);
                        double distance = 200 / view_scale;

                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);




                        //TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], vector_y, new TSG.Vector(1, 0, 0), 200);
                        angledim1.Insert();
                    }




                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    for (int j = 0; j < point_matrix1.GetLength(1); j++)
                    {
                        point_for_dim_top_right.Add(point_matrix1[0, j]);

                    }

                    if (c > 1)
                    {
                        distances.Add(Convert.ToInt16(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0])));
                    }
                    else
                    {
                        distances.Add(20);

                    }
                    ij++;
                }
                sortingHandler.SortPoints(point_for_dim_top_right, SKSortingHandler.SortBy.Y,
                    SKSortingHandler.SortOrder.Descending);
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_top_right.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_top_right[0] - point_for_dim_top_right[point_for_dim_top_right.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_top_right, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 250, fixedAttributes);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////



                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_top_right[0].Y > 0)
                {

                    gussetBoltPoints.finalPtListForRdTop.Add(point_for_dim_top_right[0]);
                }
                else
                {

                    gussetBoltPoints.finalPtListForRdBottom.Add(point_for_dim_top_right[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_top_right.Add(point_for_dim_top_right[0]);
                point_for_vertical_dim_top_right.Add(new TSG.Point(point_for_dim_top_right[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_vertical_dim_top_right[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[3]);
                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_top_right[0].X - bounding_box_x[1].X);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_top_right[0].Y - bounding_box_y[1].Y);
                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_top_right, new TSG.Vector(1, 0, 0), dist_for_vertical_dim + 300, fixedAttributes);
                }
                catch
                {
                }
                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, 1, 0), dist_for_vertical_dim_y + 200, fixedAttributes);
                }
                catch
                {
                }

            }
        }



        private void TopLeftBoltList(GussetBoltList gussetBoltList,GussetBoltPoints gussetBoltPoints,
            TSD.View current_view, string drg_att, 
            AngleDimensionAttributes angle_dim_font_height, double top_front, 
            Model model, StraightDimensionSetHandler dim_set_handler,
             PointList bounding_box_x, 
            PointList bounding_box_y, PointList pointlist_plate_corner_points, 
            StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes, 
            StraightDimensionSet.StraightDimensionSetAttributes outSide, 
            PointList point_for_dim_top_left, PointList point_for_dim_bottom_left, 
            PointList point_for_vertical_dim_top_left, double view_scale)
        {
            if (gussetBoltList.top_left_bolts_list.Count > 0)
            {
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                ArrayList distances = new ArrayList();
                int jk = 0;
                foreach (TSM.BoltGroup boltarray in gussetBoltList.top_left_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");
                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];



                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X > point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {

                        for (int j = 0; j < point_matrix1.GetLength(1); j++)
                        {
                            point_for_dim_top_left.Add(point_matrix1[0, j]);

                        }
                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (point_matrix.GetLength(1) > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;

                        try
                        {
                            for (int l = 0; l < d; l++)
                            {
                                if (jk == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, angle_dim_font_height);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[1, 0] - point_matrix1[0, 0]), new TSG.Vector(0, 0, 1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, outSide);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_top_left.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(-1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] - new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 + new TSG.Point(0, y_value, 0);

                        double distance = 200 / view_scale;


                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);




                        //TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0],new TSG.Vector(1, 0, 0), vector_x,  200, fi);
                        angledim1.Insert();
                    }




                    //point_for_dim_top_left.Add(point_matrix[0, 0]);
                    if (c > 1)
                    {
                        distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                    }
                    else
                    {
                        distances.Add(20);

                    }
                    jk++;

                }
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_top_left.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_top_left[0] - point_for_dim_top_left[point_for_dim_top_left.Count - 1]).Cross(new TSG.Vector(0, 0, 1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_top_left, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 100, fixedAttributes);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_top_left, SKSortingHandler.SortBy.Y, SKSortingHandler.SortOrder.Descending);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_top_left[0].Y > 0)
                {

                    gussetBoltPoints.finalPtListForRdTop.Add(point_for_dim_top_left[0]);
                }
                else
                {

                    gussetBoltPoints.finalPtListForRdBottom.Add(point_for_dim_top_left[0]);
                }


                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_top_left.Add(point_for_dim_top_left[0]);
                point_for_vertical_dim_top_left.Add(new TSG.Point(point_for_dim_top_left[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_dim_top_left[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[2]);

                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_top_left[0].X - bounding_box_x[0].X);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_top_left[0].Y - bounding_box_y[0].Y);
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fix = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fix.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fix.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fix.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_top_left, new TSG.Vector(-1, 0, 0), dist_for_vertical_dim + 300, fix);
                }
                catch
                {

                }

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, 1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {
                }

            }
        }

        private void BottomLeftBoltlist(GussetBoltList gussetBoltList, GussetBoltPoints gussetBoltPoints,
            TSD.View current_view, string drg_att, 
            double top_front, Model model, StraightDimensionSetHandler dim_set_handler,
            PointList bounding_box_x,
             PointList bounding_box_y, PointList pointlist_plate_corner_points,
             StraightDimensionSet.StraightDimensionSetAttributes fixedAttributes, 
             StraightDimensionSet.StraightDimensionSetAttributes outSide, 
             PointList point_for_dim_bottom_right, PointList point_for_vertical_dim_bottom_right, double view_scale)
        {
            if (gussetBoltList.bottom_left_bolts_list.Count > 0)
            {
                ArrayList distances = new ArrayList();
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                int kl = 0;
                foreach (TSM.BoltGroup boltarray in gussetBoltList.bottom_left_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");
                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];
                    ///Adding first row points of all bolt enum in the pointlist//////////////////////




                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X < point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }
                    if (c > 1)
                    {
                        for (int j = 0; j < point_matrix1.GetLength(1); j++)
                        {
                            point_for_dim_bottom_right.Add(point_matrix1[0, j]);

                        }

                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);

                        }
                        double dist_for_dim = 100;
                        if (point_matrix.GetLength(1) > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }
                        if (c > 1)
                        {
                            distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                        }
                        else
                        {
                            distances.Add(20);

                        }
                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;
                        try
                        {
                            TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                            fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                            for (int l = 0; l < d; l++)
                            {
                                if (kl == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, fi);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[1, 0] - point_matrix1[0, 0]), new TSG.Vector(0, 0, -1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, outSide);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        point_for_dim_bottom_right.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(-1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] - new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 - new TSG.Point(0, y_value, 0);

                        double distance = 200 / view_scale;


                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);

                        angledim1.Insert();
                    }


                    kl++;

                }

                sortingHandler.SortPoints(point_for_dim_bottom_right, SKSortingHandler.SortBy.Y);
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);
                if (point_for_dim_bottom_right.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_bottom_right[0] - point_for_dim_bottom_right[point_for_dim_bottom_right.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_bottom_right, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 300, fixedAttributes);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_bottom_right, SKSortingHandler.SortBy.Y);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_bottom_right[0].Y > 0)
                {

                    gussetBoltPoints.finalPtListForRdTop.Add(point_for_dim_bottom_right[0]);
                }
                else
                {

                    gussetBoltPoints.finalPtListForRdBottom.Add(point_for_dim_bottom_right[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////

                point_for_vertical_dim_bottom_right.Add(point_for_dim_bottom_right[0]);
                point_for_vertical_dim_bottom_right.Add(new TSG.Point(point_for_dim_bottom_right[0].X, top_front / 2, 0));
                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_bottom_right[0].X - bounding_box_y[0].X);
                pointlist_for_lock.Add(point_for_vertical_dim_bottom_right[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[1]);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_bottom_right[0].Y - bounding_box_y[0].Y);

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_bottom_right, new TSG.Vector(-1, 0, 0), dist_for_vertical_dim + 200, fixe);
                }
                catch
                {

                }

                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, -1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {

                }
            }
        }

        private void BottomRightBoltList(GussetBoltList gussetBoltList, GussetBoltPoints gussetBoltPoints,
            TSD.View current_view, string drg_att, double top_front, 
            Model model, StraightDimensionSetHandler dim_set_handler, 
             PointList bounding_box_x, PointList bounding_box_y, PointList pointlist_plate_corner_points, 
             StraightDimensionSet.StraightDimensionSetAttributes fixedattr, 
             StraightDimensionSet.StraightDimensionSetAttributes OUSIDE, 
             PointList point_for_dim_bottom_left, PointList point_for_vertical_dim_bottom_left, double view_scale)
        {
            if (gussetBoltList.bottom_right_bolts_list.Count > 0)
            {
                ArrayList distances = new ArrayList();
                TSD.PointList pointlist_for_lock = new TSD.PointList();
                int lm = 0;
                foreach (TSM.BoltGroup boltarray in gussetBoltList.bottom_right_bolts_list)
                {
                    TSD.PointList point_for_33_dim = new TSD.PointList();
                    //////////Getting point matrix for each bolt array/////////////
                    TSG.Point[,] point_matrix = boltMatrixHandler.GetBoltMatrixForGusset(boltarray, current_view, "x_asc");

                    int c = point_matrix.GetLength(0);
                    int d = point_matrix.GetLength(1);
                    TSG.Point[,] point_matrix1 = new TSG.Point[c, d];

                    //point_matrix1 = point_matrix;
                    ///Flipping bolt matrix left right vs right left in case of top right and bottom left only; d>1 condition-flipping only in case of more than one rows////////////////
                    if (d > 1)
                    {





                    }



                    if (d > 1)
                    {
                        if (point_matrix[0, 0].X < point_matrix[0, 1].X)
                        {
                            for (int i = 0; i < point_matrix.GetLength(0); i++)
                            {
                                for (int j = 0; j < point_matrix.GetLength(1); j++)
                                {
                                    point_matrix1[i, j] = point_matrix[i, d - j - 1];

                                }

                            }
                        }
                        else
                        {
                            point_matrix1 = point_matrix;
                        }

                    }
                    else
                    {
                        point_matrix1 = point_matrix;
                    }

                    if (c > 1)
                    {

                        for (int k = 0; k < point_matrix1.GetLength(0); k++)
                        {

                            point_for_33_dim.Add(point_matrix1[k, 0]);


                        }
                        double dist_for_dim = 100;
                        if (d > 1)
                        {
                            dist_for_dim = TSG.Distance.PointToPoint(point_matrix1[0, 0], point_matrix1[0, d - 1]) + 100;
                        }

                        double dist_for_anglular_dim = (TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]) + 450) / view_scale;
                        if (c > 1)
                        {
                            distances.Add(TSG.Distance.PointToPoint(point_matrix1[c - 1, 0], point_matrix1[0, 0]));
                        }
                        else
                        {
                            distances.Add(20);

                        }
                        try
                        {
                            TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                            fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                            fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                            for (int l = 0; l < d; l++)
                            {
                                if (lm == 0)
                                {
                                    TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[c - 1, l], point_matrix1[0, l], new TSG.Point(point_matrix1[c - 1, l].X, point_matrix1[0, l].Y, 0), dist_for_anglular_dim, fi);
                                    angledim1.Insert();
                                }
                            }
                            TSG.Vector vector_for_dim = TSG.Vector.Cross(new TSG.Vector(point_matrix1[0, 0] - point_matrix1[c - 1, 0]), new TSG.Vector(0, 0, 1));
                            dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_33_dim, vector_for_dim, dist_for_dim, OUSIDE);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        TSD.AngleDimensionAttributes fi = new TSD.AngleDimensionAttributes();
                        fi.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                        fi.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                        point_for_dim_bottom_left.Add(point_matrix1[0, 0]);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(current_view.ViewCoordinateSystem));
                        TSG.Vector vector_x = new TSG.Vector(boltarray.GetCoordinateSystem().AxisX);
                        TSG.Vector vector_y = new TSG.Vector(boltarray.GetCoordinateSystem().AxisY);
                        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


                        double angle = Math.Abs((vector_x.GetAngleBetween(new TSG.Vector(1, 0, 0))));
                        double y_value = Math.Tan(angle) * 100;
                        TSG.Point p1 = point_matrix1[0, 0] + new TSG.Point(100, 0, 0);
                        TSG.Point p2 = p1 - new TSG.Point(0, y_value, 0);
                        double distance = 200 / view_scale;

                        TSD.AngleDimension angledim1 = new TSD.AngleDimension(current_view as TSD.ViewBase, point_matrix1[0, 0], p2, p1, distance, fi);

                        angledim1.Insert();
                    }

                    ///Adding first row points of all bolt enum in the pointlist//////////////////////
                    for (int j = 0; j < point_matrix1.GetLength(1); j++)
                    {
                        point_for_dim_bottom_left.Add(point_matrix1[0, j]);

                    }

                    lm++;

                }
                sortingHandler.SortPoints(distances, SKSortingHandler.SortBy.X, SKSortingHandler.SortOrder.Descending);

                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.Y);
                if (point_for_dim_bottom_left.Count > 1)
                {
                    try
                    {
                        TSG.Vector vector_for_dim_pitch = new TSG.Vector(point_for_dim_bottom_left[0] - point_for_dim_bottom_left[point_for_dim_bottom_left.Count - 1]).Cross(new TSG.Vector(0, 0, -1));
                        vector_for_dim_pitch.Normalize();
                        dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_dim_bottom_left, vector_for_dim_pitch, Convert.ToDouble(distances[distances.Count - 1]) + 300, fixedattr);
                    }
                    catch
                    {
                    }
                }
                //Sorting the points and getting the topmost point for all dimensions//////////////////
                sortingHandler.SortPoints(point_for_dim_bottom_left, SKSortingHandler.SortBy.Y);
                //////////Adding the topmost point in the RD- top bottom depending on top/bottom flange gusset/////////////////
                if (point_for_dim_bottom_left[0].Y > 0)
                {

                    gussetBoltPoints.finalPtListForRdTop.Add(point_for_dim_bottom_left[0]);
                }
                else
                {
                    gussetBoltPoints.finalPtListForRdBottom.Add(point_for_dim_bottom_left[0]);
                }

                ////////////////////Creating vertical dimension for the topmost point///////////////////////
                point_for_vertical_dim_bottom_left.Add(point_for_dim_bottom_left[0]);
                point_for_vertical_dim_bottom_left.Add(new TSG.Point(point_for_dim_bottom_left[0].X, top_front / 2, 0));
                pointlist_for_lock.Add(point_for_dim_bottom_left[0]);
                pointlist_for_lock.Add(pointlist_plate_corner_points[0]);
                double dist_for_vertical_dim_y = Math.Abs(point_for_vertical_dim_bottom_left[0].Y - bounding_box_y[1].Y);



                double dist_for_vertical_dim = Math.Abs(point_for_vertical_dim_bottom_left[0].X - bounding_box_x[1].X);
                try
                {
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, point_for_vertical_dim_bottom_left, new TSG.Vector(1, 0, 0), dist_for_vertical_dim + 300, fixedattr);
                }
                catch
                {
                }
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes fixe = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    fixe.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    fixe.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    fixe.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dim_set_handler.CreateDimensionSet(current_view as TSD.ViewBase, pointlist_for_lock, new TSG.Vector(0, -1, 0), dist_for_vertical_dim_y + 200, fixe);
                }
                catch
                {
                }

            }
        }



        private PointList GetPlateCornerPoints(PointList xSortedBoundingBox, PointList ySortedBoundingBox)
        {
            PointList cornerPoints = new PointList
            {
                new Point(xSortedBoundingBox[1].X, ySortedBoundingBox[1].Y),
                new Point(xSortedBoundingBox[0].X, ySortedBoundingBox[1].Y),
                new Point(xSortedBoundingBox[0].X, ySortedBoundingBox[0].Y),
                new Point(xSortedBoundingBox[1].X, ySortedBoundingBox[0].Y)
            };
            return cornerPoints;
        }

        private PointList GetGussetPlatePoints(ContourPlate gusset, TSD.View currentView)
        {
            Matrix toViewMatrix = MatrixFactory.ToCoordinateSystem(currentView.ViewCoordinateSystem);
            PointList ptListGussetPts = new PointList();
            foreach (Point pt in gusset.Contour.ContourPoints)
            {
                ptListGussetPts.Add(toViewMatrix.Transform(pt));
            }
            return ptListGussetPts;
        }


        
    }

    public class GussetBoltList
    {
        public ArrayList top_left_bolts_list = new ArrayList();
        public ArrayList top_right_bolts_list = new ArrayList();
        public ArrayList bottom_left_bolts_list = new ArrayList();
        public ArrayList bottom_right_bolts_list = new ArrayList();
        public ArrayList vertical_bolt_top_list = new ArrayList();
        public ArrayList vertical_bolt_bottom_list = new ArrayList();
    }

    public class GussetBoltPoints
    {
        public PointList finalPtListForRdTop  = new PointList(); // Populate this list as needed
        public PointList finalPtListForRdBottom = new PointList(); // Populate this list as needed

        public PointList finalPtListForRdTopNoBolt = new PointList(); // Populate this list as needed

        public PointList finalPtListForRdBottomNoBolt = new PointList(); // Populate this list as needed
        
        public GussetBoltPoints()
        {
            finalPtListForRdTop.Add(new TSG.Point(0, 0, 0));
            finalPtListForRdBottom.Add(new TSG.Point(0, 0, 0));
            finalPtListForRdTopNoBolt.Add(new TSG.Point(0, 0, 0));
            finalPtListForRdBottomNoBolt.Add(new TSG.Point(0, 0, 0));
        }
       
    }
}
