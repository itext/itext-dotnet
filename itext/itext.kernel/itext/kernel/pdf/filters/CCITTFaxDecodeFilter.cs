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
using iText.IO.Codec;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles CCITTFaxDecode filter</summary>
    public class CCITTFaxDecodeFilter : IFilterHandler {
        public virtual byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            PdfNumber wn = streamDictionary.GetAsNumber(PdfName.Width);
            PdfNumber hn = streamDictionary.GetAsNumber(PdfName.Height);
            if (wn == null || hn == null) {
                throw new PdfException(KernelExceptionMessageConstant.FILTER_CCITTFAXDECODE_IS_ONLY_SUPPORTED_FOR_IMAGES);
            }
            int width = wn.IntValue();
            int height = hn.IntValue();
            PdfDictionary param = decodeParams is PdfDictionary ? (PdfDictionary)decodeParams : null;
            int k = 0;
            bool blackIs1 = false;
            bool byteAlign = false;
            if (param != null) {
                PdfNumber kn = param.GetAsNumber(PdfName.K);
                if (kn != null) {
                    k = kn.IntValue();
                }
                PdfBoolean bo = param.GetAsBoolean(PdfName.BlackIs1);
                if (bo != null) {
                    blackIs1 = bo.GetValue();
                }
                bo = param.GetAsBoolean(PdfName.EncodedByteAlign);
                if (bo != null) {
                    byteAlign = bo.GetValue();
                }
            }
            byte[] outBuf = new byte[(width + 7) / 8 * height];
            TIFFFaxDecompressor decoder = new TIFFFaxDecompressor();
            if (k == 0 || k > 0) {
                int tiffT4Options = k > 0 ? TIFFConstants.GROUP3OPT_2DENCODING : 0;
                tiffT4Options |= byteAlign ? TIFFConstants.GROUP3OPT_FILLBITS : 0;
                decoder.SetOptions(1, TIFFConstants.COMPRESSION_CCITTFAX3, tiffT4Options, 0);
                decoder.DecodeRaw(outBuf, b, width, height);
                if (decoder.fails > 0) {
                    byte[] outBuf2 = new byte[(width + 7) / 8 * height];
                    int oldFails = decoder.fails;
                    decoder.SetOptions(1, TIFFConstants.COMPRESSION_CCITTRLE, tiffT4Options, 0);
                    decoder.DecodeRaw(outBuf2, b, width, height);
                    if (decoder.fails < oldFails) {
                        outBuf = outBuf2;
                    }
                }
            }
            else {
                long tiffT6Options = 0;
                tiffT6Options |= byteAlign ? TIFFConstants.GROUP4OPT_FILLBITS : 0;
                TIFFFaxDecoder deca = new TIFFFaxDecoder(1, width, height);
                deca.DecodeT6(outBuf, b, 0, height, tiffT6Options);
            }
            if (!blackIs1) {
                int len = outBuf.Length;
                for (int t = 0; t < len; ++t) {
                    outBuf[t] ^= 0xff;
                }
            }
            b = outBuf;
            return b;
        }
    }
}
