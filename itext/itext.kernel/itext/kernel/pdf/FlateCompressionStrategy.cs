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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    /// <summary>A compression strategy that uses the Flate (DEFLATE) compression algorithm for PDF streams.</summary>
    /// <remarks>
    /// A compression strategy that uses the Flate (DEFLATE) compression algorithm for PDF streams.
    /// <para />
    /// This strategy implements the
    /// <see cref="IStreamCompressionStrategy"/>
    /// interface and provides
    /// Flate compression.
    /// </remarks>
    public class FlateCompressionStrategy : IStreamCompressionStrategy {
        // 32KB buffer size
        private const int BUFFER = 32 * 1024;

        /// <summary>
        /// Constructs a new
        /// <see cref="FlateCompressionStrategy"/>
        /// instance.
        /// </summary>
        public FlateCompressionStrategy() {
        }

        // empty constructor
        /// <summary>Returns the name of the compression filter.</summary>
        /// <returns>
        /// 
        /// <see cref="PdfName.FlateDecode"/>
        /// representing the Flate compression filter
        /// </returns>
        public virtual PdfName GetFilterName() {
            return PdfName.FlateDecode;
        }

        /// <summary>Returns the decode parameters for the Flate filter.</summary>
        /// <remarks>
        /// Returns the decode parameters for the Flate filter.
        /// <para />
        /// This implementation returns
        /// <see langword="null"/>
        /// as no special decode parameters
        /// are required for standard Flate compression.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="null"/>
        /// as no decode parameters are needed
        /// </returns>
        public virtual PdfObject GetDecodeParams() {
            return null;
        }

        /// <summary>Creates a new output stream with Flate compression applied.</summary>
        /// <remarks>
        /// Creates a new output stream with Flate compression applied.
        /// <para />
        /// This method wraps the original output stream in a
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>
        /// that applies Flate compression using the compression level specified in the
        /// PDF stream and a 32KB buffer for optimal performance.
        /// </remarks>
        /// <param name="original">the original output stream to wrap</param>
        /// <param name="stream">the PDF stream containing compression configuration</param>
        /// <returns>
        /// a new
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>
        /// that compresses data using the Flate algorithm
        /// </returns>
        public virtual Stream CreateNewOutputStream(Stream original, PdfStream stream) {
            // Use 32KB buffer size for deflater stream
            return new DeflaterOutputStream(original, stream.GetCompressionLevel(), BUFFER);
        }

        /// <summary>Finishes writing compressed data to the output stream and flushes any remaining data.</summary>
        /// <remarks>
        /// Finishes writing compressed data to the output stream and flushes any remaining data.
        /// <para />
        /// This method must be called after all data has been written to ensure that all compressed
        /// data is properly flushed to the underlying stream. The output stream must be an instance
        /// of
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>
        /// , otherwise a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// will be thrown.
        /// </remarks>
        /// <param name="outputStream">
        /// the output stream to finish, must be a
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>
        /// </param>
        public virtual void Finish(Stream outputStream) {
            if (outputStream is DeflaterOutputStream) {
                try {
                    ((DeflaterOutputStream)outputStream).Finish();
                }
                catch (Exception e) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_WRITE_TO_PDF_STREAM, e);
                }
            }
            else {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.OUTPUTSTREAM_IS_NOT_OF_INSTANCE
                    , "DeflaterOutputStream"));
            }
        }
    }
}
