/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Text;

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// A QR code (short for "quick-response code") is a type of two-dimensional matrix barcode, invented in 1994, by
    /// Japanese company Denso Wave for labelling automobile parts.[1][2] A barcode is a machine-readable optical image that
    /// contains information specific to the labelled item.
    /// </summary>
    /// <remarks>
    /// A QR code (short for "quick-response code") is a type of two-dimensional matrix barcode, invented in 1994, by
    /// Japanese company Denso Wave for labelling automobile parts.[1][2] A barcode is a machine-readable optical image that
    /// contains information specific to the labelled item. In practice, QR codes contain data for a locator, an identifier,
    /// and web tracking. To efficiently store data, QR codes use four standardized modes of encoding (i) numeric, (ii)
    /// alphanumeric, (iii) byte or binary, and (iv) kanji.[3]
    /// </remarks>
    internal sealed class QRCode {
//\cond DO_NOT_DOCUMENT
        internal const int NUM_MASK_PATTERNS = 8;
//\endcond

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

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a QR-code object with unitialized parameters</summary>
        internal QRCode() {
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Mode used by the QR code to encode data into bits.</summary>
        /// <remarks>
        /// Mode used by the QR code to encode data into bits.
        /// Possible values: TERMINATOR, NUMERIC, ALPHANUMERIC, STRUCTURED_APPEND, BYTE, ECI, KANJI, FNC1_FIRST_POSITION, FNC2_SECOND_POSITION
        /// </remarks>
        /// <returns>Mode of the QR Code.</returns>
        internal Mode GetMode() {
            return mode;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Possible error correction level values ranked from lowest error correction capability to highest: L, M, Q, H
        ///     </summary>
        /// <returns>Error correction level of the QR Code.</returns>
        internal ErrorCorrectionLevel GetECLevel() {
            return ecLevel;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Together with error correction level, the version determines the information capacity of the QR code.
        ///     </summary>
        /// <remarks>Together with error correction level, the version determines the information capacity of the QR code. Higher version numbers correspond with higher capacity. Ranges from 1 to 40.
        ///     </remarks>
        /// <returns>Version of the QR Code.</returns>
        internal int GetVersion() {
            return version;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>ByteMatrix width of the QR Code.</returns>
        internal int GetMatrixWidth() {
            return matrixWidth;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>Mask pattern of the QR Code.</returns>
        internal int GetMaskPattern() {
            return maskPattern;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>Number of total bytes in the QR Code.</returns>
        internal int GetNumTotalBytes() {
            return numTotalBytes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>Number of data bytes in the QR Code.</returns>
        internal int GetNumDataBytes() {
            return numDataBytes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>Number of error correction bytes in the QR Code.</returns>
        internal int GetNumECBytes() {
            return numECBytes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>Number of Reedsolomon blocks in the QR Code.</returns>
        internal int GetNumRSBlocks() {
            return numRSBlocks;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>ByteMatrix data of the QR Code.</returns>
        internal ByteMatrix GetMatrix() {
            return matrix;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Retrieve the value of the module (cell) pointed by "x" and "y" in the matrix of the QR Code.</summary>
        /// <remarks>
        /// Retrieve the value of the module (cell) pointed by "x" and "y" in the matrix of the QR Code.
        /// 1 represents a black cell, and 0 represents a white cell.
        /// </remarks>
        /// <param name="x">width coordinate</param>
        /// <param name="y">height coordinate</param>
        /// <returns>1 for a black cell, 0 for a white cell</returns>
        internal int At(int x, int y) {
            // The value must be zero or one.
            int value = matrix.Get(x, y);
            if (!(value == 0 || value == 1)) {
                // this is really like an assert... not sure what better exception to use?
                throw new Exception("Bad value");
            }
            return value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Check the validity of all member variables</summary>
        /// <returns>true if all variables are valid, false otherwise</returns>
        internal bool IsValid() {
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
//\endcond

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

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Set the data encoding mode of the QR code
        /// Possible modes: TERMINATOR, NUMERIC, ALPHANUMERIC, STRUCTURED_APPEND, BYTE, ECI, KANJI, FNC1_FIRST_POSITION, FNC2_SECOND_POSITION
        /// </summary>
        /// <param name="value">new data encoding mode</param>
        internal void SetMode(Mode value) {
            mode = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the error correction level of th QR code.</summary>
        /// <remarks>
        /// Set the error correction level of th QR code.
        /// Possible error correction level values ranked from lowest error correction capability to highest: L, M, Q, H
        /// </remarks>
        /// <param name="value">new error correction level</param>
        internal void SetECLevel(ErrorCorrectionLevel value) {
            ecLevel = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the version of the QR code.</summary>
        /// <remarks>
        /// Set the version of the QR code.
        /// Together with error correction level, the version determines the information capacity of the QR code. Higher version numbers correspond with higher capacity.
        /// Range: 1 to 40.
        /// </remarks>
        /// <param name="value">the new version of the QR code</param>
        internal void SetVersion(int value) {
            version = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets the width of the byte matrix</summary>
        /// <param name="value">the new width of the matrix</param>
        internal void SetMatrixWidth(int value) {
            matrixWidth = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the masking pattern</summary>
        /// <param name="value">new masking pattern of the QR code</param>
        internal void SetMaskPattern(int value) {
            maskPattern = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the number of total bytes</summary>
        /// <param name="value">new number of total bytes</param>
        internal void SetNumTotalBytes(int value) {
            numTotalBytes = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the number of data bytes</summary>
        /// <param name="value">new number of data bytes</param>
        internal void SetNumDataBytes(int value) {
            numDataBytes = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the number of error correction blocks</summary>
        /// <param name="value">new number of error correction blocks</param>
        internal void SetNumECBytes(int value) {
            numECBytes = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the number of Reed-Solomon blocks</summary>
        /// <param name="value">new number of Reed-Solomon blocks</param>
        internal void SetNumRSBlocks(int value) {
            numRSBlocks = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the byte-matrix</summary>
        /// <param name="value">the new byte-matrix</param>
        internal void SetMatrix(ByteMatrix value) {
            matrix = value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Check if "mask_pattern" is valid.</summary>
        /// <param name="maskPattern">masking pattern to check</param>
        /// <returns>true if the pattern is valid, false otherwise</returns>
        internal static bool IsValidMaskPattern(int maskPattern) {
            return maskPattern >= 0 && maskPattern < NUM_MASK_PATTERNS;
        }
//\endcond
    }
//\endcond
}
