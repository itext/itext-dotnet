/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class SvgCoordinateUtils {
        /// <summary>Converts relative coordinates to absolute ones.</summary>
        /// <remarks>
        /// Converts relative coordinates to absolute ones. Assumes that relative coordinates are represented by
        /// an array of coordinates with length proportional to the length of current coordinates array,
        /// so that current coordinates array is applied in segments to the relative coordinates array
        /// </remarks>
        /// <param name="relativeCoordinates">the initial set of coordinates</param>
        /// <param name="currentCoordinates">an array representing the point relative to which the relativeCoordinates are defined
        ///     </param>
        /// <returns>a String array of absolute coordinates, with the same length as the input array</returns>
        public static String[] MakeRelativeOperatorCoordinatesAbsolute(String[] relativeCoordinates, double[] currentCoordinates
            ) {
            if (relativeCoordinates.Length % currentCoordinates.Length != 0) {
                throw new ArgumentException(SvgExceptionMessageConstant.COORDINATE_ARRAY_LENGTH_MUST_BY_DIVISIBLE_BY_CURRENT_COORDINATES_ARRAY_LENGTH
                    );
            }
            String[] absoluteOperators = new String[relativeCoordinates.Length];
            for (int i = 0; i < relativeCoordinates.Length; ) {
                for (int j = 0; j < currentCoordinates.Length; j++, i++) {
                    double relativeDouble = Double.Parse(relativeCoordinates[i], System.Globalization.CultureInfo.InvariantCulture
                        );
                    relativeDouble += currentCoordinates[j];
                    absoluteOperators[i] = SvgCssUtils.ConvertDoubleToString(relativeDouble);
                }
            }
            return absoluteOperators;
        }

        /// <summary>Calculate the angle between two vectors</summary>
        /// <param name="vectorA">first vector</param>
        /// <param name="vectorB">second vector</param>
        /// <returns>angle between vectors in radians units</returns>
        public static double CalculateAngleBetweenTwoVectors(Vector vectorA, Vector vectorB) {
            return Math.Acos((double)vectorA.Dot(vectorB) / ((double)vectorA.Length() * (double)vectorB.Length()));
        }
    }
}
