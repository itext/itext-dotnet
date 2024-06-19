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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class LineRenderer : AbstractRenderer {
        // AbstractRenderer.EPS is not enough here
        private const float MIN_MAX_WIDTH_CORRECTION_EPS = 0.001f;

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(LineRenderer));

        protected internal float maxAscent;

        protected internal float maxDescent;

        // bidi levels
        protected internal byte[] levels;

//\cond DO_NOT_DOCUMENT
        internal float maxTextAscent;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float maxTextDescent;
//\endcond

        private float maxBlockAscent;

        private float maxBlockDescent;

        public override LayoutResult Layout(LayoutContext layoutContext) {
            bool textSequenceOverflowXProcessing = false;
            int firstChildToRelayout = -1;
            Rectangle layoutBox = layoutContext.GetArea().GetBBox().Clone();
            bool wasParentsHeightClipped = layoutContext.IsClippedHeight();
            IList<Rectangle> floatRendererAreas = layoutContext.GetFloatRendererAreas();
            OverflowPropertyValue? oldXOverflow = null;
            bool wasXOverflowChanged = false;
            bool floatsPlacedBeforeLine = false;
            if (floatRendererAreas != null) {
                float layoutWidth = layoutBox.GetWidth();
                float layoutHeight = layoutBox.GetHeight();
                // consider returning some value to check if layoutBox has been changed due to floats,
                // than reuse on non-float layout: kind of not first piece of content on the line
                FloatingHelper.AdjustLineAreaAccordingToFloats(floatRendererAreas, layoutBox);
                if (layoutWidth > layoutBox.GetWidth() || layoutHeight > layoutBox.GetHeight()) {
                    floatsPlacedBeforeLine = true;
                    oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                    wasXOverflowChanged = true;
                    SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                }
            }
            bool noSoftWrap = true.Equals(this.GetOwnProperty<bool?>(Property.NO_SOFT_WRAP_INLINE));
            LineLayoutContext lineLayoutContext = layoutContext is LineLayoutContext ? (LineLayoutContext)layoutContext
                 : new LineLayoutContext(layoutContext);
            if (lineLayoutContext.GetTextIndent() != 0) {
                layoutBox.MoveRight(lineLayoutContext.GetTextIndent()).SetWidth(layoutBox.GetWidth() - lineLayoutContext.GetTextIndent
                    ());
            }
            occupiedArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), layoutBox.Clone().MoveUp(layoutBox.
                GetHeight()).SetHeight(0).SetWidth(0));
            UpdateChildrenParent();
            TargetCounterHandler.AddPageByID(this);
            float curWidth = 0;
            if (RenderingMode.HTML_MODE.Equals(this.GetProperty<RenderingMode?>(Property.RENDERING_MODE)) && HasChildRendererInHtmlMode
                ()) {
                float[] ascenderDescender = LineHeightHelper.GetActualAscenderDescender(this);
                maxAscent = ascenderDescender[0];
                maxDescent = ascenderDescender[1];
            }
            else {
                maxAscent = 0;
                maxDescent = 0;
            }
            maxTextAscent = 0;
            maxTextDescent = 0;
            maxBlockAscent = -1e20f;
            maxBlockDescent = 1e20f;
            int childPos = 0;
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            AbstractWidthHandler widthHandler;
            if (noSoftWrap) {
                widthHandler = new SumSumWidthHandler(minMaxWidth);
            }
            else {
                widthHandler = new MaxSumWidthHandler(minMaxWidth);
            }
            ResolveChildrenFonts();
            int totalNumberOfTrimmedGlyphs = TrimFirst();
            BaseDirection? baseDirection = ApplyOtf();
            UpdateBidiLevels(totalNumberOfTrimmedGlyphs, baseDirection);
            bool anythingPlaced = false;
            TabStop hangingTabStop = null;
            LineLayoutResult result = null;
            bool floatsPlacedInLine = false;
            IDictionary<int, IRenderer> floatsToNextPageSplitRenderers = new LinkedDictionary<int, IRenderer>();
            IList<IRenderer> floatsToNextPageOverflowRenderers = new List<IRenderer>();
            IList<IRenderer> floatsOverflowedToNextLine = new List<IRenderer>();
            int lastTabIndex = 0;
            IDictionary<int, LayoutResult> specialScriptLayoutResults = new Dictionary<int, LayoutResult>();
            IDictionary<int, LayoutResult> textRendererLayoutResults = new Dictionary<int, LayoutResult>();
            IDictionary<int, float[]> textRendererSequenceAscentDescent = new Dictionary<int, float[]>();
            LineRenderer.LineAscentDescentState lineAscentDescentStateBeforeTextRendererSequence = null;
            TextSequenceWordWrapping.MinMaxWidthOfTextRendererSequenceHelper minMaxWidthOfTextRendererSequenceHelper = 
                null;
            while (childPos < GetChildRenderers().Count) {
                IRenderer childRenderer = GetChildRenderers()[childPos];
                LayoutResult childResult = null;
                Rectangle bbox = new Rectangle(layoutBox.GetX() + curWidth, layoutBox.GetY(), layoutBox.GetWidth() - curWidth
                    , layoutBox.GetHeight());
                RenderingMode? childRenderingMode = childRenderer.GetProperty<RenderingMode?>(Property.RENDERING_MODE);
                if (TextSequenceWordWrapping.IsTextRendererAndRequiresSpecialScriptPreLayoutProcessing(childRenderer) && TypographyUtils
                    .IsPdfCalligraphAvailable()) {
                    TextSequenceWordWrapping.ProcessSpecialScriptPreLayout(this, childPos);
                }
                TextSequenceWordWrapping.ResetTextSequenceIfItEnded(specialScriptLayoutResults, true, childRenderer, childPos
                    , minMaxWidthOfTextRendererSequenceHelper, noSoftWrap, widthHandler);
                TextSequenceWordWrapping.ResetTextSequenceIfItEnded(textRendererLayoutResults, false, childRenderer, childPos
                    , minMaxWidthOfTextRendererSequenceHelper, noSoftWrap, widthHandler);
                if (childRenderer is TextRenderer) {
                    // Delete these properties in case of relayout. We might have applied them during justify().
                    childRenderer.DeleteOwnProperty(Property.CHARACTER_SPACING);
                    childRenderer.DeleteOwnProperty(Property.WORD_SPACING);
                }
                else {
                    if (childRenderer is TabRenderer) {
                        if (hangingTabStop != null) {
                            IRenderer tabRenderer = GetChildRenderers()[childPos - 1];
                            tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox), wasParentsHeightClipped
                                ));
                            curWidth += tabRenderer.GetOccupiedArea().GetBBox().GetWidth();
                            widthHandler.UpdateMaxChildWidth(tabRenderer.GetOccupiedArea().GetBBox().GetWidth());
                        }
                        hangingTabStop = CalculateTab(childRenderer, curWidth, layoutBox.GetWidth());
                        if (childPos == GetChildRenderers().Count - 1) {
                            hangingTabStop = null;
                        }
                        if (hangingTabStop != null) {
                            lastTabIndex = childPos;
                            ++childPos;
                            continue;
                        }
                    }
                }
                if (hangingTabStop != null && hangingTabStop.GetTabAlignment() == TabAlignment.ANCHOR && childRenderer is 
                    TextRenderer) {
                    childRenderer.SetProperty(Property.TAB_ANCHOR, hangingTabStop.GetTabAnchor());
                }
                // Normalize child width
                Object childWidth = childRenderer.GetProperty<Object>(Property.WIDTH);
                bool childWidthWasReplaced = false;
                bool childRendererHasOwnWidthProperty = childRenderer.HasOwnProperty(Property.WIDTH);
                if (childWidth is UnitValue && ((UnitValue)childWidth).IsPercentValue()) {
                    float normalizedChildWidth = ((UnitValue)childWidth).GetValue() / 100 * layoutContext.GetArea().GetBBox().
                        GetWidth();
                    normalizedChildWidth = DecreaseRelativeWidthByChildAdditionalWidth(childRenderer, normalizedChildWidth);
                    if (normalizedChildWidth > 0) {
                        childRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(normalizedChildWidth));
                        childWidthWasReplaced = true;
                    }
                }
                FloatPropertyValue? kidFloatPropertyVal = childRenderer.GetProperty<FloatPropertyValue?>(Property.FLOAT);
                bool isChildFloating = childRenderer is AbstractRenderer && FloatingHelper.IsRendererFloating(childRenderer
                    , kidFloatPropertyVal);
                if (isChildFloating) {
                    childResult = null;
                    MinMaxWidth kidMinMaxWidth = FloatingHelper.CalculateMinMaxWidthForFloat((AbstractRenderer)childRenderer, 
                        kidFloatPropertyVal);
                    float floatingBoxFullWidth = kidMinMaxWidth.GetMaxWidth();
                    // Width will be recalculated on float layout;
                    // also not taking it into account (i.e. not setting it on child renderer) results in differences with html
                    // when floating span is split on other line;
                    // TODO DEVSIX-1730: may be process floating spans as inline blocks always?
                    if (!wasXOverflowChanged && childPos > 0) {
                        oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                        wasXOverflowChanged = true;
                        SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                    }
                    if (!lineLayoutContext.IsFloatOverflowedToNextPageWithNothing() && floatsOverflowedToNextLine.IsEmpty() &&
                         (!anythingPlaced || floatingBoxFullWidth <= bbox.GetWidth())) {
                        childResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(
                            ), layoutContext.GetArea().GetBBox().Clone()), null, floatRendererAreas, wasParentsHeightClipped));
                    }
                    // Get back child width so that it's not lost
                    if (childWidthWasReplaced) {
                        if (childRendererHasOwnWidthProperty) {
                            childRenderer.SetProperty(Property.WIDTH, childWidth);
                        }
                        else {
                            childRenderer.DeleteOwnProperty(Property.WIDTH);
                        }
                    }
                    float minChildWidth = 0;
                    float maxChildWidth = 0;
                    if (childResult is MinMaxWidthLayoutResult) {
                        if (!childWidthWasReplaced) {
                            minChildWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMinWidth();
                        }
                        maxChildWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMaxWidth();
                        widthHandler.UpdateMinChildWidth(minChildWidth + AbstractRenderer.EPS);
                        widthHandler.UpdateMaxChildWidth(maxChildWidth + AbstractRenderer.EPS);
                    }
                    else {
                        widthHandler.UpdateMinChildWidth(kidMinMaxWidth.GetMinWidth() + AbstractRenderer.EPS);
                        widthHandler.UpdateMaxChildWidth(kidMinMaxWidth.GetMaxWidth() + AbstractRenderer.EPS);
                    }
                    if (childResult == null && !lineLayoutContext.IsFloatOverflowedToNextPageWithNothing()) {
                        floatsOverflowedToNextLine.Add(childRenderer);
                    }
                    else {
                        if (lineLayoutContext.IsFloatOverflowedToNextPageWithNothing() || childResult.GetStatus() == LayoutResult.
                            NOTHING) {
                            floatsToNextPageSplitRenderers.Put(childPos, null);
                            floatsToNextPageOverflowRenderers.Add(childRenderer);
                            lineLayoutContext.SetFloatOverflowedToNextPageWithNothing(true);
                        }
                        else {
                            if (childResult.GetStatus() == LayoutResult.PARTIAL) {
                                floatsPlacedInLine = true;
                                if (childRenderer is TextRenderer) {
                                    // This code is specifically for floating inline text elements:
                                    // inline elements cannot have fixed width, also they progress horizontally, which means
                                    // that if they don't fit in one line, they will definitely be moved onto the new line (and also
                                    // under all floats). Specifying the whole width of layout area is required to avoid possible normal
                                    // content wrapping around floating text in case floating text gets wrapped onto the next line
                                    // not evenly.
                                    LineRenderer[] split = SplitNotFittingFloat(childPos, childResult);
                                    IRenderer splitRenderer = childResult.GetSplitRenderer();
                                    if (splitRenderer is TextRenderer) {
                                        ((TextRenderer)splitRenderer).TrimFirst();
                                        ((TextRenderer)splitRenderer).TrimLast();
                                    }
                                    // ensure no other thing (like text wrapping the float) will occupy the line
                                    splitRenderer.GetOccupiedArea().GetBBox().SetWidth(layoutContext.GetArea().GetBBox().GetWidth());
                                    result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], null);
                                    break;
                                }
                                else {
                                    floatsToNextPageSplitRenderers.Put(childPos, childResult.GetSplitRenderer());
                                    floatsToNextPageOverflowRenderers.Add(childResult.GetOverflowRenderer());
                                    AdjustLineOnFloatPlaced(layoutBox, childPos, kidFloatPropertyVal, childResult.GetSplitRenderer().GetOccupiedArea
                                        ().GetBBox());
                                }
                            }
                            else {
                                floatsPlacedInLine = true;
                                if (childRenderer is TextRenderer) {
                                    ((TextRenderer)childRenderer).TrimFirst();
                                    ((TextRenderer)childRenderer).TrimLast();
                                }
                                AdjustLineOnFloatPlaced(layoutBox, childPos, kidFloatPropertyVal, childRenderer.GetOccupiedArea().GetBBox(
                                    ));
                            }
                        }
                    }
                    childPos++;
                    if (!anythingPlaced && childResult != null && childResult.GetStatus() == LayoutResult.NOTHING && floatRendererAreas
                        .IsEmpty()) {
                        if (IsFirstOnRootArea()) {
                            // Current line is empty, kid returns nothing and neither floats nor content
                            // were met on root area (e.g. page area) - return NOTHING, don't layout other line content,
                            // expect FORCED_PLACEMENT to be set.
                            break;
                        }
                    }
                    continue;
                }
                MinMaxWidth childBlockMinMaxWidth = null;
                bool isInlineBlockChild = IsInlineBlockChild(childRenderer);
                if (isInlineBlockChild && childRenderer is AbstractRenderer) {
                    MinMaxWidth childBlockMinMaxWidthLocal = ((AbstractRenderer)childRenderer).GetMinMaxWidth();
                    // Don't calculate childBlockMinMaxWidth in case of relative width here
                    // and further (childBlockMinMaxWidth != null)
                    if (!childWidthWasReplaced) {
                        childBlockMinMaxWidth = childBlockMinMaxWidthLocal;
                    }
                    float childMaxWidth = childBlockMinMaxWidthLocal.GetMaxWidth();
                    float lineFullAvailableWidth = layoutContext.GetArea().GetBBox().GetWidth() - lineLayoutContext.GetTextIndent
                        ();
                    if (!noSoftWrap && childMaxWidth > bbox.GetWidth() + MIN_MAX_WIDTH_CORRECTION_EPS && bbox.GetWidth() != lineFullAvailableWidth
                        ) {
                        childResult = new LineLayoutResult(LayoutResult.NOTHING, null, null, childRenderer, childRenderer);
                    }
                    else {
                        if (childBlockMinMaxWidth != null) {
                            childMaxWidth += MIN_MAX_WIDTH_CORRECTION_EPS;
                            float inlineBlockWidth = Math.Min(childMaxWidth, lineFullAvailableWidth);
                            if (!IsOverflowFit(this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X))) {
                                float childMinWidth = childBlockMinMaxWidth.GetMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS;
                                inlineBlockWidth = Math.Max(childMinWidth, inlineBlockWidth);
                            }
                            bbox.SetWidth(inlineBlockWidth);
                            if (childBlockMinMaxWidth.GetMinWidth() > bbox.GetWidth()) {
                                if (logger.IsEnabled(LogLevel.Warning)) {
                                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED);
                                }
                                childRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                            }
                        }
                    }
                    if (childBlockMinMaxWidth != null) {
                        childBlockMinMaxWidth.SetChildrenMaxWidth(childBlockMinMaxWidth.GetChildrenMaxWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                        childBlockMinMaxWidth.SetChildrenMinWidth(childBlockMinMaxWidth.GetChildrenMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                    }
                }
                bool shouldBreakLayouting = false;
                if (childResult == null) {
                    bool setOverflowFitCausedBySpecialScripts = childRenderer is TextRenderer && ((TextRenderer)childRenderer)
                        .TextContainsSpecialScriptGlyphs(true);
                    bool setOverflowFitCausedByTextRendererInHtmlMode = RenderingMode.HTML_MODE == childRenderingMode && childRenderer
                         is TextRenderer && !((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true);
                    if (!wasXOverflowChanged && (childPos > 0 || setOverflowFitCausedBySpecialScripts || setOverflowFitCausedByTextRendererInHtmlMode
                        ) && !textSequenceOverflowXProcessing) {
                        oldXOverflow = this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X);
                        wasXOverflowChanged = true;
                        SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
                    }
                    TextSequenceWordWrapping.PreprocessTextSequenceOverflowX(this, textSequenceOverflowXProcessing, childRenderer
                        , wasXOverflowChanged, oldXOverflow);
                    childResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(
                        ), bbox), wasParentsHeightClipped));
                    shouldBreakLayouting = TextSequenceWordWrapping.PostprocessTextSequenceOverflowX(this, textSequenceOverflowXProcessing
                        , childPos, childRenderer, childResult, wasXOverflowChanged);
                    TextSequenceWordWrapping.UpdateTextSequenceLayoutResults(textRendererLayoutResults, false, childRenderer, 
                        childPos, childResult);
                    TextSequenceWordWrapping.UpdateTextSequenceLayoutResults(specialScriptLayoutResults, true, childRenderer, 
                        childPos, childResult);
                    // it means that we've already increased layout area by MIN_MAX_WIDTH_CORRECTION_EPS
                    if (childResult is MinMaxWidthLayoutResult && null != childBlockMinMaxWidth) {
                        MinMaxWidth childResultMinMaxWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth();
                        childResultMinMaxWidth.SetChildrenMaxWidth(childResultMinMaxWidth.GetChildrenMaxWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                        childResultMinMaxWidth.SetChildrenMinWidth(childResultMinMaxWidth.GetChildrenMinWidth() + MIN_MAX_WIDTH_CORRECTION_EPS
                            );
                    }
                }
                // Get back child width so that it's not lost
                if (childWidthWasReplaced) {
                    if (childRendererHasOwnWidthProperty) {
                        childRenderer.SetProperty(Property.WIDTH, childWidth);
                    }
                    else {
                        childRenderer.DeleteOwnProperty(Property.WIDTH);
                    }
                }
                float minChildWidth_1 = 0;
                float maxChildWidth_1 = 0;
                if (childResult is MinMaxWidthLayoutResult) {
                    if (!childWidthWasReplaced) {
                        minChildWidth_1 = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMinWidth();
                    }
                    maxChildWidth_1 = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth().GetMaxWidth();
                }
                else {
                    if (childBlockMinMaxWidth != null) {
                        minChildWidth_1 = childBlockMinMaxWidth.GetMinWidth();
                        maxChildWidth_1 = childBlockMinMaxWidth.GetMaxWidth();
                    }
                }
                float[] childAscentDescent = GetAscentDescentOfLayoutedChildRenderer(childRenderer, childResult, childRenderingMode
                    , isInlineBlockChild);
                lineAscentDescentStateBeforeTextRendererSequence = TextSequenceWordWrapping.UpdateTextRendererSequenceAscentDescent
                    (this, textRendererSequenceAscentDescent, childPos, childAscentDescent, lineAscentDescentStateBeforeTextRendererSequence
                    );
                minMaxWidthOfTextRendererSequenceHelper = TextSequenceWordWrapping.UpdateTextRendererSequenceMinMaxWidth(this
                    , widthHandler, childPos, minMaxWidthOfTextRendererSequenceHelper, anythingPlaced, textRendererLayoutResults
                    , specialScriptLayoutResults, lineLayoutContext.GetTextIndent());
                bool newLineOccurred = (childResult is TextLayoutResult && ((TextLayoutResult)childResult).IsSplitForcedByNewline
                    ());
                if (!shouldBreakLayouting) {
                    shouldBreakLayouting = childResult.GetStatus() != LayoutResult.FULL || newLineOccurred;
                }
                bool shouldBreakLayoutingOnTextRenderer = shouldBreakLayouting && childResult is TextLayoutResult;
                bool forceOverflowForTextRendererPartialResult = false;
                if (shouldBreakLayoutingOnTextRenderer) {
                    bool isWordHasBeenSplitLayoutRenderingMode = ((TextLayoutResult)childResult).IsWordHasBeenSplit() && RenderingMode
                        .HTML_MODE != childRenderingMode && !((TextRenderer)childRenderer).TextContainsSpecialScriptGlyphs(true
                        );
                    bool enableSpecialScriptsWrapping = ((TextRenderer)GetChildRenderers()[childPos]).TextContainsSpecialScriptGlyphs
                        (true) && !textSequenceOverflowXProcessing && !newLineOccurred;
                    bool enableTextSequenceWrapping = RenderingMode.HTML_MODE == childRenderingMode && !newLineOccurred && !textSequenceOverflowXProcessing;
                    if (isWordHasBeenSplitLayoutRenderingMode) {
                        forceOverflowForTextRendererPartialResult = IsForceOverflowForTextRendererPartialResult(childRenderer, wasXOverflowChanged
                            , oldXOverflow, layoutContext, layoutBox, wasParentsHeightClipped);
                    }
                    else {
                        if (enableSpecialScriptsWrapping) {
                            bool isOverflowFit = wasXOverflowChanged ? (oldXOverflow == OverflowPropertyValue.FIT) : IsOverflowFit(this
                                .GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X));
                            TextSequenceWordWrapping.LastFittingChildRendererData lastFittingChildRendererData = TextSequenceWordWrapping
                                .GetIndexAndLayoutResultOfTheLastTextRendererContainingSpecialScripts(this, childPos, specialScriptLayoutResults
                                , wasParentsHeightClipped, isOverflowFit);
                            if (lastFittingChildRendererData == null) {
                                textSequenceOverflowXProcessing = true;
                                shouldBreakLayouting = false;
                                firstChildToRelayout = childPos;
                            }
                            else {
                                curWidth -= TextSequenceWordWrapping.GetCurWidthRelayoutedTextSequenceDecrement(childPos, lastFittingChildRendererData
                                    .childIndex, specialScriptLayoutResults);
                                childPos = lastFittingChildRendererData.childIndex;
                                childResult = lastFittingChildRendererData.childLayoutResult;
                                specialScriptLayoutResults.Put(childPos, childResult);
                                MinMaxWidth textSequenceElemminMaxWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth();
                                minChildWidth_1 = textSequenceElemminMaxWidth.GetMinWidth();
                                maxChildWidth_1 = textSequenceElemminMaxWidth.GetMaxWidth();
                            }
                        }
                        else {
                            if (enableTextSequenceWrapping) {
                                bool isOverflowFit = wasXOverflowChanged ? (oldXOverflow == OverflowPropertyValue.FIT) : IsOverflowFit(this
                                    .GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X));
                                TextSequenceWordWrapping.LastFittingChildRendererData lastFittingChildRendererData = TextSequenceWordWrapping
                                    .GetIndexAndLayoutResultOfTheLastTextRendererWithNoSpecialScripts(this, childPos, textRendererLayoutResults
                                    , wasParentsHeightClipped, isOverflowFit, floatsPlacedInLine || floatsPlacedBeforeLine);
                                if (lastFittingChildRendererData == null) {
                                    textSequenceOverflowXProcessing = true;
                                    shouldBreakLayouting = false;
                                    firstChildToRelayout = childPos;
                                }
                                else {
                                    curWidth -= TextSequenceWordWrapping.GetCurWidthRelayoutedTextSequenceDecrement(childPos, lastFittingChildRendererData
                                        .childIndex, textRendererLayoutResults);
                                    childAscentDescent = UpdateAscentDescentAfterTextRendererSequenceProcessing((lastFittingChildRendererData.
                                        childLayoutResult.GetStatus() == LayoutResult.NOTHING) ? (lastFittingChildRendererData.childIndex - 1)
                                         : lastFittingChildRendererData.childIndex, lineAscentDescentStateBeforeTextRendererSequence, textRendererSequenceAscentDescent
                                        );
                                    childPos = lastFittingChildRendererData.childIndex;
                                    childResult = lastFittingChildRendererData.childLayoutResult;
                                    if (0 == childPos && LayoutResult.NOTHING == childResult.GetStatus()) {
                                        anythingPlaced = false;
                                    }
                                    textRendererLayoutResults.Put(childPos, childResult);
                                    MinMaxWidth textSequenceElemminMaxWidth = ((MinMaxWidthLayoutResult)childResult).GetMinMaxWidth();
                                    minChildWidth_1 = textSequenceElemminMaxWidth.GetMinWidth();
                                    maxChildWidth_1 = textSequenceElemminMaxWidth.GetMaxWidth();
                                }
                            }
                        }
                    }
                }
                if (childPos != firstChildToRelayout) {
                    if (!forceOverflowForTextRendererPartialResult) {
                        UpdateAscentDescentAfterChildLayout(childAscentDescent, childRenderer, isChildFloating);
                    }
                    float maxHeight = maxAscent - maxDescent;
                    float currChildTextIndent = anythingPlaced ? 0 : lineLayoutContext.GetTextIndent();
                    if (hangingTabStop != null && (TabAlignment.LEFT == hangingTabStop.GetTabAlignment() || shouldBreakLayouting
                         || GetChildRenderers().Count - 1 == childPos || GetChildRenderers()[childPos + 1] is TabRenderer)) {
                        IRenderer tabRenderer = GetChildRenderers()[lastTabIndex];
                        IList<IRenderer> affectedRenderers = new List<IRenderer>();
                        affectedRenderers.AddAll(GetChildRenderers().SubList(lastTabIndex + 1, childPos + 1));
                        float tabWidth = CalculateTab(layoutBox, curWidth, hangingTabStop, affectedRenderers, tabRenderer);
                        tabRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea().GetPageNumber(), bbox), wasParentsHeightClipped
                            ));
                        float sumOfAffectedRendererWidths = 0;
                        foreach (IRenderer renderer in affectedRenderers) {
                            renderer.Move(tabWidth + sumOfAffectedRendererWidths, 0);
                            sumOfAffectedRendererWidths += renderer.GetOccupiedArea().GetBBox().GetWidth();
                        }
                        if (childResult.GetSplitRenderer() != null) {
                            childResult.GetSplitRenderer().Move(tabWidth + sumOfAffectedRendererWidths - childResult.GetSplitRenderer(
                                ).GetOccupiedArea().GetBBox().GetWidth(), 0);
                        }
                        float tabAndNextElemWidth = tabWidth + childResult.GetOccupiedArea().GetBBox().GetWidth();
                        if (hangingTabStop.GetTabAlignment() == TabAlignment.RIGHT && curWidth + tabAndNextElemWidth < hangingTabStop
                            .GetTabPosition()) {
                            curWidth = hangingTabStop.GetTabPosition();
                        }
                        else {
                            curWidth += tabAndNextElemWidth;
                        }
                        widthHandler.UpdateMinChildWidth(minChildWidth_1 + currChildTextIndent);
                        widthHandler.UpdateMaxChildWidth(tabWidth + maxChildWidth_1 + currChildTextIndent);
                        hangingTabStop = null;
                    }
                    else {
                        if (null == hangingTabStop) {
                            if (childResult.GetOccupiedArea() != null && childResult.GetOccupiedArea().GetBBox() != null) {
                                curWidth += childResult.GetOccupiedArea().GetBBox().GetWidth();
                            }
                            widthHandler.UpdateMinChildWidth(minChildWidth_1 + currChildTextIndent);
                            widthHandler.UpdateMaxChildWidth(maxChildWidth_1 + currChildTextIndent);
                        }
                    }
                    if (!forceOverflowForTextRendererPartialResult) {
                        occupiedArea.SetBBox(new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox.GetHeight() - maxHeight, 
                            curWidth, maxHeight));
                    }
                }
                if (shouldBreakLayouting) {
                    LineRenderer[] split = Split();
                    split[0].SetChildRenderers(GetChildRenderers().SubList(0, childPos));
                    if (forceOverflowForTextRendererPartialResult) {
                        split[1].AddChildRenderer(childRenderer);
                    }
                    else {
                        bool forcePlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
                        bool isInlineBlockAndFirstOnRootArea = isInlineBlockChild && IsFirstOnRootArea();
                        if ((childResult.GetStatus() == LayoutResult.PARTIAL && (!isInlineBlockChild || forcePlacement || isInlineBlockAndFirstOnRootArea
                            )) || childResult.GetStatus() == LayoutResult.FULL) {
                            IRenderer splitRenderer = childResult.GetSplitRenderer();
                            split[0].AddChild(splitRenderer);
                            // TODO: DEVSIX-4717 this code should be removed if/when the AbstractRenderer
                            //  would start using the newly added methods
                            if (splitRenderer.GetParent() != split[0] && split[0].childRenderers.Contains(splitRenderer)) {
                                splitRenderer.SetParent(split[0]);
                            }
                            anythingPlaced = true;
                        }
                        if (null != childResult.GetOverflowRenderer()) {
                            if (isInlineBlockChild && !forcePlacement && !isInlineBlockAndFirstOnRootArea) {
                                split[1].AddChildRenderer(childRenderer);
                            }
                            else {
                                if (isInlineBlockChild && childResult.GetOverflowRenderer().GetChildRenderers().IsEmpty() && childResult.GetStatus
                                    () == LayoutResult.PARTIAL) {
                                    if (logger.IsEnabled(LogLevel.Warning)) {
                                        logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED);
                                    }
                                }
                                else {
                                    split[1].AddChildRenderer(childResult.GetOverflowRenderer());
                                }
                            }
                        }
                    }
                    split[1].AddAllChildRenderers(GetChildRenderers().SubList(childPos + 1, GetChildRenderers().Count));
                    ReplaceSplitRendererKidFloats(floatsToNextPageSplitRenderers, split[0]);
                    split[0].RemoveAllChildRenderers(floatsOverflowedToNextLine);
                    split[1].AddAllChildRenderers(0, floatsOverflowedToNextLine);
                    // no sense to process empty renderer
                    if (split[1].GetChildRenderers().IsEmpty() && floatsToNextPageOverflowRenderers.IsEmpty()) {
                        split[1] = null;
                    }
                    IRenderer causeOfNothing = childResult.GetStatus() == LayoutResult.NOTHING ? childResult.GetCauseOfNothing
                        () : GetChildRenderers()[childPos];
                    if (split[1] == null) {
                        result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, split[0], split[1], causeOfNothing);
                    }
                    else {
                        if (anythingPlaced || floatsPlacedInLine) {
                            result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], causeOfNothing);
                        }
                        else {
                            result = new LineLayoutResult(LayoutResult.NOTHING, null, null, split[1], null);
                        }
                    }
                    result.SetFloatsOverflowedToNextPage(floatsToNextPageOverflowRenderers);
                    if (newLineOccurred) {
                        result.SetSplitForcedByNewline(true);
                    }
                    break;
                }
                else {
                    if (childPos == firstChildToRelayout) {
                        firstChildToRelayout = -1;
                    }
                    else {
                        anythingPlaced = true;
                        childPos++;
                    }
                }
            }
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(specialScriptLayoutResults, true, null, childPos, minMaxWidthOfTextRendererSequenceHelper
                , noSoftWrap, widthHandler);
            TextSequenceWordWrapping.ResetTextSequenceIfItEnded(textRendererLayoutResults, false, null, childPos, minMaxWidthOfTextRendererSequenceHelper
                , noSoftWrap, widthHandler);
            if (result == null) {
                bool noOverflowedFloats = floatsOverflowedToNextLine.IsEmpty() && floatsToNextPageOverflowRenderers.IsEmpty
                    ();
                if (((anythingPlaced || floatsPlacedInLine) && noOverflowedFloats) || GetChildRenderers().IsEmpty()) {
                    result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                }
                else {
                    if (noOverflowedFloats) {
                        // all kids were some non-image and non-text kids (tab-stops?),
                        // but in this case, it should be okay to return FULL, as there is nothing to be placed
                        result = new LineLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
                    }
                    else {
                        if (anythingPlaced || floatsPlacedInLine) {
                            LineRenderer[] split = Split();
                            split[0].AddAllChildRenderers(GetChildRenderers().SubList(0, childPos));
                            ReplaceSplitRendererKidFloats(floatsToNextPageSplitRenderers, split[0]);
                            split[0].RemoveAllChildRenderers(floatsOverflowedToNextLine);
                            // If `result` variable is null up until now but not everything was placed - there is no
                            // content overflow, only floats are overflowing.
                            // The floatsOverflowedToNextLine might be empty, while the only overflowing floats are
                            // in floatsToNextPageOverflowRenderers. This situation is handled in ParagraphRenderer separately.
                            split[1].AddAllChildRenderers(floatsOverflowedToNextLine);
                            result = new LineLayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1], null);
                            result.SetFloatsOverflowedToNextPage(floatsToNextPageOverflowRenderers);
                        }
                        else {
                            IRenderer causeOfNothing = floatsOverflowedToNextLine.IsEmpty() ? floatsToNextPageOverflowRenderers[0] : floatsOverflowedToNextLine
                                [0];
                            result = new LineLayoutResult(LayoutResult.NOTHING, null, null, this, causeOfNothing);
                        }
                    }
                }
            }
            LineRenderer toProcess = (LineRenderer)result.GetSplitRenderer();
            if (toProcess == null && result.GetStatus() == LayoutResult.FULL) {
                toProcess = this;
            }
            if (baseDirection != null && baseDirection != BaseDirection.NO_BIDI && toProcess != null) {
                LineRenderer.LineSplitIntoGlyphsData splitIntoGlyphsData = SplitLineIntoGlyphs(toProcess);
                byte[] lineLevels = new byte[splitIntoGlyphsData.GetLineGlyphs().Count];
                if (levels != null) {
                    Array.Copy(levels, 0, lineLevels, 0, splitIntoGlyphsData.GetLineGlyphs().Count);
                }
                int[] newOrder = TypographyUtils.ReorderLine(splitIntoGlyphsData.GetLineGlyphs(), lineLevels, levels);
                if (newOrder != null) {
                    Reorder(toProcess, splitIntoGlyphsData, newOrder);
                    AdjustChildPositionsAfterReordering(toProcess.GetChildRenderers(), occupiedArea.GetBBox().GetLeft());
                }
                if (result.GetStatus() == LayoutResult.PARTIAL && levels != null) {
                    LineRenderer overflow = (LineRenderer)result.GetOverflowRenderer();
                    overflow.levels = new byte[levels.Length - lineLevels.Length];
                    Array.Copy(levels, lineLevels.Length, overflow.levels, 0, overflow.levels.Length);
                    if (overflow.levels.Length == 0) {
                        overflow.levels = null;
                    }
                }
            }
            if (anythingPlaced || floatsPlacedInLine) {
                toProcess.AdjustChildrenYLine().TrimLast();
                result.SetMinMaxWidth(minMaxWidth);
            }
            if (wasXOverflowChanged) {
                SetProperty(Property.OVERFLOW_X, oldXOverflow);
                if (null != result.GetSplitRenderer()) {
                    result.GetSplitRenderer().SetProperty(Property.OVERFLOW_X, oldXOverflow);
                }
                if (null != result.GetOverflowRenderer()) {
                    result.GetOverflowRenderer().SetProperty(Property.OVERFLOW_X, oldXOverflow);
                }
            }
            return result;
        }

        public virtual float GetMaxAscent() {
            return maxAscent;
        }

        public virtual float GetMaxDescent() {
            return maxDescent;
        }

        public virtual float GetYLine() {
            return occupiedArea.GetBBox().GetY() - maxDescent;
        }

        public virtual float GetLeadingValue(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent);
                }

                case Leading.MULTIPLIED: {
                    return GetTopLeadingIndent(leading) + GetBottomLeadingIndent(leading);
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }

        public override IRenderer GetNextRenderer() {
            return new LineRenderer();
        }

        protected internal override float? GetFirstYLineRecursively() {
            return GetYLine();
        }

        protected internal override float? GetLastYLineRecursively() {
            return GetYLine();
        }

        public virtual void Justify(float width) {
            float ratio = (float)this.GetPropertyAsFloat(Property.SPACING_RATIO);
            IRenderer lastChildRenderer = GetLastNonFloatChildRenderer();
            if (lastChildRenderer == null) {
                return;
            }
            float freeWidth = occupiedArea.GetBBox().GetX() + width - lastChildRenderer.GetOccupiedArea().GetBBox().GetX
                () - lastChildRenderer.GetOccupiedArea().GetBBox().GetWidth();
            int numberOfSpaces = GetNumberOfSpaces();
            int baseCharsCount = BaseCharactersCount();
            float baseFactor = freeWidth / (ratio * numberOfSpaces + (1 - ratio) * (baseCharsCount - 1));
            //Prevent a NaN when trying to justify a single word with spacing_ratio == 1.0
            if (float.IsInfinity(baseFactor)) {
                baseFactor = 0;
            }
            float wordSpacing = ratio * baseFactor;
            float characterSpacing = (1 - ratio) * baseFactor;
            float lastRightPos = occupiedArea.GetBBox().GetX();
            foreach (IRenderer child in GetChildRenderers()) {
                if (FloatingHelper.IsRendererFloating(child)) {
                    continue;
                }
                float childX = child.GetOccupiedArea().GetBBox().GetX();
                child.Move(lastRightPos - childX, 0);
                childX = lastRightPos;
                if (child is TextRenderer) {
                    float childHSCale = (float)((TextRenderer)child).GetPropertyAsFloat(Property.HORIZONTAL_SCALING, 1f);
                    float? oldCharacterSpacing = ((TextRenderer)child).GetPropertyAsFloat(Property.CHARACTER_SPACING);
                    float? oldWordSpacing = ((TextRenderer)child).GetPropertyAsFloat(Property.WORD_SPACING);
                    child.SetProperty(Property.CHARACTER_SPACING, (null == oldCharacterSpacing ? 0 : (float)oldCharacterSpacing
                        ) + characterSpacing / childHSCale);
                    child.SetProperty(Property.WORD_SPACING, (null == oldWordSpacing ? 0 : (float)oldWordSpacing) + wordSpacing
                         / childHSCale);
                    bool isLastTextRenderer = child == lastChildRenderer;
                    float widthAddition = (isLastTextRenderer ? (((TextRenderer)child).LineLength() - 1) : ((TextRenderer)child
                        ).LineLength()) * characterSpacing + wordSpacing * ((TextRenderer)child).GetNumberOfSpaces();
                    child.GetOccupiedArea().GetBBox().SetWidth(child.GetOccupiedArea().GetBBox().GetWidth() + widthAddition);
                }
                lastRightPos = childX + child.GetOccupiedArea().GetBBox().GetWidth();
            }
            GetOccupiedArea().GetBBox().SetWidth(width);
        }

        protected internal virtual int GetNumberOfSpaces() {
            int spaces = 0;
            foreach (IRenderer child in GetChildRenderers()) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    spaces += ((TextRenderer)child).GetNumberOfSpaces();
                }
            }
            return spaces;
        }

        /// <summary>Gets the total lengths of characters in this line.</summary>
        /// <remarks>
        /// Gets the total lengths of characters in this line. Other elements (images, tables) are not taken
        /// into account.
        /// </remarks>
        /// <returns>the total lengths of characters in this line.</returns>
        protected internal virtual int Length() {
            int length = 0;
            foreach (IRenderer child in GetChildRenderers()) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    length += ((TextRenderer)child).LineLength();
                }
            }
            return length;
        }

        /// <summary>Returns the number of base characters, i.e. non-mark characters</summary>
        /// <returns>the number of base non-mark characters</returns>
        protected internal virtual int BaseCharactersCount() {
            int count = 0;
            foreach (IRenderer child in GetChildRenderers()) {
                if (child is TextRenderer && !FloatingHelper.IsRendererFloating(child)) {
                    count += ((TextRenderer)child).BaseCharactersCount();
                }
            }
            return count;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (IRenderer renderer in GetChildRenderers()) {
                sb.Append(renderer.ToString());
            }
            return sb.ToString();
        }

        protected internal virtual LineRenderer CreateSplitRenderer() {
            return (LineRenderer)GetNextRenderer();
        }

        protected internal virtual LineRenderer CreateOverflowRenderer() {
            return (LineRenderer)GetNextRenderer();
        }

        protected internal virtual LineRenderer[] Split() {
            LineRenderer splitRenderer = CreateSplitRenderer();
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.maxAscent = maxAscent;
            splitRenderer.maxDescent = maxDescent;
            splitRenderer.maxTextAscent = maxTextAscent;
            splitRenderer.maxTextDescent = maxTextDescent;
            splitRenderer.maxBlockAscent = maxBlockAscent;
            splitRenderer.maxBlockDescent = maxBlockDescent;
            splitRenderer.levels = levels;
            splitRenderer.AddAllProperties(GetOwnProperties());
            LineRenderer overflowRenderer = CreateOverflowRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return new LineRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual LineRenderer AdjustChildrenYLine() {
            if (RenderingMode.HTML_MODE == this.GetProperty<RenderingMode?>(Property.RENDERING_MODE) && HasInlineBlocksWithVerticalAlignment
                ()) {
                InlineVerticalAlignmentHelper.AdjustChildrenYLineHtmlMode(this);
            }
            else {
                AdjustChildrenYLineDefaultMode();
            }
            return this;
        }

        protected internal virtual void ApplyLeading(float deltaY) {
            occupiedArea.GetBBox().MoveUp(deltaY);
            occupiedArea.GetBBox().DecreaseHeight(deltaY);
            foreach (IRenderer child in GetChildRenderers()) {
                if (!FloatingHelper.IsRendererFloating(child)) {
                    child.Move(0, deltaY);
                }
            }
        }

        protected internal virtual LineRenderer TrimLast() {
            int lastIndex = GetChildRenderers().Count;
            IRenderer lastRenderer = null;
            while (--lastIndex >= 0) {
                lastRenderer = GetChildRenderers()[lastIndex];
                if (!FloatingHelper.IsRendererFloating(lastRenderer)) {
                    break;
                }
            }
            if (lastRenderer is TextRenderer && lastIndex >= 0) {
                float trimmedSpace = ((TextRenderer)lastRenderer).TrimLast();
                occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() - trimmedSpace);
            }
            return this;
        }

        public virtual bool ContainsImage() {
            foreach (IRenderer renderer in GetChildRenderers()) {
                if (renderer is ImageRenderer) {
                    return true;
                }
            }
            return false;
        }

        public override MinMaxWidth GetMinMaxWidth() {
            LineLayoutResult result = (LineLayoutResult)Layout(new LayoutContext(new LayoutArea(1, new Rectangle(MinMaxWidthUtils
                .GetInfWidth(), AbstractRenderer.INF))));
            return result.GetMinMaxWidth();
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool HasChildRendererInHtmlMode() {
            foreach (IRenderer childRenderer in GetChildRenderers()) {
                if (RenderingMode.HTML_MODE.Equals(childRenderer.GetProperty<RenderingMode?>(Property.RENDERING_MODE))) {
                    return true;
                }
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float GetTopLeadingIndent(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return (Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent) - occupiedArea.GetBBox().GetHeight(
                        )) / 2;
                }

                case Leading.MULTIPLIED: {
                    UnitValue fontSize = this.GetProperty<UnitValue>(Property.FONT_SIZE, UnitValue.CreatePointValue(0f));
                    if (!fontSize.IsPointValue()) {
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                            , Property.FONT_SIZE));
                    }
                    // In HTML, depending on whether <!DOCTYPE html> is present or not, and if present then depending
                    // on the version, the behavior is different. In one case, bottom leading indent is added for images,
                    // in the other it is not added.
                    // This is why !containsImage() is present below. Depending on the presence of
                    // this !containsImage() condition, the behavior changes between the two possible scenarios in HTML.
                    float textAscent = maxTextAscent == 0 && maxTextDescent == 0 && Math.Abs(maxAscent) + Math.Abs(maxDescent)
                         != 0 && !ContainsImage() ? fontSize.GetValue() * 0.8f : maxTextAscent;
                    float textDescent = maxTextAscent == 0 && maxTextDescent == 0 && Math.Abs(maxAscent) + Math.Abs(maxDescent
                        ) != 0 && !ContainsImage() ? -fontSize.GetValue() * 0.2f : maxTextDescent;
                    return Math.Max(textAscent + ((textAscent - textDescent) * (leading.GetValue() - 1)) / 2, maxBlockAscent) 
                        - maxAscent;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float GetBottomLeadingIndent(Leading leading) {
            switch (leading.GetLeadingType()) {
                case Leading.FIXED: {
                    return (Math.Max(leading.GetValue(), maxBlockAscent - maxBlockDescent) - occupiedArea.GetBBox().GetHeight(
                        )) / 2;
                }

                case Leading.MULTIPLIED: {
                    UnitValue fontSize = this.GetProperty<UnitValue>(Property.FONT_SIZE, UnitValue.CreatePointValue(0f));
                    if (!fontSize.IsPointValue()) {
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                            , Property.FONT_SIZE));
                    }
                    // In HTML, depending on whether <!DOCTYPE html> is present or not, and if present then depending
                    // on the version, the behavior is different. In one case, bottom leading indent is added for images,
                    // in the other it is not added.
                    // This is why !containsImage() is present below. Depending on the presence of
                    // this !containsImage() condition, the behavior changes between the two possible scenarios in HTML.
                    float textAscent = maxTextAscent == 0 && maxTextDescent == 0 && !ContainsImage() ? fontSize.GetValue() * 0.8f
                         : maxTextAscent;
                    float textDescent = maxTextAscent == 0 && maxTextDescent == 0 && !ContainsImage() ? -fontSize.GetValue() *
                         0.2f : maxTextDescent;
                    return Math.Max(-textDescent + ((textAscent - textDescent) * (leading.GetValue() - 1)) / 2, -maxBlockDescent
                        ) + maxDescent;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static LineRenderer.LineSplitIntoGlyphsData SplitLineIntoGlyphs(LineRenderer toSplit) {
            LineRenderer.LineSplitIntoGlyphsData result = new LineRenderer.LineSplitIntoGlyphsData();
            bool newLineFound = false;
            TextRenderer lastTextRenderer = null;
            foreach (IRenderer child in toSplit.GetChildRenderers()) {
                if (newLineFound) {
                    break;
                }
                if (child is TextRenderer) {
                    GlyphLine childLine = ((TextRenderer)child).line;
                    for (int i = childLine.start; i < childLine.end; i++) {
                        if (iText.IO.Util.TextUtil.IsNewLine(childLine.Get(i))) {
                            newLineFound = true;
                            break;
                        }
                        result.AddLineGlyph(new LineRenderer.RendererGlyph(childLine.Get(i), (TextRenderer)child));
                    }
                    lastTextRenderer = (TextRenderer)child;
                }
                else {
                    result.AddInsertAfter(lastTextRenderer, child);
                }
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void Reorder(LineRenderer toProcess, LineRenderer.LineSplitIntoGlyphsData splitLineIntoGlyphsResult
            , int[] newOrder) {
            // Insert non-text renderers
            toProcess.SetChildRenderers(splitLineIntoGlyphsResult.GetStarterNonTextRenderers());
            IList<LineRenderer.RendererGlyph> lineGlyphs = splitLineIntoGlyphsResult.GetLineGlyphs();
            int initialPos = 0;
            for (int offset = initialPos; offset < lineGlyphs.Count; offset = initialPos) {
                TextRenderer renderer = lineGlyphs[offset].renderer;
                TextRenderer newRenderer = new TextRenderer(renderer).RemoveReversedRanges();
                toProcess.AddChildRenderer(newRenderer);
                // Insert non-text renderers
                toProcess.AddAllChildRenderers(splitLineIntoGlyphsResult.GetInsertAfterAndRemove(renderer));
                newRenderer.line = new GlyphLine(newRenderer.line);
                IList<Glyph> replacementGlyphs = new List<Glyph>();
                bool reversed = false;
                for (int pos = offset; pos < lineGlyphs.Count && lineGlyphs[pos].renderer == renderer; ++pos) {
                    replacementGlyphs.Add(lineGlyphs[pos].glyph);
                    if (pos + 1 < lineGlyphs.Count && lineGlyphs[pos + 1].renderer == renderer && newOrder[pos] == newOrder[pos
                         + 1] + 1 && !iText.IO.Util.TextUtil.IsSpaceOrWhitespace(lineGlyphs[pos + 1].glyph) && !iText.IO.Util.TextUtil
                        .IsSpaceOrWhitespace(lineGlyphs[pos].glyph)) {
                        reversed = true;
                        continue;
                    }
                    if (reversed) {
                        newRenderer.InitReversedRanges().Add(new int[] { initialPos - offset, pos - offset });
                        reversed = false;
                    }
                    initialPos = pos + 1;
                }
                newRenderer.line.SetGlyphs(replacementGlyphs);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AdjustChildPositionsAfterReordering(IList<IRenderer> children, float initialXPos) {
            float currentXPos = initialXPos;
            foreach (IRenderer child in children) {
                if (!FloatingHelper.IsRendererFloating(child)) {
                    float currentWidth;
                    if (child is TextRenderer) {
                        currentWidth = ((TextRenderer)child).CalculateLineWidth();
                        UnitValue[] margins = ((TextRenderer)child).GetMargins();
                        if (!margins[1].IsPointValue() && logger.IsEnabled(LogLevel.Error)) {
                            logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                , "right margin"));
                        }
                        if (!margins[3].IsPointValue() && logger.IsEnabled(LogLevel.Error)) {
                            logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                , "left margin"));
                        }
                        UnitValue[] paddings = ((TextRenderer)child).GetPaddings();
                        if (!paddings[1].IsPointValue() && logger.IsEnabled(LogLevel.Error)) {
                            logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                , "right padding"));
                        }
                        if (!paddings[3].IsPointValue() && logger.IsEnabled(LogLevel.Error)) {
                            logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                , "left padding"));
                        }
                        currentWidth += margins[1].GetValue() + margins[3].GetValue() + paddings[1].GetValue() + paddings[3].GetValue
                            ();
                        ((TextRenderer)child).occupiedArea.GetBBox().SetX(currentXPos).SetWidth(currentWidth);
                    }
                    else {
                        currentWidth = child.GetOccupiedArea().GetBBox().GetWidth();
                        child.Move(currentXPos - child.GetOccupiedArea().GetBBox().GetX(), 0);
                    }
                    currentXPos += currentWidth;
                }
            }
        }
//\endcond

        private LineRenderer[] SplitNotFittingFloat(int childPos, LayoutResult childResult) {
            LineRenderer[] split = Split();
            split[0].AddAllChildRenderers(GetChildRenderers().SubList(0, childPos));
            split[0].AddChildRenderer(childResult.GetSplitRenderer());
            split[1].AddChildRenderer(childResult.GetOverflowRenderer());
            split[1].AddAllChildRenderers(GetChildRenderers().SubList(childPos + 1, GetChildRenderers().Count));
            return split;
        }

        private void AdjustLineOnFloatPlaced(Rectangle layoutBox, int childPos, FloatPropertyValue? kidFloatPropertyVal
            , Rectangle justPlacedFloatBox) {
            if (justPlacedFloatBox.GetBottom() >= layoutBox.GetTop() || justPlacedFloatBox.GetTop() < layoutBox.GetTop
                ()) {
                return;
            }
            float floatWidth = justPlacedFloatBox.GetWidth();
            if (kidFloatPropertyVal.Equals(FloatPropertyValue.LEFT)) {
                layoutBox.SetWidth(layoutBox.GetWidth() - floatWidth).MoveRight(floatWidth);
                occupiedArea.GetBBox().MoveRight(floatWidth);
                for (int i = 0; i < childPos; ++i) {
                    IRenderer prevChild = GetChildRenderers()[i];
                    if (!FloatingHelper.IsRendererFloating(prevChild)) {
                        prevChild.Move(floatWidth, 0);
                    }
                }
            }
            else {
                layoutBox.SetWidth(layoutBox.GetWidth() - floatWidth);
            }
        }

        private void ReplaceSplitRendererKidFloats(IDictionary<int, IRenderer> floatsToNextPageSplitRenderers, LineRenderer
             splitRenderer) {
            foreach (KeyValuePair<int, IRenderer> splitFloat in floatsToNextPageSplitRenderers) {
                if (splitFloat.Value != null) {
                    splitRenderer.SetChildRenderer(splitFloat.Key, splitFloat.Value);
                }
                else {
                    splitRenderer.SetChildRenderer(splitFloat.Key, null);
                }
            }
            for (int i = splitRenderer.GetChildRenderers().Count - 1; i >= 0; --i) {
                if (splitRenderer.GetChildRenderers()[i] == null) {
                    splitRenderer.RemoveChildRenderer(i);
                }
            }
        }

        private IRenderer GetLastNonFloatChildRenderer() {
            IRenderer result = null;
            for (int i = GetChildRenderers().Count - 1; i >= 0; --i) {
                IRenderer current = GetChildRenderers()[i];
                if (!FloatingHelper.IsRendererFloating(current)) {
                    result = current;
                    break;
                }
            }
            return result;
        }

        private TabStop GetNextTabStop(float curWidth) {
            SortedDictionary<float, TabStop> tabStops = this.GetProperty<SortedDictionary<float, TabStop>>(Property.TAB_STOPS
                );
            KeyValuePair<float, TabStop>? nextTabStopEntry = null;
            TabStop nextTabStop = null;
            if (tabStops != null) {
                nextTabStopEntry = tabStops.HigherEntry(curWidth);
            }
            if (nextTabStopEntry != null) {
                nextTabStop = ((KeyValuePair<float, TabStop>)nextTabStopEntry).Value;
            }
            return nextTabStop;
        }

        /// <summary>Calculates and sets encountered tab size.</summary>
        /// <remarks>
        /// Calculates and sets encountered tab size.
        /// Returns null, if processing is finished and layout can be performed for the tab renderer;
        /// otherwise, in case when the tab should be processed after the next element in the line,
        /// this method returns corresponding tab stop.
        /// </remarks>
        private TabStop CalculateTab(IRenderer childRenderer, float curWidth, float lineWidth) {
            TabStop nextTabStop = GetNextTabStop(curWidth);
            if (nextTabStop == null) {
                ProcessDefaultTab(childRenderer, curWidth, lineWidth);
                return null;
            }
            childRenderer.SetProperty(Property.TAB_LEADER, nextTabStop.GetTabLeader());
            childRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(nextTabStop.GetTabPosition() - curWidth
                ));
            childRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
            if (nextTabStop.GetTabAlignment() == TabAlignment.LEFT) {
                return null;
            }
            return nextTabStop;
        }

        /// <summary>Calculates and sets tab size with the account of the element that is next in the line after the tab.
        ///     </summary>
        /// <remarks>
        /// Calculates and sets tab size with the account of the element that is next in the line after the tab.
        /// Returns resulting width of the tab.
        /// </remarks>
        private float CalculateTab(Rectangle layoutBox, float curWidth, TabStop tabStop, IList<IRenderer> affectedRenderers
            , IRenderer tabRenderer) {
            float sumOfAffectedRendererWidths = 0;
            foreach (IRenderer renderer in affectedRenderers) {
                sumOfAffectedRendererWidths += renderer.GetOccupiedArea().GetBBox().GetWidth();
            }
            float tabWidth = 0;
            switch (tabStop.GetTabAlignment()) {
                case TabAlignment.RIGHT: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - sumOfAffectedRendererWidths;
                    break;
                }

                case TabAlignment.CENTER: {
                    tabWidth = tabStop.GetTabPosition() - curWidth - sumOfAffectedRendererWidths / 2;
                    break;
                }

                case TabAlignment.ANCHOR: {
                    float anchorPosition = -1;
                    float processedRenderersWidth = 0;
                    foreach (IRenderer renderer in affectedRenderers) {
                        anchorPosition = ((TextRenderer)renderer).GetTabAnchorCharacterPosition();
                        if (-1 != anchorPosition) {
                            break;
                        }
                        else {
                            processedRenderersWidth += renderer.GetOccupiedArea().GetBBox().GetWidth();
                        }
                    }
                    if (anchorPosition == -1) {
                        anchorPosition = 0;
                    }
                    tabWidth = tabStop.GetTabPosition() - curWidth - anchorPosition - processedRenderersWidth;
                    break;
                }
            }
            if (tabWidth < 0) {
                tabWidth = 0;
            }
            if (curWidth + tabWidth + sumOfAffectedRendererWidths > layoutBox.GetWidth()) {
                tabWidth -= (curWidth + sumOfAffectedRendererWidths + tabWidth) - layoutBox.GetWidth();
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
            return tabWidth;
        }

        private void ProcessDefaultTab(IRenderer tabRenderer, float curWidth, float lineWidth) {
            float? tabDefault = this.GetPropertyAsFloat(Property.TAB_DEFAULT);
            float? tabWidth = tabDefault - curWidth % tabDefault;
            if (curWidth + tabWidth > lineWidth) {
                tabWidth = lineWidth - curWidth;
            }
            tabRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue((float)tabWidth));
            tabRenderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(maxAscent - maxDescent));
        }

        private void UpdateChildrenParent() {
            foreach (IRenderer renderer in GetChildRenderers()) {
                renderer.SetParent(this);
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Trim first child text renderers.</summary>
        /// <returns>total number of trimmed glyphs.</returns>
        internal virtual int TrimFirst() {
            int totalNumberOfTrimmedGlyphs = 0;
            foreach (IRenderer renderer in GetChildRenderers()) {
                if (FloatingHelper.IsRendererFloating(renderer)) {
                    continue;
                }
                bool trimFinished;
                if (renderer is TextRenderer) {
                    TextRenderer textRenderer = (TextRenderer)renderer;
                    GlyphLine currentText = textRenderer.GetText();
                    if (currentText != null) {
                        int prevTextStart = currentText.start;
                        textRenderer.TrimFirst();
                        int numOfTrimmedGlyphs = textRenderer.GetText().start - prevTextStart;
                        totalNumberOfTrimmedGlyphs += numOfTrimmedGlyphs;
                    }
                    trimFinished = textRenderer.Length() > 0;
                }
                else {
                    trimFinished = true;
                }
                if (trimFinished) {
                    break;
                }
            }
            return totalNumberOfTrimmedGlyphs;
        }
//\endcond

        /// <summary>Apply OTF features and return the last(!) base direction of child renderer</summary>
        /// <returns>the last(!) base direction of child renderer.</returns>
        private BaseDirection? ApplyOtf() {
            BaseDirection? baseDirection = this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
            foreach (IRenderer renderer in GetChildRenderers()) {
                if (renderer is TextRenderer) {
                    ((TextRenderer)renderer).ApplyOtf();
                    if (baseDirection == null || baseDirection == BaseDirection.NO_BIDI) {
                        baseDirection = renderer.GetOwnProperty<BaseDirection?>(Property.BASE_DIRECTION);
                    }
                }
            }
            return baseDirection;
        }

//\cond DO_NOT_DOCUMENT
        internal static bool IsChildFloating(IRenderer childRenderer) {
            FloatPropertyValue? kidFloatPropertyVal = childRenderer.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            return childRenderer is AbstractRenderer && FloatingHelper.IsRendererFloating(childRenderer, kidFloatPropertyVal
                );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool IsInlineBlockChild(IRenderer child) {
            return child is BlockRenderer || child is TableRenderer;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if the word that's been split when has been layouted on this line can fit the next line without splitting.
        ///     </summary>
        /// <param name="childRenderer">the childRenderer containing the split word</param>
        /// <param name="wasXOverflowChanged">
        /// true if
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// has been changed
        /// during layouting of
        /// <see cref="LineRenderer"/>
        /// </param>
        /// <param name="oldXOverflow">
        /// the value of
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// before it's been changed
        /// during layouting of
        /// <see cref="LineRenderer"/>
        /// or null if
        /// <see cref="iText.Layout.Properties.Property.OVERFLOW_X"/>
        /// hasn't been changed
        /// </param>
        /// <param name="layoutContext">
        /// 
        /// <see cref="iText.Layout.Layout.LayoutContext"/>
        /// </param>
        /// <param name="layoutBox">current layoutBox</param>
        /// <param name="wasParentsHeightClipped">true if layoutBox's height has been clipped</param>
        /// <returns>true if the split word can fit the next line without splitting</returns>
        internal virtual bool IsForceOverflowForTextRendererPartialResult(IRenderer childRenderer, bool wasXOverflowChanged
            , OverflowPropertyValue? oldXOverflow, LayoutContext layoutContext, Rectangle layoutBox, bool wasParentsHeightClipped
            ) {
            if (wasXOverflowChanged) {
                SetProperty(Property.OVERFLOW_X, oldXOverflow);
            }
            LayoutResult newLayoutResult = childRenderer.Layout(new LayoutContext(new LayoutArea(layoutContext.GetArea
                ().GetPageNumber(), layoutBox), wasParentsHeightClipped));
            if (wasXOverflowChanged) {
                SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
            }
            return newLayoutResult is TextLayoutResult && !((TextLayoutResult)newLayoutResult).IsWordHasBeenSplit();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Extracts ascender and descender of an already layouted
        /// <see cref="IRenderer">childRenderer</see>.
        /// </summary>
        /// <param name="childRenderer">an already layouted child who's ascender and descender are to be extracted</param>
        /// <param name="childResult">
        /// 
        /// <see cref="iText.Layout.Layout.LayoutResult"/>
        /// of the childRenderer based on which ascender and descender are defined
        /// </param>
        /// <param name="childRenderingMode">
        /// 
        /// <see cref="iText.Layout.Properties.RenderingMode?">rendering mode</see>
        /// </param>
        /// <param name="isInlineBlockChild">
        /// true if childRenderer
        /// <see cref="IsInlineBlockChild(IRenderer)"/>
        /// </param>
        /// <returns>a two-element float array where first element is ascender value and second element is descender value
        ///     </returns>
        internal virtual float[] GetAscentDescentOfLayoutedChildRenderer(IRenderer childRenderer, LayoutResult childResult
            , RenderingMode? childRenderingMode, bool isInlineBlockChild) {
            float childAscent = 0;
            float childDescent = 0;
            if (childRenderer is ILeafElementRenderer && childResult.GetStatus() != LayoutResult.NOTHING) {
                if (RenderingMode.HTML_MODE == childRenderingMode && childRenderer is TextRenderer) {
                    return LineHeightHelper.GetActualAscenderDescender((TextRenderer)childRenderer);
                }
                else {
                    childAscent = ((ILeafElementRenderer)childRenderer).GetAscent();
                    childDescent = ((ILeafElementRenderer)childRenderer).GetDescent();
                }
            }
            else {
                if (isInlineBlockChild && childResult.GetStatus() != LayoutResult.NOTHING) {
                    if (childRenderer is AbstractRenderer) {
                        float? yLine = ((AbstractRenderer)childRenderer).GetLastYLineRecursively();
                        if (yLine == null) {
                            childAscent = childRenderer.GetOccupiedArea().GetBBox().GetHeight();
                        }
                        else {
                            childAscent = childRenderer.GetOccupiedArea().GetBBox().GetTop() - (float)yLine;
                            childDescent = -((float)yLine - childRenderer.GetOccupiedArea().GetBBox().GetBottom());
                        }
                    }
                    else {
                        childAscent = childRenderer.GetOccupiedArea().GetBBox().GetHeight();
                    }
                }
            }
            return new float[] { childAscent, childDescent };
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Updates
        /// <see cref="maxAscent"/>
        /// ,
        /// <see cref="maxDescent"/>
        /// ,
        /// <see cref="maxTextAscent"/>
        /// and
        /// <see cref="maxTextDescent"/>
        /// after a
        /// <see cref="TextRenderer"/>
        /// sequence has been fully processed.
        /// </summary>
        /// <param name="newChildPos">
        /// position of the last
        /// <see cref="TextRenderer"/>
        /// child of the
        /// sequence to remain on the line
        /// </param>
        /// <param name="lineAscentDescentStateBeforeTextRendererSequence">
        /// a
        /// <see cref="LineAscentDescentState"/>
        /// containing
        /// <see cref="LineRenderer"/>
        /// 's maxAscent, maxDescent,
        /// maxTextAscent, maxTextDescent before
        /// <see cref="TextRenderer"/>
        /// sequence start
        /// </param>
        /// <param name="textRendererSequenceAscentDescent">
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with
        /// <see cref="TextRenderer"/>
        /// children's
        /// positions as keys
        /// and float arrays consisting of maxAscent, maxDescent,
        /// maxTextAscent,
        /// maxTextDescent of the corresponding
        /// <see cref="TextRenderer"/>
        /// children.
        /// </param>
        /// <returns>
        /// a two-element float array where first element is a new
        /// <see cref="LineRenderer"/>
        /// 's ascender
        /// and second element is a new
        /// <see cref="LineRenderer"/>
        /// 's descender
        /// </returns>
        internal virtual float[] UpdateAscentDescentAfterTextRendererSequenceProcessing(int newChildPos, LineRenderer.LineAscentDescentState
             lineAscentDescentStateBeforeTextRendererSequence, IDictionary<int, float[]> textRendererSequenceAscentDescent
            ) {
            float maxAscentUpdated = lineAscentDescentStateBeforeTextRendererSequence.maxAscent;
            float maxDescentUpdated = lineAscentDescentStateBeforeTextRendererSequence.maxDescent;
            float maxTextAscentUpdated = lineAscentDescentStateBeforeTextRendererSequence.maxTextAscent;
            float maxTextDescentUpdated = lineAscentDescentStateBeforeTextRendererSequence.maxTextDescent;
            foreach (KeyValuePair<int, float[]> childAscentDescent in textRendererSequenceAscentDescent) {
                if (childAscentDescent.Key <= newChildPos) {
                    maxAscentUpdated = Math.Max(maxAscentUpdated, childAscentDescent.Value[0]);
                    maxDescentUpdated = Math.Min(maxDescentUpdated, childAscentDescent.Value[1]);
                    maxTextAscentUpdated = Math.Max(maxTextAscentUpdated, childAscentDescent.Value[0]);
                    maxTextDescentUpdated = Math.Min(maxTextDescentUpdated, childAscentDescent.Value[1]);
                }
            }
            this.maxAscent = maxAscentUpdated;
            this.maxDescent = maxDescentUpdated;
            this.maxTextAscent = maxTextAscentUpdated;
            this.maxTextDescent = maxTextDescentUpdated;
            return new float[] { this.maxAscent, this.maxDescent };
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Update
        /// <see cref="maxAscent"/>
        /// ,
        /// <see cref="maxDescent"/>
        /// ,
        /// <see cref="maxTextAscent"/>
        /// ,
        /// <see cref="maxTextDescent"/>
        /// ,
        /// <see cref="maxBlockAscent"/>
        /// and
        /// <see cref="maxBlockDescent"/>
        /// after child's layout.
        /// </summary>
        /// <param name="childAscentDescent">
        /// a two-element float array where first element is ascender of a layouted child
        /// and second element is descender of a layouted child
        /// </param>
        /// <param name="childRenderer">
        /// the layouted
        /// <see cref="IRenderer">childRenderer</see>
        /// of current
        /// <see cref="LineRenderer"/>
        /// </param>
        /// <param name="isChildFloating">
        /// true if
        /// <see cref="IsChildFloating(IRenderer)"/>
        /// </param>
        internal virtual void UpdateAscentDescentAfterChildLayout(float[] childAscentDescent, IRenderer childRenderer
            , bool isChildFloating) {
            float childAscent = childAscentDescent[0];
            float childDescent = childAscentDescent[1];
            this.maxAscent = Math.Max(this.maxAscent, childAscent);
            if (childRenderer is TextRenderer) {
                this.maxTextAscent = Math.Max(this.maxTextAscent, childAscent);
            }
            else {
                if (!isChildFloating) {
                    this.maxBlockAscent = Math.Max(this.maxBlockAscent, childAscent);
                }
            }
            this.maxDescent = Math.Min(this.maxDescent, childDescent);
            if (childRenderer is TextRenderer) {
                this.maxTextDescent = Math.Min(this.maxTextDescent, childDescent);
            }
            else {
                if (!isChildFloating) {
                    this.maxBlockDescent = Math.Min(this.maxBlockDescent, childDescent);
                }
            }
        }
//\endcond

        private void UpdateBidiLevels(int totalNumberOfTrimmedGlyphs, BaseDirection? baseDirection) {
            if (totalNumberOfTrimmedGlyphs != 0 && levels != null) {
                levels = JavaUtil.ArraysCopyOfRange(levels, totalNumberOfTrimmedGlyphs, levels.Length);
            }
            IList<int> unicodeIdsReorderingList = null;
            if (levels == null && baseDirection != null && baseDirection != BaseDirection.NO_BIDI) {
                unicodeIdsReorderingList = new List<int>();
                bool newLineFound = false;
                foreach (IRenderer child in GetChildRenderers()) {
                    if (newLineFound) {
                        break;
                    }
                    if (child is TextRenderer) {
                        GlyphLine text = ((TextRenderer)child).GetText();
                        for (int i = text.start; i < text.end; i++) {
                            Glyph glyph = text.Get(i);
                            if (iText.IO.Util.TextUtil.IsNewLine(glyph)) {
                                newLineFound = true;
                                break;
                            }
                            // we assume all the chars will have the same bidi group
                            // we also assume pairing symbols won't get merged with other ones
                            int unicode = glyph.HasValidUnicode() ? glyph.GetUnicode() : glyph.GetUnicodeChars()[0];
                            unicodeIdsReorderingList.Add(unicode);
                        }
                    }
                }
                if (unicodeIdsReorderingList.Count > 0) {
                    PdfDocument pdfDocument = GetPdfDocument();
                    SequenceId sequenceId = pdfDocument == null ? null : pdfDocument.GetDocumentIdWrapper();
                    MetaInfoContainer metaInfoContainer = this.GetProperty<MetaInfoContainer>(Property.META_INFO);
                    IMetaInfo metaInfo = metaInfoContainer == null ? null : metaInfoContainer.GetMetaInfo();
                    levels = TypographyUtils.GetBidiLevels(baseDirection, ArrayUtil.ToIntArray(unicodeIdsReorderingList), sequenceId
                        , metaInfo);
                }
                else {
                    levels = null;
                }
            }
        }

        /// <summary>While resolving TextRenderer may split into several ones with different fonts.</summary>
        private void ResolveChildrenFonts() {
            IList<IRenderer> newChildRenderers = new List<IRenderer>(GetChildRenderers().Count);
            bool updateChildRenderers = false;
            foreach (IRenderer child in GetChildRenderers()) {
                if (child is TextRenderer) {
                    if (((TextRenderer)child).ResolveFonts(newChildRenderers)) {
                        updateChildRenderers = true;
                    }
                }
                else {
                    newChildRenderers.Add(child);
                }
            }
            // this mean, that some TextRenderer has been replaced.
            if (updateChildRenderers) {
                SetChildRenderers(newChildRenderers);
            }
        }

        private float DecreaseRelativeWidthByChildAdditionalWidth(IRenderer childRenderer, float normalizedChildWidth
            ) {
            // Decrease the calculated width by margins, paddings and borders so that
            // even for 100% width the content definitely fits.
            if (childRenderer is AbstractRenderer) {
                Rectangle dummyRect = new Rectangle(normalizedChildWidth, 0);
                ((AbstractRenderer)childRenderer).ApplyMargins(dummyRect, false);
                if (!IsBorderBoxSizing(childRenderer)) {
                    ((AbstractRenderer)childRenderer).ApplyBorderBox(dummyRect, false);
                    ((AbstractRenderer)childRenderer).ApplyPaddings(dummyRect, false);
                }
                normalizedChildWidth = dummyRect.GetWidth();
            }
            return normalizedChildWidth;
        }

        private void AdjustChildrenYLineDefaultMode() {
            float actualYLine = occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - maxAscent;
            foreach (IRenderer renderer in GetChildRenderers()) {
                if (FloatingHelper.IsRendererFloating(renderer)) {
                    continue;
                }
                if (renderer is ILeafElementRenderer) {
                    float descent = ((ILeafElementRenderer)renderer).GetDescent();
                    renderer.Move(0, actualYLine - renderer.GetOccupiedArea().GetBBox().GetBottom() + descent);
                }
                else {
                    float? yLine = IsInlineBlockChild(renderer) && renderer is AbstractRenderer ? ((AbstractRenderer)renderer)
                        .GetLastYLineRecursively() : null;
                    renderer.Move(0, actualYLine - (yLine == null ? renderer.GetOccupiedArea().GetBBox().GetBottom() : (float)
                        yLine));
                }
            }
        }

        private bool HasInlineBlocksWithVerticalAlignment() {
            foreach (IRenderer child in GetChildRenderers()) {
                if (child.HasProperty(Property.INLINE_VERTICAL_ALIGNMENT) && InlineVerticalAlignmentType.BASELINE != ((InlineVerticalAlignment
                    )child.GetProperty<InlineVerticalAlignment>(Property.INLINE_VERTICAL_ALIGNMENT)).GetType()) {
                    return true;
                }
            }
            return false;
        }

        public class RendererGlyph {
            public Glyph glyph;

            public TextRenderer renderer;

            public RendererGlyph(Glyph glyph, TextRenderer textRenderer) {
                this.glyph = glyph;
                this.renderer = textRenderer;
            }
        }

//\cond DO_NOT_DOCUMENT
        internal class LineAscentDescentState {
//\cond DO_NOT_DOCUMENT
            internal float maxAscent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float maxDescent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float maxTextAscent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float maxTextDescent;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal LineAscentDescentState(float maxAscent, float maxDescent, float maxTextAscent, float maxTextDescent
                ) {
                this.maxAscent = maxAscent;
                this.maxDescent = maxDescent;
                this.maxTextAscent = maxTextAscent;
                this.maxTextDescent = maxTextDescent;
            }
//\endcond
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class LineSplitIntoGlyphsData {
            private readonly IList<LineRenderer.RendererGlyph> lineGlyphs;

            private readonly IDictionary<TextRenderer, IList<IRenderer>> insertAfter;

            private readonly IList<IRenderer> starterNonTextRenderers;

            public LineSplitIntoGlyphsData() {
                lineGlyphs = new List<LineRenderer.RendererGlyph>();
                insertAfter = new Dictionary<TextRenderer, IList<IRenderer>>();
                starterNonTextRenderers = new List<IRenderer>();
            }

            public virtual IList<LineRenderer.RendererGlyph> GetLineGlyphs() {
                return lineGlyphs;
            }

            public virtual IList<IRenderer> GetInsertAfterAndRemove(TextRenderer afterRenderer) {
                return insertAfter.JRemove(afterRenderer);
            }

            public virtual IList<IRenderer> GetStarterNonTextRenderers() {
                return starterNonTextRenderers;
            }

            public virtual void AddLineGlyph(LineRenderer.RendererGlyph glyph) {
                lineGlyphs.Add(glyph);
            }

            public virtual void AddInsertAfter(TextRenderer afterRenderer, IRenderer toInsert) {
                if (afterRenderer == null) {
                    // null indicates that there were no previous renderers
                    starterNonTextRenderers.Add(toInsert);
                }
                else {
                    if (!insertAfter.ContainsKey(afterRenderer)) {
                        insertAfter.Put(afterRenderer, new List<IRenderer>());
                    }
                    insertAfter.Get(afterRenderer).Add(toInsert);
                }
            }
        }
//\endcond
    }
}
