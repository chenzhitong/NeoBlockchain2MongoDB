using Newtonsoft.Json.Linq;

namespace MongoDB.Models
{
    public class CoinReference
    {
        public string PrevHash;

        public int PrevIndex;
        
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
