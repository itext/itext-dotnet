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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    /// <summary>
    /// This class provides helpful methods for creating fonts ready to be used in a
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// </summary>
    /// <remarks>
    /// This class provides helpful methods for creating fonts ready to be used in a
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// <para />
    /// Note, just created
    /// <see cref="PdfFont"/>
    /// is almost empty until it will be flushed,
    /// because it is impossible to fulfill font data until flush.
    /// </remarks>
    public sealed class PdfFontFactory {
        /// <summary>This is the default encoding to use.</summary>
        private const String DEFAULT_ENCODING = "";

        /// <summary>This is the default value of the <var>embeddedStrategy</var> variable.</summary>
        private static readonly PdfFontFactory.EmbeddingStrategy DEFAULT_EMBEDDING = PdfFontFactory.EmbeddingStrategy
            .PREFER_EMBEDDED;

        /// <summary>This is the default value of the <var>cached</var> variable.</summary>
        private const bool DEFAULT_CACHED = true;

        /// <summary>
        /// Creates a new instance of default font, namely
        /// <see cref="iText.IO.Font.Constants.StandardFonts.HELVETICA"/>
        /// standard font
        /// with
        /// <see cref="iText.IO.Font.PdfEncodings.WINANSI"/>
        /// encoding.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of default font, namely
        /// <see cref="iText.IO.Font.Constants.StandardFonts.HELVETICA"/>
        /// standard font
        /// with
        /// <see cref="iText.IO.Font.PdfEncodings.WINANSI"/>
        /// encoding.
        /// Note, if you want to reuse the same instance of default font, you may use
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>.
        /// </remarks>
        /// <returns>created font</returns>
        public static PdfFont CreateFont() {
            return CreateFont(StandardFonts.HELVETICA, DEFAULT_ENCODING);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// by already existing font dictionary.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// by already existing font dictionary.
        /// <para />
        /// Note, the font won't be added to any document,
        /// until you add it to
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
        /// While adding to
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// , or to
        /// <see cref="iText.Kernel.Pdf.PdfResources"/>
        /// the font will be made indirect implicitly.
        /// <para />
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetFont(iText.Kernel.Pdf.PdfDictionary)"/>
        /// method is strongly recommended if you want to get PdfFont by both
        /// existing font dictionary, or just created and hasn't flushed yet.
        /// </remarks>
        /// <param name="fontDictionary">the font dictionary to create the font from</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(PdfDictionary fontDictionary) {
            if (fontDictionary == null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CREATE_FONT_FROM_NULL_PDF_DICTIONARY);
            }
            PdfObject subtypeObject = fontDictionary.Get(PdfName.Subtype);
            if (PdfName.Type1.Equals(subtypeObject)) {
                return new PdfType1Font(fontDictionary);
            }
            else {
                if (PdfName.Type0.Equals(subtypeObject)) {
                    return new PdfType0Font(fontDictionary);
                }
                else {
                    if (PdfName.TrueType.Equals(subtypeObject)) {
                        return new PdfTrueTypeFont(fontDictionary);
                    }
                    else {
                        if (PdfName.Type3.Equals(subtypeObject)) {
                            return new PdfType3Font(fontDictionary);
                        }
                        else {
                            if (PdfName.MMType1.Equals(subtypeObject)) {
                                // this very rare font type, that's why it was moved to the bottom of the if-else.
                                return new PdfType1Font(fontDictionary);
                            }
                            else {
                                throw new PdfException(KernelExceptionMessageConstant.DICTIONARY_DOES_NOT_HAVE_SUPPORTED_FONT_DATA);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file and given encoding
        /// and place it inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file and given encoding
        /// and place it inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If such
        /// <see cref="PdfFont"/>
        /// has already been created
        /// and placed inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// , then retries its instance instead of creating.
        /// <see cref="EmbeddingStrategy.PREFER_EMBEDDED"/>
        /// will be used as embedding strategy.
        /// </remarks>
        /// <param name="fontProgram">the path of the font program file</param>
        /// <param name="encoding">
        /// the font encoding. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="cacheTo">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to cache the font
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, PdfDocument cacheTo) {
            return CreateFont(fontProgram, encoding, DEFAULT_EMBEDDING, cacheTo);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file and given encoding
        /// and place it inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file and given encoding
        /// and place it inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If such
        /// <see cref="PdfFont"/>
        /// has already been created
        /// and placed inside the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// , then retries its instance instead of creating.
        /// </remarks>
        /// <param name="fontProgram">the path of the font program file</param>
        /// <param name="encoding">
        /// the font encoding. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <param name="cacheTo">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to cache the font
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            , PdfDocument cacheTo) {
            if (cacheTo == null) {
                return CreateFont(fontProgram, encoding, embeddingStrategy);
            }
            PdfFont pdfFont = cacheTo.FindFont(fontProgram, encoding);
            if (pdfFont == null) {
                pdfFont = CreateFont(fontProgram, encoding, embeddingStrategy);
                if (pdfFont != null) {
                    pdfFont.MakeIndirect(cacheTo);
                }
            }
            return pdfFont;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file
        /// </summary>
        /// <param name="fontProgram">the path of the font program file</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram) {
            return CreateFont(fontProgram, DEFAULT_ENCODING);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance by the path of the font program file and given encoding.
        /// </summary>
        /// <param name="fontProgram">the path of the font program file</param>
        /// <param name="encoding">
        /// the font encoding. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding) {
            return CreateFont(fontProgram, encoding, DEFAULT_EMBEDDING);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the path to the font file.
        /// </summary>
        /// <param name="fontProgram">the font program file</param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, PdfFontFactory.EmbeddingStrategy embeddingStrategy) {
            return CreateFont(fontProgram, DEFAULT_ENCODING, embeddingStrategy);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the path to the font file.
        /// </summary>
        /// <param name="fontProgram">the font program file</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            ) {
            return CreateFont(fontProgram, encoding, embeddingStrategy, DEFAULT_CACHED);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the path to the font file.
        /// </summary>
        /// <param name="fontProgram">the font program file</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            , bool cached) {
            FontProgram fp = FontProgramFactory.CreateFont(fontProgram, encoding, cached);
            return CreateFont(fp, encoding, embeddingStrategy);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the given underlying
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instance.
        /// </summary>
        /// <param name="fontProgram">
        /// the font program of the
        /// <see cref="PdfFont"/>
        /// instance to be created
        /// </param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(FontProgram fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy) {
            if (fontProgram == null) {
                return null;
            }
            else {
                if (fontProgram is Type1Font) {
                    return CreateFontFromType1FontProgram((Type1Font)fontProgram, encoding, embeddingStrategy);
                }
                else {
                    if (fontProgram is TrueTypeFont) {
                        if (null == encoding || DEFAULT_ENCODING.Equals(encoding)) {
                            encoding = PdfEncodings.IDENTITY_H;
                        }
                        if (PdfEncodings.IDENTITY_H.Equals(encoding) || PdfEncodings.IDENTITY_V.Equals(encoding)) {
                            return CreateType0FontFromTrueTypeFontProgram((TrueTypeFont)fontProgram, encoding, embeddingStrategy);
                        }
                        else {
                            return CreateTrueTypeFontFromTrueTypeFontProgram((TrueTypeFont)fontProgram, encoding, embeddingStrategy);
                        }
                    }
                    else {
                        if (fontProgram is CidFont) {
                            return CreateType0FontFromCidFontProgram((CidFont)fontProgram, encoding, embeddingStrategy);
                        }
                        else {
                            return null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the given underlying
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instance.
        /// </summary>
        /// <param name="fontProgram">
        /// the font program of the
        /// <see cref="PdfFont"/>
        /// instance to be created
        /// </param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(FontProgram fontProgram, String encoding) {
            return CreateFont(fontProgram, encoding, DEFAULT_EMBEDDING);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the given underlying
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instance.
        /// </summary>
        /// <param name="fontProgram">
        /// the font program of the
        /// <see cref="PdfFont"/>
        /// instance to be created
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(FontProgram fontProgram) {
            return CreateFont(fontProgram, DEFAULT_ENCODING);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance by the bytes of the underlying font program.
        /// </summary>
        /// <param name="fontProgram">the bytes of the underlying font program</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, String encoding) {
            return CreateFont(fontProgram, encoding, DEFAULT_EMBEDDING);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance by the bytes of the underlying font program.
        /// </summary>
        /// <param name="fontProgram">the bytes of the underlying font program</param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, PdfFontFactory.EmbeddingStrategy embeddingStrategy) {
            return CreateFont(fontProgram, DEFAULT_ENCODING, embeddingStrategy);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance by the bytes of the underlying font program.
        /// </summary>
        /// <param name="fontProgram">the bytes of the underlying font program</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            ) {
            return CreateFont(fontProgram, encoding, embeddingStrategy, DEFAULT_CACHED);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance by the bytes of the underlying font program.
        /// </summary>
        /// <param name="fontProgram">the bytes of the underlying font program</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            , bool cached) {
            FontProgram fp = FontProgramFactory.CreateFont(fontProgram, cached);
            return CreateFont(fp, encoding, embeddingStrategy);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance from the TrueType Collection represented by its byte
        /// contents.
        /// </summary>
        /// <param name="ttc">the byte contents of the TrueType Collection</param>
        /// <param name="ttcIndex">the index of the font in the collection, zero-based</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateTtcFont(byte[] ttc, int ttcIndex, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, bool cached) {
            FontProgram fontProgram = FontProgramFactory.CreateFont(ttc, ttcIndex, cached);
            return CreateFont(fontProgram, encoding, embeddingStrategy);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance from the TrueType Collection given by the path to the .ttc file.
        /// </summary>
        /// <param name="ttc">the path of the .ttc file</param>
        /// <param name="ttcIndex">the index of the font in the collection, zero-based</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded
        /// </param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateTtcFont(String ttc, int ttcIndex, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, bool cached) {
            FontProgram fontProgram = FontProgramFactory.CreateFont(ttc, ttcIndex, cached);
            return CreateFont(fontProgram, encoding, embeddingStrategy);
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfType3Font"/>
        /// </summary>
        /// <param name="document">the target document of the new font</param>
        /// <param name="colorized">indicates whether the font will be colorized</param>
        /// <returns>created font</returns>
        public static PdfType3Font CreateType3Font(PdfDocument document, bool colorized) {
            return new PdfType3Font(document, colorized);
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfType3Font"/>
        /// </summary>
        /// <param name="document">the target document of the new font.</param>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        /// <param name="fontFamily">a preferred font family name.</param>
        /// <param name="colorized">indicates whether the font will be colorized</param>
        /// <returns>created font.</returns>
        public static PdfType3Font CreateType3Font(PdfDocument document, String fontName, String fontFamily, bool 
            colorized) {
            return new PdfType3Font(document, fontName, fontFamily, colorized);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <param name="encoding">
        /// Font encoding from
        /// <see cref="iText.IO.Font.PdfEncodings"/>.
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded.
        /// Note, standard font won't be embedded in any case.
        /// </param>
        /// <param name="style">
        /// Font style from
        /// <see cref="iText.IO.Font.Constants.FontStyles"/>.
        /// </param>
        /// <param name="cached">If true font will be cached for another PdfDocument</param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, int style, bool cached) {
            FontProgram fp = FontProgramFactory.CreateRegisteredFont(fontName, style, cached);
            return CreateFont(fp, encoding, embeddingStrategy);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <param name="encoding">
        /// Font encoding from
        /// <see cref="iText.IO.Font.PdfEncodings"/>.
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded.
        /// Note, standard font won't be embedded in any case.
        /// </param>
        /// <param name="cached">If true font will be cached for another PdfDocument</param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, bool cached) {
            return CreateRegisteredFont(fontName, encoding, embeddingStrategy, FontStyles.UNDEFINED, cached);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <param name="encoding">
        /// Font encoding from
        /// <see cref="iText.IO.Font.PdfEncodings"/>.
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded.
        /// Note, standard font won't be embedded in any case.
        /// </param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy) {
            return CreateRegisteredFont(fontName, encoding, embeddingStrategy, FontStyles.UNDEFINED);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <param name="encoding">
        /// Font encoding from
        /// <see cref="iText.IO.Font.PdfEncodings"/>.
        /// </param>
        /// <param name="embeddingStrategy">
        /// the
        /// <see cref="EmbeddingStrategy"/>
        /// which will define whether the font will be embedded.
        /// Note, standard font won't be embedded in any case.
        /// </param>
        /// <param name="style">
        /// Font style from
        /// <see cref="iText.IO.Font.Constants.FontStyles"/>.
        /// </param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, int style) {
            return CreateRegisteredFont(fontName, encoding, embeddingStrategy, style, DEFAULT_CACHED);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <param name="encoding">
        /// Font encoding from
        /// <see cref="iText.IO.Font.PdfEncodings"/>.
        /// </param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName, String encoding) {
            return CreateRegisteredFont(fontName, encoding, DEFAULT_EMBEDDING);
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="PdfFont"/>
        /// based on registered
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// 's. Required font program is expected to be
        /// previously registered by one of the register method from
        /// <see cref="PdfFontFactory"/>.
        /// </remarks>
        /// <param name="fontName">Path to font file or Standard font name</param>
        /// <returns>
        /// created font if required
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// was found among registered, otherwise null.
        /// </returns>
        /// <seealso cref="Register(System.String)"/>
        /// <seealso cref="Register(System.String, System.String)"/>
        /// <seealso cref="RegisterFamily(System.String, System.String, System.String)"/>
        /// <seealso cref="RegisterDirectory(System.String)"/>
        /// <seealso cref="RegisterSystemDirectories()"/>
        /// <seealso cref="GetRegisteredFamilies()"/>
        /// <seealso cref="GetRegisteredFonts()"/>
        public static PdfFont CreateRegisteredFont(String fontName) {
            return CreateRegisteredFont(fontName, DEFAULT_ENCODING);
        }

        /// <summary>Register a font by giving explicitly the font family and name.</summary>
        /// <param name="familyName">the font family</param>
        /// <param name="fullName">the font name</param>
        /// <param name="path">the font path</param>
        public static void RegisterFamily(String familyName, String fullName, String path) {
            FontProgramFactory.RegisterFontFamily(familyName, fullName, path);
        }

        /// <summary>Registers a .ttf, .otf, .afm, .pfm, or a .ttc font file.</summary>
        /// <remarks>
        /// Registers a .ttf, .otf, .afm, .pfm, or a .ttc font file.
        /// In case if TrueType Collection (.ttc), an additional parameter may be specified defining the index of the font
        /// to be registered, e.g. "path/to/font/collection.ttc,0". The index is zero-based.
        /// </remarks>
        /// <param name="path">the path to a font file</param>
        public static void Register(String path) {
            Register(path, null);
        }

        /// <summary>Register a font file and use an alias for the font contained in it.</summary>
        /// <param name="path">the path to a font file</param>
        /// <param name="alias">the alias you want to use for the font</param>
        public static void Register(String path, String alias) {
            FontProgramFactory.RegisterFont(path, alias);
        }

        /// <summary>Registers all the fonts in a directory.</summary>
        /// <param name="dirPath">the directory path to be registered as a font directory path</param>
        /// <returns>the number of fonts registered</returns>
        public static int RegisterDirectory(String dirPath) {
            return FontProgramFactory.RegisterFontDirectory(dirPath);
        }

        /// <summary>Register fonts in some probable directories.</summary>
        /// <remarks>
        /// Register fonts in some probable directories. It usually works in Windows,
        /// Linux and Solaris.
        /// </remarks>
        /// <returns>the number of fonts registered</returns>
        public static int RegisterSystemDirectories() {
            return FontProgramFactory.RegisterSystemFontDirectories();
        }

        /// <summary>Gets a set of registered font names.</summary>
        /// <returns>a set of registered fonts</returns>
        public static ICollection<String> GetRegisteredFonts() {
            return FontProgramFactory.GetRegisteredFonts();
        }

        /// <summary>Gets a set of registered font families.</summary>
        /// <returns>a set of registered font families</returns>
        public static ICollection<String> GetRegisteredFamilies() {
            return FontProgramFactory.GetRegisteredFontFamilies();
        }

        /// <summary>Checks if a certain font is registered.</summary>
        /// <param name="fontName">the name of the font that has to be checked.</param>
        /// <returns><c>true</c> if the font is found, <c>false</c> otherwise</returns>
        public static bool IsRegistered(String fontName) {
            return FontProgramFactory.IsRegisteredFont(fontName);
        }

        private static PdfType1Font CreateFontFromType1FontProgram(Type1Font fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy) {
            bool embedded;
            switch (embeddingStrategy) {
                case PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED: {
                    if (fontProgram.IsBuiltInFont()) {
                        throw new PdfException(KernelExceptionMessageConstant.CANNOT_EMBED_STANDARD_FONT);
                    }
                    embedded = true;
                    break;
                }

                case PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED: {
                    // can not embed standard fonts
                    embedded = !fontProgram.IsBuiltInFont();
                    break;
                }

                case PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED: {
                    embedded = false;
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNSUPPORTED_FONT_EMBEDDING_STRATEGY);
                }
            }
            return new PdfType1Font(fontProgram, encoding, embedded);
        }

        private static PdfType0Font CreateType0FontFromTrueTypeFontProgram(TrueTypeFont fontProgram, String encoding
            , PdfFontFactory.EmbeddingStrategy embeddingStrategy) {
            if (!fontProgram.GetFontNames().AllowEmbedding()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS).SetMessageParams
                    (fontProgram.GetFontNames().GetFontName() + fontProgram.GetFontNames().GetStyle());
            }
            switch (embeddingStrategy) {
                case PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED: {
                    // always embedded
                    return new PdfType0Font(fontProgram, encoding);
                }

                case PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED: {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_CREATE_TYPE_0_FONT_WITH_TRUE_TYPE_FONT_PROGRAM_WITHOUT_EMBEDDING_IT
                        );
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNSUPPORTED_FONT_EMBEDDING_STRATEGY);
                }
            }
        }

        private static PdfTrueTypeFont CreateTrueTypeFontFromTrueTypeFontProgram(TrueTypeFont fontProgram, String 
            encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy) {
            bool embedded;
            switch (embeddingStrategy) {
                case PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED: {
                    if (!fontProgram.GetFontNames().AllowEmbedding()) {
                        throw new PdfException(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS).SetMessageParams
                            (fontProgram.GetFontNames().GetFontName() + fontProgram.GetFontNames().GetStyle());
                    }
                    embedded = true;
                    break;
                }

                case PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED: {
                    embedded = fontProgram.GetFontNames().AllowEmbedding();
                    break;
                }

                case PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED: {
                    embedded = false;
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNSUPPORTED_FONT_EMBEDDING_STRATEGY);
                }
            }
            return new PdfTrueTypeFont(fontProgram, encoding, embedded);
        }

        private static PdfType0Font CreateType0FontFromCidFontProgram(CidFont fontProgram, String encoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy) {
            if (!fontProgram.CompatibleWith(encoding)) {
                return null;
            }
            switch (embeddingStrategy) {
                case PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED: {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_EMBED_TYPE_0_FONT_WITH_CID_FONT_PROGRAM);
                }

                case PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED:
                case PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED: {
                    // always not embedded
                    return new PdfType0Font(fontProgram, encoding);
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNSUPPORTED_FONT_EMBEDDING_STRATEGY);
                }
            }
        }

        /// <summary>Enum values for font embedding strategies.</summary>
        public enum EmbeddingStrategy {
            /// <summary>Force embedding fonts.</summary>
            /// <remarks>Force embedding fonts. It expected to get an exception if the font cannot be embedded.</remarks>
            FORCE_EMBEDDED,
            /// <summary>Force not embedding fonts.</summary>
            /// <remarks>
            /// Force not embedding fonts. It is expected to get an exception if the font cannot be
            /// not embedded.
            /// </remarks>
            FORCE_NOT_EMBEDDED,
            /// <summary>Embedding fonts if possible.</summary>
            PREFER_EMBEDDED,
            /// <summary>Not embedding fonts if possible.</summary>
            PREFER_NOT_EMBEDDED
        }
    }
}
