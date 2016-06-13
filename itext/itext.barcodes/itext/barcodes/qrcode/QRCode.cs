/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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

namespace iTextSharp.Barcodes.Qrcode {
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

        // Mode of the QR Code.
        public Mode GetMode() {
            return mode;
        }

        // Error correction level of the QR Code.
        public ErrorCorrectionLevel GetECLevel() {
            return ecLevel;
        }

        // Version of the QR Code.  The bigger size, the bigger version.
        public int GetVersion() {
            return version;
        }

        // ByteMatrix width of the QR Code.
        public int GetMatrixWidth() {
            return matrixWidth;
        }

        // Mask pattern of the QR Code.
        public int GetMaskPattern() {
            return maskPattern;
        }

        // Number of total bytes in the QR Code.
        public int GetNumTotalBytes() {
            return numTotalBytes;
        }

        // Number of data bytes in the QR Code.
        public int GetNumDataBytes() {
            return numDataBytes;
        }

        // Number of error correction bytes in the QR Code.
        public int GetNumECBytes() {
            return numECBytes;
        }

        // Number of Reedsolomon blocks in the QR Code.
        public int GetNumRSBlocks() {
            return numRSBlocks;
        }

        // ByteMatrix data of the QR Code.
        public ByteMatrix GetMatrix() {
            return matrix;
        }

        // Return the value of the module (cell) pointed by "x" and "y" in the matrix of the QR Code. They
        // call cells in the matrix "modules". 1 represents a black cell, and 0 represents a white cell.
        public int At(int x, int y) {
            // The value must be zero or one.
            int value = matrix.Get(x, y);
            if (!(value == 0 || value == 1)) {
                // this is really like an assert... not sure what better exception to use?
                throw new Exception("Bad value");
            }
            return value;
        }

        // Checks all the member variables are set properly. Returns true on success. Otherwise, returns
        // false.
        public bool IsValid() {
            return mode != null && ecLevel != null && version != -1 && matrixWidth != -1 && maskPattern != -1 && numTotalBytes
                 != -1 && numDataBytes != -1 && numECBytes != -1 && numRSBlocks != -1 && IsValidMaskPattern(maskPattern
                ) && numTotalBytes == numDataBytes + numECBytes && matrix != null && matrixWidth == matrix.GetWidth() 
                && matrix.GetWidth() == matrix.GetHeight();
        }

        // First check if all version are not uninitialized.
        // Then check them in other ways..
        // ByteMatrix stuff.
        // See 7.3.1 of JISX0510:2004 (p.5).
        // Must be square.
        // Return debug String.
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

        public void SetMode(Mode value) {
            mode = value;
        }

        public void SetECLevel(ErrorCorrectionLevel value) {
            ecLevel = value;
        }

        public void SetVersion(int value) {
            version = value;
        }

        public void SetMatrixWidth(int value) {
            matrixWidth = value;
        }

        public void SetMaskPattern(int value) {
            maskPattern = value;
        }

        public void SetNumTotalBytes(int value) {
            numTotalBytes = value;
        }

        public void SetNumDataBytes(int value) {
            numDataBytes = value;
        }

        public void SetNumECBytes(int value) {
            numECBytes = value;
        }

        public void SetNumRSBlocks(int value) {
            numRSBlocks = value;
        }

        // This takes ownership of the 2D array.
        public void SetMatrix(ByteMatrix value) {
            matrix = value;
        }

        // Check if "mask_pattern" is valid.
        public static bool IsValidMaskPattern(int maskPattern) {
            return maskPattern >= 0 && maskPattern < NUM_MASK_PATTERNS;
        }
    }
}
