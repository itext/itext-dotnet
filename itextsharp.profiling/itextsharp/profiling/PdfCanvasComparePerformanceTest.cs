
using NUnit.Framework;

namespace iTextSharp.Profiling
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
