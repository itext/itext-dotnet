using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that holds the validation context while iterating the tag tree structure.</summary>
    public abstract class ContextAwareTagTreeIteratorHandler : ITagTreeIteratorHandler {
        protected internal readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ContextAwareTagTreeIteratorHandler"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        protected internal ContextAwareTagTreeIteratorHandler(PdfUAValidationContext context) {
            this.context = context;
        }

        public abstract void NextElement(IStructureNode arg1);
    }
}
