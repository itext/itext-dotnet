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
