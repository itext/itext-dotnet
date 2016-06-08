
using System;
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfReaderComparePerformancePartialFullCompressionTest : PdfReaderTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformancePartial(sourceFolder + "performanceTestWithCompression.pdf", "partial, compression", 1.9f);
        }
    }
}
