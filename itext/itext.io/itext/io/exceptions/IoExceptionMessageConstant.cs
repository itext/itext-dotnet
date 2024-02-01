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
using iText.IO.Util;

namespace iText.IO.Exceptions {
    /// <summary>Class containing constants to be used in exceptions in the IO module.</summary>
    public sealed class IoExceptionMessageConstant {
        public const String ALL_FILL_BITS_PRECEDING_EOL_CODE_MUST_BE_0 = "All fill bits preceding eol code must be 0.";

        public const String ALREADY_CLOSED = "Already closed";

        public const String BAD_ENDIANNESS_TAG_0X4949_OR_0X4D4D = "Bad endianness tag: 0x4949 or 0x4d4d.";

        public const String BAD_MAGIC_NUMBER_SHOULD_BE_42 = "Bad magic number. Should be 42.";

        public const String BITS_PER_COMPONENT_MUST_BE_1_2_4_OR_8 = "Bits per component must be 1, 2, 4 or 8.";

        public const String BITS_PER_SAMPLE_0_IS_NOT_SUPPORTED = "Bits per sample {0} is not supported.";

        public const String BIT_SAMPLES_ARE_NOT_SUPPORTED_FOR_HORIZONTAL_DIFFERENCING_PREDICTOR = "{0} bit samples are not supported for horizontal differencing predictor.";

        public const String BMP_IMAGE_EXCEPTION = "Bmp image exception.";

        public const String BROTLI_DECODING_FAILED = "Woff2 brotli decoding exception";

        public const String BUFFER_READ_FAILED = "Reading woff2 exception";

        public const String BYTES_CAN_BE_ASSIGNED_TO_BYTE_ARRAY_OUTPUT_STREAM_ONLY = "Bytes can be assigned to ByteArrayOutputStream only.";

        public const String BYTES_CAN_BE_RESET_IN_BYTE_ARRAY_OUTPUT_STREAM_ONLY = "Bytes can be reset in ByteArrayOutputStream only.";

        public const String CANNOT_FIND_FRAME = "Cannot find frame number {0} (zero-based)";

        public const String CANNOT_GET_TIFF_IMAGE_COLOR = "Cannot get TIFF image color.";

        public const String CANNOT_HANDLE_BOX_SIZES_HIGHER_THAN_2_32 = "Cannot handle box sizes higher than 2^32.";

        public const String CANNOT_INFLATE_TIFF_IMAGE = "Cannot inflate TIFF image.";

        public const String CANNOT_OPEN_OUTPUT_DIRECTORY = "Cannot open output directory for <filename>";

        public const String CANNOT_READ_TIFF_IMAGE = "Cannot read TIFF image.";

        public const String CANNOT_WRITE_BYTE = "Cannot write byte.";

        public const String CANNOT_WRITE_BYTES = "Cannot write bytes.";

        public const String CANNOT_WRITE_FLOAT_NUMBER = "Cannot write float number.";

        public const String CANNOT_WRITE_INT_NUMBER = "Cannot write int number.";

        public const String CCITT_COMPRESSION_TYPE_MUST_BE_CCITTG4_CCITTG3_1D_OR_CCITTG3_2D = "CCITT compression type must be CCITTG4, CCITTG3_1D or CCITTG3_2D.";

        public const String CHARACTER_CODE_EXCEPTION = "Character code exception.";

        public const String CMAP_WAS_NOT_FOUND = "The CMap {0} was not found.";

        public const String COLOR_DEPTH_IS_NOT_SUPPORTED = "The color depth {0} is not supported.";

        public const String COLOR_SPACE_IS_NOT_SUPPORTED = "The color space {0} is not supported.";

        public const String COMPARE_COMMAND_IS_NOT_SPECIFIED = "ImageMagick comparison command is not specified. Set the "
             + ImageMagickHelper.MAGICK_COMPARE_ENVIRONMENT_VARIABLE + " environment variable with the CLI command which can run the ImageMagic comparison."
             + " See BUILDING.MD in the root of the repository for more details.";

        public const String COMPARE_COMMAND_SPECIFIED_INCORRECTLY = "ImageMagick comparison command specified incorrectly. Set the "
             + ImageMagickHelper.MAGICK_COMPARE_ENVIRONMENT_VARIABLE + " environment variable with the CLI command which can run the ImageMagic comparison."
             + " See BUILDING.MD in the root of the repository for more details.";

        public const String COMPONENTS_MUST_BE_1_3_OR_4 = "Components must be 1, 3 or 4.";

        public const String COMPRESSION_IS_NOT_SUPPORTED = "Compression {0} is not supported.";

        public const String COMPRESSION_JPEG_IS_ONLY_SUPPORTED_WITH_A_SINGLE_STRIP_THIS_IMAGE_HAS_STRIPS = "Compression jpeg is only supported with a single strip. This image has {0} strips.";

        public const String CORRUPTED_JFIF_MARKER = "{0} corrupted jfif marker.";

        public const String DIRECTORY_NUMBER_IS_TOO_LARGE = "Directory number is too large.";

        public const String EOL_CODE_WORD_ENCOUNTERED_IN_BLACK_RUN = "EOL code word encountered in Black run.";

        public const String EOL_CODE_WORD_ENCOUNTERED_IN_WHITE_RUN = "EOL code word encountered in White run.";

        public const String ERROR_AT_FILE_POINTER = "Error at file pointer {0}.";

        public const String ERROR_READING_STRING = "Error reading string.";

        public const String ERROR_WITH_JP_MARKER = "Error with JP marker.";

        public const String EXPECTED_FTYP_MARKER = "Expected FTYP marker.";

        public const String EXPECTED_IHDR_MARKER = "Expected IHDR marker.";

        public const String EXPECTED_JP2H_MARKER = "Expected JP2H marker.";

        public const String EXPECTED_JP_MARKER = "Expected JP marker.";

        public const String EXPECTED_TRAILING_ZERO_BITS_FOR_BYTE_ALIGNED_LINES = "Expected trailing zero bits for byte-aligned lines";

        public const String EXTRA_SAMPLES_ARE_NOT_SUPPORTED = "Extra samples are not supported.";

        public const String FDF_STARTXREF_NOT_FOUND = "FDF startxref not found.";

        public const String FIRST_SCANLINE_MUST_BE_1D_ENCODED = "First scanline must be 1D encoded.";

        public const String FONT_FILE_NOT_FOUND = "Font file {0} not found.";

        public const String GHOSTSCRIPT_FAILED = "GhostScript failed for <filename>";

        public const String GIF_IMAGE_EXCEPTION = "GIF image exception.";

        public const String GIF_SIGNATURE_NOT_FOUND = "GIF signature not found.";

        public const String GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED = "Ghostscript command is not specified or specified incorrectly. Set the "
             + GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE + " environment variable to a CLI command that can run the Ghostscript application."
             + " See BUILDING.MD in the root of the repository for more details.";

        public const String GT_NOT_EXPECTED = "'>' not expected.";

        public const String ICC_PROFILE_CONTAINS_COMPONENTS_WHILE_THE_IMAGE_DATA_CONTAINS_COMPONENTS = "ICC profile contains {0} components, while the image data contains {1} components.";

        public const String ILLEGAL_VALUE_FOR_PREDICTOR_IN_TIFF_FILE = "Illegal value for predictor in TIFF file.";

        public const String IMAGE_FORMAT_CANNOT_BE_RECOGNIZED = "Image format cannot be recognized.";

        public const String IMAGE_IS_NOT_A_MASK_YOU_MUST_CALL_IMAGE_DATA_MAKE_MASK = "Image is not a mask. You must call ImageData#makeMask().";

        public const String IMAGE_MAGICK_OUTPUT_IS_NULL = "ImageMagick process output is null.";

        public const String IMAGE_MAGICK_PROCESS_EXECUTION_FAILED = "ImageMagick process execution finished with errors: ";

        public const String IMAGE_MASK_CANNOT_CONTAIN_ANOTHER_IMAGE_MASK = "Image mask cannot contain another image mask.";

        public const String INCOMPLETE_PALETTE = "Incomplete palette.";

        public const String INCORRECT_SIGNATURE = "Incorrect woff2 signature";

        public const String INVALID_BMP_FILE_COMPRESSION = "Invalid BMP file compression.";

        public const String INVALID_CODE_ENCOUNTERED = "Invalid code encountered.";

        public const String INVALID_CODE_ENCOUNTERED_WHILE_DECODING_2D_GROUP_3_COMPRESSED_DATA = "Invalid code encountered while decoding 2D group 3 compressed data.";

        public const String INVALID_CODE_ENCOUNTERED_WHILE_DECODING_2D_GROUP_4_COMPRESSED_DATA = "Invalid code encountered while decoding 2D group 4 compressed data.";

        public const String INVALID_ICC_PROFILE = "Invalid ICC profile.";

        public const String INVALID_JPEG2000_FILE = "Invalid JPEG2000 file.";

        public const String INVALID_MAGIC_VALUE_FOR_BMP_FILE_MUST_BE_BM = "Invalid magic value for bmp file. Must be 'BM'";

        public const String INVALID_TTC_FILE = "{0} is not a valid TTC file.";

        public const String INVALID_WOFF2_FONT_FILE = "Invalid WOFF2 font file.";

        public const String INVALID_WOFF_FILE = "Invalid WOFF font file.";

        public const String IO_EXCEPTION = "I/O exception.";

        public const String IS_NOT_AN_AFM_OR_PFM_FONT_FILE = "{0} is not an afm or pfm font file.";

        public const String IS_NOT_A_VALID_JPEG_FILE = "{0} is not a valid jpeg file.";

        public const String JBIG2_IMAGE_EXCEPTION = "JBIG2 image exception.";

        public const String JPEG2000_IMAGE_EXCEPTION = "JPEG2000 image exception.";

        public const String JPEG_IMAGE_EXCEPTION = "JPEG image exception.";

        public const String LOCA_SIZE_OVERFLOW = "woff2 loca table content size overflow exception";

        public const String MISSING_TAGS_FOR_OJPEG_COMPRESSION = "Missing tag(s) for OJPEG compression";

        public const String MUST_HAVE_8_BITS_PER_COMPONENT = "{0} must have 8 bits per component.";

        public const String NOT_AT_TRUE_TYPE_FILE = "{0} is not a true type file";

        public const String NOT_FOUND_AS_FILE_OR_RESOURCE = "{0} not found as file or resource.";

        public const String N_VALUE_IS_NOT_SUPPORTED = "N value {1} is not supported.";

        public const String PADDING_OVERFLOW = "woff2 padding overflow exception";

        public const String PAGE_NUMBER_MUST_BE_GT_EQ_1 = "Page number must be >= 1.";

        public const String PDF_HEADER_NOT_FOUND = "PDF header not found.";

        public const String PDF_STARTXREF_NOT_FOUND = "PDF startxref not found.";

        public const String PHOTOMETRIC_IS_NOT_SUPPORTED = "Photometric {0} is not supported.";

        public const String PLANAR_IMAGES_ARE_NOT_SUPPORTED = "Planar images are not supported.";

        public const String PNG_IMAGE_EXCEPTION = "PNG image exception.";

        public const String PREMATURE_EOF_WHILE_READING_JPEG = "Premature EOF while reading JPEG.";

        public const String READ_BASE_128_FAILED = "Reading woff2 base 128 number exception";

        public const String READ_COLLECTION_HEADER_FAILED = "Reading collection woff2 header exception";

        public const String READ_HEADER_FAILED = "Reading woff2 header exception";

        public const String READ_TABLE_DIRECTORY_FAILED = "Reading woff2 tables directory exception";

        public const String RECONSTRUCT_GLYF_TABLE_FAILED = "Reconstructing woff2 glyf table exception";

        public const String RECONSTRUCT_GLYPH_FAILED = "Reconstructing woff2 glyph exception";

        public const String RECONSTRUCT_HMTX_TABLE_FAILED = "Reconstructing woff2 hmtx table exception";

        public const String RECONSTRUCT_POINT_FAILED = "Reconstructing woff2 glyph's point exception";

        public const String RECONSTRUCT_TABLE_DIRECTORY_FAILED = "Reconstructing woff2 table directory exception";

        public const String SCANLINE_MUST_BEGIN_WITH_EOL_CODE_WORD = "Scanline must begin with EOL code word.";

        public const String TABLE_DOES_NOT_EXIST = "Table {0} does not exist.";

        public const String TABLE_DOES_NOT_EXISTS_IN = "Table {0} does not exist in {1}";

        public const String THIS_IMAGE_CAN_NOT_BE_AN_IMAGE_MASK = "This image can not be an image mask.";

        public const String TIFF_50_STYLE_LZW_CODES_ARE_NOT_SUPPORTED = "TIFF 5.0-style LZW codes are not supported.";

        public const String TIFF_FILL_ORDER_TAG_MUST_BE_EITHER_1_OR_2 = "TIFF_FILL_ORDER tag must be either 1 or 2.";

        public const String TIFF_IMAGE_EXCEPTION = "TIFF image exception.";

        public const String TILES_ARE_NOT_SUPPORTED = "Tiles are not supported.";

        public const String TRANSPARENCY_LENGTH_MUST_BE_EQUAL_TO_2_WITH_CCITT_IMAGES = "Transparency length must be equal to 2 with CCITT images";

        public const String TTC_INDEX_DOESNT_EXIST_IN_THIS_TTC_FILE = "TTC index doesn't exist in this TTC file.";

        public const String TYPE_OF_FONT_IS_NOT_RECOGNIZED = "Type of font is not recognized.";

        public const String TYPE_OF_FONT_IS_NOT_RECOGNIZED_PARAMETERIZED = "Type of font {0} is not recognized.";

        public const String UNEXPECTED_CLOSE_BRACKET = "Unexpected close bracket.";

        public const String UNEXPECTED_GT_GT = "Unexpected '>>'.";

        public const String UNKNOWN_COMPRESSION_TYPE = "Unknown compression type {0}.";

        public const String UNKNOWN_IO_EXCEPTION = "Unknown I/O exception.";

        public const String UNKNOWN_PNG_FILTER = "Unknown PNG filter.";

        public const String UNSUPPORTED_BOX_SIZE_EQ_EQ_0 = "Unsupported box size == 0.";

        public const String UNSUPPORTED_ENCODING_EXCEPTION = "Unsupported encoding exception.";

        public const String UNSUPPORTED_JPEG_MARKER = "{0} unsupported jpeg marker {1}.";

        public const String WRITE_FAILED = "Writing woff2 exception";

        public const String ENCODING_ERROR = "Error during encoding the following code point: {0} in characterset:"
             + " {1}";

        public const String ONLY_BMP_ENCODING = "This encoder only accepts BMP codepoints";
    }
}
