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
using iText.IO.Codec;
using iText.IO.Image;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Filters;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfImageXObject : PdfXObject {
        private float width;

        private float height;

        private bool mask;

        private bool softMask;

        private int pngColorType = -1;

        private int pngBitDepth;

        private int bpc;

        private byte[] palette;

        private byte[] icc;

        private int stride;

        public PdfImageXObject(ImageData image)
            : this(image, null) {
        }

        public PdfImageXObject(ImageData image, iText.Kernel.Pdf.Xobject.PdfImageXObject imageMask)
            : this(CreatePdfStream(CheckImageType(image), imageMask)) {
            mask = image.IsMask();
            softMask = image.IsSoftMask();
        }

        public PdfImageXObject(PdfStream pdfObject)
            : base(pdfObject) {
        }

        public override float GetWidth() {
            if (!IsFlushed()) {
                return GetPdfObject().GetAsNumber(PdfName.Width).FloatValue();
            }
            else {
                return width;
            }
        }

        public override float GetHeight() {
            if (!IsFlushed()) {
                return GetPdfObject().GetAsNumber(PdfName.Height).FloatValue();
            }
            else {
                return height;
            }
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </summary>
        public override void Flush() {
            if (!IsFlushed()) {
                width = GetPdfObject().GetAsNumber(PdfName.Width).FloatValue();
                height = GetPdfObject().GetAsNumber(PdfName.Height).FloatValue();
                base.Flush();
            }
        }

        public virtual iText.Kernel.Pdf.Xobject.PdfImageXObject CopyTo(PdfDocument document) {
            iText.Kernel.Pdf.Xobject.PdfImageXObject image = new iText.Kernel.Pdf.Xobject.PdfImageXObject(((PdfStream)
                GetPdfObject().CopyTo(document)));
            image.width = width;
            image.height = height;
            image.mask = mask;
            image.softMask = softMask;
            return image;
        }

        public virtual byte[] GetImageBytes() {
            return GetImageBytes(true);
        }

        public virtual byte[] GetImageBytes(bool decoded) {
            byte[] bytes;
            bytes = GetPdfObject().GetBytes(false);
            if (decoded) {
                IDictionary<PdfName, IFilterHandler> filters = new Dictionary<PdfName, IFilterHandler>(FilterHandlers.GetDefaultFilterHandlers
                    ());
                DoNothingFilter stubFilter = new DoNothingFilter();
                filters[PdfName.DCTDecode] = stubFilter;
                filters[PdfName.JBIG2Decode] = stubFilter;
                filters[PdfName.JPXDecode] = stubFilter;
                bytes = PdfReader.DecodeBytes(bytes, GetPdfObject(), filters);
                if (stubFilter.GetLastFilterName() == null) {
                    try {
                        bytes = DecodeTiffAndPngBytes(bytes);
                    }
                    catch (System.IO.IOException e) {
                        throw new Exception("IO exception in PdfImageXObject", e);
                    }
                }
            }
            return bytes;
        }

        /// <summary>
        /// Identifies the type of the image that is stored in the bytes of this
        /// <see cref="PdfImageXObject"/>
        /// .
        /// Note that this has nothing to do with the original type of the image. For instance, the return value
        /// of this method will never be
        /// <see cref="iText.IO.Image.ImageType.PNG"/>
        /// as we loose this information when converting a
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
        /// </summary>
        /// <returns>the identified type of image</returns>
        public virtual ImageType IdentifyImageType() {
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
            PdfObject colorspace = GetPdfObject().Get(PdfName.ColorSpace);
            PrepareAndFindColorspace(colorspace);
            if (pngColorType < 0) {
                return ImageType.TIFF;
            }
            else {
                return ImageType.PNG;
            }
        }

        /// <summary>
        /// Identifies recommended file extension to store the bytes of this
        /// <see cref="PdfImageXObject"/>
        /// .
        /// Possible values are: 'png', 'jpg', 'jp2', 'tif', 'jbig2'.
        /// This extension can later be used together with the result of
        /// <see cref="GetImageBytes()"/>
        /// .
        /// </summary>
        /// <seealso cref="IdentifyImageType()"/>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// with recommended file extension
        /// </returns>
        public virtual String IdentifyImageFileExtension() {
            ImageType bytesType = IdentifyImageType();
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

        public virtual iText.Kernel.Pdf.Xobject.PdfImageXObject Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        protected internal static PdfStream CreatePdfStream(ImageData image, iText.Kernel.Pdf.Xobject.PdfImageXObject
             imageMask) {
            PdfStream stream;
            if (image.GetOriginalType() == ImageType.RAW) {
                RawImageHelper.UpdateImageAttributes((RawImageData)image, null);
            }
            stream = new PdfStream(image.GetData());
            String filter = image.GetFilter();
            if (filter != null && filter.Equals("JPXDecode") && image.GetColorSpace() <= 0) {
                stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                image.SetBpc(0);
            }
            stream.Put(PdfName.Type, PdfName.XObject);
            stream.Put(PdfName.Subtype, PdfName.Image);
            PdfDictionary decodeParms = CreateDictionaryFromMap(stream, image.GetDecodeParms());
            if (decodeParms != null) {
                stream.Put(PdfName.DecodeParms, decodeParms);
            }
            PdfName colorSpace;
            switch (image.GetColorSpace()) {
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
            if (image.GetBpc() != 0) {
                stream.Put(PdfName.BitsPerComponent, new PdfNumber(image.GetBpc()));
            }
            if (image.GetFilter() != null) {
                stream.Put(PdfName.Filter, new PdfName(image.GetFilter()));
            }
            //TODO: return to this later
            //        if (image.getLayer() != null)
            //            put(PdfName.OC, image.getLayer().getRef());
            if (image.GetColorSpace() == -1) {
                stream.Remove(PdfName.ColorSpace);
            }
            PdfDictionary additional = CreateDictionaryFromMap(stream, image.GetImageAttributes());
            if (additional != null) {
                stream.PutAll(additional);
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
                                if (value.Equals("Mask")) {
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
                                    //TODO Check inline images
                                    PdfStream globalsStream = new PdfStream();
                                    globalsStream.GetOutputStream().WriteBytes((byte[])value);
                                    dictionary.Put(PdfName.JBIG2Globals, globalsStream);
                                }
                                else {
                                    if (value is bool?) {
                                        dictionary.Put(new PdfName(key), new PdfBoolean((bool)value));
                                    }
                                    else {
                                        if (value is Object[]) {
                                            dictionary.Put(new PdfName(key), CreateArray(stream, (Object[])value));
                                        }
                                        else {
                                            if (value is float[]) {
                                                dictionary.Put(new PdfName(key), new PdfArray((float[])value));
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
                                //TODO instance of was removed due to autoport
                                array.Add(CreateDictionaryFromMap(stream, (IDictionary<String, Object>)obj));
                            }
                        }
                    }
                }
            }
            return array;
        }

        private void PrepareAndFindColorspace(PdfObject colorspace) {
            pngColorType = -1;
            width = GetPdfObject().GetAsNumber(PdfName.Width).IntValue();
            height = GetPdfObject().GetAsNumber(PdfName.Height).IntValue();
            bpc = GetPdfObject().GetAsNumber(PdfName.BitsPerComponent).IntValue();
            pngBitDepth = bpc;
            palette = null;
            icc = null;
            stride = 0;
            FindColorspace(colorspace, true);
        }

        /// <exception cref="System.IO.IOException"/>
        private byte[] DecodeTiffAndPngBytes(byte[] imageBytes) {
            PdfObject colorspace = GetPdfObject().Get(PdfName.ColorSpace);
            PrepareAndFindColorspace(colorspace);
            MemoryStream ms = new MemoryStream();
            if (pngColorType < 0) {
                if (bpc != 8) {
                    throw new iText.IO.IOException(iText.IO.IOException.ColorDepthIsNotSupported).SetMessageParams(bpc);
                }
                if (colorspace is PdfArray) {
                    PdfArray ca = (PdfArray)colorspace;
                    PdfObject tyca = ca.Get(0);
                    if (!PdfName.ICCBased.Equals(tyca)) {
                        throw new iText.IO.IOException(iText.IO.IOException.ColorSpaceIsNotSupported).SetMessageParams(tyca.ToString
                            ());
                    }
                    PdfStream pr = (PdfStream)ca.Get(1);
                    int n = pr.GetAsNumber(PdfName.N).IntValue();
                    if (n != 4) {
                        throw new iText.IO.IOException(iText.IO.IOException.NValueIsNotSupported).SetMessageParams(n);
                    }
                    icc = pr.GetBytes();
                }
                else {
                    if (!PdfName.DeviceCMYK.Equals(colorspace)) {
                        throw new iText.IO.IOException(iText.IO.IOException.ColorSpaceIsNotSupported).SetMessageParams(colorspace.
                            ToString());
                    }
                }
                stride = (int)(4 * width);
                TiffWriter wr = new TiffWriter();
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_SAMPLESPERPIXEL, 4));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_BITSPERSAMPLE, new int[] { 8, 8, 8, 8 }));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_PHOTOMETRIC, TIFFConstants.PHOTOMETRIC_SEPARATED
                    ));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_IMAGEWIDTH, (int)width));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_IMAGELENGTH, (int)height));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_COMPRESSION, TIFFConstants.COMPRESSION_LZW));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_PREDICTOR, TIFFConstants.PREDICTOR_HORIZONTAL_DIFFERENCING
                    ));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_ROWSPERSTRIP, (int)height));
                wr.AddField(new TiffWriter.FieldRational(TIFFConstants.TIFFTAG_XRESOLUTION, new int[] { 300, 1 }));
                wr.AddField(new TiffWriter.FieldRational(TIFFConstants.TIFFTAG_YRESOLUTION, new int[] { 300, 1 }));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_RESOLUTIONUNIT, TIFFConstants.RESUNIT_INCH));
                wr.AddField(new TiffWriter.FieldAscii(TIFFConstants.TIFFTAG_SOFTWARE, Version.GetInstance().GetVersion()));
                MemoryStream comp = new MemoryStream();
                TiffWriter.CompressLZW(comp, 2, imageBytes, (int)height, 4, stride);
                byte[] buf = comp.ToArray();
                wr.AddField(new TiffWriter.FieldImage(buf));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_STRIPBYTECOUNTS, buf.Length));
                if (icc != null) {
                    wr.AddField(new TiffWriter.FieldUndefined(TIFFConstants.TIFFTAG_ICCPROFILE, icc));
                }
                wr.WriteFile(ms);
                imageBytes = ms.ToArray();
                return imageBytes;
            }
            else {
                PngWriter png = new PngWriter(ms);
                PdfArray decode = GetPdfObject().GetAsArray(PdfName.Decode);
                if (decode != null) {
                    if (pngBitDepth == 1) {
                        // if the decode array is 1,0, then we need to invert the image
                        if (decode.GetAsNumber(0).IntValue() == 1 && decode.GetAsNumber(1).IntValue() == 0) {
                            int len = imageBytes.Length;
                            for (int t = 0; t < len; ++t) {
                                imageBytes[t] ^= 0xff;
                            }
                        }
                    }
                }
                // if the decode array is 0,1, do nothing.  It's possible that the array could be 0,0 or 1,1 - but that would be silly, so we'll just ignore that case
                // todo: add decode transformation for other depths
                png.WriteHeader((int)width, (int)height, pngBitDepth, pngColorType);
                if (icc != null) {
                    png.WriteIccProfile(icc);
                }
                if (palette != null) {
                    png.WritePalette(palette);
                }
                png.WriteData(imageBytes, stride);
                png.WriteEnd();
                imageBytes = ms.ToArray();
                return imageBytes;
            }
        }

        /// <summary>Sets state of this object according to the color space</summary>
        /// <param name="colorspace">the colorspace to use</param>
        /// <param name="allowIndexed">whether indexed color spaces will be resolved (used for recursive call)</param>
        /// <exception cref="System.IO.IOException">if there is a problem with reading from the underlying stream</exception>
        private void FindColorspace(PdfObject colorspace, bool allowIndexed) {
            if (colorspace == null && bpc == 1) {
                // handle imagemasks
                stride = (int)((width * bpc + 7) / 8);
                pngColorType = 0;
            }
            else {
                if (PdfName.DeviceGray.Equals(colorspace)) {
                    stride = (int)((width * bpc + 7) / 8);
                    pngColorType = 0;
                }
                else {
                    if (PdfName.DeviceRGB.Equals(colorspace)) {
                        if (bpc == 8 || bpc == 16) {
                            stride = (int)((width * bpc * 3 + 7) / 8);
                            pngColorType = 2;
                        }
                    }
                    else {
                        if (colorspace is PdfArray) {
                            PdfArray ca = (PdfArray)colorspace;
                            PdfObject tyca = ca.Get(0);
                            if (PdfName.CalGray.Equals(tyca)) {
                                stride = (int)((width * bpc + 7) / 8);
                                pngColorType = 0;
                            }
                            else {
                                if (PdfName.CalRGB.Equals(tyca)) {
                                    if (bpc == 8 || bpc == 16) {
                                        stride = (int)((width * bpc * 3 + 7) / 8);
                                        pngColorType = 2;
                                    }
                                }
                                else {
                                    if (PdfName.ICCBased.Equals(tyca)) {
                                        PdfStream pr = (PdfStream)ca.Get(1);
                                        int n = pr.GetAsNumber(PdfName.N).IntValue();
                                        if (n == 1) {
                                            stride = (int)((width * bpc + 7) / 8);
                                            pngColorType = 0;
                                            icc = pr.GetBytes();
                                        }
                                        else {
                                            if (n == 3) {
                                                stride = (int)((width * bpc * 3 + 7) / 8);
                                                pngColorType = 2;
                                                icc = pr.GetBytes();
                                            }
                                        }
                                    }
                                    else {
                                        if (allowIndexed && PdfName.Indexed.Equals(tyca)) {
                                            FindColorspace(ca.Get(1), false);
                                            if (pngColorType == 2) {
                                                PdfObject id2 = ca.Get(3);
                                                if (id2 is PdfString) {
                                                    palette = ((PdfString)id2).GetValueBytes();
                                                }
                                                else {
                                                    if (id2 is PdfStream) {
                                                        palette = (((PdfStream)id2)).GetBytes();
                                                    }
                                                }
                                                stride = (int)((width * bpc + 7) / 8);
                                                pngColorType = 3;
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

        private static ImageData CheckImageType(ImageData image) {
            if (image is WmfImageData) {
                throw new PdfException(PdfException.CannotCreatePdfImageXObjectByWmfImage);
            }
            return image;
        }
    }
}
