using iTextSharp.Test;

namespace iTextSharp.Kernel.Geom
{
    public class MatrixTest : ExtendedITextTest
    {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMultiply()
        {
            Matrix m1 = new Matrix(2, 3, 4, 5, 6, 7);
            Matrix m2 = new Matrix(8, 9, 10, 11, 12, 13);
            Matrix shouldBe = new Matrix(46, 51, 82, 91, 130, 144);
            Matrix rslt = m1.Multiply(m2);
            NUnit.Framework.Assert.AreEqual(shouldBe, rslt);
        }

        [NUnit.Framework.Test]
        public virtual void TestDeterminant()
        {
            Matrix m = new Matrix(2, 3, 4, 5, 6, 7);
            NUnit.Framework.Assert.AreEqual(-2f, m.GetDeterminant(), .001f);
        }
    }
}
