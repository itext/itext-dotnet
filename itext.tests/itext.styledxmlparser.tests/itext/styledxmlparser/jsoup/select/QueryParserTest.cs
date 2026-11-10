/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests for the Selector Query Parser.</summary>
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
