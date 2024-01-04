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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    public class OtfClass {
        public const int GLYPH_BASE = 1;

        public const int GLYPH_LIGATURE = 2;

        public const int GLYPH_MARK = 3;

        //key is glyph, value is class inside all 2
        private IntHashtable mapClass = new IntHashtable();

        private OtfClass(RandomAccessFileOrArray rf, int classLocation) {
            rf.Seek(classLocation);
            int classFormat = rf.ReadUnsignedShort();
            if (classFormat == 1) {
                int startGlyph = rf.ReadUnsignedShort();
                int glyphCount = rf.ReadUnsignedShort();
                int endGlyph = startGlyph + glyphCount;
                for (int k = startGlyph; k < endGlyph; ++k) {
                    int cl = rf.ReadUnsignedShort();
                    mapClass.Put(k, cl);
                }
            }
            else {
                if (classFormat == 2) {
                    int classRangeCount = rf.ReadUnsignedShort();
                    for (int k = 0; k < classRangeCount; ++k) {
                        int glyphStart = rf.ReadUnsignedShort();
                        int glyphEnd = rf.ReadUnsignedShort();
                        int cl = rf.ReadUnsignedShort();
                        for (; glyphStart <= glyphEnd; ++glyphStart) {
                            mapClass.Put(glyphStart, cl);
                        }
                    }
                }
                else {
                    throw new System.IO.IOException("Invalid class format " + classFormat);
                }
            }
        }

        public static iText.IO.Font.Otf.OtfClass Create(RandomAccessFileOrArray rf, int classLocation) {
            iText.IO.Font.Otf.OtfClass otfClass;
            try {
                otfClass = new iText.IO.Font.Otf.OtfClass(rf, classLocation);
            }
            catch (System.IO.IOException e) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Otf.OtfClass));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OPENTYPE_GDEF_TABLE_ERROR, e.Message
                    ));
                otfClass = null;
            }
            return otfClass;
        }

        public virtual int GetOtfClass(int glyph) {
            return mapClass.Get(glyph);
        }

        public virtual bool IsMarkOtfClass(int glyph) {
            return HasClass(glyph) && GetOtfClass(glyph) == GLYPH_MARK;
        }

        public virtual bool HasClass(int glyph) {
            return mapClass.ContainsKey(glyph);
        }

        public virtual int GetOtfClass(int glyph, bool strict) {
            if (strict) {
                if (mapClass.ContainsKey(glyph)) {
                    return mapClass.Get(glyph);
                }
                else {
                    return -1;
                }
            }
            else {
                return mapClass.Get(glyph);
            }
        }
    }
}
