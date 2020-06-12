using iText.Test;

namespace iText.Kernel.Colors.Gradients {
    public class GradientColorStopTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalizationTest() {
            GradientColorStop stopToTest = new GradientColorStop(new float[] { -0.5f, 1.5f, 0.5f, 0.5f }, 1.5, GradientColorStop.OffsetType
                .AUTO).SetHint(1.5, GradientColorStop.HintOffsetType.NONE);
            NUnit.Framework.Assert.AreEqual(new float[] { 0f, 1f, 0.5f }, stopToTest.GetRgbArray());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.OffsetType.AUTO, stopToTest.GetOffsetType());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetHintOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.HintOffsetType.NONE, stopToTest.GetHintOffsetType());
        }

        [NUnit.Framework.Test]
        public virtual void CornerCasesTest() {
            GradientColorStop stopToTest = new GradientColorStop((float[])null, 1.5, GradientColorStop.OffsetType.AUTO
                ).SetHint(1.5, GradientColorStop.HintOffsetType.NONE);
            NUnit.Framework.Assert.AreEqual(new float[] { 0f, 0f, 0f }, stopToTest.GetRgbArray());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.OffsetType.AUTO, stopToTest.GetOffsetType());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetHintOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.HintOffsetType.NONE, stopToTest.GetHintOffsetType());
        }
    }
}
