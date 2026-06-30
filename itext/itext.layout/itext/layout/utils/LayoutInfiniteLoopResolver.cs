/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
