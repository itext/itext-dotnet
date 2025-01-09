/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for separator selectors.
    /// </summary>
    public class CssSeparatorSelectorItem : ICssSelectorItem {
        /// <summary>The separator character.</summary>
        private char separator;

        /// <summary>
        /// Creates a new
        /// <see cref="CssSeparatorSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="separator">the separator character</param>
        public CssSeparatorSelectorItem(char separator) {
            this.separator = separator;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#getSpecificity()
        */
        public virtual int GetSpecificity() {
            return 0;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            throw new InvalidOperationException("Separator item is not supposed to be matched against an element");
        }

        /// <summary>Gets the separator character.</summary>
        /// <returns>the separator character</returns>
        public virtual char GetSeparator() {
            return separator;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            return separator == ' ' ? " " : MessageFormatUtil.Format(" {0} ", separator);
        }
    }
}
