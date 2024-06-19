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
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Font {
    /// <summary>The content where Type3 glyphs are written to.</summary>
    public sealed class Type3Glyph : PdfCanvas {
        private const String D_0_STR = "d0\n";

        private const String D_1_STR = "d1\n";

        private static readonly byte[] d0 = ByteUtils.GetIsoBytes(D_0_STR);

        private static readonly byte[] d1 = ByteUtils.GetIsoBytes(D_1_STR);

        private float wx;

        private float llx;

        private float lly;

        private float urx;

        private float ury;

        private bool isColor = false;

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a Type3Glyph canvas with a new Content Stream.</summary>
        /// <param name="pdfDocument">the document that this canvas is created for</param>
        internal Type3Glyph(PdfDocument pdfDocument, float wx, float llx, float lly, float urx, float ury, bool isColor
            )
            : base((PdfStream)new PdfStream().MakeIndirect(pdfDocument), null, pdfDocument) {
            WriteMetrics(wx, llx, lly, urx, ury, isColor);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a Type3Glyph canvas with a non-empty Content Stream.</summary>
        /// <param name="pdfStream">
        /// 
        /// <c>PdfStream</c>
        /// from existed document.
        /// </param>
        /// <param name="document">
        /// document to which
        /// <c>PdfStream</c>
        /// belongs.
        /// </param>
        internal Type3Glyph(PdfStream pdfStream, PdfDocument document)
            : base(pdfStream, null, document) {
            if (pdfStream.GetBytes() != null) {
                FillBBFromBytes(pdfStream.GetBytes());
            }
        }
//\endcond

        public float GetWx() {
            return wx;
        }

        public float GetLlx() {
            return llx;
        }

        public float GetLly() {
            return lly;
        }

        public float GetUrx() {
            return urx;
        }

        public float GetUry() {
            return ury;
        }

        /// <summary>Indicates if the glyph color specified in the glyph description or not.</summary>
        /// <returns>whether the glyph color is specified in the glyph description or not</returns>
        public bool IsColor() {
            return isColor;
        }

        /// <summary>Writes the width and optionally the bounding box parameters for a glyph</summary>
        /// <param name="wx">the advance this character will have</param>
        /// <param name="llx">
        /// the X lower left corner of the glyph bounding box. If the <c>isColor</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="lly">
        /// the Y lower left corner of the glyph bounding box. If the <c>isColor</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="urx">
        /// the X upper right corner of the glyph bounding box. If the <c>isColor</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="ury">
        /// the Y upper right corner of the glyph bounding box. If the <c>isColor</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="isColor">
        /// defines whether the glyph color is specified in the glyph description in the font.
        /// The consequence of value <c>true</c> is that the bounding box parameters are ignored.
        /// </param>
        private void WriteMetrics(float wx, float llx, float lly, float urx, float ury, bool isColor) {
            this.isColor = isColor;
            this.wx = wx;
            this.llx = llx;
            this.lly = lly;
            this.urx = urx;
            this.ury = ury;
            if (isColor) {
                contentStream.GetOutputStream().WriteFloat(wx).WriteSpace()
                                //wy
                                .WriteFloat(0).WriteSpace().WriteBytes(d0);
            }
            else {
                contentStream.GetOutputStream().WriteFloat(wx).WriteSpace()
                                //wy
                                .WriteFloat(0).WriteSpace().WriteFloat(llx).WriteSpace().WriteFloat(lly).WriteSpace().WriteFloat(urx).WriteSpace
                    ().WriteFloat(ury).WriteSpace().WriteBytes(d1);
            }
        }

        /// <summary>Creates Image XObject from image and adds it to canvas.</summary>
        /// <remarks>
        /// Creates Image XObject from image and adds it to canvas. Performs additional checks to make
        /// sure that we only add mask images to not colorized type 3 fonts.
        /// </remarks>
        /// <param name="image">
        /// the
        /// <c>PdfImageXObject</c>
        /// object
        /// </param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <param name="inlineImage">true if to add image as in-line.</param>
        /// <returns>created Image XObject or null in case of in-line image (asInline = true).</returns>
        public override PdfXObject AddImageWithTransformationMatrix(ImageData image, float a, float b, float c, float
             d, float e, float f, bool inlineImage) {
            if (!isColor && (!image.IsMask() || !(image.GetBpc() == 1 || image.GetBpc() > 0xff))) {
                throw new PdfException("Not colorized type3 fonts accept only mask images.");
            }
            return base.AddImageWithTransformationMatrix(image, a, b, c, d, e, f, inlineImage);
        }

        private void FillBBFromBytes(byte[] bytes) {
            String str = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            int d0Pos = str.IndexOf(D_0_STR, StringComparison.Ordinal);
            int d1Pos = str.IndexOf(D_1_STR, StringComparison.Ordinal);
            if (d0Pos != -1) {
                isColor = true;
                String[] bbArray = iText.Commons.Utils.StringUtil.Split(str.JSubstring(0, d0Pos - 1), " ");
                if (bbArray.Length == 2) {
                    this.wx = float.Parse(bbArray[0], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            else {
                if (d1Pos != -1) {
                    isColor = false;
                    String[] bbArray = iText.Commons.Utils.StringUtil.Split(str.JSubstring(0, d1Pos - 1), " ");
                    if (bbArray.Length == 6) {
                        this.wx = float.Parse(bbArray[0], System.Globalization.CultureInfo.InvariantCulture);
                        this.llx = float.Parse(bbArray[2], System.Globalization.CultureInfo.InvariantCulture);
                        this.lly = float.Parse(bbArray[3], System.Globalization.CultureInfo.InvariantCulture);
                        this.urx = float.Parse(bbArray[4], System.Globalization.CultureInfo.InvariantCulture);
                        this.ury = float.Parse(bbArray[5], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
        }
    }
}
