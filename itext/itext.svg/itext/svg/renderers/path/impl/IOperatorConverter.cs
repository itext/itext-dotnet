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
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>
    /// A locally used strategy for converting relative coordinates to absolute coordinates (in the current SVG coordinate
    /// space).
    /// </summary>
    /// <remarks>
    /// A locally used strategy for converting relative coordinates to absolute coordinates (in the current SVG coordinate
    /// space). Its implementation differs between Smooth (Shorthand) Bézier curves and all other path commands.
    /// </remarks>
    public interface IOperatorConverter {
        /// <summary>Convert an array of relative coordinates to an array with the same size containing absolute coordinates.
        ///     </summary>
        /// <param name="relativeCoordinates">the initial set of coordinates</param>
        /// <param name="initialPoint">an array representing the point relative to which the relativeCoordinates are defined
        ///     </param>
        /// <returns>a String array of absolute coordinates, with the same length as the input array</returns>
        String[] MakeCoordinatesAbsolute(String[] relativeCoordinates, double[] initialPoint);
    }

//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Implementation of
    /// <see cref="IOperatorConverter"/>
    /// specifically for smooth curves.
    /// </summary>
    /// <remarks>
    /// Implementation of
    /// <see cref="IOperatorConverter"/>
    /// specifically for smooth curves. It will convert all operators from
    /// relative to absolute coordinates except the first coordinate pair.
    /// This implementation is used by the Smooth (Shorthand) Bézier curve commands, because the conversion of the first
    /// coordinate pair is calculated in
    /// <see cref="iText.Svg.Renderers.Impl.PathSvgNodeRenderer.GetShapeCoordinates(iText.Svg.Renderers.Path.IPathShape, iText.Svg.Renderers.Path.IPathShape, System.String[])
    ///     "/>.
    /// </remarks>
    internal class SmoothOperatorConverter : IOperatorConverter {
        public virtual String[] MakeCoordinatesAbsolute(String[] relativeCoordinates, double[] initialPoint) {
            String[] result = new String[relativeCoordinates.Length];
            Array.Copy(relativeCoordinates, 0, result, 0, 2);
            // convert all relative operators to absolute operators ...
            relativeCoordinates = SvgCoordinateUtils.MakeRelativeOperatorCoordinatesAbsolute(relativeCoordinates, initialPoint
                );
            // ... but don't store the first coordinate pair
            Array.Copy(relativeCoordinates, 2, result, 2, relativeCoordinates.Length - 2);
            return result;
        }
    }
//\endcond

//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Default implementation of
    /// <see cref="IOperatorConverter"/>
    /// used by the regular (not-smooth) curves and other path commands.
    /// </summary>
    /// <remarks>
    /// Default implementation of
    /// <see cref="IOperatorConverter"/>
    /// used by the regular (not-smooth) curves and other path commands.
    /// It will convert all operators from relative to absolute coordinates.
    /// </remarks>
    internal class DefaultOperatorConverter : IOperatorConverter {
        public virtual String[] MakeCoordinatesAbsolute(String[] relativeCoordinates, double[] initialPoint) {
            return SvgCoordinateUtils.MakeRelativeOperatorCoordinatesAbsolute(relativeCoordinates, initialPoint);
        }
    }
//\endcond
}
