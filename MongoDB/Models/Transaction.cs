using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class Transaction
    {
        [BsonId]
        public string Hash;

        public int Size;

        public TransactionType Type;

        public byte Version;

        [BsonIgnoreIfNull]
        public List<TransactionAttribute> Attributes;

        [BsonIgnoreIfNull]
        public List<CoinReference> Inputs;

        [BsonIgnoreIfNull]
        public List<TransactionOutput> Outputs;

        public long SystemFee;

        public long NetworkFee;
        
        public string BlockHash;

        public int BlockIndex;

        public int Timestamp;

        [BsonIgnore]
        public List<Witness> Scripts = new List<Witness>();

        public static Transaction FromJson(JToken json)
        {
            TransactionType txType = (TransactionType)Enum.Parse(typeof(TransactionType), (string)json["type"]);
            if (txType == TransactionType.MinerTransaction)
                return MinerTransaction.FromJson(json);
            if (txType == TransactionType.ClaimTransaction)
                return ClaimTransaction.FromJson(json);
            if (txType == TransactionType.ContractTransaction)
                return ContractTransaction.FromJson(json);
            if (txType == TransactionType.EnrollmentTransaction)
                return EnrollmentTransaction.FromJson(json);
            if (txType == TransactionType.InvocationTransaction)
                return InvocationTransaction.FromJson(json);
            if (txType == TransactionType.IssueTransaction)
                return IssueTransaction.FromJson(json);
            if (txType == TransactionType.PublishTransaction)
                return PublishTransaction.FromJson(json);
            if (txType == TransactionType.RegisterTransaction)
                return RegisterTransaction.FromJson(json);
            return null;
        }

        public static void FillBase(JToken json, Transaction tx)
        {
            tx.Hash = (string)json["txid"];
            tx.Size = (int)json["size"];
            tx.Type = (TransactionType)Enum.Parse(typeof(TransactionType), (string)json["type"]);
            tx.Version = (byte)json["version"];
            tx.SystemFee = (int)json["sys_fee"];
            tx.NetworkFee = (int)json["net_fee"];
            if (json["attributes"].Count() > 0)
            {
                tx.Attributes = new List<TransactionAttribute>();
                foreach (var item in json["attributes"])
                    tx.Attributes.Add(TransactionAttribute.FromJson(item));
            }
            if (json["vin"].Count() > 0)
            {
                tx.Inputs = new List<CoinReference>();
                foreach (var item in json["vin"])
                    tx.Inputs.Add(CoinReference.FromJson(item));
            }
            if (json["vout"].Count() > 0)
            {
                tx.Outputs = new List<TransactionOutput>();
                foreach (var item in json["vout"])
                    tx.Outputs.Add(TransactionOutput.FromJson(item));
            }
            
            foreach (var item in json["scripts"])
                tx.Scripts.Add(Witness.FromJson(item));
        }
    }
}
