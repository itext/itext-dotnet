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
