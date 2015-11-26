using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redis.Invoke;
using Invoke.Net.Test.Entity;

namespace Invoke.Net.Test
{
    [TestClass]
    public class RedisTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var people = new People(){
                Id=1,
                Age=12,Height=160,Name="jerry",AddTime=DateTime.Now
            };
            //var setRes = JRedis.Set("name", people);

            var res = JRedis.Get<People>("name");

           JRedis.RedisClient.AddItemToSet("ss", "sdfas");


            Assert.IsTrue(1==1);
        }
    }
}
