/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System.Collections.Generic;
using System.Linq;
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
                    FillSplitOverflowRenderersForPartialResult(splitRenderer, overflowRenderer, line, childRenderer, childResult
                        );
                }
                else {
                    foreach (FlexItemInfo itemInfo in line) {
                        if (metChildRenderer && !forcedPlacement) {
                            overflowRenderer.AddChildRenderer(itemInfo.GetRenderer());
                        }
                        else {
                            splitRenderer.AddChildRenderer(itemInfo.GetRenderer());
                        }
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
            // TODO DEVSIX-5086 When flex-wrap will be fully supported we'll need to update height on split
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
            // TODO DEVSIX-5086 When flex-wrap will be fully supported
            //  we'll need to decrease layout box with respect to the lines
            layoutBox.DecreaseWidth(result.GetOccupiedArea().GetBBox().GetRight() - layoutBox.GetLeft());
            layoutBox.SetX(result.GetOccupiedArea().GetBBox().GetRight());
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

        private void FillSplitOverflowRenderersForPartialResult(AbstractRenderer splitRenderer, AbstractRenderer overflowRenderer
            , IList<FlexItemInfo> line, IRenderer childRenderer, LayoutResult childResult) {
            // If we split, we remove (override) Property.ALIGN_ITEMS for the overflow renderer.
            // because we have to layout the remaining part at the top of the layout context.
            // TODO DEVSIX-5086 When flex-wrap will be fully supported we'll need to reconsider this.
            // The question is what should be set/calculated for the next line
            overflowRenderer.SetProperty(Property.ALIGN_ITEMS, null);
            float occupiedSpace = 0;
            bool metChildRendererInLine = false;
            foreach (FlexItemInfo itemInfo in line) {
                // Split the line
                if (itemInfo.GetRenderer() == childRenderer) {
                    metChildRendererInLine = true;
                    if (childResult.GetSplitRenderer() != null) {
                        splitRenderer.AddChildRenderer(childResult.GetSplitRenderer());
                    }
                    if (childResult.GetOverflowRenderer() != null) {
                        overflowRenderer.AddChildRenderer(childResult.GetOverflowRenderer());
                    }
                }
                else {
                    if (metChildRendererInLine) {
                        // Process all following renderers in the current line
                        // We have to layout them to understand what goes where
                        Rectangle neighbourBbox = GetOccupiedAreaBBox().Clone();
                        // Move bbox by occupied space
                        neighbourBbox.SetX(neighbourBbox.GetX() + occupiedSpace);
                        neighbourBbox.SetWidth(itemInfo.GetRectangle().GetWidth());
                        // Y of the renderer has been already calculated, move bbox accordingly
                        neighbourBbox.SetY(neighbourBbox.GetY() - itemInfo.GetRectangle().GetY());
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
                    }
                }
                // X is nonzero only for the 1st renderer in line serving for alignment adjustments
                occupiedSpace += itemInfo.GetRectangle().GetX() + itemInfo.GetRectangle().GetWidth();
            }
        }

        private void FindMinMaxWidthIfCorrespondingPropertiesAreNotSet(MinMaxWidth minMaxWidth, AbstractWidthHandler
             minMaxWidthHandler) {
            // TODO DEVSIX-5086 When flex-wrap will be fully supported we'll find min/max width with respect to the lines
            SetThisAsParent(GetChildRenderers());
            foreach (IRenderer childRenderer in GetChildRenderers()) {
                MinMaxWidth childMinMaxWidth;
                childRenderer.SetParent(this);
                if (childRenderer is AbstractRenderer) {
                    childMinMaxWidth = ((AbstractRenderer)childRenderer).GetMinMaxWidth();
                }
                else {
                    childMinMaxWidth = MinMaxWidthUtils.CountDefaultMinMaxWidth(childRenderer);
                }
                minMaxWidthHandler.UpdateMaxChildWidth(childMinMaxWidth.GetMaxWidth() + minMaxWidth.GetMaxWidth());
                minMaxWidthHandler.UpdateMinChildWidth(childMinMaxWidth.GetMinWidth() + minMaxWidth.GetMinWidth());
            }
        }

        private static void AddSimulateDiv(AbstractRenderer overflowRenderer, float width) {
            IRenderer fakeOverflowRenderer = new DivRenderer(new Div().SetMinWidth(width).SetMaxWidth(width));
            overflowRenderer.AddChildRenderer(fakeOverflowRenderer);
        }
    }
}
