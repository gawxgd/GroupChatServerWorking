using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace GroupChatServerWorking
{
    public static class SerialOps
    {
        public static string SerializeToJson(Messages.Message auth)
        {
            return JsonConvert.SerializeObject(auth);
        }
        public static string SerializeToJson(Messages.Authorization auth)
        {
            return JsonConvert.SerializeObject(auth);
        }

        // Deserialize JSON to object
        public static Messages.Authorization DeserializeFromJsonAuth(string json)
        {
            return JsonConvert.DeserializeObject<Messages.Authorization>(json);
        }
        public static Messages.Message DeserializeFromJsonMes(string json)
        {
            return JsonConvert.DeserializeObject<Messages.Message>(json);
        }
    }
}
