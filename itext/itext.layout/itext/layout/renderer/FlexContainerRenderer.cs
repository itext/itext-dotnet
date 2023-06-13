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
using System;
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class FlexContainerRenderer : DivRenderer {
        /* Used for caching purposes in FlexUtil
        * We couldn't find the real use case when this map contains more than 1 entry
        * but let it still be a map to be on a safe(r) side
        * Map mainSize (always width in our case) - hypotheticalCrossSize
        */
        private readonly IDictionary<float, float?> hypotheticalCrossSizes = new Dictionary<float, float?>();

        private IList<IList<FlexItemInfo>> lines;

        private IFlexItemMainDirector flexItemMainDirector = null;

        /// <summary>Creates a FlexContainerRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.Div"/>
        /// which this object should manage
        /// </param>
        public FlexContainerRenderer(Div modelElement)
            : base(modelElement) {
        }

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="FlexContainerRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.FlexContainerRenderer), this.GetType
                ());
            return new iText.Layout.Renderer.FlexContainerRenderer((Div)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            Rectangle layoutContextRectangle = layoutContext.GetArea().GetBBox();
            SetThisAsParent(GetChildRenderers());
            lines = FlexUtil.CalculateChildrenRectangles(layoutContextRectangle, this);
            ApplyWrapReverse();
            IList<IRenderer> renderers = GetFlexItemMainDirector().ApplyDirection(lines);
            RemoveAllChildRenderers(GetChildRenderers());
            AddAllChildRenderers(renderers);
            IList<UnitValue> previousWidths = new List<UnitValue>();
            IList<UnitValue> previousHeights = new List<UnitValue>();
            IList<UnitValue> previousMinHeights = new List<UnitValue>();
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    Rectangle rectangleWithoutBordersMarginsPaddings;
                    if (AbstractRenderer.IsBorderBoxSizing(itemInfo.GetRenderer())) {
                        rectangleWithoutBordersMarginsPaddings = itemInfo.GetRenderer().ApplyMargins(itemInfo.GetRectangle().Clone
                            (), false);
                    }
                    else {
                        rectangleWithoutBordersMarginsPaddings = itemInfo.GetRenderer().ApplyMarginsBordersPaddings(itemInfo.GetRectangle
                            ().Clone(), false);
                    }
                    previousWidths.Add(itemInfo.GetRenderer().GetProperty<UnitValue>(Property.WIDTH));
                    previousHeights.Add(itemInfo.GetRenderer().GetProperty<UnitValue>(Property.HEIGHT));
                    previousMinHeights.Add(itemInfo.GetRenderer().GetProperty<UnitValue>(Property.MIN_HEIGHT));
                    itemInfo.GetRenderer().SetProperty(Property.WIDTH, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
                        .GetWidth()));
                    itemInfo.GetRenderer().SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
                        .GetHeight()));
                    // TODO DEVSIX-1895 Once the ticket is closed, there will be no need in setting min-height
                    // In case element takes less vertical space than expected, we need to make sure
                    // it is extended to the height predicted by the algo
                    itemInfo.GetRenderer().SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
                        .GetHeight()));
                    // Property.HORIZONTAL_ALIGNMENT mustn't play, in flex container items are aligned
                    // using justify-content and align-items
                    itemInfo.GetRenderer().SetProperty(Property.HORIZONTAL_ALIGNMENT, null);
                }
            }
            LayoutResult result = base.Layout(layoutContext);
            // We must set back widths of the children because multiple layouts are possible
            // If flex-grow is less than 1, layout algorithm increases the width of the element based on the initial width
            // And if we would not set back widths, every layout flex-item width will grow.
            int counter = 0;
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    itemInfo.GetRenderer().SetProperty(Property.WIDTH, previousWidths[counter]);
                    itemInfo.GetRenderer().SetProperty(Property.HEIGHT, previousHeights[counter]);
                    itemInfo.GetRenderer().SetProperty(Property.MIN_HEIGHT, previousMinHeights[counter]);
                    ++counter;
                }
            }
            return result;
        }

        /// <summary><inheritDoc/></summary>
        public override MinMaxWidth GetMinMaxWidth() {
            MinMaxWidth minMaxWidth = new MinMaxWidth(CalculateAdditionalWidth(this));
            AbstractWidthHandler minMaxWidthHandler = new MaxMaxWidthHandler(minMaxWidth);
            if (!SetMinMaxWidthBasedOnFixedWidth(minMaxWidth)) {
                float? minWidth = HasAbsoluteUnitValue(Property.MIN_WIDTH) ? RetrieveMinWidth(0) : null;
                float? maxWidth = HasAbsoluteUnitValue(Property.MAX_WIDTH) ? RetrieveMaxWidth(0) : null;
                if (minWidth == null || maxWidth == null) {
                    FindMinMaxWidthIfCorrespondingPropertiesAreNotSet(minMaxWidth, minMaxWidthHandler);
                }
                if (minWidth != null) {
                    minMaxWidth.SetChildrenMinWidth((float)minWidth);
                }
                // if max-width was defined explicitly, it shouldn't be overwritten
                if (maxWidth == null) {
                    if (minMaxWidth.GetChildrenMinWidth() > minMaxWidth.GetChildrenMaxWidth()) {
                        minMaxWidth.SetChildrenMaxWidth(minMaxWidth.GetChildrenMinWidth());
                    }
                }
                else {
                    minMaxWidth.SetChildrenMaxWidth((float)maxWidth);
                }
            }
            if (this.GetPropertyAsFloat(Property.ROTATION_ANGLE) != null) {
                return RotationUtils.CountRotationMinMaxWidth(minMaxWidth, this);
            }
            return minMaxWidth;
        }

        internal virtual IFlexItemMainDirector GetFlexItemMainDirector() {
            if (flexItemMainDirector == null) {
                flexItemMainDirector = CreateMainDirector();
            }
            return flexItemMainDirector;
        }

        /// <summary>Check if flex container is wrapped reversely.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if flex-wrap property is set to wrap-reverse,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        internal virtual bool IsWrapReverse() {
            return FlexWrapPropertyValue.WRAP_REVERSE == this.GetProperty<FlexWrapPropertyValue?>(Property.FLEX_WRAP, 
                null);
        }

        /// <summary><inheritDoc/></summary>
        internal override AbstractRenderer[] CreateSplitAndOverflowRenderers(int childPos, int layoutStatus, LayoutResult
             childResult, IDictionary<int, IRenderer> waitingFloatsSplitRenderers, IList<IRenderer> waitingOverflowFloatRenderers
            ) {
            AbstractRenderer splitRenderer = CreateSplitRenderer(layoutStatus);
            AbstractRenderer overflowRenderer = CreateOverflowRenderer(layoutStatus);
            IRenderer childRenderer = GetChildRenderers()[childPos];
            bool forcedPlacement = true.Equals(this.GetProperty<bool?>(Property.FORCED_PLACEMENT));
            bool metChildRenderer = false;
            foreach (IList<FlexItemInfo> line in lines) {
                bool isSplitLine = line.Any((flexItem) => flexItem.GetRenderer() == childRenderer);
                metChildRenderer = metChildRenderer || isSplitLine;
                // If the renderer to split is in the current line
                if (isSplitLine && !forcedPlacement && layoutStatus == LayoutResult.PARTIAL) {
                    // It has sense to call it also for LayoutResult.NOTHING. And then try to layout remaining renderers
                    // in line inside fillSplitOverflowRenderersForPartialResult to see if some of them can be left or
                    // partially left on the first page (in split renderer). But it's not that easy.
                    // So currently, if the 1st not fully layouted renderer is layouted with LayoutResult.NOTHING,
                    // the whole line is moved to the next page (overflow renderer).
                    FillSplitOverflowRenderersForPartialResult(splitRenderer, overflowRenderer, line, childRenderer, childResult
                        );
                    GetFlexItemMainDirector().ApplyDirectionForLine(overflowRenderer.GetChildRenderers());
                }
                else {
                    IList<IRenderer> overflowRendererChildren = new List<IRenderer>();
                    foreach (FlexItemInfo itemInfo in line) {
                        if (metChildRenderer && !forcedPlacement) {
                            overflowRendererChildren.Add(itemInfo.GetRenderer());
                        }
                        else {
                            splitRenderer.AddChildRenderer(itemInfo.GetRenderer());
                        }
                    }
                    GetFlexItemMainDirector().ApplyDirectionForLine(overflowRendererChildren);
                    // If wrapped reversely we should add a line into beginning to correctly recalculate
                    // and inverse lines while layouting overflowRenderer.
                    if (IsWrapReverse()) {
                        overflowRenderer.AddAllChildRenderers(0, overflowRendererChildren);
                    }
                    else {
                        overflowRenderer.AddAllChildRenderers(overflowRendererChildren);
                    }
                }
            }
            overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            return new AbstractRenderer[] { splitRenderer, overflowRenderer };
        }

        internal override LayoutResult ProcessNotFullChildResult(LayoutContext layoutContext, IDictionary<int, IRenderer
            > waitingFloatsSplitRenderers, IList<IRenderer> waitingOverflowFloatRenderers, bool wasHeightClipped, 
            IList<Rectangle> floatRendererAreas, bool marginsCollapsingEnabled, float clearHeightCorrection, Border
            [] borders, UnitValue[] paddings, IList<Rectangle> areas, int currentAreaPos, Rectangle layoutBox, ICollection
            <Rectangle> nonChildFloatingRendererAreas, IRenderer causeOfNothing, bool anythingPlaced, int childPos
            , LayoutResult result) {
            bool keepTogether = IsKeepTogether(causeOfNothing);
            if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) || wasHeightClipped) {
                AbstractRenderer splitRenderer = keepTogether ? null : CreateSplitRenderer(result.GetStatus());
                if (splitRenderer != null) {
                    splitRenderer.SetChildRenderers(GetChildRenderers());
                }
                return new LayoutResult(LayoutResult.FULL, GetOccupiedAreaInCaseNothingWasWrappedWithFull(result, splitRenderer
                    ), splitRenderer, null, null);
            }
            AbstractRenderer[] splitAndOverflowRenderers = CreateSplitAndOverflowRenderers(childPos, result.GetStatus(
                ), result, waitingFloatsSplitRenderers, waitingOverflowFloatRenderers);
            AbstractRenderer splitRenderer_1 = splitAndOverflowRenderers[0];
            AbstractRenderer overflowRenderer = splitAndOverflowRenderers[1];
            overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            UpdateHeightsOnSplit(wasHeightClipped, splitRenderer_1, overflowRenderer);
            if (IsRelativePosition() && !positionedRenderers.IsEmpty()) {
                overflowRenderer.positionedRenderers = new List<IRenderer>(positionedRenderers);
            }
            if (keepTogether) {
                splitRenderer_1 = null;
                overflowRenderer.SetChildRenderers(GetChildRenderers());
            }
            CorrectFixedLayout(layoutBox);
            ApplyAbsolutePositionIfNeeded(layoutContext);
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (splitRenderer_1 == null || splitRenderer_1.GetChildRenderers().IsEmpty()) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, overflowRenderer, result.GetCauseOfNothing()).SetAreaBreak
                    (result.GetAreaBreak());
            }
            else {
                return new LayoutResult(LayoutResult.PARTIAL, layoutContext.GetArea(), splitRenderer_1, overflowRenderer, 
                    null).SetAreaBreak(result.GetAreaBreak());
            }
        }

        // TODO DEVSIX-5238 Consider this fix (perhaps it should be improved or unified) while working on the ticket
        internal virtual LayoutArea GetOccupiedAreaInCaseNothingWasWrappedWithFull(LayoutResult result, IRenderer 
            splitRenderer) {
            return null != result.GetOccupiedArea() ? result.GetOccupiedArea() : splitRenderer.GetOccupiedArea();
        }

        internal override bool StopLayoutingChildrenIfChildResultNotFull(LayoutResult returnResult) {
            return returnResult.GetStatus() != LayoutResult.FULL;
        }

        /// <summary><inheritDoc/></summary>
        internal override void RecalculateOccupiedAreaAfterChildLayout(Rectangle resultBBox, float? blockMaxHeight
            ) {
            Rectangle oldBBox = occupiedArea.GetBBox().Clone();
            Rectangle recalculatedRectangle = Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), resultBBox);
            occupiedArea.GetBBox().SetY(recalculatedRectangle.GetY());
            occupiedArea.GetBBox().SetHeight(recalculatedRectangle.GetHeight());
            if (oldBBox.GetTop() < occupiedArea.GetBBox().GetTop()) {
                occupiedArea.GetBBox().DecreaseHeight(occupiedArea.GetBBox().GetTop() - oldBBox.GetTop());
            }
            if (null != blockMaxHeight && occupiedArea.GetBBox().GetHeight() > ((float)blockMaxHeight)) {
                occupiedArea.GetBBox().MoveUp(occupiedArea.GetBBox().GetHeight() - ((float)blockMaxHeight));
                occupiedArea.GetBBox().SetHeight((float)blockMaxHeight);
            }
        }

        internal override MarginsCollapseInfo StartChildMarginsHandling(IRenderer childRenderer, Rectangle layoutBox
            , MarginsCollapseHandler marginsCollapseHandler) {
            return marginsCollapseHandler.StartChildMarginsHandling(null, layoutBox);
        }

        internal override void DecreaseLayoutBoxAfterChildPlacement(Rectangle layoutBox, LayoutResult result, IRenderer
             childRenderer) {
            layoutBox.DecreaseWidth(result.GetOccupiedArea().GetBBox().GetRight() - layoutBox.GetLeft());
            layoutBox.SetX(result.GetOccupiedArea().GetBBox().GetRight());
            IList<FlexItemInfo> line = FindLine(childRenderer);
            bool isLastInLine = childRenderer.Equals(line[line.Count - 1].GetRenderer());
            // If it was the last renderer in line we have to go to the next line (row)
            if (isLastInLine) {
                float minBottom = layoutBox.GetTop();
                float minLeft = layoutBox.GetLeft();
                float commonWidth = 0;
                foreach (FlexItemInfo item in line) {
                    minLeft = Math.Min(minLeft, item.GetRenderer().GetOccupiedArea().GetBBox().GetLeft() - item.GetRectangle()
                        .GetLeft());
                    minBottom = Math.Min(minBottom, item.GetRenderer().GetOccupiedArea().GetBBox().GetBottom());
                    commonWidth += item.GetRectangle().GetLeft() + item.GetRenderer().GetOccupiedArea().GetBBox().GetWidth();
                }
                layoutBox.SetX(minLeft);
                layoutBox.IncreaseWidth(commonWidth);
                layoutBox.DecreaseHeight(layoutBox.GetTop() - minBottom);
            }
        }

        internal override Rectangle RecalculateLayoutBoxBeforeChildLayout(Rectangle layoutBox, IRenderer childRenderer
            , Rectangle initialLayoutBox) {
            Rectangle layoutBoxCopy = layoutBox.Clone();
            if (childRenderer is AbstractRenderer) {
                FlexItemInfo childFlexItemInfo = FindFlexItemInfo((AbstractRenderer)childRenderer);
                if (childFlexItemInfo != null) {
                    layoutBoxCopy.DecreaseWidth(childFlexItemInfo.GetRectangle().GetX());
                    layoutBoxCopy.MoveRight(childFlexItemInfo.GetRectangle().GetX());
                    layoutBoxCopy.DecreaseHeight(childFlexItemInfo.GetRectangle().GetY());
                }
            }
            return layoutBoxCopy;
        }

        internal override void HandleForcedPlacement(bool anythingPlaced) {
        }

        // In (horizontal) FlexContainerRenderer Property.FORCED_PLACEMENT is still valid for other children
        // so do nothing
        internal virtual void SetHypotheticalCrossSize(float? mainSize, float? hypotheticalCrossSize) {
            hypotheticalCrossSizes.Put(mainSize.Value, hypotheticalCrossSize);
        }

        internal virtual float? GetHypotheticalCrossSize(float? mainSize) {
            return hypotheticalCrossSizes.Get(mainSize.Value);
        }

        /// <summary>Apply wrap-reverse property.</summary>
        private void ApplyWrapReverse() {
            if (!IsWrapReverse()) {
                return;
            }
            JavaCollectionsUtil.Reverse(lines);
            IList<IRenderer> reorderedRendererList = new List<IRenderer>();
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    reorderedRendererList.Add(itemInfo.GetRenderer());
                }
            }
            RemoveAllChildRenderers(GetChildRenderers());
            AddAllChildRenderers(reorderedRendererList);
        }

        private FlexItemInfo FindFlexItemInfo(AbstractRenderer renderer) {
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    if (itemInfo.GetRenderer().Equals(renderer)) {
                        return itemInfo;
                    }
                }
            }
            return null;
        }

        private IList<FlexItemInfo> FindLine(IRenderer renderer) {
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    if (itemInfo.GetRenderer().Equals(renderer)) {
                        return line;
                    }
                }
            }
            return null;
        }

        internal override void FixOccupiedAreaIfOverflowedX(OverflowPropertyValue? overflowX, Rectangle layoutBox) {
            // TODO DEVSIX-5087 Support overflow visible/hidden property correctly
            return;
        }

        /// <summary><inheritDoc/></summary>
        public override void AddChild(IRenderer renderer) {
            // TODO DEVSIX-5087 Since overflow-fit is an internal iText overflow value, we do not need to support if
            // for html/css objects, such as flex. As for now we will set VISIBLE by default, however, while working
            // on the ticket one may come to some more satifactory approach
            renderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            base.AddChild(renderer);
        }

        private static void AddSimulateDiv(AbstractRenderer overflowRenderer, float width) {
            IRenderer fakeOverflowRenderer = new DivRenderer(new Div().SetMinWidth(width).SetMaxWidth(width));
            overflowRenderer.AddChildRenderer(fakeOverflowRenderer);
        }

        private void FillSplitOverflowRenderersForPartialResult(AbstractRenderer splitRenderer, AbstractRenderer overflowRenderer
            , IList<FlexItemInfo> line, IRenderer childRenderer, LayoutResult childResult) {
            float occupiedSpace = 0;
            float maxHeightInLine = 0;
            bool metChildRendererInLine = false;
            foreach (FlexItemInfo itemInfo in line) {
                // Split the line
                if (itemInfo.GetRenderer() == childRenderer) {
                    metChildRendererInLine = true;
                    if (childResult.GetSplitRenderer() != null) {
                        splitRenderer.AddChildRenderer(childResult.GetSplitRenderer());
                    }
                    if (childResult.GetOverflowRenderer() != null) {
                        // Get rid of cross alignment for item with partial result
                        childResult.GetOverflowRenderer().SetProperty(Property.ALIGN_SELF, AlignmentPropertyValue.START);
                        overflowRenderer.AddChildRenderer(childResult.GetOverflowRenderer());
                    }
                    // Count the height allowed for the items after the one which was partially layouted
                    maxHeightInLine = Math.Max(maxHeightInLine, itemInfo.GetRectangle().GetY() + itemInfo.GetRenderer().GetOccupiedAreaBBox
                        ().GetHeight());
                }
                else {
                    if (metChildRendererInLine) {
                        // Process all following renderers in the current line
                        // We have to layout them to understand what goes where
                        // x - space occupied by all preceding items
                        // y - y of current occupied area
                        // width - item width
                        // height - allowed height for the item
                        Rectangle neighbourBbox = new Rectangle(GetOccupiedAreaBBox().GetX() + occupiedSpace, GetOccupiedAreaBBox(
                            ).GetY(), itemInfo.GetRectangle().GetWidth(), maxHeightInLine - itemInfo.GetRectangle().GetY());
                        LayoutResult neighbourLayoutResult = itemInfo.GetRenderer().Layout(new LayoutContext(new LayoutArea(childResult
                            .GetOccupiedArea().GetPageNumber(), neighbourBbox)));
                        // Handle result
                        if (neighbourLayoutResult.GetStatus() == LayoutResult.PARTIAL && neighbourLayoutResult.GetSplitRenderer() 
                            != null) {
                            splitRenderer.AddChildRenderer(neighbourLayoutResult.GetSplitRenderer());
                        }
                        else {
                            if (neighbourLayoutResult.GetStatus() == LayoutResult.FULL) {
                                splitRenderer.AddChildRenderer(itemInfo.GetRenderer());
                            }
                        }
                        // LayoutResult.NOTHING
                        if (neighbourLayoutResult.GetOverflowRenderer() != null) {
                            if (neighbourLayoutResult.GetStatus() == LayoutResult.PARTIAL) {
                                // Get rid of cross alignment for item with partial result
                                neighbourLayoutResult.GetOverflowRenderer().SetProperty(Property.ALIGN_SELF, AlignmentPropertyValue.START);
                            }
                            overflowRenderer.AddChildRenderer(neighbourLayoutResult.GetOverflowRenderer());
                        }
                        else {
                            // Here we might need to still occupy the space on overflow renderer
                            AddSimulateDiv(overflowRenderer, itemInfo.GetRectangle().GetWidth());
                        }
                    }
                    else {
                        // Process all preceeding renderers in the current line
                        // They all were layouted as FULL so add them into split renderer
                        splitRenderer.AddChildRenderer(itemInfo.GetRenderer());
                        // But we also need to occupy the space on overflow renderer
                        AddSimulateDiv(overflowRenderer, itemInfo.GetRectangle().GetWidth());
                        // Count the height allowed for the items after the one which was partially layouted
                        maxHeightInLine = Math.Max(maxHeightInLine, itemInfo.GetRectangle().GetY() + itemInfo.GetRenderer().GetOccupiedAreaBBox
                            ().GetHeight());
                    }
                }
                // X is nonzero only for the 1st renderer in line serving for alignment adjustments
                occupiedSpace += itemInfo.GetRectangle().GetX() + itemInfo.GetRectangle().GetWidth();
            }
        }

        private void FindMinMaxWidthIfCorrespondingPropertiesAreNotSet(MinMaxWidth minMaxWidth, AbstractWidthHandler
             minMaxWidthHandler) {
            float initialMinWidth = minMaxWidth.GetChildrenMinWidth();
            float initialMaxWidth = minMaxWidth.GetChildrenMaxWidth();
            if (lines == null || lines.Count == 1) {
                FindMinMaxWidth(initialMinWidth, initialMaxWidth, minMaxWidthHandler, GetChildRenderers());
            }
            else {
                foreach (IList<FlexItemInfo> line in lines) {
                    IList<IRenderer> childRenderers = new List<IRenderer>();
                    foreach (FlexItemInfo itemInfo in line) {
                        childRenderers.Add(itemInfo.GetRenderer());
                    }
                    FindMinMaxWidth(initialMinWidth, initialMaxWidth, minMaxWidthHandler, childRenderers);
                }
            }
        }

        private void FindMinMaxWidth(float initialMinWidth, float initialMaxWidth, AbstractWidthHandler minMaxWidthHandler
            , IList<IRenderer> childRenderers) {
            float maxWidth = initialMaxWidth;
            float minWidth = initialMinWidth;
            foreach (IRenderer childRenderer in childRenderers) {
                MinMaxWidth childMinMaxWidth;
                childRenderer.SetParent(this);
                if (childRenderer is AbstractRenderer) {
                    childMinMaxWidth = ((AbstractRenderer)childRenderer).GetMinMaxWidth();
                }
                else {
                    childMinMaxWidth = MinMaxWidthUtils.CountDefaultMinMaxWidth(childRenderer);
                }
                maxWidth += childMinMaxWidth.GetMaxWidth();
                minWidth += childMinMaxWidth.GetMinWidth();
            }
            minMaxWidthHandler.UpdateMaxChildWidth(maxWidth);
            minMaxWidthHandler.UpdateMinChildWidth(minWidth);
        }

        /// <summary>Check if flex container direction is row reverse.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if flex-direction property is set to row-reverse,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        private bool IsRowReverse() {
            return FlexDirectionPropertyValue.ROW_REVERSE == this.GetProperty<FlexDirectionPropertyValue?>(Property.FLEX_DIRECTION
                , null);
        }

        private IFlexItemMainDirector CreateMainDirector() {
            if (FlexDirectionPropertyValue.COLUMN == this.GetProperty<FlexDirectionPropertyValue?>(Property.FLEX_DIRECTION
                )) {
                return new TopToBottomFlexItemMainDirector();
            }
            else {
                bool isRtlDirection = BaseDirection.RIGHT_TO_LEFT == this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION
                    , null);
                flexItemMainDirector = IsRowReverse() ^ isRtlDirection ? (IFlexItemMainDirector)new RtlFlexItemMainDirector
                    () : new LtrFlexItemMainDirector();
                return flexItemMainDirector;
            }
        }
    }
}
