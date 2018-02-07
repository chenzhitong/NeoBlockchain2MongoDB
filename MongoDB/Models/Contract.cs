using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class Contract
    {
        public string Hash;
        public string Script;
        public List<Parameter> Parameters = new List<Parameter>();
        public string Returntype;
        public string NeedStorage;
        public string Name;
        public string Version;
        public string Author;
        public string Email;
        public string Description;
        public static Contract FromJson(JToken json)
        {
            var result = new Contract()
            {
                Hash = (string)json["hash"],
                Script = (string)json["script"],
                Returntype = (string)json["returntype"],
                NeedStorage = (string)json["needstorage"],
                Name = (string)json["name"],
                Version = (string)json["version"],
                Author = (string)json["author"],
                Email = (string)json["email"],
                Description = (string)json["description"]
            };
            foreach (var item in json["parameters"])
            {
                result.Parameters.Add(Parameter.FromJson(item));
            }
            return result;
        }
    }
}
