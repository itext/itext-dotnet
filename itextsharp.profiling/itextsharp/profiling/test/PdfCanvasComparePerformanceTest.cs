
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfCanvasComparePerformanceTest : PdfCanvasTest
    {
        [Test]
        public void Test() {
            ComparePerformance(false, 1.15f);
        }
    }
}
