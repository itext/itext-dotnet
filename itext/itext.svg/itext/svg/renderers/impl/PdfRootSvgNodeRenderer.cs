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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Root renderer responsible for applying the initial axis-flipping transform</summary>
    public class PdfRootSvgNodeRenderer : ISvgNodeRenderer {
        internal ISvgNodeRenderer subTreeRoot;

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

        // TODO DEVSIX-2283
        public virtual ISvgNodeRenderer GetParent() {
            // TODO DEVSIX-2283
            return null;
        }

        public virtual void Draw(SvgDrawContext context) {
            //Set viewport and transformation for pdf-context
            context.AddViewPort(this.CalculateViewPort(context));
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            currentCanvas.ConcatMatrix(this.CalculateTransformation(context));
            currentCanvas.WriteLiteral("% svg root\n");
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

        internal virtual Rectangle CalculateViewPort(SvgDrawContext context) {
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

        public virtual ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.PdfRootSvgNodeRenderer copy = new iText.Svg.Renderers.Impl.PdfRootSvgNodeRenderer
                (subTreeRoot.CreateDeepCopy());
            return copy;
        }
    }
}
