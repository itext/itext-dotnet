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
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.IO.Colors;
using iText.IO.Font;
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

            internal int colorType;

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

            internal int bytesPerPixel;

            internal byte[] colorTable;

            internal float gamma = 1f;

            internal bool hasCHRM = false;

            internal float xW;

            internal float yW;

            internal float xR;

            internal float yR;

            internal float xG;

            internal float yG;

            internal float xB;

            internal float yB;

            internal String intent;

            internal IccProfile iccProfile;
            // number of bytes per input pixel
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

        private static readonly String[] intents = new String[] { "/Perceptual", "/RelativeColorimetric", "/Saturation"
            , "/AbsoluteColorimetric" };

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
                throw new iText.IO.IOException(iText.IO.IOException.PngImageException, e);
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

        /// <exception cref="System.IO.IOException"/>
        private static void ProcessPng(Stream pngStream, PngImageHelper.PngParameters png) {
            ReadPng(pngStream, png);
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
                if ((png.colorType & 4) != 0) {
                    png.palShades = true;
                }
                png.genBWMask = (!png.palShades && (pal0 > 1 || png.transRedGray >= 0));
                if (!png.palShades && !png.genBWMask && pal0 == 1) {
                    png.additional["Mask"] = String.Format("[{0} {1}]", palIdx, palIdx);
                }
                bool needDecode = (png.interlaceMethod == 1) || (png.bitDepth == 16) || ((png.colorType & 4) != 0) || png.
                    palShades || png.genBWMask;
                switch (png.colorType) {
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
                if ((png.colorType & 4) != 0) {
                    --components;
                }
                int bpc = png.bitDepth;
                if (bpc == 16) {
                    bpc = 8;
                }
                if (png.imageData != null) {
                    if (png.colorType == 3) {
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
                    decodeparms["BitsPerComponent"] = png.bitDepth;
                    decodeparms["Predictor"] = 15;
                    decodeparms["Columns"] = png.width;
                    decodeparms["Colors"] = (png.colorType == 3 || (png.colorType & 2) == 0) ? 1 : 3;
                    png.image.decodeParms = decodeparms;
                }
                if (png.additional.Get("ColorSpace") == null) {
                    png.additional["ColorSpace"] = GetColorspace(png);
                }
                if (png.intent != null) {
                    png.additional["Intent"] = png.intent;
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
                throw new iText.IO.IOException(iText.IO.IOException.PngImageException, e);
            }
        }

        private static Object GetColorspace(PngImageHelper.PngParameters png) {
            if (png.iccProfile != null) {
                if ((png.colorType & 2) == 0) {
                    return "/DeviceGray";
                }
                else {
                    return "/DeviceRGB";
                }
            }
            if (png.gamma == 1f && !png.hasCHRM) {
                if ((png.colorType & 2) == 0) {
                    return "/DeviceGray";
                }
                else {
                    return "/DeviceRGB";
                }
            }
            else {
                Object[] array = new Object[2];
                IDictionary<String, Object> map = new Dictionary<String, Object>();
                if ((png.colorType & 2) == 0) {
                    if (png.gamma == 1f) {
                        return "/DeviceGray";
                    }
                    array[0] = "/CalGray";
                    map["Gamma"] = png.gamma;
                    map["WhitePoint"] = new int[] { 1, 1, 1 };
                    array[1] = map;
                }
                else {
                    float[] wp = new float[] { 1, 1, 1 };
                    array[0] = "/CalRGB";
                    if (png.gamma != 1f) {
                        float[] gm = new float[3];
                        gm[0] = png.gamma;
                        gm[1] = png.gamma;
                        gm[2] = png.gamma;
                        map["Gamma"] = gm;
                    }
                    if (png.hasCHRM) {
                        float z = png.yW * ((png.xG - png.xB) * png.yR - (png.xR - png.xB) * png.yG + (png.xR - png.xG) * png.yB);
                        float YA = png.yR * ((png.xG - png.xB) * png.yW - (png.xW - png.xB) * png.yG + (png.xW - png.xG) * png.yB)
                             / z;
                        float XA = YA * png.xR / png.yR;
                        float ZA = YA * ((1 - png.xR) / png.yR - 1);
                        float YB = -png.yG * ((png.xR - png.xB) * png.yW - (png.xW - png.xB) * png.yR + (png.xW - png.xR) * png.yB
                            ) / z;
                        float XB = YB * png.xG / png.yG;
                        float ZB = YB * ((1 - png.xG) / png.yG - 1);
                        float YC = png.yB * ((png.xR - png.xG) * png.yW - (png.xW - png.xG) * png.yW + (png.xW - png.xR) * png.yG)
                             / z;
                        float XC = YC * png.xB / png.yB;
                        float ZC = YC * ((1 - png.xB) / png.yB - 1);
                        float XW = XA + XB + XC;
                        float YW = 1;
                        //YA+YB+YC;
                        float ZW = ZA + ZB + ZC;
                        float[] wpa = new float[3];
                        wpa[0] = XW;
                        wpa[1] = YW;
                        wpa[2] = ZW;
                        wp = wpa;
                        float[] matrix = new float[9];
                        matrix[0] = XA;
                        matrix[1] = YA;
                        matrix[2] = ZA;
                        matrix[3] = XB;
                        matrix[4] = YB;
                        matrix[5] = ZB;
                        matrix[6] = XC;
                        matrix[7] = YC;
                        matrix[8] = ZC;
                        map["Matrix"] = matrix;
                    }
                    map["WhitePoint"] = wp;
                    array[1] = map;
                }
                return array;
            }
        }

        /// <exception cref="System.IO.IOException"/>
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
                        switch (png.colorType) {
                            case 0: {
                                if (len >= 2) {
                                    len -= 2;
                                    int gray = GetWord(pngStream);
                                    if (png.bitDepth == 16) {
                                        png.transRedGray = gray;
                                    }
                                    else {
                                        png.additional["Mask"] = String.Format("[{0} {1}]", gray, gray);
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
                                        png.additional["Mask"] = String.Format("[{0} {1} {2} {3} {4} {5}]", red, red, green, green, blue, blue);
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
                            png.colorType = pngStream.Read();
                            png.compressionMethod = pngStream.Read();
                            png.filterMethod = pngStream.Read();
                            png.interlaceMethod = pngStream.Read();
                        }
                        else {
                            if (PLTE.Equals(marker)) {
                                if (png.colorType == 3) {
                                    Object[] colorspace = new Object[4];
                                    colorspace[0] = "/Indexed";
                                    colorspace[1] = GetColorspace(png);
                                    colorspace[2] = len / 3 - 1;
                                    ByteBuffer colorTableBuf = new ByteBuffer();
                                    while ((len--) > 0) {
                                        colorTableBuf.Append(pngStream.Read());
                                    }
                                    png.colorTable = colorTableBuf.ToByteArray();
                                    colorspace[3] = PdfEncodings.ConvertToString(png.colorTable, null);
                                    png.additional["ColorSpace"] = colorspace;
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
                                        png.xW = GetInt(pngStream) / 100000f;
                                        png.yW = GetInt(pngStream) / 100000f;
                                        png.xR = GetInt(pngStream) / 100000f;
                                        png.yR = GetInt(pngStream) / 100000f;
                                        png.xG = GetInt(pngStream) / 100000f;
                                        png.yG = GetInt(pngStream) / 100000f;
                                        png.xB = GetInt(pngStream) / 100000f;
                                        png.yB = GetInt(pngStream) / 100000f;
                                        png.hasCHRM = !(Math.Abs(png.xW) < 0.0001f || Math.Abs(png.yW) < 0.0001f || Math.Abs(png.xR) < 0.0001f || 
                                            Math.Abs(png.yR) < 0.0001f || Math.Abs(png.xG) < 0.0001f || Math.Abs(png.yG) < 0.0001f || Math.Abs(png
                                            .xB) < 0.0001f || Math.Abs(png.yB) < 0.0001f);
                                    }
                                    else {
                                        if (sRGB.Equals(marker)) {
                                            int ri = pngStream.Read();
                                            png.intent = intents[ri];
                                            png.gamma = 2.2f;
                                            png.xW = 0.3127f;
                                            png.yW = 0.329f;
                                            png.xR = 0.64f;
                                            png.yR = 0.33f;
                                            png.xG = 0.3f;
                                            png.yG = 0.6f;
                                            png.xB = 0.15f;
                                            png.yB = 0.06f;
                                            png.hasCHRM = true;
                                        }
                                        else {
                                            if (gAMA.Equals(marker)) {
                                                int gm = GetInt(pngStream);
                                                if (gm != 0) {
                                                    png.gamma = 100000f / gm;
                                                    if (!png.hasCHRM) {
                                                        png.xW = 0.3127f;
                                                        png.yW = 0.329f;
                                                        png.xR = 0.64f;
                                                        png.yR = 0.33f;
                                                        png.xG = 0.3f;
                                                        png.yG = 0.6f;
                                                        png.xB = 0.15f;
                                                        png.yB = 0.06f;
                                                        png.hasCHRM = true;
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
            switch (png.colorType) {
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
                switch (filter) {
                    case PNG_FILTER_NONE: {
                        // empty on purpose
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
                        throw new iText.IO.IOException(iText.IO.IOException.UnknownPngFilter);
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
            int[] outPixel = GetPixel(curr, png);
            int sizes = 0;
            switch (png.colorType) {
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
                if ((png.colorType & 4) != 0) {
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
                            v[0] = 255;
                        }
                        // Patrick Valsecchi
                        SetPixel(png.smask, v, 0, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
            }
            else {
                if (png.genBWMask) {
                    switch (png.colorType) {
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

        /// <summary>Gets an <CODE>int</CODE> from an <CODE>InputStream</CODE>.</summary>
        /// <param name="pngStream">an <CODE>InputStream</CODE></param>
        /// <returns>the value of an <CODE>int</CODE></returns>
        /// <exception cref="System.IO.IOException"/>
        public static int GetInt(Stream pngStream) {
            return (pngStream.Read() << 24) + (pngStream.Read() << 16) + (pngStream.Read() << 8) + pngStream.Read();
        }

        /// <summary>Gets a <CODE>word</CODE> from an <CODE>InputStream</CODE>.</summary>
        /// <param name="pngStream">an <CODE>InputStream</CODE></param>
        /// <returns>the value of an <CODE>int</CODE></returns>
        /// <exception cref="System.IO.IOException"/>
        public static int GetWord(Stream pngStream) {
            return (pngStream.Read() << 8) + pngStream.Read();
        }

        /// <summary>Gets a <CODE>String</CODE> from an <CODE>InputStream</CODE>.</summary>
        /// <param name="pngStream">an <CODE>InputStream</CODE></param>
        /// <returns>the value of an <CODE>int</CODE></returns>
        /// <exception cref="System.IO.IOException"/>
        public static String GetString(Stream pngStream) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < 4; i++) {
                buf.Append((char)pngStream.Read());
            }
            return buf.ToString();
        }
    }
}
