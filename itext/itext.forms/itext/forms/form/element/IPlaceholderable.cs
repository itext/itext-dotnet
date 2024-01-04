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

namespace iText.Forms.Form.Element {
    /// <summary>A marker interface that specifies that the layout object has placeholder.</summary>
    public interface IPlaceholderable {
        /// <summary>Gets the placeholder paragraph.</summary>
        /// <returns>the placeholder paragraph</returns>
        Paragraph GetPlaceholder();

        /// <summary>Sets the placeholder paragraph.</summary>
        /// <param name="placeholder">the paragraph to be used as placeholder</param>
        void SetPlaceholder(Paragraph placeholder);
    }
}
