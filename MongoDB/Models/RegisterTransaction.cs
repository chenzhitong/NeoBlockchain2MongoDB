using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class RegisterTransaction : Transaction
    {
        public Asset Asset;

        public new static Transaction FromJson(JToken json)
        {
            var transaction = new RegisterTransaction()
            {
                Asset = Asset.FromJson(json["asset"])
            };
            transaction.Asset.Id = transaction.Hash;
            FillBase(json, transaction);
            return transaction;
        }
    }


}
