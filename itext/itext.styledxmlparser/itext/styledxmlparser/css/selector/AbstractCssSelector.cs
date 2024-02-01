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
using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector {
    /// <summary>Abstract superclass for CSS Selectors.</summary>
    public abstract class AbstractCssSelector : ICssSelector {
        /// <summary>The selector items.</summary>
        protected internal IList<ICssSelectorItem> selectorItems;

        /// <summary>
        /// Creates a new
        /// <see cref="AbstractCssSelector"/>
        /// instance.
        /// </summary>
        /// <param name="selectorItems">the selector items</param>
        public AbstractCssSelector(IList<ICssSelectorItem> selectorItems) {
            this.selectorItems = selectorItems;
        }

        public virtual IList<ICssSelectorItem> GetSelectorItems() {
            return JavaCollectionsUtil.UnmodifiableList(selectorItems);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.ICssSelector#calculateSpecificity()
        */
        public virtual int CalculateSpecificity() {
            int specificity = 0;
            foreach (ICssSelectorItem item in selectorItems) {
                specificity += item.GetSpecificity();
            }
            return specificity;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (ICssSelectorItem item in selectorItems) {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }

        public abstract bool Matches(INode arg1);
    }
}
