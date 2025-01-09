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

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Encapsulates a QR Code's format information, including the data mask used and
    /// error correction level.
    /// </summary>
    /// <seealso cref="ErrorCorrectionLevel"/>
    internal sealed class FormatInformation {
        private const int FORMAT_INFO_MASK_QR = 0x5412;

        /// <summary>See ISO 18004:2006, Annex C, Table C.1</summary>
        private static readonly int[][] FORMAT_INFO_DECODE_LOOKUP = new int[][] { new int[] { 0x5412, 0x00 }, new 
            int[] { 0x5125, 0x01 }, new int[] { 0x5E7C, 0x02 }, new int[] { 0x5B4B, 0x03 }, new int[] { 0x45F9, 0x04
             }, new int[] { 0x40CE, 0x05 }, new int[] { 0x4F97, 0x06 }, new int[] { 0x4AA0, 0x07 }, new int[] { 0x77C4
            , 0x08 }, new int[] { 0x72F3, 0x09 }, new int[] { 0x7DAA, 0x0A }, new int[] { 0x789D, 0x0B }, new int[
            ] { 0x662F, 0x0C }, new int[] { 0x6318, 0x0D }, new int[] { 0x6C41, 0x0E }, new int[] { 0x6976, 0x0F }
            , new int[] { 0x1689, 0x10 }, new int[] { 0x13BE, 0x11 }, new int[] { 0x1CE7, 0x12 }, new int[] { 0x19D0
            , 0x13 }, new int[] { 0x0762, 0x14 }, new int[] { 0x0255, 0x15 }, new int[] { 0x0D0C, 0x16 }, new int[
            ] { 0x083B, 0x17 }, new int[] { 0x355F, 0x18 }, new int[] { 0x3068, 0x19 }, new int[] { 0x3F31, 0x1A }
            , new int[] { 0x3A06, 0x1B }, new int[] { 0x24B4, 0x1C }, new int[] { 0x2183, 0x1D }, new int[] { 0x2EDA
            , 0x1E }, new int[] { 0x2BED, 0x1F } };

        /// <summary>Offset i holds the number of 1 bits in the binary representation of i</summary>
        private static readonly int[] BITS_SET_IN_HALF_BYTE = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3
            , 3, 4 };

        private readonly ErrorCorrectionLevel errorCorrectionLevel;

        private readonly byte dataMask;

        private FormatInformation(int formatInfo) {
            // Bits 3,4
            errorCorrectionLevel = ErrorCorrectionLevel.ForBits((formatInfo >> 3) & 0x03);
            // Bottom 3 bits
            dataMask = (byte)(formatInfo & 0x07);
        }

//\cond DO_NOT_DOCUMENT
        internal static int NumBitsDiffering(int a, int b) {
            // a now has a 1 bit exactly where its bit differs with b's
            a ^= b;
            // Count bits set quickly with a series of lookups:
            return BITS_SET_IN_HALF_BYTE[a & 0x0F] + BITS_SET_IN_HALF_BYTE[((int)(((uint)a) >> 4) & 0x0F)] + BITS_SET_IN_HALF_BYTE
                [((int)(((uint)a) >> 8) & 0x0F)] + BITS_SET_IN_HALF_BYTE[((int)(((uint)a) >> 12) & 0x0F)] + BITS_SET_IN_HALF_BYTE
                [((int)(((uint)a) >> 16) & 0x0F)] + BITS_SET_IN_HALF_BYTE[((int)(((uint)a) >> 20) & 0x0F)] + BITS_SET_IN_HALF_BYTE
                [((int)(((uint)a) >> 24) & 0x0F)] + BITS_SET_IN_HALF_BYTE[((int)(((uint)a) >> 28) & 0x0F)];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <param name="maskedFormatInfo1">format info indicator, with mask still applied</param>
        /// <param name="maskedFormatInfo2">
        /// second copy of same info; both are checked at the same time
        /// to establish best match
        /// </param>
        /// <returns>
        /// information about the format it specifies, or <c>null</c>
        /// if doesn't seem to match any known pattern
        /// </returns>
        internal static iText.Barcodes.Qrcode.FormatInformation DecodeFormatInformation(int maskedFormatInfo1, int
             maskedFormatInfo2) {
            iText.Barcodes.Qrcode.FormatInformation formatInfo = DoDecodeFormatInformation(maskedFormatInfo1, maskedFormatInfo2
                );
            if (formatInfo != null) {
                return formatInfo;
            }
            // Should return null, but, some QR codes apparently
            // do not mask this info. Try again by actually masking the pattern
            // first
            return DoDecodeFormatInformation(maskedFormatInfo1 ^ FORMAT_INFO_MASK_QR, maskedFormatInfo2 ^ FORMAT_INFO_MASK_QR
                );
        }
//\endcond

        private static iText.Barcodes.Qrcode.FormatInformation DoDecodeFormatInformation(int maskedFormatInfo1, int
             maskedFormatInfo2) {
            // Find the int in FORMAT_INFO_DECODE_LOOKUP with fewest bits differing
            int bestDifference = int.MaxValue;
            int bestFormatInfo = 0;
            for (int i = 0; i < FORMAT_INFO_DECODE_LOOKUP.Length; i++) {
                int[] decodeInfo = FORMAT_INFO_DECODE_LOOKUP[i];
                int targetInfo = decodeInfo[0];
                if (targetInfo == maskedFormatInfo1 || targetInfo == maskedFormatInfo2) {
                    // Found an exact match
                    return new iText.Barcodes.Qrcode.FormatInformation(decodeInfo[1]);
                }
                int bitsDifference = NumBitsDiffering(maskedFormatInfo1, targetInfo);
                if (bitsDifference < bestDifference) {
                    bestFormatInfo = decodeInfo[1];
                    bestDifference = bitsDifference;
                }
                if (maskedFormatInfo1 != maskedFormatInfo2) {
                    // also try the other option
                    bitsDifference = NumBitsDiffering(maskedFormatInfo2, targetInfo);
                    if (bitsDifference < bestDifference) {
                        bestFormatInfo = decodeInfo[1];
                        bestDifference = bitsDifference;
                    }
                }
            }
            // Hamming distance of the 32 masked codes is 7, by construction, so <= 3 bits
            // differing means we found a match
            if (bestDifference <= 3) {
                return new iText.Barcodes.Qrcode.FormatInformation(bestFormatInfo);
            }
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal ErrorCorrectionLevel GetErrorCorrectionLevel() {
            return errorCorrectionLevel;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>The datamask in byte-format</returns>
        internal byte GetDataMask() {
            return dataMask;
        }
//\endcond

        /// <returns>the hashcode of the QR-code format information</returns>
        public override int GetHashCode() {
            return (errorCorrectionLevel.Ordinal() << 3) | (int)dataMask;
        }

        /// <summary>Compares the Format Information of this and o</summary>
        /// <param name="o">object to compare to</param>
        /// <returns>True if o is a FormatInformationObject and the error-correction level and the datamask are equal, false otherwise
        ///     </returns>
        public override bool Equals(Object o) {
            if (!(o is iText.Barcodes.Qrcode.FormatInformation)) {
                return false;
            }
            iText.Barcodes.Qrcode.FormatInformation other = (iText.Barcodes.Qrcode.FormatInformation)o;
            return this.errorCorrectionLevel == other.errorCorrectionLevel && this.dataMask == other.dataMask;
        }
    }
//\endcond
}
