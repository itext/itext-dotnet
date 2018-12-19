using System;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class SvgCoordinateUtils {
        /// <summary>Converts relative coordinates to absolute ones.</summary>
        /// <remarks>
        /// Converts relative coordinates to absolute ones. Assumes that relative coordinates are represented by
        /// an array of coordinates with length proportional to the length of current coordinates array,
        /// so that current coordinates array is applied in segments to the relative coordinates array
        /// </remarks>
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
    }
}
