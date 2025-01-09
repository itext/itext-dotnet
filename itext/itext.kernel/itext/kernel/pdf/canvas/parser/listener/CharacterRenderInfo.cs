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
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class represents a single character and its bounding box</summary>
    public class CharacterRenderInfo : TextChunk {
        private Rectangle boundingBox;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// This method converts a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="CharacterRenderInfo"/>.
        /// </summary>
        /// <remarks>
        /// This method converts a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="CharacterRenderInfo"/>.
        /// The returned data structure contains both the plaintext
        /// and the mapping of indices (from the list to the string).
        /// These indices can differ; if there is sufficient spacing between two CharacterRenderInfo
        /// objects, this algorithm will decide to insert space. The inserted space will cause
        /// the indices to differ by at least 1.
        /// </remarks>
        internal static CharacterRenderInfo.StringConversionInfo MapString(IList<iText.Kernel.Pdf.Canvas.Parser.Listener.CharacterRenderInfo
            > cris) {
            IDictionary<int, int?> indexMap = new Dictionary<int, int?>();
            StringBuilder sb = new StringBuilder();
            iText.Kernel.Pdf.Canvas.Parser.Listener.CharacterRenderInfo lastChunk = null;
            for (int i = 0; i < cris.Count; i++) {
                iText.Kernel.Pdf.Canvas.Parser.Listener.CharacterRenderInfo chunk = cris[i];
                if (lastChunk == null) {
                    PutCharsWithIndex(chunk.GetText(), i, indexMap, sb);
                }
                else {
                    if (chunk.SameLine(lastChunk)) {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (chunk.GetLocation().IsAtWordBoundary(lastChunk.GetLocation()) && !chunk.GetText().StartsWith(" ") && !
                            chunk.GetText().EndsWith(" ")) {
                            sb.Append(' ');
                        }
                        PutCharsWithIndex(chunk.GetText(), i, indexMap, sb);
                    }
                    else {
                        // we insert a newline character in the resulting string if the chunks are placed on different lines
                        sb.Append('\n');
                        PutCharsWithIndex(chunk.GetText(), i, indexMap, sb);
                    }
                }
                lastChunk = chunk;
            }
            CharacterRenderInfo.StringConversionInfo ret = new CharacterRenderInfo.StringConversionInfo();
            ret.indexMap = indexMap;
            ret.text = sb.ToString();
            return ret;
        }
//\endcond

        private static void PutCharsWithIndex(String seq, int index, IDictionary<int, int?> indexMap, StringBuilder
             sb) {
            int charCount = seq.Length;
            for (int i = 0; i < charCount; i++) {
                indexMap.Put(sb.Length, index);
                sb.Append(seq[i]);
            }
        }

        public CharacterRenderInfo(TextRenderInfo tri)
            : base(tri == null ? "" : tri.GetText(), tri == null ? null : GetLocation(tri)) {
            if (tri == null) {
                throw new ArgumentException("TextRenderInfo argument is not nullable.");
            }
            // determine bounding box
            IList<Point> points = new List<Point>();
            points.Add(new Point(tri.GetDescentLine().GetStartPoint().Get(0), tri.GetDescentLine().GetStartPoint().Get
                (1)));
            points.Add(new Point(tri.GetDescentLine().GetEndPoint().Get(0), tri.GetDescentLine().GetEndPoint().Get(1))
                );
            points.Add(new Point(tri.GetAscentLine().GetStartPoint().Get(0), tri.GetAscentLine().GetStartPoint().Get(1
                )));
            points.Add(new Point(tri.GetAscentLine().GetEndPoint().Get(0), tri.GetAscentLine().GetEndPoint().Get(1)));
            this.boundingBox = Rectangle.CalculateBBox(points);
        }

        public virtual Rectangle GetBoundingBox() {
            return boundingBox;
        }

        private static ITextChunkLocation GetLocation(TextRenderInfo tri) {
            LineSegment baseline = tri.GetBaseline();
            return new TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint(), tri.GetSingleSpaceWidth
                ());
        }

//\cond DO_NOT_DOCUMENT
        internal class StringConversionInfo {
//\cond DO_NOT_DOCUMENT
            internal IDictionary<int, int?> indexMap;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String text;
//\endcond
        }
//\endcond
    }
}
