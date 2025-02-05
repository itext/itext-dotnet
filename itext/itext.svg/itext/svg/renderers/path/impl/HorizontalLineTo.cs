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
using System;
using iText.Kernel.Geom;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements lineTo(H) attribute of SVG's path element</summary>
    public class HorizontalLineTo : LineTo {
//\cond DO_NOT_DOCUMENT
        internal const int ARGUMENT_SIZE = 1;
//\endcond

        /// <summary>Creates an absolute Horizontal LineTo.</summary>
        public HorizontalLineTo()
            : this(false) {
        }

        /// <summary>Creates a Horizontal LineTo.</summary>
        /// <remarks>Creates a Horizontal LineTo. Set argument to true to create a relative HorizontalLineTo.</remarks>
        /// <param name="relative">whether this is a relative HorizontalLineTo or not</param>
        public HorizontalLineTo(bool relative)
            : base(relative) {
        }

        public override void SetCoordinates(String[] inputCoordinates, Point startPoint) {
            String[] normalizedCoords = new String[LineTo.ARGUMENT_SIZE];
            // An H or h command is equivalent to an L or l command with 0 specified for the y coordinate.
            normalizedCoords[0] = inputCoordinates[0];
            normalizedCoords[1] = IsRelative() ? "0" : Convert.ToString(startPoint.GetY(), System.Globalization.CultureInfo.InvariantCulture
                );
            base.SetCoordinates(normalizedCoords, startPoint);
        }
    }
}
