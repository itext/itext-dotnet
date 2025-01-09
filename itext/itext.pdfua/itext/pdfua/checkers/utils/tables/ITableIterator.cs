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
namespace iText.Pdfua.Checkers.Utils.Tables {
//\cond DO_NOT_DOCUMENT
    /// <summary>Interface that provides methods for iterating over the elements of a table.</summary>
    internal interface ITableIterator<T> {
        /// <summary>Checks if there is a next element in the iteration.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if there is a next element,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        bool HasNext();

        /// <summary>Gets the next element in the iteration.</summary>
        /// <returns>The next element.</returns>
        T Next();

        /// <summary>Gets the number of rows in the body of the table.</summary>
        /// <returns>The number of rows in the body of the table.</returns>
        int GetAmountOfRowsBody();

        /// <summary>Gets the number of rows in the header of the table.</summary>
        /// <returns>The number of rows in the header of the table.</returns>
        int GetAmountOfRowsHeader();

        /// <summary>Gets the number of rows in the footer of the table.</summary>
        /// <returns>The number of rows in the footer of the table.</returns>
        int GetAmountOfRowsFooter();

        /// <summary>Returns the amount of columns the table has.</summary>
        /// <remarks>
        /// Returns the amount of columns the table has.
        /// All rows in a table in UA specification must have the same column count.
        /// So return the max column count for correctly generated error messages.
        /// </remarks>
        /// <returns>the amount of columns</returns>
        int GetNumberOfColumns();

        /// <summary>Gets the row index of the current position.</summary>
        /// <returns>The row index.</returns>
        int GetRow();

        /// <summary>Gets the column index of current position.</summary>
        /// <returns>The column index.</returns>
        int GetCol();

        /// <summary>Gets the rowspan of current position.</summary>
        /// <returns>the rowspan</returns>
        int GetRowspan();

        /// <summary>Gets the colspan of the current position</summary>
        /// <returns>the colspan of current position</returns>
        int GetColspan();
    }
//\endcond
}
