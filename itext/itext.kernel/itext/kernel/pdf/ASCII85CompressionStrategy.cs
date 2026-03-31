/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// A compression strategy that uses the
    /// <c>ASCII85Decode</c>
    /// filter for PDF
    /// streams.
    /// </summary>
    /// <remarks>
    /// A compression strategy that uses the
    /// <c>ASCII85Decode</c>
    /// filter for PDF
    /// streams.
    /// <para />
    /// This strategy implements the
    /// <see cref="IStreamCompressionStrategy"/>
    /// interface
    /// and provides
    /// <c>ASCII85Decode</c>
    /// encoding.
    /// <para />
    /// The strategy ensures, that streams are saved using just 7-bit ASCII
    /// characters, but it typically increases sizes of streams by 25% compared to
    /// just saving them as-is. So calling this a "compression strategy" is a
    /// misnomer.
    /// </remarks>
    public class ASCII85CompressionStrategy : IStreamCompressionStrategy {
        /// <summary>
        /// Constructs a new
        /// <see cref="ASCII85CompressionStrategy"/>
        /// instance.
        /// </summary>
        public ASCII85CompressionStrategy() {
        }

        // empty constructor
        /// <summary>Returns the name of the compression filter.</summary>
        /// <returns>
        /// 
        /// <see cref="PdfName.ASCII85Decode"/>
        /// representing the
        /// <c>ASCII85Decode</c>
        /// filter
        /// </returns>
        public virtual PdfName GetFilterName() {
            return PdfName.ASCII85Decode;
        }

        /// <summary>
        /// Returns the decode parameters for the
        /// <c>ASCII85Decode</c>
        /// filter.
        /// </summary>
        /// <remarks>
        /// Returns the decode parameters for the
        /// <c>ASCII85Decode</c>
        /// filter.
        /// <para />
        /// This implementation returns
        /// <see langword="null"/>
        /// as no special decode parameters
        /// are required for standard ASCII85 compression.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="null"/>
        /// as no decode parameters are needed
        /// </returns>
        public virtual PdfObject GetDecodeParams() {
            return null;
        }

        /// <summary>Creates a new output stream with ASCII85 compression applied.</summary>
        /// <remarks>
        /// Creates a new output stream with ASCII85 compression applied.
        /// <para />
        /// This method wraps the original output stream in a
        /// <see cref="iText.IO.Source.ASCII85OutputStream"/>
        /// that applies ASCII85 compression.
        /// </remarks>
        /// <param name="original">the original output stream to wrap</param>
        /// <param name="stream">the PDF stream containing compression configuration</param>
        /// <returns>
        /// a new
        /// <see cref="iText.IO.Source.ASCII85OutputStream"/>
        /// that compresses data using the ASCII85 algorithm
        /// </returns>
        public virtual Stream CreateNewOutputStream(Stream original, PdfStream stream) {
            return new ASCII85OutputStream(original);
        }
    }
}
