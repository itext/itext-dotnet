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
