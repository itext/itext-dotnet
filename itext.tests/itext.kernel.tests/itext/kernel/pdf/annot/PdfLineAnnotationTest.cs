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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfLineAnnotationTest : ExtendedITextTest {
        private const float FLOAT_EPSILON_COMPARISON = 1E-6f;

        [NUnit.Framework.Test]
        public virtual void GetLineTest() {
            float[] lineArray = new float[] { 1f, 1f, 1f, 1f };
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new Rectangle(10, 10), lineArray);
            iText.Test.TestUtil.AreEqual(lineArray, pdfLineAnnotation.GetLine().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetBorderStylePdfDictTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Width, new PdfNumber(1));
            pdfLineAnnotation.SetBorderStyle(dict);
            NUnit.Framework.Assert.AreEqual(dict, pdfLineAnnotation.GetBorderStyle());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetBorderStylePdfNameTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            pdfLineAnnotation.SetBorderStyle(PdfName.D);
            NUnit.Framework.Assert.AreEqual(PdfName.D, pdfLineAnnotation.GetBorderStyle().GetAsName(PdfName.S));
        }

        [NUnit.Framework.Test]
        public virtual void SetDashPatternTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            PdfArray array = new PdfArray(new float[] { 1, 2 });
            pdfLineAnnotation.SetDashPattern(array);
            iText.Test.TestUtil.AreEqual(array.ToFloatArray(), pdfLineAnnotation.GetBorderStyle().GetAsArray(PdfName.D
                ).ToFloatArray(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLineEndingStylesTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            PdfArray lineEndingStyles = new PdfArray(new float[] { 1, 2 });
            pdfLineAnnotation.SetLineEndingStyles(lineEndingStyles);
            iText.Test.TestUtil.AreEqual(lineEndingStyles.ToFloatArray(), pdfLineAnnotation.GetLineEndingStyles().ToFloatArray
                (), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetInteriorColorPdfArrayTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            PdfArray array = new PdfArray(colorValues);
            pdfLineAnnotation.SetInteriorColor(array);
            Color expectedColor = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            NUnit.Framework.Assert.AreEqual(expectedColor, pdfLineAnnotation.GetInteriorColor());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetInteriorColorFloatArrayTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            pdfLineAnnotation.SetInteriorColor(colorValues);
            Color expectedColor = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            NUnit.Framework.Assert.AreEqual(expectedColor, pdfLineAnnotation.GetInteriorColor());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLeaderLineLengthTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float length = 1f;
            pdfLineAnnotation.SetLeaderLineLength(length);
            NUnit.Framework.Assert.AreEqual(length, pdfLineAnnotation.GetLeaderLineLength(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void GetLeaderLineLengthNullTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            NUnit.Framework.Assert.AreEqual(0, pdfLineAnnotation.GetLeaderLineLength(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLeaderLineExtensionTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float length = 1f;
            pdfLineAnnotation.SetLeaderLineExtension(length);
            NUnit.Framework.Assert.AreEqual(length, pdfLineAnnotation.GetLeaderLineExtension(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLeaderLineExtensionNullTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            NUnit.Framework.Assert.AreEqual(0, pdfLineAnnotation.GetLeaderLineExtension(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLeaderLineOffsetTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float length = 1f;
            pdfLineAnnotation.SetLeaderLineOffset(length);
            NUnit.Framework.Assert.AreEqual(length, pdfLineAnnotation.GetLeaderLineOffset(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void GetLeaderLineOffsetNullTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            NUnit.Framework.Assert.AreEqual(0, pdfLineAnnotation.GetLeaderLineOffset(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetContentsAsCaptionTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            bool contentsAsCaption = true;
            pdfLineAnnotation.SetContentsAsCaption(contentsAsCaption);
            NUnit.Framework.Assert.AreEqual(contentsAsCaption, pdfLineAnnotation.GetContentsAsCaption());
        }

        [NUnit.Framework.Test]
        public virtual void GetContentsAsCaptionNullTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            NUnit.Framework.Assert.IsFalse(pdfLineAnnotation.GetContentsAsCaption());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetCaptionPositionTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            pdfLineAnnotation.SetCaptionPosition(PdfName.Inline);
            NUnit.Framework.Assert.AreEqual(PdfName.Inline, pdfLineAnnotation.GetCaptionPosition());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetMeasureTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            PdfDictionary measure = new PdfDictionary();
            measure.Put(PdfName.Subtype, new PdfString(""));
            pdfLineAnnotation.SetMeasure(measure);
            NUnit.Framework.Assert.AreEqual(measure, pdfLineAnnotation.GetMeasure());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetCaptionOffsetPdfArrayTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            PdfArray offset = new PdfArray(new float[] { 1, 1 });
            pdfLineAnnotation.SetCaptionOffset(offset);
            iText.Test.TestUtil.AreEqual(offset.ToFloatArray(), pdfLineAnnotation.GetCaptionOffset().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetCaptionOffsetFloatArrayTest() {
            PdfLineAnnotation pdfLineAnnotation = new PdfLineAnnotation(new PdfDictionary());
            float[] offset = new float[] { 1, 1 };
            pdfLineAnnotation.SetCaptionOffset(offset);
            iText.Test.TestUtil.AreEqual(offset, pdfLineAnnotation.GetCaptionOffset().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }
    }
}
