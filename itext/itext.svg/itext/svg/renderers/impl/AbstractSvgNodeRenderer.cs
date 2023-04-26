/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;
using iText.Svg;
using iText.Svg.Css.Impl;
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

        /// <summary>Map that contains attributes and styles used for drawing operations.</summary>
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

        /// <summary>
        /// Retrieves the property value for a given key name or default if the property value is
        /// <see langword="null"/>
        /// or missing.
        /// </summary>
        /// <param name="key">the name of the property to search for</param>
        /// <param name="defaultValue">
        /// the default value to be returned if the property is
        /// <see langword="null"/>
        /// or missing
        /// </param>
        /// <returns>
        /// the value for this key, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        public virtual String GetAttributeOrDefault(String key, String defaultValue) {
            String rawValue = GetAttribute(key);
            return rawValue != null ? rawValue : defaultValue;
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

        /// <summary>Return font-size of the current element</summary>
        /// <returns>absolute value of font-size</returns>
        public virtual float GetCurrentFontSize() {
            return CssDimensionParsingUtils.ParseAbsoluteFontSize(GetAttribute(SvgConstants.Attributes.FONT_SIZE));
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

        /// <summary>Calculate the transformation for the viewport based on the context.</summary>
        /// <remarks>
        /// Calculate the transformation for the viewport based on the context. Only used by elements that can create
        /// viewports
        /// </remarks>
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
                    if (!(this is ISvgTextNodeRenderer)) {
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
                        String fillRawValue = GetAttributeOrDefault(SvgConstants.Attributes.FILL, "black");
                        this.doFill = !SvgConstants.Values.NONE.EqualsIgnoreCase(fillRawValue);
                        if (doFill && CanElementFill()) {
                            float fillOpacity = GetOpacityByAttributeName(SvgConstants.Attributes.FILL_OPACITY, generalOpacity);
                            Color fillColor = null;
                            TransparentColor transparentColor = GetColorFromAttributeValue(context, fillRawValue, 0, fillOpacity);
                            if (transparentColor != null) {
                                fillColor = transparentColor.GetColor();
                                fillOpacity = transparentColor.GetOpacity();
                            }
                            if (!CssUtils.CompareFloats(fillOpacity, 1f)) {
                                opacityGraphicsState.SetFillOpacity(fillOpacity);
                            }
                            // set default if no color has been parsed
                            if (fillColor == null) {
                                fillColor = ColorConstants.BLACK;
                            }
                            currentCanvas.SetFillColor(fillColor);
                        }
                    }
 {
                        // stroke
                        String strokeRawValue = GetAttributeOrDefault(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
                        if (!SvgConstants.Values.NONE.EqualsIgnoreCase(strokeRawValue)) {
                            String strokeWidthRawValue = GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                            // 1 px = 0,75 pt
                            float strokeWidth = 0.75f;
                            if (strokeWidthRawValue != null) {
                                strokeWidth = CssDimensionParsingUtils.ParseAbsoluteLength(strokeWidthRawValue);
                            }
                            float strokeOpacity = GetOpacityByAttributeName(SvgConstants.Attributes.STROKE_OPACITY, generalOpacity);
                            Color strokeColor = null;
                            TransparentColor transparentColor = GetColorFromAttributeValue(context, strokeRawValue, (float)((double)strokeWidth
                                 / 2.0), strokeOpacity);
                            if (transparentColor != null) {
                                strokeColor = transparentColor.GetColor();
                                strokeOpacity = transparentColor.GetOpacity();
                            }
                            if (!CssUtils.CompareFloats(strokeOpacity, 1f)) {
                                opacityGraphicsState.SetStrokeOpacity(strokeOpacity);
                            }
                            // as default value for stroke is 'none' we should not set
                            // it in case when value obtaining fails
                            if (strokeColor != null) {
                                currentCanvas.SetStrokeColor(strokeColor);
                            }
                            currentCanvas.SetLineWidth(strokeWidth);
                            doStroke = true;
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

        /// <summary>Parse absolute length.</summary>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// for parsing
        /// </param>
        /// <param name="percentRelativeValue">the value on which percent length is based on</param>
        /// <param name="defaultValue">default value if length is not recognized</param>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// </param>
        /// <returns>absolute value in points</returns>
        protected internal virtual float ParseAbsoluteLength(String length, float percentRelativeValue, float defaultValue
            , SvgDrawContext context) {
            if (CssTypesValidationUtils.IsPercentageValue(length)) {
                return CssDimensionParsingUtils.ParseRelativeValue(length, percentRelativeValue);
            }
            else {
                float em = GetCurrentFontSize();
                float rem = context.GetCssContext().GetRootFontSize();
                UnitValue unitValue = CssDimensionParsingUtils.ParseLengthValueToPt(length, em, rem);
                if (unitValue != null && unitValue.IsPointValue()) {
                    return unitValue.GetValue();
                }
                else {
                    return defaultValue;
                }
            }
        }

        private TransparentColor GetColorFromAttributeValue(SvgDrawContext context, String rawColorValue, float objectBoundingBoxMargin
            , float parentOpacity) {
            if (rawColorValue == null) {
                return null;
            }
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(rawColorValue);
            CssDeclarationValueTokenizer.Token token = tokenizer.GetNextValidToken();
            if (token == null) {
                return null;
            }
            String tokenValue = token.GetValue();
            if (tokenValue.StartsWith("url(#") && tokenValue.EndsWith(")")) {
                Color resolvedColor = null;
                float resolvedOpacity = 1;
                String normalizedName = tokenValue.JSubstring(5, tokenValue.Length - 1).Trim();
                ISvgNodeRenderer colorRenderer = context.GetNamedObject(normalizedName);
                if (colorRenderer is ISvgPaintServer) {
                    resolvedColor = ((ISvgPaintServer)colorRenderer).CreateColor(context, GetObjectBoundingBox(context), objectBoundingBoxMargin
                        , parentOpacity);
                }
                if (resolvedColor != null) {
                    return new TransparentColor(resolvedColor, resolvedOpacity);
                }
                token = tokenizer.GetNextValidToken();
            }
            // may become null after function parsing and reading the 2nd token
            if (token != null) {
                String value = token.GetValue();
                if (!SvgConstants.Values.NONE.EqualsIgnoreCase(value)) {
                    if (!CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants.COLOR, value))) {
                        return new TransparentColor(new DeviceRgb(0.0f, 0.0f, 0.0f), 1.0f);
                    }
                    TransparentColor result = CssDimensionParsingUtils.ParseColor(value);
                    return new TransparentColor(result.GetColor(), result.GetOpacity() * parentOpacity);
                }
            }
            return null;
        }

        private float GetOpacityByAttributeName(String attributeName, float generalOpacity) {
            float opacity = generalOpacity;
            String opacityValue = GetAttribute(attributeName);
            if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                opacity *= float.Parse(opacityValue, System.Globalization.CultureInfo.InvariantCulture);
            }
            return opacity;
        }

        private bool DrawInClipPath(SvgDrawContext context) {
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.CLIP_PATH)) {
                String clipPathName = attributesAndStyles.Get(SvgConstants.Attributes.CLIP_PATH);
                ISvgNodeRenderer template = context.GetNamedObject(NormalizeLocalUrlName(clipPathName));
                //Clone template to avoid muddying the state
                if (template is ClipPathSvgNodeRenderer) {
                    ClipPathSvgNodeRenderer clipPath = (ClipPathSvgNodeRenderer)template.CreateDeepCopy();
                    // Resolve parent inheritance
                    SvgNodeRendererInheritanceResolver.ApplyInheritanceToSubTree(this, clipPath, context.GetCssContext());
                    clipPath.SetClippedRenderer(this);
                    clipPath.Draw(context);
                    return !clipPath.GetChildren().IsEmpty();
                }
            }
            return false;
        }

        private String NormalizeLocalUrlName(String name) {
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

        public abstract Rectangle GetObjectBoundingBox(SvgDrawContext arg1);
    }
}
