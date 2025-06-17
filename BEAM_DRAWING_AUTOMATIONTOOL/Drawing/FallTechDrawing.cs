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
    /// <summary>
    /// Adjust the drawing wiht Fall Tech attributes
    /// </summary>
    public class FallTechDrawing
    {
        private static readonly log4net.ILog _logger =
       log4net.LogManager.GetLogger(typeof(FallTechDrawing));

        public FallTechDrawing() { }


        /// <summary>
        /// Mark Fall Tech Attributes in the Front view
        /// </summary>
        /// <param name="currentModel"></param>
        /// <param name="assemblyDrawing"></param>
        public void MarkFallTech(Model currentModel, AssemblyDrawing assemblyDrawing)
        {
            DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllViews();
            while (drawingObjectEnumerator.MoveNext())
            {
                TSD.View view = drawingObjectEnumerator.Current as TSD.View;
                if (view?.ViewType == TSD.View.ViewTypes.FrontView)
                {
                    _logger.Info($"In Front view {view?.Name}");
                    DrawingObjectEnumerator marks = view.GetAllObjects(typeof(TSD.Mark));
                    while (marks.MoveNext())
                    {
                        TSD.Mark mark = marks.Current as TSD.Mark;
                        if (mark == null) continue;

                        DrawingObjectEnumerator relObjects = mark.GetRelatedObjects();
                        while (relObjects.MoveNext())
                        {
                            TSD.Part part = relObjects.Current as TSD.Part;
                            if (part != null)
                            {
                                TSM.Part mdlObj = currentModel.SelectModelObject(part.ModelIdentifier) as TSM.Part;
                                if (mdlObj != null && mdlObj.Name.Contains("FALLTECH"))
                                {
                                    UpdateMarkAttributes(mark);
                                    mark.Modify();
                                    view.Modify();
                                    _logger.Info($"Fall Tech is modified");
                                }
                            }
                        }
                    }
                }
            }
            assemblyDrawing.CommitChanges();
        }
        private void UpdateMarkAttributes(TSD.Mark mark)
        {
            mark.Attributes.Content.Clear();
            TSD.PropertyElement.PropertyElementType partPos = TSD.PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.PartPosition();
            TSD.PropertyElement position = new TSD.PropertyElement(partPos)
            {
                Font = { Color = DrawingColors.Green, Height = 3.175 }
            };
            TSD.ContainerElement container = new TSD.ContainerElement { position };
            TSD.NewLineElement newLineElement = new TSD.NewLineElement();
            TSD.TextElement textElement1 = CreateTextElement("(FIT WITH PINS,");
            TSD.TextElement textElement2 = CreateTextElement("SHACKLES AND");
            TSD.TextElement textElement3 = CreateTextElement("CATENARY LINE)");
            TSD.TextElement textElement4 = CreateTextElement("REFER SHOP NOTE");

            mark.Attributes.Content.Add(container);
            mark.Attributes.Content.Add(newLineElement);
            mark.Attributes.Content.Add(textElement1);
            mark.Attributes.Content.Add(newLineElement);
            mark.Attributes.Content.Add(textElement2);
            mark.Attributes.Content.Add(newLineElement);
            mark.Attributes.Content.Add(textElement3);
            mark.Attributes.Content.Add(newLineElement);
            mark.Attributes.Content.Add(textElement4);

            mark.Attributes.Frame.Type = TSD.FrameTypes.None;
            mark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
        }

        private TSD.TextElement CreateTextElement(string text)
        {
            return new TSD.TextElement(text)
            {
                Font = { Color = DrawingColors.Red, Height = 3.175 }
            };
        }

        //public void Fall_Tech(Model currentModel, AssemblyDrawing assemblyDrawing)
        //{
        //    DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllViews();
        //    while (drawingObjectEnumerator.MoveNext())
        //    {
        //        TSD.View view = drawingObjectEnumerator.Current as TSD.View;
        //        if (view.ViewType == TSD.View.ViewTypes.FrontView)
        //        {
        //            DrawingObjectEnumerator marks = view.GetAllObjects(typeof(TSD.Mark));
        //            while (marks.MoveNext())
        //            {
        //                TSD.Mark mark = marks.Current as TSD.Mark;
        //                DrawingObjectEnumerator rel_objects = mark.GetRelatedObjects();
        //                while (rel_objects.MoveNext())
        //                {
        //                    TSD.Part part = rel_objects.Current as TSD.Part;
        //                    if (part != null)
        //                    {
        //                        TSM.Part mdl_obj = currentModel.SelectModelObject(part.ModelIdentifier) as TSM.Part;
        //                        if (mdl_obj != null)
        //                        {
        //                            if (mdl_obj.Name.Contains("FALLTECH"))
        //                            {
        //                                ContainerElement containerElement = mark.Attributes.Content;
        //                                mark.Attributes.Content.Clear();
        //                                TSD.PropertyElement.PropertyElementType part_pos = TSD.PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.PartPosition();
        //                                TSD.PropertyElement position = new TSD.PropertyElement(part_pos);
        //                                position.Font.Color = DrawingColors.Green;
        //                                position.Font.Height = 3.175;
        //                                TSD.ContainerElement mycontainer1 = new TSD.ContainerElement { position };
        //                                TSD.NewLineElement newLineElement = new TSD.NewLineElement();
        //                                TSD.TextElement textElement1 = new TSD.TextElement("(FIT WITH PINS,");
        //                                TSD.TextElement textElement2 = new TSD.TextElement("SHACKLES AND");
        //                                TSD.TextElement textElement3 = new TSD.TextElement("CATENARY LINE)");
        //                                TSD.TextElement textElement4 = new TSD.TextElement("REFER SHOP NOTE");
        //                                textElement1.Font.Color = DrawingColors.Red;
        //                                textElement1.Font.Height = 3.175;
        //                                textElement2.Font.Color = DrawingColors.Red;
        //                                textElement2.Font.Height = 3.175;
        //                                textElement3.Font.Color = DrawingColors.Red;
        //                                textElement3.Font.Height = 3.175;
        //                                textElement4.Font.Color = DrawingColors.Red;
        //                                textElement4.Font.Height = 3.175;
        //                                mark.Attributes.Content.Add(mycontainer1);
        //                                mark.Attributes.Content.Add(newLineElement);
        //                                mark.Attributes.Content.Add(textElement1);
        //                                mark.Attributes.Content.Add(newLineElement);
        //                                mark.Attributes.Content.Add(textElement2);
        //                                mark.Attributes.Content.Add(newLineElement);
        //                                mark.Attributes.Content.Add(textElement3);
        //                                mark.Attributes.Content.Add(newLineElement);
        //                                mark.Attributes.Content.Add(textElement4);

        //                                mark.Attributes.Frame.Type = TSD.FrameTypes.None;
        //                                mark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
        //                                mark.Modify();
        //                                view.Modify();
        //                            }
        //                        }


        //                    }


        //                }



        //            }
        //        }

        //    }

        //    assemblyDrawing.CommitChanges();
        //}

    }
}
