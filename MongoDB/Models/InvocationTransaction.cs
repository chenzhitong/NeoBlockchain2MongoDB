using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen.Models
{
    public class InvocationTransaction : Transaction
    {
        public string Script;

        public long Gas;

        public new static Transaction FromJson(JToken json)
        {
            var transaction = new InvocationTransaction()
            {
                Script = (string)json["script"],
                Gas = (int)json["gas"]
            };
            FillBase(json, transaction);
            return transaction;
        }
    }
}
