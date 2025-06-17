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
using System.Collections;
using SK.Tekla.Drawing.Automation.Support;
using Tekla.Structures.Drawing;
using SK.Tekla.Drawing.Automation.Utils;
using Tekla.Structures.Geometry3d;
using SK.Tekla.Drawing.Automation.Models;
namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKDrawingHandler
    {
        private readonly CustomInputModel _userInput;
        private readonly SKBoundingBoxHandler _boundingBoxHandler;

        private readonly SKCatalogHandler _catalogHandler;

        public List<SectionLocationWithParts> list2 { get; set; } = new List<SectionLocationWithParts> { };

        public List<SectionLocationWithParts> list_for_flange_section { get; set; } = new List<SectionLocationWithParts> { };

        public List<TSM.Part> list_of_parts_for_bottom_part_mark_retain { get; set; } = new List<TSM.Part>();


        public TSD.StraightDimension OVERALL_DIMENSION { get; set; }

        public List<TSD.RadiusDimension> list_of_radius_dim { get; set; } = new List<RadiusDimension> { };


        public SKDrawingHandler(SKBoundingBoxHandler boundingBoxHandler,
            SKCatalogHandler catalogHandler, CustomInputModel userInput)
        {
            _boundingBoxHandler = boundingBoxHandler;
            _catalogHandler = catalogHandler;
            _userInput = userInput;
        }

        public TSD.AssemblyDrawing ResetDrawingDimensionsExceptAssemblyDimensions(TSM.Model mymodel,
            TSM.ModelObject currentBeam, string drg_attribute,
            TSM.Part main_part, double output, TSM.Assembly ASSEMBLY, double SCALE,
            double MINI_LENGTH)
        {

            TSD.DrawingHandler drg_handler = new TSD.DrawingHandler();
            //reset the values for each drawing!!!!
            OVERALL_DIMENSION = null;
            list2 = new List<SectionLocationWithParts>();
            list_for_flange_section = new List<SectionLocationWithParts>();
            list_of_parts_for_bottom_part_mark_retain = new List<TSM.Part>();
             list_of_radius_dim = new List<TSD.RadiusDimension>();



            TSD.AssemblyDrawing skAssemblyDrawing = new TSD.AssemblyDrawing(ASSEMBLY.Identifier, drg_attribute);
            skAssemblyDrawing.Insert();
            drg_handler.SetActiveDrawing(skAssemblyDrawing, true);

            ModifyViewToScale(SCALE, MINI_LENGTH, skAssemblyDrawing);

            TSD.DrawingObjectEnumerator drawingViews = skAssemblyDrawing.GetSheet().GetAllViews();



            //////////////////Enumerating different views/////////////////////////////
            while (drawingViews.MoveNext())
            {
                TSD.View current_view = drawingViews.Current as TSD.View;
                OVERALL_DIMENSION = DeleteViewDimensionByType(current_view, ASSEMBLY);
                list_of_radius_dim = ExtractRadiusDimension(current_view);
            }
            drawingViews.Reset();


            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane
                (new TSM.TransformationPlane(main_part.GetCoordinateSystem()));
               ArrayList list_of_obj = new ArrayList();

            TSD.DrawingObjectEnumerator part_enum_for_section = skAssemblyDrawing.GetSheet().GetAllViews();
            while (part_enum_for_section.MoveNext())
            {
                TSD.View current_view = part_enum_for_section.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    TSD.DrawingObjectEnumerator mypart_en = current_view.GetAllObjects(typeof(TSD.Part));
                    while (mypart_en.MoveNext())
                    {
                        TSD.Part mypart = mypart_en.Current as TSD.Part;

                        TSM.ModelObject mymodel_obj = new TSM.Model().SelectModelObject(mypart.ModelIdentifier);
                        TSM.Part mypart_for_sect = mymodel_obj as TSM.Part;
                        if (!mypart_for_sect.Identifier.ID.Equals(main_part.Identifier.ID))
                        {
                            if (!mypart_for_sect.Name.Contains("FALLTECH"))
                            {
                                TSD.PointList ptlist = _boundingBoxHandler.BoundingBoxSort(mypart_for_sect, main_part as TSM.Beam);
                                double distance = Convert.ToInt16(ptlist[0].Z) + Convert.ToInt16(ptlist[1].Z);
                                if (distance == 0)
                                {
                                    mypart_for_sect.SetUserProperty("USERDEFINED.NOTES7", "(CTRD)");
                                }
                                list_of_obj.Add(mypart_for_sect);
                            }
                        }
                    }
                }
            }


            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            List<double> mainpart_values = _catalogHandler.Getcatalog_values(main_part);

            double heightpos_neg = mainpart_values[0] / 2;

            TSD.DrawingObjectEnumerator enum_for_drg_views_del1 = skAssemblyDrawing.GetSheet().GetAllViews();
            List<List<int>> MYLIST = new List<List<int>>();
            List<req_pts> mypoints1 = new List<req_pts>();
            List<req_pts> mypoints1_duplicate = new List<req_pts>();
            List<req_pts> mypoints_duplicate_for_dimension = new List<req_pts>();
            List<int> list_ide = new List<int>();

            while (enum_for_drg_views_del1.MoveNext())
            {


                TSD.View current_view = enum_for_drg_views_del1.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    ArrayList list_of_x = new ArrayList();
                    List<req_pts> mypoints = new List<req_pts>();
                    ArrayList list_of_points = new ArrayList();
                    foreach (TSM.Part myplaye in list_of_obj)
                    {
                        TSD.PointList ptlist = _boundingBoxHandler.BoundingBoxSort(myplaye, main_part as TSM.Beam);


                        TSD.PointList m1 = ConvertPoints(ptlist, main_part as TSM.Beam, current_view);


                        if (((ptlist[0].Y > -heightpos_neg) && (ptlist[1].Y < heightpos_neg)) || ((ptlist[0].Y < -heightpos_neg) && (ptlist[1].Y > heightpos_neg)))
                        {

                            double distanceofx = ((m1[0].X + m1[1].X) / 2);
                            double distanceofy = ((m1[0].Y + m1[1].Y) / 2);
                            double distanceofZ = ((m1[0].Z + m1[1].Z) / 2);


                            TSM.Part mypart = myplaye;
                            string PARTMARK = SkTeklaDrawingUtility.get_report_properties(myplaye, "PART_POS");
                            mypoints.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            mypoints_duplicate_for_dimension.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
 

                        }
                        else
                        {
                            double distanceofx = ((m1[0].X + m1[1].X) / 2);
                            double distanceofy = ((m1[0].Y + m1[1].Y) / 2);
                            double distanceofZ = ((m1[0].Z + m1[1].Z) / 2);


                            TSM.Part mypart = myplaye;
                            string PARTMARK = SkTeklaDrawingUtility.get_report_properties(myplaye, "PART_POS");
                            list_ide.Add(mypart.Identifier.ID);
                            mypoints1.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            mypoints1_duplicate.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });

                            mypoints_duplicate_for_dimension.Add(new req_pts() { distance = distanceofx, list_of_points = m1, distance_for_y = distanceofy, part = mypart, PART_MARK = PARTMARK, distance_for_Z = distanceofZ });
                            if ((Convert.ToInt64(ptlist[0].Y) <= -Convert.ToInt64(heightpos_neg)) && (Convert.ToInt64(ptlist[1].Y) <= -Convert.ToInt64(heightpos_neg)))
                            {
                                list_of_parts_for_bottom_part_mark_retain.Add(mypart);
                            }
                        }
                    }

                    if (mypoints1.Count > 0)
                    {
                        double change_min = Math.Abs(current_view.RestrictionBox.MinPoint.Y);
                        double change_max = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);
                        if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                        {
                            current_view.RestrictionBox.MaxPoint.Y = change_min;
                            current_view.Modify();

                        }
                        else
                        {
                            current_view.RestrictionBox.MinPoint.Y = -change_max;
                            current_view.Modify();

                        }
                    }
                    else
                    {

                        double change_min = Math.Abs(current_view.RestrictionBox.MinPoint.Y);
                        double change_max = Math.Abs(current_view.RestrictionBox.MaxPoint.Y);

                        current_view.RestrictionBox.MaxPoint.Y = change_max + 65;



                        current_view.RestrictionBox.MinPoint.Y = -change_min - 65;
                        current_view.Modify();



                    }


                    mypoints = mypoints.OrderBy(x => x.distance).ToList();



                    List<req_pts> final_distance = new List<req_pts>();

                    for (int i = 0; i < mypoints.Count; i++)
                    {

                        if (i == Convert.ToInt16(mypoints.Count - 1))
                        {
                            final_distance.Add(mypoints[i]);

                        }
                        else
                        {
                            double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                            //if (ditsnace > 25)
                            if (ditsnace > 125)
                            {
                                final_distance.Add(mypoints[i]);

                            }

                            else
                            {
                                if (mypoints[i].distance != mypoints[i + 1].distance)
                                {
                                    if (mypoints[i].distance_for_y > mypoints[i + 1].distance_for_y)
                                    {
                                        final_distance.Add(mypoints[i]);
                                    }
                                    else
                                    {
                                        final_distance.Add(mypoints[i + 1]);
                                    }
                                }
                            }
                        }
                    }

                    TSM.Part main = main_part;

                    List<double> final_distance_UNIQUE = new List<double>();


                    SectionLocationWithParts obj1 = new SectionLocationWithParts();
                    List<TSM.Part> list1 = new List<TSM.Part>();


                    for (int i = 0; i < mypoints.Count; i++)
                    {

                        if (i == Convert.ToInt16(mypoints.Count - 1))
                        {
                            final_distance_UNIQUE.Add(mypoints[i].distance);
                            list1.Add(mypoints[i].part);
                            list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distance });

                        }
                        else
                        {
                            double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                            //if (ditsnace > 25)
                            if (ditsnace > 125)
                            {

                                list1.Add(mypoints[i].part);
                                list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distance });
                                list1 = new List<TSM.Part>();
                            }

                            else
                            {
                                list1.Add(mypoints[i].part);

                            }
                        }
                    }

                    List<TSM.Part> final_list = new List<TSM.Part>();
                    List<SectionLocationWithParts> f1 = new List<SectionLocationWithParts>();
                    List<SectionLocationWithParts> f2 = new List<SectionLocationWithParts>();


                    List<SectionLocationWithParts> FINAL = list2.GroupBy(X => X.PartList.Count).Select(Y => Y.FirstOrDefault()).ToList();
                    List<string> final_check_for_unique = new List<string>();
                    for (int i = list2.Count - 1; i >= 0; i--)
                    {
                        if (i == 0)
                        {
                            list2[i].SectionViewNeeded = "YES";
                        }
                        else
                        {

                            List<string> check_for_unique = new List<string>();
                            for (int j = i - 1; j >= 0; j--)
                            {


                                var first_loop = list2[i].PartList;
                                var second_loop = list2[j].PartList;

                                if (!(first_loop.Count == second_loop.Count))
                                {
                                    check_for_unique.Add("UNIQUE");

                                }
                                else
                                {
                                    bool result = ComparePartMarkOrientation(first_loop, second_loop, main_part as TSM.Beam);

                                    if (result == true)
                                    {

                                        check_for_unique.Add("SAME");
                                    }
                                    else
                                    {
                                        check_for_unique.Add("UNIQUE");
                                    }

                                }
                            }

                            if (!check_for_unique.Contains("SAME"))
                            {
                                list2[i].SectionViewNeeded = "YES";
                            }
                            else
                            {
                                list2[i].SectionViewNeeded = "NO";
                                int check = check_for_unique.LastIndexOf("SAME");
                                int check2 = check_for_unique.Count - (check + 1);
                                list2[i].IndexOfSameSection = check2;
                            }

                        }
                    }





                    list2 = list2.OrderBy(x => x.Distance).ToList();
                    List<SectionLocationWithParts> section = new List<SectionLocationWithParts>();
                    List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();

                    for (int i = 0; i < list2.Count; i++)
                    {
                        if ((list2[i].SectionViewNeeded == "YES"))
                        {
                            //if (list2[i].partlist.Count > 1)
                            //{
                            //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                            //}
                            double minx = 0;
                            double maxx = 0;
                            double mny = 0;
                            double mxy = 0;

                            minx = list2[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].X);
                            maxx = list2[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].X);
                            mny = list2[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].Y);
                            mxy = list2[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].Y);

                            double miny = Convert.ToInt64(mny);
                            double maxy = Convert.ToInt64(mxy);

                            double distanceofx = ((minx + maxx) / 2);
                            double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


                            double distance_of_y = ((miny + maxy) / 2);
                            double height = mainpart_values[0];
                            double height_for_view = 0;
                            double height_for_view1 = 0;

                            double POSIH = Convert.ToInt64(height / 2);
                            double NEGAH = -Convert.ToInt64(height / 2);



                            if ((maxy <= POSIH) && (miny >= NEGAH))
                            {
                                height_for_view = -((height / 2));
                                height_for_view1 = (height / 2);
                            }
                            else if ((maxy >= POSIH) && (miny >= POSIH))
                            {
                                height_for_view = (height / 2);
                                height_for_view1 = maxy;

                            }
                            else if ((maxy <= NEGAH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = (height / 2);

                            }
                            else if ((maxy >= POSIH) && (miny >= NEGAH))
                            {
                                height_for_view = -((height / 2));
                                height_for_view1 = maxy;
                            }

                            else if ((maxy <= POSIH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = (height / 2);
                            }
                            else if ((maxy >= POSIH) && (miny <= NEGAH))
                            {
                                height_for_view = miny;
                                height_for_view1 = maxy;
                            }




                            TSD.View bottom_view = null;
                            TSD.SectionMark sec = null;
                            double distancefor_depthup = 0;
                            double distancefor_depthdown = 0;
                            if (DISTANCE_TO_COMPARE < 300)
                            {
                                distancefor_depthup = Math.Abs(minx - distanceofx);
                                distancefor_depthdown = Math.Abs(maxx - distanceofx);
                            }
                            else if (DISTANCE_TO_COMPARE > 300)
                            {
                                distancefor_depthup = 0;
                                distancefor_depthdown = 0;

                            }

                            height_for_view = current_view.RestrictionBox.MaxPoint.Y;
                            height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
                            TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
                            TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);
                            double dep_up = maxx - distanceofx;
                            double dep_down = distanceofx - minx;
                            if (dep_up > 100)
                            {
                                dep_up = 5;
                            }
                            else
                            {
                            }
                            if (dep_down > 100)
                            {
                                dep_down = 5;
                            }

                            try
                            {
                                if (drg_attribute == "SK_BEAM_A1")
                                {
                                    bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                                    bottom_view.Modify();
                                }
                                else
                                {
                                    bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                                    bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                                    bottom_view.Modify();

                                }

                                //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("ESSKAYBEAM"), out bottom_view, out sec);


                                //bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("standard"), out bottom_view, out sec);

                                double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.X);
                                double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.X);
                                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                                {
                                    bottom_view.RestrictionBox.MaxPoint.X = change_min;
                                    bottom_view.Modify();

                                }
                                else
                                {
                                    bottom_view.RestrictionBox.MinPoint.X = -change_max;
                                    bottom_view.Modify();

                                }


                                bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
                                bottom_view.Modify();


                                TSD.FontAttributes FONT = new TSD.FontAttributes();
                                FONT.Color = TSD.DrawingColors.Magenta;
                                FONT.Height = Convert.ToInt16(3.96875);




                                //TSD.PropertyElement VIEW_LABEL = new TSD.PropertyElement(TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName);

                                TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
                                TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
                                X.Font.Color = TSD.DrawingColors.Magenta;
                                X.Font.Height = Convert.ToInt64(3.96875);

                                //TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                                TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);
                                //TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };

                                TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };


                                //sec.Attributes.TagsAttributes



                                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

                                bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

                                bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

                                bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                                sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                                bottom_view.Attributes.Scale.Equals(current_view.Attributes.Scale);

                                bottom_view.Modify();
                                //  sec.Modify();
                                list2[i].MyView = bottom_view;
                                sectionmarklist.Add(sec);
                                TSD.DrawingObjectEnumerator BOTTOM = bottom_view.GetAllObjects(typeof(TSD.Part));
                                List<int> list_req = new List<int>();
                                List<TSM.Part> mypart_list_for_section = new List<TSM.Part>();
                                while (BOTTOM.MoveNext())
                                {
                                    TSD.Part MYDRG_PART = BOTTOM.Current as TSD.Part;
                                    TSM.ModelObject MMODEL = new TSM.Model().SelectModelObject(MYDRG_PART.ModelIdentifier);
                                    TSD.PointList bounding_box_z = _boundingBoxHandler.BoundingBoxSort(MMODEL, bottom_view, SKSortingHandler.SortBy.Z);

                                    if ((Convert.ToInt64(bounding_box_z[1].Z) >= Convert.ToInt64(bottom_view.RestrictionBox.MinPoint.Z)) && (Convert.ToInt64(bounding_box_z[0].Z) <= Convert.ToInt64(bottom_view.RestrictionBox.MaxPoint.Z)))
                                    {
                                        TSM.Part mmpart = MMODEL as TSM.Part;
                                        if (!mmpart.Identifier.ID.Equals(main_part.Identifier.ID))
                                        {
                                            list_req.Add(mmpart.Identifier.ID);
                                            //try
                                            //{
                                            //list2[i].req_partlist.Add(mmpart);
                                            mypart_list_for_section.Add(mmpart);

                                            //}
                                            //catch
                                            //{
                                            //}
                                            //MYLIST.Add(mmpart.Identifier.ID);
                                        }

                                    }


                                }
                                list2[i].RequiredPartList = mypart_list_for_section;
                                MYLIST.Add(list_req);
                            }
                            catch
                            {
                            }

                        }

                        else
                        {

                            TSD.SectionMark sec_dummy = null;
                            sectionmarklist.Add(sec_dummy);
                            TSD.SectionMark mysec = sectionmarklist[list2[i].IndexOfSameSection];

                            mysec.LeftPoint.X = list2[i].Distance;
                            mysec.RightPoint.X = list2[i].Distance;
                            mysec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                            mysec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;






                            mysec.Insert();

                        }






                    }



                }
                skAssemblyDrawing.PlaceViews();





            }
            //int end_while = Environment.TickCount;
            //Console.WriteLine("Time elapsed for outer while ---> " + (end_while-while_start));

            TSD.DrawingObjectEnumerator enum_for_flange_sect = skAssemblyDrawing.GetSheet().GetAllViews();
            while (enum_for_flange_sect.MoveNext())
            {


                TSD.View current_view = enum_for_flange_sect.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    List<List<int>> final_result = new List<List<int>>();
                    foreach (List<int> myllist in MYLIST)
                    {

                        List<int> dup_list = myllist.Intersect(list_ide).ToList();
                        final_result.Add(dup_list);



                    }


                    foreach (List<int> section_list in final_result)
                    {
                        for (int p = 0; p < mypoints1.Count; p++)
                        {
                            bool result = section_list.Any(x => x.Equals(mypoints1[p].part.Identifier.ID));
                            if (result == true)
                            {
                                mypoints1_duplicate.RemoveAll(x => x.part.Identifier.ID.Equals(mypoints1[p].part.Identifier.ID));
                            }
                        }

                    }
                    if (mypoints1_duplicate.Count > 0)
                    {



                        CompareSectionViews(mypoints1_duplicate, current_view, main_part, MYLIST, skAssemblyDrawing, drg_attribute);

                    }

                    else
                    {
                        list_for_flange_section = new List<SectionLocationWithParts>();


                    }
                }
            }
            skAssemblyDrawing.CommitChanges();
            return skAssemblyDrawing;
        }

        private List<RadiusDimension> ExtractRadiusDimension(TSD.View currentView)
        {
            List<RadiusDimension> radiusDimensions = new List<RadiusDimension>();
            Type[] radius_dim = new Type[] { typeof(TSD.RadiusDimension) };
            TSD.DrawingObjectEnumerator dim_drg = currentView.GetAllObjects(radius_dim);
            while (dim_drg.MoveNext())
            {
                var dim_del = dim_drg.Current;
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    if (dim_del.GetType().Equals(typeof(TSD.RadiusDimension)))
                    {
                        TSD.RadiusDimension dimension = dim_del as TSD.RadiusDimension;
                        radiusDimensions.Add(dimension);
                    }
                }
            }
            return radiusDimensions;
        }

        private TSD.StraightDimension DeleteViewDimensionByType(TSD.View current_view, TSM.Assembly ASSEMBLY)
        {
            TSD.PointList ASSEMBLY_BOUNDING_BOX = _boundingBoxHandler.BoundingBoxForDimension(ASSEMBLY);

            TSG.Point startWorkPoint = ASSEMBLY_BOUNDING_BOX[0];
            TSG.Point endWorkPoint = ASSEMBLY_BOUNDING_BOX[1];
            Type[] type_for_dim = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet),
                typeof(TSD.AngleDimension) };
            List<DimensionWithDifference> MYDIM_WITH_DIFFER = new List<DimensionWithDifference>();
            List<DimensionWithDifference> MYDIM_WITH_DIFFER_ORIGINAL = new List<DimensionWithDifference>();
            TSD.DrawingObjectEnumerator dim_drg = current_view.GetAllObjects(type_for_dim);
            #region front_view_dimension_delete

            while (dim_drg.MoveNext())
            {
                var dim_del = dim_drg.Current;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {

                    TSG.Matrix toviewmatrix = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);
                    TSG.Point workpointst2 = toviewmatrix.Transform(startWorkPoint);

                    TSG.Point workpointend2 = toviewmatrix.Transform(endWorkPoint);
                    if (dim_del.GetType().Equals(typeof(TSD.StraightDimension)))
                    {
                        TSD.StraightDimension DIM = dim_del as TSD.StraightDimension;
                        double X = DIM.StartPoint.X;
                        double X1 = DIM.EndPoint.X;
                        double DIFFERENCE = X1 - X;
                        TSG.Vector MYVECTOR = DIM.UpDirection;

                        MYDIM_WITH_DIFFER.Add(new DimensionWithDifference { StDimen = DIM, Difference = DIFFERENCE, MyVector = MYVECTOR });
                        MYDIM_WITH_DIFFER_ORIGINAL.Add(new DimensionWithDifference { StDimen = DIM, Difference = DIFFERENCE, MyVector = MYVECTOR });


                    }
                    else if (dim_del.GetType().Equals(typeof(TSD.StraightDimensionSet)))
                    {
                        TSD.StraightDimensionSet RD = dim_del as TSD.StraightDimensionSet;

                        if (RD.Attributes.DimensionType == TSD.DimensionSetBaseAttributes.DimensionTypes.Elevation)
                        {
                            dim_del.Delete();
                        }
                    }
                    else if (dim_del.GetType().Equals(typeof(TSD.AngleDimension)))
                    {
                        dim_del.Delete();
                    }
                    else
                    {
                        dim_del.Delete();
                    }

                }
                else if (current_view.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    if (!dim_del.GetType().Equals(typeof(TSD.AngleDimension)))
                    {
                        dim_del.Delete();
                    }
                }
                else
                {
                    dim_del.Delete();
                }

            }


            #endregion
            MYDIM_WITH_DIFFER.RemoveAll(X => X.MyVector.Y > 0);

            List<DimensionWithDifference> OVERALL_DIM = MYDIM_WITH_DIFFER.
                Where((X => (X.Difference.Equals(MYDIM_WITH_DIFFER.Max(Y => Y.Difference))))).ToList();
            List<DimensionWithDifference> IO = MYDIM_WITH_DIFFER_ORIGINAL.
                Where(X => !X.Difference.Equals(OVERALL_DIM[0].Difference)).ToList();

            if (_userInput.KnockOffDimension)
            {
                IO.RemoveAll(X => HasKnockOffDimension(X.StDimen) == true);
            }

            foreach (var MYDIM in IO)
            {

                MYDIM.StDimen.Delete();

            }
            TSD.StraightDimension OVERALL_DIMENSION = null;
            foreach (var MYDIM in OVERALL_DIM)
            {
                OVERALL_DIMENSION = MYDIM.StDimen;
            }
            return OVERALL_DIMENSION;
        }

        private void ModifyViewToScale(double SCALE, double MINI_LENGTH, AssemblyDrawing ASSEMBLY_DRAWING)
        {
            TSD.DrawingObjectEnumerator enum_for_drg_views_del_1 = ASSEMBLY_DRAWING.GetSheet().GetAllViews();
            while (enum_for_drg_views_del_1.MoveNext())
            {


                TSD.View current_view = enum_for_drg_views_del_1.Current as TSD.View;
                current_view.Attributes.Scale = SCALE;

                current_view.Attributes.Shortening.MinimumLength = MINI_LENGTH;
                current_view.Attributes.Shortening.CutPartType = TSD.View.ShorteningCutPartType.X_Direction;

                current_view.Modify();
                ASSEMBLY_DRAWING.Modify();
            }
        }

        private bool HasKnockOffDimension(TSD.StraightDimension MYDIM)
        {
            TSD.ContainerElement dimval = MYDIM.Value;

            IEnumerator check1 = dimval.GetEnumerator();

            List<string> list_of_texts = new List<string>();
            while (check1.MoveNext())
            {
                var name = check1.Current;
                if (name.GetType().Equals(typeof(TSD.TextElement)))
                {
                    TSD.TextElement check2 = check1.Current as TSD.TextElement;
                    string value = check2.Value;

                    list_of_texts.Add(value);


                }
            }
            return list_of_texts.Any(x => x.Contains("("));
        }

        private bool ComparePartMarkOrientation(List<TSM.Part> LIST1, List<TSM.Part> LIST2, TSM.Beam MAINPART)
        {
            TSM.Model MODEL = new TSM.Model();
            bool result;
            MODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(MAINPART.GetCoordinateSystem()));
            List<string> CHECK_FOR_SAME_SECTION = new List<string>();
            foreach (TSM.Part MYPART in LIST1)
            {
                string CHECK_FOR_SAME_PART = "";
                List<TSM.Part> LISTOFPARTS = new List<TSM.Part>();
                LISTOFPARTS.Add(MYPART);
                foreach (TSM.Part MYPART1 in LIST2)
                {

                    string mypartMARK_FOR_FIRST = SkTeklaDrawingUtility.get_report_properties(MYPART, "PART_POS");
                    string mypartMARK_FOR_SECOND = SkTeklaDrawingUtility.get_report_properties(MYPART1, "PART_POS");
                    if (mypartMARK_FOR_FIRST == mypartMARK_FOR_SECOND)
                    {
                        TSG.CoordinateSystem COORD1 = MYPART.GetCoordinateSystem();
                        TSG.CoordinateSystem COORD2 = MYPART1.GetCoordinateSystem();

                        if ((Convert.ToInt64(COORD1.Origin.Y) == Convert.ToInt64(COORD2.Origin.Y)) && (Convert.ToInt64(COORD1.Origin.Z) == Convert.ToInt64(COORD2.Origin.Z)))
                        {
                            if ((vectorCheck(COORD1.AxisX, COORD2.AxisX) && (vectorCheck(COORD1.AxisY, COORD2.AxisY))))
                            {
                                CHECK_FOR_SAME_PART = "SAME_PART";
                                break;
                            }

                        }

                    }


                }

                if (CHECK_FOR_SAME_PART == "SAME_PART")
                {
                    CHECK_FOR_SAME_SECTION.Add("SAME_SECTION");
                }

            }

            if (CHECK_FOR_SAME_SECTION.Count == LIST1.Count)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            MODEL.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            return result;
        }
        private bool vectorCheck(TSG.Vector vector1, TSG.Vector vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            return (Convert.ToInt64(vector1.X) == Convert.ToInt64(vector2.X)) && (Convert.ToInt64(vector1.Y) == Convert.ToInt64(vector2.Y)) &&
                (Convert.ToInt64(vector1.Z) == Convert.ToInt64(vector2.Z));
        }

        private void CompareSectionViews(List<req_pts> mypoints, TSD.View current_view, TSM.Part main_part,
            List<List<int>> mypart_list_of_created_sect_view, TSD.AssemblyDrawing ASSEMBLY_DRAWING, 
             string drg_att)
        {
            List<double> mainpart_values = _catalogHandler.Getcatalog_values(main_part);

            list2 = new List<SectionLocationWithParts>();
            mypoints = mypoints.OrderBy(x => x.distance).ToList();



            List<req_pts> final_distance = new List<req_pts>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance.Add(mypoints[i]);

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 25)
                    {
                        final_distance.Add(mypoints[i]);

                    }

                    else
                    {
                        if (mypoints[i].distance != mypoints[i + 1].distance)
                        {
                            if (mypoints[i].distance_for_y > mypoints[i + 1].distance_for_y)
                            {
                                final_distance.Add(mypoints[i]);
                            }
                            else
                            {
                                final_distance.Add(mypoints[i + 1]);
                                //final_distance.Add(mypoints[i ]);
                            }
                        }
                    }
                }
            }
            TSM.Part main = main_part;

            List<double> final_distance_UNIQUE = new List<double>();


            SectionLocationWithParts obj1 = new SectionLocationWithParts();
            List<TSM.Part> list1 = new List<TSM.Part>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance_UNIQUE.Add(mypoints[i].distance);
                    list1.Add(mypoints[i].part);
                    list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distance });

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distance) - Convert.ToInt16(mypoints[i].distance));
                    if (ditsnace > 25)
                    {

                        list1.Add(mypoints[i].part);
                        list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distance });
                        list1 = new List<TSM.Part>();
                    }

                    else
                    {
                        list1.Add(mypoints[i].part);

                    }
                }
            }

            List<TSM.Part> final_list = new List<TSM.Part>();
            List<SectionLocationWithParts> f1 = new List<SectionLocationWithParts>();
            List<SectionLocationWithParts> f2 = new List<SectionLocationWithParts>();


            List<SectionLocationWithParts> FINAL = list2.GroupBy(X => X.PartList.Count).Select(Y => Y.FirstOrDefault()).ToList();
            List<string> final_check_for_unique = new List<string>();
            for (int i = list2.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    list2[i].SectionViewNeeded = "YES";
                }
                else
                {

                    List<string> check_for_unique = new List<string>();
                    for (int j = i - 1; j >= 0; j--)
                    {


                        var first_loop = list2[i].PartList;
                        var second_loop = list2[j].PartList;

                        if (!(first_loop.Count == second_loop.Count))
                        {
                            check_for_unique.Add("UNIQUE");

                        }
                        else
                        {
                            bool result = ComparePartMarkOrientation(first_loop, second_loop, main_part as TSM.Beam);

                            if (result == true)
                            {

                                check_for_unique.Add("SAME");
                            }
                            else
                            {
                                check_for_unique.Add("UNIQUE");
                            }

                        }
                    }

                    if (!check_for_unique.Contains("SAME"))
                    {
                        list2[i].SectionViewNeeded = "YES";
                    }
                    else
                    {
                        list2[i].SectionViewNeeded = "NO";
                        int check = check_for_unique.LastIndexOf("SAME");
                        int check2 = check_for_unique.Count - (check + 1);
                        list2[i].IndexOfSameSection = check2;
                    }

                }
            }





            list2 = list2.OrderBy(x => x.Distance).ToList();
            List<SectionLocationWithParts> section = new List<SectionLocationWithParts>();
            List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();








            for (int i = 0; i < list2.Count; i++)
            {
                if ((list2[i].SectionViewNeeded == "YES"))
                {
                    //if (list2[i].partlist.Count > 1)
                    //{
                    //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                    //}
                    double minx = 0;
                    double maxx = 0;
                    double mny = 0;
                    double mxy = 0;

                    minx = list2[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].X);
                    maxx = list2[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].X);
                    mny = list2[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].Y);
                    mxy = list2[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].Y);

                    double miny = Convert.ToInt64(mny);
                    double maxy = Convert.ToInt64(mxy);

                    double distanceofx = ((minx + maxx) / 2);
                    double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


                    double distance_of_y = ((miny + maxy) / 2);
                    double height = Convert.ToDouble(mainpart_values[0]);
                    double height_for_view = 0;
                    double height_for_view1 = 0;

                    double POSIH = Convert.ToInt64(height / 2);
                    double NEGAH = -Convert.ToInt64(height / 2);



                    if ((maxy <= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny >= POSIH))
                    {
                        height_for_view = (height / 2);
                        height_for_view1 = maxy;

                    }
                    else if ((maxy <= NEGAH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);

                    }
                    else if ((maxy >= POSIH) && (miny >= NEGAH))
                    {
                        height_for_view = -((height / 2));
                        height_for_view1 = maxy;
                    }

                    else if ((maxy <= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = (height / 2);
                    }
                    else if ((maxy >= POSIH) && (miny <= NEGAH))
                    {
                        height_for_view = miny;
                        height_for_view1 = maxy;
                    }




                    TSD.View bottom_view = null;


                    TSD.SectionMark sec = null;

                    double distancefor_depthup = 0;
                    double distancefor_depthdown = 0;
                    if (DISTANCE_TO_COMPARE < 300)
                    {
                        distancefor_depthup = Math.Abs(minx - distanceofx);
                        distancefor_depthdown = Math.Abs(maxx - distanceofx);
                    }
                    else if (DISTANCE_TO_COMPARE > 300)
                    {
                        distancefor_depthup = 0;
                        distancefor_depthdown = 0;

                    }

                    height_for_view = current_view.RestrictionBox.MaxPoint.Y;
                    height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
                    TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
                    TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);


                    double dep_up = maxx - distanceofx;
                    double dep_down = distanceofx - minx;
                    try
                    {

                        if (drg_att == "SK_BEAM_A1")
                        {
                            bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                            bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                            bottom_view.Modify();


                        }
                        else
                        {
                            bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100, current_view.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                            bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                            bottom_view.Modify();
                        }

                        sec.Attributes.LineLengthOffset = 0;
                        sec.Modify();

                        double change_min = Math.Abs(bottom_view.RestrictionBox.MinPoint.X);
                        double change_max = Math.Abs(bottom_view.RestrictionBox.MaxPoint.X);


                        if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                        {
                            bottom_view.RestrictionBox.MaxPoint.X = change_min;
                            bottom_view.Modify();

                        }
                        else
                        {
                            bottom_view.RestrictionBox.MinPoint.X = -change_max;
                            bottom_view.Modify();

                        }


                        bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
                        bottom_view.Modify();


                        TSD.FontAttributes FONT = new TSD.FontAttributes();
                        FONT.Color = TSD.DrawingColors.Magenta;
                        FONT.Height = Convert.ToInt16(3.96875);
                        //bottom_view.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewFrame;
                        //bottom_view.Modify();


                        //TSD.TextElement textelement2 = new TSD.TextElement(sec.Attributes.MarkName, FONT);
                        TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);

                        TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
                        TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
                        X.Font.Color = TSD.DrawingColors.Magenta;
                        X.Font.Height = Convert.ToInt64(3.96875);


                        //TSD.ContainerElement sectionmark = new TSD.ContainerElement { textelement2, textelement3, textelement2 };

                        TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };



                        sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

                        bottom_view.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

                        bottom_view.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

                        bottom_view.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
                        sec.Attributes.LineColor = TSD.DrawingColors.Magenta;
                        bottom_view.Attributes.Scale.Equals(current_view.Attributes.Scale);

                        bottom_view.Modify();


                        list2[i].MyView = bottom_view;
                        sectionmarklist.Add(sec);


                    }
                    catch
                    {
                    }

                }

                else
                {

                    TSD.SectionMark sec_dummy = null;
                    sectionmarklist.Add(sec_dummy);
                    TSD.SectionMark mysec = sectionmarklist[list2[i].IndexOfSameSection];

                    mysec.LeftPoint.X = list2[i].Distance;
                    mysec.RightPoint.X = list2[i].Distance;
                    mysec.Insert();

                }






            }

        }

        private TSD.PointList ConvertPoints(TSD.PointList list_of_points, TSM.Beam mainpart, TSD.View current_view)
        {
            TSD.PointList webb = new TSD.PointList();

            TSG.Matrix toviewpart = TSG.MatrixFactory.FromCoordinateSystem(mainpart.GetCoordinateSystem());

            foreach (TSG.Point pt in list_of_points)
            {
                TSG.Point mtpt = toviewpart.Transform(pt);
                webb.Add(mtpt);
            }

            TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            TSD.PointList webb1 = new TSD.PointList();
            foreach (TSG.Point pt in webb)
            {
                TSG.Point mtpt = toviewpart1.Transform(pt);
                webb1.Add(mtpt);
            }

            return webb1;
        }



        public TSG.Point ConvertedPointsForChannel(TSM.Model mymodel, TSD.View current_view)
        {
            TSG.Point point = new TSG.Point(0, 0, 0);
            TSG.Matrix toglobal = mymodel.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToGlobal;


            TSG.Point mtpt = toglobal.Transform(point);


            TSG.Matrix toviewpart1 = TSG.MatrixFactory.ToCoordinateSystem(current_view.DisplayCoordinateSystem);

            TSG.Point mtpt1 = toviewpart1.Transform(mtpt);

            return mtpt1;


        }



    }
}
