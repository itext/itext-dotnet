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
using iText.Kernel.Geom;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgStrokeParameterConverterUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestStrokeDashArrayPercents() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("10pt,3%", null, 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 10, 30
                 }, 0), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeDashArrayOddNumberOfValues() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt", null, 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 5 }
                , 0), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyStrokeDashArray() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("", null, 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeDashOffsetPercents() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "10%", 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 100), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyStrokeDashOffset() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "", 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 0), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeDashOffset() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("5pt,3pt", "10", 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 5, 3 }
                , 7.5f), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeEm() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("1em,2em", "0.5em", 8f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 8, 16 }
                , 4), result);
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeRem() {
            SvgStrokeParameterConverter.PdfLineDashParameters result = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                ("1rem,2rem", "0.5rem", 12f, CreateTextSvgContext());
            NUnit.Framework.Assert.AreEqual(new SvgStrokeParameterConverter.PdfLineDashParameters(new float[] { 12, 24
                 }, 6), result);
        }

        private SvgDrawContext CreateTextSvgContext() {
            SvgDrawContext context = new SvgDrawContext(null, null);
            context.AddViewPort(new Rectangle(1000, 1000));
            return context;
        }
    }
}
