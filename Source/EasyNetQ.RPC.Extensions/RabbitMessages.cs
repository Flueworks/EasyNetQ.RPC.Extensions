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
            SendArguments = args?.Select(ObjectExExtension.CreateObjectEx).ToArray();
        }

        public string Method { get; set; }

        public object[] SendArguments { get; set; }

        public object[] Args => SendArguments.Select(x => ((IInternalValueProvider)x).GetValue()).ToArray();

        
        
    }

    public class RabbitResponseMessage
    {
        /// <summary>
        /// Json serializer requires empty constructor
        ///  </summary>
        public RabbitResponseMessage()
        {
            
        }

        public RabbitResponseMessage(object result)
        {
            Reply = ObjectExExtension.CreateObjectEx(result);
        }

        public RabbitResponseMessage(Exception ex)
        {
            Exception = ex;
        }

        public object Reply { get; set; }

        public Exception Exception { get; set; }
    }

    public class ObjectEx<T> : IInternalValueProvider
    {
        public T Value { get; set; }

        public object GetValue() => Value;

        public void SetValue(object o) => Value = (T)o;
    }

    public interface IInternalValueProvider
    {
        object GetValue();
        void SetValue(object o);
    }

    internal static class ObjectExExtension
    {
        /// <summary>
        /// Makes a strongly typed class that will force the serializer to keep the 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        internal static object CreateObjectEx(object o)
        {
            if (o == null)
            {
                return new ObjectEx<object>() { Value = null };
            }
            var type = typeof(ObjectEx<>);
            var makeGenericType = type.MakeGenericType(o.GetType());
            var target = Activator.CreateInstance(makeGenericType) as IInternalValueProvider;
            target.SetValue(o);
            return target;
        }
    }
}
