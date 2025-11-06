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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Codec;
using iText.Kernel.Actions.Data;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf.Xobject {
//\cond DO_NOT_DOCUMENT
    internal class ImagePdfBytesInfo {
        private static readonly String TIFFTAG_SOFTWARE_VALUE = "iText\u00ae " + ITextCoreProductData.GetInstance(
            ).GetVersion() + " \u00a9" + ITextCoreProductData.GetInstance().GetSinceCopyrightYear() + "-" + ITextCoreProductData
            .GetInstance().GetToCopyrightYear() + " Apryse Group NV";

        private readonly int width;

        private readonly int height;

        private readonly IList<IPdfFunction> colorTransformations = new List<IPdfFunction>();

        private readonly PdfImageXObject imageXObject;

        private readonly PdfImageXObject.ImageBytesRetrievalProperties properties;

        private double[] decodeArray = null;

        /// <summary>The number of color channels in the output</summary>
        private int channels;

        /// <summary>Is there an alpha channel of an alpha mask in the output</summary>
        private bool alphaChannel;

        /// <summary>color depth of output image</summary>
        private int colorDepth;

        /// <summary>palette information, null when not required</summary>
        private ImagePdfBytesInfo.Palette palette = null;

        private PdfColorSpace sourceColorSpace;

        private PdfColorSpace targetColorSpace;

        private PdfImageXObject transparencyMask;

        private ImagePdfBytesInfo.OutputFileType outputFileType;

        private byte[] iccData;

        public ImagePdfBytesInfo(PdfImageXObject imageXObject, PdfImageXObject.ImageBytesRetrievalProperties properties
            ) {
            this.properties = properties;
            this.imageXObject = imageXObject;
            if (properties.IsApplyDecodeArray() && imageXObject.GetPdfObject().ContainsKey(PdfName.Decode)) {
                decodeArray = imageXObject.GetPdfObject().GetAsArray(PdfName.Decode).ToDoubleArray();
            }
            ExtractColorInfo(imageXObject);
            width = (int)imageXObject.GetWidth();
            height = (int)imageXObject.GetHeight();
        }

        public virtual ImagePdfBytesInfo.OutputFileType GetImageType() {
            return outputFileType;
        }

        public virtual byte[] GetProcessedImageData(byte[] intialBytes) {
            if (channels > 1 && colorDepth != 8 && colorDepth != 16) {
                throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED_FOR_COLORSPACE
                    ).SetMessageParams(colorDepth, sourceColorSpace.GetName());
            }
            byte[] data = PdfReader.DecodeBytes(intialBytes, imageXObject.GetPdfObject());
            if (decodeArray != null && !IsNeutralDecodeArray(decodeArray)) {
                data = ApplyDecoding(data);
            }
            foreach (IPdfFunction fct in colorTransformations) {
                data = fct.CalculateFromByteArray(data, 0, data.Length, 1, 1);
            }
            if (transparencyMask != null) {
                data = Applytransparency(data);
            }
            ImagePdfBytesInfo.ImageProcesser proc;
            if (outputFileType == ImagePdfBytesInfo.OutputFileType.PNG) {
                proc = new ImagePdfBytesInfo.PngImageProcessor(data, transparencyMask, palette, iccData, targetColorSpace, 
                    colorDepth, width, height);
            }
            else {
                proc = new ImagePdfBytesInfo.TiffImageProcessor(data, transparencyMask, palette, iccData, targetColorSpace
                    , colorDepth, width, height);
            }
            return proc.ProcessImage();
        }

//\cond DO_NOT_DOCUMENT
        internal virtual int GetPngColorType() {
            if (outputFileType == ImagePdfBytesInfo.OutputFileType.PNG) {
                ImagePdfBytesInfo.PngImageProcessor proc = new ImagePdfBytesInfo.PngImageProcessor(new byte[0], transparencyMask
                    , palette, iccData, targetColorSpace, colorDepth, width, height);
                return (int)(proc.GetColorTypeFromColorSpace(targetColorSpace));
            }
            return -1;
        }
//\endcond

        private bool IsNeutralDecodeArray(double[] decodeArray) {
            for (int i = 0; i <= channels / 2; i++) {
                if (decodeArray[i * 2] > 0.0 && decodeArray[i * 2 + 1] < 1.0) {
                    return false;
                }
            }
            return true;
        }

        private void ExtractColorInfo(PdfImageXObject imageXObject) {
            if (imageXObject.GetPdfObject().ContainsKey(PdfName.BitsPerComponent)) {
                colorDepth = imageXObject.GetPdfObject().GetAsNumber(PdfName.BitsPerComponent).IntValue();
            }
            else {
                colorDepth = 1;
            }
            if (properties.IsApplyTransparency() && imageXObject.GetPdfObject().ContainsKey(PdfName.SMask)) {
                alphaChannel = true;
                transparencyMask = new PdfImageXObject(imageXObject.GetPdfObject().GetAsStream(PdfName.SMask));
            }
            PdfObject colorSpace;
            if (imageXObject.IsMask() || imageXObject.IsSoftMask()) {
                this.sourceColorSpace = new PdfDeviceCs.Gray();
                colorSpace = sourceColorSpace.GetPdfObject();
            }
            else {
                colorSpace = imageXObject.GetPdfObject().Get(PdfName.ColorSpace);
                this.sourceColorSpace = PdfColorSpace.MakeColorSpace(colorSpace);
            }
            this.targetColorSpace = sourceColorSpace;
            outputFileType = ImagePdfBytesInfo.OutputFileType.PNG;
            if (colorSpace.IsName()) {
                switch (((PdfName)colorSpace).GetValue()) {
                    case "DeviceGray": {
                        channels = 1;
                        break;
                    }

                    case "DeviceRGB": {
                        channels = 3;
                        break;
                    }

                    case "DeviceCMYK": {
                        channels = 4;
                        outputFileType = ImagePdfBytesInfo.OutputFileType.TIFF;
                        break;
                    }

                    default: {
                        throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED).SetMessageParams
                            (((PdfName)colorSpace).GetValue());
                    }
                }
            }
            else {
                if (colorSpace.IsArray()) {
                    PdfArray csArray = (PdfArray)colorSpace;
                    switch (((PdfName)csArray.Get(0)).GetValue()) {
                        case "Indexed": {
                            palette = new ImagePdfBytesInfo.Palette(csArray, colorDepth);
                            long color0 = IsPaletteBlackAndWhite(palette);
                            if (colorDepth == 1 && color0 >= 0) {
                                targetColorSpace = new PdfDeviceCs.Gray();
                                if (color0 == 1 && decodeArray == null) {
                                    decodeArray = new double[] { 1.0, 0.0 };
                                }
                                palette = null;
                                break;
                            }
                            if ((properties.IsApplyTransparency() && alphaChannel) || palette.GetBaseColorspace().GetNumberOfComponents
                                () == 1) {
                                targetColorSpace = palette.GetBaseColorspace();
                                colorTransformations.Add(new ImagePdfBytesInfo.DeIndexingTransformation(this, palette));
                                palette = null;
                            }
                            break;
                        }

                        case "DeviceN":
                        case "NChannel": {
                            throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED).SetMessageParams
                                (csArray.Get(0).ToString());
                        }

                        case "Separation": {
                            PdfSpecialCs.Separation separationCs = (PdfSpecialCs.Separation)this.sourceColorSpace;
                            if (properties.IsApplyTintTransformations()) {
                                colorTransformations.Add(separationCs.GetTintTransformation());
                                targetColorSpace = separationCs.GetBaseCs();
                                if (targetColorSpace.GetName() != PdfName.DeviceRGB && targetColorSpace.GetName() != PdfName.CalRGB) {
                                    throw new NotSupportedException(KernelExceptionMessageConstant.GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB
                                        );
                                }
                                if (colorDepth < 8) {
                                    throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED_FOR_SEPARATION_ALTERNATE_COLORSPACE
                                        ).SetMessageParams(colorDepth, targetColorSpace.GetName());
                                }
                            }
                            else {
                                targetColorSpace = new PdfDeviceCs.Gray();
                            }
                            break;
                        }

                        case "ICCBased": {
                            PdfStream iccStream = null;
                            if (csArray.Get(1).IsIndirectReference()) {
                                iccStream = (PdfStream)((PdfIndirectReference)csArray.Get(1)).GetRefersTo();
                            }
                            else {
                                iccStream = (PdfStream)csArray.Get(1);
                            }
                            if (targetColorSpace.GetNumberOfComponents() > 3) {
                                outputFileType = ImagePdfBytesInfo.OutputFileType.TIFF;
                            }
                            int iccComponents = targetColorSpace.GetNumberOfComponents();
                            if (iccComponents != 1 && iccComponents != 3 && iccComponents != 4) {
                                throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.N_VALUE_IS_NOT_SUPPORTED).SetMessageParams
                                    (iccComponents);
                            }
                            iccData = iccStream.GetBytes();
                            break;
                        }

                        case "CalGray":
                        case "CalRGB": {
                            break;
                        }

                        default: {
                            throw new iText.IO.Exceptions.IOException(KernelExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED).SetMessageParams
                                (csArray.Get(0));
                        }
                    }
                    channels = targetColorSpace.GetNumberOfComponents();
                }
            }
        }

        private static long IsPaletteBlackAndWhite(ImagePdfBytesInfo.Palette palette) {
            // more than 2 values
            if (palette.GetHiVal() > 1) {
                return -1;
            }
            long color0 = -1;
            for (int c = 0; c < palette.GetBaseColorspace().GetNumberOfComponents(); c++) {
                for (int i = 0; i < 2; i++) {
                    switch ((int)palette.GetColor(i)[c]) {
                        case 0: {
                            if (i == 0) {
                                color0 = 0;
                            }
                            break;
                        }

                        case 0xff: {
                            if (i == 0) {
                                color0 = 1;
                            }
                            break;
                        }

                        default: {
                            return -1;
                        }
                    }
                }
            }
            return color0;
        }

        private byte[] Applytransparency(byte[] imageData) {
            int maskMultiplier = 8 / transparencyMask.GetPdfObject().GetAsNumber(PdfName.BitsPerComponent).IntValue();
            byte[] mask = transparencyMask.GetImageBytes(false);
            mask = PdfReader.DecodeBytes(mask, transparencyMask.GetPdfObject());
            byte[] @out = new byte[(imageData.Length / channels) * (channels + 1)];
            BitmapImagePixels imageInPix = new BitmapImagePixels(this.width, this.height, colorDepth, channels, imageData
                );
            BitmapImagePixels imageOutPix = new BitmapImagePixels(this.width, this.height, colorDepth, channels + 1, @out
                );
            BitmapImagePixels maskPix = new BitmapImagePixels(this.width, this.height, colorDepth, 1, mask);
            long[] nPix = new long[channels + 1];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    long[] oPix = imageInPix.GetPixelAsLongs(x, y);
                    Array.Copy(oPix, 0, nPix, 0, channels);
                    nPix[channels] = maskPix.GetPixelAsLongs(x, y)[0] * maskMultiplier;
                    imageOutPix.SetPixel(x, y, nPix);
                }
            }
            return imageOutPix.GetData();
        }

        private byte[] ApplyDecoding(byte[] imageData) {
            BitmapImagePixels imagePixels = new BitmapImagePixels(width, height, colorDepth, sourceColorSpace.GetNumberOfComponents
                (), imageData);
            double[] factors = new double[sourceColorSpace.GetNumberOfComponents()];
            double[] floor = new double[sourceColorSpace.GetNumberOfComponents()];
            for (int i = 0; i < sourceColorSpace.GetNumberOfComponents(); i++) {
                factors[i] = (decodeArray[i * 2 + 1] - decodeArray[i * 2]);
                floor[i] = decodeArray[i * 2] * ((1 << colorDepth) - 1);
            }
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    long[] pix = imagePixels.GetPixelAsLongs(x, y);
                    for (int c = 0; c < sourceColorSpace.GetNumberOfComponents(); c++) {
                        pix[c] = (long)(floor[c] + pix[c] * factors[c]);
                    }
                    imagePixels.SetPixel(x, y, pix);
                }
            }
            return imagePixels.GetData();
        }

        public enum OutputFileType {
            TIFF,
            PNG
        }

        public enum PngColorType {
            GRAYSCALE,
            INVALID_1,
            RGB,
            PALETTE,
            GRAYSCALE_ALPHA,
            INVALID_5,
            RGBA
        }

        private class Palette {
            private readonly PdfColorSpace baseColorspace;

            private readonly int hiVal;

            private readonly int indexBitDepth;

            private readonly byte[] paletteData;

            private readonly int paletteChannels;

            public Palette(PdfArray csArray, int indexBitDepth) {
                if (csArray.Size() != 4) {
                    throw new PdfException(KernelExceptionMessageConstant.PALLET_CONTENT_ERROR);
                }
                this.indexBitDepth = indexBitDepth;
                baseColorspace = PdfColorSpace.MakeColorSpace(csArray.Get(1));
                paletteChannels = baseColorspace.GetNumberOfComponents();
                hiVal = ((PdfNumber)csArray.Get(2)).IntValue();
                PdfObject data = csArray.Get(3);
                if (data.IsStream()) {
                    paletteData = ((PdfStream)data).GetBytes();
                }
                else {
                    if (data.IsString()) {
                        paletteData = ((PdfString)data).GetValueBytes();
                    }
                    else {
                        paletteData = null;
                    }
                }
            }

            public virtual int GetIndexBitDepth() {
                return indexBitDepth;
            }

            public virtual byte[] GetPaletteData() {
                return paletteData;
            }

            public virtual PdfColorSpace GetBaseColorspace() {
                return baseColorspace;
            }

            public virtual long[] GetColor(long index) {
                long[] result = new long[paletteChannels];
                for (int c = 0; c < paletteChannels; c++) {
                    result[c] = paletteData[(int)index * paletteChannels + c] & 0xff;
                }
                return result;
            }

            public virtual int GetHiVal() {
                return hiVal;
            }
        }

        private interface ImageProcesser {
            byte[] ProcessImage();
        }

        private class TiffImageProcessor : ImagePdfBytesInfo.ImageProcesser {
            private readonly byte[] imageData;

            private readonly PdfImageXObject transparencyMask;

            private readonly PdfColorSpace colorSpace;

            private readonly int width;

            private readonly int height;

            private readonly int colorDepth;

            private readonly byte[] iccProfile;

            public TiffImageProcessor(byte[] imageData, PdfImageXObject transparencyMask, ImagePdfBytesInfo.Palette palette
                , byte[] iccProfile, PdfColorSpace colorSpace, int colorDepth, int width, int height) {
                this.imageData = imageData;
                this.transparencyMask = transparencyMask;
                this.iccProfile = iccProfile;
                this.colorSpace = colorSpace;
                this.colorDepth = colorDepth;
                this.width = width;
                this.height = height;
            }

            public virtual byte[] ProcessImage() {
                MemoryStream ms = new MemoryStream();
                int samples = colorSpace.GetNumberOfComponents();
                if (transparencyMask != null) {
                    samples++;
                }
                int[] bitsPerSample = new int[samples];
                for (int i = 0; i < samples; i++) {
                    bitsPerSample[i] = colorDepth;
                }
                int stride = samples * width;
                TiffWriter wr = new TiffWriter();
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_SAMPLESPERPIXEL, colorSpace.GetNumberOfComponents
                    ()));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_BITSPERSAMPLE, bitsPerSample));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_PHOTOMETRIC, TIFFConstants.PHOTOMETRIC_SEPARATED
                    ));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_IMAGEWIDTH, width));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_IMAGELENGTH, height));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_COMPRESSION, TIFFConstants.COMPRESSION_LZW));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_PREDICTOR, TIFFConstants.PREDICTOR_HORIZONTAL_DIFFERENCING
                    ));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_ROWSPERSTRIP, height));
                wr.AddField(new TiffWriter.FieldRational(TIFFConstants.TIFFTAG_XRESOLUTION, new int[] { 300, 1 }));
                wr.AddField(new TiffWriter.FieldRational(TIFFConstants.TIFFTAG_YRESOLUTION, new int[] { 300, 1 }));
                wr.AddField(new TiffWriter.FieldShort(TIFFConstants.TIFFTAG_RESOLUTIONUNIT, TIFFConstants.RESUNIT_INCH));
                wr.AddField(new TiffWriter.FieldAscii(TIFFConstants.TIFFTAG_SOFTWARE, TIFFTAG_SOFTWARE_VALUE));
                MemoryStream comp = new MemoryStream();
                TiffWriter.CompressLZW(comp, 2, imageData, height, samples, stride);
                byte[] buf = comp.ToArray();
                wr.AddField(new TiffWriter.FieldImage(buf));
                wr.AddField(new TiffWriter.FieldLong(TIFFConstants.TIFFTAG_STRIPBYTECOUNTS, buf.Length));
                if (iccProfile != null) {
                    wr.AddField(new TiffWriter.FieldUndefined(TIFFConstants.TIFFTAG_ICCPROFILE, iccProfile));
                }
                wr.WriteFile(ms);
                return ms.ToArray();
            }
        }

        private class PngImageProcessor : ImagePdfBytesInfo.ImageProcesser {
            private readonly byte[] imageData;

            private readonly PdfImageXObject transparencyMask;

            private readonly PdfColorSpace colorSpace;

            private readonly int width;

            private readonly int height;

            private readonly int colorDepth;

            private readonly byte[] iccProfile;

            private readonly ImagePdfBytesInfo.Palette palette;

            public PngImageProcessor(byte[] imageData, PdfImageXObject transparencyMask, ImagePdfBytesInfo.Palette palette
                , byte[] iccProfile, PdfColorSpace colorSpace, int colorDepth, int width, int height) {
                this.imageData = imageData;
                this.transparencyMask = transparencyMask;
                this.palette = palette;
                this.iccProfile = iccProfile;
                this.colorSpace = colorSpace;
                this.colorDepth = colorDepth;
                this.width = width;
                this.height = height;
            }

            public virtual ImagePdfBytesInfo.PngColorType GetColorTypeFromColorSpace(PdfColorSpace colorSpace) {
                switch (colorSpace.GetNumberOfComponents()) {
                    case 1: {
                        if (palette == null) {
                            if (transparencyMask == null) {
                                return ImagePdfBytesInfo.PngColorType.GRAYSCALE;
                            }
                            else {
                                return ImagePdfBytesInfo.PngColorType.GRAYSCALE_ALPHA;
                            }
                        }
                        else {
                            return ImagePdfBytesInfo.PngColorType.PALETTE;
                        }
                        goto case 3;
                    }

                    case 3: {
                        if (transparencyMask == null) {
                            return ImagePdfBytesInfo.PngColorType.RGB;
                        }
                        else {
                            return ImagePdfBytesInfo.PngColorType.RGBA;
                        }
                        goto default;
                    }

                    default: {
                        throw new NotSupportedException(MessageFormatUtil.Format(KernelExceptionMessageConstant.PNG_CHANNEL_ERROR, 
                            colorSpace.GetNumberOfComponents()));
                    }
                }
            }

            public virtual byte[] ProcessImage() {
                MemoryStream ms = new MemoryStream();
                PngWriter png = new PngWriter(ms);
                ImagePdfBytesInfo.PngColorType colorType = GetColorTypeFromColorSpace(colorSpace);
                png.WriteHeader(width, height, colorDepth, (int)(colorType));
                if (iccProfile != null) {
                    png.WriteIccProfile(iccProfile);
                }
                if (palette != null && palette.GetPaletteData() != null) {
                    png.WritePalette(palette.GetPaletteData());
                }
                int stride = (width * colorDepth * (colorSpace.GetNumberOfComponents() + (transparencyMask == null ? 0 : 1
                    )) + 7) / 8;
                png.WriteData(imageData, stride);
                png.WriteEnd();
                return ms.ToArray();
            }
        }

        private class DeIndexingTransformation : IPdfFunction {
            private readonly ImagePdfBytesInfo.Palette palette;

            public DeIndexingTransformation(ImagePdfBytesInfo _enclosing, ImagePdfBytesInfo.Palette palette) {
                this._enclosing = _enclosing;
                this.palette = palette;
            }

            public virtual int GetFunctionType() {
                return -1;
            }

            public virtual bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
                return this.palette.GetBaseColorspace().Equals(alternateSpace);
            }

            public virtual int GetInputSize() {
                return 1;
            }

            public virtual int GetOutputSize() {
                return this.palette.GetBaseColorspace().GetNumberOfComponents();
            }

            public virtual double[] GetDomain() {
                return new double[] { 0, 1 };
            }

            public virtual void SetDomain(double[] value) {
            }

            // not needed because this is not a real PdfFunction
            public virtual double[] GetRange() {
                double[] range = new double[this.palette.GetBaseColorspace().GetNumberOfComponents() * 2];
                for (int i = 0; i < this.palette.GetBaseColorspace().GetNumberOfComponents(); i++) {
                    range[i * 2] = 0;
                    range[i * 2 + 1] = 1;
                }
                return range;
            }

            public virtual void SetRange(double[] value) {
            }

            // not needed because this is not a real PdfFunction
            public virtual double[] Calculate(double[] input) {
                return new double[0];
            }

            public virtual byte[] CalculateFromByteArray(byte[] bytes, int offset, int length, int wordSizeInputLength
                , int wordSizeOutputLength) {
                byte[] output = new byte[this.palette.GetBaseColorspace().GetNumberOfComponents() * length * (9 - this.palette
                    .GetIndexBitDepth())];
                BitmapImagePixels indexedPixels = new BitmapImagePixels(this._enclosing.width, this._enclosing.height, this
                    ._enclosing.colorDepth, 1, bytes);
                BitmapImagePixels deIndexedPixels = new BitmapImagePixels(this._enclosing.width, this._enclosing.height, 8
                    , this.palette.GetBaseColorspace().GetNumberOfComponents(), output);
                for (int y = 0; y < this._enclosing.height; y++) {
                    for (int x = 0; x < this._enclosing.width; x++) {
                        long[] color = this.palette.GetColor(indexedPixels.GetPixelAsLongs(x, y)[0]);
                        deIndexedPixels.SetPixel(x, y, color);
                    }
                }
                return deIndexedPixels.GetData();
            }

            public virtual byte[] CalculateFromByteArray(byte[] bytes, int offset, int length, int wordSizeInputLength
                , int wordSizeOutputLength, BaseInputOutPutConvertors.IInputConversionFunction inputConvertor, BaseInputOutPutConvertors.IOutputConversionFunction
                 outputConvertor) {
                return new byte[0];
            }

            public virtual double[] ClipInput(double[] input) {
                return new double[0];
            }

            public virtual double[] ClipOutput(double[] input) {
                return new double[0];
            }

            public virtual PdfObject GetAsPdfObject() {
                return null;
            }

            private readonly ImagePdfBytesInfo _enclosing;
        }
    }
//\endcond
}
