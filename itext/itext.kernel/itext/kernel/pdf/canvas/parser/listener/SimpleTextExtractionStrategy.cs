/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    public class SimpleTextExtractionStrategy : ITextExtractionStrategy {
        private Vector lastStart;

        private Vector lastEnd;

        /// <summary>used to store the resulting String.</summary>
        private readonly StringBuilder result = new StringBuilder();

        public virtual void EventOccurred(IEventData data, EventType type) {
            if (type.Equals(EventType.RENDER_TEXT)) {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                bool firstRender = result.Length == 0;
                bool hardReturn = false;
                LineSegment segment = renderInfo.GetBaseline();
                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();
                if (!firstRender) {
                    Vector x1 = lastStart;
                    Vector x2 = lastEnd;
                    // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                    float dist = (x2.Subtract(x1)).Cross((x1.Subtract(start))).LengthSquared() / x2.Subtract(x1).LengthSquared
                        ();
                    // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
                    float sameLineThreshold = 1f;
                    if (dist > sameLineThreshold) {
                        hardReturn = true;
                    }
                }
                // Note:  Technically, we should check both the start and end positions, in case the angle of the text changed without any displacement
                // but this sort of thing probably doesn't happen much in reality, so we'll leave it alone for now
                if (hardReturn) {
                    //System.out.println("<< Hard Return >>");
                    AppendTextChunk("\n");
                }
                else {
                    if (!firstRender) {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (result[result.Length - 1] != ' ' && renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ') {
                            float spacing = lastEnd.Subtract(start).Length();
                            if (spacing > renderInfo.GetSingleSpaceWidth() / 2f) {
                                AppendTextChunk(" ");
                            }
                        }
                    }
                }
                //System.out.println("Inserting implied space before '" + renderInfo.getText() + "'");
                //System.out.println("Displaying first string of content '" + text + "' :: x1 = " + x1);
                //System.out.println("[" + renderInfo.getStartPoint() + "]->[" + renderInfo.getEndPoint() + "] " + renderInfo.getText());
                AppendTextChunk(renderInfo.GetText());
                lastStart = start;
                lastEnd = end;
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return JavaCollectionsUtil.UnmodifiableSet(new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(
                EventType.RENDER_TEXT)));
        }

        /// <summary>Returns the result so far.</summary>
        /// <returns>a String with the resulting text.</returns>
        public virtual String GetResultantText() {
            return result.ToString();
        }

        /// <summary>Used to actually append text to the text results.</summary>
        /// <remarks>
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        /// text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        /// image content)
        /// </remarks>
        /// <param name="text">the text to append to the text results accumulated so far</param>
        protected internal void AppendTextChunk(String text) {
            result.Append(text);
        }
    }
}
