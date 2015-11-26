namespace EasyNetQ.RPC.Extensions.Utils
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using IMessage = System.Runtime.Remoting.Messaging.IMessage;

    /// <summary>
    /// Provides a RealProxy that impersonates an interface, and provides a callback whenever an interface method is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RabbitProxy<T> : RealProxy
    {
        private readonly Func<RabbitDelegateMessage, object> _callServer;

        public RabbitProxy(Func<RabbitDelegateMessage, object> callServer) : base(typeof(T))
        {
            _callServer = callServer;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            // ReSharper disable once PossibleNullReferenceException
            var methodInfo = methodCall.MethodBase as MethodInfo;

            // ReSharper disable once PossibleNullReferenceException
            var returnValue = _callServer(new RabbitDelegateMessage(methodInfo.Name, methodCall.InArgs));

            return new ReturnMessage(returnValue, null, 0, methodCall.LogicalCallContext, methodCall);
        }
    }
}
