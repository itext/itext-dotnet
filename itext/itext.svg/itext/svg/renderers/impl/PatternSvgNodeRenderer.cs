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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Svg;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Implementation for the svg &lt;pattern&gt; tag.</summary>
    public class PatternSvgNodeRenderer : AbstractBranchSvgNodeRenderer, ISvgPaintServer {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(PatternSvgNodeRenderer));

        private const double CONVERT_COEFF = 0.75;

        public override ISvgNodeRenderer CreateDeepCopy() {
            PatternSvgNodeRenderer copy = new PatternSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public virtual Color CreateColor(SvgDrawContext context, Rectangle objectBoundingBox, float objectBoundingBoxMargin
            , float parentOpacity) {
            if (objectBoundingBox == null) {
                return null;
            }
            if (!context.PushPatternId(GetAttribute(SvgConstants.Attributes.ID))) {
                // this means that pattern is cycled
                return null;
            }
            try {
                //create color is an entry point method for pattern when drawing svg, so resolving href values here
                TemplateResolveUtils.Resolve(this, context);
                PdfPattern.Tiling tilingPattern = CreateTilingPattern(context, objectBoundingBox);
                DrawPatternContent(context, tilingPattern);
                return (tilingPattern == null) ? null : new PatternColor(tilingPattern);
            }
            finally {
                context.PopPatternId();
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        private PdfPattern.Tiling CreateTilingPattern(SvgDrawContext context, Rectangle objectBoundingBox) {
            bool isObjectBoundingBoxInPatternUnits = IsObjectBoundingBoxInPatternUnits();
            bool isObjectBoundingBoxInPatternContentUnits = IsObjectBoundingBoxInPatternContentUnits();
            // evaluate pattern rectangle on target pattern units
            Rectangle originalPatternRectangle = CalculateOriginalPatternRectangle(context, isObjectBoundingBoxInPatternUnits
                );
            // get xStep and yStep on target pattern units
            double xStep = originalPatternRectangle.GetWidth();
            double yStep = originalPatternRectangle.GetHeight();
            if (!XStepYStepAreValid(xStep, yStep)) {
                return null;
            }
            // we have to consider transforming an element that use pattern in corresponding  with SVG logic
            AffineTransform patternMatrixTransform = context.GetCurrentCanvasTransform();
            patternMatrixTransform.Concatenate(GetPatternTransform());
            if (isObjectBoundingBoxInPatternUnits) {
                patternMatrixTransform.Concatenate(GetTransformToUserSpaceOnUse(objectBoundingBox));
            }
            patternMatrixTransform.Translate(originalPatternRectangle.GetX(), originalPatternRectangle.GetY());
            float[] viewBoxValues = SvgCssUtils.ParseViewBox(this);
            Rectangle bbox;
            if (viewBoxValues == null || viewBoxValues.Length < SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                if (isObjectBoundingBoxInPatternUnits != isObjectBoundingBoxInPatternContentUnits) {
                    // If pattern units are not the same as pattern content units, then we need to scale
                    // the resulted space into a space to draw pattern content. The pattern rectangle origin
                    // is already in place, but measures should be adjusted.
                    double scaleX;
                    double scaleY;
                    if (isObjectBoundingBoxInPatternContentUnits) {
                        scaleX = objectBoundingBox.GetWidth() / CONVERT_COEFF;
                        scaleY = objectBoundingBox.GetHeight() / CONVERT_COEFF;
                    }
                    else {
                        scaleX = CONVERT_COEFF / objectBoundingBox.GetWidth();
                        scaleY = CONVERT_COEFF / objectBoundingBox.GetHeight();
                    }
                    patternMatrixTransform.Scale(scaleX, scaleY);
                    xStep /= scaleX;
                    yStep /= scaleY;
                }
                bbox = new Rectangle(0F, 0F, (float)xStep, (float)yStep);
            }
            else {
                if (IsViewBoxInvalid(viewBoxValues)) {
                    return null;
                }
                // Here we revert scaling to the object's bounding box coordinate system
                // to keep the aspect ratio of the original viewport of the pattern.
                if (isObjectBoundingBoxInPatternUnits) {
                    double scaleX = CONVERT_COEFF / objectBoundingBox.GetWidth();
                    double scaleY = CONVERT_COEFF / objectBoundingBox.GetHeight();
                    patternMatrixTransform.Scale(scaleX, scaleY);
                    xStep /= scaleX;
                    yStep /= scaleY;
                }
                Rectangle viewBox = new Rectangle(viewBoxValues[0], viewBoxValues[1], viewBoxValues[2], viewBoxValues[3]);
                Rectangle appliedViewBox = CalculateAppliedViewBox(viewBox, xStep, yStep);
                double scaleX_1 = (double)appliedViewBox.GetWidth() / (double)viewBox.GetWidth();
                double scaleY_1 = (double)appliedViewBox.GetHeight() / (double)viewBox.GetHeight();
                double xOffset = (double)appliedViewBox.GetX() / scaleX_1 - (double)viewBox.GetX();
                double yOffset = (double)appliedViewBox.GetY() / scaleY_1 - (double)viewBox.GetY();
                patternMatrixTransform.Translate(xOffset, yOffset);
                patternMatrixTransform.Scale(scaleX_1, scaleY_1);
                xStep /= scaleX_1;
                yStep /= scaleY_1;
                double bboxXOriginal = -xOffset / scaleX_1;
                double bboxYOriginal = -yOffset / scaleY_1;
                bbox = new Rectangle((float)bboxXOriginal, (float)bboxYOriginal, (float)xStep, (float)yStep);
            }
            return CreateColoredTilingPatternInstance(patternMatrixTransform, bbox, xStep, yStep);
        }

        private Rectangle CalculateAppliedViewBox(Rectangle viewBox, double xStep, double yStep) {
            String[] preserveAspectRatio = RetrieveAlignAndMeet();
            Rectangle patternRect = new Rectangle(0f, 0f, (float)xStep, (float)yStep);
            return SvgCoordinateUtils.ApplyViewBox(viewBox, patternRect, preserveAspectRatio[0], preserveAspectRatio[1
                ]);
        }

        private void DrawPatternContent(SvgDrawContext context, PdfPattern.Tiling pattern) {
            if (pattern == null) {
                return;
            }
            PdfCanvas patternCanvas = new PdfPatternCanvas(pattern, context.GetCurrentCanvas().GetDocument());
            context.PushCanvas(patternCanvas);
            try {
                foreach (ISvgNodeRenderer renderer in this.GetChildren()) {
                    renderer.Draw(context);
                }
            }
            finally {
                context.PopCanvas();
            }
        }

        private Rectangle CalculateOriginalPatternRectangle(SvgDrawContext context, bool isObjectBoundingBoxInPatternUnits
            ) {
            double xOffset;
            double yOffset;
            double xStep;
            double yStep;
            if (isObjectBoundingBoxInPatternUnits) {
                xOffset = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.X), 0)
                     * CONVERT_COEFF;
                yOffset = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.Y), 0)
                     * CONVERT_COEFF;
                xStep = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.WIDTH), 
                    0) * CONVERT_COEFF;
                yStep = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.HEIGHT), 
                    0) * CONVERT_COEFF;
            }
            else {
                Rectangle currentViewPort = this.GetCurrentViewBox(context);
                double viewPortX = currentViewPort.GetX();
                double viewPortY = currentViewPort.GetY();
                double viewPortWidth = currentViewPort.GetWidth();
                double viewPortHeight = currentViewPort.GetHeight();
                float em = GetCurrentFontSize(context);
                float rem = context.GetCssContext().GetRootFontSize();
                // get pattern coordinates in userSpaceOnUse coordinate system
                xOffset = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.X), viewPortX
                    , viewPortX, viewPortWidth, em, rem);
                yOffset = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.Y), viewPortY
                    , viewPortY, viewPortHeight, em, rem);
                xStep = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.WIDTH), viewPortX
                    , viewPortX, viewPortWidth, em, rem);
                yStep = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.HEIGHT), viewPortY
                    , viewPortY, viewPortHeight, em, rem);
            }
            return new Rectangle((float)xOffset, (float)yOffset, (float)xStep, (float)yStep);
        }

        private bool IsObjectBoundingBoxInPatternUnits() {
            String patternUnits = GetAttribute(SvgConstants.Attributes.PATTERN_UNITS);
            if (patternUnits == null) {
                patternUnits = GetAttribute(SvgConstants.Attributes.PATTERN_UNITS.ToLowerInvariant());
            }
            if (SvgConstants.Values.USER_SPACE_ON_USE.Equals(patternUnits)) {
                return false;
            }
            else {
                if (patternUnits != null && !SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(patternUnits)) {
                    ITextLogManager.GetLogger(this.GetType()).LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_INVALID_PATTERN_UNITS_LOG
                        , patternUnits));
                }
            }
            return true;
        }

        private bool IsObjectBoundingBoxInPatternContentUnits() {
            String patternContentUnits = GetAttribute(SvgConstants.Attributes.PATTERN_CONTENT_UNITS);
            if (patternContentUnits == null) {
                patternContentUnits = GetAttribute(SvgConstants.Attributes.PATTERN_CONTENT_UNITS.ToLowerInvariant());
            }
            if (SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(patternContentUnits)) {
                return true;
            }
            else {
                if (patternContentUnits != null && !SvgConstants.Values.USER_SPACE_ON_USE.Equals(patternContentUnits)) {
                    ITextLogManager.GetLogger(this.GetType()).LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_INVALID_PATTERN_CONTENT_UNITS_LOG
                        , patternContentUnits));
                }
            }
            return false;
        }

        private static PdfPattern.Tiling CreateColoredTilingPatternInstance(AffineTransform patternAffineTransform
            , Rectangle bbox, double xStep, double yStep) {
            PdfPattern.Tiling coloredTilingPattern = new PdfPattern.Tiling(bbox, (float)xStep, (float)yStep, true);
            SetPatternMatrix(coloredTilingPattern, patternAffineTransform);
            return coloredTilingPattern;
        }

        private static void SetPatternMatrix(PdfPattern.Tiling pattern, AffineTransform affineTransform) {
            if (!affineTransform.IsIdentity()) {
                double[] patternMatrix = new double[6];
                affineTransform.GetMatrix(patternMatrix);
                pattern.SetMatrix(new PdfArray(patternMatrix));
            }
        }

        private static AffineTransform GetTransformToUserSpaceOnUse(Rectangle objectBoundingBox) {
            AffineTransform transform = new AffineTransform();
            transform.Translate(objectBoundingBox.GetX(), objectBoundingBox.GetY());
            transform.Scale(objectBoundingBox.GetWidth() / CONVERT_COEFF, objectBoundingBox.GetHeight() / CONVERT_COEFF
                );
            return transform;
        }

        private static bool XStepYStepAreValid(double xStep, double yStep) {
            if (xStep < 0 || yStep < 0) {
                if (LOGGER.IsEnabled(LogLevel.Warning)) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_WIDTH_OR_HEIGHT_IS_NEGATIVE));
                }
                return false;
            }
            else {
                if (xStep == 0 || yStep == 0) {
                    if (LOGGER.IsEnabled(LogLevel.Information)) {
                        LOGGER.LogInformation(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_WIDTH_OR_HEIGHT_IS_ZERO));
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
        }

        private static bool IsViewBoxInvalid(float[] viewBoxValues) {
            // if viewBox width or height is zero we should disable rendering
            // of the element (according to the viewBox documentation)
            if (viewBoxValues[2] == 0 || viewBoxValues[3] == 0) {
                if (LOGGER.IsEnabled(LogLevel.Information)) {
                    LOGGER.LogInformation(SvgLogMessageConstant.VIEWBOX_WIDTH_OR_HEIGHT_IS_ZERO);
                }
                return true;
            }
            else {
                return false;
            }
        }

        private AffineTransform GetPatternTransform() {
            String patternTransform = GetAttribute(SvgConstants.Attributes.PATTERN_TRANSFORM);
            if (patternTransform == null) {
                patternTransform = GetAttribute(SvgConstants.Attributes.PATTERN_TRANSFORM.ToLowerInvariant());
            }
            if (patternTransform != null && !String.IsNullOrEmpty(patternTransform)) {
                return TransformUtils.ParseTransform(patternTransform);
            }
            return new AffineTransform();
        }
    }
}
