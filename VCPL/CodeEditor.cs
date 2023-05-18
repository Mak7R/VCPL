using BasicFunctions;

namespace VCPL;

public static class CodeEditor
{
    private static (int MinLeft, int MinTop) ConsoleSize = (0, 0);
    private static List<string> CodeLines = new List<string>() { "" };

    private static int PositionLeftStr = 0;
    private static int PositionTopStr = 0;

    public static void SetCursorPosition(int left, int top)
    {
        PositionTopStr = top - ConsoleSize.MinTop;
        PositionLeftStr = left - ConsoleSize.MinLeft;
        Console.SetCursorPosition(left, top);
    }
    public static void ClearString(int size)
    {
        for (int i = 0; i < size; i++) Console.Write(' ');
    }
    public static void Clear(int left, int top)
    {
        for (int i = 0; i < CodeLines.Count; i++)
        {
            SetCursorPosition(0, i);
            ClearString(CodeLines[i].Length);
        }
        SetCursorPosition(left, top);
        CodeLines = new List<string>() { "" };
    }
    
    public static List<string> ConsoleReader()
    {
        Console.SetCursorPosition(ConsoleSize.MinLeft, ConsoleSize.MinTop);
        while (true)
        {
            ConsoleKeyInfo pressed = Console.ReadKey(true);
            if (pressed.Key == ConsoleKey.Escape) return CodeLines;
            CursorController(pressed);
        }
    }

    public static void CursorController(ConsoleKeyInfo key)
    {
        int CursorLeft = Console.CursorLeft;
        int CursorTop = Console.CursorTop;
        switch (key.Key)
        {
            // Arrows
            case ConsoleKey.UpArrow:
                if (CursorTop > ConsoleSize.MinTop)
                {
                    SetCursorPosition(BasicMath.Min(CodeLines[PositionTopStr-1].Length + ConsoleSize.MinLeft, CursorLeft), CursorTop - 1);
                }
                break;
            case ConsoleKey.DownArrow:
                if (CursorTop < CodeLines.Count-1)
                {
                    SetCursorPosition(BasicMath.Min(CodeLines[PositionTopStr+1].Length+ConsoleSize.MinLeft, CursorLeft), CursorTop + 1);
                }
                break;
            case ConsoleKey.LeftArrow:
                if (CursorLeft > ConsoleSize.MinLeft)
                {
                    SetCursorPosition(CursorLeft - 1, CursorTop);
                }
                break;
            case ConsoleKey.RightArrow:
                if (CursorLeft < CodeLines[PositionTopStr].Length)
                {
                    SetCursorPosition(CursorLeft + 1, CursorTop);
                }
                break;
            // Arrows End
            // Modifiactors(Enter, Backspace, Delete)
            case ConsoleKey.Enter:
                ClearString(CodeLines[PositionTopStr].Length - PositionLeftStr);
                for (int i = PositionTopStr + 1; i < CodeLines.Count; i++)
                {
                    Console.SetCursorPosition(ConsoleSize.MinLeft,i + ConsoleSize.MinTop);
                    ClearString(CodeLines[i].Length);
                }
                CodeLines.Insert(PositionTopStr+1, CodeLines[PositionTopStr].Substring(PositionLeftStr));
                CodeLines[PositionTopStr] = CodeLines[PositionTopStr].Substring(0, PositionLeftStr);
                
                for (int i = PositionTopStr+1; i < CodeLines.Count; i++)
                {
                    Console.SetCursorPosition(ConsoleSize.MinLeft, i + ConsoleSize.MinLeft);
                    Console.Write(CodeLines[i]);
                }
                SetCursorPosition(ConsoleSize.MinLeft, CursorTop+1);
                break;
            case ConsoleKey.Backspace:
                if (Console.CursorLeft <= ConsoleSize.MinLeft)
                {
                    if (PositionTopStr == 0){ break; }
                    
                    for (int i = PositionTopStr; i < CodeLines.Count; i++)
                    {
                        Console.SetCursorPosition(ConsoleSize.MinLeft,i + ConsoleSize.MinTop);
                        ClearString(CodeLines[i].Length);
                    }

                    int posLeft = CodeLines[PositionTopStr - 1].Length + ConsoleSize.MinLeft;
                    CodeLines[PositionTopStr - 1] += CodeLines[PositionTopStr];
                    CodeLines.RemoveAt(PositionTopStr);

                    Console.SetCursorPosition(ConsoleSize.MinLeft, CursorTop-1);
                    Console.Write(CodeLines[PositionTopStr-1]);
                    
                    for (int i = PositionTopStr; i < CodeLines.Count; i++)
                    {
                        Console.SetCursorPosition(ConsoleSize.MinLeft, i + ConsoleSize.MinTop);
                        Console.Write(CodeLines[i]);
                    }
                    SetCursorPosition(posLeft, PositionTopStr+ConsoleSize.MinTop-1);
                }
                else
                {
                    SetCursorPosition(CursorLeft-1, CursorTop);
                    CodeLines[PositionTopStr] = CodeLines[PositionTopStr].Remove(PositionLeftStr, 1);
                    Console.Write(CodeLines[PositionTopStr].Substring(PositionLeftStr));
                    Console.Write(' ');
                    SetCursorPosition(CursorLeft-1, CursorTop);
                }
                break;
            case ConsoleKey.Delete:
                if (PositionLeftStr >= CodeLines[PositionTopStr].Length)
                {
                    if (PositionTopStr == CodeLines.Count - 1){ break; }
                    
                    Console.Write(CodeLines[PositionTopStr + 1]);
                    
                    for (int i = PositionTopStr + 1; i < CodeLines.Count; i++)
                    {
                        Console.SetCursorPosition(ConsoleSize.MinLeft,i + ConsoleSize.MinTop);
                        ClearString(CodeLines[i].Length);
                    }
                    
                    int posLeft = CodeLines[PositionTopStr].Length + ConsoleSize.MinLeft;
                    CodeLines[PositionTopStr] += CodeLines[PositionTopStr + 1];
                    CodeLines.RemoveAt(PositionTopStr + 1);
                    
                    for (int i = PositionTopStr+1; i < CodeLines.Count; i++)
                    {
                        Console.SetCursorPosition(ConsoleSize.MinLeft, i + ConsoleSize.MinTop);
                        Console.Write(CodeLines[i]);
                    }
                    SetCursorPosition(posLeft, PositionTopStr);
                }
                else
                {
                    CodeLines[PositionTopStr] = CodeLines[PositionTopStr].Remove(PositionLeftStr, 1);
                    Console.Write(CodeLines[PositionTopStr].Substring(PositionLeftStr));
                    Console.Write(' ');
                    SetCursorPosition(CursorLeft, CursorTop);
                }
                break;
            // Modificators End
            // Defaults
            default:
                CodeLines[PositionTopStr] = CodeLines[PositionTopStr].Insert(PositionLeftStr, key.KeyChar.ToString());
                Console.Write(CodeLines[PositionTopStr].Substring(PositionLeftStr));
                SetCursorPosition(CursorLeft+1, CursorTop);
                break;
        }
    }
}