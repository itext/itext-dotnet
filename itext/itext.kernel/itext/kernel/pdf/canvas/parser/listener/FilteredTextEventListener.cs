/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
