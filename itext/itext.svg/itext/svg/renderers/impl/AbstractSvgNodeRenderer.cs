/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// abstract implementation.
    /// </summary>
    public abstract class AbstractSvgNodeRenderer : ISvgNodeRenderer {
        private ISvgNodeRenderer parent;

        /// <summary>Map that contains attributes and styles used for drawing operations</summary>
        protected internal IDictionary<String, String> attributesAndStyles;

        private bool doFill = false;

        private bool doStroke = false;

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
            this.attributesAndStyles = attributesAndStyles;
        }

        /// <summary>
        /// Applies transformations set to this object, if any, and delegates the drawing of this element and its children
        /// to the
        /// <see cref="DoDraw(iText.Svg.Renderers.SvgDrawContext)">doDraw</see>
        /// method.
        /// </summary>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        public void Draw(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            if (this.attributesAndStyles != null) {
                String transformString = this.attributesAndStyles.Get(SvgConstants.Attributes.TRANSFORM);
                if (transformString != null && !String.IsNullOrEmpty(transformString)) {
                    AffineTransform transformation = TransformUtils.ParseTransform(transformString);
                    if (!transformation.IsIdentity()) {
                        currentCanvas.ConcatMatrix(transformation);
                    }
                }
            }
            PreDraw(context);
            DoDraw(context);
            PostDraw(context);
        }

        /// <summary>Operations to perform before drawing an element.</summary>
        /// <remarks>
        /// Operations to perform before drawing an element.
        /// This includes setting stroke color and width, fill color.
        /// </remarks>
        /// <param name="context">the svg draw context</param>
        internal virtual void PreDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
 {
                    // fill
                    String fillRawValue = GetAttribute(SvgConstants.Attributes.FILL);
                    this.doFill = !SvgConstants.Values.NONE.EqualsIgnoreCase(fillRawValue);
                    if (doFill && CanElementFill()) {
                        Color color = ColorConstants.BLACK;
                        if (fillRawValue != null) {
                            color = WebColors.GetRGBColor(fillRawValue);
                        }
                        currentCanvas.SetFillColor(color);
                    }
                }
 {
                    // stroke
                    String strokeRawValue = GetAttribute(SvgConstants.Attributes.STROKE);
                    if (!SvgConstants.Values.NONE.EqualsIgnoreCase(strokeRawValue)) {
                        DeviceRgb rgbColor = WebColors.GetRGBColor(strokeRawValue);
                        if (strokeRawValue != null && rgbColor != null) {
                            currentCanvas.SetStrokeColor(rgbColor);
                            String strokeWidthRawValue = GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                            float strokeWidth = 1f;
                            if (strokeWidthRawValue != null) {
                                strokeWidth = CssUtils.ParseAbsoluteLength(strokeWidthRawValue);
                            }
                            currentCanvas.SetLineWidth(strokeWidth);
                            doStroke = true;
                        }
                    }
                }
            }
        }

        /// <summary>Method to see if a certain renderer can use fill.</summary>
        /// <returns>true if the renderer can use fill</returns>
        protected internal virtual bool CanElementFill() {
            return true;
        }

        /// <summary>Method to see if the renderer can create a viewport</summary>
        /// <returns>true if the renderer can construct a viewport</returns>
        public virtual bool CanConstructViewPort() {
            return false;
        }

        /// <summary>Calculate the transformation for the viewport based on the context.</summary>
        /// <remarks>Calculate the transformation for the viewport based on the context. Only used by elements that can create viewports
        ///     </remarks>
        /// <param name="context">the SVG draw context</param>
        /// <returns>the transformation that needs to be applied to this renderer</returns>
        internal virtual AffineTransform CalculateViewPortTranslation(SvgDrawContext context) {
            Rectangle viewPort = context.GetCurrentViewPort();
            AffineTransform transform;
            transform = AffineTransform.GetTranslateInstance(viewPort.GetX(), viewPort.GetY());
            return transform;
        }

        /// <summary>Operations to be performed after drawing the element.</summary>
        /// <remarks>
        /// Operations to be performed after drawing the element.
        /// This includes filling, stroking.
        /// </remarks>
        /// <param name="context">the svg draw context</param>
        internal virtual void PostDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                // fill-rule
                if (doFill && CanElementFill()) {
                    String fillRuleRawValue = GetAttribute(SvgConstants.Attributes.FILL_RULE);
                    if (SvgConstants.Values.FILL_RULE_EVEN_ODD.EqualsIgnoreCase(fillRuleRawValue)) {
                        // TODO RND-878
                        if (doStroke) {
                            currentCanvas.EoFillStroke();
                        }
                        else {
                            currentCanvas.EoFill();
                        }
                    }
                    else {
                        if (doStroke) {
                            currentCanvas.FillStroke();
                        }
                        else {
                            currentCanvas.Fill();
                        }
                    }
                }
                else {
                    if (doStroke) {
                        currentCanvas.Stroke();
                    }
                }
                currentCanvas.ClosePath();
            }
        }

        /// <summary>Draws this element to a canvas-like object maintained in the context.</summary>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        protected internal abstract void DoDraw(SvgDrawContext context);

        public virtual String GetAttribute(String key) {
            return attributesAndStyles.Get(key);
        }

        public virtual void SetAttribute(String key, String value) {
            if (this.attributesAndStyles == null) {
                this.attributesAndStyles = new Dictionary<String, String>();
            }
            this.attributesAndStyles.Put(key, value);
        }
    }
}
