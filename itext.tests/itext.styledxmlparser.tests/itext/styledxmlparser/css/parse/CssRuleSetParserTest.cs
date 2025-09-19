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
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssRuleSetParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParsePropertyDeclarationsTest() {
            String src = "float:right; clear:right;width:22.0em; margin:0 0 1.0em 1.0em; background:#f9f9f9; " + "border:1px solid #aaa;padding:0.2em;border-spacing:0.4em 0; text-align:center; "
                 + "line-height:1.4em; font-size:88%;";
            String[] expected = new String[] { "float: right", "clear: right", "width: 22.0em", "margin: 0 0 1.0em 1.0em"
                , "background: #f9f9f9", "border: 1px solid #aaa", "padding: 0.2em", "border-spacing: 0.4em 0", "text-align: center"
                , "line-height: 1.4em", "font-size: 88%" };
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(src);
            NUnit.Framework.Assert.AreEqual(expected.Length, declarations.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], declarations[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CommaInsidePseudoClassTest() {
            String src = "p > :not(strong), :not(b.important)";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "p > :not(strong) {\n" + "    color: darkmagenta\n" + "}", ":not(b.important) {\n"
                 + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultPseudoClassTest() {
            String src = "p > :not(strong)";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "p > :not(strong) {\n" + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoubleQuotesPseudoClassTest() {
            String src = "p > :not(strong) \"asd\"";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "p > :not(strong) \"asd\" {\n" + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SingleQuotesPseudoClassTest() {
            String src = "p > :not(strong) 'a'";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "p > :not(strong) 'a' {\n" + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FunctionAndSelectTest() {
            String src = "*[data-dito-element=listbox] select";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "*[data-dito-element=\"listbox\"] select {\n" + "    color: darkmagenta\n"
                 + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SingleCommaEndingPseudoClassTest() {
            String src = "p > :not(strong),";
            String properties = "color: darkmagenta;";
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(0, cssRuleSets.Count);
        }

        [NUnit.Framework.Test]
        public virtual void CommaEndingPseudoClassTest() {
            String src = "p > :not(strong),,,";
            String properties = "color: darkmagenta;";
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(0, cssRuleSets.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SpaceBetweenPseudoClassesTest() {
            String src = ":not(:empty) :not(:empty) :not(:empty) :not(:empty) :not(:empty)";
            String properties = "border: green solid 3px;";
            String[] expected = new String[] { ":not(:empty) :not(:empty) :not(:empty) :not(:empty) :not(:empty) {\n" 
                + "    border: green solid 3px\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void PseudoClassWithSumTest() {
            String src = " article:not(.archived) section.highlight + aside:not(.hidden, [data-disabled=\"true\"])";
            String properties = "background-color: lightyellow;";
            String[] expected = new String[] { "article:not(.archived) section.highlight + aside:not(.hidden , [data-disabled=\"true\"]) {\n"
                 + "    background-color: lightyellow\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComplexPseudoClassTest() {
            String src = "div:not([data-role=\"admin\"], :nth-of-type(2n)):not(:empty)::before";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "div:not([data-role=\"admin\"] , :nth-of-type(2n)):not(:empty)::before {\n"
                 + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotAddSpaceAfterFourDotsTest() {
            String src = "[data-foo]::before";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "[data-foo]::before {\n" + "    color: darkmagenta\n" + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DisappearedCommasTest() {
            String src = "strong:not([data-time=\"37\"], :empty, b.warning)";
            String properties = "color: darkmagenta;";
            String[] expected = new String[] { "strong:not([data-time=\"37\"] , :empty , b.warning) {\n" + "    color: darkmagenta\n"
                 + "}" };
            IList<CssRuleSet> cssRuleSets = CssRuleSetParser.ParseRuleSet(src, properties);
            NUnit.Framework.Assert.AreEqual(expected.Length, cssRuleSets.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], cssRuleSets[i].ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SplitByTokensQuotesTest() {
            NUnit.Framework.Assert.AreEqual(new String[] { "a.className 'someText'" }, CssRuleSetParser.SplitByTokens(
                "a.className 'someText'"));
            NUnit.Framework.Assert.AreEqual(new String[] { "a.className \"someText\"" }, CssRuleSetParser.SplitByTokens
                ("a.className \"someText\""));
            NUnit.Framework.Assert.AreEqual(new String[] { "a.className text" }, CssRuleSetParser.SplitByTokens("a.className \"text"
                ));
            NUnit.Framework.Assert.AreEqual(new String[] { "a.className text" }, CssRuleSetParser.SplitByTokens("a.className text'"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SplitByTokensCommaTest() {
            NUnit.Framework.Assert.AreEqual(new String[] { "strong:not([data-time=\"37\"], :empty, b.warning)" }, CssRuleSetParser
                .SplitByTokens("strong:not([data-time=\"37\"], :empty, b.warning)"));
            NUnit.Framework.Assert.AreEqual(new String[] { "p > :not(strong, b.important)" }, CssRuleSetParser.SplitByTokens
                ("p > :not(strong, b.important)"));
            NUnit.Framework.Assert.AreEqual(new String[] { "\"a.class, b\"" }, CssRuleSetParser.SplitByTokens("\"a.class, b\""
                ));
            NUnit.Framework.Assert.AreEqual(new String[] { "'a.class, b'" }, CssRuleSetParser.SplitByTokens("'a.class, b'"
                ));
            NUnit.Framework.Assert.AreEqual(new String[] { "a.class", "p" }, CssRuleSetParser.SplitByTokens("a.class, p"
                ));
        }
    }
}
