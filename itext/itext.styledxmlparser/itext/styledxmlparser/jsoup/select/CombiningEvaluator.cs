/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Jsoup.Select {
//\cond DO_NOT_DOCUMENT
    /// <summary>Base combining (and, or) evaluator.</summary>
    internal abstract class CombiningEvaluator : Evaluator {
//\cond DO_NOT_DOCUMENT
        internal readonly List<Evaluator> evaluators;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int num = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal CombiningEvaluator()
            : base() {
            evaluators = new List<Evaluator>();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal CombiningEvaluator(ICollection<Evaluator> evaluators)
            : this() {
            this.evaluators.AddAll(evaluators);
            UpdateNumEvaluators();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Evaluator RightMostEvaluator() {
            return num > 0 ? evaluators[num - 1] : null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ReplaceRightMostEvaluator(Evaluator replacement) {
            evaluators[num - 1] = replacement;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void UpdateNumEvaluators() {
            // used so we don't need to bash on size() for every match test
            num = evaluators.Count;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class And : CombiningEvaluator {
//\cond DO_NOT_DOCUMENT
            internal And(ICollection<Evaluator> evaluators)
                : base(evaluators) {
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal And(params Evaluator[] evaluators)
                : this(JavaUtil.ArraysAsList(evaluators)) {
            }
//\endcond

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 node) {
                for (int i = num - 1; i >= 0; i--) {
                    // process backwards so that :matchText is evaled earlier, to catch parent query.
                    Evaluator s = evaluators[i];
                    if (!s.Matches(root, node)) {
                        return false;
                    }
                }
                return true;
            }

            public override String ToString() {
                return iText.StyledXmlParser.Jsoup.Internal.StringUtil.Join(evaluators, "");
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class OR : CombiningEvaluator {
//\cond DO_NOT_DOCUMENT
            /// <summary>Create a new Or evaluator.</summary>
            /// <remarks>Create a new Or evaluator. The initial evaluators are ANDed together and used as the first clause of the OR.
            ///     </remarks>
            /// <param name="evaluators">initial OR clause (these are wrapped into an AND evaluator).</param>
            internal OR(ICollection<Evaluator> evaluators)
                : base() {
                if (num > 1) {
                    this.evaluators.Add(new CombiningEvaluator.And(evaluators));
                }
                else {
                    // 0 or 1
                    this.evaluators.AddAll(evaluators);
                }
                UpdateNumEvaluators();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal OR(params Evaluator[] evaluators)
                : this(JavaUtil.ArraysAsList(evaluators)) {
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal OR()
                : base() {
            }
//\endcond

            public void Add(Evaluator e) {
                evaluators.Add(e);
                UpdateNumEvaluators();
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 node) {
                for (int i = 0; i < num; i++) {
                    Evaluator s = evaluators[i];
                    if (s.Matches(root, node)) {
                        return true;
                    }
                }
                return false;
            }

            public override String ToString() {
                return iText.StyledXmlParser.Jsoup.Internal.StringUtil.Join(evaluators, ", ");
            }
        }
//\endcond
    }
//\endcond
}
