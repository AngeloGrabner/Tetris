using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

readonly struct CollisionInfo
{
    public readonly bool any;
    public readonly bool horizontal; //collosion on x 
    public readonly bool vertical; // collision on y 
    public readonly bool rotation; // collision while rotating a Tile 
    public CollisionInfo(bool horizontal, bool vertical, bool rotation)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
        this.rotation = rotation;
        any = horizontal || vertical || rotation;
    }
}
public class Game
{
    private char[,] fild;
    private bool gameOver = false, stopInput = false, tileExists = false;
    private char input = ' ';
    private int errorFlag = 0, cooldown = 1000, forceDownCounter = 0, score = 0, fildWidth, fildHeigh;
    private Thread thd;
    private CollisionInfo move;
    private Tile tile;
    private Random rand = new Random();
    public Game(int fildWidth = 10, int fildHeigh = 20)
    {
        this.fildWidth = fildWidth;
        this.fildHeigh = fildHeigh;
        
        tile = createNewTile();
        fild = new char[this.fildWidth,this.fildHeigh];

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
            forceDownCounter++;

            if (!tileExists)
            {
                tile = createNewTile();
                tileExists = true;
            }

            if (forceDownCounter > 2) // this should work 
            {
                input = 's';
                forceDownCounter = 0;
            }
            move = collision(input);
            if (move.any) // if statment can be simplefied
            {
                if (move.horizontal||move.rotation) // maybe do something or maybe not -.-
                {
                    //continue;
                }
                else if (move.vertical)
                {
                    // lock tile in place spawn new tile etc.
                    finalMove();
                    for (int i = 0; i < 4; i++) //height of the fild array 
                    {
                        if (!(tile.Y+i < fildWidth && tile.Y+i >= 0)) // a out of bounce protection
                        {
                            continue;
                        }
                        else if (checkFullRow(tile.Y+i)) // check for any full rows on 
                        {
                            score++;
                            clearRowAndMoveAllOtherDown(tile.Y + i);
                        }
                    }              
                    tileExists = false;
                }
            }
            else
            {
                doMove(input);
            }

            gameOver = checkLose();

            Display.update(fild);
            Thread.Sleep(cooldown);
        }
    }
    private void setFinalPosition() // not to sure about that 
    {

    }
    private void clearRowAndMoveAllOtherDown(int y)
    {
        for (int i = 0; i < fildWidth; i++)
        {
            fild[i, y] = ' ';
        }

        for (int i = 0; i < fildWidth;i++)
        {
            for (int j = fildHeigh-1; j > 0; j--) // looking for a out of bounce 
            {
                fild[i,j] = fild[i,j-1]; 
            }
        }
    }
    private bool checkFullRow(int y)
    {
        for (int i = 0; i < fildWidth; i++)
        {
            if (fild[i,y]==' ')
            {
                return false;
            }
        }
        return true;
    }
    private bool checkLose()
    { 
        return false; 
    }
    private CollisionInfo collision(char direction) // checks if the move will collide with anything 
    {
        return new (false,false,false);
    }
    private Tile createNewTile()
    {
        return new(fildWidth / 2 + 2, 0, (Shape)(rand.Next(0, 7) * 4));
    }
    private void doMove(char direction) 
    {
        if (direction == 'w')
        {
            int newTile = 0;
            if ((int)tile.shape%4==3) // checks if adding 1 to the shape (with roration enum) courses the shape to change
            {
                newTile = (int)tile.shape-3;
            }
            else // add 1 to the enum or 90 degrees to the shape 
            {
                newTile = (int)tile.shape+1;
            }
            tile.fillMap((Shape)newTile);
        }
        else if (direction == 'a')
        {
            tile.X--;
        }
        else if (direction == 'd')
        {
            tile.X++;
        }
        else if (direction == 's')
        {
            tile.Y++;
        }

        for (int x = 0; x < 4; x++) // prints the tile shape onto the fild
        {
            for (int y = 0; y < 4; y++)
            {
                if (tile.map[x, y] == 'X') //if there is a pice aka if it is not empty
                {
                    fild[tile.X + x, tile.Y + y] = tile.map[x, y]; //lets just hope that that works 
                }
            }
        }
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

