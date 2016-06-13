using iTextSharp.Test;

namespace iTextSharp.Kernel.Geom {
    public class VectorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCrossVector() {
            Vector v = new Vector(2, 3, 4);
            Matrix m = new Matrix(5, 6, 7, 8, 9, 10);
            Vector shouldBe = new Vector(67, 76, 4);
            Vector rslt = v.Cross(m);
            NUnit.Framework.Assert.AreEqual(shouldBe, rslt);
        }
    }
}
