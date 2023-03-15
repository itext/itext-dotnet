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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Resolve {
    /// <summary>Utilities class to merge CSS properties.</summary>
    public sealed class CssPropertyMerger {
        /// <summary>
        /// Creates a new
        /// <see cref="CssPropertyMerger"/>
        /// class.
        /// </summary>
        private CssPropertyMerger() {
        }

        /// <summary>Merges text decoration.</summary>
        /// <param name="firstValue">the first value</param>
        /// <param name="secondValue">the second value</param>
        /// <returns>the merged value</returns>
        public static String MergeTextDecoration(String firstValue, String secondValue) {
            if (firstValue == null) {
                return secondValue;
            }
            else {
                if (secondValue == null) {
                    return firstValue;
                }
            }
            ICollection<String> merged = NormalizeTextDecoration(firstValue);
            merged.AddAll(NormalizeTextDecoration(secondValue));
            StringBuilder sb = new StringBuilder();
            foreach (String mergedProp in merged) {
                if (sb.Length != 0) {
                    sb.Append(" ");
                }
                sb.Append(mergedProp);
            }
            return sb.Length != 0 ? sb.ToString() : CommonCssConstants.NONE;
        }

        /// <summary>Normalizes text decoration values.</summary>
        /// <param name="value">the text decoration value</param>
        /// <returns>a set of normalized decoration values</returns>
        private static ICollection<String> NormalizeTextDecoration(String value) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(value, "\\s+");
            // LinkedHashSet to make order invariant of JVM
            ICollection<String> merged = new LinkedHashSet<String>();
            merged.AddAll(JavaUtil.ArraysAsList(parts));
            // if none and any other decoration are used together, none is displayed
            if (merged.Contains(CommonCssConstants.NONE)) {
                merged.Clear();
            }
            return merged;
        }
    }
}
