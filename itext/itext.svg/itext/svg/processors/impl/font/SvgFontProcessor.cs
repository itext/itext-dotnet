/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
