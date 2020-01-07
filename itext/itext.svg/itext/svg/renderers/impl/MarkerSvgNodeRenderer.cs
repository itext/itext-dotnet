/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using Common.Logging;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;marker&gt; tag.
    /// </summary>
    public class MarkerSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        // Default marker width in point units (3 px)
        private const float DEFAULT_MARKER_WIDTH = 2.25f;

        // Default marker height in point units (3 px)
        private const float DEFAULT_MARKER_HEIGHT = 2.25f;

        // Default refX value
        private const float DEFAULT_REF_X = 0f;

        // Default refY value
        private const float DEFAULT_REF_Y = 0f;

        public override ISvgNodeRenderer CreateDeepCopy() {
            MarkerSvgNodeRenderer copy = new MarkerSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        internal override void PreDraw(SvgDrawContext context) {
            base.PreDraw(context);
            float[] markerWidthHeight = GetMarkerWidthHeightValues();
            float markerWidth = markerWidthHeight[0];
            float markerHeight = markerWidthHeight[1];
            Rectangle markerViewport = new Rectangle(CssUtils.ParseAbsoluteLength(this.GetAttribute(SvgConstants.Attributes
                .X)), CssUtils.ParseAbsoluteLength(this.GetAttribute(SvgConstants.Attributes.Y)), markerWidth, markerHeight
                );
            context.AddViewPort(markerViewport);
        }

        internal virtual void ApplyMarkerAttributes(SvgDrawContext context) {
            ApplyRotation(context);
            ApplyUserSpaceScaling(context);
            ApplyCoordinatesTranslation(context);
        }

        internal static void DrawMarker(SvgDrawContext context, String moveX, String moveY, MarkerVertexType markerToUse
            , AbstractSvgNodeRenderer parent) {
            String elementToReUse = parent.attributesAndStyles.Get(markerToUse.ToString());
            String normalizedName = SvgTextUtil.FilterReferenceValue(elementToReUse);
            ISvgNodeRenderer template = context.GetNamedObject(normalizedName);
            //Clone template
            ISvgNodeRenderer namedObject = template == null ? null : template.CreateDeepCopy();
            if (namedObject is MarkerSvgNodeRenderer && 
                        // Having markerWidth or markerHeight with negative or zero value disables rendering of the element .
                        MarkerWidthHeightAreCorrect((MarkerSvgNodeRenderer)namedObject)) {
                // setting the parent of the referenced element to this instance
                namedObject.SetParent(parent);
                namedObject.SetAttribute(SvgConstants.Tags.MARKER, markerToUse.ToString());
                namedObject.SetAttribute(SvgConstants.Attributes.X, moveX);
                namedObject.SetAttribute(SvgConstants.Attributes.Y, moveY);
                namedObject.Draw(context);
                // unsetting the parent of the referenced element
                namedObject.SetParent(null);
            }
        }

        internal override void ApplyViewBox(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                float[] markerWidthHeight = GetMarkerWidthHeightValues();
                float markerWidth = markerWidthHeight[0];
                float markerHeight = markerWidthHeight[1];
                float[] values = GetViewBoxValues(markerWidth, markerHeight);
                Rectangle currentViewPort = context.GetCurrentViewPort();
                base.CalculateAndApplyViewBox(context, values, currentViewPort);
            }
        }

        private float[] GetMarkerWidthHeightValues() {
            float markerWidth = DEFAULT_MARKER_WIDTH;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_WIDTH)) {
                String markerWidthRawValue = attributesAndStyles.Get(SvgConstants.Attributes.MARKER_WIDTH);
                markerWidth = CssUtils.ParseAbsoluteLength(markerWidthRawValue);
            }
            float markerHeight = DEFAULT_MARKER_HEIGHT;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_HEIGHT)) {
                String markerHeightRawValue = attributesAndStyles.Get(SvgConstants.Attributes.MARKER_HEIGHT);
                markerHeight = CssUtils.ParseAbsoluteLength(markerHeightRawValue);
            }
            return new float[] { markerWidth, markerHeight };
        }

        private static bool MarkerWidthHeightAreCorrect(MarkerSvgNodeRenderer namedObject) {
            ILog log = LogManager.GetLogger(typeof(MarkerSvgNodeRenderer));
            String markerWidth = namedObject.GetAttribute(SvgConstants.Attributes.MARKER_WIDTH);
            String markerHeight = namedObject.GetAttribute(SvgConstants.Attributes.MARKER_HEIGHT);
            bool isCorrect = true;
            if (markerWidth != null) {
                float absoluteMarkerWidthValue = CssUtils.ParseAbsoluteLength(markerWidth);
                if (absoluteMarkerWidthValue == 0) {
                    log.Warn(SvgLogMessageConstant.MARKER_WIDTH_IS_ZERO_VALUE);
                    isCorrect = false;
                }
                else {
                    if (absoluteMarkerWidthValue < 0) {
                        log.Warn(SvgLogMessageConstant.MARKER_WIDTH_IS_NEGATIVE_VALUE);
                        isCorrect = false;
                    }
                }
            }
            if (markerHeight != null) {
                float absoluteMarkerHeightValue = CssUtils.ParseAbsoluteLength(markerHeight);
                if (absoluteMarkerHeightValue == 0) {
                    log.Warn(SvgLogMessageConstant.MARKER_HEIGHT_IS_ZERO_VALUE);
                    isCorrect = false;
                }
                else {
                    if (absoluteMarkerHeightValue < 0) {
                        log.Warn(SvgLogMessageConstant.MARKER_HEIGHT_IS_NEGATIVE_VALUE);
                        isCorrect = false;
                    }
                }
            }
            return isCorrect;
        }

        private ISvgNodeRenderer GetSvgRootElement(ISvgNodeRenderer element) {
            if (element is SvgTagSvgNodeRenderer && element.GetParent() is PdfRootSvgNodeRenderer) {
                return element;
            }
            if (element.GetParent() != null) {
                return GetSvgRootElement(element.GetParent());
            }
            return null;
        }

        // TODO (DEVSIX-3596) Add support of 'lh' 'ch' units and viewport-relative units
        private float ParseFontRelativeOrAbsoluteLengthOnMarker(String length) {
            float value = 0f;
            if (CssUtils.IsMetricValue(length) || CssUtils.IsNumericValue(length)) {
                value = CssUtils.ParseAbsoluteLength(length);
            }
            else {
                if (CssUtils.IsFontRelativeValue(length)) {
                    // Defaut font-size is medium
                    value = CssUtils.ParseRelativeValue(length, CssUtils.ParseAbsoluteFontSize(CommonCssConstants.MEDIUM));
                    // Different browsers process font-relative units for markers differently.
                    // We do it according to the css specification.
                    if (CssUtils.IsRemValue(length)) {
                        ISvgNodeRenderer rootElement = GetSvgRootElement(GetParent());
                        if (rootElement != null && rootElement.GetAttribute(CommonCssConstants.FONT_SIZE) != null) {
                            value = CssUtils.ParseRelativeValue(length, CssUtils.ParseAbsoluteFontSize(rootElement.GetAttribute(CommonCssConstants
                                .FONT_SIZE)));
                        }
                    }
                    else {
                        if (CssUtils.IsEmValue(length)) {
                            ISvgNodeRenderer parentElement = this.GetParent();
                            if (parentElement != null && parentElement.GetAttribute(CommonCssConstants.FONT_SIZE) != null) {
                                value = CssUtils.ParseRelativeValue(length, CssUtils.ParseAbsoluteFontSize(parentElement.GetAttribute(CommonCssConstants
                                    .FONT_SIZE)));
                            }
                        }
                        else {
                            if (CssUtils.IsExValue(length)) {
                                if (this.GetAttribute(CommonCssConstants.FONT_SIZE) != null) {
                                    value = CssUtils.ParseRelativeValue(length, CssUtils.ParseAbsoluteFontSize(this.GetAttribute(CommonCssConstants
                                        .FONT_SIZE)));
                                }
                            }
                        }
                    }
                }
            }
            return value;
        }

        private void ApplyRotation(SvgDrawContext context) {
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.ORIENT)) {
                String orient = this.attributesAndStyles.Get(SvgConstants.Attributes.ORIENT);
                double rotAngle = double.NaN;
                // If placed by marker-start, the marker is oriented 180° different from
                // the orientation that would be used if auto was specified.
                // For all other markers, auto-start-reverse means the same as auto.
                if (SvgConstants.Values.AUTO.Equals(orient) || (SvgConstants.Values.AUTO_START_REVERSE.Equals(orient) && !
                    SvgConstants.Attributes.MARKER_START.Equals(this.attributesAndStyles.Get(SvgConstants.Tags.MARKER)))) {
                    rotAngle = ((IMarkerCapable)GetParent()).GetAutoOrientAngle(this, false);
                }
                else {
                    if (SvgConstants.Values.AUTO_START_REVERSE.Equals(orient) && SvgConstants.Attributes.MARKER_START.Equals(this
                        .attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                        rotAngle = ((IMarkerCapable)GetParent()).GetAutoOrientAngle(this, true);
                    }
                    else {
                        if (CssUtils.IsAngleValue(orient) || CssUtils.IsNumericValue(orient)) {
                            rotAngle = CssUtils.ParseAngle(this.attributesAndStyles.Get(SvgConstants.Attributes.ORIENT));
                        }
                    }
                }
                if (!double.IsNaN(rotAngle)) {
                    context.GetCurrentCanvas().ConcatMatrix(AffineTransform.GetRotateInstance(rotAngle));
                }
            }
        }

        private void ApplyUserSpaceScaling(SvgDrawContext context) {
            if (!this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_UNITS) || SvgConstants.Values.STROKEWIDTH
                .Equals(this.attributesAndStyles.Get(SvgConstants.Attributes.MARKER_UNITS))) {
                String parentValue = this.GetParent().GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                if (parentValue != null) {
                    float strokeWidthScale;
                    if (CssUtils.IsPercentageValue(parentValue)) {
                        // If stroke width is a percentage value is always computed as a percentage of the normalized viewBox diagonal length.
                        double rootViewPortHeight = context.GetRootViewPort().GetHeight();
                        double rootViewPortWidth = context.GetRootViewPort().GetWidth();
                        double viewBoxDiagonalLength = Math.Sqrt(rootViewPortHeight * rootViewPortHeight + rootViewPortWidth * rootViewPortWidth
                            );
                        strokeWidthScale = CssUtils.ParseRelativeValue(parentValue, (float)viewBoxDiagonalLength);
                    }
                    else {
                        strokeWidthScale = SvgCssUtils.ConvertPtsToPx(ParseFontRelativeOrAbsoluteLengthOnMarker(parentValue));
                    }
                    context.GetCurrentCanvas().ConcatMatrix(AffineTransform.GetScaleInstance(strokeWidthScale, strokeWidthScale
                        ));
                }
            }
        }

        private void ApplyCoordinatesTranslation(SvgDrawContext context) {
            float xScale = 1;
            float yScale = 1;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.VIEWBOX)) {
                //Parse viewbox parameters stuff
                String viewBoxValues = attributesAndStyles.Get(SvgConstants.Attributes.VIEWBOX);
                IList<String> valueStrings = SvgCssUtils.SplitValueList(viewBoxValues);
                float[] viewBox = GetViewBoxValues();
                xScale = context.GetCurrentViewPort().GetWidth() / viewBox[2];
                yScale = context.GetCurrentViewPort().GetHeight() / viewBox[3];
            }
            float moveX = DEFAULT_REF_X;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.REFX)) {
                String refX = this.attributesAndStyles.Get(SvgConstants.Attributes.REFX);
                if (CssUtils.IsPercentageValue(refX)) {
                    moveX = CssUtils.ParseRelativeValue(refX, context.GetRootViewPort().GetWidth());
                }
                else {
                    moveX = ParseFontRelativeOrAbsoluteLengthOnMarker(refX);
                }
                //Apply scale
                moveX *= -1 * xScale;
            }
            float moveY = DEFAULT_REF_Y;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.REFY)) {
                String refY = this.attributesAndStyles.Get(SvgConstants.Attributes.REFY);
                if (CssUtils.IsPercentageValue(refY)) {
                    moveY = CssUtils.ParseRelativeValue(refY, context.GetRootViewPort().GetHeight());
                }
                else {
                    moveY = ParseFontRelativeOrAbsoluteLengthOnMarker(refY);
                }
                moveY *= -1 * yScale;
            }
            AffineTransform translation = AffineTransform.GetTranslateInstance(moveX, moveY);
            if (!translation.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(translation);
            }
        }

        private float[] GetViewBoxValues(float defaultWidth, float defaultHeight) {
            float[] values;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.VIEWBOX)) {
                //Parse viewbox parameters stuff
                values = base.GetViewBoxValues();
            }
            else {
                //If viewbox is not specified, it's width and height are the same as passed defaults
                values = new float[4];
                values[0] = 0;
                values[1] = 0;
                values[2] = defaultWidth;
                values[3] = defaultHeight;
            }
            return values;
        }
    }
}
