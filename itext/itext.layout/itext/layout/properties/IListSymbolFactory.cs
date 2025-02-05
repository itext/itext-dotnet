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
using iText.Layout;
using iText.Layout.Element;

namespace iText.Layout.Properties {
    /// <summary>Interface for implementing custom symbols for lists</summary>
    public interface IListSymbolFactory {
        /// <summary>Creates symbol.</summary>
        /// <param name="index">- the positive (greater then zero) index of list item in list.</param>
        /// <param name="list">
        /// - the
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// with all properties of corresponding list.
        /// </param>
        /// <param name="listItem">
        /// - the
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// with all properties of corresponding list item.
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Element.IElement"/>
        /// representing symbol.
        /// </returns>
        IElement CreateSymbol(int index, IPropertyContainer list, IPropertyContainer listItem);
    }
}
