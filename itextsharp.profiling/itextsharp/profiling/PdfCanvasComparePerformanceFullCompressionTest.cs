
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfCanvasComparePerformanceFullCompressionTest : PdfCanvasTest
    {
        [Test]
        [Ignore("")]
        public void Test() {
            ComparePerformance(true, 1.2f);
        }
    }
}
