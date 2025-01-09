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
using System.Collections.Generic;
using iText.Barcodes.Exceptions;

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    internal sealed class Encoder {
        // The original table is defined in the table 5 of JISX0510:2004 (p.19).
        private static readonly int[] ALPHANUMERIC_TABLE = new int[] { 
                // 0x00-0x0f
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                // 0x10-0x1f
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
                // 0x20-0x2f
                36, -1, -1, -1, 37, 38, -1, -1, -1, -1, 39, 40, -1, 41, 42, 43, 
                // 0x30-0x3f
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 44, -1, -1, -1, -1, -1, 
                // 0x40-0x4f
                -1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 
                // 0x50-0x5f
                25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1 };

//\cond DO_NOT_DOCUMENT
        internal const String DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";
//\endcond

        private Encoder() {
        }

        // The mask penalty calculation is complicated.  See Table 21 of JISX0510:2004 (p.45) for details.
        // Basically it applies four rules and summate all penalties.
        private static int CalculateMaskPenalty(ByteMatrix matrix) {
            int penalty = 0;
            penalty += MaskUtil.ApplyMaskPenaltyRule1(matrix);
            penalty += MaskUtil.ApplyMaskPenaltyRule2(matrix);
            penalty += MaskUtil.ApplyMaskPenaltyRule3(matrix);
            penalty += MaskUtil.ApplyMaskPenaltyRule4(matrix);
            return penalty;
        }

        /// <summary>Encode "bytes" with the error correction level "ecLevel".</summary>
        /// <remarks>
        /// Encode "bytes" with the error correction level "ecLevel". The encoding mode will be chosen
        /// internally by chooseMode(). On success, store the result in "qrCode".
        /// <para />
        /// We recommend you to use QRCode.EC_LEVEL_L (the lowest level) for
        /// "getECLevel" since our primary use is to show QR code on desktop screens. We don't need very
        /// strong error correction for this purpose.
        /// <para />
        /// Note that there is no way to encode bytes in MODE_KANJI. We might want to add EncodeWithMode()
        /// with which clients can specify the encoding mode. For now, we don't need the functionality.
        /// </remarks>
        /// <param name="content">String to encode</param>
        /// <param name="ecLevel">Error-correction level to use</param>
        /// <param name="qrCode">QR code to store the result in</param>
        public static void Encode(String content, ErrorCorrectionLevel ecLevel, QRCode qrCode) {
            Encode(content, ecLevel, null, qrCode);
        }

        /// <summary>Encode "bytes" with the error correction level "ecLevel".</summary>
        /// <remarks>
        /// Encode "bytes" with the error correction level "ecLevel". The encoding mode will be chosen
        /// internally by chooseMode(). On success, store the result in "qrCode".
        /// <para />
        /// We recommend you to use QRCode.EC_LEVEL_L (the lowest level) for
        /// "getECLevel" since our primary use is to show QR code on desktop screens. We don't need very
        /// strong error correction for this purpose.
        /// <para />
        /// Note that there is no way to encode bytes in MODE_KANJI. We might want to add EncodeWithMode()
        /// with which clients can specify the encoding mode. For now, we don't need the functionality.
        /// </remarks>
        /// <param name="content">String to encode</param>
        /// <param name="ecLevel">Error-correction level to use</param>
        /// <param name="hints">Optional Map containing  encoding and suggested minimum version to use</param>
        /// <param name="qrCode">QR code to store the result in</param>
        public static void Encode(String content, ErrorCorrectionLevel ecLevel, IDictionary<EncodeHintType, Object
            > hints, QRCode qrCode) {
            String encoding = hints == null ? null : (String)hints.Get(EncodeHintType.CHARACTER_SET);
            if (encoding == null) {
                encoding = DEFAULT_BYTE_MODE_ENCODING;
            }
            int desiredMinVersion = (hints == null || hints.Get(EncodeHintType.MIN_VERSION_NR) == null) ? 1 : (int)hints
                .Get(EncodeHintType.MIN_VERSION_NR);
            //Check if desired level is within bounds of [1,40]
            if (desiredMinVersion < 1) {
                desiredMinVersion = 1;
            }
            if (desiredMinVersion > 40) {
                desiredMinVersion = 40;
            }
            // Step 1: Choose the mode (encoding).
            Mode mode = ChooseMode(content, encoding);
            // Step 2: Append "bytes" into "dataBits" in appropriate encoding.
            BitVector dataBits = new BitVector();
            AppendBytes(content, mode, dataBits, encoding);
            // Step 3: Initialize QR code that can contain "dataBits".
            int numInputBytes = dataBits.SizeInBytes();
            InitQRCode(numInputBytes, ecLevel, desiredMinVersion, mode, qrCode);
            // Step 4: Build another bit vector that contains header and data.
            BitVector headerAndDataBits = new BitVector();
            // Step 4.5: Append ECI message if applicable
            if (mode == Mode.BYTE && !DEFAULT_BYTE_MODE_ENCODING.Equals(encoding)) {
                CharacterSetECI eci = CharacterSetECI.GetCharacterSetECIByName(encoding);
                if (eci != null) {
                    AppendECI(eci, headerAndDataBits);
                }
            }
            AppendModeInfo(mode, headerAndDataBits);
            int numLetters = mode.Equals(Mode.BYTE) ? dataBits.SizeInBytes() : content.Length;
            AppendLengthInfo(numLetters, qrCode.GetVersion(), mode, headerAndDataBits);
            headerAndDataBits.AppendBitVector(dataBits);
            // Step 5: Terminate the bits properly.
            TerminateBits(qrCode.GetNumDataBytes(), headerAndDataBits);
            // Step 6: Interleave data bits with error correction code.
            BitVector finalBits = new BitVector();
            InterleaveWithECBytes(headerAndDataBits, qrCode.GetNumTotalBytes(), qrCode.GetNumDataBytes(), qrCode.GetNumRSBlocks
                (), finalBits);
            // Step 7: Choose the mask pattern and set to "qrCode".
            ByteMatrix matrix = new ByteMatrix(qrCode.GetMatrixWidth(), qrCode.GetMatrixWidth());
            qrCode.SetMaskPattern(ChooseMaskPattern(finalBits, qrCode.GetECLevel(), qrCode.GetVersion(), matrix));
            // Step 8.  Build the matrix and set it to "qrCode".
            MatrixUtil.BuildMatrix(finalBits, qrCode.GetECLevel(), qrCode.GetVersion(), qrCode.GetMaskPattern(), matrix
                );
            qrCode.SetMatrix(matrix);
            // Step 9.  Make sure we have a valid QR Code.
            if (!qrCode.IsValid()) {
                throw new WriterException("Invalid QR code: " + qrCode.ToString());
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <returns>
        /// the code point of the table used in alphanumeric mode or
        /// -1 if there is no corresponding code in the table.
        /// </returns>
        internal static int GetAlphanumericCode(int code) {
            if (code < ALPHANUMERIC_TABLE.Length) {
                return ALPHANUMERIC_TABLE[code];
            }
            return -1;
        }
//\endcond

        /// <summary>Choose the best mode by examining the content.</summary>
        /// <param name="content">content to examine</param>
        /// <returns>mode to use</returns>
        public static Mode ChooseMode(String content) {
            return ChooseMode(content, null);
        }

        /// <summary>Choose the best mode by examining the content.</summary>
        /// <remarks>
        /// Choose the best mode by examining the content. Note that 'encoding' is used as a hint;
        /// if it is Shift_JIS, and the input is only double-byte Kanji, then we return
        /// <see cref="Mode.KANJI"/>
        /// </remarks>
        /// <param name="content">content to examine</param>
        /// <param name="encoding">hint for the encoding to use</param>
        /// <returns>mode to use</returns>
        public static Mode ChooseMode(String content, String encoding) {
            if ("Shift_JIS".Equals(encoding)) {
                // Choose Kanji mode if all input are double-byte characters
                return IsOnlyDoubleByteKanji(content) ? Mode.KANJI : Mode.BYTE;
            }
            bool hasNumeric = false;
            bool hasAlphanumeric = false;
            for (int i = 0; i < content.Length; ++i) {
                char c = content[i];
                if (c >= '0' && c <= '9') {
                    hasNumeric = true;
                }
                else {
                    if (GetAlphanumericCode(c) != -1) {
                        hasAlphanumeric = true;
                    }
                    else {
                        return Mode.BYTE;
                    }
                }
            }
            if (hasAlphanumeric) {
                return Mode.ALPHANUMERIC;
            }
            else {
                if (hasNumeric) {
                    return Mode.NUMERIC;
                }
            }
            return Mode.BYTE;
        }

        private static bool IsOnlyDoubleByteKanji(String content) {
            byte[] bytes;
            try {
                bytes = content.GetBytes("Shift_JIS");
            }
            catch (ArgumentException) {
                return false;
            }
            int length = bytes.Length;
            if (length % 2 != 0) {
                return false;
            }
            for (int i = 0; i < length; i += 2) {
                int byte1 = bytes[i] & 0xFF;
                if ((byte1 < 0x81 || byte1 > 0x9F) && (byte1 < 0xE0 || byte1 > 0xEB)) {
                    return false;
                }
            }
            return true;
        }

        private static int ChooseMaskPattern(BitVector bits, ErrorCorrectionLevel ecLevel, int version, ByteMatrix
             matrix) {
            // Lower penalty is better.
            int minPenalty = int.MaxValue;
            int bestMaskPattern = -1;
            // We try all mask patterns to choose the best one.
            for (int maskPattern = 0; maskPattern < QRCode.NUM_MASK_PATTERNS; maskPattern++) {
                MatrixUtil.BuildMatrix(bits, ecLevel, version, maskPattern, matrix);
                int penalty = CalculateMaskPenalty(matrix);
                if (penalty < minPenalty) {
                    minPenalty = penalty;
                    bestMaskPattern = maskPattern;
                }
            }
            return bestMaskPattern;
        }

        /// <summary>Initialize "qrCode" according to "numInputBytes", "ecLevel", and "mode".</summary>
        /// <remarks>
        /// Initialize "qrCode" according to "numInputBytes", "ecLevel", and "mode". On success,
        /// modify "qrCode".
        /// </remarks>
        private static void InitQRCode(int numInputBytes, ErrorCorrectionLevel ecLevel, int desiredMinVersion, Mode
             mode, QRCode qrCode) {
            qrCode.SetECLevel(ecLevel);
            qrCode.SetMode(mode);
            // In the following comments, we use numbers of Version 7-H.
            for (int versionNum = desiredMinVersion; versionNum <= 40; versionNum++) {
                Version version = Version.GetVersionForNumber(versionNum);
                // numBytes = 196
                int numBytes = version.GetTotalCodewords();
                // getNumECBytes = 130
                Version.ECBlocks ecBlocks = version.GetECBlocksForLevel(ecLevel);
                int numEcBytes = ecBlocks.GetTotalECCodewords();
                // getNumRSBlocks = 5
                int numRSBlocks = ecBlocks.GetNumBlocks();
                // getNumDataBytes = 196 - 130 = 66
                int numDataBytes = numBytes - numEcBytes;
                // We want to choose the smallest version which can contain data of "numInputBytes" + some
                // extra bits for the header (mode info and length info). The header can be three bytes
                // (precisely 4 + 16 bits) at most. Hence we do +3 here.
                if (numDataBytes >= numInputBytes + 3) {
                    // Yay, we found the proper rs block info!
                    qrCode.SetVersion(versionNum);
                    qrCode.SetNumTotalBytes(numBytes);
                    qrCode.SetNumDataBytes(numDataBytes);
                    qrCode.SetNumRSBlocks(numRSBlocks);
                    // getNumECBytes = 196 - 66 = 130
                    qrCode.SetNumECBytes(numEcBytes);
                    // matrix width = 21 + 6 * 4 = 45
                    qrCode.SetMatrixWidth(version.GetDimensionForVersion());
                    return;
                }
            }
            throw new WriterException("Cannot find proper rs block info (input data too big?)");
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Terminate bits as described in 8.4.8 and 8.4.9 of JISX0510:2004 (p.24).</summary>
        internal static void TerminateBits(int numDataBytes, BitVector bits) {
            int capacity = numDataBytes << 3;
            if (bits.Size() > capacity) {
                throw new WriterException("data bits cannot fit in the QR Code" + bits.Size() + " > " + capacity);
            }
            // Append termination bits. See 8.4.8 of JISX0510:2004 (p.24) for details.
            for (int i = 0; i < 4 && bits.Size() < capacity; ++i) {
                bits.AppendBit(0);
            }
            int numBitsInLastByte = bits.Size() % 8;
            // If the last byte isn't 8-bit aligned, we'll add padding bits.
            if (numBitsInLastByte > 0) {
                int numPaddingBits = 8 - numBitsInLastByte;
                for (int i = 0; i < numPaddingBits; ++i) {
                    bits.AppendBit(0);
                }
            }
            // Should be 8-bit aligned here.
            if (bits.Size() % 8 != 0) {
                throw new WriterException("Number of bits is not a multiple of 8");
            }
            // If we have more space, we'll fill the space with padding patterns defined in 8.4.9 (p.24).
            int numPaddingBytes = numDataBytes - bits.SizeInBytes();
            for (int i = 0; i < numPaddingBytes; ++i) {
                if (i % 2 == 0) {
                    bits.AppendBits(0xec, 8);
                }
                else {
                    bits.AppendBits(0x11, 8);
                }
            }
            if (bits.Size() != capacity) {
                throw new WriterException("Bits size does not equal capacity");
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get number of data bytes and number of error correction bytes for block id "blockID".</summary>
        /// <remarks>
        /// Get number of data bytes and number of error correction bytes for block id "blockID". Store
        /// the result in "numDataBytesInBlock", and "numECBytesInBlock". See table 12 in 8.5.1 of
        /// JISX0510:2004 (p.30)
        /// </remarks>
        internal static void GetNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks
            , int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock) {
            if (blockID >= numRSBlocks) {
                throw new WriterException("Block ID too large");
            }
            // numRsBlocksInGroup2 = 196 % 5 = 1
            int numRsBlocksInGroup2 = numTotalBytes % numRSBlocks;
            // numRsBlocksInGroup1 = 5 - 1 = 4
            int numRsBlocksInGroup1 = numRSBlocks - numRsBlocksInGroup2;
            // numTotalBytesInGroup1 = 196 / 5 = 39
            int numTotalBytesInGroup1 = numTotalBytes / numRSBlocks;
            // numTotalBytesInGroup2 = 39 + 1 = 40
            int numTotalBytesInGroup2 = numTotalBytesInGroup1 + 1;
            // numDataBytesInGroup1 = 66 / 5 = 13
            int numDataBytesInGroup1 = numDataBytes / numRSBlocks;
            // numDataBytesInGroup2 = 13 + 1 = 14
            int numDataBytesInGroup2 = numDataBytesInGroup1 + 1;
            // numEcBytesInGroup1 = 39 - 13 = 26
            int numEcBytesInGroup1 = numTotalBytesInGroup1 - numDataBytesInGroup1;
            // numEcBytesInGroup2 = 40 - 14 = 26
            int numEcBytesInGroup2 = numTotalBytesInGroup2 - numDataBytesInGroup2;
            // Sanity checks.
            // 26 = 26
            if (numEcBytesInGroup1 != numEcBytesInGroup2) {
                throw new WriterException("EC bytes mismatch");
            }
            // 5 = 4 + 1.
            if (numRSBlocks != numRsBlocksInGroup1 + numRsBlocksInGroup2) {
                throw new WriterException("RS blocks mismatch");
            }
            // 196 = (13 + 26) * 4 + (14 + 26) * 1
            if (numTotalBytes != ((numDataBytesInGroup1 + numEcBytesInGroup1) * numRsBlocksInGroup1) + ((numDataBytesInGroup2
                 + numEcBytesInGroup2) * numRsBlocksInGroup2)) {
                throw new WriterException("Total bytes mismatch");
            }
            if (blockID < numRsBlocksInGroup1) {
                numDataBytesInBlock[0] = numDataBytesInGroup1;
                numECBytesInBlock[0] = numEcBytesInGroup1;
            }
            else {
                numDataBytesInBlock[0] = numDataBytesInGroup2;
                numECBytesInBlock[0] = numEcBytesInGroup2;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Interleave "bits" with corresponding error correction bytes.</summary>
        /// <remarks>
        /// Interleave "bits" with corresponding error correction bytes. On success, store the result in
        /// "result". The interleave rule is complicated. See 8.6 of JISX0510:2004 (p.37) for details.
        /// </remarks>
        internal static void InterleaveWithECBytes(BitVector bits, int numTotalBytes, int numDataBytes, int numRSBlocks
            , BitVector result) {
            // "bits" must have "getNumDataBytes" bytes of data.
            if (bits.SizeInBytes() != numDataBytes) {
                throw new WriterException("Number of bits and data bytes does not match");
            }
            // Step 1.  Divide data bytes into blocks and generate error correction bytes for them. We'll
            // store the divided data bytes blocks and error correction bytes blocks into "blocks".
            int dataBytesOffset = 0;
            int maxNumDataBytes = 0;
            int maxNumEcBytes = 0;
            // Since, we know the number of reedsolmon blocks, we can initialize the vector with the number.
            IList<BlockPair> blocks = new List<BlockPair>(numRSBlocks);
            for (int i = 0; i < numRSBlocks; ++i) {
                int[] numDataBytesInBlock = new int[1];
                int[] numEcBytesInBlock = new int[1];
                GetNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, i, numDataBytesInBlock, numEcBytesInBlock
                    );
                ByteArray dataBytes = new ByteArray();
                dataBytes.Set(bits.GetArray(), dataBytesOffset, numDataBytesInBlock[0]);
                ByteArray ecBytes = GenerateECBytes(dataBytes, numEcBytesInBlock[0]);
                blocks.Add(new BlockPair(dataBytes, ecBytes));
                maxNumDataBytes = Math.Max(maxNumDataBytes, dataBytes.Size());
                maxNumEcBytes = Math.Max(maxNumEcBytes, ecBytes.Size());
                dataBytesOffset += numDataBytesInBlock[0];
            }
            if (numDataBytes != dataBytesOffset) {
                throw new WriterException("Data bytes does not match offset");
            }
            // First, place data blocks.
            for (int i = 0; i < maxNumDataBytes; ++i) {
                for (int j = 0; j < blocks.Count; ++j) {
                    ByteArray dataBytes = blocks[j].GetDataBytes();
                    if (i < dataBytes.Size()) {
                        result.AppendBits(dataBytes.At(i), 8);
                    }
                }
            }
            // Then, place error correction blocks.
            for (int i = 0; i < maxNumEcBytes; ++i) {
                for (int j = 0; j < blocks.Count; ++j) {
                    ByteArray ecBytes = blocks[j].GetErrorCorrectionBytes();
                    if (i < ecBytes.Size()) {
                        result.AppendBits(ecBytes.At(i), 8);
                    }
                }
            }
            // Should be same.
            if (numTotalBytes != result.SizeInBytes()) {
                throw new WriterException("Interleaving error: " + numTotalBytes + " and " + result.SizeInBytes() + " differ."
                    );
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static ByteArray GenerateECBytes(ByteArray dataBytes, int numEcBytesInBlock) {
            int numDataBytes = dataBytes.Size();
            int[] toEncode = new int[numDataBytes + numEcBytesInBlock];
            for (int i = 0; i < numDataBytes; i++) {
                toEncode[i] = dataBytes.At(i);
            }
            new ReedSolomonEncoder(GF256.QR_CODE_FIELD).Encode(toEncode, numEcBytesInBlock);
            ByteArray ecBytes = new ByteArray(numEcBytesInBlock);
            for (int i = 0; i < numEcBytesInBlock; i++) {
                ecBytes.Set(i, toEncode[numDataBytes + i]);
            }
            return ecBytes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Append mode info.</summary>
        /// <remarks>Append mode info. On success, store the result in "bits".</remarks>
        internal static void AppendModeInfo(Mode mode, BitVector bits) {
            bits.AppendBits(mode.GetBits(), 4);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Append length info.</summary>
        /// <remarks>Append length info. On success, store the result in "bits".</remarks>
        internal static void AppendLengthInfo(int numLetters, int version, Mode mode, BitVector bits) {
            int numBits = mode.GetCharacterCountBits(Version.GetVersionForNumber(version));
            if (numLetters > ((1 << numBits) - 1)) {
                throw new WriterException(numLetters + "is bigger than" + ((1 << numBits) - 1));
            }
            bits.AppendBits(numLetters, numBits);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Append "bytes" in "mode" mode (encoding) into "bits".</summary>
        /// <remarks>Append "bytes" in "mode" mode (encoding) into "bits". On success, store the result in "bits".</remarks>
        internal static void AppendBytes(String content, Mode mode, BitVector bits, String encoding) {
            if (mode.Equals(Mode.NUMERIC)) {
                AppendNumericBytes(content, bits);
            }
            else {
                if (mode.Equals(Mode.ALPHANUMERIC)) {
                    AppendAlphanumericBytes(content, bits);
                }
                else {
                    if (mode.Equals(Mode.BYTE)) {
                        Append8BitBytes(content, bits, encoding);
                    }
                    else {
                        if (mode.Equals(Mode.KANJI)) {
                            AppendKanjiBytes(content, bits);
                        }
                        else {
                            throw new WriterException("Invalid mode: " + mode);
                        }
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AppendNumericBytes(String content, BitVector bits) {
            int length = content.Length;
            int i = 0;
            while (i < length) {
                int num1 = content[i] - '0';
                if (i + 2 < length) {
                    // Encode three numeric letters in ten bits.
                    int num2 = content[i + 1] - '0';
                    int num3 = content[i + 2] - '0';
                    bits.AppendBits(num1 * 100 + num2 * 10 + num3, 10);
                    i += 3;
                }
                else {
                    if (i + 1 < length) {
                        // Encode two numeric letters in seven bits.
                        int num2 = content[i + 1] - '0';
                        bits.AppendBits(num1 * 10 + num2, 7);
                        i += 2;
                    }
                    else {
                        // Encode one numeric letter in four bits.
                        bits.AppendBits(num1, 4);
                        i++;
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AppendAlphanumericBytes(String content, BitVector bits) {
            int length = content.Length;
            int i = 0;
            while (i < length) {
                int code1 = GetAlphanumericCode(content[i]);
                if (code1 == -1) {
                    throw new WriterException();
                }
                if (i + 1 < length) {
                    int code2 = GetAlphanumericCode(content[i + 1]);
                    if (code2 == -1) {
                        throw new WriterException();
                    }
                    // Encode two alphanumeric letters in 11 bits.
                    bits.AppendBits(code1 * 45 + code2, 11);
                    i += 2;
                }
                else {
                    // Encode one alphanumeric letter in six bits.
                    bits.AppendBits(code1, 6);
                    i++;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void Append8BitBytes(String content, BitVector bits, String encoding) {
            byte[] bytes;
            try {
                bytes = content.GetBytes(encoding);
            }
            catch (ArgumentException uee) {
                throw new WriterException(uee.ToString());
            }
            for (int i = 0; i < bytes.Length; ++i) {
                bits.AppendBits(bytes[i], 8);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AppendKanjiBytes(String content, BitVector bits) {
            byte[] bytes;
            try {
                bytes = content.GetBytes("Shift_JIS");
            }
            catch (ArgumentException uee) {
                throw new WriterException(uee.ToString());
            }
            int length = bytes.Length;
            for (int i = 0; i < length; i += 2) {
                int byte1 = bytes[i] & 0xFF;
                int byte2 = bytes[i + 1] & 0xFF;
                int code = (byte1 << 8) | byte2;
                int subtracted = -1;
                if (code >= 0x8140 && code <= 0x9ffc) {
                    subtracted = code - 0x8140;
                }
                else {
                    if (code >= 0xe040 && code <= 0xebbf) {
                        subtracted = code - 0xc140;
                    }
                }
                if (subtracted == -1) {
                    throw new WriterException("Invalid byte sequence");
                }
                int encoded = ((subtracted >> 8) * 0xc0) + (subtracted & 0xff);
                bits.AppendBits(encoded, 13);
            }
        }
//\endcond

        private static void AppendECI(CharacterSetECI eci, BitVector bits) {
            bits.AppendBits(Mode.ECI.GetBits(), 4);
            // This is correct for values up to 127, which is all we need now.
            bits.AppendBits(eci.GetValue(), 8);
        }
    }
//\endcond
}
