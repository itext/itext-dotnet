using iText.Commons.Datastructures;
using iText.Kernel.Pdf;

namespace iText.Kernel.DI.Pagetree {
    /// <summary>This interface is used to create a list of pages from a pages dictionary.</summary>
    public interface IPageTreeListFactory {
        /// <summary>Creates a list based on the  value of the pages dictionary.</summary>
        /// <remarks>
        /// Creates a list based on the  value of the pages dictionary.
        /// If null, it means we are dealing with document creation. In other cases the pdf document pages
        /// dictionary will be passed.
        /// </remarks>
        /// <param name="pagesDictionary">The pages dictionary</param>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <returns>The list</returns>
        ISimpleList<T> CreateList<T>(PdfDictionary pagesDictionary);
    }
}
