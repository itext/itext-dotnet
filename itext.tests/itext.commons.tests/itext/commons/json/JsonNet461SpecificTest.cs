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
#if !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Json {
    [NUnit.Framework.Category("UnitTest")]
    public class JsonNet461SpecificTest : ExtendedITextTest {

        [NUnit.Framework.Test]
        public virtual void MalformedJsonTrailingCommaTest() {
            String malformedJson = "{\"key\": \"value\",}";
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("key", new JsonString("value"));
            NUnit.Framework.Assert.AreEqual(cmpObj, JsonValue.FromJson(malformedJson));
        }

        [NUnit.Framework.Test]
        public virtual void LeadingZerosInNumbersTest() {
            // JSON does not allow leading zeros, but we can test what happens after deserialization
            String jsonWithNumber = "{\"value\": 007}";
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("value", new JsonNumber(7));
            NUnit.Framework.Assert.AreEqual(cmpObj, JsonValue.FromJson(jsonWithNumber));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonSingleQuotesTest() {
            String malformedJson = "{'key': 'value'}";
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("key", new JsonString("value"));
            NUnit.Framework.Assert.AreEqual(cmpObj, JsonValue.FromJson(malformedJson));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonUnescapedNewlineInStringTest() {
            String malformedJson = "{\"key\": \"value with\nnewline\"}";
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("key", new JsonString("value with\nnewline"));
            NUnit.Framework.Assert.AreEqual(cmpObj, JsonValue.FromJson(malformedJson));
        }
    }
}

#endif // !NETSTANDARD2_0
