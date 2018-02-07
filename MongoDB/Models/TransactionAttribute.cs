using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class TransactionAttribute
    {
        /// <summary>
        /// 用途
        /// </summary>
        public TransactionAttributeUsage Usage;
        /// <summary>
        /// 特定于用途的外部数据
        /// </summary>
        public string Data;

        public static TransactionAttribute FromJson(JToken json) => new TransactionAttribute()
        {
            Usage = (TransactionAttributeUsage)Enum.Parse(typeof(TransactionAttributeUsage), (string)json["usage"]),
            Data = (string)json["data"]
        };
    }
}
