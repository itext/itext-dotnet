using System;
using iText.Kernel.Pdf;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Ua2;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Wtpdf {
    public class WellTaggedPdfForReuseAnnotationChecker : PdfUA2AnnotationChecker {
        /// <summary>
        /// Creates a new instance of the
        /// <see cref="WellTaggedPdfForReuseAnnotationChecker"/>.
        /// </summary>
        public WellTaggedPdfForReuseAnnotationChecker() {
        }

        // Empty constructor.
        protected internal override void CheckRequiredContentsEntry(PdfName subtype, PdfDictionary annotation) {
            if (PdfName.Screen.Equals(subtype)) {
                PdfString contents = annotation.GetAsString(PdfName.Contents);
                if (contents == null || String.IsNullOrEmpty(contents.GetValue())) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY);
                }
            }
        }

        /// <summary>Handler for checking annotation elements in the tag tree.</summary>
        public class WellTaggedPdfForReuseAnnotationHandler : PdfUA2AnnotationChecker.PdfUA2AnnotationHandler {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="iText.Pdfua.Checkers.Utils.Ua2.PdfUA2AnnotationChecker.PdfUA2AnnotationHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public WellTaggedPdfForReuseAnnotationHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new WellTaggedPdfForReuseAnnotationChecker();
            }
        }
    }
}
