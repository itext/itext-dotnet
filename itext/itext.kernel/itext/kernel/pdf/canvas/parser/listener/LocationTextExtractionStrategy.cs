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
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    public class LocationTextExtractionStrategy : ITextExtractionStrategy {
        /// <summary>set to true for debugging</summary>
        private static bool DUMP_STATE = false;

        /// <summary>a summary of all found text</summary>
        private readonly IList<TextChunk> locationalResult = new List<TextChunk>();

        private readonly LocationTextExtractionStrategy.ITextChunkLocationStrategy tclStrat;

        private bool useActualText = false;

        private bool rightToLeftRunDirection = false;

        private TextRenderInfo lastTextRenderInfo;

        /// <summary>Creates a new text extraction renderer.</summary>
        public LocationTextExtractionStrategy()
            : this(new LocationTextExtractionStrategy.ITextChunkLocationStrategyImpl()) {
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
        /// </summary>
        /// <remarks>
        /// Changes the behavior of text extraction so that if the parameter is set to
        /// <see langword="true"/>
        /// ,
        /// /ActualText marked content property will be used instead of raw decoded bytes.
        /// Beware: the logic is not stable yet.
        /// </remarks>
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
        /// Call this method with <c>true</c> argument for extracting Arabic, Hebrew or other
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
                        TextChunk lastTextChunk = locationalResult[locationalResult.Count - 1];
                        Vector mergedStart = new Vector(Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(0), segment.GetStartPoint
                            ().Get(0)), Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(1), segment.GetStartPoint().Get
                            (1)), Math.Min(lastTextChunk.GetLocation().GetStartLocation().Get(2), segment.GetStartPoint().Get(2)));
                        Vector mergedEnd = new Vector(Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(0), segment.GetEndPoint
                            ().Get(0)), Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(1), segment.GetEndPoint().Get(1)
                            ), Math.Max(lastTextChunk.GetLocation().GetEndLocation().Get(2), segment.GetEndPoint().Get(2)));
                        TextChunk merged = new TextChunk(lastTextChunk.GetText(), tclStrat.CreateLocation(renderInfo, new LineSegment
                            (mergedStart, mergedEnd)));
                        locationalResult[locationalResult.Count - 1] = merged;
                    }
                    else {
                        String actualText = renderInfo.GetActualText();
                        TextChunk tc = new TextChunk(actualText != null ? actualText : renderInfo.GetText(), tclStrat.CreateLocation
                            (renderInfo, segment));
                        locationalResult.Add(tc);
                    }
                }
                else {
                    TextChunk tc = new TextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
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
            IList<TextChunk> textChunks = new List<TextChunk>(locationalResult);
            SortWithMarks(textChunks);
            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;
            foreach (TextChunk chunk in textChunks) {
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
        protected internal virtual bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk) {
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
            foreach (TextChunk location in locationalResult) {
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

        private void SortWithMarks(IList<TextChunk> textChunks) {
            IDictionary<TextChunk, LocationTextExtractionStrategy.TextChunkMarks> marks = new Dictionary<TextChunk, LocationTextExtractionStrategy.TextChunkMarks
                >();
            IList<TextChunk> toSort = new List<TextChunk>();
            for (int markInd = 0; markInd < textChunks.Count; markInd++) {
                ITextChunkLocation location = textChunks[markInd].GetLocation();
                if (location.GetStartLocation().Equals(location.GetEndLocation())) {
                    bool foundBaseToAttachTo = false;
                    for (int baseInd = 0; baseInd < textChunks.Count; baseInd++) {
                        if (markInd != baseInd) {
                            ITextChunkLocation baseLocation = textChunks[baseInd].GetLocation();
                            if (!baseLocation.GetStartLocation().Equals(baseLocation.GetEndLocation()) && TextChunkLocationDefaultImp.
                                ContainsMark(baseLocation, location)) {
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
            JavaCollectionsUtil.Sort(toSort, new TextChunkLocationBasedComparator(new DefaultTextChunkLocationComparator
                (!rightToLeftRunDirection)));
            textChunks.Clear();
            foreach (TextChunk current in toSort) {
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

        public interface ITextChunkLocationStrategy {
            ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline);
        }

        private class TextChunkMarks {
            internal IList<TextChunk> preceding = new List<TextChunk>();

            internal IList<TextChunk> succeeding = new List<TextChunk>();
        }

        private sealed class ITextChunkLocationStrategyImpl : LocationTextExtractionStrategy.ITextChunkLocationStrategy {
            public ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline) {
                return new TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth
                    ());
            }
        }
    }
}
