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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Class that provides methods for searching mcr in tag tree.</summary>
    public sealed class McrCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="McrCheckUtil"/>
        /// instance.
        /// </summary>
        private McrCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if tag structure of TR element contains any mcr.</summary>
        /// <param name="elementTR">PdfDictionary of TR element.</param>
        /// <returns>true if mcr found.</returns>
        public static bool IsTrContainsMcr(PdfDictionary elementTR) {
            TagTreeIterator tagTreeIterator = new TagTreeIterator(new PdfStructElem(elementTR));
            McrCheckUtil.McrTagHandler handler = new McrCheckUtil.McrTagHandler();
            tagTreeIterator.AddHandler(handler);
            tagTreeIterator.Traverse();
            return handler.TagTreeHaveMcr();
        }

        /// <summary>Search for mcr elements in the TagTree.</summary>
        private class McrTagHandler : ITagTreeIteratorHandler {
            private bool haveMcr = false;

            /// <summary>Method returns if tag tree has mcr in it.</summary>
            public virtual bool TagTreeHaveMcr() {
                return haveMcr;
            }

            /// <summary>
            /// Creates a new
            /// <see cref="McrTagHandler"/>
            /// instance.
            /// </summary>
            public McrTagHandler() {
            }

            //empty constructor
            public virtual bool Accept(IStructureNode node) {
                return node != null;
            }

            public virtual void ProcessElement(IStructureNode elem) {
                if ((elem is PdfMcr)) {
                    haveMcr = true;
                }
            }
        }
    }
}
