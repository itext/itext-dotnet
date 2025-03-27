using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs "Formula" tag related checks according to PDF/UA-2 specification.</summary>
    public sealed class PdfUA2FormulaChecker {
        private const String MATH = "math";

        private readonly PdfUAValidationContext context;

        private PdfUA2FormulaChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if "math" structure element from "MathML" namespace is enclosed within "Formula" tag.</summary>
        /// <param name="elem">structure element to check</param>
        public void CheckStructElement(IStructureNode elem) {
            String role = context.ResolveToStandardRole(elem);
            if (role == null) {
                return;
            }
            if (MATH.Equals(role)) {
                PdfStructElem mathStructElem = context.GetElementIfRoleMatches(new PdfName(MATH), elem);
                if (mathStructElem != null) {
                    IStructureNode parent = mathStructElem.GetParent();
                    if (parent != null) {
                        if (!StandardRoles.FORMULA.Equals(context.ResolveToStandardRole(parent))) {
                            throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA);
                        }
                    }
                }
            }
        }

        /// <summary>Handler class that checks "Formula" tags while traversing the tag tree.</summary>
        public class PdfUA2FormulaTagHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2FormulaChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA2FormulaTagHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2FormulaTagHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA2FormulaChecker(context);
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
