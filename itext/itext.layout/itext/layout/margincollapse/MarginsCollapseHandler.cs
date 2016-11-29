using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Margincollapse {
    /// <summary>
    /// Rules of the margins collapsing are taken from Mozilla Developer Network:
    /// https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Box_Model/Mastering_margin_collapsing
    /// </summary>
    public class MarginsCollapseHandler {
        private IRenderer renderer;

        private MarginsCollapseInfo collapseInfo;

        private MarginsCollapseInfo childMarginInfo;

        private MarginsCollapseInfo prevChildMarginInfo;

        private int firstNotEmptyKidIndex = 0;

        private int processedChildrenNum = 0;

        private IList<IRenderer> rendererChildren;

        public MarginsCollapseHandler(IRenderer renderer, MarginsCollapseInfo marginsCollapseInfo) {
            this.renderer = renderer;
            this.collapseInfo = marginsCollapseInfo != null ? marginsCollapseInfo : new MarginsCollapseInfo();
        }

        public virtual void ProcessFixedHeightAdjustment(float heightDelta) {
            collapseInfo.SetBufferSpace(collapseInfo.GetBufferSpace() + heightDelta);
        }

        public virtual MarginsCollapseInfo StartChildMarginsHandling(IRenderer child, Rectangle layoutBox) {
            if (rendererChildren == null) {
                rendererChildren = new List<IRenderer>();
            }
            rendererChildren.Add(child);
            return StartChildMarginsHandling(processedChildrenNum++, layoutBox);
        }

        public virtual MarginsCollapseInfo StartChildMarginsHandling(int childIndex, Rectangle layoutBox) {
            prevChildMarginInfo = childMarginInfo;
            childMarginInfo = null;
            IRenderer child = GetRendererChild(childIndex);
            bool childIsBlockElement = IsBlockElement(child);
            PrepareBoxForLayoutAttempt(layoutBox, childIndex, childIsBlockElement);
            if (childIsBlockElement) {
                childMarginInfo = CreateMarginsInfoForBlockChild(childIndex);
            }
            return this.childMarginInfo;
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
                childMarginsInfo.SetBufferSpace(collapseInfo.GetBufferSpace());
            }
            return childMarginsInfo;
        }

        public virtual void EndChildMarginsHandling() {
            EndChildMarginsHandling(processedChildrenNum - 1, null);
        }

        public virtual void EndChildMarginsHandling(int childIndex, Rectangle layoutBox) {
            if (childMarginInfo != null) {
                if (firstNotEmptyKidIndex == childIndex && childMarginInfo.IsSelfCollapsing()) {
                    firstNotEmptyKidIndex = childIndex + 1;
                }
                collapseInfo.SetSelfCollapsing(collapseInfo.IsSelfCollapsing() && childMarginInfo.IsSelfCollapsing());
            }
            else {
                collapseInfo.SetSelfCollapsing(false);
            }
            if (firstNotEmptyKidIndex == childIndex && FirstChildMarginAdjoinedToParent(renderer)) {
                if (!collapseInfo.IsSelfCollapsing()) {
                    GetRidOfCollapseArtifactsAtopOccupiedArea();
                    if (childMarginInfo != null) {
                        float buffSpaceDiff = collapseInfo.GetBufferSpace() - childMarginInfo.GetBufferSpace();
                        if (buffSpaceDiff > 0) {
                            layoutBox.MoveDown(buffSpaceDiff);
                        }
                    }
                }
            }
            if (prevChildMarginInfo != null) {
                FixPrevChildOccupiedArea(childIndex);
                if (prevChildMarginInfo.IsSelfCollapsing() && prevChildMarginInfo.IsIgnoreOwnMarginTop()) {
                    collapseInfo.GetCollapseBefore().JoinMargin(prevChildMarginInfo.GetOwnCollapseAfter());
                }
            }
            prevChildMarginInfo = null;
        }

        // a sign that last kid processing finished successfully
        public virtual void StartMarginsCollapse(Rectangle parentBBox) {
            collapseInfo.GetCollapseBefore().JoinMargin(GetModelTopMargin(renderer));
            collapseInfo.GetCollapseAfter().JoinMargin(GetModelBottomMargin(renderer));
            if (!FirstChildMarginAdjoinedToParent(renderer)) {
                float topIndent = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
                AdjustBoxPosAndHeight(parentBBox, topIndent);
            }
            if (!LastChildMarginAdjoinedToParent(renderer)) {
                float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                ApplyBottomMargin(parentBBox, bottomIndent);
            }
            // ignore current margins for now
            IgnoreModelTopMargin(renderer);
            IgnoreModelBottomMargin(renderer);
        }

        public virtual void EndMarginsCollapse() {
            if (prevChildMarginInfo != null) {
                // last kid processing finished with NOTHING
                childMarginInfo = prevChildMarginInfo;
            }
            if (childMarginInfo != null && childMarginInfo.IsSelfCollapsing() && childMarginInfo.IsIgnoreOwnMarginTop(
                )) {
                collapseInfo.GetCollapseBefore().JoinMargin(childMarginInfo.GetCollapseAfter());
            }
            bool couldBeSelfCollapsing = iText.Layout.Margincollapse.MarginsCollapseHandler.MarginsCouldBeSelfCollapsing
                (renderer);
            if (FirstChildMarginAdjoinedToParent(renderer)) {
                if (collapseInfo.IsSelfCollapsing() && !couldBeSelfCollapsing) {
                    float indentTop = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
                    renderer.GetOccupiedArea().GetBBox().MoveDown(indentTop);
                }
            }
            collapseInfo.SetSelfCollapsing(collapseInfo.IsSelfCollapsing() && couldBeSelfCollapsing);
            MarginsCollapse ownCollapseAfter;
            bool lastChildMarginJoinedToParent = childMarginInfo != null && childMarginInfo.IsIgnoreOwnMarginBottom();
            if (lastChildMarginJoinedToParent) {
                ownCollapseAfter = childMarginInfo.GetOwnCollapseAfter();
            }
            else {
                ownCollapseAfter = new MarginsCollapse();
            }
            ownCollapseAfter.JoinMargin(GetModelBottomMargin(renderer));
            collapseInfo.SetOwnCollapseAfter(ownCollapseAfter);
            if (collapseInfo.IsSelfCollapsing()) {
                if (childMarginInfo != null) {
                    collapseInfo.SetCollapseAfter(childMarginInfo.GetCollapseAfter());
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
                    collapseInfo.SetCollapseAfter(childMarginInfo.GetCollapseAfter());
                }
                if (!collapseInfo.IsIgnoreOwnMarginBottom()) {
                    float collapsedMargins = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                    OverrideModelBottomMargin(renderer, collapsedMargins);
                }
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
                    float ownCollapsedMargins = prevChildMarginInfo.GetOwnCollapseAfter().GetCollapsedMarginsSize();
                    layoutBox.SetHeight(layoutBox.GetHeight() - ownCollapsedMargins);
                }
            }
            else {
                if (childIndex > firstNotEmptyKidIndex) {
                    if (LastChildMarginAdjoinedToParent(renderer)) {
                        // restore layout box after inline element
                        float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                        layoutBox.SetY(layoutBox.GetY() - bottomIndent);
                        layoutBox.SetHeight(layoutBox.GetHeight() + bottomIndent);
                    }
                }
            }
            if (!childIsBlockElement) {
                if (childIndex == firstNotEmptyKidIndex && FirstChildMarginAdjoinedToParent(renderer)) {
                    float topIndent = collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize();
                    AdjustBoxPosAndHeight(layoutBox, topIndent);
                }
                if (LastChildMarginAdjoinedToParent(renderer)) {
                    float bottomIndent = collapseInfo.GetCollapseAfter().GetCollapsedMarginsSize();
                    ApplyBottomMargin(layoutBox, bottomIndent);
                }
            }
        }

        private void AdjustBoxPosAndHeight(Rectangle box, float topIndent) {
            float bufferLeftovers = collapseInfo.GetBufferSpace() - topIndent;
            if (bufferLeftovers >= 0) {
                collapseInfo.SetBufferSpace(bufferLeftovers);
                box.MoveDown(topIndent);
            }
            else {
                box.MoveDown(collapseInfo.GetBufferSpace());
                collapseInfo.SetBufferSpace(0);
                box.SetHeight(box.GetHeight() + bufferLeftovers);
            }
        }

        private void ApplyBottomMargin(Rectangle box, float bottomIndent) {
            box.SetY(box.GetY() + bottomIndent);
            box.SetHeight(box.GetHeight() - bottomIndent);
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

        private IRenderer GetRendererChild(int index) {
            if (rendererChildren != null) {
                return rendererChildren[index];
            }
            return this.renderer.GetChildRenderers()[index];
        }

        private void GetRidOfCollapseArtifactsAtopOccupiedArea() {
            Rectangle bBox = renderer.GetOccupiedArea().GetBBox();
            bBox.SetHeight(bBox.GetHeight() - collapseInfo.GetCollapseBefore().GetCollapsedMarginsSize());
        }

        private static bool MarginsCouldBeSelfCollapsing(IRenderer renderer) {
            return !HasBottomBorders(renderer) && !HasTopBorders(renderer) && !HasBottomPadding(renderer) && !HasTopPadding
                (renderer) && !HasPositiveHeight(renderer);
        }

        private static bool FirstChildMarginAdjoinedToParent(IRenderer parent) {
            return !(parent is RootRenderer) && !HasTopBorders(parent) && !HasTopPadding(parent);
        }

        private static bool LastChildMarginAdjoinedToParent(IRenderer parent) {
            return !(parent is RootRenderer) && !HasBottomBorders(parent) && !HasBottomPadding(parent) && !HasHeightProp
                (parent);
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
            return renderer.GetOccupiedArea().GetBBox().GetHeight() > 0;
        }

        private static bool HasTopPadding(IRenderer renderer) {
            float? padding = renderer.GetModelElement().GetProperty<float?>(Property.PADDING_TOP);
            return padding != null && padding > 0;
        }

        private static bool HasBottomPadding(IRenderer renderer) {
            float? padding = renderer.GetModelElement().GetProperty<float?>(Property.PADDING_TOP);
            return padding != null && padding > 0;
        }

        private static bool HasTopBorders(IRenderer renderer) {
            IPropertyContainer modelElement = renderer.GetModelElement();
            return modelElement.HasProperty(Property.BORDER_TOP) || modelElement.HasProperty(Property.BORDER);
        }

        private static bool HasBottomBorders(IRenderer renderer) {
            IPropertyContainer modelElement = renderer.GetModelElement();
            return modelElement.HasProperty(Property.BORDER_BOTTOM) || modelElement.HasProperty(Property.BORDER);
        }

        private static float GetModelTopMargin(IRenderer renderer) {
            float? margin = renderer.GetModelElement().GetProperty<float?>(Property.MARGIN_TOP);
            return margin != null ? (float)margin : 0;
        }

        private static void IgnoreModelTopMargin(IRenderer renderer) {
            renderer.SetProperty(Property.MARGIN_TOP, 0);
        }

        private static void OverrideModelTopMargin(IRenderer renderer, float collapsedMargins) {
            renderer.SetProperty(Property.MARGIN_TOP, collapsedMargins);
        }

        private static float GetModelBottomMargin(IRenderer renderer) {
            float? margin = renderer.GetModelElement().GetProperty<float?>(Property.MARGIN_BOTTOM);
            return margin != null ? (float)margin : 0;
        }

        private static void IgnoreModelBottomMargin(IRenderer renderer) {
            renderer.SetProperty(Property.MARGIN_BOTTOM, 0);
        }

        private static void OverrideModelBottomMargin(IRenderer renderer, float collapsedMargins) {
            renderer.SetProperty(Property.MARGIN_BOTTOM, collapsedMargins);
        }
    }
}
