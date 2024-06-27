/*
Copyright 2015 The Chromium Authors

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

* Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.
* Neither the name of Google LLC nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
    // 12.3. Track Sizing Algorithm https://drafts.csswg.org/css-grid-2/#algo-track-sizing
    // More than half of the code in that class was ported from chromium code on C++
    // See https://source.chromium.org/chromium/chromium/src/+/main:third_party/blink/renderer/core/layout/grid/grid_layout_algorithm.cc;l=1858
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
            // iText's method
            this.grid = grid;
            this.availableSpace = availableSpace;
            this.gap = gap;
            tracks = new List<GridTrackSizer.Track>(values.Count);
            foreach (GridValue value in values) {
                GridTrackSizer.Track track = new GridTrackSizer.Track();
                track.value = value;
                tracks.Add(track);
                if (track.value.GetType() == TemplateValue.ValueType.FLEX) {
                    track.value = new MinMaxValue(AutoValue.VALUE, (FlexValue)track.value);
                }
            }
            if (JavaUtil.FloatCompare(availableSpace, -1f) == 0) {
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
            // iText's method
            // First step (12.4. Initialize Track Sizes)
            InitializeTrackSizes();
            // Second step (12.5. Resolve Intrinsic Track Sizes)
            ResolveIntrinsicTrackSizes();
            // Third step (12.6. Maximize Tracks)
            MaximizeTracks();
            // Fourth step (12.7. Expand Flexible Tracks)
            ExpandFlexibleTracks();
            // Fifth step (12.8. Stretch auto Tracks)
            StretchAutoTracks();
            return new GridTrackSizer.TrackSizingResult(tracks, gap, percentValueIndexes);
        }
//\endcond

        // chromium's method
        private void StretchAutoTracks() {
            // iText's comment: for now consider that we always have content-distribution property equals to `normal`
            // Expand tracks that have an 'auto' max track sizing function by dividing any
            // remaining positive, definite free space equally amongst them.
            IList<GridTrackSizer.Track> tracksToGrow = new List<GridTrackSizer.Track>();
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.HasAutoMax()) {
                    tracksToGrow.Add(track);
                }
            }
            if (tracksToGrow.IsEmpty()) {
                return;
            }
            float freeSpace = DetermineFreeSpace();
            // iText's comment: the case when grid container has min-width\height is processed in the GridContainerRenderer
            if (JavaUtil.FloatCompare(freeSpace, -1f) == 0) {
                return;
            }
            DistributeExtraSpaceToTracks(freeSpace, 0, GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE, tracksToGrow
                , tracksToGrow, true);
            foreach (GridTrackSizer.Track track in tracksToGrow) {
                track.baseSize += track.incurredIncrease;
                track.EnsureGrowthLimitIsNotLessThanBaseSize();
            }
        }

        // chromium's method
        private void MaximizeTracks() {
            float freeSpace = DetermineFreeSpace();
            if (JavaUtil.FloatCompare(freeSpace, 0f) == 0) {
                return;
            }
            IList<GridTrackSizer.Track> tracksToGrow = new List<GridTrackSizer.Track>(tracks);
            DistributeExtraSpaceToTracks(freeSpace, 0f, GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE, tracksToGrow
                , null, true);
            foreach (GridTrackSizer.Track track in tracksToGrow) {
                track.baseSize += track.incurredIncrease;
                track.EnsureGrowthLimitIsNotLessThanBaseSize();
            }
        }

        // chromium's method
        private void ExpandFlexibleTracks() {
            bool thereIsFlexibleTrack = false;
            foreach (GridTrackSizer.Track track in tracks) {
                if (track.IsFlexibleTrack()) {
                    thereIsFlexibleTrack = true;
                    break;
                }
            }
            // iText's comment: this check is performed in GridTrackSizer.sizeTracks method in chromium
            if (!thereIsFlexibleTrack) {
                return;
            }
            float freeSpace = DetermineFreeSpace();
            // If the free space is zero or if sizing the grid container under a
            // min-content constraint, the used flex fraction is zero.
            if (JavaUtil.FloatCompare(freeSpace, 0f) == 0) {
                return;
            }
            float frSize = 0;
            if (JavaUtil.FloatCompare(freeSpace, -1f) != 0) {
                // Otherwise, if the free space is a definite length, the used flex fraction
                // is the result of finding the size of an fr using all of the grid tracks
                // and a space to fill of the available grid space.
                frSize = FindFrSize(tracks, availableSpace);
            }
            else {
                // Otherwise, if the free space is an indefinite length, the used flex
                // fraction is the maximum of:
                //   - For each grid item that crosses a flexible track, the result of
                //   finding the size of an fr using all the grid tracks that the item
                //   crosses and a space to fill of the item's max-content contribution.
                foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                    IList<GridTrackSizer.Track> flexSpannedTracks = new List<GridTrackSizer.Track>();
                    IList<GridTrackSizer.Track> spannedTracks = GetSpannedTracks(cell);
                    foreach (GridTrackSizer.Track track in spannedTracks) {
                        if (track.IsFlexibleTrack()) {
                            flexSpannedTracks.Add(track);
                        }
                    }
                    // iText's comment: grid_item.IsConsideredForSizing(track_direction) check was skipped, because it isn't clear how it works
                    if (flexSpannedTracks.IsEmpty()) {
                        continue;
                    }
                    float gridItemFrSize = FindFrSize(spannedTracks, CalculateMinMaxContribution(cell, false));
                    frSize = Math.Max(gridItemFrSize, frSize);
                }
                //   - For each flexible track, if the flexible track's flex factor is
                //   greater than one, the result of dividing the track's base size by its
                //   flex factor; otherwise, the track's base size.
                foreach (GridTrackSizer.Track track in tracks) {
                    if (!track.IsFlexibleTrack()) {
                        continue;
                    }
                    float trackFlexFactor = Math.Max(track.GetFlexFactor(), 1);
                    frSize = Math.Max(track.baseSize / trackFlexFactor, frSize);
                }
            }
            // iText's comment: logic with leftover_size and expanded_size skipped because it isn't needed for java
            foreach (GridTrackSizer.Track track in tracks) {
                if (!track.IsFlexibleTrack()) {
                    continue;
                }
                float frShare = frSize * track.GetFlexFactor();
                if (frShare >= track.baseSize) {
                    track.baseSize = frShare;
                    track.EnsureGrowthLimitIsNotLessThanBaseSize();
                }
            }
        }

        // iText's method
        private IList<GridTrackSizer.Track> GetSpannedTracks(GridCell cell) {
            IList<GridTrackSizer.Track> affectedTracks = new List<GridTrackSizer.Track>();
            for (int i = cell.GetStart(order); i < cell.GetEnd(order); i++) {
                affectedTracks.Add(tracks[i]);
            }
            return affectedTracks;
        }

        // chromium's method
        private float DetermineFreeSpace() {
            // iText's comment: method was simplified, because we don't support different sizing constraint
            float freeSpace = availableSpace;
            if (JavaUtil.FloatCompare(freeSpace, -1f) != 0) {
                foreach (GridTrackSizer.Track track in tracks) {
                    freeSpace -= track.baseSize;
                }
                freeSpace -= (tracks.Count - 1) * gap;
                // If tracks consume more space than the grid container has available,
                // clamp the free space to zero as there's no more room left to grow.
                return Math.Max(freeSpace, 0);
            }
            return -1;
        }

        // iText's method
        private float FindFrSize(IList<GridTrackSizer.Track> affectedTracks, float leftoverSpace) {
            // iText's comment: initially was implemented method from chromium but it worked worse in some cases than our implementation
            // 12.7.1. Find the Size of an 'fr'
            float frSize = 0;
            bool allFlexTracksSatisfied = false;
            bool[] ignoreTracks = new bool[affectedTracks.Count];
            while (!allFlexTracksSatisfied) {
                float currentLeftoverSpace = leftoverSpace;
                int totalTrackCount = 0;
                float flexFactorSum = 0;
                for (int i = 0; i < affectedTracks.Count; i++) {
                    GridTrackSizer.Track track = affectedTracks[i];
                    totalTrackCount++;
                    if (track.IsFlexibleTrack() && !ignoreTracks[i]) {
                        flexFactorSum += track.GetFlexFactor();
                    }
                    else {
                        currentLeftoverSpace -= track.baseSize;
                    }
                }
                currentLeftoverSpace -= (totalTrackCount - 1) * gap;
                flexFactorSum = flexFactorSum < 1 ? 1 : flexFactorSum;
                float hyphFrSize = currentLeftoverSpace / flexFactorSum;
                allFlexTracksSatisfied = true;
                for (int i = 0; i < affectedTracks.Count; i++) {
                    GridTrackSizer.Track track = affectedTracks[i];
                    if (track.IsFlexibleTrack() && !ignoreTracks[i] && hyphFrSize * track.GetFlexFactor() < track.baseSize) {
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

        // chromium's method
        private void ResolveIntrinsicTrackSizes() {
            // iText's comment: part for subgrid is skipped
            // itext's comment: reordering grid items is skipped, grid items groups are created through `for` cycle
            // iText's comment: 2 - Size tracks to fit non-spanning items
            // iText's comment: 3 - Increase sizes to accommodate spanning items crossing content-sized tracks
            int maxSpanCell = 0;
            foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                maxSpanCell = Math.Max(maxSpanCell, cell.GetGridSpan(order));
            }
            // First, process the items that don't span a flexible track.
            for (int span = 1; span <= maxSpanCell; span++) {
                IList<GridCell> group = new List<GridCell>();
                foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                    // Each iteration considers all items with the same span size.
                    if (cell.GetGridSpan(order) == span) {
                        bool flexTracksExist = false;
                        IList<GridTrackSizer.Track> spannedTracks = GetSpannedTracks(cell);
                        foreach (GridTrackSizer.Track track in spannedTracks) {
                            if (track.IsFlexibleTrack()) {
                                flexTracksExist = true;
                                break;
                            }
                        }
                        if (flexTracksExist) {
                            continue;
                        }
                        group.Add(cell);
                    }
                }
                IncreaseTrackSizesToAccommodateGridItems(group, false, GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group, false, GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group, false, GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group, false, GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group, false, GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS
                    );
            }
            // iText's comment: 4 - Increase sizes to accommodate spanning items crossing flexible tracks
            // Now, process items spanning flexible tracks (if any).
            IList<GridCell> group_1 = new List<GridCell>();
            foreach (GridCell cell in grid.GetUniqueGridCells(order)) {
                IList<GridTrackSizer.Track> flexSpannedTracks = new List<GridTrackSizer.Track>();
                IList<GridTrackSizer.Track> spannedTracks = GetSpannedTracks(cell);
                foreach (GridTrackSizer.Track track in spannedTracks) {
                    if (track.IsFlexibleTrack()) {
                        flexSpannedTracks.Add(track);
                    }
                }
                if (flexSpannedTracks.IsEmpty()) {
                    continue;
                }
                group_1.Add(cell);
            }
            if (!group_1.IsEmpty()) {
                // We can safely skip contributions for maximums since a <flex> definition
                // does not have an intrinsic max track sizing function.
                IncreaseTrackSizesToAccommodateGridItems(group_1, true, GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group_1, true, GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS
                    );
                IncreaseTrackSizesToAccommodateGridItems(group_1, true, GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS
                    );
            }
            // iText's comment: 5 - If any track still has an infinite growth limit
            // iText's comment: in chromium this part in GridTrackSizer.sizeTracks method
            foreach (GridTrackSizer.Track track in tracks) {
                if (JavaUtil.FloatCompare(track.growthLimit, -1f) == 0) {
                    track.growthLimit = track.baseSize;
                }
            }
        }

        // chromium's method
        private void IncreaseTrackSizesToAccommodateGridItems(IList<GridCell> group, bool isGroupSpanningFlexTrack
            , GridTrackSizer.GridItemContributionType contributionType) {
            foreach (GridTrackSizer.Track track in tracks) {
                track.plannedIncrease = -1;
            }
            foreach (GridCell cell in group) {
                IList<GridTrackSizer.Track> tracksToGrow = new List<GridTrackSizer.Track>();
                IList<GridTrackSizer.Track> tracksToGrowBeyondLimit = new List<GridTrackSizer.Track>();
                float flexFactorSum = 0;
                float spannedTrackSize = gap * (cell.GetGridSpan(order) - 1);
                foreach (GridTrackSizer.Track track in GetSpannedTracks(cell)) {
                    spannedTrackSize += AffectedSizeForContribution(track, contributionType);
                    if (isGroupSpanningFlexTrack && !track.IsFlexibleTrack()) {
                        // From https://drafts.csswg.org/css-grid-2/#algo-spanning-flex-items:
                        //   Distributing space only to flexible tracks (i.e. treating all other
                        //   tracks as having a fixed sizing function).
                        continue;
                    }
                    if (IsContributionAppliedToTrack(track, contributionType)) {
                        if (JavaUtil.FloatCompare(track.plannedIncrease, -1f) == 0) {
                            track.plannedIncrease = 0;
                        }
                        if (isGroupSpanningFlexTrack) {
                            flexFactorSum += track.GetFlexFactor();
                        }
                        tracksToGrow.Add(track);
                        if (ShouldUsedSizeGrowBeyondLimit(track, contributionType)) {
                            tracksToGrowBeyondLimit.Add(track);
                        }
                    }
                }
                if (tracksToGrow.IsEmpty()) {
                    continue;
                }
                // iText's comment: extraSpace calculation was simplified in comparison how it works in chromium,
                // iText's comment: it is possible place for difference with browser
                bool minTypeContribution = contributionType == GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS
                     || contributionType == GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS || contributionType
                     == GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS;
                // Subtract the corresponding size (base size or growth limit) of every
                // spanned track from the grid item's size contribution to find the item's
                // remaining size contribution. For infinite growth limits, substitute with
                // the track's base size. This is the space to distribute, floor it at zero.
                float extraSpace = CalculateMinMaxContribution(cell, minTypeContribution);
                extraSpace = Math.Max(extraSpace - spannedTrackSize, 0);
                if (JavaUtil.FloatCompare(extraSpace, 0) == 0) {
                    continue;
                }
                // From https://drafts.csswg.org/css-grid-2/#algo-spanning-flex-items:
                //   If the sum of the flexible sizing functions of all flexible tracks
                //   spanned by the item is greater than zero, distributing space to such
                //   tracks according to the ratios of their flexible sizing functions
                //   rather than distributing space equally.
                if (!isGroupSpanningFlexTrack || JavaUtil.FloatCompare(flexFactorSum, 0f) == 0) {
                    DistributeExtraSpaceToTracks(extraSpace, 0, contributionType, tracksToGrow, tracksToGrowBeyondLimit.IsEmpty
                        () ? tracksToGrow : tracksToGrowBeyondLimit, true);
                }
                else {
                    // 'fr' units are only allowed as a maximum in track definitions, meaning
                    // that no track has an intrinsic max track sizing function that would allow
                    // it to grow beyond limits (see |ShouldUsedSizeGrowBeyondLimit|).
                    DistributeExtraSpaceToTracks(extraSpace, flexFactorSum, contributionType, tracksToGrow, null, false);
                }
                // For each affected track, if the track's item-incurred increase is larger
                // than its planned increase, set the planned increase to that value.
                foreach (GridTrackSizer.Track track in tracksToGrow) {
                    track.plannedIncrease = Math.Max(track.incurredIncrease, track.plannedIncrease);
                }
            }
            foreach (GridTrackSizer.Track track in tracks) {
                GrowAffectedSizeByPlannedIncrease(track, contributionType);
            }
        }

        // chromium's method
        private static void GrowAffectedSizeByPlannedIncrease(GridTrackSizer.Track track, GridTrackSizer.GridItemContributionType
             contributionType) {
            track.isInfinityGrowable = false;
            float plannedIncrease = track.plannedIncrease;
            // Only grow sets that accommodated a grid item.
            if (JavaUtil.FloatCompare(plannedIncrease, -1f) == 0) {
                return;
            }
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS: {
                    track.baseSize += plannedIncrease;
                    track.EnsureGrowthLimitIsNotLessThanBaseSize();
                    return;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS: {
                    // Mark any tracks whose growth limit changed from infinite to finite in
                    // this step as infinitely growable for the next step.
                    track.isInfinityGrowable = JavaUtil.FloatCompare(track.growthLimit, -1f) == 0;
                    track.growthLimit = track.DefiniteGrowthLimit() + plannedIncrease;
                    return;
                }

                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS: {
                    track.growthLimit = track.DefiniteGrowthLimit() + plannedIncrease;
                    return;
                }
            }
        }

        // FOR_FREE_SPACE not reachable here
        // Follow the definitions from https://drafts.csswg.org/css-grid-2/#extra-space;
        // notice that this method replaces the notion of "tracks" with "sets".
        // chromium's method
        private void DistributeExtraSpaceToTracks(float extraSpace, float flexFactorSum, GridTrackSizer.GridItemContributionType
             contributionType, IList<GridTrackSizer.Track> tracksToGrow, IList<GridTrackSizer.Track> tracksToGrowBeyondLimits
            , bool isEqualDistribution) {
            if (JavaUtil.FloatCompare(extraSpace, -1f) == 0) {
                // Infinite extra space should only happen when distributing free space at
                // the maximize tracks step; in such case, we can simplify this method by
                // "filling" every track base size up to their growth limit.
                foreach (GridTrackSizer.Track track in tracksToGrow) {
                    track.incurredIncrease = GrowthPotentialForSet(track, contributionType, false);
                }
                return;
            }
            int growableTrackCount = 0;
            foreach (GridTrackSizer.Track track in tracksToGrow) {
                track.incurredIncrease = 0;
                // From the first note in https://drafts.csswg.org/css-grid-2/#extra-space:
                //   If the affected size was a growth limit and the track is not marked
                //   "infinitely growable", then each item-incurred increase will be zero.
                //
                // When distributing space to growth limits, we need to increase each track
                // up to its 'fit-content' limit. However, because of the note above, first
                // we should only grow tracks marked as "infinitely growable" up to limits
                // and then grow all affected tracks beyond limits.
                //
                // We can correctly resolve every scenario by doing a single sort of
                // |sets_to_grow|, purposely ignoring the "infinitely growable" flag, then
                // filtering out sets that won't take a share of the extra space at each
                // step; for base sizes this is not required, but if there are no tracks
                // with growth potential > 0, we can optimize by not sorting the sets.
                if (JavaUtil.FloatCompare(GrowthPotentialForSet(track, contributionType, false), 0) != 0) {
                    growableTrackCount++;
                }
            }
            float shareRatioSum = isEqualDistribution ? growableTrackCount : flexFactorSum;
            // We will sort the tracks by growth potential in non-decreasing order to
            // distribute space up to limits; notice that if we start distributing space
            // equally among all tracks we will eventually reach the limit of a track or
            // run out of space to distribute. If the former scenario happens, it should
            // be easy to see that the group of tracks that will reach its limit first
            // will be that with the least growth potential. Otherwise, if tracks in such
            // group does not reach their limit, every upcoming track with greater growth
            // potential must be able to increase its size by the same amount.
            if (growableTrackCount != 0 || IsDistributionForGrowthLimits(contributionType)) {
                // iText's comment: in chromium CompareTracksByGrowthPotential is lambda,
                // iText's comment: but for porting purpose lambda was extracted to a class
                // Only sort for equal distributions; since the growth potential of any
                // flexible set is infinite, they don't require comparing.
                if (JavaUtil.FloatCompare(flexFactorSum, 0) == 0) {
                    JavaCollectionsUtil.Sort(tracksToGrow, new GridTrackSizer.CompareTracksByGrowthPotential(this, contributionType
                        ));
                }
            }
            // iText's comment: ExtraSpaceShare lambda was replaced with static method extraSpaceShare to resolve issue with working on java and porting
            // Distribute space up to limits:
            //   - For base sizes, grow the base size up to the growth limit.
            //   - For growth limits, the only case where a growth limit should grow at
            //   this step is when its set has already been marked "infinitely growable".
            //   Increase the growth limit up to the 'fit-content' argument (if any); note
            //   that these arguments could prevent this step to fulfill the entirety of
            //   the extra space and further distribution would be needed.
            foreach (GridTrackSizer.Track track in tracksToGrow) {
                // Break early if there are no further tracks to grow.
                if (growableTrackCount == 0) {
                    break;
                }
                // iText's comment: java doesn't allow change variables inside lambda, it is why ExtraSpaceShareFunctionParams was introduced
                GridTrackSizer.ExtraSpaceShareFunctionParams changedParams = new GridTrackSizer.ExtraSpaceShareFunctionParams
                    (growableTrackCount, shareRatioSum, extraSpace);
                track.incurredIncrease = ExtraSpaceShare(track, GrowthPotentialForSet(track, contributionType, false), isEqualDistribution
                    , changedParams);
                growableTrackCount = changedParams.growableTrackCount;
                shareRatioSum = changedParams.shareRatioSum;
                extraSpace = changedParams.extraSpace;
            }
            // Distribute space beyond limits:
            //   - For base sizes, every affected track can grow indefinitely.
            //   - For growth limits, grow tracks up to their 'fit-content' argument.
            if (tracksToGrowBeyondLimits != null && JavaUtil.FloatCompare(extraSpace, 0f) != 0) {
                foreach (GridTrackSizer.Track track in tracksToGrowBeyondLimits) {
                    // iText's comment: BeyondLimitsGrowthPotential function was replaced by 1 line of code
                    // For growth limits, ignore the "infinitely growable" flag and grow all
                    // affected tracks up to their 'fit-content' argument (note that
                    // |GrowthPotentialForSet| already accounts for it).
                    float beyondLimitsGrowthPotential = IsDistributionForGrowthLimits(contributionType) ? GrowthPotentialForSet
                        (track, contributionType, true) : -1;
                    if (JavaUtil.FloatCompare(beyondLimitsGrowthPotential, 0) != 0) {
                        growableTrackCount++;
                    }
                }
                shareRatioSum = growableTrackCount;
                foreach (GridTrackSizer.Track track in tracksToGrowBeyondLimits) {
                    // Break early if there are no further tracks to grow.
                    if (growableTrackCount == 0) {
                        break;
                    }
                    // iText's comment: BeyondLimitsGrowthPotential function was replaced by 1 line of code
                    // For growth limits, ignore the "infinitely growable" flag and grow all
                    // affected tracks up to their 'fit-content' argument (note that
                    // |GrowthPotentialForSet| already accounts for it).
                    float beyondLimitsGrowthPotential = IsDistributionForGrowthLimits(contributionType) ? GrowthPotentialForSet
                        (track, contributionType, true) : -1;
                    // iText's comment: java doesn't allow change variables inside lambda, it is why ExtraSpaceShareFunctionParams was introduced
                    GridTrackSizer.ExtraSpaceShareFunctionParams changedParams = new GridTrackSizer.ExtraSpaceShareFunctionParams
                        (growableTrackCount, shareRatioSum, extraSpace);
                    track.incurredIncrease = ExtraSpaceShare(track, beyondLimitsGrowthPotential, isEqualDistribution, changedParams
                        );
                    growableTrackCount = changedParams.growableTrackCount;
                    shareRatioSum = changedParams.shareRatioSum;
                    extraSpace = changedParams.extraSpace;
                }
            }
        }

        // chromium's method
        private static float ExtraSpaceShare(GridTrackSizer.Track track, float growthPotential, bool isEqualDistribution
            , GridTrackSizer.ExtraSpaceShareFunctionParams changedParams) {
            // If this set won't take a share of the extra space, e.g. has zero growth
            // potential, exit so that this set is filtered out of |share_ratio_sum|.
            if (JavaUtil.FloatCompare(growthPotential, 0.0f) == 0) {
                return 0;
            }
            int trackCount = 1;
            float trackShareRatio = isEqualDistribution ? 1 : track.GetFlexFactor();
            // Since |share_ratio_sum| can be greater than the wtf_size_t limit, cap the
            // value of |set_share_ratio| to prevent overflows.
            if (trackShareRatio > changedParams.shareRatioSum) {
                trackShareRatio = changedParams.shareRatioSum;
            }
            float extraSpaceShare;
            if (JavaUtil.FloatCompare(trackShareRatio, changedParams.shareRatioSum) == 0) {
                // If this set's share ratio and the remaining ratio sum are the same, it
                // means that this set will receive all of the remaining space. Hence, we
                // can optimize a little by directly using the extra space as this set's
                // share and break early by decreasing the remaining growable track count
                // to 0 (even if there are further growable tracks, since the share ratio
                // sum will be reduced to 0, their space share will also be 0).
                trackCount = changedParams.growableTrackCount;
                extraSpaceShare = changedParams.extraSpace;
            }
            else {
                extraSpaceShare = changedParams.extraSpace * trackShareRatio / changedParams.shareRatioSum;
            }
            if (JavaUtil.FloatCompare(growthPotential, -1f) != 0) {
                extraSpaceShare = Math.Min(extraSpaceShare, growthPotential);
            }
            changedParams.growableTrackCount -= trackCount;
            changedParams.shareRatioSum -= trackShareRatio;
            changedParams.extraSpace -= extraSpaceShare;
            return extraSpaceShare;
        }

        // chromium class
        // iText's comment: in chromium this class is lambda, but for porting purpose lambda was extracted to a class
        private sealed class CompareTracksByGrowthPotential : IComparer<GridTrackSizer.Track> {
            private readonly GridTrackSizer gridTrackSizer;

            private readonly GridTrackSizer.GridItemContributionType contributionType;

            public CompareTracksByGrowthPotential(GridTrackSizer gridTrackSizer, GridTrackSizer.GridItemContributionType
                 contributionType) {
                this.gridTrackSizer = gridTrackSizer;
                this.contributionType = contributionType;
            }

            public int Compare(GridTrackSizer.Track lhs, GridTrackSizer.Track rhs) {
                float growthPotentialLhs = gridTrackSizer.GrowthPotentialForSet(lhs, contributionType, true);
                float growthPotentialRhs = gridTrackSizer.GrowthPotentialForSet(rhs, contributionType, true);
                if (JavaUtil.FloatCompare(growthPotentialLhs, -1f) == 0 || JavaUtil.FloatCompare(growthPotentialRhs, -1f) 
                    == 0) {
                    // At this point we know that there is at least one set with
                    // infinite growth potential; if |a| has a definite value, then |b|
                    // must have infinite growth potential, and thus, |a| < |b|.
                    return JavaUtil.FloatCompare(growthPotentialLhs, -1f) == 0 ? 1 : -1;
                }
                // Straightforward comparison of definite growth potentials.
                return JavaUtil.FloatCompare(growthPotentialLhs, growthPotentialRhs);
            }
        }

        // chromium's method
        private static bool IsDistributionForGrowthLimits(GridTrackSizer.GridItemContributionType contributionType
            ) {
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE: {
                    return false;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS: {
                    return true;
                }

                default: {
                    return false;
                }
            }
        }

        // iText's class which is workaround to port c++ code to java
        private class ExtraSpaceShareFunctionParams {
//\cond DO_NOT_DOCUMENT
            internal int growableTrackCount;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float shareRatioSum;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float extraSpace;
//\endcond

            public ExtraSpaceShareFunctionParams(int growableTrackCount, float shareRatioSum, float extraSpace) {
                this.growableTrackCount = growableTrackCount;
                this.shareRatioSum = shareRatioSum;
                this.extraSpace = extraSpace;
            }
        }

        // We define growth potential = limit - affected size; for base sizes, the limit
        // is its growth limit. For growth limits, the limit is infinity if it is marked
        // as "infinitely growable", and equal to the growth limit otherwise.
        // chromium's method
        // iText's comment: InfinitelyGrowableBehavior enum was replaced by ignoreInfinitelyGrowable boolean
        private float GrowthPotentialForSet(GridTrackSizer.Track track, GridTrackSizer.GridItemContributionType contributionType
            , bool ignoreInfinitelyGrowable) {
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS: {
                    if (JavaUtil.FloatCompare(track.growthLimit, -1f) == 0) {
                        return -1f;
                    }
                    float increasedBaseSize = track.baseSize + track.incurredIncrease;
                    return track.growthLimit - increasedBaseSize;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS: {
                    if (!ignoreInfinitelyGrowable && JavaUtil.FloatCompare(track.growthLimit, -1f) != 0 && !track.isInfinityGrowable
                        ) {
                        // For growth limits, the potential is infinite if its value is infinite
                        // too or if the set is marked as infinitely growable; otherwise, zero.
                        return 0f;
                    }
                    // The max track sizing function of a 'fit-content' track is treated as
                    // 'max-content' until it reaches the limit specified as the 'fit-content'
                    // argument, after which it is treated as having a fixed sizing function
                    // of that argument (with a growth potential of zero).
                    // iText's comment: Case when availableSpace is indefinite and fit-content uses percent is handled in GridTrackSizer constructor
                    if (track.value.GetType() == TemplateValue.ValueType.FIT_CONTENT) {
                        FitContentValue fitContentValue = (FitContentValue)track.value;
                        float growthPotential = fitContentValue.GetMaxSizeForSpace(availableSpace) - track.DefiniteGrowthLimit() -
                             track.incurredIncrease;
                        return Math.Max(growthPotential, 0);
                    }
                    // Otherwise, this set has infinite growth potential.
                    return -1f;
                }

                case GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE: {
                    return track.growthLimit - track.baseSize;
                }

                default: {
                    return 0;
                }
            }
        }

        // https://drafts.csswg.org/css-grid-2/#extra-space
        // Returns true if a track's used size should be consider to grow beyond its limit
        // (see the "Distribute space beyond limits" section); otherwise, false.
        // Note that we will deliberately return false in cases where we don't have a
        // collection of tracks different than "all affected tracks".
        // chromium's method
        private static bool ShouldUsedSizeGrowBeyondLimit(GridTrackSizer.Track track, GridTrackSizer.GridItemContributionType
             contributionType) {
            GridValue maxTrack = track.value;
            if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                maxTrack = ((MinMaxValue)track.value).GetMax();
            }
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS: {
                    // intrinsic max track sizing function
                    return maxTrack.GetType() == TemplateValue.ValueType.MIN_CONTENT || maxTrack.GetType() == TemplateValue.ValueType
                        .MAX_CONTENT || maxTrack.GetType() == TemplateValue.ValueType.AUTO || maxTrack.GetType() == TemplateValue.ValueType
                        .FIT_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS: {
                    // max-content max track sizing function
                    return maxTrack.GetType() == TemplateValue.ValueType.MAX_CONTENT || maxTrack.GetType() == TemplateValue.ValueType
                        .AUTO;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE: {
                    return false;
                }

                default: {
                    return false;
                }
            }
        }

        // Returns true if a track should increase its used size according to the steps in
        // https://drafts.csswg.org/css-grid-2/#algo-spanning-items; false otherwise.
        //
        // chromium's method
        private static bool IsContributionAppliedToTrack(GridTrackSizer.Track track, GridTrackSizer.GridItemContributionType
             contributionType) {
            GridValue minTrack = track.value;
            GridValue maxTrack = track.value;
            if (track.value.GetType() == TemplateValue.ValueType.MINMAX) {
                minTrack = ((MinMaxValue)track.value).GetMin();
                maxTrack = ((MinMaxValue)track.value).GetMax();
            }
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS: {
                    // intrinsic min track sizing function
                    return minTrack.GetType() == TemplateValue.ValueType.MIN_CONTENT || minTrack.GetType() == TemplateValue.ValueType
                        .MAX_CONTENT || minTrack.GetType() == TemplateValue.ValueType.AUTO || minTrack.GetType() == TemplateValue.ValueType
                        .FIT_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS: {
                    // min\max-content min track sizing function
                    return minTrack.GetType() == TemplateValue.ValueType.MIN_CONTENT || minTrack.GetType() == TemplateValue.ValueType
                        .MAX_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS: {
                    // max-content min track sizing function
                    return minTrack.GetType() == TemplateValue.ValueType.MAX_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS: {
                    // intrinsic max track sizing function
                    return maxTrack.GetType() == TemplateValue.ValueType.MIN_CONTENT || maxTrack.GetType() == TemplateValue.ValueType
                        .MAX_CONTENT || maxTrack.GetType() == TemplateValue.ValueType.AUTO || maxTrack.GetType() == TemplateValue.ValueType
                        .FIT_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS: {
                    // max-content max track sizing function
                    return maxTrack.GetType() == TemplateValue.ValueType.MAX_CONTENT || maxTrack.GetType() == TemplateValue.ValueType
                        .AUTO || maxTrack.GetType() == TemplateValue.ValueType.FIT_CONTENT;
                }

                case GridTrackSizer.GridItemContributionType.FOR_FREE_SPACE: {
                    return true;
                }

                default: {
                    return false;
                }
            }
        }

        // Returns the corresponding size to be increased by accommodating a grid item's
        // contribution; for intrinsic min track sizing functions, return the base size.
        // For intrinsic max track sizing functions, return the growth limit.
        // chromium's method
        private static float AffectedSizeForContribution(GridTrackSizer.Track track, GridTrackSizer.GridItemContributionType
             contributionType) {
            switch (contributionType) {
                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_CONTENT_BASED_MINIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MINIMUMS: {
                    return track.baseSize;
                }

                case GridTrackSizer.GridItemContributionType.FOR_INTRINSIC_MAXIMUMS:
                case GridTrackSizer.GridItemContributionType.FOR_MAX_CONTENT_MAXIMUMS: {
                    return track.DefiniteGrowthLimit();
                }

                default: {
                    // FOR_FREE_SPACE not reachable here
                    return 0;
                }
            }
        }

        private enum GridItemContributionType {
            FOR_INTRINSIC_MINIMUMS,
            FOR_CONTENT_BASED_MINIMUMS,
            FOR_MAX_CONTENT_MINIMUMS,
            FOR_INTRINSIC_MAXIMUMS,
            FOR_MAX_CONTENT_MAXIMUMS,
            FOR_FREE_SPACE
        }

        // This enum corresponds to each step used to accommodate grid items across
        // intrinsic tracks according to their min and max track sizing functions
        // chromium's enum
        // iText's method
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

        /// <summary>Calculate min or max contribution of a cell.</summary>
        /// <param name="cell">cell to calculate contribution</param>
        /// <param name="minTypeContribution">type of contribution: min if true, max otherwise</param>
        /// <returns>contribution value</returns>
        private float CalculateMinMaxContribution(GridCell cell, bool minTypeContribution) {
            // iText's method
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
        // iText's class
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
        // iText's class
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

//\cond DO_NOT_DOCUMENT
            internal float plannedIncrease;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float incurredIncrease;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isInfinityGrowable = false;
//\endcond

            public virtual float DefiniteGrowthLimit() {
                // For infinite growth limits, substitute the track's base size.
                return JavaUtil.FloatCompare(growthLimit, -1f) == 0 ? baseSize : growthLimit;
            }

            public virtual bool IsFlexibleTrack() {
                // Flex is replaced by minmax in GridTrackSizer constructor
                if (value.GetType() == TemplateValue.ValueType.MINMAX) {
                    return ((MinMaxValue)value).GetMax().GetType() == TemplateValue.ValueType.FLEX;
                }
                return false;
            }

            public virtual bool HasAutoMax() {
                if (value.GetType() == TemplateValue.ValueType.MINMAX) {
                    return ((MinMaxValue)value).GetMax().GetType() == TemplateValue.ValueType.AUTO;
                }
                return value.GetType() == TemplateValue.ValueType.AUTO;
            }

            public virtual float GetFlexFactor() {
                // Flex is replaced by minmax in GridTrackSizer constructor
                if (value.GetType() == TemplateValue.ValueType.MINMAX) {
                    BreadthValue max = ((MinMaxValue)value).GetMax();
                    return max.GetType() == TemplateValue.ValueType.FLEX ? ((FlexValue)max).GetFlex() : 0f;
                }
                return 0f;
            }

            public virtual void EnsureGrowthLimitIsNotLessThanBaseSize() {
                if (JavaUtil.FloatCompare(growthLimit, -1) != 0 && growthLimit < baseSize) {
                    growthLimit = baseSize;
                }
            }
        }
//\endcond
    }
//\endcond
}
