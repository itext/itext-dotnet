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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;

namespace iText.Commons.Json {
    /// <summary>Class representing json array value.</summary>
    public sealed class JsonArray : JsonValue {
        private readonly IList<JsonValue> values;

        /// <summary>
        /// Creates a new empty
        /// <see cref="JsonArray"/>.
        /// </summary>
        public JsonArray()
            : base() {
            this.values = new List<JsonValue>();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="JsonArray"/>
        /// with provided values.
        /// </summary>
        /// <param name="values">values to put into json array</param>
        public JsonArray(IList<JsonValue> values)
            : base() {
            this.values = new List<JsonValue>(values);
        }

        /// <summary>Gets a copy of json array values.</summary>
        /// <returns>json array values</returns>
        public IList<JsonValue> GetValues() {
            return new List<JsonValue>(values);
        }

        /// <summary>Adds a new value into json array.</summary>
        /// <param name="value">a value to put into json array</param>
        public void Add(JsonValue value) {
            values.Add(value);
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.Commons.Json.JsonArray that = (iText.Commons.Json.JsonArray)obj;
            return Enumerable.SequenceEqual(this.values, that.values);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(values);
        }
    }
}
