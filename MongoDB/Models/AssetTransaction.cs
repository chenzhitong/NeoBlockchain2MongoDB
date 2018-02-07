using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Models
{
    public class AssetTransaction
    {
        public string Hash;

        public int BlockIndex;
    }
}