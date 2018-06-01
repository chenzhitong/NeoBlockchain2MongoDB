using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen.Models
{
    public class PublishTransaction : Transaction
    {
        public Contract Contract;

        public new static Transaction FromJson(JToken json)
        {
            var transaction = new PublishTransaction()
            {
                Contract = Contract.FromJson(json["contract"])
            };
            FillBase(json, transaction);
            return transaction;
        }
    }
}
