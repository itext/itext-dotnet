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
        /// <summary><inheritDoc/></summary>
        public virtual PdfName GetFilterName() {
            return PdfName.ASCII85Decode;
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfObject GetDecodeParams() {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Stream CreateNewOutputStream(Stream original, PdfStream stream) {
            return new ASCII85OutputStream(original);
        }
    }
}
