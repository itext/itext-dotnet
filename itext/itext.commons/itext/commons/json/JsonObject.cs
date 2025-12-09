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
    /// <summary>Class representing json object value.</summary>
    public sealed class JsonObject : JsonValue {
        private readonly IDictionary<String, JsonValue> fields;

        /// <summary>
        /// Creates a new empty
        /// <see cref="JsonObject"/>.
        /// </summary>
        public JsonObject()
            : base() {
            this.fields = new LinkedDictionary<String, JsonValue>();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="JsonObject"/>
        /// with provided fields.
        /// </summary>
        /// <param name="fields">fields to put into json object</param>
        public JsonObject(IDictionary<String, JsonValue> fields)
            : base() {
            this.fields = new LinkedDictionary<String, JsonValue>(fields);
        }

        /// <summary>Gets a copy of the json object fields.</summary>
        /// <returns>json object fields</returns>
        public IDictionary<String, JsonValue> GetFields() {
            return new LinkedDictionary<String, JsonValue>(fields);
        }

        /// <summary>Adds a new field into json object.</summary>
        /// <param name="key">a key to add into json object fields</param>
        /// <param name="value">
        /// a value to add into json object fields under the
        /// <paramref name="key"/>
        /// </param>
        public void Add(String key, JsonValue value) {
            fields.Put(key, value);
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.Commons.Json.JsonObject that = (iText.Commons.Json.JsonObject)obj;
            return Enumerable.SequenceEqual(this.fields, that.fields);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(fields);
        }
    }
}
