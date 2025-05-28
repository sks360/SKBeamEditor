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
    public class CreateOrientationMark
    {
        public CreateOrientationMark() { }

        public void orientationmark(TSM.Model currentModel, TSD.Drawing mydrg, TSM.Part part)
        {
            Type type_for_part = typeof(TSD.Part);
            TSD.DrawingObjectEnumerator enum_for_views = mydrg.GetSheet().GetAllViews();
            while (enum_for_views.MoveNext())
            {
                TSD.View current_view = enum_for_views.Current as TSD.View;
                TSD.Part.PartAttributes orientation = new TSD.Part.PartAttributes();

                TSD.DrawingObjectEnumerator enum_for_orientation = current_view.GetAllObjects(type_for_part);
                while (enum_for_orientation.MoveNext())
                {

                    TSD.Part mypart = enum_for_orientation.Current as TSD.Part;
                    if ((current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView)))
                    {
                        if ((currentModel.SelectModelObject(mypart.ModelIdentifier) as TSM.Part).Identifier.GUID.ToString() == part.Identifier.GUID.ToString())
                        {
                            if (mypart.Attributes.DrawOrientationMark.Equals(false))
                            {
                                orientation.DrawOrientationMark = true;
                                mypart.Attributes = orientation;
                                mypart.Modify();
                                current_view.Modify();
                            }
                        }
                        else
                        {
                            if (mypart.Attributes.DrawOrientationMark.Equals(true))
                            {
                                orientation.DrawOrientationMark = false;
                                mypart.Attributes = orientation;
                                current_view.Modify();
                                mypart.Modify();
                            }
                        }
                    }

                    else if ((current_view.ViewType.Equals(TSD.View.ViewTypes.TopView) && mypart.Attributes.DrawOrientationMark.Equals(true)))
                    {
                        orientation.DrawOrientationMark = false;
                        mypart.Attributes = orientation;
                        current_view.Modify();
                        mypart.Modify();

                    }
                    else if ((current_view.ViewType.Equals(TSD.View.ViewTypes.SectionView) && mypart.Attributes.DrawOrientationMark.Equals(true)))
                    {
                        orientation.DrawOrientationMark = false;
                        current_view.Modify();
                        mypart.Modify();

                    }

                }

            }

        }


    }
}
