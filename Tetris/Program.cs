using System;

class Program
{
    static void Main()
    {
        //Game game = new();
        //game.run();
        char[,] chars = new char[,]
                { {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', 'O' },
                  {' ', 'X', ' ', ' ' },
                  {' ', 'X', ' ', ' ' } };

        Display.update(chars);
        Console.ReadLine();
    }
}