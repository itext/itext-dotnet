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
