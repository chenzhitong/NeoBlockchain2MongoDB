using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Models
{
    public class Asset
    {
        public string Id;

        public AssetType AssetType;

        public string Name;

        public double Amount;

        public byte Precision;

        public string Owner;

        public string Admin;

        public List<AssetTransaction> Transactions;

        public static Asset FromJson(JToken json)
        {
            return new Asset()
            {
                AssetType = (AssetType)Enum.Parse(typeof(AssetType), (string)json["type"]),
                Name = json["name"].ToString().Replace("\r\n", "").Replace(" ", ""),
                Amount = (double)json["amount"],
                Precision = (byte)json["precision"],
                Owner = (string)json["owner"],
                Admin = (string)json["admin"]
            };
        }

        public List<AssetTransaction> Append(Transaction tx)
        {
            Transactions = Transactions ?? new List<AssetTransaction>();
            var current = Transactions.FirstOrDefault(p => p.Hash == tx.Hash);
            if (current == null)
            {
                Transactions.Add(new AssetTransaction() { Hash = tx.Hash, BlockIndex = tx.BlockIndex });
            }
            else
            {
                current.BlockIndex = tx.BlockIndex;
            }
            return Transactions;
        }
    }
}
