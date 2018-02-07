using MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB
{
    class Program
    {
        /// <summary>
        /// true:控制台显示详细信息
        /// false:控制台显示精简信息
        /// </summary>
        static bool showDetails = false;

        static void Main(string[] args)
        {
            Console.WriteLine("正在初始化数据库");
            Initialize();

            while (true)
            {
                Console.WriteLine("eg:>insert 0");
                Console.WriteLine("eg:>sync");
                Console.WriteLine("eg:>sync 10888");
                Console.Write(">");
                var input = Console.ReadLine();
                var command = input.Split(' ')[0];
                if (command == "sync")
                {
                    Sync();
                }
                if (command == "fix")
                {
                    if (input.Split(' ').Length > 2 &&
                        uint.TryParse(input.Split(' ')[1], out uint start) &&
                        uint.TryParse(input.Split(' ')[2], out uint end))
                    {
                        Fix(start, end);
                    }
                    else if (input.Split(' ').Length > 2 &&
                        uint.TryParse(input.Split(' ')[1], out uint start1))
                    {
                        Fix(start1, MongoDB<Block>.Count());
                    }
                }
                if (command == "insert")
                {
                    if (input.Split(' ').Length > 1)
                    {
                        var parameter = input.Split(' ')[1];
                        var start = uint.TryParse(parameter, out uint result);
                        if (start)
                        {
                            Console.WriteLine($"存储区块 {result}");
                            Insert(result);
                        }
                    }
                }
                if (command == "address")
                {
                    if (input.Split(' ').Length > 1)
                    {
                        var parameter = input.Split(' ')[1];
                        Console.WriteLine($"正在查询 {parameter}");
                        QueryAddress(parameter);
                    }

                }

                Console.WriteLine("命令执行完毕");
            }
        }

        /// <summary>
        /// 查询地址信息
        /// </summary>
        /// <param name="parameter">地址</param>
        private static void QueryAddress(string parameter)
        {
            var address = MongoDB<Address>.FirstOrDefault(p => p.Id == parameter);
            List<Coin> coins = new List<Coin>();
            foreach (var item in address.Coins)
            {
                if (item.CoinState == CoinState.Confirmed)
                {
                    coins.Add(MongoDB<Coin>.FirstOrDefault(p => p.Id == item.Id));
                }
            }
        }

        /// <summary>
        /// 同步区块
        /// </summary>
        static void Sync()
        {
            var blockCount = MongoDB<Block>.Count();
            //从已同步的前一个区块开始同步，避免程序强行关闭导致数据存储不全。
            blockCount = blockCount > 0 ? blockCount - 1 : blockCount;
            var end = Tools.GetBlockIndex();
            for (uint i = blockCount; i < end; i++)
            {
#if !DEBUG
                try { Insert(i, false); } catch (Exception e) { Console.WriteLine(e); Console.ReadLine(); };
#endif
#if DEBUG
                Insert(i);
#endif
                //Console.Clear();
                if (showDetails)
                {
                    Console.WriteLine($"{i}/{end}");
                }
                while (end - i < 1)
                {
                    Thread.Sleep(1000);
                    end = Tools.GetBlockIndex();
                    Console.Write(".");
                }
            }
        }

        /// <summary>
        /// 修复模式
        /// </summary>
        /// <param name="start">开始高度</param>
        /// <param name="end">截止高度</param>
        static void Fix(uint start, uint end)
        {
            end = Math.Min(end, MongoDB<Block>.Count());
            for (uint i = start; i < end; i++)
            {
#if !DEBUG
                try { Insert(i); } catch (Exception e) { Console.WriteLine(e); Console.ReadLine(); };
#endif
#if DEBUG
                Insert(i);
#endif
                //Console.Clear();
                Console.WriteLine($"{i}/{end}");
            }
        }

        static void Initialize()
        {
            MongoDB<Asset>.Initialize("Asset");
            MongoDB<Address>.Initialize("Address", new string[] { "FirstTimeStamp", "LastTimeStamp", "AssetId", "CoinState" });
            MongoDB<Block>.Initialize("Block", new string[] { "Index", "Timestamp", "TxCount" });
            MongoDB<Coin>.Initialize("Coin", new string[] { "CoinState" });
            MongoDB<MinerTransaction>.Initialize("MinerTransaction");
            MongoDB<Transaction>.Initialize("Transaction", new string[] { "BlockIndex", "Timestamp" });
        }

        static void Insert(uint index)
        {
            //存储区块
            var json = Tools.GetBlock(index);
            for (int i = 0; i < 3 && json == null; i++)
                json = Tools.GetBlock(index);

            var block = Block.FromJson(json);
            if (!showDetails)
            {
                //开始记录区块
                Console.Write($"Block {block.Index}");
                var blockComplexity = 1;
                block.Transactions.ForEach(p => blockComplexity += ((p as ClaimTransaction)?.Claims.Count * 2 ?? 0 + p.Inputs?.Count * 2 ?? 0 + p.Outputs?.Count * 2 ?? 0) + 1);
                //区块复杂数（预计写入数据多少次）
                Console.WriteLine($"/{blockComplexity}");
            }
            try
            {
                MongoDB<Block>.Insert(block);
            }
            catch (Exception) { Console.WriteLine($"Block {index} 已存在"); }

            foreach (var tx in block.Transactions)
            {
                //存储共识交易
                if (tx.Type == TransactionType.MinerTransaction)
                {
                    try
                    {
                        MongoDB<MinerTransaction>.Insert(tx as MinerTransaction);
                        //Console.WriteLine($"MinerTransaction {tx.Hash} 已创建");
                    }
                    catch (Exception) { Console.WriteLine($"MinerTransaction {tx.Hash} 已存在"); }
                }
                //存储注册资产交易
                else if (tx.Type == TransactionType.RegisterTransaction)
                {
                    var asset = (tx as RegisterTransaction).Asset;
                    InsertAsset(block, tx, asset);
                }
                else if (tx.Type == TransactionType.InvocationTransaction)
                {
                    //判断是否是通过智能合约注册的全局资产，如果是的话要记录在Asset集合中
                    try
                    {
                        var asset = Asset.FromJson(Tools.GetAssetState(tx.Hash));
                        InsertAsset(block, tx, asset);
                    }
                    catch (Exception)
                    {
                        //不是注册资产的智能合约
                    }
                }
                //修改资产的发行列表，修改资产管理员的交易列表
                else if (tx.Type == TransactionType.IssueTransaction)
                {
                    foreach (var output in tx.Outputs)
                    {
                        var asset = MongoDB<Asset>.FirstOrDefault(p => p.Id == output.AssetId);
                        if (asset == null)
                        {
                            Console.WriteLine($"分发资产时资产引用异常 {asset.Id}");
                        }
                        MongoDB<Asset>.UpdateItem(p => p.Id == output.AssetId, p => p.Transactions, asset.Append(tx));
                        var address = MongoDB<Address>.FirstOrDefault(p => p.Id == asset.Admin);
                        MongoDB<Address>.UpdateItem(p => p.Id == output.AssetId, p => p.Transactions, address.Append(tx));
                        MongoDB<Address>.UpdateItem(p => p.Id == output.AssetId, p => p.LastTimeStamp, block.Timestamp);

                        Console.WriteLine($"IssueTransaction Address {asset.Id} 已修改");
                    }
                }
                else if (tx.Type == TransactionType.ClaimTransaction)
                {
                    var ctx = tx as ClaimTransaction;
                    foreach (var claim in ctx.Claims)
                    {
                        //修改Coin的状态为已提取
                        var coin = MongoDB<Coin>.FirstOrDefault(p => p.Id == claim.Id);
                        if (coin == null)
                        {
                            Console.WriteLine($"交易输入引用异常 {claim.Id}");
                        }

                        MongoDB<Coin>.UpdateItem(p => p.Id == claim.Id, p => p.CoinState, coin.CoinState | CoinState.Claimed);
                        if (showDetails)
                        {
                            Console.WriteLine($"ClaimTransaction Coin {claim.Id} 已修改");
                        }

                        //修改地址的资产余额及交易记录
                        //在提取小蚁币的交易中，涉及的地址不只是在Inputs和Outputs中，还有Claims字段
                        var address = MongoDB<Address>.FirstOrDefault(p => p.Id == coin.Address);
                        if (address == null)
                        {
                            Console.WriteLine($"提取小蚁币时地址引用异常 {claim.Id}");
                        }
                        else
                        {
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Coins, address.Append(coin));
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Transactions, address.Append(tx));
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.LastTimeStamp, block.Timestamp);
                            if (showDetails)
                            {
                                Console.WriteLine($"ClaimTransaction Address {address.Id} 已修改");
                            }
                        }
                    }
                }
                //存储所有非共识交易
                if (tx.Type != TransactionType.MinerTransaction)
                {
                    try
                    {
                        MongoDB<Transaction>.Insert(tx);
                        if (showDetails)
                        {
                            Console.WriteLine($"Transaction {tx.Hash} 已创建");
                        }
                    }
                    catch (Exception) { Console.WriteLine($"Transaction {tx.Hash} 已存在"); }
                }
                //遍历交易输出，创建/修改Address及Coin
                if (tx.Outputs != null)
                {
                    foreach (var output in tx.Outputs)
                    {
                        //新建Coin
                        var asset = MongoDB<Asset>.FirstOrDefault(p => p.Id == output.AssetId);
                        if (asset == null)
                        {
                            Console.WriteLine($"交易输出中资产引用异常 {asset.Id}");
                        }
                        var coin = new Coin()
                        {
                            Address = output.Address,
                            AssetId = output.AssetId,
                            AssetName = asset.Name,
                            AssetPrecision = asset.Precision,
                            Index = output.Index,
                            TxId = tx.Hash,
                            Value = output.Value,
                            CoinState = CoinState.Confirmed
                        };
                        try
                        {
                            MongoDB<Coin>.Insert(coin);
                            if (showDetails)
                            {
                                Console.WriteLine($"Outputs Coin {coin.Id} 已创建");
                            }
                        }
                        catch (Exception) { Console.WriteLine($"Outputs Coin {coin.Id} 已存在"); }
                        var address = MongoDB<Address>.FirstOrDefault(p => p.Id == output.Address);
                        //地址首次创建
                        if (address == null)
                        {
                            address = new Address()
                            {
                                Id = output.Address,
                                FirstTimeStamp = block.Timestamp,
                                LastTimeStamp = block.Timestamp,
                            };

                            address.Append(tx);
                            address.Append(coin);
                            address.LastTimeStamp = block.Timestamp;
                            try
                            {
                                MongoDB<Address>.Insert(address);
                                if (showDetails)
                                {
                                    Console.WriteLine($"Outputs Address {output.Address} 已创建");
                                }
                            }
                            catch (Exception) { Console.WriteLine($"Outputs Address {output.Address} 已存在"); }
                        }
                        else
                        {
                            //修改地址的资产余额及交易记录
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Coins, address.Append(coin));
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Transactions, address.Append(tx));
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.LastTimeStamp, block.Timestamp);
                            if (showDetails)
                            {
                                Console.WriteLine($"Outputs Address {output.Address} 已修改");
                            }
                        }
                    }
                }
                //遍历交易输入，修改Address及Coin
                if (tx.Inputs != null)
                {
                    foreach (var input in tx.Inputs)
                    {
                        //修改Coin的去向和状态
                        var coin = MongoDB<Coin>.FirstOrDefault(p => p.Id == input.Id);
                        if (coin == null)
                        {
                            Console.WriteLine($"交易输入引用异常 {input.Id}");
                        }
                        MongoDB<Coin>.UpdateItem(p => p.Id == input.Id, p => p.CoinState, CoinState.Spent);
                        MongoDB<Coin>.UpdateItem(p => p.Id == input.Id, p => p.TracedHash, tx.Hash);
                        if (showDetails)
                        {
                            Console.WriteLine($"Inputs Coin {coin.Id} 已修改");
                        }

                        //修改地址的Coin状态及交易记录
                        var address = MongoDB<Address>.FirstOrDefault(p => p.Id == coin.Address);
                        if (address == null)
                        {
                            Console.WriteLine($"交易输入中修改地址异常 {input.Id}");
                        }
                        else
                        {
                            address.Coins.FirstOrDefault(c => c.Id == coin.Id).CoinState = CoinState.Spent;
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Transactions, address.Append(tx));
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.Coins, address.Coins);
                            MongoDB<Address>.UpdateItem(p => p.Id == coin.Address, p => p.LastTimeStamp, block.Timestamp);
                            if (showDetails)
                            {
                                Console.WriteLine($"Inputs Address {address.Id} 已修改");
                            }
                        }
                    }
                }
            }
        }

        static void InsertAsset(Block block, Transaction tx, Asset asset)
        {
            try
            {
                MongoDB<Asset>.Insert(asset);
                Console.WriteLine($"InvocationTransaction Asset {asset.Id} 已创建");
            }
            catch (Exception) { Console.WriteLine($"Asset {asset.Id} 已存在"); }

            //修改地址的资产余额及交易记录
            //在注册资产的交易中，涉及的地址不只是在Inputs和Outputs中，还有Admin字段
            var address = MongoDB<Address>.FirstOrDefault(p => p.Id == asset.Admin);
            if (address == null)
            {
                address = new Address()
                {
                    Id = asset.Admin,
                    FirstTimeStamp = block.Timestamp,
                    LastTimeStamp = block.Timestamp,
                };
                address.Append(tx);
                try
                {
                    MongoDB<Address>.Insert(address);
                    Console.WriteLine($"RegisterTransaction Address {address.Id} 已创建");
                }
                catch (Exception) { Console.WriteLine($"Address {address} 已存在"); }
            }
            else
            {
                MongoDB<Address>.UpdateItem(p => p.Id == asset.Admin, p => p.Transactions, address.Append(tx));
                MongoDB<Address>.UpdateItem(p => p.Id == asset.Admin, p => p.LastTimeStamp, block.Timestamp);
            }
        }
    }
}
