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

namespace iText.Kernel.Pdf {
    /// <summary>Strategy interface for PDF stream compression implementations.</summary>
    /// <remarks>
    /// Strategy interface for PDF stream compression implementations.
    /// <para />
    /// This interface defines the contract for compression strategies that can be applied to PDF streams.
    /// Different compression algorithms can be
    /// implemented by providing concrete implementations of this interface.
    /// </remarks>
    public interface IStreamCompressionStrategy {
        /// <summary>Gets the PDF filter name that identifies this compression algorithm.</summary>
        /// <returns>the PDF name representing the compression filter</returns>
        PdfName GetFilterName();

        /// <summary>Gets the decode parameters required for decompressing the stream.</summary>
        /// <remarks>
        /// Gets the decode parameters required for decompressing the stream.
        /// <para />
        /// Decode parameters provide additional information needed to correctly
        /// decompress the stream data. This may include predictor settings,
        /// color information, or other algorithm-specific parameters.
        /// The returned object is typically a
        /// <see cref="PdfDictionary"/>
        /// or
        /// <see cref="PdfArray"/>
        /// ,
        /// or
        /// <see langword="null"/>
        /// if no special parameters are required.
        /// </remarks>
        /// <returns>
        /// the decode parameters as a PDF object, or
        /// <see langword="null"/>
        /// if not needed
        /// </returns>
        PdfObject GetDecodeParams();

        /// <summary>Creates a new output stream that wraps the original stream and applies compression.</summary>
        /// <remarks>
        /// Creates a new output stream that wraps the original stream and applies compression.
        /// <para />
        /// This method wraps the provided output stream with a compression implementation.
        /// Data written to the returned stream will be compressed before being written
        /// to the original stream.
        /// <para />
        /// If the stream requires finalization (e.g., to flush buffers or write end markers),
        /// the returned output stream should also implement the
        /// <see cref="iText.IO.Source.IFinishable"/>
        /// interface,
        /// </remarks>
        /// <param name="original">the original output stream to wrap</param>
        /// <param name="stream">the PDF stream being compressed (may be used for context or configuration)</param>
        /// <returns>a new output stream that performs compression</returns>
        Stream CreateNewOutputStream(Stream original, PdfStream stream);
    }
}
