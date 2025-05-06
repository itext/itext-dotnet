/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Validation.Context;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs UA-2 checks related to Replacements and Alternatives.</summary>
    public class PdfUA2CanvasTextChecker {
        private readonly IList<CanvasTextAdditionContext> textWithPua = new List<CanvasTextAdditionContext>();

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2CanvasTextChecker"/>
        /// instance.
        /// </summary>
        public PdfUA2CanvasTextChecker() {
        }

        // Empty constructor.
        /// <summary>Collects all text strings, which contain PUA Unicode values.</summary>
        /// <param name="context">
        /// 
        /// <see cref="iText.Kernel.Validation.Context.CanvasTextAdditionContext"/>
        /// which contains all the data needed for validation
        /// </param>
        public virtual void CollectTextAdditionContext(CanvasTextAdditionContext context) {
            String text = context.GetText();
            PdfDictionary attributes = context.GetAttributes();
            PdfString alt = null;
            PdfString actualText = null;
            if (attributes != null) {
                alt = attributes.GetAsString(PdfName.Alt);
                actualText = attributes.GetAsString(PdfName.ActualText);
            }
            if (PdfUA2StringChecker.StringContainsPua(text)) {
                if (alt == null && actualText == null) {
                    textWithPua.Add(context);
                }
            }
        }

        /// <summary>Checks previously collected data according to Replacements and Alternatives UA-2 rules.</summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to be checked
        /// </param>
        public virtual void CheckCollectedContexts(PdfDocument document) {
            foreach (CanvasTextAdditionContext context in textWithPua) {
                if (context.GetMcId() == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT);
                }
                PdfMcr mcr = FindMcrByMcId(document, context.GetMcId(), context.GetContentStream());
                if (mcr == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT);
                }
                IStructureNode structureNode = mcr.GetParent();
                if (!(structureNode is PdfStructElem)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT);
                }
                PdfStructElem structElem = (PdfStructElem)structureNode;
                PdfString alt = structElem.GetAlt();
                PdfString actualText = structElem.GetActualText();
                if (alt == null && actualText == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT);
                }
            }
        }

        private static PdfMcr FindMcrByMcId(PdfDocument document, PdfNumber mcId, PdfStream contentStream) {
            for (int i = 1; i <= document.GetNumberOfPages(); ++i) {
                PdfPage page = document.GetPage(i);
                for (int j = 0; j < page.GetContentStreamCount(); ++j) {
                    PdfStream pageStream = page.GetContentStream(j);
                    if (pageStream.GetIndirectReference().Equals(contentStream.GetIndirectReference())) {
                        PdfMcr mcr = document.GetStructTreeRoot().FindMcrByMcid(page.GetPdfObject(), mcId.IntValue());
                        if (mcr != null) {
                            return mcr;
                        }
                    }
                }
            }
            return null;
        }
    }
}
