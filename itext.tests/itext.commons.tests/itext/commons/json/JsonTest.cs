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
using System.IO;
using System.Text;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Json {
    [NUnit.Framework.Category("UnitTest")]
    public class JsonTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/commons/json/JsonTest/";

        [NUnit.Framework.Test]
        public virtual void Utf8CharsetStringTest() {
            NUnit.Framework.Assert.AreEqual("\"©\"", new JsonString("©").ToJson());
        }

        [NUnit.Framework.Test]
        public virtual void RoundNumberTest() {
            NUnit.Framework.Assert.AreEqual("-4", new JsonNumber(-4).ToJson());
        }

        [NUnit.Framework.Test]
        public virtual void SerializeSimpleJsonTest() {
            String cmp = SOURCE_FOLDER + "simple.json";
            JsonValue simpleJson = CreateSimpleJson();
            String resultString = simpleJson.ToJson();
            JsonValue resultJson = JsonValue.FromJson(resultString);
            JsonValue cmpJson = JsonValue.FromJson(GetJsonStringFromFile(cmp));
            NUnit.Framework.Assert.AreEqual(cmpJson, resultJson);
            NUnit.Framework.Assert.AreEqual(cmpJson, simpleJson);
        }

        [NUnit.Framework.Test]
        public virtual void SerializeStringWithLineBreaksTest() {
            String cmp = SOURCE_FOLDER + "stringsWithLineBreaks.json";
            String[] stringsForSerialization = CreateStringWithLineBreaks();
            IList<JsonValue> list = new List<JsonValue>();
            foreach (String str in stringsForSerialization) {
                list.Add(new JsonString(str));
            }
            JsonValue strings = new JsonArray(list);
            String resultString = strings.ToJson();
            JsonValue resultJson = JsonValue.FromJson(resultString);
            JsonValue cmpJson = JsonValue.FromJson(GetJsonStringFromFile(cmp));
            NUnit.Framework.Assert.AreEqual(cmpJson, resultJson);
            NUnit.Framework.Assert.AreEqual(cmpJson, strings);
        }

        [NUnit.Framework.Test]
        public virtual void SerializeComplexStructureTest() {
            String cmp = SOURCE_FOLDER + "complexStructure.json";
            String cmpString = GetJsonStringFromFile(cmp);
            JsonValue complexStructure = CreateComplexStructureObject();
            String resultString = complexStructure.ToJson();
            JsonValue resultJson = JsonValue.FromJson(resultString);
            JsonValue cmpJson = JsonValue.FromJson(cmpString);
            NUnit.Framework.Assert.AreEqual(cmpString, resultString);
            NUnit.Framework.Assert.AreEqual(cmpJson, resultJson);
            NUnit.Framework.Assert.AreEqual(cmpJson, complexStructure);
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeInvalidJsonFileStringTest() {
            String source = SOURCE_FOLDER + "invalidJson.json";
            String jsonString = GetJsonStringFromFile(source);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(jsonString));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void NullStringTest() {
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("nullString", JsonNull.JSON_NULL);
            JsonObject obj = new JsonObject();
            obj.Add("nullString", new JsonString(null));
            JsonValue serDeserObj = JsonValue.FromJson(obj.ToJson());
            NUnit.Framework.Assert.AreEqual(cmpObj, serDeserObj);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStringTest() {
            JsonString emptyString = new JsonString("");
            String json = emptyString.ToJson();
            JsonValue deserialized = JsonValue.FromJson(json);
            NUnit.Framework.Assert.AreEqual(emptyString, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void SpecialCharactersInStringTest() {
            String[] specialStrings = new String[] { "\"quoted\"", "\\backslash\\", "/forward/slash/", "\b\f\n\r\t", "Mixed: \"quotes\" and \\backslash\\ and \n newlines"
                , "\u0000\u0001\u001F" };
            foreach (String str in specialStrings) {
                JsonString jsonString = new JsonString(str);
                String serialized = jsonString.ToJson();
                JsonValue deserialized = JsonValue.FromJson(serialized);
                NUnit.Framework.Assert.AreEqual(jsonString, deserialized, "Failed for: " + str);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SpecialNumberValuesTest() {
            JsonNumber nanNumber = new JsonNumber(double.NaN);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => nanNumber.ToJson());
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to serialize json into string"));
            JsonNumber infNumber = new JsonNumber(double.PositiveInfinity);
            e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => infNumber.ToJson());
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to serialize json into string"));
            JsonNumber negInfNumber = new JsonNumber(double.NegativeInfinity);
            e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => negInfNumber.ToJson());
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to serialize json into string"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyArrayTest() {
            JsonArray emptyArray = new JsonArray();
            String serialized = emptyArray.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(emptyArray, deserialized);
            NUnit.Framework.Assert.AreEqual("[]", serialized);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyObjectTest() {
            JsonObject emptyObject = new JsonObject();
            String serialized = emptyObject.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(emptyObject, deserialized);
            NUnit.Framework.Assert.AreEqual("{}", serialized);
        }

        [NUnit.Framework.Test]
        public virtual void ArrayWithMixedTypesTest() {
            JsonArray mixedArray = new JsonArray();
            mixedArray.Add(new JsonString("string"));
            mixedArray.Add(new JsonNumber(42));
            mixedArray.Add(JsonBoolean.Of(true));
            mixedArray.Add(JsonBoolean.Of(false));
            mixedArray.Add(JsonNull.JSON_NULL);
            mixedArray.Add(new JsonObject());
            mixedArray.Add(new JsonArray());
            String serialized = mixedArray.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(mixedArray, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void DuplicateKeysInObjectTest() {
            JsonObject obj = new JsonObject();
            obj.Add("key", new JsonString("first"));
            obj.Add("key", new JsonString("second"));
            String serialized = obj.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            // The second value should have overwritten the first
            JsonObject deserializedObj = (JsonObject)deserialized;
            NUnit.Framework.Assert.AreEqual(1, deserializedObj.GetFields().Count);
            NUnit.Framework.Assert.AreEqual(new JsonString("second"), deserializedObj.GetFields().Get("key"));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonMissingQuoteTest() {
            String malformedJson = "{\"key: \"value\"}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

#if NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void MalformedJsonTrailingCommaTest() {
            String malformedJson = "{\"key\": \"value\",}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }
#endif // NETSTANDARD2_0

        [NUnit.Framework.Test]
        public virtual void MalformedJsonUnterminatedArrayTest() {
            String malformedJson = "[1, 2, 3";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonUnterminatedObjectTest() {
            String malformedJson = "{\"key\": \"value\"";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonInvalidNumberTest() {
            String malformedJson = "{\"key\": 12.34.56}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyJsonStringTest() {
            String emptyJson = "";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(emptyJson));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceOnlyJsonTest() {
            String whitespaceJson = "   \t\n   ";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(whitespaceJson
                ));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidWhitespaceAroundValuesTest() {
            String jsonWithWhitespace = "  {  \"key\"  :  \"value\"  }  ";
            JsonValue deserialized = JsonValue.FromJson(jsonWithWhitespace);
            JsonObject expected = new JsonObject();
            expected.Add("key", new JsonString("value"));
            NUnit.Framework.Assert.AreEqual(expected, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void KeyWithSpecialCharactersTest() {
            JsonObject obj = new JsonObject();
            obj.Add("key with spaces", new JsonString("value1"));
            obj.Add("key\"with\"quotes", new JsonString("value2"));
            obj.Add("key\\with\\backslash", new JsonString("value3"));
            obj.Add("key\nwith\nnewlines", new JsonString("value4"));
            obj.Add("key/with/slashes", new JsonString("value5"));
            String serialized = obj.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(obj, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void BooleanSerializationTest() {
            JsonBoolean trueValue = JsonBoolean.Of(true);
            JsonBoolean falseValue = JsonBoolean.Of(false);
            String serializedTrue = trueValue.ToJson();
            String serializedFalse = falseValue.ToJson();
            NUnit.Framework.Assert.AreEqual("true", serializedTrue);
            NUnit.Framework.Assert.AreEqual("false", serializedFalse);
            JsonValue deserializedTrue = JsonValue.FromJson(serializedTrue);
            JsonValue deserializedFalse = JsonValue.FromJson(serializedFalse);
            NUnit.Framework.Assert.AreEqual(trueValue, deserializedTrue);
            NUnit.Framework.Assert.AreEqual(falseValue, deserializedFalse);
        }

        [NUnit.Framework.Test]
        public virtual void StringWithOnlyEscapedCharactersTest() {
            String onlyEscaped = "\"\\\b\f\n\r\t";
            JsonString jsonString = new JsonString(onlyEscaped);
            String serialized = jsonString.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(jsonString, deserialized);
        }

#if NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void LeadingZerosInNumbersTest() {
            // JSON does not allow leading zeros, but we can test what happens after deserialization
            String jsonWithNumber = "{\"value\": 007}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(jsonWithNumber
                ));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }
#endif // NETSTANDARD2_0

        [NUnit.Framework.Test]
        public virtual void ArrayWithOnlyNullsTest() {
            JsonArray arrayOfNulls = new JsonArray();
            for (int i = 0; i < 100; i++) {
                arrayOfNulls.Add(JsonNull.JSON_NULL);
            }
            String serialized = arrayOfNulls.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(arrayOfNulls, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void StringWithAllControlCharactersTest() {
            StringBuilder sb = new StringBuilder();
            // Test all control characters (0x00 to 0x1F)
            for (int i = 0; i <= 0x1F; i++) {
                sb.Append((char)i);
            }
            String controlChars = sb.ToString();
            JsonString jsonString = new JsonString(controlChars);
            String serialized = jsonString.ToJson();
            JsonValue deserialized = JsonValue.FromJson(serialized);
            NUnit.Framework.Assert.AreEqual(jsonString, deserialized);
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonMissingColonTest() {
            String malformedJson = "{\"key\" \"value\"}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

        [NUnit.Framework.Test]
        public virtual void MalformedJsonExtraCommaInArrayTest() {
            String malformedJson = "[1, 2,, 3]";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }

#if NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void MalformedJsonSingleQuotesTest() {
            String malformedJson = "{'key': 'value'}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }
#endif // NETSTANDARD2_0

#if NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void MalformedJsonUnescapedNewlineInStringTest() {
            String malformedJson = "{\"key\": \"value with\nnewline\"}";
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => JsonValue.FromJson(malformedJson)
                );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to parse json string"));
        }
#endif // NETSTANDARD2_0

        [NUnit.Framework.Test]
        public virtual void JsonWithCommentsTest() {
            String jsonWithComments = "{ /* comment */ \"key\": \"value\" }";
            JsonObject cmpObj = new JsonObject();
            cmpObj.Add("key", new JsonString("value"));
            NUnit.Framework.Assert.AreEqual(cmpObj, JsonValue.FromJson(jsonWithComments));
        }

        [NUnit.Framework.Test]
        public virtual void ConsecutiveBackslashesTest() {
            String[] backslashStrings = new String[] { "\\", "\\\\", "\\\\\\", "\\\\\\\\", "path\\to\\file", "\\\\server\\share"
                 };
            foreach (String str in backslashStrings) {
                JsonString jsonString = new JsonString(str);
                String serialized = jsonString.ToJson();
                JsonValue deserialized = JsonValue.FromJson(serialized);
                NUnit.Framework.Assert.AreEqual(jsonString, deserialized, "Failed for: " + str);
            }
        }

        private String GetJsonStringFromFile(String pathToFile) {
            byte[] fileBytes = File.ReadAllBytes(System.IO.Path.Combine(pathToFile));
            // Use String(byte[]) because there is autoporting for this
            // construction by sharpen by call JavaUtil#GetStringForBytes
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(fileBytes, System.Text.Encoding.UTF8);
        }

        private static JsonValue CreateComplexStructureObject() {
            JsonArray arr1 = new JsonArray();
            arr1.Add(new JsonString("someStr1"));
            arr1.Add(new JsonString("someStr2"));
            JsonObject obj4 = new JsonObject();
            obj4.Add("arrayStr", arr1);
            JsonObject obj6 = new JsonObject();
            obj6.Add("integer", new JsonNumber(13));
            obj6.Add("name", new JsonString("someName"));
            JsonArray arr2 = new JsonArray();
            arr2.Add(obj6);
            JsonObject obj7 = new JsonObject();
            obj7.Add("integer", new JsonNumber(0));
            obj7.Add("name", new JsonString(""));
            arr2.Add(obj7);
            obj4.Add("grandsons", arr2);
            JsonArray arr3 = new JsonArray();
            arr3.Add(new JsonString(""));
            JsonObject obj8 = new JsonObject();
            obj8.Add("arrayStr", arr3);
            JsonArray arr4 = new JsonArray();
            arr4.Add(obj7);
            obj8.Add("grandsons", arr4);
            IDictionary<String, JsonValue> fields3 = new LinkedDictionary<String, JsonValue>();
            fields3.Put("ChildMapkey", obj4);
            fields3.Put("ChildMapKey2", obj8);
            JsonObject obj3 = new JsonObject(fields3);
            IDictionary<String, JsonValue> fields2 = new LinkedDictionary<String, JsonValue>();
            fields2.Put("FirstMapKey", new JsonNumber(15.1234));
            fields2.Put("SecondMapKey", new JsonNumber(8));
            fields2.Put("NullInstance", JsonNull.JSON_NULL);
            fields2.Put("TrueInstance", JsonBoolean.Of(true));
            fields2.Put("FalseInstance", JsonBoolean.Of(false));
            JsonObject obj2 = new JsonObject(fields2);
            IDictionary<String, JsonValue> fields1 = new LinkedDictionary<String, JsonValue>();
            fields1.Put("map", obj2);
            fields1.Put("str", new JsonString("StringFieldValue"));
            fields1.Put("childsMap", obj3);
            return new JsonObject(fields1);
        }

        private static JsonValue CreateSimpleJson() {
            IList<JsonValue> list = new List<JsonValue>();
            list.Add(new JsonString("FIRST_VALUE"));
            list.Add(new JsonString("FIRST_VALUE"));
            list.Add(new JsonString("SECOND_VALUE"));
            IDictionary<String, JsonValue> fields = new LinkedDictionary<String, JsonValue>();
            fields.Put("firstValue", new JsonString("SECOND_VALUE"));
            fields.Put("enumArray", new JsonArray(list));
            return new JsonObject(fields);
        }

        private static String[] CreateStringWithLineBreaks() {
            return new String[] { "String\n\rtest", "  \n   \t" };
        }
    }
}
