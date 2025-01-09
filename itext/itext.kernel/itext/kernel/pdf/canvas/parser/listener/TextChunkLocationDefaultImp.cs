/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font;
using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
//\cond DO_NOT_DOCUMENT
    internal class TextChunkLocationDefaultImp : ITextChunkLocation {
        private const float DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION = 2;

        /// <summary>The starting location of the chunk.</summary>
        private readonly Vector startLocation;

        /// <summary>The ending location of the chunk.</summary>
        private readonly Vector endLocation;

        /// <summary>Unit vector in the orientation of the chunk.</summary>
        private readonly Vector orientationVector;

        /// <summary>The orientation as a scalar for quick sorting.</summary>
        private readonly int orientationMagnitude;

        /// <summary>Perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system).
        ///     </summary>
        /// <remarks>
        /// Perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system).
        /// We round to the nearest integer to handle the fuzziness of comparing floats.
        /// </remarks>
        private readonly int distPerpendicular;

        /// <summary>Distance of the start of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system).
        ///     </summary>
        private readonly float distParallelStart;

        /// <summary>Distance of the end of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system).
        ///     </summary>
        private readonly float distParallelEnd;

        /// <summary>The width of a single space character in the font of the chunk.</summary>
        private readonly float charSpaceWidth;

        public TextChunkLocationDefaultImp(Vector startLocation, Vector endLocation, float charSpaceWidth) {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.charSpaceWidth = charSpaceWidth;
            Vector oVector = endLocation.Subtract(startLocation);
            if (oVector.Length() == 0) {
                oVector = new Vector(1, 0, 0);
            }
            orientationVector = oVector.Normalize();
            orientationMagnitude = (int)FontProgram.ConvertGlyphSpaceToTextSpace(Math.Atan2(orientationVector.Get(Vector
                .I2), orientationVector.Get(Vector.I1)));
            // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // the two vectors we are crossing are in the same plane, so the result will be purely
            // in the z-axis (out of plane) direction, so we just take the I3 component of the result
            Vector origin = new Vector(0, 0, 1);
            distPerpendicular = (int)(startLocation.Subtract(origin)).Cross(orientationVector).Get(Vector.I3);
            distParallelStart = orientationVector.Dot(startLocation);
            distParallelEnd = orientationVector.Dot(endLocation);
        }

        public virtual int OrientationMagnitude() {
            return orientationMagnitude;
        }

        public virtual int DistPerpendicular() {
            return distPerpendicular;
        }

        public virtual float DistParallelStart() {
            return distParallelStart;
        }

        public virtual float DistParallelEnd() {
            return distParallelEnd;
        }

        /// <returns>the start location of the text</returns>
        public virtual Vector GetStartLocation() {
            return startLocation;
        }

        /// <returns>the end location of the text</returns>
        public virtual Vector GetEndLocation() {
            return endLocation;
        }

        /// <returns>the width of a single space character as rendered by this chunk</returns>
        public virtual float GetCharSpaceWidth() {
            return charSpaceWidth;
        }

        /// <param name="as">the location to compare to</param>
        /// <returns>true is this location is on the the same line as the other</returns>
        public virtual bool SameLine(ITextChunkLocation @as) {
            if (OrientationMagnitude() != @as.OrientationMagnitude()) {
                return false;
            }
            float distPerpendicularDiff = DistPerpendicular() - @as.DistPerpendicular();
            if (distPerpendicularDiff == 0) {
                return true;
            }
            LineSegment mySegment = new LineSegment(startLocation, endLocation);
            LineSegment otherSegment = new LineSegment(@as.GetStartLocation(), @as.GetEndLocation());
            return Math.Abs(distPerpendicularDiff) <= DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION && (mySegment.GetLength
                () == 0 || otherSegment.GetLength() == 0);
        }

        /// <summary>
        /// Computes the distance between the end of 'other' and the beginning of this chunk
        /// in the direction of this chunk's orientation vector.
        /// </summary>
        /// <remarks>
        /// Computes the distance between the end of 'other' and the beginning of this chunk
        /// in the direction of this chunk's orientation vector.  Note that it's a bad idea
        /// to call this for chunks that aren't on the same line and orientation, but we don't
        /// explicitly check for that condition for performance reasons.
        /// </remarks>
        /// <param name="other"/>
        /// <returns>the number of spaces between the end of 'other' and the beginning of this chunk</returns>
        public virtual float DistanceFromEndOf(ITextChunkLocation other) {
            return DistParallelStart() - other.DistParallelEnd();
        }

        public virtual bool IsAtWordBoundary(ITextChunkLocation previous) {
            // In case a text chunk is of zero length, this probably means this is a mark character,
            // and we do not actually want to insert a space in such case
            if (startLocation.Equals(endLocation) || previous.GetEndLocation().Equals(previous.GetStartLocation())) {
                return false;
            }
            float dist = DistanceFromEndOf(previous);
            if (dist < 0) {
                dist = previous.DistanceFromEndOf(this);
                //The situation when the chunks intersect. We don't need to add space in this case
                if (dist < 0) {
                    return false;
                }
            }
            return dist > GetCharSpaceWidth() / 2.0f;
        }

//\cond DO_NOT_DOCUMENT
        internal static bool ContainsMark(ITextChunkLocation baseLocation, ITextChunkLocation markLocation) {
            return baseLocation.GetStartLocation().Get(Vector.I1) <= markLocation.GetStartLocation().Get(Vector.I1) &&
                 baseLocation.GetEndLocation().Get(Vector.I1) >= markLocation.GetEndLocation().Get(Vector.I1) && Math.
                Abs(baseLocation.DistPerpendicular() - markLocation.DistPerpendicular()) <= DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION;
        }
//\endcond
    }
//\endcond
}
