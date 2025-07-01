using SK.Tekla.Drawing.Automation.Models;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKDrawingHandler
    {
        // Constants for magic numbers
        private const double MinDistanceForSection = 125;
        private const double MinDistanceForFlangeSection = 25;
        private const double MaxDistanceForDepth = 300;
        private const double ViewExtension = 100;
        private const double RestrictionBoxOffset = 65;
        private const double FontHeight = 3.96875;

        private readonly CustomInputModel _userInput;
        private readonly SKBoundingBoxHandler _boundingBoxHandler;

        public List<SectionLocationWithParts> SectionLocationList { get; set; } = new List<SectionLocationWithParts> { };

        public List<SectionLocationWithParts> FlangeSectionLocationList { get; set; } = new List<SectionLocationWithParts> { };

        public List<TSM.Part> BottomPartsForMarkRetainList { get; set; } = new List<TSM.Part>();


        public List<TSD.RadiusDimension> RadiusDimensionList { get; set; } = new List<RadiusDimension> { };


        public SKDrawingHandler(SKBoundingBoxHandler boundingBoxHandler,  CustomInputModel userInput)
        {
            _boundingBoxHandler = boundingBoxHandler ?? throw new ArgumentNullException(nameof(boundingBoxHandler));
            _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
        }


        #region public methods
        public TSD.AssemblyDrawing ResetDrawingDimensionsExceptAssemblyDimensions(TSM.Model currentModel,
            TSM.ModelObject currentBeam, string drgAttribute,
            TSM.Part mainPart, double output, TSM.Assembly currentAssembly, double scale,
            double minLength)
        {
            if (!(mainPart is TSM.Beam mainBeam))
                throw new ArgumentException("Main part must be a beam.", nameof(mainPart));


            //reset the values for each drawing!!!!

            ResetLists();

            AssemblyDrawing skAssemblyDrawing = CreateAssemblyDrawing(drgAttribute, currentAssembly);

            ModifyViewToScale(scale, minLength, skAssemblyDrawing);

            var drawingViews = skAssemblyDrawing.GetSheet().GetAllViews();

            double mainPartHeight = GetPartHeight(mainPart);
            var heightPosNeg = mainPartHeight / 2;

            //////////////////Enumerating different views/////////////////////////////
            while (drawingViews.MoveNext())
            {
                TSD.View currentView = drawingViews.Current as TSD.View;
                DeleteViewDimensionByType(currentView, currentAssembly);
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    RadiusDimensionList = ExtractRadiusDimension(currentView);
                }
            }
            drawingViews.Reset();

            //transform the plane to Global to get model parts
            currentModel.GetWorkPlaneHandler().SetCurrentTransformationPlane
                (new TSM.TransformationPlane(mainPart.GetCoordinateSystem()));
            ArrayList fvMPartList = new ArrayList();

            TSD.DrawingObjectEnumerator enumForParts = skAssemblyDrawing.GetSheet().GetAllViews();
            while (enumForParts.MoveNext())
            {
                TSD.View current_view = enumForParts.Current as TSD.View;
                if (current_view.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    fvMPartList = GetFrontViewParts(mainPart, current_view);
                }
            }


            currentModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());


            TSD.DrawingObjectEnumerator placeViewEnum = skAssemblyDrawing.GetSheet().GetAllViews();
            //List<List<int>> MYLIST = new List<List<int>>();
            List<RequiredPartPoints> partsOutsideHeight = new List<RequiredPartPoints>();
            List<RequiredPartPoints> partsOutsideHeightDuplicate = new List<RequiredPartPoints>();
            List<RequiredPartPoints> partsForDimension = new List<RequiredPartPoints>();
            List<int> partIdsOutsideHeight = new List<int>();

            while (placeViewEnum.MoveNext())
            {
                TSD.View currentView = placeViewEnum.Current as TSD.View;
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    List<RequiredPartPoints> partsInHeight = new List<RequiredPartPoints>();

                    foreach (TSM.Part mPart in fvMPartList)
                    {
                        TSD.PointList pmPts = _boundingBoxHandler.BoundingBoxSort(mPart, mainPart as TSM.Beam);
                        Console.WriteLine($"0th record: x: {pmPts[0].X}  y: {pmPts[0].Y}  z: {pmPts[0].Z}");
                        Console.WriteLine($"1st record: x: {pmPts[1].X}  y: {pmPts[1].Y}  z: {pmPts[1].Z}");
                        TSM.Part mypart = mPart;

                        TSD.PointList pvPts = ConvertToViewPoints(pmPts, mainPart as TSM.Beam, currentView);
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

                        partsForDimension.Add(requiredPart);
                        //((pmPts[0].Y > -heightPosNeg) && (pmPts[1].Y < heightPosNeg)) || ((pmPts[0].Y < -heightPosNeg) && (pmPts[1].Y > heightPosNeg))
                        if (IsPartWithinHeight(pmPts, heightPosNeg))
                        {
                            partsInHeight.Add(requiredPart);
                        }
                        else
                        {
                            partIdsOutsideHeight.Add(mypart.Identifier.ID);
                            partsOutsideHeight.Add(requiredPart);
                            partsOutsideHeightDuplicate.Add(requiredPart);

                            if ((Convert.ToInt64(pmPts[0].Y) <= -Convert.ToInt64(heightPosNeg)) && (Convert.ToInt64(pmPts[1].Y) <= -Convert.ToInt64(heightPosNeg)))
                            {
                                BottomPartsForMarkRetainList.Add(mPart);
                            }
                        }
                    }
                    Console.WriteLine($"partsInHeight: {partsInHeight.Count}");
                    partsInHeight.ForEach(Console.WriteLine);

                    Console.WriteLine($"(partsOutsideHeight) mypoints1: {partsOutsideHeight.Count}");
                    partsOutsideHeight.ForEach(Console.WriteLine);

                    AdjustRestrictionBox(partsOutsideHeight, currentView);

                    List<List<int>> MYLIST = PrepareSectionsInHeight(drgAttribute, mainPart, mainPartHeight, currentView, partsInHeight);
                    PrepareFlangeSectionsOutsideHeight(drgAttribute, mainPart, skAssemblyDrawing, MYLIST, partsOutsideHeight,
                            partsOutsideHeightDuplicate, partIdsOutsideHeight);
                }
                skAssemblyDrawing.PlaceViews();
            }


            skAssemblyDrawing.CommitChanges();
            //drg_handler.CloseActiveDrawing(true);
            //skAssemblyDrawing.Delete();
            return skAssemblyDrawing;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Check whether the points are within the height based of the main part
        /// </summary>
        /// <param name="points"></param>
        /// <param name="heightPosNeg"></param>
        /// <returns></returns>
        private bool IsPartWithinHeight(TSD.PointList points, double heightPosNeg)
        {
            //TODO: Check with Viswa
            //((pmPts[0].Y > -heightPosNeg) && (pmPts[1].Y < heightPosNeg)) || ((pmPts[0].Y < -heightPosNeg) && (pmPts[1].Y > heightPosNeg))
            double minY = points[0].Y;
            double maxY = points[1].Y;
            return (minY > -heightPosNeg && maxY < heightPosNeg) || (minY < -heightPosNeg && maxY > heightPosNeg);
        }

        /// <summary>
        /// Get the height of the given part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private double GetPartHeight(TSM.Part part)
        {
            double height = 0;
            part.GetReportProperty("HEIGHT", ref height);
            return height;
        }

        /// <summary>
        /// Create the assembly drawing
        /// </summary>
        /// <param name="drgAttribute"></param>
        /// <param name="currentAssembly"></param>
        /// <returns></returns>
        private AssemblyDrawing CreateAssemblyDrawing(string drgAttribute, Assembly currentAssembly)
        {
            TSD.DrawingHandler drgHandler = new TSD.DrawingHandler();
            TSD.AssemblyDrawing skAssemblyDrawing = new TSD.AssemblyDrawing(currentAssembly.Identifier, drgAttribute);
            skAssemblyDrawing.Insert();
            drgHandler.SetActiveDrawing(skAssemblyDrawing, true);
            return skAssemblyDrawing;
        }

        /// <summary>
        /// Reset the list for each assembly
        /// </summary>
        private void ResetLists()
        {
            SectionLocationList = new List<SectionLocationWithParts>();
            FlangeSectionLocationList = new List<SectionLocationWithParts>();
            BottomPartsForMarkRetainList = new List<TSM.Part>();
            RadiusDimensionList = new List<TSD.RadiusDimension>();
        }

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


        /// <summary>
        /// Get the Flange Sections
        /// </summary>
        /// <param name="drgAttribute"></param>
        /// <param name="mainPart"></param>
        /// <param name="skAssemblyDrawing"></param>
        /// <param name="sectionPartIdList"></param>
        /// <param name="partsOutsideHeight"></param>
        /// <param name="partsOutsideHeightDuplicate"></param>
        /// <param name="partIdsOutsideHeight"></param>
        private void PrepareFlangeSectionsOutsideHeight(string drgAttribute, TSM.Part mainPart, AssemblyDrawing skAssemblyDrawing,
            List<List<int>> sectionPartIdList, List<RequiredPartPoints> partsOutsideHeight,
            List<RequiredPartPoints> partsOutsideHeightDuplicate, List<int> partIdsOutsideHeight)
        {
            TSD.DrawingObjectEnumerator viewsEnum = skAssemblyDrawing.GetSheet().GetAllViews();
            while (viewsEnum.MoveNext())
            {
                TSD.View currentView = viewsEnum.Current as TSD.View;
                if (currentView.ViewType.Equals(TSD.View.ViewTypes.FrontView))
                {
                    List<List<int>> uniqueIdList = new List<List<int>>();
                    foreach (List<int> idList in sectionPartIdList)
                    {

                        List<int> dupList = idList.Intersect(partIdsOutsideHeight).ToList();
                        uniqueIdList.Add(dupList);
                    }


                    foreach (List<int> sectionIdList in uniqueIdList)
                    {
                        for (int p = 0; p < partsOutsideHeight.Count; p++)
                        {
                            bool result = sectionIdList.Any(x => x.Equals(partsOutsideHeight[p].part.Identifier.ID));
                            if (result)
                            {
                                partsOutsideHeightDuplicate.RemoveAll(x => x.part.Identifier.ID.Equals(partsOutsideHeight[p].part.Identifier.ID));
                            }
                        }

                    }
                    if (partsOutsideHeightDuplicate.Count > 0)
                    {
                        CompareFlangeSectionViews(partsOutsideHeightDuplicate, currentView, mainPart, skAssemblyDrawing, drgAttribute);
                    }

                    else
                    {
                        FlangeSectionLocationList = new List<SectionLocationWithParts>();
                    }
                }
            }
        }

        /// <summary>
        /// Prepare sections that are within the view height
        /// </summary>
        /// <param name="drgAttribute"></param>
        /// <param name="mainPart"></param>
        /// <param name="height"></param>
        /// <param name="curentView"></param>
        /// <param name="partsInHeight"></param>
        /// <returns></returns>
        private List<List<int>> PrepareSectionsInHeight(string drgAttribute, TSM.Part mainPart,
            double height, TSD.View curentView, List<RequiredPartPoints> partsInHeight)
        {
            List<List<int>> sectionPartIds = new List<List<int>>();
            partsInHeight = partsInHeight.OrderBy(x => x.distanceX).ToList();

            List<TSM.Part> currentParts = new List<TSM.Part>();
 
            for (int i = 0; i < partsInHeight.Count; i++)
            {
                currentParts.Add(partsInHeight[i].part);
                if (i == partsInHeight.Count - 1 || (partsInHeight[i + 1].distanceX - partsInHeight[i].distanceX) > MinDistanceForSection)
                {
                    SectionLocationList.Add(new SectionLocationWithParts
                    {
                        PartList = new List<TSM.Part>(currentParts),
                        Distance = partsInHeight[i].distanceX
                    });
                    currentParts.Clear();
                }
            }

            MarkUniqueSections(SectionLocationList, mainPart);

            SectionLocationList = SectionLocationList.OrderBy(x => x.Distance).ToList();

            List<TSD.SectionMark> sectionMarkList = new List<TSD.SectionMark>();

            for (int i = 0; i < SectionLocationList.Count; i++)
            {
                if ((SectionLocationList[i].SectionViewNeeded == "YES"))
                {
                    TSD.SectionMark secMark = null;
                    TSD.View sectionView = null;
                    try
                    {
                        CreateSectionView(curentView, SectionLocationList[i], out secMark, out sectionView);
                        ConfigureSectionView( secMark, sectionView, curentView);

                        SectionLocationList[i].MyView = sectionView;
                        sectionMarkList.Add(secMark);
                        TSD.DrawingObjectEnumerator partEnums = sectionView.GetAllObjects(typeof(TSD.Part));
                        List<int> partIdList = new List<int>();
                        List<TSM.Part> sectionParts = new List<TSM.Part>();
                        while (partEnums.MoveNext())
                        {
                            TSD.Part drgPart = partEnums.Current as TSD.Part;
                            TSM.ModelObject modelObj = new TSM.Model().SelectModelObject(drgPart.ModelIdentifier);
                            TSD.PointList bounding_box_z = _boundingBoxHandler.BoundingBoxSort(modelObj, sectionView, SKSortingHandler.SortBy.Z);

                            if ((Convert.ToInt64(bounding_box_z[1].Z) >= Convert.ToInt64(sectionView.RestrictionBox.MinPoint.Z)) && (Convert.ToInt64(bounding_box_z[0].Z) <= Convert.ToInt64(sectionView.RestrictionBox.MaxPoint.Z)))
                            {
                                TSM.Part mPart = modelObj as TSM.Part;
                                if (!mPart.Identifier.ID.Equals(mainPart.Identifier.ID))
                                {
                                    partIdList.Add(mPart.Identifier.ID);
                                    sectionParts.Add(mPart);
                                }

                            }


                        }
                        SectionLocationList[i].RequiredPartList = sectionParts;
                        sectionPartIds.Add(partIdList);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating section view: {ex.Message}");
                    }

                }

                else
                {

                    InsertDummySectionMark(sectionMarkList, SectionLocationList[i]);


                }






            }
            return sectionPartIds;
        }


        /// <summary>
        /// Prepare the drawing marks for the section view
        /// </summary>
        /// <param name="secMark"></param>
        /// <param name="sectionView"></param>
        /// <param name="currentView"></param>
        private void ConfigureSectionView(SectionMark secMark, TSD.View sectionView, TSD.View currentView)
        {
            TSD.FontAttributes FONT = new TSD.FontAttributes();
            FONT.Color = TSD.DrawingColors.Magenta;
            FONT.Height = Convert.ToInt16(3.96875);
            TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
            TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
            X.Font.Color = TSD.DrawingColors.Magenta;
            X.Font.Height = Convert.ToInt64(3.96875);

            TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);


            TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };

            secMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            secMark.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

            sectionView.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

            sectionView.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

            sectionView.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
            secMark.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
            secMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            sectionView.Attributes.Scale.Equals(currentView.Attributes.Scale);

            sectionView.Modify();
        }

        private void MarkUniqueSections(List<SectionLocationWithParts> sections, TSM.Part mainPart)
        {
            for (int i = sections.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    sections[i].SectionViewNeeded = "YES";
                    continue;
                }
                bool isUnique = true;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (sections[i].PartList.Count == sections[j].PartList.Count &&
                        ComparePartMarkOrientation(sections[i].PartList, sections[j].PartList, mainPart as TSM.Beam))
                    {
                        isUnique = false;
                        sections[i].SectionViewNeeded = "NO";
                        sections[i].IndexOfSameSection = i - j - 1;
                        break;
                    }
                }
                if (isUnique) sections[i].SectionViewNeeded = "YES";

                //List<string> check_for_unique = new List<string>();
                //for (int j = i - 1; j >= 0; j--)
                //{


                //    var first_loop = sections[i].PartList;
                //    var second_loop = sections[j].PartList;

                //    if (!(first_loop.Count == second_loop.Count))
                //    {
                //        check_for_unique.Add("UNIQUE");

                //    }
                //    else
                //    {
                //        bool result = ComparePartMarkOrientation(first_loop, second_loop, mainPart as TSM.Beam);

                //        if (result == true)
                //        {

                //            check_for_unique.Add("SAME");
                //        }
                //        else
                //        {
                //            check_for_unique.Add("UNIQUE");
                //        }

                //    }
                //}

                //if (!check_for_unique.Contains("SAME"))
                //{
                //    sections[i].SectionViewNeeded = "YES";
                //}
                //else
                //{
                //    sections[i].SectionViewNeeded = "NO";
                //    int check = check_for_unique.LastIndexOf("SAME");
                //    int check2 = check_for_unique.Count - (check + 1);
                //    sections[i].IndexOfSameSection = check2;
                //}

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


        /// <summary>
        /// Compare flange section views (SEC_VIEW_COMPARE)
        /// </summary>
        /// <param name="partsOutsideHeight"></param>
        /// <param name="currentView"></param>
        /// <param name="mainPart"></param>
        /// <param name="assemblyDrawing"></param>
        /// <param name="drgAttribute"></param>
        private void CompareFlangeSectionViews(List<RequiredPartPoints> partsOutsideHeight, TSD.View currentView, TSM.Part mainPart,
            TSD.AssemblyDrawing assemblyDrawing,
             string drgAttribute)
        {
            //List<double> mainpart_values = _catalogHandler.Getcatalog_values(main_part);
            double height = GetPartHeight(mainPart);
            FlangeSectionLocationList = new List<SectionLocationWithParts>();
            partsOutsideHeight = partsOutsideHeight.OrderBy(x => x.distanceX).ToList();

            List<TSM.Part> currentParts = new List<TSM.Part>();

            for (int i = 0; i < partsOutsideHeight.Count; i++)
            {
                currentParts.Add(partsOutsideHeight[i].part);
                if (i == partsOutsideHeight.Count - 1 || (partsOutsideHeight[i + 1].distanceX - partsOutsideHeight[i].distanceX) > MinDistanceForFlangeSection)
                {
                    FlangeSectionLocationList.Add(new SectionLocationWithParts
                    {
                        PartList = new List<TSM.Part>(currentParts),
                        Distance = partsOutsideHeight[i].distanceX
                    });
                    currentParts.Clear();
                }
            }


            MarkUniqueSections(FlangeSectionLocationList, mainPart);


            FlangeSectionLocationList = FlangeSectionLocationList.OrderBy(x => x.Distance).ToList();
           
            List<TSD.SectionMark> sectionMarkList = new List<TSD.SectionMark>();

            for (int i = 0; i < FlangeSectionLocationList.Count; i++)
            {
                if ((FlangeSectionLocationList[i].SectionViewNeeded == "YES"))
                {
                    TSD.View sectionView = null;
                    TSD.SectionMark sectionMark = null;

                    try
                    {
                        CreateFlangeSectionView(currentView, FlangeSectionLocationList[i], out sectionView, out sectionMark);


                        ConfigureFlangeSectionView(sectionView, sectionMark, currentView);

                        FlangeSectionLocationList[i].MyView = sectionView;
                        sectionMarkList.Add(sectionMark);


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating section view: {ex.Message}");
                    }

                }

                else
                {
                    InsertDummySectionMark(sectionMarkList, FlangeSectionLocationList[i]);

                }

            }

        }

        private void CreateSectionView(TSD.View current_view, SectionLocationWithParts sectionLocation, out SectionMark secMark, out TSD.View sectionView)
        {
            double minx = sectionLocation.PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].X);
            double maxx = sectionLocation.PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].X);

            double miny = Convert.ToInt64(sectionLocation.PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[0].Y));
            double maxy = Convert.ToInt64(sectionLocation.PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, current_view)[1].Y));

            double distanceofx = ((minx + maxx) / 2);
            double distance_of_y = ((miny + maxy) / 2);

            double height_for_view = current_view.RestrictionBox.MaxPoint.Y;
            double height_for_view1 = current_view.RestrictionBox.MinPoint.Y;
            TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
            TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);
            double dep_up = maxx - distanceofx;
            double dep_down = distanceofx - minx;
            if (dep_up > 100)
            {
                dep_up = 5;
            }
            if (dep_down > 100)
            {
                dep_down = 5;
            }


            bool result = TSD.View.CreateSectionView(current_view, P2, P1, new TSG.Point(current_view.ExtremaCenter.X, 0, 0),
                Convert.ToInt64(dep_up) + 100, Convert.ToInt64(dep_down) + 100,
                new TSD.View.ViewAttributes("SK_BEAM_A1"), new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"), out sectionView, out secMark);
            sectionView.Attributes.LoadAttributes("SK_BEAM_A1");
            sectionView.Modify();

            double change_min = Math.Abs(sectionView.RestrictionBox.MinPoint.X);
            double change_max = Math.Abs(sectionView.RestrictionBox.MaxPoint.X);
            if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
            {
                sectionView.RestrictionBox.MaxPoint.X = change_min;
                sectionView.Modify();

            }
            else
            {
                sectionView.RestrictionBox.MinPoint.X = -change_max;
                sectionView.Modify();

            }


            sectionView.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
            sectionView.Modify();
        }


        private void CreateFlangeSectionView(TSD.View currentView, SectionLocationWithParts sectionLocation, out TSD.View sectionView, out SectionMark sectionMark)
        {
            double minx = sectionLocation.PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, currentView)[0].X);
            double maxx = sectionLocation.PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, currentView)[1].X);
            double miny = Convert.ToInt64(sectionLocation.PartList.Min(x => _boundingBoxHandler.BoundingBoxSort(x, currentView)[0].Y));
            double maxy = Convert.ToInt64(sectionLocation.PartList.Max(x => _boundingBoxHandler.BoundingBoxSort(x, currentView)[1].Y));

            double distanceofx = ((minx + maxx) / 2);
            double DISTANCE_TO_COMPARE = Math.Abs((minx - maxx));


            double distance_of_y = ((miny + maxy) / 2);




            double height_for_view = currentView.RestrictionBox.MaxPoint.Y;
            double height_for_view1 = currentView.RestrictionBox.MinPoint.Y;
            TSG.Point P1 = new TSG.Point(distanceofx, height_for_view, 0);
            TSG.Point P2 = new TSG.Point(distanceofx, height_for_view1, 0);


            double dep_up = maxx - distanceofx;
            double dep_down = distanceofx - minx;

            bool result = TSD.View.CreateSectionView(currentView, P2, P1, new TSG.Point(currentView.ExtremaCenter.X, 0, 0),
                Convert.ToInt64(dep_down) + 100, Convert.ToInt64(dep_up) + 100,
                currentView.Attributes, new TSD.SectionMarkBase.SectionMarkAttributes("SK_BEAM_A1"),
                out sectionView, out sectionMark);
            sectionView.Attributes.LoadAttributes("SK_BEAM_A1");
            sectionView.Modify();

            sectionMark.Attributes.LineLengthOffset = 0;
            sectionMark.Modify();

            double change_min = Math.Abs(sectionView.RestrictionBox.MinPoint.X);
            double change_max = Math.Abs(sectionView.RestrictionBox.MaxPoint.X);


            if (Convert.ToInt64(change_min) > Convert.ToInt64(change_max))
            {
                sectionView.RestrictionBox.MaxPoint.X = change_min;
                sectionView.Modify();

            }
            else
            {
                sectionView.RestrictionBox.MinPoint.X = -change_max;
                sectionView.Modify();

            }


            sectionView.Attributes.LabelPositionHorizontal = TSD.View.HorizontalLabelPosition.CenteredByViewRestrictionBox;
            sectionView.Modify();
        }

        private void InsertDummySectionMark(List<SectionMark> sectionMarkList, SectionLocationWithParts sectionLocation)
        {
            TSD.SectionMark sec_dummy = null;
            sectionMarkList.Add(sec_dummy);
            TSD.SectionMark secMark = sectionMarkList[sectionLocation.IndexOfSameSection];

            secMark.LeftPoint.X = sectionLocation.Distance;
            secMark.RightPoint.X = sectionLocation.Distance;
            secMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            secMark.Attributes.SymbolColor = TSD.DrawingColors.Magenta;

            secMark.Insert();
        }

        /// <summary>
        /// Converts the model points to view points
        /// </summary>
        /// <param name="modelPoints"></param>
        /// <param name="mainPart"></param>
        /// <param name="currentView"></param>
        /// <returns></returns>
        private TSD.PointList ConvertToViewPoints(TSD.PointList modelPoints, TSM.Beam mainPart, TSD.View currentView)
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
        }

        private void ConfigureFlangeSectionView(TSD.View sectionView, TSD.SectionMark sectionMark, TSD.View currentView)
        {
            TSD.FontAttributes FONT = new TSD.FontAttributes();
            FONT.Color = TSD.DrawingColors.Magenta;
            FONT.Height = Convert.ToInt16(3.96875);

            TSD.TextElement textelement3 = new TSD.TextElement("-", FONT);

            TSD.PropertyElement.PropertyElementType VIEW_LABEL = TSD.PropertyElement.PropertyElementType.ViewLabelMarkPropertyElementTypes.ViewName();
            TSD.PropertyElement X = new TSD.PropertyElement(VIEW_LABEL);
            X.Font.Color = TSD.DrawingColors.Magenta;
            X.Font.Height = Convert.ToInt64(3.96875);

            TSD.ContainerElement sectionmark = new TSD.ContainerElement { X, textelement3, X };



            sectionMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            sectionMark.Attributes.TagsAttributes.TagA1 = new TSD.SectionMarkBase.SectionMarkTagAttributes(TSD.SectionMarkBase.SectionMarkTagAttributes.TagShowOnSide.ShowOnBothSides, TSD.TagLocation.AboveLine, new TSG.Vector(1, 0, 0), TSD.SectionMarkBase.SectionMarkTagAttributes.TagTextRotation.AlwaysHorizontal, new TSD.ContainerElement { X });

            sectionView.Attributes.TagsAttributes.TagA1 = new TSD.View.ViewMarkTagAttributes(new TSG.Vector(0, 0, 0), TSD.TagLocation.AboveLine, TSD.TextAlignment.Center, sectionmark);

            sectionView.Attributes.LabelPositionVertical = TSD.View.VerticalLabelPosition.Bottom;

            sectionView.Attributes.MarkSymbolColor = TSD.DrawingColors.Magenta;
            sectionMark.Attributes.SymbolColor = TSD.DrawingColors.Magenta;
            sectionMark.Attributes.LineColor = TSD.DrawingColors.Magenta;
            sectionView.Attributes.Scale.Equals(currentView.Attributes.Scale);

            sectionView.Modify();
        }

        #endregion

    }
}
