using System;
class Program
{
    static void Main()
    {
        /*
        Game game = new();
        game.run();
#pragma warning disable CA1416
        Console.WindowWidth = 120;
        Console.WindowHeight = 30;
#if DEBUG
        foreach (var element in game.debugInfo)
            Console.WriteLine(element);
#endif
        Console.WriteLine($"your score is: {game.Score}, press enter to exit");
        Console.ReadLine();*/
        int width = 20, height = 20;
        ExtendedConsole.setup(width, height);
        ExtendedConsole.changeColors();
        ExtendedConsole.changeFont(12, 24);
        ExtendedConsole.changeWindowSize((short)width, (short)(height + 1)); // doesnt work (yet)

    }
}