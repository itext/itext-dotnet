/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class FlexContainerRenderer : DivRenderer {
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

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.FlexContainerRenderer((Div)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            Rectangle layoutContextRectangle = layoutContext.GetArea().GetBBox();
            foreach (IRenderer childRenderer in this.GetChildRenderers()) {
                if (childRenderer is AbstractRenderer) {
                    AbstractRenderer abstractChildRenderer = (AbstractRenderer)childRenderer;
                    abstractChildRenderer.SetParent(this);
                }
            }
            float? containerWidth = RetrieveWidth(layoutContextRectangle.GetWidth());
            if (containerWidth == null) {
                containerWidth = layoutContextRectangle.GetWidth();
            }
            float? containerHeight = RetrieveHeight();
            if (containerHeight == null) {
                containerHeight = layoutContextRectangle.GetHeight();
            }
            lines = FlexUtil.CalculateChildrenRectangles(new Rectangle((float)containerWidth, (float)containerHeight), 
                this);
            IList<UnitValue> previousWidths = new List<UnitValue>();
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    Rectangle rectangleWithoutBordersMarginsPaddings = itemInfo.GetRenderer().ApplyMarginsBordersPaddings(itemInfo
                        .GetRectangle().Clone(), false);
                    previousWidths.Add(itemInfo.GetRenderer().GetProperty<UnitValue>(Property.WIDTH));
                    itemInfo.GetRenderer().SetProperty(Property.WIDTH, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
                        .GetWidth()));
                    itemInfo.GetRenderer().SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
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
            IRenderer childRenderer = childRenderers[childPos];
            bool forcedPlacement = true.Equals(this.GetProperty<bool?>(Property.FORCED_PLACEMENT));
            bool metChildRenderer = false;
            foreach (IList<FlexItemInfo> line in lines) {
                metChildRenderer = metChildRenderer || line.Any((flexItem) => flexItem.GetRenderer() == childRenderer);
                foreach (FlexItemInfo itemInfo in line) {
                    if (metChildRenderer && !forcedPlacement) {
                        overflowRenderer.childRenderers.Add(itemInfo.GetRenderer());
                        itemInfo.GetRenderer().SetParent(overflowRenderer);
                    }
                    else {
                        splitRenderer.childRenderers.Add(itemInfo.GetRenderer());
                        itemInfo.GetRenderer().SetParent(splitRenderer);
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
            bool keepTogether = IsKeepTogether();
            AbstractRenderer[] splitAndOverflowRenderers = CreateSplitAndOverflowRenderers(childPos, result.GetStatus(
                ), result, waitingFloatsSplitRenderers, waitingOverflowFloatRenderers);
            AbstractRenderer splitRenderer = splitAndOverflowRenderers[0];
            AbstractRenderer overflowRenderer = splitAndOverflowRenderers[1];
            overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            if (IsRelativePosition() && !positionedRenderers.IsEmpty()) {
                overflowRenderer.positionedRenderers = new List<IRenderer>(positionedRenderers);
            }
            // TODO DEVSIX-5086 When flex-wrap will be fully supported we'll need to update height on split
            if (keepTogether) {
                splitRenderer = null;
                overflowRenderer.childRenderers.Clear();
                overflowRenderer.childRenderers = new List<IRenderer>(childRenderers);
            }
            CorrectFixedLayout(layoutBox);
            ApplyAbsolutePositionIfNeeded(layoutContext);
            if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) || wasHeightClipped) {
                if (splitRenderer != null) {
                    splitRenderer.childRenderers = new List<IRenderer>(childRenderers);
                    foreach (IRenderer childRenderer in splitRenderer.childRenderers) {
                        childRenderer.SetParent(splitRenderer);
                    }
                }
                return new LayoutResult(LayoutResult.FULL, result.GetOccupiedArea(), splitRenderer, null, null);
            }
            else {
                ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                ApplyMargins(occupiedArea.GetBBox(), true);
                if (splitRenderer == null || splitRenderer.childRenderers.IsEmpty()) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, overflowRenderer, result.GetCauseOfNothing()).SetAreaBreak
                        (result.GetAreaBreak());
                }
                else {
                    return new LayoutResult(LayoutResult.PARTIAL, layoutContext.GetArea(), splitRenderer, overflowRenderer, null
                        ).SetAreaBreak(result.GetAreaBreak());
                }
            }
        }

        internal override bool StopLayoutingChildrenIfChildResultNotFull(LayoutResult returnResult) {
            return returnResult.GetStatus() != LayoutResult.FULL;
        }

        internal override void DecreaseLayoutBoxAfterChildPlacement(Rectangle layoutBox, LayoutResult result, IRenderer
             childRenderer) {
            // TODO DEVSIX-5086 When flex-wrap will be fully supported
            //  we'll need to decrease layout box with respect to the lines
            layoutBox.DecreaseWidth(result.GetOccupiedArea().GetBBox().GetWidth());
            layoutBox.MoveRight(result.GetOccupiedArea().GetBBox().GetWidth());
        }

        private void FindMinMaxWidthIfCorrespondingPropertiesAreNotSet(MinMaxWidth minMaxWidth, AbstractWidthHandler
             minMaxWidthHandler) {
            // TODO DEVSIX-5086 When flex-wrap will be fully supported we'll find min/max width with respect to the lines
            foreach (IRenderer childRenderer in childRenderers) {
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
    }
}
