namespace iText.Kernel.Utils {
    public class PageRangeTest {
        [NUnit.Framework.Test]
        public virtual void AddSingle() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), iText.IO.Util.JavaUtil.ArraysAsList(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingles() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddSinglePage(1);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(7), iText.IO.Util.JavaUtil.ArraysAsList(5, 1));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequence() {
            PageRange range = new PageRange();
            range.AddPageSequence(11, 19);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(16), iText.IO.Util.JavaUtil.ArraysAsList(11, 12
                , 13, 14, 15, 16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingle() {
            PageRange range = new PageRange();
            range.AddPageSequence(22, 27);
            range.AddSinglePage(25);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(30), iText.IO.Util.JavaUtil.ArraysAsList(22, 23
                , 24, 25, 26, 27, 25));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequence() {
            PageRange range = new PageRange();
            range.AddSinglePage(5);
            range.AddPageSequence(3, 8);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), iText.IO.Util.JavaUtil.ArraysAsList(5, 3, 
                4, 5, 6, 7, 8));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfter() {
            PageRange range = new PageRange();
            range.AddPageRangePart(new PageRange.PageRangePartAfter(3));
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), iText.IO.Util.JavaUtil.ArraysAsList(3, 4, 
                5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEven() {
            PageRange range = new PageRange();
            range.AddPageRangePart(PageRange.PageRangePartOddEven.EVEN);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), iText.IO.Util.JavaUtil.ArraysAsList(2, 4));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAnd() {
            PageRange range = new PageRange();
            PageRange.IPageRangePart odd = PageRange.PageRangePartOddEven.ODD;
            PageRange.IPageRangePart seq = new PageRange.PageRangePartSequence(2, 14);
            PageRange.IPageRangePart and = new PageRange.PageRangePartAnd(odd, seq);
            range.AddPageRangePart(and);
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(15), iText.IO.Util.JavaUtil.ArraysAsList(3, 5, 
                7, 9, 11, 13));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleConstructor() {
            PageRange range = new PageRange("5");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(7), iText.IO.Util.JavaUtil.ArraysAsList(5));
        }

        [NUnit.Framework.Test]
        public virtual void AddSinglesConstructor() {
            PageRange range = new PageRange("5, 1");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), iText.IO.Util.JavaUtil.ArraysAsList(5, 1)
                );
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceConstructor() {
            PageRange range = new PageRange("11-19");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(16), iText.IO.Util.JavaUtil.ArraysAsList(11, 12
                , 13, 14, 15, 16));
        }

        [NUnit.Framework.Test]
        public virtual void AddSequenceAndSingleConstructor() {
            PageRange range = new PageRange("22-27,25");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(30), iText.IO.Util.JavaUtil.ArraysAsList(22, 23
                , 24, 25, 26, 27, 25));
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleAndSequenceConstructor() {
            PageRange range = new PageRange("5, 3-8");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(10), iText.IO.Util.JavaUtil.ArraysAsList(5, 3, 
                4, 5, 6, 7, 8));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAfterConstructor() {
            PageRange range = new PageRange("3-");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), iText.IO.Util.JavaUtil.ArraysAsList(3, 4, 
                5));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomEvenConstructor() {
            PageRange range = new PageRange("even");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(5), iText.IO.Util.JavaUtil.ArraysAsList(2, 4));
        }

        [NUnit.Framework.Test]
        public virtual void AddCustomAndConstructor() {
            PageRange range = new PageRange("odd & 2-14");
            NUnit.Framework.Assert.AreEqual(range.GetQualifyingPageNums(15), iText.IO.Util.JavaUtil.ArraysAsList(3, 5, 
                7, 9, 11, 13));
        }
    }
}
