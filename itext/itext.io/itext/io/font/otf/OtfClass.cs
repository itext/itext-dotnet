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
