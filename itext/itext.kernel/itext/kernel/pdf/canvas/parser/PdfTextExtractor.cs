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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public sealed class PdfTextExtractor {
        private PdfTextExtractor() {
        }

        /// <summary>Extract text from a specified page using an extraction strategy.</summary>
        /// <remarks>
        /// Extract text from a specified page using an extraction strategy.
        /// Also allows registration of custom IContentOperators that can influence
        /// how (and whether or not) the PDF instructions will be parsed.
        /// Extraction strategy must be passed as a new object for every single page.
        /// </remarks>
        /// <param name="page">the page for the text to be extracted from</param>
        /// <param name="strategy">the strategy to use for extracting text</param>
        /// <param name="additionalContentOperators">
        /// an optional map of custom
        /// <see cref="IContentOperator"/>
        /// s for rendering instructions
        /// </param>
        /// <returns>the extracted text</returns>
        public static String GetTextFromPage(PdfPage page, ITextExtractionStrategy strategy, IDictionary<String, IContentOperator
            > additionalContentOperators) {
            PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy, additionalContentOperators);
            parser.ProcessPageContent(page);
            return strategy.GetResultantText();
        }

        /// <summary>Extract text from a specified page using an extraction strategy.</summary>
        /// <remarks>
        /// Extract text from a specified page using an extraction strategy.
        /// Extraction strategy must be passed as a new object for every single page.
        /// </remarks>
        /// <param name="page">the page for the text to be extracted from</param>
        /// <param name="strategy">the strategy to use for extracting text</param>
        /// <returns>the extracted text</returns>
        public static String GetTextFromPage(PdfPage page, ITextExtractionStrategy strategy) {
            return GetTextFromPage(page, strategy, new Dictionary<String, IContentOperator>());
        }

        /// <summary>Extract text from a specified page using the default strategy.</summary>
        /// <remarks>
        /// Extract text from a specified page using the default strategy.
        /// Node: the default strategy is subject to change. If using a specific strategy
        /// is important, please use
        /// <see cref="GetTextFromPage(iText.Kernel.Pdf.PdfPage, iText.Kernel.Pdf.Canvas.Parser.Listener.ITextExtractionStrategy)
        ///     "/>.
        /// </remarks>
        /// <param name="page">the page for the text to be extracted from</param>
        /// <returns>the extracted text</returns>
        public static String GetTextFromPage(PdfPage page) {
            return GetTextFromPage(page, new LocationTextExtractionStrategy());
        }
    }
}
