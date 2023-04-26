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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class RotateTransformationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalRotateTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10), CssDimensionParsingUtils
                .ParseAbsoluteLength("5"), CssDimensionParsingUtils.ParseAbsoluteLength("10"));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(10, 5, 10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoRotateValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("rotate()"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void OneRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoRotateValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("rotate(23,58)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ThreeRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(23), CssDimensionParsingUtils
                .ParseAbsoluteLength("58"), CssDimensionParsingUtils.ParseAbsoluteLength("57"));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(23, 58, 57)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TooManyRotateValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("rotate(1 2 3 4)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void NegativeRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(-23), CssDimensionParsingUtils
                .ParseAbsoluteLength("-58"), CssDimensionParsingUtils.ParseAbsoluteLength("-1"));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(-23,-58,-1)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(90));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
