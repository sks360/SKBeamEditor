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

namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class ElevationDimension
    {

        private readonly CustomInputModel _userInput;
        private string client;

        private FontSizeSelector fontSize;

        private readonly bool isElevationDim; 

        /// <summary>
        /// Creates Elevation Dimension
        /// </summary>
        /// <param name="userInput"></param>
        public ElevationDimension(CustomInputModel userInput) 
        {
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
            this.isElevationDim = userInput.NeedEleDimension;
            _userInput = userInput;
        }


        public void CreateElevationDimension(TSG.Point myPoint, TSD.View myView, double dimDistance, string drawingAttributes)
        {
            if (myPoint == null || myView == null)
                throw new ArgumentNullException("myPoint or myView cannot be null.");

            try
            {
                TSD.StraightDimensionSet.StraightDimensionSetAttributes attributes = 
                    new TSD.StraightDimensionSet.StraightDimensionSetAttributes("Elevation")
                {
                    DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.Elevation,
                    Placing = { Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed },
                    Text = { Font = { Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drawingAttributes) } },
                    ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No,
                    Arrowhead = { Head = TSD.ArrowheadTypes.FilledArrow }
                };

                TSD.TextElement textElement = new TSD.TextElement("Wp ")
                {
                    Font = { Color = TSD.DrawingColors.Green, Height = 2.38125 }
                };

                if (isElevationDim)
                {
                    TSD.StraightDimension dimension = new TSD.StraightDimension(myView, myPoint, myPoint, new TSG.Vector(-1, 0, 0), -dimDistance, attributes);
                    dimension.Attributes.DimensionValuePrefix.Add(textElement);
                    dimension.Insert();
                    dimension.UpDirection = new TSG.Vector(-1, 0, 0);
                    dimension.Modify();

                    TSD.StraightDimensionSet dimensionSet = dimension.GetDimensionSet() as TSD.StraightDimensionSet;
                    if (dimensionSet != null)
                    {
                        dimensionSet.Attributes.Arrowhead.Head = TSD.ArrowheadTypes.FilledArrow;
                        dimensionSet.Modify();
                    }
                }
                else
                {
                    TSD.PointList pointList = new TSD.PointList { myPoint, myPoint };
                    TSD.StraightDimensionSetHandler dimensionSetHandler = new TSD.StraightDimensionSetHandler();
                    dimensionSetHandler.CreateDimensionSet(myView, pointList, new TSG.Vector(-1, 0, 0), dimDistance, attributes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating elevation dimension: {ex.Message}");
            }
        }
    }
}
