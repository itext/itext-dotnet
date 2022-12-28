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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    /// <summary>Reusable font set for FontProgram related data.</summary>
    /// <remarks>
    /// Reusable font set for FontProgram related data.
    /// Add and search fonts.
    /// <para />
    /// A FontSet instance could be shared for multiple threads.
    /// However FontSet filling is not thread safe operation.
    /// </remarks>
    /// <seealso cref="FontProvider"/>
    public sealed class FontSet {
        // FontSet MUST be final to avoid overriding #add(FontInfo) method or remove functionality.
        private static readonly AtomicLong lastId = new AtomicLong();

        // Due to new logic HashSet can be used instead of List.
        // But FontInfo with or without alias will be the same FontInfo.
        private readonly ICollection<FontInfo> fonts = new LinkedHashSet<FontInfo>();

        private readonly IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private readonly long id;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="FontSet"/>.
        /// </summary>
        public FontSet() {
            this.id = lastId.IncrementAndGet();
        }

        /// <summary>Add all the fonts in a directory and possibly its subdirectories.</summary>
        /// <param name="dir">path to directory.</param>
        /// <param name="scanSubdirectories">
        /// recursively scan subdirectories if
        /// <see langword="true"/>.
        /// </param>
        /// <returns>number of added fonts.</returns>
        public int AddDirectory(String dir, bool scanSubdirectories) {
            int count = 0;
            String[] files = FileUtil.ListFilesInDirectory(dir, scanSubdirectories);
            if (files == null) {
                return 0;
            }
            foreach (String file in files) {
                try {
                    String suffix = file.Length < 4 ? null : file.Substring(file.Length - 4).ToLowerInvariant();
                    if (".afm".Equals(suffix) || ".pfm".Equals(suffix)) {
                        // Add only Type 1 fonts with matching .pfb files.
                        String pfb = file.JSubstring(0, file.Length - 4) + ".pfb";
                        if (FileUtil.FileExists(pfb) && AddFont(file)) {
                            count++;
                        }
                    }
                    else {
                        if ((".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) && AddFont(file)) {
                            count++;
                        }
                    }
                }
                catch (Exception) {
                }
            }
            return count;
        }

        /// <summary>Add all the fonts in a directory.</summary>
        /// <param name="dir">path to directory.</param>
        /// <returns>number of added fonts.</returns>
        public int AddDirectory(String dir) {
            return AddDirectory(dir, false);
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
        /// <remarks>
        /// Add not supported for auto creating FontPrograms.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontProgram">
        /// 
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="alias">font alias.</param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontProgram fontProgram, String encoding, String alias, Range unicodeRange) {
            if (fontProgram == null) {
                return false;
            }
            if (fontProgram is Type3Font) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Font.FontSet));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.TYPE3_FONT_CANNOT_BE_ADDED);
                return false;
            }
            FontInfo fi = FontInfo.Create(fontProgram, encoding, alias, unicodeRange);
            if (AddFont(fi)) {
                fontPrograms.Put(fi, fontProgram);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
        /// <remarks>
        /// Add not supported for auto creating FontPrograms.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontProgram">
        /// 
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="alias">font alias.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontProgram fontProgram, String encoding, String alias) {
            return AddFont(fontProgram, encoding, alias, null);
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
        /// <param name="fontProgram">
        /// 
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontProgram fontProgram, String encoding) {
            return AddFont(fontProgram, encoding, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontPath">path to font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <param name="alias">font alias, will replace original font family.</param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(String fontPath, String encoding, String alias, Range unicodeRange) {
            return AddFont(FontInfo.Create(fontPath, encoding, alias, unicodeRange));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontPath">path to font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <param name="alias">font alias.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(String fontPath, String encoding, String alias) {
            return AddFont(fontPath, encoding, alias, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <param name="fontPath">path to font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(String fontPath, String encoding) {
            return AddFont(FontInfo.Create(fontPath, encoding, null, null));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontData">font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <param name="alias">font alias.</param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(byte[] fontData, String encoding, String alias, Range unicodeRange) {
            return AddFont(FontInfo.Create(fontData, encoding, alias, unicodeRange));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontData">font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <param name="alias">font alias.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(byte[] fontData, String encoding, String alias) {
            return AddFont(fontData, encoding, alias, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <param name="fontData">font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public bool AddFont(byte[] fontData, String encoding) {
            return AddFont(FontInfo.Create(fontData, encoding, null, null));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <see cref="FontProvider.GetDefaultEncoding(iText.IO.Font.FontProgram)"/>
        /// will be used to determine encoding.
        /// </remarks>
        /// <param name="fontPath">path to font data.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(String fontPath) {
            return AddFont(fontPath, null, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>.
        /// <see cref="FontProvider.GetDefaultEncoding(iText.IO.Font.FontProgram)"/>
        /// will be used to determine encoding.
        /// </remarks>
        /// <param name="fontData">font data.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(byte[] fontData) {
            return AddFont(fontData, null, null);
        }

        /// <summary>
        /// Adds
        /// <see cref="FontInfo"/>
        /// with alias.
        /// </summary>
        /// <remarks>
        /// Adds
        /// <see cref="FontInfo"/>
        /// with alias. Could be used to fill temporary font set.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontInfo">font info.</param>
        /// <param name="alias">font alias.</param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontInfo fontInfo, String alias, Range unicodeRange) {
            return AddFont(FontInfo.Create(fontInfo, alias, unicodeRange));
        }

        /// <summary>
        /// Adds
        /// <see cref="FontInfo"/>
        /// with alias.
        /// </summary>
        /// <remarks>
        /// Adds
        /// <see cref="FontInfo"/>
        /// with alias. Could be used to fill temporary font set.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// Alias will replace original font family in font selector algorithm.
        /// </remarks>
        /// <param name="fontInfo">font info.</param>
        /// <param name="alias">font alias.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontInfo fontInfo, String alias) {
            return AddFont(fontInfo, alias, null);
        }

        /// <summary>
        /// Adds
        /// <see cref="FontInfo"/>.
        /// </summary>
        /// <remarks>
        /// Adds
        /// <see cref="FontInfo"/>
        /// . Could be used to fill temporary font set.
        /// <para />
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>.
        /// The same font with different alias will not be replaced.
        /// </remarks>
        /// <param name="fontInfo">font info.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public bool AddFont(FontInfo fontInfo) {
            // This method MUST be final, to avoid inconsistency with FontSelectorCache.
            // (Yes, FontSet is final. Double check.)
            if (fontInfo != null && !fonts.Contains(fontInfo)) {
                // NOTE! We SHALL NOT replace font, because it will influence on FontSelectorCache.
                // FontSelectorCache reset cache ONLY if number of fonts has been changed,
                // while replacing will modify list of fonts without size change.
                fonts.Add(fontInfo);
                return true;
            }
            return false;
        }

        /// <summary>Search in existed fonts for PostScript name or full font name.</summary>
        /// <remarks>
        /// Search in existed fonts for PostScript name or full font name.
        /// <para />
        /// Note, this method has O(n) complexity.
        /// </remarks>
        /// <param name="fontName">PostScript or full name.</param>
        /// <returns>
        /// true, if
        /// <see cref="FontSet"/>
        /// contains font with given name.
        /// </returns>
        public bool Contains(String fontName) {
            if (fontName == null || fontName.Length == 0) {
                return false;
            }
            fontName = fontName.ToLowerInvariant();
            foreach (FontInfo fi in GetFonts()) {
                if (fontName.Equals(fi.GetDescriptor().GetFullNameLowerCase()) || fontName.Equals(fi.GetDescriptor().GetFontNameLowerCase
                    ())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Search in existed fonts for PostScript name or full font name.</summary>
        /// <remarks>
        /// Search in existed fonts for PostScript name or full font name.
        /// <para />
        /// Note, this method has O(n) complexity.
        /// </remarks>
        /// <param name="fontName">PostScript or full name.</param>
        /// <returns>
        /// Collection of
        /// <see cref="FontInfo"/>
        /// from set of fonts with given PostScript or full name.
        /// </returns>
        public ICollection<FontInfo> Get(String fontName) {
            if (fontName == null || fontName.Length == 0) {
                return JavaCollectionsUtil.EmptyList<FontInfo>();
            }
            fontName = fontName.ToLowerInvariant();
            IList<FontInfo> list = new List<FontInfo>();
            foreach (FontInfo fi in GetFonts()) {
                if (fontName.Equals(fi.GetDescriptor().GetFullNameLowerCase()) || fontName.Equals(fi.GetDescriptor().GetFontNameLowerCase
                    ())) {
                    list.Add(fi);
                }
            }
            return list;
        }

        /// <summary>Gets available fonts.</summary>
        /// <remarks>
        /// Gets available fonts.
        /// <para />
        /// Note, the collection is unmodifiable.
        /// </remarks>
        /// <returns>set of all available fonts</returns>
        public ICollection<FontInfo> GetFonts() {
            return GetFonts(null);
        }

        /// <summary>Gets union of available and temporary fonts.</summary>
        /// <remarks>
        /// Gets union of available and temporary fonts.
        /// <para />
        /// Note, the collection is unmodifiable.
        /// </remarks>
        /// <param name="additionalFonts">set of temporary fonts</param>
        /// <returns>set of all available and temporary fonts</returns>
        public ICollection<FontInfo> GetFonts(iText.Layout.Font.FontSet additionalFonts) {
            return new FontSetCollection(fonts, additionalFonts != null ? additionalFonts.fonts : null);
        }

        /// <summary>
        /// Returns
        /// <see langword="true"/>
        /// if this set contains no elements.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this set contains no elements
        /// </returns>
        public bool IsEmpty() {
            return Size() == 0;
        }

        /// <summary>Returns the number of elements in this set.</summary>
        /// <returns>the number of elements in this set</returns>
        public int Size() {
            return fonts.Count;
        }

        //region Internal members
        internal long GetId() {
            return id;
        }

        internal FontProgram GetFontProgram(FontInfo fontInfo) {
            return fontPrograms.Get(fontInfo);
        }
        //endregion
    }
}
