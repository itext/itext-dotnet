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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filters;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    /// <summary>Utility methods to help with processing of inline images</summary>
    public sealed class InlineImageParsingUtils {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Canvas.Parser.Util.InlineImageParsingUtils
            ));

        private InlineImageParsingUtils() {
        }

        /// <summary>
        /// Simple class in case users need to differentiate an exception from processing
        /// inline images vs other exceptions
        /// </summary>
        public class InlineImageParseException : PdfException {
            public InlineImageParseException(String message)
                : base(message) {
            }
        }

        /// <summary>
        /// Map between key abbreviations allowed in dictionary of inline images and their
        /// equivalent image dictionary keys
        /// </summary>
        private static readonly IDictionary<PdfName, PdfName> inlineImageEntryAbbreviationMap;

        /// <summary>Map between value abbreviations allowed in dictionary of inline images for COLORSPACE</summary>
        private static readonly IDictionary<PdfName, PdfName> inlineImageColorSpaceAbbreviationMap;

        /// <summary>Map between value abbreviations allowed in dictionary of inline images for FILTER</summary>
        private static readonly IDictionary<PdfName, PdfName> inlineImageFilterAbbreviationMap;

        static InlineImageParsingUtils() {
            // Map between key abbreviations allowed in dictionary of inline images and their
            // equivalent image dictionary keys
            inlineImageEntryAbbreviationMap = new Dictionary<PdfName, PdfName>();
            // allowed entries - just pass these through
            inlineImageEntryAbbreviationMap.Put(PdfName.BitsPerComponent, PdfName.BitsPerComponent);
            inlineImageEntryAbbreviationMap.Put(PdfName.ColorSpace, PdfName.ColorSpace);
            inlineImageEntryAbbreviationMap.Put(PdfName.Decode, PdfName.Decode);
            inlineImageEntryAbbreviationMap.Put(PdfName.DecodeParms, PdfName.DecodeParms);
            inlineImageEntryAbbreviationMap.Put(PdfName.Filter, PdfName.Filter);
            inlineImageEntryAbbreviationMap.Put(PdfName.Height, PdfName.Height);
            inlineImageEntryAbbreviationMap.Put(PdfName.ImageMask, PdfName.ImageMask);
            inlineImageEntryAbbreviationMap.Put(PdfName.Intent, PdfName.Intent);
            inlineImageEntryAbbreviationMap.Put(PdfName.Interpolate, PdfName.Interpolate);
            inlineImageEntryAbbreviationMap.Put(PdfName.Width, PdfName.Width);
            // abbreviations - transform these to corresponding correct values
            inlineImageEntryAbbreviationMap.Put(new PdfName("BPC"), PdfName.BitsPerComponent);
            inlineImageEntryAbbreviationMap.Put(new PdfName("CS"), PdfName.ColorSpace);
            inlineImageEntryAbbreviationMap.Put(new PdfName("D"), PdfName.Decode);
            inlineImageEntryAbbreviationMap.Put(new PdfName("DP"), PdfName.DecodeParms);
            inlineImageEntryAbbreviationMap.Put(new PdfName("F"), PdfName.Filter);
            inlineImageEntryAbbreviationMap.Put(new PdfName("H"), PdfName.Height);
            inlineImageEntryAbbreviationMap.Put(new PdfName("IM"), PdfName.ImageMask);
            inlineImageEntryAbbreviationMap.Put(new PdfName("I"), PdfName.Interpolate);
            inlineImageEntryAbbreviationMap.Put(new PdfName("W"), PdfName.Width);
            // Map between value abbreviations allowed in dictionary of inline images for COLORSPACE
            inlineImageColorSpaceAbbreviationMap = new Dictionary<PdfName, PdfName>();
            inlineImageColorSpaceAbbreviationMap.Put(new PdfName("G"), PdfName.DeviceGray);
            inlineImageColorSpaceAbbreviationMap.Put(new PdfName("RGB"), PdfName.DeviceRGB);
            inlineImageColorSpaceAbbreviationMap.Put(new PdfName("CMYK"), PdfName.DeviceCMYK);
            inlineImageColorSpaceAbbreviationMap.Put(new PdfName("I"), PdfName.Indexed);
            // Map between value abbreviations allowed in dictionary of inline images for FILTER
            inlineImageFilterAbbreviationMap = new Dictionary<PdfName, PdfName>();
            inlineImageFilterAbbreviationMap.Put(new PdfName("AHx"), PdfName.ASCIIHexDecode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("A85"), PdfName.ASCII85Decode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("LZW"), PdfName.LZWDecode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("Fl"), PdfName.FlateDecode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("RL"), PdfName.RunLengthDecode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("CCF"), PdfName.CCITTFaxDecode);
            inlineImageFilterAbbreviationMap.Put(new PdfName("DCT"), PdfName.DCTDecode);
        }

        /// <summary>Parses an inline image from the provided content parser.</summary>
        /// <remarks>
        /// Parses an inline image from the provided content parser.  The parser must be positioned immediately following the BI operator in the content stream.
        /// The parser will be left with current position immediately following the EI operator that terminates the inline image
        /// </remarks>
        /// <param name="ps">the content parser to use for reading the image.</param>
        /// <param name="colorSpaceDic">a color space dictionary</param>
        /// <returns>the parsed image</returns>
        public static PdfStream Parse(PdfCanvasParser ps, PdfDictionary colorSpaceDic) {
            PdfDictionary inlineImageDict = ParseDictionary(ps);
            byte[] samples = ParseSamples(inlineImageDict, colorSpaceDic, ps);
            PdfStream inlineImageAsStreamObject = new PdfStream(samples);
            inlineImageAsStreamObject.PutAll(inlineImageDict);
            return inlineImageAsStreamObject;
        }

//\cond DO_NOT_DOCUMENT
        /// <param name="colorSpaceName">the name of the color space. If null, a bi-tonal (black and white) color space is assumed.
        ///     </param>
        /// <returns>the components per pixel for the specified color space</returns>
        internal static int GetComponentsPerPixel(PdfName colorSpaceName, PdfDictionary colorSpaceDic) {
            if (colorSpaceName == null) {
                return 1;
            }
            if (colorSpaceName.Equals(PdfName.DeviceGray)) {
                return 1;
            }
            if (colorSpaceName.Equals(PdfName.DeviceRGB)) {
                return 3;
            }
            if (colorSpaceName.Equals(PdfName.DeviceCMYK)) {
                return 4;
            }
            if (colorSpaceDic != null) {
                PdfArray colorSpace = colorSpaceDic.GetAsArray(colorSpaceName);
                if (colorSpace == null) {
                    PdfName tempName = colorSpaceDic.GetAsName(colorSpaceName);
                    if (tempName != null) {
                        return GetComponentsPerPixel(tempName, colorSpaceDic);
                    }
                }
                else {
                    if (PdfName.Indexed.Equals(colorSpace.GetAsName(0)) || PdfName.CalGray.Equals(colorSpace.GetAsName(0)) || 
                        PdfName.Pattern.Equals(colorSpace.GetAsName(0)) || PdfName.Separation.Equals(colorSpace.GetAsName(0))) {
                        return 1;
                    }
                    if (PdfName.CalRGB.Equals(colorSpace.GetAsName(0)) || PdfName.Lab.Equals(colorSpace.GetAsName(0))) {
                        return 3;
                    }
                    if (PdfName.ICCBased.Equals(colorSpace.GetAsName(0))) {
                        return colorSpace.GetAsStream(1).GetAsNumber(PdfName.N).IntValue();
                    }
                    if (PdfName.DeviceN.Equals(colorSpace.GetAsName(0))) {
                        //Checking colorants dict size
                        return colorSpace.GetAsArray(1).Size();
                    }
                }
            }
            throw new InlineImageParsingUtils.InlineImageParseException(KernelExceptionMessageConstant.UNEXPECTED_COLOR_SPACE
                ).SetMessageParams(colorSpaceName);
        }
//\endcond

        /// <summary>Parses the next inline image dictionary from the parser.</summary>
        /// <remarks>
        /// Parses the next inline image dictionary from the parser.  The parser must be positioned immediately following the BI operator.
        /// The parser will be left with position immediately following the whitespace character that follows the ID operator that ends the inline image dictionary.
        /// </remarks>
        /// <param name="ps">the parser to extract the embedded image information from</param>
        /// <returns>the dictionary for the inline image, with any abbreviations converted to regular image dictionary keys and values
        ///     </returns>
        private static PdfDictionary ParseDictionary(PdfCanvasParser ps) {
            // by the time we get to here, we have already parsed the BI operator
            PdfDictionary dict = new PdfDictionary();
            for (PdfObject key = ps.ReadObject(); key != null && !"ID".Equals(key.ToString()); key = ps.ReadObject()) {
                PdfObject value = ps.ReadObject();
                PdfName resolvedKey = inlineImageEntryAbbreviationMap.Get((PdfName)key);
                if (resolvedKey == null) {
                    resolvedKey = (PdfName)key;
                }
                dict.Put(resolvedKey, GetAlternateValue(resolvedKey, value));
            }
            int ch = ps.GetTokeniser().Peek();
            //ASCIIHexDecode and ASCII85Decode are not required to have a whitespace after ID operator
            if (PdfTokenizer.IsWhitespace(ch)) {
                ps.GetTokeniser().Read();
            }
            return dict;
        }

        /// <summary>Transforms value abbreviations into their corresponding real value</summary>
        /// <param name="key">the key that the value is for</param>
        /// <param name="value">the value that might be an abbreviation</param>
        /// <returns>if value is an allowed abbreviation for the key, the expanded value for that abbreviation.  Otherwise, value is returned without modification
        ///     </returns>
        private static PdfObject GetAlternateValue(PdfName key, PdfObject value) {
            if (key == PdfName.Filter) {
                if (value is PdfName) {
                    PdfName altValue = inlineImageFilterAbbreviationMap.Get((PdfName)value);
                    if (altValue != null) {
                        return altValue;
                    }
                }
                else {
                    if (value is PdfArray) {
                        PdfArray array = ((PdfArray)value);
                        PdfArray altArray = new PdfArray();
                        int count = array.Size();
                        for (int i = 0; i < count; i++) {
                            altArray.Add(GetAlternateValue(key, array.Get(i)));
                        }
                        return altArray;
                    }
                }
            }
            else {
                if (key == PdfName.ColorSpace && value is PdfName) {
                    PdfName altValue = inlineImageColorSpaceAbbreviationMap.Get((PdfName)value);
                    if (altValue != null) {
                        return altValue;
                    }
                }
            }
            return value;
        }

        /// <summary>Computes the number of unfiltered bytes that each row of the image will contain.</summary>
        /// <remarks>
        /// Computes the number of unfiltered bytes that each row of the image will contain.
        /// If the number of bytes results in a partial terminating byte, this number is rounded up
        /// per the PDF specification
        /// </remarks>
        /// <param name="imageDictionary">the dictionary of the inline image</param>
        /// <returns>the number of bytes per row of the image</returns>
        private static int ComputeBytesPerRow(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic) {
            PdfNumber wObj = imageDictionary.GetAsNumber(PdfName.Width);
            PdfNumber bpcObj = imageDictionary.GetAsNumber(PdfName.BitsPerComponent);
            int cpp = GetComponentsPerPixel(imageDictionary.GetAsName(PdfName.ColorSpace), colorSpaceDic);
            int w = wObj.IntValue();
            int bpc = bpcObj != null ? bpcObj.IntValue() : 1;
            return (w * bpc * cpp + 7) / 8;
        }

        /// <summary>Parses the samples of the image from the underlying content parser, ignoring all filters.</summary>
        /// <remarks>
        /// Parses the samples of the image from the underlying content parser, ignoring all filters.
        /// The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
        /// The parser will be left positioned immediately following the EI operator.
        /// This is primarily useful if no filters have been applied.
        /// </remarks>
        /// <param name="imageDictionary">the dictionary of the inline image</param>
        /// <param name="ps">the content parser</param>
        /// <returns>the samples of the image</returns>
        private static byte[] ParseUnfilteredSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic, PdfCanvasParser
             ps) {
            // special case:  when no filter is specified, we just read the number of bits
            // per component, multiplied by the width and height.
            if (imageDictionary.ContainsKey(PdfName.Filter)) {
                throw new ArgumentException("Dictionary contains filters");
            }
            PdfNumber h = imageDictionary.GetAsNumber(PdfName.Height);
            int bytesToRead = ComputeBytesPerRow(imageDictionary, colorSpaceDic) * h.IntValue();
            byte[] bytes = new byte[bytesToRead];
            PdfTokenizer tokeniser = ps.GetTokeniser();
            // skip next character (which better be a whitespace character - I suppose we could check for this)
            int shouldBeWhiteSpace = tokeniser.Read();
            // from the PDF spec:  Unless the image uses ASCIIHexDecode or ASCII85Decode as one of its filters, the ID operator shall be followed by a single white-space character, and the next character shall be interpreted as the first byte of image data.
            // unfortunately, we've seen some PDFs where there is no space following the ID, so we have to capture this case and handle it
            int startIndex = 0;
            if (!PdfTokenizer.IsWhitespace(shouldBeWhiteSpace) || shouldBeWhiteSpace == 0) {
                // tokeniser treats 0 as whitespace, but for our purposes, we shouldn't
                bytes[0] = (byte)shouldBeWhiteSpace;
                startIndex++;
            }
            for (int i = startIndex; i < bytesToRead; i++) {
                int ch = tokeniser.Read();
                if (ch == -1) {
                    throw new InlineImageParsingUtils.InlineImageParseException(KernelExceptionMessageConstant.END_OF_CONTENT_STREAM_REACHED_BEFORE_END_OF_IMAGE_DATA
                        );
                }
                bytes[i] = (byte)ch;
            }
            PdfObject ei = ps.ReadObject();
            if (!"EI".Equals(ei.ToString())) {
                // Some PDF producers seem to add another non-whitespace character after the image data.
                // Let's try to handle that case here.
                PdfObject ei2 = ps.ReadObject();
                if (!"EI".Equals(ei2.ToString())) {
                    throw new InlineImageParsingUtils.InlineImageParseException(KernelExceptionMessageConstant.OPERATOR_EI_NOT_FOUND_AFTER_END_OF_IMAGE_DATA
                        );
                }
            }
            return bytes;
        }

        /// <summary>
        /// Parses the samples of the image from the underlying content parser, accounting for filters
        /// The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
        /// </summary>
        /// <remarks>
        /// Parses the samples of the image from the underlying content parser, accounting for filters
        /// The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
        /// The parser will be left positioned immediately following the EI operator.
        /// <b>Note:</b>This implementation does not actually apply the filters at this time
        /// </remarks>
        /// <param name="imageDictionary">the dictionary of the inline image</param>
        /// <param name="ps">the content parser</param>
        /// <returns>the samples of the image</returns>
        private static byte[] ParseSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic, PdfCanvasParser
             ps) {
            // by the time we get to here, we have already parsed the ID operator
            //If image is unfiltered then we can calculate exact number of bytes it occupies
            if (!imageDictionary.ContainsKey(PdfName.Filter) && ImageColorSpaceIsKnown(imageDictionary, colorSpaceDic)
                ) {
                return ParseUnfilteredSamples(imageDictionary, colorSpaceDic, ps);
            }
            PdfTokenizer tokenizer = ps.GetTokeniser();
            MemoryStream imageStream = new MemoryStream();
            int lastByte = tokenizer.Read();
            int currentByte = tokenizer.Read();
            // PDF spec is unclear about how to parse inline images. Should a whitespace
            // appear before EI or not, so reading until EI<whitespace> or EOF.
            while (currentByte != -1) {
                if (lastByte == 'E' && currentByte == 'I' && PdfTokenizer.IsWhitespace(tokenizer.Peek()) && !FollowedByBinaryData
                    (tokenizer)) {
                    byte[] image = imageStream.ToArray();
                    //Try to decode inline image as an additional safeguard and also to check for unsupported encodings
                    if (InlineImageStreamBytesAreComplete(image, imageDictionary)) {
                        return image;
                    }
                }
                imageStream.Write(lastByte);
                lastByte = currentByte;
                currentByte = tokenizer.Read();
            }
            //If EOF was encountered than image was not parsed
            throw new InlineImageParsingUtils.InlineImageParseException(KernelExceptionMessageConstant.CANNOT_FIND_IMAGE_DATA_OR_EI
                );
        }

        /// <summary>Check whether next several bytes of tokenizer contain binary data.</summary>
        /// <remarks>
        /// Check whether next several bytes of tokenizer contain binary data.
        /// This method probes 10 bytes and tries to find pdf operator in them.
        /// </remarks>
        /// <param name="tokenizer">pdf tokenizer.</param>
        /// <returns>true if next 10 bytes is binary data, false if they're most likely pdf operators.</returns>
        private static bool FollowedByBinaryData(PdfTokenizer tokenizer) {
            byte[] testSequence = new byte[10];
            tokenizer.Peek(testSequence);
            // We don't need to cleanup possible zeroes at the end, they aer whitespaces
            // so can't break our logic in followedByBinaryData(byteArr)
            bool isBinaryData = false;
            int operatorStart = -1;
            int operatorEnd = -1;
            for (int i = 0; i < testSequence.Length; ++i) {
                byte b = testSequence[i];
                //Checking for ASCII and Unicode common control characters except spaces:
                //     0x00  0x10
                //0x00	NUL	 DLE
                //0x01	SOH	 DC1
                //0x02	STX	 DC2
                //0x03	ETX	 DC3
                //0x04	EOT	 DC4
                //0x05	ENQ	 NAK
                //0x06	ACK	 SYN
                //0x07	BEL	 ETB
                //0x08	BS	 CAN
                //0x09	HT	 EM
                //0x0A	LF	 SUB
                //0x0B	VT	 ESC
                //0x0C	FF	 FS
                //0x0D	CR	 GS
                //0x0E	SO	 RS
                //0x0F	SI	 US
                //0x7F	DEL -> we have binary data
                // Also if we have any byte > 0x7f (byte < 0) than we treat it also as binary data
                // because pdf operators are in range 0x0 - 0x7f
                if (b < 0x20 && !PdfTokenizer.IsWhitespace(b)) {
                    isBinaryData = true;
                    break;
                }
                // try to find PDF operator start and end
                if (operatorStart == -1 && !PdfTokenizer.IsWhitespace(b)) {
                    operatorStart = i;
                }
                if (operatorStart != -1 && PdfTokenizer.IsWhitespace(b)) {
                    operatorEnd = i;
                    break;
                }
            }
            if (operatorEnd == -1 && operatorStart != -1) {
                operatorEnd = testSequence.Length;
            }
            //checking for any ASCII sequence here having less than 3 bytes length, because it most likely a pdf operator.
            if (operatorEnd - operatorStart > 3) {
                isBinaryData = true;
            }
            //if no operator start & end was found than it means only whitespaces were encountered or eof was reached
            //earlier, so returning false in that case, it's highly unlikely inline image will have a lot of whitespaces in
            //its data.
            return isBinaryData;
        }

        private static bool ImageColorSpaceIsKnown(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic) {
            PdfName cs = imageDictionary.GetAsName(PdfName.ColorSpace);
            if (cs == null || cs.Equals(PdfName.DeviceGray) || cs.Equals(PdfName.DeviceRGB) || cs.Equals(PdfName.DeviceCMYK
                )) {
                return true;
            }
            return colorSpaceDic != null && colorSpaceDic.ContainsKey(cs);
        }

        /// <summary>This method acts like a check that bytes that were parsed are really all image bytes.</summary>
        /// <remarks>
        /// This method acts like a check that bytes that were parsed are really all image bytes. If it's true,
        /// then decoding will succeed, but if not all image bytes were read and "&lt;ws&gt;EI&lt;ws&gt;" bytes were just a part of the image,
        /// then decoding should fail.
        /// Not the best solution, but probably there is no better and more reliable way to check this.
        /// <para />
        /// Drawbacks: slow; images with DCTDecode, JBIG2Decode and JPXDecode filters couldn't be checked as iText doesn't
        /// support these filters; what if decoding will succeed eventhough it's not all bytes?; also I'm not sure that all
        /// filters throw an exception in case data is corrupted (For example, FlateDecodeFilter seems not to throw an exception).
        /// </remarks>
        private static bool InlineImageStreamBytesAreComplete(byte[] samples, PdfDictionary imageDictionary) {
            try {
                IDictionary<PdfName, IFilterHandler> filters = new Dictionary<PdfName, IFilterHandler>(FilterHandlers.GetDefaultFilterHandlers
                    ());
                // According to pdf spec JPXDecode and JBIG2Decode are unsupported for inline images encoding
                filters.Put(PdfName.JPXDecode, new InlineImageParsingUtils.UnsupportedFilter(PdfName.JPXDecode.GetValue())
                    );
                filters.Put(PdfName.JBIG2Decode, new InlineImageParsingUtils.UnsupportedFilter(PdfName.JBIG2Decode.GetValue
                    ()));
                filters.Put(PdfName.FlateDecode, new FlateDecodeStrictFilter());
                PdfReader.DecodeBytes(samples, imageDictionary, filters);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        private class UnsupportedFilter : IFilterHandler {
            private readonly String name;

            public UnsupportedFilter(String name) {
                this.name = name;
            }

            public virtual byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
                ) {
                LOGGER.LogError(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNSUPPORTED_ENCODING_FOR_INLINE_IMAGE
                    , name));
                throw new NotSupportedException();
            }
        }
    }
}
