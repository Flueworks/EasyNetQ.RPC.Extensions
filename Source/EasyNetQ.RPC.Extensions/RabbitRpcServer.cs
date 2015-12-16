using System;
using System.Reflection;

namespace EasyNetQ.RPC.Extensions
{
    public class RabbitRpcServer<TServer> : RabbitConnection<TServer>
    {
        private readonly TServer _server;
        private readonly Type _serverType;

        public RabbitRpcServer(string rabbitConnectionString, string brokerId, TServer server) : base(rabbitConnectionString, brokerId)
        {
            _server = server;
            _serverType = _server.GetType();
        }

        private RabbitResponseMessage Responder(RabbitDelegateMessage rabbitDelegateMessage)
        {
            MethodInfo methodInfo = _serverType.GetMethod(rabbitDelegateMessage.Method);

            try
            {
                var reply = methodInfo.Invoke(_server, rabbitDelegateMessage.Args);
                return new RabbitResponseMessage(reply);
            }
            catch (TargetInvocationException e)
            {
                return new RabbitResponseMessage(e.InnerException);
            }
        }

        protected override void RegisterOnBus(IBus bus)
        {
            bus.Respond<RabbitDelegateMessage, RabbitResponseMessage>(Responder);
        }
    }
}