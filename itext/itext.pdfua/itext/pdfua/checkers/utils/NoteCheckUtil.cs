using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class for delegating notes checks to the correct checking logic.</summary>
    public class NoteCheckUtil {
        /// <summary>Handler for checking Note elements in the TagTree.</summary>
        public class NoteTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="NoteTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public NoteTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            /// <summary><inheritDoc/></summary>
            public override void NextElement(IStructureNode elem) {
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
