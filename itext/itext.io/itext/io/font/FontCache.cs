/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.IO.Font.Cmap;

namespace iText.IO.Font {
    public class FontCache {
        private static IDictionary<FontCacheKey, FontProgram> fontCache = new ConcurrentDictionary<FontCacheKey, FontProgram
            >();

        /// <summary>
        /// Checks if the font with the given name and encoding is one
        /// of the predefined CID fonts.
        /// </summary>
        /// <param name="fontName">the font name.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if it is CJKFont.
        /// </returns>
        [System.ObsoleteAttribute(@"in favour of CjkResourceLoader .")]
        protected internal static bool IsPredefinedCidFont(String fontName) {
            return CjkResourceLoader.IsPredefinedCidFont(fontName);
        }

        /// <summary>Finds a CJK font family which is compatible to the given CMap.</summary>
        /// <param name="cmap">a name of the CMap for which compatible font is searched.</param>
        /// <returns>a CJK font name if there's known compatible font for the given cmap name, or null otherwise.</returns>
        [System.ObsoleteAttribute(@"in favour of CjkResourceLoader .")]
        public static String GetCompatibleCidFont(String cmap) {
            return CjkResourceLoader.GetCompatibleCidFont(cmap);
        }

        /// <summary>
        /// Finds all CMap names that belong to the same registry to which a given
        /// font belongs.
        /// </summary>
        /// <param name="fontName">a name of the font for which CMap's are searched.</param>
        /// <returns>a set of CMap names corresponding to the given font.</returns>
        [System.ObsoleteAttribute(@"in favour of CjkResourceLoader .")]
        public static ICollection<String> GetCompatibleCmaps(String fontName) {
            return CjkResourceLoader.GetCompatibleCmaps(fontName);
        }

        [Obsolete]
        public static IDictionary<String, IDictionary<String, Object>> GetAllPredefinedCidFonts() {
            return CjkResourceLoader.GetAllPredefinedCidFonts();
        }

        [Obsolete]
        public static IDictionary<String, ICollection<String>> GetRegistryNames() {
            return CjkResourceLoader.GetRegistryNames();
        }

        /// <summary>Parses CMap with a given name producing it in a form of cid to unicode mapping.</summary>
        /// <param name="uniMap">a CMap name. It is expected that CMap identified by this name defines unicode to cid mapping.
        ///     </param>
        /// <returns>an object for convenient mapping from cid to unicode. If no CMap was found for provided name an exception is thrown.
        ///     </returns>
        [System.ObsoleteAttribute(@"in favour of CjkResourceLoader .")]
        public static CMapCidUni GetCid2UniCmap(String uniMap) {
            return CjkResourceLoader.GetCid2UniCmap(uniMap);
        }

        [Obsolete]
        public static CMapUniCid GetUni2CidCmap(String uniMap) {
            return CjkResourceLoader.GetUni2CidCmap(uniMap);
        }

        [Obsolete]
        public static CMapByteCid GetByte2CidCmap(String cmap) {
            return CjkResourceLoader.GetByte2CidCmap(cmap);
        }

        [Obsolete]
        public static CMapCidToCodepoint GetCidToCodepointCmap(String cmap) {
            return CjkResourceLoader.GetCidToCodepointCmap(cmap);
        }

        [Obsolete]
        public static CMapCodepointToCid GetCodepointToCidCmap(String uniMap) {
            return CjkResourceLoader.GetCodepointToCidCmap(uniMap);
        }

        /// <summary>
        /// Clears the cache by removing fonts that were added via
        /// <see cref="SaveFont(FontProgram, System.String)"/>.
        /// </summary>
        /// <remarks>
        /// Clears the cache by removing fonts that were added via
        /// <see cref="SaveFont(FontProgram, System.String)"/>.
        /// <para />
        /// Be aware that in multithreading environment this method call will affect the result of
        /// <see cref="GetFont(System.String)"/>.
        /// This in its turn affects creation of fonts via factories when
        /// <c>cached</c>
        /// argument is set to true (which is by default).
        /// </remarks>
        public static void ClearSavedFonts() {
            fontCache.Clear();
        }

        public static FontProgram GetFont(String fontName) {
            return fontCache.Get(FontCacheKey.Create(fontName));
        }

        internal static FontProgram GetFont(FontCacheKey key) {
            return fontCache.Get(key);
        }

        public static FontProgram SaveFont(FontProgram font, String fontName) {
            return SaveFont(font, FontCacheKey.Create(fontName));
        }

        internal static FontProgram SaveFont(FontProgram font, FontCacheKey key) {
            FontProgram fontFound = fontCache.Get(key);
            if (fontFound != null) {
                return fontFound;
            }
            fontCache.Put(key, font);
            return font;
        }
    }
}
