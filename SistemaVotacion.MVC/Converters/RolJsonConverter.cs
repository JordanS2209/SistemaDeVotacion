using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVotacion.Modelos;
using System;

namespace SistemaVotacion.MVC.Converters
{
    public class RolJsonConverter : JsonConverter<Rol>
    {
        public override Rol ReadJson(JsonReader reader, Type objectType, Rol existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                // API devolvió "SuperAdmin" (u otro nombre) en lugar del objeto
                string roleName = (string)reader.Value;
                return new Rol { NombreRol = roleName, Id = 0 }; // ID desconocido, pero al menos tenemos el nombre para mostrar
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                // Objeto normal
                JObject jobject = JObject.Load(reader);
                return jobject.ToObject<Rol>();
            }

            throw new JsonSerializationException($"Unexpected token type {reader.TokenType} when parsing Rol.");
        }

        public override void WriteJson(JsonWriter writer, Rol value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
