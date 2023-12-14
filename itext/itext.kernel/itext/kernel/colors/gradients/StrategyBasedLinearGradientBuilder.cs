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

namespace iText.Kernel.Colors.Gradients {
    /// <summary>
    /// The linear gradient builder with automatic coordinates vector evaluation for the target filled
    /// area based on configured strategy
    /// </summary>
    public class StrategyBasedLinearGradientBuilder : AbstractLinearGradientBuilder {
        private double rotateVectorAngle = 0d;

        private StrategyBasedLinearGradientBuilder.GradientStrategy gradientStrategy = StrategyBasedLinearGradientBuilder.GradientStrategy
            .TO_BOTTOM;

        private bool isCentralRotationAngleStrategy = false;

        /// <summary>Create a new instance of the builder</summary>
        public StrategyBasedLinearGradientBuilder() {
        }

        /// <summary>
        /// Set the strategy to use the minimal coordinates vector that passes through the central point
        /// of the target rectangle area, rotated by the specified amount of radians counter clockwise
        /// and covers the area to be filled.
        /// </summary>
        /// <remarks>
        /// Set the strategy to use the minimal coordinates vector that passes through the central point
        /// of the target rectangle area, rotated by the specified amount of radians counter clockwise
        /// and covers the area to be filled. Zero angle corresponds to the vector from bottom to top.
        /// </remarks>
        /// <param name="radians">the radians value to rotate the coordinates vector</param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.StrategyBasedLinearGradientBuilder SetGradientDirectionAsCentralRotationAngle
            (double radians) {
            this.rotateVectorAngle = radians;
            this.isCentralRotationAngleStrategy = true;
            return this;
        }

        /// <summary>Set the strategy to predefined one</summary>
        /// <param name="gradientStrategy">the strategy to set</param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.StrategyBasedLinearGradientBuilder SetGradientDirectionAsStrategy
            (StrategyBasedLinearGradientBuilder.GradientStrategy gradientStrategy) {
            this.gradientStrategy = gradientStrategy != null ? gradientStrategy : StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM;
            this.isCentralRotationAngleStrategy = false;
            return this;
        }

        /// <summary>Get the last set rotate vector angle</summary>
        /// <returns>the last set rotate vector angle</returns>
        public virtual double GetRotateVectorAngle() {
            return rotateVectorAngle;
        }

        /// <summary>Get the last set predefined strategy</summary>
        /// <returns>the last set predefined strategy</returns>
        public virtual StrategyBasedLinearGradientBuilder.GradientStrategy GetGradientStrategy() {
            return gradientStrategy;
        }

        /// <summary>Is the central rotation angle strategy was set last</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the last strategy that has been set is a custom rotation angle
        /// </returns>
        public virtual bool IsCentralRotationAngleStrategy() {
            return isCentralRotationAngleStrategy;
        }

        protected internal override Point[] GetGradientVector(Rectangle targetBoundingBox, AffineTransform contextTransform
            ) {
            if (targetBoundingBox == null) {
                return null;
            }
            return this.isCentralRotationAngleStrategy ? BuildCentralRotationCoordinates(targetBoundingBox, this.rotateVectorAngle
                ) : BuildCoordinatesWithGradientStrategy(targetBoundingBox, this.gradientStrategy);
        }

        private static Point[] BuildCoordinatesWithGradientStrategy(Rectangle targetBoundingBox, StrategyBasedLinearGradientBuilder.GradientStrategy
             gradientStrategy) {
            double xCenter = targetBoundingBox.GetX() + targetBoundingBox.GetWidth() / 2;
            double yCenter = targetBoundingBox.GetY() + targetBoundingBox.GetHeight() / 2;
            switch (gradientStrategy) {
                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP: {
                    return CreateCoordinates(xCenter, targetBoundingBox.GetBottom(), xCenter, targetBoundingBox.GetTop());
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_LEFT: {
                    return CreateCoordinates(targetBoundingBox.GetRight(), yCenter, targetBoundingBox.GetLeft(), yCenter);
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_RIGHT: {
                    return CreateCoordinates(targetBoundingBox.GetLeft(), yCenter, targetBoundingBox.GetRight(), yCenter);
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_LEFT: {
                    return BuildToCornerCoordinates(targetBoundingBox, new Point(targetBoundingBox.GetRight(), targetBoundingBox
                        .GetTop()));
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT: {
                    return BuildToCornerCoordinates(targetBoundingBox, new Point(targetBoundingBox.GetRight(), targetBoundingBox
                        .GetBottom()));
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_RIGHT: {
                    return BuildToCornerCoordinates(targetBoundingBox, new Point(targetBoundingBox.GetLeft(), targetBoundingBox
                        .GetBottom()));
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT: {
                    return BuildToCornerCoordinates(targetBoundingBox, new Point(targetBoundingBox.GetLeft(), targetBoundingBox
                        .GetTop()));
                }

                case StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM:
                default: {
                    // default case is equal to TO_BOTTOM
                    return CreateCoordinates(xCenter, targetBoundingBox.GetTop(), xCenter, targetBoundingBox.GetBottom());
                }
            }
        }

        private static Point[] BuildCentralRotationCoordinates(Rectangle targetBoundingBox, double angle) {
            double xCenter = targetBoundingBox.GetX() + targetBoundingBox.GetWidth() / 2;
            AffineTransform rotateInstance = AffineTransform.GetRotateInstance(angle, xCenter, targetBoundingBox.GetY(
                ) + targetBoundingBox.GetHeight() / 2);
            return BuildCoordinates(targetBoundingBox, rotateInstance);
        }

        private static Point[] BuildToCornerCoordinates(Rectangle targetBoundingBox, Point gradientCenterLineRightCorner
            ) {
            AffineTransform transform = BuildToCornerTransform(new Point(targetBoundingBox.GetX() + targetBoundingBox.
                GetWidth() / 2, targetBoundingBox.GetY() + targetBoundingBox.GetHeight() / 2), gradientCenterLineRightCorner
                );
            return BuildCoordinates(targetBoundingBox, transform);
        }

        private static AffineTransform BuildToCornerTransform(Point center, Point gradientCenterLineRightCorner) {
            double scale = 1d / (center.Distance(gradientCenterLineRightCorner));
            double sin = (gradientCenterLineRightCorner.GetY() - center.GetY()) * scale;
            double cos = (gradientCenterLineRightCorner.GetX() - center.GetX()) * scale;
            if (Math.Abs(cos) < ZERO_EPSILON) {
                cos = 0d;
                sin = sin > 0d ? 1d : -1d;
            }
            else {
                if (Math.Abs(sin) < ZERO_EPSILON) {
                    sin = 0d;
                    cos = cos > 0d ? 1d : -1d;
                }
            }
            double m02 = center.GetX() * (1d - cos) + center.GetY() * sin;
            double m12 = center.GetY() * (1d - cos) - center.GetX() * sin;
            return new AffineTransform(cos, sin, -sin, cos, m02, m12);
        }

        private static Point[] BuildCoordinates(Rectangle targetBoundingBox, AffineTransform transformation) {
            double xCenter = targetBoundingBox.GetX() + targetBoundingBox.GetWidth() / 2;
            Point start = transformation.Transform(new Point(xCenter, targetBoundingBox.GetBottom()), null);
            Point end = transformation.Transform(new Point(xCenter, targetBoundingBox.GetTop()), null);
            Point[] baseVector = new Point[] { start, end };
            double[] targetDomain = EvaluateCoveringDomain(baseVector, targetBoundingBox);
            return CreateCoordinatesForNewDomain(targetDomain, baseVector);
        }

        private static Point[] CreateCoordinates(double x1, double y1, double x2, double y2) {
            return new Point[] { new Point(x1, y1), new Point(x2, y2) };
        }

        /// <summary>Specifies the predefined strategies</summary>
        public enum GradientStrategy {
            /// <summary>Gradient vector from the middle of the top side to the middle of the bottom side</summary>
            TO_BOTTOM,
            /// <summary>
            /// Evaluates the gradient vector in such way that the first color would be painted
            /// at the top right corner, the last one - at the bottom left corner and the middle color
            /// line would pass through left corners
            /// </summary>
            TO_BOTTOM_LEFT,
            /// <summary>
            /// Evaluates the gradient vector in such way that the first color would be painted
            /// at the top left corner, the last one - at the bottom right corner and the middle color
            /// line would pass through left corners
            /// </summary>
            TO_BOTTOM_RIGHT,
            /// <summary>Gradient vector from the middle of the right side to the middle of the left side</summary>
            TO_LEFT,
            /// <summary>Gradient vector from the middle of the left side to the middle of the right side</summary>
            TO_RIGHT,
            /// <summary>Gradient vector from the middle of the bottom side to the middle of the top side</summary>
            TO_TOP,
            /// <summary>
            /// Evaluates the gradient vector in such way that the first color would be painted
            /// at the bottom right corner, the last one - at the top left corner and the middle color
            /// line would pass through left corners
            /// </summary>
            TO_TOP_LEFT,
            /// <summary>
            /// Evaluates the gradient vector in such way that the first color would be painted
            /// at the bottom left corner, the last one - at the top right corner and the middle color
            /// line would pass through left corners
            /// </summary>
            TO_TOP_RIGHT
        }
    }
}
