using System;
using System.Collections.Generic;

namespace EasyNetQ.RPC.Extensions.Test
{
    public interface IClientTest
    {
        void ClientMethodA();
        void ClientMethodB(string s, byte b, int i, long l, double d, float f, DateTime dt, DateTimeOffset? dton, List<string> list, object o);
        void ClientMethodCallback1();
        void ClientMethodCallback2();
    }

    public interface IServerTest
    {
        int ServerMethodA();
        string ServerMethodB(string s, byte b, int i, long l, double d, float f, DateTime dt, DateTimeOffset? dton, List<string> list, object o);
        void ServerMethodC(IEnumerable<byte> listOfBytes, IEnumerable<int> listOfInts);
        void ServerMethodCallback1();
        void ServerMethodCallback2();
        string ServerMethodD(string nullString);
    }
}
