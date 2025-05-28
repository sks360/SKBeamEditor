using SK.Tekla.Drawing.Automation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

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


namespace SK.Tekla.Drawing.Automation.Drawing.Dimensions
{
    public class SkStudDimensions
    {
        private string client; //client

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        public SkStudDimensions(SKCatalogHandler catalogHandler, string client,
            FontSizeSelector fontSize)
        {
            this.catalogHandler = catalogHandler;
            this.client = client;
            this.fontSize = fontSize;
        }

        public void Create_stud_dimensions(TSD.View current_view, ref List<Guid> PARTMARK_TO_RETAIN, string drg_att)
        {
            try
            {

                TSM.Model MODEL = new TSM.Model();
                TSD.StraightDimensionSetHandler dimhandler = new TSD.StraightDimensionSetHandler();
                TSD.PointList list_for_stud_dim = new TSD.PointList();
                list_for_stud_dim.Add(new TSG.Point(0, 0, 0));
                Type[] type_for_bolt_beam = new Type[] { typeof(TSD.Part), typeof(TSD.Bolt) };
                TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(type_for_bolt_beam);
                TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
                while (enum_for_parts_drg.MoveNext())
                {
                    TSD.ModelObject mypart = enum_for_parts_drg.Current as TSD.ModelObject;

                    TSM.ModelObject model_part = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.ModelObject;
                    TSM.Part plate = model_part as TSM.Part;

                    if (model_part.GetType().Equals(typeof(TSM.BoltArray)))
                    {
                        TSM.BoltArray model_bolt = model_part as TSM.BoltArray;
                        string standard = model_bolt.BoltStandard;
                        if (standard.Contains("STUD"))
                        {
                            foreach (TSG.Point pt in model_bolt.BoltPositions)
                            {
                                list_for_stud_dim.Add(toviewmatrix.Transform(pt));
                                Guid ID = plate.Identifier.GUID;
                                PARTMARK_TO_RETAIN.Add(ID);
                            }
                        }

                    }
                    else if (model_part.GetType().Equals(typeof(TSM.Beam)))
                    {
                        TSM.Beam model_beam = model_part as TSM.Beam;

                        if (model_beam.Profile.ProfileString.Contains("STUD") && model_beam.Name.Contains("FALLTECH") == false)
                        {
                            list_for_stud_dim.Add(toviewmatrix.Transform(model_beam.StartPoint));
                            Guid ID = plate.Identifier.GUID;
                            PARTMARK_TO_RETAIN.Add(ID);
                        }

                    }


                }
                try
                {
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes rd_att = new TSD.StraightDimensionSet.StraightDimensionSetAttributes();
                    rd_att.DimensionType = TSD.DimensionSetBaseAttributes.DimensionTypes.USAbsolute2;
                    rd_att.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Fixed;
                    rd_att.ExtensionLine = TSD.DimensionSetBaseAttributes.ExtensionLineTypes.No;
                    rd_att.Arrowhead.Head = ArrowheadTypes.FilledArrow;
                    rd_att.Text.Font.Color = DrawingColors.Gray70;
                    rd_att.Color = DrawingColors.Gray70;
                    rd_att.Text.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                    dimhandler.CreateDimensionSet(current_view as TSD.ViewBase, list_for_stud_dim, new TSG.Vector(0, 1, 0), Math.Abs(current_view.RestrictionBox.MaxPoint.Y) + 300, rd_att);
                }
                catch
                {
                }
            }
            catch
            {
            }

        }
        public void Create_stud_dimensions_FOR_SECTION(TSD.View current_view, double HEIGHT)
        {

            TSM.Model MODEL = new TSM.Model();
            TSD.StraightDimensionSetHandler dimhandler = new TSD.StraightDimensionSetHandler();
            TSD.PointList list_for_stud_dim = new TSD.PointList();
            list_for_stud_dim.Add(new TSG.Point(0, HEIGHT, 0));
            Type[] type_for_bolt_beam = new Type[] { typeof(TSD.Part), typeof(TSD.Bolt) };

            TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.ViewCoordinateSystem);
            double X = current_view.RestrictionBox.MaxPoint.X;
            double X1 = current_view.RestrictionBox.MinPoint.X;
            double Z = current_view.RestrictionBox.MaxPoint.Z;
            double Z1 = current_view.RestrictionBox.MinPoint.Z;
            TSD.DrawingObjectEnumerator enum_for_parts_drg = current_view.GetAllObjects(type_for_bolt_beam);
            while (enum_for_parts_drg.MoveNext())
            {
                TSD.ModelObject mypart = enum_for_parts_drg.Current as TSD.ModelObject;

                TSM.ModelObject model_part = new TSM.Model().SelectModelObject(mypart.ModelIdentifier) as TSM.ModelObject;

                if (model_part.GetType().Equals(typeof(TSM.BoltArray)))
                {
                    TSM.BoltArray model_bolt = model_part as TSM.BoltArray;
                    string standard = model_bolt.BoltStandard;
                    if (standard.Contains("STUD"))
                    {
                        if (((model_bolt.BoltPositions[0] as TSG.Point).Z < Z) && (model_bolt.BoltPositions[0] as TSG.Point).Z > Z1)
                        {
                            list_for_stud_dim.Add(toviewmatrix.Transform(model_bolt.BoltPositions[0] as TSG.Point));
                        }
                    }

                }
                else if (model_part.GetType().Equals(typeof(TSM.Beam)))
                {
                    TSM.Beam model_beam = model_part as TSM.Beam;
                    TSG.Point P1 = toviewmatrix.Transform(model_beam.StartPoint);
                    TSG.Point P2 = toviewmatrix.Transform(model_beam.EndPoint);


                    if (model_beam.Profile.ProfileString.Contains("STUD"))
                    {
                        if ((P1.Z < Z) && (P1.Z > Z1))
                        {

                            list_for_stud_dim.Add(toviewmatrix.Transform(model_beam.StartPoint));
                        }
                    }
                }
            }





            try
            {
                dimhandler.CreateDimensionSet(current_view as TSD.ViewBase, list_for_stud_dim, new TSG.Vector(1, 0, 0), X + 100);
            }
            catch
            {
            }
        }

    }
}
