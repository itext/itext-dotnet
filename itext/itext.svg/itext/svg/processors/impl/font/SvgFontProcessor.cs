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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Font;
using iText.Svg.Css.Impl;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Processors.Impl.Font {
    /// <summary>Class that processes and add resolved css fonts to the FontProvider</summary>
    public class SvgFontProcessor {
        private SvgProcessorContext context;

        public SvgFontProcessor(SvgProcessorContext context) {
            this.context = context;
        }

        /// <summary>Adds @font-face fonts to the FontProvider.</summary>
        /// <param name="cssResolver">the css styles resolver</param>
        public virtual void AddFontFaceFonts(ICssResolver cssResolver) {
            if (cssResolver is SvgStyleResolver) {
                foreach (CssFontFaceRule fontFace in ((SvgStyleResolver)cssResolver).GetFonts()) {
                    bool findSupportedSrc = false;
                    CssFontFace ff = CssFontFace.Create(fontFace.GetProperties());
                    if (ff != null) {
                        foreach (CssFontFace.CssFontFaceSrc src in ff.GetSources()) {
                            if (CreateFont(ff.GetFontFamily(), src, fontFace.ResolveUnicodeRange())) {
                                findSupportedSrc = true;
                                break;
                            }
                        }
                    }
                    if (!findSupportedSrc) {
                        ITextLogManager.GetLogger(typeof(iText.Svg.Processors.Impl.Font.SvgFontProcessor)).LogError(MessageFormatUtil
                            .Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_FONT, fontFace
                            ));
                    }
                }
            }
        }

        /// <summary>Creates a font and adds it to the context.</summary>
        /// <param name="fontFamily">the font family</param>
        /// <param name="src">the source of the font</param>
        /// <returns>true, if successful</returns>
        private bool CreateFont(String fontFamily, CssFontFace.CssFontFaceSrc src, Range unicodeRange) {
            if (!CssFontFace.IsSupportedFontFormat(src.GetFormat())) {
                return false;
            }
            else {
                if (src.IsLocal()) {
                    // to method with lazy initialization
                    ICollection<FontInfo> fonts = context.GetFontProvider().GetFontSet().Get(src.GetSrc());
                    if (fonts.Count > 0) {
                        foreach (FontInfo fi in fonts) {
                            context.AddTemporaryFont(fi, fontFamily);
                        }
                        //
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    try {
                        // Cache at resource resolver level only, at font level we will create font in any case.
                        // The instance of fontProgram will be collected by GC if the is no need in it.
                        byte[] bytes = context.GetResourceResolver().RetrieveBytesFromResource(src.GetSrc());
                        if (bytes != null) {
                            FontProgram fp = FontProgramFactory.CreateFont(bytes, false);
                            context.AddTemporaryFont(fp, PdfEncodings.IDENTITY_H, fontFamily, unicodeRange);
                            return true;
                        }
                    }
                    catch (Exception) {
                    }
                    return false;
                }
            }
        }
    }
}
