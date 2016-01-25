using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyNetQ.RPC.Extensions.Test
{
    [TestClass]
    public class RabbitQueueConnectionTests
    {
        public const string ConnectionString = "host=utv-hv-service;username=akv;password=akv;virtualHost=rabbitCommunicationTest";
        public const string BrokerId = "akv.test";
        public const string Topic = "akv.Communications.Test";
        public ClientMock Client { get; set; }
        public IClientTest ClientProxy { get; set; }
        public RabbitBroadcasterPublisher<IClientTest> RabbitServer { get; set; }
        public RabbitBroadcasterSubscriber<IClientTest> RabbitClient { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Client = new ClientMock();

            RabbitServer = new RabbitBroadcasterPublisher<IClientTest>(ConnectionString, BrokerId, Topic);
            RabbitClient = new RabbitBroadcasterSubscriber<IClientTest>(ConnectionString, BrokerId, Client, Topic);
            ClientProxy = RabbitServer.GetClientProxy();

            RabbitServer.Connect();
            RabbitClient.Connect();
        }

        [TestCleanup]
        public void TearDown()
        {
            RabbitClient.Close();
            RabbitServer.Close();
        }

        [TestMethod]
        public void ConnectToRabbit()
        {
            //ASSERT
            Assert.IsTrue(RabbitServer.IsConnected);
            Assert.IsTrue(RabbitClient.IsConnected);
        }

        [TestMethod]
        public void SendServerMessageA()
        {
            ClientProxy.ClientMethodA();
            Client.WaitForMethodCall();

            Assert.IsTrue(Client.MethodCalls.Any());
            Assert.AreEqual(Client.MethodCalls[0].MethodName, nameof(ClientMock.ClientMethodA));
        }

        [TestMethod]
        public void SendServerMessageB()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            ClientProxy.ClientMethodB("test", 255, int.MaxValue, long.MaxValue, 1.5, 1.5f, dateTimeOffset.DateTime, dateTimeOffset, new List<string>() { "foo", "bar" }, Tuple.Create("hello", "world"));
            Client.WaitForMethodCall();

            Assert.IsTrue(Client.MethodCalls.Any());
            Assert.AreEqual(Client.MethodCalls[0].MethodName, nameof(ClientMock.ClientMethodB));

            var args = Client.MethodCalls[0].Args;

            AssertMethodB(args, dateTimeOffset);
        }

        [TestMethod]
        public void PerformanceTest()
        {
            int count = 100;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                ClientProxy.ClientMethodA();
            }
            sw.Stop();
            for (int i = 0; i < 5000; i++) // 5 sec
            {
                Thread.Sleep(1);
                if(Client.MethodCalls.Count == count)
                    break;
            }
            Assert.AreEqual(Client.MethodCalls.Count, count);
            Console.WriteLine(sw.Elapsed);
        }

        private static void AssertMethodB(object[] args, DateTimeOffset dateTimeOffset)
        {
            Assert.AreEqual(args[0], "test");
            Assert.AreEqual((byte)255, args[1]);
            Assert.AreEqual(args[2], int.MaxValue);
            Assert.AreEqual(args[3], long.MaxValue);
            Assert.AreEqual(args[4], 1.5);
            Assert.AreEqual(args[5], 1.5f);
            Assert.AreEqual(args[6], dateTimeOffset.DateTime);
            Assert.AreEqual(args[7], dateTimeOffset);
            Assert.IsInstanceOfType(args[8], typeof(List<string>));
            var list = args[8] as List<string>;

            Assert.AreEqual(list[0], "foo");
            Assert.AreEqual(list[1], "bar");
            Assert.IsInstanceOfType(args[9], typeof(Tuple<string, string>));
            var tuple = args[9] as Tuple<string, string>;
            Assert.AreEqual(tuple.Item1, "hello");
            Assert.AreEqual(tuple.Item2, "world");
        }
    }
}
