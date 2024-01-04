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
namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Collects a list of elements that match the supplied criteria.</summary>
    public class Collector {
        private Collector() {
        }

        /// <summary>Build a list of elements, by visiting root and every descendant of root, and testing it against the evaluator.
        ///     </summary>
        /// <param name="eval">Evaluator to test elements against</param>
        /// <param name="root">root of tree to descend</param>
        /// <returns>list of matches; empty if none</returns>
        public static Elements Collect(Evaluator eval, iText.StyledXmlParser.Jsoup.Nodes.Element root) {
            Elements elements = new Elements();
            NodeTraversor.Traverse(new Collector.Accumulator(root, elements, eval), root);
            return elements;
        }

        private class Accumulator : NodeVisitor {
            private readonly iText.StyledXmlParser.Jsoup.Nodes.Element root;

            private readonly Elements elements;

            private readonly Evaluator eval;

            internal Accumulator(iText.StyledXmlParser.Jsoup.Nodes.Element root, Elements elements, Evaluator eval) {
                this.root = root;
                this.elements = elements;
                this.eval = eval;
            }

            public virtual void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element el = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                    if (eval.Matches(root, el)) {
                        elements.Add(el);
                    }
                }
            }

            public virtual void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
            }
            // void
        }

        /// <summary>
        /// Finds the first Element that matches the Evaluator that descends from the root, and stops the query once that first
        /// match is found.
        /// </summary>
        /// <param name="eval">Evaluator to test elements against</param>
        /// <param name="root">root of tree to descend</param>
        /// <returns>
        /// the first match;
        /// <see langword="null"/>
        /// if none
        /// </returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.Element FindFirst(Evaluator eval, iText.StyledXmlParser.Jsoup.Nodes.Element
             root) {
            Collector.FirstFinder finder = new Collector.FirstFinder(root, eval);
            NodeTraversor.Filter(finder, root);
            return finder.match;
        }

        private class FirstFinder : NodeFilter {
            internal iText.StyledXmlParser.Jsoup.Nodes.Element match = null;

            private readonly iText.StyledXmlParser.Jsoup.Nodes.Element root;

            private readonly Evaluator eval;

            internal FirstFinder(iText.StyledXmlParser.Jsoup.Nodes.Element root, Evaluator eval) {
                this.root = root;
                this.eval = eval;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element el = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                    if (eval.Matches(root, el)) {
                        match = el;
                        return NodeFilter.FilterResult.STOP;
                    }
                }
                return NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return NodeFilter.FilterResult.CONTINUE;
            }
        }
    }
}
