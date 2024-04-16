using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Class that provides methods for searching mcr in tag tree.</summary>
    public sealed class McrCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="McrCheckUtil"/>
        /// instance.
        /// </summary>
        private McrCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if tag structure of TR element contains any mcr.</summary>
        /// <param name="elementTR">PdfDictionary of TR element.</param>
        /// <returns>true if mcr found.</returns>
        public static bool IsTrContainsMcr(PdfDictionary elementTR) {
            TagTreeIterator tagTreeIterator = new TagTreeIterator(new PdfStructElem(elementTR));
            McrCheckUtil.McrTagHandler handler = new McrCheckUtil.McrTagHandler();
            tagTreeIterator.AddHandler(handler);
            tagTreeIterator.Traverse();
            return handler.TagTreeHaveMcr();
        }

        /// <summary>Search for mcr elements in the TagTree.</summary>
        private class McrTagHandler : ITagTreeIteratorHandler {
            private bool haveMcr = false;

            /// <summary>Method returns if tag tree has mcr in it.</summary>
            public virtual bool TagTreeHaveMcr() {
                return haveMcr;
            }

            /// <summary>
            /// Creates a new
            /// <see cref="McrTagHandler"/>
            /// instance.
            /// </summary>
            public McrTagHandler() {
            }

            //empty constructor
            /// <summary><inheritDoc/></summary>
            public virtual void NextElement(IStructureNode elem) {
                if ((elem is PdfMcr)) {
                    haveMcr = true;
                }
            }
        }
    }
}
