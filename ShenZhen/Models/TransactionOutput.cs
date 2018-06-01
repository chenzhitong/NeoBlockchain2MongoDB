using Newtonsoft.Json.Linq;

namespace ShenZhen.Models
{
    public class TransactionOutput
    {
        public int Index;

        public string AssetId;

        public double Value;

        public string Address;

        public static TransactionOutput FromJson(JToken json) => new TransactionOutput()
        {
            Index = (int)json["n"],
            AssetId = (string)json["asset"],
            Value = (double)json["value"],
            Address = (string)json["address"]
        };
    }
}
