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
using System.Collections.Generic;
using iText.IO.Codec;
using iText.IO.Colors;
using iText.IO.Exceptions;
using iText.IO.Font;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    internal class TiffImageHelper {
        private class TiffParameters {
            internal TiffParameters(TiffImageData image) {
                this.image = image;
            }

            internal TiffImageData image;

            //ByteArrayOutputStream stream;
            internal bool jpegProcessing;

            internal IDictionary<String, Object> additional;
        }

        /// <summary>Processes the ImageData as a TIFF image.</summary>
        /// <param name="image">image to process.</param>
        public static void ProcessImage(ImageData image) {
            if (image.GetOriginalType() != ImageType.TIFF) {
                throw new ArgumentException("TIFF image expected");
            }
            try {
                IRandomAccessSource ras;
                if (image.GetData() == null) {
                    image.LoadData();
                }
                ras = new RandomAccessSourceFactory().CreateSource(image.GetData());
                RandomAccessFileOrArray raf = new RandomAccessFileOrArray(ras);
                TiffImageHelper.TiffParameters tiff = new TiffImageHelper.TiffParameters((TiffImageData)image);
                ProcessTiffImage(raf, tiff);
                raf.Close();
                if (!tiff.jpegProcessing) {
                    RawImageHelper.UpdateImageAttributes(tiff.image, tiff.additional);
                }
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TIFF_IMAGE_EXCEPTION, e);
            }
        }

        private static void ProcessTiffImage(RandomAccessFileOrArray s, TiffImageHelper.TiffParameters tiff) {
            bool recoverFromImageError = tiff.image.IsRecoverFromImageError();
            int page = tiff.image.GetPage();
            bool direct = tiff.image.IsDirect();
            if (page < 1) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PAGE_NUMBER_MUST_BE_GT_EQ_1);
            }
            try {
                TIFFDirectory dir = new TIFFDirectory(s, page - 1);
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_TILEWIDTH)) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TILES_ARE_NOT_SUPPORTED);
                }
                int compression = TIFFConstants.COMPRESSION_NONE;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_COMPRESSION)) {
                    compression = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_COMPRESSION);
                }
                switch (compression) {
                    case TIFFConstants.COMPRESSION_CCITTRLEW:
                    case TIFFConstants.COMPRESSION_CCITTRLE:
                    case TIFFConstants.COMPRESSION_CCITTFAX3:
                    case TIFFConstants.COMPRESSION_CCITTFAX4: {
                        break;
                    }

                    default: {
                        ProcessTiffImageColor(dir, s, tiff);
                        return;
                    }
                }
                float rotation = 0;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ORIENTATION)) {
                    int rot = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_ORIENTATION);
                    if (rot == TIFFConstants.ORIENTATION_BOTRIGHT || rot == TIFFConstants.ORIENTATION_BOTLEFT) {
                        rotation = (float)Math.PI;
                    }
                    else {
                        if (rot == TIFFConstants.ORIENTATION_LEFTTOP || rot == TIFFConstants.ORIENTATION_LEFTBOT) {
                            rotation = (float)(Math.PI / 2.0);
                        }
                        else {
                            if (rot == TIFFConstants.ORIENTATION_RIGHTTOP || rot == TIFFConstants.ORIENTATION_RIGHTBOT) {
                                rotation = -(float)(Math.PI / 2.0);
                            }
                        }
                    }
                }
                long tiffT4Options = 0;
                long tiffT6Options = 0;
                int fillOrder = 1;
                int h = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_IMAGELENGTH);
                int w = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_IMAGEWIDTH);
                float XYRatio = 0;
                int resolutionUnit = TIFFConstants.RESUNIT_INCH;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_RESOLUTIONUNIT)) {
                    resolutionUnit = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_RESOLUTIONUNIT);
                }
                int dpiX = GetDpi(dir.GetField(TIFFConstants.TIFFTAG_XRESOLUTION), resolutionUnit);
                int dpiY = GetDpi(dir.GetField(TIFFConstants.TIFFTAG_YRESOLUTION), resolutionUnit);
                if (resolutionUnit == TIFFConstants.RESUNIT_NONE) {
                    if (dpiY != 0) {
                        XYRatio = (float)dpiX / (float)dpiY;
                    }
                    dpiX = 0;
                    dpiY = 0;
                }
                int rowsStrip = h;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ROWSPERSTRIP)) {
                    rowsStrip = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_ROWSPERSTRIP);
                }
                if (rowsStrip <= 0 || rowsStrip > h) {
                    rowsStrip = h;
                }
                long[] offset = GetArrayLongShort(dir, TIFFConstants.TIFFTAG_STRIPOFFSETS);
                long[] size = GetArrayLongShort(dir, TIFFConstants.TIFFTAG_STRIPBYTECOUNTS);
                // some TIFF producers are really lousy, so...
                if ((size == null || (size.Length == 1 && (size[0] == 0 || size[0] + offset[0] > s.Length()))) && h == rowsStrip
                    ) {
                    size = new long[] { s.Length() - (int)offset[0] };
                }
                bool reverse = false;
                TIFFField fillOrderField = dir.GetField(TIFFConstants.TIFFTAG_FILLORDER);
                if (fillOrderField != null) {
                    fillOrder = fillOrderField.GetAsInt(0);
                }
                reverse = (fillOrder == TIFFConstants.FILLORDER_LSB2MSB);
                int parameters = 0;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_PHOTOMETRIC)) {
                    long photo = dir.GetFieldAsLong(TIFFConstants.TIFFTAG_PHOTOMETRIC);
                    if (photo == TIFFConstants.PHOTOMETRIC_MINISBLACK) {
                        parameters |= RawImageData.CCITT_BLACKIS1;
                    }
                }
                int imagecomp = 0;
                switch (compression) {
                    case TIFFConstants.COMPRESSION_CCITTRLEW:
                    case TIFFConstants.COMPRESSION_CCITTRLE: {
                        imagecomp = RawImageData.CCITTG3_1D;
                        parameters |= RawImageData.CCITT_ENCODEDBYTEALIGN | RawImageData.CCITT_ENDOFBLOCK;
                        break;
                    }

                    case TIFFConstants.COMPRESSION_CCITTFAX3: {
                        imagecomp = RawImageData.CCITTG3_1D;
                        parameters |= RawImageData.CCITT_ENDOFLINE | RawImageData.CCITT_ENDOFBLOCK;
                        TIFFField t4OptionsField = dir.GetField(TIFFConstants.TIFFTAG_GROUP3OPTIONS);
                        if (t4OptionsField != null) {
                            tiffT4Options = t4OptionsField.GetAsLong(0);
                            if ((tiffT4Options & TIFFConstants.GROUP3OPT_2DENCODING) != 0) {
                                imagecomp = RawImageData.CCITTG3_2D;
                            }
                            if ((tiffT4Options & TIFFConstants.GROUP3OPT_FILLBITS) != 0) {
                                parameters |= RawImageData.CCITT_ENCODEDBYTEALIGN;
                            }
                        }
                        break;
                    }

                    case TIFFConstants.COMPRESSION_CCITTFAX4: {
                        imagecomp = RawImageData.CCITTG4;
                        TIFFField t6OptionsField = dir.GetField(TIFFConstants.TIFFTAG_GROUP4OPTIONS);
                        if (t6OptionsField != null) {
                            tiffT6Options = t6OptionsField.GetAsLong(0);
                        }
                        break;
                    }
                }
                //single strip, direct
                if (direct && rowsStrip == h) {
                    byte[] im = new byte[(int)size[0]];
                    s.Seek(offset[0]);
                    s.ReadFully(im);
                    RawImageHelper.UpdateRawImageParameters(tiff.image, w, h, false, imagecomp, parameters, im, null);
                    tiff.image.SetInverted(true);
                }
                else {
                    int rowsLeft = h;
                    CCITTG4Encoder g4 = new CCITTG4Encoder(w);
                    for (int k = 0; k < offset.Length; ++k) {
                        byte[] im = new byte[(int)size[k]];
                        s.Seek(offset[k]);
                        s.ReadFully(im);
                        int height = Math.Min(rowsStrip, rowsLeft);
                        TIFFFaxDecoder decoder = new TIFFFaxDecoder(fillOrder, w, height);
                        decoder.SetRecoverFromImageError(recoverFromImageError);
                        byte[] outBuf = new byte[(w + 7) / 8 * height];
                        switch (compression) {
                            case TIFFConstants.COMPRESSION_CCITTRLEW:
                            case TIFFConstants.COMPRESSION_CCITTRLE: {
                                decoder.Decode1D(outBuf, im, 0, height);
                                g4.Fax4Encode(outBuf, height);
                                break;
                            }

                            case TIFFConstants.COMPRESSION_CCITTFAX3: {
                                try {
                                    decoder.Decode2D(outBuf, im, 0, height, tiffT4Options);
                                }
                                catch (Exception e) {
                                    // let's flip the fill bits and try again...
                                    tiffT4Options ^= TIFFConstants.GROUP3OPT_FILLBITS;
                                    try {
                                        decoder.Decode2D(outBuf, im, 0, height, tiffT4Options);
                                    }
                                    catch (Exception) {
                                        if (!recoverFromImageError) {
                                            throw e;
                                        }
                                        if (rowsStrip == 1) {
                                            throw e;
                                        }
                                        // repeat of reading the tiff directly (the if section of this if else structure)
                                        // copy pasted to avoid making a method with 10 tiff
                                        im = new byte[(int)size[0]];
                                        s.Seek(offset[0]);
                                        s.ReadFully(im);
                                        RawImageHelper.UpdateRawImageParameters(tiff.image, w, h, false, imagecomp, parameters, im, null);
                                        tiff.image.SetInverted(true);
                                        tiff.image.SetDpi(dpiX, dpiY);
                                        tiff.image.SetXYRatio(XYRatio);
                                        if (rotation != 0) {
                                            tiff.image.SetRotation(rotation);
                                        }
                                        return;
                                    }
                                }
                                g4.Fax4Encode(outBuf, height);
                                break;
                            }

                            case TIFFConstants.COMPRESSION_CCITTFAX4: {
                                try {
                                    decoder.DecodeT6(outBuf, im, 0, height, tiffT6Options);
                                }
                                catch (iText.IO.Exceptions.IOException e) {
                                    if (!recoverFromImageError) {
                                        throw;
                                    }
                                }
                                g4.Fax4Encode(outBuf, height);
                                break;
                            }
                        }
                        rowsLeft -= rowsStrip;
                    }
                    byte[] g4pic = g4.Close();
                    RawImageHelper.UpdateRawImageParameters(tiff.image, w, h, false, RawImageData.CCITTG4, parameters & RawImageData
                        .CCITT_BLACKIS1, g4pic, null);
                }
                tiff.image.SetDpi(dpiX, dpiY);
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ICCPROFILE)) {
                    try {
                        TIFFField fd = dir.GetField(TIFFConstants.TIFFTAG_ICCPROFILE);
                        IccProfile icc_prof = IccProfile.GetInstance(fd.GetAsBytes());
                        if (icc_prof.GetNumComponents() == 1) {
                            tiff.image.SetProfile(icc_prof);
                        }
                    }
                    catch (Exception) {
                    }
                }
                //empty
                if (rotation != 0) {
                    tiff.image.SetRotation(rotation);
                }
            }
            catch (Exception) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE);
            }
        }

        private static void ProcessTiffImageColor(TIFFDirectory dir, RandomAccessFileOrArray s, TiffImageHelper.TiffParameters
             tiff) {
            try {
                int compression = TIFFConstants.COMPRESSION_NONE;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_COMPRESSION)) {
                    compression = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_COMPRESSION);
                }
                int predictor = 1;
                TIFFLZWDecoder lzwDecoder = null;
                switch (compression) {
                    case TIFFConstants.COMPRESSION_NONE:
                    case TIFFConstants.COMPRESSION_LZW:
                    case TIFFConstants.COMPRESSION_PACKBITS:
                    case TIFFConstants.COMPRESSION_DEFLATE:
                    case TIFFConstants.COMPRESSION_ADOBE_DEFLATE:
                    case TIFFConstants.COMPRESSION_OJPEG:
                    case TIFFConstants.COMPRESSION_JPEG: {
                        break;
                    }

                    default: {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COMPRESSION_IS_NOT_SUPPORTED).SetMessageParams
                            (compression);
                    }
                }
                int photometric = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_PHOTOMETRIC);
                switch (photometric) {
                    case TIFFConstants.PHOTOMETRIC_MINISWHITE:
                    case TIFFConstants.PHOTOMETRIC_MINISBLACK:
                    case TIFFConstants.PHOTOMETRIC_RGB:
                    case TIFFConstants.PHOTOMETRIC_SEPARATED:
                    case TIFFConstants.PHOTOMETRIC_PALETTE: {
                        break;
                    }

                    default: {
                        if (compression != TIFFConstants.COMPRESSION_OJPEG && compression != TIFFConstants.COMPRESSION_JPEG) {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PHOTOMETRIC_IS_NOT_SUPPORTED).SetMessageParams
                                (photometric);
                        }
                        break;
                    }
                }
                float rotation = 0;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ORIENTATION)) {
                    int rot = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_ORIENTATION);
                    if (rot == TIFFConstants.ORIENTATION_BOTRIGHT || rot == TIFFConstants.ORIENTATION_BOTLEFT) {
                        rotation = (float)Math.PI;
                    }
                    else {
                        if (rot == TIFFConstants.ORIENTATION_LEFTTOP || rot == TIFFConstants.ORIENTATION_LEFTBOT) {
                            rotation = (float)(Math.PI / 2.0);
                        }
                        else {
                            if (rot == TIFFConstants.ORIENTATION_RIGHTTOP || rot == TIFFConstants.ORIENTATION_RIGHTBOT) {
                                rotation = -(float)(Math.PI / 2.0);
                            }
                        }
                    }
                }
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_PLANARCONFIG) && dir.GetFieldAsLong(TIFFConstants.TIFFTAG_PLANARCONFIG
                    ) == TIFFConstants.PLANARCONFIG_SEPARATE) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PLANAR_IMAGES_ARE_NOT_SUPPORTED);
                }
                int extraSamples = 0;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_EXTRASAMPLES)) {
                    extraSamples = 1;
                }
                int samplePerPixel = 1;
                // 1,3,4
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_SAMPLESPERPIXEL)) {
                    samplePerPixel = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_SAMPLESPERPIXEL);
                }
                int bitsPerSample = 1;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_BITSPERSAMPLE)) {
                    bitsPerSample = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_BITSPERSAMPLE);
                }
                switch (bitsPerSample) {
                    case 1:
                    case 2:
                    case 4:
                    case 8: {
                        break;
                    }

                    default: {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BITS_PER_SAMPLE_0_IS_NOT_SUPPORTED).SetMessageParams
                            (bitsPerSample);
                    }
                }
                int h = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_IMAGELENGTH);
                int w = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_IMAGEWIDTH);
                int dpiX;
                int dpiY;
                int resolutionUnit = TIFFConstants.RESUNIT_INCH;
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_RESOLUTIONUNIT)) {
                    resolutionUnit = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_RESOLUTIONUNIT);
                }
                dpiX = GetDpi(dir.GetField(TIFFConstants.TIFFTAG_XRESOLUTION), resolutionUnit);
                dpiY = GetDpi(dir.GetField(TIFFConstants.TIFFTAG_YRESOLUTION), resolutionUnit);
                int fillOrder = 1;
                TIFFField fillOrderField = dir.GetField(TIFFConstants.TIFFTAG_FILLORDER);
                if (fillOrderField != null) {
                    fillOrder = fillOrderField.GetAsInt(0);
                }
                bool reverse = (fillOrder == TIFFConstants.FILLORDER_LSB2MSB);
                int rowsStrip = h;
                // another hack for broken tiffs
                if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ROWSPERSTRIP)) {
                    rowsStrip = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_ROWSPERSTRIP);
                }
                if (rowsStrip <= 0 || rowsStrip > h) {
                    rowsStrip = h;
                }
                long[] offset = GetArrayLongShort(dir, TIFFConstants.TIFFTAG_STRIPOFFSETS);
                long[] size = GetArrayLongShort(dir, TIFFConstants.TIFFTAG_STRIPBYTECOUNTS);
                // some TIFF producers are really lousy, so...
                if ((size == null || (size.Length == 1 && (size[0] == 0 || size[0] + offset[0] > s.Length()))) && h == rowsStrip
                    ) {
                    size = new long[] { s.Length() - (int)offset[0] };
                }
                if (compression == TIFFConstants.COMPRESSION_LZW || compression == TIFFConstants.COMPRESSION_DEFLATE || compression
                     == TIFFConstants.COMPRESSION_ADOBE_DEFLATE) {
                    TIFFField predictorField = dir.GetField(TIFFConstants.TIFFTAG_PREDICTOR);
                    if (predictorField != null) {
                        predictor = predictorField.GetAsInt(0);
                        if (predictor != 1 && predictor != 2) {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.ILLEGAL_VALUE_FOR_PREDICTOR_IN_TIFF_FILE
                                );
                        }
                        if (predictor == 2 && bitsPerSample != 8) {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BIT_SAMPLES_ARE_NOT_SUPPORTED_FOR_HORIZONTAL_DIFFERENCING_PREDICTOR
                                ).SetMessageParams(bitsPerSample);
                        }
                    }
                }
                if (compression == TIFFConstants.COMPRESSION_LZW) {
                    lzwDecoder = new TIFFLZWDecoder(w, predictor, samplePerPixel);
                }
                int rowsLeft = h;
                ByteArrayOutputStream stream = null;
                ByteArrayOutputStream mstream = null;
                DeflaterOutputStream zip = null;
                DeflaterOutputStream mzip = null;
                if (extraSamples > 0) {
                    mstream = new ByteArrayOutputStream();
                    mzip = new DeflaterOutputStream(mstream);
                }
                CCITTG4Encoder g4 = null;
                if (bitsPerSample == 1 && samplePerPixel == 1 && photometric != TIFFConstants.PHOTOMETRIC_PALETTE) {
                    g4 = new CCITTG4Encoder(w);
                }
                else {
                    stream = new ByteArrayOutputStream();
                    if (compression != TIFFConstants.COMPRESSION_OJPEG && compression != TIFFConstants.COMPRESSION_JPEG) {
                        zip = new DeflaterOutputStream(stream);
                    }
                }
                if (compression == TIFFConstants.COMPRESSION_OJPEG) {
                    // Assume that the TIFFTAG_JPEGIFBYTECOUNT tag is optional, since it's obsolete and
                    // is often missing
                    if ((!dir.IsTagPresent(TIFFConstants.TIFFTAG_JPEGIFOFFSET))) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.MISSING_TAGS_FOR_OJPEG_COMPRESSION);
                    }
                    int jpegOffset = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_JPEGIFOFFSET);
                    int jpegLength = (int)s.Length() - jpegOffset;
                    if (dir.IsTagPresent(TIFFConstants.TIFFTAG_JPEGIFBYTECOUNT)) {
                        jpegLength = (int)dir.GetFieldAsLong(TIFFConstants.TIFFTAG_JPEGIFBYTECOUNT) + (int)size[0];
                    }
                    byte[] jpeg = new byte[Math.Min(jpegLength, (int)s.Length() - jpegOffset)];
                    int posFilePointer = (int)s.GetPosition();
                    posFilePointer += jpegOffset;
                    s.Seek(posFilePointer);
                    s.ReadFully(jpeg);
                    tiff.image.data = jpeg;
                    tiff.image.SetOriginalType(ImageType.JPEG);
                    JpegImageHelper.ProcessImage(tiff.image);
                    tiff.jpegProcessing = true;
                }
                else {
                    if (compression == TIFFConstants.COMPRESSION_JPEG) {
                        if (size.Length > 1) {
                            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.COMPRESSION_JPEG_IS_ONLY_SUPPORTED_WITH_A_SINGLE_STRIP_THIS_IMAGE_HAS_STRIPS
                                ).SetMessageParams(size.Length);
                        }
                        byte[] jpeg = new byte[(int)size[0]];
                        s.Seek(offset[0]);
                        s.ReadFully(jpeg);
                        // if quantization and/or Huffman tables are stored separately in the tiff,
                        // we need to add them to the jpeg data
                        TIFFField jpegtables = dir.GetField(TIFFConstants.TIFFTAG_JPEGTABLES);
                        if (jpegtables != null) {
                            byte[] temp = jpegtables.GetAsBytes();
                            int tableoffset = 0;
                            int tablelength = temp.Length;
                            // remove FFD8 from start
                            if (temp[0] == (byte)0xFF && temp[1] == (byte)0xD8) {
                                tableoffset = 2;
                                tablelength -= 2;
                            }
                            // remove FFD9 from end
                            if (temp[temp.Length - 2] == (byte)0xFF && temp[temp.Length - 1] == (byte)0xD9) {
                                tablelength -= 2;
                            }
                            byte[] tables = new byte[tablelength];
                            Array.Copy(temp, tableoffset, tables, 0, tablelength);
                            byte[] jpegwithtables = new byte[jpeg.Length + tables.Length];
                            Array.Copy(jpeg, 0, jpegwithtables, 0, 2);
                            Array.Copy(tables, 0, jpegwithtables, 2, tables.Length);
                            Array.Copy(jpeg, 2, jpegwithtables, tables.Length + 2, jpeg.Length - 2);
                            jpeg = jpegwithtables;
                        }
                        tiff.image.data = jpeg;
                        tiff.image.SetOriginalType(ImageType.JPEG);
                        JpegImageHelper.ProcessImage(tiff.image);
                        tiff.jpegProcessing = true;
                        if (photometric == TIFFConstants.PHOTOMETRIC_RGB) {
                            tiff.image.SetColorTransform(0);
                        }
                    }
                    else {
                        for (int k = 0; k < offset.Length; ++k) {
                            byte[] im = new byte[(int)size[k]];
                            s.Seek(offset[k]);
                            s.ReadFully(im);
                            int height = Math.Min(rowsStrip, rowsLeft);
                            byte[] outBuf = null;
                            if (compression != TIFFConstants.COMPRESSION_NONE) {
                                outBuf = new byte[(w * bitsPerSample * samplePerPixel + 7) / 8 * height];
                            }
                            if (reverse) {
                                TIFFFaxDecoder.ReverseBits(im);
                            }
                            switch (compression) {
                                case TIFFConstants.COMPRESSION_DEFLATE:
                                case TIFFConstants.COMPRESSION_ADOBE_DEFLATE: {
                                    FilterUtil.InflateData(im, outBuf);
                                    ApplyPredictor(outBuf, predictor, w, height, samplePerPixel);
                                    break;
                                }

                                case TIFFConstants.COMPRESSION_NONE: {
                                    outBuf = im;
                                    break;
                                }

                                case TIFFConstants.COMPRESSION_PACKBITS: {
                                    DecodePackbits(im, outBuf);
                                    break;
                                }

                                case TIFFConstants.COMPRESSION_LZW: {
                                    lzwDecoder.Decode(im, outBuf, height);
                                    break;
                                }
                            }
                            if (bitsPerSample == 1 && samplePerPixel == 1 && photometric != TIFFConstants.PHOTOMETRIC_PALETTE) {
                                g4.Fax4Encode(outBuf, height);
                            }
                            else {
                                if (extraSamples > 0) {
                                    ProcessExtraSamples(zip, mzip, outBuf, samplePerPixel, bitsPerSample, w, height);
                                }
                                else {
                                    zip.Write(outBuf);
                                }
                            }
                            rowsLeft -= rowsStrip;
                        }
                        if (bitsPerSample == 1 && samplePerPixel == 1 && photometric != TIFFConstants.PHOTOMETRIC_PALETTE) {
                            RawImageHelper.UpdateRawImageParameters(tiff.image, w, h, false, RawImageData.CCITTG4, photometric == TIFFConstants
                                .PHOTOMETRIC_MINISBLACK ? RawImageData.CCITT_BLACKIS1 : 0, g4.Close(), null);
                        }
                        else {
                            zip.Dispose();
                            RawImageHelper.UpdateRawImageParameters(tiff.image, w, h, samplePerPixel - extraSamples, bitsPerSample, stream
                                .ToArray());
                            tiff.image.SetDeflated(true);
                        }
                    }
                }
                tiff.image.SetDpi(dpiX, dpiY);
                if (compression != TIFFConstants.COMPRESSION_OJPEG && compression != TIFFConstants.COMPRESSION_JPEG) {
                    if (dir.IsTagPresent(TIFFConstants.TIFFTAG_ICCPROFILE)) {
                        try {
                            TIFFField fd = dir.GetField(TIFFConstants.TIFFTAG_ICCPROFILE);
                            IccProfile icc_prof = IccProfile.GetInstance(fd.GetAsBytes());
                            if (samplePerPixel - extraSamples == icc_prof.GetNumComponents()) {
                                tiff.image.SetProfile(icc_prof);
                            }
                        }
                        catch (Exception) {
                        }
                    }
                    //empty
                    if (dir.IsTagPresent(TIFFConstants.TIFFTAG_COLORMAP)) {
                        TIFFField fd = dir.GetField(TIFFConstants.TIFFTAG_COLORMAP);
                        char[] rgb = fd.GetAsChars();
                        byte[] palette = new byte[rgb.Length];
                        int gColor = rgb.Length / 3;
                        int bColor = gColor * 2;
                        for (int k = 0; k < gColor; ++k) {
                            //there is no sense in >>> for unsigned char
                            palette[k * 3] = (byte)(rgb[k] >> 8);
                            palette[k * 3 + 1] = (byte)(rgb[k + gColor] >> 8);
                            palette[k * 3 + 2] = (byte)(rgb[k + bColor] >> 8);
                        }
                        // Colormap components are supposed to go from 0 to 655535 but,
                        // as usually, some tiff producers just put values from 0 to 255.
                        // Let's check for these broken tiffs.
                        bool colormapBroken = true;
                        for (int k = 0; k < palette.Length; ++k) {
                            if (palette[k] != 0) {
                                colormapBroken = false;
                                break;
                            }
                        }
                        if (colormapBroken) {
                            for (int k = 0; k < gColor; ++k) {
                                palette[k * 3] = (byte)rgb[k];
                                palette[k * 3 + 1] = (byte)rgb[k + gColor];
                                palette[k * 3 + 2] = (byte)rgb[k + bColor];
                            }
                        }
                        Object[] indexed = new Object[4];
                        indexed[0] = "/Indexed";
                        indexed[1] = "/DeviceRGB";
                        indexed[2] = gColor - 1;
                        indexed[3] = PdfEncodings.ConvertToString(palette, null);
                        tiff.additional = new Dictionary<String, Object>();
                        tiff.additional.Put("ColorSpace", indexed);
                    }
                }
                if (photometric == TIFFConstants.PHOTOMETRIC_MINISWHITE) {
                    tiff.image.SetInverted(true);
                }
                if (rotation != 0) {
                    tiff.image.SetRotation(rotation);
                }
                if (extraSamples > 0) {
                    mzip.Dispose();
                    RawImageData mimg = (RawImageData)ImageDataFactory.CreateRawImage(null);
                    RawImageHelper.UpdateRawImageParameters(mimg, w, h, 1, bitsPerSample, mstream.ToArray());
                    mimg.MakeMask();
                    mimg.SetDeflated(true);
                    tiff.image.SetImageMask(mimg);
                }
            }
            catch (Exception) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_GET_TIFF_IMAGE_COLOR);
            }
        }

        private static int GetDpi(TIFFField fd, int resolutionUnit) {
            if (fd == null) {
                return 0;
            }
            long[] res = fd.GetAsRational(0);
            float frac = (float)res[0] / (float)res[1];
            int dpi = 0;
            switch (resolutionUnit) {
                case TIFFConstants.RESUNIT_INCH:
                case TIFFConstants.RESUNIT_NONE: {
                    dpi = (int)(frac + 0.5);
                    break;
                }

                case TIFFConstants.RESUNIT_CENTIMETER: {
                    dpi = (int)(frac * 2.54 + 0.5);
                    break;
                }
            }
            return dpi;
        }

        private static void ProcessExtraSamples(DeflaterOutputStream zip, DeflaterOutputStream mzip, byte[] outBuf
            , int samplePerPixel, int bitsPerSample, int width, int height) {
            if (bitsPerSample == 8) {
                byte[] mask = new byte[width * height];
                int mptr = 0;
                int optr = 0;
                int total = width * height * samplePerPixel;
                for (int k = 0; k < total; k += samplePerPixel) {
                    for (int s = 0; s < samplePerPixel - 1; ++s) {
                        outBuf[optr++] = outBuf[k + s];
                    }
                    mask[mptr++] = outBuf[k + samplePerPixel - 1];
                }
                zip.Write(outBuf, 0, optr);
                mzip.Write(mask, 0, mptr);
            }
            else {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.EXTRA_SAMPLES_ARE_NOT_SUPPORTED);
            }
        }

        private static long[] GetArrayLongShort(TIFFDirectory dir, int tag) {
            TIFFField field = dir.GetField(tag);
            if (field == null) {
                return null;
            }
            long[] offset;
            if (field.GetFieldType() == TIFFField.TIFF_LONG) {
                offset = field.GetAsLongs();
            }
            else {
                // must be short
                char[] temp = field.GetAsChars();
                offset = new long[temp.Length];
                for (int k = 0; k < temp.Length; ++k) {
                    offset[k] = temp[k];
                }
            }
            return offset;
        }

        // Uncompress packbits compressed image data.
        private static void DecodePackbits(byte[] data, byte[] dst) {
            int srcCount = 0;
            int dstCount = 0;
            byte repeat;
            byte b;
            try {
                while (dstCount < dst.Length) {
                    b = data[srcCount++];
                    // In Java b <= 127 is always true and the same is for .NET and b >= 0 expression,
                    // checking both for the sake of consistency.
                    if (b >= 0 && b <= 127) {
                        // literal run packet
                        for (int i = 0; i < (b + 1); i++) {
                            dst[dstCount++] = data[srcCount++];
                        }
                    }
                    else {
                        // It seems that in Java and .NET (b & 0x80) != 0 would always be true here, however still checking it
                        // to be more explicit.
                        if ((b & 0x80) != 0 && b != (byte)0x80) {
                            // 2 byte encoded run packet
                            repeat = data[srcCount++];
                            // (~b & 0xff) + 2 is getting -b + 1 via bitwise operations,
                            // treating b as signed byte. This approach works both for Java and .NET.
                            // This is because `~x == (-x) - 1` for signed number values.
                            for (int i = 0; i < (~b & 0xff) + 2; i++) {
                                dst[dstCount++] = repeat;
                            }
                        }
                        else {
                            // no-op packet. Do nothing
                            srcCount++;
                        }
                    }
                }
            }
            catch (Exception) {
            }
        }

        // do nothing
        private static void ApplyPredictor(byte[] uncompData, int predictor, int w, int h, int samplesPerPixel) {
            if (predictor != 2) {
                return;
            }
            int count;
            for (int j = 0; j < h; j++) {
                count = samplesPerPixel * (j * w + 1);
                for (int i = samplesPerPixel; i < w * samplesPerPixel; i++) {
                    uncompData[count] += uncompData[count - samplesPerPixel];
                    count++;
                }
            }
        }
    }
}
