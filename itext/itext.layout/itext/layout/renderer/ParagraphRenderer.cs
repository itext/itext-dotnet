/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// object. It will draw the glyphs of the textual content on the
    /// <see cref="DrawContext"/>.
    /// </remarks>
    public class ParagraphRenderer : BlockRenderer {
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
            ParagraphOrphansControl orphansControl = this.GetProperty<ParagraphOrphansControl>(Property.ORPHANS_CONTROL
                );
            ParagraphWidowsControl widowsControl = this.GetProperty<ParagraphWidowsControl>(Property.WIDOWS_CONTROL);
            if (orphansControl != null || widowsControl != null) {
                return OrphansWidowsLayoutHelper.OrphansWidowsAwareLayout(this, layoutContext, orphansControl, widowsControl
                    );
            }
            if (RenderingMode.SVG_MODE == this.GetProperty<RenderingMode?>(Property.RENDERING_MODE) && !TypographyUtils
                .IsPdfCalligraphAvailable()) {
                // BASE_DIRECTION property is always set to the SVG text since we can't easily check whether typography is
                // available at svg module level, but it makes no sense without typography, so it is removed here.
                this.DeleteProperty(Property.BASE_DIRECTION);
            }
            LayoutResult layoutResult = DirectLayout(layoutContext);
            UpdateParentLines(this);
            UpdateParentLines((iText.Layout.Renderer.ParagraphRenderer)layoutResult.GetSplitRenderer());
            return layoutResult;
        }

        protected internal virtual LayoutResult DirectLayout(LayoutContext layoutContext) {
            bool wasHeightClipped = false;
            bool wasParentsHeightClipped = layoutContext.IsClippedHeight();
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            bool anythingPlaced = false;
            bool firstLineInBox = true;
            LineRenderer currentRenderer = (LineRenderer)new LineRenderer().SetParent(this);
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
            }
            ContinuousContainer.SetupContinuousContainerIfNeeded(this);
            OverflowPropertyValue? overflowX = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            bool? nowrapProp = this.GetPropertyAsBoolean(Property.NO_SOFT_WRAP_INLINE);
            currentRenderer.SetProperty(Property.NO_SOFT_WRAP_INLINE, nowrapProp);
            bool notAllKidsAreFloats = false;
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            float clearHeightCorrection = FloatingHelper.CalculateClearHeightCorrection(this, floatRendererAreas, parentBBox
                );
            FloatingHelper.ApplyClearance(parentBBox, marginsCollapseHandler, clearHeightCorrection, FloatingHelper.IsRendererFloating
                (this));
            float? blockWidth = RetrieveWidth(parentBBox.GetWidth());
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                blockWidth = FloatingHelper.AdjustFloatedBlockLayoutBox(this, parentBBox, blockWidth, floatRendererAreas, 
                    floatPropertyValue, overflowX);
                floatRendererAreas = new List<Rectangle>();
            }
            if (0 == childRenderers.Count) {
                anythingPlaced = true;
                currentRenderer = null;
            }
            bool isPositioned = IsPositioned();
            float? rotation = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            float? blockMaxHeight = RetrieveMaxHeight();
            OverflowPropertyValue? overflowY = (null == blockMaxHeight || blockMaxHeight > parentBBox.GetHeight()) && 
                !wasParentsHeightClipped ? OverflowPropertyValue.FIT : this.GetProperty<OverflowPropertyValue?>(Property
                .OVERFLOW_Y);
            if (rotation != null || IsFixedLayout()) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            if (rotation != null && !FloatingHelper.IsRendererFloating(this)) {
                blockWidth = RotationUtils.RetrieveRotatedLayoutWidth(parentBBox.GetWidth(), this);
            }
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.StartMarginsCollapse(parentBBox);
            }
            Border[] borders = GetBorders();
            UnitValue[] paddings = GetPaddings();
            float parentWidth = parentBBox.GetWidth();
            ApplyMargins(parentBBox, false);
            ApplyBorderBox(parentBBox, borders, false);
            if (IsFixedLayout()) {
                parentBBox.SetX((float)this.GetPropertyAsFloat(Property.LEFT));
            }
            ApplyPaddings(parentBBox, paddings, false);
            float additionalWidth = parentWidth - parentBBox.GetWidth();
            ApplyWidth(parentBBox, blockWidth, overflowX);
            wasHeightClipped = ApplyMaxHeight(parentBBox, blockMaxHeight, marginsCollapseHandler, false, wasParentsHeightClipped
                , overflowY);
            MinMaxWidth minMaxWidth = new MinMaxWidth(additionalWidth);
            AbstractWidthHandler widthHandler = new MaxMaxWidthHandler(minMaxWidth);
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
            TargetCounterHandler.AddPageByID(this);
            int currentAreaPos = 0;
            Rectangle layoutBox = areas[0].Clone();
            lines = new List<LineRenderer>();
            foreach (IRenderer child in childRenderers) {
                notAllKidsAreFloats = notAllKidsAreFloats || !FloatingHelper.IsRendererFloating(child);
                currentRenderer.AddChild(child);
            }
            float lastYLine = layoutBox.GetY() + layoutBox.GetHeight();
            float previousDescent = 0;
            float lastLineBottomLeadingIndent = 0;
            bool onlyOverflowedFloatsLeft = false;
            IList<IRenderer> inlineFloatsOverflowedToNextPage = new List<IRenderer>();
            bool floatOverflowedToNextPageWithNothing = false;
            // rectangles are compared by instances
            ICollection<Rectangle> nonChildFloatingRendererAreas = new HashSet<Rectangle>(floatRendererAreas);
            if (marginsCollapsingEnabled && childRenderers.Count > 0) {
                // passing null is sufficient to notify that there is a kid, however we don't care about it and it's margins
                marginsCollapseHandler.StartChildMarginsHandling(null, layoutBox);
            }
            bool includeFloatsInOccupiedArea = BlockFormattingContextUtil.IsRendererCreateBfc(this);
            while (currentRenderer != null) {
                currentRenderer.SetProperty(Property.TAB_DEFAULT, this.GetPropertyAsFloat(Property.TAB_DEFAULT));
                currentRenderer.SetProperty(Property.TAB_STOPS, this.GetProperty<Object>(Property.TAB_STOPS));
                float lineIndent = anythingPlaced ? 0 : (float)this.GetPropertyAsFloat(Property.FIRST_LINE_INDENT);
                Rectangle childLayoutBox = new Rectangle(layoutBox.GetX(), layoutBox.GetY(), layoutBox.GetWidth(), layoutBox
                    .GetHeight());
                currentRenderer.SetProperty(Property.OVERFLOW_X, overflowX);
                currentRenderer.SetProperty(Property.OVERFLOW_Y, overflowY);
                LineLayoutContext lineLayoutContext = new LineLayoutContext(new LayoutArea(pageNumber, childLayoutBox), null
                    , floatRendererAreas, wasHeightClipped || wasParentsHeightClipped).SetTextIndent(lineIndent).SetFloatOverflowedToNextPageWithNothing
                    (floatOverflowedToNextPageWithNothing);
                LineLayoutResult result = (LineLayoutResult)((LineRenderer)currentRenderer.SetParent(this)).Layout(lineLayoutContext
                    );
                bool isLastLineReLaidOut = false;
                if (result.GetStatus() == LayoutResult.NOTHING) {
                    //relayouting the child for allowing the vertical overflow in order to take into account the negative
                    // leading adjustment in case of a clipped-height context
                    if (layoutContext.IsClippedHeight()) {
                        OverflowPropertyValue? previousOverflowProperty = currentRenderer.GetProperty<OverflowPropertyValue?>(Property
                            .OVERFLOW_Y);
                        currentRenderer.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                        lineLayoutContext.SetClippedHeight(true);
                        result = (LineLayoutResult)((LineRenderer)currentRenderer.SetParent(this)).Layout(lineLayoutContext);
                        currentRenderer.SetProperty(Property.OVERFLOW_Y, previousOverflowProperty);
                        isLastLineReLaidOut = true;
                    }
                    float? lineShiftUnderFloats = FloatingHelper.CalculateLineShiftUnderFloats(floatRendererAreas, layoutBox);
                    if (lineShiftUnderFloats != null) {
                        layoutBox.DecreaseHeight((float)lineShiftUnderFloats);
                        firstLineInBox = true;
                        continue;
                    }
                    bool allRemainingKidsAreFloats = !currentRenderer.childRenderers.IsEmpty();
                    foreach (IRenderer renderer in currentRenderer.childRenderers) {
                        allRemainingKidsAreFloats = allRemainingKidsAreFloats && FloatingHelper.IsRendererFloating(renderer);
                    }
                    if (allRemainingKidsAreFloats) {
                        onlyOverflowedFloatsLeft = true;
                    }
                }
                floatOverflowedToNextPageWithNothing = lineLayoutContext.IsFloatOverflowedToNextPageWithNothing();
                if (result.GetFloatsOverflowedToNextPage() != null) {
                    inlineFloatsOverflowedToNextPage.AddAll(result.GetFloatsOverflowedToNextPage());
                }
                float minChildWidth = 0;
                float maxChildWidth = 0;
                if (result is MinMaxWidthLayoutResult) {
                    minChildWidth = ((MinMaxWidthLayoutResult)result).GetMinMaxWidth().GetMinWidth();
                    maxChildWidth = ((MinMaxWidthLayoutResult)result).GetMinMaxWidth().GetMaxWidth();
                }
                widthHandler.UpdateMinChildWidth(minChildWidth);
                widthHandler.UpdateMaxChildWidth(maxChildWidth);
                LineRenderer processedRenderer = (LineRenderer)result.GetSplitRenderer();
                if (processedRenderer == null && result.GetStatus() == LayoutResult.FULL) {
                    processedRenderer = currentRenderer;
                }
                if (onlyOverflowedFloatsLeft) {
                    // This is done to trick ParagraphRenderer to break rendering and to overflow to the next page.
                    // The `onlyOverflowedFloatsLeft` is set to true only when no other content is left except
                    // overflowed floating elements.
                    processedRenderer = null;
                }
                TextAlignment? textAlignment = (TextAlignment?)this.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT, TextAlignment
                    .LEFT);
                ApplyTextAlignment(textAlignment, result, processedRenderer, layoutBox, floatRendererAreas, onlyOverflowedFloatsLeft
                    , lineIndent);
                Leading leading = RenderingMode.HTML_MODE.Equals(this.GetProperty<RenderingMode?>(Property.RENDERING_MODE)
                    ) ? null : this.GetProperty<Leading>(Property.LEADING);
                // could be false if e.g. line contains only floats
                bool lineHasContent = processedRenderer != null && processedRenderer.GetOccupiedArea().GetBBox().GetHeight
                    () > 0;
                bool isFit = processedRenderer != null;
                float deltaY = 0;
                if (isFit && !RenderingMode.HTML_MODE.Equals(this.GetProperty<RenderingMode?>(Property.RENDERING_MODE))) {
                    if (lineHasContent) {
                        float indentFromLastLine = previousDescent - lastLineBottomLeadingIndent - (leading != null ? processedRenderer
                            .GetTopLeadingIndent(leading) : 0) - processedRenderer.GetMaxAscent();
                        if (processedRenderer.ContainsImage()) {
                            indentFromLastLine += previousDescent;
                        }
                        deltaY = lastYLine + indentFromLastLine - processedRenderer.GetYLine();
                        lastLineBottomLeadingIndent = leading != null ? processedRenderer.GetBottomLeadingIndent(leading) : 0;
                        if (lastLineBottomLeadingIndent < 0 && processedRenderer.ContainsImage()) {
                            lastLineBottomLeadingIndent = 0;
                        }
                    }
                    // for the first and last line in a paragraph, leading is smaller
                    if (firstLineInBox) {
                        deltaY = processedRenderer != null && leading != null ? -processedRenderer.GetTopLeadingIndent(leading) : 
                            0;
                    }
                    if (isLastLineReLaidOut) {
                        isFit = leading == null || processedRenderer.GetOccupiedArea().GetBBox().GetY() + deltaY - lastLineBottomLeadingIndent
                             >= layoutBox.GetY();
                    }
                    else {
                        isFit = leading == null || processedRenderer.GetOccupiedArea().GetBBox().GetY() + deltaY >= layoutBox.GetY
                            ();
                    }
                }
                if (!isFit && (null == processedRenderer || IsOverflowFit(overflowY))) {
                    if (currentAreaPos + 1 < areas.Count) {
                        layoutBox = areas[++currentAreaPos].Clone();
                        lastYLine = layoutBox.GetY() + layoutBox.GetHeight();
                        firstLineInBox = true;
                    }
                    else {
                        bool keepTogether = IsKeepTogether(result.GetCauseOfNothing());
                        if (keepTogether) {
                            floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                            // Use paragraph as a cause of nothing because parent relationship between TextRenderer
                            // and ParagraphRenderer can be broken by ParagraphRenderer#updateParentLines method.
                            return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, this);
                        }
                        else {
                            if (marginsCollapsingEnabled) {
                                if (anythingPlaced && notAllKidsAreFloats) {
                                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                                }
                            }
                            // On page split, if not only overflowed floats left, content will be drawn on next page, i.e. under all floats on this page
                            bool includeFloatsInOccupiedAreaOnSplit = !onlyOverflowedFloatsLeft || includeFloatsInOccupiedArea;
                            if (includeFloatsInOccupiedAreaOnSplit) {
                                FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this, nonChildFloatingRendererAreas);
                                FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                            }
                            if (marginsCollapsingEnabled) {
                                marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                            }
                            bool minHeightOverflowed = false;
                            if (!includeFloatsInOccupiedAreaOnSplit) {
                                AbstractRenderer minHeightOverflow = ApplyMinHeight(overflowY, layoutBox);
                                minHeightOverflowed = minHeightOverflow != null;
                                ApplyVerticalAlignment();
                            }
                            iText.Layout.Renderer.ParagraphRenderer[] split = Split();
                            split[0].lines = lines;
                            foreach (LineRenderer line in lines) {
                                split[0].childRenderers.AddAll(line.GetChildRenderers());
                            }
                            split[1].childRenderers.AddAll(inlineFloatsOverflowedToNextPage);
                            if (processedRenderer != null) {
                                split[1].childRenderers.AddAll(processedRenderer.GetChildRenderers());
                            }
                            if (result.GetOverflowRenderer() != null) {
                                split[1].childRenderers.AddAll(result.GetOverflowRenderer().GetChildRenderers());
                            }
                            if (onlyOverflowedFloatsLeft && !includeFloatsInOccupiedArea && !minHeightOverflowed) {
                                FloatingHelper.RemoveParentArtifactsOnPageSplitIfOnlyFloatsOverflow(split[1]);
                            }
                            float usedHeight = occupiedArea.GetBBox().GetHeight();
                            if (!includeFloatsInOccupiedAreaOnSplit) {
                                Rectangle commonRectangle = Rectangle.GetCommonRectangle(layoutBox, occupiedArea.GetBBox());
                                usedHeight = commonRectangle.GetHeight();
                            }
                            UpdateHeightsOnSplit(usedHeight, wasHeightClipped, this, split[1], includeFloatsInOccupiedAreaOnSplit);
                            CorrectFixedLayout(layoutBox);
                            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                            ApplyMargins(occupiedArea.GetBBox(), true);
                            ApplyAbsolutePositionIfNeeded(layoutContext);
                            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                                (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            if (wasHeightClipped) {
                                return new MinMaxWidthLayoutResult(LayoutResult.FULL, editedArea, split[0], null).SetMinMaxWidth(minMaxWidth
                                    );
                            }
                            else {
                                if (anythingPlaced) {
                                    return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, editedArea, split[0], split[1]).SetMinMaxWidth(minMaxWidth
                                        );
                                }
                                else {
                                    if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), currentRenderer.GetOccupiedArea(
                                            ).GetBBox()));
                                        FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                                        parent.SetProperty(Property.FULL, true);
                                        lines.Add(currentRenderer);
                                        // Force placement of children we have and do not force placement of the others
                                        if (LayoutResult.PARTIAL == result.GetStatus()) {
                                            IRenderer childNotRendered = result.GetCauseOfNothing();
                                            int firstNotRendered = currentRenderer.childRenderers.IndexOf(childNotRendered);
                                            currentRenderer.childRenderers.RetainAll(currentRenderer.childRenderers.SubList(0, firstNotRendered));
                                            // as we ignore split result here and use current line - we should update parents
                                            foreach (IRenderer child in currentRenderer.GetChildRenderers()) {
                                                child.SetParent(currentRenderer);
                                            }
                                            split[1].childRenderers.RemoveAll(split[1].childRenderers.SubList(0, firstNotRendered));
                                            return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, editedArea, this, split[1], null).SetMinMaxWidth(
                                                minMaxWidth);
                                        }
                                        else {
                                            return new MinMaxWidthLayoutResult(LayoutResult.FULL, editedArea, null, null, this).SetMinMaxWidth(minMaxWidth
                                                );
                                        }
                                    }
                                    else {
                                        floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                                        IRenderer overflowRenderer = result.GetCauseOfNothing() is AreaBreakRenderer ? split[1] : this;
                                        return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, overflowRenderer, null == result.GetCauseOfNothing
                                            () ? this : result.GetCauseOfNothing());
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (leading != null) {
                        processedRenderer.ApplyLeading(deltaY);
                        if (lineHasContent) {
                            lastYLine = processedRenderer.GetYLine();
                        }
                    }
                    if (lineHasContent) {
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), processedRenderer.GetOccupiedArea
                            ().GetBBox()));
                        FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                    }
                    firstLineInBox = false;
                    layoutBox.SetHeight(processedRenderer.GetOccupiedArea().GetBBox().GetY() - layoutBox.GetY());
                    lines.Add(processedRenderer);
                    anythingPlaced = true;
                    currentRenderer = (LineRenderer)result.GetOverflowRenderer();
                    previousDescent = processedRenderer.GetMaxDescent();
                    if (!inlineFloatsOverflowedToNextPage.IsEmpty() && result.GetOverflowRenderer() == null) {
                        onlyOverflowedFloatsLeft = true;
                        // dummy renderer to trick paragraph renderer to continue kids loop
                        currentRenderer = new LineRenderer();
                    }
                }
            }
            if (!RenderingMode.HTML_MODE.Equals(this.GetProperty<RenderingMode?>(Property.RENDERING_MODE))) {
                float moveDown = lastLineBottomLeadingIndent;
                if (IsOverflowFit(overflowY) && moveDown > occupiedArea.GetBBox().GetY() - layoutBox.GetY()) {
                    moveDown = occupiedArea.GetBBox().GetY() - layoutBox.GetY();
                }
                occupiedArea.GetBBox().MoveDown(moveDown);
                occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + moveDown);
            }
            if (marginsCollapsingEnabled) {
                if (childRenderers.Count > 0 && notAllKidsAreFloats) {
                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                }
            }
            if (includeFloatsInOccupiedArea) {
                FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this, nonChildFloatingRendererAreas);
                FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
            }
            if (wasHeightClipped) {
                FixOccupiedAreaIfOverflowedY(overflowY, layoutBox);
            }
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.EndMarginsCollapse(layoutBox);
            }
            AbstractRenderer overflowRenderer_1 = ApplyMinHeight(overflowY, layoutBox);
            if (overflowRenderer_1 != null && IsKeepTogether()) {
                floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
            }
            ContinuousContainer continuousContainer = this.GetProperty<ContinuousContainer>(Property.TREAT_AS_CONTINUOUS_CONTAINER_RESULT
                );
            if (continuousContainer != null && overflowRenderer_1 == null) {
                continuousContainer.ReApplyProperties(this);
                paddings = GetPaddings();
                borders = GetBorders();
            }
            CorrectFixedLayout(layoutBox);
            ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
            ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            ApplyAbsolutePositionIfNeeded(layoutContext);
            if (rotation != null) {
                ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
                if (IsNotFittingLayoutArea(layoutContext.GetArea())) {
                    if (IsNotFittingWidth(layoutContext.GetArea()) && !IsNotFittingHeight(layoutContext.GetArea())) {
                        ITextLogManager.GetLogger(GetType()).LogWarning(MessageFormatUtil.Format(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA
                            , "It fits by height so it will be forced placed"));
                    }
                    else {
                        if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                            floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                            return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, this);
                        }
                    }
                }
            }
            ApplyVerticalAlignment();
            FloatingHelper.RemoveFloatsAboveRendererBottom(floatRendererAreas, this);
            LayoutArea editedArea_1 = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer_1);
            if (null == overflowRenderer_1) {
                return new MinMaxWidthLayoutResult(LayoutResult.FULL, editedArea_1, null, null, null).SetMinMaxWidth(minMaxWidth
                    );
            }
            else {
                return new MinMaxWidthLayoutResult(LayoutResult.PARTIAL, editedArea_1, this, overflowRenderer_1, null).SetMinMaxWidth
                    (minMaxWidth);
            }
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
        /// <see cref="ParagraphRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.ParagraphRenderer), this.GetType());
            return new iText.Layout.Renderer.ParagraphRenderer((Paragraph)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override T1 GetDefaultProperty<T1>(int property) {
            if ((property == Property.MARGIN_TOP || property == Property.MARGIN_BOTTOM) && parent is CellRenderer) {
                return (T1)(Object)UnitValue.CreatePointValue(0f);
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
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ParagraphRenderer));
            if (occupiedArea == null) {
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Moving won't be performed."));
                return;
            }
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
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>.
        /// </summary>
        /// <returns>paragraph lines, or <c>null</c> if layout hasn't been called yet</returns>
        public virtual IList<LineRenderer> GetLines() {
            return lines;
        }

        protected internal override float? GetFirstYLineRecursively() {
            if (lines == null || lines.Count == 0) {
                return null;
            }
            return lines[0].GetFirstYLineRecursively();
        }

        protected internal override float? GetLastYLineRecursively() {
            if (!AllowLastYLineRecursiveExtraction()) {
                return null;
            }
            if (lines == null || lines.Count == 0) {
                return null;
            }
            for (int i = lines.Count - 1; i >= 0; i--) {
                float? yLine = lines[i].GetLastYLineRecursively();
                if (yLine != null) {
                    return yLine;
                }
            }
            return null;
        }

        private iText.Layout.Renderer.ParagraphRenderer CreateOverflowRenderer() {
            return (iText.Layout.Renderer.ParagraphRenderer)GetNextRenderer();
        }

        private iText.Layout.Renderer.ParagraphRenderer CreateSplitRenderer() {
            return (iText.Layout.Renderer.ParagraphRenderer)GetNextRenderer();
        }

        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateOverflowRenderer(IRenderer parent
            ) {
            iText.Layout.Renderer.ParagraphRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.parent = parent;
            FixOverflowRenderer(overflowRenderer);
            overflowRenderer.AddAllProperties(GetOwnProperties());
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        protected internal virtual iText.Layout.Renderer.ParagraphRenderer CreateSplitRenderer(IRenderer parent) {
            iText.Layout.Renderer.ParagraphRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.parent = parent;
            splitRenderer.AddAllProperties(GetOwnProperties());
            return splitRenderer;
        }

        protected internal override AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            return CreateOverflowRenderer(parent);
        }

        public override MinMaxWidth GetMinMaxWidth() {
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            float? rotation = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (!SetMinMaxWidthBasedOnFixedWidth(minMaxWidth)) {
                float? minWidth = HasAbsoluteUnitValue(Property.MIN_WIDTH) ? RetrieveMinWidth(0) : null;
                float? maxWidth = HasAbsoluteUnitValue(Property.MAX_WIDTH) ? RetrieveMaxWidth(0) : null;
                if (minWidth == null || maxWidth == null) {
                    bool restoreRotation = HasOwnProperty(Property.ROTATION_ANGLE);
                    SetProperty(Property.ROTATION_ANGLE, null);
                    MinMaxWidthLayoutResult result = (MinMaxWidthLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle
                        (MinMaxWidthUtils.GetInfWidth(), AbstractRenderer.INF))));
                    if (restoreRotation) {
                        SetProperty(Property.ROTATION_ANGLE, rotation);
                    }
                    else {
                        DeleteOwnProperty(Property.ROTATION_ANGLE);
                    }
                    minMaxWidth = result.GetMinMaxWidth();
                }
                if (minWidth != null) {
                    minMaxWidth.SetChildrenMinWidth((float)minWidth);
                }
                if (maxWidth != null) {
                    minMaxWidth.SetChildrenMaxWidth((float)maxWidth);
                }
                if (minMaxWidth.GetChildrenMinWidth() > minMaxWidth.GetChildrenMaxWidth()) {
                    minMaxWidth.SetChildrenMaxWidth(minMaxWidth.GetChildrenMaxWidth());
                }
            }
            else {
                minMaxWidth.SetAdditionalWidth(CalculateAdditionalWidth(this));
            }
            return rotation != null ? RotationUtils.CountRotationMinMaxWidth(minMaxWidth, this) : minMaxWidth;
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
                overflowRenderer.SetProperty(Property.FIRST_LINE_INDENT, 0f);
            }
        }

        private void AlignStaticKids(LineRenderer renderer, float dxRight) {
            renderer.GetOccupiedArea().GetBBox().MoveRight(dxRight);
            foreach (IRenderer childRenderer in renderer.GetChildRenderers()) {
                if (FloatingHelper.IsRendererFloating(childRenderer)) {
                    continue;
                }
                childRenderer.Move(dxRight, 0);
            }
        }

        private void ApplyTextAlignment(TextAlignment? textAlignment, LineLayoutResult result, LineRenderer processedRenderer
            , Rectangle layoutBox, IList<Rectangle> floatRendererAreas, bool onlyOverflowedFloatsLeft, float lineIndent
            ) {
            if (textAlignment == TextAlignment.JUSTIFIED && result.GetStatus() == LayoutResult.PARTIAL && !result.IsSplitForcedByNewline
                () && !onlyOverflowedFloatsLeft || textAlignment == TextAlignment.JUSTIFIED_ALL) {
                if (processedRenderer != null) {
                    Rectangle actualLineLayoutBox = layoutBox.Clone();
                    FloatingHelper.AdjustLineAreaAccordingToFloats(floatRendererAreas, actualLineLayoutBox);
                    processedRenderer.Justify(actualLineLayoutBox.GetWidth() - lineIndent);
                }
            }
            else {
                if (textAlignment != TextAlignment.LEFT && processedRenderer != null) {
                    Rectangle actualLineLayoutBox = layoutBox.Clone();
                    FloatingHelper.AdjustLineAreaAccordingToFloats(floatRendererAreas, actualLineLayoutBox);
                    float deltaX = Math.Max(0, actualLineLayoutBox.GetWidth() - lineIndent - processedRenderer.GetOccupiedArea
                        ().GetBBox().GetWidth());
                    switch (textAlignment) {
                        case TextAlignment.RIGHT: {
                            AlignStaticKids(processedRenderer, deltaX);
                            break;
                        }

                        case TextAlignment.CENTER: {
                            AlignStaticKids(processedRenderer, deltaX / 2);
                            break;
                        }

                        case TextAlignment.JUSTIFIED: {
                            if (BaseDirection.RIGHT_TO_LEFT.Equals(this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION))) {
                                AlignStaticKids(processedRenderer, deltaX);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static void UpdateParentLines(iText.Layout.Renderer.ParagraphRenderer re) {
            if (re == null) {
                return;
            }
            foreach (LineRenderer lineRenderer in re.lines) {
                lineRenderer.SetParent(re);
            }
            foreach (IRenderer childRenderer in re.GetChildRenderers()) {
                IRenderer line = childRenderer.GetParent();
                if (!(line is LineRenderer && re.lines.Contains((LineRenderer)line))) {
                    childRenderer.SetParent(null);
                }
            }
        }
    }
}
