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
using iText.Commons.Utils;

namespace iText.Commons.Json {
    /// <summary>Class representing json number value.</summary>
    public sealed class JsonNumber : JsonValue {
        private readonly double value;

        /// <summary>
        /// Creates a new
        /// <see cref="JsonNumber"/>
        /// representing a provided value.
        /// </summary>
        /// <param name="value">
        /// to wrap into this
        /// <see cref="JsonNumber"/>
        /// </param>
        public JsonNumber(double value)
            : base() {
            this.value = value;
        }

        /// <summary>
        /// Gets a number value wrapped into this
        /// <see cref="JsonNumber"/>.
        /// </summary>
        /// <returns>a number value</returns>
        public double GetValue() {
            return value;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.Commons.Json.JsonNumber that = (iText.Commons.Json.JsonNumber)obj;
            return Object.Equals(this.value, that.value);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(value);
        }
    }
}
