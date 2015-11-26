using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.RPC.Extensions.Utils;

namespace EasyNetQ.RPC.Extensions
{
    public abstract class RabbitConnection<T>
    {
        protected IBus Bus { get; private set; }
        private readonly string _rabbitConnectionString;
        private readonly string _brokerId;

        /// <summary>
        /// Gets whether this connection is still connected
        /// </summary>
        public bool IsConnected => Bus.IsConnected;

        protected RabbitConnection(string rabbitConnectionString, string brokerId)
        {
            _rabbitConnectionString = rabbitConnectionString;
            _brokerId = brokerId;
        }

        /// <summary>
        /// Connects to the rabbit service, register logging and naming, and allows subclasses to do their own subscription registration.
        /// </summary>
        public void Connect()
        {
            Bus = RabbitHutch.CreateBus(_rabbitConnectionString, register =>
            {
                register.Register<IConventions>(provider => new CustomConventions<T>(new TypeNameSerializer(), _brokerId));
            });
            RegisterOnBus(Bus);
        }

        /// <summary>
        /// Called on Connect()
        /// </summary>
        /// <param name="bus"></param>
        protected abstract void RegisterOnBus(IBus bus);

        /// <summary>
        /// Closes the connection to rabbit.
        /// </summary>
        public void Close()
        {
            if (Bus != null)
            {
                Bus.Dispose();
                Bus = null;
            }
        }

    }
}
