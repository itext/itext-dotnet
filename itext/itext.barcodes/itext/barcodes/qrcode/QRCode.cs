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
using System.Text;

namespace iText.Barcodes.Qrcode {
    /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
    /// <author>dswitkin@google.com (Daniel Switkin) - ported from C++</author>
    internal sealed class QRCode {
        public const int NUM_MASK_PATTERNS = 8;

        private Mode mode;

        private ErrorCorrectionLevel ecLevel;

        private int version;

        private int matrixWidth;

        private int maskPattern;

        private int numTotalBytes;

        private int numDataBytes;

        private int numECBytes;

        private int numRSBlocks;

        private ByteMatrix matrix;

        /// <summary>Create a QR-code object with unitialized parameters</summary>
        public QRCode() {
            mode = null;
            ecLevel = null;
            version = -1;
            matrixWidth = -1;
            maskPattern = -1;
            numTotalBytes = -1;
            numDataBytes = -1;
            numECBytes = -1;
            numRSBlocks = -1;
            matrix = null;
        }

        /// <summary>Mode used by the QR code to encode data into bits.</summary>
        /// <remarks>
        /// Mode used by the QR code to encode data into bits.
        /// Possible values: TERMINATOR, NUMERIC, ALPHANUMERIC, STRUCTURED_APPEND, BYTE, ECI, KANJI, FNC1_FIRST_POSITION, FNC2_SECOND_POSITION
        /// </remarks>
        /// <returns>Mode of the QR Code.</returns>
        public Mode GetMode() {
            return mode;
        }

        /// <summary>Possible error correction level values ranked from lowest error correction capability to highest: L, M, Q, H
        ///     </summary>
        /// <returns>Error correction level of the QR Code.</returns>
        public ErrorCorrectionLevel GetECLevel() {
            return ecLevel;
        }

        /// <summary>Together with error correction level, the version determines the information capacity of the QR code.
        ///     </summary>
        /// <remarks>Together with error correction level, the version determines the information capacity of the QR code. Higher version numbers correspond with higher capacity. Ranges from 1 to 40.
        ///     </remarks>
        /// <returns>Version of the QR Code.</returns>
        public int GetVersion() {
            return version;
        }

        /// <returns>ByteMatrix width of the QR Code.</returns>
        public int GetMatrixWidth() {
            return matrixWidth;
        }

        /// <returns>Mask pattern of the QR Code.</returns>
        public int GetMaskPattern() {
            return maskPattern;
        }

        /// <returns>Number of total bytes in the QR Code.</returns>
        public int GetNumTotalBytes() {
            return numTotalBytes;
        }

        /// <returns>Number of data bytes in the QR Code.</returns>
        public int GetNumDataBytes() {
            return numDataBytes;
        }

        /// <returns>Number of error correction bytes in the QR Code.</returns>
        public int GetNumECBytes() {
            return numECBytes;
        }

        /// <returns>Number of Reedsolomon blocks in the QR Code.</returns>
        public int GetNumRSBlocks() {
            return numRSBlocks;
        }

        /// <returns>ByteMatrix data of the QR Code.</returns>
        public ByteMatrix GetMatrix() {
            return matrix;
        }

        /// <summary>Retrieve the value of the module (cell) pointed by "x" and "y" in the matrix of the QR Code.</summary>
        /// <remarks>
        /// Retrieve the value of the module (cell) pointed by "x" and "y" in the matrix of the QR Code.
        /// 1 represents a black cell, and 0 represents a white cell.
        /// </remarks>
        /// <param name="x">width coordinate</param>
        /// <param name="y">height coordinate</param>
        /// <returns>1 for a black cell, 0 for a white cell</returns>
        public int At(int x, int y) {
            // The value must be zero or one.
            int value = matrix.Get(x, y);
            if (!(value == 0 || value == 1)) {
                // this is really like an assert... not sure what better exception to use?
                throw new Exception("Bad value");
            }
            return value;
        }

        /// <summary>Check the validity of all member variables</summary>
        /// <returns>true if all variables are valid, false otherwise</returns>
        public bool IsValid() {
            return 
                        // First check if all version are not uninitialized.
                        mode != null && ecLevel != null && version != -1 && matrixWidth != -1 && maskPattern != -1 && numTotalBytes
                 != -1 && numDataBytes != -1 && numECBytes != -1 && numRSBlocks != -1 && 
                        // Then check them in other ways..
                        IsValidMaskPattern(maskPattern) && numTotalBytes == numDataBytes + numECBytes && 
                        // ByteMatrix stuff.
                        matrix != null && matrixWidth == matrix.GetWidth() && 
                        // Must be square.
                        
                        // See 7.3.1 of JISX0510:2004 (p.5).
                        matrix.GetWidth() == matrix.GetHeight();
        }

        /// <summary>Prints all parameters</summary>
        /// <returns>string containing all parameters</returns>
        public override String ToString() {
            StringBuilder result = new StringBuilder(200);
            result.Append("<<\n");
            result.Append(" mode: ");
            result.Append(mode);
            result.Append("\n ecLevel: ");
            result.Append(ecLevel);
            result.Append("\n version: ");
            result.Append(version);
            result.Append("\n matrixWidth: ");
            result.Append(matrixWidth);
            result.Append("\n maskPattern: ");
            result.Append(maskPattern);
            result.Append("\n numTotalBytes: ");
            result.Append(numTotalBytes);
            result.Append("\n numDataBytes: ");
            result.Append(numDataBytes);
            result.Append("\n numECBytes: ");
            result.Append(numECBytes);
            result.Append("\n numRSBlocks: ");
            result.Append(numRSBlocks);
            if (matrix == null) {
                result.Append("\n matrix: null\n");
            }
            else {
                result.Append("\n matrix:\n");
                result.Append(matrix.ToString());
            }
            result.Append(">>\n");
            return result.ToString();
        }

        /// <summary>
        /// Set the data encoding mode of the QR code
        /// Possible modes: TERMINATOR, NUMERIC, ALPHANUMERIC, STRUCTURED_APPEND, BYTE, ECI, KANJI, FNC1_FIRST_POSITION, FNC2_SECOND_POSITION
        /// </summary>
        /// <param name="value">new data encoding mode</param>
        public void SetMode(Mode value) {
            mode = value;
        }

        /// <summary>Set the error correction level of th QR code.</summary>
        /// <remarks>
        /// Set the error correction level of th QR code.
        /// Possible error correction level values ranked from lowest error correction capability to highest: L, M, Q, H
        /// </remarks>
        /// <param name="value">new error correction level</param>
        public void SetECLevel(ErrorCorrectionLevel value) {
            ecLevel = value;
        }

        /// <summary>Set the version of the QR code.</summary>
        /// <remarks>
        /// Set the version of the QR code.
        /// Together with error correction level, the version determines the information capacity of the QR code. Higher version numbers correspond with higher capacity.
        /// Range: 1 to 40.
        /// </remarks>
        /// <param name="value">the new version of the QR code</param>
        public void SetVersion(int value) {
            version = value;
        }

        /// <summary>Sets the width of the byte matrix</summary>
        /// <param name="value">the new width of the matrix</param>
        public void SetMatrixWidth(int value) {
            matrixWidth = value;
        }

        /// <summary>Set the masking pattern</summary>
        /// <param name="value">new masking pattern of the QR code</param>
        public void SetMaskPattern(int value) {
            maskPattern = value;
        }

        /// <summary>Set the number of total bytes</summary>
        /// <param name="value">new number of total bytes</param>
        public void SetNumTotalBytes(int value) {
            numTotalBytes = value;
        }

        /// <summary>Set the number of data bytes</summary>
        /// <param name="value">new number of data bytes</param>
        public void SetNumDataBytes(int value) {
            numDataBytes = value;
        }

        /// <summary>Set the number of error correction blocks</summary>
        /// <param name="value">new number of error correction blocks</param>
        public void SetNumECBytes(int value) {
            numECBytes = value;
        }

        /// <summary>Set the number of Reed-Solomon blocks</summary>
        /// <param name="value">new number of Reed-Solomon blocks</param>
        public void SetNumRSBlocks(int value) {
            numRSBlocks = value;
        }

        /// <summary>Set the byte-matrix</summary>
        /// <param name="value">the new byte-matrix</param>
        public void SetMatrix(ByteMatrix value) {
            matrix = value;
        }

        /// <summary>Check if "mask_pattern" is valid.</summary>
        /// <param name="maskPattern">masking pattern to check</param>
        /// <returns>true if the pattern is valid, false otherwise</returns>
        public static bool IsValidMaskPattern(int maskPattern) {
            return maskPattern >= 0 && maskPattern < NUM_MASK_PATTERNS;
        }
    }
}
