/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Margincollapse {
    /// <summary>
    /// Rules of the margins collapsing are taken from Mozilla Developer Network:
    /// https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Box_Model/Mastering_margin_collapsing
    /// See also:
    /// https://www.w3.org/TR/CSS2/box.html#collapsing-margins
    /// </summary>
    public class MarginsCollapseHandler {
        private IRenderer renderer;

        private MarginsCollapseInfo collapseInfo;

        private MarginsCollapseInfo childMarginInfo;

        private MarginsCollapseInfo prevChildMarginInfo;

        private int firstNotEmptyKidIndex = 0;

        private int processedChildrenNum = 0;

        private IList<IRenderer> rendererChildren = new List<IRenderer>();

        // Layout box and collapse info are saved before processing the next kid, in order to be able to restore it in case
        // the next kid is not placed. These values are not null only between startChildMarginsHandling and endChildMarginsHandling calls.
        private Rectangle backupLayoutBox;

        private MarginsCollapseInfo backupCollapseInfo;

        private bool lastKidCollapsedAfterHasClearanceApplied;

        public MarginsCollapseHandler(IRenderer renderer, MarginsCollapseInfo marginsCollapseInfo) {
            this.renderer = renderer;
            this.collapseInfo = marginsCollapseInfo != null ? marginsCollapseInfo : new MarginsCollapseInfo();
        }

        public virtual void ProcessFixedHeightAdjustment(float heightDelta) {
            collapseInfo.SetBufferSpaceOnTop(collapseInfo.GetBufferSpaceOnTop() + heightDelta);
            collapseInfo.SetBufferSpaceOnBottom(collapseInfo.GetBufferSpaceOnBottom() + heightDelta);
        }

        public virtual MarginsCollapseInfo StartChildMarginsHandling(IRenderer child, Rectangle layoutBox) {
            if (backupLayoutBox != null) {
                // this should happen only if previous kid was floated
                RestoreLayoutBoxAfterFailedLayoutAttempt(layoutBox);
                RemoveRendererChild(--processedChildrenNum);
                childMarginInfo = null;
            }
            rendererChildren.Add(child);
            int childIndex = processedChildrenNum++;
            // If renderer is floated, prepare layout box as if it was inline,
            // however it will be restored from backup when next kid processing will start.
            bool childIsBlockElement = !RendererIsFloated(child) && IsBlockElement(child);
            backupLayoutBox = layoutBox.Clone();
            backupCollapseInfo = new MarginsCollapseInfo();
            collapseInfo.CopyTo(backupCollapseInfo);
            PrepareBoxForLayoutAttempt(layoutBox, childIndex, childIsBlockElement);
            if (childIsBlockElement) {
                childMarginInfo = CreateMarginsInfoForBlockChild(childIndex);
            }
            return this.childMarginInfo;
        }

        public virtual void ApplyClearance(float clearHeightCorrection) {
            // Actually, clearance is applied only in case margins were not enough,
            // however I wasn't able to notice difference in browsers behaviour.
            // Also, iText behaviour concerning margins self collapsing and clearance differs from browsers in some cases.
            collapseInfo.SetClearanceApplied(true);
            collapseInfo.GetCollapseBefore().JoinMargin(clearHeightCorrection);
        }

        private MarginsCollapseInfo CreateMarginsInfoForBlockChild(int childIndex) {
            bool ignoreChildTopMargin = false;
            // always assume that current child might be the last on this area
            bool ignoreChildBottomMargin = LastChildMarginAdjoinedToParent(renderer);
            if (childIndex == firstNotEmptyKidIndex) {
                ignoreChildTopMargin = FirstChildMarginAdjoinedToParent(renderer);
            }
            MarginsCollapse childCollapseBefore;
            if (childIndex == 0) {
                MarginsCollapse parentCollapseBefore = collapseInfo.GetCollapseBefore();
                childCollapseBefore = ignoreChildTopMargin ? parentCollapseBefore : new MarginsCollapse();
            }
            else {
                MarginsCollapse prevChildCollapseAfter = prevChildMarginInfo != null ? prevChildMarginInfo.GetOwnCollapseAfter
                    () : null;
                childCollapseBefore = prevChildCollapseAfter != null ? prevChildCollapseAfter : new MarginsCollapse();
            }
            MarginsCollapse parentCollapseAfter = collapseInfo.GetCollapseAfter().Clone();
            MarginsCollapse childCollapseAfter = ignoreChildBottomMargin ? parentCollapseAfter : new MarginsCollapse();
            MarginsCollapseInfo childMarginsInfo = new MarginsCollapseInfo(ignoreChildTopMargin, ignoreChildBottomMargin
                , childCollapseBefore, childCollapseAfter);
            if (ignoreChildTopMargin && childIndex == firstNotEmptyKidIndex) {
                childMarginsInfo.SetBufferSpaceOnTop(collapseInfo.GetBufferSpaceOnTop());
            }
            if (ignoreChildBottomMargin) {
                childMarginsInfo.SetBufferSpaceOnBottom(collapseInfo.GetBufferSpaceOnBottom());
            }
            return childMarginsInfo;
        }

        /// <summary>This method shall be called after child occupied area is included into parent occupied area.</summary>
        /// <param name="layoutBox">available area for child and its siblings layout. It might be adjusted inside the method
        ///     </param>
        public virtual void EndChildMarginsHandling(Rectangle layoutBox) {
            int childIndex = processedChildrenNum - 1;
            if (RendererIsFloated(GetRendererChild(childIndex))) {
                return;
            }
            if (childMarginInfo != null) {
                if (firstNotEmptyKidIndex == childIndex && childMarginInfo.IsSelfCollapsing()) {
                    firstNotEmptyKidIndex = childIndex + 1;
                }
                collapseInfo.SetSelfCollapsing(collapseInfo.IsSelfCollapsing() && childMarginInfo.IsSelfCollapsing());
                lastKidCollapsedAfterHasClearanceApplied = childMarginInfo.IsSelfCollapsing() && childMarginInfo.IsClearanceApplied
                    ();
            }
            else {
                lastKidCollapsedAfterHasClearanceApplied = false;
                collapseInfo.SetSelfCollapsing(false);
            }
            if (prevChildMarginInfo != null) {
                FixPrevChildOccupiedArea(childIndex);
                UpdateCollapseBeforeIfPrevKidIsFirstAndSelfCollapsed(prevChildMarginInfo.GetOwnCollapseAfter());
            }
            if (firstNotEmptyKidIndex == childIndex && FirstChildMarginAdjoinedToParent(renderer)) {
                if (!collapseInfo.IsSelfCollapsing()) {
                    GetRidOfCollapseArtifactsAtopOccupiedArea();
                    if (childMarginInfo != null) {
                        ProcessUsedChildBufferSpaceOnTop(layoutBox);
                    }
                }
            }
            prevChildMarginInfo = childMarginInfo;
            childMarginInfo = null;
            backupLayoutBox = null;
            backupCollapseInfo = null;
        }

        public virtual void StartMarginsCollapse(Rectangle parentBBox) {
            collapseInfo.GetCollapseBefore().JoinMargin(DefineTopMarginValueForCollapse(renderer));
            collapseInfo.GetCollapseAfter().JoinMargin(DefineBottomMarginValueForCollapse(renderer));
            if (!FirstChildMarginAdjoinedToParent(renderer)) {
                float topIndent = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
                ApplyTopMargin(parentBBox, topIndent);
            }
            if (!LastChildMarginAdjoinedToParent(renderer)) {
                float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                ApplyBottomMargin(parentBBox, bottomIndent);
            }
            // ignore current margins for now
            IgnoreModelTopMargin(renderer);
            IgnoreModelBottomMargin(renderer);
        }

        public virtual void EndMarginsCollapse(Rectangle layoutBox) {
            if (backupLayoutBox != null) {
                RestoreLayoutBoxAfterFailedLayoutAttempt(layoutBox);
            }
            if (prevChildMarginInfo != null) {
                UpdateCollapseBeforeIfPrevKidIsFirstAndSelfCollapsed(prevChildMarginInfo.GetCollapseAfter());
            }
            bool couldBeSelfCollapsing = iText.Layout.Margincollapse.MarginsCollapseHandler.MarginsCouldBeSelfCollapsing
                (renderer) && !lastKidCollapsedAfterHasClearanceApplied;
            bool blockHasNoKidsWithContent = collapseInfo.IsSelfCollapsing();
            if (FirstChildMarginAdjoinedToParent(renderer)) {
                if (blockHasNoKidsWithContent && !couldBeSelfCollapsing) {
                    AddNotYetAppliedTopMargin(layoutBox);
                }
            }
            collapseInfo.SetSelfCollapsing(collapseInfo.IsSelfCollapsing() && couldBeSelfCollapsing);
            if (!blockHasNoKidsWithContent && lastKidCollapsedAfterHasClearanceApplied) {
                ApplySelfCollapsedKidMarginWithClearance(layoutBox);
            }
            MarginsCollapse ownCollapseAfter;
            bool lastChildMarginJoinedToParent = prevChildMarginInfo != null && prevChildMarginInfo.IsIgnoreOwnMarginBottom
                () && !lastKidCollapsedAfterHasClearanceApplied;
            if (lastChildMarginJoinedToParent) {
                ownCollapseAfter = prevChildMarginInfo.GetOwnCollapseAfter();
            }
            else {
                ownCollapseAfter = new MarginsCollapse();
            }
            ownCollapseAfter.JoinMargin(DefineBottomMarginValueForCollapse(renderer));
            collapseInfo.SetOwnCollapseAfter(ownCollapseAfter);
            if (collapseInfo.IsSelfCollapsing()) {
                if (prevChildMarginInfo != null) {
                    collapseInfo.SetCollapseAfter(prevChildMarginInfo.GetCollapseAfter());
                }
                else {
                    collapseInfo.GetCollapseAfter().JoinMargin(collapseInfo.GetCollapseBefore());
                    collapseInfo.GetOwnCollapseAfter().JoinMargin(collapseInfo.GetCollapseBefore());
                }
                if (!collapseInfo.IsIgnoreOwnMarginBottom() && !collapseInfo.IsIgnoreOwnMarginTop()) {
                    float collapsedMargins = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                    OverrideModelBottomMargin(renderer, collapsedMargins);
                }
            }
            else {
                MarginsCollapse marginsCollapseBefore = collapseInfo.GetCollapseBefore();
                if (!collapseInfo.IsIgnoreOwnMarginTop()) {
                    float collapsedMargins = marginsCollapseBefore.GetCollapsedMarginsSize();
                    OverrideModelTopMargin(renderer, collapsedMargins);
                }
                if (lastChildMarginJoinedToParent) {
                    collapseInfo.SetCollapseAfter(prevChildMarginInfo.GetCollapseAfter());
                }
                if (!collapseInfo.IsIgnoreOwnMarginBottom()) {
                    float collapsedMargins = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                    OverrideModelBottomMargin(renderer, collapsedMargins);
                }
            }
            if (LastChildMarginAdjoinedToParent(renderer) && (prevChildMarginInfo != null || blockHasNoKidsWithContent
                )) {
                // Adjust layout box here in order to make it represent the available area left.
                float collapsedMargins = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                // May be in case of self-collapsed margins it would make more sense to apply this value to topMargin,
                // because that way the layout box would represent the area left after the empty self-collapsed block, not
                // before it. However at the same time any considerations about the layout (i.e. content) area in case
                // of the self-collapsed block seem to be invalid, because self-collapsed block shall have content area
                // of zero height.
                ApplyBottomMargin(layoutBox, collapsedMargins);
            }
        }

        private void UpdateCollapseBeforeIfPrevKidIsFirstAndSelfCollapsed(MarginsCollapse collapseAfter) {
            if (prevChildMarginInfo.IsSelfCollapsing() && prevChildMarginInfo.IsIgnoreOwnMarginTop()) {
                // prevChildMarginInfo.isIgnoreOwnMarginTop() is true only if it's the first kid and is adjoined to parent margin
                collapseInfo.GetCollapseBefore().JoinMargin(collapseAfter);
            }
        }

        private void PrepareBoxForLayoutAttempt(Rectangle layoutBox, int childIndex, bool childIsBlockElement) {
            if (prevChildMarginInfo != null) {
                bool prevChildHasAppliedCollapseAfter = !prevChildMarginInfo.IsIgnoreOwnMarginBottom() && (!prevChildMarginInfo
                    .IsSelfCollapsing() || !prevChildMarginInfo.IsIgnoreOwnMarginTop());
                if (prevChildHasAppliedCollapseAfter) {
                    layoutBox.SetHeight(layoutBox.GetHeight() + prevChildMarginInfo.GetCollapseAfter().GetCollapsedMarginsSize
                        ());
                }
                bool prevChildCanApplyCollapseAfter = !prevChildMarginInfo.IsSelfCollapsing() || !prevChildMarginInfo.IsIgnoreOwnMarginTop
                    ();
                if (!childIsBlockElement && prevChildCanApplyCollapseAfter) {
                    MarginsCollapse ownCollapseAfter = prevChildMarginInfo.GetOwnCollapseAfter();
                    float ownCollapsedMargins = ownCollapseAfter == null ? 0 : ownCollapseAfter.GetCollapsedMarginsSize();
                    layoutBox.SetHeight(layoutBox.GetHeight() - ownCollapsedMargins);
                }
            }
            else {
                if (childIndex > firstNotEmptyKidIndex) {
                    if (LastChildMarginAdjoinedToParent(renderer)) {
                        // restore layout box after inline element
                        // used space shall be always less or equal to collapsedMarginAfter size
                        float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize() - collapseInfo.GetUsedBufferSpaceOnBottom
                            ();
                        collapseInfo.SetBufferSpaceOnBottom(collapseInfo.GetBufferSpaceOnBottom() + collapseInfo.GetUsedBufferSpaceOnBottom
                            ());
                        collapseInfo.SetUsedBufferSpaceOnBottom(0);
                        layoutBox.SetY(layoutBox.GetY() - bottomIndent);
                        layoutBox.SetHeight(layoutBox.GetHeight() + bottomIndent);
                    }
                }
            }
            if (!childIsBlockElement) {
                if (childIndex == firstNotEmptyKidIndex && FirstChildMarginAdjoinedToParent(renderer)) {
                    float topIndent = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
                    ApplyTopMargin(layoutBox, topIndent);
                }
                // if not adjoined - bottom margin have been already applied on startMarginsCollapse
                if (LastChildMarginAdjoinedToParent(renderer)) {
                    float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                    ApplyBottomMargin(layoutBox, bottomIndent);
                }
            }
        }

        private void RestoreLayoutBoxAfterFailedLayoutAttempt(Rectangle layoutBox) {
            layoutBox.SetX(backupLayoutBox.GetX()).SetY(backupLayoutBox.GetY()).SetWidth(backupLayoutBox.GetWidth()).SetHeight
                (backupLayoutBox.GetHeight());
            backupCollapseInfo.CopyTo(collapseInfo);
            backupLayoutBox = null;
            backupCollapseInfo = null;
        }

        private void ApplyTopMargin(Rectangle box, float topIndent) {
            float bufferLeftoversOnTop = collapseInfo.GetBufferSpaceOnTop() - topIndent;
            float usedTopBuffer = bufferLeftoversOnTop > 0 ? topIndent : collapseInfo.GetBufferSpaceOnTop();
            collapseInfo.SetUsedBufferSpaceOnTop(usedTopBuffer);
            SubtractUsedTopBufferFromBottomBuffer(usedTopBuffer);
            if (bufferLeftoversOnTop >= 0) {
                collapseInfo.SetBufferSpaceOnTop(bufferLeftoversOnTop);
                box.MoveDown(topIndent);
            }
            else {
                box.MoveDown(collapseInfo.GetBufferSpaceOnTop());
                collapseInfo.SetBufferSpaceOnTop(0);
                box.SetHeight(box.GetHeight() + bufferLeftoversOnTop);
            }
        }

        private void ApplyBottomMargin(Rectangle box, float bottomIndent) {
            // Here we don't subtract used buffer space from topBuffer, because every kid is assumed to be
            // the last one on the page, and so every kid always has parent's bottom buffer, however only the true last kid
            // uses it for real. Also, bottom margin are always applied after top margins, so it doesn't matter anyway.
            float bottomIndentLeftovers = bottomIndent - collapseInfo.GetBufferSpaceOnBottom();
            if (bottomIndentLeftovers < 0) {
                collapseInfo.SetUsedBufferSpaceOnBottom(bottomIndent);
                collapseInfo.SetBufferSpaceOnBottom(-bottomIndentLeftovers);
            }
            else {
                collapseInfo.SetUsedBufferSpaceOnBottom(collapseInfo.GetBufferSpaceOnBottom());
                collapseInfo.SetBufferSpaceOnBottom(0);
                box.SetY(box.GetY() + bottomIndentLeftovers);
                box.SetHeight(box.GetHeight() - bottomIndentLeftovers);
            }
        }

        private void ProcessUsedChildBufferSpaceOnTop(Rectangle layoutBox) {
            float childUsedBufferSpaceOnTop = childMarginInfo.GetUsedBufferSpaceOnTop();
            if (childUsedBufferSpaceOnTop > 0) {
                if (childUsedBufferSpaceOnTop > collapseInfo.GetBufferSpaceOnTop()) {
                    childUsedBufferSpaceOnTop = collapseInfo.GetBufferSpaceOnTop();
                }
                collapseInfo.SetBufferSpaceOnTop(collapseInfo.GetBufferSpaceOnTop() - childUsedBufferSpaceOnTop);
                collapseInfo.SetUsedBufferSpaceOnTop(childUsedBufferSpaceOnTop);
                // usage of top buffer space on child is expressed by moving layout box down instead of making it smaller,
                // so in order to process next kids correctly, we need to move parent layout box also
                layoutBox.MoveDown(childUsedBufferSpaceOnTop);
                SubtractUsedTopBufferFromBottomBuffer(childUsedBufferSpaceOnTop);
            }
        }

        private void SubtractUsedTopBufferFromBottomBuffer(float usedTopBuffer) {
            if (collapseInfo.GetBufferSpaceOnTop() > collapseInfo.GetBufferSpaceOnBottom()) {
                float bufferLeftoversOnTop = collapseInfo.GetBufferSpaceOnTop() - usedTopBuffer;
                if (bufferLeftoversOnTop < collapseInfo.GetBufferSpaceOnBottom()) {
                    collapseInfo.SetBufferSpaceOnBottom(bufferLeftoversOnTop);
                }
            }
            else {
                collapseInfo.SetBufferSpaceOnBottom(collapseInfo.GetBufferSpaceOnBottom() - usedTopBuffer);
            }
        }

        private void FixPrevChildOccupiedArea(int childIndex) {
            IRenderer prevRenderer = GetRendererChild(childIndex - 1);
            Rectangle bBox = prevRenderer.GetOccupiedArea().GetBBox();
            bool prevChildHasAppliedCollapseAfter = !prevChildMarginInfo.IsIgnoreOwnMarginBottom() && (!prevChildMarginInfo
                .IsSelfCollapsing() || !prevChildMarginInfo.IsIgnoreOwnMarginTop());
            if (prevChildHasAppliedCollapseAfter) {
                float bottomMargin = prevChildMarginInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                bBox.SetHeight(bBox.GetHeight() - bottomMargin);
                bBox.MoveUp(bottomMargin);
                IgnoreModelBottomMargin(prevRenderer);
            }
            bool isNotBlockChild = !IsBlockElement(GetRendererChild(childIndex));
            bool prevChildCanApplyCollapseAfter = !prevChildMarginInfo.IsSelfCollapsing() || !prevChildMarginInfo.IsIgnoreOwnMarginTop
                ();
            if (isNotBlockChild && prevChildCanApplyCollapseAfter) {
                float ownCollapsedMargins = prevChildMarginInfo.GetOwnCollapseAfter().GetCollapsedMarginsSize();
                bBox.SetHeight(bBox.GetHeight() + ownCollapsedMargins);
                bBox.MoveDown(ownCollapsedMargins);
                OverrideModelBottomMargin(prevRenderer, ownCollapsedMargins);
            }
        }

        private void AddNotYetAppliedTopMargin(Rectangle layoutBox) {
            // normally, space for margins is added when content is met, however if all kids were self-collapsing (i.e.
            // had no content) or if there were no kids, we need to add it when no more adjoining margins will be met
            float indentTop = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
            renderer.GetOccupiedArea().GetBBox().MoveDown(indentTop);
            // Even though all kids have been already drawn, we still need to adjust layout box here
            // in order to make it represent the available area for element content (e.g. needed for fixed height elements).
            ApplyTopMargin(layoutBox, indentTop);
        }

        // Actually, this should be taken into account when layouting a kid and assuming it's the last one on page.
        // However it's not feasible, because
        // - before kid layout, we don't know if it's self-collapsing or if we have applied clearance to it;
        // - this might be very difficult to correctly change kid and parent occupy area, based on if it's
        // the last kid on page or not;
        // - in the worst case scenario (which is kinda rare) page last kid (self-collapsed and with clearance)
        // margin applying would result in margins page overflow, which will not be visible except
        // margins would be visually less than expected.
        private void ApplySelfCollapsedKidMarginWithClearance(Rectangle layoutBox) {
            // Self-collapsed kid margin with clearance will not be applied to parent top margin
            // if parent is not self-collapsing. It's self-collapsing kid, thus we just can
            // add this area to occupied area of parent.
            float clearedKidMarginWithClearance = prevChildMarginInfo.GetOwnCollapseAfter().GetCollapsedMarginsSize();
            renderer.GetOccupiedArea().GetBBox().IncreaseHeight(clearedKidMarginWithClearance).MoveDown(clearedKidMarginWithClearance
                );
            layoutBox.DecreaseHeight(clearedKidMarginWithClearance);
        }

        private IRenderer GetRendererChild(int index) {
            return rendererChildren[index];
        }

        private IRenderer RemoveRendererChild(int index) {
            return rendererChildren.JRemoveAt(index);
        }

        private void GetRidOfCollapseArtifactsAtopOccupiedArea() {
            Rectangle bBox = renderer.GetOccupiedArea().GetBBox();
            bBox.DecreaseHeight(collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize());
        }

        private static bool MarginsCouldBeSelfCollapsing(IRenderer renderer) {
            return !(renderer is TableRenderer) && !RendererIsFloated(renderer) && !HasBottomBorders(renderer) && !HasTopBorders
                (renderer) && !HasBottomPadding(renderer) && !HasTopPadding(renderer) && !HasPositiveHeight(renderer) 
                && 
                        // inline block
                        !(IsBlockElement(renderer) && renderer is AbstractRenderer && ((AbstractRenderer)renderer).GetParent() is 
                LineRenderer);
        }

        private static bool FirstChildMarginAdjoinedToParent(IRenderer parent) {
            return !BlockFormattingContextUtil.IsRendererCreateBfc(parent) && !(parent is TableRenderer) && !HasTopBorders
                (parent) && !HasTopPadding(parent);
        }

        private static bool LastChildMarginAdjoinedToParent(IRenderer parent) {
            return !BlockFormattingContextUtil.IsRendererCreateBfc(parent) && !(parent is TableRenderer) && !HasBottomBorders
                (parent) && !HasBottomPadding(parent) && !HasHeightProp(parent);
        }

        private static bool IsBlockElement(IRenderer renderer) {
            return renderer is BlockRenderer || renderer is TableRenderer;
        }

        private static bool HasHeightProp(IRenderer renderer) {
            // in mozilla and chrome height always prevents margins collapse in all cases.
            return renderer.GetModelElement().HasProperty(Property.HEIGHT);
        }

        // "min-height" property affects margins collapse differently in chrome and mozilla. While in chrome, this property
        // seems to not have any effect on collapsing margins at all (child margins collapse with parent margins even if
        // there is a considerable space between them due to the min-height property on parent), mozilla behaves better
        // and collapse happens only in case min-height of parent is less than actual height of the content and therefore
        // collapse really should happen. However even in mozilla, if parent has min-height which is a little bigger then
        // it's content actual height and margin collapse doesn't happen, in this case the child's margin is not shown fully however.
        //
        // || styles.containsKey(CssConstants.MIN_HEIGHT)
        // "max-height" doesn't seem to affect margins collapse in any way at least in chrome.
        // In mozilla it affects collapsing when parent's max-height is less than children actual height,
        // in this case collapse doesn't happen. However, at the moment in iText we won't show anything at all if
        // kid's height is bigger than parent's max-height, therefore this logic is irrelevant now anyway.
        //
        // || (includingMaxHeight && styles.containsKey(CssConstants.MAX_HEIGHT));
        private static bool HasPositiveHeight(IRenderer renderer) {
            float height = renderer.GetOccupiedArea().GetBBox().GetHeight();
            if (height == 0) {
                UnitValue heightPropVal = renderer.GetProperty<UnitValue>(Property.HEIGHT);
                UnitValue minHeightPropVal = renderer.GetProperty<UnitValue>(Property.MIN_HEIGHT);
                height = minHeightPropVal != null ? (float)minHeightPropVal.GetValue() : heightPropVal != null ? (float)heightPropVal
                    .GetValue() : 0;
            }
            return height > 0;
        }

        private static bool HasTopPadding(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.HasPadding(renderer, Property.PADDING_TOP);
        }

        private static bool HasBottomPadding(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.HasPadding(renderer, Property.PADDING_BOTTOM);
        }

        private static bool HasTopBorders(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.HasBorders(renderer, Property.BORDER_TOP);
        }

        private static bool HasBottomBorders(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.HasBorders(renderer, Property.BORDER_BOTTOM);
        }

        private static bool RendererIsFloated(IRenderer renderer) {
            if (renderer == null) {
                return false;
            }
            FloatPropertyValue? floatPropertyValue = renderer.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            return floatPropertyValue != null && !floatPropertyValue.Equals(FloatPropertyValue.NONE);
        }

        private static float DefineTopMarginValueForCollapse(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.DefineMarginValueForCollapse(renderer, Property.
                MARGIN_TOP);
        }

        private static void IgnoreModelTopMargin(IRenderer renderer) {
            iText.Layout.Margincollapse.MarginsCollapseHandler.OverrideModelTopMargin(renderer, 0f);
        }

        private static void OverrideModelTopMargin(IRenderer renderer, float collapsedMargins) {
            iText.Layout.Margincollapse.MarginsCollapseHandler.OverrideModelMargin(renderer, Property.MARGIN_TOP, collapsedMargins
                );
        }

        private static float DefineBottomMarginValueForCollapse(IRenderer renderer) {
            return iText.Layout.Margincollapse.MarginsCollapseHandler.DefineMarginValueForCollapse(renderer, Property.
                MARGIN_BOTTOM);
        }

        private static void IgnoreModelBottomMargin(IRenderer renderer) {
            iText.Layout.Margincollapse.MarginsCollapseHandler.OverrideModelBottomMargin(renderer, 0f);
        }

        private static void OverrideModelBottomMargin(IRenderer renderer, float collapsedMargins) {
            iText.Layout.Margincollapse.MarginsCollapseHandler.OverrideModelMargin(renderer, Property.MARGIN_BOTTOM, collapsedMargins
                );
        }

        private static float DefineMarginValueForCollapse(IRenderer renderer, int property) {
            UnitValue marginUV = renderer.GetModelElement().GetProperty<UnitValue>(property);
            if (null != marginUV && !marginUV.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Margincollapse.MarginsCollapseHandler));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , property));
            }
            return marginUV != null && !(renderer is CellRenderer) ? marginUV.GetValue() : 0;
        }

        private static void OverrideModelMargin(IRenderer renderer, int property, float collapsedMargins) {
            renderer.SetProperty(property, UnitValue.CreatePointValue(collapsedMargins));
        }

        private static bool HasPadding(IRenderer renderer, int property) {
            UnitValue padding = renderer.GetModelElement().GetProperty<UnitValue>(property);
            if (null != padding && !padding.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Margincollapse.MarginsCollapseHandler));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , property));
            }
            return padding != null && padding.GetValue() > 0;
        }

        private static bool HasBorders(IRenderer renderer, int property) {
            IPropertyContainer modelElement = renderer.GetModelElement();
            return modelElement.HasProperty(property) || modelElement.HasProperty(Property.BORDER);
        }
    }
}
