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
using iText.Layout.Properties;

namespace iText.Layout.Renderer.Objectfit {
    /// <summary>
    /// Utility class which supports the
    /// <see cref="iText.Layout.Properties.ObjectFit"/>
    /// property.
    /// </summary>
    public sealed class ObjectFitCalculator {
        private ObjectFitCalculator() {
        }

        /// <summary>
        /// Calculates size of image to be render when certain
        /// <see cref="iText.Layout.Properties.ObjectFit"/>
        /// mode is
        /// applied.
        /// </summary>
        /// <remarks>
        /// Calculates size of image to be render when certain
        /// <see cref="iText.Layout.Properties.ObjectFit"/>
        /// mode is
        /// applied. The width or the height of the image might be greater than the same
        /// property of the image in a document. In this case parts of the image will not
        /// be shown.
        /// </remarks>
        /// <param name="objectFit">is an object-fit mode</param>
        /// <param name="absoluteImageWidth">is a width of the original image</param>
        /// <param name="absoluteImageHeight">is a height of the original image</param>
        /// <param name="imageContainerWidth">is a width of the image to draw</param>
        /// <param name="imageContainerHeight">is a width of the image to draw</param>
        /// <returns>
        /// results of object-fit mode applying as an
        /// <see cref="ObjectFitApplyingResult"/>
        /// object
        /// </returns>
        public static ObjectFitApplyingResult CalculateRenderedImageSize(ObjectFit objectFit, double absoluteImageWidth
            , double absoluteImageHeight, double imageContainerWidth, double imageContainerHeight) {
            switch (objectFit) {
                case ObjectFit.FILL: {
                    return ProcessFill(imageContainerWidth, imageContainerHeight);
                }

                case ObjectFit.CONTAIN: {
                    return ProcessContain(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight);
                }

                case ObjectFit.COVER: {
                    return ProcessCover(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight);
                }

                case ObjectFit.SCALE_DOWN: {
                    return ProcessScaleDown(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight
                        );
                }

                case ObjectFit.NONE: {
                    return ProcessNone(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight);
                }

                default: {
                    throw new ArgumentException("Object fit parameter cannot be null!");
                }
            }
        }

        private static ObjectFitApplyingResult ProcessFill(double imageContainerWidth, double imageContainerHeight
            ) {
            return new ObjectFitApplyingResult(imageContainerWidth, imageContainerHeight, false);
        }

        private static ObjectFitApplyingResult ProcessContain(double absoluteImageWidth, double absoluteImageHeight
            , double imageContainerWidth, double imageContainerHeight) {
            return ProcessToFitSide(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight
                , false);
        }

        private static ObjectFitApplyingResult ProcessCover(double absoluteImageWidth, double absoluteImageHeight, 
            double imageContainerWidth, double imageContainerHeight) {
            return ProcessToFitSide(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight
                , true);
        }

        private static ObjectFitApplyingResult ProcessScaleDown(double absoluteImageWidth, double absoluteImageHeight
            , double imageContainerWidth, double imageContainerHeight) {
            if (imageContainerWidth >= absoluteImageWidth && imageContainerHeight >= absoluteImageHeight) {
                return new ObjectFitApplyingResult(absoluteImageWidth, absoluteImageHeight, false);
            }
            else {
                return ProcessToFitSide(absoluteImageWidth, absoluteImageHeight, imageContainerWidth, imageContainerHeight
                    , false);
            }
        }

        private static ObjectFitApplyingResult ProcessNone(double absoluteImageWidth, double absoluteImageHeight, 
            double imageContainerWidth, double imageContainerHeight) {
            bool doesObjectFitRequireCutting = imageContainerWidth <= absoluteImageWidth || imageContainerHeight <= absoluteImageHeight;
            return new ObjectFitApplyingResult(absoluteImageWidth, absoluteImageHeight, doesObjectFitRequireCutting);
        }

        private static ObjectFitApplyingResult ProcessToFitSide(double absoluteImageWidth, double absoluteImageHeight
            , double imageContainerWidth, double imageContainerHeight, bool clipToFit) {
            double widthCoeff = imageContainerWidth / absoluteImageWidth;
            double heightCoeff = imageContainerHeight / absoluteImageHeight;
            double renderedImageWidth;
            double renderedImageHeight;
            bool isWidthFitted = heightCoeff > widthCoeff ^ clipToFit;
            if (isWidthFitted) {
                renderedImageWidth = imageContainerWidth;
                renderedImageHeight = absoluteImageHeight * imageContainerWidth / absoluteImageWidth;
            }
            else {
                renderedImageHeight = imageContainerHeight;
                renderedImageWidth = absoluteImageWidth * imageContainerHeight / absoluteImageHeight;
            }
            return new ObjectFitApplyingResult(renderedImageWidth, renderedImageHeight, clipToFit);
        }
    }
}
