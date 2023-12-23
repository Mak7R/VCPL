using GlobalRealization;
using GlobalRealization.Memory;

namespace TestProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test.Test1();
            object a = Test.Test1;
            Console.WriteLine(a);
        }
    }
}

public class TestInst
{
    public int value;
    public TestInst(int val) { 
        this.value = val;
    }

    public override string ToString()
    {
        return $"Test inst: {value}";
    }
}

public static class Test
{
    public static void Test1()
    {
        
    }
}