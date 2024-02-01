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
using System.Collections.Generic;

namespace iText.Layout.Font {
    internal class FontSelectorCache {
        private readonly FontSelectorCache.FontSetSelectors defaultSelectors;

        private readonly FontSet defaultFontSet;

        private readonly IDictionary<long, FontSelectorCache.FontSetSelectors> caches = new Dictionary<long, FontSelectorCache.FontSetSelectors
            >();

        internal FontSelectorCache(FontSet defaultFontSet) {
            System.Diagnostics.Debug.Assert(defaultFontSet != null);
            this.defaultSelectors = new FontSelectorCache.FontSetSelectors();
            this.defaultSelectors.Update(defaultFontSet);
            this.defaultFontSet = defaultFontSet;
        }

        internal virtual FontSelector Get(FontSelectorKey key) {
            if (Update(null, null)) {
                return null;
            }
            else {
                return defaultSelectors.map.Get(key);
            }
        }

        internal virtual FontSelector Get(FontSelectorKey key, FontSet additionalFonts) {
            if (additionalFonts == null) {
                return Get(key);
            }
            else {
                FontSelectorCache.FontSetSelectors selectors = caches.Get(additionalFonts.GetId());
                if (selectors == null) {
                    caches.Put(additionalFonts.GetId(), selectors = new FontSelectorCache.FontSetSelectors());
                }
                if (Update(selectors, additionalFonts)) {
                    return null;
                }
                else {
                    return selectors.map.Get(key);
                }
            }
        }

        internal virtual void Put(FontSelectorKey key, FontSelector fontSelector) {
            //update defaultSelectors to reset counter before pushing if needed.
            Update(null, null);
            defaultSelectors.map.Put(key, fontSelector);
        }

        internal virtual void Put(FontSelectorKey key, FontSelector fontSelector, FontSet fontSet) {
            if (fontSet == null) {
                Put(key, fontSelector);
            }
            else {
                FontSelectorCache.FontSetSelectors selectors = caches.Get(fontSet.GetId());
                if (selectors == null) {
                    caches.Put(fontSet.GetId(), selectors = new FontSelectorCache.FontSetSelectors());
                }
                //update selectors and defaultSelectors to reset counter before pushing if needed.
                Update(selectors, fontSet);
                selectors.map.Put(key, fontSelector);
            }
        }

        private bool Update(FontSelectorCache.FontSetSelectors selectors, FontSet fontSet) {
            bool updated = false;
            if (defaultSelectors.Update(defaultFontSet)) {
                updated = true;
            }
            if (selectors != null && selectors.Update(fontSet)) {
                updated = true;
            }
            return updated;
        }

        private class FontSetSelectors {
            internal readonly IDictionary<FontSelectorKey, FontSelector> map = new Dictionary<FontSelectorKey, FontSelector
                >();

            private int fontSetSize = -1;

            internal virtual bool Update(FontSet fontSet) {
                System.Diagnostics.Debug.Assert(fontSet != null);
                if (fontSetSize == fontSet.Size()) {
                    return false;
                }
                else {
                    map.Clear();
                    fontSetSize = fontSet.Size();
                    return true;
                }
            }
        }
    }
}
