
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfCanvasComparePerformanceWithFlateTest : PdfCanvasTest
    {
        [Test]
        public void Test() {
            ComparePerformanceWithFlateFilter(true, 1.15f);
        }
    }
}
