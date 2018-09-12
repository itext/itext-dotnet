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
                            this.nthB = Convert.ToInt32(arguments);
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
                                    this.nthA = Convert.ToInt32(aParticle);
                                }
                            }
                            String bParticle = arguments.Substring(indexOfN + 1).Trim();
                            if (!String.IsNullOrEmpty(bParticle)) {
                                this.nthB = Convert.ToInt32(bParticle[0] + bParticle.Substring(1).Trim());
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
