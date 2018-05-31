using Newtonsoft.Json.Linq;

namespace ShenZhen.Models
{
    public class Parameter
    {
        public string Name;

        public static Parameter FromJson(JToken json) => new Parameter()
        {
            Name = (string)json
        };
    }
}