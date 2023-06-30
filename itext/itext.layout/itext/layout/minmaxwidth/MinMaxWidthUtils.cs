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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Minmaxwidth {
    public sealed class MinMaxWidthUtils {
        private const float eps = 0.01f;

        private const float max = 32760f;

        public static float GetEps() {
            return eps;
        }

        public static float GetInfWidth() {
            return max;
        }

        public static float GetInfHeight() {
            return 1e6f;
        }

        public static bool IsEqual(double x, double y) {
            return Math.Abs(x - y) < eps;
        }

        public static MinMaxWidth CountDefaultMinMaxWidth(IRenderer renderer) {
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(GetInfWidth(), GetInfHeight
                ()))));
            return result.GetStatus() == LayoutResult.NOTHING ? new MinMaxWidth() : new MinMaxWidth(0, result.GetOccupiedArea
                ().GetBBox().GetWidth(), 0);
        }

        public static float GetBorderWidth(IPropertyContainer element) {
            Border border = element.GetProperty<Border>(Property.BORDER);
            Border rightBorder = element.GetProperty<Border>(Property.BORDER_RIGHT);
            Border leftBorder = element.GetProperty<Border>(Property.BORDER_LEFT);
            if (!element.HasOwnProperty(Property.BORDER_RIGHT)) {
                rightBorder = border;
            }
            if (!element.HasOwnProperty(Property.BORDER_LEFT)) {
                leftBorder = border;
            }
            float rightBorderWidth = rightBorder != null ? rightBorder.GetWidth() : 0;
            float leftBorderWidth = leftBorder != null ? leftBorder.GetWidth() : 0;
            return rightBorderWidth + leftBorderWidth;
        }

        public static float GetMarginsWidth(IPropertyContainer element) {
            UnitValue rightMargin = element.GetProperty<UnitValue>(Property.MARGIN_RIGHT);
            if (null != rightMargin && !rightMargin.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(MinMaxWidthUtils));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_RIGHT));
            }
            UnitValue leftMargin = element.GetProperty<UnitValue>(Property.MARGIN_LEFT);
            if (null != leftMargin && !leftMargin.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(MinMaxWidthUtils));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            float rightMarginWidth = rightMargin != null ? rightMargin.GetValue() : 0;
            float leftMarginWidth = leftMargin != null ? leftMargin.GetValue() : 0;
            return rightMarginWidth + leftMarginWidth;
        }

        public static float GetPaddingWidth(IPropertyContainer element) {
            UnitValue rightPadding = element.GetProperty<UnitValue>(Property.PADDING_RIGHT);
            if (null != rightPadding && !rightPadding.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(MinMaxWidthUtils));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_RIGHT));
            }
            UnitValue leftPadding = element.GetProperty<UnitValue>(Property.PADDING_LEFT);
            if (null != leftPadding && !leftPadding.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(MinMaxWidthUtils));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_LEFT));
            }
            float rightPaddingWidth = rightPadding != null ? rightPadding.GetValue() : 0;
            float leftPaddingWidth = leftPadding != null ? leftPadding.GetValue() : 0;
            return rightPaddingWidth + leftPaddingWidth;
        }
    }
}
