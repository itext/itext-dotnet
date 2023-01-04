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
            float[] viewBoxValues = GetViewBoxValues();
            Rectangle bbox;
            if (viewBoxValues.Length < VIEWBOX_VALUES_NUMBER) {
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
                patternMatrixTransform.Translate(appliedViewBox.GetX(), appliedViewBox.GetY());
                double scaleX_1 = (double)appliedViewBox.GetWidth() / (double)viewBox.GetWidth();
                double scaleY_1 = (double)appliedViewBox.GetHeight() / (double)viewBox.GetHeight();
                patternMatrixTransform.Scale(scaleX_1, scaleY_1);
                xStep /= scaleX_1;
                yStep /= scaleY_1;
                patternMatrixTransform.Translate(-viewBox.GetX(), -viewBox.GetY());
                double bboxXOriginal = viewBox.GetX() - appliedViewBox.GetX() / scaleX_1;
                double bboxYOriginal = viewBox.GetY() - appliedViewBox.GetY() / scaleY_1;
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
                Rectangle currentViewPort = context.GetCurrentViewPort();
                double viewPortX = currentViewPort.GetX();
                double viewPortY = currentViewPort.GetY();
                double viewPortWidth = currentViewPort.GetWidth();
                double viewPortHeight = currentViewPort.GetHeight();
                float em = GetCurrentFontSize();
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
                    LOGGER.LogInformation(MessageFormatUtil.Format(SvgLogMessageConstant.VIEWBOX_WIDTH_OR_HEIGHT_IS_ZERO));
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
