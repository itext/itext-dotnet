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
#if NETSTANDARD2_0

using iText.Commons.Exceptions;
using iText.Commons.Utils;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace iText.Commons.Json {

//\cond DO_NOT_DOCUMENT

    internal class JsonValueConverter : JsonConverter<JsonValue>
    {
        public override void Write(Utf8JsonWriter writer, JsonValue value, JsonSerializerOptions options)
        {
            if (value is JsonNull)
            {
                writer.WriteNullValue();
                return;
            }
        
            if (value is JsonBoolean)
            {
                writer.WriteBooleanValue(((JsonBoolean) value).GetValue());
                return;
            }

            if (value is JsonNumber)
            {
                double doubleValue = ((JsonNumber) value).GetValue();
                if (double.NaN.Equals(doubleValue) || double.PositiveInfinity.Equals(doubleValue) || double.NegativeInfinity.Equals(doubleValue))
                {
                    throw new JsonException("NAN and INFINITE are not supported");
                }

                writer.WriteNumberValue(doubleValue);
                return;
            }

            if (value is JsonString)
            {
                String stringValue = ((JsonString) value).GetValue();
                if (stringValue == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteStringValue(stringValue);
                }
                return;
            }

            if (value is JsonArray)
            {
                writer.WriteStartArray();
                foreach (var item in ((JsonArray) value).GetValues())
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                return;
            }

            if (value is JsonObject)
            {
                writer.WriteStartObject();
                foreach (var field in ((JsonObject) value).GetFields())
                {
                    writer.WritePropertyName(field.Key);
                    JsonSerializer.Serialize(writer, field.Value, options);
                }
                writer.WriteEndObject();
                return;
            }
        
            // Should never be here
            throw new JsonException("Unknown JsonValue type: " + value.GetType());
        }

        public override JsonValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadValue(ref reader, options);
        }

        internal static String ToJson(JsonValue value) {
            var options = CreateJsonSerializerOptions();
            try {
                return JsonSerializer.Serialize(value, options);
            } catch (JsonException e) {
                // Should never be here
                throw new ITextException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.JSON_SERIALIZATION_FAILED, e.Message));
            }
        }

        internal static JsonValue FromJson(String json) {
            var options = CreateJsonSerializerOptions();
            try {
                return JsonSerializer.Deserialize<JsonValue>(json, options);
            } catch (JsonException e) {
                throw new ITextException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.JSON_PARSE_FAILED, e.Message));
            }
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            options.Converters.Add(new JsonValueConverter());
            return options;
        }
        private JsonValue ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        return new JsonString(reader.GetString());

                    case JsonTokenType.Number:
                        return new JsonNumber(reader.GetDouble());

                    case JsonTokenType.True:
                        return JsonBoolean.Of(true);

                    case JsonTokenType.False:
                        return JsonBoolean.Of(false);

                    case JsonTokenType.Null:
                        return JsonNull.JSON_NULL;

                    case JsonTokenType.StartArray:
                        return ReadArray(ref reader, options);

                    case JsonTokenType.StartObject:
                        return ReadObject(ref reader, options);

                    default:
                        throw new System.IO.IOException("Unexpected token: " + reader.TokenType);
                }
        }

        private JsonValue ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var list = new List<JsonValue>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                list.Add(ReadValue(ref reader, options));
            }

            return new JsonArray(list);
        }

        private JsonValue ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, JsonValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new System.IO.IOException("Expected property name");
                }

                string key = reader.GetString();
                reader.Read();

                dict[key] = ReadValue(ref reader, options);
            }

            return new JsonObject(dict);
        }
    }

//\endcond

}

#endif
