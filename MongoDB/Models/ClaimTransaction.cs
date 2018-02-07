using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class ClaimTransaction : Transaction
    {
        public List<CoinReference> Claims = new List<CoinReference>();

        public new static Transaction FromJson(JToken json)
        {
            var tx = new ClaimTransaction();
            var claimCount = json["claims"].Count();
            for (int i = 0; i < claimCount; i++)
            {
                tx.Claims.Add(CoinReference.FromJson(json["claims"][i]));
            }
            FillBase(json, tx);
            return tx;
        }
    }
}
