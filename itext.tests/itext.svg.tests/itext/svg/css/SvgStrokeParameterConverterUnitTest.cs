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
using iText.Svg.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgStrokeParameterConverterUnitTest : ExtendedITextTest {
        [LogMessage(SvgLogMessageConstant.PERCENTAGE_VALUES_IN_STROKE_DASHARRAY_AND_STROKE_DASHOFFSET_ARE_NOT_SUPPORTED
            )]
        [NUnit.Framework.Test]
        public virtual void TestStrokeDashArrayPercentsAreNotSupported() {
            NUnit.Framework.Assert.IsNull(SvgStrokeParameterConverter.ConvertStrokeDashParameters("5,3%", null));
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeDashArrayOddNumberOfValues() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt", null);
            NUnit.Framework.Assert.IsNotNull(result);
            NUnit.Framework.Assert.AreEqual(0, result.GetDashPhase(), 0);
            iText.Test.TestUtil.AreEqual(new float[] { 5, 5 }, result.GetDashArray(), 1e-5f);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyStrokeDashArray() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("", null);
            NUnit.Framework.Assert.IsNull(result);
        }

        [LogMessage(SvgLogMessageConstant.PERCENTAGE_VALUES_IN_STROKE_DASHARRAY_AND_STROKE_DASHOFFSET_ARE_NOT_SUPPORTED
            )]
        [NUnit.Framework.Test]
        public virtual void TestStrokeDashOffsetPercentsAreNotSupported() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "10%");
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 0), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyStrokeDashOffset() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "");
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 0), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeDashOffset() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "10");
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 7.5f), result);
        }
    }
}
