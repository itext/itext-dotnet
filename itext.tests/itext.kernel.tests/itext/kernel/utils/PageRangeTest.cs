/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.IO.Util;

namespace iText.Kernel.Utils {
    public class PageRangeTest {
        [NUnit.Framework.Test]
        public virtual void AddSingle() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), JavaUtil.ArraysAsList(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingles() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddSinglePage(1);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(7), JavaUtil.ArraysAsList(5, 1));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequence() {
            PageRange range = new PageRange();
            range.AddPageSequence(11, 19);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(16), JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 
                16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingle() {
            PageRange range = new PageRange();
            range.AddPageSequence(22, 27);
            range.AddSinglePage(25);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(30), JavaUtil.ArraysAsList(22, 23, 24, 25, 26, 
                27, 25));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequence() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddPageSequence(3, 8);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), JavaUtil.ArraysAsList(5, 3, 4, 5, 6, 7, 8
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfter() {
            PageRange range = new PageRange();
            range.AddPageRangePart(new PageRange.PageRangePartAfter(3));
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), JavaUtil.ArraysAsList(3, 4, 5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEven() {
            PageRange range = new PageRange();
            range.AddPageRangePart(PageRange.PageRangePartOddEven.EVEN);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), JavaUtil.ArraysAsList(2, 4));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAnd() {
            PageRange range = new PageRange();
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.IPageRangePart and = new PageRange.PageRangePartAnd(odd, seq);
            range.AddPageRangePart(and);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(15), JavaUtil.ArraysAsList(3, 5, 7, 9, 11, 13)
                );
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleConstructor() {
            PageRange range = new PageRange("5");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(7), JavaUtil.ArraysAsList(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructor() {
            PageRange range = new PageRange("5, 1");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), JavaUtil.ArraysAsList(5, 1));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructor() {
            PageRange range = new PageRange("11-19");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(16), JavaUtil.ArraysAsList(11, 12, 13, 14, 15, 
                16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingleConstructor() {
            PageRange range = new PageRange("22-27,25");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(30), JavaUtil.ArraysAsList(22, 23, 24, 25, 26, 
                27, 25));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequenceConstructor() {
            PageRange range = new PageRange("5, 3-8");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), JavaUtil.ArraysAsList(5, 3, 4, 5, 6, 7, 8
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfterConstructor() {
            PageRange range = new PageRange("3-");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), JavaUtil.ArraysAsList(3, 4, 5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEvenConstructor() {
            PageRange range = new PageRange("even");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), JavaUtil.ArraysAsList(2, 4));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAndConstructor() {
            PageRange range = new PageRange("odd & 2-14");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(15), JavaUtil.ArraysAsList(3, 5, 7, 9, 11, 13)
                );
        }
    }
}
