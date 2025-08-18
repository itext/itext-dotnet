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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TransparencyTest : ExtendedITextTest {
        private static readonly PdfName DEFAULT_RESOURCE_NAME = new PdfName("Gs1");

        private static readonly PdfName FILL_OPAC = new PdfName("ca");

        private static readonly PdfName STROKE_OPAC = new PdfName("CA");

        [NUnit.Framework.Test]
        public virtual void NoOpacitySet() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStroke() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStroke() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFill() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStroke() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFill() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFill() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFill() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStroke() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "blue");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFill() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "blue");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "green");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(2, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStrokeRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStrokeRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFillRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStrokeRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFillRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFillRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFillRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStrokeRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(100,20,80)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFillRGB() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgb(100,20,80)");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgb(60,90,180)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(2, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80, .75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.5625));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithoutStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacitySetWithNoneStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.5625));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithoutFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithNoneFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacitySetWithStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(2, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillAndStrokeOpacitySetWithStrokeAndFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(60,90,180,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(2, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.5625));
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.5625));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithoutStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(1, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithNoneStrokeRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.STROKE_OPACITY, "0.75");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoOpacitySetWithNoneFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL, SvgConstants.Values.NONE);
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.IsTrue(resources.GetResourceNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoAndStrokeOpacitySetWithStrokeAndFillRGBA() {
            // set compression to none, in case you want to write to disk and inspect the created document
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetCompressionLevel(0)))) {
                SvgDrawContext sdc = SetupDrawContext(pdfDocument);
                AbstractSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
                renderer.SetAttribute(SvgConstants.Attributes.FILL, "rgba(100,20,80,.75)");
                renderer.SetAttribute(SvgConstants.Attributes.STROKE, "rgba(60,90,180,.75)");
                renderer.Draw(sdc);
                PdfCanvas cv = sdc.GetCurrentCanvas();
                PdfResources resources = cv.GetResources();
                NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
                PdfDictionary resDic = (PdfDictionary)resources.GetResourceObject(PdfName.ExtGState, DEFAULT_RESOURCE_NAME
                    );
                NUnit.Framework.Assert.AreEqual(2, resDic.Size());
                NUnit.Framework.Assert.AreEqual(resDic.Get(FILL_OPAC), new PdfNumber(0.75));
                NUnit.Framework.Assert.AreEqual(resDic.Get(STROKE_OPAC), new PdfNumber(0.75));
            }
        }

        private SvgDrawContext SetupDrawContext(PdfDocument pdfDocument) {
            SvgDrawContext sdc = new SvgDrawContext(new ResourceResolver(""), new FontProvider());
            PdfCanvas cv = new PdfCanvas(pdfDocument.AddNewPage());
            sdc.PushCanvas(cv);
            return sdc;
        }
    }
}
