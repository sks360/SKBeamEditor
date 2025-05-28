using System;
using System.Collections;
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

using Tekla.Structures.Drawing;
using Tekla.Structures.DrawingInternal;
using MySqlX.XDevAPI;

namespace SK.Tekla.Drawing.Automation.Services
{
    public class DimensionMarker
    {
        private readonly string client;

        public DimensionMarker(string client)
        {
            this.client = client;
        }

        public void Dim_Fix(AssemblyDrawing assemblyDrawing)
        {
            DrawingObjectEnumerator drawingObjectEnumerator = assemblyDrawing.GetSheet().GetAllObjects();
            ArrayList st_dims = new ArrayList();
            Dictionary<int, double> distances = new Dictionary<int, double>();
            while (drawingObjectEnumerator.MoveNext())
            {
                var mark = drawingObjectEnumerator.Current;

                if (mark.GetType().Equals(typeof(TSD.AngleDimension)))
                {
                    TSD.AngleDimension angledim = mark as TSD.AngleDimension;
                    angledim.Attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Free;
                    angledim.Modify();


                }
                else if (mark.GetType().Equals(typeof(TSD.RadiusDimension)))
                {
                    TSD.RadiusDimension angledim = mark as TSD.RadiusDimension;
                    angledim.Attributes.Placing.Placing = TSD.DimensionSetBaseAttributes.Placings.Free;
                    angledim.Modify();

                }
                else if (mark.GetType().Equals(typeof(TSD.StraightDimension)))
                {

                    st_dims.Add(mark);
                }
            }

            foreach (StraightDimension straightDimension in st_dims)
            {

                int id = straightDimension.GetDimensionSet().GetIdentifier().ID;
                if (distances.Count == 0)
                {
                    distances.Add(id, Math.Abs(straightDimension.EndPoint.X - straightDimension.StartPoint.X));
                }
                else
                {
                    if (distances.ContainsKey(id))
                    {
                        double temp_dist = distances[id];
                        if (temp_dist < Math.Abs(straightDimension.EndPoint.X - straightDimension.StartPoint.X))
                        {
                            distances.Remove(id);
                            distances.Add(id, Math.Abs(straightDimension.EndPoint.X - straightDimension.StartPoint.X));
                        }
                    }
                    else
                    {
                        distances.Add(id, Math.Abs(straightDimension.EndPoint.X - straightDimension.StartPoint.X));
                    }
                }
            }

            if (distances.Count > 0)
            {
                var max = distances.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                int big_id = Convert.ToInt32(max);


                foreach (StraightDimension straightDimension in st_dims)
                {
                    if (straightDimension.GetDimensionSet().GetIdentifier().ID != big_id)
                    {
                        TSD.StraightDimensionSet strdim = straightDimension.GetDimensionSet() as TSD.StraightDimensionSet;
                        strdim.Attributes.Placing.Placing = DimensionSetBaseAttributes.Placings.Free;
                        strdim.Modify();

                    }
                    else
                    {

                    }
                }
            }



            assemblyDrawing.CommitChanges();
        }

        public void ATT_SETT(TSD.AssemblyDrawing assembly_drg, TSM.Part main_part)
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
                            if (mark_part.GetType().Equals(typeof(TSD.Part)))
                            {
                                TSM.Part modelpart = new TSM.Model().SelectModelObject((mark_part as TSD.Part).ModelIdentifier) as TSM.Part;

                                Guid guid = modelpart.Identifier.GUID;
                                if (guid != main_part.Identifier.GUID)
                                {
                                    TSD.ContainerElement MYCONTAINER_ELEMENT = mymark.Attributes.Content;
                                    List<string> RESULT_FOR_SLOT = new List<string>();
                                    IEnumerator CHECK1 = MYCONTAINER_ELEMENT.GetEnumerator();
                                    while (CHECK1.MoveNext())
                                    {
                                        var NAME = CHECK1.Current;
                                        if (NAME.GetType().Equals(typeof(TSD.SymbolElement)))
                                        {

                                            RESULT_FOR_SLOT.Add("TRUE");

                                            mymark.Attributes.ArrowHead.ArrowPosition = TSD.ArrowheadPositions.None;
                                            mymark.Modify();


                                        }
                                    }
                                    bool RESULT = RESULT_FOR_SLOT.Any(X => X.Contains("TRUE"));
                                    if (RESULT == false)
                                    {
                                        if ((client.Equals("LIPHART")) || (client.Equals("TRINITY") || (client.Equals("SME"))))
                                        {
                                            mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                            mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
                                            mymark.Modify();
                                        }
                                        else
                                        {
                                            mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                                            mymark.Attributes.ArrowHead.ArrowPosition = TSD.ArrowheadPositions.None;
                                            mymark.Modify();

                                        }

                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        TSD.MarkSet mymark = MYMARK_ENUM.Current as TSD.MarkSet;
                        if ((client.Equals("LIPHART")) || (client.Equals("TRINITY")) || (client.Equals("SME")))
                        {
                            mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                            mymark.Attributes.ArrowHead.Head = TSD.ArrowheadTypes.FilledArrow;
                            mymark.Modify();
                        }
                        else
                        {
                            mymark.Attributes.Frame.Type = TSD.FrameTypes.None;
                            mymark.Attributes.ArrowHead.ArrowPosition = TSD.ArrowheadPositions.None;
                            mymark.Modify();

                        }

                    }
                }
            }
        }
    }
}
