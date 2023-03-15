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

namespace iText.Layout.Minmaxwidth {
    /// <summary>Class for min-max-width of rotated elements.</summary>
    /// <remarks>
    /// Class for min-max-width of rotated elements.
    /// Also contains heuristic methods for it calculation based on the assumption that area of element stays the same
    /// when we try to layout it with different available width (available width is between min-width and max-width).
    /// </remarks>
    public class RotationMinMaxWidth : MinMaxWidth {
        private double minWidthOrigin;

        private double maxWidthOrigin;

        private double minWidthHeight;

        private double maxWidthHeight;

        /// <summary>Create new instance</summary>
        /// <param name="minWidth">min-width of rotated element</param>
        /// <param name="maxWidth">max-width of rotated element</param>
        /// <param name="minWidthOrigin">the width of not rotated element, that will have min-width after rotation</param>
        /// <param name="maxWidthOrigin">the width of not rotated element, that will have max-width after rotation</param>
        /// <param name="minWidthHeight">the height of rotated element, that have min-width as its rotated width</param>
        /// <param name="maxWidthHeight">the height of rotated element, that have min-width as its rotated width</param>
        public RotationMinMaxWidth(double minWidth, double maxWidth, double minWidthOrigin, double maxWidthOrigin, 
            double minWidthHeight, double maxWidthHeight)
            : base((float)minWidth, (float)maxWidth, 0) {
            this.maxWidthOrigin = maxWidthOrigin;
            this.minWidthOrigin = minWidthOrigin;
            this.minWidthHeight = minWidthHeight;
            this.maxWidthHeight = maxWidthHeight;
        }

        public virtual double GetMinWidthOrigin() {
            return minWidthOrigin;
        }

        public virtual double GetMaxWidthOrigin() {
            return maxWidthOrigin;
        }

        public virtual double GetMinWidthHeight() {
            return minWidthHeight;
        }

        public virtual double GetMaxWidthHeight() {
            return maxWidthHeight;
        }

        /// <summary>
        /// Heuristic method, based on the assumption that area of element stays the same, when we try to
        /// layout it with different available width (available width is between min-width and max-width).
        /// </summary>
        /// <param name="angle">rotation angle in radians</param>
        /// <param name="area">the constant area</param>
        /// <param name="elementMinMaxWidth">NOT rotated element min-max-width</param>
        /// <returns>possible min-max-width of element after rotation</returns>
        public static iText.Layout.Minmaxwidth.RotationMinMaxWidth Calculate(double angle, double area, MinMaxWidth
             elementMinMaxWidth) {
            RotationMinMaxWidth.WidthFunction function = new RotationMinMaxWidth.WidthFunction(angle, area);
            return Calculate(function, elementMinMaxWidth.GetMinWidth(), elementMinMaxWidth.GetMaxWidth());
        }

        /// <summary>
        /// Heuristic method, based on the assumption that area of element stays the same, when we try to
        /// layout it with different available width (available width is between min-width and max-width).
        /// </summary>
        /// <param name="angle">rotation angle in radians</param>
        /// <param name="area">the constant area</param>
        /// <param name="elementMinMaxWidth">NOT rotated element min-max-width</param>
        /// <param name="availableWidth">the maximum width of area the element will occupy after rotation.</param>
        /// <returns>possible min-max-width of element after rotation</returns>
        public static iText.Layout.Minmaxwidth.RotationMinMaxWidth Calculate(double angle, double area, MinMaxWidth
             elementMinMaxWidth, double availableWidth) {
            RotationMinMaxWidth.WidthFunction function = new RotationMinMaxWidth.WidthFunction(angle, area);
            RotationMinMaxWidth.WidthFunction.Interval validArguments = function.GetValidOriginalWidths(availableWidth
                );
            if (validArguments == null) {
                return null;
            }
            double xMin = Math.Max(elementMinMaxWidth.GetMinWidth(), validArguments.GetMin());
            double xMax = Math.Min(elementMinMaxWidth.GetMaxWidth(), validArguments.GetMax());
            if (xMax < xMin) {
                //Initially the null was returned in this case, but this result in old layout logic that looks worse in most cases.
                //The difference between min and max is not that big and not critical.
                double rotatedWidth = function.GetRotatedWidth(xMin);
                double rotatedHeight = function.GetRotatedHeight(xMin);
                return new iText.Layout.Minmaxwidth.RotationMinMaxWidth(rotatedWidth, rotatedWidth, xMin, xMin, rotatedHeight
                    , rotatedHeight);
            }
            return Calculate(function, xMin, xMax);
        }

        /// <summary>Utility method for calculating rotated width of area in a similar way to other calculations in this class.
        ///     </summary>
        /// <param name="area">the initial area</param>
        /// <param name="angle">the rotation angle in radians</param>
        /// <returns>width of rotated area</returns>
        public static double CalculateRotatedWidth(Rectangle area, double angle) {
            return area.GetWidth() * Cos(angle) + area.GetHeight() * Sin(angle);
        }

        /// <summary>This method use derivative of function defined on interval: [xMin, xMax] to find its local minimum and maximum.
        ///     </summary>
        /// <remarks>
        /// This method use derivative of function defined on interval: [xMin, xMax] to find its local minimum and maximum.
        /// It also calculate other handy values needed for the creation of
        /// <see cref="RotationMinMaxWidth"/>.
        /// </remarks>
        /// <param name="func">
        /// the
        /// <see cref="WidthFunction.GetRotatedWidth(double)"/>
        /// of this instance is used as analysed function
        /// </param>
        /// <param name="xMin">the smallest possible value of function argument</param>
        /// <param name="xMax">the biggest possible value of function argument</param>
        /// <returns>
        /// the calculated
        /// <see cref="RotationMinMaxWidth"/>
        /// </returns>
        private static iText.Layout.Minmaxwidth.RotationMinMaxWidth Calculate(RotationMinMaxWidth.WidthFunction func
            , double xMin, double xMax) {
            double minWidthOrigin;
            double maxWidthOrigin;
            //Derivative sign change point
            double x0 = func.GetWidthDerivativeZeroPoint();
            //The point x0 may be in three different positions in relation to function interval.
            if (x0 < xMin) {
                //The function is decreasing in this case on whole interval so the local mim and max are on interval borders
                minWidthOrigin = xMin;
                maxWidthOrigin = xMax;
            }
            else {
                if (x0 > xMax) {
                    //The function is increasing in this case on whole interval so the local mim and max are on interval borders
                    minWidthOrigin = xMax;
                    maxWidthOrigin = xMin;
                }
                else {
                    //The function derivative changes its sign from negative to positive on function interval in point x0,
                    //so its local min is x0, and its local maximum is on one of interval borders
                    minWidthOrigin = x0;
                    maxWidthOrigin = func.GetRotatedWidth(xMax) > func.GetRotatedWidth(xMin) ? xMax : xMin;
                }
            }
            return new iText.Layout.Minmaxwidth.RotationMinMaxWidth(func.GetRotatedWidth(minWidthOrigin), func.GetRotatedWidth
                (maxWidthOrigin), minWidthOrigin, maxWidthOrigin, func.GetRotatedHeight(minWidthOrigin), func.GetRotatedHeight
                (maxWidthOrigin));
        }

        private static double Sin(double angle) {
            return CorrectSinCos(Math.Abs((Math.Sin(angle))));
        }

        private static double Cos(double angle) {
            return CorrectSinCos(Math.Abs((Math.Cos(angle))));
        }

        private static double CorrectSinCos(double value) {
            if (MinMaxWidthUtils.IsEqual(value, 0)) {
                return 0;
            }
            else {
                if (MinMaxWidthUtils.IsEqual(value, 1)) {
                    return 1;
                }
            }
            return value;
        }

        /// <summary>
        /// Class that represents functions used, for calculation of width of element after rotation
        /// based on it's NOT rotated width and assumption, that area of element stays the same when
        /// we try to layout it with different available width.
        /// </summary>
        /// <remarks>
        /// Class that represents functions used, for calculation of width of element after rotation
        /// based on it's NOT rotated width and assumption, that area of element stays the same when
        /// we try to layout it with different available width.
        /// Contains handy methods for function analysis.
        /// </remarks>
        private class WidthFunction {
            private double sin;

            private double cos;

            private double area;

            /// <summary>Create new instance</summary>
            /// <param name="angle">rotation angle in radians</param>
            /// <param name="area">the constant area</param>
            public WidthFunction(double angle, double area) {
                this.sin = Sin(angle);
                this.cos = Cos(angle);
                this.area = area;
            }

            /// <summary>Function used for width calculations of rotated element.</summary>
            /// <remarks>Function used for width calculations of rotated element. This function is continuous on interval: (0, Infinity)
            ///     </remarks>
            /// <param name="x">width value of NOT rotated element</param>
            /// <returns>width of rotated element</returns>
            public virtual double GetRotatedWidth(double x) {
                return x * cos + area * sin / x;
            }

            /// <summary>Function used for height calculations of rotated element.</summary>
            /// <remarks>Function used for height calculations of rotated element. This function is continuous on interval: (0, Infinity)
            ///     </remarks>
            /// <param name="x">width value of NOT rotated element</param>
            /// <returns>width of rotated element</returns>
            public virtual double GetRotatedHeight(double x) {
                return x * sin + area * cos / x;
            }

            /// <summary>Get's possible values of NOT rotated width of all element that have therer rotated width less that availableWidth
            ///     </summary>
            /// <param name="availableWidth">the highest possible width of rotated element.</param>
            /// <returns>interval that specify biggest and smallest possible values of NOT rotated width of such elements.
            ///     </returns>
            public virtual RotationMinMaxWidth.WidthFunction.Interval GetValidOriginalWidths(double availableWidth) {
                double minWidth;
                double maxWidth;
                if (cos == 0) {
                    minWidth = area * sin / availableWidth;
                    maxWidth = MinMaxWidthUtils.GetInfWidth();
                }
                else {
                    if (sin == 0) {
                        minWidth = 0;
                        maxWidth = availableWidth / cos;
                    }
                    else {
                        double D = availableWidth * availableWidth - 4 * area * sin * cos;
                        if (D < 0) {
                            return null;
                        }
                        minWidth = (availableWidth - Math.Sqrt(D)) / (2 * cos);
                        maxWidth = (availableWidth + Math.Sqrt(D)) / (2 * cos);
                    }
                }
                return new RotationMinMaxWidth.WidthFunction.Interval(minWidth, maxWidth);
            }

            /// <summary>
            /// Gets the argument of
            /// <see cref="GetRotatedWidth(double)"/>
            /// that results in zero derivative.
            /// </summary>
            /// <remarks>
            /// Gets the argument of
            /// <see cref="GetRotatedWidth(double)"/>
            /// that results in zero derivative.
            /// In case we have
            /// <see cref="sin"/>
            /// <c>== 0</c>
            /// or
            /// <see cref="sin"/>
            /// <c>== 0</c>
            /// the function doesn't have
            /// zero derivative on defined interval, but value returned by this method fits well in the calculations above.
            /// </remarks>
            /// <returns>
            /// the argument of
            /// <see cref="GetRotatedWidth(double)"/>
            /// that results in zero derivative
            /// </returns>
            public virtual double GetWidthDerivativeZeroPoint() {
                return Math.Sqrt(area * sin / cos);
            }

            public class Interval {
                private double min;

                private double max;

                public Interval(double min, double max) {
                    this.min = min;
                    this.max = max;
                }

                public virtual double GetMin() {
                    return min;
                }

                public virtual double GetMax() {
                    return max;
                }
            }
        }
    }
}
