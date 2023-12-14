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
using iText.Kernel.Geom;

namespace iText.Layout.Properties {
    /// <summary>Class to hold background-repeat property.</summary>
    public class BackgroundRepeat {
        private readonly BackgroundRepeat.BackgroundRepeatValue xAxisRepeat;

        private readonly BackgroundRepeat.BackgroundRepeatValue yAxisRepeat;

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// The axis will have the value
        /// <see cref="BackgroundRepeatValue.REPEAT"/>.
        /// </remarks>
        public BackgroundRepeat()
            : this(BackgroundRepeat.BackgroundRepeatValue.REPEAT) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance based on one
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <param name="repeat">the repeat value that will be set for for both axes</param>
        public BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue repeat)
            : this(repeat, repeat) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance based on two
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <param name="xAxisRepeat">the repeat value that will be set for for X axis</param>
        /// <param name="yAxisRepeat">the repeat value that will be set for for Y axis</param>
        public BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue xAxisRepeat, BackgroundRepeat.BackgroundRepeatValue
             yAxisRepeat) {
            this.xAxisRepeat = xAxisRepeat;
            this.yAxisRepeat = yAxisRepeat;
        }

        /// <summary>
        /// Gets the
        /// <see cref="BackgroundRepeatValue"/>
        /// value for X axis.
        /// </summary>
        /// <returns>the repeat value for X axis.</returns>
        public virtual BackgroundRepeat.BackgroundRepeatValue GetXAxisRepeat() {
            return xAxisRepeat;
        }

        /// <summary>
        /// Gets the
        /// <see cref="BackgroundRepeatValue"/>
        /// value for Y axis.
        /// </summary>
        /// <returns>the repeat value for Y axis.</returns>
        public virtual BackgroundRepeat.BackgroundRepeatValue GetYAxisRepeat() {
            return yAxisRepeat;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="BackgroundRepeatValue.NO_REPEAT"/>
        /// value is set on X axis or not.
        /// </summary>
        /// <returns>
        /// is the X axis have
        /// <see cref="BackgroundRepeatValue.NO_REPEAT"/>
        /// value
        /// </returns>
        public virtual bool IsNoRepeatOnXAxis() {
            return xAxisRepeat == BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="BackgroundRepeatValue.NO_REPEAT"/>
        /// value is set on Y axis or not.
        /// </summary>
        /// <returns>
        /// is the Y axis have
        /// <see cref="BackgroundRepeatValue.NO_REPEAT"/>
        /// value
        /// </returns>
        public virtual bool IsNoRepeatOnYAxis() {
            return yAxisRepeat == BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT;
        }

        /// <summary>Prepares the image rectangle for drawing.</summary>
        /// <remarks>
        /// Prepares the image rectangle for drawing. This means that the size and position of the image
        /// rectangle will be changed to match the
        /// <see cref="BackgroundRepeatValue"/>
        /// values for the axes.
        /// </remarks>
        /// <param name="imageRectangle">the image rectangle which will be changed</param>
        /// <param name="backgroundArea">the background available area</param>
        /// <param name="backgroundSize">the image background size property</param>
        /// <returns>the necessary whitespace between backgrounds</returns>
        public virtual Point PrepareRectangleToDrawingAndGetWhitespace(Rectangle imageRectangle, Rectangle backgroundArea
            , BackgroundSize backgroundSize) {
            if (BackgroundRepeat.BackgroundRepeatValue.ROUND == xAxisRepeat) {
                int ratio = iText.Layout.Properties.BackgroundRepeat.CalculateRatio(backgroundArea.GetWidth(), imageRectangle
                    .GetWidth());
                float initialImageRatio = imageRectangle.GetHeight() / imageRectangle.GetWidth();
                imageRectangle.SetWidth(backgroundArea.GetWidth() / ratio);
                if (BackgroundRepeat.BackgroundRepeatValue.ROUND != yAxisRepeat && backgroundSize.GetBackgroundHeightSize(
                    ) == null) {
                    imageRectangle.MoveUp(imageRectangle.GetHeight() - imageRectangle.GetWidth() * initialImageRatio);
                    imageRectangle.SetHeight(imageRectangle.GetWidth() * initialImageRatio);
                }
            }
            if (BackgroundRepeat.BackgroundRepeatValue.ROUND == yAxisRepeat) {
                int ratio = iText.Layout.Properties.BackgroundRepeat.CalculateRatio(backgroundArea.GetHeight(), imageRectangle
                    .GetHeight());
                float initialImageRatio = imageRectangle.GetWidth() / imageRectangle.GetHeight();
                imageRectangle.MoveUp(imageRectangle.GetHeight() - backgroundArea.GetHeight() / ratio);
                imageRectangle.SetHeight(backgroundArea.GetHeight() / ratio);
                if (BackgroundRepeat.BackgroundRepeatValue.ROUND != xAxisRepeat && backgroundSize.GetBackgroundWidthSize()
                     == null) {
                    imageRectangle.SetWidth(imageRectangle.GetHeight() * initialImageRatio);
                }
            }
            return ProcessSpaceValueAndCalculateWhitespace(imageRectangle, backgroundArea);
        }

        private Point ProcessSpaceValueAndCalculateWhitespace(Rectangle imageRectangle, Rectangle backgroundArea) {
            Point whitespace = new Point();
            if (BackgroundRepeat.BackgroundRepeatValue.SPACE == xAxisRepeat) {
                if (imageRectangle.GetWidth() * 2 <= backgroundArea.GetWidth()) {
                    imageRectangle.SetX(backgroundArea.GetX());
                    whitespace.SetLocation(iText.Layout.Properties.BackgroundRepeat.CalculateWhitespace(backgroundArea.GetWidth
                        (), imageRectangle.GetWidth()), 0);
                }
                else {
                    float rightSpace = backgroundArea.GetRight() - imageRectangle.GetRight();
                    float leftSpace = imageRectangle.GetLeft() - backgroundArea.GetLeft();
                    float xWhitespace = Math.Max(rightSpace, leftSpace);
                    xWhitespace = xWhitespace > 0 ? xWhitespace : 0;
                    whitespace.SetLocation(xWhitespace, 0);
                }
            }
            if (BackgroundRepeat.BackgroundRepeatValue.SPACE == yAxisRepeat) {
                if (imageRectangle.GetHeight() * 2 <= backgroundArea.GetHeight()) {
                    imageRectangle.SetY(backgroundArea.GetY() + backgroundArea.GetHeight() - imageRectangle.GetHeight());
                    whitespace.SetLocation(whitespace.GetX(), iText.Layout.Properties.BackgroundRepeat.CalculateWhitespace(backgroundArea
                        .GetHeight(), imageRectangle.GetHeight()));
                }
                else {
                    float topSpace = backgroundArea.GetTop() - imageRectangle.GetTop();
                    float bottomSpace = imageRectangle.GetBottom() - backgroundArea.GetBottom();
                    float yWhitespace = Math.Max(topSpace, bottomSpace);
                    yWhitespace = yWhitespace > 0 ? yWhitespace : 0;
                    whitespace.SetLocation(whitespace.GetX(), yWhitespace);
                }
            }
            return whitespace;
        }

        private static int CalculateRatio(float areaSize, float backgroundSize) {
            int ratio = (int)Math.Floor(areaSize / backgroundSize);
            float remainSpace = areaSize - (ratio * backgroundSize);
            if (remainSpace >= (backgroundSize / 2)) {
                ratio++;
            }
            return ratio == 0 ? 1 : ratio;
        }

        private static float CalculateWhitespace(float areaSize, float backgroundSize) {
            float whitespace = 0;
            int ratio = (int)Math.Floor(areaSize / backgroundSize);
            if (ratio > 0) {
                whitespace = areaSize - (ratio * backgroundSize);
                if (ratio > 1) {
                    whitespace /= (ratio - 1);
                }
            }
            return whitespace;
        }

        /// <summary>Defines all possible background repeat values for one axis.</summary>
        public enum BackgroundRepeatValue {
            /// <summary>
            /// The no repeat value which mean that the background will not be repeated, but displayed once with its
            /// original size.
            /// </summary>
            NO_REPEAT,
            /// <summary>
            /// The repeat value which means that the background with its original size will be repeated over the entire
            /// available space.
            /// </summary>
            REPEAT,
            /// <summary>The round value which mean that the background will stretch or compress.</summary>
            /// <remarks>
            /// The round value which mean that the background will stretch or compress. Initially, the available space is
            /// divided by module by the size of the background, if the result is less than half the size of the background,
            /// then the background is stretched in such a way that when it is repeated it will take up all the space,
            /// otherwise the background is compressed to fit one more background in the available space.
            /// </remarks>
            ROUND,
            /// <summary>
            /// The space value which means that the background will be repeated as much as possible with its original size
            /// and without cropping.
            /// </summary>
            /// <remarks>
            /// The space value which means that the background will be repeated as much as possible with its original size
            /// and without cropping. the first and last backgrounds are attached to opposite edges of the available space,
            /// and the whitespaces are evenly distributed between the backgrounds.
            /// </remarks>
            SPACE
        }
    }
}
