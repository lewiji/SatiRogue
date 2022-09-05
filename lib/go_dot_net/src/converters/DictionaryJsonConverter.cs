using System;
using Godot.Collections;
using Newtonsoft.Json;
namespace SatiRogue.lib.go_dot_net.src.converters;

/// <summary>
/// Newtonsoft.JSON converter used to help serialize Godot dictionary objects.
/// </summary>
public class DictionaryJsonConverter : JsonConverter {
   /// <inheritdoc/>
   public override bool CanConvert(Type objectType) {
      return objectType == typeof(Dictionary<,>);
   }

   /// <inheritdoc/>
   public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
      var map = new Dictionary<dynamic?, dynamic?>();

      if (reader.TokenType == JsonToken.Null) {
         return map;
      }

      if (reader.TokenType != JsonToken.StartObject) {
         throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing map.");
      }

      while (reader.Read()) {
         if (reader.TokenType == JsonToken.EndObject) {
            break;
         }

         if (reader.TokenType != JsonToken.PropertyName) {
            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing map.");
         }

         var key = serializer.Deserialize<dynamic?>(reader);
         _ = reader.Read();
         var value = serializer.Deserialize<dynamic?>(reader);
         map.Add(key, value);
      }
      return map;
   }

   /// <inheritdoc/>
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
      if (value == null) {
         return;
      }

      writer.WriteStartObject();
      var map = (value as Dictionary<dynamic, dynamic?>)!;

      foreach (var item in map) {
         serializer.Serialize(writer, item.Key);
         serializer.Serialize(writer, item.Value);
      }
      writer.WriteEndObject();
   }
}