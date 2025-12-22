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

using iText.Commons.Exceptions;
using iText.Commons.Utils;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iText.Commons.Json {

//\cond DO_NOT_DOCUMENT

    internal class JsonValueConverter : JsonConverter<JsonValue>
    {
        public override void WriteJson(JsonWriter writer, JsonValue value, JsonSerializer serializer)
        {
            if (value is JsonNull)
            {
                writer.WriteNull();
                return;
            }
        
            if (value is JsonBoolean)
            {
                writer.WriteValue(((JsonBoolean) value).GetValue());
                return;
            }

            if (value is JsonNumber)
            {
                double doubleValue = ((JsonNumber) value).GetValue();
                if (double.NaN.Equals(doubleValue) || double.PositiveInfinity.Equals(doubleValue) || double.NegativeInfinity.Equals(doubleValue))
                {
                    throw new JsonException("NAN and INFINITE are not supported");
                }

                if ((long) doubleValue == doubleValue)
                {
                    writer.WriteValue((long) doubleValue);
                }
                else
                {
                    writer.WriteValue(doubleValue);
                }
                return;
            }

            if (value is JsonString)
            {
                String stringValue = ((JsonString) value).GetValue();
                if (stringValue == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.WriteValue(stringValue);
                }
                return;
            }

            if (value is JsonArray)
            {
                writer.WriteStartArray();
                foreach (var item in ((JsonArray) value).GetValues())
                {
                    serializer.Serialize(writer, item);
                }
                writer.WriteEndArray();
                return;
            }

            if (value is JsonObject)
            {
                writer.WriteStartObject();
                foreach (var kv in ((JsonObject) value).GetFields())
                {
                    writer.WritePropertyName(kv.Key);
                    serializer.Serialize(writer, kv.Value);
                }
                writer.WriteEndObject();
                return;
            }
        
            // Should never be here
            throw new JsonException("Unknown JsonValue type: " + value.GetType());
        }

        public override JsonValue ReadJson(JsonReader reader, Type objectType, JsonValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return ReadValue(JToken.ReadFrom(reader));
        }

        internal static String ToJson(JsonValue value) {
            var settings = CreateJsonSerializerSettings();
            try {
                return JsonConvert.SerializeObject(value, settings);
            } catch (JsonException e) {
                // Should never be here
                throw new ITextException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.JSON_SERIALIZATION_FAILED, e.Message));
            }
        }

        internal static JsonValue FromJson(String json) {
            var settings = CreateJsonSerializerSettings();
            try
            {
                JsonValue value = JsonConvert.DeserializeObject<JsonValue>(json, settings);
                if (value == null)
                {
                    throw new ITextException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.JSON_PARSE_FAILED, "unknown error"));
                }
                return value;
            }
            catch (JsonException e)
            {
                throw new ITextException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.JSON_PARSE_FAILED, e.Message));
            }
        }

        private static JsonSerializerSettings CreateJsonSerializerSettings() {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include
            };

            settings.Converters.Add(new JsonValueConverter());
            return settings;
        }

        private JsonValue ReadValue(JToken token)
        {
            switch (token.Type)
                {
                    case JTokenType.String:
                        return new JsonString(token.Value<string>());

                    case JTokenType.Integer:
                    case JTokenType.Float:
                        return new JsonNumber(token.Value<double>());

                    case JTokenType.Boolean:
                        return JsonBoolean.Of(token.Value<bool>());

                    case JTokenType.Null:
                        return JsonNull.JSON_NULL;

                    case JTokenType.Array:
                        return ReadArray((JArray) token);

                    case JTokenType.Object:
                        return new JsonObject(ReadObject((JObject) token));

                    default:
                        throw new JsonException("Unexpected token: " + token.Type);
                }
        }

        private JsonValue ReadArray(JArray array)
        {
            JsonArray arr = new JsonArray();
            foreach (var item in array) {
                arr.Add(ReadValue(item));
            }

            return arr;
        }

        private IDictionary<string, JsonValue> ReadObject(JObject obj)
        {
            var dict = new Dictionary<string, JsonValue>();
            foreach (var prop in obj.Properties())
            {
                dict[prop.Name] = ReadValue(prop.Value);
            }

            return dict;
        }
    }

//\endcond

}

#endif
