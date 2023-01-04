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
using iText.IO.Codec;

namespace iText.IO.Image {
    public sealed class RawImageHelper {
        public static void UpdateImageAttributes(RawImageData image, IDictionary<String, Object> additional) {
            if (!image.IsRawImage()) {
                throw new ArgumentException("Raw image expected.");
            }
            // will also have the CCITT parameters
            int colorSpace = image.GetColorEncodingComponentsNumber();
            int typeCCITT = image.GetTypeCcitt();
            if (typeCCITT > 0xff) {
                if (!image.IsMask()) {
                    image.SetColorEncodingComponentsNumber(1);
                }
                image.SetBpc(1);
                image.SetFilter("CCITTFaxDecode");
                int k = typeCCITT - RawImageData.CCITTG3_1D;
                IDictionary<String, Object> decodeparms = new Dictionary<String, Object>();
                if (k != 0) {
                    decodeparms.Put("K", k);
                }
                if ((colorSpace & RawImageData.CCITT_BLACKIS1) != 0) {
                    decodeparms.Put("BlackIs1", true);
                }
                if ((colorSpace & RawImageData.CCITT_ENCODEDBYTEALIGN) != 0) {
                    decodeparms.Put("EncodedByteAlign", true);
                }
                if ((colorSpace & RawImageData.CCITT_ENDOFLINE) != 0) {
                    decodeparms.Put("EndOfLine", true);
                }
                if ((colorSpace & RawImageData.CCITT_ENDOFBLOCK) != 0) {
                    decodeparms.Put("EndOfBlock", false);
                }
                decodeparms.Put("Columns", image.GetWidth());
                decodeparms.Put("Rows", image.GetHeight());
                image.decodeParms = decodeparms;
            }
            else {
                switch (colorSpace) {
                    case 1: {
                        if (image.IsInverted()) {
                            image.decode = new float[] { 1, 0 };
                        }
                        break;
                    }

                    case 3: {
                        if (image.IsInverted()) {
                            image.decode = new float[] { 1, 0, 1, 0, 1, 0 };
                        }
                        break;
                    }

                    case 4:
                    default: {
                        if (image.IsInverted()) {
                            image.decode = new float[] { 1, 0, 1, 0, 1, 0, 1, 0 };
                        }
                        break;
                    }
                }
                if (additional != null) {
                    image.SetImageAttributes(additional);
                }
                if (image.IsMask() && (image.GetBpc() == 1 || image.GetBpc() > 8)) {
                    image.SetColorEncodingComponentsNumber(-1);
                }
                if (image.IsDeflated()) {
                    image.SetFilter("FlateDecode");
                }
            }
        }

        /// <summary>Update original image with Raw Image parameters.</summary>
        /// <param name="image">to update its parameters with Raw Image parameters.</param>
        /// <param name="width">the exact width of the image</param>
        /// <param name="height">the exact height of the image</param>
        /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
        /// <param name="bpc">bits per component. Must be 1,2,4 or 8</param>
        /// <param name="data">the image data</param>
        protected internal static void UpdateRawImageParameters(RawImageData image, int width, int height, int components
            , int bpc, byte[] data) {
            image.SetHeight(height);
            image.SetWidth(width);
            if (components != 1 && components != 3 && components != 4) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.ComponentsMustBe1_3Or4);
            }
            if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.BitsPerComponentMustBe1_2_4or8);
            }
            image.SetColorEncodingComponentsNumber(components);
            image.SetBpc(bpc);
            image.data = data;
        }

        protected internal static void UpdateRawImageParameters(RawImageData image, int width, int height, int components
            , int bpc, byte[] data, int[] transparency) {
            if (transparency != null && transparency.Length != components * 2) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.TransparencyLengthMustBeEqualTo2WithCcittImages
                    );
            }
            if (components == 1 && bpc == 1) {
                byte[] g4 = CCITTG4Encoder.Compress(data, width, height);
                UpdateRawImageParameters(image, width, height, false, RawImageData.CCITTG4, RawImageData.CCITT_BLACKIS1, g4
                    , transparency);
            }
            else {
                UpdateRawImageParameters(image, width, height, components, bpc, data);
                image.SetTransparency(transparency);
            }
        }

        protected internal static void UpdateRawImageParameters(RawImageData image, int width, int height, bool reverseBits
            , int typeCCITT, int parameters, byte[] data, int[] transparency) {
            if (transparency != null && transparency.Length != 2) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.TransparencyLengthMustBeEqualTo2WithCcittImages
                    );
            }
            UpdateCcittImageParameters(image, width, height, reverseBits, typeCCITT, parameters, data);
            image.SetTransparency(transparency);
        }

        protected internal static void UpdateCcittImageParameters(RawImageData image, int width, int height, bool 
            reverseBits, int typeCcitt, int parameters, byte[] data) {
            if (typeCcitt != RawImageData.CCITTG4 && typeCcitt != RawImageData.CCITTG3_1D && typeCcitt != RawImageData
                .CCITTG3_2D) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.CcittCompressionTypeMustBeCcittg4Ccittg3_1dOrCcittg3_2d
                    );
            }
            if (reverseBits) {
                TIFFFaxDecoder.ReverseBits(data);
            }
            image.SetHeight(height);
            image.SetWidth(width);
            image.SetColorEncodingComponentsNumber(parameters);
            image.SetTypeCcitt(typeCcitt);
            image.data = data;
        }
    }
}
