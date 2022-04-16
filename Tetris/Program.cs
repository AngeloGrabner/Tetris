using System;
class Program
{
    static void Main()
    {
        Game game = new(20,20);
        game.run();
#pragma warning disable CA1416
        Console.WindowWidth = 120;
        Console.WindowHeight = 30;
#if DEBUG
        foreach (var element in game.debugInfo)
            Console.WriteLine(element);
#endif
        Console.WriteLine($"your score is: {game.Score}, press enter to exit");
        Console.ReadLine();
    }
}