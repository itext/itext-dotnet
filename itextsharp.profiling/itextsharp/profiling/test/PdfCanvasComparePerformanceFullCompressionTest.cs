
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfCanvasComparePerformanceFullCompressionTest : PdfCanvasTest
    {
        [Test]
        public void Test() {
            ComparePerformance(true, 1.2f);
        }
    }
}
