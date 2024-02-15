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
using iText.Layout.Element;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that provides methods for checking PDF/UA compliance of table elements.</summary>
    public sealed class TableCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="TableCheckUtil"/>
        /// instance.
        /// </summary>
        private TableCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if the table is pdf/ua compliant.</summary>
        /// <param name="table">the table to check.</param>
        public static void CheckLayoutTable(Table table) {
            new CellResultMatrix(table.GetNumberOfColumns(), table).CheckValidTableTagging();
        }
    }
}
