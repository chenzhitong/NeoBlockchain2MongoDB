using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Models
{
    public class Address
    {
        [BsonId]
        public string Id;

        public List<AddressTransaction> Transactions;

        [BsonIgnoreIfNull]
        public List<AddressCoin> Coins;

        public int FirstTimeStamp;

        public int LastTimeStamp;

        public List<AddressTransaction> Append(Transaction tx)
        {
            Transactions = Transactions ?? new List<AddressTransaction>();
            var current = Transactions.FirstOrDefault(p => p.Hash == tx.Hash);
            if (current == null)
            {
                Transactions.Add(new AddressTransaction() { Hash = tx.Hash, BlockIndex = tx.BlockIndex });
            }
            else
            {
                current.BlockIndex = tx.BlockIndex;
            }
            return Transactions;
        }

        public List<AddressCoin> Append(Coin coin)
        {
            Coins = Coins ?? new List<AddressCoin>();
            var current = Coins.FirstOrDefault(p => p.Id == coin.Id);
            if (current == null)
            {
                Coins.Add(new AddressCoin()
                {
                    Id = coin.Id,
                    AssetId = coin.AssetId,
                    CoinState = coin.CoinState
                });
            }
            else
            {
                current.AssetId = coin.AssetId;
                current.CoinState = coin.CoinState;
            }
            return Coins;
        }
    }
}
