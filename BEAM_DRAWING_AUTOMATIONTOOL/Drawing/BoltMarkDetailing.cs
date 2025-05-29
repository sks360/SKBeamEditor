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
using Tekla.Structures;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;

using Tekla.Structures.Drawing;
using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Handlers;
using System.Collections;
using Tekla.Structures.Model;
using Tekla.Structures.Datatype;

namespace SK.Tekla.Drawing.Automation.Drawing
{
    public class BoltMarkDetailing
    {
        private readonly CustomInputModel _userInput;

        private string client; //client

        private FontSizeSelector fontSize;

        private readonly SKCatalogHandler catalogHandler;

        public BoltMarkDetailing(SKCatalogHandler catalogHandler,  CustomInputModel userInput)
        {
            this.catalogHandler = catalogHandler;
            _userInput = userInput;
            this.client = userInput.Client;
            this.fontSize = userInput.FontSize;
        }


    
        public void bolt_mark_detail(Model currentModel,TSD.AssemblyDrawing assembly_drg, TSM.Part MAIN_PART, string drg_att)
        {
            Type[] type_for_mark = new Type[] { typeof(TSD.Mark), typeof(TSD.MarkSet) };


            TSD.DrawingObjectEnumerator enum_for_views = assembly_drg.GetSheet().GetAllViews();
            while (enum_for_views.MoveNext())
            {
                TSD.View CURRENT_VIEW = enum_for_views.Current as TSD.View;

                TSD.DrawingObjectEnumerator MYMARK_ENUM = CURRENT_VIEW.GetAllObjects(type_for_mark);

                while (MYMARK_ENUM.MoveNext())
                {
                    var obj = MYMARK_ENUM.Current;
                    if (obj.GetType().Equals(typeof(TSD.Mark)))
                    {
                        TSD.Mark mymark = MYMARK_ENUM.Current as TSD.Mark;

                        TSD.DrawingObjectEnumerator MYBJ = mymark.GetRelatedObjects();
                        while (MYBJ.MoveNext())
                        {
                            var mark_part = MYBJ.Current;
                            if (mark_part.GetType().Equals(typeof(TSD.Bolt)))
                            {



                                TSM.BoltGroup modelpart = new TSM.Model().SelectModelObject((mark_part as TSD.Bolt).ModelIdentifier) as TSM.BoltGroup;
                                int NO_BOLT = modelpart.BoltPositions.Count;
                                if (modelpart.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE))
                                {
                                    mymark.Attributes.Content.Clear();
                                    TSD.PropertyElement.PropertyElementType no_of_bolts = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.NumberOfBolts();
                                    TSD.PropertyElement final_no_of_bolts = new TSD.PropertyElement(no_of_bolts);
                                    final_no_of_bolts.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_no_of_bolts.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_no_of_bolts.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_no_of_bolts.Font.Height = 2.38125;

                                    }
                                    TSD.SymbolElement mysymbol = new TSD.SymbolElement(new TSD.SymbolInfo("xsteel", 64));
                                    mysymbol.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            mysymbol.Height = 3.571875;
                                        }
                                        else
                                        {
                                            mysymbol.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        mysymbol.Height = 2.38125;

                                    }
                                    TSD.PropertyElement.PropertyElementType size = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.Size();
                                    TSD.PropertyElement final_size = new TSD.PropertyElement(size);
                                    final_size.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_size.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_size.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_size.Font.Height = 2.38125;

                                    }

                                    string HLSTEXT = "";
                                    if (NO_BOLT == 1)
                                    {
                                        HLSTEXT = "HL";
                                    }
                                    else
                                    {
                                        HLSTEXT = "HLS";

                                    }


                                    TSD.TextElement hls = new TSD.TextElement(HLSTEXT);
                                    hls.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            hls.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            hls.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        hls.Font.Height = 2.38125;
                                    }

                                    TSD.PropertyElement.PropertyElementType Assemblytype = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.AssemblyType();
                                    TSD.PropertyElement final_Assemblytype = new TSD.PropertyElement(Assemblytype);
                                    final_Assemblytype.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_Assemblytype.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_Assemblytype.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_Assemblytype.Font.Height = 2.38125;

                                    }

                                    TSD.TextElement Boltstext = new TSD.TextElement("BOLTS");
                                    Boltstext.Font.Color = TSD.DrawingColors.Green;
                                    Boltstext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                   

                                    TSD.PropertyElement.PropertyElementType boltdia = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.BoltDiameter();
                                    TSD.PropertyElement final_boltdia = new TSD.PropertyElement(boltdia);
                                    final_boltdia.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_boltdia.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_boltdia.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_boltdia.Font.Height = 2.38125;

                                    }

                                    TSD.TextElement DIA_SYMBOL = new TSD.TextElement("\"Ø");
                                    DIA_SYMBOL.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            DIA_SYMBOL.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            DIA_SYMBOL.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        DIA_SYMBOL.Font.Height = 2.38125;

                                    }

                                    TSD.PropertyElement.PropertyElementType STANDARD = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.Standard();
                                    TSD.PropertyElement final_STANDARD = new TSD.PropertyElement(STANDARD);
                                    final_STANDARD.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_STANDARD.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_STANDARD.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_STANDARD.Font.Height = 2.38125;

                                    }

                                    TSD.PropertyElement.PropertyElementType BOLT_LENGTH = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.BoltLength();
                                    TSD.PropertyElement final_BOLT_LENGTH = new TSD.PropertyElement(BOLT_LENGTH);
                                    final_BOLT_LENGTH.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_BOLT_LENGTH.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_BOLT_LENGTH.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_BOLT_LENGTH.Font.Height = 2.38125;

                                    }


                                    if ((client.Equals("BENHUR")) || (client.Equals("STEFFY&SON")))
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                        TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls };
                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);
                                        mymark.Attributes.Content.Add(mycontainer4);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Sharpened;

                                    }
                                    else if ((client.Equals("FORD")) || (client.Equals("HAMILTON")))
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };

                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);

                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Round;
                                    }
                                    else if (client.Equals("TRINITY"))
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                        TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls };
                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);
                                        mymark.Attributes.Content.Add(mycontainer4);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                    }
                                    else if (client.Equals("NONE"))
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };

                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);

                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Round;
                                    }
                                    else if (client.Equals("SME"))
                                    {
                                        string dia_text = "";
                                        Hashtable hashtable = new Hashtable();
                                        currentModel.GetProjectInfo().GetStringUserProperties(ref hashtable);

                                        dia_text = hashtable["PROJECT_STD_HOLE_DI"] as string;
                                        double dia = 0;
                                        if (dia_text.EndsWith("\""))
                                        {
                                            string[] split_text = dia_text.Split('/');
                                            double numerator = System.Double.Parse(split_text[0]);
                                            double denominator = Convert.ToDouble(split_text[1].Substring(0, split_text[1].Length - 1));
                                            dia = (numerator / denominator) * 25.4;
                                        }
                                        else
                                        {
                                            string[] split_text = dia_text.Split('\"');
                                            double inch = System.Double.Parse(split_text[0]);
                                            string[] fraction_split = split_text[1].Split('/');
                                            double numerator = System.Double.Parse(fraction_split[0]);
                                            double denominator = Convert.ToDouble(fraction_split[1]);
                                            dia = (inch + (numerator / denominator)) * 25.4;
                                        }

                                        if (modelpart.HoleType == TSM.BoltGroup.BoltHoleTypeEnum.HOLE_TYPE_OVERSIZED)
                                        {

                                            double oversize_dia = modelpart.BoltSize + modelpart.Tolerance + modelpart.SlottedHoleX;
                                            Distance distance = new Distance();
                                            distance.Millimeters = oversize_dia;
                                            TSD.TextElement textElement1 = new TSD.TextElement("( ");
                                            TSD.TextElement textElement2 = new TSD.TextElement(" )");
                                            TSD.TextElement textElement3 = new TSD.TextElement(" - ");
                                            TSD.TextElement textElement4 = new TSD.TextElement(distance.ToFractionalInchesString());
                                            textElement1.Font.Color = TSD.DrawingColors.Green;
                                            textElement2.Font.Color = TSD.DrawingColors.Green;
                                            textElement3.Font.Color = TSD.DrawingColors.Green;

                                            TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };

                                            mymark.Attributes.Content.Add(textElement1);
                                            mymark.Attributes.Content.Add(mycontainer1);
                                            mymark.Attributes.Content.Add(textElement2);
                                            mymark.Attributes.Content.Add(textElement3);
                                            mymark.Attributes.Content.Add(textElement4);

                                            mymark.Attributes.Frame.Type = TSD.FrameTypes.Rectangular;
                                            mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;

                                        }
                                        else
                                        {
                                            if (Math.Abs(modelpart.BoltSize + modelpart.Tolerance - dia) < 0.2)
                                            {
                                                TSM.Part part1 = modelpart.PartToBeBolted;
                                                TSM.Part part2 = modelpart.PartToBoltTo;
                                                ArrayList parts = modelpart.GetOtherPartsToBolt();
                                                parts.Add(part1);
                                                parts.Add(part2);
                                                bool found = false;
                                                foreach (TSM.Part part in parts)
                                                {
                                                    if (part != null)
                                                    {
                                                        if (MAIN_PART.Identifier.GUID.ToString() == part.Identifier.GUID.ToString())
                                                        {
                                                            found = true;
                                                            mymark.Delete();
                                                        }
                                                    }
                                                }
                                                if (found == false)
                                                {
                                                    TSD.TextElement textElement1 = new TSD.TextElement("( ");
                                                    TSD.TextElement textElement2 = new TSD.TextElement(" )");
                                                    TSD.TextElement textElement3 = new TSD.TextElement(" - ");
                                                    textElement1.Font.Color = TSD.DrawingColors.Green;
                                                    textElement2.Font.Color = TSD.DrawingColors.Green;
                                                    textElement3.Font.Color = TSD.DrawingColors.Green;


                                                    TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                                    TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                                    TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };

                                                    mymark.Attributes.Content.Add(textElement1);
                                                    mymark.Attributes.Content.Add(mycontainer1);
                                                    mymark.Attributes.Content.Add(textElement2);
                                                    mymark.Attributes.Content.Add(textElement3);
                                                    mymark.Attributes.Content.Add(mycontainer3);

                                                    mymark.Attributes.Frame.Type = TSD.FrameTypes.Rectangular;
                                                    mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
                                                }

                                            }
                                            else
                                            {
                                                TSD.TextElement textElement1 = new TSD.TextElement("( ");
                                                TSD.TextElement textElement2 = new TSD.TextElement(" )");
                                                TSD.TextElement textElement3 = new TSD.TextElement(" - ");
                                                textElement1.Font.Color = TSD.DrawingColors.Green;
                                                textElement2.Font.Color = TSD.DrawingColors.Green;
                                                textElement3.Font.Color = TSD.DrawingColors.Green;


                                                TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                                TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                                TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };

                                                mymark.Attributes.Content.Add(textElement1);
                                                mymark.Attributes.Content.Add(mycontainer1);
                                                mymark.Attributes.Content.Add(textElement2);
                                                mymark.Attributes.Content.Add(textElement3);
                                                mymark.Attributes.Content.Add(mycontainer3);

                                                mymark.Attributes.Frame.Type = TSD.FrameTypes.Rectangular;
                                                mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
                                            }
                                            if (modelpart.HoleType == TSM.BoltGroup.BoltHoleTypeEnum.HOLE_TYPE_OVERSIZED)
                                            {

                                                double oversize_dia = modelpart.BoltSize + modelpart.Tolerance + modelpart.SlottedHoleX;
                                                Distance distance = new Distance();
                                                distance.Millimeters = oversize_dia;
                                                TSD.TextElement textElement1 = new TSD.TextElement("( ");
                                                TSD.TextElement textElement2 = new TSD.TextElement(" )");
                                                TSD.TextElement textElement3 = new TSD.TextElement(" - ");
                                                TSD.TextElement textElement4 = new TSD.TextElement(distance.ToFractionalInchesString());
                                                textElement1.Font.Color = TSD.DrawingColors.Green;
                                                textElement2.Font.Color = TSD.DrawingColors.Green;
                                                textElement3.Font.Color = TSD.DrawingColors.Green;

                                                TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };

                                                mymark.Attributes.Content.Add(textElement1);
                                                mymark.Attributes.Content.Add(mycontainer1);
                                                mymark.Attributes.Content.Add(textElement2);
                                                mymark.Attributes.Content.Add(textElement3);
                                                mymark.Attributes.Content.Add(textElement4);

                                                mymark.Attributes.Frame.Type = TSD.FrameTypes.Rectangular;
                                                mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;

                                            }
                                        }
                                    }
                                    else if (client.Equals("HILLSDALE"))
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                        TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { final_Assemblytype };
                                        TSD.ContainerElement mycontainer5 = new TSD.ContainerElement { Boltstext };
                                        TSD.ContainerElement mycontainer6 = new TSD.ContainerElement { final_boltdia };
                                        TSD.ContainerElement mycontainer7 = new TSD.ContainerElement { DIA_SYMBOL };
                                        TSD.ContainerElement mycontainer8 = new TSD.ContainerElement { final_STANDARD };
                                        TSD.ContainerElement mycontainer9 = new TSD.ContainerElement { final_BOLT_LENGTH };

                                        //TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls };
                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer4);
                                        mymark.Attributes.Content.Add(mycontainer5);
                                        mymark.Attributes.Content.Add(mycontainer6);
                                        mymark.Attributes.Content.Add(mycontainer7);
                                        mymark.Attributes.Content.Add(mycontainer8);
                                        mymark.Attributes.Content.Add(mycontainer9);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Round;
                                    }
                                    else
                                    {
                                        TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                        TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls };
                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);
                                        mymark.Attributes.Content.Add(mycontainer4);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Round;
                                    }


                                    mymark.Modify();

                                }
                                else if (modelpart.BoltType.Equals(TSM.BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP))
                                {
                                    TSM.Part PARTTOBOLTTO = modelpart.PartToBoltTo;
                                    TSM.Part PARTTOBEBOLTED = modelpart.PartToBeBolted;
                                    ArrayList OTHER_PART = modelpart.OtherPartsToBolt;

                                    string HLSTEXT = "";
                                    if (NO_BOLT == 1)
                                    {
                                        HLSTEXT = "HL";
                                    }
                                    else
                                    {
                                        HLSTEXT = "HLS";

                                    }


                                    TSD.TextElement hls = new TSD.TextElement(HLSTEXT);
                                    hls.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            hls.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            hls.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        hls.Font.Height = 2.38125;
                                    }

                                    string TEXT_FOR_SUBPART = "";
                                    if (!PARTTOBEBOLTED.Identifier.GUID.Equals(MAIN_PART.Identifier.GUID))
                                    {
                                        string prof_type = "";
                                        PARTTOBEBOLTED.GetReportProperty("PROFILE_TYPE", ref prof_type);
                                        if (prof_type == "L")
                                        {
                                            TEXT_FOR_SUBPART = "HLS IN ANGLE";

                                        }
                                        else if (prof_type == "B")
                                        {
                                            TEXT_FOR_SUBPART = "HLS IN PLATE";

                                        }
                                        else
                                        {

                                        }
                                    }
                                    else if (!PARTTOBOLTTO.Identifier.GUID.Equals(MAIN_PART.Identifier.GUID))
                                    {
                                        string prof_type = "";
                                        PARTTOBOLTTO.GetReportProperty("PROFILE_TYPE", ref prof_type);
                                        if (prof_type == "L")
                                        {
                                            TEXT_FOR_SUBPART = "HLS IN ANGLE";

                                        }
                                        else if (prof_type == "B")
                                        {
                                            TEXT_FOR_SUBPART = "HLS IN PLATE";

                                        }
                                        else
                                        {

                                        }


                                    }


                                    mymark.Attributes.Content.Clear();
                                    TSD.PropertyElement.PropertyElementType no_of_bolts = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.NumberOfBolts();
                                    TSD.PropertyElement final_no_of_bolts = new TSD.PropertyElement(no_of_bolts);
                                    final_no_of_bolts.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_no_of_bolts.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_no_of_bolts.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_no_of_bolts.Font.Height = 2.38125;

                                    }
                                    TSD.SymbolElement mysymbol = new TSD.SymbolElement(new TSD.SymbolInfo("xsteel", 64));
                                    mysymbol.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            mysymbol.Height = 3.571875;
                                        }
                                        else
                                        {
                                            mysymbol.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        mysymbol.Height = 2.38125;

                                    }
                                    TSD.PropertyElement.PropertyElementType size = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.Size();
                                    TSD.PropertyElement final_size = new TSD.PropertyElement(size);
                                    final_size.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_size.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_size.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_size.Font.Height = 2.38125;

                                    }
                                    TSD.TextElement hls_IN_MAINPART = new TSD.TextElement(" HLS IN BEAM");
                                    hls_IN_MAINPART.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            hls_IN_MAINPART.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            hls_IN_MAINPART.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        hls_IN_MAINPART.Font.Height = 2.38125;

                                    }



                                    TSD.PropertyElement.PropertyElementType HOLE_DIA = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.HoleDiameter();
                                    TSD.PropertyElement final_HOLE_DIA = new TSD.PropertyElement(HOLE_DIA);
                                    final_HOLE_DIA.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_HOLE_DIA.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_HOLE_DIA.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_HOLE_DIA.Font.Height = 2.38125;

                                    }



                                    TSD.TextElement hls_IN_SUBPART = new TSD.TextElement(TEXT_FOR_SUBPART);
                                    hls_IN_SUBPART.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            hls_IN_SUBPART.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            hls_IN_SUBPART.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        hls_IN_SUBPART.Font.Height = 2.38125;

                                    }

                                    TSD.TextElement FOR = new TSD.TextElement("FOR");
                                    FOR.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            FOR.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            FOR.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        FOR.Font.Height = 2.38125;

                                    }

                                    TSD.PropertyElement.PropertyElementType BOLT_DIA = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.BoltDiameter();
                                    TSD.PropertyElement final_BOLT_DIA = new TSD.PropertyElement(BOLT_DIA);
                                    final_BOLT_DIA.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_BOLT_DIA.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_BOLT_DIA.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_BOLT_DIA.Font.Height = 2.38125;

                                    }

                                    TSD.PropertyElement.PropertyElementType STANDARD = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.Standard();
                                    TSD.PropertyElement final_STANDARD = new TSD.PropertyElement(STANDARD);
                                    final_STANDARD.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_STANDARD.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_STANDARD.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_STANDARD.Font.Height = 2.38125;

                                    }

                                    TSD.TextElement DIA_SYMBOLwithx = new TSD.TextElement("\"Ø X");
                                    DIA_SYMBOLwithx.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            DIA_SYMBOLwithx.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            DIA_SYMBOLwithx.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        DIA_SYMBOLwithx.Font.Height = 2.38125;

                                    }
                                    TSD.TextElement DIA_SYMBOL = new TSD.TextElement("\"Ø");
                                    DIA_SYMBOL.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            DIA_SYMBOL.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            DIA_SYMBOL.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        DIA_SYMBOL.Font.Height = 2.38125;

                                    }

                                    TSD.PropertyElement.PropertyElementType BOLT_LENGTH = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.BoltLength();
                                    TSD.PropertyElement final_BOLT_LENGTH = new TSD.PropertyElement(BOLT_LENGTH);
                                    final_BOLT_LENGTH.Font.Color = TSD.DrawingColors.Green;
                                    if ((drg_att == "SK_BEAM_A1") || (fontSize == FontSizeSelector.OneBy8))
                                    {
                                        if ((client.Equals("HILLSDALE")) || (fontSize == FontSizeSelector.NineBy64))
                                        {
                                            final_BOLT_LENGTH.Font.Height = 3.571875;
                                        }
                                        else
                                        {
                                            final_BOLT_LENGTH.Font.Height = 3.175;
                                        }
                                    }
                                    else
                                    {
                                        final_BOLT_LENGTH.Font.Height = 2.38125;

                                    }
                                    TSD.TextElement shoptext = new TSD.TextElement("SHOPBOLT");
                                    shoptext.Font.Color = TSD.DrawingColors.Green;
                                    shoptext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);
                                   
                                    TSD.TextElement BOLTS = new TSD.TextElement("BOLTS");
                                    BOLTS.Font.Color = TSD.DrawingColors.Green;
                                    BOLTS.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                    TSD.TextElement x = new TSD.TextElement("X");
                                    x.Font.Color = TSD.DrawingColors.Green;
                                    x.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);



                                    TSD.PropertyElement.PropertyElementType Assemblytype = TSD.PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.AssemblyType();
                                    TSD.PropertyElement final_Assemblytype = new TSD.PropertyElement(Assemblytype);
                                    final_Assemblytype.Font.Color = TSD.DrawingColors.Green;
                                    final_Assemblytype.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);

                                    TSD.TextElement Boltstext = new TSD.TextElement("BOLTS");
                                    Boltstext.Font.Color = TSD.DrawingColors.Green;
                                    Boltstext.Font.Height = SkTeklaDrawingUtility.GetFontHeight(client, fontSize, drg_att);




                                    TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                    TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                    TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                    TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls_IN_MAINPART };
                                    TSD.ContainerElement mycontainer5 = new TSD.ContainerElement { final_no_of_bolts };
                                    TSD.ContainerElement mycontainer6 = new TSD.ContainerElement { mysymbol };
                                    TSD.ContainerElement mycontainer7 = new TSD.ContainerElement { final_HOLE_DIA };
                                    TSD.ContainerElement mycontainer8 = new TSD.ContainerElement { DIA_SYMBOL };
                                    TSD.ContainerElement mycontainer9 = new TSD.ContainerElement { hls_IN_SUBPART };
                                    TSD.ContainerElement mycontainer10 = new TSD.ContainerElement { FOR };
                                    TSD.ContainerElement mycontainer11 = new TSD.ContainerElement { final_BOLT_DIA };
                                    TSD.ContainerElement mycontainer12 = new TSD.ContainerElement { DIA_SYMBOLwithx };
                                    TSD.ContainerElement mycontainer13 = new TSD.ContainerElement { final_BOLT_LENGTH };
                                    TSD.ContainerElement mycontainer14 = new TSD.ContainerElement { final_STANDARD };
                                    TSD.ContainerElement mycontainer15 = new TSD.ContainerElement { shoptext };
                                    TSD.ContainerElement mycontainer16 = new TSD.ContainerElement { BOLTS };
                                    TSD.ContainerElement mycontainer17 = new TSD.ContainerElement { x };


                                    TSD.ContainerElement mycontainer3_1 = new TSD.ContainerElement { hls };
                                    TSD.ContainerElement mycontainer3_2 = new TSD.ContainerElement { final_Assemblytype };
                                    TSD.ContainerElement mycontainer3_3 = new TSD.ContainerElement { Boltstext };





                                    TSD.ContainerElement FIRST_LINE_CONTENT = new TSD.ContainerElement { };
                                    FIRST_LINE_CONTENT.Add(mycontainer1);
                                    FIRST_LINE_CONTENT.Add(mycontainer2);
                                    FIRST_LINE_CONTENT.Add(mycontainer7);
                                    FIRST_LINE_CONTENT.Add(mycontainer8);
                                    FIRST_LINE_CONTENT.Add(mycontainer4);
                                    TSD.ContainerElement SECOND_LINE_CONTENT = new TSD.ContainerElement { };
                                    SECOND_LINE_CONTENT.Add(mycontainer1);
                                    SECOND_LINE_CONTENT.Add(mycontainer2);
                                    SECOND_LINE_CONTENT.Add(mycontainer3);
                                    SECOND_LINE_CONTENT.Add(mycontainer9);

                                    if (client.Equals("BENHUR"))
                                    {

                                        TSD.ContainerElement MAIN_CONTENT = new TSD.ContainerElement();
                                        MAIN_CONTENT.Frame.Type = TSD.FrameTypes.Sharpened;
                                        MAIN_CONTENT.Add(FIRST_LINE_CONTENT);
                                        MAIN_CONTENT.Add(new TSD.NewLineElement());
                                        MAIN_CONTENT.Add(SECOND_LINE_CONTENT);


                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                        mymark.Attributes.Content.Add(MAIN_CONTENT);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer10);
                                        mymark.Attributes.Content.Add(mycontainer11);
                                        mymark.Attributes.Content.Add(mycontainer12);
                                        mymark.Attributes.Content.Add(mycontainer13);
                                        mymark.Attributes.Content.Add(mycontainer14);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer15);
                                        mymark.Modify();
                                    }
                                    else if (client.Equals("STEFFY&SON"))
                                    {
                                        TSD.ContainerElement MAIN_CONTENT = new TSD.ContainerElement();
                                        MAIN_CONTENT.Frame.Type = TSD.FrameTypes.Sharpened;
                                        MAIN_CONTENT.Add(FIRST_LINE_CONTENT);
                                        MAIN_CONTENT.Add(new TSD.NewLineElement());
                                        MAIN_CONTENT.Add(SECOND_LINE_CONTENT);


                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                        mymark.Attributes.Content.Add(MAIN_CONTENT);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer10);
                                        mymark.Attributes.Content.Add(mycontainer11);
                                        //mymark.Attributes.Content.Add(mycontainer8);
                                        mymark.Attributes.Content.Add(mycontainer12);
                                        mymark.Attributes.Content.Add(mycontainer13);
                                        mymark.Attributes.Content.Add(mycontainer14);
                                        mymark.Attributes.Content.Add(mycontainer16);

                                        mymark.Modify();

                                    }

                                    else if (client.Equals("TRINITY"))
                                    {
                                        //TSD.ContainerElement MAIN_CONTENT = new TSD.ContainerElement();
                                        //MAIN_CONTENT.Frame.Type = TSD.FrameTypes.None;
                                        //MAIN_CONTENT.Add(FIRST_LINE_CONTENT);
                                        //MAIN_CONTENT.Add(new TSD.NewLineElement());
                                        //MAIN_CONTENT.Add(SECOND_LINE_CONTENT);


                                        //mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                        //mymark.Attributes.Content.Add(MAIN_CONTENT);
                                        //mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        //mymark.Attributes.Content.Add(mycontainer10);
                                        //mymark.Attributes.Content.Add(mycontainer11);
                                        ////mymark.Attributes.Content.Add(mycontainer8);
                                        //mymark.Attributes.Content.Add(mycontainer12);
                                        //mymark.Attributes.Content.Add(mycontainer13);
                                        //mymark.Attributes.Content.Add(mycontainer14);
                                        //mymark.Attributes.Content.Add(mycontainer16);


                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer11);
                                        mymark.Attributes.Content.Add(mycontainer8);
                                        mymark.Attributes.Content.Add(mycontainer14);
                                        mymark.Attributes.Content.Add(mycontainer17);
                                        mymark.Attributes.Content.Add(mycontainer13);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.None;

                                        mymark.Modify();

                                        mymark.Modify();

                                    }
                                    else if (client.Equals("HILLSDALE"))
                                    {
                                        //TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { final_no_of_bolts };
                                        //TSD.ContainerElement mycontainer2 = new TSD.ContainerElement { mysymbol };
                                        //TSD.ContainerElement mycontainer3 = new TSD.ContainerElement { final_size };
                                        //TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { final_Assemblytype };
                                        //TSD.ContainerElement mycontainer5 = new TSD.ContainerElement { Boltstext };
                                        //TSD.ContainerElement mycontainer6 = new TSD.ContainerElement { final_boltdia };
                                        //TSD.ContainerElement mycontainer7 = new TSD.ContainerElement { DIA_SYMBOL };
                                        //TSD.ContainerElement mycontainer8 = new TSD.ContainerElement { final_STANDARD };
                                        //TSD.ContainerElement mycontainer9 = new TSD.ContainerElement { final_BOLT_LENGTH };

                                        //TSD.ContainerElement mycontainer4 = new TSD.ContainerElement { hls };
                                        mymark.Attributes.Content.Add(mycontainer1);
                                        mymark.Attributes.Content.Add(mycontainer2);
                                        mymark.Attributes.Content.Add(mycontainer3);
                                        mymark.Attributes.Content.Add(mycontainer3_1);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer3_2);
                                        mymark.Attributes.Content.Add(mycontainer3_3);
                                        mymark.Attributes.Content.Add(mycontainer11);
                                        mymark.Attributes.Content.Add(mycontainer8);
                                        mymark.Attributes.Content.Add(mycontainer14);
                                        mymark.Attributes.Content.Add(mycontainer13);
                                        //mymark.Attributes.Content.Add(mycontainer4);
                                        //mymark.Attributes.Content.Add(mycontainer5);
                                        //mymark.Attributes.Content.Add(mycontainer6);
                                        //mymark.Attributes.Content.Add(mycontainer7);
                                        //mymark.Attributes.Content.Add(mycontainer8);
                                        //mymark.Attributes.Content.Add(mycontainer9);
                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.Round;
                                        mymark.Modify();
                                    }
                                    else
                                    {
                                        TSD.ContainerElement MAIN_CONTENT = new TSD.ContainerElement();
                                        MAIN_CONTENT.Frame.Type = TSD.FrameTypes.Round;
                                        MAIN_CONTENT.Add(FIRST_LINE_CONTENT);
                                        MAIN_CONTENT.Add(new TSD.NewLineElement());
                                        MAIN_CONTENT.Add(SECOND_LINE_CONTENT);


                                        mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                        mymark.Attributes.Content.Add(MAIN_CONTENT);
                                        mymark.Attributes.Content.Add(new TSD.NewLineElement());
                                        mymark.Attributes.Content.Add(mycontainer10);
                                        mymark.Attributes.Content.Add(mycontainer11);
                                        //mymark.Attributes.Content.Add(mycontainer8);
                                        mymark.Attributes.Content.Add(mycontainer12);
                                        mymark.Attributes.Content.Add(mycontainer13);
                                        mymark.Attributes.Content.Add(mycontainer14);
                                        mymark.Attributes.Content.Add(mycontainer16);

                                        mymark.Modify();

                                    }

                                }

                            }
                        }

                    }
                    else if (obj.GetType().Equals(typeof(TSD.MarkSet)))
                    {

                    }

                }

            }
        }

    }
}