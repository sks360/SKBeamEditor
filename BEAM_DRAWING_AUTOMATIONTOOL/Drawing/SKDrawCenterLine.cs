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
namespace SK.Tekla.Drawing.Automation.Drawing
{
    public class SKDrawCenterLine
    {
        public SKDrawCenterLine() { }



        public void centre_line(TSD.AssemblyDrawing beam_assembly_drg, TSM.Part main_part)
        {

            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingObjectEnumerator MYDRG_VIEWS = beam_assembly_drg.GetSheet().GetAllViews();
            while (MYDRG_VIEWS.MoveNext())
            {
                TSD.View MYVIEW = MYDRG_VIEWS.Current as TSD.View;
                if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    TSD.DrawingObjectEnumerator MYPARTENUM = MYVIEW.GetAllObjects(type_for_part);
                    while (MYPARTENUM.MoveNext())
                    {
                        TSD.Part MYPART = MYPARTENUM.Current as TSD.Part;
                        TSM.ModelObject modelPART = new TSM.Model().SelectModelObject(MYPART.ModelIdentifier);
                        TSM.Part PART = modelPART as TSM.Part;
                        if (PART.Identifier.GUID.Equals(main_part.Identifier.GUID))
                        {


                            MYPART.Attributes.DrawCenterLine = true;
                            MYPART.Attributes.SymbolOffset = 0;
                            MYPART.Modify();
                            MYVIEW.Modify();
                            beam_assembly_drg.CommitChanges();

                        }
                    }
                }
                else if (MYVIEW.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    TSD.DrawingObjectEnumerator MYPARTENUM = MYVIEW.GetAllObjects(type_for_part);
                    while (MYPARTENUM.MoveNext())
                    {
                        TSD.Part MYPART = MYPARTENUM.Current as TSD.Part;
                        TSM.ModelObject modelPART = new TSM.Model().SelectModelObject(MYPART.ModelIdentifier);
                        TSM.Part PART = modelPART as TSM.Part;
                        if (PART.Identifier.GUID.Equals(main_part.Identifier.GUID))
                        {
                            string prof_typ = "";
                            main_part.GetReportProperty("PROFILE_TYPE", ref prof_typ);
                            if ((prof_typ != "I") && (prof_typ != "U"))
                            {

                                MYPART.Attributes.DrawCenterLine = true;
                                MYPART.Attributes.SymbolOffset = 0;
                                MYPART.Modify();
                                MYVIEW.Modify();
                            }
                            else if ((prof_typ == "U") && (!PART.Name.Contains("BEAM")))
                            {
                                MYPART.Attributes.DrawOrientationMark = false;
                                MYPART.Modify();

                            }


                        }
                    }
                }
            }
        }

    }
}
