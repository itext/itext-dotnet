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
using iText.Kernel.Pdf.Tagutils;
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
        /// <summary>Creates the handler that handles PDF/UA compliance for Formula tags</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagutils.ITagTreeIteratorHandler"/>
        /// The formula tag handler.
        /// </returns>
        public static ITagTreeIteratorHandler CreateFormulaTagHandler() {
            return new _ITagTreeIteratorHandler_48();
        }

        private sealed class _ITagTreeIteratorHandler_48 : ITagTreeIteratorHandler {
            public _ITagTreeIteratorHandler_48() {
            }

            public void NextElement(IStructureNode elem) {
                PdfStructElem structElem = TagTreeHandlerUtil.GetElementIfRoleMatches(PdfName.Formula, elem);
                if (structElem == null) {
                    return;
                }
                PdfDictionary pdfObject = structElem.GetPdfObject();
                if (iText.Pdfua.Checkers.Utils.FormulaCheckUtil.HasInvalidValues(pdfObject.GetAsString(PdfName.Alt), pdfObject
                    .GetAsString(PdfName.ActualText))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT);
                }
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
    }
}
