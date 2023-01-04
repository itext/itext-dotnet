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
using System.Text;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class EncodingUtil {
        private const String UTF8 = "UTF-8";

        static EncodingUtil() {
#if NETSTANDARD2_0
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        public static Encoding ISO_8859_1 = EncodingUtil.GetEncoding("ISO-8859-1");

        /// <summary>
        /// Converts to byte array an array of chars, taking the provided encoding into account.
        /// </summary>
        /// <param name="chars">an array of chars to be converted to bytes</param>
        /// <param name="encoding">the encoding to be taken into account while converting the provided array of chars</param>
        /// <returns>the resultant array of bytes</returns>
        public static byte[] ConvertToBytes(char[] chars, String encoding) {
            Encoding encw = IanaEncodings.GetEncodingEncoding(encoding);
            byte[] preamble = encw.GetPreamble();
            if (preamble.Length == 0) {
                return encw.GetBytes(chars);
            } else {
                byte[] encoded = encw.GetBytes(chars);
                byte[] total = new byte[encoded.Length + preamble.Length];
                Array.Copy(preamble, 0, total, 0, preamble.Length);
                Array.Copy(encoded, 0, total, preamble.Length, encoded.Length);
                return total;
            }
        }

        /// <summary>
        /// Converts to String an array of bytes, taking the provided encoding into account.
        /// </summary>
        /// <param name="chars">an array of bytes to be converted to String</param>
        /// <param name="encoding">the encoding to be taken into account while converting the provided bytes</param>
        /// <returns>the resultant string</returns>
        public static String ConvertToString(byte[] bytes, String encoding) {
            if (encoding.Equals(EncodingUtil.UTF8) &&
                bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return IanaEncodings.GetEncodingEncoding(encoding).GetString(bytes, 3, bytes.Length - 3);
            String nameU = encoding.ToUpperInvariant();
            Encoding enc = null;
            if (nameU.Equals("UNICODEBIGUNMARKED"))
                enc = new UnicodeEncoding(true, false);
            else if (nameU.Equals("UNICODELITTLEUNMARKED"))
                enc = new UnicodeEncoding(false, false);
            if (enc != null)
                return enc.GetString(bytes);
            bool marker = false;
            bool big = false;
            int offset = 0;
            if (bytes.Length >= 2) {
                if (bytes[0] == 0xFE && bytes[1] == 0xFF) {
                    marker = true;
                    big = true;
                    offset = 2;
                } else if (bytes[0] == 0xFF && bytes[1] == 0xFE) {
                    marker = true;
                    offset = 2;
                }
            }
            if (nameU.Equals("UNICODEBIG"))
                enc = new UnicodeEncoding(!marker || big, false);
            else if (nameU.Equals("UNICODELITTLE"))
                enc = new UnicodeEncoding(marker && big, false);
            if (enc != null)
                return enc.GetString(bytes, offset, bytes.Length - offset);
            return IanaEncodings.GetEncodingEncoding(encoding).GetString(bytes);
        }

        public static Encoding GetEncoding(String encodingName) {
            return Encoding.GetEncoding(encodingName);
        }

        public static Encoding GetEncoding(int encodingName) {
            return Encoding.GetEncoding(encodingName);
        }

        public static Encoding GetEncoding(String encodingName, EncoderFallback encoderFallback, DecoderFallback decoderFallback) {
            return Encoding.GetEncoding(encodingName, encoderFallback, decoderFallback);
        }

        public static Encoding GetEncoding(int encodingName, EncoderFallback encoderFallback, DecoderFallback decoderFallback) {
            return Encoding.GetEncoding(encodingName, encoderFallback, decoderFallback);
        }
    }
}
