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
using System.Collections.Generic;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgCssUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CommaSplitValueTest() {
            String input = "a,b,c,d";
            IList<String> expected = new List<String>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void LeadingAndTrailingWhiteSpaceTest() {
            String input = "          -140.465,-116.438 -163.725,-103.028 -259.805,-47.618         ";
            IList<String> expected = new List<String>();
            expected.Add("-140.465");
            expected.Add("-116.438");
            expected.Add("-163.725");
            expected.Add("-103.028");
            expected.Add("-259.805");
            expected.Add("-47.618");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceSplitValueTest() {
            String input = "1 2 3 4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NewLineSplitValueTest() {
            String input = "1\n2\n3\n4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TabSplitValueTest() {
            String input = "1\t2\t3\t4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedCommaWhitespaceSplitValueTest() {
            String input = "1,2 a,b";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("a");
            expected.Add("b");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStringsSplitValueTest() {
            String input = " \n1,,\n 2   a  ,\tb  ,";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("a");
            expected.Add("b");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NullSplitValueTest() {
            IList<String> actual = SvgCssUtils.SplitValueList(null);
            NUnit.Framework.Assert.IsTrue(actual.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void EmptySplitValueTest() {
            IList<String> actual = SvgCssUtils.SplitValueList("");
            NUnit.Framework.Assert.IsTrue(actual.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ConvertFloatToStringTest() {
            String expected = "0.5";
            String actual = SvgCssUtils.ConvertFloatToString(0.5f);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertLongerFloatToStringTest() {
            String expected = "0.1234567";
            String actual = SvgCssUtils.ConvertFloatToString(0.1234567f);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
