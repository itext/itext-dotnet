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
using System;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPropertyMergerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSimpleTest() {
            String firstValue = "underline";
            String secondValue = "strikethrough bold";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationNormalizeFirstTest() {
            String firstValue = "   underline  ";
            String secondValue = "strikethrough bold";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationNormalizeSecondTest() {
            String firstValue = "underline";
            String secondValue = "strikethrough     bold   ";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationFirstNullTest() {
            String firstValue = null;
            String secondValue = "strikethrough bold";
            String expected = "strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSecondNullTest() {
            String firstValue = "underline";
            String secondValue = null;
            String expected = "underline";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationBothNullTest() {
            String firstValue = null;
            String secondValue = null;
            String expected = null;
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationEmpyInputsTest() {
            String firstValue = "";
            String secondValue = "";
            String expected = "none";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSecondInputContainsNoneTest() {
            String firstValue = "underline";
            String secondValue = "none strikethrough";
            String expected = "underline";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationFirstInputNoneTest() {
            String firstValue = "underline none";
            String secondValue = "strikethrough";
            String expected = "strikethrough";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationBothInputsNoneTest() {
            String firstValue = "underline none";
            String secondValue = "strikethrough none";
            String expected = "none";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
