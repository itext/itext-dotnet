/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Base structural evaluator.</summary>
    internal abstract class StructuralEvaluator : Evaluator {
        internal Evaluator evaluator;

        internal class Root : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return root == element;
            }
        }

        internal class Has : StructuralEvaluator {
            public Has(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element e in element.GetAllElements()) {
                    if (e != element && evaluator.Matches(root, e)) {
                        return true;
                    }
                }
                return false;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":has({0})", evaluator);
            }
        }

        internal class Not : StructuralEvaluator {
            public Not(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 node) {
                return !evaluator.Matches(root, node);
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":not{0}", evaluator);
            }
        }

        internal class Parent : StructuralEvaluator {
            public Parent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                if (root == element) {
                    return false;
                }
                iText.StyledXmlParser.Jsoup.Nodes.Element parent = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent
                    ();
                while (true) {
                    if (evaluator.Matches(root, parent)) {
                        return true;
                    }
                    if (parent == root) {
                        break;
                    }
                    parent = (iText.StyledXmlParser.Jsoup.Nodes.Element)parent.Parent();
                }
                return false;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":parent{0}", evaluator);
            }
        }

        internal class ImmediateParent : StructuralEvaluator {
            public ImmediateParent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                if (root == element) {
                    return false;
                }
                iText.StyledXmlParser.Jsoup.Nodes.Element parent = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent
                    ();
                return parent != null && evaluator.Matches(root, parent);
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":ImmediateParent{0}", evaluator);
            }
        }

        internal class PreviousSibling : StructuralEvaluator {
            public PreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                if (root == element) {
                    return false;
                }
                iText.StyledXmlParser.Jsoup.Nodes.Element prev = element.PreviousElementSibling();
                while (prev != null) {
                    if (evaluator.Matches(root, prev)) {
                        return true;
                    }
                    prev = prev.PreviousElementSibling();
                }
                return false;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":prev*{0}", evaluator);
            }
        }

        internal class ImmediatePreviousSibling : StructuralEvaluator {
            public ImmediatePreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                if (root == element) {
                    return false;
                }
                iText.StyledXmlParser.Jsoup.Nodes.Element prev = element.PreviousElementSibling();
                return prev != null && evaluator.Matches(root, prev);
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":prev{0}", evaluator);
            }
        }
    }
}
