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
    /// <summary>These are a set of hints that you may pass to Writers to specify their behavior.</summary>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    public sealed class EncodeHintType {
        /// <summary>Specifies what degree of error correction to use, for example in QR Codes (type Integer).</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType ERROR_CORRECTION = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        /// <summary>Specifies what character encoding to use where applicable (type String)</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType CHARACTER_SET = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        /// <summary>Specifies the minimal version level to use, for example in QR Codes (type Integer).</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType MIN_VERSION_NR = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        private EncodeHintType() {
        }
    }
}
