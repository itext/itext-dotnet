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
using System.IO;
using System.util.zlib;
using iText.Commons.Utils;
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
                    int bytesRead = dataStream.JRead(curr, 0, bytesPerRow);
                    if (bytesRead < bytesPerRow) {
                        JavaUtil.Fill(curr, bytesRead, bytesPerRow, (byte)0);
                    }
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
