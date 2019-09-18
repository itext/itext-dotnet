using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfDashPatternTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorNoParamTest() {
            PdfDashPattern dashPattern = new PdfDashPattern();
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorOneParamTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorTwoParamsTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10, 20);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorThreeParamsTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10, 20, 30);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(30, dashPattern.GetPhase(), 0.0001);
        }
    }
}
