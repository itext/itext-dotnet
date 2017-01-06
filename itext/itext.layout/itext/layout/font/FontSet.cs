using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    public class FontSet {
        private static IDictionary<String, FontProgramInfo> fontInfoCache = new ConcurrentDictionary<String, FontProgramInfo
            >();

        private ICollection<FontProgramInfo> fonts = new HashSet<FontProgramInfo>();

        private IDictionary<FontProgramInfo, FontProgram> fontPrograms = new Dictionary<FontProgramInfo, FontProgram
            >();

        private IDictionary<FontSelectorKey, FontSelector> fontSelectorCache = new Dictionary<FontSelectorKey, FontSelector
            >();

        //"fontName+encoding" or "hash(fontProgram)+encoding" as key
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

        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            if (fontProgram == null) {
                return false;
            }
            FontProgramInfo fontInfo = FontProgramInfo.Create(fontProgram, encoding);
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

        public virtual void AddFont(String fontProgram) {
            AddFont(fontProgram, null);
        }

        public virtual void AddFont(FontProgram fontProgram) {
            AddFont(fontProgram, null);
        }

        public virtual void AddFont(byte[] fontProgram) {
            AddFont(fontProgram, null);
        }

        public virtual ICollection<FontProgramInfo> GetFonts() {
            return fonts;
        }

        public virtual IDictionary<FontProgramInfo, FontProgram> GetFontPrograms() {
            return fontPrograms;
        }

        protected internal virtual bool AddFont(String fontName, byte[] fontProgram, String encoding) {
            if (fontName == null && fontProgram == null) {
                return false;
            }
            String fontInfoKey = CalculateFontProgramInfoKey(fontName, fontProgram, encoding);
            FontProgramInfo fontInfo;
            if (fontInfoCache.ContainsKey(fontInfoKey)) {
                fontInfo = fontInfoCache.Get(fontInfoKey);
            }
            else {
                fontInfo = FontProgramInfo.Create(fontName, fontProgram, encoding);
                if (fontInfo != null) {
                    fontInfoCache[fontInfoKey] = fontInfo;
                }
                else {
                    return false;
                }
            }
            AddFontInfo(fontInfo);
            return true;
        }

        internal virtual IDictionary<FontSelectorKey, FontSelector> GetFontSelectorCache() {
            return fontSelectorCache;
        }

        private String CalculateFontProgramInfoKey(String fontName, byte[] fontProgram, String encoding) {
            String key;
            if (fontName != null) {
                key = fontName;
            }
            else {
                key = iText.IO.Util.JavaUtil.IntegerToHexString(ArrayUtil.HashCode(fontProgram));
            }
            return key + encoding;
        }

        private void AddFontInfo(FontProgramInfo fontInfo) {
            fonts.Add(fontInfo);
            fontSelectorCache.Clear();
        }
    }
}
