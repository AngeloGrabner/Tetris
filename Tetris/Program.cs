using System;

class Program
{
    static void Main()
    {
        //Game game = new();
        //game.run();
        char[,] chars = new char[,]
                { {'Z', 'X', 'L', ' ' },
                  {'S', 'X', 'J', 'O' },
                  {'T', 'X', ' ', ' ' },
                  {'I', 'X', ' ', ' ' } };

        Display.update(chars);
        Console.ReadLine();
    }
}