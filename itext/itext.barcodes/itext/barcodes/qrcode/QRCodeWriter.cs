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
using System.Collections.Generic;

namespace iText.Barcodes.Qrcode {
    /// <summary>This object renders a QR Code as a ByteMatrix 2D array of greyscale values.</summary>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    public sealed class QRCodeWriter {
        private const int QUIET_ZONE_SIZE = 4;

        /// <summary>Encode a string into a QR code with dimensions width x height, using error-correction level L and the smallest version for which he contents fit into the QR-code?
        ///     </summary>
        /// <param name="contents">String to encode into the QR code</param>
        /// <param name="width">width of the QR-code</param>
        /// <param name="height">height of the QR-code</param>
        /// <returns>2D Greyscale map containing the visual representation of the QR-code, stored as a Bytematrix</returns>
        public ByteMatrix Encode(String contents, int width, int height) {
            return Encode(contents, width, height, null);
        }

        /// <summary>Encode a string into a QR code with dimensions width x height.</summary>
        /// <remarks>
        /// Encode a string into a QR code with dimensions width x height. Hints contains suggestions for error-correction level and version.
        /// The default error-correction level is L, the default version is the smallest version for which the contents will fit into the QR-code.
        /// </remarks>
        /// <param name="contents">String to encode into the QR code</param>
        /// <param name="width">width of the QR-code</param>
        /// <param name="height">height of the QR-code</param>
        /// <param name="hints">Map containing suggestions for error-correction level and version</param>
        /// <returns>2D Greyscale map containing the visual representation of the QR-code, stored as a Bytematrix</returns>
        public ByteMatrix Encode(String contents, int width, int height, IDictionary<EncodeHintType, Object> hints
            ) {
            if (contents == null || contents.Length == 0) {
                throw new ArgumentException("Found empty contents");
            }
            if (width < 0 || height < 0) {
                throw new ArgumentException("Requested dimensions are too small: " + width + 'x' + height);
            }
            ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.L;
            if (hints != null) {
                ErrorCorrectionLevel requestedECLevel = (ErrorCorrectionLevel)hints.Get(EncodeHintType.ERROR_CORRECTION);
                if (requestedECLevel != null) {
                    errorCorrectionLevel = requestedECLevel;
                }
            }
            QRCode code = new QRCode();
            Encoder.Encode(contents, errorCorrectionLevel, hints, code);
            return RenderResult(code, width, height);
        }

        // Note that the input matrix uses 0 == white, 1 == black, while the output matrix uses
        // 0 == black, 255 == white (i.e. an 8 bit greyscale bitmap).
        private static ByteMatrix RenderResult(QRCode code, int width, int height) {
            ByteMatrix input = code.GetMatrix();
            int inputWidth = input.GetWidth();
            int inputHeight = input.GetHeight();
            int qrWidth = inputWidth + (QUIET_ZONE_SIZE << 1);
            int qrHeight = inputHeight + (QUIET_ZONE_SIZE << 1);
            int outputWidth = Math.Max(width, qrWidth);
            int outputHeight = Math.Max(height, qrHeight);
            int multiple = Math.Min(outputWidth / qrWidth, outputHeight / qrHeight);
            // Padding includes both the quiet zone and the extra white pixels to accommodate the requested
            // dimensions. For example, if input is 25x25 the QR will be 33x33 including the quiet zone.
            // If the requested size is 200x160, the multiple will be 4, for a QR of 132x132. These will
            // handle all the padding from 100x100 (the actual QR) up to 200x160.
            int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;
            int topPadding = (outputHeight - (inputHeight * multiple)) / 2;
            ByteMatrix output = new ByteMatrix(outputWidth, outputHeight);
            byte[][] outputArray = output.GetArray();
            // We could be tricky and use the first row in each set of multiple as the temporary storage,
            // instead of allocating this separate array.
            byte[] row = new byte[outputWidth];
            // 1. Write the white lines at the top
            for (int y = 0; y < topPadding; y++) {
                SetRowColor(outputArray[y], (byte)255);
            }
            // 2. Expand the QR image to the multiple
            byte[][] inputArray = input.GetArray();
            for (int y = 0; y < inputHeight; y++) {
                // a. Write the white pixels at the left of each row
                for (int x = 0; x < leftPadding; x++) {
                    row[x] = (byte)255;
                }
                // b. Write the contents of this row of the barcode
                int offset = leftPadding;
                for (int x = 0; x < inputWidth; x++) {
                    byte value = (inputArray[y][x] == 1) ? (byte)0 : (byte)255;
                    for (int z = 0; z < multiple; z++) {
                        row[offset + z] = value;
                    }
                    offset += multiple;
                }
                // c. Write the white pixels at the right of each row
                offset = leftPadding + (inputWidth * multiple);
                for (int x = offset; x < outputWidth; x++) {
                    row[x] = (byte)255;
                }
                // d. Write the completed row multiple times
                offset = topPadding + (y * multiple);
                for (int z = 0; z < multiple; z++) {
                    Array.Copy(row, 0, outputArray[offset + z], 0, outputWidth);
                }
            }
            // 3. Write the white lines at the bottom
            int offset_1 = topPadding + (inputHeight * multiple);
            for (int y = offset_1; y < outputHeight; y++) {
                SetRowColor(outputArray[y], (byte)255);
            }
            return output;
        }

        private static void SetRowColor(byte[] row, byte value) {
            for (int x = 0; x < row.Length; x++) {
                row[x] = value;
            }
        }
    }
}
