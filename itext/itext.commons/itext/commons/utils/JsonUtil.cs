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
using System.Globalization;
using System.IO;
using System.Text;
using iText.Commons.Logs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace iText.Commons.Utils
{
    /// <summary>Utility class for JSON serialization and deserialization operations. Not for public use.</summary>
    public sealed class JsonUtil
    {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(JsonUtil));
        
        private JsonUtil()
        {
            // empty constructor
        }
        
        /// <summary>
        /// Compares two json strings without considering the order of the elements.
        /// </summary>
        /// <param name="expectedString">expected json string</param>
        /// <param name="toCompare">string for comparison</param>
        /// <returns>true if two json string are equals, false otherwise</returns>
        public static bool AreTwoJsonObjectEquals(String expectedString, String toCompare) {
            JObject expectedObject = JObject.Parse(expectedString);
            JObject actualObject = JObject.Parse(toCompare);
            
            return JObject.DeepEquals(expectedObject, actualObject);
        }
        
        /// <summary>
        /// Serializes passed object to JSON string.
        /// </summary>
        /// <param name="value">the object which will be serialized</param>
        /// <returns>the JSON string representation of passed object or null if it is impossible to serialize to JSON</returns>
        public static string SerializeToString(Object value) {
            try
            {
                JsonSerializer jsonSerializer = CreateAndConfigureJsonSerializer();
                jsonSerializer.Formatting = Formatting.Indented;
                
                StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
                stringWriter.NewLine = "\n";

                JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter);
                
                    jsonTextWriter.CloseOutput = false;
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, value, null);
                
                
                return stringWriter.ToString();
            }
            catch (Exception ex) {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_SERIALIZE_OBJECT, ex.GetType().Name, ex.Message));
                return null;
            }
        }
        
        /// <summary>
        /// Serializes passed object to minimal JSON string without spaces and line breaks.
        /// </summary>
        /// <param name="value">the object which will be serialized</param>
        /// <returns>the minimal JSON string representation of passed object or null if it is impossible to serialize to JSON</returns>
        public static string SerializeToMinimalString(Object value) {
            try {
                JsonSerializer jsonSerializer = CreateAndConfigureJsonSerializer();
                jsonSerializer.Formatting = Formatting.None;
                
                StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
                stringWriter.NewLine = "";
                
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, value, null);
                }
                
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_SERIALIZE_OBJECT, ex.GetType().Name, ex.Message));
                return null;
            }
        }

        public static void SerializeToStream(Stream outputStream, Object value) {
            try {
                JsonSerializer jsonSerializer = CreateAndConfigureJsonSerializer();
                jsonSerializer.Formatting = Formatting.Indented;
                
                StreamWriter streamWriter = new StreamWriter(outputStream,new System.Text.UTF8Encoding(),1024, true);
                streamWriter.NewLine = "\n";
                
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, value, null);
                }
            }
            catch (Exception ex)
            {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_SERIALIZE_OBJECT, ex.GetType().Name, ex.Message));
            }
        }
        
        public static void SerializeToMinimalStream(Stream outputStream, Object value) {
            try {
                JsonSerializer jsonSerializer = CreateAndConfigureJsonSerializer();
                jsonSerializer.Formatting = Formatting.None;
                
                StreamWriter streamWriter = new StreamWriter(outputStream,new System.Text.UTF8Encoding(),1024, true);
                streamWriter.NewLine = "";
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter)) {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, value, null);
                }
            }
            catch (Exception ex) {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_SERIALIZE_OBJECT, ex.GetType().Name, ex.Message));
            }
        } 

        /// <summary>
        /// Deserializes passed JSON stream to object with passed type.
        /// </summary>
        /// <param name="content">the JSON stream which represent object</param>
        /// <param name="objectType">the type of object which will be deserialized</param>
        /// <typeparam name="T">the type of object which will be deserialized</typeparam>
        /// <returns>the deserialized object or null if operation of deserialization is impossible</returns>
        public static T DeserializeFromStream<T>(Stream content) {
            try {
                JsonReader reader = new JsonTextReader(new StreamReader(content));
                JsonSerializer serializer = new JsonSerializer();
                return (T) serializer.Deserialize(reader, typeof(T));
            }
            catch (JsonException ex) {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_DESERIALIZE_JSON, ex.GetType(), ex.Message));
                return default(T);
            }
        }
        
        /// <summary>
        /// Deserializes passed JSON string to object with passed type.
        /// </summary>
        /// <param name="content">the JSON string which represent object</param>
        /// <param name="objectType">the type of object which will be deserialized</param>
        /// <typeparam name="T">the type of object which will be deserialized</typeparam>
        /// <returns>the deserialized object or null if operation of deserialization is impossible</returns>
        public static T DeserializeFromString<T>(String content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (JsonException ex) {
                LOGGER.LogWarning(MessageFormatUtil.Format(
                    CommonsLogMessageConstant.UNABLE_TO_DESERIALIZE_JSON, ex.GetType(), ex.Message));
                return default(T);
            }
        }

        private static JsonSerializer CreateAndConfigureJsonSerializer() {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            // Use StringEnumConverter to serialize enum as string
            settings.Converters.Add(new StringEnumConverter());
            // Don't serialize null fields
            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonSerializer.Create(settings);
        }
    }
}
