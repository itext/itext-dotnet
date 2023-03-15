/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Geom;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class TransformUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullStringTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                (null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                (""));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_EMPTY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoTransformationTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("Lorem ipsum"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.INVALID_TRANSFORM_DECLARATION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void WrongTypeOfValuesTest() {
            NUnit.Framework.Assert.Catch(typeof(FormatException), () => TransformUtils.ParseTransform("matrix(a b c d e f)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TooManyParenthesesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("(((())))"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.INVALID_TRANSFORM_DECLARATION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoClosingParenthesisTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(0 0 0 0 0 0");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedCaseTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("maTRix(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void UpperCaseTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("MATRIX(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CommasWithWhitespaceTest() {
            AffineTransform expected = new AffineTransform(10d, 20d, 30d, 40d, 37.5d, 45d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(10, 20, 30, 40, 50, 60)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CommasTest() {
            AffineTransform expected = new AffineTransform(10d, 20d, 30d, 40d, 37.5d, 45d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(10,20,30,40,50,60)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CombinedTransformTest() {
            AffineTransform actual = TransformUtils.ParseTransform("translate(40,20) scale(3)");
            AffineTransform expected = new AffineTransform(3.0, 0d, 0d, 3.0d, 30d, 15d);
            NUnit.Framework.Assert.AreEqual(actual, expected);
        }

        [NUnit.Framework.Test]
        public virtual void CombinedReverseTransformTest() {
            AffineTransform actual = TransformUtils.ParseTransform("scale(3) translate(40,20)");
            AffineTransform expected = new AffineTransform(3d, 0d, 0d, 3d, 90d, 45d);
            NUnit.Framework.Assert.AreEqual(actual, expected);
        }

        [NUnit.Framework.Test]
        public virtual void DoubleTransformationTest() {
            AffineTransform expected = new AffineTransform(9d, 0d, 0d, 9d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(3) scale(3)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void OppositeTransformationSequenceTest() {
            AffineTransform expected = new AffineTransform(1, 0, 0, 1, 0, 0);
            AffineTransform actual = TransformUtils.ParseTransform("translate(10 10) translate(-10 -10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownTransformationTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("unknown(1 2 3)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.UNKNOWN_TRANSFORMATION_TYPE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrailingWhiteSpace() {
            AffineTransform actual = TransformUtils.ParseTransform("translate(1) translate(2) ");
            AffineTransform expected = AffineTransform.GetTranslateInstance(2.25, 0);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void LeadingWhiteSpace() {
            AffineTransform actual = TransformUtils.ParseTransform("   translate(1) translate(2)");
            AffineTransform expected = AffineTransform.GetTranslateInstance(2.25, 0);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MiddleWhiteSpace() {
            AffineTransform actual = TransformUtils.ParseTransform("translate(1)     translate(2)");
            AffineTransform expected = AffineTransform.GetTranslateInstance(2.25, 0);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedWhiteSpace() {
            AffineTransform actual = TransformUtils.ParseTransform("   translate(1)     translate(2)   ");
            AffineTransform expected = AffineTransform.GetTranslateInstance(2.25, 0);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
