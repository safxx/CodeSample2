using System;
using Newtonsoft.Json;

namespace HighLoad.Framework.Json
{
    public class MyDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?) || objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long) reader.Value).LocalDateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
           writer.WriteRawValue(new DateTimeOffset((DateTime) value).ToUnixTimeSeconds().ToString());
        }
    }
}


    
