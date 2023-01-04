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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <summary>This class represents a CMap file.</summary>
    /// <author>Ben Litchfield (ben@benlitchfield.com)</author>
    public class CMapToUnicode : AbstractCMap {
        public static iText.IO.Font.Cmap.CMapToUnicode EmptyCMapToUnicodeMap = new iText.IO.Font.Cmap.CMapToUnicode
            (true);

        private IDictionary<int, char[]> byteMappings;

        private CMapToUnicode(bool emptyCMap) {
            byteMappings = JavaCollectionsUtil.EmptyMap<int, char[]>();
        }

        /// <summary>Creates a new instance of CMap.</summary>
        public CMapToUnicode() {
            byteMappings = new Dictionary<int, char[]>();
        }

        public static iText.IO.Font.Cmap.CMapToUnicode GetIdentity() {
            iText.IO.Font.Cmap.CMapToUnicode uni = new iText.IO.Font.Cmap.CMapToUnicode();
            for (int i = 0; i < 65537; i++) {
                uni.AddChar(i, iText.IO.Util.TextUtil.ConvertFromUtf32(i));
            }
            return uni;
        }

        /// <summary>This will tell if this cmap has any two byte mappings.</summary>
        /// <returns>true If there are any two byte mappings, false otherwise.</returns>
        public virtual bool HasByteMappings() {
            return byteMappings.Count != 0;
        }

        /// <summary>This will perform a lookup into the map.</summary>
        /// <param name="code">The code used to lookup.</param>
        /// <param name="offset">The offset into the byte array.</param>
        /// <param name="length">The length of the data we are getting.</param>
        /// <returns>The string that matches the lookup.</returns>
        public virtual char[] Lookup(byte[] code, int offset, int length) {
            char[] result = null;
            int key;
            if (length == 1) {
                key = code[offset] & 0xff;
                result = byteMappings.Get(key);
            }
            else {
                if (length == 2) {
                    int intKey = code[offset] & 0xff;
                    intKey <<= 8;
                    intKey += code[offset + 1] & 0xff;
                    key = intKey;
                    result = byteMappings.Get(key);
                }
            }
            return result;
        }

        public virtual char[] Lookup(byte[] code) {
            return Lookup(code, 0, code.Length);
        }

        public virtual char[] Lookup(int code) {
            return byteMappings.Get(code);
        }

        public virtual ICollection<int> GetCodes() {
            return byteMappings.Keys;
        }

        public virtual IntHashtable CreateDirectMapping() {
            IntHashtable result = new IntHashtable();
            foreach (KeyValuePair<int, char[]> entry in byteMappings) {
                if (entry.Value.Length == 1) {
                    result.Put((int)entry.Key, ConvertToInt(entry.Value));
                }
            }
            return result;
        }

        public virtual IDictionary<int, int?> CreateReverseMapping() {
            IDictionary<int, int?> result = new Dictionary<int, int?>();
            foreach (KeyValuePair<int, char[]> entry in byteMappings) {
                if (entry.Value.Length == 1) {
                    result.Put(ConvertToInt(entry.Value), entry.Key);
                }
            }
            return result;
        }

        private int ConvertToInt(char[] s) {
            int value = 0;
            for (int i = 0; i < s.Length - 1; i++) {
                value += s[i];
                value <<= 8;
            }
            value += s[s.Length - 1];
            return value;
        }

        internal virtual void AddChar(int cid, char[] uni) {
            byteMappings.Put(cid, uni);
        }

        internal override void AddChar(String mark, CMapObject code) {
            if (mark.Length == 1) {
                char[] dest = CreateCharsFromDoubleBytes((byte[])code.GetValue());
                byteMappings.Put((int)mark[0], dest);
            }
            else {
                if (mark.Length == 2) {
                    char[] dest = CreateCharsFromDoubleBytes((byte[])code.GetValue());
                    byteMappings.Put((mark[0] << 8) + mark[1], dest);
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Cmap.CMapToUnicode));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.TOUNICODE_CMAP_MORE_THAN_2_BYTES_NOT_SUPPORTED);
                }
            }
        }

        private char[] CreateCharsFromSingleBytes(byte[] bytes) {
            if (bytes.Length == 1) {
                return new char[] { (char)(bytes[0] & 0xff) };
            }
            else {
                char[] chars = new char[bytes.Length];
                for (int i = 0; i < bytes.Length; i++) {
                    chars[i] = (char)(bytes[i] & 0xff);
                }
                return chars;
            }
        }

        private char[] CreateCharsFromDoubleBytes(byte[] bytes) {
            char[] chars = new char[bytes.Length / 2];
            for (int i = 0; i < bytes.Length; i += 2) {
                chars[i / 2] = (char)(((bytes[i] & 0xff) << 8) + (bytes[i + 1] & 0xff));
            }
            return chars;
        }
    }
}
