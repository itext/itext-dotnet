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
using iText.Layout;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="ILargeElement"/>
    /// is a layout element which may get added to
    /// indefinitely, making the object prohibitively large.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="ILargeElement"/>
    /// is a layout element which may get added to
    /// indefinitely, making the object prohibitively large.
    /// In order to avoid consuming and holding on to undesirable amounts of
    /// resources, the contents of a
    /// <see cref="ILargeElement"/>
    /// can be flushed regularly
    /// by client code, e.g. at page boundaries or after a certain amount of additions.
    /// </remarks>
    public interface ILargeElement : IElement {
        /// <summary>Checks whether an element has already been marked as complete.</summary>
        /// <returns>the completion marker boolean</returns>
        bool IsComplete();

        /// <summary>Indicates that all the desired content has been added to this large element.</summary>
        void Complete();

        /// <summary>Writes the newly added content to the document.</summary>
        void Flush();

        /// <summary>Flushes the content which has just been added to the document.</summary>
        /// <remarks>
        /// Flushes the content which has just been added to the document.
        /// This is a method for internal usage and is called automatically by the document.
        /// </remarks>
        void FlushContent();

        /// <summary>Sets the document this element is bound to.</summary>
        /// <remarks>
        /// Sets the document this element is bound to.
        /// We cannot write a large element into several documents simultaneously because we would need
        /// more bulky interfaces for this feature. For now we went for simplicity.
        /// </remarks>
        /// <param name="document">the document</param>
        void SetDocument(Document document);
    }
}
