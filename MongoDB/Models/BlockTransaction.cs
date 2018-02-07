using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class BlockTransaction
    {
        public string Hash;

        public static BlockTransaction FromJson(JToken json, Transaction tx) => new BlockTransaction
        {
            Hash = (string)json["txid"]
        };
    }
}
