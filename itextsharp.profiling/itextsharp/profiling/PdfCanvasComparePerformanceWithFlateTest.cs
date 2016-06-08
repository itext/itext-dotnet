
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfCanvasComparePerformanceWithFlateTest : PdfCanvasTest
    {
        [Test]
        [Ignore("")]
        public void Test() {
            ComparePerformanceWithFlateFilter(true, 1.15f);
        }
    }
}
