
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfCanvasComparePerformanceWithFlateTest : PdfCanvasTest
    {
        [Test]
        [Timeout(300000)]
        public void Test() {
            ComparePerformanceWithFlateFilter(true, 1.15f);
        }
    }
}
