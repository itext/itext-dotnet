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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Root renderer responsible for applying the initial axis-flipping transform</summary>
    public class PdfRootSvgNodeRenderer : ISvgNodeRenderer {
//\cond DO_NOT_DOCUMENT
        internal ISvgNodeRenderer subTreeRoot;
//\endcond

        /// <summary>
        /// Creates a
        /// <see cref="PdfRootSvgNodeRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="subTreeRoot">root of the subtree</param>
        public PdfRootSvgNodeRenderer(ISvgNodeRenderer subTreeRoot) {
            this.subTreeRoot = subTreeRoot;
            subTreeRoot.SetParent(this);
        }

        public virtual void SetParent(ISvgNodeRenderer parent) {
        }

        // Do nothing because it is root
        public virtual ISvgNodeRenderer GetParent() {
            return null;
        }

        public virtual void Draw(SvgDrawContext context) {
            //Set viewport and transformation for pdf-context
            context.AddViewPort(CalculateViewPort(context));
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            currentCanvas.ConcatMatrix(this.CalculateTransformation(context));
            currentCanvas.WriteLiteral("% svg root\n");
            ApplyBackgroundColor(context);
            subTreeRoot.Draw(context);
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
        }

        public virtual String GetAttribute(String key) {
            return null;
        }

        public virtual void SetAttribute(String key, String value) {
        }

        public virtual IDictionary<String, String> GetAttributeMapCopy() {
            return null;
        }

        public virtual Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual AffineTransform CalculateTransformation(SvgDrawContext context) {
            Rectangle viewPort = context.GetCurrentViewPort();
            float horizontal = viewPort.GetX();
            float vertical = viewPort.GetY() + viewPort.GetHeight();
            // flip coordinate space vertically and translate on the y axis with the viewport height
            AffineTransform transform = AffineTransform.GetTranslateInstance(0, 0);
            //Identity-transform
            transform.Concatenate(AffineTransform.GetTranslateInstance(horizontal, vertical));
            transform.Concatenate(new AffineTransform(1, 0, 0, -1, 0, 0));
            return transform;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static Rectangle CalculateViewPort(SvgDrawContext context) {
            float portX = 0f;
            float portY = 0f;
            float portWidth = 0f;
            float portHeight = 0f;
            PdfStream contentStream = context.GetCurrentCanvas().GetContentStream();
            if (!contentStream.ContainsKey(PdfName.BBox)) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.ROOT_SVG_NO_BBOX);
            }
            PdfArray bboxArray = contentStream.GetAsArray(PdfName.BBox);
            portX = bboxArray.GetAsNumber(0).FloatValue();
            portY = bboxArray.GetAsNumber(1).FloatValue();
            portWidth = bboxArray.GetAsNumber(2).FloatValue() - portX;
            portHeight = bboxArray.GetAsNumber(3).FloatValue() - portY;
            return new Rectangle(portX, portY, portWidth, portHeight);
        }
//\endcond

        public virtual ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.PdfRootSvgNodeRenderer copy = new iText.Svg.Renderers.Impl.PdfRootSvgNodeRenderer
                (subTreeRoot.CreateDeepCopy());
            return copy;
        }

        private void ApplyBackgroundColor(SvgDrawContext context) {
            String backgroundColorStr = subTreeRoot.GetAttribute(CommonCssConstants.BACKGROUND_COLOR);
            if (backgroundColorStr != null && !CommonCssConstants.TRANSPARENT.Equals(backgroundColorStr)) {
                Rectangle backgroundArea = context.GetCurrentViewPort();
                //Since we don't have info about margins/paddings/borders, background box can't work correctly, so we don't
                //count for it when applying background color in svg
                TransparentColor color = CssDimensionParsingUtils.ParseColor(backgroundColorStr);
                context.GetCurrentCanvas().SaveState().SetFillColor(color.GetColor());
                color.ApplyFillTransparency(context.GetCurrentCanvas());
                context.GetCurrentCanvas().Rectangle((double)backgroundArea.GetX(), (double)backgroundArea.GetY(), (double
                    )backgroundArea.GetWidth(), (double)backgroundArea.GetHeight()).Fill().RestoreState();
            }
        }
    }
}
