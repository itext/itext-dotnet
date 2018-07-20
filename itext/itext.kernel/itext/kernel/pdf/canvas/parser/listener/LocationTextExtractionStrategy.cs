/*

This file is part of the iText (R) project.
    Copyright (c) 1998-2018 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    public class LocationTextExtractionStrategy : ITextExtractionStrategy {
        /// <summary>set to true for debugging</summary>
        private static bool DUMP_STATE = false;

        private const float DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION = 2;

        /// <summary>a summary of all found text</summary>
        private readonly IList<LocationTextExtractionStrategy.TextChunk> locationalResult = new List<LocationTextExtractionStrategy.TextChunk
            >();

        private readonly LocationTextExtractionStrategy.ITextChunkLocationStrategy tclStrat;

        private bool useActualText = false;

        private bool rightToLeftRunDirection = false;

        private TextRenderInfo lastTextRenderInfo;

        /// <summary>Creates a new text extraction renderer.</summary>
        public LocationTextExtractionStrategy()
            : this(new _ITextChunkLocationStrategy_88()) {
        }

        private sealed class _ITextChunkLocationStrategy_88 : LocationTextExtractionStrategy.ITextChunkLocationStrategy {
            public _ITextChunkLocationStrategy_88() {
            }

            public LocationTextExtractionStrategy.ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment
                 baseline) {
                return new LocationTextExtractionStrategy.TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint
                    (), renderInfo.GetSingleSpaceWidth());
            }
        }

        /// <summary>
        /// Creates a new text extraction renderer, with a custom strategy for
        /// creating new TextChunkLocation objects based on the input of the
        /// TextRenderInfo.
        /// </summary>
        /// <param name="strat">the custom strategy</param>
        public LocationTextExtractionStrategy(LocationTextExtractionStrategy.ITextChunkLocationStrategy strat) {
            tclStrat = strat;
        }

        /// <summary>
        /// Changes the behavior of text extraction so that if the parameter is set to
        /// <see langword="true"/>
        /// ,
        /// /ActualText marked content property will be used instead of raw decoded bytes.
        /// Beware: the logic is not stable yet.
        /// </summary>
        /// <param name="useActualText">true to use /ActualText, false otherwise</param>
        /// <returns>this object</returns>
        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy SetUseActualText(bool
             useActualText) {
            this.useActualText = useActualText;
            return this;
        }

        /// <summary>Sets if text flows from left to right or from right to left.</summary>
        /// <remarks>
        /// Sets if text flows from left to right or from right to left.
        /// Call this method with <code>true</code> argument for extracting Arabic, Hebrew or other
        /// text with right-to-left writing direction.
        /// </remarks>
        /// <param name="rightToLeftRunDirection">value specifying whether the direction should be right to left</param>
        /// <returns>this object</returns>
        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy SetRightToLeftRunDirection
            (bool rightToLeftRunDirection) {
            this.rightToLeftRunDirection = rightToLeftRunDirection;
            return this;
        }

        /// <summary>
        /// Gets the value of the property which determines if /ActualText will be used when extracting
        /// the text
        /// </summary>
        /// <returns>true if /ActualText value is used, false otherwise</returns>
        public virtual bool IsUseActualText() {
            return useActualText;
        }

        public virtual void EventOccurred(IEventData data, EventType type) {
            if (type.Equals(EventType.RENDER_TEXT)) {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                LineSegment segment = renderInfo.GetBaseline();
                if (renderInfo.GetRise() != 0) {
                    // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to
                    Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                    segment = segment.TransformBy(riseOffsetTransform);
                }
                if (useActualText) {
                    CanvasTag lastTagWithActualText = lastTextRenderInfo != null ? FindLastTagWithActualText(lastTextRenderInfo
                        .GetCanvasTagHierarchy()) : null;
                    if (lastTagWithActualText != null && lastTagWithActualText == FindLastTagWithActualText(renderInfo.GetCanvasTagHierarchy
                        ())) {
                        // Merge two text pieces, assume they will be in the same line
                        LocationTextExtractionStrategy.TextChunk lastTextChunk = locationalResult[locationalResult.Count - 1];
                        Vector mergedStart = new Vector(Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(0), segment.GetStartPoint
                            ().Get(0)), Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(1), segment.GetStartPoint().Get
                            (1)), Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(2), segment.GetStartPoint().Get(2)));
                        Vector mergedEnd = new Vector(Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(0), segment.GetEndPoint
                            ().Get(0)), Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(1), segment.GetEndPoint().Get(1)
                            ), Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(2), segment.GetEndPoint().Get(2)));
                        LocationTextExtractionStrategy.TextChunk merged = new LocationTextExtractionStrategy.TextChunk(lastTextChunk
                            .GetText(), tclStrat.CreateLocation(renderInfo, new LineSegment(mergedStart, mergedEnd)));
                        locationalResult[locationalResult.Count - 1] = merged;
                    }
                    else {
                        String actualText = renderInfo.GetActualText();
                        LocationTextExtractionStrategy.TextChunk tc = new LocationTextExtractionStrategy.TextChunk(actualText != null
                             ? actualText : renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
                        locationalResult.Add(tc);
                    }
                }
                else {
                    LocationTextExtractionStrategy.TextChunk tc = new LocationTextExtractionStrategy.TextChunk(renderInfo.GetText
                        (), tclStrat.CreateLocation(renderInfo, segment));
                    locationalResult.Add(tc);
                }
                lastTextRenderInfo = renderInfo;
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return null;
        }

        public virtual String GetResultantText() {
            if (DUMP_STATE) {
                DumpState();
            }
            IList<LocationTextExtractionStrategy.TextChunk> textChunks = new List<LocationTextExtractionStrategy.TextChunk
                >(locationalResult);
            SortWithMarks(textChunks);
            StringBuilder sb = new StringBuilder();
            LocationTextExtractionStrategy.TextChunk lastChunk = null;
            foreach (LocationTextExtractionStrategy.TextChunk chunk in textChunks) {
                if (lastChunk == null) {
                    sb.Append(chunk.text);
                }
                else {
                    if (chunk.SameLine(lastChunk)) {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.text) && !EndsWithSpace(lastChunk.text
                            )) {
                            sb.Append(' ');
                        }
                        sb.Append(chunk.text);
                    }
                    else {
                        sb.Append('\n');
                        sb.Append(chunk.text);
                    }
                }
                lastChunk = chunk;
            }
            return sb.ToString();
        }

        /// <summary>Determines if a space character should be inserted between a previous chunk and the current chunk.
        ///     </summary>
        /// <remarks>
        /// Determines if a space character should be inserted between a previous chunk and the current chunk.
        /// This method is exposed as a callback so subclasses can fine time the algorithm for determining whether a space should be inserted or not.
        /// By default, this method will insert a space if the there is a gap of more than half the font space character width between the end of the
        /// previous chunk and the beginning of the current chunk.  It will also indicate that a space is needed if the starting point of the new chunk
        /// appears *before* the end of the previous chunk (i.e. overlapping text).
        /// </remarks>
        /// <param name="chunk">the new chunk being evaluated</param>
        /// <param name="previousChunk">the chunk that appeared immediately before the current chunk</param>
        /// <returns>true if the two chunks represent different words (i.e. should have a space between them).  False otherwise.
        ///     </returns>
        protected internal virtual bool IsChunkAtWordBoundary(LocationTextExtractionStrategy.TextChunk chunk, LocationTextExtractionStrategy.TextChunk
             previousChunk) {
            return chunk.GetLocation().IsAtWordBoundary(previousChunk.GetLocation());
        }

        /// <summary>Checks if the string starts with a space character, false if the string is empty or starts with a non-space character.
        ///     </summary>
        /// <param name="str">the string to be checked</param>
        /// <returns>true if the string starts with a space character, false if the string is empty or starts with a non-space character
        ///     </returns>
        private bool StartsWithSpace(String str) {
            return str.Length != 0 && str[0] == ' ';
        }

        /// <summary>Checks if the string ends with a space character, false if the string is empty or ends with a non-space character
        ///     </summary>
        /// <param name="str">the string to be checked</param>
        /// <returns>true if the string ends with a space character, false if the string is empty or ends with a non-space character
        ///     </returns>
        private bool EndsWithSpace(String str) {
            return str.Length != 0 && str[str.Length - 1] == ' ';
        }

        /// <summary>Used for debugging only</summary>
        private void DumpState() {
            foreach (LocationTextExtractionStrategy.TextChunk location in locationalResult) {
                location.PrintDiagnostics();
                System.Console.Out.WriteLine();
            }
        }

        private CanvasTag FindLastTagWithActualText(IList<CanvasTag> canvasTagHierarchy) {
            CanvasTag lastActualText = null;
            foreach (CanvasTag tag in canvasTagHierarchy) {
                if (tag.GetActualText() != null) {
                    lastActualText = tag;
                    break;
                }
            }
            return lastActualText;
        }

        private void SortWithMarks(IList<LocationTextExtractionStrategy.TextChunk> textChunks) {
            IDictionary<LocationTextExtractionStrategy.TextChunk, LocationTextExtractionStrategy.TextChunkMarks> marks
                 = new Dictionary<LocationTextExtractionStrategy.TextChunk, LocationTextExtractionStrategy.TextChunkMarks
                >();
            IList<LocationTextExtractionStrategy.TextChunk> toSort = new List<LocationTextExtractionStrategy.TextChunk
                >();
            for (int markInd = 0; markInd < textChunks.Count; markInd++) {
                LocationTextExtractionStrategy.ITextChunkLocation location = textChunks[markInd].GetLocation();
                if (location.GetStartLocation().Equals(location.GetEndLocation())) {
                    bool foundBaseToAttachTo = false;
                    for (int baseInd = 0; baseInd < textChunks.Count; baseInd++) {
                        if (markInd != baseInd) {
                            LocationTextExtractionStrategy.ITextChunkLocation baseLocation = textChunks[baseInd].GetLocation();
                            if (!baseLocation.GetStartLocation().Equals(baseLocation.GetEndLocation()) && ContainsMark(baseLocation, location
                                )) {
                                LocationTextExtractionStrategy.TextChunkMarks currentMarks = marks.Get(textChunks[baseInd]);
                                if (currentMarks == null) {
                                    currentMarks = new LocationTextExtractionStrategy.TextChunkMarks();
                                    marks.Put(textChunks[baseInd], currentMarks);
                                }
                                if (markInd < baseInd) {
                                    currentMarks.preceding.Add(textChunks[markInd]);
                                }
                                else {
                                    currentMarks.succeeding.Add(textChunks[markInd]);
                                }
                                foundBaseToAttachTo = true;
                                break;
                            }
                        }
                    }
                    if (!foundBaseToAttachTo) {
                        toSort.Add(textChunks[markInd]);
                    }
                }
                else {
                    toSort.Add(textChunks[markInd]);
                }
            }
            if (rightToLeftRunDirection) {
                JavaCollectionsUtil.Sort(toSort, new LocationTextExtractionStrategy.TextChunkComparator(new LocationTextExtractionStrategy.TextChunkLocationComparator
                    (false)));
            }
            else {
                JavaCollectionsUtil.Sort(toSort);
            }
            textChunks.Clear();
            foreach (LocationTextExtractionStrategy.TextChunk current in toSort) {
                LocationTextExtractionStrategy.TextChunkMarks currentMarks = marks.Get(current);
                if (currentMarks != null) {
                    if (!rightToLeftRunDirection) {
                        for (int j = 0; j < currentMarks.preceding.Count; j++) {
                            textChunks.Add(currentMarks.preceding[j]);
                        }
                    }
                    else {
                        for (int j = currentMarks.succeeding.Count - 1; j >= 0; j--) {
                            textChunks.Add(currentMarks.succeeding[j]);
                        }
                    }
                }
                textChunks.Add(current);
                if (currentMarks != null) {
                    if (!rightToLeftRunDirection) {
                        for (int j = 0; j < currentMarks.succeeding.Count; j++) {
                            textChunks.Add(currentMarks.succeeding[j]);
                        }
                    }
                    else {
                        for (int j = currentMarks.preceding.Count - 1; j >= 0; j--) {
                            textChunks.Add(currentMarks.preceding[j]);
                        }
                    }
                }
            }
        }

        private bool ContainsMark(LocationTextExtractionStrategy.ITextChunkLocation baseLocation, LocationTextExtractionStrategy.ITextChunkLocation
             markLocation) {
            return baseLocation.GetStartLocation().Get(Vector.I1) <= markLocation.GetStartLocation().Get(Vector.I1) &&
                 baseLocation.GetEndLocation().Get(Vector.I1) >= markLocation.GetEndLocation().Get(Vector.I1) && Math.
                Abs(baseLocation.DistPerpendicular() - markLocation.DistPerpendicular()) <= DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION;
        }

        public interface ITextChunkLocationStrategy {
            LocationTextExtractionStrategy.ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline
                );
        }

        public interface ITextChunkLocation : IComparable<LocationTextExtractionStrategy.ITextChunkLocation> {
            float DistParallelEnd();

            float DistParallelStart();

            int DistPerpendicular();

            float GetCharSpaceWidth();

            Vector GetEndLocation();

            Vector GetStartLocation();

            int OrientationMagnitude();

            bool SameLine(LocationTextExtractionStrategy.ITextChunkLocation @as);

            float DistanceFromEndOf(LocationTextExtractionStrategy.ITextChunkLocation other);

            bool IsAtWordBoundary(LocationTextExtractionStrategy.ITextChunkLocation previous);
        }

        /// <summary>Represents a chunk of text, it's orientation, and location relative to the orientation vector</summary>
        public class TextChunk : IComparable<LocationTextExtractionStrategy.TextChunk> {
            /// <summary>the text of the chunk</summary>
            protected internal readonly String text;

            protected internal readonly LocationTextExtractionStrategy.ITextChunkLocation location;

            public TextChunk(String @string, LocationTextExtractionStrategy.ITextChunkLocation loc) {
                this.text = @string;
                this.location = loc;
            }

            /// <returns>the text captured by this chunk</returns>
            public virtual String GetText() {
                return text;
            }

            public virtual LocationTextExtractionStrategy.ITextChunkLocation GetLocation() {
                return location;
            }

            /// <summary>Compares based on orientation, perpendicular distance, then parallel distance</summary>
            /// <seealso cref="System.IComparable{T}.CompareTo(System.Object)"/>
            public virtual int CompareTo(LocationTextExtractionStrategy.TextChunk rhs) {
                return location.CompareTo(rhs.location);
            }

            internal virtual void PrintDiagnostics() {
                System.Console.Out.WriteLine("Text (@" + location.GetStartLocation() + " -> " + location.GetEndLocation() 
                    + "): " + text);
                System.Console.Out.WriteLine("orientationMagnitude: " + location.OrientationMagnitude());
                System.Console.Out.WriteLine("distPerpendicular: " + location.DistPerpendicular());
                System.Console.Out.WriteLine("distParallel: " + location.DistParallelStart());
            }

            internal virtual bool SameLine(LocationTextExtractionStrategy.TextChunk lastChunk) {
                return GetLocation().SameLine(lastChunk.GetLocation());
            }
        }

        public class TextChunkLocationDefaultImp : LocationTextExtractionStrategy.ITextChunkLocation {
            private static readonly LocationTextExtractionStrategy.TextChunkLocationComparator defaultComparator = new 
                LocationTextExtractionStrategy.TextChunkLocationComparator();

            /// <summary>the starting location of the chunk</summary>
            private readonly Vector startLocation;

            /// <summary>the ending location of the chunk</summary>
            private readonly Vector endLocation;

            /// <summary>unit vector in the orientation of the chunk</summary>
            private readonly Vector orientationVector;

            /// <summary>the orientation as a scalar for quick sorting</summary>
            private readonly int orientationMagnitude;

            /// <summary>perpendicular distance to the orientation unit vector (i.e.</summary>
            /// <remarks>
            /// perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system)
            /// we round to the nearest integer to handle the fuzziness of comparing floats
            /// </remarks>
            private readonly int distPerpendicular;

            /// <summary>distance of the start of the chunk parallel to the orientation unit vector (i.e.</summary>
            /// <remarks>distance of the start of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system)
            ///     </remarks>
            private readonly float distParallelStart;

            /// <summary>distance of the end of the chunk parallel to the orientation unit vector (i.e.</summary>
            /// <remarks>distance of the end of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system)
            ///     </remarks>
            private readonly float distParallelEnd;

            /// <summary>the width of a single space character in the font of the chunk</summary>
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
                orientationMagnitude = (int)(Math.Atan2(orientationVector.Get(Vector.I2), orientationVector.Get(Vector.I1)
                    ) * 1000);
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
            public virtual bool SameLine(LocationTextExtractionStrategy.ITextChunkLocation @as) {
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
            public virtual float DistanceFromEndOf(LocationTextExtractionStrategy.ITextChunkLocation other) {
                return DistParallelStart() - other.DistParallelEnd();
            }

            public virtual bool IsAtWordBoundary(LocationTextExtractionStrategy.ITextChunkLocation previous) {
                /*
                * Here we handle a very specific case which in PDF may look like:
                * -.232 Tc [( P)-226.2(r)-231.8(e)-230.8(f)-238(a)-238.9(c)-228.9(e)]TJ
                * The font's charSpace width is 0.232 and it's compensated with charSpacing of 0.232.
                * And a resultant TextChunk.charSpaceWidth comes to TextChunk constructor as 0.
                * In this case every chunk is considered as a word boundary and space is added.
                * We should consider charSpaceWidth equal (or close) to zero as a no-space.
                */
                if (GetCharSpaceWidth() < 0.1f) {
                    return false;
                }
                // In case a text chunk is of zero length, this probably means this is a mark character,
                // and we do not actually want to insert a space in such case
                if (startLocation.Equals(endLocation) || previous.GetEndLocation().Equals(previous.GetStartLocation())) {
                    return false;
                }
                float dist = DistanceFromEndOf(previous);
                return dist < -GetCharSpaceWidth() || dist > GetCharSpaceWidth() / 2.0f;
            }

            public virtual int CompareTo(LocationTextExtractionStrategy.ITextChunkLocation other) {
                return defaultComparator.Compare(this, other);
            }
        }

        private class TextChunkComparator : IComparer<LocationTextExtractionStrategy.TextChunk> {
            private IComparer<LocationTextExtractionStrategy.ITextChunkLocation> locationComparator;

            public TextChunkComparator(IComparer<LocationTextExtractionStrategy.ITextChunkLocation> locationComparator
                ) {
                this.locationComparator = locationComparator;
            }

            public virtual int Compare(LocationTextExtractionStrategy.TextChunk o1, LocationTextExtractionStrategy.TextChunk
                 o2) {
                return locationComparator.Compare(o1.location, o2.location);
            }
        }

        private class TextChunkLocationComparator : IComparer<LocationTextExtractionStrategy.ITextChunkLocation> {
            private bool leftToRight = true;

            public TextChunkLocationComparator() {
            }

            public TextChunkLocationComparator(bool leftToRight) {
                this.leftToRight = leftToRight;
            }

            public virtual int Compare(LocationTextExtractionStrategy.ITextChunkLocation first, LocationTextExtractionStrategy.ITextChunkLocation
                 second) {
                if (first == second) {
                    return 0;
                }
                // not really needed, but just in case
                int result;
                result = JavaUtil.IntegerCompare(first.OrientationMagnitude(), second.OrientationMagnitude());
                if (result != 0) {
                    return result;
                }
                int distPerpendicularDiff = first.DistPerpendicular() - second.DistPerpendicular();
                if (distPerpendicularDiff != 0) {
                    return distPerpendicularDiff;
                }
                return leftToRight ? JavaUtil.FloatCompare(first.DistParallelStart(), second.DistParallelStart()) : -JavaUtil.FloatCompare
                    (first.DistParallelEnd(), second.DistParallelEnd());
            }
        }

        private class TextChunkMarks {
            internal IList<LocationTextExtractionStrategy.TextChunk> preceding = new List<LocationTextExtractionStrategy.TextChunk
                >();

            internal IList<LocationTextExtractionStrategy.TextChunk> succeeding = new List<LocationTextExtractionStrategy.TextChunk
                >();
        }
    }
}
