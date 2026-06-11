namespace iText.Layout.Utils {
    /// <summary>This resolver is used during the layout process to prevent infinite loops.</summary>
    public class LayoutInfiniteLoopResolver {
        private const int DEFAULT_LIMIT = 1_000_000;

        private readonly int maxPagesCountForSingleElement;

        /// <summary>
        /// Creates default instance of
        /// <see cref="LayoutInfiniteLoopResolver"/>.
        /// </summary>
        /// <remarks>
        /// Creates default instance of
        /// <see cref="LayoutInfiniteLoopResolver"/>
        /// . Limit in this case is set to 333_333 pages.
        /// </remarks>
        public LayoutInfiniteLoopResolver() {
            maxPagesCountForSingleElement = DEFAULT_LIMIT;
        }

        /// <summary>
        /// Creates
        /// <see cref="LayoutInfiniteLoopResolver"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="LayoutInfiniteLoopResolver"/>
        /// instance.
        /// <para />
        /// This resolver is used during the layout process to prevent infinite loops. In particular,
        /// it limits the amount of times same element will be split across multiple pages. If the limit is exceeded,
        /// exception is thrown. It is guaranteed, that this limit will not be exceeded,
        /// unless the document contains at least the same amount of pages, as specified in the limit.
        /// </remarks>
        /// <param name="maxPagesCountForSingleElement">
        /// property which defines,
        /// how many times single element can be split across multiple pages
        /// </param>
        public LayoutInfiniteLoopResolver(int maxPagesCountForSingleElement) {
            // In here we multiply provided number by 3,
            // because only after the multiplication it corresponds to the number of pages per each element.
            // In particular same element may be layouted on the same page if keep_together or forced_placement is set.
            this.maxPagesCountForSingleElement = maxPagesCountForSingleElement * 3;
        }

        /// <summary>Gets maximum pages count per element.</summary>
        /// <remarks>
        /// Gets maximum pages count per element.
        /// <para />
        /// This property defines, how many times single element can be split across multiple pages.
        /// It is used to detect potential infinite loops during the layout process.
        /// </remarks>
        /// <returns>maximum pages count per element</returns>
        public virtual int GetMaxPagesCountForSingleElement() {
            return maxPagesCountForSingleElement;
        }
    }
}
