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
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    internal class CssPseudoClassNthSelectorItem : CssPseudoClassChildSelectorItem {
        /// <summary>The nth A.</summary>
        private int nthA;

        /// <summary>The nth B.</summary>
        private int nthB;

//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassNthSelectorItem(String pseudoClass, String arguments)
            : base(pseudoClass, arguments) {
            GetNthArguments();
        }
//\endcond

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            IList<INode> children = GetAllSiblings(node);
            return !children.IsEmpty() && ResolveNth(node, children);
        }

        /// <summary>Gets the nth arguments.</summary>
        protected internal virtual void GetNthArguments() {
            if (arguments.Matches("((-|\\+)?[0-9]*n(\\s*(-|\\+)\\s*[0-9]+)?|(-|\\+)?[0-9]+|odd|even)")) {
                if (arguments.Equals("odd")) {
                    this.nthA = 2;
                    this.nthB = 1;
                }
                else {
                    if (arguments.Equals("even")) {
                        this.nthA = 2;
                        this.nthB = 0;
                    }
                    else {
                        int indexOfN = arguments.IndexOf('n');
                        if (indexOfN == -1) {
                            this.nthA = 0;
                            this.nthB = Convert.ToInt32(arguments, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else {
                            String aParticle = arguments.JSubstring(0, indexOfN).Trim();
                            if (String.IsNullOrEmpty(aParticle)) {
                                this.nthA = 0;
                            }
                            else {
                                if (aParticle.Length == 1 && !char.IsDigit(aParticle[0])) {
                                    this.nthA = aParticle.Equals("+") ? 1 : -1;
                                }
                                else {
                                    this.nthA = Convert.ToInt32(aParticle, System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                            String bParticle = arguments.Substring(indexOfN + 1).Trim();
                            if (!String.IsNullOrEmpty(bParticle)) {
                                this.nthB = Convert.ToInt32(bParticle[0] + bParticle.Substring(1).Trim(), System.Globalization.CultureInfo.InvariantCulture
                                    );
                            }
                            else {
                                this.nthB = 0;
                            }
                        }
                    }
                }
            }
            else {
                this.nthA = 0;
                this.nthB = 0;
            }
        }

        /// <summary>Resolves the nth.</summary>
        /// <param name="node">a node</param>
        /// <param name="children">the children</param>
        /// <returns>true, if successful</returns>
        protected internal virtual bool ResolveNth(INode node, IList<INode> children) {
            if (!children.Contains(node)) {
                return false;
            }
            if (this.nthA > 0) {
                int temp = children.IndexOf(node) + 1 - this.nthB;
                return temp >= 0 && temp % this.nthA == 0;
            }
            else {
                if (this.nthA < 0) {
                    int temp = children.IndexOf(node) + 1 - this.nthB;
                    return temp <= 0 && temp % this.nthA == 0;
                }
                else {
                    return (children.IndexOf(node) + 1) - this.nthB == 0;
                }
            }
        }
    }
//\endcond
}
