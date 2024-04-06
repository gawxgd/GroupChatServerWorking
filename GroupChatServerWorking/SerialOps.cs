using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace GroupChatServerWorking
{
    public class SerialOps
    {
        public string SerializeToJson(Authorization auth)
        {
            return JsonConvert.SerializeObject(auth);
        }
        public string SerializeToJson(Message auth)
        {
            return JsonConvert.SerializeObject(auth);
        }

        // Deserialize JSON to object
        public static Authorization DeserializeFromJsonAuth(string json)
        {
            return JsonConvert.DeserializeObject<Authorization>(json);
        }
        public static Message DeserializeFromJsonMes(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }
    }
}
