using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Game
{
    private bool gameOver = false, stopInput = false;
    private char input = ' ';
    private int errorFlag = 0, cooldown = 1000, forceDownCounter = 0, score = 0;
    Thread thd;
    public Game()
    {
        thd = new Thread(getInput);
        thd.Start();
        if (!thd.IsAlive)
        {
            errorFlag = 1;
            return;
        }
        thd.IsBackground = true;
    }
    public void run()
    {
        if (errorFlag > 0)
        {
            Console.WriteLine("error: "+errorFlag);
            return;
        }

        while (!gameOver)
        {  
            if (!collision(input)) // no collision
            {
                doMove(input);
            }
            else  // collision
            {
                if (checkFullRow(/*int pos*/)) // check for any full rows 
                {
                    score++;
                    //for height of tile
                    //clearRowAndMoveAllOtherDown(int pos)
                }
                // lock tile in place spawn new tile etc.
              
            }
            if (forceDownCounter == 3&&!collision('s')) 
            {
                forceDownCounter = 0;
                doMove('s');
            }
            else  // collision
            {
                if (checkFullRow(/*int pos*/)) // check for any full rows 
                {
                    score++;
                    //for height of tile
                    //clearRowAndMoveAllOtherDown(int pos)
                }
                // lock tile in place spawn new tile etc.

            }
            forceDownCounter++;
            Thread.Sleep(cooldown);
        }
    }
    private bool collision(char direction) // checks if the move will collide with anything 
    {
        return false;
    }
    private void doMove(char direction) 
    {

    }
    private void getInput()
    {
        while (!stopInput)
        {
            input = ' ';
            ConsoleKeyInfo temp = Console.ReadKey();
            if (!(temp.KeyChar == 'w'||temp.KeyChar == 's'||temp.KeyChar=='a'||temp.KeyChar=='d'))
            {
                continue;
            }
            input = temp.KeyChar;
        }
    }
}

