using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MongoDB.Models
{
    public class AddressCoin
    {
        public string Id;

        public string AssetId;

        public CoinState CoinState;
    }
}