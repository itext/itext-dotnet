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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Responsible for drawing Images to the canvas.</summary>
    /// <remarks>
    /// Responsible for drawing Images to the canvas.
    /// Referenced SVG images aren't supported yet. TODO DEVSIX-2277
    /// </remarks>
    public class ImageSvgNodeRenderer : AbstractSvgNodeRenderer {
        public override ISvgNodeRenderer CreateDeepCopy() {
            ImageSvgNodeRenderer copy = new ImageSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            ResourceResolver resourceResolver = context.GetResourceResolver();
            if (resourceResolver == null || this.attributesAndStyles == null) {
                return;
            }
            String uri = this.attributesAndStyles.Get(SvgConstants.Attributes.XLINK_HREF);
            PdfXObject xObject = resourceResolver.RetrieveImage(uri);
            if (xObject == null) {
                return;
            }
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            float x = 0;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                x = ParseHorizontalLength(attributesAndStyles.Get(SvgConstants.Attributes.X), context);
            }
            float y = 0;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                y = ParseVerticalLength(attributesAndStyles.Get(SvgConstants.Attributes.Y), context);
            }
            float width;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.WIDTH)) {
                width = ParseHorizontalLength(attributesAndStyles.Get(SvgConstants.Attributes.WIDTH), context);
            }
            else {
                width = xObject.GetWidth();
            }
            float height;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.HEIGHT)) {
                height = ParseVerticalLength(attributesAndStyles.Get(SvgConstants.Attributes.HEIGHT), context);
            }
            else {
                height = xObject.GetHeight();
            }
            String preserveAspectRatio = "";
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO)) {
                preserveAspectRatio = attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO);
            }
            else {
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO.ToLowerInvariant())) {
                    // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
                    preserveAspectRatio = attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO.ToLowerInvariant
                        ());
                }
            }
            preserveAspectRatio = preserveAspectRatio.ToLowerInvariant();
            if (!SvgConstants.Values.NONE.Equals(preserveAspectRatio) && !(width == 0 || height == 0)) {
                float normalizedWidth;
                float normalizedHeight;
                if (xObject.GetWidth() / width > xObject.GetHeight() / height) {
                    normalizedWidth = width;
                    normalizedHeight = xObject.GetHeight() / xObject.GetWidth() * width;
                }
                else {
                    normalizedWidth = xObject.GetWidth() / xObject.GetHeight() * height;
                    normalizedHeight = height;
                }
                switch (preserveAspectRatio.ToLowerInvariant()) {
                    case SvgConstants.Values.XMIN_YMIN: {
                        break;
                    }

                    case SvgConstants.Values.XMIN_YMID: {
                        y += Math.Abs(normalizedHeight - height) / 2;
                        break;
                    }

                    case SvgConstants.Values.XMIN_YMAX: {
                        y += Math.Abs(normalizedHeight - height);
                        break;
                    }

                    case SvgConstants.Values.XMID_YMIN: {
                        x += Math.Abs(normalizedWidth - width) / 2;
                        break;
                    }

                    case SvgConstants.Values.XMID_YMAX: {
                        x += Math.Abs(normalizedWidth - width) / 2;
                        y += Math.Abs(normalizedHeight - height);
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMIN: {
                        x += Math.Abs(normalizedWidth - width);
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMID: {
                        x += Math.Abs(normalizedWidth - width);
                        y += Math.Abs(normalizedHeight - height) / 2;
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMAX: {
                        x += Math.Abs(normalizedWidth - width);
                        y += Math.Abs(normalizedHeight - height);
                        break;
                    }

                    case SvgConstants.Values.DEFAULT_ASPECT_RATIO:
                    default: {
                        x += Math.Abs(normalizedWidth - width) / 2;
                        y += Math.Abs(normalizedHeight - height) / 2;
                        break;
                    }
                }
                width = normalizedWidth;
                height = normalizedHeight;
            }
            float v = y + height;
            currentCanvas.AddXObjectWithTransformationMatrix(xObject, width, 0, 0, -height, x, v);
        }
    }
}
