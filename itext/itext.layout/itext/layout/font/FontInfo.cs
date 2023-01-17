/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>
    /// Contains all font related data to create
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// and
    /// <see cref="iText.Kernel.Font.PdfFont"/>.
    /// </summary>
    /// <remarks>
    /// Contains all font related data to create
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// and
    /// <see cref="iText.Kernel.Font.PdfFont"/>.
    /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
    /// fetches with
    /// <see cref="iText.IO.Font.FontProgramDescriptorFactory"/>.
    /// </remarks>
    /// <seealso cref="FontProvider.GetPdfFont(FontInfo)"/>
    /// <seealso cref="FontProvider.GetPdfFont(FontInfo, FontSet)">
    /// Note,
    /// <see cref="GetAlias()"/>
    /// and
    /// <see cref="GetDescriptor()"/>
    /// are not taken into account in
    /// <see cref="Equals(System.Object)"/>
    /// ,
    /// the same font with different aliases will have equal FontInfo's,
    /// and therefore the same
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// in the end document.
    /// </seealso>
    public sealed class FontInfo {
        private static readonly IDictionary<FontCacheKey, FontProgramDescriptor> fontNamesCache = new ConcurrentDictionary
            <FontCacheKey, FontProgramDescriptor>();

        private readonly String fontName;

        private readonly byte[] fontData;

        private readonly FontProgramDescriptor descriptor;

        private readonly Range range;

        private readonly int hash;

        private readonly String encoding;

        private readonly String alias;

        private FontInfo(String fontName, byte[] fontData, String encoding, FontProgramDescriptor descriptor, Range
             unicodeRange, String alias) {
            this.fontName = fontName;
            this.fontData = fontData;
            this.encoding = encoding;
            this.descriptor = descriptor;
            this.range = unicodeRange != null ? unicodeRange : RangeBuilder.GetFullRange();
            this.alias = alias != null ? alias.ToLowerInvariant() : null;
            this.hash = CalculateHashCode(this.fontName, this.fontData, this.encoding, this.range);
        }

        public static iText.Layout.Font.FontInfo Create(iText.Layout.Font.FontInfo fontInfo, String alias, Range range
            ) {
            return new iText.Layout.Font.FontInfo(fontInfo.fontName, fontInfo.fontData, fontInfo.encoding, fontInfo.descriptor
                , range, alias);
        }

        public static iText.Layout.Font.FontInfo Create(iText.Layout.Font.FontInfo fontInfo, String alias) {
            return Create(fontInfo, alias, null);
        }

        public static iText.Layout.Font.FontInfo Create(FontProgram fontProgram, String encoding, String alias, Range
             range) {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(fontProgram);
            return new iText.Layout.Font.FontInfo(descriptor.GetFontName(), null, encoding, descriptor, range, alias);
        }

        public static iText.Layout.Font.FontInfo Create(FontProgram fontProgram, String encoding, String alias) {
            return Create(fontProgram, encoding, alias, null);
        }

        internal static iText.Layout.Font.FontInfo Create(String fontName, String encoding, String alias, Range range
            ) {
            FontCacheKey cacheKey = FontCacheKey.Create(fontName);
            FontProgramDescriptor descriptor = GetFontNamesFromCache(cacheKey);
            if (descriptor == null) {
                descriptor = FontProgramDescriptorFactory.FetchDescriptor(fontName);
                PutFontNamesToCache(cacheKey, descriptor);
            }
            return descriptor != null ? new iText.Layout.Font.FontInfo(fontName, null, encoding, descriptor, range, alias
                ) : null;
        }

        internal static iText.Layout.Font.FontInfo Create(byte[] fontProgram, String encoding, String alias, Range
             range) {
            FontCacheKey cacheKey = FontCacheKey.Create(fontProgram);
            FontProgramDescriptor descriptor = GetFontNamesFromCache(cacheKey);
            if (descriptor == null) {
                descriptor = FontProgramDescriptorFactory.FetchDescriptor(fontProgram);
                PutFontNamesToCache(cacheKey, descriptor);
            }
            return descriptor != null ? new iText.Layout.Font.FontInfo(null, fontProgram, encoding, descriptor, range, 
                alias) : null;
        }

        public FontProgramDescriptor GetDescriptor() {
            return descriptor;
        }

        //shall not be null
        public Range GetFontUnicodeRange() {
            return range;
        }

        /// <summary>
        /// Gets path to font, if
        /// <see cref="FontInfo"/>
        /// was created by String.
        /// </summary>
        /// <remarks>
        /// Gets path to font, if
        /// <see cref="FontInfo"/>
        /// was created by String.
        /// Note, to get PostScript or full name, use
        /// <see cref="GetDescriptor()"/>.
        /// </remarks>
        /// <returns>the font name</returns>
        public String GetFontName() {
            return fontName;
        }

        /// <summary>
        /// Gets font data, if
        /// <see cref="FontInfo"/>
        /// was created with
        /// <c>byte[]</c>.
        /// </summary>
        /// <returns>font data</returns>
        public byte[] GetFontData() {
            return fontData;
        }

        public String GetEncoding() {
            return encoding;
        }

        /// <summary>Gets font alias.</summary>
        /// <returns>alias if exist, otherwise null.</returns>
        public String GetAlias() {
            return alias;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (!(o is iText.Layout.Font.FontInfo)) {
                return false;
            }
            iText.Layout.Font.FontInfo that = (iText.Layout.Font.FontInfo)o;
            return (fontName != null ? fontName.Equals(that.fontName) : that.fontName == null) && range.Equals(that.range
                ) && JavaUtil.ArraysEquals(fontData, that.fontData) && (encoding != null ? encoding.Equals(that.encoding
                ) : that.encoding == null);
        }

        public override int GetHashCode() {
            return hash;
        }

        public override String ToString() {
            String name = descriptor.GetFontName();
            if (name.Length > 0) {
                if (encoding != null) {
                    return MessageFormatUtil.Format("{0}+{1}", name, encoding);
                }
                else {
                    return name;
                }
            }
            return base.ToString();
        }

        private static int CalculateHashCode(String fontName, byte[] bytes, String encoding, Range range) {
            int result = fontName != null ? fontName.GetHashCode() : 0;
            result = 31 * result + ArrayUtil.HashCode(bytes);
            result = 31 * result + (encoding != null ? encoding.GetHashCode() : 0);
            result = 31 * result + range.GetHashCode();
            return result;
        }

        private static FontProgramDescriptor GetFontNamesFromCache(FontCacheKey key) {
            return fontNamesCache.Get(key);
        }

        private static void PutFontNamesToCache(FontCacheKey key, FontProgramDescriptor descriptor) {
            if (descriptor != null) {
                fontNamesCache.Put(key, descriptor);
            }
        }
    }
}
