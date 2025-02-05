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
using iText.IO.Source;

namespace iText.IO.Font.Otf {
    public class OpenTypeGdefTableReader {
//\cond DO_NOT_DOCUMENT
        internal const int FLAG_IGNORE_BASE = 2;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int FLAG_IGNORE_LIGATURE = 4;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int FLAG_IGNORE_MARK = 8;
//\endcond

        private readonly int tableLocation;

        private readonly RandomAccessFileOrArray rf;

        private OtfClass glyphClass;

        private OtfClass markAttachmentClass;

        public OpenTypeGdefTableReader(RandomAccessFileOrArray rf, int tableLocation) {
            this.rf = rf;
            this.tableLocation = tableLocation;
        }

        public virtual void ReadTable() {
            if (tableLocation > 0) {
                rf.Seek(tableLocation);
                // version, we only support 0x00010000
                rf.ReadUnsignedInt();
                int glyphClassDefOffset = rf.ReadUnsignedShort();
                // skip Attachment Point List Table
                rf.ReadUnsignedShort();
                // skip Ligature Caret List Table
                rf.ReadUnsignedShort();
                int markAttachClassDefOffset = rf.ReadUnsignedShort();
                if (glyphClassDefOffset > 0) {
                    glyphClass = OtfClass.Create(rf, glyphClassDefOffset + tableLocation);
                }
                if (markAttachClassDefOffset > 0) {
                    markAttachmentClass = OtfClass.Create(rf, markAttachClassDefOffset + tableLocation);
                }
            }
        }

        public virtual bool IsSkip(int glyph, int flag) {
            if (glyphClass != null && (flag & (FLAG_IGNORE_BASE | FLAG_IGNORE_LIGATURE | FLAG_IGNORE_MARK)) != 0) {
                int cla = glyphClass.GetOtfClass(glyph);
                if (cla == OtfClass.GLYPH_BASE && (flag & FLAG_IGNORE_BASE) != 0) {
                    return true;
                }
                if (cla == OtfClass.GLYPH_MARK && (flag & FLAG_IGNORE_MARK) != 0) {
                    return true;
                }
                if (cla == OtfClass.GLYPH_LIGATURE && (flag & FLAG_IGNORE_LIGATURE) != 0) {
                    return true;
                }
            }
            int markAttachmentType = (flag >> 8);
            // If MarkAttachmentType is non-zero, then mark attachment classes must be defined in the
            // Mark Attachment Class Definition Table in the GDEF table. When processing glyph sequences,
            // a lookup must ignore any mark glyphs that are not in the specified mark attachment class;
            // only marks of the specified type are processed.
            if (markAttachmentType != 0 && glyphClass != null) {
                int currentGlyphClass = glyphClass.GetOtfClass(glyph);
                // Will be 0 in case the class is not defined for this particular glyph
                int glyphMarkAttachmentClass = markAttachmentClass != null ? markAttachmentClass.GetOtfClass(glyph) : 0;
                return currentGlyphClass == OtfClass.GLYPH_MARK && glyphMarkAttachmentClass != markAttachmentType;
            }
            return false;
        }

        public virtual OtfClass GetGlyphClassTable() {
            return glyphClass;
        }
    }
}
