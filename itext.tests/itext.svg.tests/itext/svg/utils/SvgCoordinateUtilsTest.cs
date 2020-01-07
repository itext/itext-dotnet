using System;
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Svg.Utils {
    public class SvgCoordinateUtilsTest : ExtendedITextTest {
        private const double delta = 0.0000001;

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors45degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(1, 1, 0);
            double expected = Math.PI / 4;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors45degInverseTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(1, -1, 0);
            double expected = Math.PI / 4;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors135degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, 1, 0);
            double expected = (Math.PI - Math.PI / 4);
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors135degInverseTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, -1, 0);
            double expected = (Math.PI - Math.PI / 4);
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors90degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(0, 1, 0);
            double expected = Math.PI / 2;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors180degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, 0, 0);
            double expected = Math.PI;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, delta);
        }
    }
}
