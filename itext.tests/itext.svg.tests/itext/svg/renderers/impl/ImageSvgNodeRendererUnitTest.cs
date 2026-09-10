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
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageSvgNodeRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            ImageSvgNodeRenderer renderer = new ImageSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroSizedViewBoxDoesNotProduceExceptionTest() {
            PdfFormXObject zeroSizedXObject = new PdfFormXObject(new Rectangle(0, 0, 0, 0));
            ResourceResolver resourceResolver = new _ResourceResolver_57(zeroSizedXObject, "");
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
                //should not throw a when view box is not existing
                renderer.DoDraw(context);
                String contentStream = iText.Commons.Utils.JavaUtil.GetStringForBytes(canvas.GetContentStream().GetBytes()
                    , System.Text.Encoding.UTF8);
                // 100px x 50px converted to points.
                NUnit.Framework.Assert.IsTrue(contentStream.Contains("75 0 0 -37.5 0 37.5 cm"));
            }
        }

        private sealed class _ResourceResolver_57 : ResourceResolver {
            public _ResourceResolver_57(PdfFormXObject zeroSizedXObject, String baseArg1)
                : base(baseArg1) {
                this.zeroSizedXObject = zeroSizedXObject;
            }

            public override PdfXObject RetrieveImage(String src) {
                return zeroSizedXObject;
            }

            private readonly PdfFormXObject zeroSizedXObject;
        }
    }
}
