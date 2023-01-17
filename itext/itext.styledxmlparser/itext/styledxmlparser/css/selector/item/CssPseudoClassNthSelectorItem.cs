/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassNthSelectorItem : CssPseudoClassChildSelectorItem {
        /// <summary>The nth A.</summary>
        private int nthA;

        /// <summary>The nth B.</summary>
        private int nthB;

        internal CssPseudoClassNthSelectorItem(String pseudoClass, String arguments)
            : base(pseudoClass, arguments) {
            GetNthArguments();
        }

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
}
