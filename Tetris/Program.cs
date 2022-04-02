using System;
class Program
{
    static void Main()
    {
        Game game = new();
        game.run();
        Console.WindowWidth = 120;
        Console.WindowHeight = 30;
        foreach (var element in game.debugInfo)
            Console.WriteLine(element);
        Console.ReadLine();
    }
}