/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Colors;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Filters;

namespace iText.Kernel.Pdf.Xobject {
    /// <summary>A wrapper for Image XObject.</summary>
    /// <remarks>A wrapper for Image XObject. ISO 32000-1, 8.9 Images.</remarks>
    public class PdfImageXObject : PdfXObject {
        private bool newImage = false;

        private float width;

        private float height;

        private bool mask;

        private bool softMask;

        /// <summary>Creates Image XObject by image.</summary>
        /// <param name="image">
        /// 
        /// <see cref="iText.IO.Image.ImageData"/>
        /// with actual image data.
        /// </param>
        public PdfImageXObject(ImageData image)
            : this(image, null) {
        }

        /// <summary>Creates Image XObject by image.</summary>
        /// <param name="image">
        /// 
        /// <see cref="iText.IO.Image.ImageData"/>
        /// with actual image data.
        /// </param>
        /// <param name="imageMask">
        /// 
        /// <see cref="PdfImageXObject"/>
        /// with image mask.
        /// </param>
        public PdfImageXObject(ImageData image, iText.Kernel.Pdf.Xobject.PdfImageXObject imageMask)
            : this(CreatePdfStream(CheckImageType(image), imageMask)) {
            newImage = true;
            mask = image.IsMask();
            softMask = image.IsSoftMask();
        }

        /// <summary>
        /// Create
        /// <see cref="PdfImageXObject"/>
        /// instance by
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <remarks>
        /// Create
        /// <see cref="PdfImageXObject"/>
        /// instance by
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// Note, this constructor doesn't perform any additional checks
        /// </remarks>
        /// <param name="pdfStream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// with Image XObject.
        /// </param>
        /// <seealso cref="PdfXObject.MakeXObject(iText.Kernel.Pdf.PdfStream)"/>
        public PdfImageXObject(PdfStream pdfStream)
            : base(pdfStream) {
            if (!pdfStream.IsFlushed()) {
                InitWidthField();
                InitHeightField();
            }
        }

        /// <summary>
        /// Gets width of image,
        /// <c>Width</c>
        /// key.
        /// </summary>
        /// <returns>float value.</returns>
        public override float GetWidth() {
            return width;
        }

        /// <summary>
        /// Gets height of image,
        /// <c>Height</c>
        /// key.
        /// </summary>
        /// <returns>float value.</returns>
        public override float GetHeight() {
            return height;
        }

        /// <summary>Returns whether this image is a mask that can be used for other images.</summary>
        /// <remarks>
        /// Returns whether this image is a mask that can be used for other images.
        /// <para />
        /// A mask can hide parts of an image by making those parts fully transparent.
        /// </remarks>
        /// <returns>whether this image is a mask that can be used for other images</returns>
        public virtual bool IsMask() {
            if (newImage) {
                return mask;
            }
            PdfObject val = GetPdfObject().Get(PdfName.ImageMask);
            return val != null && val.IsBoolean() && ((PdfBoolean)val).GetValue();
        }

        /// <summary>Returns whether this image is a soft mask that can be used for other images.</summary>
        /// <remarks>
        /// Returns whether this image is a soft mask that can be used for other images.
        /// <para />
        /// A soft mask sets the transparency for an image.
        /// </remarks>
        /// <returns>whether this image is a soft mask.</returns>
        public virtual bool IsSoftMask() {
            if (newImage) {
                return softMask;
            }
            PdfObject val = GetPdfObject().Get(PdfName.SMask);
            return val != null && val.IsBoolean() && ((PdfBoolean)val).GetValue();
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note, that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary>Copy Image XObject to the specified document.</summary>
        /// <param name="document">target document</param>
        /// <returns>
        /// just created instance of
        /// <see cref="PdfImageXObject"/>.
        /// </returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfImageXObject CopyTo(PdfDocument document) {
            iText.Kernel.Pdf.Xobject.PdfImageXObject image = new iText.Kernel.Pdf.Xobject.PdfImageXObject((PdfStream)GetPdfObject
                ().CopyTo(document));
            image.mask = mask;
            image.softMask = softMask;
            return image;
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        /// <summary>Gets decoded image bytes.</summary>
        /// <returns>byte array.</returns>
        public virtual byte[] GetImageBytes() {
            return GetImageBytes(PdfImageXObject.ImageBytesRetrievalProperties.GetApplyFiltersOnly());
        }

        /// <summary>Gets image bytes.</summary>
        /// <remarks>
        /// Gets image bytes.
        /// Note,
        /// <see cref="iText.Kernel.Pdf.PdfName.DCTDecode"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.JBIG2Decode"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.JPXDecode"/>
        /// filters will be ignored.
        /// </remarks>
        /// <param name="decoded">
        /// if
        /// <see langword="true"/>
        /// , decodes stream bytes.
        /// </param>
        /// <returns>byte array.</returns>
        public virtual byte[] GetImageBytes(bool decoded) {
            PdfImageXObject.ImageBytesRetrievalProperties props = PdfImageXObject.ImageBytesRetrievalProperties.GetApplyFiltersOnly
                ();
            props.SetApplyFilters(decoded);
            return GetImageBytes(props);
        }

        /// <summary>Gets image bytes.</summary>
        /// <remarks>
        /// Gets image bytes.
        /// Note,
        /// <see cref="iText.Kernel.Pdf.PdfName.DCTDecode"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.JBIG2Decode"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.JPXDecode"/>
        /// filters will be ignored.
        /// </remarks>
        /// <param name="properties">
        /// an instance of
        /// <see cref="ImageBytesRetrievalProperties"/>
        /// to configure the options.
        /// </param>
        /// <returns>byte array.</returns>
        public virtual byte[] GetImageBytes(PdfImageXObject.ImageBytesRetrievalProperties properties) {
            // TODO: DEVSIX-1792 replace `.getBytes(false)` with `getBytes(true) and remove manual decoding
            byte[] bytes = GetPdfObject().GetBytes(false);
            if (properties.IsApplyFilters()) {
                try {
                    return GetImageBytesDecoded(bytes, properties);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
            return bytes;
        }

        /// <summary>
        /// Identifies the type of the image that is stored in the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// </summary>
        /// <remarks>
        /// Identifies the type of the image that is stored in the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// Note that this has nothing to do with the original type of the image. For instance, the return value
        /// of this method will never be
        /// <see cref="iText.IO.Image.ImageType.PNG"/>
        /// as we lose this information when converting a
        /// PNG image into something that can be put into a PDF file.
        /// The possible values are:
        /// <see cref="iText.IO.Image.ImageType.JPEG"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.JPEG2000"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.JBIG2"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.TIFF"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.PNG"/>
        /// </remarks>
        /// <returns>the identified type of image</returns>
        public virtual ImageType IdentifyImageType() {
            return IdentifyImageType(PdfImageXObject.ImageBytesRetrievalProperties.GetApplyFiltersOnly());
        }

        /// <summary>
        /// Identifies the type of the image that is stored in the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// </summary>
        /// <remarks>
        /// Identifies the type of the image that is stored in the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// Note that this has nothing to do with the original type of the image. For instance, the return value
        /// of this method will never be
        /// <see cref="iText.IO.Image.ImageType.PNG"/>
        /// as we lose this information when converting a
        /// PNG image into something that can be put into a PDF file.
        /// The possible values are:
        /// <see cref="iText.IO.Image.ImageType.JPEG"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.JPEG2000"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.JBIG2"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.TIFF"/>
        /// ,
        /// <see cref="iText.IO.Image.ImageType.PNG"/>
        /// </remarks>
        /// <param name="properties">
        /// an instance of
        /// <see cref="ImageBytesRetrievalProperties"/>
        /// to configure the options,
        /// these options can influence the type
        /// </param>
        /// <returns>the identified type of image</returns>
        public virtual ImageType IdentifyImageType(PdfImageXObject.ImageBytesRetrievalProperties properties) {
            PdfObject filter = GetPdfObject().Get(PdfName.Filter);
            PdfArray filters = new PdfArray();
            if (filter != null) {
                if (filter.GetObjectType() == PdfObject.NAME) {
                    filters.Add(filter);
                }
                else {
                    if (filter.GetObjectType() == PdfObject.ARRAY) {
                        filters = ((PdfArray)filter);
                    }
                }
            }
            for (int i = filters.Size() - 1; i >= 0; i--) {
                PdfName filterName = (PdfName)filters.Get(i);
                if (PdfName.DCTDecode.Equals(filterName)) {
                    return ImageType.JPEG;
                }
                else {
                    if (PdfName.JBIG2Decode.Equals(filterName)) {
                        return ImageType.JBIG2;
                    }
                    else {
                        if (PdfName.JPXDecode.Equals(filterName)) {
                            return ImageType.JPEG2000;
                        }
                    }
                }
            }
            // None of the previous types match
            ImagePdfBytesInfo imageInfo = new ImagePdfBytesInfo(this, properties);
            if (imageInfo.GetImageType() == ImagePdfBytesInfo.OutputFileType.TIFF) {
                return ImageType.TIFF;
            }
            return ImageType.PNG;
        }

        /// <summary>
        /// Identifies recommended file extension to store the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// </summary>
        /// <remarks>
        /// Identifies recommended file extension to store the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// Possible values are: 'png', 'jpg', 'jp2', 'tif', 'jbig2'.
        /// This extension can later be used together with the result of
        /// <see cref="GetImageBytes()"/>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// with recommended file extension
        /// </returns>
        /// <seealso cref="IdentifyImageType(ImageBytesRetrievalProperties)"/>
        public virtual String IdentifyImageFileExtension() {
            return IdentifyImageFileExtension(PdfImageXObject.ImageBytesRetrievalProperties.GetApplyFiltersOnly());
        }

        /// <summary>
        /// Identifies recommended file extension to store the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// </summary>
        /// <remarks>
        /// Identifies recommended file extension to store the bytes of this
        /// <see cref="PdfImageXObject"/>.
        /// Possible values are: 'png', 'jpg', 'jp2', 'tif', 'jbig2'.
        /// This extension can later be used together with the result of
        /// <see cref="GetImageBytes()"/>.
        /// </remarks>
        /// <param name="properties">
        /// an instance of
        /// <see cref="ImageBytesRetrievalProperties"/>
        /// to configure the options,
        /// *                    these options can influence the file extension
        /// </param>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// with recommended file extension
        /// </returns>
        public virtual String IdentifyImageFileExtension(PdfImageXObject.ImageBytesRetrievalProperties properties) {
            ImageType bytesType = IdentifyImageType(properties);
            switch (bytesType) {
                case ImageType.PNG: {
                    return "png";
                }

                case ImageType.JPEG: {
                    return "jpg";
                }

                case ImageType.JPEG2000: {
                    return "jp2";
                }

                case ImageType.TIFF: {
                    return "tif";
                }

                case ImageType.JBIG2: {
                    return "jbig2";
                }

                default: {
                    throw new InvalidOperationException("Should have never happened. This type of image is not allowed for ImageXObject"
                        );
                }
            }
        }

        /// <summary>Puts the value into Image XObject dictionary and associates it with the specified key.</summary>
        /// <remarks>
        /// Puts the value into Image XObject dictionary and associates it with the specified key.
        /// If the key is already present, it will override the old value with the specified one.
        /// </remarks>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfImageXObject Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        private byte[] GetImageBytesDecoded(byte[] bytes, PdfImageXObject.ImageBytesRetrievalProperties properties
            ) {
            ImageType imageType = IdentifyImageType(properties);
            if (imageType == ImageType.TIFF || imageType == ImageType.PNG) {
                ImagePdfBytesInfo ibi = new ImagePdfBytesInfo(this, properties);
                return ibi.GetProcessedImageData(bytes);
            }
            else {
                IDictionary<PdfName, IFilterHandler> filters = new Dictionary<PdfName, IFilterHandler>(FilterHandlers.GetDefaultFilterHandlers
                    ());
                filters.Put(PdfName.JBIG2Decode, new DoNothingFilter());
                return PdfReader.DecodeBytes(bytes, GetPdfObject(), filters);
            }
        }

        private float InitWidthField() {
            PdfNumber wNum = GetPdfObject().GetAsNumber(PdfName.Width);
            if (wNum != null) {
                width = wNum.FloatValue();
            }
            return width;
        }

        private float InitHeightField() {
            PdfNumber hNum = GetPdfObject().GetAsNumber(PdfName.Height);
            if (hNum != null) {
                height = hNum.FloatValue();
            }
            return height;
        }

        private static PdfStream CreatePdfStream(ImageData image, iText.Kernel.Pdf.Xobject.PdfImageXObject imageMask
            ) {
            PdfStream stream;
            if (image.GetOriginalType() == ImageType.RAW) {
                RawImageHelper.UpdateImageAttributes((RawImageData)image, null);
            }
            stream = new PdfStream(image.GetData());
            String filter = image.GetFilter();
            if ("JPXDecode".Equals(filter) && image.GetColorEncodingComponentsNumber() <= 0) {
                stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                image.SetBpc(0);
            }
            stream.Put(PdfName.Type, PdfName.XObject);
            stream.Put(PdfName.Subtype, PdfName.Image);
            PdfDictionary decodeParms = CreateDictionaryFromMap(stream, image.GetDecodeParms());
            if (decodeParms != null) {
                stream.Put(PdfName.DecodeParms, decodeParms);
            }
            if (!(image is PngImageData)) {
                PdfName colorSpace;
                switch (image.GetColorEncodingComponentsNumber()) {
                    case 1: {
                        colorSpace = PdfName.DeviceGray;
                        break;
                    }

                    case 3: {
                        colorSpace = PdfName.DeviceRGB;
                        break;
                    }

                    default: {
                        colorSpace = PdfName.DeviceCMYK;
                        break;
                    }
                }
                stream.Put(PdfName.ColorSpace, colorSpace);
            }
            if (image.GetBpc() != 0) {
                stream.Put(PdfName.BitsPerComponent, new PdfNumber(image.GetBpc()));
            }
            if (image.GetFilter() != null) {
                stream.Put(PdfName.Filter, new PdfName(image.GetFilter()));
            }
            if (image.GetColorEncodingComponentsNumber() == -1) {
                stream.Remove(PdfName.ColorSpace);
            }
            PdfDictionary additional = null;
            if (image is PngImageData) {
                PngImageData pngImage = (PngImageData)image;
                if (pngImage.IsIndexed()) {
                    PdfArray colorspace = new PdfArray();
                    colorspace.Add(PdfName.Indexed);
                    colorspace.Add(GetColorSpaceInfo(pngImage));
                    if ((pngImage.GetColorPalette() != null) && (pngImage.GetColorPalette().Length > 0)) {
                        //Each palette entry is a three-byte series, so the number of entries is calculated as the length
                        //of the stream divided by 3. The number below specifies the maximum valid index value (starting
                        // from 0 up)
                        colorspace.Add(new PdfNumber(pngImage.GetColorPalette().Length / 3 - 1));
                    }
                    if (pngImage.GetColorPalette() != null) {
                        colorspace.Add(new PdfString(PdfEncodings.ConvertToString(pngImage.GetColorPalette(), null)));
                    }
                    stream.Put(PdfName.ColorSpace, colorspace);
                }
                else {
                    stream.Put(PdfName.ColorSpace, GetColorSpaceInfo(pngImage));
                }
            }
            additional = CreateDictionaryFromMap(stream, image.GetImageAttributes());
            if (additional != null) {
                stream.PutAll(additional);
            }
            IccProfile iccProfile = image.GetProfile();
            if (iccProfile != null) {
                PdfStream iccProfileStream = PdfCieBasedCs.IccBased.GetIccProfileStream(iccProfile);
                PdfArray iccBasedColorSpace = new PdfArray();
                iccBasedColorSpace.Add(PdfName.ICCBased);
                iccBasedColorSpace.Add(iccProfileStream);
                PdfObject colorSpaceObject = stream.Get(PdfName.ColorSpace);
                bool iccProfileShouldBeApplied = true;
                if (colorSpaceObject != null) {
                    PdfColorSpace cs = PdfColorSpace.MakeColorSpace(colorSpaceObject);
                    if (cs == null) {
                        ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Xobject.PdfImageXObject)).LogError(iText.IO.Logs.IoLogMessageConstant
                            .IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE);
                    }
                    else {
                        if (cs is PdfSpecialCs.Indexed) {
                            PdfColorSpace baseCs = ((PdfSpecialCs.Indexed)cs).GetBaseCs();
                            if (baseCs == null) {
                                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Xobject.PdfImageXObject)).LogError(iText.IO.Logs.IoLogMessageConstant
                                    .IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE);
                            }
                            else {
                                if (baseCs.GetNumberOfComponents() != iccProfile.GetNumComponents()) {
                                    ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Xobject.PdfImageXObject)).LogError(iText.IO.Logs.IoLogMessageConstant
                                        .IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE
                                        );
                                    iccProfileShouldBeApplied = false;
                                }
                                else {
                                    iccProfileStream.Put(PdfName.Alternate, baseCs.GetPdfObject());
                                }
                            }
                            if (iccProfileShouldBeApplied) {
                                ((PdfArray)colorSpaceObject).Set(1, iccBasedColorSpace);
                                iccProfileShouldBeApplied = false;
                            }
                        }
                        else {
                            if (cs.GetNumberOfComponents() != iccProfile.GetNumComponents()) {
                                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Xobject.PdfImageXObject)).LogError(iText.IO.Logs.IoLogMessageConstant
                                    .IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_COLOR_SPACE);
                                iccProfileShouldBeApplied = false;
                            }
                            else {
                                iccProfileStream.Put(PdfName.Alternate, colorSpaceObject);
                            }
                        }
                    }
                }
                if (iccProfileShouldBeApplied) {
                    stream.Put(PdfName.ColorSpace, iccBasedColorSpace);
                }
            }
            if (image.IsMask() && (image.GetBpc() == 1 || image.GetBpc() > 0xff)) {
                stream.Put(PdfName.ImageMask, PdfBoolean.TRUE);
            }
            if (imageMask != null) {
                if (imageMask.softMask) {
                    stream.Put(PdfName.SMask, imageMask.GetPdfObject());
                }
                else {
                    if (imageMask.mask) {
                        stream.Put(PdfName.Mask, imageMask.GetPdfObject());
                    }
                }
            }
            ImageData mask = image.GetImageMask();
            if (mask != null) {
                if (mask.IsSoftMask()) {
                    stream.Put(PdfName.SMask, new iText.Kernel.Pdf.Xobject.PdfImageXObject(image.GetImageMask()).GetPdfObject(
                        ));
                }
                else {
                    if (mask.IsMask()) {
                        stream.Put(PdfName.Mask, new iText.Kernel.Pdf.Xobject.PdfImageXObject(image.GetImageMask()).GetPdfObject()
                            );
                    }
                }
            }
            if (image.GetDecode() != null) {
                stream.Put(PdfName.Decode, new PdfArray(image.GetDecode()));
            }
            if (image.IsMask() && image.IsInverted()) {
                stream.Put(PdfName.Decode, new PdfArray(new float[] { 1, 0 }));
            }
            if (image.IsInterpolation()) {
                stream.Put(PdfName.Interpolate, PdfBoolean.TRUE);
            }
            // deal with transparency
            int[] transparency = image.GetTransparency();
            if (transparency != null && !image.IsMask() && imageMask == null) {
                PdfArray t = new PdfArray();
                foreach (int transparencyItem in transparency) {
                    t.Add(new PdfNumber(transparencyItem));
                }
                stream.Put(PdfName.Mask, t);
            }
            stream.Put(PdfName.Width, new PdfNumber(image.GetWidth()));
            stream.Put(PdfName.Height, new PdfNumber(image.GetHeight()));
            return stream;
        }

        private static PdfDictionary CreateDictionaryFromMap(PdfStream stream, IDictionary<String, Object> parms) {
            if (parms != null) {
                PdfDictionary dictionary = new PdfDictionary();
                foreach (KeyValuePair<String, Object> entry in parms) {
                    Object value = entry.Value;
                    String key = entry.Key;
                    if (value is int?) {
                        dictionary.Put(new PdfName(key), new PdfNumber((int)value));
                    }
                    else {
                        if (value is float?) {
                            dictionary.Put(new PdfName(key), new PdfNumber((float)value));
                        }
                        else {
                            if (value is String) {
                                if (value.Equals(PngImageHelperConstants.MASK)) {
                                    dictionary.Put(PdfName.Mask, new PdfLiteral((String)value));
                                }
                                else {
                                    String str = (String)value;
                                    if (str.IndexOf('/') == 0) {
                                        dictionary.Put(new PdfName(key), new PdfName(str.Substring(1)));
                                    }
                                    else {
                                        dictionary.Put(new PdfName(key), new PdfString(str));
                                    }
                                }
                            }
                            else {
                                if (value is byte[]) {
                                    PdfStream globalsStream = new PdfStream();
                                    globalsStream.GetOutputStream().WriteBytes((byte[])value);
                                    dictionary.Put(PdfName.JBIG2Globals, globalsStream);
                                }
                                else {
                                    if (value is bool?) {
                                        dictionary.Put(new PdfName(key), PdfBoolean.ValueOf((bool)value));
                                    }
                                    else {
                                        if (value is Object[]) {
                                            dictionary.Put(new PdfName(key), CreateArray(stream, (Object[])value));
                                        }
                                        else {
                                            if (value is float[]) {
                                                dictionary.Put(new PdfName(key), new PdfArray((float[])value));
                                            }
                                            else {
                                                if (value is int[]) {
                                                    dictionary.Put(new PdfName(key), new PdfArray((int[])value));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return dictionary;
            }
            return null;
        }

        private static PdfArray CreateArray(PdfStream stream, Object[] objects) {
            PdfArray array = new PdfArray();
            foreach (Object obj in objects) {
                if (obj is String) {
                    String str = (String)obj;
                    if (str.IndexOf('/') == 0) {
                        array.Add(new PdfName(str.Substring(1)));
                    }
                    else {
                        array.Add(new PdfString(str));
                    }
                }
                else {
                    if (obj is int?) {
                        array.Add(new PdfNumber((int)obj));
                    }
                    else {
                        if (obj is float?) {
                            array.Add(new PdfNumber((float)obj));
                        }
                        else {
                            if (obj is Object[]) {
                                array.Add(CreateArray(stream, (Object[])obj));
                            }
                            else {
                                array.Add(CreateDictionaryFromMap(stream, (IDictionary<String, Object>)obj));
                            }
                        }
                    }
                }
            }
            return array;
        }

        private static ImageData CheckImageType(ImageData image) {
            if (image is WmfImageData) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CREATE_PDF_IMAGE_XOBJECT_BY_WMF_IMAGE);
            }
            return image;
        }

        private static PdfObject GetColorSpaceInfo(PngImageData pngImageData) {
            if (pngImageData.GetProfile() != null) {
                if (pngImageData.IsGrayscaleImage()) {
                    return PdfName.DeviceGray;
                }
                else {
                    return PdfName.DeviceRGB;
                }
            }
            if (pngImageData.GetGamma() == 1f && !pngImageData.IsHasCHRM()) {
                if (pngImageData.IsGrayscaleImage()) {
                    return PdfName.DeviceGray;
                }
                else {
                    return PdfName.DeviceRGB;
                }
            }
            else {
                PdfArray array = new PdfArray();
                PdfDictionary map = new PdfDictionary();
                if (pngImageData.IsGrayscaleImage()) {
                    if (pngImageData.GetGamma() == 1f) {
                        return PdfName.DeviceGray;
                    }
                    array.Add(PdfName.CalGray);
                    map.Put(PdfName.Gamma, new PdfNumber(pngImageData.GetGamma()));
                    map.Put(PdfName.WhitePoint, new PdfArray(new int[] { 1, 1, 1 }));
                }
                else {
                    float[] wp = new float[] { 1, 1, 1 };
                    array.Add(PdfName.CalRGB);
                    float gamma = pngImageData.GetGamma();
                    if (gamma != 1f) {
                        float[] gm = new float[3];
                        gm[0] = gamma;
                        gm[1] = gamma;
                        gm[2] = gamma;
                        map.Put(PdfName.Gamma, new PdfArray(gm));
                    }
                    if (pngImageData.IsHasCHRM()) {
                        PdfImageXObject.PngChromaticitiesHelper helper = new PdfImageXObject.PngChromaticitiesHelper();
                        helper.ConstructMatrix(pngImageData);
                        wp = helper.wp;
                        map.Put(PdfName.Matrix, new PdfArray(helper.matrix));
                    }
                    map.Put(PdfName.WhitePoint, new PdfArray(wp));
                }
                array.Add(map);
                return array;
            }
        }

        /// <summary>Manages the steps taken in extracting the image.</summary>
        /// <remarks>
        /// Manages the steps taken in extracting the image.
        /// <para />
        /// Use getApplyFiltersOnly to get the image as close to the stored state as possible.
        /// No pixels will be manipulated.
        /// <para />
        /// Use getFullOption to get an image as displayed in the document.
        /// The full options will be extended in the future to support more color spaces
        /// and can lead to different outcomes for some image types.
        /// </remarks>
        public sealed class ImageBytesRetrievalProperties {
            private bool applyTransparency;

            private bool applyDecodeArray;

            private bool applyTintTransformations;

            private bool applyFilters;

            private ImageBytesRetrievalProperties() {
            }

            //empty contructor
            /// <summary>Create a property set with only the options applyFilters activates.</summary>
            /// <remarks>
            /// Create a property set with only the options applyFilters activates.
            /// <para />
            /// Use this to retrieve images as close to the stored state as possible.
            /// No pixels will be manipulated.
            /// </remarks>
            /// <returns>A property set with only the options applyFilters activated.</returns>
            public static PdfImageXObject.ImageBytesRetrievalProperties GetApplyFiltersOnly() {
                PdfImageXObject.ImageBytesRetrievalProperties props = new PdfImageXObject.ImageBytesRetrievalProperties();
                props.applyFilters = true;
                return props;
            }

            /// <summary>Create a property set with all options activated.</summary>
            /// <remarks>
            /// Create a property set with all options activated.
            /// <para />
            /// Use this to retrieve images as closely as possible
            /// to how they are being displayed in the document.
            /// </remarks>
            /// <returns>A property set with all options activated.</returns>
            public static PdfImageXObject.ImageBytesRetrievalProperties GetFullOption() {
                PdfImageXObject.ImageBytesRetrievalProperties props = new PdfImageXObject.ImageBytesRetrievalProperties();
                props.applyFilters = true;
                props.applyTintTransformations = true;
                props.applyDecodeArray = true;
                props.applyTransparency = true;
                return props;
            }

            /// <summary>Returns whether transparency will be applied to the result.</summary>
            /// <returns>Whether transparency will be applied to the result</returns>
            public bool IsApplyTransparency() {
                return applyTransparency;
            }

            /// <summary>Sets whether transparency will be applied to the result.</summary>
            /// <param name="applyTransparency">set to true to apply transparency.</param>
            public void SetApplyTransparency(bool applyTransparency) {
                this.applyTransparency = applyTransparency;
            }

            /// <summary>Returns whether to apply an images decode transformation or not.</summary>
            /// <remarks>
            /// Returns whether to apply an images decode transformation or not.
            /// <para />
            /// An image’s Decode array specifies a linear mapping
            /// of each integer component.
            /// </remarks>
            /// <returns>whether to apply an images decode transformation or not</returns>
            public bool IsApplyDecodeArray() {
                return applyDecodeArray;
            }

            /// <summary>Sets whether to apply an image's decode transformation.</summary>
            /// <remarks>
            /// Sets whether to apply an image's decode transformation.
            /// <para />
            /// An image’s Decode array specifies a linear mapping
            /// of each integer component.
            /// </remarks>
            /// <param name="applyDecodeArray">whether to apply an images decode transformation</param>
            public void SetApplyDecodeArray(bool applyDecodeArray) {
                this.applyDecodeArray = applyDecodeArray;
            }

            /// <summary>Returns whether tint transformations will be applied.</summary>
            /// <remarks>
            /// Returns whether tint transformations will be applied.
            /// <para />
            /// Some color spaces define application-specific colors.
            /// To be able to display these images in an rgb or cmyk color space,
            /// there can be tint transformations defined to translate these colors.
            /// </remarks>
            /// <returns>whether any tint transformations will be applied.</returns>
            public bool IsApplyTintTransformations() {
                return applyTintTransformations;
            }

            /// <summary>Sets whether tint transformations will be applied.</summary>
            /// <remarks>
            /// Sets whether tint transformations will be applied.
            /// <para />
            /// Some color spaces define application-specific colors.
            /// To be able to display these images in an rgb or cmyk color space,
            /// there can be tint transformations defined to translate these colors.
            /// </remarks>
            /// <param name="applyTintTransformations">whether to apply tint transformations</param>
            public void SetApplyTintTransformations(bool applyTintTransformations) {
                this.applyTintTransformations = applyTintTransformations;
            }

            /// <summary>Returns whether to apply the bytestreams filters.</summary>
            /// <remarks>
            /// Returns whether to apply the bytestreams filters.
            /// <para />
            /// Without this option, the other options are disregarded.
            /// </remarks>
            /// <returns>whether to apply the filters to the byte stream.</returns>
            public bool IsApplyFilters() {
                return applyFilters;
            }

            /// <summary>Sets whether to apply the bytestreams filters.</summary>
            /// <remarks>
            /// Sets whether to apply the bytestreams filters.
            /// <para />
            /// Without this option, the other options are disregarded.
            /// </remarks>
            /// <param name="applyFilters">whether to apply the bytestreams filters</param>
            public void SetApplyFilters(bool applyFilters) {
                this.applyFilters = applyFilters;
            }
        }

        private class PngChromaticitiesHelper {
//\cond DO_NOT_DOCUMENT
            internal float[] matrix = new float[9];
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float[] wp = new float[3];
//\endcond

            public virtual void ConstructMatrix(PngImageData pngImageData) {
                PngChromaticities pngChromaticities = pngImageData.GetPngChromaticities();
                float z = pngChromaticities.GetYW() * ((pngChromaticities.GetXG() - pngChromaticities.GetXB()) * pngChromaticities
                    .GetYR() - (pngChromaticities.GetXR() - pngChromaticities.GetXB()) * pngChromaticities.GetYG() + (pngChromaticities
                    .GetXR() - pngChromaticities.GetXG()) * pngChromaticities.GetYB());
                float YA = pngChromaticities.GetYR() * ((pngChromaticities.GetXG() - pngChromaticities.GetXB()) * pngChromaticities
                    .GetYW() - (pngChromaticities.GetXW() - pngChromaticities.GetXB()) * pngChromaticities.GetYG() + (pngChromaticities
                    .GetXW() - pngChromaticities.GetXG()) * pngChromaticities.GetYB()) / z;
                float XA = YA * pngChromaticities.GetXR() / pngChromaticities.GetYR();
                float ZA = YA * ((1 - pngChromaticities.GetXR()) / pngChromaticities.GetYR() - 1);
                float YB = -pngChromaticities.GetYG() * ((pngChromaticities.GetXR() - pngChromaticities.GetXB()) * pngChromaticities
                    .GetYW() - (pngChromaticities.GetXW() - pngChromaticities.GetXB()) * pngChromaticities.GetYR() + (pngChromaticities
                    .GetXW() - pngChromaticities.GetXR()) * pngChromaticities.GetYB()) / z;
                float XB = YB * pngChromaticities.GetXG() / pngChromaticities.GetYG();
                float ZB = YB * ((1 - pngChromaticities.GetXG()) / pngChromaticities.GetYG() - 1);
                float YC = pngChromaticities.GetYB() * ((pngChromaticities.GetXR() - pngChromaticities.GetXG()) * pngChromaticities
                    .GetYW() - (pngChromaticities.GetXW() - pngChromaticities.GetXG()) * pngChromaticities.GetYW() + (pngChromaticities
                    .GetXW() - pngChromaticities.GetXR()) * pngChromaticities.GetYG()) / z;
                float XC = YC * pngChromaticities.GetXB() / pngChromaticities.GetYB();
                float ZC = YC * ((1 - pngChromaticities.GetXB()) / pngChromaticities.GetYB() - 1);
                float XW = XA + XB + XC;
                float YW = 1;
                float ZW = ZA + ZB + ZC;
                float[] wpa = new float[3];
                wpa[0] = XW;
                wpa[1] = YW;
                wpa[2] = ZW;
                this.wp = JavaUtil.ArraysCopyOf(wpa, 3);
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
                this.matrix = JavaUtil.ArraysCopyOf(matrix, 9);
            }
        }
    }
}
