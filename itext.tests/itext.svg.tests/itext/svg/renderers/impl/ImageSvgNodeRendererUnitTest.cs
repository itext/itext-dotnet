/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Svg.Xobject;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageSvgNodeRendererUnitTest : ExtendedITextTest {
        private const float EPSILON = 0.00001f;

        private const String IMAGE_DATA = "data:image/png;base64," + "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";

        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            ImageSvgNodeRenderer renderer = new ImageSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroSizedViewBoxDoesNotProduceExceptionTest() {
            PdfFormXObject zeroSizedXObject = new PdfFormXObject(new Rectangle(0, 0, 0, 0));
            ResourceResolver resourceResolver = new _ResourceResolver_66(zeroSizedXObject, "");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument document = new PdfDocument(new PdfWriter(baos))) {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                SvgDrawContext context = new SvgDrawContext(resourceResolver, null);
                context.AddViewPort(new Rectangle(0, 0, 500, 500));
                context.PushCanvas(canvas);
                IDictionary<String, String> attributes = new ConcurrentDictionary<String, String>();
                attributes.Put(SvgConstants.Attributes.HREF, "any.png");
                attributes.Put(SvgConstants.Attributes.WIDTH, "100");
                attributes.Put(SvgConstants.Attributes.HEIGHT, "50");
                ImageSvgNodeRenderer renderer = new ImageSvgNodeRenderer();
                renderer.SetAttributesAndStyles(attributes);
                // Should not throw when a view box does not exist
                renderer.DoDraw(context);
                String contentStream = iText.Commons.Utils.JavaUtil.GetStringForBytes(canvas.GetContentStream().GetBytes()
                    , System.Text.Encoding.UTF8);
                // 100px x 50px converted to points
                NUnit.Framework.Assert.IsTrue(contentStream.Contains("75 0 0 -37.5 0 37.5 cm"));
            }
        }

        private sealed class _ResourceResolver_66 : ResourceResolver {
            public _ResourceResolver_66(PdfFormXObject zeroSizedXObject, String baseArg1)
                : base(baseArg1) {
                this.zeroSizedXObject = zeroSizedXObject;
            }

            public override PdfXObject RetrieveImage(String src) {
                return zeroSizedXObject;
            }

            private readonly PdfFormXObject zeroSizedXObject;
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxTest() {
            ImageSvgNodeRenderer renderer = new ImageSvgNodeRenderer();
            renderer.SetAttribute(SvgConstants.Attributes.HREF, IMAGE_DATA);
            renderer.SetAttribute(SvgConstants.Attributes.X, "10");
            renderer.SetAttribute(SvgConstants.Attributes.Y, "20");
            renderer.SetAttribute(SvgConstants.Attributes.WIDTH, "40");
            renderer.SetAttribute(SvgConstants.Attributes.HEIGHT, "30");
            renderer.SetAttribute(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO, SvgConstants.Values.NONE);
            SvgDrawContext context = new SvgDrawContext(null, null);
            context.AddViewPort(new Rectangle(0, 0, 200, 200));
            Rectangle objectBoundingBox = renderer.GetObjectBoundingBox(context);
            NUnit.Framework.Assert.IsNotNull(objectBoundingBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(7.5f, 15f, 30f, 22.5f).EqualsWithEpsilon(objectBoundingBox, EPSILON
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroSizedSvgImageXObjectUpdatesBBoxTest() {
            SvgImageXObject zeroSizedXObject = new SvgImageXObject(null, new ImageSvgNodeRendererUnitTest.TestSvgProcessorResult
                (), new ResourceResolver(""));
            ResourceResolver resourceResolver = new _ResourceResolver_121(zeroSizedXObject, "");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument document = new PdfDocument(new PdfWriter(baos))) {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                SvgDrawContext context = new SvgDrawContext(resourceResolver, null);
                context.AddViewPort(new Rectangle(0, 0, 500, 500));
                context.PushCanvas(canvas);
                IDictionary<String, String> attributes = new ConcurrentDictionary<String, String>();
                attributes.Put(SvgConstants.Attributes.HREF, "any.svg");
                attributes.Put(SvgConstants.Attributes.WIDTH, "100");
                attributes.Put(SvgConstants.Attributes.HEIGHT, "50");
                ImageSvgNodeRenderer renderer = new ImageSvgNodeRenderer();
                renderer.SetAttributesAndStyles(attributes);
                renderer.DoDraw(context);
                NUnit.Framework.Assert.IsNotNull(zeroSizedXObject.GetBBox());
                // 100px x 50px converted to points
                NUnit.Framework.Assert.IsTrue(new Rectangle(0, 0, 75, 37.5f).EqualsWithEpsilon(zeroSizedXObject.GetBBox().
                    ToRectangle()));
            }
        }

        private sealed class _ResourceResolver_121 : ResourceResolver {
            public _ResourceResolver_121(SvgImageXObject zeroSizedXObject, String baseArg1)
                : base(baseArg1) {
                this.zeroSizedXObject = zeroSizedXObject;
            }

            public override PdfXObject RetrieveImage(String src) {
                return zeroSizedXObject;
            }

            private readonly SvgImageXObject zeroSizedXObject;
        }

        private class TestSvgProcessorResult : ISvgProcessorResult {
            public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
                return null;
            }

            public virtual ISvgNodeRenderer GetRootRenderer() {
                return null;
            }

            public virtual FontProvider GetFontProvider() {
                return null;
            }

            public virtual FontSet GetTempFonts() {
                return null;
            }
        }
    }
}
