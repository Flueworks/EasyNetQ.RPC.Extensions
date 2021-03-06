﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.RPC.Extensions.Utils;

namespace EasyNetQ.RPC.Extensions
{
    public class RabbitRpcClient<TServer> : RabbitConnection<TServer>
    {
        private readonly RabbitProxy<TServer> _rabbitProxy;

        public RabbitRpcClient(string rabbitConnectionString, string brokerId) : base(rabbitConnectionString, brokerId)
        {
            _rabbitProxy = new RabbitProxy<TServer>(CallServer);
        }

        protected override void RegisterOnBus(IBus bus)
        {

        }

        private object CallServer(RabbitDelegateMessage message)
        {
            try
            {
                if (!IsConnected) { throw new NotConnectedException(); }
                RabbitResponseMessage response = Bus.Request<RabbitDelegateMessage, RabbitResponseMessage>(message);

                if (response.Exception != null)
                {
                    throw response.Exception;
                }
                var reply = response.Reply as IInternalValueProvider;
                return reply.GetValue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public TServer GetServerProxy() => (TServer)_rabbitProxy.GetTransparentProxy();


    }
}
