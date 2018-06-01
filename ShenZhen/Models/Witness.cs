using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenZhen.Models
{
    public class Witness
    {
        public string InvocationScript;

        public string VerificationScript;

        public static Witness FromJson(JToken json) => new Witness()
        {
            InvocationScript = (string)json["invocation"],
            VerificationScript = (string)json["verification"],
        };
    }
}
