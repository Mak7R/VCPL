
namespace VCPL
{
    static class Program
    {
        static void Main(string[] args)
        {
            // here will be code editor which will create string code of Program
            string code = "";
            MainFunction main = new MainFunction(code);
            main.Run();
        }
    }
}