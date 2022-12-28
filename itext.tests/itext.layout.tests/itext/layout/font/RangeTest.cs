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
using iText.Test;

namespace iText.Layout.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class RangeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestWrongRange() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new RangeBuilder().AddRange(11, 10));
        }

        [NUnit.Framework.Test]
        public virtual void TestWrongRangeSize() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new RangeBuilder().Create());
        }

        [NUnit.Framework.Test]
        public virtual void TestFullRange() {
            NUnit.Framework.Assert.IsTrue(RangeBuilder.GetFullRange().Contains(new Random().Next()));
            NUnit.Framework.Assert.IsTrue(RangeBuilder.GetFullRange().Equals(RangeBuilder.GetFullRange()));
            NUnit.Framework.Assert.IsTrue(RangeBuilder.GetFullRange() == RangeBuilder.GetFullRange());
            NUnit.Framework.Assert.IsFalse(RangeBuilder.GetFullRange().Equals(new RangeBuilder().AddRange(1).Create())
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestHashCodeAndEquals() {
            Range range = new RangeBuilder((char)25, (char)26).AddRange(1, 5).AddRange(4, 7).Create();
            Range range2 = new RangeBuilder(25, 26).AddRange((char)1, (char)7).Create();
            NUnit.Framework.Assert.IsTrue(range.GetHashCode() == range2.GetHashCode());
            NUnit.Framework.Assert.IsTrue(range.Equals(range2));
            NUnit.Framework.Assert.AreEqual(range.ToString(), range2.ToString());
            Range range3 = new RangeBuilder(25).AddRange((char)26).AddRange((char)1, (char)7).Create();
            NUnit.Framework.Assert.IsFalse(range2.GetHashCode() == range3.GetHashCode());
            NUnit.Framework.Assert.IsFalse(range2.Equals(range3));
            NUnit.Framework.Assert.AreNotEqual(range2.ToString(), range3.ToString());
            Range range4 = new RangeBuilder(26).AddRange((char)25).AddRange((char)1, (char)4).AddRange((char)3, (char)
                7).Create();
            NUnit.Framework.Assert.IsTrue(range3.GetHashCode() == range4.GetHashCode());
            NUnit.Framework.Assert.IsTrue(range3.Equals(range4));
            NUnit.Framework.Assert.AreEqual(range3.ToString(), range4.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestUnionAndContains() {
            Range range = new RangeBuilder((char)25, (char)27).AddRange(2, 10).AddRange(0, 20).AddRange(1, 19).AddRange
                (33, 40).AddRange(0, 5).AddRange(20, 22).AddRange(8, 15).AddRange(25, 30).Create();
            NUnit.Framework.Assert.AreEqual("[(0; 22), (25; 30), (33; 40)]", range.ToString());
            NUnit.Framework.Assert.IsTrue(range.Contains(0));
            NUnit.Framework.Assert.IsTrue(range.Contains(10));
            NUnit.Framework.Assert.IsTrue(range.Contains(22));
            NUnit.Framework.Assert.IsTrue(range.Contains(25));
            NUnit.Framework.Assert.IsTrue(range.Contains(27));
            NUnit.Framework.Assert.IsTrue(range.Contains(30));
            NUnit.Framework.Assert.IsTrue(range.Contains(33));
            NUnit.Framework.Assert.IsTrue(range.Contains(34));
            NUnit.Framework.Assert.IsTrue(range.Contains(40));
            NUnit.Framework.Assert.IsFalse(range.Contains(-1));
            NUnit.Framework.Assert.IsFalse(range.Contains(23));
            NUnit.Framework.Assert.IsFalse(range.Contains(31));
            NUnit.Framework.Assert.IsFalse(range.Contains(32));
            NUnit.Framework.Assert.IsFalse(range.Contains(41));
        }

        [NUnit.Framework.Test]
        public virtual void TestSingles() {
            Range range = new RangeBuilder((char)1).AddRange(2).AddRange(3).AddRange(6).Create();
            NUnit.Framework.Assert.AreEqual("[(1; 1), (2; 2), (3; 3), (6; 6)]", range.ToString());
            NUnit.Framework.Assert.IsTrue(range.Contains(1));
            NUnit.Framework.Assert.IsTrue(range.Contains(2));
            NUnit.Framework.Assert.IsTrue(range.Contains(3));
            NUnit.Framework.Assert.IsTrue(range.Contains(6));
            NUnit.Framework.Assert.IsFalse(range.Contains(0));
            NUnit.Framework.Assert.IsFalse(range.Contains(5));
            NUnit.Framework.Assert.IsFalse(range.Contains(7));
        }
    }
}
