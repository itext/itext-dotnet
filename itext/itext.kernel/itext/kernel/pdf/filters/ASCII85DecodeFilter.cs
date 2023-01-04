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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles ASCII85Decode filter</summary>
    public class ASCII85DecodeFilter : MemoryLimitsAwareFilter {
        /// <summary>Decodes the input bytes according to ASCII85.</summary>
        /// <param name="in">the byte[] to be decoded</param>
        /// <returns>the decoded byte[]</returns>
        public static byte[] ASCII85Decode(byte[] @in) {
            return ASCII85DecodeInternal(@in, new MemoryStream());
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            MemoryStream outputStream = EnableMemoryLimitsAwareHandler(streamDictionary);
            b = ASCII85DecodeInternal(b, outputStream);
            return b;
        }

        /// <summary>Decodes the input bytes according to ASCII85.</summary>
        /// <param name="in">the byte[] to be decoded</param>
        /// <param name="out">the out stream which will be used to write the bytes.</param>
        /// <returns>the decoded byte[]</returns>
        private static byte[] ASCII85DecodeInternal(byte[] @in, MemoryStream @out) {
            int state = 0;
            int[] chn = new int[5];
            for (int k = 0; k < @in.Length; ++k) {
                int ch = @in[k] & 0xff;
                if (ch == '~') {
                    break;
                }
                if (PdfTokenizer.IsWhitespace(ch)) {
                    continue;
                }
                if (ch == 'z' && state == 0) {
                    @out.Write(0);
                    @out.Write(0);
                    @out.Write(0);
                    @out.Write(0);
                    continue;
                }
                if (ch < '!' || ch > 'u') {
                    throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_CHARACTER_IN_ASCII85DECODE);
                }
                chn[state] = ch - '!';
                ++state;
                if (state == 5) {
                    state = 0;
                    int r = 0;
                    for (int j = 0; j < 5; ++j) {
                        r = r * 85 + chn[j];
                    }
                    @out.Write((byte)(r >> 24));
                    @out.Write((byte)(r >> 16));
                    @out.Write((byte)(r >> 8));
                    @out.Write((byte)r);
                }
            }
            if (state == 2) {
                int r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + 85 * 85 * 85 + 85 * 85 + 85;
                @out.Write((byte)(r >> 24));
            }
            else {
                if (state == 3) {
                    int r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + chn[2] * 85 * 85 + 85 * 85 + 85;
                    @out.Write((byte)(r >> 24));
                    @out.Write((byte)(r >> 16));
                }
                else {
                    if (state == 4) {
                        int r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + chn[2] * 85 * 85 + chn[3] * 85 + 85;
                        @out.Write((byte)(r >> 24));
                        @out.Write((byte)(r >> 16));
                        @out.Write((byte)(r >> 8));
                    }
                }
            }
            return @out.ToArray();
        }
    }
}
