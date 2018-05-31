using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
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
