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
using iText.Commons.Utils;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class PageRangeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddSingleTest() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5), range.GetQualifyingPageNums(10));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesTest() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddSinglePage(1);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 1), range.GetQualifyingPageNums(7));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceTest() {
            PageRange range = new PageRange();
            range.AddPageSequence(11, 19);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 16), range.GetQualifyingPageNums
                (16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingleTest() {
            PageRange range = new PageRange();
            range.AddPageSequence(22, 27);
            range.AddSinglePage(25);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(22, 23, 24, 25, 26, 27, 25), range.GetQualifyingPageNums
                (30));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequenceTest() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddPageSequence(3, 8);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 3, 4, 5, 6, 7, 8), range.GetQualifyingPageNums(10
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfterTest() {
            PageRange range = new PageRange();
            range.AddPageRangePart(new PageRange.PageRangePartAfter(3));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(3, 4, 5), range.GetQualifyingPageNums(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEvenTest() {
            PageRange range = new PageRange();
            range.AddPageRangePart(PageRange.PageRangePartOddEven.EVEN);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(2, 4), range.GetQualifyingPageNums(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAndTest() {
            PageRange range = new PageRange();
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.IPageRangePart and = new PageRange.PageRangePartAnd(odd, seq);
            range.AddPageRangePart(and);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(3, 5, 7, 9, 11, 13), range.GetQualifyingPageNums(15)
                );
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleConstructorTest() {
            PageRange range = new PageRange("5");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5), range.GetQualifyingPageNums(7));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructorTest() {
            PageRange range = new PageRange("5, 1");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 1), range.GetQualifyingPageNums(10));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructorWithNegativeNumbersTest() {
            PageRange range = new PageRange("-5, -1");
            NUnit.Framework.Assert.AreNotEqual(JavaUtil.ArraysAsList(5, 1), range.GetQualifyingPageNums(10));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructorWithWhitespacesTest() {
            PageRange range = new PageRange(" 5 , 1  ");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 1), range.GetQualifyingPageNums(10));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructorWithLetterTest() {
            PageRange range = new PageRange("5, A, 1");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 1), range.GetQualifyingPageNums(10));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructorTest() {
            PageRange range = new PageRange("11-19");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 16), range.GetQualifyingPageNums
                (16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructorWithWhitespacesTest() {
            PageRange range1 = new PageRange(" 11- 19");
            PageRange range2 = new PageRange(" 11 -19");
            PageRange range3 = new PageRange(" 11 - 19");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 16), range1.GetQualifyingPageNums
                (16));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 16), range2.GetQualifyingPageNums
                (16));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 16), range3.GetQualifyingPageNums
                (16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingleConstructorTest() {
            PageRange range = new PageRange("22-27,25");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(22, 23, 24, 25, 26, 27, 25), range.GetQualifyingPageNums
                (30));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequenceConstructorTest() {
            PageRange range = new PageRange("5, 3-8");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 3, 4, 5, 6, 7, 8), range.GetQualifyingPageNums(10
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfterConstructorTest() {
            PageRange range = new PageRange("3-");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(3, 4, 5), range.GetQualifyingPageNums(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEvenConstructorTest() {
            PageRange range = new PageRange("even");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(2, 4), range.GetQualifyingPageNums(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAndConstructorTest() {
            PageRange range = new PageRange("odd & 2-14");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(3, 5, 7, 9, 11, 13), range.GetQualifyingPageNums(15)
                );
        }

        [NUnit.Framework.Test]
        public virtual void AddIncorrectCustomAndConstructorTest() {
            PageRange range = new PageRange("&");
            NUnit.Framework.Assert.AreEqual(new List<Object>(), range.GetQualifyingPageNums(0));
        }

        [NUnit.Framework.Test]
        public virtual void AddIncorrectConstructorTest() {
            PageRange range = new PageRange("");
            NUnit.Framework.Assert.AreEqual(new List<Object>(), range.GetQualifyingPageNums(0));
        }

        [NUnit.Framework.Test]
        public virtual void IsPageInRangeTrueTest() {
            PageRange range = new PageRange("3-8");
            NUnit.Framework.Assert.IsTrue(range.IsPageInRange(6));
        }

        [NUnit.Framework.Test]
        public virtual void IsPageInRangeFalseTest() {
            PageRange range = new PageRange("3-8");
            NUnit.Framework.Assert.IsFalse(range.IsPageInRange(2));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructorWithNegativeNumberTest() {
            PageRange range = new PageRange("-3-8");
            NUnit.Framework.Assert.AreEqual(new List<Object>(), range.GetQualifyingPageNums(3));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructorWithLetterTest() {
            PageRange range1 = new PageRange("3-F");
            PageRange range2 = new PageRange("3-8F");
            NUnit.Framework.Assert.AreEqual(new List<Object>(), range1.GetQualifyingPageNums(3));
            NUnit.Framework.Assert.AreEqual(new List<Object>(), range2.GetQualifyingPageNums(3));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPageRangeEqualsNullTest() {
            PageRange range1 = new PageRange("3-8");
            NUnit.Framework.Assert.IsFalse(range1.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPageRangeEqualsAndHashCodeTest() {
            PageRange range1 = new PageRange("3-8");
            PageRange range2 = new PageRange("3-8");
            bool result = range1.Equals(range2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(range1.GetHashCode(), range2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckPageRangeNotEqualsAndHashCodeTest() {
            PageRange range1 = new PageRange("3-8");
            PageRange range2 = new PageRange("1-2");
            bool result = range1.Equals(range2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(range1.GetHashCode(), range2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetAllPagesInRangeEmptyTest() {
            PageRange.PageRangePartSingle pageRangePartSingle = new PageRange.PageRangePartSingle(10);
            NUnit.Framework.Assert.AreEqual(new List<Object>(), pageRangePartSingle.GetAllPagesInRange(4));
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartSingleInRangeTrueTest() {
            PageRange.PageRangePartSingle pageRangePartSingle = new PageRange.PageRangePartSingle(10);
            NUnit.Framework.Assert.IsTrue(pageRangePartSingle.IsPageInRange(10));
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartSingleInRangeFalseTest() {
            PageRange.PageRangePartSingle pageRangePartSingle = new PageRange.PageRangePartSingle(10);
            NUnit.Framework.Assert.IsFalse(pageRangePartSingle.IsPageInRange(1));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSingleEqualsNullTest() {
            PageRange.PageRangePartSingle pageRangePartSingle = new PageRange.PageRangePartSingle(10);
            NUnit.Framework.Assert.IsFalse(pageRangePartSingle.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSingleEqualsAndHashCodeTest() {
            PageRange.PageRangePartSingle pageRangePartSingle1 = new PageRange.PageRangePartSingle(10);
            PageRange.PageRangePartSingle pageRangePartSingle2 = new PageRange.PageRangePartSingle(10);
            bool result = pageRangePartSingle1.Equals(pageRangePartSingle2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(pageRangePartSingle1.GetHashCode(), pageRangePartSingle2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSingleNotEqualsAndHashCodeTest() {
            PageRange.PageRangePartSingle pageRangePartSingle1 = new PageRange.PageRangePartSingle(10);
            PageRange.PageRangePartSingle pageRangePartSingle2 = new PageRange.PageRangePartSingle(1);
            bool result = pageRangePartSingle1.Equals(pageRangePartSingle2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(pageRangePartSingle1.GetHashCode(), pageRangePartSingle2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSequenceEqualsNullTest() {
            PageRange.PageRangePartSequence pageRangePartSequence = new PageRange.PageRangePartSequence(1, 2);
            NUnit.Framework.Assert.IsFalse(pageRangePartSequence.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSequenceEqualsAndHashCodeTest() {
            PageRange.PageRangePartSequence pageRangePartSequence = new PageRange.PageRangePartSequence(1, 2);
            PageRange.PageRangePartSequence pageRangePartSequence2 = new PageRange.PageRangePartSequence(1, 2);
            bool result = pageRangePartSequence.Equals(pageRangePartSequence2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(pageRangePartSequence.GetHashCode(), pageRangePartSequence2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartSequenceNotEqualsAndHashCodeTest() {
            PageRange.PageRangePartSequence pageRangePartSequence = new PageRange.PageRangePartSequence(1, 2);
            PageRange.PageRangePartSequence pageRangePartSequence2 = new PageRange.PageRangePartSequence(3, 4);
            bool result = pageRangePartSequence.Equals(pageRangePartSequence2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(pageRangePartSequence.GetHashCode(), pageRangePartSequence2.GetHashCode
                ());
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartAfterInRangeTrueTest() {
            PageRange.PageRangePartAfter pageRangePartAfter = new PageRange.PageRangePartAfter(10);
            NUnit.Framework.Assert.IsTrue(pageRangePartAfter.IsPageInRange(11));
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartAfterInRangeFalseTest() {
            PageRange.PageRangePartAfter pageRangePartAfter = new PageRange.PageRangePartAfter(10);
            NUnit.Framework.Assert.IsFalse(pageRangePartAfter.IsPageInRange(1));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAfterEqualsNullTest() {
            PageRange.PageRangePartAfter pageRangePartAfter = new PageRange.PageRangePartAfter(10);
            NUnit.Framework.Assert.IsFalse(pageRangePartAfter.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAfterEqualsAndHashCodeTest() {
            PageRange.PageRangePartAfter pageRangePartAfter = new PageRange.PageRangePartAfter(10);
            PageRange.PageRangePartAfter pageRangePartAfter2 = new PageRange.PageRangePartAfter(10);
            bool result = pageRangePartAfter.Equals(pageRangePartAfter2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(pageRangePartAfter.GetHashCode(), pageRangePartAfter2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAfterNotEqualsAndHashCodeTest() {
            PageRange.PageRangePartAfter pageRangePartAfter = new PageRange.PageRangePartAfter(10);
            PageRange.PageRangePartAfter pageRangePartAfter2 = new PageRange.PageRangePartAfter(1);
            bool result = pageRangePartAfter.Equals(pageRangePartAfter2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(pageRangePartAfter.GetHashCode(), pageRangePartAfter2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartOddEvenInRangeTrueTest() {
            NUnit.Framework.Assert.IsTrue(PageRange.PageRangePartOddEven.ODD.IsPageInRange(11));
            NUnit.Framework.Assert.IsTrue(PageRange.PageRangePartOddEven.EVEN.IsPageInRange(10));
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartOddEvenInRangeFalseTest() {
            NUnit.Framework.Assert.IsFalse(PageRange.PageRangePartOddEven.ODD.IsPageInRange(10));
            NUnit.Framework.Assert.IsFalse(PageRange.PageRangePartOddEven.EVEN.IsPageInRange(11));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartOddEvenEqualsNullTest() {
            NUnit.Framework.Assert.IsFalse(PageRange.PageRangePartOddEven.EVEN.Equals(null));
            NUnit.Framework.Assert.IsFalse(PageRange.PageRangePartOddEven.ODD.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartOddEvenEqualsAndHashCodeTest() {
            NUnit.Framework.Assert.IsTrue(PageRange.PageRangePartOddEven.EVEN.Equals(PageRange.PageRangePartOddEven.EVEN
                ));
            NUnit.Framework.Assert.IsTrue(PageRange.PageRangePartOddEven.ODD.Equals(PageRange.PageRangePartOddEven.ODD
                ));
            NUnit.Framework.Assert.AreEqual(PageRange.PageRangePartOddEven.EVEN.GetHashCode(), PageRange.PageRangePartOddEven
                .EVEN.GetHashCode());
            NUnit.Framework.Assert.AreEqual(PageRange.PageRangePartOddEven.ODD.GetHashCode(), PageRange.PageRangePartOddEven
                .ODD.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartOddEvenNotEqualsAndHashCodeTest() {
            NUnit.Framework.Assert.IsFalse(PageRange.PageRangePartOddEven.EVEN.Equals(PageRange.PageRangePartOddEven.ODD
                ));
            NUnit.Framework.Assert.AreNotEqual(PageRange.PageRangePartOddEven.EVEN.GetHashCode(), PageRange.PageRangePartOddEven
                .ODD.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartAndInRangeTrueTest() {
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.PageRangePartAnd pageRangePartAnd = new PageRange.PageRangePartAnd(odd, seq);
            NUnit.Framework.Assert.IsTrue(pageRangePartAnd.IsPageInRange(5));
        }

        [NUnit.Framework.Test]
        public virtual void IsRangePartAndInRangeFalseTest() {
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.PageRangePartAnd pageRangePartAnd = new PageRange.PageRangePartAnd(odd, seq);
            NUnit.Framework.Assert.IsFalse(pageRangePartAnd.IsPageInRange(1));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAndEqualsNullTest() {
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.PageRangePartAnd pageRangePartAnd = new PageRange.PageRangePartAnd(odd, seq);
            NUnit.Framework.Assert.IsFalse(pageRangePartAnd.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAndEqualsAndHashCodeTest() {
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.PageRangePartAnd pageRangePartAnd = new PageRange.PageRangePartAnd(odd, seq);
            PageRange.PageRangePartAnd pageRangePartAnd2 = new PageRange.PageRangePartAnd(odd, seq);
            bool result = pageRangePartAnd.Equals(pageRangePartAnd2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(pageRangePartAnd.GetHashCode(), pageRangePartAnd2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckRangePartAndNotEqualsAndHashCodeTest() {
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.PageRangePartAnd pageRangePartAnd = new PageRange.PageRangePartAnd(odd, seq);
            PageRange.PageRangePartAnd pageRangePartAnd2 = new PageRange.PageRangePartAnd();
            bool result = pageRangePartAnd.Equals(pageRangePartAnd2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(pageRangePartAnd.GetHashCode(), pageRangePartAnd2.GetHashCode());
        }
    }
}
