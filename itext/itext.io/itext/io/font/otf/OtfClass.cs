/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    public class OtfClass {
        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.IO.Font.Otf.OtfClass));

        public const int GLYPH_BASE = 1;

        public const int GLYPH_LIGATURE = 2;

        public const int GLYPH_MARK = 3;

        // Key is glyph, value is class inside all 2
        private readonly IntHashtable mapClass = new IntHashtable();

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

        /// <summary>
        /// Creates new
        /// <see cref="OtfClass"/>
        /// instance.
        /// </summary>
        /// <param name="rf">
        /// 
        /// <see cref="iText.IO.Source.RandomAccessFileOrArray"/>
        /// </param>
        /// <param name="classLocation">class location</param>
        /// <returns>
        /// new
        /// <see cref="OtfClass"/>
        /// instance
        /// </returns>
        public static iText.IO.Font.Otf.OtfClass Create(RandomAccessFileOrArray rf, int classLocation) {
            iText.IO.Font.Otf.OtfClass otfClass;
            try {
                otfClass = new iText.IO.Font.Otf.OtfClass(rf, classLocation);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(() => MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OPENTYPE_GDEF_TABLE_ERROR, 
                    e.Message));
                otfClass = null;
            }
            return otfClass;
        }

        /// <summary>Returns the otf class for the passed glyph.</summary>
        /// <param name="glyph">the glyph</param>
        /// <returns>the requested result</returns>
        public virtual int GetOtfClass(int glyph) {
            return mapClass.Get(glyph);
        }

        /// <summary>Determines whether passed glyph is Mark or not.</summary>
        /// <param name="glyph">the glyph to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the passed glyph is Mark; otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsMarkOtfClass(int glyph) {
            return HasClass(glyph) && GetOtfClass(glyph) == GLYPH_MARK;
        }

        /// <summary>Determines whether passed glyph has class.</summary>
        /// <param name="glyph">the glyph</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if has; otherwise
        /// <see langword="false"/>.
        /// </returns>
        public virtual bool HasClass(int glyph) {
            return mapClass.ContainsKey(glyph);
        }

        /// <summary>Returns the otf class for the passed glyph.</summary>
        /// <param name="glyph">the glyph</param>
        /// <param name="strict">
        /// boolean value identifying whether the check if passed glyph has class should be done first
        /// (-1 is returned if glyph doesn't have class and strict is true)
        /// </param>
        /// <returns>the requested result</returns>
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
