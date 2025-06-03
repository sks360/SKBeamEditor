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
    public class GussetDimension
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

        public GussetDimension(SKCatalogHandler catalogHandler, 
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

            DrawingObjectEnumerator partEnumerator = currentView.GetAllObjects(typeof(TSD.Part));
            while (partEnumerator.MoveNext())
            {
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
                PointList boundingBoxY = boundingBoxHandler.BoundingBoxSort(plate, currentView,SKSortingHandler.SortBy.Y);

                PointList plateCornerPoints = GetPlateCornerPoints(boundingBoxX, boundingBoxY);

                if (Math.Abs(zVectorPlate.Z) > Tolerance) 
               // if(zVectorPlate.Z != 0) //TODO check if this condition is needed instead of Tolerance!
                {
                    if (boundingBoxY[0].Y >= topFront / 2)
                    {
                        ProcessGussetPlate(plate, currentView, dimSetHandler, angleDimAttributes, fixedAttributes, topFront, true, ref partMarkToRetain, ref boltMarkToRetain);
                    }
                    else if (boundingBoxY[1].Y <= -topFront / 2)
                    {
                        ProcessGussetPlate(plate, currentView, dimSetHandler, angleDimAttributes, fixedAttributes, topFront, false, ref partMarkToRetain, ref boltMarkToRetain);
                    }
                }
            }

            // Create RD dimensions
            try
            {
                PointList finalPtListForRdTop = new PointList(); // Populate this list as needed
                PointList finalPtListForRdBottom = new PointList(); // Populate this list as needed
                double distanceTop = Math.Abs(Math.Abs(finalPtListForRdTop[0].Y) - currentView.RestrictionBox.MaxPoint.Y);
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdTop, new Vector(0, 1, 0), distanceTop + 200, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for top: {ex.Message}");
            }

            try
            {
                PointList finalPtListForRdBottom = new PointList(); // Populate this list as needed
                double distanceBottom = Math.Abs(Math.Abs(finalPtListForRdBottom[0].Y) - currentView.RestrictionBox.MinPoint.Y);
                dimSetHandler.CreateDimensionSet(currentView, finalPtListForRdBottom, new Vector(0, -1, 0), distanceBottom + 100, dimAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating RD dimension for bottom: {ex.Message}");
            }
        }

        private void ProcessGussetPlate(
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
                        // Process bolt group logic here if needed
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
}
