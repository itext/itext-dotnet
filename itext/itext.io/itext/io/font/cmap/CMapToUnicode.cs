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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <summary>This class represents a CMap file.</summary>
    public class CMapToUnicode : AbstractCMap {
        public static readonly iText.IO.Font.Cmap.CMapToUnicode EMPTY_CMAP = new iText.IO.Font.Cmap.CMapToUnicode(
            true);

        private readonly IDictionary<int, char[]> byteMappings;

        private readonly IList<byte[]> codeSpaceRanges = new List<byte[]>();

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
            uni.AddCodeSpaceRange(new byte[] { 0, 0 }, new byte[] { (byte)0xff, (byte)0xff });
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

        /// <summary>
        /// Returns a list containing sequential pairs of code space beginning and endings:
        /// (begincodespacerange1, endcodespacerange1, begincodespacerange2, endcodespacerange1, ...)
        /// </summary>
        /// <returns>
        /// list of
        /// <c>byte[]</c>
        /// that contain code space ranges
        /// </returns>
        public virtual IList<byte[]> GetCodeSpaceRanges() {
            return codeSpaceRanges;
        }

//\cond DO_NOT_DOCUMENT
        internal override void AddCodeSpaceRange(byte[] low, byte[] high) {
            codeSpaceRanges.Add(low);
            codeSpaceRanges.Add(high);
        }
//\endcond

        private int ConvertToInt(char[] s) {
            int value = 0;
            for (int i = 0; i < s.Length - 1; i++) {
                value += s[i];
                value <<= 8;
            }
            value += s[s.Length - 1];
            return value;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void AddChar(int cid, char[] uni) {
            byteMappings.Put(cid, uni);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

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
