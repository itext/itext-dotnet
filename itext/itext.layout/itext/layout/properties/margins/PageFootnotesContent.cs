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
namespace iText.Layout.Properties.Margins {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Helper class to store information about footnotes content on the page represented by
    /// <see cref="FootnotesContainer"/>.
    /// </summary>
    internal class PageFootnotesContent : AbstractPageContent {
        private int pageNumber = 0;

        /// <summary>
        /// Creates new
        /// <see cref="PageFootnotesContent"/>
        /// instance.
        /// </summary>
        /// <param name="footnotesContent">
        /// 
        /// <see cref="FootnotesContainer"/>
        /// container with all page footnotes
        /// </param>
        public PageFootnotesContent(FootnotesContainer footnotesContent)
            : base(footnotesContent) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="PageFootnotesContent"/>
        /// instance by copying existing one.
        /// </summary>
        /// <param name="other">
        /// 
        /// <see cref="PageFootnotesContent"/>
        /// instance to copy
        /// </param>
        public PageFootnotesContent(iText.Layout.Properties.Margins.PageFootnotesContent other)
            : base(other) {
            this.pageNumber = other.pageNumber;
        }

        /// <summary>
        /// Sets page number for this
        /// <see cref="PageFootnotesContent"/>.
        /// </summary>
        /// <remarks>
        /// Sets page number for this
        /// <see cref="PageFootnotesContent"/>
        /// . Treated automatically during layout.
        /// </remarks>
        /// <param name="pageNumber">page number to set</param>
        /// <returns>
        /// this same
        /// <see cref="PageFootnotesContent"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.PageFootnotesContent SetPageNumber(int pageNumber) {
            this.pageNumber = pageNumber;
            return this;
        }

        /// <summary>
        /// Gets page number for this
        /// <see cref="PageFootnotesContent"/>.
        /// </summary>
        /// <returns>
        /// page number for this
        /// <see cref="PageFootnotesContent"/>
        /// </returns>
        public virtual int GetPageNumber() {
            return pageNumber;
        }
    }
//\endcond
}
