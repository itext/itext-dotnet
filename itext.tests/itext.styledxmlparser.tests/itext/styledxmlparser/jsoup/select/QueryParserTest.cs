/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests for the Selector Query Parser.</summary>
    /// <author>Jonathan Hedley</author>
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
                NUnit.Framework.Assert.IsTrue(and.evaluators[0] is Evaluator.Tag);
                NUnit.Framework.Assert.IsTrue(and.evaluators[1] is StructuralEvaluator.Parent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestParsesMultiCorrectly() {
            Evaluator eval = QueryParser.Parse(".foo > ol, ol > li + li");
            NUnit.Framework.Assert.IsTrue(eval is CombiningEvaluator.OR);
            CombiningEvaluator.OR or = (CombiningEvaluator.OR)eval;
            NUnit.Framework.Assert.AreEqual(2, or.evaluators.Count);
            CombiningEvaluator.And andLeft = (CombiningEvaluator.And)or.evaluators[0];
            CombiningEvaluator.And andRight = (CombiningEvaluator.And)or.evaluators[1];
            NUnit.Framework.Assert.AreEqual("ol :ImmediateParent.foo", andLeft.ToString());
            NUnit.Framework.Assert.AreEqual(2, andLeft.evaluators.Count);
            NUnit.Framework.Assert.AreEqual("li :prevli :ImmediateParentol", andRight.ToString());
            NUnit.Framework.Assert.AreEqual(2, andLeft.evaluators.Count);
        }
    }
}
