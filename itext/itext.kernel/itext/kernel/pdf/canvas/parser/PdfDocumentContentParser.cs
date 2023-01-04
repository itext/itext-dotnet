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
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <summary>
    /// A utility class that makes it cleaner to process content from pages of a
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// through a specified RenderListener.
    /// </summary>
    public class PdfDocumentContentParser {
        private readonly PdfDocument pdfDocument;

        public PdfDocumentContentParser(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary>Processes content from the specified page number using the specified listener.</summary>
        /// <remarks>
        /// Processes content from the specified page number using the specified listener.
        /// Also allows registration of custom IContentOperators that can influence
        /// how (and whether or not) the PDF instructions will be parsed.
        /// </remarks>
        /// <typeparam name="E">the type of the renderListener - this makes it easy to chain calls</typeparam>
        /// <param name="pageNumber">the page number to process</param>
        /// <param name="renderListener">the listener that will receive render callbacks</param>
        /// <param name="additionalContentOperators">an optional map of custom ContentOperators for rendering instructions
        ///     </param>
        /// <returns>the provided renderListener</returns>
        public virtual E ProcessContent<E>(int pageNumber, E renderListener, IDictionary<String, IContentOperator>
             additionalContentOperators)
            where E : IEventListener {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(renderListener, additionalContentOperators);
            processor.ProcessPageContent(pdfDocument.GetPage(pageNumber));
            return renderListener;
        }

        /// <summary>Processes content from the specified page number using the specified listener</summary>
        /// <typeparam name="E">the type of the renderListener - this makes it easy to chain calls</typeparam>
        /// <param name="pageNumber">the page number to process</param>
        /// <param name="renderListener">the listener that will receive render callbacks</param>
        /// <returns>the provided renderListener</returns>
        public virtual E ProcessContent<E>(int pageNumber, E renderListener)
            where E : IEventListener {
            return ProcessContent(pageNumber, renderListener, new Dictionary<String, IContentOperator>());
        }
    }
}
