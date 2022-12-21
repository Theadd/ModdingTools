using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModdingTools.Core.Common;

public class JsonLib
{
    public static T Print<T>(T obj)
    {
        Console.WriteLine(Stringify(obj));
        return obj;
    }
    
    public static string Stringify<T>(T obj)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new FileSystemInfoConverter()
            },
            Formatting = Formatting.Indented
        };
        
        return JsonConvert.SerializeObject(obj, settings);
        
        // Deserialize JSON string to type T
        // T info = JsonConvert.DeserializeObject<T>(json, settings);
    }
    
    public class FileSystemInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(FileSystemInfo).IsAssignableFrom(objectType);
        }
 
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jObject = JObject.Load(reader);
            var fullPath = jObject["FullPath"].Value<string>();
            return Activator.CreateInstance(objectType, fullPath);
        }
 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var info = value as FileSystemInfo;
            var obj = info == null
                ? null
                : new
                {
                    FullPath = info.FullName
                };
            var token = JToken.FromObject(obj);
            token.WriteTo(writer);
        }
    }
}

