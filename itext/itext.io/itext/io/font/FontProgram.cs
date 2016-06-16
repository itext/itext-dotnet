/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.IO.Font.Otf;
using iText.IO.Util;

namespace iText.IO.Font {
    public abstract class FontProgram {
        public const int DEFAULT_WIDTH = 1000;

        public const int UNITS_NORMALIZATION = 1000;

        protected internal IDictionary<int, Glyph> codeToGlyph = new Dictionary<int, Glyph>();

        protected internal IDictionary<int, Glyph> unicodeToGlyph = new Dictionary<int, Glyph>();

        protected internal bool isFontSpecific;

        protected internal FontNames fontNames = new FontNames();

        protected internal FontMetrics fontMetrics = new FontMetrics();

        protected internal FontIdentification fontIdentification = new FontIdentification();

        protected internal int avgWidth;

        /// <summary>The font's encoding name.</summary>
        /// <remarks>
        /// The font's encoding name. This encoding is 'StandardEncoding' or 'AdobeStandardEncoding' for a font
        /// that can be totally encoded according to the characters names. For all other names the font is treated as symbolic.
        /// </remarks>
        protected internal String encodingScheme = FontEncoding.FONT_SPECIFIC;

        protected internal String registry;

        // In case Type1: char code to glyph.
        // In case TrueType: glyph index to glyph.
        public virtual int CountOfGlyphs() {
            return Math.Max(codeToGlyph.Count, unicodeToGlyph.Count);
        }

        public virtual FontNames GetFontNames() {
            return fontNames;
        }

        public virtual FontMetrics GetFontMetrics() {
            return fontMetrics;
        }

        public virtual FontIdentification GetFontIdentification() {
            return fontIdentification;
        }

        public virtual String GetRegistry() {
            return registry;
        }

        public abstract int GetPdfFontFlags();

        public virtual bool IsFontSpecific() {
            return isFontSpecific;
        }

        /// <summary>Get glyph's width.</summary>
        /// <param name="unicode">a unicode symbol or FontSpecif code.</param>
        /// <returns>Gets width in normalized 1000 units.</returns>
        public virtual int GetWidth(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            return glyph != null ? glyph.GetWidth() : 0;
        }

        public virtual int GetAvgWidth() {
            return avgWidth;
        }

        /// <summary>Get glyph's bbox.</summary>
        /// <param name="unicode">a unicode symbol or FontSpecif code.</param>
        /// <returns>Gets bbox in normalized 1000 units.</returns>
        public virtual int[] GetCharBBox(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            return glyph != null ? glyph.GetBbox() : null;
        }

        public virtual Glyph GetGlyph(int unicode) {
            return unicodeToGlyph.Get(unicode);
        }

        // char code in case Type1 or index in case OpenType
        public virtual Glyph GetGlyphByCode(int charCode) {
            return codeToGlyph.Get(charCode);
        }

        public virtual bool HasKernPairs() {
            return false;
        }

        /// <summary>Gets the kerning between two glyphs.</summary>
        /// <param name="first">the first unicode value</param>
        /// <param name="second">the second unicode value</param>
        /// <returns>the kerning to be applied</returns>
        public virtual int GetKerning(int first, int second) {
            return GetKerning(unicodeToGlyph.Get(first), unicodeToGlyph.Get(second));
        }

        /// <summary>Gets the kerning between two glyphs.</summary>
        /// <param name="first">the first glyph</param>
        /// <param name="second">the second glyph</param>
        /// <returns>the kerning to be applied</returns>
        public abstract int GetKerning(Glyph first, Glyph second);

        protected internal virtual void SetRegistry(String registry) {
            this.registry = registry;
        }

        /// <summary>Gets the name without the modifiers Bold, Italic or BoldItalic.</summary>
        /// <param name="name">the full name of the font</param>
        /// <returns>the name without the modifiers Bold, Italic or BoldItalic</returns>
        protected internal static String GetBaseName(String name) {
            if (name == null) {
                return null;
            }
            if (name.EndsWith(",Bold")) {
                return name.JSubstring(0, name.Length - 5);
            }
            else {
                if (name.EndsWith(",Italic")) {
                    return name.JSubstring(0, name.Length - 7);
                }
                else {
                    if (name.EndsWith(",BoldItalic")) {
                        return name.JSubstring(0, name.Length - 11);
                    }
                    else {
                        return name;
                    }
                }
            }
        }

        protected internal virtual void SetTypoAscender(int ascender) {
            fontMetrics.SetTypoAscender(ascender);
        }

        protected internal virtual void SetTypoDescender(int descender) {
            fontMetrics.SetTypoDescender(descender);
        }

        protected internal virtual void SetCapHeight(int capHeight) {
            fontMetrics.SetCapHeight(capHeight);
        }

        protected internal virtual void SetXHeight(int xHeight) {
            fontMetrics.SetXHeight(xHeight);
        }

        protected internal virtual void SetItalicAngle(int italicAngle) {
            fontMetrics.SetItalicAngle(italicAngle);
        }

        protected internal virtual void SetStemV(int stemV) {
            fontMetrics.SetStemV(stemV);
        }

        protected internal virtual void SetStemH(int stemH) {
            fontMetrics.SetStemH(stemH);
        }

        protected internal virtual void SetFontWeight(int fontWeight) {
            fontNames.SetFontWeight(fontWeight);
        }

        protected internal virtual void SetFontWidth(String fontWidth) {
            fontWidth = fontWidth.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            int fontWidthValue = FontNames.FWIDTH_NORMAL;
            switch (fontWidth) {
                case "ultracondensed": {
                    fontWidthValue = FontNames.FWIDTH_ULTRA_CONDENSED;
                    break;
                }

                case "extracondensed": {
                    fontWidthValue = FontNames.FWIDTH_EXTRA_CONDENSED;
                    break;
                }

                case "condensed": {
                    fontWidthValue = FontNames.FWIDTH_CONDENSED;
                    break;
                }

                case "semicondensed": {
                    fontWidthValue = FontNames.FWIDTH_SEMI_CONDENSED;
                    break;
                }

                case "normal": {
                    fontWidthValue = FontNames.FWIDTH_NORMAL;
                    break;
                }

                case "semiexpanded": {
                    fontWidthValue = FontNames.FWIDTH_SEMI_EXPANDED;
                    break;
                }

                case "expanded": {
                    fontWidthValue = FontNames.FWIDTH_EXPANDED;
                    break;
                }

                case "extraexpanded": {
                    fontWidthValue = FontNames.FWIDTH_EXTRA_EXPANDED;
                    break;
                }

                case "ultraexpanded": {
                    fontWidthValue = FontNames.FWIDTH_ULTRA_EXPANDED;
                    break;
                }
            }
            fontNames.SetFontWidth(fontWidthValue);
        }

        protected internal virtual void SetFixedPitch(bool isFixedPitch) {
            fontMetrics.SetIsFixedPitch(isFixedPitch);
        }

        protected internal virtual void SetBold(bool isBold) {
            if (isBold) {
                fontNames.SetMacStyle(fontNames.GetMacStyle() | FontNames.BOLD_FLAG);
            }
            else {
                fontNames.SetMacStyle(fontNames.GetMacStyle() & (~FontNames.BOLD_FLAG));
            }
        }

        protected internal virtual void SetBbox(int[] bbox) {
            fontMetrics.SetBbox(bbox[0], bbox[1], bbox[2], bbox[3]);
        }

        protected internal virtual void SetFontFamily(String fontFamily) {
            fontNames.SetFamilyName(fontFamily);
        }

        protected internal virtual void SetFontName(String psFontName) {
            fontNames.SetFontName(psFontName);
        }

        protected internal virtual void CheckFilePath(String path) {
            if (path != null && !FontConstants.BUILTIN_FONTS_14.Contains(path) && !FileUtil.FileExists(path)) {
                throw new iText.IO.IOException(iText.IO.IOException.FontFile1NotFound).SetMessageParams(path);
            }
        }

        protected internal virtual void FixSpaceIssue() {
            Glyph space = unicodeToGlyph.Get(32);
            if (space != null) {
                codeToGlyph[space.GetCode()] = space;
            }
        }
    }
}
