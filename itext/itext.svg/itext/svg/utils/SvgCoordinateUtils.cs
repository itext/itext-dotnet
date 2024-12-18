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
using iText.Kernel.Geom;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Utils {
    /// <summary>Utility class that facilitates various methods for calculating/transforming coordinates.</summary>
    public class SvgCoordinateUtils {
        /// <summary>Converts relative coordinates to absolute ones.</summary>
        /// <remarks>
        /// Converts relative coordinates to absolute ones. Assumes that relative coordinates are represented by
        /// an array of coordinates with length proportional to the length of current coordinates array,
        /// so that current coordinates array is applied in segments to the relative coordinates array
        /// </remarks>
        /// <param name="relativeCoordinates">the initial set of coordinates</param>
        /// <param name="currentCoordinates">an array representing the point relative to which the relativeCoordinates are defined
        ///     </param>
        /// <returns>a String array of absolute coordinates, with the same length as the input array</returns>
        public static String[] MakeRelativeOperatorCoordinatesAbsolute(String[] relativeCoordinates, double[] currentCoordinates
            ) {
            if (relativeCoordinates.Length % currentCoordinates.Length != 0) {
                throw new ArgumentException(SvgExceptionMessageConstant.COORDINATE_ARRAY_LENGTH_MUST_BY_DIVISIBLE_BY_CURRENT_COORDINATES_ARRAY_LENGTH
                    );
            }
            String[] absoluteOperators = new String[relativeCoordinates.Length];
            for (int i = 0; i < relativeCoordinates.Length; ) {
                for (int j = 0; j < currentCoordinates.Length; j++, i++) {
                    double relativeDouble = Double.Parse(relativeCoordinates[i], System.Globalization.CultureInfo.InvariantCulture
                        );
                    relativeDouble += currentCoordinates[j];
                    absoluteOperators[i] = SvgCssUtils.ConvertDoubleToString(relativeDouble);
                }
            }
            return absoluteOperators;
        }

        /// <summary>Calculate the angle between two vectors</summary>
        /// <param name="vectorA">first vector</param>
        /// <param name="vectorB">second vector</param>
        /// <returns>angle between vectors in radians units</returns>
        public static double CalculateAngleBetweenTwoVectors(Vector vectorA, Vector vectorB) {
            return Math.Acos((double)vectorA.Dot(vectorB) / ((double)vectorA.Length() * (double)vectorB.Length()));
        }

        /// <summary>Returns absolute value for attribute in userSpaceOnUse coordinate system.</summary>
        /// <param name="attributeValue">value of attribute.</param>
        /// <param name="defaultValue">default value.</param>
        /// <param name="start">start border for calculating percent value.</param>
        /// <param name="length">length for calculating percent value.</param>
        /// <param name="em">em value.</param>
        /// <param name="rem">rem value.</param>
        /// <returns>absolute value in the userSpaceOnUse coordinate system.</returns>
        public static double GetCoordinateForUserSpaceOnUse(String attributeValue, double defaultValue, double start
            , double length, float em, float rem) {
            double absoluteValue;
            UnitValue unitValue = CssDimensionParsingUtils.ParseLengthValueToPt(attributeValue, em, rem);
            if (unitValue == null) {
                absoluteValue = defaultValue;
            }
            else {
                if (unitValue.GetUnitType() == UnitValue.PERCENT) {
                    absoluteValue = start + (length * unitValue.GetValue() / 100);
                }
                else {
                    absoluteValue = unitValue.GetValue();
                }
            }
            return absoluteValue;
        }

        /// <summary>Returns a value relative to the object bounding box.</summary>
        /// <remarks>
        /// Returns a value relative to the object bounding box.
        /// We should only call this method for attributes with coordinates relative to the object bounding rectangle.
        /// </remarks>
        /// <param name="attributeValue">attribute value to parse</param>
        /// <param name="defaultValue">this value will be returned if an error occurs while parsing the attribute value
        ///     </param>
        /// <returns>
        /// if
        /// <paramref name="attributeValue"/>
        /// is a percentage value, the given percentage of 1 will be returned.
        /// And if it's a valid value with a number, the number will be extracted from that value.
        /// </returns>
        public static double GetCoordinateForObjectBoundingBox(String attributeValue, double defaultValue) {
            if (CssTypesValidationUtils.IsPercentageValue(attributeValue)) {
                return CssDimensionParsingUtils.ParseRelativeValue(attributeValue, 1);
            }
            if (CssTypesValidationUtils.IsNumber(attributeValue) || CssTypesValidationUtils.IsMetricValue(attributeValue
                ) || CssTypesValidationUtils.IsRelativeValue(attributeValue)) {
                // if there is incorrect value metric, then we do not need to parse the value
                int unitsPosition = CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(attributeValue);
                if (unitsPosition > 0) {
                    // We want to ignore the unit type how this is done in the "Google Chrome" approach
                    // which treats the "abstract coordinate system" in the coordinate metric measure,
                    // i.e. for value '0.5cm' the top/left of the object bounding box would be (1cm, 1cm),
                    // for value '0.5em' the top/left of the object bounding box would be (1em, 1em) and etc.
                    // no null pointer should be thrown as determine
                    return CssDimensionParsingUtils.ParseDouble(attributeValue.JSubstring(0, unitsPosition)).Value;
                }
            }
            return defaultValue;
        }

        /// <summary>Calculate normalized diagonal length.</summary>
        /// <param name="context">svg draw context.</param>
        /// <returns>diagonal length in px.</returns>
        public static float CalculateNormalizedDiagonalLength(SvgDrawContext context) {
            float viewPortHeight = context.GetCurrentViewPort().GetHeight();
            float viewPortWidth = context.GetCurrentViewPort().GetWidth();
            return (float)(Math.Sqrt(viewPortHeight * viewPortHeight + viewPortWidth * viewPortWidth) / Math.Sqrt(2));
        }

        /// <summary>Returns the viewBox received after scaling and displacement given preserveAspectRatio.</summary>
        /// <param name="viewBox">
        /// parsed viewBox rectangle. It should be a valid
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        /// <param name="currentViewPort">
        /// current element view port. It should be a valid
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        /// <param name="align">
        /// the alignment value that indicates whether to force uniform scaling
        /// and, if so, the alignment method to use in case the aspect ratio of
        /// the viewBox doesn't match the aspect ratio of the viewport. If align
        /// is
        /// <see langword="null"/>
        /// or align is invalid (i.e. not in the predefined list),
        /// then the default logic with align = "xMidYMid", and meetOrSlice = "meet" would be used
        /// </param>
        /// <param name="meetOrSlice">
        /// the way to scale the viewBox. If meetOrSlice is not
        /// <see langword="null"/>
        /// and invalid,
        /// then the default logic with align = "xMidYMid"
        /// and meetOrSlice = "meet" would be used, if meetOrSlice is
        /// <see langword="null"/>
        /// then default "meet" value would be used with the specified align
        /// </param>
        /// <returns>
        /// the applied viewBox
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </returns>
        public static Rectangle ApplyViewBox(Rectangle viewBox, Rectangle currentViewPort, String align, String meetOrSlice
            ) {
            if (currentViewPort == null) {
                throw new ArgumentException(SvgExceptionMessageConstant.CURRENT_VIEWPORT_IS_NULL);
            }
            if (viewBox == null || viewBox.GetWidth() <= 0 || viewBox.GetHeight() <= 0) {
                throw new ArgumentException(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT);
            }
            if (align == null || (meetOrSlice != null && !SvgConstants.Values.MEET.Equals(meetOrSlice) && !SvgConstants.Values
                .SLICE.Equals(meetOrSlice))) {
                return ApplyViewBox(viewBox, currentViewPort, SvgConstants.Values.XMID_YMID, SvgConstants.Values.MEET);
            }
            double scaleWidth;
            double scaleHeight;
            if (SvgConstants.Values.NONE.EqualsIgnoreCase(align)) {
                scaleWidth = (double)currentViewPort.GetWidth() / (double)viewBox.GetWidth();
                scaleHeight = (double)currentViewPort.GetHeight() / (double)viewBox.GetHeight();
            }
            else {
                double scale = GetScaleWidthHeight(viewBox, currentViewPort, meetOrSlice);
                scaleWidth = scale;
                scaleHeight = scale;
            }
            // Apply scale for width and height.
            Rectangle appliedViewBox = new Rectangle(viewBox.GetX(), viewBox.GetY(), (float)((double)viewBox.GetWidth(
                ) * scaleWidth), (float)((double)viewBox.GetHeight() * scaleHeight));
            // Calculate offset.
            double minXOffset = (double)currentViewPort.GetX() - ((double)appliedViewBox.GetX() * scaleWidth);
            double minYOffset = (double)currentViewPort.GetY() - ((double)appliedViewBox.GetY() * scaleHeight);
            double midXOffset = (double)currentViewPort.GetX() + ((double)currentViewPort.GetWidth() / 2) - (((double)
                appliedViewBox.GetX() * scaleWidth) + ((double)appliedViewBox.GetWidth() / 2));
            double midYOffset = (double)currentViewPort.GetY() + ((double)currentViewPort.GetHeight() / 2) - (((double
                )appliedViewBox.GetY() * scaleHeight) + ((double)appliedViewBox.GetHeight() / 2));
            double maxXOffset = (double)currentViewPort.GetX() + (double)currentViewPort.GetWidth() - (((double)appliedViewBox
                .GetX() * scaleWidth) + (double)appliedViewBox.GetWidth());
            double maxYOffset = (double)currentViewPort.GetY() + (double)currentViewPort.GetHeight() - (((double)appliedViewBox
                .GetY() * scaleHeight) + (double)appliedViewBox.GetHeight());
            double xOffset;
            double yOffset;
            switch (align.ToLowerInvariant()) {
                case SvgConstants.Values.NONE:
                case SvgConstants.Values.XMIN_YMIN: {
                    xOffset = minXOffset;
                    yOffset = minYOffset;
                    break;
                }

                case SvgConstants.Values.XMIN_YMID: {
                    xOffset = minXOffset;
                    yOffset = midYOffset;
                    break;
                }

                case SvgConstants.Values.XMIN_YMAX: {
                    xOffset = minXOffset;
                    yOffset = maxYOffset;
                    break;
                }

                case SvgConstants.Values.XMID_YMIN: {
                    xOffset = midXOffset;
                    yOffset = minYOffset;
                    break;
                }

                case SvgConstants.Values.XMID_YMAX: {
                    xOffset = midXOffset;
                    yOffset = maxYOffset;
                    break;
                }

                case SvgConstants.Values.XMAX_YMIN: {
                    xOffset = maxXOffset;
                    yOffset = minYOffset;
                    break;
                }

                case SvgConstants.Values.XMAX_YMID: {
                    xOffset = maxXOffset;
                    yOffset = midYOffset;
                    break;
                }

                case SvgConstants.Values.XMAX_YMAX: {
                    xOffset = maxXOffset;
                    yOffset = maxYOffset;
                    break;
                }

                case SvgConstants.Values.XMID_YMID: {
                    xOffset = midXOffset;
                    yOffset = midYOffset;
                    break;
                }

                default: {
                    return ApplyViewBox(viewBox, currentViewPort, SvgConstants.Values.XMID_YMID, SvgConstants.Values.MEET);
                }
            }
            // Apply offset.
            appliedViewBox.MoveRight((float)xOffset);
            appliedViewBox.MoveUp((float)yOffset);
            // Apply scale for coordinates.
            appliedViewBox.SetX((float)((double)appliedViewBox.GetX() * scaleWidth));
            appliedViewBox.SetY((float)((double)appliedViewBox.GetY() * scaleHeight));
            return appliedViewBox;
        }

        private static double GetScaleWidthHeight(Rectangle viewBox, Rectangle currentViewPort, String meetOrSlice
            ) {
            double scaleWidth = (double)currentViewPort.GetWidth() / (double)viewBox.GetWidth();
            double scaleHeight = (double)currentViewPort.GetHeight() / (double)viewBox.GetHeight();
            if (SvgConstants.Values.SLICE.EqualsIgnoreCase(meetOrSlice)) {
                return Math.Max(scaleWidth, scaleHeight);
            }
            else {
                if (SvgConstants.Values.MEET.EqualsIgnoreCase(meetOrSlice) || meetOrSlice == null) {
                    return Math.Min(scaleWidth, scaleHeight);
                }
                else {
                    // This code should be unreachable. We check for incorrect cases
                    // in the applyViewBox method and instead use the default implementation (xMidYMid meet).
                    throw new InvalidOperationException(SvgExceptionMessageConstant.MEET_OR_SLICE_ARGUMENT_IS_INCORRECT);
                }
            }
        }
    }
}
