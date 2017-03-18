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
    /// Add, remove and search fonts.
    /// </remarks>
    /// <seealso cref="FontProvider"/>
    public class FontSet {
        private readonly LinkedList<FontInfo> fonts = new LinkedList<FontInfo>();

        private readonly IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private readonly IDictionary<FontSelectorKey, FontSelector> fontSelectorCache = new Dictionary<FontSelectorKey
            , FontSelector>();

        private readonly FontSet.FontNameSet fontNames = new FontSet.FontNameSet();

        /// <summary>Add all the fonts in a directory and possibly its subdirectories.</summary>
        /// <param name="dir">path to directory.</param>
        /// <param name="scanSubdirectories">
        /// recursively scan subdirectories if
        /// <see langword="true"/>
        /// .
        /// </param>
        /// <returns>number of added fonts.</returns>
        public virtual int AddDirectory(String dir, bool scanSubdirectories) {
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
                        if (FileUtil.FileExists(pfb) && Add(file, null) != null) {
                            count++;
                        }
                    }
                    else {
                        if ((".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) && Add(file, null) != null) {
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
        public virtual int AddDirectory(String dir) {
            return AddDirectory(dir, false);
        }

        /// <summary>
        /// Clone existing fontInfo with alias and add to the
        /// <see cref="FontSet"/>
        /// .
        /// Note, font selector will match either original font names and alias.
        /// </summary>
        /// <param name="fontInfo">
        /// already created
        /// <see cref="FontInfo"/>
        /// .
        /// </param>
        /// <param name="alias">font alias, shall not be null.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public virtual FontInfo Add(FontInfo fontInfo, String alias) {
            if (alias == null) {
                return null;
            }
            FontInfo newFontInfo = FontInfo.Create(fontInfo, alias);
            return Add(newFontInfo);
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
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
        public virtual FontInfo Add(FontProgram fontProgram, String encoding) {
            if (fontProgram == null) {
                return null;
            }
            FontInfo fontInfo = Add(FontInfo.Create(fontProgram, encoding, null));
            fontPrograms.Put(fontInfo, fontProgram);
            return fontInfo;
        }

        /// <summary>Add not supported for auto creating FontPrograms.</summary>
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
        public virtual FontInfo Add(FontProgram fontProgram, String encoding, String alias) {
            if (fontProgram == null) {
                return null;
            }
            FontInfo fontInfo = Add(FontInfo.Create(fontProgram, encoding, alias));
            fontPrograms.Put(fontInfo, fontProgram);
            return fontInfo;
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
        /// </summary>
        /// <param name="fontProgram">path to font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public virtual FontInfo Add(String fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding));
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
        /// </summary>
        /// <param name="fontProgram">font data.</param>
        /// <param name="encoding">preferred font encoding.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public virtual FontInfo Add(byte[] fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding));
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
        /// </summary>
        /// <param name="fontProgram">path to font data.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public virtual FontInfo Add(String fontProgram) {
            return Add(fontProgram, null);
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
        /// </summary>
        /// <param name="fontProgram">font data.</param>
        /// <returns>
        /// just created
        /// <see cref="FontInfo"/>
        /// on success, otherwise null.
        /// </returns>
        public virtual FontInfo Add(byte[] fontProgram) {
            return Add(FontInfo.Create(fontProgram, null));
        }

        /// <summary>
        /// Removes pre saved
        /// <see cref="FontInfo"/>
        /// .
        /// </summary>
        /// <param name="fontInfo">
        /// 
        /// <see cref="FontInfo"/>
        /// from group of
        /// <c>#add()</c>
        /// methods.
        /// </param>
        /// <returns>true, if font was found and successfully removed.</returns>
        public virtual bool Remove(FontInfo fontInfo) {
            if (fonts.Contains(fontInfo) || fontPrograms.ContainsKey(fontInfo)) {
                fonts.Remove(fontInfo);
                fontPrograms.JRemove(fontInfo);
                fontNames.Remove(fontInfo);
                fontSelectorCache.Clear();
                return true;
            }
            return false;
        }

        /// <summary>Search in existed fonts for PostScript name or full font name.</summary>
        /// <param name="fontName">PostScript or full name.</param>
        /// <returns>
        /// true, if
        /// <see cref="FontSet"/>
        /// contains font with given name.
        /// </returns>
        public virtual bool Contains(String fontName) {
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
        public virtual FontInfo Get(String fontName) {
            return fontNames.Get(fontName);
        }

        /// <summary>Set of available fonts.</summary>
        /// <remarks>
        /// Set of available fonts.
        /// Note, the set is unmodifiable.
        /// </remarks>
        public virtual ICollection<FontInfo> GetFonts() {
            return JavaCollectionsUtil.UnmodifiableCollection<FontInfo>(fonts);
        }

        //region Deprecated addFont methods
        /// <summary>Add not supported for auto creating FontPrograms.</summary>
        /// <param name="fontProgram">
        /// instance of
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// .
        /// </param>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </param>
        /// <returns>false, if fontProgram is null, otherwise true.</returns>
        [System.ObsoleteAttribute(@"use Add(iText.IO.Font.FontProgram, System.String) instead.")]
        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            return Add(fontProgram, encoding) != null;
        }

        [System.ObsoleteAttribute(@"use Add(System.String, System.String) instead.")]
        public virtual bool AddFont(String fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding)) != null;
        }

        [System.ObsoleteAttribute(@"use Add(byte[], System.String) instead.")]
        public virtual bool AddFont(byte[] fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding)) != null;
        }

        [System.ObsoleteAttribute(@"use Add(System.String) instead.")]
        public virtual bool AddFont(String fontProgram) {
            return Add(fontProgram) != null;
        }

        [System.ObsoleteAttribute(@"use Add(byte[]) instead.")]
        public virtual bool AddFont(byte[] fontProgram) {
            return Add(fontProgram) != null;
        }

        //endregion
        //region Internal members
        internal virtual IDictionary<FontInfo, FontProgram> GetFontPrograms() {
            return fontPrograms;
        }

        internal virtual IDictionary<FontSelectorKey, FontSelector> GetFontSelectorCache() {
            return fontSelectorCache;
        }

        private FontInfo Add(FontInfo fontInfo) {
            if (fontInfo != null) {
                fonts.AddLast(fontInfo);
                fontSelectorCache.Clear();
                fontNames.Add(fontInfo);
            }
            return fontInfo;
        }

        /// <summary>
        /// FontNameSet used for quick search of lowercased fontName or fullName,
        /// supports remove FontInfo at FontSet level.
        /// </summary>
        /// <remarks>
        /// FontNameSet used for quick search of lowercased fontName or fullName,
        /// supports remove FontInfo at FontSet level.
        /// FontInfoName has tricky implementation. Hashcode builds by fontName String,
        /// but equals() works in different ways, depends whether FontInfoName used for search (no FontInfo)
        /// or for adding/removing (contains FontInfo).
        /// </remarks>
        private class FontNameSet {
            private readonly IDictionary<FontSet.FontInfoName, FontInfo> fontInfoNames = new Dictionary<FontSet.FontInfoName
                , FontInfo>();

            //endregion
            //region Set for quick search of font names
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

            internal virtual void Remove(FontInfo fontInfo) {
                fontInfoNames.JRemove(new FontSet.FontInfoName(fontInfo.GetDescriptor().GetFontNameLowerCase(), fontInfo));
                fontInfoNames.JRemove(new FontSet.FontInfoName(fontInfo.GetDescriptor().GetFullNameLowerCase(), fontInfo));
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
