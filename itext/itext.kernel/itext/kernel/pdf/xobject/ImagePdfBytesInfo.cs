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
using System.IO;
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.Kernel.Actions.Data;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Xobject {
    internal class ImagePdfBytesInfo {
        private static readonly String TIFFTAG_SOFTWARE_VALUE = "iText\u00ae " + ITextCoreProductData.GetInstance(
            ).GetVersion() + " \u00a9" + ITextCoreProductData.GetInstance().GetSinceCopyrightYear() + "-" + ITextCoreProductData
            .GetInstance().GetToCopyrightYear() + " Apryse Group NV";

        private readonly int bpc;

        private readonly int width;

        private readonly int height;

        private readonly PdfObject colorspace;

        private readonly PdfArray decode;

        private int pngColorType;

        private int pngBitDepth;

        private byte[] palette;

        private byte[] icc;

        private int stride;

        public ImagePdfBytesInfo(PdfImageXObject imageXObject) {
            pngColorType = -1;
            bpc = imageXObject.GetPdfObject().GetAsNumber(PdfName.BitsPerComponent).IntValue();
            pngBitDepth = bpc;
            palette = null;
            icc = null;
            stride = 0;
            width = (int)imageXObject.GetWidth();
            height = (int)imageXObject.GetHeight();
            colorspace = imageXObject.GetPdfObject().Get(PdfName.ColorSpace);
            decode = imageXObject.GetPdfObject().GetAsArray(PdfName.Decode);
            FindColorspace(colorspace, true);
        }

        public virtual int GetPngColorType() {
            return pngColorType;
        }

        public virtual byte[] DecodeTiffAndPngBytes(byte[] imageBytes) {
            if (pngColorType < 0) {
                if (bpc != 8) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED).SetMessageParams
                        (bpc);
                }
                if (colorspace is PdfArray) {
                    PdfArray ca = (PdfArray)colorspace;
                    PdfObject tyca = ca.Get(0);
                    if (!PdfName.ICCBased.Equals(tyca)) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED).SetMessageParams
                            (tyca.ToString());
                    }
                    PdfStream pr = (PdfStream)ca.Get(1);
                    int n = pr.GetAsNumber(PdfName.N).IntValue();
                    if (n != 4) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.N_VALUE_IS_NOT_SUPPORTED).SetMessageParams
                            (n);
                    }
                    icc = pr.GetBytes();
                }
                else {
                    if (!PdfName.DeviceCMYK.Equals(colorspace)) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED).SetMessageParams
                            (colorspace.ToString());
                    }
                }
                MemoryStream ms = new MemoryStream();
                stride = 4 * width;
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
                wr.AddField(new TiffWriter.FieldAscii(TIFFConstants.TIFFTAG_SOFTWARE, TIFFTAG_SOFTWARE_VALUE));
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
                if (colorspace is PdfArray) {
                    PdfArray ca = (PdfArray)colorspace;
                    PdfObject tyca = ca.Get(0);
                    if (PdfName.Separation.Equals(tyca)) {
                        return ProcessSeperationColor(imageBytes, ca);
                    }
                }
                return ProcessPng(imageBytes, pngBitDepth, pngColorType);
            }
        }

        private byte[] ProcessSeperationColor(byte[] imageBytes, PdfArray colorSpaceArray) {
            PdfSpecialCs.Separation scs = new PdfSpecialCs.Separation(colorSpaceArray);
            byte[] newImageBytes = scs.GetTintTransformation().CalculateFromByteArray(imageBytes, 0, imageBytes.Length
                , 8, 8);
            // TODO DEVSIX-6757 switch top tiff for CMYK
            // TODO DEVSIX-6757 verify RGBA is working
            if (scs.GetBaseCs().GetNumberOfComponents() > 3) {
                throw new NotSupportedException(KernelExceptionMessageConstant.GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB
                    );
            }
            stride = (width * bpc * 3 + 7) / 8;
            return ProcessPng(newImageBytes, pngBitDepth, 2);
        }

        private byte[] ProcessPng(byte[] imageBytes, int pngBitDepth, int pngColorType) {
            MemoryStream ms = new MemoryStream();
            PngWriter png = new PngWriter(ms);
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
            // TODO DEVSIX-7015 add decode transformation for other depths
            png.WriteHeader(width, height, pngBitDepth, pngColorType);
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

        /// <summary>Sets state of this object according to the color space</summary>
        /// <param name="csObj">the colorspace to use</param>
        /// <param name="allowIndexed">whether indexed color spaces will be resolved (used for recursive call)</param>
        private void FindColorspace(PdfObject csObj, bool allowIndexed) {
            if (PdfName.DeviceGray.Equals(csObj) || (csObj == null && bpc == 1)) {
                // handle imagemasks
                stride = (width * bpc + 7) / 8;
                pngColorType = 0;
            }
            else {
                if (PdfName.DeviceRGB.Equals(csObj)) {
                    if (bpc == 8 || bpc == 16) {
                        stride = (width * bpc * 3 + 7) / 8;
                        pngColorType = 2;
                    }
                }
                else {
                    if (csObj is PdfArray) {
                        PdfArray ca = (PdfArray)csObj;
                        PdfObject tyca = ca.Get(0);
                        if (PdfName.CalGray.Equals(tyca)) {
                            stride = (width * bpc + 7) / 8;
                            pngColorType = 0;
                        }
                        else {
                            if (PdfName.CalRGB.Equals(tyca)) {
                                if (bpc == 8 || bpc == 16) {
                                    stride = (width * bpc * 3 + 7) / 8;
                                    pngColorType = 2;
                                }
                            }
                            else {
                                if (PdfName.ICCBased.Equals(tyca)) {
                                    PdfStream pr = (PdfStream)ca.Get(1);
                                    int n = pr.GetAsNumber(PdfName.N).IntValue();
                                    if (n == 1) {
                                        stride = (width * bpc + 7) / 8;
                                        pngColorType = 0;
                                        icc = pr.GetBytes();
                                    }
                                    else {
                                        if (n == 3) {
                                            stride = (width * bpc * 3 + 7) / 8;
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
                                                    palette = ((PdfStream)id2).GetBytes();
                                                }
                                            }
                                            stride = (width * bpc + 7) / 8;
                                            pngColorType = 3;
                                        }
                                    }
                                    else {
                                        if (PdfName.Separation.Equals(tyca)) {
                                            IPdfFunction fct = PdfFunctionFactory.Create(ca.Get(3));
                                            int components = fct.GetOutputSize();
                                            pngColorType = components == 1 ? 1 : 2;
                                            pngBitDepth = 8;
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
}
