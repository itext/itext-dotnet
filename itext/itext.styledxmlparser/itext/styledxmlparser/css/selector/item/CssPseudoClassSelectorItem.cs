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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for pseudo class selectors.
    /// </summary>
    public abstract class CssPseudoClassSelectorItem : ICssSelectorItem {
        /// <summary>The arguments.</summary>
        protected internal String arguments;

        /// <summary>The pseudo class.</summary>
        private String pseudoClass;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPseudoClassSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="pseudoClass">the pseudo class name</param>
        protected internal CssPseudoClassSelectorItem(String pseudoClass)
            : this(pseudoClass, "") {
        }

        protected internal CssPseudoClassSelectorItem(String pseudoClass, String arguments) {
            this.pseudoClass = pseudoClass;
            this.arguments = arguments;
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassSelectorItem Create(String fullSelectorString
            ) {
            int indexOfParentheses = fullSelectorString.IndexOf('(');
            String pseudoClass;
            String arguments;
            if (indexOfParentheses == -1) {
                pseudoClass = fullSelectorString;
                arguments = "";
            }
            else {
                pseudoClass = fullSelectorString.JSubstring(0, indexOfParentheses);
                arguments = fullSelectorString.JSubstring(indexOfParentheses + 1, fullSelectorString.Length - 1).Trim();
            }
            return Create(pseudoClass, arguments);
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassSelectorItem Create(String pseudoClass
            , String arguments) {
            switch (pseudoClass) {
                case CommonCssConstants.EMPTY: {
                    return CssPseudoClassEmptySelectorItem.GetInstance();
                }

                case CommonCssConstants.FIRST_CHILD: {
                    return CssPseudoClassFirstChildSelectorItem.GetInstance();
                }

                case CommonCssConstants.FIRST_OF_TYPE: {
                    return CssPseudoClassFirstOfTypeSelectorItem.GetInstance();
                }

                case CommonCssConstants.LAST_CHILD: {
                    return CssPseudoClassLastChildSelectorItem.GetInstance();
                }

                case CommonCssConstants.LAST_OF_TYPE: {
                    return CssPseudoClassLastOfTypeSelectorItem.GetInstance();
                }

                case CommonCssConstants.NTH_CHILD: {
                    return new CssPseudoClassNthChildSelectorItem(arguments);
                }

                case CommonCssConstants.NTH_OF_TYPE: {
                    return new CssPseudoClassNthOfTypeSelectorItem(arguments);
                }

                case CommonCssConstants.NOT: {
                    CssSelector selector = new CssSelector(arguments);
                    foreach (ICssSelectorItem item in selector.GetSelectorItems()) {
                        if (item is CssPseudoClassNotSelectorItem || item is CssPseudoElementSelectorItem) {
                            return null;
                        }
                    }
                    return new CssPseudoClassNotSelectorItem(selector);
                }

                case CommonCssConstants.ROOT: {
                    return CssPseudoClassRootSelectorItem.GetInstance();
                }

                case CommonCssConstants.LINK: {
                    return new CssPseudoClassSelectorItem.AlwaysApplySelectorItem(pseudoClass, arguments);
                }

                case CommonCssConstants.ACTIVE:
                case CommonCssConstants.FOCUS:
                case CommonCssConstants.HOVER:
                case CommonCssConstants.TARGET:
                case CommonCssConstants.VISITED: {
                    return new CssPseudoClassSelectorItem.AlwaysNotApplySelectorItem(pseudoClass, arguments);
                }

                case CommonCssConstants.DISABLED: {
                    return CssPseudoClassDisabledSelectorItem.GetInstance();
                }

                default: {
                    //Still unsupported, should be addressed in DEVSIX-1440
                    //case CommonCssConstants.CHECKED:
                    //case CommonCssConstants.ENABLED:
                    //case CommonCssConstants.IN_RANGE:
                    //case CommonCssConstants.INVALID:
                    //case CommonCssConstants.LANG:
                    //case CommonCssConstants.NTH_LAST_CHILD:
                    //case CommonCssConstants.NTH_LAST_OF_TYPE:
                    //case CommonCssConstants.ONLY_OF_TYPE:
                    //case CommonCssConstants.ONLY_CHILD:
                    //case CommonCssConstants.OPTIONAL:
                    //case CommonCssConstants.OUT_OF_RANGE:
                    //case CommonCssConstants.READ_ONLY:
                    //case CommonCssConstants.READ_WRITE:
                    //case CommonCssConstants.REQUIRED:
                    //case CommonCssConstants.VALID:
                    return null;
                }
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#getSpecificity()
        */
        public virtual int GetSpecificity() {
            return CssSpecificityConstants.CLASS_SPECIFICITY;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            return false;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            return ":" + pseudoClass + (!String.IsNullOrEmpty(arguments) ? "(" + arguments + ")" : "");
        }

        public virtual String GetPseudoClass() {
            return pseudoClass;
        }

        private class AlwaysApplySelectorItem : CssPseudoClassSelectorItem {
            internal AlwaysApplySelectorItem(String pseudoClass, String arguments)
                : base(pseudoClass, arguments) {
            }

            public override bool Matches(INode node) {
                return true;
            }
        }

        private class AlwaysNotApplySelectorItem : CssPseudoClassSelectorItem {
            internal AlwaysNotApplySelectorItem(String pseudoClass, String arguments)
                : base(pseudoClass, arguments) {
            }

            public override bool Matches(INode node) {
                return false;
            }
        }
    }
}
