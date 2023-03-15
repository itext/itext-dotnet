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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Colors;
using iText.IO.Util;

namespace iText.IO.Image {
    internal class JpegImageHelper {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(JpegImageHelper));

        /// <summary>This is a type of marker.</summary>
        private const int NOT_A_MARKER = -1;

        /// <summary>This is a type of marker.</summary>
        private const int VALID_MARKER = 0;

        /// <summary>Acceptable Jpeg markers.</summary>
        private static readonly int[] VALID_MARKERS = new int[] { 0xC0, 0xC1, 0xC2 };

        /// <summary>This is a type of marker.</summary>
        private const int UNSUPPORTED_MARKER = 1;

        /// <summary>Unsupported Jpeg markers.</summary>
        private static readonly int[] UNSUPPORTED_MARKERS = new int[] { 0xC3, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 
            0xCB, 0xCD, 0xCE, 0xCF };

        /// <summary>This is a type of marker.</summary>
        private const int NOPARAM_MARKER = 2;

        /// <summary>Jpeg markers without additional parameters.</summary>
        private static readonly int[] NOPARAM_MARKERS = new int[] { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7
            , 0xD8, 0x01 };

        /// <summary>Marker value</summary>
        private const int M_APP0 = 0xE0;

        /// <summary>Marker value</summary>
        private const int M_APP2 = 0xE2;

        /// <summary>Marker value</summary>
        private const int M_APPE = 0xEE;

        /// <summary>Marker value for Photoshop IRB</summary>
        private const int M_APPD = 0xED;

        /// <summary>sequence that is used in all Jpeg files</summary>
        private static readonly byte[] JFIF_ID = new byte[] { 0x4A, 0x46, 0x49, 0x46, 0x00 };

        /// <summary>sequence preceding Photoshop resolution data</summary>
        private static readonly byte[] PS_8BIM_RESO = new byte[] { 0x38, 0x42, 0x49, 0x4d, 0x03, (byte)0xed };

        /// <summary>Process the passed Image data as a JPEG image.</summary>
        /// <remarks>
        /// Process the passed Image data as a JPEG image.
        /// Image is loaded and all image attributes are initialized and/or updated.
        /// </remarks>
        /// <param name="image">the image to process as a JPEG image</param>
        public static void ProcessImage(ImageData image) {
            if (image.GetOriginalType() != ImageType.JPEG) {
                throw new ArgumentException("JPEG image expected");
            }
            Stream jpegStream = null;
            try {
                String errorID;
                if (image.GetData() == null) {
                    image.LoadData();
                    errorID = image.GetUrl().ToString();
                }
                else {
                    errorID = "Byte array";
                }
                jpegStream = new MemoryStream(image.GetData());
                image.imageSize = image.GetData().Length;
                ProcessParameters(jpegStream, errorID, image);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.JpegImageException, e);
            }
            finally {
                if (jpegStream != null) {
                    try {
                        jpegStream.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
            }
            UpdateAttributes(image);
        }

        internal static void AttemptToSetIccProfileToImage(byte[][] icc, ImageData image) {
            if (icc != null) {
                int total = 0;
                foreach (byte[] value in icc) {
                    if (value == null) {
                        return;
                    }
                    total += value.Length - 14;
                }
                byte[] ficc = new byte[total];
                total = 0;
                foreach (byte[] bytes in icc) {
                    Array.Copy(bytes, 14, ficc, total, bytes.Length - 14);
                    total += bytes.Length - 14;
                }
                try {
                    image.SetProfile(IccProfile.GetInstance(ficc, image.GetColorEncodingComponentsNumber()));
                }
                catch (Exception e) {
                    LOGGER.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.DURING_CONSTRUCTION_OF_ICC_PROFILE_ERROR_OCCURRED
                        , e.GetType().Name, e.Message));
                }
            }
        }

        private static void UpdateAttributes(ImageData image) {
            image.filter = "DCTDecode";
            if (image.GetColorTransform() == 0) {
                IDictionary<String, Object> decodeParms = new Dictionary<String, Object>();
                decodeParms.Put("ColorTransform", 0);
                image.decodeParms = decodeParms;
            }
            int colorComponents = image.GetColorEncodingComponentsNumber();
            if (colorComponents != 1 && colorComponents != 3 && image.IsInverted()) {
                image.decode = new float[] { 1, 0, 1, 0, 1, 0, 1, 0 };
            }
        }

        /// <summary>This method checks if the image is a valid JPEG and processes some parameters.</summary>
        private static void ProcessParameters(Stream jpegStream, String errorID, ImageData image) {
            byte[][] icc = null;
            if (jpegStream.Read() != 0xFF || jpegStream.Read() != 0xD8) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException._1IsNotAValidJpegFile).SetMessageParams
                    (errorID);
            }
            bool firstPass = true;
            int len;
            while (true) {
                int v = jpegStream.Read();
                if (v < 0) {
                    throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.PrematureEofWhileReadingJpeg);
                }
                if (v == 0xFF) {
                    int marker = jpegStream.Read();
                    if (firstPass && marker == M_APP0) {
                        firstPass = false;
                        len = GetShort(jpegStream);
                        if (len < 16) {
                            StreamUtil.Skip(jpegStream, len - 2);
                            continue;
                        }
                        byte[] bcomp = new byte[JFIF_ID.Length];
                        int r = jpegStream.Read(bcomp);
                        if (r != bcomp.Length) {
                            throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException._1CorruptedJfifMarker).SetMessageParams
                                (errorID);
                        }
                        bool found = true;
                        for (int k = 0; k < bcomp.Length; ++k) {
                            if (bcomp[k] != JFIF_ID[k]) {
                                found = false;
                                break;
                            }
                        }
                        if (!found) {
                            StreamUtil.Skip(jpegStream, len - 2 - bcomp.Length);
                            continue;
                        }
                        StreamUtil.Skip(jpegStream, 2);
                        int units = jpegStream.Read();
                        int dx = GetShort(jpegStream);
                        int dy = GetShort(jpegStream);
                        if (units == 1) {
                            image.SetDpi(dx, dy);
                        }
                        else {
                            if (units == 2) {
                                image.SetDpi((int)(dx * 2.54f + 0.5f), (int)(dy * 2.54f + 0.5f));
                            }
                        }
                        StreamUtil.Skip(jpegStream, len - 2 - bcomp.Length - 7);
                        continue;
                    }
                    if (marker == M_APPE) {
                        len = GetShort(jpegStream) - 2;
                        byte[] byteappe = new byte[len];
                        for (int k = 0; k < len; ++k) {
                            byteappe[k] = (byte)jpegStream.Read();
                        }
                        if (byteappe.Length >= 12) {
                            String appe = iText.Commons.Utils.JavaUtil.GetStringForBytes(byteappe, 0, 5, "ISO-8859-1");
                            if (appe.Equals("Adobe")) {
                                image.SetInverted(true);
                            }
                        }
                        continue;
                    }
                    if (marker == M_APP2) {
                        len = GetShort(jpegStream) - 2;
                        byte[] byteapp2 = new byte[len];
                        for (int k = 0; k < len; ++k) {
                            byteapp2[k] = (byte)jpegStream.Read();
                        }
                        if (byteapp2.Length >= 14) {
                            String app2 = iText.Commons.Utils.JavaUtil.GetStringForBytes(byteapp2, 0, 11, "ISO-8859-1");
                            if (app2.Equals("ICC_PROFILE")) {
                                int order = byteapp2[12] & 0xff;
                                int count = byteapp2[13] & 0xff;
                                // some jpeg producers don't know how to count to 1
                                if (order < 1) {
                                    order = 1;
                                }
                                if (count < 1) {
                                    count = 1;
                                }
                                if (icc == null) {
                                    icc = new byte[count][];
                                }
                                icc[order - 1] = byteapp2;
                            }
                        }
                        continue;
                    }
                    if (marker == M_APPD) {
                        len = GetShort(jpegStream) - 2;
                        byte[] byteappd = new byte[len];
                        for (int k = 0; k < len; k++) {
                            byteappd[k] = (byte)jpegStream.Read();
                        }
                        // search for '8BIM Resolution' marker
                        int k_1;
                        for (k_1 = 0; k_1 < len - PS_8BIM_RESO.Length; k_1++) {
                            bool found = true;
                            for (int j = 0; j < PS_8BIM_RESO.Length; j++) {
                                if (byteappd[k_1 + j] != PS_8BIM_RESO[j]) {
                                    found = false;
                                    break;
                                }
                            }
                            if (found) {
                                break;
                            }
                        }
                        k_1 += PS_8BIM_RESO.Length;
                        if (k_1 < len - PS_8BIM_RESO.Length) {
                            // "PASCAL String" for name, i.e. string prefix with length byte
                            // padded to be even length; 2 null bytes if empty
                            byte namelength = byteappd[k_1];
                            // add length byte
                            namelength++;
                            // add padding
                            if (namelength % 2 == 1) {
                                namelength++;
                            }
                            // just skip name
                            k_1 += namelength;
                            // size of the resolution data
                            int resosize = (byteappd[k_1] << 24) + (byteappd[k_1 + 1] << 16) + (byteappd[k_1 + 2] << 8) + byteappd[k_1
                                 + 3];
                            // should be 16
                            if (resosize != 16) {
                                // fail silently, for now
                                //System.err.println("DEBUG: unsupported resolution IRB size");
                                continue;
                            }
                            k_1 += 4;
                            int dx = (byteappd[k_1] << 8) + (byteappd[k_1 + 1] & 0xff);
                            k_1 += 2;
                            // skip 2 unknown bytes
                            k_1 += 2;
                            int unitsx = (byteappd[k_1] << 8) + (byteappd[k_1 + 1] & 0xff);
                            k_1 += 2;
                            // skip 2 unknown bytes
                            k_1 += 2;
                            int dy = (byteappd[k_1] << 8) + (byteappd[k_1 + 1] & 0xff);
                            k_1 += 2;
                            // skip 2 unknown bytes
                            k_1 += 2;
                            int unitsy = (byteappd[k_1] << 8) + (byteappd[k_1 + 1] & 0xff);
                            if (unitsx == 1 || unitsx == 2) {
                                dx = (unitsx == 2 ? (int)(dx * 2.54f + 0.5f) : dx);
                                // make sure this is consistent with JFIF data
                                if (image.GetDpiX() != 0 && image.GetDpiX() != dx) {
                                    LOGGER.LogDebug(MessageFormatUtil.Format("Inconsistent metadata (dpiX: {0} vs {1})", image.GetDpiX(), dx));
                                }
                                else {
                                    image.SetDpi(dx, image.GetDpiY());
                                }
                            }
                            if (unitsy == 1 || unitsy == 2) {
                                dy = (unitsy == 2 ? (int)(dy * 2.54f + 0.5f) : dy);
                                // make sure this is consistent with JFIF data
                                if (image.GetDpiY() != 0 && image.GetDpiY() != dy) {
                                    LOGGER.LogDebug(MessageFormatUtil.Format("Inconsistent metadata (dpiY: {0} vs {1})", image.GetDpiY(), dy));
                                }
                                else {
                                    image.SetDpi(image.GetDpiX(), dx);
                                }
                            }
                        }
                        continue;
                    }
                    firstPass = false;
                    int markertype = Marker(marker);
                    if (markertype == VALID_MARKER) {
                        StreamUtil.Skip(jpegStream, 2);
                        if (jpegStream.Read() != 0x08) {
                            throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException._1MustHave8BitsPerComponent).SetMessageParams
                                (errorID);
                        }
                        image.SetHeight(GetShort(jpegStream));
                        image.SetWidth(GetShort(jpegStream));
                        image.SetColorEncodingComponentsNumber(jpegStream.Read());
                        image.SetBpc(8);
                        break;
                    }
                    else {
                        if (markertype == UNSUPPORTED_MARKER) {
                            throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException._1UnsupportedJpegMarker2).SetMessageParams
                                (errorID, JavaUtil.IntegerToString(marker));
                        }
                        else {
                            if (markertype != NOPARAM_MARKER) {
                                StreamUtil.Skip(jpegStream, GetShort(jpegStream) - 2);
                            }
                        }
                    }
                }
            }
            AttemptToSetIccProfileToImage(icc, image);
        }

        /// <summary>Reads a short from the <c>InputStream</c>.</summary>
        /// <param name="jpegStream">the <c>InputStream</c></param>
        /// <returns>an int</returns>
        private static int GetShort(Stream jpegStream) {
            return (jpegStream.Read() << 8) + jpegStream.Read();
        }

        /// <summary>Returns a type of marker.</summary>
        /// <param name="marker">an int</param>
        /// <returns>a type: <var>VALID_MARKER</var>, <var>UNSUPPORTED_MARKER</var> or <var>NOPARAM_MARKER</var></returns>
        private static int Marker(int marker) {
            for (int i = 0; i < VALID_MARKERS.Length; i++) {
                if (marker == VALID_MARKERS[i]) {
                    return VALID_MARKER;
                }
            }
            for (int i = 0; i < NOPARAM_MARKERS.Length; i++) {
                if (marker == NOPARAM_MARKERS[i]) {
                    return NOPARAM_MARKER;
                }
            }
            for (int i = 0; i < UNSUPPORTED_MARKERS.Length; i++) {
                if (marker == UNSUPPORTED_MARKERS[i]) {
                    return UNSUPPORTED_MARKER;
                }
            }
            return NOT_A_MARKER;
        }
    }
}
