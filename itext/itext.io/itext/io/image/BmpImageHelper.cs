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
using System.Collections.Generic;
using System.IO;
using iText.IO.Exceptions;
using iText.IO.Font;

namespace iText.IO.Image {
//\cond DO_NOT_DOCUMENT
    internal sealed class BmpImageHelper {
        private class BmpParameters {
            public BmpParameters(BmpImageData image) {
                this.image = image;
            }

//\cond DO_NOT_DOCUMENT
            internal BmpImageData image;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int width;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int height;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal IDictionary<String, Object> additional;
//\endcond

//\cond DO_NOT_DOCUMENT
            // BMP variables
            internal Stream inputStream;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long bitmapFileSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long bitmapOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long compression;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long imageSize;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal byte[] palette;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int imageType;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int numBands;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool isBottomUp;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int bitsPerPixel;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int redMask;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int greenMask;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int blueMask;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int alphaMask;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal IDictionary<String, Object> properties = new Dictionary<String, Object>();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long xPelsPerMeter;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal long yPelsPerMeter;
//\endcond
        }

        // BMP Image types
        private const int VERSION_2_1_BIT = 0;

        private const int VERSION_2_4_BIT = 1;

        private const int VERSION_2_8_BIT = 2;

        private const int VERSION_2_24_BIT = 3;

        private const int VERSION_3_1_BIT = 4;

        private const int VERSION_3_4_BIT = 5;

        private const int VERSION_3_8_BIT = 6;

        private const int VERSION_3_24_BIT = 7;

        private const int VERSION_3_NT_16_BIT = 8;

        private const int VERSION_3_NT_32_BIT = 9;

        private const int VERSION_4_1_BIT = 10;

        private const int VERSION_4_4_BIT = 11;

        private const int VERSION_4_8_BIT = 12;

        private const int VERSION_4_16_BIT = 13;

        private const int VERSION_4_24_BIT = 14;

        private const int VERSION_4_32_BIT = 15;

        // Color space types
        private const int LCS_CALIBRATED_RGB = 0;

        private const int LCS_SRGB = 1;

        private const int LCS_CMYK = 2;

        // Compression Types
        private const int BI_RGB = 0;

        private const int BI_RLE8 = 1;

        private const int BI_RLE4 = 2;

        private const int BI_BITFIELDS = 3;

        /// <summary>Process the passed Image data as a BMP image.</summary>
        /// <remarks>
        /// Process the passed Image data as a BMP image.
        /// Image is loaded and all image attributes are initialized and/or updated
        /// </remarks>
        /// <param name="image">the image to process as a BMP image</param>
        public static void ProcessImage(ImageData image) {
            if (image.GetOriginalType() != ImageType.BMP) {
                throw new ArgumentException("BMP image expected");
            }
            BmpImageHelper.BmpParameters bmp;
            Stream bmpStream;
            try {
                if (image.GetData() == null) {
                    image.LoadData();
                }
                bmpStream = new MemoryStream(image.GetData());
                image.imageSize = image.GetData().Length;
                bmp = new BmpImageHelper.BmpParameters((BmpImageData)image);
                Process(bmp, bmpStream);
                if (GetImage(bmp)) {
                    image.SetWidth(bmp.width);
                    image.SetHeight(bmp.height);
                    image.SetDpi((int)(bmp.xPelsPerMeter * 0.0254d + 0.5d), (int)(bmp.yPelsPerMeter * 0.0254d + 0.5d));
                }
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BMP_IMAGE_EXCEPTION, e);
            }
            RawImageHelper.UpdateImageAttributes(bmp.image, bmp.additional);
        }

        private static void Process(BmpImageHelper.BmpParameters bmp, Stream stream) {
            bmp.inputStream = stream;
            if (!bmp.image.IsNoHeader()) {
                // Start File Header
                if (!(ReadUnsignedByte(bmp.inputStream) == 'B' && ReadUnsignedByte(bmp.inputStream) == 'M')) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_MAGIC_VALUE_FOR_BMP_FILE_MUST_BE_BM
                        );
                }
                // Read file size
                bmp.bitmapFileSize = ReadDWord(bmp.inputStream);
                // Read the two reserved fields
                ReadWord(bmp.inputStream);
                ReadWord(bmp.inputStream);
                // Offset to the bitmap from the beginning
                bmp.bitmapOffset = ReadDWord(bmp.inputStream);
            }
            // End File Header
            // Start BitmapCoreHeader
            long size = ReadDWord(bmp.inputStream);
            if (size == 12) {
                bmp.width = ReadWord(bmp.inputStream);
                bmp.height = ReadWord(bmp.inputStream);
            }
            else {
                bmp.width = ReadLong(bmp.inputStream);
                bmp.height = ReadLong(bmp.inputStream);
            }
            int planes = ReadWord(bmp.inputStream);
            bmp.bitsPerPixel = ReadWord(bmp.inputStream);
            bmp.properties.Put("color_planes", planes);
            bmp.properties.Put("bits_per_pixel", bmp.bitsPerPixel);
            // As BMP always has 3 rgb bands, except for Version 5,
            // which is bgra
            bmp.numBands = 3;
            if (bmp.bitmapOffset == 0) {
                bmp.bitmapOffset = size;
            }
            if (size == 12) {
                // Windows 2.x and OS/2 1.x
                bmp.properties.Put("bmp_version", "BMP v. 2.x");
                // Classify the image type
                if (bmp.bitsPerPixel == 1) {
                    bmp.imageType = VERSION_2_1_BIT;
                }
                else {
                    if (bmp.bitsPerPixel == 4) {
                        bmp.imageType = VERSION_2_4_BIT;
                    }
                    else {
                        if (bmp.bitsPerPixel == 8) {
                            bmp.imageType = VERSION_2_8_BIT;
                        }
                        else {
                            if (bmp.bitsPerPixel == 24) {
                                bmp.imageType = VERSION_2_24_BIT;
                            }
                        }
                    }
                }
                // Read in the palette
                int numberOfEntries = (int)((bmp.bitmapOffset - 14 - size) / 3);
                int sizeOfPalette = numberOfEntries * 3;
                if (bmp.bitmapOffset == size) {
                    switch (bmp.imageType) {
                        case VERSION_2_1_BIT: {
                            sizeOfPalette = 2 * 3;
                            break;
                        }

                        case VERSION_2_4_BIT: {
                            sizeOfPalette = 16 * 3;
                            break;
                        }

                        case VERSION_2_8_BIT: {
                            sizeOfPalette = 256 * 3;
                            break;
                        }

                        case VERSION_2_24_BIT: {
                            sizeOfPalette = 0;
                            break;
                        }
                    }
                    bmp.bitmapOffset = size + sizeOfPalette;
                }
                ReadPalette(sizeOfPalette, bmp);
            }
            else {
                bmp.compression = ReadDWord(bmp.inputStream);
                bmp.imageSize = ReadDWord(bmp.inputStream);
                bmp.xPelsPerMeter = ReadLong(bmp.inputStream);
                bmp.yPelsPerMeter = ReadLong(bmp.inputStream);
                long colorsUsed = ReadDWord(bmp.inputStream);
                long colorsImportant = ReadDWord(bmp.inputStream);
                switch ((int)bmp.compression) {
                    case BI_RGB: {
                        bmp.properties.Put("compression", "BI_RGB");
                        break;
                    }

                    case BI_RLE8: {
                        bmp.properties.Put("compression", "BI_RLE8");
                        break;
                    }

                    case BI_RLE4: {
                        bmp.properties.Put("compression", "BI_RLE4");
                        break;
                    }

                    case BI_BITFIELDS: {
                        bmp.properties.Put("compression", "BI_BITFIELDS");
                        break;
                    }
                }
                bmp.properties.Put("x_pixels_per_meter", bmp.xPelsPerMeter);
                bmp.properties.Put("y_pixels_per_meter", bmp.yPelsPerMeter);
                bmp.properties.Put("colors_used", colorsUsed);
                bmp.properties.Put("colors_important", colorsImportant);
                if (size == 40 || size == 52 || size == 56) {
                    int sizeOfPalette;
                    // Windows 3.x and Windows NT
                    switch ((int)bmp.compression) {
                        case BI_RGB:
                        // No compression
                        case BI_RLE8:
                        // 8-bit RLE compression
                        case BI_RLE4: {
                            // 4-bit RLE compression
                            if (bmp.bitsPerPixel == 1) {
                                bmp.imageType = VERSION_3_1_BIT;
                            }
                            else {
                                if (bmp.bitsPerPixel == 4) {
                                    bmp.imageType = VERSION_3_4_BIT;
                                }
                                else {
                                    if (bmp.bitsPerPixel == 8) {
                                        bmp.imageType = VERSION_3_8_BIT;
                                    }
                                    else {
                                        if (bmp.bitsPerPixel == 24) {
                                            bmp.imageType = VERSION_3_24_BIT;
                                        }
                                        else {
                                            if (bmp.bitsPerPixel == 16) {
                                                bmp.imageType = VERSION_3_NT_16_BIT;
                                                bmp.redMask = 0x7C00;
                                                bmp.greenMask = 0x3E0;
                                                bmp.blueMask = 0x1F;
                                                bmp.properties.Put("red_mask", bmp.redMask);
                                                bmp.properties.Put("green_mask", bmp.greenMask);
                                                bmp.properties.Put("blue_mask", bmp.blueMask);
                                            }
                                            else {
                                                if (bmp.bitsPerPixel == 32) {
                                                    bmp.imageType = VERSION_3_NT_32_BIT;
                                                    bmp.redMask = 0x00FF0000;
                                                    bmp.greenMask = 0x0000FF00;
                                                    bmp.blueMask = 0x000000FF;
                                                    bmp.properties.Put("red_mask", bmp.redMask);
                                                    bmp.properties.Put("green_mask", bmp.greenMask);
                                                    bmp.properties.Put("blue_mask", bmp.blueMask);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // 52 and 56 byte header have mandatory R, G and B masks
                            if (size >= 52) {
                                bmp.redMask = (int)ReadDWord(bmp.inputStream);
                                bmp.greenMask = (int)ReadDWord(bmp.inputStream);
                                bmp.blueMask = (int)ReadDWord(bmp.inputStream);
                                bmp.properties.Put("red_mask", bmp.redMask);
                                bmp.properties.Put("green_mask", bmp.greenMask);
                                bmp.properties.Put("blue_mask", bmp.blueMask);
                            }
                            // 56 byte header has mandatory alpha mask
                            if (size == 56) {
                                bmp.alphaMask = (int)ReadDWord(bmp.inputStream);
                                bmp.properties.Put("alpha_mask", bmp.alphaMask);
                            }
                            // Read in the palette
                            int numberOfEntries = (int)((bmp.bitmapOffset - 14 - size) / 4);
                            sizeOfPalette = numberOfEntries * 4;
                            if (bmp.bitmapOffset == size) {
                                switch (bmp.imageType) {
                                    case VERSION_3_1_BIT: {
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;
                                        break;
                                    }

                                    case VERSION_3_4_BIT: {
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;
                                        break;
                                    }

                                    case VERSION_3_8_BIT: {
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;
                                        break;
                                    }

                                    default: {
                                        sizeOfPalette = 0;
                                        break;
                                    }
                                }
                                bmp.bitmapOffset = size + sizeOfPalette;
                            }
                            ReadPalette(sizeOfPalette, bmp);
                            bmp.properties.Put("bmp_version", "BMP v. 3.x");
                            break;
                        }

                        case BI_BITFIELDS: {
                            if (bmp.bitsPerPixel == 16) {
                                bmp.imageType = VERSION_3_NT_16_BIT;
                            }
                            else {
                                if (bmp.bitsPerPixel == 32) {
                                    bmp.imageType = VERSION_3_NT_32_BIT;
                                }
                            }
                            // BitsField encoding
                            bmp.redMask = (int)ReadDWord(bmp.inputStream);
                            bmp.greenMask = (int)ReadDWord(bmp.inputStream);
                            bmp.blueMask = (int)ReadDWord(bmp.inputStream);
                            // 56 byte header has mandatory alpha mask
                            if (size == 56) {
                                bmp.alphaMask = (int)ReadDWord(bmp.inputStream);
                                bmp.properties.Put("alpha_mask", bmp.alphaMask);
                            }
                            bmp.properties.Put("red_mask", bmp.redMask);
                            bmp.properties.Put("green_mask", bmp.greenMask);
                            bmp.properties.Put("blue_mask", bmp.blueMask);
                            if (colorsUsed != 0) {
                                // there is a palette
                                sizeOfPalette = (int)colorsUsed * 4;
                                ReadPalette(sizeOfPalette, bmp);
                            }
                            bmp.properties.Put("bmp_version", "BMP v. 3.x NT");
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_BMP_FILE_COMPRESSION);
                        }
                    }
                }
                else {
                    if (size == 108) {
                        // Windows 4.x BMP
                        bmp.properties.Put("bmp_version", "BMP v. 4.x");
                        // rgb masks, valid only if comp is BI_BITFIELDS
                        bmp.redMask = (int)ReadDWord(bmp.inputStream);
                        bmp.greenMask = (int)ReadDWord(bmp.inputStream);
                        bmp.blueMask = (int)ReadDWord(bmp.inputStream);
                        // Only supported for 32bpp BI_RGB argb
                        bmp.alphaMask = (int)ReadDWord(bmp.inputStream);
                        long csType = ReadDWord(bmp.inputStream);
                        int redX = ReadLong(bmp.inputStream);
                        int redY = ReadLong(bmp.inputStream);
                        int redZ = ReadLong(bmp.inputStream);
                        int greenX = ReadLong(bmp.inputStream);
                        int greenY = ReadLong(bmp.inputStream);
                        int greenZ = ReadLong(bmp.inputStream);
                        int blueX = ReadLong(bmp.inputStream);
                        int blueY = ReadLong(bmp.inputStream);
                        int blueZ = ReadLong(bmp.inputStream);
                        long gammaRed = ReadDWord(bmp.inputStream);
                        long gammaGreen = ReadDWord(bmp.inputStream);
                        long gammaBlue = ReadDWord(bmp.inputStream);
                        if (bmp.bitsPerPixel == 1) {
                            bmp.imageType = VERSION_4_1_BIT;
                        }
                        else {
                            if (bmp.bitsPerPixel == 4) {
                                bmp.imageType = VERSION_4_4_BIT;
                            }
                            else {
                                if (bmp.bitsPerPixel == 8) {
                                    bmp.imageType = VERSION_4_8_BIT;
                                }
                                else {
                                    if (bmp.bitsPerPixel == 16) {
                                        bmp.imageType = VERSION_4_16_BIT;
                                        if ((int)bmp.compression == BI_RGB) {
                                            bmp.redMask = 0x7C00;
                                            bmp.greenMask = 0x3E0;
                                            bmp.blueMask = 0x1F;
                                        }
                                    }
                                    else {
                                        if (bmp.bitsPerPixel == 24) {
                                            bmp.imageType = VERSION_4_24_BIT;
                                        }
                                        else {
                                            if (bmp.bitsPerPixel == 32) {
                                                bmp.imageType = VERSION_4_32_BIT;
                                                if ((int)bmp.compression == BI_RGB) {
                                                    bmp.redMask = 0x00FF0000;
                                                    bmp.greenMask = 0x0000FF00;
                                                    bmp.blueMask = 0x000000FF;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        bmp.properties.Put("red_mask", bmp.redMask);
                        bmp.properties.Put("green_mask", bmp.greenMask);
                        bmp.properties.Put("blue_mask", bmp.blueMask);
                        bmp.properties.Put("alpha_mask", bmp.alphaMask);
                        // Read in the palette
                        int numberOfEntries = (int)((bmp.bitmapOffset - 14 - size) / 4);
                        int sizeOfPalette = numberOfEntries * 4;
                        if (bmp.bitmapOffset == size) {
                            switch (bmp.imageType) {
                                case VERSION_4_1_BIT: {
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;
                                    break;
                                }

                                case VERSION_4_4_BIT: {
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;
                                    break;
                                }

                                case VERSION_4_8_BIT: {
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;
                                    break;
                                }

                                default: {
                                    sizeOfPalette = 0;
                                    break;
                                }
                            }
                            bmp.bitmapOffset = size + sizeOfPalette;
                        }
                        ReadPalette(sizeOfPalette, bmp);
                        switch ((int)csType) {
                            case LCS_CALIBRATED_RGB: {
                                // All the new fields are valid only for this case
                                bmp.properties.Put("color_space", "LCS_CALIBRATED_RGB");
                                bmp.properties.Put("redX", redX);
                                bmp.properties.Put("redY", redY);
                                bmp.properties.Put("redZ", redZ);
                                bmp.properties.Put("greenX", greenX);
                                bmp.properties.Put("greenY", greenY);
                                bmp.properties.Put("greenZ", greenZ);
                                bmp.properties.Put("blueX", blueX);
                                bmp.properties.Put("blueY", blueY);
                                bmp.properties.Put("blueZ", blueZ);
                                bmp.properties.Put("gamma_red", gammaRed);
                                bmp.properties.Put("gamma_green", gammaGreen);
                                bmp.properties.Put("gamma_blue", gammaBlue);
                                throw new Exception("Not implemented yet.");
                            }

                            case LCS_SRGB: {
                                // Default Windows color space
                                bmp.properties.Put("color_space", "LCS_sRGB");
                                break;
                            }

                            case LCS_CMYK: {
                                bmp.properties.Put("color_space", "LCS_CMYK");
                                //		    break;
                                throw new Exception("Not implemented yet.");
                            }
                        }
                    }
                    else {
                        bmp.properties.Put("bmp_version", "BMP v. 5.x");
                        throw new Exception("Not implemented yet.");
                    }
                }
            }
            if (bmp.height > 0) {
                // bottom up image
                bmp.isBottomUp = true;
            }
            else {
                // top down image
                bmp.isBottomUp = false;
                bmp.height = Math.Abs(bmp.height);
            }
            // When number of bitsPerPixel is <= 8, we use IndexColorModel.
            if (bmp.bitsPerPixel == 1 || bmp.bitsPerPixel == 4 || bmp.bitsPerPixel == 8) {
                bmp.numBands = 1;
                // Create IndexColorModel from the palette.
                byte[] r;
                byte[] g;
                byte[] b;
                int sizep;
                if (bmp.imageType == VERSION_2_1_BIT || bmp.imageType == VERSION_2_4_BIT || bmp.imageType == VERSION_2_8_BIT
                    ) {
                    sizep = bmp.palette.Length / 3;
                    if (sizep > 256) {
                        sizep = 256;
                    }
                    int off;
                    r = new byte[sizep];
                    g = new byte[sizep];
                    b = new byte[sizep];
                    for (int i = 0; i < sizep; i++) {
                        off = 3 * i;
                        b[i] = bmp.palette[off];
                        g[i] = bmp.palette[off + 1];
                        r[i] = bmp.palette[off + 2];
                    }
                }
                else {
                    sizep = bmp.palette.Length / 4;
                    if (sizep > 256) {
                        sizep = 256;
                    }
                    int off;
                    r = new byte[sizep];
                    g = new byte[sizep];
                    b = new byte[sizep];
                    for (int i = 0; i < sizep; i++) {
                        off = 4 * i;
                        b[i] = bmp.palette[off];
                        g[i] = bmp.palette[off + 1];
                        r[i] = bmp.palette[off + 2];
                    }
                }
            }
            else {
                if (bmp.bitsPerPixel == 16) {
                    bmp.numBands = 3;
                }
                else {
                    if (bmp.bitsPerPixel == 32) {
                        bmp.numBands = bmp.alphaMask == 0 ? 3 : 4;
                    }
                    else {
                        // The number of bands in the SampleModel is determined by
                        // the length of the mask array passed in.
                        bmp.numBands = 3;
                    }
                }
            }
        }

        private static byte[] GetPalette(int group, BmpImageHelper.BmpParameters bmp) {
            if (bmp.palette == null) {
                return null;
            }
            byte[] np = new byte[bmp.palette.Length / group * 3];
            int e = bmp.palette.Length / group;
            for (int k = 0; k < e; ++k) {
                int src = k * group;
                int dest = k * 3;
                np[dest + 2] = bmp.palette[src++];
                np[dest + 1] = bmp.palette[src++];
                np[dest] = bmp.palette[src];
            }
            return np;
        }

        private static bool GetImage(BmpImageHelper.BmpParameters bmp) {
            // buffer for byte data
            byte[] bdata;
            //	if (sampleModel.getDataType() == DataBuffer.TYPE_BYTE)
            //	    bdata = (byte[])((DataBufferByte)tile.getDataBuffer()).getData();
            //	else if (sampleModel.getDataType() == DataBuffer.TYPE_USHORT)
            //	    sdata = (short[])((DataBufferUShort)tile.getDataBuffer()).getData();
            //	else if (sampleModel.getDataType() == DataBuffer.TYPE_INT)
            //	    idata = (int[])((DataBufferInt)tile.getDataBuffer()).getData();
            // There should only be one tile.
            switch (bmp.imageType) {
                case VERSION_2_1_BIT: {
                    // no compression
                    Read1Bit(3, bmp);
                    return true;
                }

                case VERSION_2_4_BIT: {
                    // no compression
                    Read4Bit(3, bmp);
                    return true;
                }

                case VERSION_2_8_BIT: {
                    // no compression
                    Read8Bit(3, bmp);
                    return true;
                }

                case VERSION_2_24_BIT: {
                    // no compression
                    bdata = new byte[bmp.width * bmp.height * 3];
                    Read24Bit(bdata, bmp);
                    RawImageHelper.UpdateRawImageParameters(bmp.image, bmp.width, bmp.height, 3, 8, bdata);
                    return true;
                }

                case VERSION_3_1_BIT: {
                    // 1-bit images cannot be compressed.
                    Read1Bit(4, bmp);
                    return true;
                }

                case VERSION_3_4_BIT: {
                    switch ((int)bmp.compression) {
                        case BI_RGB: {
                            Read4Bit(4, bmp);
                            break;
                        }

                        case BI_RLE4: {
                            ReadRLE4(bmp);
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_BMP_FILE_COMPRESSION);
                        }
                    }
                    return true;
                }

                case VERSION_3_8_BIT: {
                    switch ((int)bmp.compression) {
                        case BI_RGB: {
                            Read8Bit(4, bmp);
                            break;
                        }

                        case BI_RLE8: {
                            ReadRLE8(bmp);
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_BMP_FILE_COMPRESSION);
                        }
                    }
                    return true;
                }

                case VERSION_3_24_BIT: {
                    // 24-bit images are not compressed
                    bdata = new byte[bmp.width * bmp.height * 3];
                    Read24Bit(bdata, bmp);
                    RawImageHelper.UpdateRawImageParameters(bmp.image, bmp.width, bmp.height, 3, 8, bdata);
                    return true;
                }

                case VERSION_3_NT_16_BIT: {
                    Read1632Bit(false, bmp);
                    return true;
                }

                case VERSION_3_NT_32_BIT: {
                    Read1632Bit(true, bmp);
                    return true;
                }

                case VERSION_4_1_BIT: {
                    Read1Bit(4, bmp);
                    return true;
                }

                case VERSION_4_4_BIT: {
                    switch ((int)bmp.compression) {
                        case BI_RGB: {
                            Read4Bit(4, bmp);
                            break;
                        }

                        case BI_RLE4: {
                            ReadRLE4(bmp);
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_BMP_FILE_COMPRESSION);
                        }
                    }
                    return true;
                }

                case VERSION_4_8_BIT: {
                    switch ((int)bmp.compression) {
                        case BI_RGB: {
                            Read8Bit(4, bmp);
                            break;
                        }

                        case BI_RLE8: {
                            ReadRLE8(bmp);
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_BMP_FILE_COMPRESSION);
                        }
                    }
                    return true;
                }

                case VERSION_4_16_BIT: {
                    Read1632Bit(false, bmp);
                    return true;
                }

                case VERSION_4_24_BIT: {
                    bdata = new byte[bmp.width * bmp.height * 3];
                    Read24Bit(bdata, bmp);
                    RawImageHelper.UpdateRawImageParameters(bmp.image, bmp.width, bmp.height, 3, 8, bdata);
                    return true;
                }

                case VERSION_4_32_BIT: {
                    Read1632Bit(true, bmp);
                    return true;
                }
            }
            return false;
        }

        private static void IndexedModel(byte[] bdata, int bpc, int paletteEntries, BmpImageHelper.BmpParameters bmp
            ) {
            RawImageHelper.UpdateRawImageParameters(bmp.image, bmp.width, bmp.height, 1, bpc, bdata);
            Object[] colorSpace = new Object[4];
            colorSpace[0] = "/Indexed";
            colorSpace[1] = "/DeviceRGB";
            byte[] np = GetPalette(paletteEntries, bmp);
            int len = np.Length;
            colorSpace[2] = len / 3 - 1;
            colorSpace[3] = PdfEncodings.ConvertToString(np, null);
            bmp.additional = new Dictionary<String, Object>();
            bmp.additional.Put("ColorSpace", colorSpace);
        }

        private static void ReadPalette(int sizeOfPalette, BmpImageHelper.BmpParameters bmp) {
            if (sizeOfPalette == 0) {
                return;
            }
            bmp.palette = new byte[sizeOfPalette];
            int bytesRead = 0;
            while (bytesRead < sizeOfPalette) {
                int r = bmp.inputStream.JRead(bmp.palette, bytesRead, sizeOfPalette - bytesRead);
                if (r < 0) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INCOMPLETE_PALETTE);
                }
                bytesRead += r;
            }
            bmp.properties.Put("palette", bmp.palette);
        }

        // Deal with 1 Bit images using IndexColorModels
        private static void Read1Bit(int paletteEntries, BmpImageHelper.BmpParameters bmp) {
            byte[] bdata = new byte[(bmp.width + 7) / 8 * bmp.height];
            int padding = 0;
            int bytesPerScanline = (int)Math.Ceiling(bmp.width / 8.0d);
            int remainder = bytesPerScanline % 4;
            if (remainder != 0) {
                padding = 4 - remainder;
            }
            int imSize = (bytesPerScanline + padding) * bmp.height;
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
            }
            if (bmp.isBottomUp) {
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, imSize - (i + 1) * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline
                        );
                }
            }
            else {
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, i * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline);
                }
            }
            IndexedModel(bdata, 1, paletteEntries, bmp);
        }

        // Method to read a 4 bit BMP image data
        private static void Read4Bit(int paletteEntries, BmpImageHelper.BmpParameters bmp) {
            byte[] bdata = new byte[(bmp.width + 1) / 2 * bmp.height];
            // Padding bytes at the end of each scanline
            int padding = 0;
            int bytesPerScanline = (int)Math.Ceiling(bmp.width / 2.0d);
            int remainder = bytesPerScanline % 4;
            if (remainder != 0) {
                padding = 4 - remainder;
            }
            int imSize = (bytesPerScanline + padding) * bmp.height;
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
            }
            if (bmp.isBottomUp) {
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, imSize - (i + 1) * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline
                        );
                }
            }
            else {
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, i * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline);
                }
            }
            IndexedModel(bdata, 4, paletteEntries, bmp);
        }

        // Method to read 8 bit BMP image data
        private static void Read8Bit(int paletteEntries, BmpImageHelper.BmpParameters bmp) {
            byte[] bdata = new byte[bmp.width * bmp.height];
            // Padding bytes at the end of each scanline
            int padding = 0;
            // width * bitsPerPixel should be divisible by 32
            int bitsPerScanline = bmp.width * 8;
            if (bitsPerScanline % 32 != 0) {
                padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding / 8.0);
            }
            int imSize = (bmp.width + padding) * bmp.height;
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
            }
            if (bmp.isBottomUp) {
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, imSize - (i + 1) * (bmp.width + padding), bdata, i * bmp.width, bmp.width);
                }
            }
            else {
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(values, i * (bmp.width + padding), bdata, i * bmp.width, bmp.width);
                }
            }
            IndexedModel(bdata, 8, paletteEntries, bmp);
        }

        // Method to read 24 bit BMP image data
        private static void Read24Bit(byte[] bdata, BmpImageHelper.BmpParameters bmp) {
            // Padding bytes at the end of each scanline
            int padding = 0;
            // width * bitsPerPixel should be divisible by 32
            int bitsPerScanline = bmp.width * 24;
            if (bitsPerScanline % 32 != 0) {
                padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding / 8.0);
            }
            int imSize = (bmp.width * 3 + 3) / 4 * 4 * bmp.height;
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                int r = bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
                if (r < 0) {
                    break;
                }
                bytesRead += r;
            }
            int l = 0;
            int count;
            if (bmp.isBottomUp) {
                int max = bmp.width * bmp.height * 3 - 1;
                count = -padding;
                for (int i = 0; i < bmp.height; i++) {
                    l = max - (i + 1) * bmp.width * 3 + 1;
                    count += padding;
                    for (int j = 0; j < bmp.width; j++) {
                        bdata[l + 2] = values[count++];
                        bdata[l + 1] = values[count++];
                        bdata[l] = values[count++];
                        l += 3;
                    }
                }
            }
            else {
                count = -padding;
                for (int i = 0; i < bmp.height; i++) {
                    count += padding;
                    for (int j = 0; j < bmp.width; j++) {
                        bdata[l + 2] = values[count++];
                        bdata[l + 1] = values[count++];
                        bdata[l] = values[count++];
                        l += 3;
                    }
                }
            }
        }

        private static int FindMask(int mask) {
            int k = 0;
            for (; k < 32; ++k) {
                if ((mask & 1) == 1) {
                    break;
                }
                mask = (int)(((uint)mask) >> 1);
            }
            return mask;
        }

        private static int FindShift(int mask) {
            int k = 0;
            for (; k < 32; ++k) {
                if ((mask & 1) == 1) {
                    break;
                }
                mask = (int)(((uint)mask) >> 1);
            }
            return k;
        }

        private static void Read1632Bit(bool is32, BmpImageHelper.BmpParameters bmp) {
            int red_mask = FindMask(bmp.redMask);
            int red_shift = FindShift(bmp.redMask);
            int red_factor = red_mask + 1;
            int green_mask = FindMask(bmp.greenMask);
            int green_shift = FindShift(bmp.greenMask);
            int green_factor = green_mask + 1;
            int blue_mask = FindMask(bmp.blueMask);
            int blue_shift = FindShift(bmp.blueMask);
            int blue_factor = blue_mask + 1;
            byte[] bdata = new byte[bmp.width * bmp.height * 3];
            // Padding bytes at the end of each scanline
            int padding = 0;
            if (!is32) {
                // width * bitsPerPixel should be divisible by 32
                int bitsPerScanline = bmp.width * 16;
                if (bitsPerScanline % 32 != 0) {
                    padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                    padding = (int)Math.Ceiling(padding / 8.0);
                }
            }
            int imSize = (int)bmp.imageSize;
            if (imSize == 0) {
                imSize = (int)(bmp.bitmapFileSize - bmp.bitmapOffset);
            }
            int l = 0;
            int v;
            if (bmp.isBottomUp) {
                for (int i = bmp.height - 1; i >= 0; --i) {
                    l = bmp.width * 3 * i;
                    for (int j = 0; j < bmp.width; j++) {
                        if (is32) {
                            v = (int)ReadDWord(bmp.inputStream);
                        }
                        else {
                            v = ReadWord(bmp.inputStream);
                        }
                        bdata[l++] = (byte)(((int)(((uint)v) >> red_shift) & red_mask) * 256 / red_factor);
                        bdata[l++] = (byte)(((int)(((uint)v) >> green_shift) & green_mask) * 256 / green_factor);
                        bdata[l++] = (byte)(((int)(((uint)v) >> blue_shift) & blue_mask) * 256 / blue_factor);
                    }
                    for (int m = 0; m < padding; m++) {
                        bmp.inputStream.Read();
                    }
                }
            }
            else {
                for (int i = 0; i < bmp.height; i++) {
                    for (int j = 0; j < bmp.width; j++) {
                        if (is32) {
                            v = (int)ReadDWord(bmp.inputStream);
                        }
                        else {
                            v = ReadWord(bmp.inputStream);
                        }
                        bdata[l++] = (byte)(((int)(((uint)v) >> red_shift) & red_mask) * 256 / red_factor);
                        bdata[l++] = (byte)(((int)(((uint)v) >> green_shift) & green_mask) * 256 / green_factor);
                        bdata[l++] = (byte)(((int)(((uint)v) >> blue_shift) & blue_mask) * 256 / blue_factor);
                    }
                    for (int m = 0; m < padding; m++) {
                        bmp.inputStream.Read();
                    }
                }
            }
            RawImageHelper.UpdateRawImageParameters(bmp.image, bmp.width, bmp.height, 3, 8, bdata);
        }

        private static void ReadRLE8(BmpImageHelper.BmpParameters bmp) {
            // If imageSize field is not provided, calculate it.
            int imSize = (int)bmp.imageSize;
            if (imSize == 0) {
                imSize = (int)(bmp.bitmapFileSize - bmp.bitmapOffset);
            }
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
            }
            // Since data is compressed, decompress it
            byte[] val = DecodeRLE(true, values, bmp);
            // Uncompressed data does not have any padding
            imSize = bmp.width * bmp.height;
            if (bmp.isBottomUp) {
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                // int bytesPerScanline = (int)Math.ceil((double)width/8.0);
                byte[] temp = new byte[val.Length];
                int bytesPerScanline = bmp.width;
                for (int i = 0; i < bmp.height; i++) {
                    Array.Copy(val, imSize - (i + 1) * bytesPerScanline, temp, i * bytesPerScanline, bytesPerScanline);
                }
                val = temp;
            }
            IndexedModel(val, 8, 4, bmp);
        }

        private static void ReadRLE4(BmpImageHelper.BmpParameters bmp) {
            // If imageSize field is not specified, calculate it.
            int imSize = (int)bmp.imageSize;
            if (imSize == 0) {
                imSize = (int)(bmp.bitmapFileSize - bmp.bitmapOffset);
            }
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += bmp.inputStream.JRead(values, bytesRead, imSize - bytesRead);
            }
            // Decompress the RLE4 compressed data.
            byte[] val = DecodeRLE(false, values, bmp);
            // Invert it as it is bottom up format.
            if (bmp.isBottomUp) {
                byte[] inverted = val;
                val = new byte[bmp.width * bmp.height];
                int l = 0;
                int index;
                int lineEnd;
                for (int i = bmp.height - 1; i >= 0; i--) {
                    index = i * bmp.width;
                    lineEnd = l + bmp.width;
                    while (l != lineEnd) {
                        val[l++] = inverted[index++];
                    }
                }
            }
            int stride = (bmp.width + 1) / 2;
            byte[] bdata = new byte[stride * bmp.height];
            int ptr = 0;
            int sh = 0;
            for (int h = 0; h < bmp.height; ++h) {
                for (int w = 0; w < bmp.width; ++w) {
                    if ((w & 1) == 0) {
                        bdata[sh + w / 2] = (byte)(val[ptr++] << 4);
                    }
                    else {
                        bdata[sh + w / 2] |= (byte)(val[ptr++] & 0x0f);
                    }
                }
                sh += stride;
            }
            IndexedModel(bdata, 4, 4, bmp);
        }

        private static byte[] DecodeRLE(bool is8, byte[] values, BmpImageHelper.BmpParameters bmp) {
            byte[] val = new byte[bmp.width * bmp.height];
            try {
                int ptr = 0;
                int x = 0;
                int q = 0;
                for (int y = 0; y < bmp.height && ptr < values.Length; ) {
                    int count = values[ptr++] & 0xff;
                    if (count != 0) {
                        // encoded mode
                        int bt = values[ptr++] & 0xff;
                        if (is8) {
                            for (int i = count; i != 0; --i) {
                                val[q++] = (byte)bt;
                            }
                        }
                        else {
                            for (int i = 0; i < count; ++i) {
                                val[q++] = (byte)((i & 1) == 1 ? bt & 0x0f : (int)(((uint)bt) >> 4) & 0x0f);
                            }
                        }
                        x += count;
                    }
                    else {
                        // escape mode
                        count = values[ptr++] & 0xff;
                        if (count == 1) {
                            break;
                        }
                        switch (count) {
                            case 0: {
                                x = 0;
                                ++y;
                                q = y * bmp.width;
                                break;
                            }

                            case 2: {
                                // delta mode
                                x += values[ptr++] & 0xff;
                                y += values[ptr++] & 0xff;
                                q = y * bmp.width + x;
                                break;
                            }

                            default: {
                                // absolute mode
                                if (is8) {
                                    for (int i = count; i != 0; --i) {
                                        val[q++] = (byte)(values[ptr++] & 0xff);
                                    }
                                }
                                else {
                                    int bt = 0;
                                    for (int i = 0; i < count; ++i) {
                                        if ((i & 1) == 0) {
                                            bt = values[ptr++] & 0xff;
                                        }
                                        val[q++] = (byte)((i & 1) == 1 ? bt & 0x0f : (int)(((uint)bt) >> 4) & 0x0f);
                                    }
                                }
                                x += count;
                                // read pad byte
                                if (is8) {
                                    if ((count & 1) == 1) {
                                        ++ptr;
                                    }
                                }
                                else {
                                    if ((count & 3) == 1 || (count & 3) == 2) {
                                        ++ptr;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception) {
            }
            //empty on purpose
            return val;
        }

        // Windows defined data type reading methods - everything is little endian
        // Unsigned 8 bits
        private static int ReadUnsignedByte(Stream stream) {
            return stream.Read() & 0xff;
        }

        // Unsigned 2 bytes
        private static int ReadUnsignedShort(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            return (b2 << 8 | b1) & 0xffff;
        }

        // Signed 16 bits
        private static int ReadShort(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            return b2 << 8 | b1;
        }

        // Unsigned 16 bits
        private static int ReadWord(Stream stream) {
            return ReadUnsignedShort(stream);
        }

        // Unsigned 4 bytes
        private static long ReadUnsignedInt(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            int b3 = ReadUnsignedByte(stream);
            int b4 = ReadUnsignedByte(stream);
            long l = b4 << 24 | b3 << 16 | b2 << 8 | b1;
            return l & unchecked((int)(0xffffffff));
        }

        // Signed 4 bytes
        private static int ReadInt(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            int b3 = ReadUnsignedByte(stream);
            int b4 = ReadUnsignedByte(stream);
            return b4 << 24 | b3 << 16 | b2 << 8 | b1;
        }

        // Unsigned 4 bytes
        private static long ReadDWord(Stream stream) {
            return ReadUnsignedInt(stream);
        }

        // 32 bit signed value
        private static int ReadLong(Stream stream) {
            return ReadInt(stream);
        }
    }
//\endcond
}
