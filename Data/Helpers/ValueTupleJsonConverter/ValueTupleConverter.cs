﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sue3.Web.Blazor.Data
{
    public class ValueTupleConverter<T1> : JsonConverter<ValueTuple<T1>>
    {
        public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ValueTuple<T1> result = default;

            if (!reader.Read())
            {
                throw new JsonException();
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.ValueTextEquals("Item1") && reader.Read())
                {
                    result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }
                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Item1");
            JsonSerializer.Serialize<T1>(writer, value.Item1, options);
            writer.WriteEndObject();
        }
    }

    public class ValueTupleConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
    {
        public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            (T1, T2) result = default;

            if (!reader.Read())
            {
                throw new JsonException();
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.ValueTextEquals("Item1") && reader.Read())
                {
                    result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                }
                else if (reader.ValueTextEquals("Item2") && reader.Read())
                {
                    result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }
                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Item1");
            JsonSerializer.Serialize<T1>(writer, value.Item1, options);
            writer.WritePropertyName("Item2");
            JsonSerializer.Serialize<T2>(writer, value.Item2, options);
            writer.WriteEndObject();
        }
    }

    public class ValueTupleConverter<T1, T2, T3> : JsonConverter<ValueTuple<T1, T2, T3>>
    {
        public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            (T1, T2, T3) result = default;

            if (!reader.Read())
            {
                throw new JsonException();
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.ValueTextEquals("Item1") && reader.Read())
                {
                    result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                }
                else if (reader.ValueTextEquals("Item2") && reader.Read())
                {
                    result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                }
                else if (reader.ValueTextEquals("Item3") && reader.Read())
                {
                    result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }
                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Item1");
            JsonSerializer.Serialize<T1>(writer, value.Item1, options);
            writer.WritePropertyName("Item2");
            JsonSerializer.Serialize<T2>(writer, value.Item2, options);
            writer.WritePropertyName("Item3");
            JsonSerializer.Serialize<T3>(writer, value.Item3, options);
            writer.WriteEndObject();
        }
    }
}
