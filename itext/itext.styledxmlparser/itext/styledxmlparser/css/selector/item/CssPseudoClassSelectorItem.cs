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
