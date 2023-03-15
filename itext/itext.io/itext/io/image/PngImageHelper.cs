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
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Colors;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    internal class PngImageHelper {
        private class PngParameters {
            internal PngParameters(PngImageData image) {
                this.image = image;
            }

            internal PngImageData image;

            internal Stream dataStream;

            internal int width;

            internal int height;

            internal int bitDepth;

            internal int compressionMethod;

            internal int filterMethod;

            internal int interlaceMethod;

            internal IDictionary<String, Object> additional = new Dictionary<String, Object>();

            internal byte[] imageData;

            internal byte[] smask;

            internal byte[] trans;

            internal ByteArrayOutputStream idat = new ByteArrayOutputStream();

            internal int dpiX;

            internal int dpiY;

            internal float XYRatio;

            internal bool genBWMask;

            internal bool palShades;

            internal int transRedGray = -1;

            internal int transGreen = -1;

            internal int transBlue = -1;

            internal int inputBands;

            // number of bytes per input pixel
            internal int bytesPerPixel;

            internal String intent;

            internal IccProfile iccProfile;
        }

        /// <summary>Some PNG specific values.</summary>
        public static readonly int[] PNGID = new int[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        /// <summary>A PNG marker.</summary>
        public const String IHDR = "IHDR";

        /// <summary>A PNG marker.</summary>
        public const String PLTE = "PLTE";

        /// <summary>A PNG marker.</summary>
        public const String IDAT = "IDAT";

        /// <summary>A PNG marker.</summary>
        public const String IEND = "IEND";

        /// <summary>A PNG marker.</summary>
        public const String tRNS = "tRNS";

        /// <summary>A PNG marker.</summary>
        public const String pHYs = "pHYs";

        /// <summary>A PNG marker.</summary>
        public const String gAMA = "gAMA";

        /// <summary>A PNG marker.</summary>
        public const String cHRM = "cHRM";

        /// <summary>A PNG marker.</summary>
        public const String sRGB = "sRGB";

        /// <summary>A PNG marker.</summary>
        public const String iCCP = "iCCP";

        private const int TRANSFERSIZE = 4096;

        private const int PNG_FILTER_NONE = 0;

        private const int PNG_FILTER_SUB = 1;

        private const int PNG_FILTER_UP = 2;

        private const int PNG_FILTER_AVERAGE = 3;

        private const int PNG_FILTER_PAETH = 4;

        private static readonly String[] intents = new String[] { PngImageHelperConstants.PERCEPTUAL, PngImageHelperConstants
            .RELATIVE_COLORIMETRIC, PngImageHelperConstants.SATURATION, PngImageHelperConstants.ABSOLUTE_COLORMETRIC
             };

        public static void ProcessImage(ImageData image) {
            if (image.GetOriginalType() != ImageType.PNG) {
                throw new ArgumentException("PNG image expected");
            }
            PngImageHelper.PngParameters png;
            Stream pngStream = null;
            try {
                if (image.GetData() == null) {
                    image.LoadData();
                }
                pngStream = new MemoryStream(image.GetData());
                image.imageSize = image.GetData().Length;
                png = new PngImageHelper.PngParameters((PngImageData)image);
                ProcessPng(pngStream, png);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.PngImageException, e);
            }
            finally {
                if (pngStream != null) {
                    try {
                        pngStream.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
            }
            RawImageHelper.UpdateImageAttributes(png.image, png.additional);
        }

        private static void ProcessPng(Stream pngStream, PngImageHelper.PngParameters png) {
            ReadPng(pngStream, png);
            int colorType = png.image.GetColorType();
            if (png.iccProfile != null && png.iccProfile.GetNumComponents() != GetExpectedNumberOfColorComponents(png)
                ) {
                ITextLogManager.GetLogger(typeof(PngImageHelper)).LogWarning(iText.IO.Logs.IoLogMessageConstant.PNG_IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS
                    );
            }
            try {
                int pal0 = 0;
                int palIdx = 0;
                png.palShades = false;
                if (png.trans != null) {
                    for (int k = 0; k < png.trans.Length; ++k) {
                        int n = png.trans[k] & 0xff;
                        if (n == 0) {
                            ++pal0;
                            palIdx = k;
                        }
                        if (n != 0 && n != 255) {
                            png.palShades = true;
                            break;
                        }
                    }
                }
                if ((colorType & 4) != 0) {
                    png.palShades = true;
                }
                png.genBWMask = (!png.palShades && (pal0 > 1 || png.transRedGray >= 0));
                if (!png.palShades && !png.genBWMask && pal0 == 1) {
                    png.additional.Put(PngImageHelperConstants.MASK, new int[] { palIdx, palIdx });
                }
                bool needDecode = (png.interlaceMethod == 1) || (png.bitDepth == 16) || ((colorType & 4) != 0) || png.palShades
                     || png.genBWMask;
                switch (colorType) {
                    case 0: {
                        png.inputBands = 1;
                        break;
                    }

                    case 2: {
                        png.inputBands = 3;
                        break;
                    }

                    case 3: {
                        png.inputBands = 1;
                        break;
                    }

                    case 4: {
                        png.inputBands = 2;
                        break;
                    }

                    case 6: {
                        png.inputBands = 4;
                        break;
                    }
                }
                if (needDecode) {
                    DecodeIdat(png);
                }
                int components = png.inputBands;
                if ((colorType & 4) != 0) {
                    --components;
                }
                int bpc = png.bitDepth;
                if (bpc == 16) {
                    bpc = 8;
                }
                if (png.imageData != null) {
                    if (png.image.IsIndexed()) {
                        RawImageHelper.UpdateRawImageParameters(png.image, png.width, png.height, components, bpc, png.imageData);
                    }
                    else {
                        RawImageHelper.UpdateRawImageParameters(png.image, png.width, png.height, components, bpc, png.imageData, 
                            null);
                    }
                }
                else {
                    RawImageHelper.UpdateRawImageParameters(png.image, png.width, png.height, components, bpc, png.idat.ToArray
                        ());
                    png.image.SetDeflated(true);
                    IDictionary<String, Object> decodeparms = new Dictionary<String, Object>();
                    decodeparms.Put(PngImageHelperConstants.BITS_PER_COMPONENT, png.bitDepth);
                    decodeparms.Put(PngImageHelperConstants.PREDICTOR, 15);
                    decodeparms.Put(PngImageHelperConstants.COLUMNS, png.width);
                    decodeparms.Put(PngImageHelperConstants.COLORS, (png.image.IsIndexed() || png.image.IsGrayscaleImage()) ? 
                        1 : 3);
                    png.image.decodeParms = decodeparms;
                }
                if (png.intent != null) {
                    png.additional.Put(PngImageHelperConstants.INTENT, png.intent);
                }
                if (png.iccProfile != null) {
                    png.image.SetProfile(png.iccProfile);
                }
                if (png.palShades) {
                    RawImageData im2 = (RawImageData)ImageDataFactory.CreateRawImage(null);
                    RawImageHelper.UpdateRawImageParameters(im2, png.width, png.height, 1, 8, png.smask);
                    im2.MakeMask();
                    png.image.SetImageMask(im2);
                }
                if (png.genBWMask) {
                    RawImageData im2 = (RawImageData)ImageDataFactory.CreateRawImage(null);
                    RawImageHelper.UpdateRawImageParameters(im2, png.width, png.height, 1, 1, png.smask);
                    im2.MakeMask();
                    png.image.SetImageMask(im2);
                }
                png.image.SetDpi(png.dpiX, png.dpiY);
                png.image.SetXYRatio(png.XYRatio);
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.PngImageException, e);
            }
        }

        private static int GetExpectedNumberOfColorComponents(PngImageHelper.PngParameters png) {
            return png.image.IsGrayscaleImage() ? 1 : 3;
        }

        private static void ReadPng(Stream pngStream, PngImageHelper.PngParameters png) {
            for (int i = 0; i < PNGID.Length; i++) {
                if (PNGID[i] != pngStream.Read()) {
                    throw new System.IO.IOException("file.is.not.a.valid.png");
                }
            }
            byte[] buffer = new byte[TRANSFERSIZE];
            while (true) {
                int len = GetInt(pngStream);
                String marker = GetString(pngStream);
                if (len < 0 || !CheckMarker(marker)) {
                    throw new System.IO.IOException("corrupted.png.file");
                }
                if (IDAT.Equals(marker)) {
                    int size;
                    while (len != 0) {
                        size = pngStream.JRead(buffer, 0, Math.Min(len, TRANSFERSIZE));
                        if (size < 0) {
                            return;
                        }
                        png.idat.Write(buffer, 0, size);
                        len -= size;
                    }
                }
                else {
                    if (tRNS.Equals(marker)) {
                        switch (png.image.GetColorType()) {
                            case 0: {
                                if (len >= 2) {
                                    len -= 2;
                                    int gray = GetWord(pngStream);
                                    if (png.bitDepth == 16) {
                                        png.transRedGray = gray;
                                    }
                                    else {
                                        png.additional.Put(PngImageHelperConstants.MASK, MessageFormatUtil.Format("[{0} {1}]", gray, gray));
                                    }
                                }
                                break;
                            }

                            case 2: {
                                if (len >= 6) {
                                    len -= 6;
                                    int red = GetWord(pngStream);
                                    int green = GetWord(pngStream);
                                    int blue = GetWord(pngStream);
                                    if (png.bitDepth == 16) {
                                        png.transRedGray = red;
                                        png.transGreen = green;
                                        png.transBlue = blue;
                                    }
                                    else {
                                        png.additional.Put(PngImageHelperConstants.MASK, MessageFormatUtil.Format("[{0} {1} {2} {3} {4} {5}]", red
                                            , red, green, green, blue, blue));
                                    }
                                }
                                break;
                            }

                            case 3: {
                                if (len > 0) {
                                    png.trans = new byte[len];
                                    for (int k = 0; k < len; ++k) {
                                        png.trans[k] = (byte)pngStream.Read();
                                    }
                                    len = 0;
                                }
                                break;
                            }
                        }
                        StreamUtil.Skip(pngStream, len);
                    }
                    else {
                        if (IHDR.Equals(marker)) {
                            png.width = GetInt(pngStream);
                            png.height = GetInt(pngStream);
                            png.bitDepth = pngStream.Read();
                            png.image.SetColorType(pngStream.Read());
                            png.compressionMethod = pngStream.Read();
                            png.filterMethod = pngStream.Read();
                            png.interlaceMethod = pngStream.Read();
                        }
                        else {
                            if (PLTE.Equals(marker)) {
                                if (png.image.IsIndexed()) {
                                    ByteBuffer colorTableBuf = new ByteBuffer();
                                    while ((len--) > 0) {
                                        colorTableBuf.Append(pngStream.Read());
                                    }
                                    png.image.SetColorPalette(colorTableBuf.ToByteArray());
                                }
                                else {
                                    StreamUtil.Skip(pngStream, len);
                                }
                            }
                            else {
                                if (pHYs.Equals(marker)) {
                                    int dx = GetInt(pngStream);
                                    int dy = GetInt(pngStream);
                                    int unit = pngStream.Read();
                                    if (unit == 1) {
                                        png.dpiX = (int)(dx * 0.0254f + 0.5f);
                                        png.dpiY = (int)(dy * 0.0254f + 0.5f);
                                    }
                                    else {
                                        if (dy != 0) {
                                            png.XYRatio = (float)dx / (float)dy;
                                        }
                                    }
                                }
                                else {
                                    if (cHRM.Equals(marker)) {
                                        PngChromaticities pngChromaticities = new PngChromaticities(GetInt(pngStream) / 100000f, GetInt(pngStream)
                                             / 100000f, GetInt(pngStream) / 100000f, GetInt(pngStream) / 100000f, GetInt(pngStream) / 100000f, GetInt
                                            (pngStream) / 100000f, GetInt(pngStream) / 100000f, GetInt(pngStream) / 100000f);
                                        if (!(Math.Abs(pngChromaticities.GetXW()) < 0.0001f || Math.Abs(pngChromaticities.GetYW()) < 0.0001f || Math
                                            .Abs(pngChromaticities.GetXR()) < 0.0001f || Math.Abs(pngChromaticities.GetYR()) < 0.0001f || Math.Abs
                                            (pngChromaticities.GetXG()) < 0.0001f || Math.Abs(pngChromaticities.GetYG()) < 0.0001f || Math.Abs(pngChromaticities
                                            .GetXB()) < 0.0001f || Math.Abs(pngChromaticities.GetYB()) < 0.0001f)) {
                                            png.image.SetPngChromaticities(pngChromaticities);
                                        }
                                    }
                                    else {
                                        if (sRGB.Equals(marker)) {
                                            int ri = pngStream.Read();
                                            png.intent = intents[ri];
                                            png.image.SetGamma(2.2f);
                                            PngChromaticities pngChromaticities = new PngChromaticities(0.3127f, 0.329f, 0.64f, 0.33f, 0.3f, 0.6f, 0.15f
                                                , 0.06f);
                                            png.image.SetPngChromaticities(pngChromaticities);
                                        }
                                        else {
                                            if (gAMA.Equals(marker)) {
                                                int gm = GetInt(pngStream);
                                                if (gm != 0) {
                                                    png.image.SetGamma(100000f / gm);
                                                    if (!png.image.IsHasCHRM()) {
                                                        PngChromaticities pngChromaticities = new PngChromaticities(0.3127f, 0.329f, 0.64f, 0.33f, 0.3f, 0.6f, 0.15f
                                                            , 0.06f);
                                                        png.image.SetPngChromaticities(pngChromaticities);
                                                    }
                                                }
                                            }
                                            else {
                                                if (iCCP.Equals(marker)) {
                                                    do {
                                                        --len;
                                                    }
                                                    while (pngStream.Read() != 0);
                                                    pngStream.Read();
                                                    --len;
                                                    byte[] icccom = new byte[len];
                                                    int p = 0;
                                                    while (len > 0) {
                                                        int r = pngStream.JRead(icccom, p, len);
                                                        if (r < 0) {
                                                            throw new System.IO.IOException("premature.end.of.file");
                                                        }
                                                        p += r;
                                                        len -= r;
                                                    }
                                                    byte[] iccp = FilterUtil.FlateDecode(icccom, true);
                                                    icccom = null;
                                                    try {
                                                        png.iccProfile = IccProfile.GetInstance(iccp);
                                                    }
                                                    catch (Exception) {
                                                        png.iccProfile = null;
                                                    }
                                                }
                                                else {
                                                    if (IEND.Equals(marker)) {
                                                        break;
                                                    }
                                                    else {
                                                        StreamUtil.Skip(pngStream, len);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                StreamUtil.Skip(pngStream, 4);
            }
        }

        private static bool CheckMarker(String s) {
            if (s.Length != 4) {
                return false;
            }
            for (int k = 0; k < 4; ++k) {
                char c = s[k];
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z')) {
                    return false;
                }
            }
            return true;
        }

        private static void DecodeIdat(PngImageHelper.PngParameters png) {
            int nbitDepth = png.bitDepth;
            if (nbitDepth == 16) {
                nbitDepth = 8;
            }
            int size = -1;
            png.bytesPerPixel = (png.bitDepth == 16) ? 2 : 1;
            switch (png.image.GetColorType()) {
                case 0: {
                    size = (nbitDepth * png.width + 7) / 8 * png.height;
                    break;
                }

                case 2: {
                    size = png.width * 3 * png.height;
                    png.bytesPerPixel *= 3;
                    break;
                }

                case 3: {
                    if (png.interlaceMethod == 1) {
                        size = (nbitDepth * png.width + 7) / 8 * png.height;
                    }
                    png.bytesPerPixel = 1;
                    break;
                }

                case 4: {
                    size = png.width * png.height;
                    png.bytesPerPixel *= 2;
                    break;
                }

                case 6: {
                    size = png.width * 3 * png.height;
                    png.bytesPerPixel *= 4;
                    break;
                }
            }
            if (size >= 0) {
                png.imageData = new byte[size];
            }
            if (png.palShades) {
                png.smask = new byte[png.width * png.height];
            }
            else {
                if (png.genBWMask) {
                    png.smask = new byte[(png.width + 7) / 8 * png.height];
                }
            }
            MemoryStream bai = new MemoryStream(png.idat.ToArray());
            png.dataStream = FilterUtil.GetInflaterInputStream(bai);
            if (png.interlaceMethod != 1) {
                DecodePass(0, 0, 1, 1, png.width, png.height, png);
            }
            else {
                DecodePass(0, 0, 8, 8, (png.width + 7) / 8, (png.height + 7) / 8, png);
                DecodePass(4, 0, 8, 8, (png.width + 3) / 8, (png.height + 7) / 8, png);
                DecodePass(0, 4, 4, 8, (png.width + 3) / 4, (png.height + 3) / 8, png);
                DecodePass(2, 0, 4, 4, (png.width + 1) / 4, (png.height + 3) / 4, png);
                DecodePass(0, 2, 2, 4, (png.width + 1) / 2, (png.height + 1) / 4, png);
                DecodePass(1, 0, 2, 2, png.width / 2, (png.height + 1) / 2, png);
                DecodePass(0, 1, 1, 2, png.width, png.height / 2, png);
            }
        }

        private static void DecodePass(int xOffset, int yOffset, int xStep, int yStep, int passWidth, int passHeight
            , PngImageHelper.PngParameters png) {
            if ((passWidth == 0) || (passHeight == 0)) {
                return;
            }
            int bytesPerRow = (png.inputBands * passWidth * png.bitDepth + 7) / 8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];
            // Decode the (sub)image row-by-row
            int srcY;
            int dstY;
            for (srcY = 0, dstY = yOffset; srcY < passHeight; srcY++, dstY += yStep) {
                // Read the filter type byte and a row of data
                int filter = 0;
                try {
                    filter = png.dataStream.Read();
                    StreamUtil.ReadFully(png.dataStream, curr, 0, bytesPerRow);
                }
                catch (Exception) {
                }
                // empty on purpose
                switch (filter) {
                    case PNG_FILTER_NONE: {
                        break;
                    }

                    case PNG_FILTER_SUB: {
                        DecodeSubFilter(curr, bytesPerRow, png.bytesPerPixel);
                        break;
                    }

                    case PNG_FILTER_UP: {
                        DecodeUpFilter(curr, prior, bytesPerRow);
                        break;
                    }

                    case PNG_FILTER_AVERAGE: {
                        DecodeAverageFilter(curr, prior, bytesPerRow, png.bytesPerPixel);
                        break;
                    }

                    case PNG_FILTER_PAETH: {
                        DecodePaethFilter(curr, prior, bytesPerRow, png.bytesPerPixel);
                        break;
                    }

                    default: {
                        // Error -- uknown filter type
                        throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.UnknownPngFilter);
                    }
                }
                ProcessPixels(curr, xOffset, xStep, dstY, passWidth, png);
                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }
        }

        private static void ProcessPixels(byte[] curr, int xOffset, int step, int y, int width, PngImageHelper.PngParameters
             png) {
            int srcX;
            int dstX;
            int colorType = png.image.GetColorType();
            int[] outPixel = GetPixel(curr, png);
            int sizes = 0;
            switch (colorType) {
                case 0:
                case 3:
                case 4: {
                    sizes = 1;
                    break;
                }

                case 2:
                case 6: {
                    sizes = 3;
                    break;
                }
            }
            if (png.imageData != null) {
                dstX = xOffset;
                int yStride = (sizes * png.width * (png.bitDepth == 16 ? 8 : png.bitDepth) + 7) / 8;
                for (srcX = 0; srcX < width; srcX++) {
                    SetPixel(png.imageData, outPixel, png.inputBands * srcX, sizes, dstX, y, png.bitDepth, yStride);
                    dstX += step;
                }
            }
            if (png.palShades) {
                if ((colorType & 4) != 0) {
                    if (png.bitDepth == 16) {
                        for (int k = 0; k < width; ++k) {
                            outPixel[k * png.inputBands + sizes] = (int)(((uint)outPixel[k * png.inputBands + sizes]) >> 8);
                        }
                    }
                    int yStride = png.width;
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++) {
                        SetPixel(png.smask, outPixel, png.inputBands * srcX + sizes, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
                else {
                    //colorType 3
                    int yStride = png.width;
                    int[] v = new int[1];
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++) {
                        int idx = outPixel[srcX];
                        if (idx < png.trans.Length) {
                            v[0] = png.trans[idx];
                        }
                        else {
                            // Patrick Valsecchi
                            v[0] = 255;
                        }
                        SetPixel(png.smask, v, 0, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
            }
            else {
                if (png.genBWMask) {
                    switch (colorType) {
                        case 3: {
                            int yStride = (png.width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++) {
                                int idx = outPixel[srcX];
                                v[0] = ((idx < png.trans.Length && png.trans[idx] == 0) ? 1 : 0);
                                SetPixel(png.smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }

                        case 0: {
                            int yStride = (png.width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++) {
                                int g = outPixel[srcX];
                                v[0] = (g == png.transRedGray ? 1 : 0);
                                SetPixel(png.smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }

                        case 2: {
                            int yStride = (png.width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++) {
                                int markRed = png.inputBands * srcX;
                                v[0] = (outPixel[markRed] == png.transRedGray && outPixel[markRed + 1] == png.transGreen && outPixel[markRed
                                     + 2] == png.transBlue ? 1 : 0);
                                SetPixel(png.smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static int GetPixel(byte[] image, int x, int y, int bitDepth, int bytesPerRow) {
            if (bitDepth == 8) {
                int pos = bytesPerRow * y + x;
                return image[pos] & 0xff;
            }
            else {
                int pos = bytesPerRow * y + x / (8 / bitDepth);
                int v = image[pos] >> (8 - bitDepth * (x % (8 / bitDepth)) - bitDepth);
                return v & ((1 << bitDepth) - 1);
            }
        }

        internal static void SetPixel(byte[] image, int[] data, int offset, int size, int x, int y, int bitDepth, 
            int bytesPerRow) {
            if (bitDepth == 8) {
                int pos = bytesPerRow * y + size * x;
                for (int k = 0; k < size; ++k) {
                    image[pos + k] = (byte)data[k + offset];
                }
            }
            else {
                if (bitDepth == 16) {
                    int pos = bytesPerRow * y + size * x;
                    for (int k = 0; k < size; ++k) {
                        image[pos + k] = (byte)((int)(((uint)data[k + offset]) >> 8));
                    }
                }
                else {
                    int pos = bytesPerRow * y + x / (8 / bitDepth);
                    int v = data[offset] << (8 - bitDepth * (x % (8 / bitDepth)) - bitDepth);
                    image[pos] |= (byte)v;
                }
            }
        }

        private static int[] GetPixel(byte[] curr, PngImageHelper.PngParameters png) {
            switch (png.bitDepth) {
                case 8: {
                    int[] res = new int[curr.Length];
                    for (int k = 0; k < res.Length; ++k) {
                        res[k] = curr[k] & 0xff;
                    }
                    return res;
                }

                case 16: {
                    int[] res = new int[curr.Length / 2];
                    for (int k = 0; k < res.Length; ++k) {
                        res[k] = ((curr[k * 2] & 0xff) << 8) + (curr[k * 2 + 1] & 0xff);
                    }
                    return res;
                }

                default: {
                    int[] res = new int[curr.Length * 8 / png.bitDepth];
                    int idx = 0;
                    int passes = 8 / png.bitDepth;
                    int mask = (1 << png.bitDepth) - 1;
                    for (int k = 0; k < curr.Length; ++k) {
                        for (int j = passes - 1; j >= 0; --j) {
                            res[idx++] = (curr[k] >> (png.bitDepth * j)) & mask;
                        }
                    }
                    return res;
                }
            }
        }

        private static void DecodeSubFilter(byte[] curr, int count, int bpp) {
            for (int i = bpp; i < count; i++) {
                int val = curr[i] & 0xff;
                val += curr[i - bpp] & 0xff;
                curr[i] = (byte)val;
            }
        }

        private static void DecodeUpFilter(byte[] curr, byte[] prev, int count) {
            for (int i = 0; i < count; i++) {
                int raw = curr[i] & 0xff;
                int prior = prev[i] & 0xff;
                curr[i] = (byte)(raw + prior);
            }
        }

        private static void DecodeAverageFilter(byte[] curr, byte[] prev, int count, int bpp) {
            int raw;
            int priorPixel;
            int priorRow;
            for (int i = 0; i < bpp; i++) {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;
                curr[i] = (byte)(raw + priorRow / 2);
            }
            for (int i = bpp; i < count; i++) {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;
                curr[i] = (byte)(raw + (priorPixel + priorRow) / 2);
            }
        }

        private static int PaethPredictor(int a, int b, int c) {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);
            if ((pa <= pb) && (pa <= pc)) {
                return a;
            }
            else {
                if (pb <= pc) {
                    return b;
                }
                else {
                    return c;
                }
            }
        }

        private static void DecodePaethFilter(byte[] curr, byte[] prev, int count, int bpp) {
            int raw;
            int priorPixel;
            int priorRow;
            int priorRowPixel;
            for (int i = 0; i < bpp; i++) {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;
                curr[i] = (byte)(raw + priorRow);
            }
            for (int i = bpp; i < count; i++) {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;
                priorRowPixel = prev[i - bpp] & 0xff;
                curr[i] = (byte)(raw + PaethPredictor(priorPixel, priorRow, priorRowPixel));
            }
        }

        /// <summary>Gets an <c>int</c> from an <c>InputStream</c>.</summary>
        /// <param name="pngStream">an <c>InputStream</c></param>
        /// <returns>the value of an <c>int</c></returns>
        public static int GetInt(Stream pngStream) {
            return (pngStream.Read() << 24) + (pngStream.Read() << 16) + (pngStream.Read() << 8) + pngStream.Read();
        }

        /// <summary>Gets a <c>word</c> from an <c>InputStream</c>.</summary>
        /// <param name="pngStream">an <c>InputStream</c></param>
        /// <returns>the value of an <c>int</c></returns>
        public static int GetWord(Stream pngStream) {
            return (pngStream.Read() << 8) + pngStream.Read();
        }

        /// <summary>Gets a <c>String</c> from an <c>InputStream</c>.</summary>
        /// <param name="pngStream">an <c>InputStream</c></param>
        /// <returns>the value of an <c>int</c></returns>
        public static String GetString(Stream pngStream) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < 4; i++) {
                buf.Append((char)pngStream.Read());
            }
            return buf.ToString();
        }
    }
}
