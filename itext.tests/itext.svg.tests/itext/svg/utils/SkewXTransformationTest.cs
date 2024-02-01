/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    public class SkewXTransformationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalSkewXTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians((float)CssDimensionParsingUtils
                .ParseFloat("143"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(143)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoSkewXValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("skewX()"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TwoSkewXValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("skewX(1 2)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void NegativeSkewXTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians((float)CssDimensionParsingUtils
                .ParseFloat("-26"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(-26)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians((float)CssDimensionParsingUtils
                .ParseFloat("90"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
