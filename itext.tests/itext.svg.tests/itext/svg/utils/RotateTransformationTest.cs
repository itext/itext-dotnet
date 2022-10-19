/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
