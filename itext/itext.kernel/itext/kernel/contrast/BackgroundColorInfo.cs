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
    /// <summary>Represents rendering information for a background element in contrast analysis.</summary>
    /// <remarks>
    /// Represents rendering information for a background element in contrast analysis.
    /// <para />
    /// This class extends
    /// <see cref="ColorInfo"/>
    /// to specifically represent
    /// background shapes and their colors, which are used to calculate contrast ratios
    /// against text elements for accessibility compliance.
    /// <para />
    /// Background elements include filled paths, rectangles, and other non-text content
    /// that may appear behind text on a PDF page.
    /// </remarks>
    public class BackgroundColorInfo : ColorInfo {
        /// <summary>
        /// Constructs a new
        /// <see cref="BackgroundColorInfo"/>
        /// with the specified color and path.
        /// </summary>
        /// <param name="color">the fill color of the background element</param>
        /// <param name="path">the geometric path defining the shape and position of the background element</param>
        public BackgroundColorInfo(Color color, Path path)
            : base(color, path) {
        }
    }
}
