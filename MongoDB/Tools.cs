using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen
{
    public static class Tools
    {
        private static string neo_cli = ConfigurationManager.AppSettings["neo_cli"];

        public static string HttpPost(string Url, string postData, int timeOut = 5000)
        {
            WebRequest request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            request.Timeout = timeOut;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public static JToken GetBlock(uint index)
        {
            string json;
            try
            {
                json = HttpPost(neo_cli,
                    $"{{'jsonrpc': '2.0', 'method': 'getblock', 'params': [{index}, 1], 'id': 1}}");
                return JObject.Parse(json)["result"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JToken GetAssetState(string hash)
        {
            string json;
            try
            {
                json = HttpPost(neo_cli,
                    $"{{'jsonrpc': '2.0', 'method': 'getassetstate', 'params': [{hash}], 'id': 1}}");
                return JObject.Parse(json)["result"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int GetBlockCount()
        {
            Console.Write("正在获取节点区块数量 ");
            string json;
            try
            {
                json = HttpPost(neo_cli,
                    $"{{'jsonrpc': '2.0', 'method': 'getblockcount', 'params': [], 'id': 1}}");
                var result = (int)JObject.Parse(json)["result"];
                Console.WriteLine(result);
                return result;
            }
            catch (Exception)
            {
                Console.WriteLine("NEO节点未启动");
                return 0;
            }
        }

        public static int GetBlockIndex() => GetBlockCount() - 1;
    }
}
