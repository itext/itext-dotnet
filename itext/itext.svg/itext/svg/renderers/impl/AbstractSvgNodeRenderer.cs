/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Pdf.Extgstate;
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
        // TODO (DEVSIX-3397) Add MarkerVertexType.MARKER_MID after ticket will be finished.
        private static readonly MarkerVertexType[] MARKER_VERTEX_TYPES = new MarkerVertexType[] { MarkerVertexType
            .MARKER_START, MarkerVertexType.MARKER_END };

        /// <summary>Map that contains attributes and styles used for drawing operations</summary>
        protected internal IDictionary<String, String> attributesAndStyles;

        internal bool partOfClipPath;

        internal bool doFill = false;

        internal bool doStroke = false;

        private ISvgNodeRenderer parent;

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
            this.attributesAndStyles = attributesAndStyles;
        }

        public virtual String GetAttribute(String key) {
            return attributesAndStyles.Get(key);
        }

        public virtual void SetAttribute(String key, String value) {
            if (this.attributesAndStyles == null) {
                this.attributesAndStyles = new Dictionary<String, String>();
            }
            this.attributesAndStyles.Put(key, value);
        }

        public virtual IDictionary<String, String> GetAttributeMapCopy() {
            Dictionary<String, String> copy = new Dictionary<String, String>();
            if (attributesAndStyles == null) {
                return copy;
            }
            copy.AddAll(attributesAndStyles);
            return copy;
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
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.ID)) {
                    context.AddUsedId(attributesAndStyles.Get(SvgConstants.Attributes.ID));
                }
            }
            /* If a (non-empty) clipping path exists, drawing operations must be surrounded by q/Q operators
            and may have to be drawn multiple times
            */
            if (!DrawInClipPath(context)) {
                PreDraw(context);
                DoDraw(context);
                PostDraw(context);
            }
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.ID)) {
                context.RemoveUsedId(attributesAndStyles.Get(SvgConstants.Attributes.ID));
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

        /// <summary>
        /// Make a deep copy of the styles and attributes of this renderer
        /// Helper method for deep copying logic
        /// </summary>
        /// <param name="deepCopy">renderer to insert the deep copied attributes into</param>
        protected internal virtual void DeepCopyAttributesAndStyles(ISvgNodeRenderer deepCopy) {
            IDictionary<String, String> stylesDeepCopy = new Dictionary<String, String>();
            if (this.attributesAndStyles != null) {
                stylesDeepCopy.AddAll(this.attributesAndStyles);
                deepCopy.SetAttributesAndStyles(stylesDeepCopy);
            }
        }

        /// <summary>Draws this element to a canvas-like object maintained in the context.</summary>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        protected internal abstract void DoDraw(SvgDrawContext context);

        internal static float GetAlphaFromRGBA(String value) {
            try {
                return WebColors.GetRGBAColor(value)[3];
            }
            catch (Exception) {
                return 1f;
            }
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
                if (partOfClipPath) {
                    if (SvgConstants.Values.FILL_RULE_EVEN_ODD.EqualsIgnoreCase(this.GetAttribute(SvgConstants.Attributes.CLIP_RULE
                        ))) {
                        currentCanvas.EoClip();
                    }
                    else {
                        currentCanvas.Clip();
                    }
                    currentCanvas.EndPath();
                }
                else {
                    if (doFill && CanElementFill()) {
                        String fillRuleRawValue = GetAttribute(SvgConstants.Attributes.FILL_RULE);
                        if (SvgConstants.Values.FILL_RULE_EVEN_ODD.EqualsIgnoreCase(fillRuleRawValue)) {
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
                        else {
                            if (!typeof(TextSvgBranchRenderer).IsInstanceOfType(this)) {
                                currentCanvas.EndPath();
                            }
                        }
                    }
                }
                // Marker drawing
                if (this is IMarkerCapable) {
                    // TODO (DEVSIX-3397) add processing of 'marker' property (shorthand for a joint using of all other properties)
                    foreach (MarkerVertexType markerVertexType in MARKER_VERTEX_TYPES) {
                        if (attributesAndStyles.ContainsKey(markerVertexType.ToString())) {
                            currentCanvas.SaveState();
                            ((IMarkerCapable)this).DrawMarker(context, markerVertexType);
                            currentCanvas.RestoreState();
                        }
                    }
                }
            }
        }

        internal virtual void SetPartOfClipPath(bool value) {
            partOfClipPath = value;
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
                PdfExtGState opacityGraphicsState = new PdfExtGState();
                if (!partOfClipPath) {
                    float generalOpacity = GetOpacity();
 {
                        // fill
                        String fillRawValue = GetAttribute(SvgConstants.Attributes.FILL);
                        this.doFill = !SvgConstants.Values.NONE.EqualsIgnoreCase(fillRawValue);
                        if (doFill && CanElementFill()) {
                            Color rgbColor = ColorConstants.BLACK;
                            float fillOpacity = generalOpacity;
                            String opacityValue = GetAttribute(SvgConstants.Attributes.FILL_OPACITY);
                            if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                                fillOpacity *= float.Parse(opacityValue, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (fillRawValue != null) {
                                fillOpacity *= GetAlphaFromRGBA(fillRawValue);
                                rgbColor = WebColors.GetRGBColor(fillRawValue);
                            }
                            if (!CssUtils.CompareFloats(fillOpacity, 1f)) {
                                opacityGraphicsState.SetFillOpacity(fillOpacity);
                            }
                            currentCanvas.SetFillColor(rgbColor);
                        }
                    }
 {
                        // stroke
                        String strokeRawValue = GetAttribute(SvgConstants.Attributes.STROKE);
                        if (!SvgConstants.Values.NONE.EqualsIgnoreCase(strokeRawValue)) {
                            if (strokeRawValue != null) {
                                Color rgbColor = WebColors.GetRGBColor(strokeRawValue);
                                float strokeOpacity = generalOpacity;
                                String opacityValue = GetAttribute(SvgConstants.Attributes.STROKE_OPACITY);
                                if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                                    strokeOpacity *= float.Parse(opacityValue, System.Globalization.CultureInfo.InvariantCulture);
                                }
                                strokeOpacity *= GetAlphaFromRGBA(strokeRawValue);
                                if (!CssUtils.CompareFloats(strokeOpacity, 1f)) {
                                    opacityGraphicsState.SetStrokeOpacity(strokeOpacity);
                                }
                                currentCanvas.SetStrokeColor(rgbColor);
                                String strokeWidthRawValue = GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                                // 1 px = 0,75 pt
                                float strokeWidth = 0.75f;
                                if (strokeWidthRawValue != null) {
                                    strokeWidth = CssUtils.ParseAbsoluteLength(strokeWidthRawValue);
                                }
                                currentCanvas.SetLineWidth(strokeWidth);
                                doStroke = true;
                            }
                        }
                    }
 {
                        // opacity
                        if (!opacityGraphicsState.GetPdfObject().IsEmpty()) {
                            currentCanvas.SetExtGState(opacityGraphicsState);
                        }
                    }
                }
            }
        }

        private bool DrawInClipPath(SvgDrawContext context) {
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.CLIP_PATH)) {
                String clipPathName = attributesAndStyles.Get(SvgConstants.Attributes.CLIP_PATH);
                ISvgNodeRenderer template = context.GetNamedObject(NormalizeClipPathName(clipPathName));
                //Clone template to avoid muddying the state
                if (template is ClipPathSvgNodeRenderer) {
                    ClipPathSvgNodeRenderer clipPath = (ClipPathSvgNodeRenderer)template.CreateDeepCopy();
                    clipPath.SetClippedRenderer(this);
                    clipPath.Draw(context);
                    return !clipPath.GetChildren().IsEmpty();
                }
            }
            return false;
        }

        private String NormalizeClipPathName(String name) {
            return name.Replace("url(#", "").Replace(")", "").Trim();
        }

        private float GetOpacity() {
            float result = 1f;
            String opacityValue = GetAttribute(SvgConstants.Attributes.OPACITY);
            if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                result = float.Parse(opacityValue, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (parent != null && parent is AbstractSvgNodeRenderer) {
                result *= ((AbstractSvgNodeRenderer)parent).GetOpacity();
            }
            return result;
        }

        public abstract ISvgNodeRenderer CreateDeepCopy();
    }
}
