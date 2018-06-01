using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen.Models
{
    public class MinerTransaction : Transaction
    {
        public ulong Nonce;

        public new static Transaction FromJson(JToken json)
        {
            var transaction = new MinerTransaction()
            {
                Nonce = (ulong)json["nonce"]
            };
            FillBase(json, transaction);
            return transaction;
        }
    }
}
