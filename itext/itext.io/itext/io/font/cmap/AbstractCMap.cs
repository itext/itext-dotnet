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
using iText.IO.Font;

namespace iText.IO.Font.Cmap {
    public abstract class AbstractCMap {
        private String cmapName;

        private String registry;

        private String ordering;

        private int supplement;

        public virtual String GetName() {
            return cmapName;
        }

        internal virtual void SetName(String cmapName) {
            this.cmapName = cmapName;
        }

        public virtual String GetOrdering() {
            return ordering;
        }

        internal virtual void SetOrdering(String ordering) {
            this.ordering = ordering;
        }

        public virtual String GetRegistry() {
            return registry;
        }

        internal virtual void SetRegistry(String registry) {
            this.registry = registry;
        }

        public virtual int GetSupplement() {
            return supplement;
        }

        internal virtual void SetSupplement(int supplement) {
            this.supplement = supplement;
        }

        internal abstract void AddChar(String mark, CMapObject code);

        internal virtual void AddCodeSpaceRange(byte[] low, byte[] high) {
        }

        internal virtual void AddRange(String from, String to, CMapObject code) {
            byte[] a1 = DecodeStringToByte(from);
            byte[] a2 = DecodeStringToByte(to);
            if (a1.Length != a2.Length || a1.Length == 0) {
                throw new ArgumentException("Invalid map.");
            }
            byte[] sout = null;
            if (code.IsString()) {
                sout = DecodeStringToByte(code.ToString());
            }
            int start = ByteArrayToInt(a1);
            int end = ByteArrayToInt(a2);
            for (int k = start; k <= end; ++k) {
                IntToByteArray(k, a1);
                String mark = PdfEncodings.ConvertToString(a1, null);
                if (code.IsArray()) {
                    IList<CMapObject> codes = (List<CMapObject>)code.GetValue();
                    AddChar(mark, codes[k - start]);
                }
                else {
                    if (code.IsNumber()) {
                        int nn = (int)code.GetValue() + k - start;
                        AddChar(mark, new CMapObject(CMapObject.NUMBER, nn));
                    }
                    else {
                        if (code.IsString()) {
                            CMapObject s1 = new CMapObject(CMapObject.HEX_STRING, sout);
                            AddChar(mark, s1);
                            System.Diagnostics.Debug.Assert(sout != null);
                            IntToByteArray(ByteArrayToInt(sout) + 1, sout);
                        }
                    }
                }
            }
        }

        //    protected static byte[] toByteArray(String value) {
        //        if (PdfEncodings.isPdfDocEncoding(value)) {
        //            return PdfEncodings.convertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
        //        } else {
        //            return PdfEncodings.convertToBytes(value, null);
        //        }
        //    }
        public static byte[] DecodeStringToByte(String range) {
            byte[] bytes = new byte[range.Length];
            for (int i = 0; i < range.Length; i++) {
                bytes[i] = (byte)range[i];
            }
            return bytes;
        }

        protected internal virtual String ToUnicodeString(String value, bool isHexWriting) {
            byte[] bytes = DecodeStringToByte(value);
            if (isHexWriting) {
                return PdfEncodings.ConvertToString(bytes, PdfEncodings.UNICODE_BIG_UNMARKED);
            }
            else {
                if (bytes.Length >= 2 && bytes[0] == (byte)0xfe && bytes[1] == (byte)0xff) {
                    return PdfEncodings.ConvertToString(bytes, PdfEncodings.UNICODE_BIG);
                }
                else {
                    return PdfEncodings.ConvertToString(bytes, PdfEncodings.PDF_DOC_ENCODING);
                }
            }
        }

        private static void IntToByteArray(int n, byte[] b) {
            for (int k = b.Length - 1; k >= 0; --k) {
                b[k] = (byte)n;
                n = (int)(((uint)n) >> 8);
            }
        }

        private static int ByteArrayToInt(byte[] b) {
            int n = 0;
            for (int k = 0; k < b.Length; ++k) {
                n = n << 8;
                n |= b[k] & 0xff;
            }
            return n;
        }
    }
}
