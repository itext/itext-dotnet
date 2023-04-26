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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for block elements.</summary>
    public abstract class BlockRenderer : AbstractRenderer {
        /// <summary>Creates a BlockRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.IElement"/>
        /// which this object should manage
        /// </param>
        protected internal BlockRenderer(IElement modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            this.isLastRendererForModelElement = true;
            IDictionary<int, IRenderer> waitingFloatsSplitRenderers = new LinkedDictionary<int, IRenderer>();
            IList<IRenderer> waitingOverflowFloatRenderers = new List<IRenderer>();
            bool floatOverflowedCompletely = false;
            bool wasHeightClipped = false;
            bool wasParentsHeightClipped = layoutContext.IsClippedHeight();
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            bool isPositioned = IsPositioned();
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            float? rotation = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            OverflowPropertyValue? overflowX = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
            }
            float? blockWidth = RetrieveWidth(parentBBox.GetWidth());
            if (rotation != null || IsFixedLayout()) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            if (rotation != null && !FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                blockWidth = RotationUtils.RetrieveRotatedLayoutWidth(parentBBox.GetWidth(), this);
            }
            bool includeFloatsInOccupiedArea = BlockFormattingContextUtil.IsRendererCreateBfc(this);
            float clearHeightCorrection = FloatingHelper.CalculateClearHeightCorrection(this, floatRendererAreas, parentBBox
                );
            FloatingHelper.ApplyClearance(parentBBox, marginsCollapseHandler, clearHeightCorrection, FloatingHelper.IsRendererFloating
                (this));
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                blockWidth = FloatingHelper.AdjustFloatedBlockLayoutBox(this, parentBBox, blockWidth, floatRendererAreas, 
                    floatPropertyValue, overflowX);
                floatRendererAreas = new List<Rectangle>();
            }
            bool isCellRenderer = this is CellRenderer;
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.StartMarginsCollapse(parentBBox);
            }
            Border[] borders = GetBorders();
            UnitValue[] paddings = GetPaddings();
            ApplyMargins(parentBBox, false);
            ApplyBorderBox(parentBBox, borders, false);
            if (IsFixedLayout()) {
                parentBBox.SetX((float)this.GetPropertyAsFloat(Property.LEFT));
            }
            ApplyPaddings(parentBBox, paddings, false);
            float? blockMaxHeight = RetrieveMaxHeight();
            OverflowPropertyValue? overflowY = (null == blockMaxHeight || blockMaxHeight > parentBBox.GetHeight()) && 
                !wasParentsHeightClipped ? OverflowPropertyValue.FIT : this.GetProperty<OverflowPropertyValue?>(Property
                .OVERFLOW_Y);
            ApplyWidth(parentBBox, blockWidth, overflowX);
            wasHeightClipped = ApplyMaxHeight(parentBBox, blockMaxHeight, marginsCollapseHandler, isCellRenderer, wasParentsHeightClipped
                , overflowY);
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
            // rectangles are compared by instances
            ICollection<Rectangle> nonChildFloatingRendererAreas = new HashSet<Rectangle>(floatRendererAreas);
            // the first renderer (one of childRenderers or their children) to produce LayoutResult.NOTHING
            IRenderer causeOfNothing = null;
            bool anythingPlaced = false;
            // We have to remember initial FORCED_PLACEMENT property of this renderer to use it later
            // to define if rotated content should be placed or not
            bool initialForcePlacementForRotationAdjustments = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT
                ));
            for (int childPos = 0; childPos < childRenderers.Count; childPos++) {
                IRenderer childRenderer = childRenderers[childPos];
                LayoutResult result;
                childRenderer.SetParent(this);
                MarginsCollapseInfo childMarginsInfo = null;
                if (floatOverflowedCompletely && FloatingHelper.IsRendererFloating(childRenderer)) {
                    waitingFloatsSplitRenderers.Put(childPos, null);
                    waitingOverflowFloatRenderers.Add(childRenderer);
                    continue;
                }
                if (!waitingOverflowFloatRenderers.IsEmpty() && FloatingHelper.IsClearanceApplied(waitingOverflowFloatRenderers
                    , childRenderer.GetProperty<ClearPropertyValue?>(Property.CLEAR))) {
                    if (FloatingHelper.IsRendererFloating(childRenderer)) {
                        waitingFloatsSplitRenderers.Put(childPos, null);
                        waitingOverflowFloatRenderers.Add(childRenderer);
                        floatOverflowedCompletely = true;
                        continue;
                    }
                    if (marginsCollapsingEnabled && !isCellRenderer) {
                        marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                    }
                    FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this, nonChildFloatingRendererAreas);
                    FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                    result = new LayoutResult(LayoutResult.NOTHING, null, null, childRenderer);
                    bool isKeepTogether = IsKeepTogether(childRenderer);
                    int layoutResult = anythingPlaced && !isKeepTogether ? LayoutResult.PARTIAL : LayoutResult.NOTHING;
                    AbstractRenderer[] splitAndOverflowRenderers = CreateSplitAndOverflowRenderers(childPos, layoutResult, result
                        , waitingFloatsSplitRenderers, waitingOverflowFloatRenderers);
                    AbstractRenderer splitRenderer = splitAndOverflowRenderers[0];
                    AbstractRenderer overflowRenderer = splitAndOverflowRenderers[1];
                    if (isKeepTogether) {
                        splitRenderer = null;
                        overflowRenderer.childRenderers.Clear();
                        overflowRenderer.childRenderers = new List<IRenderer>(childRenderers);
                    }
                    UpdateHeightsOnSplit(wasHeightClipped, splitRenderer, overflowRenderer);
                    ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                    ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                    ApplyMargins(occupiedArea.GetBBox(), true);
                    if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) || wasHeightClipped) {
                        LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                            (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                        return new LayoutResult(LayoutResult.FULL, editedArea, splitRenderer, null, null);
                    }
                    else {
                        if (layoutResult != LayoutResult.NOTHING) {
                            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                                (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            return new LayoutResult(layoutResult, editedArea, splitRenderer, overflowRenderer, null).SetAreaBreak(result
                                .GetAreaBreak());
                        }
                        else {
                            floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                            return new LayoutResult(layoutResult, null, null, overflowRenderer, result.GetCauseOfNothing()).SetAreaBreak
                                (result.GetAreaBreak());
                        }
                    }
                }
                if (marginsCollapsingEnabled) {
                    childMarginsInfo = StartChildMarginsHandling(childRenderer, layoutBox, marginsCollapseHandler);
                }
                Rectangle changedLayoutBox = RecalculateLayoutBoxBeforeChildLayout(layoutBox, childRenderer, areas[0].Clone
                    ());
                while ((result = childRenderer.SetParent(this).Layout(new LayoutContext(new LayoutArea(pageNumber, changedLayoutBox
                    ), childMarginsInfo, floatRendererAreas, wasHeightClipped || wasParentsHeightClipped))).GetStatus() !=
                     LayoutResult.FULL) {
                    if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA_ON_SPLIT)) || true.Equals(GetPropertyAsBoolean
                        (Property.FILL_AVAILABLE_AREA))) {
                        occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), layoutBox));
                    }
                    else {
                        if (result.GetOccupiedArea() != null && result.GetStatus() != LayoutResult.NOTHING) {
                            RecalculateOccupiedAreaAfterChildLayout(result.GetOccupiedArea().GetBBox(), blockMaxHeight);
                            FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                        }
                    }
                    if (marginsCollapsingEnabled && result.GetStatus() != LayoutResult.NOTHING) {
                        marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                    }
                    if (FloatingHelper.IsRendererFloating(childRenderer)) {
                        // Check if current block is empty, kid returns nothing and neither floats nor content
                        // were met on root area (e.g. page area) - return NOTHING, don't layout other kids,
                        // expect FORCED_PLACEMENT to be set.
                        bool immediatelyReturnNothing = result.GetStatus() == LayoutResult.NOTHING && !anythingPlaced && floatRendererAreas
                            .IsEmpty() && IsFirstOnRootArea();
                        if (!immediatelyReturnNothing) {
                            waitingFloatsSplitRenderers.Put(childPos, result.GetStatus() == LayoutResult.PARTIAL ? result.GetSplitRenderer
                                () : null);
                            waitingOverflowFloatRenderers.Add(result.GetOverflowRenderer());
                            floatOverflowedCompletely = result.GetStatus() == LayoutResult.NOTHING;
                            break;
                        }
                    }
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                    }
                    // On page split, content will be drawn on next page, i.e. under all floats on this page
                    FloatingHelper.IncludeChildFloatsInOccupiedArea(floatRendererAreas, this, nonChildFloatingRendererAreas);
                    FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                    if (result.GetSplitRenderer() != null) {
                        // TODO DEVSIX-6488 all elements should be layouted first in case when parent box should wrap around child boxes
                        AlignChildHorizontally(result.GetSplitRenderer(), occupiedArea.GetBBox());
                    }
                    // Save the first renderer to produce LayoutResult.NOTHING
                    if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                        causeOfNothing = result.GetCauseOfNothing();
                    }
                    // have more areas
                    if (currentAreaPos + 1 < areas.Count && !(result.GetAreaBreak() != null && result.GetAreaBreak().GetAreaType
                        () == AreaBreakType.NEXT_PAGE)) {
                        if (result.GetStatus() == LayoutResult.PARTIAL) {
                            childRenderers[childPos] = result.GetSplitRenderer();
                            childRenderers.Add(childPos + 1, result.GetOverflowRenderer());
                        }
                        else {
                            if (result.GetOverflowRenderer() != null) {
                                childRenderers[childPos] = result.GetOverflowRenderer();
                            }
                            else {
                                childRenderers.JRemoveAt(childPos);
                            }
                            childPos--;
                        }
                        layoutBox = areas[++currentAreaPos].Clone();
                        break;
                    }
                    else {
                        LayoutResult layoutResult = ProcessNotFullChildResult(layoutContext, waitingFloatsSplitRenderers, waitingOverflowFloatRenderers
                            , wasHeightClipped, floatRendererAreas, marginsCollapsingEnabled, clearHeightCorrection, borders, paddings
                            , areas, currentAreaPos, layoutBox, nonChildFloatingRendererAreas, causeOfNothing, anythingPlaced, childPos
                            , result);
                        if (layoutResult == null) {
                            layoutBox = areas[++currentAreaPos].Clone();
                            break;
                        }
                        if (StopLayoutingChildrenIfChildResultNotFull(layoutResult)) {
                            return layoutResult;
                        }
                        result = layoutResult;
                        break;
                    }
                }
                anythingPlaced = anythingPlaced || result.GetStatus() != LayoutResult.NOTHING;
                HandleForcedPlacement(anythingPlaced);
                // The second condition check (after &&) is needed only if margins collapsing is enabled
                if (result.GetOccupiedArea() != null && (!FloatingHelper.IsRendererFloating(childRenderer) || includeFloatsInOccupiedArea
                    )) {
                    RecalculateOccupiedAreaAfterChildLayout(result.GetOccupiedArea().GetBBox(), blockMaxHeight);
                    FixOccupiedAreaIfOverflowedX(overflowX, layoutBox);
                }
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler.EndChildMarginsHandling(layoutBox);
                }
                if (result.GetStatus() == LayoutResult.FULL) {
                    DecreaseLayoutBoxAfterChildPlacement(layoutBox, result, childRenderer);
                    if (childRenderer.GetOccupiedArea() != null) {
                        // TODO DEVSIX-6488 all elements should be layouted first in case when parent box should wrap around child boxes
                        AlignChildHorizontally(childRenderer, occupiedArea.GetBBox());
                    }
                }
                // Save the first renderer to produce LayoutResult.NOTHING
                if (null == causeOfNothing && null != result.GetCauseOfNothing()) {
                    causeOfNothing = result.GetCauseOfNothing();
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
            if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA))) {
                occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), layoutBox));
            }
            int layoutResult_1 = LayoutResult.FULL;
            bool processOverflowedFloats = !waitingOverflowFloatRenderers.IsEmpty() && !wasHeightClipped && !true.Equals
                (GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
            AbstractRenderer overflowRenderer_1 = null;
            if (!includeFloatsInOccupiedArea || !processOverflowedFloats) {
                overflowRenderer_1 = ApplyMinHeight(overflowY, layoutBox);
            }
            bool minHeightOverflow = overflowRenderer_1 != null;
            if (minHeightOverflow && IsKeepTogether()) {
                floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
            }
            // in this case layout result need to be changed
            if (overflowRenderer_1 != null || processOverflowedFloats) {
                layoutResult_1 = !anythingPlaced && !waitingOverflowFloatRenderers.IsEmpty() ? LayoutResult.NOTHING : LayoutResult
                    .PARTIAL;
            }
            // nothing was placed and there are some overflowed floats
            // either something was placed or (since there are no overflowed floats) there is overflow renderer
            // that indicates overflowed min_height
            if (processOverflowedFloats) {
                if (overflowRenderer_1 == null || layoutResult_1 == LayoutResult.NOTHING) {
                    // if layout result is NOTHING - avoid possible usage of the overflowRenderer created
                    // for overflow of min_height with adjusted height properties
                    overflowRenderer_1 = CreateOverflowRenderer(layoutResult_1);
                }
                overflowRenderer_1.GetChildRenderers().AddAll(waitingOverflowFloatRenderers);
                if (layoutResult_1 == LayoutResult.PARTIAL && !minHeightOverflow && !includeFloatsInOccupiedArea) {
                    FloatingHelper.RemoveParentArtifactsOnPageSplitIfOnlyFloatsOverflow(overflowRenderer_1);
                }
            }
            AbstractRenderer splitRenderer_1 = this;
            if (waitingFloatsSplitRenderers.Count > 0 && layoutResult_1 != LayoutResult.NOTHING) {
                splitRenderer_1 = CreateSplitRenderer(layoutResult_1);
                splitRenderer_1.childRenderers = new List<IRenderer>(childRenderers);
                ReplaceSplitRendererKidFloats(waitingFloatsSplitRenderers, splitRenderer_1);
                float usedHeight = occupiedArea.GetBBox().GetHeight();
                if (!includeFloatsInOccupiedArea) {
                    Rectangle commonRectangle = Rectangle.GetCommonRectangle(layoutBox, occupiedArea.GetBBox());
                    usedHeight = commonRectangle.GetHeight();
                }
                // this must be processed before margin/border/padding
                UpdateHeightsOnSplit(usedHeight, wasHeightClipped, splitRenderer_1, overflowRenderer_1, includeFloatsInOccupiedArea
                    );
            }
            if (positionedRenderers.Count > 0) {
                foreach (IRenderer childPositionedRenderer in positionedRenderers) {
                    Rectangle fullBbox = occupiedArea.GetBBox().Clone();
                    // Use that value so that layout is independent of whether we are in the bottom of the page or in the top of the page
                    float layoutMinHeight = 1000;
                    fullBbox.MoveDown(layoutMinHeight).SetHeight(layoutMinHeight + fullBbox.GetHeight());
                    LayoutArea parentArea = new LayoutArea(occupiedArea.GetPageNumber(), occupiedArea.GetBBox().Clone());
                    ApplyPaddings(parentArea.GetBBox(), paddings, true);
                    PreparePositionedRendererAndAreaForLayout(childPositionedRenderer, fullBbox, parentArea.GetBBox());
                    childPositionedRenderer.Layout(new PositionedLayoutContext(new LayoutArea(occupiedArea.GetPageNumber(), fullBbox
                        ), parentArea));
                }
            }
            if (isPositioned) {
                CorrectFixedLayout(layoutBox);
            }
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
                        if (!initialForcePlacementForRotationAdjustments) {
                            floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                            return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, null, null, this, this);
                        }
                    }
                }
            }
            ApplyVerticalAlignment();
            FloatingHelper.RemoveFloatsAboveRendererBottom(floatRendererAreas, this);
            if (layoutResult_1 != LayoutResult.NOTHING) {
                LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                    (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                return new LayoutResult(layoutResult_1, editedArea, splitRenderer_1, overflowRenderer_1, causeOfNothing);
            }
            else {
                if (positionedRenderers.Count > 0) {
                    overflowRenderer_1.positionedRenderers = new List<IRenderer>(positionedRenderers);
                }
                floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                return new LayoutResult(LayoutResult.NOTHING, null, null, overflowRenderer_1, causeOfNothing);
            }
        }

        public override void Draw(DrawContext drawContext) {
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
            if (occupiedArea == null) {
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
                return;
            }
            bool isTagged = drawContext.IsTaggingEnabled();
            LayoutTaggingHelper taggingHelper = null;
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isTagged = false;
                }
                else {
                    TagTreePointer tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                    if (taggingHelper.CreateTag(this, tagPointer)) {
                        tagPointer.GetProperties().AddAttributes(0, AccessibleAttributesApplier.GetListAttributes(this, tagPointer
                            )).AddAttributes(0, AccessibleAttributesApplier.GetTableAttributes(this, tagPointer)).AddAttributes(0, 
                            AccessibleAttributesApplier.GetLayoutAttributes(this, tagPointer));
                    }
                }
            }
            BeginTransformationIfApplied(drawContext.GetCanvas());
            ApplyDestinationsAndAnnotation(drawContext);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            BeginElementOpacityApplying(drawContext);
            BeginRotationIfApplied(drawContext.GetCanvas());
            bool overflowXHidden = IsOverflowProperty(OverflowPropertyValue.HIDDEN, Property.OVERFLOW_X);
            bool overflowYHidden = IsOverflowProperty(OverflowPropertyValue.HIDDEN, Property.OVERFLOW_Y);
            bool processOverflow = overflowXHidden || overflowYHidden;
            DrawBackground(drawContext);
            DrawBorder(drawContext);
            AddMarkedContent(drawContext, true);
            if (processOverflow) {
                drawContext.GetCanvas().SaveState();
                int pageNumber = occupiedArea.GetPageNumber();
                Rectangle clippedArea;
                if (pageNumber < 1 || pageNumber > drawContext.GetDocument().GetNumberOfPages()) {
                    clippedArea = new Rectangle(-INF / 2, -INF / 2, INF, INF);
                }
                else {
                    PdfPage page = drawContext.GetDocument().GetPage(pageNumber);
                    // TODO DEVSIX-1655 This check is necessary because, in some cases, our renderer's hierarchy may contain
                    //  a renderer from the different page that was already flushed
                    if (page.IsFlushed()) {
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED
                            , "area clipping"));
                        clippedArea = new Rectangle(-INF / 2, -INF / 2, INF, INF);
                    }
                    else {
                        clippedArea = page.GetPageSize();
                    }
                }
                Rectangle area = GetBorderAreaBBox();
                if (overflowXHidden) {
                    clippedArea.SetX(area.GetX()).SetWidth(area.GetWidth());
                }
                if (overflowYHidden) {
                    clippedArea.SetY(area.GetY()).SetHeight(area.GetHeight());
                }
                drawContext.GetCanvas().Rectangle(clippedArea).Clip().EndPath();
            }
            DrawChildren(drawContext);
            AddMarkedContent(drawContext, false);
            DrawPositionedChildren(drawContext);
            if (processOverflow) {
                drawContext.GetCanvas().RestoreState();
            }
            EndRotationIfApplied(drawContext.GetCanvas());
            EndElementOpacityApplying(drawContext);
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(true);
            }
            if (isTagged) {
                if (isLastRendererForModelElement) {
                    taggingHelper.FinishTaggingHint(this);
                }
                taggingHelper.RestoreAutoTaggingPointerPosition(this);
            }
            flushed = true;
            EndTransformationIfApplied(drawContext.GetCanvas());
        }

        public override Rectangle GetOccupiedAreaBBox() {
            Rectangle bBox = occupiedArea.GetBBox().Clone();
            float? rotationAngle = this.GetProperty<float?>(Property.ROTATION_ANGLE);
            if (rotationAngle != null) {
                if (!HasOwnProperty(Property.ROTATION_INITIAL_WIDTH) || !HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER
                        , GetType().Name));
                }
                else {
                    bBox.SetWidth((float)this.GetPropertyAsFloat(Property.ROTATION_INITIAL_WIDTH));
                    bBox.SetHeight((float)this.GetPropertyAsFloat(Property.ROTATION_INITIAL_HEIGHT));
                }
            }
            return bBox;
        }

        /// <summary>Creates a split renderer.</summary>
        /// <param name="layoutResult">the result of content layouting</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        protected internal virtual AbstractRenderer CreateSplitRenderer(int layoutResult) {
            AbstractRenderer splitRenderer = (AbstractRenderer)GetNextRenderer();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.AddAllProperties(GetOwnProperties());
            return splitRenderer;
        }

        /// <summary>Creates an overflow renderer.</summary>
        /// <param name="layoutResult">the result of content layouting</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        protected internal virtual AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            AbstractRenderer overflowRenderer = (AbstractRenderer)GetNextRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return overflowRenderer;
        }

        internal virtual void RecalculateOccupiedAreaAfterChildLayout(Rectangle resultBBox, float? blockMaxHeight) {
            occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), resultBBox));
        }

        internal virtual MarginsCollapseInfo StartChildMarginsHandling(IRenderer childRenderer, Rectangle layoutBox
            , MarginsCollapseHandler marginsCollapseHandler) {
            return marginsCollapseHandler.StartChildMarginsHandling(childRenderer, layoutBox);
        }

        internal virtual Rectangle RecalculateLayoutBoxBeforeChildLayout(Rectangle layoutBox, IRenderer childRenderer
            , Rectangle initialLayoutBox) {
            return layoutBox;
        }

        internal virtual AbstractRenderer[] CreateSplitAndOverflowRenderers(int childPos, int layoutStatus, LayoutResult
             childResult, IDictionary<int, IRenderer> waitingFloatsSplitRenderers, IList<IRenderer> waitingOverflowFloatRenderers
            ) {
            AbstractRenderer splitRenderer = CreateSplitRenderer(layoutStatus);
            splitRenderer.childRenderers = new List<IRenderer>(childRenderers.SubList(0, childPos));
            if (childResult.GetStatus() == LayoutResult.PARTIAL && childResult.GetSplitRenderer() != null) {
                splitRenderer.childRenderers.Add(childResult.GetSplitRenderer());
            }
            ReplaceSplitRendererKidFloats(waitingFloatsSplitRenderers, splitRenderer);
            foreach (IRenderer renderer in splitRenderer.childRenderers) {
                renderer.SetParent(splitRenderer);
            }
            AbstractRenderer overflowRenderer = CreateOverflowRenderer(layoutStatus);
            overflowRenderer.childRenderers.AddAll(waitingOverflowFloatRenderers);
            if (childResult.GetOverflowRenderer() != null) {
                overflowRenderer.childRenderers.Add(childResult.GetOverflowRenderer());
            }
            overflowRenderer.childRenderers.AddAll(childRenderers.SubList(childPos + 1, childRenderers.Count));
            if (childResult.GetStatus() == LayoutResult.PARTIAL) {
                // Apply forced placement only on split renderer
                overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            }
            return new AbstractRenderer[] { splitRenderer, overflowRenderer };
        }

        /// <summary>
        /// This method applies vertical alignment for the occupied area
        /// of the renderer and its children renderers.
        /// </summary>
        protected internal virtual void ApplyVerticalAlignment() {
            VerticalAlignment? verticalAlignment = this.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
            if (verticalAlignment == null || verticalAlignment == VerticalAlignment.TOP || childRenderers.IsEmpty()) {
                return;
            }
            float lowestChildBottom = float.MaxValue;
            if (FloatingHelper.IsRendererFloating(this) || this is CellRenderer) {
                // include floats in vertical alignment
                foreach (IRenderer child in childRenderers) {
                    if (child.GetOccupiedArea() != null && child.GetOccupiedArea().GetBBox().GetBottom() < lowestChildBottom) {
                        lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                    }
                }
            }
            else {
                int lastChildIndex = childRenderers.Count - 1;
                while (lastChildIndex >= 0) {
                    IRenderer child = childRenderers[lastChildIndex--];
                    if (!FloatingHelper.IsRendererFloating(child) && child.GetOccupiedArea() != null) {
                        lowestChildBottom = child.GetOccupiedArea().GetBBox().GetBottom();
                        break;
                    }
                }
            }
            if (lowestChildBottom == float.MaxValue) {
                return;
            }
            float deltaY = lowestChildBottom - GetInnerAreaBBox().GetY();
            if (deltaY < 0) {
                return;
            }
            switch (verticalAlignment) {
                case VerticalAlignment.BOTTOM: {
                    foreach (IRenderer child in childRenderers) {
                        child.Move(0, -deltaY);
                    }
                    break;
                }

                case VerticalAlignment.MIDDLE: {
                    foreach (IRenderer child in childRenderers) {
                        child.Move(0, -deltaY / 2);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// This method rotates content of the renderer and
        /// calculates correct occupied area for the rotated element.
        /// </summary>
        /// <param name="layoutBox">
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        protected internal virtual void ApplyRotationLayout(Rectangle layoutBox) {
            float angle = (float)this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            float x = occupiedArea.GetBBox().GetX();
            float y = occupiedArea.GetBBox().GetY();
            float height = occupiedArea.GetBBox().GetHeight();
            float width = occupiedArea.GetBBox().GetWidth();
            SetProperty(Property.ROTATION_INITIAL_WIDTH, width);
            SetProperty(Property.ROTATION_INITIAL_HEIGHT, height);
            AffineTransform rotationTransform = new AffineTransform();
            // here we calculate and set the actual occupied area of the rotated content
            if (IsPositioned()) {
                float? rotationPointX = this.GetPropertyAsFloat(Property.ROTATION_POINT_X);
                float? rotationPointY = this.GetPropertyAsFloat(Property.ROTATION_POINT_Y);
                if (rotationPointX == null || rotationPointY == null) {
                    // if rotation point was not specified, the most bottom-left point is used
                    rotationPointX = x;
                    rotationPointY = y;
                }
                // transforms apply from bottom to top
                // move point back at place
                rotationTransform.Translate((float)rotationPointX, (float)rotationPointY);
                // rotate
                rotationTransform.Rotate(angle);
                // move rotation point to origin
                rotationTransform.Translate((float)-rotationPointX, (float)-rotationPointY);
                IList<Point> rotatedPoints = TransformPoints(RectangleToPointsList(occupiedArea.GetBBox()), rotationTransform
                    );
                Rectangle newBBox = CalculateBBox(rotatedPoints);
                // make occupied area be of size and position of actual content
                occupiedArea.GetBBox().SetWidth(newBBox.GetWidth());
                occupiedArea.GetBBox().SetHeight(newBBox.GetHeight());
                float occupiedAreaShiftX = newBBox.GetX() - x;
                float occupiedAreaShiftY = newBBox.GetY() - y;
                Move(occupiedAreaShiftX, occupiedAreaShiftY);
            }
            else {
                rotationTransform = AffineTransform.GetRotateInstance(angle);
                IList<Point> rotatedPoints = TransformPoints(RectangleToPointsList(occupiedArea.GetBBox()), rotationTransform
                    );
                float[] shift = CalculateShiftToPositionBBoxOfPointsAt(x, y + height, rotatedPoints);
                foreach (Point point in rotatedPoints) {
                    point.SetLocation(point.GetX() + shift[0], point.GetY() + shift[1]);
                }
                Rectangle newBBox = CalculateBBox(rotatedPoints);
                occupiedArea.GetBBox().SetWidth(newBBox.GetWidth());
                occupiedArea.GetBBox().SetHeight(newBBox.GetHeight());
                float heightDiff = height - newBBox.GetHeight();
                Move(0, heightDiff);
            }
        }

        /// <summary>
        /// This method creates
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// instance that could be used
        /// to rotate content inside the occupied area.
        /// </summary>
        /// <remarks>
        /// This method creates
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// instance that could be used
        /// to rotate content inside the occupied area. Be aware that it should be used only after
        /// layout rendering is finished and correct occupied area for the rotated element is calculated.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// that rotates the content and places it inside occupied area.
        /// </returns>
        protected internal virtual AffineTransform CreateRotationTransformInsideOccupiedArea() {
            float? angle = this.GetProperty<float?>(Property.ROTATION_ANGLE);
            AffineTransform rotationTransform = AffineTransform.GetRotateInstance((float)angle);
            Rectangle contentBox = this.GetOccupiedAreaBBox();
            IList<Point> rotatedContentBoxPoints = TransformPoints(RectangleToPointsList(contentBox), rotationTransform
                );
            // Occupied area for rotated elements is already calculated on layout in such way to enclose rotated content;
            // therefore we can simply rotate content as is and then shift it to the occupied area.
            float[] shift = CalculateShiftToPositionBBoxOfPointsAt(occupiedArea.GetBBox().GetLeft(), occupiedArea.GetBBox
                ().GetTop(), rotatedContentBoxPoints);
            rotationTransform.PreConcatenate(AffineTransform.GetTranslateInstance(shift[0], shift[1]));
            return rotationTransform;
        }

        /// <summary>This method starts rotation for the renderer if rotation angle property is specified.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to draw on
        /// </param>
        protected internal virtual void BeginRotationIfApplied(PdfCanvas canvas) {
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null) {
                if (!HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.BlockRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER
                        , GetType().Name));
                }
                else {
                    AffineTransform transform = CreateRotationTransformInsideOccupiedArea();
                    canvas.SaveState().ConcatMatrix(transform);
                }
            }
        }

        /// <summary>This method ends rotation for the renderer if applied.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to draw on
        /// </param>
        protected internal virtual void EndRotationIfApplied(PdfCanvas canvas) {
            float? angle = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (angle != null && HasOwnProperty(Property.ROTATION_INITIAL_HEIGHT)) {
                canvas.RestoreState();
            }
        }

        internal virtual bool StopLayoutingChildrenIfChildResultNotFull(LayoutResult returnResult) {
            return true;
        }

        internal virtual LayoutResult ProcessNotFullChildResult(LayoutContext layoutContext, IDictionary<int, IRenderer
            > waitingFloatsSplitRenderers, IList<IRenderer> waitingOverflowFloatRenderers, bool wasHeightClipped, 
            IList<Rectangle> floatRendererAreas, bool marginsCollapsingEnabled, float clearHeightCorrection, Border
            [] borders, UnitValue[] paddings, IList<Rectangle> areas, int currentAreaPos, Rectangle layoutBox, ICollection
            <Rectangle> nonChildFloatingRendererAreas, IRenderer causeOfNothing, bool anythingPlaced, int childPos
            , LayoutResult result) {
            if (result.GetStatus() == LayoutResult.PARTIAL) {
                if (currentAreaPos + 1 == areas.Count) {
                    AbstractRenderer[] splitAndOverflowRenderers = CreateSplitAndOverflowRenderers(childPos, LayoutResult.PARTIAL
                        , result, waitingFloatsSplitRenderers, waitingOverflowFloatRenderers);
                    AbstractRenderer splitRenderer = splitAndOverflowRenderers[0];
                    AbstractRenderer overflowRenderer = splitAndOverflowRenderers[1];
                    overflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
                    UpdateHeightsOnSplit(wasHeightClipped, splitRenderer, overflowRenderer);
                    ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                    ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                    ApplyMargins(occupiedArea.GetBBox(), true);
                    CorrectFixedLayout(layoutBox);
                    LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                        (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                    if (wasHeightClipped) {
                        return new LayoutResult(LayoutResult.FULL, editedArea, splitRenderer, null);
                    }
                    else {
                        return new LayoutResult(LayoutResult.PARTIAL, editedArea, splitRenderer, overflowRenderer, causeOfNothing);
                    }
                }
                else {
                    childRenderers[childPos] = result.GetSplitRenderer();
                    childRenderers.Add(childPos + 1, result.GetOverflowRenderer());
                    return null;
                }
            }
            else {
                if (result.GetStatus() == LayoutResult.NOTHING) {
                    bool keepTogether = IsKeepTogether(causeOfNothing);
                    int layoutResult = anythingPlaced && !keepTogether ? LayoutResult.PARTIAL : LayoutResult.NOTHING;
                    AbstractRenderer[] splitAndOverflowRenderers = CreateSplitAndOverflowRenderers(childPos, layoutResult, result
                        , waitingFloatsSplitRenderers, waitingOverflowFloatRenderers);
                    AbstractRenderer splitRenderer = splitAndOverflowRenderers[0];
                    AbstractRenderer overflowRenderer = splitAndOverflowRenderers[1];
                    if (IsRelativePosition() && positionedRenderers.Count > 0) {
                        overflowRenderer.positionedRenderers = new List<IRenderer>(positionedRenderers);
                    }
                    UpdateHeightsOnSplit(wasHeightClipped, splitRenderer, overflowRenderer);
                    if (keepTogether) {
                        splitRenderer = null;
                        overflowRenderer.childRenderers.Clear();
                        overflowRenderer.childRenderers = new List<IRenderer>(childRenderers);
                    }
                    CorrectFixedLayout(layoutBox);
                    ApplyPaddings(occupiedArea.GetBBox(), paddings, true);
                    ApplyBorderBox(occupiedArea.GetBBox(), borders, true);
                    ApplyMargins(occupiedArea.GetBBox(), true);
                    ApplyAbsolutePositionIfNeeded(layoutContext);
                    if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) || wasHeightClipped) {
                        LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                            (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                        return new LayoutResult(LayoutResult.FULL, editedArea, splitRenderer, null, null);
                    }
                    else {
                        if (layoutResult != LayoutResult.NOTHING) {
                            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, layoutContext.GetFloatRendererAreas
                                (), layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            return new LayoutResult(layoutResult, editedArea, splitRenderer, overflowRenderer, null).SetAreaBreak(result
                                .GetAreaBreak());
                        }
                        else {
                            floatRendererAreas.RetainAll(nonChildFloatingRendererAreas);
                            return new LayoutResult(layoutResult, null, null, overflowRenderer, result.GetCauseOfNothing()).SetAreaBreak
                                (result.GetAreaBreak());
                        }
                    }
                }
            }
            return null;
        }

        internal virtual void DecreaseLayoutBoxAfterChildPlacement(Rectangle layoutBox, LayoutResult result, IRenderer
             childRenderer) {
            layoutBox.SetHeight(result.GetOccupiedArea().GetBBox().GetY() - layoutBox.GetY());
        }

        internal virtual void CorrectFixedLayout(Rectangle layoutBox) {
            if (IsFixedLayout()) {
                float y = (float)this.GetPropertyAsFloat(Property.BOTTOM);
                Move(0, y - occupiedArea.GetBBox().GetY());
            }
        }

        internal virtual void ApplyWidth(Rectangle parentBBox, float? blockWidth, OverflowPropertyValue? overflowX
            ) {
            // maxWidth has already taken in attention in blockWidth,
            // therefore only `parentBBox > minWidth` needs to be checked.
            float? rotation = this.GetPropertyAsFloat(Property.ROTATION_ANGLE);
            if (blockWidth != null && (blockWidth < parentBBox.GetWidth() || IsPositioned() || rotation != null || (!IsOverflowFit
                (overflowX)))) {
                parentBBox.SetWidth((float)blockWidth);
            }
            else {
                float? minWidth = RetrieveMinWidth(parentBBox.GetWidth());
                //Shall we check overflow-x here?
                if (minWidth != null && minWidth > parentBBox.GetWidth()) {
                    parentBBox.SetWidth((float)minWidth);
                }
            }
        }

        internal virtual bool ApplyMaxHeight(Rectangle parentBBox, float? blockMaxHeight, MarginsCollapseHandler marginsCollapseHandler
            , bool isCellRenderer, bool wasParentsHeightClipped, OverflowPropertyValue? overflowY) {
            if (null == blockMaxHeight || (blockMaxHeight >= parentBBox.GetHeight() && (IsOverflowFit(overflowY)))) {
                return false;
            }
            bool wasHeightClipped = false;
            if (blockMaxHeight <= parentBBox.GetHeight()) {
                wasHeightClipped = true;
            }
            float heightDelta = parentBBox.GetHeight() - (float)blockMaxHeight;
            if (marginsCollapseHandler != null && !isCellRenderer) {
                marginsCollapseHandler.ProcessFixedHeightAdjustment(heightDelta);
            }
            parentBBox.MoveUp(heightDelta).SetHeight((float)blockMaxHeight);
            return wasHeightClipped;
        }

        internal virtual AbstractRenderer ApplyMinHeight(OverflowPropertyValue? overflowY, Rectangle layoutBox) {
            AbstractRenderer overflowRenderer = null;
            float? blockMinHeight = RetrieveMinHeight();
            if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) && null != blockMinHeight && blockMinHeight
                 > occupiedArea.GetBBox().GetHeight()) {
                float blockBottom = occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                    ());
                if (IsFixedLayout()) {
                    occupiedArea.GetBBox().SetY(blockBottom).SetHeight((float)blockMinHeight);
                }
                else {
                    // Because of float precision inaccuracy, iText can incorrectly calculate that the block of fixed height
                    // needs to be split. As a result, an empty block with a height equal to sum of paddings
                    // may appear on the next area. To prevent such situations epsilon is used.
                    if (IsOverflowFit(overflowY) && blockBottom + EPS < layoutBox.GetBottom()) {
                        float hDelta = occupiedArea.GetBBox().GetBottom() - layoutBox.GetBottom();
                        occupiedArea.GetBBox().IncreaseHeight(hDelta).SetY(layoutBox.GetBottom());
                        if (occupiedArea.GetBBox().GetHeight() < 0) {
                            occupiedArea.GetBBox().SetHeight(0);
                        }
                        this.isLastRendererForModelElement = false;
                        overflowRenderer = CreateOverflowRenderer(LayoutResult.PARTIAL);
                        overflowRenderer.UpdateMinHeight(UnitValue.CreatePointValue((float)blockMinHeight - occupiedArea.GetBBox()
                            .GetHeight()));
                        if (HasProperty(Property.HEIGHT)) {
                            overflowRenderer.UpdateHeight(UnitValue.CreatePointValue((float)RetrieveHeight() - occupiedArea.GetBBox().
                                GetHeight()));
                        }
                    }
                    else {
                        occupiedArea.GetBBox().SetY(blockBottom).SetHeight((float)blockMinHeight);
                    }
                }
            }
            return overflowRenderer;
        }

        internal virtual void FixOccupiedAreaIfOverflowedX(OverflowPropertyValue? overflowX, Rectangle layoutBox) {
            if (IsOverflowFit(overflowX)) {
                return;
            }
            if ((occupiedArea.GetBBox().GetWidth() > layoutBox.GetWidth() || occupiedArea.GetBBox().GetLeft() < layoutBox
                .GetLeft())) {
                occupiedArea.GetBBox().SetX(layoutBox.GetX()).SetWidth(layoutBox.GetWidth());
            }
        }

        internal virtual void FixOccupiedAreaIfOverflowedY(OverflowPropertyValue? overflowY, Rectangle layoutBox) {
            if (IsOverflowFit(overflowY)) {
                return;
            }
            if (occupiedArea.GetBBox().GetBottom() < layoutBox.GetBottom()) {
                float difference = layoutBox.GetBottom() - occupiedArea.GetBBox().GetBottom();
                occupiedArea.GetBBox().MoveUp(difference).DecreaseHeight(difference);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override MinMaxWidth GetMinMaxWidth() {
            MinMaxWidth minMaxWidth = new MinMaxWidth(CalculateAdditionalWidth(this));
            if (!SetMinMaxWidthBasedOnFixedWidth(minMaxWidth)) {
                float? minWidth = HasAbsoluteUnitValue(Property.MIN_WIDTH) ? RetrieveMinWidth(0) : null;
                float? maxWidth = HasAbsoluteUnitValue(Property.MAX_WIDTH) ? RetrieveMaxWidth(0) : null;
                if (minWidth == null || maxWidth == null) {
                    AbstractWidthHandler handler = new MaxMaxWidthHandler(minMaxWidth);
                    int epsilonNum = 0;
                    int curEpsNum = 0;
                    float previousFloatingChildWidth = 0;
                    foreach (IRenderer childRenderer in childRenderers) {
                        MinMaxWidth childMinMaxWidth;
                        childRenderer.SetParent(this);
                        if (childRenderer is AbstractRenderer) {
                            childMinMaxWidth = ((AbstractRenderer)childRenderer).GetMinMaxWidth();
                        }
                        else {
                            childMinMaxWidth = MinMaxWidthUtils.CountDefaultMinMaxWidth(childRenderer);
                        }
                        handler.UpdateMaxChildWidth(childMinMaxWidth.GetMaxWidth() + (FloatingHelper.IsRendererFloating(childRenderer
                            ) ? previousFloatingChildWidth : 0));
                        handler.UpdateMinChildWidth(childMinMaxWidth.GetMinWidth());
                        previousFloatingChildWidth = FloatingHelper.IsRendererFloating(childRenderer) ? previousFloatingChildWidth
                             + childMinMaxWidth.GetMaxWidth() : 0;
                        if (FloatingHelper.IsRendererFloating(childRenderer)) {
                            curEpsNum++;
                        }
                        else {
                            epsilonNum = Math.Max(epsilonNum, curEpsNum);
                            curEpsNum = 0;
                        }
                    }
                    epsilonNum = Math.Max(epsilonNum, curEpsNum);
                    handler.minMaxWidth.SetChildrenMaxWidth(handler.minMaxWidth.GetChildrenMaxWidth() + epsilonNum * AbstractRenderer
                        .EPS);
                    handler.minMaxWidth.SetChildrenMinWidth(handler.minMaxWidth.GetChildrenMinWidth() + epsilonNum * AbstractRenderer
                        .EPS);
                }
                if (minWidth != null) {
                    minMaxWidth.SetChildrenMinWidth((float)minWidth);
                }
                // if max-width was defined explicitly, it shouldn't be overwritten
                if (maxWidth != null) {
                    minMaxWidth.SetChildrenMaxWidth((float)maxWidth);
                }
                else {
                    if (minMaxWidth.GetChildrenMinWidth() > minMaxWidth.GetChildrenMaxWidth()) {
                        minMaxWidth.SetChildrenMaxWidth(minMaxWidth.GetChildrenMinWidth());
                    }
                }
            }
            if (this.GetPropertyAsFloat(Property.ROTATION_ANGLE) != null) {
                return RotationUtils.CountRotationMinMaxWidth(minMaxWidth, this);
            }
            return minMaxWidth;
        }

        internal virtual void HandleForcedPlacement(bool anythingPlaced) {
            // We placed something meaning that we don't need this property anymore while processing other children
            // to do not force place them
            if (anythingPlaced && HasOwnProperty(Property.FORCED_PLACEMENT)) {
                DeleteOwnProperty(Property.FORCED_PLACEMENT);
            }
        }

        private void ReplaceSplitRendererKidFloats(IDictionary<int, IRenderer> waitingFloatsSplitRenderers, IRenderer
             splitRenderer) {
            foreach (KeyValuePair<int, IRenderer> waitingSplitRenderer in waitingFloatsSplitRenderers) {
                if (waitingSplitRenderer.Value != null) {
                    splitRenderer.GetChildRenderers()[waitingSplitRenderer.Key] = waitingSplitRenderer.Value;
                }
                else {
                    splitRenderer.GetChildRenderers()[(int)waitingSplitRenderer.Key] = null;
                }
            }
            for (int i = splitRenderer.GetChildRenderers().Count - 1; i >= 0; --i) {
                if (splitRenderer.GetChildRenderers()[i] == null) {
                    splitRenderer.GetChildRenderers().JRemoveAt(i);
                }
            }
        }

        private void AddMarkedContent(DrawContext drawContext, bool isBegin) {
            if (true.Equals(this.GetProperty<bool?>(Property.ADD_MARKED_CONTENT_TEXT))) {
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isBegin) {
                    canvas.BeginVariableText().SaveState().EndPath();
                }
                else {
                    canvas.RestoreState().EndVariableText();
                }
            }
        }

        private IList<Point> ClipPolygon(IList<Point> points, Point clipLineBeg, Point clipLineEnd) {
            IList<Point> filteredPoints = new List<Point>();
            bool prevOnRightSide = false;
            Point filteringPoint = points[0];
            if (CheckPointSide(filteringPoint, clipLineBeg, clipLineEnd) >= 0) {
                filteredPoints.Add(filteringPoint);
                prevOnRightSide = true;
            }
            Point prevPoint = filteringPoint;
            for (int i = 1; i < points.Count + 1; ++i) {
                filteringPoint = points[i % points.Count];
                if (CheckPointSide(filteringPoint, clipLineBeg, clipLineEnd) >= 0) {
                    if (!prevOnRightSide) {
                        filteredPoints.Add(GetIntersectionPoint(prevPoint, filteringPoint, clipLineBeg, clipLineEnd));
                    }
                    filteredPoints.Add(filteringPoint);
                    prevOnRightSide = true;
                }
                else {
                    if (prevOnRightSide) {
                        filteredPoints.Add(GetIntersectionPoint(prevPoint, filteringPoint, clipLineBeg, clipLineEnd));
                    }
                }
                prevPoint = filteringPoint;
            }
            return filteredPoints;
        }

        private int CheckPointSide(Point filteredPoint, Point clipLineBeg, Point clipLineEnd) {
            double x1;
            double x2;
            double y1;
            double y2;
            x1 = filteredPoint.GetX() - clipLineBeg.GetX();
            y2 = clipLineEnd.GetY() - clipLineBeg.GetY();
            x2 = clipLineEnd.GetX() - clipLineBeg.GetX();
            y1 = filteredPoint.GetY() - clipLineBeg.GetY();
            double sgn = x1 * y2 - x2 * y1;
            if (Math.Abs(sgn) < 0.001) {
                return 0;
            }
            if (sgn > 0) {
                return 1;
            }
            if (sgn < 0) {
                return -1;
            }
            return 0;
        }

        private Point GetIntersectionPoint(Point lineBeg, Point lineEnd, Point clipLineBeg, Point clipLineEnd) {
            double A1 = lineBeg.GetY() - lineEnd.GetY();
            double A2 = clipLineBeg.GetY() - clipLineEnd.GetY();
            double B1 = lineEnd.GetX() - lineBeg.GetX();
            double B2 = clipLineEnd.GetX() - clipLineBeg.GetX();
            double C1 = lineBeg.GetX() * lineEnd.GetY() - lineBeg.GetY() * lineEnd.GetX();
            double C2 = clipLineBeg.GetX() * clipLineEnd.GetY() - clipLineBeg.GetY() * clipLineEnd.GetX();
            double M = B1 * A2 - B2 * A1;
            return new Point((B2 * C1 - B1 * C2) / M, (C2 * A1 - C1 * A2) / M);
        }
    }
}
