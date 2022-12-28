/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Util;

namespace iText.StyledXmlParser.Css.Font {
    /// <summary>
    /// Class that will examine the font as described in the CSS, and store it
    /// in a form that the font provider will understand.
    /// </summary>
    public class CssFontFace {
        /// <summary>Name that will be used as the alias of the font.</summary>
        private readonly String alias;

        /// <summary>A list of font face sources.</summary>
        private readonly IList<CssFontFace.CssFontFaceSrc> sources;

        /// <summary>
        /// Create a
        /// <see cref="CssFontFace"/>
        /// instance from a list of
        /// CSS font attributes ("font-family" or "src").
        /// </summary>
        /// <param name="properties">the font properties</param>
        /// <returns>
        /// the
        /// <see cref="CssFontFace"/>
        /// instance
        /// </returns>
        public static iText.StyledXmlParser.Css.Font.CssFontFace Create(IList<CssDeclaration> properties) {
            String fontFamily = null;
            String srcs = null;
            foreach (CssDeclaration descriptor in properties) {
                if ("font-family".Equals(descriptor.GetProperty())) {
                    // TODO DEVSIX-2534
                    fontFamily = FontFamilySplitterUtil.RemoveQuotes(descriptor.GetExpression());
                }
                else {
                    if ("src".Equals(descriptor.GetProperty())) {
                        srcs = descriptor.GetExpression();
                    }
                }
            }
            if (fontFamily == null || srcs == null) {
                // 'font-family' and 'src' is required according to spec:
                // https://www.w3.org/TR/2013/CR-css-fonts-3-20131003/#descdef-font-family\
                // https://www.w3.org/TR/2013/CR-css-fonts-3-20131003/#descdef-src
                return null;
            }
            IList<CssFontFace.CssFontFaceSrc> sources = new List<CssFontFace.CssFontFaceSrc>();
            // ttc collection are supported via url(Arial.ttc#1), url(Arial.ttc#2), etc.
            foreach (String src in SplitSourcesSequence(srcs)) {
                //local|url("ideal-sans-serif.woff")( format("woff"))?
                CssFontFace.CssFontFaceSrc source = CssFontFace.CssFontFaceSrc.Create(src.Trim());
                if (source != null) {
                    sources.Add(source);
                }
            }
            if (sources.Count > 0) {
                return new iText.StyledXmlParser.Css.Font.CssFontFace(fontFamily, sources);
            }
            else {
                return null;
            }
        }

        // NOTE: If src property is written in incorrect format
        // (for example, contains token url(<url_content>)<some_nonsense>),
        // then browser ignores it altogether and doesn't load font at all, even if there are valid tokens.
        // iText will still process all split tokens and can possibly load this font in case it contains some correct urls.
        /// <summary>Processes and splits a string sequence containing a url/uri.</summary>
        /// <param name="src">a string representing css src attribute</param>
        /// <returns>
        /// an array of
        /// <see cref="System.String"/>
        /// urls for font loading
        /// </returns>
        public static String[] SplitSourcesSequence(String src) {
            IList<String> list = new List<String>();
            int indexToStart = 0;
            while (indexToStart < src.Length) {
                int indexToCut;
                int indexUnescapedOpeningQuoteMark = Math.Min(CssUtils.FindNextUnescapedChar(src, '\'', indexToStart) >= 0
                     ? CssUtils.FindNextUnescapedChar(src, '\'', indexToStart) : int.MaxValue, CssUtils.FindNextUnescapedChar
                    (src, '"', indexToStart) >= 0 ? CssUtils.FindNextUnescapedChar(src, '"', indexToStart) : int.MaxValue);
                int indexUnescapedBracket = CssUtils.FindNextUnescapedChar(src, ')', indexToStart);
                if (indexUnescapedOpeningQuoteMark < indexUnescapedBracket) {
                    indexToCut = CssUtils.FindNextUnescapedChar(src, src[indexUnescapedOpeningQuoteMark], indexUnescapedOpeningQuoteMark
                         + 1);
                    if (indexToCut == -1) {
                        indexToCut = src.Length;
                    }
                }
                else {
                    indexToCut = indexUnescapedBracket;
                }
                while (indexToCut < src.Length && src[indexToCut] != ',') {
                    indexToCut++;
                }
                list.Add(src.JSubstring(indexToStart, indexToCut).Trim());
                indexToStart = ++indexToCut;
            }
            String[] result = new String[list.Count];
            list.ToArray(result);
            return result;
        }

        /// <summary>Checks whether in general we support requested font format.</summary>
        /// <param name="format">
        /// 
        /// <see cref="FontFormat"/>
        /// </param>
        /// <returns>true, if supported or unrecognized.</returns>
        public static bool IsSupportedFontFormat(CssFontFace.FontFormat format) {
            switch (format) {
                case CssFontFace.FontFormat.None:
                case CssFontFace.FontFormat.TrueType:
                case CssFontFace.FontFormat.OpenType:
                case CssFontFace.FontFormat.WOFF:
                case CssFontFace.FontFormat.WOFF2: {
                    return true;
                }

                default: {
                    return false;
                }
            }
        }

        /// <summary>Gets the font-family.</summary>
        /// <remarks>
        /// Gets the font-family.
        /// Actually font-family is an alias.
        /// </remarks>
        /// <returns>the font family (or alias)</returns>
        public virtual String GetFontFamily() {
            return alias;
        }

        /// <summary>Gets the font face sources.</summary>
        /// <returns>the sources</returns>
        public virtual IList<CssFontFace.CssFontFaceSrc> GetSources() {
            return new List<CssFontFace.CssFontFaceSrc>(sources);
        }

        /// <summary>Instantiates a new font face.</summary>
        /// <param name="alias">the font-family (or alias)</param>
        /// <param name="sources">the sources</param>
        private CssFontFace(String alias, IList<CssFontFace.CssFontFaceSrc> sources) {
            this.alias = alias;
            this.sources = new List<CssFontFace.CssFontFaceSrc>(sources);
        }

        /// <summary>The Enum FontFormat.</summary>
        public enum FontFormat {
            None,
            /// <summary>"truetype"</summary>
            TrueType,
            /// <summary>"opentype"</summary>
            OpenType,
            /// <summary>"woff"</summary>
            WOFF,
            /// <summary>"woff2"</summary>
            WOFF2,
            /// <summary>"embedded-opentype"</summary>
            EOT,
            /// <summary>"svg"</summary>
            SVG
        }

        /// <summary>Class that defines a font face source.</summary>
        public class CssFontFaceSrc {
            /// <summary>The UrlPattern used to compose a source path.</summary>
            public static readonly Regex UrlPattern = iText.Commons.Utils.StringUtil.RegexCompile("^((local)|(url))\\(((\'[^\']*\')|(\"[^\"]*\")|([^\'\"\\)]*))\\)( format\\(((\'[^\']*\')|(\"[^\"]*\")|([^\'\"\\)]*))\\))?$"
                );

            /// <summary>The Constant TypeGroup.</summary>
            public const int TypeGroup = 1;

            /// <summary>The Constant UrlGroup.</summary>
            public const int UrlGroup = 4;

            /// <summary>The Constant FormatGroup.</summary>
            public const int FormatGroup = 9;

            /// <summary>The font format.</summary>
            internal readonly CssFontFace.FontFormat format;

            /// <summary>The source path.</summary>
            internal readonly String src;

            /// <summary>Indicates if the font is local.</summary>
            internal readonly bool isLocal;

            public virtual CssFontFace.FontFormat GetFormat() {
                return format;
            }

            public virtual String GetSrc() {
                return src;
            }

            public virtual bool IsLocal() {
                return isLocal;
            }

            /* (non-Javadoc)
            * @see java.lang.Object#toString()
            */
            public override String ToString() {
                return MessageFormatUtil.Format("{0}({1}){2}", isLocal ? "local" : "url", src, format != CssFontFace.FontFormat
                    .None ? MessageFormatUtil.Format(" format({0})", format) : "");
            }

            /// <summary>
            /// Creates a
            /// <see cref="CssFontFaceSrc"/>
            /// object by parsing a
            /// <see cref="System.String"/>
            /// trying to match patterns that reveal the font name, whether that font is local,
            /// and which format the font is in.
            /// </summary>
            /// <param name="src">a string containing information about a font</param>
            /// <returns>
            /// the font in the form of a
            /// <see cref="CssFontFaceSrc"/>
            /// object
            /// </returns>
            public static CssFontFace.CssFontFaceSrc Create(String src) {
                Matcher m = iText.Commons.Utils.Matcher.Match(UrlPattern, src);
                if (!m.Matches()) {
                    return null;
                }
                return new CssFontFace.CssFontFaceSrc(Unquote(m.Group(UrlGroup)), "local".Equals(m.Group(TypeGroup)), ParseFormat
                    (m.Group(FormatGroup)));
            }

            /// <summary>
            /// Parses a
            /// <see cref="System.String"/>
            /// to a font format.
            /// </summary>
            /// <param name="formatStr">a string</param>
            /// <returns>a font format</returns>
            public static CssFontFace.FontFormat ParseFormat(String formatStr) {
                if (formatStr != null && formatStr.Length > 0) {
                    switch (Unquote(formatStr).ToLowerInvariant()) {
                        case "truetype": {
                            return CssFontFace.FontFormat.TrueType;
                        }

                        case "opentype": {
                            return CssFontFace.FontFormat.OpenType;
                        }

                        case "woff": {
                            return CssFontFace.FontFormat.WOFF;
                        }

                        case "woff2": {
                            return CssFontFace.FontFormat.WOFF2;
                        }

                        case "embedded-opentype": {
                            return CssFontFace.FontFormat.EOT;
                        }

                        case "svg": {
                            return CssFontFace.FontFormat.SVG;
                        }
                    }
                }
                return CssFontFace.FontFormat.None;
            }

            /// <summary>
            /// Removes single and double quotes at the start and the end of a
            /// <see cref="System.String"/>.
            /// </summary>
            /// <param name="quotedString">
            /// a
            /// <see cref="System.String"/>
            /// that might be between quotes
            /// </param>
            /// <returns>
            /// the
            /// <see cref="System.String"/>
            /// without the quotes
            /// </returns>
            public static String Unquote(String quotedString) {
                if (quotedString[0] == '\'' || quotedString[0] == '\"') {
                    return quotedString.JSubstring(1, quotedString.Length - 1);
                }
                return quotedString;
            }

            /// <summary>
            /// Instantiates a new
            /// <see cref="CssFontFaceSrc"/>
            /// instance.
            /// </summary>
            /// <param name="src">a source path</param>
            /// <param name="isLocal">indicates if the font is local</param>
            /// <param name="format">the font format (true type, open type, woff,...)</param>
            private CssFontFaceSrc(String src, bool isLocal, CssFontFace.FontFormat format) {
                this.format = format;
                this.src = src;
                this.isLocal = isLocal;
            }
        }
    }
}
