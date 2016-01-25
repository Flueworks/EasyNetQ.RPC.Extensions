using System;
using System.Collections.Generic;
using System.Threading;

namespace EasyNetQ.RPC.Extensions.Test
{
    public class ServerMock : MockClass, IServerTest
    {
        public int ServerMethodA()
        {
            LogCall(nameof(ServerMethodA));
            return 10;
        }

        public string ServerMethodB(string s, byte b, int i, long l, double d, float f, DateTime dt, DateTimeOffset? dton, List<string> list, object o)
        {
            LogCall(nameof(ServerMethodB), s, b, i, l, d, f, dt, dton, list, o);
            return null;
        }

        public void ServerMethodC(IEnumerable<byte> listOfBytes, IEnumerable<int> listOfInts)
        {
            LogCall(nameof(ServerMethodC));
        }

        public IClientTest ClientProxy { get; set; }

        public void ServerMethodCallback1()
        {
            LogCall(nameof(ServerMethodCallback1));
            ClientProxy.ClientMethodCallback2();
        }

        public void ServerMethodCallback2()
        {
            LogCall(nameof(ServerMethodCallback2));
            ClientProxy.ClientMethodA();
        }

        public string ServerMethodD(string nullString)
        {
            LogCall(nameof(ServerMethodD), nullString);
            return null;
        }
    }

    public class ClientMock : MockClass, IClientTest
    {
        public void ClientMethodA()
        {
            LogCall(nameof(ClientMethodA));
        }

        public void ClientMethodB(string s, byte b, int i, long l, double d, float f, DateTime dt, DateTimeOffset? dton, List<string> list, object o)
        {
            LogCall(nameof(ClientMethodB), s, b, i, l, d, f, dt, dton, list, o);
        }

        public IServerTest ServerProxy { get; set; }
        public void ClientMethodCallback1()
        {
            LogCall(nameof(ClientMethodCallback1));
            ServerProxy.ServerMethodCallback2();
        }

        public void ClientMethodCallback2()
        {
            LogCall(nameof(ClientMethodCallback2));
            ServerProxy.ServerMethodA();
        }
    }

    public class MockClass
    {
        public readonly List<MethodCallResult> MethodCalls = new List<MethodCallResult>();
        public readonly AutoResetEvent MethodCalled = new AutoResetEvent(false);

        public void LogCall(string name, params object[] args)
        {
            MethodCalls.Add(new MethodCallResult(name, args));
            MethodCalled.Set();
        }

        public void WaitForMethodCall()
        {
            MethodCalled.WaitOne(5000);
        }

    }
    public class MethodCallResult
    {
        public MethodCallResult(string name, object[] args)
        {
            MethodName = name;
            Args = args;
        }

        public string MethodName { get; set; }
        public object[] Args { get; set; }

    }
}
