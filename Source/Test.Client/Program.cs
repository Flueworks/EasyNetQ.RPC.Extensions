using System;
using EasyNetQ.RPC.Extensions;
using Test.Shared;

namespace Test.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculatorRpcClient = new RabbitRpcClient<ICalculator>("host=localhost;username=guest;password=guest;virtualHost=rabbitCommunicationTest", "rabbit.test");
            calculatorRpcClient.Connect();
            var calculatorModule = calculatorRpcClient.GetServerProxy();
            
            while (true)
            {
                Console.WriteLine("1) Add");
                Console.WriteLine("2) Subtract");
                Console.WriteLine("3) Multiply");
                Console.WriteLine("4) Divide");
                var readLine = Console.ReadLine();
                if (readLine == null) continue;
                var line = readLine.ToLower();

                if(line == "exit")
                    break;

                ParseLine(line, calculatorModule);
            }
            calculatorRpcClient.Close();
        }

        private static void ParseLine(string line, ICalculator calculatorModule)
        {
            var strings = line.Split(' ');

            if(strings.Length == 0) { return;}

            switch (strings[0])
            {
                case "1":
                    Console.WriteLine("1 + 2 = {0}",calculatorModule.Add(1, 2));
                    break;
                case "2":
                    Console.WriteLine("1 + 2 = {0}", calculatorModule.Subtract(2, 1));
                    break;
                case "3":
                    Console.WriteLine("1 + 2 = {0}", calculatorModule.Multiply(2, 3));
                    break;
                case "4":
                    Console.WriteLine("1 + 2 = {0}", calculatorModule.Divide(4, 2));
                    break;

            }
        }
    }
}
