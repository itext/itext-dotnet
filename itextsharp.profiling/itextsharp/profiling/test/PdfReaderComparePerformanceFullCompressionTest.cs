
using System;
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfReaderComparePerformanceFullCompressionTest : PdfReaderTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformance(sourceFolder + "performanceTestWithCompression.pdf", "compression", 1.5f);
        }
    }
}
