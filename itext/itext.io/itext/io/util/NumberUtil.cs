/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class NumberUtil {
        /// <summary>
        /// Converts a number to a float.
        /// </summary>
        /// <param name="obj">the number to convert, or <see langword="null"/></param>
        /// <returns>the float value, or <see langword="null"/> when <see langword="obj"/> is <see langword="null"/></returns>
        public static float? AsFloat(Object obj) {
            return obj != null ? Convert.ToSingle(obj) : (float?)null;
        }

        /// <summary>s
        /// Converts a number to an integer.
        /// </summary>
        /// <param name="obj">the number to convert, or <see langword="null"/></param>
        /// <returns>the integer value, or <see langword="null"/> when <see langword="obj"/> is <see langword="null"/></returns>
        public static int? AsInteger(Object obj) {
            return obj != null ? Convert.ToInt32(obj) : (int?)null;
        }
        
    }
}
