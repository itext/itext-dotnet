using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class that contains utility methods used  when working with the TagTreeHandler</summary>
    public sealed class TagTreeHandlerUtil {
        private TagTreeHandlerUtil() {
        }

        //Empty constructor.
        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// if the element matches the provided role and the structureNode is indeed an
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// </summary>
        /// <param name="role">The role that needs to be matched.</param>
        /// <param name="structureNode">The structure node.</param>
        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// if the structure matches the role.
        /// </returns>
        public static PdfStructElem GetElementIfRoleMatches(PdfName role, IStructureNode structureNode) {
            if (structureNode == null) {
                return null;
            }
            if (!(structureNode is PdfStructElem)) {
                return null;
            }
            if (!role.Equals(structureNode.GetRole())) {
                return null;
            }
            return (PdfStructElem)structureNode;
        }
    }
}
