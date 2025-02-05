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
using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

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
            String uri = this.attributesAndStyles.Get(SvgConstants.Attributes.HREF);
            if (uri == null) {
                uri = this.attributesAndStyles.Get(SvgConstants.Attributes.XLINK_HREF);
            }
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
            float width = -1;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.WIDTH)) {
                width = ParseHorizontalLength(attributesAndStyles.Get(SvgConstants.Attributes.WIDTH), context);
            }
            if (width < 0) {
                width = CssUtils.ConvertPxToPts(xObject.GetWidth());
            }
            float height = -1;
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.HEIGHT)) {
                height = ParseVerticalLength(attributesAndStyles.Get(SvgConstants.Attributes.HEIGHT), context);
            }
            if (height < 0) {
                height = CssUtils.ConvertPxToPts(xObject.GetHeight());
            }
            if (width != 0 && height != 0) {
                String[] alignAndMeet = RetrieveAlignAndMeet();
                String align = alignAndMeet[0];
                String meetOrSlice = alignAndMeet[1];
                Rectangle currentViewPort = new Rectangle(0, 0, width, height);
                Rectangle viewBox = new Rectangle(0, 0, xObject.GetWidth(), xObject.GetHeight());
                Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(viewBox, currentViewPort, align, meetOrSlice);
                float scaleWidth = appliedViewBox.GetWidth() / viewBox.GetWidth();
                float scaleHeight = appliedViewBox.GetHeight() / viewBox.GetHeight();
                float xOffset = appliedViewBox.GetX() / scaleWidth - viewBox.GetX();
                float yOffset = appliedViewBox.GetY() / scaleHeight - viewBox.GetY();
                x += xOffset;
                y += yOffset;
                width = appliedViewBox.GetWidth();
                height = appliedViewBox.GetHeight();
                if (SvgConstants.Values.SLICE.Equals(meetOrSlice)) {
                    currentCanvas.SaveState().Rectangle(currentViewPort).Clip().EndPath().AddXObjectWithTransformationMatrix(xObject
                        , width, 0, 0, -height, x, y + height).RestoreState();
                    return;
                }
            }
            currentCanvas.AddXObjectWithTransformationMatrix(xObject, width, 0, 0, -height, x, y + height);
        }
    }
}
