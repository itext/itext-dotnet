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
namespace iText.Commons.Json {
    /// <summary>Class representing json boolean value.</summary>
    public sealed class JsonBoolean : JsonValue {
        private readonly bool value;

        private static readonly iText.Commons.Json.JsonBoolean TRUE = new iText.Commons.Json.JsonBoolean(true);

        private static readonly iText.Commons.Json.JsonBoolean FALSE = new iText.Commons.Json.JsonBoolean(false);

        private JsonBoolean(bool value)
            : base() {
            this.value = value;
        }

        /// <summary>
        /// Gets
        /// <see cref="JsonBoolean"/>
        /// representing input boolean value.
        /// </summary>
        /// <param name="value">boolean value</param>
        /// <returns>
        /// 
        /// <see cref="JsonBoolean"/>
        /// representing input boolean value
        /// </returns>
        public static iText.Commons.Json.JsonBoolean Of(bool value) {
            return value ? TRUE : FALSE;
        }

        /// <summary>
        /// Gets a boolean value wrapped into this
        /// <see cref="JsonBoolean"/>.
        /// </summary>
        /// <returns>a boolean value</returns>
        public bool GetValue() {
            return value;
        }
    }
}
