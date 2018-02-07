using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class EnrollmentTransaction : Transaction
    {
        public string PublicKey;

        public new static Transaction FromJson(JToken json)
        {
            var transaction = new EnrollmentTransaction()
            {
                PublicKey = (string)json["pubkey"]
            };
            FillBase(json, transaction);
            return transaction;
        }
    }
}
