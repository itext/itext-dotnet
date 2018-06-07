using System;
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Util;
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.Svg.Css.Impl;

namespace iText.Svg.Processors.Impl {
    /// <summary>Class that processes and add resolved css fonts to the FontProvider</summary>
    public class SvgFontProcessor {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Svg.Processors.Impl.SvgFontProcessor
            ));

        private ProcessorContext context;

        public SvgFontProcessor(ProcessorContext context) {
            this.context = context;
        }

        /// <summary>Adds @font-face fonts to the FontProvider.</summary>
        /// <param name="cssResolver">the css styles resolver</param>
        public virtual void AddFontFaceFonts(ICssResolver cssResolver) {
            //TODO Shall we add getFonts() to ICssResolver?
            if (cssResolver is DefaultSvgStyleResolver) {
                foreach (CssFontFaceRule fontFace in ((DefaultSvgStyleResolver)cssResolver).GetFonts()) {
                    bool findSupportedSrc = false;
                    FontFace ff = FontFace.Create(fontFace.GetProperties());
                    if (ff != null) {
                        foreach (FontFace.FontFaceSrc src in ff.GetSources()) {
                            if (CreateFont(ff.GetFontFamily(), src)) {
                                findSupportedSrc = true;
                                break;
                            }
                        }
                    }
                    if (!findSupportedSrc) {
                        LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_FONT, fontFace
                            ));
                    }
                }
            }
        }

        /// <summary>Creates a font and adds it to the context.</summary>
        /// <param name="fontFamily">the font family</param>
        /// <param name="src">the source of the font</param>
        /// <returns>true, if successful</returns>
        private bool CreateFont(String fontFamily, FontFace.FontFaceSrc src) {
            if (!SupportedFontFormat(src.format)) {
                return false;
            }
            else {
                if (src.isLocal) {
                    // to method with lazy initialization
                    ICollection<FontInfo> fonts = context.GetFontProvider().GetFontSet().Get(src.src);
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
                        byte[] bytes = context.GetResourceResolver().RetrieveStream(src.src);
                        if (bytes != null) {
                            FontProgram fp = FontProgramFactory.CreateFont(bytes, false);
                            context.AddTemporaryFont(fp, PdfEncodings.IDENTITY_H, fontFamily);
                            return true;
                        }
                    }
                    catch (Exception) {
                    }
                    return false;
                }
            }
        }

        /// <summary>Checks whether in general we support requested font format.</summary>
        /// <param name="format">
        /// 
        /// <see cref="FontFormat"/>
        /// </param>
        /// <returns>true, if supported or unrecognized.</returns>
        private bool SupportedFontFormat(FontFace.FontFormat format) {
            switch (format) {
                case FontFace.FontFormat.None:
                case FontFace.FontFormat.TrueType:
                case FontFace.FontFormat.OpenType:
                case FontFace.FontFormat.WOFF:
                case FontFace.FontFormat.WOFF2: {
                    return true;
                }

                default: {
                    return false;
                }
            }
        }
    }
}
