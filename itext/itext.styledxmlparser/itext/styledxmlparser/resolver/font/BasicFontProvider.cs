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
using iText.IO.Util;
using iText.Layout.Font;
using iText.Layout.Renderer;

namespace iText.StyledXmlParser.Resolver.Font {
    /// <summary>
    /// A basic
    /// <see cref="iText.Layout.Font.FontProvider"/>
    /// that allows configuring in the constructor which fonts are loaded by default.
    /// </summary>
    public class BasicFontProvider : FontProvider {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Resolver.Font.BasicFontProvider
            ));

        private const String DEFAULT_FONT_FAMILY = "Times";

        // This range excludes Hebrew, Arabic, Syriac, Arabic Supplement, Thaana, NKo, Samaritan,
        // Mandaic, Syriac Supplement, Arabic Extended-A, Devanagari, Bengali, Gurmukhi, Gujarati,
        // Oriya, Tamil, Telugu, Kannada, Malayalam, Sinhala, Thai unicode blocks.
        // Those blocks either require pdfCalligraph or aren't supported by GNU Free Fonts.
        private static readonly Range FREE_FONT_RANGE = new RangeBuilder().AddRange(0, 0x058F).AddRange(0x0E80, int.MaxValue
            ).Create();

        /// <summary>The path to the html2pdf shipped fonts.</summary>
        private const String HTML_TO_PDF_SHIPPED_FONT_RESOURCE_PATH = "iText.Html2Pdf.font.";

        /// <summary>The file names of the html2pdf shipped fonts.</summary>
        private static readonly String[] HTML_TO_PDF_SHIPPED_FONT_NAMES = new String[] { "NotoSansMono-Regular.ttf"
            , "NotoSansMono-Bold.ttf", "NotoSans-Regular.ttf", "NotoSans-Bold.ttf", "NotoSans-BoldItalic.ttf", "NotoSans-Italic.ttf"
            , "NotoSerif-Regular.ttf", "NotoSerif-Bold.ttf", "NotoSerif-BoldItalic.ttf", "NotoSerif-Italic.ttf" };

        //we want to add free fonts to font provider before calligraph fonts. However, the existing public API states
        // that addCalligraphFonts() should be used first to load calligraph fonts and to define the range for loading free fonts.
        // In order to maintain backward compatibility, this temporary field is used to stash calligraph fonts before free fonts are loaded.
        private readonly IList<byte[]> calligraphyFontsTempList = new List<byte[]>();

        /// <summary>The path to the shipped fonts.</summary>
        protected internal String shippedFontResourcePath;

        /// <summary>The file names of the shipped fonts.</summary>
        protected internal IList<String> shippedFontNames;

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        public BasicFontProvider()
            : this(true, false) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerSystemFonts)
            : this(registerStandardPdfFonts, registerSystemFonts, DEFAULT_FONT_FAMILY) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerShippedFonts">use true if you want to register the shipped fonts (can be embedded)</param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerShippedFonts, bool registerSystemFonts
            )
            : this(registerStandardPdfFonts, registerShippedFonts, registerSystemFonts, DEFAULT_FONT_FAMILY) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        /// <param name="defaultFontFamily">default font family</param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerSystemFonts, String defaultFontFamily
            )
            : this(registerStandardPdfFonts, true, registerSystemFonts, defaultFontFamily) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="registerStandardPdfFonts">use true if you want to register the standard Type 1 fonts (can't be embedded)
        ///     </param>
        /// <param name="registerShippedFonts">use true if you want to register the shipped fonts (can be embedded)</param>
        /// <param name="registerSystemFonts">use true if you want to register the system fonts (can require quite some resources)
        ///     </param>
        /// <param name="defaultFontFamily">default font family</param>
        public BasicFontProvider(bool registerStandardPdfFonts, bool registerShippedFonts, bool registerSystemFonts
            , String defaultFontFamily)
            : base(defaultFontFamily) {
            if (registerStandardPdfFonts) {
                AddStandardPdfFonts();
            }
            if (registerSystemFonts) {
                AddSystemFonts();
            }
            if (registerShippedFonts) {
                InitShippedFontsResourcePath();
                AddAllAvailableFonts(AddCalligraphFonts());
            }
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BasicFontProvider"/>
        /// instance.
        /// </summary>
        /// <param name="fontSet">predefined set of fonts, could be null.</param>
        /// <param name="defaultFontFamily">default font family.</param>
        public BasicFontProvider(FontSet fontSet, String defaultFontFamily)
            : base(fontSet, defaultFontFamily) {
        }

        /// <summary>This method loads a list of noto fonts from pdfCalligraph (if it is present in the classpath) into FontProvider.
        ///     </summary>
        /// <remarks>
        /// This method loads a list of noto fonts from pdfCalligraph (if it is present in the classpath) into FontProvider.
        /// The list is the following (each font is represented in regular and bold types): NotoSansArabic, NotoSansGurmukhi,
        /// NotoSansOriya, NotoSerifBengali, NotoSerifDevanagari, NotoSerifGujarati, NotoSerifHebrew, NotoSerifKannada,
        /// NotoSerifKhmer, NotoSerifMalayalam, NotoSerifTamil, NotoSerifTelugu, NotoSerifThai.
        /// If it's needed to have a BasicFontProvider without typography fonts loaded,
        /// create an extension of BasicFontProvider and override this method, so it does nothing and only returns null.
        /// </remarks>
        /// <returns>
        /// a unicode
        /// <see cref="iText.Layout.Font.Range"/>
        /// that excludes the loaded from pdfCalligraph fonts,
        /// i.e. the unicode range that is to be rendered with any other font contained in this FontProvider
        /// </returns>
        protected internal virtual Range AddCalligraphFonts() {
            if (TypographyUtils.IsPdfCalligraphAvailable()) {
                try {
                    IDictionary<String, byte[]> fontStreams = TypographyUtils.LoadShippedFonts();
                    this.calligraphyFontsTempList.AddAll(fontStreams.Values);
                    // here we return a unicode range that excludes the loaded from the calligraph module fonts
                    // i.e. the unicode range that is to be rendered with standard or shipped free fonts
                    return FREE_FONT_RANGE;
                }
                catch (Exception e) {
                    LOGGER.LogError(e, iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_LOADING_FONT);
                }
            }
            return null;
        }

        /// <summary>Adds fonts shipped with the font provider.</summary>
        /// <remarks>
        /// Adds fonts shipped with the font provider.
        /// For
        /// <see cref="BasicFontProvider"/>
        /// this method does nothing but can be overridden to load additional fonts.
        /// </remarks>
        /// <param name="rangeToLoad">
        /// a unicode
        /// <see cref="iText.Layout.Font.Range"/>
        /// to load characters
        /// </param>
        protected internal virtual void AddShippedFonts(Range rangeToLoad) {
            if (!IsResourcePathAvailable()) {
                return;
            }
            foreach (String fontName in shippedFontNames) {
                try {
                    using (Stream stream = ResourceUtil.GetResourceStream(shippedFontResourcePath + fontName)) {
                        byte[] fontProgramBytes = StreamUtil.InputStreamToArray(stream);
                        AddFont(fontProgramBytes, null, rangeToLoad);
                    }
                }
                catch (Exception e) {
                    LOGGER.LogError(e, iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_LOADING_FONT);
                }
            }
        }

        /// <summary>Initialize path to shipped fonts and list of font files.</summary>
        protected internal virtual void InitShippedFontsResourcePath() {
            shippedFontResourcePath = HTML_TO_PDF_SHIPPED_FONT_RESOURCE_PATH;
            shippedFontNames = new List<String>();
            //not using Collection.addAll() for auto porting
            foreach (String font in HTML_TO_PDF_SHIPPED_FONT_NAMES) {
                shippedFontNames.Add(font);
            }
        }

        private bool IsResourcePathAvailable() {
            try {
                using (Stream stream = ResourceUtil.GetResourceStream(shippedFontResourcePath + shippedFontNames[0])) {
                    if (stream == null) {
                        return false;
                    }
                }
            }
            catch (System.IO.IOException) {
                //ignore this exception, since we're just checking that such resource path is available
                return false;
            }
            return true;
        }

        private void AddAllAvailableFonts(Range rangeToLoad) {
            AddShippedFonts(rangeToLoad);
            foreach (byte[] fontData in calligraphyFontsTempList) {
                AddFont(fontData, null);
            }
            calligraphyFontsTempList.Clear();
        }
    }
}
