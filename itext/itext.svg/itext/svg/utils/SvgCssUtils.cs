/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Util;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Node;

namespace iText.Svg.Utils {
    /// <summary>Utility class that facilitates parsing values from CSS.</summary>
    public sealed class SvgCssUtils {
        private SvgCssUtils() {
        }

        /// <summary>Splits a given String into a list of substrings.</summary>
        /// <remarks>
        /// Splits a given String into a list of substrings.
        /// The string is split up by commas and whitespace characters (\t, \n, \r, \f).
        /// </remarks>
        /// <param name="value">the string to be split</param>
        /// <returns>a list containing the split strings, an empty list if the value is null or empty</returns>
        public static IList<String> SplitValueList(String value) {
            IList<String> result = new List<String>();
            if (value != null && value.Length > 0) {
                value = value.Trim();
                String[] list = iText.IO.Util.StringUtil.Split(value, "\\s*(,|\\s)\\s*");
                result.AddAll(JavaUtil.ArraysAsList(list));
            }
            return result;
        }

        ///<summary>Converts a float pts values to pixels </summary>
        ///<param name="v"> the value to be converted pixels</param>
        ///<returns>float converted value pts/0.75f</returns>
        [System.ObsoleteAttribute(@"Will be replaced by the iText.StyledXmlParser.Css.Util.CssUtils#convertPtsToPx(float) in update 7.2.")]
        public static float ConvertPtsToPx(float v)
        {
            return v / 0.75f;
        }
        
        ///<summary>Converts a double pts values to pixels </summary>
        ///<param name="v"> the value to be converted pixels</param>
        ///<returns>double converted value pts/0.75</returns>
        [System.ObsoleteAttribute(@"Will be replaced by the iText.StyledXmlParser.Css.Util.CssUtils#convertPtsToPx(float) in update 7.2.")]
        public static double ConvertPtsToPx(double v)
        {
            return v / 0.75;
        }
       
        ///<summary>Converts a float to a String.</summary>
        ///<param name="value">to be converted float value</param>
        ///<returns>the value in a String representation</returns>   
        public static string ConvertFloatToString(float value)
        {
            return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture); ;
        }
       
        ///<summary>Converts a double to a String.</summary>
        ///<param name="value">to be converted double value</param>
        ///<returns>the value in a String representation</returns>   
        public static string ConvertDoubleToString(double value)
        {
            return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture); ;
        }

        /// <summary>
        /// Checks if an
        /// <see cref="iText.StyledXmlParser.Node.IElementNode"/>
        /// represents a style sheet link.
        /// </summary>
        /// <param name="headChildElement">the head child element</param>
        /// <returns>true, if the element node represents a style sheet link</returns>
        [System.ObsoleteAttribute(@"Will be replaced by the iText.StyledXmlParser.Css.Util.CssUtils.IsStyleSheetLink(headChildElement) in the 7.2 update")]
        public static bool IsStyleSheetLink(IElementNode headChildElement) {
            return CssUtils.IsStyleSheetLink(headChildElement);
        }
    }
}
