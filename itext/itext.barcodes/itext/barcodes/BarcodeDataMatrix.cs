/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Barcodes.Dmcode;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Barcodes {
    /// <summary>
    /// A Data Matrix is a two-dimensional bar code consisting of black and white "cells" or dots arranged in either a square
    /// or
    /// rectangular pattern, also known as a matrix.
    /// </summary>
    /// <remarks>
    /// A Data Matrix is a two-dimensional bar code consisting of black and white "cells" or dots arranged in either a square
    /// or
    /// rectangular pattern, also known as a matrix. The information to be encoded can be text or numeric data. Usual data
    /// size is from a few bytes up to 1556 bytes. The length of the encoded data depends on the number of cells in the
    /// matrix.
    /// </remarks>
    public class BarcodeDataMatrix : Barcode2D {
        /// <summary>No error.</summary>
        public const int DM_NO_ERROR = 0;

        /// <summary>The text is too big for the symbology capabilities.</summary>
        public const int DM_ERROR_TEXT_TOO_BIG = 1;

        /// <summary>The dimensions given for the symbol are illegal.</summary>
        public const int DM_ERROR_INVALID_SQUARE = 3;

        /// <summary>An error while parsing an extension.</summary>
        public const int DM_ERROR_EXTENSION = 5;

        /// <summary>The best encodation will be used.</summary>
        public const int DM_AUTO = 0;

        /// <summary>ASCII encodation.</summary>
        public const int DM_ASCII = 1;

        /// <summary>C40 encodation.</summary>
        public const int DM_C40 = 2;

        /// <summary>TEXT encodation.</summary>
        public const int DM_TEXT = 3;

        /// <summary>Binary encodation.</summary>
        public const int DM_B256 = 4;

        /// <summary>X12 encodation.</summary>
        public const int DM_X12 = 5;

        /// <summary>EDIFACT encodation.</summary>
        public const int DM_EDIFACT = 6;

        /// <summary>No encodation needed.</summary>
        /// <remarks>No encodation needed. The bytes provided are already encoded.</remarks>
        public const int DM_RAW = 7;

        /// <summary>Allows extensions to be embedded at the start of the text.</summary>
        public const int DM_EXTENSION = 32;

        /// <summary>Doesn't generate the image but returns all the other information.</summary>
        public const int DM_TEST = 64;

        public const String DEFAULT_DATA_MATRIX_ENCODING = "iso-8859-1";

        private const byte LATCH_B256 = (byte)231;

        private const byte LATCH_EDIFACT = (byte)240;

        private const byte LATCH_X12 = (byte)238;

        private const byte LATCH_TEXT = (byte)239;

        private const byte LATCH_C40 = (byte)230;

        private const byte UNLATCH = (byte)254;

        private const byte EXTENDED_ASCII = (byte)235;

        private const byte PADDING = (byte)129;

        private String encoding;

        private static readonly DmParams[] dmSizes = new DmParams[] { new DmParams(10, 10, 10, 10, 3, 3, 5), new DmParams
            (12, 12, 12, 12, 5, 5, 7), new DmParams(8, 18, 8, 18, 5, 5, 7), new DmParams(14, 14, 14, 14, 8, 8, 10)
            , new DmParams(8, 32, 8, 16, 10, 10, 11), new DmParams(16, 16, 16, 16, 12, 12, 12), new DmParams(12, 26
            , 12, 26, 16, 16, 14), new DmParams(18, 18, 18, 18, 18, 18, 14), new DmParams(20, 20, 20, 20, 22, 22, 
            18), new DmParams(12, 36, 12, 18, 22, 22, 18), new DmParams(22, 22, 22, 22, 30, 30, 20), new DmParams(
            16, 36, 16, 18, 32, 32, 24), new DmParams(24, 24, 24, 24, 36, 36, 24), new DmParams(26, 26, 26, 26, 44
            , 44, 28), new DmParams(16, 48, 16, 24, 49, 49, 28), new DmParams(32, 32, 16, 16, 62, 62, 36), new DmParams
            (36, 36, 18, 18, 86, 86, 42), new DmParams(40, 40, 20, 20, 114, 114, 48), new DmParams(44, 44, 22, 22, 
            144, 144, 56), new DmParams(48, 48, 24, 24, 174, 174, 68), new DmParams(52, 52, 26, 26, 204, 102, 42), 
            new DmParams(64, 64, 16, 16, 280, 140, 56), new DmParams(72, 72, 18, 18, 368, 92, 36), new DmParams(80
            , 80, 20, 20, 456, 114, 48), new DmParams(88, 88, 22, 22, 576, 144, 56), new DmParams(96, 96, 24, 24, 
            696, 174, 68), new DmParams(104, 104, 26, 26, 816, 136, 56), new DmParams(120, 120, 20, 20, 1050, 175, 
            68), new DmParams(132, 132, 22, 22, 1304, 163, 62), new DmParams(144, 144, 24, 24, 1558, 156, 62) };

        private const String X12 = "\r*> 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private int extOut;

        private short[] place;

        private byte[] image;

        private int height;

        private int width;

        private int ws;

        private int options;

        // value f[i][j] is the optimal amount of bytes required to encode substring(0, j)
        private int[][] f;

        // switchMode[i][j] = k means that when encoding j-th symbol with mode = i + 1,
        // we have to encode the previous symbol with mode = k in order to get optimal f[i][j] value
        private int[][] switchMode;

        /// <summary>Creates an instance of this class.</summary>
        public BarcodeDataMatrix() {
            encoding = DEFAULT_DATA_MATRIX_ENCODING;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="BarcodeDataMatrix"/>
        /// </summary>
        /// <param name="code">
        /// the code to generate. It should be noted that all characters will be encoded using the default
        /// encoding, ISO-8859-1
        /// </param>
        public BarcodeDataMatrix(String code) {
            encoding = DEFAULT_DATA_MATRIX_ENCODING;
            SetCode(code);
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="BarcodeDataMatrix"/>
        /// </summary>
        /// <param name="code">the code to generate.</param>
        /// <param name="encoding">the encoding to use when generating the barcode</param>
        public BarcodeDataMatrix(String code, String encoding) {
            this.encoding = encoding;
            SetCode(code);
        }

        /// <summary><inheritDoc/></summary>
        public override Rectangle GetBarcodeSize() {
            return new Rectangle(0, 0, width + 2 * ws, height + 2 * ws);
        }

        /// <summary><inheritDoc/></summary>
        public override Rectangle PlaceBarcode(PdfCanvas canvas, Color foreground) {
            return PlaceBarcode(canvas, foreground, DEFAULT_MODULE_SIZE);
        }

        /// <summary><inheritDoc/></summary>
        public override PdfFormXObject CreateFormXObject(Color foreground, PdfDocument document) {
            return CreateFormXObject(foreground, DEFAULT_MODULE_SIZE, document);
        }

        /// <summary>Creates a PdfFormXObject with the barcode with given module width and module height.</summary>
        /// <param name="foreground">The color of the pixels. It can be <c>null</c></param>
        /// <param name="moduleSide">The side (width and height) of the pixels.</param>
        /// <param name="document">The document</param>
        /// <returns>the XObject.</returns>
        public virtual PdfFormXObject CreateFormXObject(Color foreground, float moduleSide, PdfDocument document) {
            PdfFormXObject xObject = new PdfFormXObject((Rectangle)null);
            Rectangle rect = PlaceBarcode(new PdfCanvas(xObject, document), foreground, moduleSide);
            xObject.SetBBox(new PdfArray(rect));
            return xObject;
        }

        /// <summary>Places the barcode in a PdfCanvas</summary>
        /// <param name="canvas">the canvas to place the barcode on</param>
        /// <param name="foreground">The foreground color of the barcode</param>
        /// <param name="moduleSide">The side (width and height) of the pixels.</param>
        /// <returns>the dimensions the barcode occupies</returns>
        public virtual Rectangle PlaceBarcode(PdfCanvas canvas, Color foreground, float moduleSide) {
            if (image == null) {
                return null;
            }
            if (foreground != null) {
                canvas.SetFillColor(foreground);
            }
            int w = width + 2 * ws;
            int h = height + 2 * ws;
            int stride = (w + 7) / 8;
            for (int k = 0; k < h; ++k) {
                int p = k * stride;
                for (int j = 0; j < w; ++j) {
                    int b = image[p + j / 8] & 0xff;
                    b <<= j % 8;
                    if ((b & 0x80) != 0) {
                        canvas.Rectangle(j * moduleSide, (h - k - 1) * moduleSide, moduleSide, moduleSide);
                    }
                }
            }
            canvas.Fill();
            return GetBarcodeSize();
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        /// <summary>Gets the barcode size</summary>
        /// <param name="moduleHeight">The height of the module</param>
        /// <param name="moduleWidth">The width of the module</param>
        /// <returns>The size of the barcode</returns>
        public virtual Rectangle GetBarcodeSize(float moduleHeight, float moduleWidth) {
            return new Rectangle(0, 0, (width + 2 * ws) * moduleHeight, (height + 2 * ws) * moduleWidth);
        }

        /// <summary>Creates a barcode.</summary>
        /// <remarks>Creates a barcode. The <c>String</c> is interpreted with the ISO-8859-1 encoding</remarks>
        /// <param name="text">the text</param>
        /// <returns>
        /// the status of the generation. It can be one of this values:
        /// <c>DM_NO_ERROR</c> - no error.<br />
        /// <c>DM_ERROR_TEXT_TOO_BIG</c> - the text is too big for the symbology capabilities.<br />
        /// <c>DM_ERROR_INVALID_SQUARE</c> - the dimensions given for the symbol are illegal.<br />
        /// <c>DM_ERROR_EXTENSION</c> - an error was while parsing an extension.
        /// </returns>
        public virtual int SetCode(String text) {
            byte[] t;
            try {
                t = text.GetBytes(encoding);
            }
            catch (ArgumentException) {
                throw new ArgumentException("text has to be encoded in iso-8859-1");
            }
            return SetCode(t, 0, t.Length);
        }

        /// <summary>Creates a barcode.</summary>
        /// <param name="text">the text</param>
        /// <param name="textOffset">the offset to the start of the text</param>
        /// <param name="textSize">the text size</param>
        /// <returns>
        /// the status of the generation. It can be one of this values:
        /// <c>DM_NO_ERROR</c> - no error.<br />
        /// <c>DM_ERROR_TEXT_TOO_BIG</c> - the text is too big for the symbology capabilities.<br />
        /// <c>DM_ERROR_INVALID_SQUARE</c> - the dimensions given for the symbol are illegal.<br />
        /// <c>DM_ERROR_EXTENSION</c> - an error was while parsing an extension.
        /// </returns>
        public virtual int SetCode(byte[] text, int textOffset, int textSize) {
            if (textOffset < 0) {
                throw new IndexOutOfRangeException("" + textOffset);
            }
            if (textOffset + textSize > text.Length || textSize < 0) {
                throw new IndexOutOfRangeException("" + textSize);
            }
            int extCount;
            int e;
            int k;
            int full;
            DmParams dm;
            DmParams last;
            byte[] data = new byte[2500];
            extOut = 0;
            extCount = ProcessExtensions(text, textOffset, textSize, data);
            if (extCount < 0) {
                return DM_ERROR_EXTENSION;
            }
            e = -1;
            f = new int[][] { new int[textSize - extOut], new int[textSize - extOut], new int[textSize - extOut], new 
                int[textSize - extOut], new int[textSize - extOut], new int[textSize - extOut] };
            switchMode = new int[][] { new int[textSize - extOut], new int[textSize - extOut], new int[textSize - extOut
                ], new int[textSize - extOut], new int[textSize - extOut], new int[textSize - extOut] };
            if (height == 0 || width == 0) {
                last = dmSizes[dmSizes.Length - 1];
                e = GetEncodation(text, textOffset + extOut, textSize - extOut, data, extCount, last.GetDataSize() - extCount
                    , options, false);
                if (e < 0) {
                    return DM_ERROR_TEXT_TOO_BIG;
                }
                e += extCount;
                for (k = 0; k < dmSizes.Length; ++k) {
                    if (dmSizes[k].GetDataSize() >= e) {
                        break;
                    }
                }
                dm = dmSizes[k];
                height = dm.GetHeight();
                width = dm.GetWidth();
            }
            else {
                for (k = 0; k < dmSizes.Length; ++k) {
                    if (height == dmSizes[k].GetHeight() && width == dmSizes[k].GetWidth()) {
                        break;
                    }
                }
                if (k == dmSizes.Length) {
                    return DM_ERROR_INVALID_SQUARE;
                }
                dm = dmSizes[k];
                e = GetEncodation(text, textOffset + extOut, textSize - extOut, data, extCount, dm.GetDataSize() - extCount
                    , options, true);
                if (e < 0) {
                    return DM_ERROR_TEXT_TOO_BIG;
                }
                e += extCount;
            }
            if ((options & DM_TEST) != 0) {
                return DM_NO_ERROR;
            }
            image = new byte[(dm.GetWidth() + 2 * ws + 7) / 8 * (dm.GetHeight() + 2 * ws)];
            MakePadding(data, e, dm.GetDataSize() - e);
            place = Placement.DoPlacement(dm.GetHeight() - dm.GetHeight() / dm.GetHeightSection() * 2, dm.GetWidth() -
                 dm.GetWidth() / dm.GetWidthSection() * 2);
            full = dm.GetDataSize() + (dm.GetDataSize() + 2) / dm.GetDataBlock() * dm.GetErrorBlock();
            ReedSolomon.GenerateECC(data, dm.GetDataSize(), dm.GetDataBlock(), dm.GetErrorBlock());
            Draw(data, full, dm);
            return DM_NO_ERROR;
        }

        /// <summary>Gets the height of the barcode.</summary>
        /// <remarks>
        /// Gets the height of the barcode. Will contain the real height used after a successful call
        /// to <c>generate()</c>. This height doesn't include the whitespace border, if any.
        /// </remarks>
        /// <returns>the height of the barcode</returns>
        public virtual int GetHeight() {
            return height;
        }

        /// <summary>Sets the height of the barcode.</summary>
        /// <remarks>
        /// Sets the height of the barcode. If the height is zero it will be calculated.
        /// This height doesn't include the whitespace border, if any.
        /// The allowed dimensions are (width, height):<para />
        /// 10, 10<br />
        /// 12, 12<br />
        /// 18, 8<br />
        /// 14, 14<br />
        /// 32, 8<br />
        /// 16, 16<br />
        /// 26, 12<br />
        /// 18, 18<br />
        /// 20, 20<br />
        /// 36, 12<br />
        /// 22, 22<br />
        /// 36, 16<br />
        /// 24, 24<br />
        /// 26, 26<br />
        /// 48, 16<br />
        /// 32, 32<br />
        /// 36, 36<br />
        /// 40, 40<br />
        /// 44, 44<br />
        /// 48, 48<br />
        /// 52, 52<br />
        /// 64, 64<br />
        /// 72, 72<br />
        /// 80, 80<br />
        /// 88, 88<br />
        /// 96, 96<br />
        /// 104, 104<br />
        /// 120, 120<br />
        /// 132, 132<br />
        /// 144, 144<br />
        /// </remarks>
        /// <param name="height">the height of the barcode</param>
        public virtual void SetHeight(int height) {
            this.height = height;
        }

        /// <summary>Gets the width of the barcode.</summary>
        /// <remarks>
        /// Gets the width of the barcode. Will contain the real width used after a successful call
        /// to <c>generate()</c>. This width doesn't include the whitespace border, if any.
        /// </remarks>
        /// <returns>the width of the barcode</returns>
        public virtual int GetWidth() {
            return width;
        }

        /// <summary>Sets the width of the barcode.</summary>
        /// <remarks>
        /// Sets the width of the barcode. If the width is zero it will be calculated.
        /// This width doesn't include the whitespace border, if any.
        /// The allowed dimensions are (width, height):<para />
        /// 10, 10<br />
        /// 12, 12<br />
        /// 18, 8<br />
        /// 14, 14<br />
        /// 32, 8<br />
        /// 16, 16<br />
        /// 26, 12<br />
        /// 18, 18<br />
        /// 20, 20<br />
        /// 36, 12<br />
        /// 22, 22<br />
        /// 36, 16<br />
        /// 24, 24<br />
        /// 26, 26<br />
        /// 48, 16<br />
        /// 32, 32<br />
        /// 36, 36<br />
        /// 40, 40<br />
        /// 44, 44<br />
        /// 48, 48<br />
        /// 52, 52<br />
        /// 64, 64<br />
        /// 72, 72<br />
        /// 80, 80<br />
        /// 88, 88<br />
        /// 96, 96<br />
        /// 104, 104<br />
        /// 120, 120<br />
        /// 132, 132<br />
        /// 144, 144<br />
        /// </remarks>
        /// <param name="width">the width of the barcode</param>
        public virtual void SetWidth(int width) {
            this.width = width;
        }

        /// <summary>Gets the whitespace border around the barcode.</summary>
        /// <returns>the whitespace border around the barcode</returns>
        public virtual int GetWs() {
            return ws;
        }

        /// <summary>Sets the whitespace border around the barcode.</summary>
        /// <param name="ws">the whitespace border around the barcode</param>
        public virtual void SetWs(int ws) {
            this.ws = ws;
        }

        /// <summary>Gets the barcode options.</summary>
        /// <returns>the barcode options</returns>
        public virtual int GetOptions() {
            return options;
        }

        /// <summary>Sets the options for the barcode generation.</summary>
        /// <remarks>
        /// Sets the options for the barcode generation. The options can be:<para />
        /// One of:<br />
        /// <c>DM_AUTO</c> - the best encodation will be used<br />
        /// <c>DM_ASCII</c> - ASCII encodation<br />
        /// <c>DM_C40</c> - C40 encodation<br />
        /// <c>DM_TEXT</c> - TEXT encodation<br />
        /// <c>DM_B256</c> - binary encodation<br />
        /// <c>DM_X12</c> - X12 encodation<br />
        /// <c>DM_EDIFACT</c> - EDIFACT encodation<br />
        /// <c>DM_RAW</c> - no encodation. The bytes provided are already encoded and will be added directly to the barcode, using padding if needed. It assumes that the encodation state is left at ASCII after the last byte.<br />
        /// <br />
        /// One of:<br />
        /// <c>DM_EXTENSION</c> - allows extensions to be embedded at the start of the text:<para />
        /// exxxxxx - ECI number xxxxxx<br />
        /// m5 - macro 5<br />
        /// m6 - macro 6<br />
        /// f - FNC1<br />
        /// saabbccccc - Structured Append, aa symbol position (1-16), bb total number of symbols (2-16), ccccc file identification (0-64515)<br />
        /// p - Reader programming<br />
        /// . - extension terminator<para />
        /// Example for a structured append, symbol 2 of 6, with FNC1 and ECI 000005. The actual text is "Hello".<para />
        /// s020600075fe000005.Hello<para />
        /// One of:<br />
        /// <c>DM_TEST</c> - doesn't generate the image but returns all the other information.
        /// </remarks>
        /// <param name="options">the barcode options</param>
        public virtual void SetOptions(int options) {
            this.options = options;
        }

        /// <summary>setting encoding for data matrix code ( default  encoding iso-8859-1)</summary>
        /// <param name="encoding">encoding for data matrix code</param>
        public virtual void SetEncoding(String encoding) {
            this.encoding = encoding;
        }

        /// <summary>getting encoding for data matrix code</summary>
        /// <returns>encoding for data matrix code</returns>
        public virtual String GetEncoding() {
            return encoding;
        }

        private static void MakePadding(byte[] data, int position, int count) {
            //already in ascii mode
            if (count <= 0) {
                return;
            }
            data[position++] = PADDING;
            while (--count > 0) {
                int t = 129 + (position + 1) * 149 % 253 + 1;
                if (t > 254) {
                    t -= 254;
                }
                data[position++] = (byte)t;
            }
        }

        private static bool IsDigit(int c) {
            return c >= '0' && c <= '9';
        }

        // when symbolIndex is non-negative, textLength should equal 1. All other encodations behave the same way.
        private int AsciiEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int 
            dataLength, int symbolIndex, int prevEnc, int origDataOffset) {
            int ptrIn;
            int ptrOut;
            int c;
            ptrIn = textOffset;
            ptrOut = dataOffset;
            textLength += textOffset;
            dataLength += dataOffset;
            while (ptrIn < textLength) {
                c = text[ptrIn++] & 0xff;
                if (IsDigit(c) && symbolIndex > 0 && prevEnc == DM_ASCII && IsDigit(text[ptrIn - 2] & 0xff) && data[dataOffset
                     - 1] > 48 && data[dataOffset - 1] < 59) {
                    data[ptrOut - 1] = (byte)(((text[ptrIn - 2] & 0xff) - '0') * 10 + c - '0' + 130);
                    return ptrOut - origDataOffset;
                }
                if (ptrOut >= dataLength) {
                    return -1;
                }
                if (IsDigit(c) && symbolIndex < 0 && ptrIn < textLength && IsDigit(text[ptrIn] & 0xff)) {
                    data[ptrOut++] = (byte)((c - '0') * 10 + (text[ptrIn++] & 0xff) - '0' + 130);
                }
                else {
                    if (c > 127) {
                        if (ptrOut + 1 >= dataLength) {
                            return -1;
                        }
                        data[ptrOut++] = EXTENDED_ASCII;
                        data[ptrOut++] = (byte)(c - 128 + 1);
                    }
                    else {
                        data[ptrOut++] = (byte)(c + 1);
                    }
                }
            }
            return ptrOut - origDataOffset;
        }

        private int B256Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength
            , int symbolIndex, int prevEnc, int origDataOffset) {
            int minRequiredDataIncrement;
            if (textLength == 0) {
                return 0;
            }
            int simulatedDataOffset = dataOffset;
            if (prevEnc != DM_B256) {
                if (textLength < 250 && textLength + 2 > dataLength) {
                    return -1;
                }
                if (textLength >= 250 && textLength + 3 > dataLength) {
                    return -1;
                }
                data[dataOffset] = LATCH_B256;
            }
            else {
                int latestModeEntry = symbolIndex - 1;
                while (latestModeEntry > 0 && switchMode[DM_B256 - 1][latestModeEntry] == DM_B256) {
                    latestModeEntry--;
                }
                textLength = symbolIndex - latestModeEntry + 1;
                if (textLength != 250 && 1 > dataLength) {
                    return -1;
                }
                if (textLength == 250 && 2 > dataLength) {
                    return -1;
                }
                simulatedDataOffset -= (textLength - 1) + (textLength < 250 ? 2 : 3);
            }
            if (textLength < 250) {
                data[simulatedDataOffset + 1] = (byte)textLength;
                minRequiredDataIncrement = prevEnc != DM_B256 ? 2 : 0;
            }
            else {
                if (textLength == 250 && prevEnc == DM_B256) {
                    data[simulatedDataOffset + 1] = (byte)(textLength / 250 + 249);
                    for (int i = dataOffset + 1; i > simulatedDataOffset + 2; i--) {
                        data[i] = data[i - 1];
                    }
                    data[simulatedDataOffset + 2] = (byte)(textLength % 250);
                    minRequiredDataIncrement = 1;
                }
                else {
                    data[simulatedDataOffset + 1] = (byte)(textLength / 250 + 249);
                    data[simulatedDataOffset + 2] = (byte)(textLength % 250);
                    minRequiredDataIncrement = prevEnc != DM_B256 ? 3 : 0;
                }
            }
            if (prevEnc == DM_B256) {
                textLength = 1;
            }
            Array.Copy(text, textOffset, data, minRequiredDataIncrement + dataOffset, textLength);
            for (int j = prevEnc != DM_B256 ? dataOffset + 1 : dataOffset; j < minRequiredDataIncrement + textLength +
                 dataOffset; ++j) {
                RandomizationAlgorithm255(data, j);
            }
            if (prevEnc == DM_B256) {
                RandomizationAlgorithm255(data, simulatedDataOffset + 1);
            }
            return textLength + dataOffset + minRequiredDataIncrement - origDataOffset;
        }

        private void RandomizationAlgorithm255(byte[] data, int j) {
            int c = data[j] & 0xff;
            int prn = 149 * (j + 1) % 255 + 1;
            int tv = c + prn;
            if (tv > 255) {
                tv -= 256;
            }
            data[j] = (byte)tv;
        }

        private int X12Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength
            , int symbolIndex, int prevEnc, int origDataOffset) {
            int ptrIn;
            int ptrOut;
            int count;
            int k;
            int n;
            int ci;
            bool latch = true;
            byte c;
            if (textLength == 0) {
                return 0;
            }
            ptrIn = 0;
            ptrOut = 0;
            byte[] x = new byte[textLength];
            count = 0;
            for (; ptrIn < textLength; ++ptrIn) {
                int i = X12.IndexOf((char)text[ptrIn + textOffset]);
                if (i >= 0) {
                    x[ptrIn] = (byte)i;
                    ++count;
                }
                else {
                    x[ptrIn] = 100;
                    if (count >= 6) {
                        count -= count / 3 * 3;
                    }
                    for (k = 0; k < count; ++k) {
                        x[ptrIn - k - 1] = 100;
                    }
                    count = 0;
                }
            }
            if (count >= 6) {
                count -= count / 3 * 3;
            }
            for (k = 0; k < count; ++k) {
                x[ptrIn - k - 1] = 100;
            }
            ptrIn = 0;
            c = 0;
            for (; ptrIn < textLength; ++ptrIn) {
                c = x[ptrIn];
                if (ptrOut > dataLength) {
                    break;
                }
                if (c < 40) {
                    if (ptrIn == 0 && latch || ptrIn > 0 && x[ptrIn - 1] > 40) {
                        data[dataOffset + ptrOut++] = LATCH_X12;
                    }
                    if (ptrOut + 2 > dataLength) {
                        break;
                    }
                    n = 1600 * x[ptrIn] + 40 * x[ptrIn + 1] + x[ptrIn + 2] + 1;
                    data[dataOffset + ptrOut++] = (byte)(n / 256);
                    data[dataOffset + ptrOut++] = (byte)n;
                    ptrIn += 2;
                }
                else {
                    bool enterASCII = true;
                    if (symbolIndex <= 0) {
                        if (ptrIn > 0 && x[ptrIn - 1] < 40) {
                            data[dataOffset + ptrOut++] = UNLATCH;
                        }
                    }
                    else {
                        if (symbolIndex > 4 && prevEnc == DM_X12 && X12.IndexOf((char)text[textOffset]) >= 0 && X12.IndexOf((char)
                            text[textOffset - 1]) >= 0) {
                            int latestModeEntry = symbolIndex - 1;
                            while (latestModeEntry > 0 && switchMode[DM_X12 - 1][latestModeEntry] == DM_X12 && (X12.IndexOf((char)text
                                [textOffset - (symbolIndex - latestModeEntry + 1)])) >= 0) {
                                latestModeEntry--;
                            }
                            int unlatch = -1;
                            if (symbolIndex - latestModeEntry >= 5) {
                                for (int i = 1; i <= symbolIndex - latestModeEntry; i++) {
                                    if (data[dataOffset - i] == UNLATCH) {
                                        unlatch = dataOffset - i;
                                        break;
                                    }
                                }
                                int amountOfEncodedWithASCII = unlatch >= 0 ? dataOffset - unlatch - 1 : symbolIndex - latestModeEntry;
                                if (amountOfEncodedWithASCII % 3 == 2) {
                                    enterASCII = false;
                                    textLength = amountOfEncodedWithASCII + 1;
                                    textOffset -= amountOfEncodedWithASCII;
                                    dataLength += unlatch < 0 ? amountOfEncodedWithASCII : amountOfEncodedWithASCII + 1;
                                    dataOffset -= unlatch < 0 ? amountOfEncodedWithASCII : amountOfEncodedWithASCII + 1;
                                    ptrIn = -1;
                                    latch = unlatch != dataOffset;
                                    x = new byte[amountOfEncodedWithASCII + 1];
                                    for (int i = 0; i <= amountOfEncodedWithASCII; i++) {
                                        x[i] = (byte)X12.IndexOf((char)text[textOffset + i]);
                                    }
                                }
                                else {
                                    x = new byte[1];
                                    x[0] = 100;
                                }
                            }
                        }
                    }
                    if (enterASCII) {
                        int i = AsciiEncodation(text, textOffset + ptrIn, 1, data, dataOffset + ptrOut, dataLength, -1, -1, origDataOffset
                            );
                        if (i < 0) {
                            return -1;
                        }
                        if (data[dataOffset + ptrOut] == EXTENDED_ASCII) {
                            ptrOut++;
                        }
                        ptrOut++;
                    }
                }
            }
            c = 100;
            if (textLength > 0) {
                c = x[textLength - 1];
            }
            if (ptrIn != textLength) {
                return -1;
            }
            if (c < 40) {
                data[dataOffset + ptrOut++] = UNLATCH;
            }
            if (ptrOut > dataLength) {
                return -1;
            }
            return ptrOut + dataOffset - origDataOffset;
        }

        private int EdifactEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int
             dataLength, int symbolIndex, int prevEnc, int origDataOffset, bool sizeFixed) {
            int ptrIn;
            int ptrOut;
            int edi;
            int pedi;
            int c;
            if (textLength == 0) {
                return 0;
            }
            ptrIn = 0;
            ptrOut = 0;
            edi = 0;
            pedi = 18;
            bool ascii = true;
            int latestModeEntryActual = -1;
            int latestModeEntryC40orX12 = -1;
            int prevMode = -1;
            if (prevEnc == DM_EDIFACT && ((text[textOffset] & 0xff & 0xe0) == 0x40 || (text[textOffset] & 0xff & 0xe0)
                 == 0x20) && (text[textOffset] & 0xff) != '_' && ((text[textOffset - 1] & 0xff & 0xe0) == 0x40 || (text
                [textOffset - 1] & 0xff & 0xe0) == 0x20) && (text[textOffset - 1] & 0xff) != '_') {
                latestModeEntryActual = symbolIndex - 1;
                while (latestModeEntryActual > 0 && switchMode[DM_EDIFACT - 1][latestModeEntryActual] == DM_EDIFACT) {
                    c = text[textOffset - (symbolIndex - latestModeEntryActual + 1)] & 0xff;
                    if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_') {
                        latestModeEntryActual--;
                    }
                    else {
                        break;
                    }
                }
                prevMode = switchMode[DM_EDIFACT - 1][latestModeEntryActual] == DM_C40 || switchMode[DM_EDIFACT - 1][latestModeEntryActual
                    ] == DM_X12 ? switchMode[DM_EDIFACT - 1][latestModeEntryActual] : -1;
                if (prevMode > 0) {
                    latestModeEntryC40orX12 = latestModeEntryActual;
                }
                while (prevMode > 0 && latestModeEntryC40orX12 > 0 && switchMode[prevMode - 1][latestModeEntryC40orX12] ==
                     prevMode) {
                    c = text[textOffset - (symbolIndex - latestModeEntryC40orX12 + 1)] & 0xff;
                    if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_') {
                        latestModeEntryC40orX12--;
                    }
                    else {
                        latestModeEntryC40orX12 = -1;
                        break;
                    }
                }
            }
            int dataSize = dataOffset + dataLength;
            bool asciiOneSymbol = false;
            if (symbolIndex != -1) {
                asciiOneSymbol = true;
            }
            int dataTaken = 0;
            int dataRequired = 0;
            if (latestModeEntryC40orX12 >= 0 && symbolIndex - latestModeEntryC40orX12 + 1 > 9) {
                textLength = symbolIndex - latestModeEntryC40orX12 + 1;
                dataTaken = 0;
                dataRequired = 0;
                dataRequired += 1 + (textLength / 4 * 3);
                if (!sizeFixed && (symbolIndex == text.Length - 1 || symbolIndex < 0) && textLength % 4 < 3) {
                    dataSize = int.MaxValue;
                    for (int i = 0; i < dmSizes.Length; ++i) {
                        if (dmSizes[i].GetDataSize() >= dataRequired + textLength % 4) {
                            dataSize = dmSizes[i].GetDataSize();
                            break;
                        }
                    }
                }
                if (dataSize - dataOffset - dataRequired <= 2 && textLength % 4 <= 2) {
                    dataRequired += (textLength % 4);
                }
                else {
                    dataRequired += (textLength % 4) + 1;
                    if (textLength % 4 == 3) {
                        dataRequired--;
                    }
                }
                for (int i = dataOffset - 1; i >= 0; i--) {
                    dataTaken++;
                    if (data[i] == (prevMode == DM_C40 ? LATCH_C40 : LATCH_X12)) {
                        break;
                    }
                }
                if (dataRequired <= dataTaken) {
                    asciiOneSymbol = false;
                    textOffset -= textLength - 1;
                    dataOffset -= dataTaken;
                    dataLength += dataTaken;
                }
            }
            else {
                if (latestModeEntryActual >= 0 && symbolIndex - latestModeEntryActual + 1 > 9) {
                    textLength = symbolIndex - latestModeEntryActual + 1;
                    dataRequired += 1 + (textLength / 4 * 3);
                    if (dataSize - dataOffset - dataRequired <= 2 && textLength % 4 <= 2) {
                        dataRequired += (textLength % 4);
                    }
                    else {
                        dataRequired += (textLength % 4) + 1;
                        if (textLength % 4 == 3) {
                            dataRequired--;
                        }
                    }
                    int dataNewOffset = 0;
                    int latchEdi = -1;
                    for (int i = origDataOffset; i < dataOffset; i++) {
                        if (data[i] == LATCH_EDIFACT && dataOffset - i <= dataRequired) {
                            latchEdi = i;
                            break;
                        }
                    }
                    if (latchEdi != -1) {
                        dataTaken += dataOffset - latchEdi;
                        if ((text[textOffset] & 0xff) > 127) {
                            dataTaken += 2;
                        }
                        else {
                            if (IsDigit(text[textOffset] & 0xff) && IsDigit(text[textOffset - 1] & 0xff) && data[dataOffset - 1] >= 49
                                 && data[dataOffset - 1] <= 58) {
                                dataTaken--;
                            }
                            dataTaken++;
                        }
                        dataNewOffset = dataOffset - latchEdi;
                    }
                    else {
                        for (int j = symbolIndex - latestModeEntryActual; j >= 0; j--) {
                            if ((text[textOffset - j] & 0xff) > 127) {
                                dataTaken += 2;
                            }
                            else {
                                if (j > 0 && IsDigit(text[textOffset - j] & 0xff) && IsDigit(text[textOffset - j + 1] & 0xff)) {
                                    if (j == 1) {
                                        dataNewOffset = dataTaken;
                                    }
                                    j--;
                                }
                                dataTaken++;
                            }
                            if (j == 1) {
                                dataNewOffset = dataTaken;
                            }
                        }
                    }
                    if (dataRequired <= dataTaken) {
                        asciiOneSymbol = false;
                        textOffset -= textLength - 1;
                        dataOffset -= dataNewOffset;
                        dataLength += dataNewOffset;
                    }
                }
            }
            if (asciiOneSymbol) {
                c = text[textOffset] & 0xff;
                if (IsDigit(c) && textOffset + ptrIn > 0 && IsDigit(text[textOffset - 1] & 0xff) && prevEnc == DM_EDIFACT 
                    && data[dataOffset - 1] >= 49 && data[dataOffset - 1] <= 58) {
                    data[dataOffset + ptrOut - 1] = (byte)(((text[textOffset - 1] & 0xff) - '0') * 10 + c - '0' + 130);
                    return dataOffset - origDataOffset;
                }
                else {
                    return AsciiEncodation(text, textOffset + ptrIn, 1, data, dataOffset + ptrOut, dataLength, -1, -1, origDataOffset
                        );
                }
            }
            for (; ptrIn < textLength; ++ptrIn) {
                c = text[ptrIn + textOffset] & 0xff;
                if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_') {
                    if (ascii) {
                        if (ptrOut + 1 > dataLength) {
                            break;
                        }
                        data[dataOffset + ptrOut++] = LATCH_EDIFACT;
                        ascii = false;
                    }
                    c &= 0x3f;
                    edi |= c << pedi;
                    if (pedi == 0) {
                        if (ptrOut + 3 > dataLength) {
                            break;
                        }
                        data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                        data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                        data[dataOffset + ptrOut++] = (byte)edi;
                        edi = 0;
                        pedi = 18;
                    }
                    else {
                        pedi -= 6;
                    }
                }
                else {
                    if (!ascii) {
                        edi |= ('_' & 0x3f) << pedi;
                        if (ptrOut + 3 - pedi / 8 > dataLength) {
                            break;
                        }
                        data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                        if (pedi <= 12) {
                            data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                        }
                        if (pedi <= 6) {
                            data[dataOffset + ptrOut++] = (byte)edi;
                        }
                        ascii = true;
                        pedi = 18;
                        edi = 0;
                    }
                    if (IsDigit(c) && textOffset + ptrIn > 0 && IsDigit(text[textOffset + ptrIn - 1] & 0xff) && prevEnc == DM_EDIFACT
                         && data[dataOffset - 1] >= 49 && data[dataOffset - 1] <= 58) {
                        data[dataOffset + ptrOut - 1] = (byte)(((text[textOffset - 1] & 0xff) - '0') * 10 + c - '0' + 130);
                        ptrOut--;
                    }
                    else {
                        int i = AsciiEncodation(text, textOffset + ptrIn, 1, data, dataOffset + ptrOut, dataLength, -1, -1, origDataOffset
                            );
                        if (i < 0) {
                            return -1;
                        }
                        if (data[dataOffset + ptrOut] == EXTENDED_ASCII) {
                            ptrOut++;
                        }
                        ptrOut++;
                    }
                }
            }
            if (ptrIn != textLength) {
                return -1;
            }
            if (!sizeFixed && (symbolIndex == text.Length - 1 || symbolIndex < 0)) {
                dataSize = int.MaxValue;
                for (int i = 0; i < dmSizes.Length; ++i) {
                    if (dmSizes[i].GetDataSize() >= dataOffset + ptrOut + (3 - pedi / 6)) {
                        dataSize = dmSizes[i].GetDataSize();
                        break;
                    }
                }
            }
            if (dataSize - dataOffset - ptrOut <= 2 && pedi >= 6) {
                if (pedi != 18 && ptrOut + 2 - pedi / 8 > dataLength) {
                    return -1;
                }
                if (pedi <= 12) {
                    byte val = (byte)((edi >> 18) & 0x3F);
                    if ((val & 0x20) == 0) {
                        val |= 0x40;
                    }
                    data[dataOffset + ptrOut++] = (byte)(val + 1);
                }
                if (pedi <= 6) {
                    byte val = (byte)((edi >> 12) & 0x3F);
                    if ((val & 0x20) == 0) {
                        val |= 0x40;
                    }
                    data[dataOffset + ptrOut++] = (byte)(val + 1);
                }
            }
            else {
                if (!ascii) {
                    edi |= ('_' & 0x3f) << pedi;
                    if (ptrOut + 3 - pedi / 8 > dataLength) {
                        return -1;
                    }
                    data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                    if (pedi <= 12) {
                        data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                    }
                    if (pedi <= 6) {
                        data[dataOffset + ptrOut++] = (byte)edi;
                    }
                }
            }
            return ptrOut + dataOffset - origDataOffset;
        }

        private int C40OrTextEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, 
            int dataLength, bool c40, int symbolIndex, int prevEnc, int origDataOffset) {
            int ptrIn;
            int ptrOut;
            int encPtr;
            int last0;
            int last1;
            int i;
            int a;
            int c;
            String basic;
            String shift2;
            String shift3;
            if (textLength == 0) {
                return 0;
            }
            ptrIn = 0;
            ptrOut = 0;
            shift2 = "!\"#$%&'()*+,-./:;<=>?@[\\]^_";
            if (c40) {
                basic = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                shift3 = "`abcdefghijklmnopqrstuvwxyz{|}~\xb1";
            }
            else {
                basic = " 0123456789abcdefghijklmnopqrstuvwxyz";
                shift3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\xb1";
            }
            bool addLatch = true;
            bool usingASCII = false;
            int mode = c40 ? DM_C40 : DM_TEXT;
            if (prevEnc == mode) {
                usingASCII = true;
                int latestModeEntry = symbolIndex - 1;
                while (latestModeEntry > 0 && switchMode[mode - 1][latestModeEntry] == mode) {
                    latestModeEntry--;
                }
                int unlatch = -1;
                int dataAmountOfEncodedWithASCII = 0;
                if (symbolIndex - latestModeEntry >= 5) {
                    for (i = symbolIndex - latestModeEntry; i > 0; i--) {
                        c = text[textOffset - i] & 0xff;
                        if (c > 127) {
                            dataAmountOfEncodedWithASCII += 2;
                        }
                        else {
                            dataAmountOfEncodedWithASCII++;
                        }
                    }
                    for (i = 1; i <= dataAmountOfEncodedWithASCII; i++) {
                        if (i > dataOffset) {
                            break;
                        }
                        if (data[dataOffset - i] == UNLATCH) {
                            unlatch = dataOffset - i;
                            break;
                        }
                    }
                    int amountOfEncodedWithASCII = 0;
                    if (unlatch >= 0) {
                        for (i = unlatch + 1; i < dataOffset; i++) {
                            if (data[i] == EXTENDED_ASCII) {
                                i++;
                            }
                            if (data[i] >= (byte)129 && data[i] <= (byte)229) {
                                amountOfEncodedWithASCII++;
                            }
                            amountOfEncodedWithASCII++;
                        }
                    }
                    else {
                        amountOfEncodedWithASCII = symbolIndex - latestModeEntry;
                    }
                    int dataOffsetNew = 0;
                    for (i = amountOfEncodedWithASCII; i > 0; i--) {
                        int requiredCapacityForASCII = 0;
                        int requiredCapacityForC40orText = 0;
                        for (int j = i; j >= 0; j--) {
                            c = text[textOffset - j] & 0xff;
                            if (c > 127) {
                                c -= 128;
                                requiredCapacityForC40orText += 2;
                            }
                            requiredCapacityForC40orText += basic.IndexOf((char)c) >= 0 ? 1 : 2;
                            if (c > 127) {
                                requiredCapacityForASCII += 2;
                            }
                            else {
                                if (j > 0 && IsDigit(c) && IsDigit(text[textOffset - j + 1] & 0xff)) {
                                    requiredCapacityForC40orText += basic.IndexOf((char)text[textOffset - j + 1]) >= 0 ? 1 : 2;
                                    j--;
                                    dataOffsetNew = requiredCapacityForASCII + 1;
                                }
                                requiredCapacityForASCII++;
                            }
                            if (j == 1) {
                                dataOffsetNew = requiredCapacityForASCII;
                            }
                        }
                        addLatch = (unlatch < 0) || ((dataOffset - requiredCapacityForASCII) != unlatch);
                        if (requiredCapacityForC40orText % 3 == 0 && requiredCapacityForC40orText / 3 * 2 + (addLatch ? 2 : 0) < requiredCapacityForASCII
                            ) {
                            usingASCII = false;
                            textLength = i + 1;
                            textOffset -= i;
                            dataOffset -= addLatch ? dataOffsetNew : dataOffsetNew + 1;
                            dataLength += addLatch ? dataOffsetNew : dataOffsetNew + 1;
                            break;
                        }
                        if (IsDigit(text[textOffset - i] & 0xff) && IsDigit(text[textOffset - i + 1] & 0xff)) {
                            i--;
                        }
                    }
                }
            }
            else {
                if (symbolIndex != -1) {
                    usingASCII = true;
                }
            }
            if (dataOffset < 0) {
                return -1;
            }
            if (usingASCII) {
                return AsciiEncodation(text, textOffset, 1, data, dataOffset, dataLength, prevEnc == mode ? 1 : -1, DM_ASCII
                    , origDataOffset);
            }
            if (addLatch) {
                data[dataOffset + ptrOut++] = c40 ? LATCH_C40 : LATCH_TEXT;
            }
            int[] enc = new int[textLength * 4 + 10];
            encPtr = 0;
            last0 = 0;
            last1 = 0;
            while (ptrIn < textLength) {
                if (encPtr % 3 == 0) {
                    last0 = ptrIn;
                    last1 = encPtr;
                }
                c = text[textOffset + ptrIn++] & 0xff;
                if (c > 127) {
                    c -= 128;
                    enc[encPtr++] = 1;
                    enc[encPtr++] = 30;
                }
                int idx = basic.IndexOf((char)c);
                if (idx >= 0) {
                    enc[encPtr++] = idx + 3;
                }
                else {
                    if (c < 32) {
                        enc[encPtr++] = 0;
                        enc[encPtr++] = c;
                    }
                    else {
                        if ((idx = shift2.IndexOf((char)c)) >= 0) {
                            enc[encPtr++] = 1;
                            enc[encPtr++] = idx;
                        }
                        else {
                            if ((idx = shift3.IndexOf((char)c)) >= 0) {
                                enc[encPtr++] = 2;
                                enc[encPtr++] = idx;
                            }
                        }
                    }
                }
            }
            if (encPtr % 3 != 0) {
                ptrIn = last0;
                encPtr = last1;
            }
            if (encPtr / 3 * 2 > dataLength - 2) {
                return -1;
            }
            i = 0;
            for (; i < encPtr; i += 3) {
                a = 1600 * enc[i] + 40 * enc[i + 1] + enc[i + 2] + 1;
                data[dataOffset + ptrOut++] = (byte)(a / 256);
                data[dataOffset + ptrOut++] = (byte)a;
            }
            if (dataLength - ptrOut > 2) {
                data[dataOffset + ptrOut++] = UNLATCH;
            }
            if (symbolIndex < 0 && textLength > ptrIn) {
                i = AsciiEncodation(text, textOffset + ptrIn, textLength - ptrIn, data, dataOffset + ptrOut, dataLength - 
                    ptrOut, -1, -1, origDataOffset);
                return i;
            }
            return ptrOut + dataOffset - origDataOffset;
        }

        private void SetBit(int x, int y, int xByte) {
            image[y * xByte + x / 8] |= (byte)(128 >> (x & 7));
        }

        private void Draw(byte[] data, int dataSize, DmParams dm) {
            int i;
            int j;
            int p;
            int x;
            int y;
            int xs;
            int ys;
            int z;
            int xByte = (dm.GetWidth() + ws * 2 + 7) / 8;
            JavaUtil.Fill(image, (byte)0);
            //alignment patterns
            //dotted horizontal line
            for (i = ws; i < dm.GetHeight() + ws; i += dm.GetHeightSection()) {
                for (j = ws; j < dm.GetWidth() + ws; j += 2) {
                    SetBit(j, i, xByte);
                }
            }
            //solid horizontal line
            for (i = dm.GetHeightSection() - 1 + ws; i < dm.GetHeight() + ws; i += dm.GetHeightSection()) {
                for (j = ws; j < dm.GetWidth() + ws; ++j) {
                    SetBit(j, i, xByte);
                }
            }
            //solid vertical line
            for (i = ws; i < dm.GetWidth() + ws; i += dm.GetWidthSection()) {
                for (j = ws; j < dm.GetHeight() + ws; ++j) {
                    SetBit(i, j, xByte);
                }
            }
            //dotted vertical line
            for (i = dm.GetWidthSection() - 1 + ws; i < dm.GetWidth() + ws; i += dm.GetWidthSection()) {
                for (j = 1 + ws; j < dm.GetHeight() + ws; j += 2) {
                    SetBit(i, j, xByte);
                }
            }
            p = 0;
            for (ys = 0; ys < dm.GetHeight(); ys += dm.GetHeightSection()) {
                for (y = 1; y < dm.GetHeightSection() - 1; ++y) {
                    for (xs = 0; xs < dm.GetWidth(); xs += dm.GetWidthSection()) {
                        for (x = 1; x < dm.GetWidthSection() - 1; ++x) {
                            z = place[p++];
                            if (z == 1 || z > 1 && (data[z / 8 - 1] & 0xff & 128 >> z % 8) != 0) {
                                SetBit(x + xs + ws, y + ys + ws, xByte);
                            }
                        }
                    }
                }
            }
        }

        private static int MinValueInColumn(int[][] array, int column) {
            int min = int.MaxValue;
            for (int i = 0; i < 6; i++) {
                if (array[i][column] < min && array[i][column] >= 0) {
                    min = array[i][column];
                }
            }
            return min != int.MaxValue ? min : -1;
        }

        private static int ValuePositionInColumn(int[][] array, int column, int value) {
            for (int i = 0; i < 6; i++) {
                if (array[i][column] == value) {
                    return i;
                }
            }
            return -1;
        }

        private void SolveFAndSwitchMode(int[] forMin, int mode, int currIndex) {
            if (forMin[mode] >= 0 && f[mode][currIndex - 1] >= 0) {
                f[mode][currIndex] = forMin[mode];
                switchMode[mode][currIndex] = mode + 1;
            }
            else {
                f[mode][currIndex] = int.MaxValue;
            }
            for (int i = 0; i < 6; i++) {
                if (forMin[i] < f[mode][currIndex] && forMin[i] >= 0 && f[i][currIndex - 1] >= 0) {
                    f[mode][currIndex] = forMin[i];
                    switchMode[mode][currIndex] = i + 1;
                }
            }
            if (f[mode][currIndex] == int.MaxValue) {
                f[mode][currIndex] = -1;
            }
        }

        private int GetEncodation(byte[] text, int textOffset, int textSize, byte[] data, int dataOffset, int dataSize
            , int options, bool sizeFixed) {
            int e;
            if (dataSize < 0) {
                return -1;
            }
            options &= 7;
            if (options == 0) {
                if (textSize == 0) {
                    return 0;
                }
                byte[][] dataDynamic = new byte[][] { new byte[data.Length], new byte[data.Length], new byte[data.Length], 
                    new byte[data.Length], new byte[data.Length], new byte[data.Length] };
                for (int i = 0; i < 6; i++) {
                    Array.Copy(data, 0, dataDynamic[i], 0, data.Length);
                    switchMode[i][0] = i + 1;
                }
                f[0][0] = AsciiEncodation(text, textOffset, 1, dataDynamic[0], dataOffset, dataSize, 0, -1, dataOffset);
                f[1][0] = C40OrTextEncodation(text, textOffset, 1, dataDynamic[1], dataOffset, dataSize, true, 0, -1, dataOffset
                    );
                f[2][0] = C40OrTextEncodation(text, textOffset, 1, dataDynamic[2], dataOffset, dataSize, false, 0, -1, dataOffset
                    );
                f[3][0] = B256Encodation(text, textOffset, 1, dataDynamic[3], dataOffset, dataSize, 0, -1, dataOffset);
                f[4][0] = X12Encodation(text, textOffset, 1, dataDynamic[4], dataOffset, dataSize, 0, -1, dataOffset);
                f[5][0] = EdifactEncodation(text, textOffset, 1, dataDynamic[5], dataOffset, dataSize, 0, -1, dataOffset, 
                    sizeFixed);
                for (int i = 1; i < textSize; i++) {
                    int[] tempForMin = new int[6];
                    for (int currEnc = 0; currEnc < 6; currEnc++) {
                        byte[][] dataDynamicInner = new byte[][] { new byte[data.Length], new byte[data.Length], new byte[data.Length
                            ], new byte[data.Length], new byte[data.Length], new byte[data.Length] };
                        for (int prevEnc = 0; prevEnc < 6; prevEnc++) {
                            Array.Copy(dataDynamic[prevEnc], 0, dataDynamicInner[prevEnc], 0, data.Length);
                            if (f[prevEnc][i - 1] < 0) {
                                tempForMin[prevEnc] = -1;
                            }
                            else {
                                if (currEnc == 0) {
                                    tempForMin[prevEnc] = AsciiEncodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i - 1
                                        ] + dataOffset, dataSize - f[prevEnc][i - 1], i, prevEnc + 1, dataOffset);
                                }
                                if (currEnc == 1) {
                                    tempForMin[prevEnc] = C40OrTextEncodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i
                                         - 1] + dataOffset, dataSize - f[prevEnc][i - 1], true, i, prevEnc + 1, dataOffset);
                                }
                                if (currEnc == 2) {
                                    tempForMin[prevEnc] = C40OrTextEncodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i
                                         - 1] + dataOffset, dataSize - f[prevEnc][i - 1], false, i, prevEnc + 1, dataOffset);
                                }
                                if (currEnc == 3) {
                                    tempForMin[prevEnc] = B256Encodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i - 1]
                                         + dataOffset, dataSize - f[prevEnc][i - 1], i, prevEnc + 1, dataOffset);
                                }
                                if (currEnc == 4) {
                                    tempForMin[prevEnc] = X12Encodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i - 1] 
                                        + dataOffset, dataSize - f[prevEnc][i - 1], i, prevEnc + 1, dataOffset);
                                }
                                if (currEnc == 5) {
                                    tempForMin[prevEnc] = EdifactEncodation(text, textOffset + i, 1, dataDynamicInner[prevEnc], f[prevEnc][i -
                                         1] + dataOffset, dataSize - f[prevEnc][i - 1], i, prevEnc + 1, dataOffset, sizeFixed);
                                }
                            }
                        }
                        SolveFAndSwitchMode(tempForMin, currEnc, i);
                        if (switchMode[currEnc][i] != 0) {
                            Array.Copy(dataDynamicInner[switchMode[currEnc][i] - 1], 0, dataDynamic[currEnc], 0, data.Length);
                        }
                    }
                }
                e = MinValueInColumn(f, textSize - 1);
                if (e > dataSize || e < 0) {
                    return -1;
                }
                int bestDataDynamicResultIndex = ValuePositionInColumn(f, textSize - 1, e);
                Array.Copy(dataDynamic[bestDataDynamicResultIndex], 0, data, 0, data.Length);
                return e;
            }
            switch (options) {
                case DM_ASCII: {
                    return AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize, -1, -1, dataOffset);
                }

                case DM_C40: {
                    return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true, -1, -1, dataOffset
                        );
                }

                case DM_TEXT: {
                    return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false, -1, -1, dataOffset
                        );
                }

                case DM_B256: {
                    return B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize, -1, -1, dataOffset);
                }

                case DM_X12: {
                    return X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize, -1, -1, dataOffset);
                }

                case DM_EDIFACT: {
                    return EdifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize, -1, -1, dataOffset, sizeFixed
                        );
                }

                case DM_RAW: {
                    if (textSize > dataSize) {
                        return -1;
                    }
                    Array.Copy(text, textOffset, data, dataOffset, textSize);
                    return textSize;
                }
            }
            return -1;
        }

        private static int GetNumber(byte[] text, int ptrIn, int n) {
            int v;
            int j;
            int c;
            v = 0;
            for (j = 0; j < n; ++j) {
                c = text[ptrIn++] & 0xff;
                if (c < '0' || c > '9') {
                    return -1;
                }
                v = v * 10 + c - '0';
            }
            return v;
        }

        private int ProcessExtensions(byte[] text, int textOffset, int textSize, byte[] data) {
            int order;
            int ptrIn;
            int ptrOut;
            int eci;
            int fn;
            int ft;
            int fi;
            int c;
            if ((options & DM_EXTENSION) == 0) {
                return 0;
            }
            order = 0;
            ptrIn = 0;
            ptrOut = 0;
            while (ptrIn < textSize) {
                if (order > 20) {
                    return -1;
                }
                c = text[textOffset + ptrIn++] & 0xff;
                ++order;
                switch (c) {
                    case '.': {
                        extOut = ptrIn;
                        return ptrOut;
                    }

                    case 'e': {
                        if (ptrIn + 6 > textSize) {
                            return -1;
                        }
                        eci = GetNumber(text, textOffset + ptrIn, 6);
                        if (eci < 0) {
                            return -1;
                        }
                        ptrIn += 6;
                        data[ptrOut++] = (byte)241;
                        if (eci < 127) {
                            data[ptrOut++] = (byte)(eci + 1);
                        }
                        else {
                            if (eci < 16383) {
                                data[ptrOut++] = (byte)((eci - 127) / 254 + 128);
                                data[ptrOut++] = (byte)((eci - 127) % 254 + 1);
                            }
                            else {
                                data[ptrOut++] = (byte)((eci - 16383) / 64516 + 192);
                                data[ptrOut++] = (byte)((eci - 16383) / 254 % 254 + 1);
                                data[ptrOut++] = (byte)((eci - 16383) % 254 + 1);
                            }
                        }
                        break;
                    }

                    case 's': {
                        if (order != 1) {
                            return -1;
                        }
                        if (ptrIn + 9 > textSize) {
                            return -1;
                        }
                        fn = GetNumber(text, textOffset + ptrIn, 2);
                        if (fn <= 0 || fn > 16) {
                            return -1;
                        }
                        ptrIn += 2;
                        ft = GetNumber(text, textOffset + ptrIn, 2);
                        if (ft <= 1 || ft > 16) {
                            return -1;
                        }
                        ptrIn += 2;
                        fi = GetNumber(text, textOffset + ptrIn, 5);
                        if (fi < 0 || fn >= 64516) {
                            return -1;
                        }
                        ptrIn += 5;
                        data[ptrOut++] = (byte)233;
                        data[ptrOut++] = (byte)(fn - 1 << 4 | 17 - ft);
                        data[ptrOut++] = (byte)(fi / 254 + 1);
                        data[ptrOut++] = (byte)(fi % 254 + 1);
                        break;
                    }

                    case 'p': {
                        if (order != 1) {
                            return -1;
                        }
                        data[ptrOut++] = (byte)234;
                        break;
                    }

                    case 'm': {
                        if (order != 1) {
                            return -1;
                        }
                        if (ptrIn + 1 > textSize) {
                            return -1;
                        }
                        c = text[textOffset + ptrIn++] & 0xff;
                        if (c != '5') {
                            return -1;
                        }
                        data[ptrOut++] = (byte)234;
                        data[ptrOut++] = (byte)236;
                        break;
                    }

                    case 'f': {
                        if (order != 1 && (order != 2 || text[textOffset] != 's' && text[textOffset] != 'm')) {
                            return -1;
                        }
                        data[ptrOut++] = (byte)232;
                        break;
                    }
                }
            }
            return -1;
        }
    }
}
