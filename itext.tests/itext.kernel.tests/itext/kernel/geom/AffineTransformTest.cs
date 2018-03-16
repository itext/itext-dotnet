using System;

namespace iText.Kernel.Geom {
    public class AffineTransformTest {
        [NUnit.Framework.Test]
        public virtual void SelfTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsTrue(affineTransform.Equals(affineTransform));
        }

        [NUnit.Framework.Test]
        public virtual void NullTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void OtherClassTest() {
            AffineTransform affineTransform = new AffineTransform();
            String @string = "Test";
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(@string));
        }

        [NUnit.Framework.Test]
        public virtual void SameValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreEqual(hash1, hash2);
            NUnit.Framework.Assert.IsTrue(affineTransform1.Equals(affineTransform2));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(5d, 4d, 3d, 2d, 1d, 1d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreNotEqual(hash1, hash2);
            NUnit.Framework.Assert.IsFalse(affineTransform1.Equals(affineTransform2));
        }
    }
}
