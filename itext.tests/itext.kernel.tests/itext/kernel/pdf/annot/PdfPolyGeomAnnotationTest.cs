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
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfPolyGeomAnnotationTest : ExtendedITextTest {
        private const float FLOAT_EPSILON_COMPARISON = 1E-6f;

        [NUnit.Framework.Test]
        public virtual void CreatePolygonTest() {
            Rectangle rect = new Rectangle(10, 10);
            float[] vertices = new float[] { 1, 1, 1, 1 };
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = PdfPolyGeomAnnotation.CreatePolygon(rect, vertices);
            NUnit.Framework.Assert.IsTrue(pdfPolyGeomAnnotation.GetRectangle().ToRectangle().EqualsWithEpsilon(rect), 
                "Rectangles are not equal");
            iText.Test.TestUtil.AreEqual(vertices, pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void CreatePolylineTest() {
            Rectangle rect = new Rectangle(10, 10);
            float[] vertices = new float[] { 1, 1, 1, 1 };
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = PdfPolyGeomAnnotation.CreatePolyLine(rect, vertices);
            NUnit.Framework.Assert.IsTrue(pdfPolyGeomAnnotation.GetRectangle().ToRectangle().EqualsWithEpsilon(rect), 
                "Rectangles are not equal");
            iText.Test.TestUtil.AreEqual(vertices, pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetVerticesFloatArrayTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            float[] vertices = new float[] { 1, 1, 1, 1 };
            pdfPolyGeomAnnotation.SetVertices(vertices);
            iText.Test.TestUtil.AreEqual(vertices, pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PATH_KEY_IS_PRESENT_VERTICES_WILL_BE_IGNORED)]
        public virtual void SetAndGetVerticesFloatArrayLogMessageTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Path, new PdfString(""));
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(dict);
            float[] vertices = new float[] { 1, 1, 1, 1 };
            pdfPolyGeomAnnotation.SetVertices(vertices);
            iText.Test.TestUtil.AreEqual(vertices, pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), FLOAT_EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetVerticesPdfArrayTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            PdfArray vertices = new PdfArray(new float[] { 1, 1, 1, 1 });
            pdfPolyGeomAnnotation.SetVertices(vertices);
            iText.Test.TestUtil.AreEqual(vertices.ToFloatArray(), pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), 
                FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PATH_KEY_IS_PRESENT_VERTICES_WILL_BE_IGNORED)]
        public virtual void SetAndGetVerticesPdfArrayLogMessageTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Path, new PdfString(""));
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(dict);
            PdfArray vertices = new PdfArray(new float[] { 1, 1, 1, 1 });
            pdfPolyGeomAnnotation.SetVertices(vertices);
            iText.Test.TestUtil.AreEqual(vertices.ToFloatArray(), pdfPolyGeomAnnotation.GetVertices().ToFloatArray(), 
                FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetLineEndingStylesTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Path, new PdfString(""));
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(dict);
            PdfArray lineEndingStyles = new PdfArray(new float[] { 1, 2 });
            pdfPolyGeomAnnotation.SetLineEndingStyles(lineEndingStyles);
            iText.Test.TestUtil.AreEqual(lineEndingStyles.ToFloatArray(), pdfPolyGeomAnnotation.GetLineEndingStyles().
                ToFloatArray(), FLOAT_EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetMeasureTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Path, new PdfString(""));
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(dict);
            PdfDictionary measure = new PdfDictionary();
            measure.Put(PdfName.Subtype, new PdfString(""));
            pdfPolyGeomAnnotation.SetMeasure(measure);
            NUnit.Framework.Assert.AreEqual(measure, pdfPolyGeomAnnotation.GetMeasure());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetPathTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            IList<PdfObject> arrays = new List<PdfObject>();
            arrays.Add(new PdfArray(new float[] { 10, 10 }));
            PdfArray path = new PdfArray(arrays);
            pdfPolyGeomAnnotation.SetPath(path);
            NUnit.Framework.Assert.AreEqual(path.ToString(), pdfPolyGeomAnnotation.GetPath().ToString());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.IF_PATH_IS_SET_VERTICES_SHALL_NOT_BE_PRESENT)]
        public virtual void SetAndGetPathLogMessageTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            pdfPolyGeomAnnotation.SetVertices(new float[] { 1, 1, 1, 1 });
            IList<PdfObject> arrays = new List<PdfObject>();
            arrays.Add(new PdfArray(new float[] { 10, 10 }));
            PdfArray path = new PdfArray(arrays);
            pdfPolyGeomAnnotation.SetPath(path);
            NUnit.Framework.Assert.AreEqual(path, pdfPolyGeomAnnotation.GetPath());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetBorderStylePdfDictTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            PdfDictionary style = new PdfDictionary();
            style.Put(PdfName.Width, new PdfNumber(1));
            pdfPolyGeomAnnotation.SetBorderStyle(style);
            NUnit.Framework.Assert.AreEqual(style, pdfPolyGeomAnnotation.GetBorderStyle());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetBorderStylePdfNameTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            pdfPolyGeomAnnotation.SetBorderStyle(PdfName.D);
            NUnit.Framework.Assert.AreEqual(PdfName.D, pdfPolyGeomAnnotation.GetBorderStyle().GetAsName(PdfName.S));
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetDashPatternTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            PdfArray array = new PdfArray(new float[] { 1, 2 });
            pdfPolyGeomAnnotation.SetDashPattern(array);
            NUnit.Framework.Assert.AreEqual(array, pdfPolyGeomAnnotation.GetBorderStyle().GetAsArray(PdfName.D));
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetBorderEffectTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            PdfDictionary dict = new PdfDictionary();
            pdfPolyGeomAnnotation.SetBorderEffect(dict);
            NUnit.Framework.Assert.AreEqual(dict, pdfPolyGeomAnnotation.GetBorderEffect());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetInteriorColorPdfArrayTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            PdfArray array = new PdfArray(colorValues);
            pdfPolyGeomAnnotation.SetInteriorColor(array);
            Color expectedColor = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            NUnit.Framework.Assert.AreEqual(expectedColor, pdfPolyGeomAnnotation.GetInteriorColor());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetInteriorColorFloatArrayTest() {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = new PdfPolygonAnnotation(new PdfDictionary());
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            pdfPolyGeomAnnotation.SetInteriorColor(colorValues);
            Color expectedColor = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            NUnit.Framework.Assert.AreEqual(expectedColor, pdfPolyGeomAnnotation.GetInteriorColor());
        }
    }
}
