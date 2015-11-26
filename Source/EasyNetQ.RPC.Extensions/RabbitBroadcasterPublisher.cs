using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.RPC.Extensions.Utils;

namespace EasyNetQ.RPC.Extensions
{
    public class RabbitBroadcasterPublisher<TClient> : RabbitConnection<TClient>
    {
        private readonly string _topic;
        private readonly RabbitProxy<TClient> _rabbitProxy;

        public RabbitBroadcasterPublisher(string rabbitConnectionString, string brokerId, string topic) : base(rabbitConnectionString, brokerId)
        {
            _topic = topic;
            _rabbitProxy = new RabbitProxy<TClient>(CallClients);
        }

        protected override void RegisterOnBus(IBus bus)
        {
        }

        private object CallClients(RabbitDelegateMessage message)
        {
            if (Bus.IsConnected)
            {
                Bus.Publish(message, _topic);
            }
            else
            {
                throw new NotConnectedException();
            }
            return null;
        }

        public TClient GetClientProxy() => (TClient)_rabbitProxy.GetTransparentProxy();
    }
}
