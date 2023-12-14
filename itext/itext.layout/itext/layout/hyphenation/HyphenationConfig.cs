/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Layout.Hyphenation {
    /// <summary>This is the class used to configure hyphenation on layout level</summary>
    public class HyphenationConfig {
        /// <summary>The Hyphenator object.</summary>
        protected internal Hyphenator hyphenator;

        /// <summary>The hyphenation symbol used when hyphenating.</summary>
        protected internal char hyphenSymbol = '-';

        /// <summary>
        /// Constructs a new
        /// <see cref="HyphenationConfig"/>.
        /// </summary>
        /// <remarks>
        /// Constructs a new
        /// <see cref="HyphenationConfig"/>
        /// . No language hyphenation files will be used.
        /// Only soft hyphen symbols ('\u00ad') will be taken into account.
        /// </remarks>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        public HyphenationConfig(int leftMin, int rightMin) {
            this.hyphenator = new Hyphenator(null, null, leftMin, rightMin);
        }

        /// <summary>
        /// Constructs a new
        /// <see cref="HyphenationConfig"/>
        /// by a
        /// <see cref="Hyphenator"/>
        /// which will be used to
        /// find hyphenation points.
        /// </summary>
        /// <param name="hyphenator">
        /// the
        /// <see cref="Hyphenator"/>
        /// instance
        /// </param>
        public HyphenationConfig(Hyphenator hyphenator) {
            this.hyphenator = hyphenator;
        }

        /// <summary>
        /// Constructs a new
        /// <see cref="HyphenationConfig"/>
        /// instance.
        /// </summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the optional country code (may be null or "none")</param>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        public HyphenationConfig(String lang, String country, int leftMin, int rightMin) {
            this.hyphenator = new Hyphenator(lang, country, leftMin, rightMin);
        }

        /// <summary>Hyphenates a given word.</summary>
        /// <returns>
        /// 
        /// <see cref="Hyphenation"/>
        /// object representing possible hyphenation points
        /// or
        /// <see langword="null"/>
        /// if no hyphenation points are found.
        /// </returns>
        /// <param name="word">Tee word to hyphenate</param>
        public virtual iText.Layout.Hyphenation.Hyphenation Hyphenate(String word) {
            return hyphenator != null ? hyphenator.Hyphenate(word) : null;
        }

        /// <summary>Gets the hyphenation symbol.</summary>
        /// <returns>the hyphenation symbol</returns>
        public virtual char GetHyphenSymbol() {
            return hyphenSymbol;
        }

        /// <summary>Sets the hyphenation symbol to the specified value.</summary>
        /// <param name="hyphenSymbol">the new hyphenation symbol</param>
        public virtual void SetHyphenSymbol(char hyphenSymbol) {
            this.hyphenSymbol = hyphenSymbol;
        }
    }
}
