/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils.Collections;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    public class DocumentRenderer : RootRenderer {
//\cond DO_NOT_DOCUMENT
        internal readonly FootnotesCounterHandler footnotesCounterHandler = new FootnotesCounterHandler();
//\endcond

        protected internal Document document;

        protected internal IList<int> wrappedContentPage = new List<int>();

        protected internal TargetCounterHandler targetCounterHandler = new TargetCounterHandler();

        private DocumentRenderer.PageMarginBoxesDrawingHandler marginBoxesHandler;

        private bool dynamicPageMarginsUsed = false;

        private PageSize currentPageSize = null;

        private SectionBreak prevSectionBreak = null;

        public DocumentRenderer(Document document)
            : this(document, true) {
        }

        public DocumentRenderer(Document document, bool immediateFlush) {
            this.document = document;
            this.immediateFlush = immediateFlush;
            this.modelElement = document;
            this.marginBoxesHandler = new DocumentRenderer.PageMarginBoxesDrawingHandler().SetDocumentRenderer(this);
            if (this.document != null) {
                this.document.GetPdfDocument().AddEventHandler(PdfDocumentEvent.END_PAGE, marginBoxesHandler);
            }
        }

        /// <summary>Get handler for target-counters.</summary>
        /// <returns>
        /// the
        /// <see cref="TargetCounterHandler"/>
        /// instance
        /// </returns>
        public virtual TargetCounterHandler GetTargetCounterHandler() {
            return targetCounterHandler;
        }

        /// <summary>Indicates if relayout is required for targetCounterHandler.</summary>
        /// <returns>true if relayout is required, false otherwise</returns>
        public virtual bool IsRelayoutRequired() {
            return targetCounterHandler.IsRelayoutRequired();
        }

        public override void AddChild(IRenderer renderer) {
            if (renderer is SectionBreakRenderer) {
                SectionBreak sectionBreak = (SectionBreak)renderer.GetModelElement();
                if (sectionBreak != null) {
                    this.currentPageSize = sectionBreak.GetPageSize();
                }
            }
            base.AddChild(renderer);
        }

        public override LayoutArea GetOccupiedArea() {
            throw new InvalidOperationException("Not applicable for DocumentRenderer");
        }

        /// <summary>
        /// For
        /// <see cref="DocumentRenderer"/>
        /// , this has a meaning of the renderer that will be used for relayout.
        /// </summary>
        /// <returns>relayout renderer.</returns>
        public override IRenderer GetNextRenderer() {
            iText.Layout.Renderer.DocumentRenderer renderer = new iText.Layout.Renderer.DocumentRenderer(document, immediateFlush
                );
            renderer.targetCounterHandler = new TargetCounterHandler(targetCounterHandler);
            renderer.marginBoxesHandler = marginBoxesHandler.SetDocumentRenderer(renderer);
            return renderer;
        }

        public override void Close() {
            base.Close();
            document.GetPdfDocument().RemoveEventHandler(marginBoxesHandler);
            if (!document.GetPdfDocument().IsClosed()) {
                for (int i = 1; i <= document.GetPdfDocument().GetNumberOfPages(); ++i) {
                    PdfPage page = document.GetPdfDocument().GetPage(i);
                    if (!page.IsFlushed()) {
                        marginBoxesHandler.ProcessPage(document.GetPdfDocument(), i);
                    }
                }
            }
        }

        protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
            FlushWaitingDrawingElements(false);
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                taggingHelper.ReleaseFinishedHints();
            }
            AreaBreak areaBreak = overflowResult != null && overflowResult.GetAreaBreak() != null ? overflowResult.GetAreaBreak
                () : null;
            SectionBreak sectionBreak = overflowResult != null && overflowResult.GetSectionBreak() != null ? overflowResult
                .GetSectionBreak() : null;
            int currentPageNumber = currentArea == null ? 0 : currentArea.GetPageNumber();
            if (areaBreak != null && areaBreak.GetAreaType() == AreaBreakType.LAST_PAGE) {
                while (currentPageNumber < document.GetPdfDocument().GetNumberOfPages()) {
                    PossiblyFlushPreviousPage(currentPageNumber);
                    currentPageNumber++;
                }
            }
            else {
                PossiblyFlushPreviousPage(currentPageNumber);
                // Don't bump page number in case SectionBreak is added to the empty page which is not the 1st.
                // Or if page margins weren't changed.
                if (sectionBreak == null || SectionBreakUtil.BreakPage(sectionBreak)) {
                    currentPageNumber++;
                }
            }
            PageSize customPageSize = currentPageSize;
            if (areaBreak != null) {
                customPageSize = areaBreak.GetPageSize();
            }
            else {
                if (sectionBreak != null) {
                    customPageSize = sectionBreak.GetPageSize();
                    currentPageSize = customPageSize;
                }
            }
            while (document.GetPdfDocument().GetNumberOfPages() >= currentPageNumber && document.GetPdfDocument().GetPage
                (currentPageNumber).IsFlushed()) {
                currentPageNumber++;
            }
            PageSize lastPageSize = EnsureDocumentHasNPages(currentPageNumber, customPageSize);
            if (lastPageSize == null) {
                lastPageSize = new PageSize(document.GetPdfDocument().GetPage(currentPageNumber).GetTrimBox());
            }
            if (sectionBreak != null) {
                this.document.SetPageMargins(currentPageNumber, sectionBreak.GetPageMargins());
                FootnotesProperties sectionBreakFootnotesProperties = sectionBreak.GetFootnotesProperties();
                if (sectionBreakFootnotesProperties != null) {
                    document.SetFootnotesProperties(sectionBreakFootnotesProperties);
                }
            }
            FootnotesProperties footnotesProperties = document.GetFootnotesProperties();
            FootnoteNumberingConfig footnoteNumberingConfig = footnotesProperties.GetFootnoteNumberingConfig();
            if (sectionBreak != null && FootnoteNumberingConfig.PER_SECTION == footnoteNumberingConfig) {
                this.latestFootnoteNumber.Put(currentPageNumber, 0);
            }
            else {
                if (FootnoteNumberingConfig.PER_PAGE != footnoteNumberingConfig) {
                    this.latestFootnoteNumber.Put(currentPageNumber, this.latestFootnoteNumber.GetOrDefault(currentPageNumber 
                        - 1, 0));
                }
            }
            ComputeLayoutMargins(currentPageNumber);
            if (sectionBreak != null) {
                // Save section break to apply same page margins for all the following pages
                // in case their margins were not set via Document by page number, rule or function.
                prevSectionBreak = sectionBreak;
            }
            Rectangle updatedAreaRect = GetCurrentPageEffectiveArea(lastPageSize);
            if (sectionBreak != null && currentArea.GetPageNumber() == currentPageNumber && overflowResult.GetOccupiedArea
                () != null) {
                updatedAreaRect.SetHeight(overflowResult.GetOccupiedArea().GetBBox().GetY() - updatedAreaRect.GetY());
            }
            return (currentArea = new RootLayoutArea(currentPageNumber, updatedAreaRect));
        }

        protected internal override void FlushSingleRenderer(IRenderer resultRenderer) {
            LinkRenderToDocument(resultRenderer, document.GetPdfDocument());
            Transform transformProp = resultRenderer.GetProperty<Transform>(Property.TRANSFORM);
            if (!waitingDrawingElements.Contains(resultRenderer)) {
                ProcessWaitingDrawing(resultRenderer, transformProp, waitingDrawingElements);
                if (FloatingHelper.IsRendererFloating(resultRenderer) || transformProp != null) {
                    return;
                }
            }
            // TODO Remove checking occupied area to be not null when DEVSIX-1655 is resolved.
            if (!resultRenderer.IsFlushed() && null != resultRenderer.GetOccupiedArea()) {
                int pageNum = resultRenderer.GetOccupiedArea().GetPageNumber();
                PdfDocument pdfDocument = document.GetPdfDocument();
                EnsureDocumentHasNPages(pageNum, null);
                PdfPage correspondingPage = pdfDocument.GetPage(pageNum);
                if (correspondingPage.IsFlushed()) {
                    throw new PdfException(LayoutExceptionMessageConstant.CANNOT_DRAW_ELEMENTS_ON_ALREADY_FLUSHED_PAGES);
                }
                bool wrapOldContent = pdfDocument.GetReader() != null && pdfDocument.GetWriter() != null && correspondingPage
                    .GetContentStreamCount() > 0 && correspondingPage.GetLastContentStream().GetLength() > 0 && !wrappedContentPage
                    .Contains(pageNum) && pdfDocument.GetNumberOfPages() >= pageNum;
                wrappedContentPage.Add(pageNum);
                if (pdfDocument.IsTagged()) {
                    pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetPageForTagging(correspondingPage);
                }
                resultRenderer.Draw(new DrawContext(pdfDocument, new PdfCanvas(correspondingPage, wrapOldContent), pdfDocument
                    .IsTagged()));
            }
        }

        /// <summary>Adds new page with defined page size to PDF document.</summary>
        /// <param name="customPageSize">the size of new page, can be null</param>
        /// <returns>the page size of created page</returns>
        protected internal virtual PageSize AddNewPage(PageSize customPageSize) {
            if (customPageSize != null) {
                document.GetPdfDocument().AddNewPage(customPageSize);
            }
            else {
                document.GetPdfDocument().AddNewPage();
            }
            return customPageSize != null ? customPageSize : document.GetPdfDocument().GetDefaultPageSize();
        }

        /// <summary>Ensures that PDF document has n pages.</summary>
        /// <remarks>
        /// Ensures that PDF document has n pages. If document has fewer pages,
        /// adds new pages by calling
        /// <see cref="AddNewPage(iText.Kernel.Geom.PageSize)"/>
        /// method.
        /// </remarks>
        /// <param name="n">the expected number of pages if document</param>
        /// <param name="customPageSize">the size of created pages, can be null</param>
        /// <returns>the page size of the last created page, or null if no page was created</returns>
        protected internal virtual PageSize EnsureDocumentHasNPages(int n, PageSize customPageSize) {
            PageSize lastPageSize = null;
            while (document.GetPdfDocument().GetNumberOfPages() < n) {
                lastPageSize = AddNewPage(customPageSize);
            }
            return lastPageSize;
        }

        private Rectangle GetCurrentPageEffectiveArea(PageSize pageSize) {
            float leftMargin = (float)GetPropertyAsFloat(Property.MARGIN_LEFT);
            float bottomMargin = (float)GetPropertyAsFloat(Property.MARGIN_BOTTOM);
            float topMargin = (float)GetPropertyAsFloat(Property.MARGIN_TOP);
            float rightMargin = (float)GetPropertyAsFloat(Property.MARGIN_RIGHT);
            return new Rectangle(pageSize.GetLeft() + leftMargin, pageSize.GetBottom() + bottomMargin, pageSize.GetWidth
                () - leftMargin - rightMargin, pageSize.GetHeight() - bottomMargin - topMargin);
        }

        private void PossiblyFlushPreviousPage(int currentPageNumber) {
            if (immediateFlush && currentPageNumber > 1) {
                // We don't flush current page immediately, but only flush previous one
                // because of manipulations with areas in case of keepTogether property
                document.GetPdfDocument().GetPage(currentPageNumber - 1).Flush();
            }
        }

        private void ComputeLayoutMargins(int pageNumber) {
            PageMarginBoxes pageMarginBoxes = this.document.GetPageMargins(pageNumber);
            PdfPage page = document.GetPdfDocument().GetPage(pageNumber);
            if (pageMarginBoxes == null) {
                PageMarginBoxes prevPageMarginBoxes = prevSectionBreak == null ? null : prevSectionBreak.GetPageMargins();
                if (this.document.IsPageMarginsSpecified(pageNumber) || prevPageMarginBoxes == null) {
                    this.ResetDynamicPageMargins();
                    return;
                }
                pageMarginBoxes = new PageMarginBoxes(prevPageMarginBoxes);
                PageSize prevPageSize = prevSectionBreak.GetPageSize();
                if (prevPageSize == null) {
                    prevPageSize = document.GetPdfDocument().GetDefaultPageSize();
                }
                if (prevPageSize.EqualsWithEpsilon(page.GetPageSize())) {
                    this.document.SetPageMargins(pageNumber, pageMarginBoxes);
                    this.SetDynamicPageMargins(pageMarginBoxes.GetMarginSizes());
                    return;
                }
            }
            float[] margins = pageMarginBoxes.Layout(this, pageNumber, page.GetPageSize());
            pageMarginBoxes.SetMarginSizes(margins);
            // Set page margins for page number to prioritize them and save the layout result.
            this.document.SetPageMargins(pageNumber, pageMarginBoxes);
            this.SetDynamicPageMargins(margins);
        }

        private void SetDynamicPageMargins(float[] margins) {
            dynamicPageMarginsUsed = true;
            SetProperty(Property.MARGIN_TOP, margins[0]);
            SetProperty(Property.MARGIN_RIGHT, margins[1]);
            SetProperty(Property.MARGIN_BOTTOM, margins[2]);
            SetProperty(Property.MARGIN_LEFT, margins[3]);
        }

        private void ResetDynamicPageMargins() {
            if (dynamicPageMarginsUsed) {
                DeleteOwnProperty(Property.MARGIN_TOP);
                DeleteOwnProperty(Property.MARGIN_RIGHT);
                DeleteOwnProperty(Property.MARGIN_BOTTOM);
                DeleteOwnProperty(Property.MARGIN_LEFT);
                dynamicPageMarginsUsed = false;
            }
        }

        /// <summary>
        /// Handler for drawing page margins on
        /// <c>END_PAGE</c>
        /// event.
        /// </summary>
        private sealed class PageMarginBoxesDrawingHandler : AbstractPdfDocumentEventHandler {
            private DocumentRenderer documentRenderer;

            public PageMarginBoxesDrawingHandler() {
            }

//\cond DO_NOT_DOCUMENT
            // Default constructor.
            internal DocumentRenderer.PageMarginBoxesDrawingHandler SetDocumentRenderer(DocumentRenderer documentRenderer
                ) {
                this.documentRenderer = documentRenderer;
                return this;
            }
//\endcond

            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                if (@event is PdfDocumentEvent) {
                    PdfPage page = ((PdfDocumentEvent)@event).GetPage();
                    PdfDocument pdfDoc = @event.GetDocument();
                    int pageNumber = pdfDoc.GetPageNumber(page);
                    ProcessPage(pdfDoc, pageNumber);
                }
            }

//\cond DO_NOT_DOCUMENT
            internal void ProcessPage(PdfDocument document, int pageNumber) {
                PageMarginBoxes pageMarginBoxes = documentRenderer.document.GetPageMargins(pageNumber);
                if (pageMarginBoxes != null) {
                    pageMarginBoxes.Draw(documentRenderer, document, pageNumber);
                }
            }
//\endcond
        }
    }
}
