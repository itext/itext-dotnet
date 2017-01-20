/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class DocumentRenderer : RootRenderer {
        protected internal Document document;

        protected internal IList<int> wrappedContentPage = new List<int>();

        public DocumentRenderer(Document document)
            : this(document, true) {
        }

        public DocumentRenderer(Document document, bool immediateFlush) {
            this.document = document;
            this.immediateFlush = immediateFlush;
            this.modelElement = document;
        }

        public override LayoutArea GetOccupiedArea() {
            throw new InvalidOperationException("Not applicable for DocumentRenderer");
        }

        public override IRenderer GetNextRenderer() {
            return null;
        }

        protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
            AreaBreak areaBreak = overflowResult != null && overflowResult.GetAreaBreak() != null ? overflowResult.GetAreaBreak
                () : null;
            MoveToNextPage();
            while (areaBreak != null && areaBreak.GetAreaType() == AreaBreakType.LAST_PAGE && currentPageNumber < document
                .GetPdfDocument().GetNumberOfPages()) {
                MoveToNextPage();
            }
            PageSize customPageSize = areaBreak != null ? areaBreak.GetPageSize() : null;
            while (document.GetPdfDocument().GetNumberOfPages() >= currentPageNumber && document.GetPdfDocument().GetPage
                (currentPageNumber).IsFlushed()) {
                currentPageNumber++;
            }
            PageSize lastPageSize = EnsureDocumentHasNPages(currentPageNumber, customPageSize);
            if (lastPageSize == null) {
                lastPageSize = new PageSize(document.GetPdfDocument().GetPage(currentPageNumber).GetTrimBox());
            }
            return (currentArea = new LayoutArea(currentPageNumber, document.GetPageEffectiveArea(lastPageSize)));
        }

        protected internal override void FlushSingleRenderer(IRenderer resultRenderer) {
            if (!resultRenderer.IsFlushed()) {
                int pageNum = resultRenderer.GetOccupiedArea().GetPageNumber();
                PdfDocument pdfDocument = document.GetPdfDocument();
                EnsureDocumentHasNPages(pageNum, null);
                PdfPage correspondingPage = pdfDocument.GetPage(pageNum);
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

        protected internal virtual PageSize AddNewPage(PageSize customPageSize) {
            if (customPageSize != null) {
                document.GetPdfDocument().AddNewPage(customPageSize);
            }
            else {
                document.GetPdfDocument().AddNewPage();
            }
            return customPageSize != null ? customPageSize : document.GetPdfDocument().GetDefaultPageSize();
        }

        /// <summary>Adds some pages so that the overall number is at least n.</summary>
        /// <remarks>
        /// Adds some pages so that the overall number is at least n.
        /// Returns the page size of the n'th page.
        /// </remarks>
        private PageSize EnsureDocumentHasNPages(int n, PageSize customPageSize) {
            PageSize lastPageSize = null;
            while (document.GetPdfDocument().GetNumberOfPages() < n) {
                lastPageSize = AddNewPage(customPageSize);
            }
            return lastPageSize;
        }

        private void MoveToNextPage() {
            // We don't flush this page immediately, but only flush previous one because of manipulations with areas in case
            // of keepTogether property.
            if (immediateFlush && currentPageNumber > 1) {
                document.GetPdfDocument().GetPage(currentPageNumber - 1).Flush();
            }
            currentPageNumber++;
        }
    }
}
