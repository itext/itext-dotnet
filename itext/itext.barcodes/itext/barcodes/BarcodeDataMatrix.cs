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
using iText.Barcodes.Dmcode;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Barcodes {
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

        /// <summary>X21 encodation.</summary>
        public const int DM_X21 = 5;

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

        /// <summary>Creates an instance of this class.</summary>
        public BarcodeDataMatrix() {
            encoding = DEFAULT_DATA_MATRIX_ENCODING;
        }

        public BarcodeDataMatrix(String code) {
            encoding = DEFAULT_DATA_MATRIX_ENCODING;
            SetCode(code);
        }

        public BarcodeDataMatrix(String code, String encoding) {
            this.encoding = encoding;
            SetCode(code);
        }

        public override Rectangle GetBarcodeSize() {
            return new Rectangle(0, 0, width + 2 * ws, height + 2 * ws);
        }

        public override Rectangle PlaceBarcode(PdfCanvas canvas, iText.Kernel.Color.Color foreground) {
            return PlaceBarcode(canvas, foreground, DEFAULT_MODULE_SIZE);
        }

        public override PdfFormXObject CreateFormXObject(iText.Kernel.Color.Color foreground, PdfDocument document
            ) {
            return CreateFormXObject(foreground, DEFAULT_MODULE_SIZE, document);
        }

        /// <summary>Creates a PdfFormXObject with the barcode with given module width and module height.</summary>
        /// <param name="foreground">the color of the pixels. It can be <CODE>null</CODE></param>
        /// <param name="moduleSide">the side (width and height) of the pixels.</param>
        /// <returns>the XObject.</returns>
        public virtual PdfFormXObject CreateFormXObject(iText.Kernel.Color.Color foreground, float moduleSide, PdfDocument
             document) {
            PdfFormXObject xObject = new PdfFormXObject((Rectangle)null);
            Rectangle rect = PlaceBarcode(new PdfCanvas(xObject, document), foreground, moduleSide);
            xObject.SetBBox(new PdfArray(rect));
            return xObject;
        }

        public virtual Rectangle PlaceBarcode(PdfCanvas canvas, iText.Kernel.Color.Color foreground, float moduleSide
            ) {
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

        // AWT related methods (remove this if you port to Android / GAE)
        /// <summary>Gets the barcode size</summary>
        public virtual Rectangle GetBarcodeSize(float moduleHeight, float moduleWidth) {
            return new Rectangle(0, 0, (width + 2 * ws) * moduleHeight, (height + 2 * ws) * moduleWidth);
        }

        /// <summary>Creates a barcode.</summary>
        /// <remarks>Creates a barcode. The <CODE>String</CODE> is interpreted with the ISO-8859-1 encoding</remarks>
        /// <param name="text">the text</param>
        /// <returns>
        /// the status of the generation. It can be one of this values:
        /// <p/>
        /// <CODE>DM_NO_ERROR</CODE> - no error.<br />
        /// <CODE>DM_ERROR_TEXT_TOO_BIG</CODE> - the text is too big for the symbology capabilities.<br />
        /// <CODE>DM_ERROR_INVALID_SQUARE</CODE> - the dimensions given for the symbol are illegal.<br />
        /// <CODE>DM_ERROR_EXTENSION</CODE> - an error was while parsing an extension.
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
        /// <p/>
        /// <CODE>DM_NO_ERROR</CODE> - no error.<br />
        /// <CODE>DM_ERROR_TEXT_TOO_BIG</CODE> - the text is too big for the symbology capabilities.<br />
        /// <CODE>DM_ERROR_INVALID_SQUARE</CODE> - the dimensions given for the symbol are illegal.<br />
        /// <CODE>DM_ERROR_EXTENSION</CODE> - an error was while parsing an extension.
        /// </returns>
        public virtual int SetCode(byte[] text, int textOffset, int textSize) {
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
            if (height == 0 || width == 0) {
                last = dmSizes[dmSizes.Length - 1];
                e = GetEncodation(text, textOffset + extOut, textSize - extOut, data, extCount, last.dataSize - extCount, 
                    options, false);
                if (e < 0) {
                    return DM_ERROR_TEXT_TOO_BIG;
                }
                e += extCount;
                for (k = 0; k < dmSizes.Length; ++k) {
                    if (dmSizes[k].dataSize >= e) {
                        break;
                    }
                }
                dm = dmSizes[k];
                height = dm.height;
                width = dm.width;
            }
            else {
                for (k = 0; k < dmSizes.Length; ++k) {
                    if (height == dmSizes[k].height && width == dmSizes[k].width) {
                        break;
                    }
                }
                if (k == dmSizes.Length) {
                    return DM_ERROR_INVALID_SQUARE;
                }
                dm = dmSizes[k];
                e = GetEncodation(text, textOffset + extOut, textSize - extOut, data, extCount, dm.dataSize - extCount, options
                    , true);
                if (e < 0) {
                    return DM_ERROR_TEXT_TOO_BIG;
                }
                e += extCount;
            }
            if ((options & DM_TEST) != 0) {
                return DM_NO_ERROR;
            }
            image = new byte[(dm.width + 2 * ws + 7) / 8 * (dm.height + 2 * ws)];
            MakePadding(data, e, dm.dataSize - e);
            place = Placement.DoPlacement(dm.height - dm.height / dm.heightSection * 2, dm.width - dm.width / dm.widthSection
                 * 2);
            full = dm.dataSize + (dm.dataSize + 2) / dm.dataBlock * dm.errorBlock;
            ReedSolomon.GenerateECC(data, dm.dataSize, dm.dataBlock, dm.errorBlock);
            Draw(data, full, dm);
            return DM_NO_ERROR;
        }

        /// <summary>Gets the height of the barcode.</summary>
        /// <remarks>
        /// Gets the height of the barcode. Will contain the real height used after a successful call
        /// to <CODE>generate()</CODE>. This height doesn't include the whitespace border, if any.
        /// </remarks>
        /// <returns>the height of the barcode</returns>
        public virtual int GetHeight() {
            return height;
        }

        /// <summary>Sets the height of the barcode.</summary>
        /// <remarks>
        /// Sets the height of the barcode. If the height is zero it will be calculated. This height doesn't include the whitespace border, if any.
        /// <p/>
        /// The allowed dimensions are (height, width):<p>
        /// 10, 10<br />
        /// 12, 12<br />
        /// 8, 18<br />
        /// 14, 14<br />
        /// 8, 32<br />
        /// 16, 16<br />
        /// 12, 26<br />
        /// 18, 18<br />
        /// 20, 20<br />
        /// 12, 36<br />
        /// 22, 22<br />
        /// 16, 36<br />
        /// 24, 24<br />
        /// 26, 26<br />
        /// 16, 48<br />
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
        /// to <CODE>generate()</CODE>. This width doesn't include the whitespace border, if any.
        /// </remarks>
        /// <returns>the width of the barcode</returns>
        public virtual int GetWidth() {
            return width;
        }

        /// <summary>Sets the width of the barcode.</summary>
        /// <remarks>
        /// Sets the width of the barcode. If the width is zero it will be calculated. This width doesn't include the whitespace border, if any.
        /// <p/>
        /// The allowed dimensions are (height, width):<p>
        /// 10, 10<br />
        /// 12, 12<br />
        /// 8, 18<br />
        /// 14, 14<br />
        /// 8, 32<br />
        /// 16, 16<br />
        /// 12, 26<br />
        /// 18, 18<br />
        /// 20, 20<br />
        /// 12, 36<br />
        /// 22, 22<br />
        /// 16, 36<br />
        /// 24, 24<br />
        /// 26, 26<br />
        /// 16, 48<br />
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
        /// Sets the options for the barcode generation. The options can be:<p>
        /// One of:<br />
        /// <CODE>DM_AUTO</CODE> - the best encodation will be used<br />
        /// <CODE>DM_ASCII</CODE> - ASCII encodation<br />
        /// <CODE>DM_C40</CODE> - C40 encodation<br />
        /// <CODE>DM_TEXT</CODE> - TEXT encodation<br />
        /// <CODE>DM_B256</CODE> - binary encodation<br />
        /// <CODE>DM_X21</CODE> - X21 encodation<br />
        /// <CODE>DM_EDIFACT</CODE> - EDIFACT encodation<br />
        /// <CODE>DM_RAW</CODE> - no encodation. The bytes provided are already encoded and will be added directly to the barcode, using padding if needed. It assumes that the encodation state is left at ASCII after the last byte.<br />
        /// <p/>
        /// One of:<br />
        /// <CODE>DM_EXTENSION</CODE> - allows extensions to be embedded at the start of the text:<p>
        /// exxxxxx - ECI number xxxxxx<br />
        /// m5 - macro 5<br />
        /// m6 - macro 6<br />
        /// f - FNC1<br />
        /// saabbccccc - Structured Append, aa symbol position (1-16), bb total number of symbols (2-16), ccccc file identification (0-64515)<br />
        /// p - Reader programming<br />
        /// . - extension terminator<p>
        /// Example for a structured append, symbol 2 of 6, with FNC1 and ECI 000005. The actual text is "Hello".<p>
        /// s020600075fe000005.Hello<p>
        /// One of:<br />
        /// <CODE>DM_TEST</CODE> - doesn't generate the image but returns all the other information.
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

        public virtual String GetEncoding() {
            return encoding;
        }

        private static void MakePadding(byte[] data, int position, int count) {
            //already in ascii mode
            if (count <= 0) {
                return;
            }
            data[position++] = (byte)129;
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

        private static int AsciiEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset
            , int dataLength) {
            int ptrIn;
            int ptrOut;
            int c;
            ptrIn = textOffset;
            ptrOut = dataOffset;
            textLength += textOffset;
            dataLength += dataOffset;
            while (ptrIn < textLength) {
                if (ptrOut >= dataLength) {
                    return -1;
                }
                c = text[ptrIn++] & 0xff;
                if (IsDigit(c) && ptrIn < textLength && IsDigit(text[ptrIn] & 0xff)) {
                    data[ptrOut++] = (byte)((c - '0') * 10 + (text[ptrIn++] & 0xff) - '0' + 130);
                }
                else {
                    if (c > 127) {
                        if (ptrOut + 1 >= dataLength) {
                            return -1;
                        }
                        data[ptrOut++] = (byte)235;
                        data[ptrOut++] = (byte)(c - 128 + 1);
                    }
                    else {
                        data[ptrOut++] = (byte)(c + 1);
                    }
                }
            }
            return ptrOut - dataOffset;
        }

        private static int B256Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset
            , int dataLength) {
            int k;
            int j;
            int prn;
            int tv;
            int c;
            if (textLength == 0) {
                return 0;
            }
            if (textLength < 250 && textLength + 2 > dataLength) {
                return -1;
            }
            if (textLength >= 250 && textLength + 3 > dataLength) {
                return -1;
            }
            data[dataOffset] = (byte)231;
            if (textLength < 250) {
                data[dataOffset + 1] = (byte)textLength;
                k = 2;
            }
            else {
                data[dataOffset + 1] = (byte)(textLength / 250 + 249);
                data[dataOffset + 2] = (byte)(textLength % 250);
                k = 3;
            }
            System.Array.Copy(text, textOffset, data, k + dataOffset, textLength);
            k += textLength + dataOffset;
            for (j = dataOffset + 1; j < k; ++j) {
                c = data[j] & 0xff;
                prn = 149 * (j + 1) % 255 + 1;
                tv = c + prn;
                if (tv > 255) {
                    tv -= 256;
                }
                data[j] = (byte)tv;
            }
            return k - dataOffset;
        }

        private static int X12Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, 
            int dataLength) {
            int ptrIn;
            int ptrOut;
            int count;
            int k;
            int n;
            int ci;
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
                if (ptrOut >= dataLength) {
                    break;
                }
                if (c < 40) {
                    if (ptrIn == 0 || ptrIn > 0 && x[ptrIn - 1] > 40) {
                        data[dataOffset + ptrOut++] = (byte)238;
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
                    if (ptrIn > 0 && x[ptrIn - 1] < 40) {
                        data[dataOffset + ptrOut++] = (byte)254;
                    }
                    ci = text[ptrIn + textOffset] & 0xff;
                    if (ci > 127) {
                        data[dataOffset + ptrOut++] = (byte)235;
                        ci -= 128;
                    }
                    if (ptrOut >= dataLength) {
                        break;
                    }
                    data[dataOffset + ptrOut++] = (byte)(ci + 1);
                }
            }
            c = 100;
            if (textLength > 0) {
                c = x[textLength - 1];
            }
            if (ptrIn != textLength || c < 40 && ptrOut >= dataLength) {
                return -1;
            }
            if (c < 40) {
                data[dataOffset + ptrOut++] = (byte)254;
            }
            return ptrOut;
        }

        private static int EdifactEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset
            , int dataLength) {
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
            for (; ptrIn < textLength; ++ptrIn) {
                c = text[ptrIn + textOffset] & 0xff;
                if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_') {
                    if (ascii) {
                        if (ptrOut + 1 > dataLength) {
                            break;
                        }
                        data[dataOffset + ptrOut++] = (byte)240;
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
                    if (c > 127) {
                        if (ptrOut >= dataLength) {
                            break;
                        }
                        data[dataOffset + ptrOut++] = (byte)235;
                        c -= 128;
                    }
                    if (ptrOut >= dataLength) {
                        break;
                    }
                    data[dataOffset + ptrOut++] = (byte)(c + 1);
                }
            }
            if (ptrIn != textLength) {
                return -1;
            }
            int dataSize = int.MaxValue;
            for (int i = 0; i < dmSizes.Length; ++i) {
                if (dmSizes[i].dataSize >= dataOffset + ptrOut + (3 - pedi / 6)) {
                    dataSize = dmSizes[i].dataSize;
                    break;
                }
            }
            if (dataSize - dataOffset - ptrOut <= 2 && pedi >= 6) {
                //have to write up to 2 bytes and up to 2 symbols
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
            return ptrOut;
        }

        private static int C40OrTextEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset
            , int dataLength, bool c40) {
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
            if (c40) {
                data[dataOffset + ptrOut++] = (byte)230;
            }
            else {
                data[dataOffset + ptrOut++] = (byte)239;
            }
            shift2 = "!\"#$%&'()*+,-./:;<=>?@[\\]^_";
            if (c40) {
                basic = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                shift3 = "`abcdefghijklmnopqrstuvwxyz{|}~\xb1";
            }
            else {
                basic = " 0123456789abcdefghijklmnopqrstuvwxyz";
                shift3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\xb1";
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
            data[ptrOut++] = (byte)254;
            i = AsciiEncodation(text, ptrIn, textLength - ptrIn, data, ptrOut, dataLength - ptrOut);
            if (i < 0) {
                return i;
            }
            return ptrOut + i;
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
            int xByte = (dm.width + ws * 2 + 7) / 8;
            iText.IO.Util.JavaUtil.Fill(image, (byte)0);
            //alignment patterns
            //dotted horizontal line
            for (i = ws; i < dm.height + ws; i += dm.heightSection) {
                for (j = ws; j < dm.width + ws; j += 2) {
                    SetBit(j, i, xByte);
                }
            }
            //solid horizontal line
            for (i = dm.heightSection - 1 + ws; i < dm.height + ws; i += dm.heightSection) {
                for (j = ws; j < dm.width + ws; ++j) {
                    SetBit(j, i, xByte);
                }
            }
            //solid vertical line
            for (i = ws; i < dm.width + ws; i += dm.widthSection) {
                for (j = ws; j < dm.height + ws; ++j) {
                    SetBit(i, j, xByte);
                }
            }
            //dotted vertical line
            for (i = dm.widthSection - 1 + ws; i < dm.width + ws; i += dm.widthSection) {
                for (j = 1 + ws; j < dm.height + ws; j += 2) {
                    SetBit(i, j, xByte);
                }
            }
            p = 0;
            for (ys = 0; ys < dm.height; ys += dm.heightSection) {
                for (y = 1; y < dm.heightSection - 1; ++y) {
                    for (xs = 0; xs < dm.width; xs += dm.widthSection) {
                        for (x = 1; x < dm.widthSection - 1; ++x) {
                            z = place[p++];
                            if (z == 1 || z > 1 && (data[z / 8 - 1] & 0xff & 128 >> z % 8) != 0) {
                                SetBit(x + xs + ws, y + ys + ws, xByte);
                            }
                        }
                    }
                }
            }
        }

        private static int GetEncodation(byte[] text, int textOffset, int textSize, byte[] data, int dataOffset, int
             dataSize, int options, bool firstMatch) {
            int e;
            int j;
            int k;
            int[] e1 = new int[6];
            if (dataSize < 0) {
                return -1;
            }
            e = -1;
            options &= 7;
            if (options == 0) {
                e1[0] = AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
                if (firstMatch && e1[0] >= 0) {
                    return e1[0];
                }
                e1[1] = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
                if (firstMatch && e1[1] >= 0) {
                    return e1[1];
                }
                e1[2] = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
                if (firstMatch && e1[2] >= 0) {
                    return e1[2];
                }
                e1[3] = B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                if (firstMatch && e1[3] >= 0) {
                    return e1[3];
                }
                e1[4] = X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                if (firstMatch && e1[4] >= 0) {
                    return e1[4];
                }
                e1[5] = EdifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
                if (firstMatch && e1[5] >= 0) {
                    return e1[5];
                }
                if (e1[0] < 0 && e1[1] < 0 && e1[2] < 0 && e1[3] < 0 && e1[4] < 0 && e1[5] < 0) {
                    return -1;
                }
                j = 0;
                e = 99999;
                for (k = 0; k < 6; ++k) {
                    if (e1[k] >= 0 && e1[k] < e) {
                        e = e1[k];
                        j = k;
                    }
                }
                if (j == 0) {
                    e = AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
                }
                else {
                    if (j == 1) {
                        e = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
                    }
                    else {
                        if (j == 2) {
                            e = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
                        }
                        else {
                            if (j == 3) {
                                e = B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                            }
                            else {
                                if (j == 4) {
                                    e = X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                                }
                            }
                        }
                    }
                }
                return e;
            }
            switch (options) {
                case DM_ASCII: {
                    return AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
                }

                case DM_C40: {
                    return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
                }

                case DM_TEXT: {
                    return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
                }

                case DM_B256: {
                    return B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                }

                case DM_X21: {
                    return X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
                }

                case DM_EDIFACT: {
                    return EdifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
                }

                case DM_RAW: {
                    if (textSize > dataSize) {
                        return -1;
                    }
                    System.Array.Copy(text, textOffset, data, dataOffset, textSize);
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
                        if (c != '5' && c != '5') {
                            return -1;
                        }
                        data[ptrOut++] = (byte)234;
                        data[ptrOut++] = (byte)(c == '5' ? 236 : 237);
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
