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
    /// <seealso cref="FontProvider"/>
    public class FontSet {
        private readonly ICollection<FontInfo> fonts = new LinkedHashSet<FontInfo>();

        private readonly IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private readonly IDictionary<FontSelectorKey, FontSelector> fontSelectorCache = new Dictionary<FontSelectorKey
            , FontSelector>();

        private readonly FontSet.FontNameSet fontNames = new FontSet.FontNameSet();

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

        public virtual int AddDirectory(String dir) {
            return AddDirectory(dir, false);
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
        /// <returns>false, if fontProgram is null, otherwise true.</returns>
        public virtual FontInfo Add(FontProgram fontProgram, String encoding) {
            if (fontProgram == null) {
                return null;
            }
            FontInfo fontInfo = Add(FontInfo.Create(fontProgram, encoding));
            fontPrograms.Put(fontInfo, fontProgram);
            return fontInfo;
        }

        public virtual FontInfo Add(String fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding));
        }

        public virtual FontInfo Add(byte[] fontProgram, String encoding) {
            return Add(FontInfo.Create(fontProgram, encoding));
        }

        public virtual FontInfo Add(String fontProgram) {
            return Add(fontProgram, null);
        }

        public virtual FontInfo Add(byte[] fontProgram) {
            return Add(FontInfo.Create(fontProgram, null));
        }

        public virtual bool Remove(FontInfo fontInfo) {
            if (fonts.Contains(fontInfo) || fontPrograms.ContainsKey(fontInfo)) {
                fonts.Remove(fontInfo);
                fontPrograms.JRemove(fontInfo);
                fontNames.RemoveFontInfo(fontInfo);
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
            return fontNames.ContainsFont(fontName);
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
                fonts.Add(fontInfo);
                fontSelectorCache.Clear();
                fontNames.AddFontInfo(fontInfo);
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
        /// FontInfoNames has tricky implementation. Hashcode builds by fontName String,
        /// but equals() works in different ways, depends whether FontInfoNames used for search (no FontInfo)
        /// or for adding/removing (contains FontInfo).
        /// </remarks>
        private class FontNameSet {

            ISet<FontSet.FontInfoNames> set = new HashSet<FontInfoNames>();
            //endregion
            //region Set for quick search of font names
            internal virtual bool ContainsFont(String fontName) {
                return set.Contains(new FontSet.FontInfoNames(fontName.ToLowerInvariant()));
            }

            internal virtual bool AddFontInfo(FontInfo fontInfo) {
                bool fontName = set.Add(new FontSet.FontInfoNames(fontInfo.GetDescriptor().GetFontNameLowerCase(), fontInfo
                    ));
                bool fullName = set.Add(new FontSet.FontInfoNames(fontInfo.GetDescriptor().GetFullNameLowerCase(), fontInfo
                    ));
                return fontName || fullName;
            }

            internal virtual bool RemoveFontInfo(FontInfo fontInfo) {
                bool fontName = set.Remove(new FontSet.FontInfoNames(fontInfo.GetDescriptor().GetFontNameLowerCase(), fontInfo
                    ));
                bool fullName = set.Remove(new FontSet.FontInfoNames(fontInfo.GetDescriptor().GetFullNameLowerCase(), fontInfo
                    ));
                return fontName || fullName;
            }

            public bool Add(FontSet.FontInfoNames fontInfoNames) {
                throw new InvalidOperationException("Use #addFontInfo(FontInfo) instead.");
            }

            public bool Remove(Object o) {
                throw new InvalidOperationException("Use #removeFontInfo(FontInfo) instead.");
            }
        }

        private class FontInfoNames {
            private readonly FontInfo fontInfo;

            private readonly String fontName;

            internal FontInfoNames(String fontName, FontInfo fontInfo) {
                this.fontInfo = fontInfo;
                this.fontName = fontName;
            }

            internal FontInfoNames(String fontName) {
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
                FontSet.FontInfoNames that = (FontSet.FontInfoNames)o;
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
