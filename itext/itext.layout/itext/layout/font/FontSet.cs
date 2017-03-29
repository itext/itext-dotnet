/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Reusable font set for FontProgram related data.</summary>
    /// <remarks>
    /// Reusable font set for FontProgram related data.
    /// Add and search fonts.
    /// <p/>
    /// A FontSet instance could be shared for multiple threads.
    /// However FontSet filling is not thread safe operation.
    /// </remarks>
    /// <seealso cref="FontProvider"/>
    public sealed class FontSet {
        private static AtomicLong lastId = new AtomicLong();

        private readonly ICollection<FontInfo> fonts = new LinkedHashSet<FontInfo>();

        private readonly IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private readonly FontSet.FontNameSet fontNames = new FontSet.FontNameSet();

        private long id;

        public FontSet() {
            // FontSet MUST be final to avoid overriding #add(FontInfo) method or remove functionality.
            // Due to new logic HashSet can be used instead of List.
            // But FontInfo with or without alias will be the same FontInfo.
            this.id = IncrementId();
        }

        /// <summary>Add all the fonts in a directory and possibly its subdirectories.</summary>
        /// <param name="dir">path to directory.</param>
        /// <param name="scanSubdirectories">
        /// recursively scan subdirectories if
        /// <see langword="true"/>
        /// .
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
                        if (FileUtil.FileExists(pfb) && Add(file, null, null) != null) {
                            count++;
                        }
                    }
                    else {
                        if ((".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) && Add(file, null, null) != 
                            null) {
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
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </remarks>
        /// <param name="fontProgram">
        /// 
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </param>
        /// <param name="alias">font alias.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public FontInfo Add(FontProgram fontProgram, String encoding, String alias) {
            if (fontProgram == null) {
                return null;
            }
            FontInfo fontInfo = Add(FontInfo.Create(fontProgram, encoding, alias));
            fontPrograms.Put(fontInfo, fontProgram);
            return fontInfo;
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
        /// <remarks>
        /// Add not supported for auto creating FontPrograms.
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </remarks>
        /// <param name="fontProgram">
        /// 
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public FontInfo Add(FontProgram fontProgram, String encoding) {
            return Add(fontProgram, encoding, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>
        /// .
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontPath">path to font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public FontInfo Add(String fontPath, String encoding, String alias) {
            return Add(FontInfo.Create(fontPath, encoding, alias));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>
        /// .
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontData">font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public FontInfo Add(byte[] fontData, String encoding, String alias) {
            return Add(FontInfo.Create(fontData, encoding, alias));
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>
        /// .
        /// <see cref="FontProvider.GetDefaultEncoding(iText.IO.Font.FontProgram)"/>
        /// will be used to determine encoding.
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontPath">path to font data.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public FontInfo Add(String fontPath) {
            return Add(fontPath, null, null);
        }

        /// <summary>
        /// Creates
        /// <see cref="FontInfo"/>
        /// , fetches
        /// <see cref="iText.IO.Font.FontProgramDescriptor"/>
        /// and adds just created
        /// <see cref="FontInfo"/>
        /// to
        /// <see cref="FontSet"/>
        /// .
        /// <see cref="FontProvider.GetDefaultEncoding(iText.IO.Font.FontProgram)"/>
        /// will be used to determine encoding.
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontData">font data.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public FontInfo Add(byte[] fontData) {
            return Add(fontData, null, null);
        }

        /// <summary>
        /// Adds
        /// <see cref="FontInfo"/>
        /// with alias. Could be used to fill temporary font set.
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontInfo">font info.</param>
        /// <param name="alias">font alias.</param>
        /// <returns>just created object on success or null, if equal FontInfo already exist.</returns>
        public FontInfo Add(FontInfo fontInfo, String alias) {
            return Add(FontInfo.Create(fontInfo, alias));
        }

        /// <summary>
        /// Adds
        /// <see cref="FontInfo"/>
        /// . Could be used to fill temporary font set.
        /// <p/>
        /// Note,
        /// <see cref="FontInfo.GetAlias()"/>
        /// do not taken into account in
        /// <see cref="FontInfo.Equals(System.Object)"/>
        /// .
        /// The same font with different alias will not be replaced.
        /// </summary>
        /// <param name="fontInfo">font info.</param>
        /// <returns>the same object on success or null, if equal FontInfo already exist.</returns>
        public FontInfo Add(FontInfo fontInfo) {
            // This method MUST be final, to avoid inconsistency with FontSelectorCache.
            // (Yes, FontSet is final. Double check.)
            if (fontInfo != null) {
                if (fonts.Contains(fontInfo)) {
                    // NOTE! We SHALL NOT replace font, because it will influence on FontSelectorCache.
                    // FontSelectorCache reset cache ONLY if number of fonts has been changed,
                    // while replacing will modify list of fonts without size change.
                    return null;
                }
                fonts.Add(fontInfo);
                fontNames.Add(fontInfo);
            }
            return fontInfo;
        }

        /// <summary>Search in existed fonts for PostScript name or full font name.</summary>
        /// <param name="fontName">PostScript or full name.</param>
        /// <returns>
        /// true, if
        /// <see cref="FontSet"/>
        /// contains font with given name.
        /// </returns>
        public bool Contains(String fontName) {
            return fontNames.Contains(fontName);
        }

        /// <summary>Search in existed fonts for PostScript name or full font name.</summary>
        /// <param name="fontName">PostScript or full name.</param>
        /// <returns>
        /// 
        /// <see cref="FontInfo"/>
        /// if
        /// <see cref="FontSet"/>
        /// contains font with given name, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public FontInfo Get(String fontName) {
            return fontNames.Get(fontName);
        }

        /// <summary>Gets available fonts.</summary>
        /// <remarks>
        /// Gets available fonts.
        /// Note, the collection is unmodifiable.
        /// </remarks>
        public ICollection<FontInfo> GetFonts() {
            return GetFonts(null);
        }

        /// <summary>Gets union of available and temporary fonts.</summary>
        /// <remarks>
        /// Gets union of available and temporary fonts.
        /// Note, the collection is unmodifiable.
        /// </remarks>
        public ICollection<FontInfo> GetFonts(iText.Layout.Font.FontSet tempFonts) {
            return new FontSetCollection(fonts, tempFonts != null ? tempFonts.fonts : null);
        }

        public bool IsEmpty() {
            return Size() == 0;
        }

        public int Size() {
            return fonts.Count;
        }

        //region Deprecated addFont methods
        [System.ObsoleteAttribute(@"use Add(iText.IO.Font.FontProgram, System.String) instead.")]
        public bool AddFont(FontProgram fontProgram, String encoding) {
            return Add(fontProgram, encoding) != null;
        }

        [System.ObsoleteAttribute(@"use Add(System.String, System.String, System.String) instead.")]
        public bool AddFont(String fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding, null)) != null;
        }

        [System.ObsoleteAttribute(@"use Add(byte[], System.String, System.String) instead.")]
        public bool AddFont(byte[] fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding, null)) != null;
        }

        [System.ObsoleteAttribute(@"use Add(System.String) instead.")]
        public bool AddFont(String fontProgram) {
            return Add(fontProgram) != null;
        }

        [System.ObsoleteAttribute(@"use Add(byte[]) instead.")]
        public bool AddFont(byte[] fontProgram) {
            return Add(fontProgram) != null;
        }

        //endregion
        //region Internal members
        internal long GetId() {
            return id;
        }

        internal FontProgram GetFontProgram(FontInfo fontInfo) {
            return fontPrograms.Get(fontInfo);
        }

        private long IncrementId() {
            return lastId.IncrementAndGet();
        }

        /// <summary>
        /// FontNameSet used for quick search of lowercased fontName or fullName,
        /// supports remove FontInfo at FontSet level.
        /// </summary>
        /// <remarks>
        /// FontNameSet used for quick search of lowercased fontName or fullName,
        /// supports remove FontInfo at FontSet level.
        /// <p>
        /// FontInfoName has tricky implementation. Hashcode builds by fontName String,
        /// but equals() works in different ways, depends whether FontInfoName used to search (no FontInfo)
        /// or to add (contains FontInfo).
        /// </remarks>
        private class FontNameSet {
            private readonly IDictionary<FontSet.FontInfoName, FontInfo> fontInfoNames = new Dictionary<FontSet.FontInfoName
                , FontInfo>();

            //endregion
            //region Set for font names quick search
            internal virtual bool Contains(String fontName) {
                return fontInfoNames.ContainsKey(new FontSet.FontInfoName(fontName.ToLowerInvariant()));
            }

            internal virtual FontInfo Get(String fontName) {
                return fontInfoNames.Get(new FontSet.FontInfoName(fontName.ToLowerInvariant()));
            }

            internal virtual void Add(FontInfo fontInfo) {
                fontInfoNames.Put(new FontSet.FontInfoName(fontInfo.GetDescriptor().GetFontNameLowerCase(), fontInfo), fontInfo
                    );
                fontInfoNames.Put(new FontSet.FontInfoName(fontInfo.GetDescriptor().GetFullNameLowerCase(), fontInfo), fontInfo
                    );
            }
        }

        private class FontInfoName {
            private readonly FontInfo fontInfo;

            private readonly String fontName;

            internal FontInfoName(String fontName, FontInfo fontInfo) {
                this.fontInfo = fontInfo;
                this.fontName = fontName;
            }

            internal FontInfoName(String fontName) {
                this.fontInfo = null;
                this.fontName = fontName;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                FontSet.FontInfoName that = (FontSet.FontInfoName)o;
                bool equalFontInfo = true;
                if (fontInfo != null && that.fontInfo != null) {
                    equalFontInfo = fontInfo.Equals(that.fontInfo);
                }
                return fontName.Equals(that.fontName) && equalFontInfo;
            }

            public override int GetHashCode() {
                return fontName.GetHashCode();
            }
        }
        //endregion
    }
}
