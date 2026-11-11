/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Jsoup.Select {
//\cond DO_NOT_DOCUMENT
    /// <summary>Base structural evaluator.</summary>
    internal abstract class StructuralEvaluator : Evaluator {
//\cond DO_NOT_DOCUMENT
        internal Evaluator evaluator;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class Root : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return root == element;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond
    }
//\endcond
}
