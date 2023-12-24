namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            object objA = 10;
            object objB = objA;

            objB = 21;

            Console.WriteLine(objA);
        }
    }
}
