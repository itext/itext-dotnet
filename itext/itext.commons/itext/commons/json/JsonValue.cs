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

namespace iText.Commons.Json {
    /// <summary>Class representing generic json value.</summary>
    public abstract class JsonValue {
        /// <summary>
        /// Creates a new
        /// <see cref="JsonValue"/>.
        /// </summary>
        protected internal JsonValue() {
        }

        // Empty constructor
        /// <summary>
        /// Parses string and creates
        /// <see cref="JsonValue"/>
        /// out of it.
        /// </summary>
        /// <param name="json">string to parse</param>
        /// <returns>
        /// 
        /// <see cref="JsonValue"/>
        /// representing the input string
        /// </returns>
        public static iText.Commons.Json.JsonValue FromJson(String json) {
            return JsonValueConverter.FromJson(json);
        }

        /// <summary>
        /// Converts this
        /// <see cref="JsonValue"/>
        /// into a string.
        /// </summary>
        /// <returns>
        /// string representing this
        /// <see cref="JsonValue"/>
        /// </returns>
        public String ToJson() {
            return JsonValueConverter.ToJson(this);
        }
    }
}
