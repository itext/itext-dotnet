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
using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css.Util;
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
                x = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.X));
            }
            float y = 0;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                y = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.Y));
            }
            float width = 0;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.WIDTH)) {
                width = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.WIDTH
                    ));
            }
            float height = 0;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.HEIGHT)) {
                height = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.HEIGHT
                    ));
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
