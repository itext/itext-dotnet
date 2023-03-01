/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>Base class for linear gradient builders implementations.</summary>
    /// <remarks>
    /// Base class for linear gradient builders implementations.
    /// <para />
    /// Color transitions for linear gradients are defined by a series of color stops along a gradient
    /// vector. A gradient normal defines how the colors in a vector are painted to the surface. For
    /// a linear gradient, a normal is a line perpendicular to the vector.
    /// <para />
    /// Contains the main logic that works with stop colors and creation of the resulted pdf color object.
    /// </remarks>
    public abstract class AbstractLinearGradientBuilder {
        /// <summary>The epsilon value used for data creation</summary>
        protected internal const double ZERO_EPSILON = 1E-10;

        private readonly IList<GradientColorStop> stops = new List<GradientColorStop>();

        private GradientSpreadMethod spreadMethod = GradientSpreadMethod.NONE;

        /// <summary>
        /// Adds the new color stop to the end (
        /// <see cref="AbstractLinearGradientBuilder">more info</see>
        /// ).
        /// </summary>
        /// <remarks>
        /// Adds the new color stop to the end (
        /// <see cref="AbstractLinearGradientBuilder">more info</see>
        /// ).
        /// Note: if the previously added color stop's offset would have grater offset than the added
        /// one, then the new offset would be normalized to be equal to the previous one. (Comparison
        /// made between relative on coordinates vector offsets. If any of them has
        /// the absolute offset, then the absolute value would converted to relative first.)
        /// </remarks>
        /// <param name="gradientColorStop">the gradient stop color to add</param>
        /// <returns>the current builder instance</returns>
        public virtual AbstractLinearGradientBuilder AddColorStop(GradientColorStop gradientColorStop) {
            if (gradientColorStop != null) {
                this.stops.Add(gradientColorStop);
            }
            return this;
        }

        /// <summary>Set the spread method to use for the gradient</summary>
        /// <param name="gradientSpreadMethod">the gradient spread method to set</param>
        /// <returns>the current builder instance</returns>
        public virtual AbstractLinearGradientBuilder SetSpreadMethod(GradientSpreadMethod gradientSpreadMethod) {
            if (spreadMethod != null) {
                this.spreadMethod = gradientSpreadMethod;
            }
            else {
                this.spreadMethod = GradientSpreadMethod.NONE;
            }
            return this;
        }

        /// <summary>Get the copy of current color stops list.</summary>
        /// <remarks>Get the copy of current color stops list. Note that the stop colors are not copied here</remarks>
        /// <returns>the copy of current stop colors list</returns>
        public virtual IList<GradientColorStop> GetColorStops() {
            return new List<GradientColorStop>(this.stops);
        }

        /// <summary>Get the current spread method</summary>
        /// <returns>the current spread method</returns>
        public virtual GradientSpreadMethod GetSpreadMethod() {
            return this.spreadMethod;
        }

        /// <summary>
        /// Builds the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// object representing the linear gradient with specified configuration
        /// that fills the target bounding box.
        /// </summary>
        /// <param name="targetBoundingBox">the bounding box to be filled in current space</param>
        /// <param name="contextTransform">
        /// the transformation from the base coordinates space into
        /// the current space. The
        /// <see langword="null"/>
        /// value is valid and can be used
        /// if there is no transformation from base coordinates to current space
        /// specified, or it is equal to identity transformation.
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which the linear gradient would be built.
        /// </param>
        /// <returns>
        /// the constructed
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// or
        /// <see langword="null"/>
        /// if no color to be applied
        /// or base gradient vector has been specified
        /// </returns>
        public virtual Color BuildColor(Rectangle targetBoundingBox, AffineTransform contextTransform, PdfDocument
             document) {
            // TODO: DEVSIX-4136 the document argument would be required for opaque gradients (as we would need to create a mask form xObject)
            Point[] baseCoordinatesVector = GetGradientVector(targetBoundingBox, contextTransform);
            if (baseCoordinatesVector == null || this.stops.IsEmpty()) {
                // Can not create gradient color with 0 stops or null coordinates vector
                return null;
            }
            // evaluate actual coordinates and transformation
            AffineTransform shadingTransform = new AffineTransform();
            if (contextTransform != null) {
                shadingTransform.Concatenate(contextTransform);
            }
            AffineTransform gradientTransformation = GetCurrentSpaceToGradientVectorSpaceTransformation(targetBoundingBox
                , contextTransform);
            if (gradientTransformation != null) {
                try {
                    if (targetBoundingBox != null) {
                        targetBoundingBox = Rectangle.CalculateBBox(JavaUtil.ArraysAsList(gradientTransformation.InverseTransform(
                            new Point(targetBoundingBox.GetLeft(), targetBoundingBox.GetBottom()), null), gradientTransformation.InverseTransform
                            (new Point(targetBoundingBox.GetLeft(), targetBoundingBox.GetTop()), null), gradientTransformation.InverseTransform
                            (new Point(targetBoundingBox.GetRight(), targetBoundingBox.GetBottom()), null), gradientTransformation
                            .InverseTransform(new Point(targetBoundingBox.GetRight(), targetBoundingBox.GetTop()), null)));
                    }
                    shadingTransform.Concatenate(gradientTransformation);
                }
                catch (NoninvertibleTransformException) {
                    ITextLogManager.GetLogger(GetType()).LogError(iText.IO.Logs.IoLogMessageConstant.UNABLE_TO_INVERT_GRADIENT_TRANSFORMATION
                        );
                }
            }
            PdfShading.Axial axial = CreateAxialShading(baseCoordinatesVector, this.stops, this.spreadMethod, targetBoundingBox
                );
            if (axial == null) {
                return null;
            }
            PdfPattern.Shading shading = new PdfPattern.Shading(axial);
            if (!shadingTransform.IsIdentity()) {
                double[] matrix = new double[6];
                shadingTransform.GetMatrix(matrix);
                shading.SetMatrix(new PdfArray(matrix));
            }
            return new PatternColor(shading);
        }

        /// <summary>Returns the base gradient vector in gradient vector space.</summary>
        /// <remarks>
        /// Returns the base gradient vector in gradient vector space. This vector would be set
        /// as shading coordinates vector and its length would be used to translate all color stops
        /// absolute offsets into the relatives.
        /// </remarks>
        /// <param name="targetBoundingBox">the rectangle to be covered by constructed color in current space</param>
        /// <param name="contextTransform">the current canvas transformation</param>
        /// <returns>the array of exactly two elements specifying the gradient coordinates vector</returns>
        protected internal abstract Point[] GetGradientVector(Rectangle targetBoundingBox, AffineTransform contextTransform
            );

        /// <summary>
        /// Returns the current space to gradient vector space transformations that should be applied
        /// to the shading color.
        /// </summary>
        /// <remarks>
        /// Returns the current space to gradient vector space transformations that should be applied
        /// to the shading color. The transformation should be invertible as the current target
        /// bounding box coordinates should be transformed into the resulted shading space coordinates.
        /// </remarks>
        /// <param name="targetBoundingBox">the rectangle to be covered by constructed color in current space</param>
        /// <param name="contextTransform">the current canvas transformation</param>
        /// <returns>
        /// the additional transformation to be concatenated to the current for resulted shading
        /// or
        /// <see langword="null"/>
        /// if no additional transformation is specified
        /// </returns>
        protected internal virtual AffineTransform GetCurrentSpaceToGradientVectorSpaceTransformation(Rectangle targetBoundingBox
            , AffineTransform contextTransform) {
            return null;
        }

        /// <summary>Evaluates the minimal domain that covers the box with vector normals.</summary>
        /// <remarks>
        /// Evaluates the minimal domain that covers the box with vector normals.
        /// The domain corresponding to the initial vector is [0, 1].
        /// </remarks>
        /// <param name="coords">
        /// the array of exactly two elements that describe
        /// the base vector (corresponding to [0,1] domain, that need to be adjusted
        /// to cover the box
        /// </param>
        /// <param name="toCover">the box that needs to be covered</param>
        /// <returns>
        /// the array of two elements in ascending order specifying the calculated covering
        /// domain
        /// </returns>
        protected internal static double[] EvaluateCoveringDomain(Point[] coords, Rectangle toCover) {
            if (toCover == null) {
                return new double[] { 0d, 1d };
            }
            AffineTransform transform = new AffineTransform();
            double scale = 1d / (coords[0].Distance(coords[1]));
            double sin = -(coords[1].GetY() - coords[0].GetY()) * scale;
            double cos = (coords[1].GetX() - coords[0].GetX()) * scale;
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
            transform.Concatenate(new AffineTransform(cos, sin, -sin, cos, 0, 0));
            transform.Scale(scale, scale);
            transform.Translate(-coords[0].GetX(), -coords[0].GetY());
            Point[] rectanglePoints = toCover.ToPointsArray();
            double minX = transform.Transform(rectanglePoints[0], null).GetX();
            double maxX = minX;
            for (int i = 1; i < rectanglePoints.Length; ++i) {
                double currentX = transform.Transform(rectanglePoints[i], null).GetX();
                minX = Math.Min(minX, currentX);
                maxX = Math.Max(maxX, currentX);
            }
            return new double[] { minX, maxX };
        }

        /// <summary>Expand the base vector to cover the new domain</summary>
        /// <param name="newDomain">
        /// the array of exactly two elements that specifies the domain
        /// that should be covered by the created vector
        /// </param>
        /// <param name="baseVector">
        /// the array of exactly two elements that specifies the base vector
        /// which corresponds to [0, 1] domain
        /// </param>
        /// <returns>the array of two</returns>
        protected internal static Point[] CreateCoordinatesForNewDomain(double[] newDomain, Point[] baseVector) {
            double xDiff = baseVector[1].GetX() - baseVector[0].GetX();
            double yDiff = baseVector[1].GetY() - baseVector[0].GetY();
            Point[] targetCoords = new Point[] { baseVector[0].GetLocation(), baseVector[1].GetLocation() };
            targetCoords[0].Translate(xDiff * newDomain[0], yDiff * newDomain[0]);
            targetCoords[1].Translate(xDiff * (newDomain[1] - 1), yDiff * (newDomain[1] - 1));
            return targetCoords;
        }

        private static PdfShading.Axial CreateAxialShading(Point[] baseCoordinatesVector, IList<GradientColorStop>
             stops, GradientSpreadMethod spreadMethod, Rectangle targetBoundingBox) {
            double baseVectorLength = baseCoordinatesVector[1].Distance(baseCoordinatesVector[0]);
            IList<GradientColorStop> stopsToConstruct = NormalizeStops(stops, baseVectorLength);
            double[] coordinatesDomain = new double[] { 0, 1 };
            Point[] actualCoordinates;
            if (baseVectorLength < ZERO_EPSILON || stopsToConstruct.Count == 1) {
                // single color case
                if (spreadMethod == GradientSpreadMethod.NONE) {
                    return null;
                }
                actualCoordinates = new Point[] { new Point(targetBoundingBox.GetLeft(), targetBoundingBox.GetBottom()), new 
                    Point(targetBoundingBox.GetRight(), targetBoundingBox.GetBottom()) };
                GradientColorStop lastColorStop = stopsToConstruct[stopsToConstruct.Count - 1];
                stopsToConstruct = JavaUtil.ArraysAsList(new GradientColorStop(lastColorStop, 0d, GradientColorStop.OffsetType
                    .RELATIVE), new GradientColorStop(lastColorStop, 1d, GradientColorStop.OffsetType.RELATIVE));
            }
            else {
                coordinatesDomain = EvaluateCoveringDomain(baseCoordinatesVector, targetBoundingBox);
                if (spreadMethod == GradientSpreadMethod.REPEAT || spreadMethod == GradientSpreadMethod.REFLECT) {
                    stopsToConstruct = AdjustNormalizedStopsToCoverDomain(stopsToConstruct, coordinatesDomain, spreadMethod);
                }
                else {
                    if (spreadMethod == GradientSpreadMethod.PAD) {
                        AdjustStopsForPadIfNeeded(stopsToConstruct, coordinatesDomain);
                    }
                    else {
                        // none case
                        double firstStopOffset = stopsToConstruct[0].GetOffset();
                        double lastStopOffset = stopsToConstruct[stopsToConstruct.Count - 1].GetOffset();
                        if ((lastStopOffset - firstStopOffset < ZERO_EPSILON) || coordinatesDomain[1] <= firstStopOffset || coordinatesDomain
                            [0] >= lastStopOffset) {
                            return null;
                        }
                        coordinatesDomain[0] = Math.Max(coordinatesDomain[0], firstStopOffset);
                        coordinatesDomain[1] = Math.Min(coordinatesDomain[1], lastStopOffset);
                    }
                }
                System.Diagnostics.Debug.Assert(coordinatesDomain[0] <= coordinatesDomain[1]);
                actualCoordinates = CreateCoordinatesForNewDomain(coordinatesDomain, baseCoordinatesVector);
            }
            return new PdfShading.Axial(new PdfDeviceCs.Rgb(), CreateCoordsPdfArray(actualCoordinates), new PdfArray(coordinatesDomain
                ), ConstructFunction(stopsToConstruct));
        }

        // the result list would have the same list of stop colors as the original one
        // with all offsets on coordinates domain dimension and adjusted for ascending values
        private static IList<GradientColorStop> NormalizeStops(IList<GradientColorStop> toNormalize, double baseVectorLength
            ) {
            if (baseVectorLength < ZERO_EPSILON) {
                return JavaUtil.ArraysAsList(new GradientColorStop(toNormalize[toNormalize.Count - 1], 0d, GradientColorStop.OffsetType
                    .RELATIVE));
            }
            // get rid of all absolute on vector offsets and hint offsets
            IList<GradientColorStop> result = CopyStopsAndNormalizeAbsoluteOffsets(toNormalize, baseVectorLength);
            // normalize 1st stop as it may be a special case
            NormalizeFirstStopOffset(result);
            // now we have 1st stop with relative offset, all other stops are either auto or relative
            NormalizeAutoStops(result);
            // normalize hints to left only none or relative to colors hint offset types
            NormalizeHintsOffsets(result);
            return result;
        }

        private static void NormalizeHintsOffsets(IList<GradientColorStop> result) {
            // normalize all except last
            for (int i = 0; i < result.Count - 1; ++i) {
                GradientColorStop stopColor = result[i];
                if (stopColor.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT) {
                    double currentStopOffset = stopColor.GetOffset();
                    double nextStopOffset = result[i + 1].GetOffset();
                    if (currentStopOffset != nextStopOffset) {
                        double hintOffset = (stopColor.GetHintOffset() - currentStopOffset) / (nextStopOffset - currentStopOffset);
                        stopColor.SetHint(hintOffset, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS);
                    }
                    else {
                        // if stops has the same offset, then no hint needed
                        stopColor.SetHint(0, GradientColorStop.HintOffsetType.NONE);
                    }
                }
            }
            // the last color hint is not needed as even with pad and reflect it won't be used
            result[result.Count - 1].SetHint(0, GradientColorStop.HintOffsetType.NONE);
        }

        private static void NormalizeAutoStops(IList<GradientColorStop> toNormalize) {
            System.Diagnostics.Debug.Assert(toNormalize[0].GetOffsetType() == GradientColorStop.OffsetType.RELATIVE);
            int firstAutoStopIndex = 1;
            GradientColorStop firstStopColor = toNormalize[0];
            double prevOffset = firstStopColor.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT
                 ? firstStopColor.GetHintOffset() : firstStopColor.GetOffset();
            for (int i = 1; i < toNormalize.Count; ++i) {
                GradientColorStop currentStop = toNormalize[i];
                if (currentStop.GetOffsetType() == GradientColorStop.OffsetType.AUTO) {
                    if (currentStop.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT) {
                        double hintOffset = currentStop.GetHintOffset();
                        NormalizeAutoStops(toNormalize, firstAutoStopIndex, i + 1, prevOffset, hintOffset);
                        prevOffset = hintOffset;
                        firstAutoStopIndex = i + 1;
                    }
                }
                else {
                    if (firstAutoStopIndex < i) {
                        // current stop offset is relative
                        double offset = currentStop.GetOffset();
                        NormalizeAutoStops(toNormalize, firstAutoStopIndex, i, prevOffset, offset);
                    }
                    firstAutoStopIndex = i + 1;
                    prevOffset = currentStop.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT ? currentStop
                        .GetHintOffset() : currentStop.GetOffset();
                }
            }
            // check whether the last interval has auto
            if (firstAutoStopIndex < toNormalize.Count) {
                double lastStopOffset = Math.Max(1, prevOffset);
                NormalizeAutoStops(toNormalize, firstAutoStopIndex, toNormalize.Count, prevOffset, lastStopOffset);
            }
        }

        private static void NormalizeAutoStops(IList<GradientColorStop> toNormalizeList, int fromIndex, int toIndex
            , double prevOffset, double nextOffset) {
            System.Diagnostics.Debug.Assert(toIndex >= fromIndex);
            int intervalsCount = Math.Min(toIndex, toNormalizeList.Count - 1) - fromIndex + 1;
            double offsetShift = (nextOffset - prevOffset) / intervalsCount;
            double currentOffset = prevOffset;
            for (int i = fromIndex; i < toIndex; ++i) {
                currentOffset += offsetShift;
                GradientColorStop currentAutoStop = toNormalizeList[i];
                System.Diagnostics.Debug.Assert(currentAutoStop.GetOffsetType() == GradientColorStop.OffsetType.AUTO);
                currentAutoStop.SetOffset(currentOffset, GradientColorStop.OffsetType.RELATIVE);
            }
        }

        private static void NormalizeFirstStopOffset(IList<GradientColorStop> result) {
            // assert that all stops has no absolute on vector offsets and hints
            GradientColorStop firstStop = result[0];
            if (firstStop.GetOffsetType() != GradientColorStop.OffsetType.AUTO) {
                return;
            }
            double firstStopOffset = 0;
            foreach (GradientColorStop stopColor in result) {
                if (stopColor.GetOffsetType() == GradientColorStop.OffsetType.RELATIVE) {
                    firstStopOffset = stopColor.GetOffset();
                    break;
                }
                else {
                    if (stopColor.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT) {
                        firstStopOffset = stopColor.GetHintOffset();
                        break;
                    }
                }
            }
            firstStopOffset = Math.Min(0, firstStopOffset);
            firstStop.SetOffset(firstStopOffset, GradientColorStop.OffsetType.RELATIVE);
        }

        private static IList<GradientColorStop> CopyStopsAndNormalizeAbsoluteOffsets(IList<GradientColorStop> toNormalize
            , double baseVectorLength) {
            double lastUsedOffset = double.NegativeInfinity;
            IList<GradientColorStop> copy = new List<GradientColorStop>(toNormalize.Count);
            foreach (GradientColorStop stop in toNormalize) {
                double offset = stop.GetOffset();
                GradientColorStop.OffsetType offsetType = stop.GetOffsetType();
                if (offsetType == GradientColorStop.OffsetType.ABSOLUTE) {
                    offsetType = GradientColorStop.OffsetType.RELATIVE;
                    offset /= baseVectorLength;
                }
                if (offsetType == GradientColorStop.OffsetType.RELATIVE) {
                    if (offset < lastUsedOffset) {
                        offset = lastUsedOffset;
                    }
                    lastUsedOffset = offset;
                }
                GradientColorStop result = new GradientColorStop(stop, offset, offsetType);
                double hintOffset = stop.GetHintOffset();
                GradientColorStop.HintOffsetType hintOffsetType = stop.GetHintOffsetType();
                if (hintOffsetType == GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT) {
                    hintOffsetType = GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT;
                    hintOffset /= baseVectorLength;
                }
                if (hintOffsetType == GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT) {
                    if (hintOffset < lastUsedOffset) {
                        hintOffset = lastUsedOffset;
                    }
                    lastUsedOffset = hintOffset;
                }
                result.SetHint(hintOffset, hintOffsetType);
                copy.Add(result);
            }
            return copy;
        }

        private static void AdjustStopsForPadIfNeeded(IList<GradientColorStop> stopsToConstruct, double[] coordinatesDomain
            ) {
            GradientColorStop firstStop = stopsToConstruct[0];
            if (coordinatesDomain[0] < firstStop.GetOffset()) {
                stopsToConstruct.Add(0, new GradientColorStop(firstStop, coordinatesDomain[0], GradientColorStop.OffsetType
                    .RELATIVE));
            }
            GradientColorStop lastStop = stopsToConstruct[stopsToConstruct.Count - 1];
            if (coordinatesDomain[1] > lastStop.GetOffset()) {
                stopsToConstruct.Add(new GradientColorStop(lastStop, coordinatesDomain[1], GradientColorStop.OffsetType.RELATIVE
                    ));
            }
        }

        private static IList<GradientColorStop> AdjustNormalizedStopsToCoverDomain(IList<GradientColorStop> normalizedStops
            , double[] targetDomain, GradientSpreadMethod spreadMethod) {
            IList<GradientColorStop> adjustedStops = new List<GradientColorStop>();
            GradientColorStop lastColorStop = normalizedStops[normalizedStops.Count - 1];
            double originalIntervalEnd = lastColorStop.GetOffset();
            double originalIntervalStart = normalizedStops[0].GetOffset();
            double originalIntervalLength = originalIntervalEnd - originalIntervalStart;
            if (originalIntervalLength <= ZERO_EPSILON) {
                return JavaUtil.ArraysAsList(new GradientColorStop(lastColorStop, targetDomain[0], GradientColorStop.OffsetType
                    .RELATIVE), new GradientColorStop(lastColorStop, targetDomain[1], GradientColorStop.OffsetType.RELATIVE
                    ));
            }
            double startIntervalsShift = Math.Floor((targetDomain[0] - originalIntervalStart) / originalIntervalLength
                );
            double iterationOffset = originalIntervalStart + (originalIntervalLength * startIntervalsShift);
            bool isIterationInverse = spreadMethod == GradientSpreadMethod.REFLECT && Math.Abs(startIntervalsShift) % 
                2 != 0;
            int currentIterationIndex = isIterationInverse ? normalizedStops.Count - 1 : 0;
            double lastComputedOffset = iterationOffset;
            while (lastComputedOffset <= targetDomain[1]) {
                GradientColorStop currentStop = normalizedStops[currentIterationIndex];
                lastComputedOffset = isIterationInverse ? iterationOffset + originalIntervalEnd - currentStop.GetOffset() : 
                    iterationOffset + currentStop.GetOffset() - originalIntervalStart;
                GradientColorStop computedStop = new GradientColorStop(currentStop, lastComputedOffset, GradientColorStop.OffsetType
                    .RELATIVE);
                if (lastComputedOffset < targetDomain[0] && !adjustedStops.IsEmpty()) {
                    adjustedStops[0] = computedStop;
                }
                else {
                    adjustedStops.Add(computedStop);
                }
                if (isIterationInverse) {
                    --currentIterationIndex;
                    if (currentIterationIndex < 0) {
                        iterationOffset += originalIntervalLength;
                        isIterationInverse = false;
                        currentIterationIndex = 1;
                    }
                }
                else {
                    ++currentIterationIndex;
                    if (currentIterationIndex == normalizedStops.Count) {
                        iterationOffset += originalIntervalLength;
                        isIterationInverse = spreadMethod == GradientSpreadMethod.REFLECT;
                        currentIterationIndex = isIterationInverse ? normalizedStops.Count - 2 : 0;
                    }
                }
                // check the next iteration type to set the correct stop color hint for just added stop
                if (isIterationInverse) {
                    GradientColorStop nextColor = normalizedStops[currentIterationIndex];
                    // this method should be invoked only after the normalization. it means that
                    // the hint offset type for each stop is either relative to colors interval
                    // (i.e. for inverse iteration we need to inverse the hint offset), or is none
                    // (i.e. the hint offset value should be ignored)
                    computedStop.SetHint(1 - nextColor.GetHintOffset(), nextColor.GetHintOffsetType());
                }
                else {
                    computedStop.SetHint(currentStop.GetHintOffset(), currentStop.GetHintOffsetType());
                }
            }
            return adjustedStops;
        }

        private static IPdfFunction ConstructFunction(IList<GradientColorStop> toConstruct) {
            int functionsAmount = toConstruct.Count - 1;
            double[] bounds = new double[functionsAmount - 1];
            IList<AbstractPdfFunction<PdfDictionary>> type2Functions = new List<AbstractPdfFunction<PdfDictionary>>(functionsAmount
                );
            GradientColorStop currentStop;
            GradientColorStop nextStop = toConstruct[0];
            double domainStart = nextStop.GetOffset();
            for (int i = 1; i < functionsAmount; ++i) {
                currentStop = nextStop;
                nextStop = toConstruct[i];
                bounds[i - 1] = nextStop.GetOffset();
                type2Functions.Add(ConstructSingleGradientSegmentFunction(currentStop, nextStop));
            }
            currentStop = nextStop;
            nextStop = toConstruct[toConstruct.Count - 1];
            type2Functions.Add(ConstructSingleGradientSegmentFunction(currentStop, nextStop));
            double domainEnd = nextStop.GetOffset();
            double[] encode = new double[functionsAmount * 2];
            for (int i = 0; i < encode.Length; i += 2) {
                encode[i] = 0d;
                encode[i + 1] = 1d;
            }
            return new PdfType3Function(new double[] { domainStart, domainEnd }, null, type2Functions, bounds, encode);
        }

        private static AbstractPdfFunction<PdfDictionary> ConstructSingleGradientSegmentFunction(GradientColorStop
             from, GradientColorStop to) {
            double exponent = 1d;
            float[] fromColor = from.GetRgbArray();
            float[] toColor = to.GetRgbArray();
            if (from.GetHintOffsetType() == GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS) {
                double hintOffset = from.GetHintOffset();
                if (hintOffset <= 0d + ZERO_EPSILON) {
                    fromColor = toColor;
                }
                else {
                    if (hintOffset >= 1d - ZERO_EPSILON) {
                        toColor = fromColor;
                    }
                    else {
                        // similar to css color hint logic
                        exponent = Math.Log(0.5) / Math.Log(hintOffset);
                    }
                }
            }
            return new PdfType2Function(new float[] { 0f, 1f }, null, fromColor, toColor, exponent);
        }

        private static PdfArray CreateCoordsPdfArray(Point[] coordsPoints) {
            System.Diagnostics.Debug.Assert(coordsPoints != null && coordsPoints.Length == 2);
            return new PdfArray(new double[] { coordsPoints[0].GetX(), coordsPoints[0].GetY(), coordsPoints[1].GetX(), 
                coordsPoints[1].GetY() });
        }
    }
}
