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
using System.Collections.Generic;

namespace iText.Svg.Renderers {
    /// <summary>Interface that defines branches in the NodeRenderer structure.</summary>
    /// <remarks>
    /// Interface that defines branches in the NodeRenderer structure. Differs from a leaf renderer
    /// in that a branch has children and as such methods that can add or retrieve those children.
    /// </remarks>
    public interface IBranchSvgNodeRenderer : ISvgNodeRenderer {
        /// <summary>Adds a renderer as the last element of the list of children.</summary>
        /// <param name="child">any renderer</param>
        void AddChild(ISvgNodeRenderer child);

        /// <summary>Gets all child renderers of this object.</summary>
        /// <returns>the list of child renderers (in the order that they were added)</returns>
        IList<ISvgNodeRenderer> GetChildren();
    }
}
