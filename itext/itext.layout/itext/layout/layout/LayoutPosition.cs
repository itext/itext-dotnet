/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Layout.Layout {
    /// <summary>We use a simplified version of CSS positioning.</summary>
    /// <remarks>
    /// We use a simplified version of CSS positioning.
    /// See https://www.webkit.org/blog/117/webcore-rendering-iv-absolutefixed-and-relative-positioning
    /// </remarks>
    public class LayoutPosition {
        /// <summary>Default positioning by normal rules of block and line layout.</summary>
        public const int STATIC = 1;

        /// <summary>
        /// Relative positioning is exactly like static positioning except that the left, top, right and bottom properties
        /// can be used to apply a translation to the object.
        /// </summary>
        /// <remarks>
        /// Relative positioning is exactly like static positioning except that the left, top, right and bottom properties
        /// can be used to apply a translation to the object. Relative positioning is literally nothing more than a paint-time translation.
        /// As far as layout is concerned, the object is at its original position.
        /// </remarks>
        public const int RELATIVE = 2;

        /// <summary>
        /// Absolute positioned objects are positioned relative to the containing block, which is the nearest enclosing
        /// ancestor block with a position other than 'static'.
        /// </summary>
        public const int ABSOLUTE = 3;

        /// <summary>Fixed positioned objects are positioned relative to the viewport, i.e., the page area of the current page.
        ///     </summary>
        public const int FIXED = 4;
    }
}
