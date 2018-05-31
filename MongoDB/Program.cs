using MongoDB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MongoDB
{
    class Program
    {
        static List<UTXO> utxoList = new List<UTXO>();
        static string NEO = "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";
        static uint BlockIndex = 0;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("> eg: 1001");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int index))
                {
                    //查询数据
                    if (BlockIndex < index)
                    {
                        var end = Tools.GetBlockIndex();
                        for (; BlockIndex < index; BlockIndex++)
                        {
                            ProcessBlock(BlockIndex);
                        }
                    }
                    var result = utxoList.GroupBy(p => p.Address).Select(p => new Result { Address = p.FirstOrDefault().Address, Value = p.Sum(q => q.Value) });
                    Console.WriteLine();
                    //输出结果
                    var sb = new StringBuilder();
                    foreach (var item in result)
                    {
                        sb.Append($"{item.Address},{ item.Value}\r\n");
                        Console.WriteLine(item.Address + "\t" + item.Value);
                    }
                    File.Delete(index + ".csv");
                    File.WriteAllText(index + ".csv", sb.ToString());
                    Console.WriteLine($"output {index}.csv");
                }
            }
        }

        static void ProcessBlock(uint index)
        {
            var block = Block.FromJson(Tools.GetBlock(index));
            Console.Write(block.Index + "\t");
            if (block.Transactions.Count <= 1) return;
            foreach (var tx in block.Transactions)
            {
                if (tx.Outputs == null) continue;
                //添加UTXO
                foreach (var output in tx.Outputs)
                {
                    if (output.AssetId != NEO) continue;
                    utxoList.Add(new UTXO()
                    {
                        Address = output.Address,
                        TxId = tx.Hash,
                        Index = output.Index,
                        Value = output.Value
                    });
                }
                //移除UTXO
                if (tx.Inputs == null) continue;
                foreach (var input in tx.Inputs)
                {
                    utxoList.RemoveAll(p => p.TxId == input.PrevHash && p.Index == input.PrevIndex);
                }
            }
        }
    }
}
