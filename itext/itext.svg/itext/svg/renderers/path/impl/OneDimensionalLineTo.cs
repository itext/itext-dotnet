/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>
    /// Implements the abstract functionality of a line pathing operation that only changes the path's coordinate
    /// in one dimension, i.e a vertical (V/v) or horizontal (H/h) line.
    /// </summary>
    public abstract class OneDimensionalLineTo : AbstractPathShape {
        /// <summary>The current x or y coordinate that will remain unchanged when drawing the line.</summary>
        /// <remarks>
        /// The current x or y coordinate that will remain unchanged when drawing the line.
        /// For vertical lines the x value will not change,
        /// for horizontal lines y value will not change.
        /// </remarks>
        protected internal readonly String CURRENT_NONCHANGING_DIMENSION_VALUE = "CURRENT_NONCHANGING_DIMENSION_VALUE";

        /// <summary>The minimum x or y value in the dimension that will change.</summary>
        /// <remarks>
        /// The minimum x or y value in the dimension that will change.
        /// For vertical lines this will be the y value of the bottom-most point.
        /// For horizontal lines this will be the x value of the left-most point.
        /// </remarks>
        protected internal readonly String MINIMUM_CHANGING_DIMENSION_VALUE = "MINIMUM_CHANGING_DIMENSION_VALUE";

        /// <summary>The maximum x or y value in the dimension that will change.</summary>
        /// <remarks>
        /// The maximum x or y value in the dimension that will change.
        /// For vertical lines this will be the y value of the top-most point.
        /// For horizontal lines this will be the x value of the righ-most point.
        /// </remarks>
        protected internal readonly String MAXIMUM_CHANGING_DIMENSION_VALUE = "MAXIMUM_CHANGING_DIMENSION_VALUE";

        /// <summary>The final x or y value in the dimension that will change.</summary>
        /// <remarks>
        /// The final x or y value in the dimension that will change.
        /// For vertical lines this will be the y value of the last point.
        /// For horizontal lines this will be the x value of the last point.
        /// </remarks>
        protected internal readonly String ENDING_CHANGING_DIMENSION_VALUE = "ENDING_CHANGING_DIMENSION_VALUE";

        /// <summary>Sets the minimum and maximum value of the coordinates in the dimension that changes for this line segment.
        ///     </summary>
        private void SetSegmentPoints(String[] coordinates) {
            float min = float.MaxValue;
            float max = float.MinValue;
            float current;
            for (int x = 0; x < coordinates.Length; x++) {
                current = float.Parse(coordinates[x], System.Globalization.CultureInfo.InvariantCulture);
                if (current > max) {
                    max = current;
                }
                if (current < min) {
                    min = current;
                }
            }
            if (properties == null) {
                properties = new Dictionary<String, String>();
            }
            properties.Put(MAXIMUM_CHANGING_DIMENSION_VALUE, max.ToString());
            properties.Put(MINIMUM_CHANGING_DIMENSION_VALUE, min.ToString());
        }

        /// <summary>The coordinates for a one dimensional line.</summary>
        /// <remarks>
        /// The coordinates for a one dimensional line. The first argument is the x or y value of the dimension that does not
        /// change. The rest of the coordinates represent the operators to the (V/v) pathing operator and the current
        /// coordinate in the dimension that does change.
        /// </remarks>
        /// <param name="staticDimensionValue">The value of the non-changing dimension.</param>
        /// <param name="coordinates">an array containing point values for path coordinates</param>
        private void SetCoordinates(String staticDimensionValue, String[] coordinates) {
            properties = new Dictionary<String, String>();
            SetSegmentPoints(coordinates);
            properties.Put(CURRENT_NONCHANGING_DIMENSION_VALUE, staticDimensionValue);
            properties.Put(ENDING_CHANGING_DIMENSION_VALUE, coordinates[coordinates.Length - 1]);
        }

        public override void SetCoordinates(String[] coordinates) {
            String staticDimensionValue = coordinates[0];
            String[] operatorArgs = JavaUtil.ArraysCopyOfRange(coordinates, 1, coordinates.Length);
            SetCoordinates(staticDimensionValue, operatorArgs);
        }
    }
}
