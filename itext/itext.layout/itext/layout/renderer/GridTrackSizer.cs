/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    // 12.3. Track Sizing Algorithm
    /// <summary>Class representing a track sizing algorithm.</summary>
    internal class GridTrackSizer {
        private readonly Grid grid;

        private readonly IList<GridTrackSizer.Track> tracks;

        private readonly float gap;

        private readonly float availableSpace;

        private readonly Grid.GridOrder order;

        private readonly ICollection<int> percentValueIndexes = new HashSet<int>();

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a track sizing algorithm for given template.</summary>
        /// <param name="grid">grid to process</param>
        /// <param name="values">template values</param>
        /// <param name="gap">gap between tracks</param>
        /// <param name="availableSpace">space to fit tracks on</param>
        /// <param name="order">grid order</param>
        internal GridTrackSizer(Grid grid, IList<GridValue> values, float gap, float availableSpace, Grid.GridOrder
             order) {
            this.grid = grid;
            this.availableSpace = availableSpace;
            this.gap = gap;
            tracks = new List<GridTrackSizer.Track>(values.Count);
            foreach (GridValue value in values) {
                GridTrackSizer.Track track = new GridTrackSizer.Track();
                track.value = value;
                tracks.Add(track);
            }
            if (availableSpace < 0) {
                for (int i = 0; i < tracks.Count; ++i) {
                    GridTrackSizer.Track track = tracks[i];
                    if (track.value.GetType() == TemplateValue.ValueType.PERCENT) {
                        // 7.2.1. Track Sizes: If the size of the grid container depends on the
                        // size of its tracks, then the <percentage> must be treated as auto
                        percentValueIndexes.Add(i);
                        track.value = AutoValue.VALUE;
                    }
                    if (track.value.GetType() == TemplateValue.ValueType.FIT_CONTENT && ((FitContentValue)track.value).GetLength
                        ().GetType() == TemplateValue.ValueType.PERCENT) {
                        // "7.2.1. Track Sizes: If the size of the grid container depends on the
                        // size of its tracks, then the <percentage> must be treated as auto"
                        // for fit content this means, that formula becomes max(auto-minimum, auto-maximum) = auto
                        track.value = AutoValue.VALUE;
                    }
                }
            }
            this.order = order;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resolves template values and auto-values to point values.</summary>
        /// <returns>list of points, representing track sizes with expanded percentages in case of inline calculation.
        ///     </returns>
        internal virtual GridTrackSizer.TrackSizingResult SizeTracks() {
            // First step (12.4. Initialize Track Sizes)
            InitializeTrackSizes();
            // Second step (12.5. Resolve Intrinsic Track Sizes)
            ResolveIntrinsicTrackSizes();
            // Third step (12.6. Maximize Tracks)
            MaximizeTracks();
            // Fourth step (12.7. Expand Flexible Tracks)
            ExpandFlexibleTracks();
            // Fifth step (12.8. Stretch auto Tracks)
            // Skip for now
            return new GridTrackSizer.TrackSizingResult(tracks, gap, percentValueIndexes);
        }
//\endcond

        private void MaximizeTracks() {
            float freeSpace = GetFreeSpace();
            if (availableSpace > 0) {
                float leftSpace = (float)freeSpace;
                while (leftSpace > 0.0f) {
                    int unfrozenTracks = 0;
                    foreach (GridTrackSizer.Track track in tracks) {
                        if (JavaUtil.FloatCompare(track.baseSize, track.growthLimit) < 0) {
                            unfrozenTracks++;
                        }
                    }
                    if (unfrozenTracks == 0) {
                        break;
                    }
                    float diff = leftSpace / unfrozenTracks;
                    foreach (GridTrackSizer.Track track in tracks) {
                        if (JavaUtil.FloatCompare(track.baseSize, track.growthLimit) < 0) {
                            float trackDiff = Math.Min(track.growthLimit, track.baseSize + diff) - track.baseSize;
                            track.baseSize += trackDiff;
                            leftSpace -= trackDiff;
                        }
                    }
                }
            }
            else {
                foreach (GridTrackSizer.Track track in tracks) {
                    if (JavaUtil.FloatCompare(track.baseSize, track.growthLimit) < 0) {
                        track.baseSize = track.growthLimit;
                    }
                }
            }
        }

        private void ExpandFlexibleTracks() {
            bool thereIsFlexibleTrack = false;
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                    thereIsFlexibleTrack = true;
                    break;
                }
            }
            if (!thereIsFlexibleTrack) {
                return;
            }
            float frSize = 0;
            if (availableSpace > 0.0f) {
                // If the free space is zero or if sizing the grid container under a min-content constraint:
                float freeSpace = (float)GetFreeSpace();
                if (freeSpace < 0.0f) {
                    return;
                }
                // Otherwise, if the free space is a definite length:
                frSize = FindFrSize(tracks, GetAvailableSpaceForSizing());
            }
            else {
                // Otherwise, if the free space is an indefinite length:
                foreach (GridTrackSizer.Track track in tracks) {
                    if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                        frSize = Math.Max(frSize, track.baseSize / ((FlexValue)track.value).GetFlex());
                    }
                }
                foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                    bool atLeastOneFlexTrack = false;
                    IList<GridTrackSizer.Track> affectedTracks = GetAffectedTracks(cell);
                    foreach (GridTrackSizer.Track track in affectedTracks) {
                        if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                            atLeastOneFlexTrack = true;
                            break;
                        }
                    }
                    if (!atLeastOneFlexTrack) {
                        continue;
                    }
                    float maxContribution = CalculateMinMaxContribution(cell, false);
                    frSize = Math.Max(frSize, FindFrSize(affectedTracks, maxContribution));
                }
            }
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                    float newBaseSize = frSize * ((FlexValue)track.value).GetFlex();
                    if (newBaseSize > track.baseSize) {
                        track.baseSize = newBaseSize;
                    }
                }
            }
        }

        private IList<GridTrackSizer.Track> GetAffectedTracks(GridCell cell) {
            IList<GridTrackSizer.Track> affectedTracks = new List<GridTrackSizer.Track>();
            for (int i = cell.GetStart(order); i < cell.GetEnd(order); i++) {
                affectedTracks.Add(tracks[i]);
            }
            return affectedTracks;
        }

        private float GetAvailableSpaceForSizing() {
            // Grid sizing algorithm says "Gutters are treated as empty fixed-size tracks for the purpose of the algorithm."
            // But relative gaps haven't supported yet, it is why to make algorithm simpler, available space just reduced by gaps.
            return availableSpace - ((tracks.Count - 1) * gap);
        }

        private float GetFreeSpace() {
            float freeSpace = GetAvailableSpaceForSizing();
            foreach (GridTrackSizer.Track track in tracks) {
                freeSpace -= track.baseSize;
            }
            return freeSpace;
        }

        private static float FindFrSize(IList<GridTrackSizer.Track> affectedTracks, float spaceToFill) {
            // 12.7.1. Find the Size of an 'fr'
            float frSize = 0;
            bool allFlexTracksSatisfied = false;
            bool[] ignoreTracks = new bool[affectedTracks.Count];
            while (!allFlexTracksSatisfied) {
                float leftoverSpace = spaceToFill;
                float flexFactorSum = 0;
                for (int i = 0; i < affectedTracks.Count; i++) {
                    GridTrackSizer.Track track = affectedTracks[i];
                    if (track.value.GetType() == TemplateValue.ValueType.FLEX && !ignoreTracks[i]) {
                        flexFactorSum += ((FlexValue)track.value).GetFlex();
                    }
                    else {
                        leftoverSpace -= track.baseSize;
                    }
                }
                flexFactorSum = flexFactorSum < 1 ? 1 : flexFactorSum;
                float hyphFrSize = leftoverSpace / flexFactorSum;
                allFlexTracksSatisfied = true;
                for (int i = 0; i < affectedTracks.Count; i++) {
                    GridTrackSizer.Track track = affectedTracks[i];
                    if (track.value.GetType() == TemplateValue.ValueType.FLEX && !ignoreTracks[i] && hyphFrSize * ((FlexValue)
                        track.value).GetFlex() < track.baseSize) {
                        ignoreTracks[i] = true;
                        allFlexTracksSatisfied = false;
                    }
                }
                if (allFlexTracksSatisfied) {
                    frSize = hyphFrSize;
                }
            }
            return frSize;
        }

        private void ResolveIntrinsicTrackSizes() {
            // 1. Shim baseline-aligned items so their intrinsic size contributions reflect their baseline alignment.
            // Not sure whether we need to do anything in first point
            // 2. Size tracks to fit non-spanning items.
            for (int i = 0; i < tracks.Count; i++) {
                GridTrackSizer.Track track = tracks[i];
                GridValue minTrackSizingValue = track.value;
                GridValue maxTrackSizingValue = track.value;
                if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                    minTrackSizingValue = ((MinMaxValue)track.value).GetMin();
                    maxTrackSizingValue = ((MinMaxValue)track.value).GetMax();
                }
                ICollection<GridCell> cells = grid.GetUniqueCellsInTrack(order, i);
                // -> For max-content minimums:
                if (minTrackSizingValue.GetType() == TemplateValue.ValueType.MAX_CONTENT) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMinMaxContribution(cell, false);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    track.baseSize = maxContribution;
                }
                // -> For min-content minimums:
                // -> For auto minimums: (also the case if track specified by fr value)
                if (minTrackSizingValue.GetType() == TemplateValue.ValueType.AUTO || minTrackSizingValue.GetType() == TemplateValue.ValueType
                    .FLEX || minTrackSizingValue.GetType() == TemplateValue.ValueType.MIN_CONTENT || minTrackSizingValue.GetType
                    () == TemplateValue.ValueType.FIT_CONTENT) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMinMaxContribution(cell, true);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    track.baseSize = maxContribution;
                }
                // -> For min-content maximums:
                if (maxTrackSizingValue.GetType() == TemplateValue.ValueType.MIN_CONTENT && track.baseSize > 0.0f) {
                    track.growthLimit = track.baseSize;
                }
                // -> For max-content maximums:
                // Treat auto as max-content for max track sizing function
                if (maxTrackSizingValue.GetType() == TemplateValue.ValueType.AUTO || maxTrackSizingValue.GetType() == TemplateValue.ValueType
                    .MAX_CONTENT || maxTrackSizingValue.GetType() == TemplateValue.ValueType.FIT_CONTENT) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMinMaxContribution(cell, false);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    if (maxContribution > 0.0f) {
                        track.growthLimit = maxContribution;
                        if (maxTrackSizingValue.GetType() == TemplateValue.ValueType.FIT_CONTENT) {
                            //For fit-content() maximums, furthermore clamp this growth limit by the fit-content() argument.
                            float maxSize = ((FitContentValue)maxTrackSizingValue).GetMaxSizeForSpace(availableSpace);
                            track.growthLimit = Math.Min(track.growthLimit, maxSize);
                        }
                    }
                }
                // if a track’s growth limit is now less than its base size
                if (track.growthLimit > 0.0f && track.baseSize > 0.0f && track.baseSize > track.growthLimit) {
                    track.growthLimit = track.baseSize;
                }
            }
            // 3. Increase sizes to accommodate spanning items crossing content-sized tracks.
            int maxSpanCell = 0;
            foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                maxSpanCell = Math.Max(maxSpanCell, cell.GetGridSpan(order));
            }
            for (int span = 2; span <= maxSpanCell; span++) {
                foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                    if (cell.GetGridSpan(order) == span) {
                        bool flexTracksExist = false;
                        IList<GridTrackSizer.Track> affectedTracks = GetAffectedTracks(cell);
                        foreach (GridTrackSizer.Track track in affectedTracks) {
                            if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                                flexTracksExist = true;
                            }
                        }
                        if (flexTracksExist) {
                            continue;
                        }
                        float contribution = CalculateMinMaxContribution(cell, true);
                        // 3.1 For intrinsic minimums:
                        // 3.2 For content-based minimums:
                        // 3.3 For max-content minimums:
                        DistributeExtraSpace(affectedTracks, true, contribution);
                    }
                }
            }
            // 3.4 If at this point any track’s growth limit is now less than its base size:
            // 3.5 For intrinsic maximums:
            // 3.6 For max-content maximums:
            // 4. Increase sizes to accommodate spanning items crossing flexible tracks:
            foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                IList<GridTrackSizer.Track> affectedTracks = new List<GridTrackSizer.Track>();
                for (int i = cell.GetStart(order); i < cell.GetEnd(order); i++) {
                    if (tracks[i].value.GetType() == TemplateValue.ValueType.FLEX) {
                        affectedTracks.Add(tracks[i]);
                    }
                }
                if (affectedTracks.IsEmpty()) {
                    continue;
                }
                float contribution = CalculateMinMaxContribution(cell, true);
                DistributeExtraSpaceWithFlexTracks(affectedTracks, contribution);
            }
            // 5. If any track still has an infinite growth limit
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.growthLimit < 0) {
                    track.growthLimit = track.baseSize;
                }
            }
        }

        private void DistributeExtraSpaceWithFlexTracks(IList<GridTrackSizer.Track> tracks, float sizeContribution
            ) {
            // 1. Find the space to distribute:
            float trackSizes = 0.0f;
            float sumFraction = 0.0f;
            foreach (GridTrackSizer.Track track in tracks) {
                trackSizes += track.baseSize;
                sumFraction += ((FlexValue)track.value).GetFlex();
            }
            if (sumFraction < 1.0f) {
                sumFraction = 1.0f;
            }
            float space = Math.Max(0.0f, sizeContribution - trackSizes);
            float spacePerFraction = space / sumFraction;
            // 2. Distribute space up to limits:
            // For flex values we know that they're can't be frozen so we can distribute all available space at once
            foreach (GridTrackSizer.Track track in tracks) {
                DistributeSpaceToTrack(track, spacePerFraction * ((FlexValue)track.value).GetFlex());
            }
        }

        // 3. Distribute space to non-affected tracks: skipped
        // 4. Distribute space beyond limits: skipped
        private void DistributeExtraSpace(IList<GridTrackSizer.Track> tracks, bool affectsBase, float sizeContribution
            ) {
            // 1. Find the space to distribute:
            float trackSizes = 0;
            int numberOfAffectedTracks = 0;
            foreach (GridTrackSizer.Track track in tracks) {
                GridValue value = track.value;
                if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                    value = affectsBase ? ((MinMaxValue)track.value).GetMin() : ((MinMaxValue)track.value).GetMax();
                }
                trackSizes += affectsBase ? track.baseSize : track.growthLimit;
                if (value.GetType() != TemplateValue.ValueType.POINT && value.GetType() != TemplateValue.ValueType.PERCENT
                    ) {
                    numberOfAffectedTracks++;
                }
            }
            float space = Math.Max(0, sizeContribution - trackSizes);
            // 2. Distribute space up to limits:
            while (space > 0.0f) {
                float distributedSpace = space / numberOfAffectedTracks;
                bool allFrozen = true;
                foreach (GridTrackSizer.Track track in tracks) {
                    GridValue value = track.value;
                    if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                        value = affectsBase ? ((MinMaxValue)track.value).GetMin() : ((MinMaxValue)track.value).GetMax();
                    }
                    if (value.GetType() != TemplateValue.ValueType.POINT && value.GetType() != TemplateValue.ValueType.PERCENT
                        ) {
                        float added = DistributeSpaceToTrack(track, distributedSpace);
                        if (added > 0) {
                            space -= (float)added;
                            allFrozen = false;
                        }
                    }
                }
                if (allFrozen) {
                    break;
                }
            }
        }

        // 3. Distribute space to non-affected tracks: skipped
        // 4. Distribute space beyond limits: skipped
        private void InitializeTrackSizes() {
            foreach (GridTrackSizer.Track track in tracks) {
                GridValue minTrackSizingValue = track.value;
                GridValue maxTrackSizingValue = track.value;
                if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                    minTrackSizingValue = ((MinMaxValue)track.value).GetMin();
                    maxTrackSizingValue = ((MinMaxValue)track.value).GetMax();
                }
                // A fixed sizing function
                if (minTrackSizingValue.GetType() == TemplateValue.ValueType.POINT || minTrackSizingValue.GetType() == TemplateValue.ValueType
                    .PERCENT) {
                    if (minTrackSizingValue.GetType() == TemplateValue.ValueType.POINT) {
                        track.baseSize = ((LengthValue)minTrackSizingValue).GetValue();
                    }
                    else {
                        track.baseSize = ((LengthValue)minTrackSizingValue).GetValue() / 100 * availableSpace;
                    }
                }
                else {
                    track.baseSize = 0;
                }
                // A fixed sizing function
                if (maxTrackSizingValue.GetType() == TemplateValue.ValueType.POINT || maxTrackSizingValue.GetType() == TemplateValue.ValueType
                    .PERCENT) {
                    if (maxTrackSizingValue.GetType() == TemplateValue.ValueType.POINT) {
                        track.growthLimit = ((LengthValue)maxTrackSizingValue).GetValue();
                    }
                    else {
                        track.growthLimit = ((LengthValue)maxTrackSizingValue).GetValue() / 100 * availableSpace;
                    }
                }
                else {
                    track.growthLimit = -1;
                }
            }
        }

        /// <summary>
        /// Distributes given space to track, if given space can't be fully distributed returns
        /// as many space as was distributed.
        /// </summary>
        /// <param name="track">track to which distribute space</param>
        /// <param name="distributedSpace">how much space to distribute</param>
        /// <returns>how much space was distributed</returns>
        private static float DistributeSpaceToTrack(GridTrackSizer.Track track, float distributedSpace) {
            if (track.growthLimit < 0 || distributedSpace + track.baseSize <= track.growthLimit) {
                track.baseSize += distributedSpace;
                return distributedSpace;
            }
            else {
                if (JavaUtil.FloatCompare(track.growthLimit, track.baseSize) != 0) {
                    float addedToLimit = track.growthLimit - track.baseSize;
                    track.baseSize += addedToLimit;
                    return addedToLimit;
                }
            }
            return -1.0f;
        }

        /// <summary>Calculate min or max contribution of a cell.</summary>
        /// <param name="cell">cell to calculate contribution</param>
        /// <param name="minTypeContribution">type of contribution: min if true, max otherwise</param>
        /// <returns>contribution value</returns>
        private float CalculateMinMaxContribution(GridCell cell, bool minTypeContribution) {
            if (Grid.GridOrder.COLUMN == order) {
                if (cell.GetValue() is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)cell.GetValue();
                    return minTypeContribution ? abstractRenderer.GetMinMaxWidth().GetMinWidth() : abstractRenderer.GetMinMaxWidth
                        ().GetMaxWidth();
                }
            }
            else {
                // https://drafts.csswg.org/css-sizing-3/#auto-box-sizes:
                // min-content block size - For block containers, tables, and
                // inline boxes, this is equivalent to the max-content block size.
                cell.GetValue().SetProperty(Property.FILL_AVAILABLE_AREA, false);
                LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, new Rectangle(cell.GetLayoutArea().GetWidth
                    (), AbstractRenderer.INF)));
                LayoutResult inifiniteHeighLayoutResult = cell.GetValue().Layout(layoutContext);
                if (inifiniteHeighLayoutResult.GetStatus() == LayoutResult.NOTHING || inifiniteHeighLayoutResult.GetStatus
                    () == LayoutResult.PARTIAL) {
                    return 0;
                }
                return inifiniteHeighLayoutResult.GetOccupiedArea().GetBBox().GetHeight();
            }
            return 0;
        }

//\cond DO_NOT_DOCUMENT
        internal class TrackSizingResult {
            private readonly IList<GridTrackSizer.Track> tracks;

            private readonly ICollection<int> percentValueIndexes;

            private readonly float gap;

//\cond DO_NOT_DOCUMENT
            internal TrackSizingResult(IList<GridTrackSizer.Track> tracks, float gap, ICollection<int> percentValueIndexes
                ) {
                this.tracks = tracks;
                this.percentValueIndexes = percentValueIndexes;
                this.gap = gap;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            /// <summary>Get original track sizes which are were resolved during track sizing algorithm.</summary>
            /// <remarks>
            /// Get original track sizes which are were resolved during track sizing algorithm.
            /// If result contains inline percentages those are not expanded/reduced and have a size equivalent
            /// of AUTO.
            /// </remarks>
            /// <returns>original track sizes list</returns>
            internal virtual IList<float> GetTrackSizes() {
                IList<float> result = new List<float>(tracks.Count);
                foreach (GridTrackSizer.Track track in tracks) {
                    result.Add(track.baseSize);
                }
                return result;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            /// <summary>Get expanded track sizes where inline percents are resolved against calculated grid area.</summary>
            /// <param name="template">grid value template</param>
            /// <returns>expanded track sizes list</returns>
            internal virtual IList<float> GetTrackSizesAndExpandPercents(IList<GridValue> template) {
                if (percentValueIndexes.IsEmpty()) {
                    return GetTrackSizes();
                }
                // Resolve inline percentage values (7.2.1. Track Sizes)
                float total = 0.0f;
                foreach (GridTrackSizer.Track track in tracks) {
                    total += track.baseSize;
                }
                total += ((tracks.Count - 1) * gap);
                IList<float> expandedTrackSizes = new List<float>(tracks.Count);
                for (int i = 0; i < tracks.Count; ++i) {
                    if (percentValueIndexes.Contains(i)) {
                        expandedTrackSizes.Add(((PercentValue)template[i]).GetValue() / 100 * total);
                    }
                    else {
                        expandedTrackSizes.Add(tracks[i].baseSize);
                    }
                }
                return expandedTrackSizes;
            }
//\endcond
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class Track {
//\cond DO_NOT_DOCUMENT
            internal float baseSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            // consider -1 as an infinity value
            internal float growthLimit;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal GridValue value;
//\endcond
        }
//\endcond
    }
//\endcond
}
