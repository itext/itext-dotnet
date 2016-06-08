
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfReaderComparePerformancePartialTest : PdfReaderTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformancePartial(sourceFolder + "performanceTest.pdf", "partial, no compression", 1.7f);
        }
    }
}
