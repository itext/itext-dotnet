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
using iText.Commons.Datastructures;
using iText.Kernel.Pdf;

namespace iText.Kernel.DI.Pagetree {
    /// <summary>This interface is used to create a list of pages from a pages dictionary.</summary>
    public interface IPageTreeListFactory {
        /// <summary>Creates a list based on the  value of the pages dictionary.</summary>
        /// <remarks>
        /// Creates a list based on the  value of the pages dictionary.
        /// If null, it means we are dealing with document creation. In other cases the pdf document pages
        /// dictionary will be passed.
        /// </remarks>
        /// <param name="pagesDictionary">The pages dictionary</param>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <returns>The list</returns>
        ISimpleList<T> CreateList<T>(PdfDictionary pagesDictionary);
    }
}
