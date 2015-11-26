using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace Redis.Invoke
{
    public static class JRedis
    {
        static string ServerIp { get; set; }
        static int ServerPort { get; set; }
        public static  RedisClient RedisClient{ get; set; }

        static JRedis()
        {
            ServerIp = ConfigurationManager.AppSettings["Redis_ServerIp"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["Redis_ServerPort"]);
            RedisClient=new RedisClient(ServerIp,ServerPort);
        }

        public static bool Set(string key,string value)
        {
            return RedisClient.Set(key,value);
        }

        public static string Get(string key)
        {
            return RedisClient.Get<string>(key);
        }

        public static bool Remove(string key)
        {
            return RedisClient.Remove(key);
        }

        public static bool Set<T>(string key, T value)
        {
            return RedisClient.Set(key, value);
        }

        public static T Get<T>(string name)
        {
            return RedisClient.Get<T>(name);
        }

    }
}
