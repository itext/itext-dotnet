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
