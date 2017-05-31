using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class represents a single character and its bounding box</summary>
    public class CharacterRenderInfo : LocationTextExtractionStrategy.TextChunk {
        private Rectangle boundingBox;

        /// <summary>
        /// This method converts a List<CharacterRenderInfo>
        /// The datastructure that gets returned contains both the plaintext,
        /// as well as the mapping of indices (from the list to the string).
        /// </summary>
        /// <remarks>
        /// This method converts a List<CharacterRenderInfo>
        /// The datastructure that gets returned contains both the plaintext,
        /// as well as the mapping of indices (from the list to the string).
        /// These indices can differ; if there is sufficient spacing between two CharacterRenderInfo
        /// objects, this algorithm will decide to insert space. The inserted space will cause
        /// the indices to differ by at least 1.
        /// </remarks>
        /// <param name="cris"/>
        /// <returns/>
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

        private static LocationTextExtractionStrategy.ITextChunkLocation GetLocation(TextRenderInfo tri) {
            LineSegment baseline = tri.GetBaseline();
            return new LocationTextExtractionStrategy.TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint
                (), tri.GetSingleSpaceWidth());
        }

        internal class StringConversionInfo {
            internal IDictionary<int, int?> indexMap;

            internal String text;
        }
    }
}
