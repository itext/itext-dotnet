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
using System.Collections.Generic;
using System.Linq;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class EnumUtil {
        /// <summary>
        /// Returns an enum value, rejecting <see langword="null"/>.
        /// </summary>
        /// <param name="enumInstance">the enum value to validate</param>
        /// <typeparam name="T">the enum type</typeparam>
        /// <returns><see langword="enumInstance"/></returns>
        public static T ThrowIfNull<T>(T? enumInstance) where T : struct {
            if (enumInstance == null) {
                throw new Exception("Expected not null enum instance");
            }

            return enumInstance.Value;
        }

        /// <summary>
        /// Returns all declared values of an enum type.
        /// </summary>
        /// <typeparam name="T">the enum type</typeparam>
        /// <returns>a list containing the enum's declared values</returns>
        public static  List<T> GetAllValuesOfEnum<T>() where T : struct {
            var enumValues = Enum.GetValues( typeof(T));
            return enumValues.Cast<T>().ToList();
        }
        
    }
}
