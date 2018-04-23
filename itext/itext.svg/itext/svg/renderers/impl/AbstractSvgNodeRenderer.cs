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
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary><inheritDoc/></summary>
    public abstract class AbstractSvgNodeRenderer : ISvgNodeRenderer {
        private ISvgNodeRenderer parent;

        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        protected internal IDictionary<String, String> attributesAndStyles;

        private bool doFill = false;

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public void AddChild(ISvgNodeRenderer child) {
            // final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgNodeRenderer> GetChildren() {
            // final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
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
                String transformString = this.attributesAndStyles.Get(SvgTagConstants.TRANSFORM);
                if (transformString != null && !String.IsNullOrEmpty(transformString)) {
                    AffineTransform transformation = TransformUtils.ParseTransform(transformString);
                    currentCanvas.ConcatMatrix(transformation);
                }
            }
            PreDraw(context);
            DoDraw(context);
            PostDraw(context);
            if (attributesAndStyles != null && attributesAndStyles.ContainsKey(SvgTagConstants.ID)) {
                context.AddNamedObject(attributesAndStyles.Get(SvgTagConstants.ID), this);
            }
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
                    String fillRawValue = GetAttribute(SvgTagConstants.FILL);
                    this.doFill = !SvgTagConstants.NONE.EqualsIgnoreCase(fillRawValue);
                    if (doFill && CanElementFill()) {
                        // todo RND-865 default style sheets
                        Color color = ColorConstants.BLACK;
                        if (fillRawValue != null) {
                            color = WebColors.GetRGBColor(fillRawValue);
                        }
                        currentCanvas.SetFillColor(color);
                    }
                }
 {
                    // stroke
                    String strokeRawValue = GetAttribute(SvgTagConstants.STROKE);
                    DeviceRgb rgbColor = WebColors.GetRGBColor(strokeRawValue);
                    if (strokeRawValue != null && rgbColor != null) {
                        currentCanvas.SetStrokeColor(rgbColor);
                        String strokeWidthRawValue = GetAttribute(SvgTagConstants.STROKE_WIDTH);
                        float strokeWidth = 1f;
                        if (strokeWidthRawValue != null) {
                            strokeWidth = CssUtils.ParseAbsoluteLength(strokeWidthRawValue);
                        }
                        currentCanvas.SetLineWidth(strokeWidth);
                    }
                }
            }
        }

        protected internal virtual bool CanElementFill() {
            return true;
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
                    String fillRuleRawValue = GetAttribute(SvgTagConstants.FILL_RULE);
                    if (SvgTagConstants.FILL_RULE_EVEN_ODD.EqualsIgnoreCase(fillRuleRawValue)) {
                        // TODO RND-878
                        currentCanvas.EoFill();
                    }
                    else {
                        currentCanvas.Fill();
                    }
                }
                if (GetAttribute(SvgTagConstants.STROKE) != null) {
                    currentCanvas.Stroke();
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
            this.attributesAndStyles.Put(key, value);
        }
    }
}
