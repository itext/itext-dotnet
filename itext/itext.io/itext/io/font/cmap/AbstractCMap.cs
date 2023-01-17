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
using iText.IO.Font;

namespace iText.IO.Font.Cmap {
    /// <author>psoares</author>
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
