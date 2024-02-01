/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Selector;

namespace iText.StyledXmlParser.Css {
    /// <summary>Comparator class used to sort CSS rule set objects.</summary>
    public class CssRuleSetComparator : IComparer<CssRuleSet> {
        /// <summary>The selector comparator.</summary>
        private CssSelectorComparator selectorComparator = new CssSelectorComparator();

        /* (non-Javadoc)
        * @see java.util.Comparator#compare(java.lang.Object, java.lang.Object)
        */
        public virtual int Compare(CssRuleSet o1, CssRuleSet o2) {
            return selectorComparator.Compare(o1.GetSelector(), o2.GetSelector());
        }
    }
}
