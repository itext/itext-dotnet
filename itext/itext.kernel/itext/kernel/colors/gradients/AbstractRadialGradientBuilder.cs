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
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>Base class for radial gradient builders implementations.</summary>
    public abstract class AbstractRadialGradientBuilder : AbstractGradientBuilder<RadialGradientPoint> {
        // tan(t/2), with t == 2 deg we have tan(t/2) = 0.017455
        private const double TAN_CONSTANT = 0.017455;

        /// <summary><inheritDoc/></summary>
        protected internal override RadialGradientPoint[] CreateCoordsForNewDomain(double[] newDomain, RadialGradientPoint
            [] baseVector) {
            double xDiff = baseVector[1].GetX() - baseVector[0].GetX();
            double yDiff = baseVector[1].GetY() - baseVector[0].GetY();
            double rDiff = baseVector[1].GetRadius() - baseVector[0].GetRadius();
            RadialGradientPoint[] targetCoords = new RadialGradientPoint[] { new RadialGradientPoint(baseVector[0]), new 
                RadialGradientPoint(baseVector[1]) };
            targetCoords[0].GetCenter().Move(xDiff * newDomain[0], yDiff * newDomain[0]);
            targetCoords[0].SetRadius(targetCoords[0].GetRadius() + rDiff * newDomain[0]);
            targetCoords[1].GetCenter().Move(xDiff * (newDomain[1] - 1), yDiff * (newDomain[1] - 1));
            targetCoords[1].SetRadius(targetCoords[1].GetRadius() + rDiff * (newDomain[1] - 1));
            return targetCoords;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override RadialGradientPoint[] CreateCoveringCoordinates(Rectangle targetBoundingBox) {
            // take one vertex as a center of both circles
            // and bigger circles should cover the whole rectangle, i.e. radius should cover the farthest corner
            Point center = new Point(targetBoundingBox.GetLeft(), targetBoundingBox.GetBottom());
            double radius = center.Distance(new Point(targetBoundingBox.GetRight(), targetBoundingBox.GetTop()));
            return new RadialGradientPoint[] { new RadialGradientPoint(center, 0d), new RadialGradientPoint(center, radius
                ) };
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double GetBaseVectorLength(RadialGradientPoint[] coordinates) {
            return coordinates[1].GetCenter().Distance(coordinates[0].GetCenter()) + Math.Abs(coordinates[1].GetRadius
                () - coordinates[0].GetRadius());
        }

        /// <summary><inheritDoc/></summary>
        protected internal override PdfArray CreateCoordsDictEntry(RadialGradientPoint[] coordsPoints) {
            System.Diagnostics.Debug.Assert(coordsPoints != null && coordsPoints.Length == 2);
            return new PdfArray(new double[] { coordsPoints[0].GetX(), coordsPoints[0].GetY(), coordsPoints[0].GetRadius
                (), coordsPoints[1].GetX(), coordsPoints[1].GetY(), coordsPoints[1].GetRadius() });
        }

        /// <summary><inheritDoc/></summary>
        protected internal override AbstractPdfShading CreatePdfShading(PdfColorSpace colorSpace, PdfArray coordinates
            , PdfArray coordinatesDomain, IPdfFunction stopsFunction) {
            return new PdfRadialShading(colorSpace, coordinates, coordinatesDomain, stopsFunction);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double[] ComputeCoveringDomain(RadialGradientPoint[] coords, Rectangle toCover
            ) {
            if (toCover == null) {
                return new double[] { 0d, 1d };
            }
            double originalCentersDistance = coords[0].Distance(coords[1]);
            // matching centers case
            if (IsZero(originalCentersDistance)) {
                return GetDomainForMatchingCenters(coords, toCover);
            }
            // For all other cases we will transform the plane to have base domain circles in points (0,0) and (1,0).
            // So that covering circles will have x coordinate equals to target domain.
            double scale = 1d / originalCentersDistance;
            AffineTransform transform = GetToIntervalTransform(coords[0].GetCenter(), coords[1].GetCenter(), scale);
            IList<Point> transformedRectVertices = new List<Point>(4);
            foreach (Point v in toCover.ToPointsArray()) {
                transformedRectVertices.Add(transform.Transform(v, null));
            }
            // radii on transformed plane
            double r0 = coords[0].GetRadius() * scale;
            double r1 = coords[1].GetRadius() * scale;
            double rDiffAbs = Math.Abs(r1 - r0);
            // four possible cases: rDiffAbs == 0 (lane), 0 < rDiffAbs < 1 (cone),
            // rDiffAbs == 1 (half-plane), rDiffAbs > 1 (full plane)
            if (IsZero(rDiffAbs - 1)) {
                // rDiffAbs == 1 (half-plane)
                return GetDomainForHalfPlaneCase(r0, r1, transformedRectVertices);
            }
            else {
                if (rDiffAbs > 1) {
                    // rDiffAbs > 1 (full plane)
                    return GetDomainForFullPlaneCase(r0, r1, transformedRectVertices);
                }
                else {
                    // rDiffAbs == 0 (lane), 0 < rDiffAbs < 1 (cone)
                    return GetDomainForConeCase(r0, r1, transformedRectVertices);
                }
            }
        }

        private static double[] GetDomainForHalfPlaneCase(double r0, double r1, IList<Point> rectVertices) {
            double rDiff = r1 - r0;
            // The method assumes that radii diff is 1 (i.e. all circles has one common touch point)
            System.Diagnostics.Debug.Assert(IsZero(Math.Abs(rDiff) - 1));
            System.Diagnostics.Debug.Assert(!rectVertices.IsEmpty());
            double xZeroRad = -1d * r0 / rDiff;
            bool hasCoveredVertex = false;
            bool hasUncoveredVertex = false;
            double xMin = xZeroRad;
            double xMax = xZeroRad;
            foreach (Point point in rectVertices) {
                double px = point.GetX();
                double denominator = 2 * r0 * rDiff + 2 * px;
                if (IsZero(denominator)) {
                    // With zero denominator the point is placed on non-covered edge of the surface.
                    // So we need infinite max domain.
                    hasUncoveredVertex = true;
                }
                else {
                    double py = point.GetY();
                    double xCandidate = (-1d * r0 * r0 + px * px + py * py) / denominator;
                    if (GetRadius(xCandidate, r0, r1) < 0) {
                        // uncovered half of the surface
                        hasUncoveredVertex = true;
                    }
                    else {
                        hasCoveredVertex = true;
                        xMin = Math.Min(xMin, xCandidate);
                        xMax = Math.Max(xMax, xCandidate);
                    }
                }
            }
            bool isIncreasingRadius = rDiff > 0;
            xMin = isIncreasingRadius ? xZeroRad : xMin;
            xMax = isIncreasingRadius ? xMax : xZeroRad;
            // Cases:
            // - hasUncoveredVertex == false, hasCoveredVertex == false: unreachable
            // - hasUncoveredVertex == false, hasCoveredVertex == true: all vertices are covered, we have valid xMax
            // - hasUncoveredVertex == false, hasCoveredVertex == true: all vertices are uncovered, xMax = xZeroRad
            // - hasUncoveredVertex == false, hasCoveredVertex == true: xMax should be equal to positive infinity
            if (hasUncoveredVertex && hasCoveredVertex) {
                // TODO: DEVSIX-10037 we should choose finite but big enough domain to cover the surface
                //  if we will have stops and domain reduction with max stops count in the future,
                //  then we can make `xMax = Double.POSITIVE_INFINITE;` here
                //  For now will try to choose finite xMax close enough to cover:
                double maxY = 0;
                foreach (Point point in rectVertices) {
                    double py = point.GetY();
                    maxY = Math.Abs(py) > Math.Abs(maxY) ? py : maxY;
                }
                double coveredSign = isIncreasingRadius ? 1d : -1d;
                // looking for px so that arc between (xZeroRad, 0) and (px, maxY) would correspond predefined t deg
                // formula: px = xZeroRad +/- maxY * tan(t/2)
                double px = xZeroRad + coveredSign * maxY * TAN_CONSTANT;
                double denominator = 2 * r0 * rDiff + 2 * px;
                double targetX = (-1d * r0 * r0 + px * px + maxY * maxY) / denominator;
                if (isIncreasingRadius) {
                    xMax = targetX;
                }
                else {
                    xMin = targetX;
                }
            }
            return new double[] { xMin, xMax };
        }

        private static double[] GetDomainForFullPlaneCase(double r0, double r1, IList<Point> rectVertices) {
            double rDiff = r1 - r0;
            // The method assumes that radii diff is greater than 1 (i.e. any circles covers all smaller circles)
            System.Diagnostics.Debug.Assert(Math.Abs(rDiff) > 1);
            double xZeroRad = -1d * r0 / rDiff;
            double xMin = xZeroRad;
            double xMax = xZeroRad;
            foreach (Point point in rectVertices) {
                double px = point.GetX();
                double py = point.GetY();
                // solving ax^2 + bx + c = 0;
                double a = rDiff * rDiff - 1d;
                double b = 2 * (r0 * rDiff + px);
                double c = r0 * r0 - px * px - py * py;
                double dSqrt = Math.Sqrt(b * b - 4 * a * c);
                double x1 = (-1 * b - dSqrt) / (2 * a);
                double x2 = (-1 * b + dSqrt) / (2 * a);
                if (GetRadius(x1, r0, r1) >= 0) {
                    xMin = Math.Min(xMin, x1);
                    xMax = Math.Max(xMax, x1);
                }
                if (GetRadius(x2, r0, r1) >= 0) {
                    xMin = Math.Min(xMin, x2);
                    xMax = Math.Max(xMax, x2);
                }
            }
            bool isIncreasingRadius = rDiff > 0;
            xMin = isIncreasingRadius ? xZeroRad : xMin;
            xMax = isIncreasingRadius ? xMax : xZeroRad;
            return new double[] { xMin, xMax };
        }

        private static double[] GetDomainForConeCase(double r0, double r1, IList<Point> rectVertices) {
            double rDiff = r1 - r0;
            // The method assumes that radii diff is smaller than 1 (i.e. cone or lane)
            System.Diagnostics.Debug.Assert(Math.Abs(rDiff) < 1);
            double vXMax = rectVertices[0].GetX();
            double vXMin = vXMax;
            for (int i = 1; i < rectVertices.Count; ++i) {
                vXMax = Math.Max(vXMax, rectVertices[i].GetX());
                vXMin = Math.Min(vXMin, rectVertices[i].GetX());
            }
            double xMax = (vXMax + r0) / (1 - rDiff);
            double xMin = (vXMin - r0) / (1 + rDiff);
            if (!IsZero(rDiff)) {
                // for zero diff case there is no xZeroRad value,
                // but both xMin and xMax should have non-negative radius (equal to r0)
                double xZeroRad = -1 * r0 / rDiff;
                if (GetRadius(xMin, r0, r1) < 0) {
                    xMin = xZeroRad;
                }
                if (GetRadius(xMax, r0, r1) < 0) {
                    xMax = xZeroRad;
                }
            }
            return new double[] { xMin, xMax };
        }

        private static double GetRadius(double x, double r0, double r1) {
            return r0 + x * (r1 - r0);
        }

        private static double[] GetDomainForMatchingCenters(RadialGradientPoint[] coords, Rectangle toCover) {
            // The method assumes that circles has identical centers
            System.Diagnostics.Debug.Assert(IsZero(coords[0].Distance(coords[1])));
            // First calculate min and max radii to cover the rectangle.
            Point center = coords[0].GetCenter();
            double minRadius = GetMinDistance(center, toCover);
            double maxRadius = 0.0;
            foreach (Point p in toCover.ToPointsArray()) {
                maxRadius = Math.Max(maxRadius, center.Distance(p));
            }
            // Second calculate the domain
            double domainStep = coords[1].GetRadius() - coords[0].GetRadius();
            double maxRadDomain = (maxRadius - coords[0].GetRadius()) / domainStep;
            double minRadDomain = (minRadius - coords[0].GetRadius()) / domainStep;
            double domainStart = Math.Min(minRadDomain, maxRadDomain);
            double domainEnd = Math.Max(minRadDomain, maxRadDomain);
            return new double[] { domainStart, domainEnd };
        }

        private static double GetMinDistance(Point from, Rectangle to) {
            double dx = 0.0;
            if (from.GetX() < to.GetLeft()) {
                dx = to.GetLeft() - from.GetX();
            }
            else {
                if (from.GetX() > to.GetRight()) {
                    dx = from.GetX() - to.GetRight();
                }
            }
            double dy = 0.0;
            if (from.GetY() < to.GetBottom()) {
                dy = to.GetBottom() - from.GetY();
            }
            else {
                if (from.GetY() > to.GetTop()) {
                    dy = from.GetY() - to.GetTop();
                }
            }
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
