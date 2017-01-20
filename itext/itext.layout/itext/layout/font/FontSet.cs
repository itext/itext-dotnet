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
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Reusable font set for FontProgram related data.</summary>
    /// <seealso cref="FontProvider"/>
    public class FontSet {
        private ICollection<FontInfo> fonts = new LinkedHashSet<FontInfo>();

        private IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private IDictionary<FontSelectorKey, FontSelector> fontSelectorCache = new Dictionary<FontSelectorKey, FontSelector
            >();

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
                        if (FileUtil.FileExists(pfb)) {
                            AddFont(file, null);
                            count++;
                        }
                    }
                    else {
                        if (".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) {
                            AddFont(file, null);
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
        /// <param name="fontProgram"/>
        /// <param name="encoding">
        /// FontEncoding for creating
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </param>
        /// <returns>false, if fontProgram is null, otherwise true.</returns>
        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            if (fontProgram == null) {
                return false;
            }
            FontInfo fontInfo = FontInfo.Create(fontProgram, encoding);
            AddFontInfo(fontInfo);
            fontPrograms[fontInfo] = fontProgram;
            return true;
        }

        public virtual bool AddFont(String fontProgram, String encoding) {
            return AddFont(fontProgram, null, encoding);
        }

        public virtual bool AddFont(byte[] fontProgram, String encoding) {
            return AddFont(null, fontProgram, encoding);
        }

        public virtual bool AddFont(String fontProgram) {
            return AddFont(fontProgram, null);
        }

        public virtual bool AddFont(byte[] fontProgram) {
            return AddFont(fontProgram, null);
        }

        public virtual ICollection<FontInfo> GetFonts() {
            return fonts;
        }

        internal virtual bool AddFont(String fontName, byte[] fontProgram, String encoding) {
            if (fontName != null) {
                return AddFontInfo(FontInfo.Create(fontName, encoding));
            }
            else {
                if (fontProgram != null) {
                    return AddFontInfo(FontInfo.Create(fontProgram, encoding));
                }
                else {
                    return false;
                }
            }
        }

        internal virtual IDictionary<FontInfo, FontProgram> GetFontPrograms() {
            return fontPrograms;
        }

        internal virtual IDictionary<FontSelectorKey, FontSelector> GetFontSelectorCache() {
            return fontSelectorCache;
        }

        private bool AddFontInfo(FontInfo fontInfo) {
            if (fontInfo != null) {
                fonts.Add(fontInfo);
                fontSelectorCache.Clear();
                return true;
            }
            else {
                return false;
            }
        }
    }
}
