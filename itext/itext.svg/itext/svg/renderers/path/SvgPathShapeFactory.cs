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
using System;
using System.Collections.Generic;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Path {
    /// <summary>
    /// A factory for creating
    /// <see cref="IPathShape"/>
    /// objects.
    /// </summary>
    public class SvgPathShapeFactory {
        private SvgPathShapeFactory() {
        }

        /// <summary>
        /// Creates a configured
        /// <see cref="IPathShape"/>
        /// object based on the passed Svg path data instruction tag.
        /// </summary>
        /// <param name="name">svg path element's path-data instruction name.</param>
        /// <returns>IPathShape implementation</returns>
        public static IPathShape CreatePathShape(String name) {
            return new PathShapeMapper().GetMapping().Get(name);
        }

        /// <summary>Finds the appropriate number of arguments for a path command, based on the passed Svg path data instruction tag.
        ///     </summary>
        /// <param name="name">svg path element's path-data instruction name.</param>
        /// <returns>an integer value with the required number of arguments or null if there is no mapping for the given value
        ///     </returns>
        public static int GetArgumentCount(String name) {
            IDictionary<String, int?> map = new PathShapeMapper().GetArgumentCount();
            if (map.ContainsKey(name.ToUpperInvariant())) {
                return (int)map.Get(name.ToUpperInvariant());
            }
            return -1;
        }
    }
}
