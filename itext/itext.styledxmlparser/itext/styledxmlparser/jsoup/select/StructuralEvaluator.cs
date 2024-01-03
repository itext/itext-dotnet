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
using iText.Commons.Utils;

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
                    if (e != element && evaluator.Matches(element, e)) {
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
                return MessageFormatUtil.Format(":not({0})", evaluator);
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
                while (parent != null) {
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
                return MessageFormatUtil.Format("{0} ", evaluator);
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
                return MessageFormatUtil.Format("{0} > ", evaluator);
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
                return MessageFormatUtil.Format("{0} ~ ", evaluator);
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
                return MessageFormatUtil.Format("{0} + ", evaluator);
            }
        }
    }
}
