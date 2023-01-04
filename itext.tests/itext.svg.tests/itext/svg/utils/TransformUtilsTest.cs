/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
