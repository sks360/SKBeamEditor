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
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;

namespace SK.Tekla.Drawing.Automation.Drawing.Views
{
    public class FixFreeSpace
    {
        public FixFreeSpace() { }

        public void PART_MARK_FREE_FIX(TSD.AssemblyDrawing beam_assembly_drg, TSM.Part main, TSD.DrawingHandler my_handler)
        {

            TSD.DrawingObjectEnumerator WELD_MARK_ENUM = beam_assembly_drg.GetSheet().GetAllViews();
            while (WELD_MARK_ENUM.MoveNext())
            {
                Type type_for_PART_MARK = typeof(TSD.Mark);
                TSD.View MYVIEW = WELD_MARK_ENUM.Current as TSD.View;
                TSD.DrawingObjectEnumerator my_top_view_dimension_check = MYVIEW.GetAllObjects(type_for_PART_MARK);

                while (my_top_view_dimension_check.MoveNext())
                {
                    TSD.Mark PARTmark = my_top_view_dimension_check.Current as TSD.Mark;
                    my_handler.GetDrawingObjectSelector().SelectObject(PARTmark);
                    TSM.Operations.Operation.RunMacro(@"..\drawings\ReFreeplaceSelected.cs");
                    my_handler.GetDrawingObjectSelector().UnselectAllObjects();
                    beam_assembly_drg.CommitChanges();
                }
            }
        }
    }
}
