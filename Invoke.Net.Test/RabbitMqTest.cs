using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Invoke;

namespace Invoke.Net.Test
{
    /// <summary>
    /// RabbitMqTest 的摘要说明
    /// </summary>
    [TestClass]
    public class RabbitMqTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //JRabbitMq.Send("hello,test");

            var ss = new JRabbitMq();
            ss.OnReceived += Queue_OnReceived;
            ss.Receive();
            Assert.IsTrue(1 == 1);
        }

        private void Queue_OnReceived(object obj, JRabbitMq.ReceivedEventArgs e)
        {
            var result = e;
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsTrue(1 == 1);
        }

    }
}





