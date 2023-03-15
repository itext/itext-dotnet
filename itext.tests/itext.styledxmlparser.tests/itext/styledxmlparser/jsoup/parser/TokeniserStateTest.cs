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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class TokeniserStateTest : ExtendedITextTest {
        internal readonly char[] whiteSpace = new char[] { '\t', '\n', '\r', '\f', ' ' };

        internal readonly char[] quote = new char[] { '\'', '"' };

        [NUnit.Framework.Test]
        public virtual void EnsureSearchArraysAreSorted() {
            char[][] arrays = new char[][] { TokeniserState.attributeNameCharsSorted, TokeniserState.attributeValueUnquoted
                 };
            foreach (char[] array in arrays) {
                char[] copy = JavaUtil.ArraysCopyOf(array, array.Length);
                JavaUtil.Sort(array);
                NUnit.Framework.Assert.AreEqual(array, copy);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCharacterReferenceInRcdata() {
            String body = "<textarea>You&I</textarea>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            Elements els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("You&I", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestBeforeTagName() {
            foreach (char c in whiteSpace) {
                String body = MessageFormatUtil.Format("<div{0}>test</div>", c);
                Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
                Elements els = doc.Select("div");
                NUnit.Framework.Assert.AreEqual("test", els.Text());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEndTagOpen() {
            String body;
            Document doc;
            Elements els;
            body = "<div>hello world</";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("hello world</", els.Text());
            body = "<div>hello world</div>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("hello world", els.Text());
            body = "<div>fake</></div>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("fake", els.Text());
            body = "<div>fake</?</div>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("fake", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestRcdataLessthanSign() {
            String body;
            Document doc;
            Elements els;
            body = "<textarea><fake></textarea>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("<fake>", els.Text());
            body = "<textarea><open";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("", els.Text());
            body = "<textarea>hello world</?fake</textarea>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("hello world</?fake", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestRCDATAEndTagName() {
            foreach (char c in whiteSpace) {
                String body = MessageFormatUtil.Format("<textarea>data</textarea{0}>", c);
                Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
                Elements els = doc.Select("textarea");
                NUnit.Framework.Assert.AreEqual("data", els.Text());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCommentEndCoverage() {
            String html = "<html><head></head><body><img src=foo><!-- <table><tr><td></table> --! --- --><p>Hello</p></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            Comment comment = (Comment)body.ChildNode(1);
            NUnit.Framework.Assert.AreEqual(" <table><tr><td></table> --! --- ", comment.GetData());
            iText.StyledXmlParser.Jsoup.Nodes.Element p = body.Child(1);
            TextNode text = (TextNode)p.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("Hello", text.GetWholeText());
        }

        [NUnit.Framework.Test]
        public virtual void TestCommentEndBangCoverage() {
            String html = "<html><head></head><body><img src=foo><!-- <table><tr><td></table> --!---!>--><p>Hello</p></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            Comment comment = (Comment)body.ChildNode(1);
            NUnit.Framework.Assert.AreEqual(" <table><tr><td></table> --!-", comment.GetData());
            iText.StyledXmlParser.Jsoup.Nodes.Element p = body.Child(1);
            TextNode text = (TextNode)p.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("Hello", text.GetWholeText());
        }

        [NUnit.Framework.Test]
        public virtual void TestPublicIdentifiersWithWhitespace() {
            String expectedOutput = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\">";
            foreach (char q in quote) {
                foreach (char ws in whiteSpace) {
                    String[] htmls = new String[] { MessageFormatUtil.Format("<!DOCTYPE html{0}PUBLIC {1}-//W3C//DTD HTML 4.0//EN{2}>"
                        , ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html {0}PUBLIC {1}-//W3C//DTD HTML 4.0//EN{2}>", ws, 
                        q, q), MessageFormatUtil.Format("<!DOCTYPE html PUBLIC{0}{1}-//W3C//DTD HTML 4.0//EN{2}>", ws, q, q), 
                        MessageFormatUtil.Format("<!DOCTYPE html PUBLIC {0}{1}-//W3C//DTD HTML 4.0//EN{2}>", ws, q, q), MessageFormatUtil
                        .Format("<!DOCTYPE html PUBLIC {0}-//W3C//DTD HTML 4.0//EN{1}{2}>", q, q, ws), MessageFormatUtil.Format
                        ("<!DOCTYPE html PUBLIC{0}-//W3C//DTD HTML 4.0//EN{1}{2}>", q, q, ws) };
                    foreach (String html in htmls) {
                        Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
                        NUnit.Framework.Assert.AreEqual(expectedOutput, doc.ChildNode(0).OuterHtml());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestSystemIdentifiersWithWhitespace() {
            String expectedOutput = "<!DOCTYPE html SYSTEM \"http://www.w3.org/TR/REC-html40/strict.dtd\">";
            foreach (char q in quote) {
                foreach (char ws in whiteSpace) {
                    String[] htmls = new String[] { MessageFormatUtil.Format("<!DOCTYPE html{0}SYSTEM {1}http://www.w3.org/TR/REC-html40/strict.dtd{2}>"
                        , ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html {0}SYSTEM {1}http://www.w3.org/TR/REC-html40/strict.dtd{2}>"
                        , ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html SYSTEM{0}{1}http://www.w3.org/TR/REC-html40/strict.dtd{2}>"
                        , ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html SYSTEM {0}{1}http://www.w3.org/TR/REC-html40/strict.dtd{2}>"
                        , ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html SYSTEM {0}http://www.w3.org/TR/REC-html40/strict.dtd{1}{2}>"
                        , q, q, ws), MessageFormatUtil.Format("<!DOCTYPE html SYSTEM{0}http://www.w3.org/TR/REC-html40/strict.dtd{1}{2}>"
                        , q, q, ws) };
                    foreach (String html in htmls) {
                        Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
                        NUnit.Framework.Assert.AreEqual(expectedOutput, doc.ChildNode(0).OuterHtml());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPublicAndSystemIdentifiersWithWhitespace() {
            String expectedOutput = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\"" + " \"http://www.w3.org/TR/REC-html40/strict.dtd\">";
            foreach (char q in quote) {
                foreach (char ws in whiteSpace) {
                    String[] htmls = new String[] { MessageFormatUtil.Format("<!DOCTYPE html PUBLIC {0}-//W3C//DTD HTML 4.0//EN{1}"
                         + "{2}{3}http://www.w3.org/TR/REC-html40/strict.dtd{4}>", q, q, ws, q, q), MessageFormatUtil.Format("<!DOCTYPE html PUBLIC {0}-//W3C//DTD HTML 4.0//EN{1}"
                         + "{2}http://www.w3.org/TR/REC-html40/strict.dtd{3}>", q, q, q, q) };
                    foreach (String html in htmls) {
                        Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
                        NUnit.Framework.Assert.AreEqual(expectedOutput, doc.ChildNode(0).OuterHtml());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HandlesLessInTagThanAsNewTag() {
            // out of spec, but clear author intent
            String html = "<p\n<p<div id=one <span>Two";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<p></p><p></p><div id=\"one\"><span>Two</span></div>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestUnconsumeAtBufferBoundary() {
            String triggeringSnippet = "<a href=\"\"foo";
            char[] padding = new char[CharacterReader.readAheadLimit - triggeringSnippet.Length + 2];
            // The "foo" part must be just at the limit.
            JavaUtil.Fill(padding, ' ');
            String paddedSnippet = JavaUtil.GetStringForChars(padding) + triggeringSnippet;
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(paddedSnippet, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(CharacterReader.readAheadLimit - 1, errorList[0].GetPosition());
        }

        [NUnit.Framework.Test]
        public virtual void TestUnconsumeAfterBufferUp() {
            // test for after consume() a bufferUp occurs (look-forward) but then attempts to unconsume. Would throw a "No buffer left to unconsume"
            String triggeringSnippet = "<title>One <span>Two";
            char[] padding = new char[CharacterReader.readAheadLimit - triggeringSnippet.Length + 8];
            // The "<span" part must be just at the limit. The "containsIgnoreCase" scan does a bufferUp, losing the unconsume
            JavaUtil.Fill(padding, ' ');
            String paddedSnippet = JavaUtil.GetStringForChars(padding) + triggeringSnippet;
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(paddedSnippet, null, "", errorList);
        }

        [NUnit.Framework.Test]
        public virtual void TestOpeningAngleBracketInsteadOfAttribute() {
            String triggeringSnippet = "<html <";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(triggeringSnippet, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(6, errorList[0].GetPosition());
        }

        [NUnit.Framework.Test]
        public virtual void TestMalformedSelfClosingTag() {
            String triggeringSnippet = "<html /ouch";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(triggeringSnippet, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(7, errorList[0].GetPosition());
        }

        [NUnit.Framework.Test]
        public virtual void TestOpeningAngleBracketInTagName() {
            String triggeringSnippet = "<html<";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(triggeringSnippet, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(5, errorList[0].GetPosition());
        }

        [NUnit.Framework.Test]
        public virtual void RcData() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>One \x0Two</title>");
            NUnit.Framework.Assert.AreEqual("One �Two", doc.Title());
        }

        [NUnit.Framework.Test]
        public virtual void Plaintext() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One<plaintext><div>Two</plaintext>\x0no < Return"
                );
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><div>One<plaintext>&lt;div&gt;Two&lt;/plaintext&gt;�no &lt; Return</plaintext></div></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void NullInTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<di\x0v>One</di\x0v>Two");
            NUnit.Framework.Assert.AreEqual("<di�v>\n One\n</di�v>Two", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValUnquoted() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p name=foo&lt;bar>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            NUnit.Framework.Assert.AreEqual("foo<bar", p.Attr("name"));
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p foo=");
            NUnit.Framework.Assert.AreEqual("<p foo></p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestRCDATAEndTagNameDiffTag() {
            String body = "<textarea>data</textare >";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected token"));
        }

        [NUnit.Framework.Test]
        public virtual void TestRCDATAEndTagNameValidSlash() {
            String body = "<textarea>data</textarea/>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            Elements els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("data", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestRCDATAEndTagNameInvalidSlash() {
            String body = "<textarea>data</textare/>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected token"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapeStartDashValid() {
            String body = "<script><!-- text --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapeStartDashInvalid() {
            String body = "<script><!- text --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedEmpty() {
            String body = "<script><!-- ";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedStartTag() {
            String body = "<script><!--<</script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedNullChar() {
            String body = "<script><!--a\x0";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashEmpty() {
            String body = "<script><!-- -";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashStTag() {
            String body = "<script><!-- -<</script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashNullChar() {
            String body = "<script><!-- -\x0";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashDashEmpty() {
            String body = "<script><!-- --";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashDashStTag() {
            String body = "<script><!-- --<</script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedDashDashNullChar() {
            String body = "<script><!-- --\x0";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataEscapedEndTagOpen() {
            String body = "<script><!-- --</---></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedNullChar() {
            String body = "<script><!--<script><\x0!-";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedEof() {
            String body = "<script><!--<script><!-";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDash() {
            String body = "<script><!--<script><!-- --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashStTag() {
            String body = "<script><!--<script><!-< --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashNull() {
            String body = "<script><!--<script><!-\x0 --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashEof() {
            String body = "<script><!--<script><!-";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDefault() {
            String body = "<script><!--<script><!-aaa --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDash() {
            String body = "<script><!--<script><!--- --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDashStTag() {
            String body = "<script><!--<script><!--< --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDashNull() {
            String body = "<script><!--<script><!--\x0 --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDashEof() {
            String body = "<script><!--<script><!--";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ScriptDataDoubleEscapedDashDashDefault() {
            String body = "<script><!--<script><!--aaa --></script>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void AttributeNameStTag() {
            String body = "<p name< />";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AfterAttributeNameEndTag() {
            String body = "<p name > />";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void AfterAttributeNameNull() {
            String body = "<p name \x0 />";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AfterAttributeNameEof() {
            String body = "<p name ";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AfterAttributeNameStTag() {
            String body = "<p name <";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void BeforeAttributeNameClTag() {
            String body = "<p name=></p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void BeforeAttributeNameStTag() {
            String body = "<p name=<</p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValueDoubleQuotedNull() {
            String body = "<p name=\"\x0\"></p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValueSingleQuotedNull() {
            String body = "<p name='\x0'></p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValueSingleUnquotedAmp() {
            String body = "<p name=&a></p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(0, errorList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValueSingleUnquotedNull() {
            String body = "<p name=a\x0></p>";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpected character"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributeValueSingleUnquotedEof() {
            String body = "<p name=a";
            ParseErrorList errorList = ParseErrorList.Tracking(1);
            iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment(body, null, "", errorList);
            NUnit.Framework.Assert.AreEqual(1, errorList.Count);
            NUnit.Framework.Assert.IsTrue(errorList[0].GetErrorMessage().Contains("Unexpectedly reached end of file (EOF) in input state"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TokeniserStateToStringTest() {
            NUnit.Framework.Assert.AreEqual("Data", TokeniserState.Data.ToString());
            NUnit.Framework.Assert.AreEqual("CharacterReferenceInData", TokeniserState.CharacterReferenceInData.ToString
                ());
            NUnit.Framework.Assert.AreEqual("Rcdata", TokeniserState.Rcdata.ToString());
            NUnit.Framework.Assert.AreEqual("CharacterReferenceInRcdata", TokeniserState.CharacterReferenceInRcdata.ToString
                ());
            NUnit.Framework.Assert.AreEqual("Rawtext", TokeniserState.Rawtext.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptData", TokeniserState.ScriptData.ToString());
            NUnit.Framework.Assert.AreEqual("PLAINTEXT", TokeniserState.PLAINTEXT.ToString());
            NUnit.Framework.Assert.AreEqual("TagOpen", TokeniserState.TagOpen.ToString());
            NUnit.Framework.Assert.AreEqual("EndTagOpen", TokeniserState.EndTagOpen.ToString());
            NUnit.Framework.Assert.AreEqual("TagName", TokeniserState.TagName.ToString());
            NUnit.Framework.Assert.AreEqual("RcdataLessthanSign", TokeniserState.RcdataLessthanSign.ToString());
            NUnit.Framework.Assert.AreEqual("RCDATAEndTagOpen", TokeniserState.RCDATAEndTagOpen.ToString());
            NUnit.Framework.Assert.AreEqual("RCDATAEndTagName", TokeniserState.RCDATAEndTagName.ToString());
            NUnit.Framework.Assert.AreEqual("RawtextLessthanSign", TokeniserState.RawtextLessthanSign.ToString());
            NUnit.Framework.Assert.AreEqual("RawtextEndTagOpen", TokeniserState.RawtextEndTagOpen.ToString());
            NUnit.Framework.Assert.AreEqual("RawtextEndTagName", TokeniserState.RawtextEndTagName.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataLessthanSign", TokeniserState.ScriptDataLessthanSign.ToString()
                );
            NUnit.Framework.Assert.AreEqual("ScriptDataEndTagOpen", TokeniserState.ScriptDataEndTagOpen.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEndTagName", TokeniserState.ScriptDataEndTagName.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapeStart", TokeniserState.ScriptDataEscapeStart.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapeStartDash", TokeniserState.ScriptDataEscapeStartDash.ToString
                ());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscaped", TokeniserState.ScriptDataEscaped.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapedDash", TokeniserState.ScriptDataEscapedDash.ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapedDashDash", TokeniserState.ScriptDataEscapedDashDash.ToString
                ());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapedLessthanSign", TokeniserState.ScriptDataEscapedLessthanSign
                .ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapedEndTagOpen", TokeniserState.ScriptDataEscapedEndTagOpen.
                ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataEscapedEndTagName", TokeniserState.ScriptDataEscapedEndTagName.
                ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscapeStart", TokeniserState.ScriptDataDoubleEscapeStart.
                ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscaped", TokeniserState.ScriptDataDoubleEscaped.ToString
                ());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscapedDash", TokeniserState.ScriptDataDoubleEscapedDash.
                ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscapedDashDash", TokeniserState.ScriptDataDoubleEscapedDashDash
                .ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscapedLessthanSign", TokeniserState.ScriptDataDoubleEscapedLessthanSign
                .ToString());
            NUnit.Framework.Assert.AreEqual("ScriptDataDoubleEscapeEnd", TokeniserState.ScriptDataDoubleEscapeEnd.ToString
                ());
            NUnit.Framework.Assert.AreEqual("BeforeAttributeName", TokeniserState.BeforeAttributeName.ToString());
            NUnit.Framework.Assert.AreEqual("AttributeName", TokeniserState.AttributeName.ToString());
            NUnit.Framework.Assert.AreEqual("AfterAttributeName", TokeniserState.AfterAttributeName.ToString());
            NUnit.Framework.Assert.AreEqual("BeforeAttributeValue", TokeniserState.BeforeAttributeValue.ToString());
            NUnit.Framework.Assert.AreEqual("AttributeValue_doubleQuoted", TokeniserState.AttributeValue_doubleQuoted.
                ToString());
            NUnit.Framework.Assert.AreEqual("AttributeValue_singleQuoted", TokeniserState.AttributeValue_singleQuoted.
                ToString());
            NUnit.Framework.Assert.AreEqual("AttributeValue_unquoted", TokeniserState.AttributeValue_unquoted.ToString
                ());
            NUnit.Framework.Assert.AreEqual("AfterAttributeValue_quoted", TokeniserState.AfterAttributeValue_quoted.ToString
                ());
            NUnit.Framework.Assert.AreEqual("SelfClosingStartTag", TokeniserState.SelfClosingStartTag.ToString());
            NUnit.Framework.Assert.AreEqual("BogusComment", TokeniserState.BogusComment.ToString());
            NUnit.Framework.Assert.AreEqual("MarkupDeclarationOpen", TokeniserState.MarkupDeclarationOpen.ToString());
            NUnit.Framework.Assert.AreEqual("CommentStart", TokeniserState.CommentStart.ToString());
            NUnit.Framework.Assert.AreEqual("CommentStartDash", TokeniserState.CommentStartDash.ToString());
            NUnit.Framework.Assert.AreEqual("Comment", TokeniserState.Comment.ToString());
            NUnit.Framework.Assert.AreEqual("CommentEndDash", TokeniserState.CommentEndDash.ToString());
            NUnit.Framework.Assert.AreEqual("CommentEnd", TokeniserState.CommentEnd.ToString());
            NUnit.Framework.Assert.AreEqual("CommentEndBang", TokeniserState.CommentEndBang.ToString());
            NUnit.Framework.Assert.AreEqual("Doctype", TokeniserState.Doctype.ToString());
            NUnit.Framework.Assert.AreEqual("BeforeDoctypeName", TokeniserState.BeforeDoctypeName.ToString());
            NUnit.Framework.Assert.AreEqual("DoctypeName", TokeniserState.DoctypeName.ToString());
            NUnit.Framework.Assert.AreEqual("AfterDoctypeName", TokeniserState.AfterDoctypeName.ToString());
            NUnit.Framework.Assert.AreEqual("AfterDoctypePublicKeyword", TokeniserState.AfterDoctypePublicKeyword.ToString
                ());
            NUnit.Framework.Assert.AreEqual("BeforeDoctypePublicIdentifier", TokeniserState.BeforeDoctypePublicIdentifier
                .ToString());
            NUnit.Framework.Assert.AreEqual("DoctypePublicIdentifier_doubleQuoted", TokeniserState.DoctypePublicIdentifier_doubleQuoted
                .ToString());
            NUnit.Framework.Assert.AreEqual("DoctypePublicIdentifier_singleQuoted", TokeniserState.DoctypePublicIdentifier_singleQuoted
                .ToString());
            NUnit.Framework.Assert.AreEqual("AfterDoctypePublicIdentifier", TokeniserState.AfterDoctypePublicIdentifier
                .ToString());
            NUnit.Framework.Assert.AreEqual("BetweenDoctypePublicAndSystemIdentifiers", TokeniserState.BetweenDoctypePublicAndSystemIdentifiers
                .ToString());
            NUnit.Framework.Assert.AreEqual("AfterDoctypeSystemKeyword", TokeniserState.AfterDoctypeSystemKeyword.ToString
                ());
            NUnit.Framework.Assert.AreEqual("BeforeDoctypeSystemIdentifier", TokeniserState.BeforeDoctypeSystemIdentifier
                .ToString());
            NUnit.Framework.Assert.AreEqual("DoctypeSystemIdentifier_doubleQuoted", TokeniserState.DoctypeSystemIdentifier_doubleQuoted
                .ToString());
            NUnit.Framework.Assert.AreEqual("DoctypeSystemIdentifier_singleQuoted", TokeniserState.DoctypeSystemIdentifier_singleQuoted
                .ToString());
            NUnit.Framework.Assert.AreEqual("AfterDoctypeSystemIdentifier", TokeniserState.AfterDoctypeSystemIdentifier
                .ToString());
            NUnit.Framework.Assert.AreEqual("BogusDoctype", TokeniserState.BogusDoctype.ToString());
            NUnit.Framework.Assert.AreEqual("CdataSection", TokeniserState.CdataSection.ToString());
        }
    }
}
