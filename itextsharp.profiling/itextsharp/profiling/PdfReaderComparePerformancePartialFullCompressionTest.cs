
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
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
