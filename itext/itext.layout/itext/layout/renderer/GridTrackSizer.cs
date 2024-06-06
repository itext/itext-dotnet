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

namespace iText.Layout.Renderer {
    // 12.3. Track Sizing Algorithm
    internal class GridTrackSizer {
        private const float EPSILON = 0.001f;

        private readonly Grid grid;

        private readonly IList<GridTrackSizer.Track> tracks;

        private readonly float availableSpace;

        private readonly Grid.GridOrder order;

        private readonly ICollection<GridCell> cachedUniqueGridCells;

        internal GridTrackSizer(Grid grid, IList<GridValue> values, float gap, float availableSpace, Grid.GridOrder
             order) {
            this.grid = grid;
            // Cache the result of the getUniqueGridCells to speed up calculations
            this.cachedUniqueGridCells = grid.GetUniqueGridCells(order);
            tracks = new List<GridTrackSizer.Track>(values.Count);
            foreach (GridValue value in values) {
                GridTrackSizer.Track track = new GridTrackSizer.Track();
                track.value = value;
                tracks.Add(track);
            }
            if (availableSpace < 0) {
                foreach (GridTrackSizer.Track track in tracks) {
                    if (track.value.IsPercentValue()) {
                        // 7.2.1. Track Sizes: If the size of the grid container depends on the
                        // size of its tracks, then the <percentage> must be treated as auto
                        track.value = GridValue.CreateAutoValue();
                    }
                }
            }
            // Grid sizing algorithm says "Gutters are treated as empty fixed-size tracks for the purpose of the algorithm."
            // But relative gaps haven't supported yet, it is why to make algorithm simpler, available space just reduced by gaps.
            this.availableSpace = availableSpace - ((values.Count - 1) * gap);
            this.order = order;
        }

        internal virtual IList<float> SizeTracks() {
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
            IList<float> result = new List<float>(tracks.Count);
            foreach (GridTrackSizer.Track track in tracks) {
                result.Add(track.baseSize);
            }
            return result;
        }

        private void MaximizeTracks() {
            float? freeSpace = GetFreeSpace();
            if (freeSpace != null) {
                float leftSpace = (float)freeSpace;
                while (leftSpace > EPSILON) {
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
                if (track.value.IsFlexibleValue()) {
                    thereIsFlexibleTrack = true;
                    break;
                }
            }
            if (!thereIsFlexibleTrack) {
                return;
            }
            float frSize = 0;
            if (availableSpace >= 0) {
                // If the free space is zero or if sizing the grid container under a min-content constraint:
                float freeSpace = (float)GetFreeSpace();
                if (freeSpace < EPSILON) {
                    return;
                }
                // Otherwise, if the free space is a definite length:
                frSize = FindFrSize(tracks, availableSpace);
            }
            else {
                // Otherwise, if the free space is an indefinite length:
                foreach (GridTrackSizer.Track track in tracks) {
                    if (track.value.IsFlexibleValue()) {
                        frSize = Math.Max(frSize, (float)(track.baseSize / track.value.GetValue()));
                    }
                }
                foreach (GridCell cell in cachedUniqueGridCells) {
                    bool atLeastOneFlexTrack = false;
                    IList<GridTrackSizer.Track> affectedTracks = GetAffectedTracks(cell);
                    foreach (GridTrackSizer.Track track in affectedTracks) {
                        if (track.value.IsFlexibleValue()) {
                            atLeastOneFlexTrack = true;
                        }
                    }
                    if (!atLeastOneFlexTrack) {
                        continue;
                    }
                    float maxContribution = CalculateMaxContribution(cell, order);
                    frSize = Math.Max(frSize, FindFrSize(affectedTracks, maxContribution));
                }
            }
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.value.IsFlexibleValue()) {
                    float newBaseSize = frSize * (float)track.value.GetValue();
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

        private float? GetFreeSpace() {
            if (availableSpace < 0) {
                return null;
            }
            float freeSpace = availableSpace;
            foreach (GridTrackSizer.Track track in tracks) {
                freeSpace -= track.baseSize;
            }
            return freeSpace;
        }

        private static float FindFrSize(IList<GridTrackSizer.Track> affectedTracks, float spaceToFill) {
            // 12.7.1. Find the Size of an fr
            float frSize = 0;
            bool allFlexTracksSatisfied = false;
            bool[] ignoreTracks = new bool[affectedTracks.Count];
            while (!allFlexTracksSatisfied) {
                float leftoverSpace = spaceToFill;
                float flexFactorSum = 0;
                for (int i = 0; i < affectedTracks.Count; i++) {
                    GridTrackSizer.Track track = affectedTracks[i];
                    if (track.value.IsFlexibleValue() && !ignoreTracks[i]) {
                        flexFactorSum += (float)track.value.GetValue();
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
                    if (track.value.IsFlexibleValue() && !ignoreTracks[i]) {
                        if (hyphFrSize * track.value.GetValue() < track.baseSize) {
                            ignoreTracks[i] = true;
                            allFlexTracksSatisfied = false;
                        }
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
                // TODO DEVSIX-8384 percent value can be resolvable for height if height of grid container is specified
                if (track.value.IsPointValue() || track.value.IsPercentValue()) {
                    continue;
                }
                ICollection<GridCell> cells = grid.GetUniqueCellsInTrack(order, i);
                // -> For max-content minimums:
                if (track.value.IsMaxContentValue()) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMaxContribution(cell, order);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    track.baseSize = maxContribution;
                }
                // -> For min-content minimums:
                // -> For auto minimums: (also the case if track specified by fr value)
                if (track.value.IsAutoValue() || track.value.IsFlexibleValue() || track.value.IsMinContentValue()) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMinContribution(cell, order);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    track.baseSize = maxContribution;
                }
                // -> For min-content maximums:
                if (track.value.IsMinContentValue() && track.baseSize > EPSILON) {
                    track.growthLimit = track.baseSize;
                }
                // -> For max-content maximums:
                // Treat auto as max-content for max track sizing function
                if (track.value.IsAutoValue() || track.value.IsMaxContentValue()) {
                    float maxContribution = 0;
                    foreach (GridCell cell in cells) {
                        // non-spanning items only
                        if (cell.GetGridSpan(order) == 1) {
                            float contribution = CalculateMaxContribution(cell, order);
                            maxContribution = Math.Max(maxContribution, contribution);
                        }
                    }
                    if (maxContribution > EPSILON) {
                        track.growthLimit = maxContribution;
                    }
                }
                // if a track’s growth limit is now less than its base size
                if (track.growthLimit > 0 && track.baseSize > EPSILON && track.baseSize > track.growthLimit) {
                    track.growthLimit = track.baseSize;
                }
            }
            // 3. Increase sizes to accommodate spanning items crossing content-sized tracks.
            int maxSpanCell = 0;
            foreach (GridCell cell in cachedUniqueGridCells) {
                maxSpanCell = Math.Max(maxSpanCell, cell.GetGridSpan(order));
            }
            for (int span = 2; span <= maxSpanCell; span++) {
                foreach (GridCell cell in cachedUniqueGridCells) {
                    if (cell.GetGridSpan(order) == span) {
                        bool flexTracksExist = false;
                        IList<GridTrackSizer.Track> affectedTracks = GetAffectedTracks(cell);
                        foreach (GridTrackSizer.Track track in affectedTracks) {
                            if (track.value.IsFlexibleValue()) {
                                flexTracksExist = true;
                            }
                        }
                        if (flexTracksExist) {
                            continue;
                        }
                        float contribution = CalculateMinContribution(cell, order);
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
            foreach (GridCell cell in cachedUniqueGridCells) {
                bool atLeastOneFlexTrack = false;
                IList<GridTrackSizer.Track> affectedTracks = new List<GridTrackSizer.Track>();
                for (int i = cell.GetStart(order); i < cell.GetEnd(order); i++) {
                    if (tracks[i].value.IsFlexibleValue()) {
                        atLeastOneFlexTrack = true;
                        affectedTracks.Add(tracks[i]);
                    }
                }
                if (!atLeastOneFlexTrack) {
                    continue;
                }
                float contribution = CalculateMinContribution(cell, order);
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
            float trackSizes = 0;
            float sumFraction = 0;
            foreach (GridTrackSizer.Track track in tracks) {
                trackSizes += track.baseSize;
                if (track.value.IsFlexibleValue()) {
                    sumFraction += (float)track.value.GetValue();
                }
            }
            float space = Math.Max(0, sizeContribution - trackSizes);
            // 2. Distribute space up to limits:
            while (space > EPSILON) {
                float distributedSpace = space / sumFraction;
                bool allFrozen = true;
                foreach (GridTrackSizer.Track track in tracks) {
                    if (track.value.IsFlexibleValue()) {
                        float? added = DistributeSpaceToTrack(track, distributedSpace);
                        if (added != null) {
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
        private void DistributeExtraSpace(IList<GridTrackSizer.Track> tracks, bool affectsBase, float sizeContribution
            ) {
            // 1. Find the space to distribute:
            float trackSizes = 0;
            int numberOfAffectedTracks = 0;
            foreach (GridTrackSizer.Track track in tracks) {
                trackSizes += affectsBase ? track.baseSize : track.growthLimit;
                if (!track.value.IsPointValue() && !track.value.IsPercentValue()) {
                    numberOfAffectedTracks++;
                }
            }
            float space = Math.Max(0, sizeContribution - trackSizes);
            // 2. Distribute space up to limits:
            while (space > EPSILON) {
                float distributedSpace = space / numberOfAffectedTracks;
                bool allFrozen = true;
                foreach (GridTrackSizer.Track track in tracks) {
                    if (!track.value.IsPointValue() && !track.value.IsPercentValue()) {
                        float? added = DistributeSpaceToTrack(track, distributedSpace);
                        if (added != null) {
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
                // A fixed sizing function
                // TODO DEVSIX-8384 percent value can be resolvable for height if height of grid container is specified
                if (track.value.IsPointValue() || track.value.IsPercentValue()) {
                    if (track.value.IsPointValue()) {
                        track.baseSize = (float)track.value.GetValue();
                    }
                    else {
                        track.baseSize = (float)track.value.GetValue() / 100 * availableSpace;
                    }
                    track.growthLimit = track.baseSize;
                }
                else {
                    track.baseSize = 0;
                    track.growthLimit = -1;
                }
            }
        }

        private static float? DistributeSpaceToTrack(GridTrackSizer.Track track, float distributedSpace) {
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
            return null;
        }

        private static float CalculateMinContribution(GridCell cell, Grid.GridOrder order) {
            if (Grid.GridOrder.COLUMN == order) {
                if (cell.GetValue() is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)cell.GetValue();
                    return abstractRenderer.GetMinMaxWidth().GetMinWidth();
                }
            }
            else {
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

        private static float CalculateMaxContribution(GridCell cell, Grid.GridOrder gridOrder) {
            if (Grid.GridOrder.COLUMN == gridOrder) {
                if (cell.GetValue() is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)cell.GetValue();
                    return abstractRenderer.GetMinMaxWidth().GetMaxWidth();
                }
            }
            else {
                cell.GetValue().SetProperty(Property.FILL_AVAILABLE_AREA, false);
                // https://drafts.csswg.org/css-sizing-3/#auto-box-sizes:
                // min-content block size - For block containers, tables, and
                // inline boxes, this is equivalent to the max-content block size.
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

        private class Track {
            internal float baseSize;

            // consider -1 as an infinity value
            internal float growthLimit;

            internal GridValue value;
        }
    }
}
