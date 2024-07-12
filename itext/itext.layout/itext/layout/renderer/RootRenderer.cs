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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Actions.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Margincollapse;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    public abstract class RootRenderer : AbstractRenderer {
        protected internal bool immediateFlush = true;

        protected internal RootLayoutArea currentArea;

        protected internal IList<IRenderer> waitingDrawingElements = new List<IRenderer>();

//\cond DO_NOT_DOCUMENT
        internal IList<Rectangle> floatRendererAreas;
//\endcond

        private IRenderer keepWithNextHangingRenderer;

        private LayoutResult keepWithNextHangingRendererLayoutResult;

        private MarginsCollapseHandler marginsCollapseHandler;

        private LayoutArea initialCurrentArea;

        private IList<IRenderer> waitingNextPageRenderers = new List<IRenderer>();

        private bool floatOverflowedCompletely = false;

        public override void AddChild(IRenderer renderer) {
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                LayoutTaggingHelper.AddTreeHints(taggingHelper, renderer);
            }
            // Some positioned renderers might have been fetched from non-positioned child and added to this renderer,
            // so we use this generic mechanism of determining which renderers have been just added.
            int numberOfChildRenderers = childRenderers.Count;
            int numberOfPositionedChildRenderers = positionedRenderers.Count;
            base.AddChild(renderer);
            IList<IRenderer> addedRenderers = new List<IRenderer>(1);
            IList<IRenderer> addedPositionedRenderers = new List<IRenderer>(1);
            while (childRenderers.Count > numberOfChildRenderers) {
                addedRenderers.Add(childRenderers[numberOfChildRenderers]);
                childRenderers.JRemoveAt(numberOfChildRenderers);
            }
            while (positionedRenderers.Count > numberOfPositionedChildRenderers) {
                addedPositionedRenderers.Add(positionedRenderers[numberOfPositionedChildRenderers]);
                positionedRenderers.JRemoveAt(numberOfPositionedChildRenderers);
            }
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (currentArea == null) {
                UpdateCurrentAndInitialArea(null);
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler = new MarginsCollapseHandler(this, null);
                }
            }
            // Static layout
            for (int i = 0; currentArea != null && i < addedRenderers.Count; i++) {
                RootRendererAreaStateHandler rootRendererStateHandler = new RootRendererAreaStateHandler();
                renderer = addedRenderers[i];
                bool rendererIsFloat = FloatingHelper.IsRendererFloating(renderer);
                bool clearanceOverflowsToNextPage = FloatingHelper.IsClearanceApplied(waitingNextPageRenderers, renderer.GetProperty
                    <ClearPropertyValue?>(Property.CLEAR));
                if (rendererIsFloat && (floatOverflowedCompletely || clearanceOverflowsToNextPage)) {
                    waitingNextPageRenderers.Add(renderer);
                    floatOverflowedCompletely = true;
                    continue;
                }
                ProcessWaitingKeepWithNextElement(renderer);
                IList<IRenderer> resultRenderers = new List<IRenderer>();
                LayoutResult result = null;
                MarginsCollapseInfo childMarginsInfo = null;
                if (marginsCollapsingEnabled && currentArea != null && renderer != null) {
                    childMarginsInfo = marginsCollapseHandler.StartChildMarginsHandling(renderer, currentArea.GetBBox());
                }
                while (clearanceOverflowsToNextPage || currentArea != null && renderer != null && (result = renderer.SetParent
                    (this).Layout(new LayoutContext(currentArea.Clone(), childMarginsInfo, floatRendererAreas))).GetStatus
                    () != LayoutResult.FULL) {
                    bool currentAreaNeedsToBeUpdated = false;
                    if (clearanceOverflowsToNextPage) {
                        result = new LayoutResult(LayoutResult.NOTHING, null, null, renderer);
                        currentAreaNeedsToBeUpdated = true;
                    }
                    if (result.GetStatus() == LayoutResult.PARTIAL) {
                        if (rendererIsFloat) {
                            waitingNextPageRenderers.Add(result.GetOverflowRenderer());
                            break;
                        }
                        else {
                            ProcessRenderer(result.GetSplitRenderer(), resultRenderers);
                            if (!rootRendererStateHandler.AttemptGoForwardToStoredNextState(this)) {
                                currentAreaNeedsToBeUpdated = true;
                            }
                        }
                    }
                    else {
                        if (result.GetStatus() == LayoutResult.NOTHING && !clearanceOverflowsToNextPage) {
                            if (result.GetOverflowRenderer() is ImageRenderer) {
                                float imgHeight = ((ImageRenderer)result.GetOverflowRenderer()).GetOccupiedArea().GetBBox().GetHeight();
                                if (!floatRendererAreas.IsEmpty() || currentArea.GetBBox().GetHeight() < imgHeight && !currentArea.IsEmptyArea
                                    ()) {
                                    if (rendererIsFloat) {
                                        waitingNextPageRenderers.Add(result.GetOverflowRenderer());
                                        floatOverflowedCompletely = true;
                                        break;
                                    }
                                    currentAreaNeedsToBeUpdated = true;
                                }
                                else {
                                    ((ImageRenderer)result.GetOverflowRenderer()).AutoScale(currentArea);
                                    result.GetOverflowRenderer().SetProperty(Property.FORCED_PLACEMENT, true);
                                    ILogger logger = ITextLogManager.GetLogger(typeof(RootRenderer));
                                    logger.LogWarning(MessageFormatUtil.Format(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, ""));
                                }
                            }
                            else {
                                if (currentArea.IsEmptyArea() && result.GetAreaBreak() == null) {
                                    bool keepTogetherChanged = TryDisableKeepTogether(result, rendererIsFloat, rootRendererStateHandler);
                                    bool areKeepTogetherAndForcedPlacementBothNotChanged = !keepTogetherChanged;
                                    if (areKeepTogetherAndForcedPlacementBothNotChanged) {
                                        areKeepTogetherAndForcedPlacementBothNotChanged = !UpdateForcedPlacement(renderer, result.GetOverflowRenderer
                                            ());
                                    }
                                    if (areKeepTogetherAndForcedPlacementBothNotChanged) {
                                        // FORCED_PLACEMENT was already set to the renderer and
                                        // LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA message was logged.
                                        // This else-clause should never be hit, otherwise there is a bug in FORCED_PLACEMENT implementation.
                                        System.Diagnostics.Debug.Assert(false);
                                        // Still handling this case in order to avoid nasty infinite loops.
                                        break;
                                    }
                                }
                                else {
                                    rootRendererStateHandler.StorePreviousState(this);
                                    if (!rootRendererStateHandler.AttemptGoForwardToStoredNextState(this)) {
                                        if (rendererIsFloat) {
                                            waitingNextPageRenderers.Add(result.GetOverflowRenderer());
                                            floatOverflowedCompletely = true;
                                            break;
                                        }
                                        currentAreaNeedsToBeUpdated = true;
                                    }
                                }
                            }
                        }
                    }
                    renderer = result.GetOverflowRenderer();
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler.EndChildMarginsHandling(currentArea.GetBBox());
                    }
                    if (currentAreaNeedsToBeUpdated) {
                        UpdateCurrentAndInitialArea(result);
                    }
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler = new MarginsCollapseHandler(this, null);
                        childMarginsInfo = marginsCollapseHandler.StartChildMarginsHandling(renderer, currentArea.GetBBox());
                    }
                    clearanceOverflowsToNextPage = clearanceOverflowsToNextPage && FloatingHelper.IsClearanceApplied(waitingNextPageRenderers
                        , renderer.GetProperty<ClearPropertyValue?>(Property.CLEAR));
                }
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler.EndChildMarginsHandling(currentArea.GetBBox());
                }
                if (null != result && null != result.GetSplitRenderer()) {
                    renderer = result.GetSplitRenderer();
                }
                // Keep renderer until next element is added for future keep with next adjustments
                if (renderer != null && result != null) {
                    if (true.Equals(renderer.GetProperty<bool?>(Property.KEEP_WITH_NEXT))) {
                        if (true.Equals(renderer.GetProperty<bool?>(Property.FORCED_PLACEMENT))) {
                            ILogger logger = ITextLogManager.GetLogger(typeof(RootRenderer));
                            logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.ELEMENT_WAS_FORCE_PLACED_KEEP_WITH_NEXT_WILL_BE_IGNORED
                                );
                            ShrinkCurrentAreaAndProcessRenderer(renderer, resultRenderers, result);
                        }
                        else {
                            keepWithNextHangingRenderer = renderer;
                            keepWithNextHangingRendererLayoutResult = result;
                        }
                    }
                    else {
                        if (result.GetStatus() != LayoutResult.NOTHING) {
                            ShrinkCurrentAreaAndProcessRenderer(renderer, resultRenderers, result);
                        }
                    }
                }
            }
            for (int i = 0; i < addedPositionedRenderers.Count; i++) {
                positionedRenderers.Add(addedPositionedRenderers[i]);
                renderer = positionedRenderers[positionedRenderers.Count - 1];
                int? positionedPageNumber = renderer.GetProperty<int?>(Property.PAGE_NUMBER);
                if (positionedPageNumber == null) {
                    positionedPageNumber = currentArea.GetPageNumber();
                }
                LayoutArea layoutArea;
                // For position=absolute, if none of the top, bottom, left, right properties are provided,
                // the content should be displayed in the flow of the current content, not overlapping it.
                // The behavior is just if it would be statically positioned except it does not affect other elements
                if (Convert.ToInt32(LayoutPosition.ABSOLUTE).Equals(renderer.GetProperty<int?>(Property.POSITION)) && AbstractRenderer
                    .NoAbsolutePositionInfo(renderer)) {
                    layoutArea = new LayoutArea((int)positionedPageNumber, currentArea.GetBBox().Clone());
                }
                else {
                    layoutArea = new LayoutArea((int)positionedPageNumber, initialCurrentArea.GetBBox().Clone());
                }
                Rectangle fullBbox = layoutArea.GetBBox().Clone();
                PreparePositionedRendererAndAreaForLayout(renderer, fullBbox, layoutArea.GetBBox());
                renderer.Layout(new PositionedLayoutContext(new LayoutArea(layoutArea.GetPageNumber(), fullBbox), layoutArea
                    ));
                if (immediateFlush) {
                    FlushSingleRenderer(renderer);
                    positionedRenderers.JRemoveAt(positionedRenderers.Count - 1);
                }
            }
        }

        /// <summary>Draws (flushes) the content.</summary>
        /// <seealso cref="AbstractRenderer.Draw(DrawContext)"/>
        public virtual void Flush() {
            foreach (IRenderer resultRenderer in childRenderers) {
                FlushSingleRenderer(resultRenderer);
            }
            foreach (IRenderer resultRenderer in positionedRenderers) {
                FlushSingleRenderer(resultRenderer);
            }
            childRenderers.Clear();
            positionedRenderers.Clear();
        }

        /// <summary>
        /// This method correctly closes the
        /// <see cref="RootRenderer"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method correctly closes the
        /// <see cref="RootRenderer"/>
        /// instance.
        /// There might be hanging elements, like in case of
        /// <see cref="iText.Layout.Properties.Property.KEEP_WITH_NEXT"/>
        /// is set to true
        /// and when no consequent element has been added. This method addresses such situations.
        /// </remarks>
        public virtual void Close() {
            AddAllWaitingNextPageRenderers();
            if (keepWithNextHangingRenderer != null) {
                keepWithNextHangingRenderer.SetProperty(Property.KEEP_WITH_NEXT, false);
                IRenderer rendererToBeAdded = keepWithNextHangingRenderer;
                keepWithNextHangingRenderer = null;
                AddChild(rendererToBeAdded);
            }
            if (!immediateFlush) {
                Flush();
            }
            FlushWaitingDrawingElements(true);
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                taggingHelper.ReleaseAllHints();
            }
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            throw new InvalidOperationException("Layout is not supported for root renderers.");
        }

        public virtual LayoutArea GetCurrentArea() {
            if (currentArea == null) {
                UpdateCurrentAndInitialArea(null);
            }
            return currentArea;
        }

        protected internal abstract void FlushSingleRenderer(IRenderer resultRenderer);

        protected internal abstract LayoutArea UpdateCurrentArea(LayoutResult overflowResult);

        protected internal virtual void ShrinkCurrentAreaAndProcessRenderer(IRenderer renderer, IList<IRenderer> resultRenderers
            , LayoutResult result) {
            if (currentArea != null) {
                float resultRendererHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                currentArea.GetBBox().SetHeight(currentArea.GetBBox().GetHeight() - resultRendererHeight);
                if (currentArea.IsEmptyArea() && (resultRendererHeight > 0 || FloatingHelper.IsRendererFloating(renderer))
                    ) {
                    currentArea.SetEmptyArea(false);
                }
                ProcessRenderer(renderer, resultRenderers);
            }
            if (!immediateFlush) {
                childRenderers.AddAll(resultRenderers);
            }
        }

        protected internal virtual void FlushWaitingDrawingElements() {
            FlushWaitingDrawingElements(true);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void FlushWaitingDrawingElements(bool force) {
            ICollection<IRenderer> flushedElements = new HashSet<IRenderer>();
            for (int i = 0; i < waitingDrawingElements.Count; ++i) {
                IRenderer waitingDrawingElement = waitingDrawingElements[i];
                // TODO Remove checking occupied area to be not null when DEVSIX-1655 is resolved.
                if (force || (null != waitingDrawingElement.GetOccupiedArea() && waitingDrawingElement.GetOccupiedArea().GetPageNumber
                    () < currentArea.GetPageNumber())) {
                    FlushSingleRenderer(waitingDrawingElement);
                    flushedElements.Add(waitingDrawingElement);
                }
                else {
                    if (null == waitingDrawingElement.GetOccupiedArea()) {
                        flushedElements.Add(waitingDrawingElement);
                    }
                }
            }
            waitingDrawingElements.RemoveAll(flushedElements);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal void LinkRenderToDocument(IRenderer renderer, PdfDocument pdfDocument) {
            if (renderer == null) {
                return;
            }
            IPropertyContainer container = renderer.GetModelElement();
            if (container is AbstractIdentifiableElement) {
                EventManager.GetInstance().OnEvent(new LinkDocumentIdEvent(pdfDocument, (AbstractIdentifiableElement)container
                    ));
            }
            IList<IRenderer> children = renderer.GetChildRenderers();
            if (children != null) {
                foreach (IRenderer child in children) {
                    LinkRenderToDocument(child, pdfDocument);
                }
            }
        }
//\endcond

        private void ProcessRenderer(IRenderer renderer, IList<IRenderer> resultRenderers) {
            AlignChildHorizontally(renderer, currentArea.GetBBox());
            if (immediateFlush) {
                FlushSingleRenderer(renderer);
            }
            else {
                resultRenderers.Add(renderer);
            }
        }

        private void ProcessWaitingKeepWithNextElement(IRenderer renderer) {
            if (keepWithNextHangingRenderer != null) {
                LayoutArea rest = currentArea.Clone();
                rest.GetBBox().SetHeight(rest.GetBBox().GetHeight() - keepWithNextHangingRendererLayoutResult.GetOccupiedArea
                    ().GetBBox().GetHeight());
                bool ableToProcessKeepWithNext = false;
                if (renderer.SetParent(this).Layout(new LayoutContext(rest)).GetStatus() != LayoutResult.NOTHING) {
                    // The area break will not be introduced and we are safe to place everything as is
                    ShrinkCurrentAreaAndProcessRenderer(keepWithNextHangingRenderer, new List<IRenderer>(), keepWithNextHangingRendererLayoutResult
                        );
                    ableToProcessKeepWithNext = true;
                }
                else {
                    float originalElementHeight = keepWithNextHangingRendererLayoutResult.GetOccupiedArea().GetBBox().GetHeight
                        ();
                    IList<float> trySplitHeightPoints = new List<float>();
                    float delta = 35;
                    for (int i = 1; i <= 5 && originalElementHeight - delta * i > originalElementHeight / 2; i++) {
                        trySplitHeightPoints.Add(originalElementHeight - delta * i);
                    }
                    for (int i = 0; i < trySplitHeightPoints.Count && !ableToProcessKeepWithNext; i++) {
                        float curElementSplitHeight = trySplitHeightPoints[i];
                        RootLayoutArea firstElementSplitLayoutArea = (RootLayoutArea)currentArea.Clone();
                        firstElementSplitLayoutArea.GetBBox().SetHeight(curElementSplitHeight).MoveUp(currentArea.GetBBox().GetHeight
                            () - curElementSplitHeight);
                        LayoutResult firstElementSplitLayoutResult = keepWithNextHangingRenderer.SetParent(this).Layout(new LayoutContext
                            (firstElementSplitLayoutArea.Clone()));
                        if (firstElementSplitLayoutResult.GetStatus() == LayoutResult.PARTIAL) {
                            RootLayoutArea storedArea = currentArea;
                            UpdateCurrentAndInitialArea(firstElementSplitLayoutResult);
                            LayoutResult firstElementOverflowLayoutResult = firstElementSplitLayoutResult.GetOverflowRenderer().Layout
                                (new LayoutContext(currentArea.Clone()));
                            if (firstElementOverflowLayoutResult.GetStatus() == LayoutResult.FULL) {
                                LayoutArea secondElementLayoutArea = currentArea.Clone();
                                secondElementLayoutArea.GetBBox().SetHeight(secondElementLayoutArea.GetBBox().GetHeight() - firstElementOverflowLayoutResult
                                    .GetOccupiedArea().GetBBox().GetHeight());
                                LayoutResult secondElementLayoutResult = renderer.SetParent(this).Layout(new LayoutContext(secondElementLayoutArea
                                    ));
                                if (secondElementLayoutResult.GetStatus() != LayoutResult.NOTHING) {
                                    ableToProcessKeepWithNext = true;
                                    currentArea = firstElementSplitLayoutArea;
                                    ShrinkCurrentAreaAndProcessRenderer(firstElementSplitLayoutResult.GetSplitRenderer(), new List<IRenderer>(
                                        ), firstElementSplitLayoutResult);
                                    UpdateCurrentAndInitialArea(firstElementSplitLayoutResult);
                                    ShrinkCurrentAreaAndProcessRenderer(firstElementSplitLayoutResult.GetOverflowRenderer(), new List<IRenderer
                                        >(), firstElementOverflowLayoutResult);
                                }
                            }
                            if (!ableToProcessKeepWithNext) {
                                currentArea = storedArea;
                            }
                        }
                    }
                }
                if (!ableToProcessKeepWithNext && !currentArea.IsEmptyArea()) {
                    RootLayoutArea storedArea = currentArea;
                    UpdateCurrentAndInitialArea(null);
                    LayoutResult firstElementLayoutResult = keepWithNextHangingRenderer.SetParent(this).Layout(new LayoutContext
                        (currentArea.Clone()));
                    if (firstElementLayoutResult.GetStatus() == LayoutResult.FULL) {
                        LayoutArea secondElementLayoutArea = currentArea.Clone();
                        secondElementLayoutArea.GetBBox().SetHeight(secondElementLayoutArea.GetBBox().GetHeight() - firstElementLayoutResult
                            .GetOccupiedArea().GetBBox().GetHeight());
                        LayoutResult secondElementLayoutResult = renderer.SetParent(this).Layout(new LayoutContext(secondElementLayoutArea
                            ));
                        if (secondElementLayoutResult.GetStatus() != LayoutResult.NOTHING) {
                            ableToProcessKeepWithNext = true;
                            ShrinkCurrentAreaAndProcessRenderer(keepWithNextHangingRenderer, new List<IRenderer>(), keepWithNextHangingRendererLayoutResult
                                );
                        }
                    }
                    if (!ableToProcessKeepWithNext) {
                        currentArea = storedArea;
                    }
                }
                if (!ableToProcessKeepWithNext) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(RootRenderer));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT);
                    ShrinkCurrentAreaAndProcessRenderer(keepWithNextHangingRenderer, new List<IRenderer>(), keepWithNextHangingRendererLayoutResult
                        );
                }
                keepWithNextHangingRenderer = null;
                keepWithNextHangingRendererLayoutResult = null;
            }
        }

        private void UpdateCurrentAndInitialArea(LayoutResult overflowResult) {
            floatRendererAreas = new List<Rectangle>();
            UpdateCurrentArea(overflowResult);
            initialCurrentArea = currentArea == null ? null : currentArea.Clone();
            AddWaitingNextPageRenderers();
        }

        private void AddAllWaitingNextPageRenderers() {
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            while (!waitingNextPageRenderers.IsEmpty()) {
                if (marginsCollapsingEnabled) {
                    marginsCollapseHandler = new MarginsCollapseHandler(this, null);
                }
                UpdateCurrentAndInitialArea(null);
            }
        }

        private void AddWaitingNextPageRenderers() {
            floatOverflowedCompletely = false;
            IList<IRenderer> waitingFloatRenderers = new List<IRenderer>(waitingNextPageRenderers);
            waitingNextPageRenderers.Clear();
            foreach (IRenderer renderer in waitingFloatRenderers) {
                AddChild(renderer);
            }
        }

        private bool UpdateForcedPlacement(IRenderer currentRenderer, IRenderer overflowRenderer) {
            if (true.Equals(currentRenderer.GetProperty<bool?>(Property.FORCED_PLACEMENT))) {
                return false;
            }
            else {
                overflowRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                ILogger logger = ITextLogManager.GetLogger(typeof(RootRenderer));
                if (logger.IsEnabled(LogLevel.Warning)) {
                    logger.LogWarning(MessageFormatUtil.Format(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, ""));
                }
                return true;
            }
        }

        private bool TryDisableKeepTogether(LayoutResult result, bool rendererIsFloat, RootRendererAreaStateHandler
             rootRendererStateHandler) {
            IRenderer toDisableKeepTogether = null;
            // looking for the most outer keep together element
            IRenderer current = result.GetCauseOfNothing();
            while (current != null) {
                if (true.Equals(current.GetProperty<bool?>(Property.KEEP_TOGETHER))) {
                    toDisableKeepTogether = current;
                }
                current = current.GetParent();
            }
            if (toDisableKeepTogether == null) {
                return false;
            }
            toDisableKeepTogether.SetProperty(Property.KEEP_TOGETHER, false);
            ILogger logger = ITextLogManager.GetLogger(typeof(RootRenderer));
            if (logger.IsEnabled(LogLevel.Warning)) {
                logger.LogWarning(MessageFormatUtil.Format(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, "KeepTogether property will be ignored."
                    ));
            }
            if (!rendererIsFloat) {
                rootRendererStateHandler.AttemptGoBackToStoredPreviousStateAndStoreNextState(this);
            }
            return true;
        }
    }
}
