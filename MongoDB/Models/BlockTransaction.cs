using Newtonsoft.Json.Linq;

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
