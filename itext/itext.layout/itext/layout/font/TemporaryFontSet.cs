using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    public class TemporaryFontSet {
        private readonly FontSet mainFontSet;

        private int mainFontSetSize = -1;

        private readonly ICollection<FontInfo> fonts = new HashSet<FontInfo>();

        private readonly IDictionary<FontInfo, FontProgram> fontPrograms = new Dictionary<FontInfo, FontProgram>();

        private readonly IDictionary<FontSelectorKey, FontSelector> fontSelectorCache = new Dictionary<FontSelectorKey
            , FontSelector>();

        /// <summary>Create new instance.</summary>
        /// <param name="mainFontSet">
        /// base font set, can be
        /// <see langword="null"/>
        /// .
        /// </param>
        public TemporaryFontSet(FontSet mainFontSet) {
            // Due to new logic HashSet can be used instead of List.
            // But FontInfo with or without alias will be the same FontInfo.
            this.mainFontSet = mainFontSet;
            UpdateMainFontSetSize();
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
            else {
                return Add(FontInfo.Create(fontInfo, alias));
            }
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

        /// <summary>Set of available fonts.</summary>
        /// <remarks>
        /// Set of available fonts.
        /// Note, the set is unmodifiable.
        /// </remarks>
        public virtual ICollection<FontInfo> GetFonts() {
            //TODO create custom unmodifiable collection!
            ICollection<FontInfo> allFonts = new LinkedList<FontInfo>(mainFontSet.GetFonts());
            allFonts.AddAll(fonts);
            return JavaCollectionsUtil.UnmodifiableCollection<FontInfo>(allFonts);
        }

        internal virtual FontProgram GetFontProgram(FontInfo fontInfo) {
            FontProgram fontProgram = mainFontSet.GetFontProgram(fontInfo);
            if (fontProgram == null) {
                fontProgram = fontPrograms.Get(fontInfo);
            }
            return fontProgram;
        }

        internal virtual FontSelector GetCachedFontSelector(FontSelectorKey fontSelectorKey) {
            // FontSelector shall not be get from mainFontSet.
            if (UpdateMainFontSetSize()) {
                // Cache shall be cleared due to updated font collection.
                fontSelectorCache.Clear();
                return null;
            }
            else {
                return fontSelectorCache.Get(fontSelectorKey);
            }
        }

        internal virtual void PutCachedFontSelector(FontSelectorKey fontSelectorKey, FontSelector fontSelector) {
            fontSelectorCache.Put(fontSelectorKey, fontSelector);
        }

        private FontInfo Add(FontInfo fontInfo) {
            if (fontInfo != null) {
                fonts.Add(fontInfo);
                UpdateMainFontSetSize();
                fontSelectorCache.Clear();
            }
            return fontInfo;
        }

        private bool UpdateMainFontSetSize() {
            if (this.mainFontSet != null && mainFontSetSize != this.mainFontSet.GetFonts().Count) {
                mainFontSetSize = this.mainFontSet.GetFonts().Count;
                return true;
            }
            return false;
        }
    }
}
