/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Pseudo;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector {
    /// <summary>
    /// <see cref="ICssSelector"/>
    /// implementation for CSS selectors.
    /// </summary>
    public class CssSelector : AbstractCssSelector {
        /// <summary>
        /// Creates a new
        /// <see cref="CssSelector"/>
        /// instance.
        /// </summary>
        /// <param name="selectorItems">the selector items</param>
        public CssSelector(IList<ICssSelectorItem> selectorItems)
            : base(selectorItems) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssSelector"/>
        /// instance.
        /// </summary>
        /// <param name="selector">the selector</param>
        public CssSelector(String selector)
            : this(CssSelectorParser.ParseSelectorItems(selector)) {
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.ICssSelector#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public override bool Matches(INode element) {
            return Matches(element, selectorItems.Count - 1);
        }

        /// <summary>Checks if a node matches the selector.</summary>
        /// <param name="element">the node</param>
        /// <param name="lastSelectorItemInd">the index of the last selector</param>
        /// <returns>true, if there's a match</returns>
        private bool Matches(INode element, int lastSelectorItemInd) {
            if (!(element is IElementNode)) {
                return false;
            }
            if (lastSelectorItemInd < 0) {
                return true;
            }
            bool isPseudoElement = element is CssPseudoElementNode;
            for (int i = lastSelectorItemInd; i >= 0; i--) {
                if (isPseudoElement && selectorItems[lastSelectorItemInd] is CssPseudoElementSelectorItem && i < lastSelectorItemInd
                    ) {
                    // Pseudo element selector item shall be at the end of the selector string
                    // and be single pseudo element selector item in it. All other selector items are checked against
                    // pseudo element node parent.
                    element = element.ParentNode();
                    isPseudoElement = false;
                }
                ICssSelectorItem currentItem = selectorItems[i];
                if (currentItem is CssSeparatorSelectorItem) {
                    char separator = ((CssSeparatorSelectorItem)currentItem).GetSeparator();
                    switch (separator) {
                        case '>': {
                            return Matches(element.ParentNode(), i - 1);
                        }

                        case ' ': {
                            INode parent = element.ParentNode();
                            while (parent != null) {
                                bool parentMatches = Matches(parent, i - 1);
                                if (parentMatches) {
                                    return true;
                                }
                                else {
                                    parent = parent.ParentNode();
                                }
                            }
                            return false;
                        }

                        case '~': {
                            INode parent = element.ParentNode();
                            if (parent != null) {
                                int indexOfElement = parent.ChildNodes().IndexOf(element);
                                for (int j = indexOfElement - 1; j >= 0; j--) {
                                    if (Matches(parent.ChildNodes()[j], i - 1)) {
                                        return true;
                                    }
                                }
                            }
                            return false;
                        }

                        case '+': {
                            INode parent = element.ParentNode();
                            if (parent != null) {
                                int indexOfElement = parent.ChildNodes().IndexOf(element);
                                INode previousElement = null;
                                for (int j = indexOfElement - 1; j >= 0; j--) {
                                    if (parent.ChildNodes()[j] is IElementNode) {
                                        previousElement = parent.ChildNodes()[j];
                                        break;
                                    }
                                }
                                if (previousElement != null) {
                                    return indexOfElement > 0 && Matches(previousElement, i - 1);
                                }
                            }
                            return false;
                        }

                        default: {
                            return false;
                        }
                    }
                }
                else {
                    if (!currentItem.Matches(element)) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
