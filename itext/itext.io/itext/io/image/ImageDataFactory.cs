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
#if !NETSTANDARD1_6
using System.Drawing;
#endif
using System.IO;
using iText.IO.Codec;
using iText.IO.Util;

namespace iText.IO.Image {
    public sealed class ImageDataFactory {
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

        private ImageDataFactory() {
        }

        /// <summary>Create an ImageData instance representing the image from the image bytes.</summary>
        /// <param name="bytes">byte representation of the image.</param>
        /// <param name="recoverImage">whether to recover from a image error (for TIFF-images)</param>
        /// <returns>The created ImageData object.</returns>
        public static ImageData Create(byte[] bytes, bool recoverImage) {
            return CreateImageInstance(bytes, recoverImage);
        }

        /// <summary>Create an ImageData instance representing the image from the image bytes.</summary>
        /// <param name="bytes">byte representation of the image.</param>
        /// <returns>The created ImageData object.</returns>
        public static ImageData Create(byte[] bytes) {
            return Create(bytes, false);
        }

        /// <summary>Create an ImageData instance representing the image from the file located at the specified url.</summary>
        /// <param name="url">location of the image</param>
        /// <param name="recoverImage">whether to recover from a image error (for TIFF-images)</param>
        /// <returns>The created ImageData object.</returns>
        public static ImageData Create(Uri url, bool recoverImage) {
            return CreateImageInstance(url, recoverImage);
        }

        /// <summary>Create an ImageData instance representing the image from the file located at the specified url.</summary>
        /// <param name="url">location of the image</param>
        /// <returns>The created ImageData object.</returns>
        public static ImageData Create(Uri url) {
            return Create(url, false);
        }

        /// <summary>Create an ImageData instance representing the image from the specified file.</summary>
        /// <param name="filename">filename of the file containing the image</param>
        /// <param name="recoverImage">whether to recover from a image error (for TIFF-images)</param>
        /// <returns>The created ImageData object.</returns>
        /// <exception cref="Java.Net.MalformedURLException"/>
        public static ImageData Create(String filename, bool recoverImage) {
            return Create(UrlUtil.ToURL(filename), recoverImage);
        }

        /// <summary>Create an ImageData instance representing the image from the specified file.</summary>
        /// <param name="filename">filename of the file containing the image</param>
        /// <returns>The created ImageData object.</returns>
        /// <exception cref="Java.Net.MalformedURLException"/>
        public static ImageData Create(String filename) {
            return Create(filename, false);
        }

        /// <summary>Create an ImageData instance from the passed parameters.</summary>
        /// <param name="width">width of the image in pixels</param>
        /// <param name="height">height of the image in pixels</param>
        /// <param name="reverseBits">whether to reverse the bits stored in data (TIFF images).</param>
        /// <param name="typeCCITT">Type of CCITT encoding</param>
        /// <param name="parameters">colour space parameters</param>
        /// <param name="data">array containing raw image data</param>
        /// <param name="transparency">array containing transparency information</param>
        /// <returns>created ImageData object.</returns>
        public static ImageData Create(int width, int height, bool reverseBits, int typeCCITT, int parameters, byte
            [] data, int[] transparency) {
            if (transparency != null && transparency.Length != 2) {
                throw new iText.IO.IOException(iText.IO.IOException.TransparencyLengthMustBeEqualTo2WithCcittImages);
            }
            if (typeCCITT != RawImageData.CCITTG4 && typeCCITT != RawImageData.CCITTG3_1D && typeCCITT != RawImageData
                .CCITTG3_2D) {
                throw new iText.IO.IOException(iText.IO.IOException.CcittCompressionTypeMustBeCcittg4Ccittg3_1dOrCcittg3_2d
                    );
            }
            if (reverseBits) {
                TIFFFaxDecoder.ReverseBits(data);
            }
            RawImageData image = new RawImageData(data, ImageType.RAW);
            image.SetTypeCcitt(typeCCITT);
            image.height = height;
            image.width = width;
            image.colorSpace = parameters;
            image.transparency = transparency;
            return image;
        }

        /// <summary>Create an ImageData instance from the passed parameters.</summary>
        /// <param name="width">width of the image in pixels</param>
        /// <param name="height">height of the image in pixels</param>
        /// <param name="components">colour space components</param>
        /// <param name="bpc">bits per colour.</param>
        /// <param name="data">array containing raw image data</param>
        /// <param name="transparency">array containing transparency information</param>
        /// <returns>created ImageData object.</returns>
        public static ImageData Create(int width, int height, int components, int bpc, byte[] data, int[] transparency
            ) {
            if (transparency != null && transparency.Length != components * 2) {
                throw new iText.IO.IOException(iText.IO.IOException.TransparencyLengthMustBeEqualTo2WithCcittImages);
            }
            if (components == 1 && bpc == 1) {
                byte[] g4 = CCITTG4Encoder.Compress(data, width, height);
                return iText.IO.Image.ImageDataFactory.Create(width, height, false, RawImageData.CCITTG4, RawImageData.CCITT_BLACKIS1
                    , g4, transparency);
            }
            RawImageData image = new RawImageData(data, ImageType.RAW);
            image.height = height;
            image.width = width;
            if (components != 1 && components != 3 && components != 4) {
                throw new iText.IO.IOException(iText.IO.IOException.ComponentsMustBe1_3Or4);
            }
            if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8) {
                throw new iText.IO.IOException(iText.IO.IOException.BitsPerComponentMustBe1_2_4or8);
            }
            image.colorSpace = components;
            image.bpc = bpc;
            image.data = data;
            image.transparency = transparency;
            return image;
        }

#if !NETSTANDARD1_6
        /// <summary>Gets an instance of an Image from a java.awt.Image</summary>
        /// <param name="image">the java.awt.Image to convert</param>
        /// <param name="color">if different from <CODE>null</CODE> the transparency pixels are replaced by this color
        ///     </param>
        /// <returns>RawImage</returns>
        /// <exception cref="System.IO.IOException"/>
        public static ImageData Create(System.Drawing.Image image, Color? color) {
            return iText.IO.Image.ImageDataFactory.Create(image, color, false);
        }

        /// <summary>Gets an instance of an Image from a java.awt.Image.</summary>
        /// <param name="image">the <CODE>java.awt.Image</CODE> to convert</param>
        /// <param name="color">if different from <CODE>null</CODE> the transparency pixels are replaced by this color
        ///     </param>
        /// <param name="forceBW">if <CODE>true</CODE> the image is treated as black and white</param>
        /// <returns>RawImage</returns>
        /// <exception cref="System.IO.IOException"/>
        public static ImageData Create(System.Drawing.Image image, Color? color, bool forceBW) {
            return DrawingImageFactory.GetImage(image, color, forceBW);
        }
#endif

        /// <summary>Get a bitmap ImageData instance from the specified url.</summary>
        /// <param name="url">location of the image.</param>
        /// <param name="noHeader">Whether the image contains a header.</param>
        /// <param name="size">size of the image</param>
        /// <returns>created ImageData.</returns>
        public static ImageData CreateBmp(Uri url, bool noHeader, int size) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, bmp)) {
                ImageData image = new BmpImageData(url, noHeader, size);
                BmpImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("BMP image expected.");
        }

        /// <summary>Get a bitmap ImageData instance from the provided bytes.</summary>
        /// <param name="bytes">array containing the raw image data</param>
        /// <param name="noHeader">Whether the image contains a header.</param>
        /// <param name="size">size of the image</param>
        /// <returns>created ImageData.</returns>
        public static ImageData CreateBmp(byte[] bytes, bool noHeader, int size) {
            byte[] imageType = ReadImageType(bytes);
            if (noHeader || ImageTypeIs(imageType, bmp)) {
                ImageData image = new BmpImageData(bytes, noHeader, size);
                BmpImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("BMP image expected.");
        }

        /// <summary>Return a GifImage object.</summary>
        /// <remarks>Return a GifImage object. This object cannot be added to a document</remarks>
        /// <param name="bytes">array containing the raw image data</param>
        /// <returns>GifImageData instance.</returns>
        public static GifImageData CreateGif(byte[] bytes) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(bytes);
                GifImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns a specified frame of the gif image</summary>
        /// <param name="url">url of gif image</param>
        /// <param name="frame">number of frame to be returned</param>
        /// <returns>GifImageData instance.</returns>
        public static ImageData CreateGifFrame(Uri url, int frame) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(url);
                GifImageHelper.ProcessImage(image, frame - 1);
                return image.GetFrames()[frame - 1];
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns a specified frame of the gif image</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <param name="frame">number of frame to be returned</param>
        /// <returns/>
        public static ImageData CreateGifFrame(byte[] bytes, int frame) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(bytes);
                GifImageHelper.ProcessImage(image, frame - 1);
                return image.GetFrames()[frame - 1];
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns <CODE>List</CODE> of gif image frames</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <param name="frameNumbers">array of frame numbers of gif image</param>
        /// <returns/>
        public static IList<ImageData> CreateGifFrames(byte[] bytes, int[] frameNumbers) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(bytes);
                iText.IO.Util.JavaUtil.Sort(frameNumbers);
                GifImageHelper.ProcessImage(image, frameNumbers[frameNumbers.Length - 1] - 1);
                IList<ImageData> frames = new List<ImageData>();
                foreach (int frame in frameNumbers) {
                    frames.Add(image.GetFrames()[frame - 1]);
                }
                return frames;
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns <CODE>List</CODE> of gif image frames</summary>
        /// <param name="url">url of gif image</param>
        /// <param name="frameNumbers">array of frame numbers of gif image</param>
        /// <returns/>
        public static IList<ImageData> CreateGifFrames(Uri url, int[] frameNumbers) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(url);
                iText.IO.Util.JavaUtil.Sort(frameNumbers);
                GifImageHelper.ProcessImage(image, frameNumbers[frameNumbers.Length - 1] - 1);
                IList<ImageData> frames = new List<ImageData>();
                foreach (int frame in frameNumbers) {
                    frames.Add(image.GetFrames()[frame - 1]);
                }
                return frames;
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns <CODE>List</CODE> of gif image frames</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(byte[] bytes) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(bytes);
                GifImageHelper.ProcessImage(image);
                return image.GetFrames();
            }
            throw new ArgumentException("GIF image expected.");
        }

        /// <summary>Returns <CODE>List</CODE> of gif image frames</summary>
        /// <param name="url">url of gif image</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(Uri url) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(url);
                GifImageHelper.ProcessImage(image);
                return image.GetFrames();
            }
            throw new ArgumentException("GIF image expected.");
        }

        public static ImageData CreateJbig2(Uri url, int page) {
            if (page < 1) {
                throw new ArgumentException("The page number must be greater than 0");
            }
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, jbig2)) {
                ImageData image = new Jbig2ImageData(url, page);
                Jbig2ImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JBIG2 image expected.");
        }

        public static ImageData CreateJbig2(byte[] bytes, int page) {
            if (page < 1) {
                throw new ArgumentException("The page number must be greater than 0");
            }
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, jbig2)) {
                ImageData image = new Jbig2ImageData(bytes, page);
                Jbig2ImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JBIG2 image expected.");
        }

        /// <summary>Create a ImageData instance from a Jpeg image url</summary>
        /// <param name="url"/>
        /// <returns/>
        public static ImageData CreateJpeg(Uri url) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, jpeg)) {
                ImageData image = new JpegImageData(url);
                JpegImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JPEG image expected.");
        }

        public static ImageData CreateJpeg(byte[] bytes) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, jpeg)) {
                ImageData image = new JpegImageData(bytes);
                JpegImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JPEG image expected.");
        }

        public static ImageData CreateJpeg2000(Uri url) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, jpeg2000_1) || ImageTypeIs(imageType, jpeg2000_2)) {
                ImageData image = new Jpeg2000ImageData(url);
                Jpeg2000ImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JPEG2000 image expected.");
        }

        public static ImageData CreateJpeg2000(byte[] bytes) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, jpeg2000_1) || ImageTypeIs(imageType, jpeg2000_2)) {
                ImageData image = new Jpeg2000ImageData(bytes);
                Jpeg2000ImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("JPEG2000 image expected.");
        }

        public static ImageData CreatePng(Uri url) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, png)) {
                ImageData image = new PngImageData(url);
                PngImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("PNG image expected.");
        }

        public static ImageData CreatePng(byte[] bytes) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, png)) {
                ImageData image = new PngImageData(bytes);
                PngImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("PNG image expected.");
        }

        public static ImageData CreateTiff(Uri url, bool recoverFromImageError, int page, bool direct) {
            byte[] imageType = ReadImageType(url);
            if (ImageTypeIs(imageType, tiff_1) || ImageTypeIs(imageType, tiff_2)) {
                ImageData image = new TiffImageData(url, recoverFromImageError, page, direct);
                TiffImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("TIFF image expected.");
        }

        public static ImageData CreateTiff(byte[] bytes, bool recoverFromImageError, int page, bool direct) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, tiff_1) || ImageTypeIs(imageType, tiff_2)) {
                ImageData image = new TiffImageData(bytes, recoverFromImageError, page, direct);
                TiffImageHelper.ProcessImage(image);
                return image;
            }
            throw new ArgumentException("TIFF image expected.");
        }

        public static ImageData CreateRawImage(byte[] bytes) {
            return new RawImageData(bytes, ImageType.RAW);
        }

        private static ImageData CreateImageInstance(Uri source, bool recoverImage) {
            byte[] imageType = ReadImageType(source);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(source);
                GifImageHelper.ProcessImage(image, 0);
                return image.GetFrames()[0];
            }
            else {
                if (ImageTypeIs(imageType, jpeg)) {
                    ImageData image = new JpegImageData(source);
                    JpegImageHelper.ProcessImage(image);
                    return image;
                }
                else {
                    if (ImageTypeIs(imageType, jpeg2000_1) || ImageTypeIs(imageType, jpeg2000_2)) {
                        ImageData image = new Jpeg2000ImageData(source);
                        Jpeg2000ImageHelper.ProcessImage(image);
                        return image;
                    }
                    else {
                        if (ImageTypeIs(imageType, png)) {
                            ImageData image = new PngImageData(source);
                            PngImageHelper.ProcessImage(image);
                            return image;
                        }
                        else {
                            if (ImageTypeIs(imageType, bmp)) {
                                ImageData image = new BmpImageData(source, false, 0);
                                BmpImageHelper.ProcessImage(image);
                                return image;
                            }
                            else {
                                if (ImageTypeIs(imageType, tiff_1) || ImageTypeIs(imageType, tiff_2)) {
                                    ImageData image = new TiffImageData(source, recoverImage, 1, false);
                                    TiffImageHelper.ProcessImage(image);
                                    return image;
                                }
                                else {
                                    if (ImageTypeIs(imageType, jbig2)) {
                                        ImageData image = new Jbig2ImageData(source, 1);
                                        Jbig2ImageHelper.ProcessImage(image);
                                        return image;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new iText.IO.IOException(iText.IO.IOException.ImageFormatCannotBeRecognized);
        }

        private static ImageData CreateImageInstance(byte[] bytes, bool recoverImage) {
            byte[] imageType = ReadImageType(bytes);
            if (ImageTypeIs(imageType, gif)) {
                GifImageData image = new GifImageData(bytes);
                GifImageHelper.ProcessImage(image, 0);
                return image.GetFrames()[0];
            }
            else {
                if (ImageTypeIs(imageType, jpeg)) {
                    ImageData image = new JpegImageData(bytes);
                    JpegImageHelper.ProcessImage(image);
                    return image;
                }
                else {
                    if (ImageTypeIs(imageType, jpeg2000_1) || ImageTypeIs(imageType, jpeg2000_2)) {
                        ImageData image = new Jpeg2000ImageData(bytes);
                        Jpeg2000ImageHelper.ProcessImage(image);
                        return image;
                    }
                    else {
                        if (ImageTypeIs(imageType, png)) {
                            ImageData image = new PngImageData(bytes);
                            PngImageHelper.ProcessImage(image);
                            return image;
                        }
                        else {
                            if (ImageTypeIs(imageType, bmp)) {
                                ImageData image = new BmpImageData(bytes, false, 0);
                                BmpImageHelper.ProcessImage(image);
                                return image;
                            }
                            else {
                                if (ImageTypeIs(imageType, tiff_1) || ImageTypeIs(imageType, tiff_2)) {
                                    ImageData image = new TiffImageData(bytes, recoverImage, 1, false);
                                    TiffImageHelper.ProcessImage(image);
                                    return image;
                                }
                                else {
                                    if (ImageTypeIs(imageType, jbig2)) {
                                        ImageData image = new Jbig2ImageData(bytes, 1);
                                        Jbig2ImageHelper.ProcessImage(image);
                                        return image;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new iText.IO.IOException(iText.IO.IOException.ImageFormatCannotBeRecognized);
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
            Stream stream = null;
            try {
                stream = UrlUtil.OpenStream(source);
                byte[] bytes = new byte[8];
                stream.Read(bytes);
                return bytes;
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.IOException(iText.IO.IOException.IoException, e);
            }
            finally {
                if (stream != null) {
                    try {
                        stream.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
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
