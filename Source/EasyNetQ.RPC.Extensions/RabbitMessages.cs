using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.RPC.Extensions
{
    internal class RabbitDelegateMessage
    {
        /// <summary>
        /// Json serializer requires empty constructor
        /// </summary>
        public RabbitDelegateMessage()
        {

        }

        public RabbitDelegateMessage(string method, object[] args = null)
        {
            Method = method;
            SendArguments = args?.Select(CreateObjects).ToArray();
        }

        public string Method { get; set; }

        public object[] SendArguments { get; set; }

        public object[] Args => SendArguments.Select(x => ((IInternalValueProvider)x).GetValue()).ToArray();

        /// <summary>
        /// Makes a strongly typed class that will force the serializer to keep the 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static object CreateObjects(object o)
        {
            var type = typeof(ObjectEx<>);
            var makeGenericType = type.MakeGenericType(o.GetType());
            var target = Activator.CreateInstance(makeGenericType) as IInternalValueProvider;
            target.SetValue(o);
            return target;
        }
    }

    public class RabbitResponseMessage
    {
        public object Reply { get; set; }
        public Exception Exception { get; set; }
    }

    public class ObjectEx<T> : IInternalValueProvider
    {
        public T Value { get; set; }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object o)
        {
            Value = (T)o;
        }
    }

    public interface IInternalValueProvider
    {
        object GetValue();
        void SetValue(object o);
    }
}
