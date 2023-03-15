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
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    public class PdfSplitter {
        private PdfDocument pdfDocument;

        private bool preserveTagged;

        private bool preserveOutlines;

        private IMetaInfo metaInfo;

        /// <summary>Creates a new instance of PdfSplitter class.</summary>
        /// <param name="pdfDocument">the document to be split.</param>
        public PdfSplitter(PdfDocument pdfDocument) {
            if (pdfDocument.GetWriter() != null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_SPLIT_DOCUMENT_THAT_IS_BEING_WRITTEN);
            }
            this.pdfDocument = pdfDocument;
            this.preserveTagged = true;
            this.preserveOutlines = true;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        public virtual void SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        /// <summary>If original document is tagged, then by default all resultant document will also be tagged.</summary>
        /// <remarks>
        /// If original document is tagged, then by default all resultant document will also be tagged.
        /// This could be changed with this flag - if set to false, resultant documents will be not tagged, even if
        /// original document is tagged.
        /// </remarks>
        /// <param name="preserveTagged">defines whether the resultant documents need to be tagged</param>
        public virtual void SetPreserveTagged(bool preserveTagged) {
            this.preserveTagged = preserveTagged;
        }

        /// <summary>If original document has outlines, then by default all resultant document will also have outlines.
        ///     </summary>
        /// <remarks>
        /// If original document has outlines, then by default all resultant document will also have outlines.
        /// This could be changed with this flag - if set to false, resultant documents won't contain outlines, even if
        /// original document had them.
        /// </remarks>
        /// <param name="preserveOutlines">defines whether the resultant documents will preserve outlines or not</param>
        public virtual void SetPreserveOutlines(bool preserveOutlines) {
            this.preserveOutlines = preserveOutlines;
        }

        /// <summary>Splits the document basing on the given size specified in bytes.</summary>
        /// <param name="size"><strong>Preferred</strong> size specified in bytes for splitting.</param>
        /// <returns>
        /// The documents which the source document was split into.
        /// Be warned that these documents are not closed.
        /// </returns>
        public virtual IList<PdfDocument> SplitBySize(long size) {
            IList<PageRange> splitRanges = new List<PageRange>();
            int currentPage = 1;
            int numOfPages = pdfDocument.GetNumberOfPages();
            while (currentPage <= numOfPages) {
                PageRange nextRange = GetNextRange(currentPage, numOfPages, size);
                splitRanges.Add(nextRange);
                IList<int> allPages = nextRange.GetQualifyingPageNums(numOfPages);
                currentPage = (int)allPages[allPages.Count - 1] + 1;
            }
            return ExtractPageRanges(splitRanges);
        }

        /// <summary>Splits the document by page numbers.</summary>
        /// <param name="pageNumbers">
        /// the numbers of pages from which another document is to be started.
        /// If the first element is not 1, then 1 is implied (i.e. the first split document will start from page 1 in any case).
        /// </param>
        /// <param name="documentReady">
        /// the event listener which is called when another document is ready.
        /// You can close this document in this listener, for instance.
        /// </param>
        public virtual void SplitByPageNumbers(IList<int> pageNumbers, PdfSplitter.IDocumentReadyListener documentReady
            ) {
            int currentPageNumber = 1;
            for (int ind = 0; ind <= pageNumbers.Count; ind++) {
                int nextPageNumber = ind == pageNumbers.Count ? pdfDocument.GetNumberOfPages() + 1 : (int)pageNumbers[ind];
                if (ind == 0 && nextPageNumber == 1) {
                    continue;
                }
                PageRange currentPageRange = new PageRange().AddPageSequence(currentPageNumber, nextPageNumber - 1);
                PdfDocument currentDocument = CreatePdfDocument(currentPageRange);
                pdfDocument.CopyPagesTo(currentPageNumber, nextPageNumber - 1, currentDocument);
                documentReady.DocumentReady(currentDocument, currentPageRange);
                currentPageNumber = nextPageNumber;
            }
        }

        /// <summary>Splits the document by page numbers.</summary>
        /// <param name="pageNumbers">
        /// the numbers of pages from which another document is to be started.
        /// If the first element is not 1, then 1 is implied (i.e. the first split document will start from page 1 in any case).
        /// </param>
        /// <returns>the list of resultant documents. By warned that they are not closed.</returns>
        public virtual IList<PdfDocument> SplitByPageNumbers(IList<int> pageNumbers) {
            IList<PdfDocument> splitDocuments = new List<PdfDocument>();
            SplitByPageNumbers(pageNumbers, new PdfSplitter.SplitReadyListener(splitDocuments));
            return splitDocuments;
        }

        /// <summary>Splits a document into smaller documents with no more than @pageCount pages each.</summary>
        /// <param name="pageCount">the biggest possible number of pages in a split document.</param>
        /// <param name="documentReady">
        /// the event listener which is called when another document is ready.
        /// You can close this document in this listener, for instance.
        /// </param>
        public virtual void SplitByPageCount(int pageCount, PdfSplitter.IDocumentReadyListener documentReady) {
            for (int startPage = 1; startPage <= pdfDocument.GetNumberOfPages(); startPage += pageCount) {
                int endPage = Math.Min(startPage + pageCount - 1, pdfDocument.GetNumberOfPages());
                PageRange currentPageRange = new PageRange().AddPageSequence(startPage, endPage);
                PdfDocument currentDocument = CreatePdfDocument(currentPageRange);
                pdfDocument.CopyPagesTo(startPage, endPage, currentDocument);
                documentReady.DocumentReady(currentDocument, currentPageRange);
            }
        }

        /// <summary>Splits a document into smaller documents with no more than @pageCount pages each.</summary>
        /// <param name="pageCount">the biggest possible number of pages in a split document.</param>
        /// <returns>the list of resultant documents. By warned that they are not closed.</returns>
        public virtual IList<PdfDocument> SplitByPageCount(int pageCount) {
            IList<PdfDocument> splitDocuments = new List<PdfDocument>();
            SplitByPageCount(pageCount, new PdfSplitter.SplitReadyListener(splitDocuments));
            return splitDocuments;
        }

        /// <summary>Extracts the specified page ranges from a document.</summary>
        /// <param name="pageRanges">the list of page ranges for each of the resultant document.</param>
        /// <returns>
        /// the list of the resultant documents for each of the specified page range.
        /// Be warned that these documents are not closed.
        /// </returns>
        public virtual IList<PdfDocument> ExtractPageRanges(IList<PageRange> pageRanges) {
            IList<PdfDocument> splitDocuments = new List<PdfDocument>();
            foreach (PageRange currentPageRange in pageRanges) {
                PdfDocument currentPdfDocument = CreatePdfDocument(currentPageRange);
                splitDocuments.Add(currentPdfDocument);
                pdfDocument.CopyPagesTo(currentPageRange.GetQualifyingPageNums(pdfDocument.GetNumberOfPages()), currentPdfDocument
                    );
            }
            return splitDocuments;
        }

        /// <summary>Extracts the specified page ranges from a document.</summary>
        /// <param name="pageRange">the page range to be extracted from the document.</param>
        /// <returns>
        /// the resultant document containing the pages specified by the provided page range.
        /// Be warned that this document is not closed.
        /// </returns>
        public virtual PdfDocument ExtractPageRange(PageRange pageRange) {
            return ExtractPageRanges(JavaCollectionsUtil.SingletonList(pageRange))[0];
        }

        public virtual PdfDocument GetPdfDocument() {
            return pdfDocument;
        }

        /// <summary>This method is called when another split document is to be created.</summary>
        /// <remarks>
        /// This method is called when another split document is to be created.
        /// You can override this method and return your own
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// depending on your needs.
        /// </remarks>
        /// <param name="documentPageRange">
        /// the page range of the original document to be included
        /// in the document being created now.
        /// </param>
        /// <returns>the PdfWriter instance for the document which is being created.</returns>
        protected internal virtual PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
            return new PdfWriter(new ByteArrayOutputStream());
        }

        private PdfDocument CreatePdfDocument(PageRange currentPageRange) {
            PdfDocument newDocument = new PdfDocument(GetNextPdfWriter(currentPageRange), new DocumentProperties().SetEventCountingMetaInfo
                (metaInfo));
            if (pdfDocument.IsTagged() && preserveTagged) {
                newDocument.SetTagged();
            }
            if (pdfDocument.HasOutlines() && preserveOutlines) {
                newDocument.InitializeOutlines();
            }
            return newDocument;
        }

        public interface IDocumentReadyListener {
            void DocumentReady(PdfDocument pdfDocument, PageRange pageRange);
        }

        /// <summary>
        /// Split a document by outline title (bookmark name), find outline by name
        /// and places the entire hierarchy in a separate document ( outlines and pages ) .
        /// </summary>
        /// <param name="outlineTitles">list of outline titles .</param>
        /// <returns>
        /// Collection of
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// which contains split parts of a document
        /// </returns>
        public virtual IList<PdfDocument> SplitByOutlines(IList<String> outlineTitles) {
            if (outlineTitles == null || outlineTitles.Count == 0) {
                return JavaCollectionsUtil.EmptyList<PdfDocument>();
            }
            IList<PdfDocument> documentList = new List<PdfDocument>(outlineTitles.Count);
            foreach (String title in outlineTitles) {
                PdfDocument document = SplitByOutline(title);
                if (document != null) {
                    documentList.Add(document);
                }
            }
            return documentList;
        }

        private PdfDocument SplitByOutline(String outlineTitle) {
            int startPage = -1;
            int endPage = -1;
            PdfDocument toDocument = CreatePdfDocument(null);
            int size = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= size; i++) {
                PdfPage pdfPage = pdfDocument.GetPage(i);
                IList<PdfOutline> outlineList = pdfPage.GetOutlines(false);
                if (outlineList != null) {
                    foreach (PdfOutline pdfOutline in outlineList) {
                        if (pdfOutline.GetTitle().Equals(outlineTitle)) {
                            startPage = pdfDocument.GetPageNumber(pdfPage);
                            PdfOutline nextOutLine = GetAbsoluteTreeNextOutline(pdfOutline);
                            if (nextOutLine != null) {
                                endPage = pdfDocument.GetPageNumber(GetPageByOutline(i, nextOutLine)) - 1;
                            }
                            else {
                                endPage = size;
                            }
                            // fix case: if two sequential bookmark point to one page
                            if (startPage - endPage == 1) {
                                endPage = startPage;
                            }
                            break;
                        }
                    }
                }
            }
            if (startPage == -1 || endPage == -1) {
                return null;
            }
            pdfDocument.CopyPagesTo(startPage, endPage, toDocument);
            return toDocument;
        }

        private PdfPage GetPageByOutline(int fromPage, PdfOutline outline) {
            int size = pdfDocument.GetNumberOfPages();
            for (int i = fromPage; i <= size; i++) {
                PdfPage pdfPage = pdfDocument.GetPage(i);
                IList<PdfOutline> outlineList = pdfPage.GetOutlines(false);
                if (outlineList != null) {
                    foreach (PdfOutline pdfOutline in outlineList) {
                        if (pdfOutline.Equals(outline)) {
                            return pdfPage;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>the next element in the entire hierarchy</summary>
        /// <param name="outline"></param>
        private PdfOutline GetAbsoluteTreeNextOutline(PdfOutline outline) {
            PdfObject nextPdfObject = outline.GetContent().Get(PdfName.Next);
            PdfOutline nextPdfOutline = null;
            if (outline.GetParent() != null && nextPdfObject != null) {
                foreach (PdfOutline pdfOutline in outline.GetParent().GetAllChildren()) {
                    if (pdfOutline.GetContent().GetIndirectReference().Equals(nextPdfObject.GetIndirectReference())) {
                        nextPdfOutline = pdfOutline;
                        break;
                    }
                }
            }
            if (nextPdfOutline == null && outline.GetParent() != null) {
                nextPdfOutline = GetAbsoluteTreeNextOutline(outline.GetParent());
            }
            return nextPdfOutline;
        }

        private PageRange GetNextRange(int startPage, int endPage, long size) {
            PdfResourceCounter counter = new PdfResourceCounter(pdfDocument.GetTrailer());
            IDictionary<int, PdfObject> resources = counter.GetResources();
            // initialize with trailer length
            long lengthWithoutXref = counter.GetLength(null);
            int currentPage = startPage;
            bool oversized = false;
            do {
                PdfPage page = pdfDocument.GetPage(currentPage++);
                counter = new PdfResourceCounter(page.GetPdfObject());
                lengthWithoutXref += counter.GetLength(resources);
                resources.AddAll(counter.GetResources());
                if (lengthWithoutXref + XrefLength(resources.Count) > size) {
                    oversized = true;
                }
            }
            while (currentPage <= endPage && !oversized);
            // true if at least the first page to be copied didn't cause the oversize
            if (oversized && (currentPage - 1) != startPage) {
                // we shouldn't copy previous page because it caused
                // the oversize and it isn't the first page to be copied
                --currentPage;
            }
            return new PageRange().AddPageSequence(startPage, currentPage - 1);
        }

        private long XrefLength(int size) {
            return 20L * (size + 1);
        }

        private sealed class SplitReadyListener : PdfSplitter.IDocumentReadyListener {
            private IList<PdfDocument> splitDocuments;

            public SplitReadyListener(IList<PdfDocument> splitDocuments) {
                this.splitDocuments = splitDocuments;
            }

            public void DocumentReady(PdfDocument pdfDocument, PageRange pageRange) {
                splitDocuments.Add(pdfDocument);
            }
        }
    }
}
