/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iText.Kernel.Utils {
    /// <summary>
    /// Class representing a page range, for instance a page range can contain
    /// pages 5, then pages 10 through 15, then page 18, then page 21 and so on.
    /// </summary>
    public class PageRange {
        private IList<int> sequenceStarts = new List<int>();

        private IList<int> sequenceEnds = new List<int>();

        /// <summary>
        /// Constructs an empty
        /// <see cref="PageRange"/>
        /// instance.
        /// </summary>
        public PageRange() {
        }

        /// <summary>
        /// Constructs a
        /// <see cref="PageRange"/>
        /// instance from a range in a string form, for example: "1-12, 15, 45-66".
        /// </summary>
        /// <param name="pageRange">the page range</param>
        public PageRange(String pageRange) {
            pageRange = iText.IO.Util.StringUtil.ReplaceAll(pageRange, "\\s+", "");
            Regex sequencePattern = iText.IO.Util.StringUtil.RegexCompile("(\\d+)-(\\d+)");
            Regex singlePagePattern = iText.IO.Util.StringUtil.RegexCompile("(\\d+)");
            foreach (String pageRangePart in iText.IO.Util.StringUtil.Split(pageRange, ",")) {
                Match matcher;
                if ((matcher = iText.IO.Util.StringUtil.Match(sequencePattern, pageRangePart)).Success) {
                    sequenceStarts.Add(System.Convert.ToInt32(iText.IO.Util.StringUtil.Group(matcher, 1)));
                    sequenceEnds.Add(System.Convert.ToInt32(iText.IO.Util.StringUtil.Group(matcher, 2)));
                }
                else {
                    if ((matcher = iText.IO.Util.StringUtil.Match(singlePagePattern, pageRangePart)).Success) {
                        int pageNumber = System.Convert.ToInt32(iText.IO.Util.StringUtil.Group(matcher, 1));
                        sequenceStarts.Add(pageNumber);
                        sequenceEnds.Add(pageNumber);
                    }
                }
            }
        }

        /// <summary>Adds a page sequence to the range.</summary>
        /// <param name="startPageNumber">the starting page number of the sequence</param>
        /// <param name="endPageNumber">the finishing page number of the sequnce</param>
        /// <returns>this range, already modified</returns>
        public virtual iText.Kernel.Utils.PageRange AddPageSequence(int startPageNumber, int endPageNumber) {
            sequenceStarts.Add(startPageNumber);
            sequenceEnds.Add(endPageNumber);
            return this;
        }

        /// <summary>Adds a single page to the range.</summary>
        /// <param name="pageNumber">the page number to add</param>
        /// <returns>this range, already modified</returns>
        public virtual iText.Kernel.Utils.PageRange AddSinglePage(int pageNumber) {
            sequenceStarts.Add(pageNumber);
            sequenceEnds.Add(pageNumber);
            return this;
        }

        /// <summary>Gets the list if pages that have been added to the range so far.</summary>
        /// <returns>the list containing page numbers added to the range</returns>
        public virtual IList<int> GetAllPages() {
            IList<int> allPages = new List<int>();
            for (int ind = 0; ind < sequenceStarts.Count; ind++) {
                for (int pageInRange = (int)sequenceStarts[ind]; pageInRange <= sequenceEnds[ind]; pageInRange++) {
                    allPages.Add(pageInRange);
                }
            }
            return allPages;
        }

        /// <summary>Checks if a given page is present in the range built so far.</summary>
        /// <param name="pageNumber">the page number to check</param>
        /// <returns><code>true</code> if the page is present in this range, <code>false</code> otherwise</returns>
        public virtual bool IsPageInRange(int pageNumber) {
            for (int ind = 0; ind < sequenceStarts.Count; ind++) {
                if (sequenceStarts[ind] <= pageNumber && pageNumber <= sequenceEnds[ind]) {
                    return true;
                }
            }
            return false;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object obj) {
            if (!(obj is iText.Kernel.Utils.PageRange)) {
                return false;
            }
            iText.Kernel.Utils.PageRange other = (iText.Kernel.Utils.PageRange)obj;
            return sequenceStarts.Equals(other.sequenceStarts) && sequenceEnds.Equals(other.sequenceEnds);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return sequenceStarts.GetHashCode() * 31 + sequenceEnds.GetHashCode();
        }
    }
}
