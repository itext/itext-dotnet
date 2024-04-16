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
using iText.Layout.Font;

namespace iText.Layout.Font.Selectorstrategy {
    /// <summary>The class implements strategy where the best font for each symbol is used if possible.</summary>
    public class BestMatchFontSelectorStrategy : AbstractFontSelectorStrategy {
        /// <summary>
        /// Creates a new instance of
        /// <see cref="BestMatchFontSelectorStrategy"/>.
        /// </summary>
        /// <param name="fontProvider">the font provider</param>
        /// <param name="fontSelector">the font selector</param>
        /// <param name="additionalFonts">the set of fonts to be used additionally to the fonts added to font provider.
        ///     </param>
        public BestMatchFontSelectorStrategy(FontProvider fontProvider, FontSelector fontSelector, FontSet additionalFonts
            )
            : base(fontProvider, fontSelector, additionalFonts) {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsCurrentFontCheckRequired() {
            return true;
        }

        /// <summary>
        /// The factory for
        /// <see cref="BestMatchFontSelectorStrategy"/>.
        /// </summary>
        public sealed class BestMatchFontSelectorStrategyFactory : IFontSelectorStrategyFactory {
            /// <summary><inheritDoc/></summary>
            public IFontSelectorStrategy CreateFontSelectorStrategy(FontProvider fontProvider, FontSelector fontSelector
                , FontSet additionalFonts) {
                return new BestMatchFontSelectorStrategy(fontProvider, fontSelector, additionalFonts);
            }
        }
    }
}
