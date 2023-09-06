using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAConformanceLevelTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetConformanceTest() {
            PdfAConformanceLevel level = PdfAConformanceLevel.GetConformanceLevel("4", null);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4, level);
        }

        [NUnit.Framework.Test]
        public virtual void GetXmpConformanceNullTest() {
            XMPMeta meta = new XMPMetaImpl();
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, "4");
            PdfAConformanceLevel level = PdfAConformanceLevel.GetConformanceLevel(meta);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4, level);
        }

        [NUnit.Framework.Test]
        public virtual void GetXmpConformanceBTest() {
            XMPMeta meta = new XMPMetaImpl();
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, "2");
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, "B");
            PdfAConformanceLevel level = PdfAConformanceLevel.GetConformanceLevel(meta);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_2B, level);
        }
    }
}
