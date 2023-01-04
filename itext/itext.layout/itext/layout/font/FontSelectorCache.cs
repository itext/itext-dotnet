/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
