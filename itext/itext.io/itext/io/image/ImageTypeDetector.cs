/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.IO;
using iText.IO.Util;

namespace iText.IO.Image {
    /// <summary>Helper class that detects image type by magic bytes</summary>
    public sealed class ImageTypeDetector {
        private static readonly byte[] gif = new byte[] { (byte)'G', (byte)'I', (byte)'F' };

        private static readonly byte[] jpeg = new byte[] { (byte)0xFF, (byte)0xD8 };

        private static readonly byte[] jpeg2000_1 = new byte[] { 0x00, 0x00, 0x00, 0x0c };

        private static readonly byte[] jpeg2000_2 = new byte[] { (byte)0xff, (byte)0x4f, (byte)0xff, 0x51 };

        private static readonly byte[] png = new byte[] { (byte)137, 80, 78, 71 };

        private static readonly byte[] wmf = new byte[] { (byte)0xD7, (byte)0xCD };

        private static readonly byte[] bmp = new byte[] { (byte)'B', (byte)'M' };

        private static readonly byte[] tiff_1 = new byte[] { (byte)'M', (byte)'M', 0, 42 };

        private static readonly byte[] tiff_2 = new byte[] { (byte)'I', (byte)'I', 42, 0 };

        private static readonly byte[] jbig2 = new byte[] { (byte)0x97, (byte)'J', (byte)'B', (byte)'2', (byte)'\r'
            , (byte)'\n', 0x1a, (byte)'\n' };

        private ImageTypeDetector() {
        }

        /// <summary>Detect image type by magic bytes given the byte array source.</summary>
        /// <param name="source">image bytes</param>
        /// <returns>
        /// detected image type, see
        /// <see cref="ImageType"/>
        /// . Returns
        /// <see cref="ImageType.NONE"/>
        /// if image type is unknown
        /// </returns>
        public static ImageType DetectImageType(byte[] source) {
            byte[] header = ReadImageType(source);
            return DetectImageTypeByHeader(header);
        }

        /// <summary>Detect image type by magic bytes given the source URL.</summary>
        /// <param name="source">image URL</param>
        /// <returns>
        /// detected image type, see
        /// <see cref="ImageType"/>
        /// . Returns
        /// <see cref="ImageType.NONE"/>
        /// if image type is unknown
        /// </returns>
        public static ImageType DetectImageType(Uri source) {
            byte[] header = ReadImageType(source);
            return DetectImageTypeByHeader(header);
        }

        /// <summary>Detect image type by magic bytes given the input stream.</summary>
        /// <param name="stream">image stream</param>
        /// <returns>
        /// detected image type, see
        /// <see cref="ImageType"/>
        /// . Returns
        /// <see cref="ImageType.NONE"/>
        /// if image type is unknown
        /// </returns>
        public static ImageType DetectImageType(Stream stream) {
            byte[] header = ReadImageType(stream);
            return DetectImageTypeByHeader(header);
        }

        private static ImageType DetectImageTypeByHeader(byte[] header) {
            if (ImageTypeIs(header, gif)) {
                return ImageType.GIF;
            }
            else {
                if (ImageTypeIs(header, jpeg)) {
                    return ImageType.JPEG;
                }
                else {
                    if (ImageTypeIs(header, jpeg2000_1) || ImageTypeIs(header, jpeg2000_2)) {
                        return ImageType.JPEG2000;
                    }
                    else {
                        if (ImageTypeIs(header, png)) {
                            return ImageType.PNG;
                        }
                        else {
                            if (ImageTypeIs(header, bmp)) {
                                return ImageType.BMP;
                            }
                            else {
                                if (ImageTypeIs(header, tiff_1) || ImageTypeIs(header, tiff_2)) {
                                    return ImageType.TIFF;
                                }
                                else {
                                    if (ImageTypeIs(header, jbig2)) {
                                        return ImageType.JBIG2;
                                    }
                                    else {
                                        if (ImageTypeIs(header, wmf)) {
                                            return ImageType.WMF;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ImageType.NONE;
        }

        private static bool ImageTypeIs(byte[] imageType, byte[] compareWith) {
            for (int i = 0; i < compareWith.Length; i++) {
                if (imageType[i] != compareWith[i]) {
                    return false;
                }
            }
            return true;
        }

        private static byte[] ReadImageType(Uri source) {
            try {
                using (Stream stream = UrlUtil.OpenStream(source)) {
                    return ReadImageType(stream);
                }
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        private static byte[] ReadImageType(Stream stream) {
            try {
                byte[] bytes = new byte[8];
                stream.Read(bytes);
                return bytes;
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IoException, e);
            }
        }

        private static byte[] ReadImageType(byte[] source) {
            try {
                Stream stream = new MemoryStream(source);
                byte[] bytes = new byte[8];
                stream.Read(bytes);
                return bytes;
            }
            catch (System.IO.IOException) {
                return null;
            }
        }
    }
}
