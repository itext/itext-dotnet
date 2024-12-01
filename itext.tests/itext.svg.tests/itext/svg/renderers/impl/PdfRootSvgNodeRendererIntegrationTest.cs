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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Svg.Converter;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfRootSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void CalculateOutermostViewportTest() {
            Rectangle expected = new Rectangle(0, 0, 600, 600);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            PdfRootSvgNodeRenderer root = new PdfRootSvgNodeRenderer(renderer);
            Rectangle actual = PdfRootSvgNodeRenderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateOutermostViewportWithDifferentXYTest() {
            Rectangle expected = new Rectangle(10, 20, 600, 600);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            PdfRootSvgNodeRenderer root = new PdfRootSvgNodeRenderer(renderer);
            Rectangle actual = PdfRootSvgNodeRenderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateNestedViewportDifferentFromParentTest() {
            Rectangle expected = new Rectangle(0, 0, 500, 500);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            context.AddViewPort(expected);
            SvgTagSvgNodeRenderer parent = new SvgTagSvgNodeRenderer();
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            PdfRootSvgNodeRenderer root = new PdfRootSvgNodeRenderer(parent);
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("width", "500");
            styles.Put("height", "500");
            renderer.SetAttributesAndStyles(styles);
            renderer.SetParent(parent);
            Rectangle actual = PdfRootSvgNodeRenderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void NoBoundingBoxOnXObjectTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            ISvgNodeRenderer processed = SvgConverter.Process(SvgConverter.Parse("<svg />"), null).GetRootRenderer();
            PdfRootSvgNodeRenderer root = new PdfRootSvgNodeRenderer(processed);
            PdfFormXObject pdfForm = new PdfFormXObject(new PdfStream());
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            SvgDrawContext context = new SvgDrawContext(null, null);
            context.PushCanvas(canvas);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => root.Draw(context));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.ROOT_SVG_NO_BBOX, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateOutermostTransformation() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, -1d, 0d, 600d);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(new Rectangle(0, 0, 600, 600));
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            PdfRootSvgNodeRenderer root = new PdfRootSvgNodeRenderer(renderer);
            context.AddViewPort(PdfRootSvgNodeRenderer.CalculateViewPort(context));
            AffineTransform actual = root.CalculateTransformation(context);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
