/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
        /// This method converts a List<CharacterRenderInfo>
        /// The data structure that gets returned contains both the plaintext,
        /// as well as the mapping of indices (from the list to the string).
        /// </summary>
        /// <remarks>
        /// This method converts a List<CharacterRenderInfo>
        /// The data structure that gets returned contains both the plaintext,
        /// as well as the mapping of indices (from the list to the string).
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
                    indexMap.Put(sb.Length, i);
                    sb.Append(chunk.GetText());
                }
                else {
                    if (chunk.SameLine(lastChunk)) {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (chunk.GetLocation().IsAtWordBoundary(lastChunk.GetLocation()) && !chunk.GetText().StartsWith(" ") && !
                            chunk.GetText().EndsWith(" ")) {
                            sb.Append(' ');
                        }
                        indexMap.Put(sb.Length, i);
                        sb.Append(chunk.GetText());
                    }
                    else {
                        indexMap.Put(sb.Length, i);
                        sb.Append(chunk.GetText());
                    }
                }
                lastChunk = chunk;
            }
            CharacterRenderInfo.StringConversionInfo ret = new CharacterRenderInfo.StringConversionInfo();
            ret.indexMap = indexMap;
            ret.text = sb.ToString();
            return ret;
        }

        public CharacterRenderInfo(TextRenderInfo tri)
            : base(tri == null ? "" : tri.GetText(), tri == null ? null : GetLocation(tri)) {
            if (tri == null) {
                throw new ArgumentException("TextRenderInfo argument is not nullable.");
            }
            if (tri.GetText().Length != 1) {
                throw new ArgumentException("CharacterRenderInfo objects represent a single character. They should not be made from TextRenderInfo objects containing more than a single character of text."
                    );
            }
            // determine bounding box
            float x0 = tri.GetDescentLine().GetStartPoint().Get(0);
            float y0 = tri.GetDescentLine().GetStartPoint().Get(1);
            float h = tri.GetAscentLine().GetStartPoint().Get(1) - tri.GetDescentLine().GetStartPoint().Get(1);
            float w = Math.Abs(tri.GetBaseline().GetStartPoint().Get(0) - tri.GetBaseline().GetEndPoint().Get(0));
            this.boundingBox = new Rectangle(x0, y0, w, h);
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
