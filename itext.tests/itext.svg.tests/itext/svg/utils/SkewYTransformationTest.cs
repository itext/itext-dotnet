/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class SkewYTransformationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalSkewYTest() {
            AffineTransform expected = new AffineTransform(1d, Math.Tan(MathUtil.ToRadians(143)), 0d, 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(143)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoSkewYValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("skewY()"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TwoSkewYValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("skewY(1 2)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void NegativeSkewYTest() {
            AffineTransform expected = new AffineTransform(1d, Math.Tan(MathUtil.ToRadians(-26)), 0d, 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(-26)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = new AffineTransform(1d, Math.Tan(MathUtil.ToRadians(90)), 0d, 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
