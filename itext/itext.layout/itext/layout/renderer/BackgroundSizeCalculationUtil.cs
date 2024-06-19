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
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Utility class for calculate background image width and height.</summary>
    internal sealed class BackgroundSizeCalculationUtil {
        private const int PERCENT_100 = 100;

        private static readonly UnitValue PERCENT_VALUE_100 = UnitValue.CreatePercentValue(100);

        private BackgroundSizeCalculationUtil() {
        }

        //no instance required
        /// <summary>Calculates width and height values for image with a given area params.</summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Layout.Properties.BackgroundImage"/>
        /// width and height of which you want to calculate
        /// </param>
        /// <param name="areaWidth">width of the area of this images</param>
        /// <param name="areaHeight">height of the area of this images</param>
        /// <returns>array of two Float values. NOTE that first value defines width, second defines height.</returns>
        /// <seealso cref="iText.Layout.Properties.BackgroundSize"/>
        public static float[] CalculateBackgroundImageSize(BackgroundImage image, float areaWidth, float areaHeight
            ) {
            bool isGradient = image.GetLinearGradientBuilder() != null;
            BackgroundSize size;
            if (!isGradient && image.GetBackgroundSize().IsSpecificSize()) {
                size = CalculateBackgroundSizeForArea(image, areaWidth, areaHeight);
            }
            else {
                size = image.GetBackgroundSize();
            }
            UnitValue width = size.GetBackgroundWidthSize();
            UnitValue height = size.GetBackgroundHeightSize();
            float?[] widthAndHeight = new float?[2];
            if (width != null && width.GetValue() >= 0) {
                bool needScale = !isGradient && height == null;
                CalculateBackgroundWidth(width, areaWidth, needScale, image, widthAndHeight);
            }
            if (height != null && height.GetValue() >= 0) {
                bool needScale = !isGradient && width == null;
                CalculateBackgroundHeight(height, areaHeight, needScale, image, widthAndHeight);
            }
            SetDefaultSizeIfNull(widthAndHeight, areaWidth, areaHeight, image, isGradient);
            return new float[] { (float)widthAndHeight[0], (float)widthAndHeight[1] };
        }

        private static BackgroundSize CalculateBackgroundSizeForArea(BackgroundImage image, float areaWidth, float
             areaHeight) {
            double widthDifference = areaWidth / image.GetImageWidth();
            double heightDifference = areaHeight / image.GetImageHeight();
            if (image.GetBackgroundSize().IsCover()) {
                return CreateSizeWithMaxValueSide(widthDifference > heightDifference);
            }
            else {
                if (image.GetBackgroundSize().IsContain()) {
                    return CreateSizeWithMaxValueSide(widthDifference < heightDifference);
                }
                else {
                    return new BackgroundSize();
                }
            }
        }

        private static BackgroundSize CreateSizeWithMaxValueSide(bool maxWidth) {
            BackgroundSize size = new BackgroundSize();
            if (maxWidth) {
                size.SetBackgroundSizeToValues(PERCENT_VALUE_100, null);
            }
            else {
                size.SetBackgroundSizeToValues(null, PERCENT_VALUE_100);
            }
            return size;
        }

        private static void CalculateBackgroundWidth(UnitValue width, float areaWidth, bool scale, BackgroundImage
             image, float?[] widthAndHeight) {
            if (scale) {
                if (width.IsPercentValue()) {
                    ScaleWidth(areaWidth * width.GetValue() / PERCENT_100, image, widthAndHeight);
                }
                else {
                    ScaleWidth(width.GetValue(), image, widthAndHeight);
                }
            }
            else {
                if (width.IsPercentValue()) {
                    widthAndHeight[0] = areaWidth * width.GetValue() / PERCENT_100;
                }
                else {
                    widthAndHeight[0] = width.GetValue();
                }
            }
        }

        private static void CalculateBackgroundHeight(UnitValue height, float areaHeight, bool scale, BackgroundImage
             image, float?[] widthAndHeight) {
            if (scale) {
                if (height.IsPercentValue()) {
                    ScaleHeight(areaHeight * height.GetValue() / PERCENT_100, image, widthAndHeight);
                }
                else {
                    ScaleHeight(height.GetValue(), image, widthAndHeight);
                }
            }
            else {
                if (height.IsPercentValue()) {
                    widthAndHeight[1] = areaHeight * height.GetValue() / PERCENT_100;
                }
                else {
                    widthAndHeight[1] = height.GetValue();
                }
            }
        }

        private static void ScaleWidth(float newWidth, BackgroundImage image, float?[] imageWidthAndHeight) {
            float difference = image.GetImageWidth() == 0f ? 1f : newWidth / image.GetImageWidth();
            imageWidthAndHeight[0] = newWidth;
            imageWidthAndHeight[1] = image.GetImageHeight() * difference;
        }

        private static void ScaleHeight(float newHeight, BackgroundImage image, float?[] imageWidthAndHeight) {
            float difference = image.GetImageHeight() == 0f ? 1f : newHeight / image.GetImageHeight();
            imageWidthAndHeight[0] = image.GetImageWidth() * difference;
            imageWidthAndHeight[1] = newHeight;
        }

        private static void SetDefaultSizeIfNull(float?[] widthAndHeight, float areaWidth, float areaHeight, BackgroundImage
             image, bool isGradient) {
            if (isGradient) {
                widthAndHeight[0] = widthAndHeight[0] == null ? areaWidth : widthAndHeight[0];
                widthAndHeight[1] = widthAndHeight[1] == null ? areaHeight : widthAndHeight[1];
            }
            else {
                widthAndHeight[0] = widthAndHeight[0] == null ? image.GetImageWidth() : widthAndHeight[0];
                widthAndHeight[1] = widthAndHeight[1] == null ? image.GetImageHeight() : widthAndHeight[1];
            }
        }
    }
//\endcond
}
