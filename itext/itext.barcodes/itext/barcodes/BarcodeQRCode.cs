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
using System;
using System.Collections.Generic;
using iText.Barcodes.Exceptions;
using iText.Barcodes.Qrcode;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Barcodes {
    /// <summary>A QRCode implementation based on the zxing code.</summary>
    public class BarcodeQRCode : Barcode2D {
        internal ByteMatrix bm;

        /// <summary>modifiers to change the way the barcode is create.</summary>
        internal IDictionary<EncodeHintType, Object> hints;

        internal String code;

        /// <summary>Creates the QR barcode.</summary>
        /// <param name="code">the text to be encoded</param>
        /// <param name="hints">barcode hints. See #setHints for description.</param>
        public BarcodeQRCode(String code, IDictionary<EncodeHintType, Object> hints) {
            this.code = code;
            this.hints = hints;
            Regenerate();
        }

        /// <summary>
        /// Creates the QR barcode with default error correction level (ErrorCorrectionLevel.L)
        /// and default character set (ISO-8859-1).
        /// </summary>
        /// <param name="content">the text to be encoded</param>
        public BarcodeQRCode(String content)
            : this(content, null) {
        }

        public BarcodeQRCode() {
        }

        /// <summary>Gets the current data.</summary>
        /// <returns>the encoded data</returns>
        public virtual String GetCode() {
            return code;
        }

        /// <summary>Sets the data to be encoded by the barcode.</summary>
        /// <remarks>Sets the data to be encoded by the barcode. If not specified in hints otherwise, the character set should be ISO-8859-1.
        ///     </remarks>
        /// <param name="code">The data to encode</param>
        public virtual void SetCode(String code) {
            this.code = code;
            Regenerate();
        }

        /// <returns>modifiers to change the way the barcode is created.</returns>
        public virtual IDictionary<EncodeHintType, Object> GetHints() {
            return hints;
        }

        /// <param name="hints">
        /// modifiers to change the way the barcode is created. They can be EncodeHintType.ERROR_CORRECTION
        /// and EncodeHintType.CHARACTER_SET. For EncodeHintType.ERROR_CORRECTION the values can be ErrorCorrectionLevel.L, M, Q, H.
        /// For EncodeHintType.CHARACTER_SET the values are strings and can be Cp437, Shift_JIS and ISO-8859-1 to ISO-8859-16.
        /// You can also use UTF-8, but correct behaviour is not guaranteed as Unicode is not supported in QRCodes.
        /// The default value is ISO-8859-1.
        /// </param>
        public virtual void SetHints(IDictionary<EncodeHintType, Object> hints) {
            this.hints = hints;
            Regenerate();
        }

        /// <summary>Regenerates barcode after changes in hints or code.</summary>
        public virtual void Regenerate() {
            if (code != null) {
                try {
                    QRCodeWriter qc = new QRCodeWriter();
                    bm = qc.Encode(code, 1, 1, hints);
                }
                catch (WriterException ex) {
                    throw new ArgumentException(ex.Message, ex.InnerException);
                }
            }
        }

        /// <summary>Gets the size of the barcode grid</summary>
        public override Rectangle GetBarcodeSize() {
            return new Rectangle(0, 0, bm.GetWidth(), bm.GetHeight());
        }

        /// <summary>Gets the barcode size</summary>
        /// <param name="moduleSize">The module size</param>
        /// <returns>The size of the barcode</returns>
        public virtual Rectangle GetBarcodeSize(float moduleSize) {
            return new Rectangle(0, 0, bm.GetWidth() * moduleSize, bm.GetHeight() * moduleSize);
        }

        public override Rectangle PlaceBarcode(PdfCanvas canvas, Color foreground) {
            return PlaceBarcode(canvas, foreground, DEFAULT_MODULE_SIZE);
        }

        /// <summary>* Places the barcode in a <c>PdfCanvas</c>.</summary>
        /// <remarks>
        /// * Places the barcode in a <c>PdfCanvas</c>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.
        /// </remarks>
        /// <param name="canvas">the <c>PdfCanvas</c> where the barcode will be placed</param>
        /// <param name="foreground">the foreground color. It can be <c>null</c></param>
        /// <param name="moduleSide">the size of the square grid cell</param>
        /// <returns>the dimensions the barcode occupies</returns>
        public virtual Rectangle PlaceBarcode(PdfCanvas canvas, Color foreground, float moduleSide) {
            int width = bm.GetWidth();
            int height = bm.GetHeight();
            byte[][] mt = bm.GetArray();
            if (foreground != null) {
                canvas.SetFillColor(foreground);
            }
            for (int y = 0; y < height; ++y) {
                byte[] line = mt[y];
                for (int x = 0; x < width; ++x) {
                    if (line[x] == 0) {
                        canvas.Rectangle(x * moduleSide, (height - y - 1) * moduleSide, moduleSide, moduleSide);
                    }
                }
            }
            canvas.Fill();
            return GetBarcodeSize(moduleSide);
        }

        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <param name="foreground">the color of the pixels. It can be <c>null</c></param>
        /// <returns>the XObject.</returns>
        public override PdfFormXObject CreateFormXObject(Color foreground, PdfDocument document) {
            return CreateFormXObject(foreground, DEFAULT_MODULE_SIZE, document);
        }

        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <param name="foreground">The color of the pixels. It can be <c>null</c></param>
        /// <param name="moduleSize">The size of the pixels.</param>
        /// <param name="document">The document</param>
        /// <returns>the XObject.</returns>
        public virtual PdfFormXObject CreateFormXObject(Color foreground, float moduleSize, PdfDocument document) {
            PdfFormXObject xObject = new PdfFormXObject((Rectangle)null);
            Rectangle rect = PlaceBarcode(new PdfCanvas(xObject, document), foreground, moduleSize);
            xObject.SetBBox(new PdfArray(rect));
            return xObject;
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        private byte[] GetBitMatrix() {
            int width = bm.GetWidth();
            int height = bm.GetHeight();
            int stride = (width + 7) / 8;
            byte[] b = new byte[stride * height];
            byte[][] mt = bm.GetArray();
            for (int y = 0; y < height; ++y) {
                byte[] line = mt[y];
                for (int x = 0; x < width; ++x) {
                    if (line[x] != 0) {
                        int offset = stride * y + x / 8;
                        b[offset] |= (byte)(0x80 >> (x % 8));
                    }
                }
            }
            return b;
        }
    }
}
