using System.Collections.Generic;

namespace Test.Shared
{
    public interface ICalculator
    {
        int Add(int a, int b);
        int Subtract(int a, int b);
        double Multiply(int a, int b);
        double Divide(int a, int b);
        int Mod(int a, int b);
        int Square(int a);
    }

    public interface IServiceHealth
    {
        void UpdateLogicRules(List<LogicRule> logicRules);
    }

    public class LogicRule
    {
        
    }
}
