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
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Tagging {
    /// <summary>
    /// A layout element which has
    /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties">accessibility properties</see>.
    /// </summary>
    /// <remarks>
    /// A layout element which has
    /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties">accessibility properties</see>.
    /// They define element's <em>role</em> (
    /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties.GetRole()"/>
    /// ) - the name
    /// that will be used to tag the element if it is added to a Tagged PDF document.
    /// They can also define other metadata for the tag.
    /// </remarks>
    public interface IAccessibleElement {
        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties">accessibility properties</see>.
        /// </summary>
        /// <remarks>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties">accessibility properties</see>
        /// . See also
        /// <see cref="IAccessibleElement"/>.
        /// </remarks>
        /// <returns>an interface that allows to specify properties of a tagged element in Tagged PDF.</returns>
        AccessibilityProperties GetAccessibilityProperties();
    }
}
