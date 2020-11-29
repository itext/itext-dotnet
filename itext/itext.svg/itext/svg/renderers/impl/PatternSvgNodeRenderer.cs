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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Implementation for the svg &lt;pattern&gt; tag.</summary>
    public class PatternSvgNodeRenderer : AbstractBranchSvgNodeRenderer, ISvgPaintServer {
        private const double ZERO = 1E-10;

        public override ISvgNodeRenderer CreateDeepCopy() {
            PatternSvgNodeRenderer copy = new PatternSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public virtual Color CreateColor(SvgDrawContext context, Rectangle objectBoundingBox, float objectBoundingBoxMargin
            , float parentOpacity) {
            if (!context.PushPatternId(GetAttribute(SvgConstants.Attributes.ID))) {
                // this means that pattern is cycled
                return null;
            }
            try {
                PdfPattern.Tiling tilingPattern = CreateTilingPattern(context);
                DrawPatternContent(context, tilingPattern);
                return (tilingPattern == null) ? null : new PatternColor(tilingPattern);
            }
            finally {
                context.PopPatternId();
            }
        }

        private PdfPattern.Tiling CreateTilingPattern(SvgDrawContext context) {
            bool isObjectBoundingBoxInPatternUnits = IsObjectBoundingBoxInPatternUnits();
            bool isObjectBoundingBoxInPatternContentUnits = IsObjectBoundingBoxInPatternContentUnits();
            bool isViewBoxExist = GetAttribute(SvgConstants.Attributes.VIEWBOX) != null;
            AffineTransform patternAffineTransform = new AffineTransform();
            double xOffset;
            double yOffset;
            double xStep;
            double yStep;
            Rectangle bbox;
            if (isObjectBoundingBoxInPatternUnits || isViewBoxExist || isObjectBoundingBoxInPatternContentUnits) {
                return null;
            }
            else {
                Rectangle currentViewPort = context.GetCurrentViewPort();
                double viewPortX = currentViewPort.GetX();
                double viewPortY = currentViewPort.GetY();
                double viewPortWidth = currentViewPort.GetWidth();
                double viewPortHeight = currentViewPort.GetHeight();
                float em = GetCurrentFontSize();
                float rem = context.GetCssContext().GetRootFontSize();
                xOffset = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.X), viewPortX
                    , viewPortX, viewPortWidth, em, rem);
                yOffset = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.Y), viewPortY
                    , viewPortY, viewPortHeight, em, rem);
                xStep = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.WIDTH), viewPortX
                    , viewPortX, viewPortWidth, em, rem);
                yStep = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.HEIGHT), viewPortX
                    , viewPortX, viewPortHeight, em, rem);
                bbox = new Rectangle(0F, 0F, (float)xStep, (float)yStep);
                if (!IsZero(xOffset, ZERO) || !IsZero(yOffset, ZERO)) {
                    patternAffineTransform.Translate(xOffset, yOffset);
                }
            }
            return CreateColoredTilingPatternInstance(patternAffineTransform, bbox, xStep, yStep);
        }

        private PdfPattern.Tiling CreateColoredTilingPatternInstance(AffineTransform patternAffineTransform, Rectangle
             bbox, double xStep, double yStep) {
            PdfPattern.Tiling coloredTilingPattern = new PdfPattern.Tiling(bbox, (float)xStep, (float)yStep, true);
            SetPatternMatrix(coloredTilingPattern, patternAffineTransform);
            return coloredTilingPattern;
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

        private void SetPatternMatrix(PdfPattern.Tiling pattern, AffineTransform affineTransform) {
            if (!affineTransform.IsIdentity()) {
                double[] patternMatrix = new double[6];
                affineTransform.GetMatrix(patternMatrix);
                pattern.SetMatrix(new PdfArray(patternMatrix));
            }
        }

        private bool IsObjectBoundingBoxInPatternUnits() {
            String patternUnits = GetAttribute(SvgConstants.Attributes.PATTERN_UNITS);
            if (SvgConstants.Values.USER_SPACE_ON_USE.Equals(patternUnits)) {
                return false;
            }
            else {
                if (patternUnits != null && !SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(patternUnits)) {
                    LogManager.GetLogger(this.GetType()).Warn(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_INVALID_PATTERN_UNITS_LOG
                        , patternUnits));
                }
            }
            return true;
        }

        private bool IsObjectBoundingBoxInPatternContentUnits() {
            String patternContentUnits = GetAttribute(SvgConstants.Attributes.PATTERN_CONTENT_UNITS);
            if (SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(patternContentUnits)) {
                return true;
            }
            else {
                if (patternContentUnits != null && !SvgConstants.Values.USER_SPACE_ON_USE.Equals(patternContentUnits)) {
                    LogManager.GetLogger(this.GetType()).Warn(MessageFormatUtil.Format(SvgLogMessageConstant.PATTERN_INVALID_PATTERN_CONTENT_UNITS_LOG
                        , patternContentUnits));
                }
            }
            return false;
        }

        private static bool IsZero(double val, double delta) {
            return -delta < val && val < delta;
        }
    }
}
