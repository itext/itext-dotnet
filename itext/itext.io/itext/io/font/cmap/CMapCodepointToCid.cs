using System;
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <summary>Class represents real codepoint-CID mapping without any additional manipulation.</summary>
    /// <remarks>
    /// Class represents real codepoint-CID mapping without any additional manipulation.
    /// <para />
    /// See
    /// <see cref="CMapCidToCodepoint"/>
    /// for CID-codepoint representation.
    /// </remarks>
    public class CMapCodepointToCid : AbstractCMap {
        private readonly IntHashtable map;

        public CMapCodepointToCid() {
            map = new IntHashtable();
        }

        public CMapCodepointToCid(CMapCidToCodepoint reverseMap) {
            map = reverseMap.GetReversMap();
        }

        internal override void AddChar(String mark, CMapObject code) {
            if (code.IsNumber()) {
                byte[] ser = DecodeStringToByte(mark);
                int byteCode = 0;
                foreach (byte b in ser) {
                    byteCode <<= 8;
                    byteCode += b & 0xff;
                }
                map.Put(byteCode, (int)code.GetValue());
            }
        }

        public virtual int Lookup(int codepoint) {
            return this.map.Get(codepoint);
        }
    }
}
