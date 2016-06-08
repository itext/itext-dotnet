
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfReaderComparePerformanceFlateTest : PdfReaderTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformance(sourceFolder + "performanceTestWithFlate.pdf", "compression, flate", 1.5f);
        }
    }
}
