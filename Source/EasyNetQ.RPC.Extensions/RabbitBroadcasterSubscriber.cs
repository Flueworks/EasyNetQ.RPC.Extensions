using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.RPC.Extensions
{
    public class RabbitBroadcasterSubscriber<TClient> : RabbitConnection<TClient>
    {
        private readonly TClient _client;
        private readonly string _topic;
        private readonly Type _clientType;

        /// <summary>
        /// Creates a new client
        /// </summary>
        /// <param name="rabbitConnectionString">The connection string for Rabbit</param>
        /// <param name="brokerId">Used to separate different clusters</param>
        /// <param name="client">The class which is the endpoint to calls made from a server.</param>
        /// <param name="topic">Is this even needed?</param>
        public RabbitBroadcasterSubscriber(string rabbitConnectionString, string brokerId, TClient client, string topic) : base(rabbitConnectionString, brokerId)
        {
            _client = client;
            _topic = topic;
            _clientType = _client.GetType();
        }

        /// <summary>
        /// Called after we connect, used to subscribe to messages
        /// </summary>
        /// <param name="bus"></param>
        protected override void RegisterOnBus(IBus bus)
        {
            // We uses a unique subscriptionID, because this will cause the message to be received by all subscribers. A common subscription ID would make it into a round robin.
            bus.SubscribeAsync<RabbitDelegateMessage>(Guid.NewGuid().ToString(), OnMessage, configuration => configuration.WithTopic(_topic).WithAutoDelete());
        }

        private Task OnMessage(RabbitDelegateMessage rabbitDelegateMessage)
        {
            // Uses reflection to get the requested method from the provided client class and invokes it.
            MethodInfo methodInfo = _clientType.GetMethod(rabbitDelegateMessage.Method);
            return Task.Factory.StartNew(() => methodInfo.Invoke(_client, rabbitDelegateMessage.Args));
        }
    }
}
