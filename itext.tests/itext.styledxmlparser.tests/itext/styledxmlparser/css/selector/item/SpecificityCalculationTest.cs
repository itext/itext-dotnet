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
using iText.StyledXmlParser.Css.Selector;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class SpecificityCalculationTest : ExtendedITextTest {
        // https://www.smashingmagazine.com/2007/07/css-specificity-things-you-should-know/
        // https://specificity.keegan.st/
        [NUnit.Framework.Test]
        public virtual void Test01() {
            NUnit.Framework.Assert.AreEqual(0, GetSpecificity("*"));
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            NUnit.Framework.Assert.AreEqual(1, GetSpecificity("li"));
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            NUnit.Framework.Assert.AreEqual(2, GetSpecificity("li:first-line"));
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            NUnit.Framework.Assert.AreEqual(2, GetSpecificity("ul li"));
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            NUnit.Framework.Assert.AreEqual(3, GetSpecificity("ul ol+li"));
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY + CssSpecificityConstants.ELEMENT_SPECIFICITY
                , GetSpecificity("h1 + *[rel=up]"));
        }

        [NUnit.Framework.Test]
        public virtual void Test07() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY + CssSpecificityConstants.ELEMENT_SPECIFICITY
                 * 3, GetSpecificity("ul ol li.red"));
        }

        [NUnit.Framework.Test]
        public virtual void Test08() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY * 2 + CssSpecificityConstants.ELEMENT_SPECIFICITY
                , GetSpecificity("li.red.level"));
        }

        [NUnit.Framework.Test]
        public virtual void Test09() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY, GetSpecificity(".sith"));
        }

        [NUnit.Framework.Test]
        public virtual void Test10() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY + CssSpecificityConstants.ELEMENT_SPECIFICITY
                 * 2, GetSpecificity("div p.sith"));
        }

        [NUnit.Framework.Test]
        public virtual void Test11() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY, GetSpecificity("#sith"));
        }

        [NUnit.Framework.Test]
        public virtual void Test12() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                 + CssSpecificityConstants.ELEMENT_SPECIFICITY * 2, GetSpecificity("body #darkside .sith p"));
        }

        [NUnit.Framework.Test]
        public virtual void Test13() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY * 2 + CssSpecificityConstants.ELEMENT_SPECIFICITY
                 * 2, GetSpecificity("li:first-child h2 .title"));
        }

        [NUnit.Framework.Test]
        public virtual void Test14() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                 * 2 + CssSpecificityConstants.ELEMENT_SPECIFICITY, GetSpecificity("#nav .selected > a:hover"));
        }

        [NUnit.Framework.Test]
        public virtual void Test15() {
            NUnit.Framework.Assert.AreEqual(2, GetSpecificity("p:before"));
            NUnit.Framework.Assert.AreEqual(2, GetSpecificity("p::before"));
        }

        [NUnit.Framework.Test]
        public virtual void Test16() {
            NUnit.Framework.Assert.AreEqual(2, GetSpecificity("a::hover"));
        }

        [NUnit.Framework.Test]
        public virtual void Test17() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY * 2, GetSpecificity(".class_name:nth-child(3n + 1)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Test18() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY * 2, GetSpecificity(".class_name:nth-child(2n - 3)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Test19() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY * 2, GetSpecificity(".class_name:hover"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Test20() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY, GetSpecificity(":not(p)"));
        }

        [NUnit.Framework.Test]
        public virtual void Test21() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY, GetSpecificity(":not(#id)"));
        }

        [NUnit.Framework.Test]
        public virtual void Test22() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.CLASS_SPECIFICITY, GetSpecificity(":not(.class_name)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest01() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY, GetPageSelectorSpecificity("customPageName"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest02() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                , GetPageSelectorSpecificity("customPageName:first"));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest03() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                 * 2, GetPageSelectorSpecificity("customPageName:first:blank"));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest04() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ELEMENT_SPECIFICITY * 2, GetPageSelectorSpecificity
                (":left:right"));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest05() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ID_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                , GetPageSelectorSpecificity("left:blank"));
        }

        [NUnit.Framework.Test]
        public virtual void PageTest06() {
            NUnit.Framework.Assert.AreEqual(CssSpecificityConstants.ELEMENT_SPECIFICITY + CssSpecificityConstants.CLASS_SPECIFICITY
                , GetPageSelectorSpecificity(":left:blank"));
        }

        private int GetSpecificity(String selector) {
            return new CssSelector(selector).CalculateSpecificity();
        }

        private int GetPageSelectorSpecificity(String selector) {
            return new CssPageSelector(selector).CalculateSpecificity();
        }
    }
}
