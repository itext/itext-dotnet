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
