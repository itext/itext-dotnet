using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// Handler for
    /// <see cref="TagTreeIterator"/>.
    /// </summary>
    /// <remarks>
    /// Handler for
    /// <see cref="TagTreeIterator"/>.
    /// Is used to handle specific events during the traversal.
    /// </remarks>
    public interface ITagTreeIteratorHandler {
        /// <summary>Called when the next element is reached during the traversal.</summary>
        /// <param name="elem">the next element</param>
        void NextElement(IStructureNode elem);
    }
}
