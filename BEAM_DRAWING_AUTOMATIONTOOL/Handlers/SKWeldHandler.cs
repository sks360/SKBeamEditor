using System;
using System.Collections.Generic;
using System.Linq;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Drawing;
using Tekla.Structures;
using SK.Tekla.Drawing.Automation.Support;
using SK.Tekla.Drawing.Automation.Utils;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.ObjectPropertiesLibrary;
using System.Collections;

namespace SK.Tekla.Drawing.Automation.Handlers
{
    public class SKWeldHandler
    {
        private readonly string _client;

        public SKWeldHandler(string client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void WeldMerge(TSD.AssemblyDrawing beamAssemblyDrawing, TSM.Part mainPart, TSD.DrawingHandler drawingHandler)
        {
            Type weldMarkType = typeof(TSD.WeldMark);
            TSD.DrawingObjectEnumerator viewEnumerator = beamAssemblyDrawing.GetSheet().GetAllViews();

            while (viewEnumerator.MoveNext())
            {
                List<WeldMergeItem> weldItems = new List<WeldMergeItem>();
                TSD.View currentView = viewEnumerator.Current as TSD.View;

                if (currentView != null && (currentView.ViewType == TSD.View.ViewTypes.SectionView || currentView.ViewType == TSD.View.ViewTypes.FrontView))
                {
                    TSD.DrawingObjectEnumerator weldMarkEnumerator = currentView.GetAllObjects(weldMarkType);
                    while (weldMarkEnumerator.MoveNext())
                    {
                        TSD.WeldMark weldMark = weldMarkEnumerator.Current as TSD.WeldMark;
                        if (weldMark != null)
                        {
                            TSM.BaseWeld weld = new TSM.Model().SelectModelObject(weldMark.ModelIdentifier) as TSM.BaseWeld;
                            if (weld != null)
                            {
                                TSM.Part weldMainPart = weld.MainObject as TSM.Part;
                                TSM.Part weldSecondaryPart = weld.SecondaryObject as TSM.Part;
                                Guid mainPartGuid = mainPart.Identifier.GUID;

                                if (weldMainPart != null && !weldMainPart.Identifier.GUID.Equals(mainPartGuid))
                                {
                                    weldItems.Add(new WeldMergeItem { WeldMark = weldMark, PartGuid = weldMainPart.Identifier.GUID });
                                }
                                else if (weldSecondaryPart != null && !weldSecondaryPart.Identifier.GUID.Equals(mainPartGuid))
                                {
                                    weldItems.Add(new WeldMergeItem { WeldMark = weldMark, PartGuid = weldSecondaryPart.Identifier.GUID });
                                }
                            }
                        }
                    }

                    var groupedWeldItems = weldItems.GroupBy(x => x.PartGuid);
                    foreach (var weldGroup in groupedWeldItems)
                    {
                        List<TSD.WeldMark> mergeWeldMarks = weldGroup.Select(x => x.WeldMark).ToList();
                        drawingHandler.GetDrawingObjectSelector().SelectObjects(new ArrayList(mergeWeldMarks), true);
                        Operation.RunMacro(@"..\drawings\acmdMergeSelectedMarks.cs");
                        drawingHandler.GetDrawingObjectSelector().UnselectAllObjects();
                    }
                    beamAssemblyDrawing.CommitChanges();
                }
            }

            if (_client == "SME")
            {
                viewEnumerator.Reset();
                while (viewEnumerator.MoveNext())
                {
                    TSD.View currentView = viewEnumerator.Current as TSD.View;
                    if (currentView != null)
                    {
                        TSD.DrawingObjectEnumerator weldMarkEnumerator = currentView.GetAllObjects(weldMarkType);
                        while (weldMarkEnumerator.MoveNext())
                        {
                            TSD.WeldMark weldMark = weldMarkEnumerator.Current as TSD.WeldMark;
                            if (weldMark != null)
                            {
                                weldMark.Attributes.Font.Color = TSD.DrawingColors.Gray70;
                                weldMark.Modify();
                                currentView.Modify();
                            }
                        }
                    }
                }
            }
        }

        public void WeldDelete(TSD.View currentViewFrontViewReq, List<section_loc_with_parts> list, TSD.Drawing beamDrawing)
        {
            // Use HashSet for O(1) lookup performance
            HashSet<Identifier> weldsToKeep = new HashSet<Identifier>();

            // Iterate over sections using foreach for clarity
            foreach (var section in list)
            {
                // Filter beams with profile type "L" efficiently using LINQ
                var angleBeams = section.partlist
                    .OfType<TSM.Beam>()
                    .Where(b => SkTeklaDrawingUtility.get_report_properties(b, "PROFILE_TYPE") == "L")
                    .ToList();

                if (angleBeams.Count > 1)
                {
                    // Process the first two angle beams
                    TSM.Beam beam0 = angleBeams[0];
                    TSM.Beam beam1 = angleBeams[1];

                    string partMark0 = SkTeklaDrawingUtility.get_report_properties(beam0, "PART_POS");
                    string partMark1 = SkTeklaDrawingUtility.get_report_properties(beam1, "PART_POS");

                    if (partMark0 == partMark1)
                    {
                        // Identical part marks: keep welds from the first beam only
                        AddWeldIdentifiers(beam0, weldsToKeep);
                    }
                    else
                    {
                        // Different part marks: keep welds from both beams
                        AddWeldIdentifiers(beam0, weldsToKeep);
                        AddWeldIdentifiers(beam1, weldsToKeep);
                    }
                }
                else if (angleBeams.Count == 1)
                {
                    // Single angle beam: keep its welds
                    TSM.Beam beam = angleBeams[0];
                    AddWeldIdentifiers(beam, weldsToKeep);
                }
            }

            // Delete weld marks not in the weldsToKeep set
            TSD.DrawingObjectEnumerator weldMarkEnumerator = currentViewFrontViewReq.GetAllObjects(typeof(TSD.WeldMark));
            while (weldMarkEnumerator.MoveNext())
            {
                TSD.WeldMark weldMark = weldMarkEnumerator.Current as TSD.WeldMark;
                if (weldMark != null)
                {
                    Identifier weldModelIdentifier = weldMark.ModelIdentifier;
                    if (!weldsToKeep.Contains(weldModelIdentifier))
                    {
                        weldMark.Delete();
                    }
                }
            }

            // Apply changes to the view and drawing
            currentViewFrontViewReq.Modify();
            beamDrawing.Modify();
            beamDrawing.CommitChanges();
        }

        /// <summary>
        /// Adds weld identifiers from a part to the specified HashSet.
        /// </summary>
        /// <param name="part">The part to extract welds from.</param>
        /// <param name="identifiers">The HashSet to store weld identifiers.</param>
        private void AddWeldIdentifiers(TSM.Part part, HashSet<Identifier> identifiers)
        {
            if (part == null) return;

            TSM.ModelObjectEnumerator weldEnumerator = part.GetWelds();
            while (weldEnumerator.MoveNext())
            {
                TSM.BaseWeld weld = weldEnumerator.Current as TSM.BaseWeld;
                if (weld != null)
                {
                    identifiers.Add(weld.Identifier);
                }
            }
        }
       
    }

    public class WeldMergeItem
    {
        public TSD.WeldMark WeldMark { get; set; }
        public Guid PartGuid { get; set; }
    }
}