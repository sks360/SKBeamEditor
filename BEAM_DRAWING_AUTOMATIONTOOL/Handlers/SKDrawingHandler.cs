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
using static SK.Tekla.Drawing.Automation.Handlers.SKSortingHandler;
using Tekla.Structures;

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


        public List<TSD.RadiusDimension> radiusDimensionList { get; set; } = new List<RadiusDimension> { };


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

            list2 = new List<SectionLocationWithParts>();
            list_for_flange_section = new List<SectionLocationWithParts>();
            list_of_parts_for_bottom_part_mark_retain = new List<TSM.Part>();
            radiusDimensionList = new List<TSD.RadiusDimension>();



            TSD.AssemblyDrawing skAssemblyDrawing = new TSD.AssemblyDrawing(ASSEMBLY.Identifier, drg_attribute);
            skAssemblyDrawing.Insert();
            drg_handler.SetActiveDrawing(skAssemblyDrawing, true);

            ModifyViewToScale(SCALE, MINI_LENGTH, skAssemblyDrawing);

            var drawingViews = skAssemblyDrawing.GetSheet().GetAllViews();

            double mainPartHeight = 0;
            main_part.GetReportProperty("HEIGHT", ref mainPartHeight);
            var heightPosNeg = mainPartHeight / 2;

            //////////////////Enumerating different views/////////////////////////////
            while (drawingViews.MoveNext())
            {
                TSD.View currentView = drawingViews.Current as TSD.View;
                DeleteViewDimensionByType(currentView, ASSEMBLY);
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    radiusDimensionList = ExtractRadiusDimension(currentView);
                }
            }
            drawingViews.Reset();

            //transform the plane to Global to get model parts
            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane
                (new TSM.TransformationPlane(main_part.GetCoordinateSystem()));
            ArrayList fvMPartList = new ArrayList();

            TSD.DrawingObjectEnumerator enumForParts = skAssemblyDrawing.GetSheet().GetAllViews();
            while (enumForParts.MoveNext())
            {
                TSD.View current_view = enumForParts.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    fvMPartList = GetFrontViewParts(main_part, current_view);
                }
            }


            mymodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            TSD.DrawingObjectEnumerator placeViewEnum = skAssemblyDrawing.GetSheet().GetAllViews();
            List<List<int>> MYLIST = new List<List<int>>();
            List<RequiredPartPoints> mypoints1 = new List<RequiredPartPoints>();
            List<RequiredPartPoints> mypoints1_duplicate = new List<RequiredPartPoints>();
            List<RequiredPartPoints> mypoints_duplicate_for_dimension = new List<RequiredPartPoints>();
            List<int> list_ide = new List<int>();

            while (placeViewEnum.MoveNext())
            {
                TSD.View current_view = placeViewEnum.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    List<RequiredPartPoints> mypoints = new List<RequiredPartPoints>();

                    foreach (TSM.Part mPart in fvMPartList)
                    {
                        TSD.PointList pmPts = _boundingBoxHandler.BoundingBoxSort(mPart, main_part as TSM.Beam);
                        Console.WriteLine($"0th record: x: {pmPts[0].X}  y: {pmPts[0].Y}  z: {pmPts[0].Z}");
                        Console.WriteLine($"1st record: x: {pmPts[1].X}  y: {pmPts[1].Y}  z: {pmPts[1].Z}");
                        TSM.Part mypart = mPart;

                        TSD.PointList pvPts = ConvertToViewPoints(pmPts, main_part as TSM.Beam, current_view);
                        Console.WriteLine($"m1 0th record: x: {pvPts[0].X}  y: {pvPts[0].Y}  z: {pvPts[0].Z}");
                        Console.WriteLine($"m1 1st record: x: {pvPts[1].X}  y: {pvPts[1].Y}  z: {pvPts[1].Z}");
                        RequiredPartPoints requiredPart =
                            new RequiredPartPoints()
                            {
                                distanceX = ((pvPts[0].X + pvPts[1].X) / 2),
                                distanceY = ((pvPts[0].Y + pvPts[1].Y) / 2),
                                distanceZ = ((pvPts[0].Z + pvPts[1].Z) / 2),
                                pointList = pvPts,
                                part = mPart,
                                partMark = SkTeklaDrawingUtility.get_report_properties(mPart, "PART_POS"),
                                ID = mPart.Identifier.ID
                            };



                        if (((pmPts[0].Y > -heightPosNeg) && (pmPts[1].Y < heightPosNeg)) || ((pmPts[0].Y < -heightPosNeg) && (pmPts[1].Y > heightPosNeg)))
                        {
                            mypoints.Add(requiredPart);
                            mypoints_duplicate_for_dimension.Add(requiredPart);
                        }
                        else
                        {
                            list_ide.Add(mypart.Identifier.ID);
                            mypoints1.Add(requiredPart);
                            mypoints1_duplicate.Add(requiredPart);
                            mypoints_duplicate_for_dimension.Add(requiredPart);
                            if ((Convert.ToInt64(pmPts[0].Y) <= -Convert.ToInt64(heightPosNeg)) && (Convert.ToInt64(pmPts[1].Y) <= -Convert.ToInt64(heightPosNeg)))
                            {
                                list_of_parts_for_bottom_part_mark_retain.Add(mPart);
                            }
                        }
                    }
                    Console.WriteLine($"mypoints: {mypoints.Count}");
                    mypoints.ForEach(Console.WriteLine);

                    Console.WriteLine($"mypoints1: {mypoints1.Count}");
                    mypoints1.ForEach(Console.WriteLine);

                    AdjustRestrictionBox(mypoints1, current_view);

                    GetFrontViewPoints(drg_attribute, main_part, mainPartHeight, MYLIST, current_view, mypoints);

                }
                skAssemblyDrawing.PlaceViews();
            }

            GetFlangeSections(drg_attribute, main_part, skAssemblyDrawing, MYLIST, mypoints1, mypoints1_duplicate, list_ide);
            skAssemblyDrawing.CommitChanges();
            //drg_handler.CloseActiveDrawing(true);
            //skAssemblyDrawing.Delete();
            return skAssemblyDrawing;
        }

        #region private methods
        /// <summary>
        /// Modify current view's restriction box
        /// </summary>
        /// <param name="mypoints1"></param>
        /// <param name="currentView"></param>
        private void AdjustRestrictionBox(List<RequiredPartPoints> mypoints1, TSD.View currentView)
        {
            double change_min = Math.Abs(currentView.RestrictionBox.MinPoint.Y);
            double change_max = Math.Abs(currentView.RestrictionBox.MaxPoint.Y);
            if (mypoints1.Count > 0)
            {
                if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
                {
                    currentView.RestrictionBox.MaxPoint.Y = change_min;
                }
                else
                {
                    currentView.RestrictionBox.MinPoint.Y = -change_max;
                }
            }
            else
            {
                currentView.RestrictionBox.MaxPoint.Y = change_max + 65;
                currentView.RestrictionBox.MinPoint.Y = -change_min - 65;
            }
            currentView.Modify();
        }


        
        private void GetFlangeSections(string drg_attribute, TSM.Part main_part, AssemblyDrawing skAssemblyDrawing, List<List<int>> MYLIST, List<RequiredPartPoints> mypoints1, List<RequiredPartPoints> mypoints1_duplicate, List<int> list_ide)
        {
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
        }

        private void GetFrontViewPoints(string drg_attribute, TSM.Part main_part,
            double height, List<List<int>> MYLIST, TSD.View current_view, List<RequiredPartPoints> mypoints)
        {
            mypoints = mypoints.OrderBy(x => x.distanceX).ToList();

            SectionLocationWithParts obj1 = new SectionLocationWithParts();
            List<TSM.Part> list1 = new List<TSM.Part>();


            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    list1.Add(mypoints[i].part);
                    list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distanceX });

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distanceX) - Convert.ToInt16(mypoints[i].distanceX));
                    //if (ditsnace > 25)
                    if (ditsnace > 125)
                    {

                        list1.Add(mypoints[i].part);
                        list2.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distanceX });
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
                        TSD.View bottom_view = null;
                        bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0), Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100, new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out bottom_view, out sec);
                        bottom_view.Attributes.LoadAttributes("SK_BEAM_A1");
                        bottom_view.Modify();

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
                                    mypart_list_for_section.Add(mmpart);
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

        /// <summary>
        /// Fetch the front view parts for manipulation and set the userproperty "Centered"
        /// Do not include main part in the part list
        /// Do not include FALLTECH in the part list
        /// </summary>
        /// <param name="mainPart"></param>
        /// <param name="frontView"></param>
        /// <returns></returns>
        private ArrayList GetFrontViewParts(TSM.Part mainPart, TSD.View frontView)
        {
            ArrayList fvPartList = new ArrayList();
            TSD.DrawingObjectEnumerator partsEnum = frontView.GetAllObjects(typeof(TSD.Part));
            while (partsEnum.MoveNext())
            {
                TSD.Part currentDPart = partsEnum.Current as TSD.Part;

                TSM.Part currentMPart = (new TSM.Model().SelectModelObject(currentDPart.ModelIdentifier)) as TSM.Part;
                if (!currentMPart.Identifier.ID.Equals(mainPart.Identifier.ID))
                {
                    if (!currentMPart.Name.Contains("FALLTECH"))
                    {
                        TSD.PointList ptlist = _boundingBoxHandler.BoundingBoxSort(currentMPart, mainPart as TSM.Beam);
                        double distance = Convert.ToInt16(ptlist[0].Z) + Convert.ToInt16(ptlist[1].Z);
                        if (distance == 0)
                        {
                            currentMPart.SetUserProperty("USERDEFINED.NOTES7", "(CTRD)");
                        }
                        fvPartList.Add(currentMPart);
                    }
                }
            }
            return fvPartList;
        }

        /// <summary>
        /// Extract the radius dimensions from the Front View
        /// </summary>
        /// <param name="frontView"></param>
        /// <returns></returns>
        private List<RadiusDimension> ExtractRadiusDimension(TSD.View frontView)
        {
            List<RadiusDimension> radiusDimensions = new List<RadiusDimension>();
            Type[] dimTypes = new Type[] { typeof(TSD.RadiusDimension) };
            TSD.DrawingObjectEnumerator dimDrawings = frontView.GetAllObjects(dimTypes);
            while (dimDrawings.MoveNext())
            {
                var dim_del = dimDrawings.Current;

                if (dim_del.GetType().Equals(typeof(TSD.RadiusDimension)))
                {
                    TSD.RadiusDimension dimension = dim_del as TSD.RadiusDimension;
                    radiusDimensions.Add(dimension);
                }

            }
            return radiusDimensions;
        }


        /// <summary>
        /// Deletes all dimensions of views retaing the Straight Dimension of Front View which 
        /// is the overall assembly dimension Eg: W16X40 x 11'-65/16
        /// Clear knockOff dimensions based on user setting
        /// </summary>
        /// <param name="currentView"></param>
        /// <param name="currentAssembly"></param>
        private void DeleteViewDimensionByType(TSD.View currentView, TSM.Assembly currentAssembly)
        {
            TSD.PointList assemblyBoundingBox = _boundingBoxHandler.BoundingBoxForDimension(currentAssembly);

            TSG.Point startWorkPoint = assemblyBoundingBox[0]; //maxPoint
            TSG.Point endWorkPoint = assemblyBoundingBox[1]; //minPoint
            Type[] dimTypes = new Type[] { typeof(TSD.StraightDimension), typeof(TSD.StraightDimensionSet),
                typeof(TSD.AngleDimension) };
            List<DimensionWithDifference> fvDimWithDifference = new List<DimensionWithDifference>();
            List<DimensionWithDifference> fvDimWithDifferenceOri = new List<DimensionWithDifference>();
            TSD.DrawingObjectEnumerator dimDrawings = currentView.GetAllObjects(dimTypes);


            while (dimDrawings.MoveNext())
            {
                var currentDimDrawing = dimDrawings.Current;
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    if (currentDimDrawing.GetType().Equals(typeof(TSD.StraightDimension)))
                    {
                        TSD.StraightDimension stDimen = currentDimDrawing as TSD.StraightDimension;
                        //fetch only dimensions below the view
                        fvDimWithDifference.Add(new DimensionWithDifference
                        {
                            StDimen = stDimen,
                            Difference = (stDimen.EndPoint.X - stDimen.StartPoint.X),
                            MyVector = stDimen.UpDirection
                        });
                        //include all dimensions to delete
                        fvDimWithDifferenceOri.Add(new DimensionWithDifference
                        {
                            StDimen = stDimen,
                            Difference = (stDimen.EndPoint.X - stDimen.StartPoint.X),
                            MyVector = stDimen.UpDirection
                        });
                    }
                    else if (currentDimDrawing.GetType().Equals(typeof(TSD.StraightDimensionSet)))
                    {
                        //delete only elevation dimensions 
                        if ((currentDimDrawing as TSD.StraightDimensionSet).Attributes.DimensionType
                            == TSD.DimensionSetBaseAttributes.DimensionTypes.Elevation)
                        {
                            currentDimDrawing.Delete();
                        }
                    }
                    else if (currentDimDrawing.GetType().Equals(typeof(TSD.AngleDimension)))
                    {
                        //delete all in angle dimension
                        currentDimDrawing.Delete();
                    }
                    else
                    {
                        currentDimDrawing.Delete();
                    }
                }
                else if (currentView.ViewType.Equals(TSD.View.ViewTypes.TopView))
                {
                    //retain angle dimension and delete the reset
                    if (!currentDimDrawing.GetType().Equals(typeof(TSD.AngleDimension)))
                    {
                        currentDimDrawing.Delete();
                    }
                }
                else
                {
                    //delete all other view's dimensions
                    currentDimDrawing.Delete();
                }

            }

            fvDimWithDifference.RemoveAll(X => X.MyVector.Y > 0);
            //extract overall dimension and retain it
            DimensionWithDifference overAll = fvDimWithDifference.
                Where((X => (X.Difference.Equals(fvDimWithDifference.Max(Y => Y.Difference))))).ToList().FirstOrDefault();

            List<DimensionWithDifference> toDeleteFVDimensions = fvDimWithDifferenceOri.
                Where(X => !X.Difference.Equals(overAll.Difference)).ToList();

            //if knockoff dimension is needed, remove them from deletion list
            if (_userInput.KnockOffDimension)
            {
                toDeleteFVDimensions.RemoveAll(X => HasKnockOffDimension(X.StDimen) == true);
            }

            foreach (var dim in toDeleteFVDimensions)
            {
                dim.StDimen.Delete();
            }
        }



        /// <summary>
        /// Modify the view to defined scale and min length
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="minLength"></param>
        /// <param name="assemblyDrawing"></param>
        private void ModifyViewToScale(double scale, double minLength, AssemblyDrawing assemblyDrawing)
        {
            TSD.DrawingObjectEnumerator viewEnums = assemblyDrawing.GetSheet().GetAllViews();
            while (viewEnums.MoveNext())
            {
                TSD.View currentView = viewEnums.Current as TSD.View;
                currentView.Attributes.Scale = scale;

                currentView.Attributes.Shortening.MinimumLength = minLength;
                currentView.Attributes.Shortening.CutPartType = TSD.View.ShorteningCutPartType.X_Direction;

                currentView.Modify();
                assemblyDrawing.Modify();
            }
        }


        /// <summary>
        /// The Knock off dimension has "(" and check the text element based on the value
        /// </summary>
        /// <param name="stDimension"></param>
        /// <returns></returns>
        private bool HasKnockOffDimension(TSD.StraightDimension stDimension)
        {
            bool hasKnockOff = false;
            IEnumerator stDimenValues = stDimension.Value.GetEnumerator();

            while (stDimenValues.MoveNext())
            {
                if (stDimenValues.Current.GetType().Equals(typeof(TSD.TextElement)))
                {
                    TSD.TextElement ele = stDimenValues.Current as TSD.TextElement;
                    if (ele.Value.Contains("("))
                    {
                        hasKnockOff = true;
                        break;
                    }
                }
            }
            return hasKnockOff;
        }


        /// <summary>
        /// Compare 2 list of parts and check if they are the same based on
        /// Part Name, Orgin, Axis
        /// </summary>
        /// <param name="partList1"></param>
        /// <param name="partList2"></param>
        /// <param name="mainPart"></param>
        /// <returns></returns>
        private bool ComparePartMarkOrientation(List<TSM.Part> partList1, List<TSM.Part> partList2, TSM.Beam mainPart)
        {
            if (partList1.Count != partList2.Count)
            {
                return false;
            }
            TSM.Model currentModel = new TSM.Model();
            int samePartCount = 0;
            currentModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(mainPart.GetCoordinateSystem()));
            foreach (TSM.Part part1 in partList1)
            {
                string mark1 = SkTeklaDrawingUtility.get_report_properties(part1, "PART_POS");
                TSG.CoordinateSystem coord1 = part1.GetCoordinateSystem();
                foreach (TSM.Part part2 in partList2)
                {
                    string mark2 = SkTeklaDrawingUtility.get_report_properties(part2, "PART_POS");
                    if (mark1 == mark2)
                    {
                        TSG.CoordinateSystem coord2 = part2.GetCoordinateSystem();

                        if ((Convert.ToInt64(coord1.Origin.Y) == Convert.ToInt64(coord2.Origin.Y))
                            && (Convert.ToInt64(coord1.Origin.Z) == Convert.ToInt64(coord2.Origin.Z)))
                        {
                            if ((VectorCheck(coord1.AxisX, coord2.AxisX) && (VectorCheck(coord1.AxisY, coord2.AxisY))))
                            {
                                samePartCount++;
                                break;
                            }

                        }

                    }


                }
            }
            currentModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            return (samePartCount == partList1.Count);

        }

        /// <summary>
        /// Compare if both the vectors are the same
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        private bool VectorCheck(TSG.Vector vector1, TSG.Vector vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            return (Convert.ToInt64(vector1.X) == Convert.ToInt64(vector2.X))
                && (Convert.ToInt64(vector1.Y) == Convert.ToInt64(vector2.Y)) &&
                (Convert.ToInt64(vector1.Z) == Convert.ToInt64(vector2.Z));
        }

        private void CompareSectionViews(List<RequiredPartPoints> mypoints, TSD.View current_view, TSM.Part main_part,
            List<List<int>> mypart_list_of_created_sect_view, TSD.AssemblyDrawing ASSEMBLY_DRAWING,
             string drg_att)
        {
            List<double> mainpart_values = _catalogHandler.Getcatalog_values(main_part);

            list_for_flange_section = new List<SectionLocationWithParts>();
            mypoints = mypoints.OrderBy(x => x.distanceX).ToList();



            List<RequiredPartPoints> final_distance = new List<RequiredPartPoints>();

            for (int i = 0; i < mypoints.Count; i++)
            {

                if (i == Convert.ToInt16(mypoints.Count - 1))
                {
                    final_distance.Add(mypoints[i]);

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distanceX) - Convert.ToInt16(mypoints[i].distanceX));
                    if (ditsnace > 25)
                    {
                        final_distance.Add(mypoints[i]);

                    }

                    else
                    {
                        if (mypoints[i].distanceX != mypoints[i + 1].distanceX)
                        {
                            if (mypoints[i].distanceY > mypoints[i + 1].distanceY)
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
                    final_distance_UNIQUE.Add(mypoints[i].distanceX);
                    list1.Add(mypoints[i].part);
                    list_for_flange_section.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distanceX });

                }
                else
                {
                    double ditsnace = (Convert.ToInt16(mypoints[i + 1].distanceX) - Convert.ToInt16(mypoints[i].distanceX));
                    if (ditsnace > 25)
                    {

                        list1.Add(mypoints[i].part);
                        list_for_flange_section.Add(new SectionLocationWithParts() { PartList = list1, Distance = mypoints[i].distanceX });
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


            List<SectionLocationWithParts> FINAL = list_for_flange_section.GroupBy(X => X.PartList.Count).Select(Y => Y.FirstOrDefault()).ToList();
            List<string> final_check_for_unique = new List<string>();
            for (int i = list_for_flange_section.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    list_for_flange_section[i].SectionViewNeeded = "YES";
                }
                else
                {

                    List<string> check_for_unique = new List<string>();
                    for (int j = i - 1; j >= 0; j--)
                    {


                        var first_loop = list_for_flange_section[i].PartList;
                        var second_loop = list_for_flange_section[j].PartList;

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
                        list_for_flange_section[i].SectionViewNeeded = "YES";
                    }
                    else
                    {
                        list_for_flange_section[i].SectionViewNeeded = "NO";
                        int check = check_for_unique.LastIndexOf("SAME");
                        int check2 = check_for_unique.Count - (check + 1);
                        list_for_flange_section[i].IndexOfSameSection = check2;
                    }

                }
            }





            list_for_flange_section = list_for_flange_section.OrderBy(x => x.Distance).ToList();
            List<SectionLocationWithParts> section = new List<SectionLocationWithParts>();
            List<TSD.SectionMark> sectionmarklist = new List<TSD.SectionMark>();








            for (int i = 0; i < list_for_flange_section.Count; i++)
            {
                if ((list_for_flange_section[i].SectionViewNeeded == "YES"))
                {
                    //if (list2[i].partlist.Count > 1)
                    //{
                    //    List<TSM.Beam> list_of_angles = new List<TSM.Beam>();


                    //}
                    double minx = 0;
                    double maxx = 0;
                    double mny = 0;
                    double mxy = 0;

                    minx = list_for_flange_section[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].X);
                    maxx = list_for_flange_section[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].X);
                    mny = list_for_flange_section[i].PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].Y);
                    mxy = list_for_flange_section[i].PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].Y);

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


                        list_for_flange_section[i].MyView = bottom_view;
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
                    TSD.SectionMark mysec = sectionmarklist[list_for_flange_section[i].IndexOfSameSection];

                    mysec.LeftPoint.X = list_for_flange_section[i].Distance;
                    mysec.RightPoint.X = list_for_flange_section[i].Distance;
                    mysec.Insert();

                }






            }

        }

        /// <summary>
        /// Converts the model points to view points
        /// </summary>
        /// <param name="modelPoints"></param>
        /// <param name="mainPart"></param>
        /// <param name="currentView"></param>
        /// <returns></returns>
        private TSD.PointList ConvertToViewPoints(TSD.PointList modelPoints,TSM.Beam mainPart, TSD.View currentView)
        {
            TSD.PointList pmList = new TSD.PointList();

            TSG.Matrix toPartMatrix = TSG.MatrixFactory.FromCoordinateSystem(mainPart.GetCoordinateSystem());

            foreach (TSG.Point pt in modelPoints)
            {
                TSG.Point mtpt = toPartMatrix.Transform(pt);
                pmList.Add(mtpt);
            }

            TSG.Matrix viewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.DisplayCoordinateSystem);

            TSD.PointList vpList = new TSD.PointList();
            foreach (TSG.Point pt in pmList)
            {
                TSG.Point mtpt = viewMatrix.Transform(pt);
                vpList.Add(mtpt);
            }

            return vpList;
            ////global to view's coordinate system
            //TSG.Matrix viewMatrix = TSG.MatrixFactory.ToCoordinateSystem(currentView.DisplayCoordinateSystem);


            //TSD.PointList viewPoints = new TSD.PointList();
            //foreach (TSG.Point pt in modelPoints)
            //{
            //    viewPoints.Add(viewMatrix.Transform(pt));
            //}

            //return viewPoints;
        }


        #endregion

    }
}
