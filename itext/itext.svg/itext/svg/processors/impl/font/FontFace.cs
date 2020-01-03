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
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;

namespace iText.Svg.Processors.Impl.Font {
    /// <summary>
    /// Class that will examine the font as described in the CSS, and store it
    /// in a form that the font provider will understand.
    /// </summary>
    internal class FontFace {
        /// <summary>Name that will be used as the alias of the font.</summary>
        private readonly String alias;

        /// <summary>A list of font face sources.</summary>
        private readonly IList<FontFace.FontFaceSrc> sources;

        /// <summary>
        /// Create a
        /// <see cref="FontFace"/>
        /// instance from a list of
        /// CSS font attributes ("font-family" or "src").
        /// </summary>
        /// <param name="properties">the font properties</param>
        /// <returns>
        /// the
        /// <see cref="FontFace"/>
        /// instance
        /// </returns>
        public static iText.Svg.Processors.Impl.Font.FontFace Create(IList<CssDeclaration> properties) {
            String fontFamily = null;
            String srcs = null;
            foreach (CssDeclaration descriptor in properties) {
                if ("font-family".Equals(descriptor.GetProperty())) {
                    fontFamily = FontFamilySplitter.RemoveQuotes(descriptor.GetExpression());
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
            IList<FontFace.FontFaceSrc> sources = new List<FontFace.FontFaceSrc>();
            // ttc collection are supported via url(Arial.ttc#1), url(Arial.ttc#2), etc.
            foreach (String src in SplitSourcesSequence(srcs)) {
                //local|url("ideal-sans-serif.woff")( format("woff"))?
                FontFace.FontFaceSrc source = FontFace.FontFaceSrc.Create(src.Trim());
                if (source != null) {
                    sources.Add(source);
                }
            }
            if (sources.Count > 0) {
                return new iText.Svg.Processors.Impl.Font.FontFace(fontFamily, sources);
            }
            else {
                return null;
            }
        }

        // NOTE: If src property is written in incorrect format (for example, contains token url(<url_content>)<some_nonsense>),
        // then browser ignores it altogether and doesn't load font at all, even if there are valid tokens.
        // iText will still process all split tokens and can possibly load this font in case it contains some correct urls.
        /// <summary>Processes and splits a string sequence containing a url/uri</summary>
        /// <param name="src">a string representing css src attribute</param>
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
        public virtual IList<FontFace.FontFaceSrc> GetSources() {
            return new List<FontFace.FontFaceSrc>(sources);
        }

        /// <summary>Instantiates a new font face.</summary>
        /// <param name="alias">the font-family (or alias)</param>
        /// <param name="sources">the sources</param>
        private FontFace(String alias, IList<FontFace.FontFaceSrc> sources) {
            this.alias = alias;
            this.sources = new List<FontFace.FontFaceSrc>(sources);
        }

        //region Nested types
        /// <summary>Class that defines a font face source.</summary>
        internal class FontFaceSrc {
            /// <summary>The UrlPattern used to compose a source path.</summary>
            internal static readonly Regex UrlPattern = iText.IO.Util.StringUtil.RegexCompile("^((local)|(url))\\(((\'[^\']*\')|(\"[^\"]*\")|([^\'\"\\)]*))\\)( format\\(((\'[^\']*\')|(\"[^\"]*\")|([^\'\"\\)]*))\\))?$"
                );

            /// <summary>The Constant TypeGroup.</summary>
            internal const int TypeGroup = 1;

            /// <summary>The Constant UrlGroup.</summary>
            internal const int UrlGroup = 4;

            /// <summary>The Constant FormatGroup.</summary>
            internal const int FormatGroup = 9;

            /// <summary>The font format.</summary>
            internal readonly FontFace.FontFormat format;

            /// <summary>The source path.</summary>
            internal readonly String src;

            /// <summary>Indicates if the font is local.</summary>
            internal readonly bool isLocal;

            /* (non-Javadoc)
            * @see java.lang.Object#toString()
            */
            public override String ToString() {
                return MessageFormatUtil.Format("{0}({1}){2}", isLocal ? "local" : "url", src, format != FontFace.FontFormat
                    .None ? MessageFormatUtil.Format(" format({0})", format) : "");
            }

            /// <summary>
            /// Creates a
            /// <see cref="FontFace"/>
            /// object by parsing a
            /// <see cref="System.String"/>
            /// trying to match patterns that reveal the font name, whether that font is local,
            /// and which format the font is in.
            /// </summary>
            /// <param name="src">a string containing information about a font</param>
            /// <returns>
            /// the font in the form of a
            /// <see cref="FontFace"/>
            /// object
            /// </returns>
            internal static FontFace.FontFaceSrc Create(String src) {
                Match m = iText.IO.Util.StringUtil.Match(UrlPattern, src);
                if (!m.Success) {
                    return null;
                }
                return new FontFace.FontFaceSrc(Unquote(iText.IO.Util.StringUtil.Group(m, UrlGroup)), "local".Equals(iText.IO.Util.StringUtil.Group
                    (m, TypeGroup)), ParseFormat(iText.IO.Util.StringUtil.Group(m, FormatGroup)));
            }

            /// <summary>
            /// Parses a
            /// <see cref="System.String"/>
            /// to a font format.
            /// </summary>
            /// <param name="formatStr">a string</param>
            /// <returns>a font format</returns>
            internal static FontFace.FontFormat ParseFormat(String formatStr) {
                if (formatStr != null && formatStr.Length > 0) {
                    switch (Unquote(formatStr).ToLowerInvariant()) {
                        case "truetype": {
                            return FontFace.FontFormat.TrueType;
                        }

                        case "opentype": {
                            return FontFace.FontFormat.OpenType;
                        }

                        case "woff": {
                            return FontFace.FontFormat.WOFF;
                        }

                        case "woff2": {
                            return FontFace.FontFormat.WOFF2;
                        }

                        case "embedded-opentype": {
                            return FontFace.FontFormat.EOT;
                        }

                        case "svg": {
                            return FontFace.FontFormat.SVG;
                        }
                    }
                }
                return FontFace.FontFormat.None;
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
            internal static String Unquote(String quotedString) {
                if (quotedString[0] == '\'' || quotedString[0] == '\"') {
                    return quotedString.JSubstring(1, quotedString.Length - 1);
                }
                return quotedString;
            }

            /// <summary>
            /// Instantiates a new
            /// <see cref="FontFaceSrc"/>
            /// instance.
            /// </summary>
            /// <param name="src">a source path</param>
            /// <param name="isLocal">indicates if the font is local</param>
            /// <param name="format">the font format (true type, open type, woff,...)</param>
            private FontFaceSrc(String src, bool isLocal, FontFace.FontFormat format) {
                this.format = format;
                this.src = src;
                this.isLocal = isLocal;
            }
        }

        /// <summary>The Enum FontFormat.</summary>
        internal enum FontFormat {
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
        //endregion
    }
}
