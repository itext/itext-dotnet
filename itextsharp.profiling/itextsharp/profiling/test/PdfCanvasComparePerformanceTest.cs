
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfCanvasComparePerformanceTest : PdfCanvasTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformance(false, 1.15f);
        }
    }
}
