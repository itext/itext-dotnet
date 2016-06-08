
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfCanvasComparePerformanceTest : PdfCanvasTest
    {
        [Test]
        [Ignore("")]
        public void Test() {
            ComparePerformance(false, 1.15f);
        }
    }
}
