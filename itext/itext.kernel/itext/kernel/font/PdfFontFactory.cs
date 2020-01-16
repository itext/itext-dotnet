/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel;
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
        private static String DEFAULT_ENCODING = "";

        /// <summary>This is the default value of the <var>embedded</var> variable.</summary>
        private static bool DEFAULT_EMBEDDING = false;

        /// <summary>This is the default value of the <var>cached</var> variable.</summary>
        private static bool DEFAULT_CACHED = true;

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
            if (CheckFontDictionary(fontDictionary, PdfName.Type1, false)) {
                return new PdfType1Font(fontDictionary);
            }
            else {
                if (CheckFontDictionary(fontDictionary, PdfName.Type0, false)) {
                    return new PdfType0Font(fontDictionary);
                }
                else {
                    if (CheckFontDictionary(fontDictionary, PdfName.TrueType, false)) {
                        return new PdfTrueTypeFont(fontDictionary);
                    }
                    else {
                        if (CheckFontDictionary(fontDictionary, PdfName.Type3, false)) {
                            return new PdfType3Font(fontDictionary);
                        }
                    }
                }
            }
            if (CheckFontDictionary(fontDictionary, PdfName.MMType1, false)) {
                // this very rare font type, that's why it was moved to the bottom of the if-else.
                return new PdfType1Font(fontDictionary);
            }
            else {
                throw new PdfException(PdfException.DictionaryDoesntHaveSupportedFontData);
            }
        }

        public static PdfFont CreateFont(String fontProgram, String encoding, PdfDocument cacheTo) {
            PdfFont pdfFont;
            if (cacheTo != null) {
                pdfFont = cacheTo.FindFont(fontProgram, encoding);
                if (pdfFont != null) {
                    return pdfFont;
                }
            }
            pdfFont = CreateFont(fontProgram, encoding);
            if (cacheTo != null) {
                pdfFont.MakeIndirect(cacheTo);
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
        /// Creates a
        /// <see cref="PdfFont"/>
        /// instance from the TrueType Collection represented by its byte contents.
        /// </summary>
        /// <param name="ttc">the byte contents of the TrueType Collection</param>
        /// <param name="ttcIndex">the index of the font in the collection, zero-based</param>
        /// <param name="encoding">
        /// the encoding of the font to be created. See
        /// <see cref="iText.IO.Font.PdfEncodings"/>
        /// </param>
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateTtcFont(byte[] ttc, int ttcIndex, String encoding, bool embedded, bool cached) {
            FontProgram fontProgram = FontProgramFactory.CreateFont(ttc, ttcIndex, cached);
            return CreateFont(fontProgram, encoding, embedded);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateTtcFont(String ttc, int ttcIndex, String encoding, bool embedded, bool cached) {
            FontProgram fontProgram = FontProgramFactory.CreateFont(ttc, ttcIndex, cached);
            return CreateFont(fontProgram, encoding, embedded);
        }

        /// <summary>
        /// Created a
        /// <see cref="PdfFont"/>
        /// instance given the path to the font file.
        /// </summary>
        /// <param name="fontProgram">the font program file</param>
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, bool embedded) {
            return CreateFont(fontProgram, DEFAULT_ENCODING, embedded);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, bool embedded) {
            return CreateFont(fontProgram, encoding, embedded, DEFAULT_CACHED);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(String fontProgram, String encoding, bool embedded, bool cached) {
            FontProgram fp = FontProgramFactory.CreateFont(fontProgram, cached);
            return CreateFont(fp, encoding, embedded);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(FontProgram fontProgram, String encoding, bool embedded) {
            if (fontProgram == null) {
                return null;
            }
            else {
                if (fontProgram is Type1Font) {
                    return new PdfType1Font((Type1Font)fontProgram, encoding, embedded);
                }
                else {
                    if (fontProgram is TrueTypeFont) {
                        if (PdfEncodings.IDENTITY_H.Equals(encoding) || PdfEncodings.IDENTITY_V.Equals(encoding)) {
                            return new PdfType0Font((TrueTypeFont)fontProgram, encoding);
                        }
                        else {
                            return new PdfTrueTypeFont((TrueTypeFont)fontProgram, encoding, embedded);
                        }
                    }
                    else {
                        if (fontProgram is CidFont) {
                            if (((CidFont)fontProgram).CompatibleWith(encoding)) {
                                return new PdfType0Font((CidFont)fontProgram, encoding);
                            }
                            else {
                                return null;
                            }
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, bool embedded) {
            return CreateFont(fontProgram, null, embedded);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, String encoding, bool embedded) {
            return CreateFont(fontProgram, encoding, embedded, DEFAULT_CACHED);
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
        /// <param name="embedded">indicates whether the font is to be embedded into the target document</param>
        /// <param name="cached">indicates whether the font will be cached</param>
        /// <returns>
        /// created
        /// <see cref="PdfFont"/>
        /// instance
        /// </returns>
        public static PdfFont CreateFont(byte[] fontProgram, String encoding, bool embedded, bool cached) {
            FontProgram fp = FontProgramFactory.CreateFont(fontProgram, cached);
            return CreateFont(fp, encoding, embedded);
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
        /// <param name="embedded">if true font will be embedded. Note, standard font won't be embedded in any case.</param>
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
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, bool embedded, int style, bool
             cached) {
            FontProgram fp = FontProgramFactory.CreateRegisteredFont(fontName, style, cached);
            return CreateFont(fp, encoding, embedded);
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
        /// <param name="embedded">if true font will be embedded. Note, standard font won't be embedded in any case.</param>
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
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, bool embedded, bool cached) {
            return CreateRegisteredFont(fontName, encoding, embedded, FontStyles.UNDEFINED, cached);
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
        /// <param name="embedded">if true font will be embedded. Note, standard font won't be embedded in any case.</param>
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
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, bool embedded) {
            return CreateRegisteredFont(fontName, encoding, embedded, FontStyles.UNDEFINED);
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
        /// <param name="embedded">if true font will be embedded. Note, standard font won't be embedded in any case.</param>
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
        public static PdfFont CreateRegisteredFont(String fontName, String encoding, bool embedded, int style) {
            return CreateRegisteredFont(fontName, encoding, embedded, style, DEFAULT_CACHED);
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
            return CreateRegisteredFont(fontName, encoding, false, FontStyles.UNDEFINED);
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
            return CreateRegisteredFont(fontName, null, false, FontStyles.UNDEFINED);
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

        /// <summary>Checks if the provided dictionary is a valid font dictionary of the provided font type.</summary>
        /// <returns><c>true</c> if the passed dictionary is a valid dictionary, <c>false</c> otherwise</returns>
        private static bool CheckFontDictionary(PdfDictionary fontDic, PdfName fontType, bool isException) {
            if (fontDic == null || fontDic.Get(PdfName.Subtype) == null || !fontDic.Get(PdfName.Subtype).Equals(fontType
                )) {
                if (isException) {
                    throw new PdfException(PdfException.DictionaryDoesntHave1FontData).SetMessageParams(fontType.GetValue());
                }
                return false;
            }
            return true;
        }
    }
}
