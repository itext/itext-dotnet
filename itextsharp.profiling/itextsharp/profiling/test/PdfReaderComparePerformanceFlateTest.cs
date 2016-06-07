
using System;
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfReaderComparePerformanceFlateTest : PdfReaderTest
    {
        [Test]
        public void Test() {
            ComparePerformance(sourceFolder + "performanceTestWithFlate.pdf", "compression, flate", 1.5f);
        }
    }
}
