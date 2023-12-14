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
namespace iText.Barcodes.Qrcode {
    /// <summary>Helper class that groups a block of databytes with its corresponding block of error correction block
    ///     </summary>
    internal sealed class BlockPair {
        private readonly ByteArray dataBytes;

        private readonly ByteArray errorCorrectionBytes;

        internal BlockPair(ByteArray data, ByteArray errorCorrection) {
            dataBytes = data;
            errorCorrectionBytes = errorCorrection;
        }

        /// <returns>data block of the pair</returns>
        public ByteArray GetDataBytes() {
            return dataBytes;
        }

        /// <returns>error correction block of the pair</returns>
        public ByteArray GetErrorCorrectionBytes() {
            return errorCorrectionBytes;
        }
    }
}
