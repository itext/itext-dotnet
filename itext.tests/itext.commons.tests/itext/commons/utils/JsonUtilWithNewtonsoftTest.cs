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
using System.IO;
using iText.Test;
using Newtonsoft.Json;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class JsonUtilWithNewtonsoftTest : ExtendedITextTest {
        private static Func<JsonSerializerSettings> DEFAULT_JSON_CONVERTER_SETTINGS;
        
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/commons/utils/JsonUtilTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUpHandler() {
            DEFAULT_JSON_CONVERTER_SETTINGS = JsonConvert.DefaultSettings;
        }

        [NUnit.Framework.TearDown]
        public virtual void ResetHandler() {
            JsonConvert.DefaultSettings = DEFAULT_JSON_CONVERTER_SETTINGS;
        }
        
                [NUnit.Framework.Test]
        public virtual void TryToOverrideDefaultSerializationSettingForSerializeToStringTest() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            String cmp = SOURCE_FOLDER + "classWithEnum.json";
            ClassWithEnum classWithEnum = CreateClassWithEnumObject();
            String resultString = JsonUtil.SerializeToString(classWithEnum);
            String cmpString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(cmpString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void TryToOverrideDefaultSerializationSettingForSerializeToStreamTest() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            String path = SOURCE_FOLDER + "classWithEnum.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
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
        public virtual void TryToOverrideDefaultSerializationSettingForSerializeToMinimalStringTest() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Arrays
            };
            String cmp = SOURCE_FOLDER + "minimalClassWithEnum.json";
            ClassWithEnum classWithEnum = CreateClassWithEnumObject();
            String resultString = JsonUtil.SerializeToMinimalString(classWithEnum);
            String compareString = GetJsonStringFromFile(cmp);
            NUnit.Framework.Assert.IsTrue(JsonUtil.AreTwoJsonObjectEquals(compareString, resultString));
        }

        [NUnit.Framework.Test]
        public virtual void TryToOverrideDefaultSerializationSettingForSerializeToMinimalStreamTest() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            String path = SOURCE_FOLDER + "minimalClassWithEnum.json";
            using (Stream inputStream = FileUtil.GetInputStreamForFile(path)) {
                using (MemoryStream baos = ConvertInputStreamToOutput(inputStream)) {
                    using (MemoryStream serializationResult = new MemoryStream()) {
                        JsonUtil.SerializeToMinimalStream(serializationResult, CreateClassWithEnumObject());
                        serializationResult.Flush();
                        NUnit.Framework.Assert.AreEqual(baos.ToArray(), serializationResult.ToArray());
                    }
                }
            }
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
        
        private static ClassWithEnum CreateClassWithEnumObject() {
            ClassWithEnum classWithEnum = new ClassWithEnum();
            classWithEnum.enumArray = new SomeEnum[] { SomeEnum.FIRST_VALUE, SomeEnum
                .FIRST_VALUE, SomeEnum.SECOND_VALUE };
            classWithEnum.firstValue = SomeEnum.SECOND_VALUE;
            return classWithEnum;
        }
        
        private class ClassWithEnum {
            public SomeEnum firstValue;

            public SomeEnum[] enumArray = new SomeEnum[] {  };

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                ClassWithEnum that = (ClassWithEnum)o;
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
    }
}
