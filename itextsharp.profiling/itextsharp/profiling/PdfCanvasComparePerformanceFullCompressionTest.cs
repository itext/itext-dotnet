
using NUnit.Framework;

namespace iTextSharp.Profiling
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
