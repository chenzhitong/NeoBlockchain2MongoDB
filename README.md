
MinerTransaction# NeoBlockchain2MongoDB

简介：

该程序可以将 NeoBlockchain 中的数据转到 MongoDB 中，本程序可以当做区块链浏览器的入库程序。

使用方法：

1、安装 MongoDB [下载链接](https://www.mongodb.com/download-center#community) [使用说明](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/)

2、安装 neo-cli，并启动 rpc 服务 [安装说明](http://docs.neo.org/zh-cn/node/setup.html)  [命令参考](http://docs.neo.org/zh-cn/node/cli.html)

3、在 App.config 配置 MongoDB 链接字符串和 neo-cli 的 rpc 请求地址

4、运行 NeoBlockchain2MongoDB 程序

注：本程序在 2017 年 7 月时可以稳定工作，之后进行了少量更新，但未经完整测试，不确定是否与最新版的 neo-cli 兼容。

截图：

Address
![Address](Sreenshot/Address.png)

Asset
![Asset](Sreenshot/Asset.png)

Block
![Block](Sreenshot/Block.png)

Coin
![Coin](Sreenshot/Coin.png)

MinerTransaction
![MinerTransaction](Sreenshot/MinerTransaction.png)

Transaction
![Transaction](Sreenshot/Transaction.png)
