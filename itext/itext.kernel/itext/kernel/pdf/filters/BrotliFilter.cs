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
using System.IO;
using iText.IO.Codec.Brotli.Dec;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Filter implementation for decoding Brotli-compressed PDF streams.</summary>
    /// <remarks>
    /// Filter implementation for decoding Brotli-compressed PDF streams.
    /// This filter supports optional Brotli dictionary streams and memory limits awareness.
    /// </remarks>
    public class BrotliFilter : MemoryLimitsAwareFilter {
        private const int DEFAULT_INTERNAL_BUFFER_SIZE = 16384;

        /// <summary>Default buffer size for Brotli decompression (64 KiB).</summary>
        private const int DEFAULT_BUFFER_SIZE = 65536;

        /// <summary>Constructs an empty BrotliFilter instance.</summary>
        public BrotliFilter() {
        }

        //empty constructor
        /// <summary>Decodes Brotli-compressed data from a PDF stream.</summary>
        /// <param name="b">the bytes that need to be decoded</param>
        /// <param name="filterName">PdfName of the filter (unused)</param>
        /// <param name="decodeParams">decode parameters, may contain a Brotli dictionary stream under key 'D'</param>
        /// <param name="streamDictionary">
        /// the dictionary of the stream. Can contain additional information needed to decode the
        /// byte[]
        /// </param>
        /// <returns>the decoded byte[]</returns>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            try {
                PdfStream brotliDictionary = GetBrotliDictionaryStream(decodeParams);
                byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
                MemoryStream input = new MemoryStream(b);
                MemoryStream output = EnableMemoryLimitsAwareHandler(streamDictionary);
                BrotliInputStream brotliInput;
                if (brotliDictionary != null) {
                    brotliInput = new BrotliInputStream(input, DEFAULT_INTERNAL_BUFFER_SIZE);
                    brotliInput.AttachDictionaryChunk(brotliDictionary.GetBytes());
                }
                else {
                    brotliInput = new BrotliInputStream(input);
                }
                int len;
                while ((len = brotliInput.JRead(buffer, 0, buffer.Length)) > 0) {
                    output.Write(buffer, 0, len);
                }
                brotliInput.Close();
                return FlateDecodeFilter.DecodePredictor(output.ToArray(), decodeParams);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.FAILED_TO_DECODE_BROTLI_STREAM, e);
            }
        }

        /// <summary>Extracts the Brotli dictionary stream from decode parameters if present.</summary>
        /// <param name="decodeParams">decode parameters, may contain a Brotli dictionary stream under key 'D'</param>
        /// <returns>an Optional containing the Brotli dictionary stream if present, otherwise empty</returns>
        private static PdfStream GetBrotliDictionaryStream(PdfObject decodeParams) {
            if (!(decodeParams is PdfDictionary)) {
                return null;
            }
            PdfDictionary dict = (PdfDictionary)decodeParams;
            PdfObject brotliDecompressionDictionary = dict.Get(PdfName.D);
            if (brotliDecompressionDictionary is PdfStream) {
                // Brotli dictionary stream found
                return (PdfStream)brotliDecompressionDictionary;
            }
            else {
                if (brotliDecompressionDictionary != null) {
                    throw new PdfException(KernelExceptionMessageConstant.BROTLI_DICTIONARY_IS_NOT_A_STREAM);
                }
                else {
                    return null;
                }
            }
        }
    }
}
