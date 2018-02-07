using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDB.Models
{
    public class Parameter
    {
        [BsonId]
        public string Name;

        public static Parameter FromJson(JToken json) => new Parameter()
        {
            Name = (string)json
        };
    }
}