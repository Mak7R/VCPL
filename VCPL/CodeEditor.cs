namespace VCPL;

public class CodeEditor
{
    private (int MaxUp, int MaxDown) MaxConsoleSize = (0, 100);
    private string codeString = "";

    public string Endstr;
    
    public string CodeString
    {
        get { return codeString; }
        private set { codeString = value; }
    }
    public void ConsoleReader()
    {
        while (true)
        {
            ConsoleKeyInfo pressed = Console.ReadKey(true);
            CursorController(pressed);
        }
    }

    public void CursorController(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                ConsoleMove(true);
                break;
            case ConsoleKey.DownArrow:
                ConsoleMove(false);
                break;
            default:
                Console.Write(key);
                break;
        }
    }

    public void ConsoleMove(bool isUp)
    {
        if (isUp)
        {
            Console.CursorTop++;
        }
        else
        {
            Console.CursorTop--;
        }
    }
}