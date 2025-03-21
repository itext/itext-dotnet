using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua1 {
    /// <summary>Utility class for delegating notes checks to the correct checking logic.</summary>
    public sealed class PdfUA1NotesChecker {
        private PdfUA1NotesChecker() {
        }

        // Empty constructor.
        /// <summary>Handler for checking Note elements in the TagTree.</summary>
        public class PdfUA1NotesTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="PdfUA1NotesTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public PdfUA1NotesTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                PdfStructElem structElem = context.GetElementIfRoleMatches(PdfName.Note, elem);
                if (structElem == null) {
                    return;
                }
                PdfDictionary pdfObject = structElem.GetPdfObject();
                if (pdfObject.Get(PdfName.ID) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY);
                }
            }
        }
    }
}
