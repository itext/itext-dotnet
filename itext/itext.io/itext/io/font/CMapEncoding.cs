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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Cmap;
using iText.IO.Source;

namespace iText.IO.Font {
    /// <summary>Class representing CMap encoding in pdf document.</summary>
    public class CMapEncoding {
        private static readonly IList<byte[]> IDENTITY_H_V_CODESPACE_RANGES = JavaUtil.ArraysAsList(new byte[] { 0
            , 0 }, new byte[] { (byte)0xff, (byte)0xff });

        private readonly String cmap;

        private String uniMap;

        // true if CMap is Identity-H/V
        private bool isDirect;

        private CMapCidToCodepoint cid2Code;

        private CMapCodepointToCid code2Cid;

        private IList<byte[]> codeSpaceRanges;

        /// <summary>Creates a new CMap encoding.</summary>
        /// <param name="cmap">CMap name</param>
        public CMapEncoding(String cmap) {
            this.cmap = cmap;
            if (cmap.Equals(PdfEncodings.IDENTITY_H) || cmap.Equals(PdfEncodings.IDENTITY_V)) {
                isDirect = true;
            }
            // Actually this constructor is only called for Identity-H/V cmaps currently.
            // Even for hypothetical case of non-Identity-H/V, let's use Identity-H/V ranges (two byte ranges) for compatibility with previous behavior
            this.codeSpaceRanges = IDENTITY_H_V_CODESPACE_RANGES;
        }

        /// <summary>Creates a new CMap encoding.</summary>
        /// <param name="cmap">CMap name</param>
        /// <param name="uniMap">CMap to convert Unicode value to CID</param>
        public CMapEncoding(String cmap, String uniMap) {
            this.cmap = cmap;
            this.uniMap = uniMap;
            if (cmap.Equals(PdfEncodings.IDENTITY_H) || cmap.Equals(PdfEncodings.IDENTITY_V)) {
                isDirect = true;
                this.codeSpaceRanges = IDENTITY_H_V_CODESPACE_RANGES;
            }
            else {
                cid2Code = CjkResourceLoader.GetCidToCodepointCmap(cmap);
                code2Cid = iText.IO.Font.CMapEncoding.GetCodeToCidCmap(cmap, cid2Code);
                this.codeSpaceRanges = cid2Code.GetCodeSpaceRanges();
            }
        }

        /// <summary>Creates a new CMap encoding.</summary>
        /// <param name="cmap">CMap name</param>
        /// <param name="cmapBytes">CMap binary data</param>
        public CMapEncoding(String cmap, byte[] cmapBytes) {
            this.cmap = cmap;
            cid2Code = new CMapCidToCodepoint();
            try {
                CMapParser.ParseCid(cmap, cid2Code, new CMapLocationFromBytes(cmapBytes));
                code2Cid = iText.IO.Font.CMapEncoding.GetCodeToCidCmap(cmap, cid2Code);
                this.codeSpaceRanges = cid2Code.GetCodeSpaceRanges();
            }
            catch (System.IO.IOException) {
                ITextLogManager.GetLogger(GetType()).LogError(iText.IO.Logs.IoLogMessageConstant.FAILED_TO_PARSE_ENCODING_STREAM
                    );
            }
        }

        /// <summary>Checks if CMap is direct or indirect pdf object</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if direct,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsDirect() {
            return isDirect;
        }

        /// <summary>Checks if CMap to convert Unicode value to CID is present.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if present,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasUniMap() {
            return uniMap != null && uniMap.Length > 0;
        }

        /// <summary>Gets string identifying the issuer of the character collection.</summary>
        /// <returns>name of the issuer</returns>
        public virtual String GetRegistry() {
            if (IsDirect()) {
                return "Adobe";
            }
            else {
                return cid2Code.GetRegistry();
            }
        }

        /// <summary>Gets string that uniquely names the character collection within the specified registry.</summary>
        /// <returns>character collection name</returns>
        public virtual String GetOrdering() {
            if (IsDirect()) {
                return "Identity";
            }
            else {
                return cid2Code.GetOrdering();
            }
        }

        /// <summary>Gets the supplement number of the character collection</summary>
        /// <returns>supplement number</returns>
        public virtual int GetSupplement() {
            if (IsDirect()) {
                return 0;
            }
            else {
                return cid2Code.GetSupplement();
            }
        }

        /// <summary>Gets CMap name which converts Unicode value to CID.</summary>
        /// <returns>CMap name</returns>
        public virtual String GetUniMapName() {
            return uniMap;
        }

        /// <summary>Gets CMap name.</summary>
        /// <returns>CMap name</returns>
        public virtual String GetCmapName() {
            return cmap;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="CMapEncoding"/>
        /// was built with corresponding cmap name.
        /// </summary>
        /// <param name="cmap">a CMAP</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// , if the CMapEncoding was built with the cmap,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsBuiltWith(String cmap) {
            return Object.Equals(cmap, this.cmap);
        }

        /// <summary>Gets CMap bytes by CID.</summary>
        /// <param name="cid">id of the CMap</param>
        /// <returns>cmap as byte array</returns>
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
                return code2Cid.Lookup(cmapCode);
            }
        }

        public virtual IList<byte[]> GetCodeSpaceRanges() {
            return codeSpaceRanges;
        }

        private static CMapCodepointToCid GetCodeToCidCmap(String cmap, CMapCidToCodepoint cid2Code) {
            try {
                return CjkResourceLoader.GetCodepointToCidCmap(cmap);
            }
            catch (iText.IO.Exceptions.IOException) {
                // if not found, fall back to reversing
                return new CMapCodepointToCid(cid2Code);
            }
        }
    }
}
