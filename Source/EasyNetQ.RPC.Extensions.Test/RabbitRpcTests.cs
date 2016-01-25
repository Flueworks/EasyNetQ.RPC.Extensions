using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyNetQ.RPC.Extensions.Test
{
    [TestClass]
    public class RabbitRpcConnectionTests
    {
        public const string ConnectionString = "host=utv-hv-service;username=akv;password=akv;virtualHost=rabbitCommunicationTest";
        public const string BrokerId = "akv.test";

        public ServerMock Server { get; set; }
        public IServerTest ServerProxy { get; set; }
        public RabbitRpcServer<IServerTest> RabbitRpcServer { get; set; }
        public RabbitRpcClient<IServerTest> RabbitRpcClient { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Server = new ServerMock();

            RabbitRpcServer = new RabbitRpcServer<IServerTest>(ConnectionString, BrokerId, Server);
            RabbitRpcClient = new RabbitRpcClient<IServerTest>(ConnectionString, BrokerId);
            ServerProxy = RabbitRpcClient.GetServerProxy();

            RabbitRpcServer.Connect();
            RabbitRpcClient.Connect();
        }

        [TestCleanup]
        public void TearDown()
        {
            RabbitRpcClient.Close();
            RabbitRpcServer.Close();
        }

        [TestMethod]
        public void ConnectToRabbit()
        {
            //ASSERT
            Assert.IsTrue(RabbitRpcServer.IsConnected);
            Assert.IsTrue(RabbitRpcClient.IsConnected);
        }

        [TestMethod]
        public void SendServerMessageA()
        {
            var result = ServerProxy.ServerMethodA();
            Assert.AreEqual(10, result);
            Assert.IsTrue(Server.MethodCalls.Any());
            Assert.AreEqual(Server.MethodCalls[0].MethodName, nameof(ServerMock.ServerMethodA));
        }

        [TestMethod]
        public void SendServerMessageB()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            ServerProxy.ServerMethodB("test", 255, int.MaxValue, long.MaxValue, 1.5, 1.5f, dateTimeOffset.DateTime, dateTimeOffset, new List<string>() { "foo", "bar" }, Tuple.Create("hello", "world"));

            Assert.IsTrue(Server.MethodCalls.Any());
            Assert.AreEqual(Server.MethodCalls[0].MethodName, nameof(ServerMock.ServerMethodB));

            var args = Server.MethodCalls[0].Args;

            AssertMethodB(args, dateTimeOffset);
        }

        [TestMethod]
        public void SendServerMessageC()
        {
            ServerProxy.ServerMethodC(new List<byte>() {0,1,2}, new []{0,1,2} );

            Assert.IsTrue(Server.MethodCalls.Any());
            Assert.AreEqual(Server.MethodCalls[0].MethodName, nameof(ServerMock.ServerMethodC));
        }

        [TestMethod]
        public void SendNullParam()
        {
            string returnValue = ServerProxy.ServerMethodD(null);

            Assert.IsNull(returnValue);
            Assert.IsNull(Server.MethodCalls[0].Args[0]);
        }

        [TestMethod]
        public void PerformanceTest()
        {
            int count = 100;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                ServerProxy.ServerMethodA();
            }
            sw.Stop();

            Assert.AreEqual(Server.MethodCalls.Count, count);
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
