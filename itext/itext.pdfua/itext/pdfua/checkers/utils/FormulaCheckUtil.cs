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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    public sealed class FormulaCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="FormulaCheckUtil"/>
        /// instance.
        /// </summary>
        private FormulaCheckUtil() {
        }

        // Empty constructor
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
        public class FormulaTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="FormulaTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public FormulaTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            /// <summary><inheritDoc/></summary>
            public override void NextElement(IStructureNode elem) {
                PdfStructElem structElem = context.GetElementIfRoleMatches(PdfName.Formula, elem);
                if (structElem == null) {
                    return;
                }
                PdfDictionary pdfObject = structElem.GetPdfObject();
                if (HasInvalidValues(pdfObject.GetAsString(PdfName.Alt), pdfObject.GetAsString(PdfName.ActualText))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT);
                }
            }
        }
    }
}
