/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
#if !NETSTANDARD2_0
using System.Drawing;
#endif // !NETSTANDARD2_0
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.IO.Util;

namespace iText.IO.Image {
    public sealed class ImageDataFactory {
        private static AbstractWebPLoader webpLoader = null;

        private const String WEBP_PACKAGE = "iText.Webpimagesupport.";

        private const String WEBP_APPLIER = "WebPLoader,itext.webp-image-support";

        private const String WEBP_APPLIER_INITIALIZE = "RegisterForIo";

        private ImageDataFactory() {
        }

        static ImageDataFactory() {
            // Android-Conversion-Skip-Block-Start
            try {
                Type type = GetWebPClass(WEBP_PACKAGE + WEBP_APPLIER);
                if (type != null) {
                    MethodInfo method = type.GetMethod(WEBP_APPLIER_INITIALIZE, new Type[] {  });
                    if (method != null) {
                        method.Invoke(null, new Object[] {  });
                    }
                }
            }
            catch (Exception) {
            }
            // do nothing
            // Android-Conversion-Skip-Block-End
            if (webpLoader == null) {
                webpLoader = new NoWebPLoader();
            }
        }

        /// <summary>
        /// Sets
        /// <see cref="AbstractWebPLoader"/>
        /// instance to use.
        /// </summary>
        /// <param name="newInstance">the instance to set</param>
        public static void SetWebPLoaderInstance(AbstractWebPLoader newInstance) {
            webpLoader = newInstance;
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
        public static ImageData Create(String filename, bool recoverImage) {
            return Create(UrlUtil.ToURL(filename), recoverImage);
        }

        /// <summary>Create an ImageData instance representing the image from the specified file.</summary>
        /// <param name="filename">filename of the file containing the image</param>
        /// <returns>The created ImageData object.</returns>
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TRANSPARENCY_LENGTH_MUST_BE_EQUAL_TO_2_WITH_CCITT_IMAGES
                    );
            }
            if (typeCCITT != RawImageData.CCITTG4 && typeCCITT != RawImageData.CCITTG3_1D && typeCCITT != RawImageData
                .CCITTG3_2D) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CCITT_COMPRESSION_TYPE_MUST_BE_CCITTG4_CCITTG3_1D_OR_CCITTG3_2D
                    );
            }
            if (reverseBits) {
                TIFFFaxDecoder.ReverseBits(data);
            }
            RawImageData image = new RawImageData(data, ImageType.RAW);
            image.SetTypeCcitt(typeCCITT);
            image.height = height;
            image.width = width;
            image.colorEncodingComponentsNumber = parameters;
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TRANSPARENCY_LENGTH_MUST_BE_EQUAL_TO_2_WITH_CCITT_IMAGES
                    );
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COMPONENTS_MUST_BE_1_3_OR_4);
            }
            if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BITS_PER_COMPONENT_MUST_BE_1_2_4_OR_8
                    );
            }
            image.colorEncodingComponentsNumber = components;
            image.bpc = bpc;
            image.data = data;
            image.transparency = transparency;
            return image;
        }

#if !NETSTANDARD2_0
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        /// <summary>Gets an instance of an Image from a java.awt.Image.</summary>
        /// <param name="image">the java.awt.Image to convert</param>
        /// <param name="color">if different from <c>null</c> the transparency pixels are replaced by this color</param>
        /// <returns>RawImage</returns>
        public static ImageData Create(System.Drawing.Image image, Color? color) {
            return iText.IO.Image.ImageDataFactory.Create(image, color, false);
        }
#endif // !NETSTANDARD2_0

#if !NETSTANDARD2_0
        /// <summary>Gets an instance of an Image from a java.awt.Image.</summary>
        /// <param name="image">the <c>java.awt.Image</c> to convert</param>
        /// <param name="color">if different from <c>null</c> the transparency pixels are replaced by this color</param>
        /// <param name="forceBW">if <c>true</c> the image is treated as black and white</param>
        /// <returns>RawImage</returns>
        public static ImageData Create(System.Drawing.Image image, Color? color, bool forceBW) {
            return DrawingImageFactory.GetImage(image, color, forceBW);
        }
#endif // !NETSTANDARD2_0

        // Android-Conversion-Skip-Block-End
        /// <summary>Gets a bitmap ImageData instance from the specified URL.</summary>
        /// <param name="url">location of the image</param>
        /// <param name="noHeader">whether the image contains a header</param>
        /// <returns>created ImageData</returns>
        public static ImageData CreateBmp(Uri url, bool noHeader) {
            ValidateImageType(url, ImageType.BMP);
            ImageData image = new BmpImageData(url, noHeader);
            BmpImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>Gets a bitmap ImageData instance from the provided bytes.</summary>
        /// <param name="bytes">array containing the raw image data</param>
        /// <param name="noHeader">Whether the image contains a header.</param>
        /// <returns>created ImageData</returns>
        public static ImageData CreateBmp(byte[] bytes, bool noHeader) {
            if (noHeader || ImageTypeDetector.DetectImageType(bytes) == ImageType.BMP) {
                ImageData image = new BmpImageData(bytes, noHeader);
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
            ValidateImageType(bytes, ImageType.GIF);
            GifImageData image = new GifImageData(bytes);
            GifImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>Returns a specified frame of the gif image.</summary>
        /// <param name="url">url of gif image</param>
        /// <param name="frame">number of frame to be returned, 1-based</param>
        /// <returns>GifImageData instance.</returns>
        public static ImageData CreateGifFrame(Uri url, int frame) {
            return CreateGifFrames(url, new int[] { frame })[0];
        }

        /// <summary>Returns a specified frame of the gif image.</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <param name="frame">number of frame to be returned, 1-based</param>
        /// <returns>GifImageData instance</returns>
        public static ImageData CreateGifFrame(byte[] bytes, int frame) {
            return CreateGifFrames(bytes, new int[] { frame })[0];
        }

        /// <summary>Returns <c>List</c> of gif image frames.</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <param name="frameNumbers">array of frame numbers of gif image, 1-based</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(byte[] bytes, int[] frameNumbers) {
            ValidateImageType(bytes, ImageType.GIF);
            GifImageData image = new GifImageData(bytes);
            return ProcessGifImageAndExtractFrames(frameNumbers, image);
        }

        /// <summary>Returns <c>List</c> of gif image frames.</summary>
        /// <param name="url">url of gif image</param>
        /// <param name="frameNumbers">array of frame numbers of gif image, 1-based</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(Uri url, int[] frameNumbers) {
            ValidateImageType(url, ImageType.GIF);
            GifImageData image = new GifImageData(url);
            return ProcessGifImageAndExtractFrames(frameNumbers, image);
        }

        /// <summary>Returns <c>List</c> of gif image frames.</summary>
        /// <param name="bytes">byte array of gif image</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(byte[] bytes) {
            ValidateImageType(bytes, ImageType.GIF);
            GifImageData image = new GifImageData(bytes);
            GifImageHelper.ProcessImage(image);
            return image.GetFrames();
        }

        /// <summary>Returns <c>List</c> of gif image frames.</summary>
        /// <param name="url">url of gif image</param>
        /// <returns>all frames of gif image</returns>
        public static IList<ImageData> CreateGifFrames(Uri url) {
            ValidateImageType(url, ImageType.GIF);
            GifImageData image = new GifImageData(url);
            GifImageHelper.ProcessImage(image);
            return image.GetFrames();
        }

        public static ImageData CreateJbig2(Uri url, int page) {
            if (page < 1) {
                throw new ArgumentException("The page number must be greater than 0");
            }
            ValidateImageType(url, ImageType.JBIG2);
            ImageData image = new Jbig2ImageData(url, page);
            Jbig2ImageHelper.ProcessImage(image);
            return image;
        }

        public static ImageData CreateJbig2(byte[] bytes, int page) {
            if (page < 1) {
                throw new ArgumentException("The page number must be greater than 0");
            }
            ValidateImageType(bytes, ImageType.JBIG2);
            ImageData image = new Jbig2ImageData(bytes, page);
            Jbig2ImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Create an
        /// <see cref="ImageData"/>
        /// instance from a JPEG image URL.
        /// </summary>
        /// <param name="url">URL to create JPEG image data from</param>
        /// <returns>the created JPEG image</returns>
        public static ImageData CreateJpeg(Uri url) {
            ValidateImageType(url, ImageType.JPEG);
            ImageData image = new JpegImageData(url);
            JpegImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a JPEG image raw bytes.
        /// </summary>
        /// <param name="bytes">raw bytes to create JPEG image data from</param>
        /// <returns>the created JPEG image</returns>
        public static ImageData CreateJpeg(byte[] bytes) {
            ValidateImageType(bytes, ImageType.JPEG);
            ImageData image = new JpegImageData(bytes);
            JpegImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a JPEG2000 image URL.
        /// </summary>
        /// <param name="url">URL to create JPEG2000 image data from</param>
        /// <returns>the created JPEG2000 image</returns>
        public static ImageData CreateJpeg2000(Uri url) {
            ValidateImageType(url, ImageType.JPEG2000);
            ImageData image = new Jpeg2000ImageData(url);
            Jpeg2000ImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a JPEG2000 image raw bytes.
        /// </summary>
        /// <param name="bytes">raw bytes to create JPEG2000 image data from</param>
        /// <returns>the created JPEG2000 image</returns>
        public static ImageData CreateJpeg2000(byte[] bytes) {
            ValidateImageType(bytes, ImageType.JPEG2000);
            ImageData image = new Jpeg2000ImageData(bytes);
            Jpeg2000ImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a PNG image URL.
        /// </summary>
        /// <param name="url">URL to create PNG image data from</param>
        /// <returns>the created PNG image</returns>
        public static ImageData CreatePng(Uri url) {
            ValidateImageType(url, ImageType.PNG);
            ImageData image = new PngImageData(url);
            PngImageHelper.ProcessImage(image);
            return image;
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a PNG image raw bytes.
        /// </summary>
        /// <param name="bytes">raw bytes to create PNG image data from</param>
        /// <returns>the created PNG image</returns>
        public static ImageData CreatePng(byte[] bytes) {
            ValidateImageType(bytes, ImageType.PNG);
            ImageData image = new PngImageData(bytes);
            PngImageHelper.ProcessImage(image);
            return image;
        }

        public static ImageData CreateTiff(Uri url, bool recoverFromImageError, int page, bool direct) {
            ValidateImageType(url, ImageType.TIFF);
            ImageData image = new TiffImageData(url, recoverFromImageError, page, direct);
            TiffImageHelper.ProcessImage(image);
            return image;
        }

        public static ImageData CreateTiff(byte[] bytes, bool recoverFromImageError, int page, bool direct) {
            ValidateImageType(bytes, ImageType.TIFF);
            ImageData image = new TiffImageData(bytes, recoverFromImageError, page, direct);
            TiffImageHelper.ProcessImage(image);
            return image;
        }

        public static ImageData CreateRawImage(byte[] bytes) {
            return new RawImageData(bytes, ImageType.RAW);
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a WebP image URL.
        /// </summary>
        /// <param name="url">URL to create WebP image data from</param>
        /// <returns>the created WebP image</returns>
        public static ImageData CreateWebP(Uri url) {
            ValidateImageType(url, ImageType.WEBP);
            return webpLoader.GetImageData(url);
        }

        /// <summary>
        /// Creates an
        /// <see cref="ImageData"/>
        /// instance from a WebP image raw bytes.
        /// </summary>
        /// <param name="bytes">raw bytes to create WebP image data from</param>
        /// <returns>the created WebP image</returns>
        public static ImageData CreateWebP(byte[] bytes) {
            ValidateImageType(bytes, ImageType.WEBP);
            return webpLoader.GetImageData(bytes);
        }

        /// <summary>Checks if the type of image (based on first 8 bytes) is supported by factory.</summary>
        /// <remarks>
        /// Checks if the type of image (based on first 8 bytes) is supported by factory.
        /// <br />
        /// <b>Note:</b> if this method returns
        /// <see langword="true"/>
        /// it doesn't means that
        /// <see cref="Create(byte[])"/>
        /// won't throw exception
        /// </remarks>
        /// <param name="source">image raw bytes</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if first eight bytes are recognised by factory as valid image type and
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsSupportedType(byte[] source) {
            if (source == null) {
                return false;
            }
            ImageType imageType = ImageTypeDetector.DetectImageType(source);
            return IsSupportedType(imageType);
        }

        /// <summary>Checks if the type of image (based on first 8 bytes) is supported by factory.</summary>
        /// <remarks>
        /// Checks if the type of image (based on first 8 bytes) is supported by factory.
        /// <br />
        /// <b>Note:</b> if this method returns
        /// <see langword="true"/>
        /// it doesn't means that
        /// <see cref="Create(byte[])"/>
        /// won't throw exception
        /// </remarks>
        /// <param name="source">image URL</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if first eight bytes are recognised by factory as valid image type and
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsSupportedType(Uri source) {
            if (source == null) {
                return false;
            }
            ImageType imageType = ImageTypeDetector.DetectImageType(source);
            return IsSupportedType(imageType);
        }

        /// <summary>Checks if the type of image is supported by factory.</summary>
        /// <remarks>
        /// Checks if the type of image is supported by factory.
        /// <br />
        /// <b>Note:</b> if this method returns
        /// <see langword="true"/>
        /// it doesn't means that
        /// <see cref="Create(byte[])"/>
        /// won't throw exception
        /// </remarks>
        /// <param name="imageType">image type</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if image type is supported and
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsSupportedType(ImageType imageType) {
            return imageType == ImageType.GIF || imageType == ImageType.JPEG || imageType == ImageType.JPEG2000 || imageType
                 == ImageType.PNG || imageType == ImageType.BMP || imageType == ImageType.TIFF || imageType == ImageType
                .JBIG2 || (imageType == ImageType.WEBP && webpLoader.IsWebPSupported());
        }

        private static ImageData CreateImageInstance(Uri source, bool recoverImage) {
            ImageType imageType = ImageTypeDetector.DetectImageType(source);
            switch (imageType) {
                case ImageType.GIF: {
                    GifImageData image = new GifImageData(source);
                    GifImageHelper.ProcessImage(image, 0);
                    return image.GetFrames()[0];
                }

                case ImageType.JPEG: {
                    ImageData image = new JpegImageData(source);
                    JpegImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.JPEG2000: {
                    ImageData image = new Jpeg2000ImageData(source);
                    Jpeg2000ImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.PNG: {
                    ImageData image = new PngImageData(source);
                    PngImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.BMP: {
                    ImageData image = new BmpImageData(source, false);
                    BmpImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.TIFF: {
                    ImageData image = new TiffImageData(source, recoverImage, 1, false);
                    TiffImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.JBIG2: {
                    ImageData image = new Jbig2ImageData(source, 1);
                    Jbig2ImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.WEBP: {
                    return webpLoader.GetImageData(source);
                }

                default: {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IMAGE_FORMAT_CANNOT_BE_RECOGNIZED);
                }
            }
        }

        private static ImageData CreateImageInstance(byte[] bytes, bool recoverImage) {
            ImageType imageType = ImageTypeDetector.DetectImageType(bytes);
            switch (imageType) {
                case ImageType.GIF: {
                    GifImageData image = new GifImageData(bytes);
                    GifImageHelper.ProcessImage(image, 0);
                    return image.GetFrames()[0];
                }

                case ImageType.JPEG: {
                    ImageData image = new JpegImageData(bytes);
                    JpegImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.JPEG2000: {
                    ImageData image = new Jpeg2000ImageData(bytes);
                    Jpeg2000ImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.PNG: {
                    ImageData image = new PngImageData(bytes);
                    PngImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.BMP: {
                    ImageData image = new BmpImageData(bytes, false);
                    BmpImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.TIFF: {
                    ImageData image = new TiffImageData(bytes, recoverImage, 1, false);
                    TiffImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.JBIG2: {
                    ImageData image = new Jbig2ImageData(bytes, 1);
                    Jbig2ImageHelper.ProcessImage(image);
                    return image;
                }

                case ImageType.WEBP: {
                    return webpLoader.GetImageData(bytes);
                }

                default: {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IMAGE_FORMAT_CANNOT_BE_RECOGNIZED);
                }
            }
        }

        private static IList<ImageData> ProcessGifImageAndExtractFrames(int[] frameNumbers, GifImageData image) {
            JavaUtil.Sort(frameNumbers);
            GifImageHelper.ProcessImage(image, frameNumbers[frameNumbers.Length - 1] - 1);
            IList<ImageData> frames = new List<ImageData>();
            foreach (int frame in frameNumbers) {
                frames.Add(image.GetFrames()[frame - 1]);
            }
            return frames;
        }

        private static void ValidateImageType(byte[] image, ImageType expectedType) {
            ImageType detectedType = ImageTypeDetector.DetectImageType(image);
            if (detectedType != expectedType) {
                throw new ArgumentException(expectedType.ToString() + " image expected. Detected image type: " + detectedType
                    .ToString());
            }
        }

        private static void ValidateImageType(Uri imageUrl, ImageType expectedType) {
            ImageType detectedType = ImageTypeDetector.DetectImageType(imageUrl);
            if (detectedType != expectedType) {
                throw new ArgumentException(expectedType.ToString() + " image expected. Detected image type: " + detectedType
                    .ToString());
            }
        }

        private static Type GetWebPClass(String partialName) {
            String classFullName = null;

            Assembly ioAssembly = typeof(ImageDataFactory).GetAssembly();
            try {
                string webPVersion = ioAssembly.GetName().Version.ToString();
                string format = "{0}, Version={1}, Culture=neutral, PublicKeyToken=8354ae6d2174ddca";
                classFullName = String.Format(format, partialName, webPVersion);
            } catch (Exception ignored) {
            }

            Type type = null;
            if (classFullName != null) {
                String fileLoadExceptionMessage = null;
                try {
                    type = System.Type.GetType(classFullName);
                } catch (FileLoadException fileLoadException) {
                    fileLoadExceptionMessage = fileLoadException.Message;
                }
                if (type == null) {
                    // try to find webp-image-support assembly by it's partial name and check if it refers to current version of itext core
                    try {
                        type = System.Type.GetType(partialName);
                    } catch {
                        // ignore
                    }
                    if (type != null) {
                        bool doesReferToCurrentVersionOfCore = false;
                        foreach (AssemblyName assemblyName in type.GetAssembly().GetReferencedAssemblies()) {
                            if ("itext.io".Equals(assemblyName.Name)) {
                                doesReferToCurrentVersionOfCore = assemblyName.Version.Equals(ioAssembly.GetName().Version);
                                break;
                            }
                        }
                        if (!doesReferToCurrentVersionOfCore) {
                            type = null;
                        }
                    }
                    if (type == null && fileLoadExceptionMessage != null) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(ImageDataFactory));
                        logger.LogError(fileLoadExceptionMessage);
                    }
                }
            }

            return type;
        }
    }
}
