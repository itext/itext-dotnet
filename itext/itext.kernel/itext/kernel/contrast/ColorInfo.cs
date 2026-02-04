/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;

namespace iText.Kernel.Contrast {
    /// <summary>Abstract base class representing rendering information for contrast analysis.</summary>
    /// <remarks>
    /// Abstract base class representing rendering information for contrast analysis.
    /// <para />
    /// This class encapsulates the common properties shared by all render information types
    /// used in contrast checking: the color and geometric path of the rendered element.
    /// <para />
    /// Subclasses should extend this class to represent specific types of rendered content,
    /// such as text or background elements.
    /// </remarks>
    public abstract class ColorInfo {
        private readonly Color color;

        private readonly Path path;

        /// <summary>Constructs a new ContrastInformationRenderInfo with the specified color and path.</summary>
        /// <param name="color">the color of the rendered element</param>
        /// <param name="path">the geometric path defining the shape and position of the rendered element</param>
        public ColorInfo(Color color, Path path) {
            this.color = color;
            this.path = path;
        }

        /// <summary>Gets the color of the rendered element.</summary>
        /// <returns>the color of this render information</returns>
        public virtual Color GetColor() {
            return color;
        }

        /// <summary>Gets the geometric path of the rendered element.</summary>
        /// <remarks>
        /// Gets the geometric path of the rendered element.
        /// <para />
        /// The path defines the shape and position of the element on the page.
        /// </remarks>
        /// <returns>the path of this render information</returns>
        public virtual Path GetPath() {
            return path;
        }
    }
}
