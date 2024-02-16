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
            return new _ITagTreeIteratorHandler_26();
        }

        private sealed class _ITagTreeIteratorHandler_26 : ITagTreeIteratorHandler {
            public _ITagTreeIteratorHandler_26() {
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
