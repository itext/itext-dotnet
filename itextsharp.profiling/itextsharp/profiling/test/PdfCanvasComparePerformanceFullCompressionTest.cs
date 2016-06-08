
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfCanvasComparePerformanceFullCompressionTest : PdfCanvasTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformance(true, 1.2f);
        }
    }
}
