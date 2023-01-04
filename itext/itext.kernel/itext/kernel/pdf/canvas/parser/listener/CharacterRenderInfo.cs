/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class represents a single character and its bounding box</summary>
    public class CharacterRenderInfo : TextChunk {
        private Rectangle boundingBox;

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

        internal class StringConversionInfo {
            internal IDictionary<int, int?> indexMap;

            internal String text;
        }
    }
}
