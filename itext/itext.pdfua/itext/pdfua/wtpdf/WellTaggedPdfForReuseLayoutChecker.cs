using iText.Forms.Form.Renderer;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Tables;

namespace iText.Pdfua.Wtpdf {
    /// <summary>Performs layout checks for a PDF document being validated against the Well Tagged PDF for Reuse standard.
    ///     </summary>
    public class WellTaggedPdfForReuseLayoutChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new
        /// <see cref="WellTaggedPdfForReuseLayoutChecker"/>
        /// instance.
        /// </summary>
        /// <param name="context">the validation context</param>
        public WellTaggedPdfForReuseLayoutChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks renderer for PDF UA compliance.</summary>
        /// <param name="renderer">the renderer to check</param>
        public virtual void CheckRenderer(IRenderer renderer) {
            if (renderer == null) {
                return;
            }
            if (IsPartOfSignatureAppearance(renderer)) {
                // Tagging of the current layout element will be skipped in that case.
                return;
            }
            IPropertyContainer layoutElement = renderer.GetModelElement();
            if (layoutElement is Table) {
                new TableCheckUtil(context).CheckTable((Table)layoutElement);
            }
        }

        private static bool IsPartOfSignatureAppearance(IRenderer renderer) {
            IRenderer parent = renderer.GetParent();
            while (parent != null) {
                if (parent is SignatureAppearanceRenderer) {
                    return true;
                }
                parent = parent.GetParent();
            }
            return false;
        }
    }
}
