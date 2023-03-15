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
using iText.Commons.Exceptions;
using iText.Commons.Utils;

namespace iText.IO.Exceptions {
    /// <summary>Exception class for exceptions in io module.</summary>
    public class IOException : ITextException {
        public const String AllFillBitsPrecedingEolCodeMustBe0 = "All fill bits preceding eol code must be 0.";

        public const String BadEndiannessTag0x4949Or0x4d4d = "Bad endianness tag: 0x4949 or 0x4d4d.";

        public const String BadMagicNumberShouldBe42 = "Bad magic number. Should be 42.";

        public const String BitsPerComponentMustBe1_2_4or8 = "Bits per component must be 1, 2, 4 or 8.";

        public const String BitsPerSample1IsNotSupported = "Bits per sample {0} is not supported.";

        public const String BmpImageException = "Bmp image exception.";

        public const String BytesCanBeAssignedToByteArrayOutputStreamOnly = "Bytes can be assigned to ByteArrayOutputStream only.";

        public const String BytesCanBeResetInByteArrayOutputStreamOnly = "Bytes can be reset in ByteArrayOutputStream only.";

        public const String CannotFind1Frame = "Cannot find frame number {0} (zero-based)";

        public const String CannotGetTiffImageColor = "Cannot get TIFF image color.";

        public const String CannotHandleBoxSizesHigherThan2_32 = "Cannot handle box sizes higher than 2^32.";

        public const String CannotInflateTiffImage = "Cannot inflate TIFF image.";

        public const String CannotReadTiffImage = "Cannot read TIFF image.";

        public const String CannotWriteByte = "Cannot write byte.";

        public const String CannotWriteBytes = "Cannot write bytes.";

        public const String CannotWriteFloatNumber = "Cannot write float number.";

        public const String CannotWriteIntNumber = "Cannot write int number.";

        public const String CcittCompressionTypeMustBeCcittg4Ccittg3_1dOrCcittg3_2d = "CCITT compression type must be CCITTG4, CCITTG3_1D or CCITTG3_2D.";

        public const String CharacterCodeException = "Character code exception.";

        public const String Cmap1WasNotFound = "The CMap {0} was not found.";

        public const String ColorDepthIsNotSupported = "The color depth {0} is not supported.";

        public const String ColorSpaceIsNotSupported = "The color space {0} is not supported.";

        public const String ComponentsMustBe1_3Or4 = "Components must be 1, 3 or 4.";

        public const String Compression1IsNotSupported = "Compression {0} is not supported.";

        public const String CompressionJpegIsOnlySupportedWithASingleStripThisImageHas1Strips = "Compression jpeg is only supported with a single strip. This image has {0} strips.";

        public const String DirectoryNumberIsTooLarge = "Directory number is too large.";

        public const String EolCodeWordEncounteredInBlackRun = "EOL code word encountered in Black run.";

        public const String EolCodeWordEncounteredInWhiteRun = "EOL code word encountered in White run.";

        public const String ErrorAtFilePointer1 = "Error at file pointer {0}.";

        public const String ErrorReadingString = "Error reading string.";

        public const String ErrorWithJpMarker = "Error with JP marker.";

        public const String ExpectedFtypMarker = "Expected FTYP marker.";

        public const String ExpectedIhdrMarker = "Expected IHDR marker.";

        public const String ExpectedJp2hMarker = "Expected JP2H marker.";

        public const String ExpectedJpMarker = "Expected JP marker.";

        public const String ExpectedTrailingZeroBitsForByteAlignedLines = "Expected trailing zero bits for byte-aligned lines";

        public const String ExtraSamplesAreNotSupported = "Extra samples are not supported.";

        public const String FdfStartxrefNotFound = "FDF startxref not found.";

        public const String FirstScanlineMustBe1dEncoded = "First scanline must be 1D encoded.";

        public const String FontFile1NotFound = "Font file {0} not found.";

        public const String GifImageException = "GIF image exception.";

        public const String GifSignatureNotFound = "GIF signature not found.";

        public const String GtNotExpected = "'>' not expected.";

        public const String IccProfileContains0ComponentsWhileImageDataContains1Components = "ICC profile contains {0} components, while the image data contains {1} components.";

        public const String IllegalValueForPredictorInTiffFile = "Illegal value for predictor in TIFF file.";

        public const String ImageFormatCannotBeRecognized = "Image format cannot be recognized.";

        public const String ImageIsNotMaskYouMustCallImageDataMakeMask = "Image is not a mask. You must call ImageData#makeMask().";

        public const String ImageMaskCannotContainAnotherImageMask = "Image mask cannot contain another image mask.";

        public const String IncompletePalette = "Incomplete palette.";

        public const String InvalidBmpFileCompression = "Invalid BMP file compression.";

        public const String InvalidCodeEncountered = "Invalid code encountered.";

        public const String InvalidCodeEncounteredWhileDecoding2dGroup3CompressedData = "Invalid code encountered while decoding 2D group 3 compressed data.";

        public const String InvalidCodeEncounteredWhileDecoding2dGroup4CompressedData = "Invalid code encountered while decoding 2D group 4 compressed data.";

        public const String InvalidIccProfile = "Invalid ICC profile.";

        public const String InvalidJpeg2000File = "Invalid JPEG2000 file.";

        public const String InvalidWoff2File = "Invalid WOFF2 font file.";

        public const String InvalidWoffFile = "Invalid WOFF font file.";

        public const String InvalidMagicValueForBmpFileMustBeBM = "Invalid magic value for bmp file. Must be 'BM'";

        public const String InvalidTtcFile = "{0} is not a valid TTC file.";

        public const String IoException = "I/O exception.";

        public const String Jbig2ImageException = "JBIG2 image exception.";

        public const String Jpeg2000ImageException = "JPEG2000 image exception.";

        public const String JpegImageException = "JPEG image exception.";

        public const String MissingTagsForOjpegCompression = "Missing tag(s) for OJPEG compression";

        public const String NValueIsNotSupported = "N value {1} is not supported.";

        public const String NotAtTrueTypeFile = "{0} is not a true type file";

        public const String PageNumberMustBeGtEq1 = "Page number must be >= 1.";

        public const String PdfHeaderNotFound = "PDF header not found.";

        public const String PdfStartxrefNotFound = "PDF startxref not found.";

        public const String Photometric1IsNotSupported = "Photometric {0} is not supported.";

        public const String PlanarImagesAreNotSupported = "Planar images are not supported.";

        public const String PngImageException = "PNG image exception.";

        public const String PrematureEofWhileReadingJpeg = "Premature EOF while reading JPEG.";

        public const String ScanlineMustBeginWithEolCodeWord = "Scanline must begin with EOL code word.";

        public const String TableDoesNotExist = "Table {0} does not exist.";

        public const String TableDoesNotExistsIn = "Table {0} does not exist in {1}";

        public const String ThisImageCanNotBeAnImageMask = "This image can not be an image mask.";

        public const String Tiff50StyleLzwCodesAreNotSupported = "TIFF 5.0-style LZW codes are not supported.";

        public const String TiffFillOrderTagMustBeEither1Or2 = "TIFF_FILL_ORDER tag must be either 1 or 2.";

        public const String TiffImageException = "TIFF image exception.";

        public const String TilesAreNotSupported = "Tiles are not supported.";

        public const String TransparencyLengthMustBeEqualTo2WithCcittImages = "Transparency length must be equal to 2 with CCITT images";

        public const String TtcIndexDoesNotExistInThisTtcFile = "TTC index doesn't exist in this TTC file.";

        public const String TypeOfFont1IsNotRecognized = "Type of font {0} is not recognized.";

        public const String TypeOfFontIsNotRecognized = "Type of font is not recognized.";

        public const String UnexpectedCloseBracket = "Unexpected close bracket.";

        public const String UnexpectedGtGt = "Unexpected '>>'.";

        public const String UnknownCompressionType1 = "Unknown compression type {0}.";

        public const String UnknownIOException = "Unknown I/O exception.";

        public const String UnknownPngFilter = "Unknown PNG filter.";

        public const String UnsupportedBoxSizeEqEq0 = "Unsupported box size == 0.";

        public const String UnsupportedEncodingException = "Unsupported encoding exception.";

        public const String _1BitSamplesAreNotSupportedForHorizontalDifferencingPredictor = "{0} bit samples are not supported for horizontal differencing predictor.";

        public const String _1CorruptedJfifMarker = "{0} corrupted jfif marker.";

        public const String _1IsNotAValidJpegFile = "{0} is not a valid jpeg file.";

        public const String _1IsNotAnAfmOrPfmFontFile = "{0} is not an afm or pfm font file.";

        public const String _1MustHave8BitsPerComponent = "{0} must have 8 bits per component.";

        public const String _1NotFoundAsFileOrResource = "{0} not found as file or resource.";

        public const String _1UnsupportedJpegMarker2 = "{0} unsupported jpeg marker {1}.";

        /// <summary>Object for more details</summary>
        protected internal Object obj;

        private IList<Object> messageParams;

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        public IOException(String message)
            : base(message) {
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public IOException(Exception cause)
            : this(UnknownIOException, cause) {
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="obj">an object for more details.</param>
        public IOException(String message, Object obj)
            : this(message) {
            this.obj = obj;
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public IOException(String message, Exception cause)
            : base(message, cause) {
        }

        /// <summary>Creates a new instance of IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        /// <param name="obj">an object for more details.</param>
        public IOException(String message, Exception cause, Object obj)
            : this(message, cause) {
            this.obj = obj;
        }

        /// <summary><inheritDoc/></summary>
        public override String Message {
            get {
                if (messageParams == null || messageParams.Count == 0) {
                    return base.Message;
                }
                else {
                    return MessageFormatUtil.Format(base.Message, GetMessageParams());
                }
            }
        }

        /// <summary>Gets additional params for Exception message.</summary>
        /// <returns>params for exception message.</returns>
        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[messageParams.Count];
            for (int i = 0; i < messageParams.Count; i++) {
                parameters[i] = messageParams[i];
            }
            return parameters;
        }

        /// <summary>Sets additional params for Exception message.</summary>
        /// <param name="messageParams">additional params.</param>
        /// <returns>object itself.</returns>
        public virtual iText.IO.Exceptions.IOException SetMessageParams(params Object[] messageParams) {
            this.messageParams = new List<Object>();
            this.messageParams.AddAll(messageParams);
            return this;
        }
    }
}
