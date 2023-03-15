/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
