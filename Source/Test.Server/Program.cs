using System;
using EasyNetQ.RPC.Extensions;
using Test.Shared;

namespace Test.Server
{
    class Program
    {
        static void Main(string[] args)
        {

            var calculatorModule = new CalculatorModule();
            var calculatorRpcServer = new RabbitRpcServer<ICalculator>("host=localhost;username=guest;password=guest;virtualHost=rabbitCommunicationTest", "rabbit.test", calculatorModule);
            calculatorRpcServer.Connect();

            Console.ReadLine();

            calculatorRpcServer.Close();
        }
    }
}
