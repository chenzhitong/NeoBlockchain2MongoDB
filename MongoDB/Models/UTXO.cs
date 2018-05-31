namespace MongoDB.Models
{
    public class UTXO
    {
        public string Address;

        public string TxId;

        public int Index;

        public double Value;
    }

    public class Result
    {
        public string Address;
        public double Value;
    }
}