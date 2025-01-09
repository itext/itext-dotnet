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
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Base interface to customize placing flex items on main flex direction.</summary>
    internal interface IFlexItemMainDirector {
        /// <summary>Apply the direction for placement the items in flex container.</summary>
        /// <param name="lines">
        /// flex lines calculated by
        /// <see cref="FlexUtil"/>.
        /// </param>
        /// <returns>All child renderers in updated order.</returns>
        IList<IRenderer> ApplyDirection(IList<IList<FlexItemInfo>> lines);

        /// <summary>Apply the direction for placement the items in flex line.</summary>
        /// <param name="renderers">
        /// list of renderers or
        /// <see cref="FlexItemInfo"/>.
        /// </param>
        void ApplyDirectionForLine<T>(IList<T> renderers);

        /// <summary>Apply alignment on main flex direction.</summary>
        /// <param name="line">flex line of items to apply alignment to.</param>
        /// <param name="justifyContent">alignment to apply.</param>
        /// <param name="freeSpace">precalculated free space to distribute between flex items in a line.</param>
        void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent, float
             freeSpace);
    }
//\endcond
}
