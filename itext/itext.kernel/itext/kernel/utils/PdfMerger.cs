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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    /// <summary>Helper class to merge a number of existing documents into one.</summary>
    public class PdfMerger {
        private PdfDocument pdfDocument;

        private PdfMergerProperties properties;

        /// <summary>This class is used to merge a number of existing documents into one.</summary>
        /// <remarks>
        /// This class is used to merge a number of existing documents into one. By default, if source document
        /// contains tags and outlines, they will be also copied to the destination document.
        /// </remarks>
        /// <param name="pdfDocument">the document into which source documents will be merged</param>
        public PdfMerger(PdfDocument pdfDocument)
            : this(pdfDocument, new PdfMergerProperties().SetMergeTags(true).SetMergeOutlines(true)) {
        }

        /// <summary>This class is used to merge a number of existing documents into one.</summary>
        /// <param name="pdfDocument">the document into which source documents will be merged</param>
        /// <param name="properties">properties for the created <c>PdfMerger</c></param>
        public PdfMerger(PdfDocument pdfDocument, PdfMergerProperties properties) {
            this.pdfDocument = pdfDocument;
            this.properties = properties != null ? properties : new PdfMergerProperties();
        }

        /// <summary>
        /// If set to <i>true</i> then passed to the <i>
        /// <c>PdfMerger#merge</c>
        /// </i> method source documents will be closed
        /// immediately after merging specified pages into current document.
        /// </summary>
        /// <remarks>
        /// If set to <i>true</i> then passed to the <i>
        /// <c>PdfMerger#merge</c>
        /// </i> method source documents will be closed
        /// immediately after merging specified pages into current document. If <i>false</i> - PdfDocuments are left open.
        /// Default value - <i>false</i>.
        /// </remarks>
        /// <param name="closeSourceDocuments">should be true to close pdf documents in merge method</param>
        /// <returns>
        /// this
        /// <c>PdfMerger</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Utils.PdfMerger SetCloseSourceDocuments(bool closeSourceDocuments) {
            this.properties.SetCloseSrcDocuments(closeSourceDocuments);
            return this;
        }

        /// <summary>This method merges pages from the source document to the current one.</summary>
        /// <remarks>
        /// This method merges pages from the source document to the current one.
        /// <para />
        /// If <i>closeSourceDocuments</i> flag is set to <i>true</i> (see
        /// <see cref="SetCloseSourceDocuments(bool)"/>
        /// ),
        /// passed
        /// <c>PdfDocument</c>
        /// will be closed after pages are merged.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.PdfDocument.CopyPagesTo(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfDocument)
        ///     "/>.
        /// </remarks>
        /// <param name="from">- document, from which pages will be copied</param>
        /// <param name="fromPage">- start page in the range of pages to be copied</param>
        /// <param name="toPage">- end (inclusive) page in the range to be copied</param>
        /// <returns>
        /// this
        /// <c>PdfMerger</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Utils.PdfMerger Merge(PdfDocument from, int fromPage, int toPage) {
            IList<int> pages = new List<int>(toPage - fromPage);
            for (int pageNum = fromPage; pageNum <= toPage; pageNum++) {
                pages.Add(pageNum);
            }
            return Merge(from, pages);
        }

        /// <summary>This method merges pages from the source document to the current one.</summary>
        /// <remarks>
        /// This method merges pages from the source document to the current one.
        /// <para />
        /// If <i>closeSourceDocuments</i> flag is set to <i>true</i> (see
        /// <see cref="SetCloseSourceDocuments(bool)"/>
        /// ),
        /// passed
        /// <c>PdfDocument</c>
        /// will be closed after pages are merged.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.PdfDocument.CopyPagesTo(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfDocument)
        ///     "/>.
        /// </remarks>
        /// <param name="from">- document, from which pages will be copied</param>
        /// <param name="pages">- List of numbers of pages which will be copied</param>
        /// <returns>
        /// this
        /// <c>PdfMerger</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Utils.PdfMerger Merge(PdfDocument from, IList<int> pages) {
            return Merge(from, pages, null);
        }

        /// <summary>This method merges pages from the source document to the current one.</summary>
        /// <remarks>
        /// This method merges pages from the source document to the current one.
        /// <para />
        /// If <i>closeSourceDocuments</i> flag is set to <i>true</i> (see
        /// <see cref="SetCloseSourceDocuments(bool)"/>
        /// ),
        /// passed
        /// <c>PdfDocument</c>
        /// will be closed after pages are merged.
        /// <para />
        /// See also
        /// <see cref="iText.Kernel.Pdf.PdfDocument.CopyPagesTo(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfDocument)
        ///     "/>.
        /// </remarks>
        /// <param name="from">- document, from which pages will be copied</param>
        /// <param name="pages">- List of numbers of pages which will be copied</param>
        /// <param name="copier">
        /// - a copier which bears a special copy logic. May be null.
        /// It is recommended to use the same instance of
        /// <see cref="iText.Kernel.Pdf.IPdfPageExtraCopier"/>
        /// for the same output document.
        /// </param>
        /// <returns>
        /// this
        /// <c>PdfMerger</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Utils.PdfMerger Merge(PdfDocument from, IList<int> pages, IPdfPageExtraCopier 
            copier) {
            if (properties.IsMergeTags() && from.IsTagged()) {
                pdfDocument.SetTagged();
            }
            if (properties.IsMergeOutlines() && from.HasOutlines()) {
                pdfDocument.InitializeOutlines();
            }
            if (properties.IsMergeScripts()) {
                PdfScriptMerger.MergeScripts(from, this.pdfDocument);
            }
            from.CopyPagesTo(pages, pdfDocument, copier);
            if (properties.IsCloseSrcDocuments()) {
                from.Close();
            }
            return this;
        }

        /// <summary>Closes the current document.</summary>
        /// <remarks>
        /// Closes the current document.
        /// <para />
        /// It is a complete equivalent of calling
        /// <c>PdfDocument#close</c>
        /// on the PdfDocument
        /// passed to the constructor of this PdfMerger instance. This means that it is enough to call
        /// <i>close</i> either on passed PdfDocument or on this PdfMerger instance, but there is no need
        /// to call them both.
        /// </remarks>
        public virtual void Close() {
            pdfDocument.Close();
        }
    }
}
