/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Token queue tests.</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class TokenQueueTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ChompBalanced() {
            TokenQueue tq = new TokenQueue(":contains(one (two) three) four");
            String pre = tq.ConsumeTo("(");
            String guts = tq.ChompBalanced('(', ')');
            String remainder = tq.Remainder();
            NUnit.Framework.Assert.AreEqual(":contains", pre);
            NUnit.Framework.Assert.AreEqual("one (two) three", guts);
            NUnit.Framework.Assert.AreEqual(" four", remainder);
        }

        [NUnit.Framework.Test]
        public virtual void ChompEscapedBalanced() {
            TokenQueue tq = new TokenQueue(":contains(one (two) \\( \\) \\) three) four");
            String pre = tq.ConsumeTo("(");
            String guts = tq.ChompBalanced('(', ')');
            String remainder = tq.Remainder();
            NUnit.Framework.Assert.AreEqual(":contains", pre);
            NUnit.Framework.Assert.AreEqual("one (two) \\( \\) \\) three", guts);
            NUnit.Framework.Assert.AreEqual("one (two) ( ) ) three", TokenQueue.Unescape(guts));
            NUnit.Framework.Assert.AreEqual(" four", remainder);
        }

        [NUnit.Framework.Test]
        public virtual void ChompBalancedMatchesAsMuchAsPossible() {
            TokenQueue tq = new TokenQueue("unbalanced(something(or another)) else");
            tq.ConsumeTo("(");
            String match = tq.ChompBalanced('(', ')');
            NUnit.Framework.Assert.AreEqual("something(or another)", match);
        }

        [NUnit.Framework.Test]
        public virtual void Unescape() {
            NUnit.Framework.Assert.AreEqual("one ( ) \\", TokenQueue.Unescape("one \\( \\) \\\\"));
        }

        [NUnit.Framework.Test]
        public virtual void ChompToIgnoreCase() {
            String t = "<textarea>one < two </TEXTarea>";
            TokenQueue tq = new TokenQueue(t);
            String data = tq.ChompToIgnoreCase("</textarea");
            NUnit.Framework.Assert.AreEqual("<textarea>one < two ", data);
            tq = new TokenQueue("<textarea> one two < three </oops>");
            data = tq.ChompToIgnoreCase("</textarea");
            NUnit.Framework.Assert.AreEqual("<textarea> one two < three </oops>", data);
        }

        [NUnit.Framework.Test]
        public virtual void AddFirst() {
            TokenQueue tq = new TokenQueue("One Two");
            tq.ConsumeWord();
            tq.AddFirst("Three");
            NUnit.Framework.Assert.AreEqual("Three Two", tq.Remainder());
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeToIgnoreSecondCallTest() {
            String t = "<textarea>one < two </TEXTarea> third </TEXTarea>";
            TokenQueue tq = new TokenQueue(t);
            String data = tq.ChompToIgnoreCase("</textarea>");
            NUnit.Framework.Assert.AreEqual("<textarea>one < two ", data);
            data = tq.ChompToIgnoreCase("</textarea>");
            NUnit.Framework.Assert.AreEqual(" third ", data);
        }

        [NUnit.Framework.Test]
        public virtual void TestNestedQuotes() {
            ValidateNestedQuotes("<html><body><a id=\"identifier\" onclick=\"func('arg')\" /></body></html>", "a[onclick*=\"('arg\"]"
                );
            ValidateNestedQuotes("<html><body><a id=\"identifier\" onclick=func('arg') /></body></html>", "a[onclick*=\"('arg\"]"
                );
            ValidateNestedQuotes("<html><body><a id=\"identifier\" onclick='func(\"arg\")' /></body></html>", "a[onclick*='(\"arg']"
                );
            ValidateNestedQuotes("<html><body><a id=\"identifier\" onclick=func(\"arg\") /></body></html>", "a[onclick*='(\"arg']"
                );
        }

        private static void ValidateNestedQuotes(String html, String selector) {
            NUnit.Framework.Assert.AreEqual("#identifier", iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select(selector
                ).First().CssSelector());
        }

        [NUnit.Framework.Test]
        public virtual void ChompBalancedThrowIllegalArgumentException() {
            try {
                TokenQueue tq = new TokenQueue("unbalanced(something(or another)) else");
                tq.ConsumeTo("(");
                tq.ChompBalanced('(', '+');
                NUnit.Framework.Assert.Fail("should have thrown IllegalArgumentException");
            }
            catch (ArgumentException expected) {
                NUnit.Framework.Assert.AreEqual("Did not find balanced marker at 'something(or another)) else'", expected.
                    Message);
            }
        }
    }
}
