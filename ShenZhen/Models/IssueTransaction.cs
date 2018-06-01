using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen.Models
{
    public class IssueTransaction : Transaction
    {
        public new static Transaction FromJson(JToken json)
        {
            var transaction = new IssueTransaction();
            FillBase(json, transaction);
            return transaction;
        }
    }
}
