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
using iText.Commons.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class JsonUtilTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/commons/utils/JsonUtilTest/";

        private static bool isRunOnJava = false;

        static JsonUtilTest() {
            // Android-Conversion-Skip-Block-Start (cutting area is used to understand whether code is running on Android or not)
            isRunOnJava = true;
        }

        // Android-Conversion-Skip-Block-End
        [NUnit.Framework.Test]
        public virtual void Utf8CharsetStringTest() {
            NUnit.Framework.Assert.AreEqual("\"©\"", JsonUtil.SerializeToString("©"));
        }

        [NUnit.Framework.Test]
        public virtual void Utf8CharsetStreamTest() {
            MemoryStream byteArray = new MemoryStream();
            JsonUtil.SerializeToStream(byteArray, "©");
            NUnit.Framework.Assert.AreEqual("\"©\"", EncodingUtil.ConvertToString(byteArray.ToArray(), "UTF-8"));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeInstanceWithEnumStringTest() {
            String cmp = SOURCE_FOLDER + "classWithEnum.json";
            JsonUtilTest.ClassWithEnum classWithEnum = CreateClassWithEnumObject();
            String resultString = JsonUtil.SerializeToString(classWithEnum);
            String cmpString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(cmpString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeInstanceWithEnumStreamTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-7371 investigate different behavior of a few iTextCore tests on Java and Android)
            String cmp;
            if (isRunOnJava) {
                cmp = SOURCE_FOLDER + "classWithEnum.json";
            }
            else {
                // Test is run on Android, so field order will be different from Java.
                cmp = SOURCE_FOLDER + "classWithEnumAndroid.json";
            }
            using (Stream inputStream = FileUtil.GetInputStreamForFile(cmp)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToStream(serializationResult, CreateClassWithEnumObject());
                        serializationResult.Flush();
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalInstanceWithEnumStringTest() {
            String cmp = SOURCE_FOLDER + "minimalClassWithEnum.json";
            JsonUtilTest.ClassWithEnum classWithEnum = CreateClassWithEnumObject();
            String resultString = JsonUtil.SerializeToMinimalString(classWithEnum);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalInstanceWithEnumStreamTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-7371 investigate different behavior of a few iTextCore tests on Java and Android)
            String cmp;
            if (isRunOnJava) {
                cmp = SOURCE_FOLDER + "minimalClassWithEnum.json";
            }
            else {
                // Test is run on Android, so field order will be different from Java.
                cmp = SOURCE_FOLDER + "minimalClassWithEnumAndroid.json";
            }
            using (Stream inputStream = FileUtil.GetInputStreamForFile(cmp)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToMinimalStream(serializationResult, CreateClassWithEnumObject());
                        serializationResult.Flush();
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeStringWithLineBreakStringTest() {
            String cmp = SOURCE_FOLDER + "stringsWithLineBreaks.json";
            String[] stringsForSerialization = CreateStringWithLineBreaks();
            String resultString = JsonUtil.SerializeToString(stringsForSerialization);
            String cmpString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.AreEqual(cmpString, resultString);
        }

        [NUnit.Framework.Test]
        public virtual void SerializeStringWithLineBreakStreamTest() {
            String path = SOURCE_FOLDER + "stringsWithLineBreaks.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToStream(serializationResult, CreateStringWithLineBreaks());
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalStringWithLineBreakStringTest() {
            String cmp = SOURCE_FOLDER + "minimalStringsWithLineBreaks.json";
            String[] stringsForSerialization = CreateStringWithLineBreaks();
            String resultString = JsonUtil.SerializeToMinimalString(stringsForSerialization);
            String cmpString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.AreEqual(cmpString, resultString);
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalStringWithLineBreakStreamTest() {
            String path = SOURCE_FOLDER + "minimalStringsWithLineBreaks.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToMinimalStream(serializationResult, CreateStringWithLineBreaks());
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeComplexStructureStringTest() {
            String cmp = SOURCE_FOLDER + "complexStructure.json";
            JsonUtilTest.ComplexStructure complexStructure = CreateComplexStructureObject();
            String resultString = JsonUtil.SerializeToString(complexStructure);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeComplexStructureStreamTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-7371 investigate different behavior of a few iTextCore tests on Java and Android)
            String cmp;
            if (isRunOnJava) {
                cmp = SOURCE_FOLDER + "complexStructure.json";
            }
            else {
                // Test is run on Android, so field order will be different from Java.
                cmp = SOURCE_FOLDER + "complexStructureAndroid.json";
            }
            using (Stream inputStream = FileUtil.GetInputStreamForFile(cmp)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToStream(serializationResult, CreateComplexStructureObject());
                        NUnit.Framework.Assert.AreNotEqual(0, serializationResult.Length);
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalComplexStructureStringTest() {
            String cmp = SOURCE_FOLDER + "minimalComplexStructure.json";
            JsonUtilTest.ComplexStructure complexStructure = CreateComplexStructureObject();
            String resultString = JsonUtil.SerializeToMinimalString(complexStructure);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalComplexStructureStreamTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-7371 investigate different behavior of a few iTextCore tests on Java and Android)
            String cmp;
            if (isRunOnJava) {
                cmp = SOURCE_FOLDER + "minimalComplexStructure.json";
            }
            else {
                // Test is run on Android, so field order will be different from Java.
                cmp = SOURCE_FOLDER + "minimalComplexStructureAndroid.json";
            }
            using (Stream inputStream = FileUtil.GetInputStreamForFile(cmp)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToMinimalStream(serializationResult, CreateComplexStructureObject());
                        NUnit.Framework.Assert.AreNotEqual(0, serializationResult.Length);
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeWithNullFieldsStringTest() {
            String cmp = SOURCE_FOLDER + "serializeWithNullFields.json";
            JsonUtilTest.ClassWithDefaultValue complexStructure = CreateClassWithDefaultValueObject(null, 4, null);
            String resultString = JsonUtil.SerializeToString(complexStructure);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeWithNullFieldsStreamTest() {
            String path = SOURCE_FOLDER + "serializeWithNullFields.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToStream(serializationResult, CreateClassWithDefaultValueObject(null, 4, null));
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalWithNullFieldsStringTest() {
            String cmp = SOURCE_FOLDER + "minimalSerializeWithNullFields.json";
            JsonUtilTest.ClassWithDefaultValue complexStructure = CreateClassWithDefaultValueObject(null, 4, null);
            String resultString = JsonUtil.SerializeToMinimalString(complexStructure);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void SerializeToMinimalWithNullFieldsStreamTest() {
            String path = SOURCE_FOLDER + "minimalSerializeWithNullFields.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToMinimalStream(serializationResult, CreateClassWithDefaultValueObject(null, 4, null));
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.UNABLE_TO_DESERIALIZE_JSON, LogLevel = LogLevelConstants.WARN)]
        public virtual void DeserializeInvalidJsonFileStringTest() {
            String source = SOURCE_FOLDER + "invalidJson.json";
            String jsonString = GetJsonStringFromFile(source);
            String resultStr = JsonUtil.DeserializeFromString<String>(jsonString);
            NUnit.Framework.Assert.IsNull(resultStr);
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.UNABLE_TO_DESERIALIZE_JSON, LogLevel = LogLevelConstants.WARN)]
        public virtual void DeserializeInvalidJsonFileStreamTest() {
            String source = SOURCE_FOLDER + "invalidJson.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                String resultStr = JsonUtil.DeserializeFromStream<String>(inputStream);
                NUnit.Framework.Assert.IsNull(resultStr);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithDefaultValueStringTest() {
            String source = SOURCE_FOLDER + "classWithDefaultValue.json";
            String jsonString = GetJsonStringFromFile(source);
            JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromString<JsonUtilTest.ClassWithDefaultValue
                >(jsonString);
            NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject(null, 2, 5.0), instance);
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithDefaultValueStreamTest() {
            String source = SOURCE_FOLDER + "classWithDefaultValue.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromStream<JsonUtilTest.ClassWithDefaultValue
                    >(inputStream);
                NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject(null, 2, 5.0), instance);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeComplexStructureStringTest() {
            String source = SOURCE_FOLDER + "complexStructure.json";
            String jsonString = GetJsonStringFromFile(source);
            JsonUtilTest.ComplexStructure complexStructure = JsonUtil.DeserializeFromString<JsonUtilTest.ComplexStructure
                >(jsonString);
            NUnit.Framework.Assert.AreEqual(CreateComplexStructureObject(), complexStructure);
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeComplexStructureStreamTest() {
            String source = SOURCE_FOLDER + "complexStructure.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                JsonUtilTest.ComplexStructure complexStructure = JsonUtil.DeserializeFromStream<JsonUtilTest.ComplexStructure
                    >(inputStream);
                NUnit.Framework.Assert.AreEqual(CreateComplexStructureObject(), complexStructure);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeInstanceWithEnumStringTest() {
            String source = SOURCE_FOLDER + "classWithEnum.json";
            String jsonString = GetJsonStringFromFile(source);
            JsonUtilTest.ClassWithEnum classWithEnum = JsonUtil.DeserializeFromString<JsonUtilTest.ClassWithEnum>(jsonString
                );
            NUnit.Framework.Assert.AreEqual(CreateClassWithEnumObject(), classWithEnum);
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeInstanceWithEnumStreamTest() {
            String source = SOURCE_FOLDER + "classWithEnum.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                JsonUtilTest.ClassWithEnum classWithEnum = JsonUtil.DeserializeFromStream<JsonUtilTest.ClassWithEnum>(inputStream
                    );
                NUnit.Framework.Assert.AreEqual(CreateClassWithEnumObject(), classWithEnum);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithUnknownPropertiesStringTest() {
            String source = SOURCE_FOLDER + "classWithUnknownProperties.json";
            String jsonString = GetJsonStringFromFile(source);
            JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromString<JsonUtilTest.ClassWithDefaultValue
                >(jsonString);
            NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject("some small string", 8, 26.0), instance);
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithUnknownPropertiesStreamTest() {
            String source = SOURCE_FOLDER + "classWithUnknownProperties.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromStream<JsonUtilTest.ClassWithDefaultValue
                    >(inputStream);
                NUnit.Framework.Assert.IsNotNull(instance);
                NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject("some small string", 8, 26.0), instance);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithDefaultValueTypeReferenceStreamTest() {
            String source = SOURCE_FOLDER + "classWithDefaultValue.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(source)) {
                JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromStream<JsonUtilTest.ClassWithDefaultValue
                    >(inputStream);
                NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject(null, 2, 5.0), instance);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DeserializeWithDefaultValueTypeReferenceStringTest() {
            String source = SOURCE_FOLDER + "classWithDefaultValue.json";
            String jsonString = GetJsonStringFromFile(source);
            JsonUtilTest.ClassWithDefaultValue instance = JsonUtil.DeserializeFromString<JsonUtilTest.ClassWithDefaultValue
                >(jsonString);
            NUnit.Framework.Assert.AreEqual(CreateClassWithDefaultValueObject(null, 2, 5.0), instance);
        }

        private String GetJsonStringFromFile(String pathToFile) {
            byte[] fileBytes = File.ReadAllBytes(System.IO.Path.Combine(pathToFile));
            // Use String(byte[]) because there is autoporting for this
            // construction by sharpen by call JavaUtil#GetStringForBytes
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(fileBytes, System.Text.Encoding.UTF8);
        }

        private static MemoryStream ConvertInputStreamToOutput(Stream inputStream) {
            MemoryStream result = new MemoryStream();
            byte[] buffer = new byte[1024];
            int length;
            while ((length = inputStream.Read(buffer)) != -1) {
                result.Write(buffer, 0, length);
            }
            result.Flush();
            return result;
        }

        private static JsonUtilTest.ComplexStructure CreateComplexStructureObject() {
            JsonUtilTest.ComplexStructure complexStructure = new JsonUtilTest.ComplexStructure();
            complexStructure.map.Put("FirstMapKey", 15);
            complexStructure.map.Put("SecondMapKey", 8);
            complexStructure.str = "StringFieldValue";
            JsonUtilTest.ChildInComplexStructure child = new JsonUtilTest.ChildInComplexStructure();
            child.arrayStr = new String[] { "someStr1", "someStr2" };
            JsonUtilTest.GrandsonComplexStructure grandson = new JsonUtilTest.GrandsonComplexStructure();
            grandson.integer = 13;
            grandson.name = "someName";
            child.grandsons = new JsonUtilTest.GrandsonComplexStructure[] { grandson, new JsonUtilTest.GrandsonComplexStructure
                () };
            complexStructure.childsMap.Put("ChildMapkey", child);
            complexStructure.childsMap.Put("ChildMapKey2", new JsonUtilTest.ChildInComplexStructure());
            return complexStructure;
        }

        private static JsonUtilTest.ClassWithDefaultValue CreateClassWithDefaultValueObject(String firstString, int
             value, double? doubleValue) {
            return new JsonUtilTest.ClassWithDefaultValue(firstString, value, doubleValue);
        }

        private static JsonUtilTest.ClassWithEnum CreateClassWithEnumObject() {
            JsonUtilTest.ClassWithEnum classWithEnum = new JsonUtilTest.ClassWithEnum();
            classWithEnum.enumArray = new JsonUtilTest.SomeEnum[] { JsonUtilTest.SomeEnum.FIRST_VALUE, JsonUtilTest.SomeEnum
                .FIRST_VALUE, JsonUtilTest.SomeEnum.SECOND_VALUE };
            classWithEnum.firstValue = JsonUtilTest.SomeEnum.SECOND_VALUE;
            return classWithEnum;
        }

        private static String[] CreateStringWithLineBreaks() {
            return new String[] { "String\n\rtest", "  \n   \t" };
        }

        private class ComplexStructure {
            public IDictionary<String, int?> map = new LinkedDictionary<String, int?>();

            public String str = "";

            public IDictionary<String, JsonUtilTest.ChildInComplexStructure> childsMap = new LinkedDictionary<String, 
                JsonUtilTest.ChildInComplexStructure>();

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                JsonUtilTest.ComplexStructure that = (JsonUtilTest.ComplexStructure)o;
                return MapUtil.Equals(map, that.map) && Object.Equals(str, that.str) && MapUtil.Equals(childsMap, that.childsMap
                    );
            }

            public override int GetHashCode() {
                int result = map != null ? MapUtil.GetHashCode(map) : 0;
                result = 31 * result + (str != null ? str.GetHashCode() : 0);
                result = 31 * result + (childsMap != null ? MapUtil.GetHashCode(childsMap) : 0);
                return result;
            }
        }

        private class ChildInComplexStructure {
            public String[] arrayStr = new String[] { "" };

            public JsonUtilTest.GrandsonComplexStructure[] grandsons = new JsonUtilTest.GrandsonComplexStructure[] { new 
                JsonUtilTest.GrandsonComplexStructure() };

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                JsonUtilTest.ChildInComplexStructure that = (JsonUtilTest.ChildInComplexStructure)o;
                return JavaUtil.ArraysEquals(arrayStr, that.arrayStr) && JavaUtil.ArraysEquals(grandsons, that.grandsons);
            }

            public override int GetHashCode() {
                int result = JavaUtil.ArraysHashCode(arrayStr);
                result = 31 * result + JavaUtil.ArraysHashCode(grandsons);
                return result;
            }
        }

        private class GrandsonComplexStructure {
            public int integer = 0;

            public String name = "";

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                JsonUtilTest.GrandsonComplexStructure that = (JsonUtilTest.GrandsonComplexStructure)o;
                return integer == that.integer && Object.Equals(name, that.name);
            }

            public override int GetHashCode() {
                int result = integer;
                result = 31 * result + (name != null ? name.GetHashCode() : 0);
                return result;
            }
        }

        private class ClassWithEnum {
            public JsonUtilTest.SomeEnum firstValue;

            public JsonUtilTest.SomeEnum[] enumArray = new JsonUtilTest.SomeEnum[] {  };

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                JsonUtilTest.ClassWithEnum that = (JsonUtilTest.ClassWithEnum)o;
                return firstValue == that.firstValue && JavaUtil.ArraysEquals(enumArray, that.enumArray);
            }

            public override int GetHashCode() {
                int result = JavaUtil.ArraysHashCode(firstValue);
                result = 31 * result + JavaUtil.ArraysHashCode(enumArray);
                return result;
            }
        }

        private enum SomeEnum {
            FIRST_VALUE,
            SECOND_VALUE,
            THIRD_VALUE
        }

        private class ClassWithDefaultValue {
            public String firstString = "defaultValue";

            public int? integer = 3;

            public double? doubleValue = 0.0;

            public ClassWithDefaultValue(String firstString, int? integer, double? doubleValue) {
                if (firstString != null) {
                    this.firstString = firstString;
                }
                if (integer != null) {
                    this.integer = integer;
                }
                this.doubleValue = doubleValue;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                JsonUtilTest.ClassWithDefaultValue that = (JsonUtilTest.ClassWithDefaultValue)o;
                return Object.Equals(firstString, that.firstString) && Object.Equals(integer, that.integer) && Object.Equals
                    (doubleValue, that.doubleValue);
            }

            public override int GetHashCode() {
                int result = (firstString == null ? 0 : firstString.GetHashCode());
                result = 31 * result + (integer == null ? 0 : integer.GetHashCode());
                result = 31 * result + (doubleValue == null ? 0 : doubleValue.GetHashCode());
                return result;
            }
        }
    }
}
