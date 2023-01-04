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
using iText.IO.Font.Cmap;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font {
    public class CMapEncoding {
        private static readonly IList<byte[]> IDENTITY_H_V_CODESPACE_RANGES = JavaUtil.ArraysAsList(new byte[] { 0
            , 0 }, new byte[] { (byte)0xff, (byte)0xff });

        private String cmap;

        private String uniMap;

        // true if CMap is Identity-H/V
        private bool isDirect;

        private CMapCidUni cid2Uni;

        private CMapCidByte cid2Code;

        private IntHashtable code2Cid;

        private IList<byte[]> codeSpaceRanges;

        /// <param name="cmap">CMap name.</param>
        public CMapEncoding(String cmap) {
            this.cmap = cmap;
            if (cmap.Equals(PdfEncodings.IDENTITY_H) || cmap.Equals(PdfEncodings.IDENTITY_V)) {
                isDirect = true;
            }
            // Actually this constructor is only called for Identity-H/V cmaps currently.
            // Even for hypothetical case of non-Identity-H/V, let's use Identity-H/V ranges (two byte ranges) for compatibility with previous behavior
            this.codeSpaceRanges = IDENTITY_H_V_CODESPACE_RANGES;
        }

        /// <param name="cmap">CMap name.</param>
        /// <param name="uniMap">CMap to convert Unicode value to CID.</param>
        public CMapEncoding(String cmap, String uniMap) {
            this.cmap = cmap;
            this.uniMap = uniMap;
            if (cmap.Equals(PdfEncodings.IDENTITY_H) || cmap.Equals(PdfEncodings.IDENTITY_V)) {
                cid2Uni = FontCache.GetCid2UniCmap(uniMap);
                isDirect = true;
                this.codeSpaceRanges = IDENTITY_H_V_CODESPACE_RANGES;
            }
            else {
                cid2Code = FontCache.GetCid2Byte(cmap);
                code2Cid = cid2Code.GetReversMap();
                this.codeSpaceRanges = cid2Code.GetCodeSpaceRanges();
            }
        }

        public CMapEncoding(String cmap, byte[] cmapBytes) {
            this.cmap = cmap;
            cid2Code = new CMapCidByte();
            try {
                CMapParser.ParseCid(cmap, cid2Code, new CMapLocationFromBytes(cmapBytes));
                code2Cid = cid2Code.GetReversMap();
                this.codeSpaceRanges = cid2Code.GetCodeSpaceRanges();
            }
            catch (System.IO.IOException) {
                ITextLogManager.GetLogger(GetType()).LogError(iText.IO.Logs.IoLogMessageConstant.FAILED_TO_PARSE_ENCODING_STREAM
                    );
            }
        }

        public virtual bool IsDirect() {
            return isDirect;
        }

        public virtual bool HasUniMap() {
            return uniMap != null && uniMap.Length > 0;
        }

        public virtual String GetRegistry() {
            if (IsDirect()) {
                return "Adobe";
            }
            else {
                return cid2Code.GetRegistry();
            }
        }

        public virtual String GetOrdering() {
            if (IsDirect()) {
                return "Identity";
            }
            else {
                return cid2Code.GetOrdering();
            }
        }

        public virtual int GetSupplement() {
            if (IsDirect()) {
                return 0;
            }
            else {
                return cid2Code.GetSupplement();
            }
        }

        public virtual String GetUniMapName() {
            return uniMap;
        }

        public virtual String GetCmapName() {
            return cmap;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="CMapEncoding"/>
        /// was built with corresponding cmap name.
        /// </summary>
        /// <param name="cmap">a CMAP</param>
        /// <returns>true, if the CMapEncoding was built with the cmap. Otherwise false.</returns>
        public virtual bool IsBuiltWith(String cmap) {
            return Object.Equals(cmap, this.cmap);
        }

        public virtual byte[] GetCmapBytes(int cid) {
            int length = GetCmapBytesLength(cid);
            byte[] result = new byte[length];
            FillCmapBytes(cid, result, 0);
            return result;
        }

        public virtual int FillCmapBytes(int cid, byte[] array, int offset) {
            if (isDirect) {
                array[offset++] = (byte)((cid & 0xff00) >> 8);
                array[offset++] = (byte)(cid & 0xff);
            }
            else {
                byte[] bytes = cid2Code.Lookup(cid);
                for (int i = 0; i < bytes.Length; i++) {
                    array[offset++] = bytes[i];
                }
            }
            return offset;
        }

        public virtual void FillCmapBytes(int cid, ByteBuffer buffer) {
            if (isDirect) {
                buffer.Append((byte)((cid & 0xff00) >> 8));
                buffer.Append((byte)(cid & 0xff));
            }
            else {
                byte[] bytes = cid2Code.Lookup(cid);
                buffer.Append(bytes);
            }
        }

        public virtual int GetCmapBytesLength(int cid) {
            if (isDirect) {
                return 2;
            }
            else {
                return cid2Code.Lookup(cid).Length;
            }
        }

        public virtual int GetCidCode(int cmapCode) {
            if (isDirect) {
                return cmapCode;
            }
            else {
                return code2Cid.Get(cmapCode);
            }
        }

        public virtual bool ContainsCodeInCodeSpaceRange(int code, int length) {
            for (int i = 0; i < codeSpaceRanges.Count; i += 2) {
                if (length == codeSpaceRanges[i].Length) {
                    int mask = 0xff;
                    int totalShift = 0;
                    byte[] low = codeSpaceRanges[i];
                    byte[] high = codeSpaceRanges[i + 1];
                    bool fitsIntoRange = true;
                    for (int ind = length - 1; ind >= 0; ind--, totalShift += 8, mask <<= 8) {
                        int actualByteValue = (code & mask) >> totalShift;
                        if (!(actualByteValue >= (0xff & low[ind]) && actualByteValue <= (0xff & high[ind]))) {
                            fitsIntoRange = false;
                        }
                    }
                    if (fitsIntoRange) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
