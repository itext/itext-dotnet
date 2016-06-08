
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfReaderComparePerformanceTest : PdfReaderTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformance(sourceFolder + "performanceTest.pdf", "no compression", 1.45f);
        }
    }
}
