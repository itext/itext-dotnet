/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests for the Selector Query Parser.</summary>
    /// <author>Jonathan Hedley</author>
    [NUnit.Framework.Category("UnitTest")]
    public class QueryParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestOrGetsCorrectPrecedence() {
            // tests that a selector "a b, c d, e f" evals to (a AND b) OR (c AND d) OR (e AND f)"
            // top level or, three child ands
            Evaluator eval = QueryParser.Parse("a b, c d, e f");
            NUnit.Framework.Assert.IsTrue(eval is CombiningEvaluator.OR);
            CombiningEvaluator.OR or = (CombiningEvaluator.OR)eval;
            NUnit.Framework.Assert.AreEqual(3, or.evaluators.Count);
            foreach (Evaluator innerEval in or.evaluators) {
                NUnit.Framework.Assert.IsTrue(innerEval is CombiningEvaluator.And);
                CombiningEvaluator.And and = (CombiningEvaluator.And)innerEval;
                NUnit.Framework.Assert.AreEqual(2, and.evaluators.Count);
                NUnit.Framework.Assert.IsTrue(and.evaluators[0] is StructuralEvaluator.Parent);
                NUnit.Framework.Assert.IsTrue(and.evaluators[1] is Evaluator.Tag);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestParsesMultiCorrectly() {
            String query = ".foo > ol, ol > li + li";
            Evaluator eval = QueryParser.Parse(query);
            NUnit.Framework.Assert.IsTrue(eval is CombiningEvaluator.OR);
            CombiningEvaluator.OR or = (CombiningEvaluator.OR)eval;
            NUnit.Framework.Assert.AreEqual(2, or.evaluators.Count);
            CombiningEvaluator.And andLeft = (CombiningEvaluator.And)or.evaluators[0];
            CombiningEvaluator.And andRight = (CombiningEvaluator.And)or.evaluators[1];
            NUnit.Framework.Assert.AreEqual(".foo > ol", andLeft.ToString());
            NUnit.Framework.Assert.AreEqual(2, andLeft.evaluators.Count);
            NUnit.Framework.Assert.AreEqual("ol > li + li", andRight.ToString());
            NUnit.Framework.Assert.AreEqual(2, andRight.evaluators.Count);
            NUnit.Framework.Assert.AreEqual(query, eval.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ExceptionOnUncloseAttribute() {
            NUnit.Framework.Assert.Catch(typeof(Selector.SelectorParseException), () => QueryParser.Parse("section > a[href=\"]"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestParsesSingleQuoteInContains() {
            NUnit.Framework.Assert.Catch(typeof(Selector.SelectorParseException), () => QueryParser.Parse("p:contains(One \" One)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ExceptOnEmptySelector() {
            NUnit.Framework.Assert.Catch(typeof(Selector.SelectorParseException), () => QueryParser.Parse(""));
        }

        [NUnit.Framework.Test]
        public virtual void ExceptOnNullSelector() {
            NUnit.Framework.Assert.Catch(typeof(Selector.SelectorParseException), () => QueryParser.Parse(null));
        }

        [NUnit.Framework.Test]
        public virtual void OkOnSpacesForeAndAft() {
            Evaluator parse = QueryParser.Parse(" span div  ");
            NUnit.Framework.Assert.AreEqual("span div", parse.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void StructuralEvaluatorsToString() {
            String q = "a:not(:has(span.foo)) b d > e + f ~ g";
            Evaluator parse = QueryParser.Parse(q);
            NUnit.Framework.Assert.AreEqual(q, parse.ToString());
        }
    }
}
