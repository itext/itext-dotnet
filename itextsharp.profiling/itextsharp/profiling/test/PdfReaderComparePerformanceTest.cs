
using System;
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
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
