/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text;
using iText.Kernel.Pdf.Canvas.Parser.Filter;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>A text event listener which filters events on the fly before passing them on to the delegate.</summary>
    /// <remarks>
    /// A text event listener which filters events on the fly before passing them on to the delegate.
    /// The only difference from
    /// <see cref="FilteredEventListener"/>
    /// is that this class conveniently implements
    /// <see cref="ITextExtractionStrategy"/>
    /// and can therefore used as a strategy on its own, apart from the inherited
    /// function of filtering event appropriately to its delegates.
    /// </remarks>
    public class FilteredTextEventListener : FilteredEventListener, ITextExtractionStrategy {
        /// <summary>
        /// Constructs a
        /// <see cref="FilteredTextEventListener"/>
        /// instance with a
        /// <see cref="ITextExtractionStrategy"/>
        /// delegate.
        /// </summary>
        /// <param name="delegate_">a delegate that fill be called when all the corresponding filters for an event pass
        ///     </param>
        /// <param name="filterSet">filters attached to the delegate that will be tested before passing an event on to the delegate
        ///     </param>
        public FilteredTextEventListener(ITextExtractionStrategy delegate_, params IEventFilter[] filterSet)
            : base(delegate_, filterSet) {
        }

        /// <summary>
        /// As an resultant text we use the concatenation of all the resultant text of all the delegates that implement
        /// <see cref="ITextExtractionStrategy"/>.
        /// </summary>
        /// <returns>the resulting concatenation of the text extracted from the delegates</returns>
        public virtual String GetResultantText() {
            StringBuilder sb = new StringBuilder();
            foreach (IEventListener delegate_ in delegates) {
                if (delegate_ is ITextExtractionStrategy) {
                    sb.Append(((ITextExtractionStrategy)delegate_).GetResultantText());
                }
            }
            return sb.ToString();
        }
    }
}
