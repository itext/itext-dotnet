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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua1 {
    /// <summary>Class that provides methods for checking PDF/UA-1 compliance of Formula elements.</summary>
    public sealed class PdfUA1FormulaChecker {
        private readonly PdfUAValidationContext context;

        private PdfUA1FormulaChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks "Formula" structure element.</summary>
        /// <param name="elem">structure element to check</param>
        public void CheckStructElement(IStructureNode elem) {
            PdfStructElem structElem = context.GetElementIfRoleMatches(PdfName.Formula, elem);
            if (structElem == null) {
                return;
            }
            PdfDictionary pdfObject = structElem.GetPdfObject();
            if (HasInvalidValues(pdfObject.GetAsString(PdfName.Alt), pdfObject.GetAsString(PdfName.ActualText))) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT);
            }
        }

        private static bool HasInvalidValues(PdfString altText, PdfString actualText) {
            String altTextValue = null;
            if (altText != null) {
                altTextValue = altText.GetValue();
            }
            String actualTextValue = null;
            if (actualText != null) {
                actualTextValue = actualText.GetValue();
            }
            return !(!(altTextValue == null || String.IsNullOrEmpty(altTextValue)) || actualTextValue != null);
        }

        /// <summary>Handler for checking Formula elements in the TagTree.</summary>
        public class PdfUA1FormulaTagHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA1FormulaChecker checker;

            /// <summary>
            /// Creates a new
            /// <see cref="PdfUA1FormulaTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public PdfUA1FormulaTagHandler(PdfUAValidationContext context)
                : base(context) {
                this.checker = new PdfUA1FormulaChecker(context);
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                checker.CheckStructElement(elem);
            }
        }
    }
}
