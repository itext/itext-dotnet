using iText.Commons.Datastructures;
using iText.Kernel.Pdf;

namespace iText.Kernel.DI.Pagetree {
    /// <summary>
    /// This class is a default implementation of
    /// <see cref="IPageTreeListFactory"/>
    /// that is used as a default.
    /// </summary>
    /// <remarks>
    /// This class is a default implementation of
    /// <see cref="IPageTreeListFactory"/>
    /// that is used as a default.
    /// <para />
    /// This class will create an arraylist when in creation mode.
    /// In reading and editing mode, it will create a NullUnlimitedList if the count is greater than the
    /// maxEntriesBeforeSwitchingToNullUnlimitedList. This is to prevent potential OOM exceptions when loading a document
    /// with a large number of pages where only a few pages are needed.
    /// </remarks>
    public class DefaultPageTreeListFactory : IPageTreeListFactory {
        private readonly int maxEntriesBeforeSwitchingToNullUnlimitedList;

        /// <summary>Creates a new instance of DefaultPageTreeListFactory.</summary>
        /// <param name="maxEntriesBeforeSwitchingToNullUnlimitedList">
        /// the maximum number of entries before switching to
        /// a NullUnlimitedList.
        /// </param>
        public DefaultPageTreeListFactory(int maxEntriesBeforeSwitchingToNullUnlimitedList) {
            this.maxEntriesBeforeSwitchingToNullUnlimitedList = maxEntriesBeforeSwitchingToNullUnlimitedList;
        }

        /// <summary>Creates a list based on the count value in the pages dictionary.</summary>
        /// <remarks>
        /// Creates a list based on the count value in the pages dictionary. If the count value is greater than the
        /// maxEntriesBeforeSwitchingToNullUnlimitedList, a NullUnlimitedList is created. This is to optimize memory usage
        /// when loading a document with a large number of pages where only a few pages are needed.
        /// </remarks>
        /// <param name="pagesDictionary">The pages dictionary</param>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <returns>The list</returns>
        public virtual ISimpleList<T> CreateList<T>(PdfDictionary pagesDictionary) {
            //If dictionary is null, it means we are dealing with document creation.
            if (pagesDictionary == null) {
                return new SimpleArrayList<T>();
            }
            PdfNumber count = pagesDictionary.GetAsNumber(PdfName.Count);
            if (count == null) {
                //If count is null, it means we are dealing with a possible corrupted document.
                //In this case we use NullUnlimitedList to avoid creating a huge list.
                return new NullUnlimitedList<T>();
            }
            int countValue = count.IntValue();
            if (countValue > maxEntriesBeforeSwitchingToNullUnlimitedList) {
                return new NullUnlimitedList<T>();
            }
            if (countValue < 0) {
                //If count is negative, it means we are dealing with a possible corrupted document.
                return new NullUnlimitedList<T>();
            }
            //Initial capacity is set to count value to avoid resizing of the list.
            return new SimpleArrayList<T>(countValue);
        }
    }
}
