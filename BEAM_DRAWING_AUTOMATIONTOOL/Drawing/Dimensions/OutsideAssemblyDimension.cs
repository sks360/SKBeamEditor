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
using SK.Tekla.Drawing.Automation.Handlers;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class OutsideAssemblyDimension
    {
        private readonly CustomInputModel _userInput;

        private string client;

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        private readonly BoltMatrixHandler boltMatrixHandler;

        private readonly SKBoundingBoxHandler boundingBoxHandler;

        private readonly SKSortingHandler sortingHandler;

        private readonly DuplicateRemover duplicateRemover;

        private const double DimensionOffset = 10;

        public OutsideAssemblyDimension(SKCatalogHandler catalogHandler, 
            BoltMatrixHandler boltMatrixHandler, SKBoundingBoxHandler boundingBoxHandler,
            SKSortingHandler sortingHandler,
                DuplicateRemover duplicateRemover, CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler ?? throw new ArgumentNullException(nameof(catalogHandler));
            this.boltMatrixHandler = boltMatrixHandler ?? throw new ArgumentNullException(nameof(boltMatrixHandler));
            this.boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            this.sortingHandler = sortingHandler ?? throw new ArgumentNullException(nameof(sortingHandler));
            this.duplicateRemover = duplicateRemover ?? throw new ArgumentNullException(nameof(duplicateRemover));
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }

        public void DimensionForPartsOutsideAssembly(TSM.Beam mainPart, TSD.View currentView, double output, 
            ref List<Guid> partMarkToRetain, string drawingAttributes)
        {
            if (mainPart == null || currentView == null)
                throw new ArgumentNullException("mainPart or currentView cannot be null.");

            List<double> catalogValues = catalogHandler.Getcatalog_values(mainPart);
            double distance = catalogValues[0] / 2;

            StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                new StraightDimensionSet.StraightDimensionSetAttributes
            {
                Placing = { Placing = DimensionSetBaseAttributes.Placings.Fixed,
                    Distance = { MinimalDistance = distance }},
                ExtensionLine = DimensionSetBaseAttributes.ExtensionLineTypes.No,
                Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } }
            };

            List<TSG.Point> leftPoints = new List<TSG.Point>();
            List<TSG.Point> rightPoints = new List<TSG.Point>();

            DrawingObjectEnumerator partEnumerator = currentView.GetAllObjects(typeof(TSD.Part));
            while (partEnumerator.MoveNext())
            {
                TSD.Part myDrg = partEnumerator.Current as TSD.Part;
                if (myDrg == null) continue;

                TSM.ModelObject myPart = new Model().SelectModelObject(myDrg.ModelIdentifier);
                TSM.Part plate = myPart as TSM.Part;
                if (plate == null || plate.Identifier.GUID == mainPart.Identifier.GUID) continue;

                PointList myPointList = boundingBoxHandler.BoundingBoxSort(plate, currentView);
                AddPartToListIfOutside(plate, myPointList, output, leftPoints, rightPoints, partMarkToRetain);
            }

            try
            {
                var leftPointList = new TSD.PointList();
                foreach (var point in leftPoints)
                {
                    leftPointList.Add(point);
                }
                PointList uniqueLeftPoints = duplicateRemover.RemoveDuplicateXValues(leftPointList);
                CreateDimensionSet(currentView, uniqueLeftPoints, new TSG.Vector(0, 1, 0), distance + DimensionOffset, attributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating left dimension: {ex.Message}");
            }

            try
            {
                var rightPointList = new TSD.PointList();
                foreach (var point in rightPoints)
                {
                    rightPointList.Add(point);
                }
                PointList uniqueRightPoints = duplicateRemover.RemoveDuplicateXValues(rightPointList);
                if (uniqueRightPoints[0].Y > 0)
                {
                    sortingHandler.SortPoints(uniqueRightPoints, SKSortingHandler.SortBy.Y);
                }
                else
                {
                    sortingHandler.SortPoints(uniqueRightPoints, SKSortingHandler.SortBy.Y, 
                        SKSortingHandler.SortOrder.Descending);
                }
                CreateDimensionSet(currentView, uniqueRightPoints, new TSG.Vector(0, 1, 0), 
                    distance + DimensionOffset, attributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating right dimension: {ex.Message}");
            }
        }

        private void AddPartToListIfOutside(TSM.Part plate, PointList myPointList, 
            double output, List<TSG.Point> leftList, List<TSG.Point> rightList, List<Guid> partMarkToRetain)
        {
            if (myPointList[0].X < 0)
            {
                partMarkToRetain.Add(plate.Identifier.GUID);
                leftList.Add(new TSG.Point(0, 0, 0));
                leftList.Add(new TSG.Point(myPointList[0].X, myPointList[1].Y, 0));
            }
            else if (myPointList[1].X > output)
            {
                partMarkToRetain.Add(plate.Identifier.GUID);
                rightList.Add(new TSG.Point(output, 0, 0));
                rightList.Add(new TSG.Point(myPointList[1].X, myPointList[1].Y, 0));
            }
        }

        private void CreateDimensionSet(TSD.View view, PointList points, TSG.Vector vector, 
            double distance, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            StraightDimensionSetHandler dimensionSetHandler = new StraightDimensionSetHandler();
            dimensionSetHandler.CreateDimensionSet(view, points, vector, distance, attributes);
        }

    }
}
