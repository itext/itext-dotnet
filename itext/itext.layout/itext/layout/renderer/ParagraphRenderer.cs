/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// object. It will draw the glyphs of the textual content on the
    /// <see cref="DrawContext"/>
    /// .
    /// </summary>
    public class ParagraphRenderer : BlockRenderer {
        protected internal float previousDescent = 0;

        protected internal IList<LineRenderer> lines = null;

        /// <summary>Creates a ParagraphRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.Paragraph"/>
        /// which this object should manage
        /// </param>
        public ParagraphRenderer(Paragraph modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            OverrideHeightProperties();
            bool wasHeightClipped = false;
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            bool anythingPlaced = false;
            bool firstLineInBox = true;
            LineRenderer currentRenderer = (LineRenderer)new LineRenderer().SetParent(this);
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = GetProperty(Property.FLOAT);
            float? blockWidth = RetrieveWidth(parentBBox.GetWidth());
            if (floatPropertyValue != null) {
                if (floatPropertyValue.Equals(FloatPropertyValue.LEFT)) {
                    SetProperty(Property.HORIZONTAL_ALIGNMENT, HorizontalAlignment.LEFT);
                }
                else {
                    if (floatPropertyValue.Equals(FloatPropertyValue.RIGHT)) {
                        SetProperty(Property.HORIZONTAL_ALIGNMENT, HorizontalAlignment.RIGHT);
                    }
                }
            }
            if (0 == childRenderers.Count) {
                anythingPlaced = true;
                currentRenderer = null;
            }
            bool isPositioned = IsPositioned();
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
                marginsCollapseHandler.StartMarginsCollapse(parentBBox);
            }
            Border[] borders = GetBorders();
            float[] paddings = GetPaddings();
            float additionalWidth = ApplyBordersPaddingsMargins(parentBBox, borders, paddings);
            if (blockWidth != null && (blockWidth < parentBBox.GetWidth() || isPositioned)) {
                parentBBox.SetWidth((float)blockWidth);
            }
            if (floatPropertyValue != null && !FloatPropertyValue.NONE.Equals(floatPropertyValue)) {
                Rectangle layoutBox = layoutContext.GetArea().GetBBox();
                AdjustBlockRendererAccordingToFloatRenderers(floatRendererAreas, parentBBox, layoutBox.GetX() + layoutBox.
                    GetWidth(), blockWidth, marginsCollapseHandler);
            }
            MinMaxWidth minMaxWidth = new MinMaxWidth(additionalWidth, layoutContext.GetArea().GetBBox().GetWidth());
            AbstractWidthHandler widthHandler = new MaxMaxWidthHandler(minMaxWidth);
            float? blockMaxHeight = RetrieveMaxHeight();
            if (null != blockMaxHeight && parentBBox.GetHeight() > blockMaxHeight) {
                float heightDelta = parentBBox.GetHeight() - (float)blockMaxHeight;
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler.ProcessFixedHeightAdjustment(heightDelta);
                }
                parentBBox.MoveUp(heightDelta).SetHeight((float)blockMaxHeight);
                wasHeightClipped = true;
            }
            float clearHeightCorrection = CalculateClearHeightCorrection(floatRendererAreas, parentBBox);
            IList<Rectangle> areas;
            if (isPositioned) {
                areas = JavaCollectionsUtil.SingletonList(parentBBox);
            }
            else {
                areas = InitElementAreas(new LayoutArea(pageNumber, parentBBox));
            }
            occupiedArea = new LayoutArea(pageNumber, new Rectangle(parentBBox.GetX(), parentBBox.GetY() + parentBBox.
                GetHeight(), parentBBox.GetWidth(), 0));
            ShrinkOccupiedAreaForAbsolutePosition();
            int currentAreaPos = 0;
            Rectangle layoutBox_1 = areas[0].Clone();
            lines = new List<LineRenderer>();
            foreach (IRenderer child in childRenderers) {
                currentRenderer.AddChild(child);
            }
            float lastYLine = layoutBox_1.GetY() + layoutBox_1.GetHeight();
            Leading leading = this.GetProperty<Leading>(Property.LEADING);
            float leadingValue = 0;
            float lastLineHeight = 0;
            if (marginsCollapsingEnabled && childRenderers.Count > 0) {
                // passing null is sufficient to notify that there is a kid, however we don't care about it and it's margins
                marginsCollapseHandler.StartChildMarginsHandling(null, layoutBox_1);
            }
            bool parentBBoxWasAdjusted = false;
            while (currentRenderer != null) {
                currentRenderer.SetProperty(Property.TAB_DEFAULT, this.GetPropertyAsFloat(Property.TAB_DEFAULT));
                currentRenderer.SetProperty(Property.TAB_STOPS, this.GetProperty<Object>(Property.TAB_STOPS));
                float lineIndent = anythingPlaced ? 0 : (float)this.GetPropertyAsFloat(Property.FIRST_LINE_INDENT);
                float childBBoxWidth = layoutBox_1.GetWidth() - lineIndent;
                Rectangle childLayoutBox = new Rectangle(layoutBox_1.GetX() + lineIndent, layoutBox_1.GetY(), childBBoxWidth
                    , layoutBox_1.GetHeight());
                LineLayoutResult result = ((LineLayoutResult)((LineRenderer)currentRenderer.SetParent(this)).Layout(new LayoutContext
                    (new LayoutArea(pageNumber, childLayoutBox), layoutContext.GetMarginsCollapseInfo(), floatRendererAreas
                    )));
                if (result.GetStatus() != LayoutResult.FULL && result.GetSplitRenderer() != null) {
                    for (int i = floatRendererAreas.Count - 1; i >= 0; i--) {
                        Rectangle floatRendererArea = floatRendererAreas[i];
                        if (floatRendererArea.GetY() > result.GetSplitRenderer().GetOccupiedArea().GetBBox().GetY()) {
                            floatRendererAreas.JRemoveAt(i);
                            if (layoutBox_1.GetX() >= floatRendererArea.GetX() + floatRendererArea.GetWidth()) {
                                layoutBox_1.MoveLeft(floatRendererArea.GetWidth());
                                layoutBox_1.SetWidth(layoutBox_1.GetWidth() + floatRendererArea.GetWidth());
                                parentBBoxWasAdjusted = true;
                            }
                        }
                    }
                }
                float minChildWidth = 0;
                float maxChildWidth = 0;
                if (result is MinMaxWidthLayoutResult) {
                    minChildWidth = ((MinMaxWidthLayoutResult)result).GetNotNullMinMaxWidth(childBBoxWidth).GetMinWidth();
                    maxChildWidth = ((MinMaxWidthLayoutResult)result).GetNotNullMinMaxWidth(childBBoxWidth).GetMaxWidth();
                }
                widthHandler.UpdateMinChildWidth(minChildWidth + lineIndent);
                widthHandler.UpdateMaxChildWidth(maxChildWidth + lineIndent);
                LineRenderer processedRenderer = null;
                if (result.GetStatus() == LayoutResult.FULL) {
                    processedRenderer = currentRenderer;
                }
                else {
                    if (result.GetStatus() == LayoutResult.PARTIAL) {
                        processedRenderer = (LineRenderer)result.GetSplitRenderer();
                    }
                    else {
                        if (currentRenderer.affectedByFloat) {
                            processedRenderer = new LineRenderer((LineRenderer)result.GetSplitRenderer());
                            float borderTotalHeight = 0;
                            if (borders[0] != null) {
                                borderTotalHeight += borders[0].GetWidth();
                            }
                            if (borders[2] != null) {
                                borderTotalHeight += borders[2].GetWidth();
                            }
                            processedRenderer.GetOccupiedArea().GetBBox().MoveUp(borderTotalHeight);
                            processedRenderer.GetOccupiedArea().GetBBox().SetHeight(processedRenderer.GetOccupiedArea().GetBBox().GetHeight
                                () - borderTotalHeight);
                        }
                    }
                }
                TextAlignment? textAlignment = (TextAlignment?)this.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT, TextAlignment
                    .LEFT);
                if (result.GetStatus() == LayoutResult.PARTIAL && textAlignment == TextAlignment.JUSTIFIED && !result.IsSplitForcedByNewline
                    () || textAlignment == TextAlignment.JUSTIFIED_ALL) {
                    if (processedRenderer != null) {
                        processedRenderer.Justify(layoutBox_1.GetWidth() - lineIndent);
                    }
                }
                else {
                    if (textAlignment != TextAlignment.LEFT && processedRenderer != null) {
                        float maxFloatWidth = 0;
                        foreach (Rectangle floatRendererArea in floatRendererAreas) {
                            if (floatRendererArea.GetWidth() > maxFloatWidth) {
                                maxFloatWidth = floatRendererArea.GetWidth();
                            }
                        }
                        float deltaX = childBBoxWidth - maxFloatWidth - processedRenderer.GetOccupiedArea().GetBBox().GetWidth();
                        switch (textAlignment) {
                            case TextAlignment.RIGHT: {
                                processedRenderer.Move(deltaX, 0);
                                break;
                            }

                            case TextAlignment.CENTER: {
                                processedRenderer.Move(deltaX / 2, 0);
                                break;
                            }
                        }
                    }
                }
                leadingValue = processedRenderer != null && leading != null ? processedRenderer.GetLeadingValue(leading) : 
                    0;
                if (processedRenderer != null && processedRenderer.ContainsImage()) {
                    leadingValue -= previousDescent;
                }
                bool doesNotFit = processedRenderer == null;
                float deltaY = 0;
                if (!doesNotFit) {
                    lastLineHeight = processedRenderer.GetOccupiedArea().GetBBox().GetHeight();
                    deltaY = lastYLine - leadingValue - processedRenderer.GetYLine();
                    // for the first and last line in a paragraph, leading is smaller
                    if (firstLineInBox) {
                        deltaY = -(leadingValue - lastLineHeight) / 2;
                    }
                    doesNotFit = leading != null && processedRenderer.GetOccupiedArea().GetBBox().GetY() + deltaY < layoutBox_1
                        .GetY();
                }
                if (doesNotFit) {
                    if (currentAreaPos + 1 < areas.Count) {
                        layoutBox_1 = areas[++currentAreaPos].Clone();
                        lastYLine = layoutBox_1.GetY() + layoutBox_1.GetHeight();
                        firstLineInBox = true;
                    }
                    else {
                        bool keepTogether = IsKeepTogether();
                        if (keepTogether) {
                            return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, null == result.GetCauseOfNothing
                                () ? this : result.GetCauseOfNothing());
                        }
                        else {
                            if (marginsCollapsingEnabled) {
                                if (anythingPlaced) {
                                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox_1);
                                }
                                marginsCollapseHandler.EndMarginsCollapse(layoutBox_1);
                            }
                            iText.Layout.Renderer.ParagraphRenderer[] split = Split();
                            split[0].lines = lines;
                            foreach (LineRenderer line in lines) {
                                split[0].childRenderers.AddAll(line.GetChildRenderers());
                            }
                            if (processedRenderer != null) {
                                split[1].childRenderers.AddAll(processedRenderer.GetChildRenderers());
                            }
                            if (result.GetOverflowRenderer() != null) {
                                split[1].childRenderers.AddAll(result.GetOverflowRenderer().GetChildRenderers());
                            }
                            if (HasProperty(Property.MAX_HEIGHT)) {
                                if (isPositioned) {
                                    CorrectPositionedLayout(layoutBox_1);
                                }
                                split[1].SetProperty(Property.MAX_HEIGHT, RetrieveMaxHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (HasProperty(Property.MIN_HEIGHT)) {
                                split[1].SetProperty(Property.MIN_HEIGHT, RetrieveMinHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (HasProperty(Property.HEIGHT)) {
                                split[1].SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (wasHeightClipped) {
                                split[0].GetOccupiedArea().GetBBox().MoveDown((float)blockMaxHeight - occupiedArea.GetBBox().GetHeight()).
                                    SetHeight((float)blockMaxHeight);
                                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.ParagraphRenderer));
                                logger.Warn(iText.IO.LogMessageConstant.CLIP_ELEMENT);
                            }
                            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                            ApplyMargins(occupiedArea.GetBBox(), true);
                            if (wasHeightClipped) {
                                return new MinMaxWidthLayoutResult(LayoutResult.FULL, occupiedArea, split[0], null).SetMinMaxWidth(minMaxWidth
                                    );
                            }
                            else {
                                if (anythingPlaced) {
                                    return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1]).SetMinMaxWidth(
                                        minMaxWidth);
                                }
                                else {
                                    if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), currentRenderer.GetOccupiedArea(
                                            ).GetBBox()));
                                        parent.SetProperty(Property.FULL, true);
                                        lines.Add(currentRenderer);
                                        // Force placement of children we have and do not force placement of the others
                                        if (LayoutResult.PARTIAL == result.GetStatus()) {
                                            IRenderer childNotRendered = result.GetCauseOfNothing();
                                            int firstNotRendered = currentRenderer.childRenderers.IndexOf(childNotRendered);
                                            currentRenderer.childRenderers.RetainAll(currentRenderer.childRenderers.SubList(0, firstNotRendered));
                                            split[1].childRenderers.RemoveAll(split[1].childRenderers.SubList(0, firstNotRendered));
                                            return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, occupiedArea, this, split[1], null, layoutBox_1, 
                                                parentBBoxWasAdjusted).SetMinMaxWidth(minMaxWidth);
                                        }
                                        else {
                                            return new MinMaxWidthLayoutResult(LayoutResult.FULL, occupiedArea, null, null, this, layoutBox_1, parentBBoxWasAdjusted
                                                ).SetMinMaxWidth(minMaxWidth);
                                        }
                                    }
                                    else {
                                        return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, null == result.GetCauseOfNothing
                                            () ? this : result.GetCauseOfNothing(), layoutBox_1, parentBBoxWasAdjusted);
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (leading != null) {
                        processedRenderer.Move(0, deltaY);
                        lastYLine = processedRenderer.GetYLine();
                    }
                    if (!currentRenderer.affectedByFloat) {
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), processedRenderer.GetOccupiedArea
                            ().GetBBox()));
                        if (firstLineInBox) {
                            firstLineInBox = false;
                        }
                    }
                    layoutBox_1.SetHeight(processedRenderer.GetOccupiedArea().GetBBox().GetY() - layoutBox_1.GetY());
                    lines.Add(processedRenderer);
                    anythingPlaced = true;
                    currentRenderer = (LineRenderer)result.GetOverflowRenderer();
                    previousDescent = processedRenderer.GetMaxDescent();
                }
            }
            if (marginsCollapsingEnabled) {
                if (childRenderers.Count > 0) {
                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox_1);
                }
                marginsCollapseHandler.EndMarginsCollapse(layoutBox_1);
            }
            float moveDown = Math.Min((leadingValue - lastLineHeight) / 2, occupiedArea.GetBBox().GetY() - layoutBox_1
                .GetY());
            occupiedArea.GetBBox().MoveDown(moveDown);
            occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + moveDown);
            IRenderer overflowRenderer = null;
            float? blockMinHeight = RetrieveMinHeight();
            if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight()) {
                float blockBottom = occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                    ());
                if (blockBottom >= layoutContext.GetArea().GetBBox().GetBottom()) {
                    occupiedArea.GetBBox().SetY(blockBottom).SetHeight((float)blockMinHeight);
                }
                else {
                    occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - layoutContext.GetArea().GetBBox
                        ().GetBottom()).SetY(layoutContext.GetArea().GetBBox().GetBottom());
                    overflowRenderer = CreateOverflowRenderer(parent);
                    overflowRenderer.SetProperty(Property.MIN_HEIGHT, (float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                        ());
                    if (HasProperty(Property.HEIGHT)) {
                        overflowRenderer.SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                    }
                }
                ApplyVerticalAlignment();
            }
            if (isPositioned) {
                CorrectPositionedLayout(layoutBox_1);
            }
            float initialWidth = occupiedArea.GetBBox().GetWidth();
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            Rectangle rect = ApplyMargins(occupiedArea.GetBBox(), true);
            float childrenMaxWidth = minMaxWidth.GetChildrenMaxWidth();
            if (blockWidth != null && childrenMaxWidth < blockWidth) {
                childrenMaxWidth = blockWidth;
            }
            childrenMaxWidth = childrenMaxWidth != 0 ? childrenMaxWidth + rect.GetWidth() - initialWidth : 0;
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null) {
                ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
                if (IsNotFittingLayoutArea(layoutContext.GetArea())) {
                    if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                        return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, this);
                    }
                }
            }
            RemoveUnnecessaryFloatRendererAreas(floatRendererAreas);
            LayoutArea editedArea = ApplyFloatPropertyOnCurrentArea(floatRendererAreas, layoutContext.GetArea().GetBBox
                ().GetWidth(), childrenMaxWidth);
            AdjustLayoutAreaIfClearPropertyIsPresented(clearHeightCorrection, editedArea, floatPropertyValue);
            if (null == overflowRenderer) {
                return new MinMaxWidthLayoutResult(LayoutResult.FULL, editedArea, null, null, null, layoutBox_1, parentBBoxWasAdjusted
                    ).SetMinMaxWidth(minMaxWidth);
            }
            else {
                return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, editedArea, this, overflowRenderer, null, layoutBox_1
                    , parentBBoxWasAdjusted).SetMinMaxWidth(minMaxWidth);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.ParagraphRenderer((Paragraph)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override T1 GetDefaultProperty<T1>(int property) {
            if ((property == Property.MARGIN_TOP || property == Property.MARGIN_BOTTOM) && parent is CellRenderer) {
                return (T1)(Object)0f;
            }
            return base.GetDefaultProperty<T1>(property);
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            if (lines != null && lines.Count > 0) {
                for (int i = 0; i < lines.Count; i++) {
                    if (i > 0) {
                        sb.Append("\n");
                    }
                    sb.Append(lines[i].ToString());
                }
            }
            else {
                foreach (IRenderer renderer in childRenderers) {
                    sb.Append(renderer.ToString());
                }
            }
            return sb.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            if (lines != null) {
                foreach (LineRenderer line in lines) {
                    line.Draw(drawContext);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void Move(float dxRight, float dyUp) {
            occupiedArea.GetBBox().MoveRight(dxRight);
            occupiedArea.GetBBox().MoveUp(dyUp);
            if (null != lines) {
                foreach (LineRenderer line in lines) {
                    line.Move(dxRight, dyUp);
                }
            }
        }

        /// <summary>
        /// Gets the lines which are the result of the
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// .
        /// </summary>
        /// <returns>paragraph lines, or <code>null</code> if layout hasn't been called yet</returns>
        public virtual IList<LineRenderer> GetLines() {
            return lines;
        }

        protected internal override float? GetFirstYLineRecursively() {
            if (lines == null || lines.Count == 0) {
                return null;
            }
            return lines[0].GetFirstYLineRecursively();
        }

        [Obsolete]
        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateOverflowRenderer() {
            return (iText.Layout.Renderer.ParagraphRenderer)GetNextRenderer();
        }

        [Obsolete]
        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateSplitRenderer() {
            return (iText.Layout.Renderer.ParagraphRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateOverflowRenderer(IRenderer parent
            ) {
            iText.Layout.Renderer.ParagraphRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.parent = parent;
            FixOverflowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateSplitRenderer(IRenderer parent) {
            iText.Layout.Renderer.ParagraphRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.parent = parent;
            splitRenderer.properties = new Dictionary<int, Object>(properties);
            return splitRenderer;
        }

        internal override MinMaxWidth GetMinMaxWidth(float availableWidth) {
            MinMaxWidthLayoutResult result = (MinMaxWidthLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle
                (availableWidth, AbstractRenderer.INF))));
            return CountRotationMinMaxWidth(CorrectMinMaxWidth(result.GetNotNullMinMaxWidth(availableWidth)));
        }

        protected internal virtual iText.Layout.Renderer.ParagraphRenderer[] Split() {
            iText.Layout.Renderer.ParagraphRenderer splitRenderer = CreateSplitRenderer(parent);
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            iText.Layout.Renderer.ParagraphRenderer overflowRenderer = CreateOverflowRenderer(parent);
            return new iText.Layout.Renderer.ParagraphRenderer[] { splitRenderer, overflowRenderer };
        }

        private void FixOverflowRenderer(iText.Layout.Renderer.ParagraphRenderer overflowRenderer) {
            // Reset first line indent in case of overflow.
            float firstLineIndent = (float)overflowRenderer.GetPropertyAsFloat(Property.FIRST_LINE_INDENT);
            if (firstLineIndent != 0) {
                overflowRenderer.SetProperty(Property.FIRST_LINE_INDENT, 0);
            }
        }
    }
}
