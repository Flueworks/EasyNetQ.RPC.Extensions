using System;

namespace EasyNetQ.RPC.Extensions.Utils
{
    /// <summary>
    /// Provides naming conventions so that we can use brokerId to separate different versions, and also separate the queues on Interface name.
    /// </summary>
    /// <typeparam name="T">The interface which this queue should use to generate naming</typeparam>
    internal class CustomConventions<T> : IConventions
    {
        public CustomConventions(ITypeNameSerializer typeNameSerializer, string brokerId)
        {
            // Exchange naming convention: brokerId.Interface.MessageType
            ExchangeNamingConvention = messageType =>
            {
                var attr = GetQueueAttribute(messageType);

                return string.IsNullOrEmpty(attr.ExchangeName)
                    ? $"{brokerId}.{typeof(T).Name}.{typeNameSerializer.Serialize(messageType)}"
                    : $"{brokerId}.{attr.ExchangeName}";
            };

            TopicNamingConvention = messageType => "";

            // Queue naming convention: brokerId.MessageType_SubscriptionId
            QueueNamingConvention = (messageType, subscriptionId) =>
            {
                var attr = GetQueueAttribute(messageType);

                if (!string.IsNullOrEmpty(attr.QueueName))
                {
                    return string.IsNullOrEmpty(subscriptionId)
                        ? $"{brokerId}.{attr.QueueName}"
                        : $"{brokerId}.{attr.QueueName}_{subscriptionId}";
                }

                var typeName = typeNameSerializer.Serialize(messageType);

                return string.IsNullOrEmpty(subscriptionId)
                    ? $"{brokerId}.{typeName}"
                    : $"{brokerId}.{typeName}_{subscriptionId}";
            };

            ErrorQueueNamingConvention = () => $"{brokerId}.EasyNetQ_Default_Error_Queue";
            ErrorExchangeNamingConvention = info => $"{brokerId}.ErrorExchange_{info.RoutingKey}";

            RpcExchangeNamingConvention = () => $"{brokerId}.RPC";
            RpcReturnQueueNamingConvention = () => $"{brokerId}.RPC.{Guid.NewGuid()}";
            RpcRoutingKeyNamingConvention = type => $"{brokerId}.{typeof(T).Name}.{typeNameSerializer.Serialize(type)}";

            ConsumerTagConvention = () => Guid.NewGuid().ToString();
        }

        private QueueAttribute GetQueueAttribute(Type messageType) => messageType.GetAttribute<QueueAttribute>() ?? new QueueAttribute(string.Empty);

        public ExchangeNameConvention ExchangeNamingConvention { get; set; }
        public TopicNameConvention TopicNamingConvention { get; set; }
        public QueueNameConvention QueueNamingConvention { get; set; }
        public RpcRoutingKeyNamingConvention RpcRoutingKeyNamingConvention { get; set; }
        public ErrorQueueNameConvention ErrorQueueNamingConvention { get; set; }
        public ErrorExchangeNameConvention ErrorExchangeNamingConvention { get; set; }
        public RpcExchangeNameConvention RpcExchangeNamingConvention { get; set; }
        public RpcReturnQueueNamingConvention RpcReturnQueueNamingConvention { get; set; }
        public ConsumerTagConvention ConsumerTagConvention { get; set; }
    }
}
