using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class Coin : TransactionOutput
    {
        [BsonId]
        public string Id
        {
            get { return $"{TxId}_{Index}"; }
        }

        public string TxId;

        public string AssetName;

        public byte AssetPrecision;

        [BsonIgnoreIfNull]
        public string TracedHash;

        public CoinState CoinState;
    }
}
