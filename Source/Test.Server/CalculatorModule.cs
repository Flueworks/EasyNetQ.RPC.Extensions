using System;
using Test.Shared;

namespace Test.Server
{
    class CalculatorModule : ICalculator
    {
        public int Add(int a, int b) => a + b;

        public int Subtract(int a, int b) => a - b;

        public double Multiply(int a, int b) => a * b;

        public double Divide(int a, int b) => a / (double)b;

        public int Mod(int a, int b) => a % b;

        public int Square(int a) => (int) Math.Sqrt(a);
    }
}
