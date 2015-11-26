using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Invoke
{
    public  class JRabbitMq
    {
        static string RabbitMqUserName { get; set; }
        static string RabbitMqPwd { get; set; }
        static string RabbitMqServerIp { get; set; }
        static string RabbitMqServerPort { get; set; }
        static string RabbitMqQueueName { get; set; }
        static string RabbitMqExchangeName { get; set; }
        static string RabbitMqExchangeType { get; set; }
        static string RabbitMqRoutingKey { get; set; }

        public  event ReceivedEventHandler OnReceived;
        static JRabbitMq()
        {
            RabbitMqUserName = ConfigurationManager.AppSettings["RabbitMq_UserName"];
            RabbitMqPwd = ConfigurationManager.AppSettings["RabbitMq_Pwd"];
            RabbitMqServerIp = ConfigurationManager.AppSettings["RabbitMq_ServerIp"];
            RabbitMqServerPort = ConfigurationManager.AppSettings["RabbitMq_ServerPort"];
            RabbitMqQueueName = ConfigurationManager.AppSettings["RabbitMq_QueueName"];
            RabbitMqExchangeName = ConfigurationManager.AppSettings["RabbitMq_ExchangeName"];
            RabbitMqExchangeType = ConfigurationManager.AppSettings["RabbitMq_ExchangeType"];
            RabbitMqRoutingKey = ConfigurationManager.AppSettings["RabbitMq_RoutingKey"];
        }

        public static bool Send(string msg)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = RabbitMqUserName,
                    Password = RabbitMqPwd,
                    RequestedHeartbeat = 0,
                    Endpoint = new AmqpTcpEndpoint(new Uri("amqp://" + RabbitMqServerIp + ":" + RabbitMqServerPort + "/"))
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(RabbitMqExchangeName, RabbitMqExchangeType);
                        channel.QueueDeclare(RabbitMqQueueName, true, false, false, null);
                        channel.QueueBind(RabbitMqQueueName, RabbitMqExchangeName, RabbitMqRoutingKey);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(RabbitMqExchangeName, RabbitMqRoutingKey, properties, body);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Receive()
        {
            var factory = new ConnectionFactory{
                UserName = RabbitMqUserName,
                Password = RabbitMqPwd,
                RequestedHeartbeat = 0,
                Endpoint = new AmqpTcpEndpoint(new Uri("amqp://" + RabbitMqServerIp + ":" + RabbitMqServerPort + "/"))
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(RabbitMqQueueName, true, false, false, null);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(RabbitMqQueueName, false, consumer);
                channel.BasicQos(0U, 1, false);
                while (true)
                {
                    var deliverEventArgs = consumer.Queue.Dequeue();
                    if (deliverEventArgs == null) continue;
                    var body = deliverEventArgs.Body;
                    if (body == null) continue;
                    var @data = Encoding.UTF8.GetString(body);
                    try
                    {
                        if (OnReceived == null) continue;
                        OnReceived(this, new ReceivedEventArgs { Code = 0, Data = @data });
                        channel.BasicAck(deliverEventArgs.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        if (OnReceived == null) continue;
                        OnReceived(this, new ReceivedEventArgs { Code = -1, Data = ex.Message });
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public delegate void ReceivedEventHandler(object sender, ReceivedEventArgs args);

        public class ReceivedEventArgs : EventArgs
        {
            public int Code { get; set; }
            public string Data { get; set; }
        }

    }
}
