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

        private static float GetInfHeight() {
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
