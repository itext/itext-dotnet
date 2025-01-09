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
using iText.Kernel.Geom;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class TranslateTransformationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalTranslateTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 15d, 37.5d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(20, 50)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoTranslateValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("translate()"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void OneTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 7.5d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 17.25d, 43.5d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(23,58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, -17.25d, -43.5d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(-23,-58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TooManyTranslateValuesTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => TransformUtils.ParseTransform
                ("translate(1 2 3)"));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES, e.Message
                );
        }
    }
}
