using System;
class Program
{
    static void Main()
    {
        Game game = new();
        game.run();
#pragma warning disable CA1416
        Console.WindowWidth = 120;
        Console.WindowHeight = 30;
#if DEBUG
        foreach (var element in game.debugInfo)
            Console.WriteLine(element);
#endif
        Console.ReadLine();
    }
}