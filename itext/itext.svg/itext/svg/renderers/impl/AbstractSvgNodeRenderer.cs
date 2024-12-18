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
using iText.Svg.Css;
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

//\cond DO_NOT_DOCUMENT
        internal bool partOfClipPath;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool doFill = false;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool doStroke = false;
//\endcond

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

        /// <summary>Return font-size of the current element in px.</summary>
        /// <remarks>
        /// Return font-size of the current element in px.
        /// <para />
        /// This method is deprecated in favour of
        /// <see cref="GetCurrentFontSize(iText.Svg.Renderers.SvgDrawContext)"/>
        /// because
        /// current one can't support relative values (em, rem) and those can't be resolved without
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>.
        /// </remarks>
        /// <returns>absolute value of font-size</returns>
        [Obsolete]
        public virtual float GetCurrentFontSize() {
            return GetCurrentFontSize(new SvgDrawContext(null, null));
        }

        /// <summary>Return font-size of the current element in px.</summary>
        /// <param name="context">draw context from which root font size can be extracted</param>
        /// <returns>absolute value of font-size</returns>
        public virtual float GetCurrentFontSize(SvgDrawContext context) {
            String fontSizeAttribute = GetAttribute(SvgConstants.Attributes.FONT_SIZE);
            if (CssTypesValidationUtils.IsRemValue(fontSizeAttribute)) {
                return CssDimensionParsingUtils.ParseRelativeValue(fontSizeAttribute, context.GetCssContext().GetRootFontSize
                    ());
            }
            if (CssTypesValidationUtils.IsEmValue(fontSizeAttribute) && GetParent() != null && GetParent() is AbstractSvgNodeRenderer
                ) {
                return CssDimensionParsingUtils.ParseRelativeValue(fontSizeAttribute, ((AbstractSvgNodeRenderer)GetParent(
                    )).GetCurrentFontSize(context));
            }
            return CssDimensionParsingUtils.ParseAbsoluteFontSize(fontSizeAttribute);
        }

        /// <summary>Gets the viewbox from the first parent element which can define it.</summary>
        /// <remarks>
        /// Gets the viewbox from the first parent element which can define it.
        /// <para />
        /// See <a href="https://svgwg.org/svg2-draft/coords.html#establishinganewsvgviewport">SVG specification</a>
        /// to find which elements can define a viewbox.
        /// </remarks>
        /// <param name="context">draw context from which fallback viewbox can be extracted</param>
        /// <returns>
        /// the viewbox or
        /// <see langword="null"/>
        /// if the element doesn't have parent which can define the viewbox
        /// </returns>
        public virtual Rectangle GetCurrentViewBox(SvgDrawContext context) {
            // According to https://svgwg.org/svg2-draft/coords.html#EstablishingANewSVGViewport: "For historical reasons,
            // the ‘pattern’ and ‘marker’ elements do not create a new viewport, despite accepting a ‘viewBox’ attribute".
            // So get viewbox only from symbol and svg elements
            if (this is AbstractContainerSvgNodeRenderer) {
                float[] viewBoxValues = SvgCssUtils.ParseViewBox(this);
                if (viewBoxValues == null || viewBoxValues.Length < SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                    Rectangle currentViewPort = context.GetCurrentViewPort();
                    viewBoxValues = new float[] { 0, 0, currentViewPort.GetWidth(), currentViewPort.GetHeight() };
                }
                return new Rectangle(viewBoxValues[0], viewBoxValues[1], viewBoxValues[2], viewBoxValues[3]);
            }
            else {
                if (GetParent() is AbstractSvgNodeRenderer) {
                    return ((AbstractSvgNodeRenderer)GetParent()).GetCurrentViewBox(context);
                }
                else {
                    // From iText this line isn't reachable, in custom renderer tree fallback to context's view port
                    return context.GetCurrentViewPort();
                }
            }
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

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetPartOfClipPath(bool value) {
            partOfClipPath = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Operations to perform before drawing an element.</summary>
        /// <remarks>
        /// Operations to perform before drawing an element.
        /// This includes setting stroke color and width, fill color.
        /// </remarks>
        /// <param name="context">the svg draw context</param>
        internal virtual void PreDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null && !partOfClipPath) {
                AbstractSvgNodeRenderer.FillProperties fillProperties = CalculateFillProperties(context);
                AbstractSvgNodeRenderer.StrokeProperties strokeProperties = CalculateStrokeProperties(context);
                ApplyFillAndStrokeProperties(fillProperties, strokeProperties, context);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyFillAndStrokeProperties(AbstractSvgNodeRenderer.FillProperties fillProperties, 
            AbstractSvgNodeRenderer.StrokeProperties strokeProperties, SvgDrawContext context) {
            PdfExtGState opacityGraphicsState = new PdfExtGState();
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            if (fillProperties != null) {
                currentCanvas.SetFillColor(fillProperties.GetColor());
                if (!CssUtils.CompareFloats(fillProperties.GetOpacity(), 1f)) {
                    opacityGraphicsState.SetFillOpacity(fillProperties.GetOpacity());
                }
            }
            if (strokeProperties != null) {
                if (strokeProperties.GetLineDashParameters() != null) {
                    SvgStrokeParameterConverter.PdfLineDashParameters lineDashParameters = strokeProperties.GetLineDashParameters
                        ();
                    currentCanvas.SetLineDash(lineDashParameters.GetDashArray(), lineDashParameters.GetDashPhase());
                }
                // As default value for stroke is 'none' we should not set it in case value obtaining fails
                if (strokeProperties.GetColor() != null) {
                    currentCanvas.SetStrokeColor(strokeProperties.GetColor());
                }
                currentCanvas.SetLineWidth(strokeProperties.GetWidth());
                if (!CssUtils.CompareFloats(strokeProperties.GetOpacity(), 1f)) {
                    opacityGraphicsState.SetStrokeOpacity(strokeProperties.GetOpacity());
                }
            }
            if (!opacityGraphicsState.GetPdfObject().IsEmpty()) {
                currentCanvas.SetExtGState(opacityGraphicsState);
            }
        }
//\endcond

        /// <summary>Parse x-axis length value.</summary>
        /// <remarks>
        /// Parse x-axis length value.
        /// If this method is called and there is no view port in
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// , a default current viewport
        /// will be created. This can happen if svg is created manually
        /// (not through
        /// <see cref="iText.Svg.Element.SvgImage"/>
        /// or
        /// <see cref="iText.Svg.Xobject.SvgImageXObject"/>
        /// )
        /// and don't have
        /// <see cref="PdfRootSvgNodeRenderer"/>
        /// as its parent.
        /// </remarks>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// length for parsing
        /// </param>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// instance
        /// </param>
        /// <returns>absolute length in points</returns>
        protected internal virtual float ParseHorizontalLength(String length, SvgDrawContext context) {
            return SvgCssUtils.ParseAbsoluteHorizontalLength(this, length, 0.0F, context);
        }

        /// <summary>Parse y-axis length value.</summary>
        /// <remarks>
        /// Parse y-axis length value.
        /// If this method is called and there is no view port in
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// , a default current viewport
        /// will be created. This can happen if svg is created manually
        /// (not through
        /// <see cref="iText.Svg.Element.SvgImage"/>
        /// or
        /// <see cref="iText.Svg.Xobject.SvgImageXObject"/>
        /// )
        /// and don't have
        /// <see cref="PdfRootSvgNodeRenderer"/>
        /// as its parent.
        /// </remarks>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// length for parsing
        /// </param>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// instance
        /// </param>
        /// <returns>absolute length in points</returns>
        protected internal virtual float ParseVerticalLength(String length, SvgDrawContext context) {
            return SvgCssUtils.ParseAbsoluteVerticalLength(this, length, 0.0F, context);
        }

        /// <summary>Parse length attributes.</summary>
        /// <remarks>
        /// Parse length attributes.
        /// <para />
        /// This method is deprecated and
        /// <see cref="iText.Svg.Utils.SvgCssUtils.ParseAbsoluteLength(AbstractSvgNodeRenderer, System.String, float, float, iText.Svg.Renderers.SvgDrawContext)
        ///     "/>
        /// should
        /// be used instead.
        /// </remarks>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// for parsing
        /// </param>
        /// <param name="percentBaseValue">the value on which percent length is based on</param>
        /// <param name="defaultValue">default value if length is not recognized</param>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// </param>
        /// <returns>absolute value in points</returns>
        [Obsolete]
        protected internal virtual float ParseAbsoluteLength(String length, float percentBaseValue, float defaultValue
            , SvgDrawContext context) {
            return SvgCssUtils.ParseAbsoluteLength(this, length, percentBaseValue, defaultValue, context);
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
                    if (colorRenderer.GetParent() == null) {
                        colorRenderer.SetParent(this);
                    }
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

        private AbstractSvgNodeRenderer.FillProperties CalculateFillProperties(SvgDrawContext context) {
            float generalOpacity = GetOpacity();
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
                // set default if no color has been parsed
                if (fillColor == null) {
                    fillColor = ColorConstants.BLACK;
                }
                return new AbstractSvgNodeRenderer.FillProperties(fillOpacity, fillColor);
            }
            return null;
        }

        private AbstractSvgNodeRenderer.StrokeProperties CalculateStrokeProperties(SvgDrawContext context) {
            String strokeRawValue = GetAttributeOrDefault(SvgConstants.Attributes.STROKE, SvgConstants.Values.NONE);
            if (!SvgConstants.Values.NONE.EqualsIgnoreCase(strokeRawValue)) {
                String strokeWidthRawValue = GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                // 1 px = 0,75 pt
                float strokeWidth = 0.75f;
                if (strokeWidthRawValue != null) {
                    strokeWidth = ParseHorizontalLength(strokeWidthRawValue, context);
                }
                float generalOpacity = GetOpacity();
                float strokeOpacity = GetOpacityByAttributeName(SvgConstants.Attributes.STROKE_OPACITY, generalOpacity);
                Color strokeColor = null;
                TransparentColor transparentColor = GetColorFromAttributeValue(context, strokeRawValue, (float)((double)strokeWidth
                     / 2.0), strokeOpacity);
                if (transparentColor != null) {
                    strokeColor = transparentColor.GetColor();
                    strokeOpacity = transparentColor.GetOpacity();
                }
                String strokeDashArrayRawValue = GetAttribute(SvgConstants.Attributes.STROKE_DASHARRAY);
                String strokeDashOffsetRawValue = GetAttribute(SvgConstants.Attributes.STROKE_DASHOFFSET);
                SvgStrokeParameterConverter.PdfLineDashParameters lineDashParameters = SvgStrokeParameterConverter.ConvertStrokeDashParameters
                    (strokeDashArrayRawValue, strokeDashOffsetRawValue, GetCurrentFontSize(context), context);
                doStroke = true;
                return new AbstractSvgNodeRenderer.StrokeProperties(strokeColor, strokeWidth, strokeOpacity, lineDashParameters
                    );
            }
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class FillProperties {
//\cond DO_NOT_DOCUMENT
            internal readonly float opacity;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly Color color;
//\endcond

            public FillProperties(float opacity, Color color) {
                this.opacity = opacity;
                this.color = color;
            }

            public float GetOpacity() {
                return opacity;
            }

            public Color GetColor() {
                return color;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class StrokeProperties {
//\cond DO_NOT_DOCUMENT
            internal readonly Color color;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly float width;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly float opacity;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly SvgStrokeParameterConverter.PdfLineDashParameters lineDashParameters;
//\endcond

            public StrokeProperties(Color color, float width, float opacity, SvgStrokeParameterConverter.PdfLineDashParameters
                 lineDashParameters) {
                this.color = color;
                this.width = width;
                this.opacity = opacity;
                this.lineDashParameters = lineDashParameters;
            }

            public Color GetColor() {
                return color;
            }

            public float GetWidth() {
                return width;
            }

            public float GetOpacity() {
                return opacity;
            }

            public SvgStrokeParameterConverter.PdfLineDashParameters GetLineDashParameters() {
                return lineDashParameters;
            }
        }
//\endcond

        public abstract ISvgNodeRenderer CreateDeepCopy();

        public abstract Rectangle GetObjectBoundingBox(SvgDrawContext arg1);
    }
}
