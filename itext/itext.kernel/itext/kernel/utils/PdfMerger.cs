/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    public class PdfMerger {
        private PdfDocument pdfDocument;

        private bool closeSrcDocuments;

        private bool mergeTags;

        private bool mergeOutlines;

        /// <summary>This class is used to merge a number of existing documents into one.</summary>
        /// <remarks>
        /// This class is used to merge a number of existing documents into one. By default, if source document
        /// contains tags and outlines, they will be also copied to the destination document.
        /// </remarks>
        /// <param name="pdfDocument">the document into which source documents will be merged</param>
        public PdfMerger(PdfDocument pdfDocument)
            : this(pdfDocument, true, true) {
        }

        /// <summary>This class is used to merge a number of existing documents into one.</summary>
        /// <param name="pdfDocument">the document into which source documents will be merged</param>
        /// <param name="mergeTags">
        /// if true, then tags from the source document are copied even if destination document is not set as
        /// tagged. Note, that if false, tag structure is still could be copied if the destination document
        /// is explicitly marked as tagged with
        /// <see cref="iText.Kernel.Pdf.PdfDocument.SetTagged()"/>
        /// </param>
        /// <param name="mergeOutlines">
        /// if true, then outlines from the source document are copied even if in destination document
        /// outlines are not initialized. Note, that if false, outlines are still could be copied if the
        /// destination document outlines were explicitly initialized with
        /// <see cref="iText.Kernel.Pdf.PdfDocument.InitializeOutlines()"/>
        /// </param>
        public PdfMerger(PdfDocument pdfDocument, bool mergeTags, bool mergeOutlines) {
            this.pdfDocument = pdfDocument;
            this.mergeTags = mergeTags;
            this.mergeOutlines = mergeOutlines;
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
            this.closeSrcDocuments = closeSourceDocuments;
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
            if (mergeTags && from.IsTagged()) {
                pdfDocument.SetTagged();
            }
            if (mergeOutlines && from.HasOutlines()) {
                pdfDocument.InitializeOutlines();
            }
            from.CopyPagesTo(pages, pdfDocument);
            if (closeSrcDocuments) {
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
