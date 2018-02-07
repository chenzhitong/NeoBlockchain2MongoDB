using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Models
{
    public class AddressTransaction
    {
        public string Hash;

        public int BlockIndex;
    }
}