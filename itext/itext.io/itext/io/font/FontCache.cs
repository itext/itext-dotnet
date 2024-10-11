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

namespace iText.IO.Font {
    public class FontCache {
        private static readonly IDictionary<FontCacheKey, FontProgram> fontCache = new ConcurrentDictionary<FontCacheKey
            , FontProgram>();

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

//\cond DO_NOT_DOCUMENT
        internal static FontProgram GetFont(FontCacheKey key) {
            return fontCache.Get(key);
        }
//\endcond

        public static FontProgram SaveFont(FontProgram font, String fontName) {
            return SaveFont(font, FontCacheKey.Create(fontName));
        }

//\cond DO_NOT_DOCUMENT
        internal static FontProgram SaveFont(FontProgram font, FontCacheKey key) {
            FontProgram fontFound = fontCache.Get(key);
            if (fontFound != null) {
                return fontFound;
            }
            fontCache.Put(key, font);
            return font;
        }
//\endcond
    }
}
