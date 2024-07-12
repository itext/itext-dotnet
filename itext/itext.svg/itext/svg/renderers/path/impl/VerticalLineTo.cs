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
using iText.Kernel.Geom;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements lineTo(V) attribute of SVG's path element</summary>
    public class VerticalLineTo : LineTo {
//\cond DO_NOT_DOCUMENT
        internal const int ARGUMENT_SIZE = 1;
//\endcond

        /// <summary>Creates an absolute Vertical LineTo.</summary>
        public VerticalLineTo()
            : this(false) {
        }

        /// <summary>Creates a Vertical LineTo.</summary>
        /// <remarks>Creates a Vertical LineTo. Set argument to true to create a relative VerticalLineTo.</remarks>
        /// <param name="relative">whether this is a relative VerticalLineTo or not</param>
        public VerticalLineTo(bool relative)
            : base(relative) {
        }

        public override void SetCoordinates(String[] inputCoordinates, Point startPoint) {
            String[] normalizedCoords = new String[LineTo.ARGUMENT_SIZE];
            // A V or v command is equivalent to an L or l command with 0 specified for the x coordinate.
            normalizedCoords[0] = IsRelative() ? "0" : SvgCssUtils.ConvertDoubleToString(startPoint.GetX());
            normalizedCoords[1] = inputCoordinates[0];
            base.SetCoordinates(normalizedCoords, startPoint);
        }
    }
}
