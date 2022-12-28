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
using System.IO;
using System.util.zlib;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles FlateDecode filter.</summary>
    public class FlateDecodeFilter : MemoryLimitsAwareFilter {
        /// <summary>A helper to flateDecode.</summary>
        /// <param name="in">the input data</param>
        /// <param name="strict">
        /// 
        /// <see langword="true"/>
        /// to read a correct stream.
        /// <see langword="false"/>
        /// to try to read a corrupted stream.
        /// </param>
        /// <returns>the decoded data</returns>
        public static byte[] FlateDecode(byte[] @in, bool strict) {
            return FlateDecodeInternal(@in, strict, new MemoryStream());
        }

        /// <param name="in">Input byte array.</param>
        /// <param name="decodeParams">PdfDictionary of decodeParams.</param>
        /// <returns>a byte array</returns>
        public static byte[] DecodePredictor(byte[] @in, PdfObject decodeParams) {
            if (decodeParams == null || decodeParams.GetObjectType() != PdfObject.DICTIONARY) {
                return @in;
            }
            PdfDictionary dic = (PdfDictionary)decodeParams;
            PdfObject obj = dic.Get(PdfName.Predictor);
            if (obj == null || obj.GetObjectType() != PdfObject.NUMBER) {
                return @in;
            }
            int predictor = ((PdfNumber)obj).IntValue();
            if (predictor < 10 && predictor != 2) {
                return @in;
            }
            int width = GetNumberOrDefault(dic, PdfName.Columns, 1);
            int colors = GetNumberOrDefault(dic, PdfName.Colors, 1);
            int bpc = GetNumberOrDefault(dic, PdfName.BitsPerComponent, 8);
            BinaryReader dataStream = new BinaryReader(new MemoryStream(@in));
            MemoryStream fout = new MemoryStream(@in.Length);
            int bytesPerPixel = colors * bpc / 8;
            int bytesPerRow = (colors * width * bpc + 7) / 8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];
            if (predictor == 2) {
                if (bpc == 8) {
                    int numRows = @in.Length / bytesPerRow;
                    for (int row = 0; row < numRows; row++) {
                        int rowStart = row * bytesPerRow;
                        for (int col = bytesPerPixel; col < bytesPerRow; col++) {
                            @in[rowStart + col] = (byte)(@in[rowStart + col] + @in[rowStart + col - bytesPerPixel]);
                        }
                    }
                }
                return @in;
            }
            // Decode the (sub)image row-by-row
            while (true) {
                // Read the filter type byte and a row of data
                int filter;
                try {
                    filter = dataStream.Read();
                    if (filter < 0) {
                        return fout.ToArray();
                    }
                    dataStream.ReadFully(curr, 0, bytesPerRow);
                }
                catch (Exception) {
                    return fout.ToArray();
                }
                switch (filter) {
                    case 0: {
                        //PNG_FILTER_NONE
                        break;
                    }

                    case 1: {
                        //PNG_FILTER_SUB
                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            curr[i] += curr[i - bytesPerPixel];
                        }
                        break;
                    }

                    case 2: {
                        //PNG_FILTER_UP
                        for (int i = 0; i < bytesPerRow; i++) {
                            curr[i] += prior[i];
                        }
                        break;
                    }

                    case 3: {
                        //PNG_FILTER_AVERAGE
                        for (int i = 0; i < bytesPerPixel; i++) {
                            curr[i] += (byte)(prior[i] / 2);
                        }
                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            curr[i] += (byte)(((curr[i - bytesPerPixel] & 0xff) + (prior[i] & 0xff)) / 2);
                        }
                        break;
                    }

                    case 4: {
                        //PNG_FILTER_PAETH
                        for (int i = 0; i < bytesPerPixel; i++) {
                            curr[i] += prior[i];
                        }
                        for (int i = bytesPerPixel; i < bytesPerRow; i++) {
                            int a = curr[i - bytesPerPixel] & 0xff;
                            int b = prior[i] & 0xff;
                            int c = prior[i - bytesPerPixel] & 0xff;
                            int p = a + b - c;
                            int pa = Math.Abs(p - a);
                            int pb = Math.Abs(p - b);
                            int pc = Math.Abs(p - c);
                            int ret;
                            if (pa <= pb && pa <= pc) {
                                ret = a;
                            }
                            else {
                                if (pb <= pc) {
                                    ret = b;
                                }
                                else {
                                    ret = c;
                                }
                            }
                            curr[i] += (byte)ret;
                        }
                        break;
                    }

                    default: {
                        // Error -- unknown filter type
                        throw new PdfException(KernelExceptionMessageConstant.PNG_FILTER_UNKNOWN);
                    }
                }
                try {
                    fout.Write(curr);
                }
                catch (System.IO.IOException) {
                    // Never happens
                    System.Diagnostics.Debug.Assert(true, "Happens!");
                }
                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            MemoryStream outputStream = EnableMemoryLimitsAwareHandler(streamDictionary);
            byte[] res = FlateDecodeInternal(b, true, outputStream);
            if (res == null) {
                outputStream.JReset();
                res = FlateDecodeInternal(b, false, outputStream);
            }
            b = DecodePredictor(res, decodeParams);
            return b;
        }

        /// <summary>A helper to flateDecode.</summary>
        /// <param name="in">the input data</param>
        /// <param name="strict">
        /// 
        /// <see langword="true"/>
        /// to read a correct stream.
        /// <see langword="false"/>
        /// to try to read a corrupted stream.
        /// </param>
        /// <param name="out">the out stream which will be used to write the bytes.</param>
        /// <returns>the decoded data</returns>
        protected internal static byte[] FlateDecodeInternal(byte[] @in, bool strict, MemoryStream @out) {
            MemoryStream stream = new MemoryStream(@in);
            ZInflaterInputStream zip = new ZInflaterInputStream(stream);
            byte[] b = new byte[strict ? 4092 : 1];
            try {
                int n;
                while ((n = zip.Read(b)) >= 0) {
                    @out.Write(b, 0, n);
                }
                zip.Dispose();
                @out.Dispose();
                return @out.ToArray();
            }
            catch (MemoryLimitsAwareException e) {
                throw;
            }
            catch (Exception) {
                if (strict) {
                    return null;
                }
                return @out.ToArray();
            }
        }

        private static int GetNumberOrDefault(PdfDictionary dict, PdfName key, int defaultInt) {
            int result = defaultInt;
            PdfObject obj = dict.Get(key);
            if (obj != null && obj.GetObjectType() == PdfObject.NUMBER) {
                result = ((PdfNumber)obj).IntValue();
            }
            return result;
        }
    }
}
