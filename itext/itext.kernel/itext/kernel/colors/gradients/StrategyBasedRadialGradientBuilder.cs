/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Datastructures;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>
    /// The radial gradient builder with automatic end circle (or ellipse) evaluation for the target filled
    /// area based on configured strategy.
    /// </summary>
    /// <remarks>
    /// The radial gradient builder with automatic end circle (or ellipse) evaluation for the target filled
    /// area based on configured strategy. Start center would be equal to end center, radius would be 0.
    /// </remarks>
    public class StrategyBasedRadialGradientBuilder : AbstractRadialGradientBuilder {
        // center point
        private bool leftToRight = true;

        private double xOffset = 0.5d;

        private bool xOffsetRelative = true;

        private bool bottomToTop = false;

        private double yOffset = 0.5d;

        private bool yOffsetRelative = true;

        // radius
        // manual radius definition based on target bbox
        private double xRadius = 0d;

        private bool xRadiusRelative = false;

        private double yRadius = 0d;

        private bool yRadiusRelative = false;

        // from center based radius definition
        private StrategyBasedRadialGradientBuilder.GradientStrategy gradientStrategy = StrategyBasedRadialGradientBuilder.GradientStrategy
            .FARTHEST_CORNER;

        private bool isCircular = false;

        // whether from center based strategy to use or manual
        private bool isFromCenterStrategy = true;

        /// <summary>Constructs the builder instance</summary>
        public StrategyBasedRadialGradientBuilder() {
        }

        // empty constructor
        /// <summary>Specifies the strategy to determine center point.</summary>
        /// <param name="leftToRight">
        /// if
        /// <see langword="true"/>
        /// then X offset would be added to the left side X value,
        /// if
        /// <see langword="false"/>
        /// then X offset would be subtracted from the right side X value
        /// </param>
        /// <param name="xOffset">the offset value to add/subtract for X</param>
        /// <param name="xOffsetRelative">
        /// if
        /// <see langword="true"/>
        /// then X offset treated as relative to bbox width,
        /// if
        /// <see langword="false"/>
        /// then X offset treated as absolute
        /// </param>
        /// <param name="bottomToTop">
        /// if
        /// <see langword="true"/>
        /// then Y offset would be subtracted from the top side Y value,
        /// if
        /// <see langword="false"/>
        /// then Y offset would be added to the bottom side Y value
        /// </param>
        /// <param name="yOffset">the offset value to add/subtract for Y</param>
        /// <param name="yOffsetRelative">
        /// if
        /// <see langword="true"/>
        /// then Y offset treated as relative to bbox height,
        /// if
        /// <see langword="false"/>
        /// then Y offset treated as absolute
        /// </param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.StrategyBasedRadialGradientBuilder SetCenterStrategy(bool leftToRight
            , double xOffset, bool xOffsetRelative, bool bottomToTop, double yOffset, bool yOffsetRelative) {
            this.leftToRight = leftToRight;
            this.xOffset = xOffset;
            this.xOffsetRelative = xOffsetRelative;
            this.bottomToTop = bottomToTop;
            this.yOffset = yOffset;
            this.yOffsetRelative = yOffsetRelative;
            return this;
        }

        /// <summary>Set the strategy to calculate radius based on bounding box dimensions.</summary>
        /// <param name="xRadius">X radius value</param>
        /// <param name="xRadiusRelative">
        /// if
        /// <see langword="true"/>
        /// then X radius treated as relative to bbox width,
        /// if
        /// <see langword="false"/>
        /// then X radius treated as absolute
        /// </param>
        /// <param name="yRadius">Y radius value</param>
        /// <param name="yRadiusRelative">
        /// if
        /// <see langword="true"/>
        /// then Y radius treated as relative to bbox height,
        /// if
        /// <see langword="false"/>
        /// then Y radius treated as absolute
        /// </param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.StrategyBasedRadialGradientBuilder SetRadiusRelativeToBoundingBoxSize
            (double xRadius, bool xRadiusRelative, double yRadius, bool yRadiusRelative) {
            this.xRadius = xRadius;
            this.xRadiusRelative = xRadiusRelative;
            this.yRadius = yRadius;
            this.yRadiusRelative = yRadiusRelative;
            this.isFromCenterStrategy = false;
            return this;
        }

        /// <summary>Set the strategy to predefined one from gradient center.</summary>
        /// <param name="isCircular">whether the gradient strategy to be applied for circular or elliptical gradient spreading
        ///     </param>
        /// <param name="gradientStrategy">strategy to be used for calculating target radius from gradient center</param>
        /// <returns>the current builder instance</returns>
        public virtual iText.Kernel.Colors.Gradients.StrategyBasedRadialGradientBuilder SetRadiusFromCenterStrategy
            (bool isCircular, StrategyBasedRadialGradientBuilder.GradientStrategy gradientStrategy) {
            this.isCircular = isCircular;
            this.gradientStrategy = gradientStrategy != null ? gradientStrategy : StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER;
            this.isFromCenterStrategy = true;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override Tuple2<RadialGradientPoint[], AffineTransform> GetGradientVectorWithTransform(
            Rectangle targetBoundingBox, AffineTransform contextTransform) {
            Tuple2<RadialGradientPoint, AffineTransform> coordsWithTransform = EvaluateCoordAndTransform(targetBoundingBox
                );
            RadialGradientPoint target = coordsWithTransform.GetFirst();
            RadialGradientPoint[] vector = target == null ? null : new RadialGradientPoint[] { new RadialGradientPoint
                (target.GetCenter(), 0), target };
            return new Tuple2<RadialGradientPoint[], AffineTransform>(vector, coordsWithTransform.GetSecond());
        }

        private Tuple2<RadialGradientPoint, AffineTransform> EvaluateCoordAndTransform(Rectangle targetBoundingBox
            ) {
            if (targetBoundingBox == null) {
                return new Tuple2<RadialGradientPoint, AffineTransform>(null, null);
            }
            double cX = EvaluateValueOnSegment(targetBoundingBox.GetLeft(), targetBoundingBox.GetRight(), leftToRight, 
                xOffset, xOffsetRelative);
            double cY = EvaluateValueOnSegment(targetBoundingBox.GetBottom(), targetBoundingBox.GetTop(), bottomToTop, 
                yOffset, yOffsetRelative);
            Point center = new Point(cX, cY);
            double[] radius = isFromCenterStrategy ? GetRadiusForCenterBasedStrategy(targetBoundingBox, center) : GetRadiusForManualStrategy
                (targetBoundingBox);
            double rX = radius[0];
            double rY = radius[1];
            // if any of radii is 0 or negative, then normalize both to 0
            if (IsZero(rX) || IsZero(rY)) {
                rX = 0d;
                rY = 0d;
            }
            AffineTransform transform = null;
            if (!IsZero(rY - rX)) {
                // ellipse case
                transform = new AffineTransform();
                transform.Scale(1.0d, rY / rX);
                try {
                    center = transform.InverseTransform(center, null);
                }
                catch (NoninvertibleTransformException e) {
                    throw new PdfException(e.Message, e);
                }
            }
            return new Tuple2<RadialGradientPoint, AffineTransform>(new RadialGradientPoint(center, rX), transform);
        }

        private double[] GetRadiusForManualStrategy(Rectangle targetBoundingBox) {
            double rX = EvaluateValueOnSegment(0, targetBoundingBox.GetWidth(), true, xRadius, xRadiusRelative);
            double rY = EvaluateValueOnSegment(0, targetBoundingBox.GetHeight(), true, yRadius, yRadiusRelative);
            return new double[] { rX, rY };
        }

        private double[] GetRadiusForCenterBasedStrategy(Rectangle targetBoundingBox, Point center) {
            switch (gradientStrategy) {
                case StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_SIDE: {
                    return EvaluateClosestSideRadius(targetBoundingBox, center);
                }

                case StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_CORNER: {
                    return EvaluateClosestCornerRadius(targetBoundingBox, center);
                }

                case StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_SIDE: {
                    return EvaluateFarthestSideRadius(targetBoundingBox, center);
                }

                case StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER:
                default: {
                    // default case is equal to FARTHEST_CORNER
                    return EvaluateFarthestCornerRadius(targetBoundingBox, center);
                }
            }
        }

        private double[] EvaluateClosestSideRadius(Rectangle targetBoundingBox, Point center) {
            double leftDist = Math.Abs(center.GetX() - targetBoundingBox.GetLeft());
            double rightDist = Math.Abs(center.GetX() - targetBoundingBox.GetRight());
            double bottomDist = Math.Abs(center.GetY() - targetBoundingBox.GetBottom());
            double topDist = Math.Abs(center.GetY() - targetBoundingBox.GetTop());
            double horizontalClosest = Math.Min(leftDist, rightDist);
            double verticalClosest = Math.Min(bottomDist, topDist);
            if (isCircular) {
                double closestDist = Math.Min(horizontalClosest, verticalClosest);
                return new double[] { closestDist, closestDist };
            }
            else {
                return new double[] { horizontalClosest, verticalClosest };
            }
        }

        private double[] EvaluateFarthestSideRadius(Rectangle targetBoundingBox, Point center) {
            double leftDist = Math.Abs(center.GetX() - targetBoundingBox.GetLeft());
            double rightDist = Math.Abs(center.GetX() - targetBoundingBox.GetRight());
            double bottomDist = Math.Abs(center.GetY() - targetBoundingBox.GetBottom());
            double topDist = Math.Abs(center.GetY() - targetBoundingBox.GetTop());
            double horizontalFarthest = Math.Max(leftDist, rightDist);
            double verticalFarthest = Math.Max(bottomDist, topDist);
            if (isCircular) {
                double farthestDist = Math.Max(horizontalFarthest, verticalFarthest);
                return new double[] { farthestDist, farthestDist };
            }
            else {
                return new double[] { horizontalFarthest, verticalFarthest };
            }
        }

        private double[] EvaluateClosestCornerRadius(Rectangle targetBoundingBox, Point center) {
            Point[] vertices = targetBoundingBox.ToPointsArray();
            Point closestCorner = vertices[0];
            for (int i = 1; i < vertices.Length; ++i) {
                if (center.Distance(closestCorner) > center.Distance(vertices[i])) {
                    closestCorner = vertices[i];
                }
            }
            return EvaluateRadiusForCorner(center, closestCorner);
        }

        private double[] EvaluateFarthestCornerRadius(Rectangle targetBoundingBox, Point center) {
            Point[] vertices = targetBoundingBox.ToPointsArray();
            Point farthestCorner = vertices[0];
            for (int i = 1; i < vertices.Length; ++i) {
                if (center.Distance(farthestCorner) < center.Distance(vertices[i])) {
                    farthestCorner = vertices[i];
                }
            }
            return EvaluateRadiusForCorner(center, farthestCorner);
        }

        private double[] EvaluateRadiusForCorner(Point center, Point corner) {
            if (isCircular) {
                double distance = center.Distance(corner);
                return new double[] { distance, distance };
            }
            else {
                double xDiff = Math.Abs(corner.GetX() - center.GetX());
                double yDiff = Math.Abs(corner.GetY() - center.GetY());
                double aspectRatio = yDiff / xDiff;
                double xR = Math.Sqrt(xDiff * xDiff + (yDiff / aspectRatio) * (yDiff / aspectRatio));
                double yR = aspectRatio * xR;
                return new double[] { xR, yR };
            }
        }

        private static double EvaluateValueOnSegment(double segmentStart, double segmentEnd, bool isFromStart, double
             offset, bool offsetRelative) {
            double absoluteOffset = offsetRelative ? (segmentEnd - segmentStart) * offset : offset;
            return isFromStart ? segmentStart + absoluteOffset : segmentEnd - absoluteOffset;
        }

        /// <summary>Specifies the predefined strategies</summary>
        public enum GradientStrategy {
            /// <summary>
            /// Circle radius equal to the closest side distance from center,
            /// ellipse radii equal to the closest horizontal and vertical sides from center.
            /// </summary>
            CLOSEST_SIDE,
            /// <summary>
            /// Circle radius equal to the closest corner distance from center,
            /// ellipse passes through the closest corner from center
            /// while radii aspect ratio is the same as for CLOSEST_SIDE strategy for ellipse.
            /// </summary>
            CLOSEST_CORNER,
            /// <summary>
            /// Circle radius equal to the farthest side distance from center,
            /// ellipse radii equal to the farthest horizontal and vertical sides from center.
            /// </summary>
            FARTHEST_SIDE,
            /// <summary>
            /// Circle radius equal to the farthest corner distance from center,
            /// ellipse passes through the farthest corner from center
            /// while radii aspect ratio is the same as for FARTHEST_SIDE strategy for ellipse.
            /// </summary>
            FARTHEST_CORNER
        }
    }
}
