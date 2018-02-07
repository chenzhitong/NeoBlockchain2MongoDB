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
    public class CoinReference
    {
        [BsonId]
        public string PrevHash;

        public int PrevIndex;

        [BsonIgnore]
        public string Id
        {
            get { return $"{PrevHash}_{PrevIndex}"; }
        }

        public static CoinReference FromJson(JToken json) => new CoinReference()
        {
            PrevHash = (string)json["txid"],
            PrevIndex = (int)json["vout"]
        };
    }
}
