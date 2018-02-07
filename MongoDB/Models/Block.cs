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
    public class Block
    {
        [BsonId]
        public string Hash;

        public int Size;

        public byte Version;

        public string PreviousBlockHash;

        public string MerkleRoot;

        public int Timestamp;

        public int Index;

        public string ConsensusData;

        public string NextConsensus;

        public string NextBlockHash;

        public List<string> TxHashs = new List<string>();

        public int TxCount;

        [BsonIgnore]
        public Witness Script;

        [BsonIgnore]
        public int Confirmations;

        [BsonIgnore]
        public List<Transaction> Transactions = new List<Transaction>();

        public static Block FromJson(JToken json)
        {
            var block = new Block()
            {
                Hash = (string)json["hash"],
                Size = (int)json["size"],
                Version = (byte)json["version"],
                PreviousBlockHash = (string)json["previousblockhash"],
                MerkleRoot = (string)json["merkleroot"],
                Timestamp = (int)json["time"],
                Index = (int)json["index"],
                ConsensusData = (string)json["nonce"],
                NextConsensus = (string)json["nextconsensus"],
                Script = Witness.FromJson(json["script"]),
                Confirmations = (int)json["confirmations"],
                NextBlockHash = (string)json["nextblockhash"],
                TxCount = json["tx"].Count()
            };

            foreach (var item in json["tx"])
            {
                var tx = Transaction.FromJson(item);
                tx.BlockHash = block.Hash;
                tx.BlockIndex = block.Index;
                tx.Timestamp = block.Timestamp;
                block.Transactions.Add(tx);
                block.TxHashs.Add(tx.Hash);
            }
            return block;
        }
    }

}
