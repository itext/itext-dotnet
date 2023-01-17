/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
