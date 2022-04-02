// für die prokect arbeit grafic: https://stackoverflow.com/questions/4053837/colorizing-text-in-the-console-with-c
readonly struct CollisionInfo // this struct is useless, but ey now it exists 
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
    public List<string> debugInfo = new();
    private const int forceDwonConst = 5;
    private char[,] fild;
    private bool gameOver = false, stopInput = false, tileExists = false;
    private char input = ' ';
    private int errorFlag = 0, cooldown = 1000, forceDownCounter = 0, score = 0, fildWidth, fildHeigh;
    public int ErrorFlag { get { return errorFlag; } }
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

        thd = new Thread(getInputAsync);
        thd.Start();
        if (!thd.IsAlive)
        {
            errorFlag = 1;
            throw new Exception("thread is dead");
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
        int count = 0;
        while (!gameOver)
        {
            count++;
            debugInfo.Add($"in run() interation: {count}");
            forceDownCounter++;

            if (!tileExists)
            {
                tile = new(0,0,Shape.I_0);
                //tile = createNewTile();
                tileExists = true;
            }

            if (forceDownCounter == forceDwonConst) // this should work 
            {
                input = 's';
                forceDownCounter = 0;
            }
            move = collision(input);
            if (move.any)
            {
                /*
                if (move.horizontal||move.rotation) // maybe do something or maybe not -.-
                {
                    //continue;
                }
                else*/ 
                if (move.vertical)
                {
                    for (int i = 0; i < 4; i++) //height of the fild array 
                    {
                        if (checkFullRow(tile.Y+i)) // check for any full rows 
                        {
                            score++;
                            clearRowAndMoveAllOtherDown(tile.Y + i);
                        }
                    }
                    // lock tile in place spawn new tile etc.
                    setInPlace();
                    tileExists = false;
                }
            }
            else
            {
                doMove(input);
            }

            gameOver = checkLose();
            //calculateCooldown();
            Display.update(fild);
            //Console.ReadLine();
            Thread.Sleep(cooldown);
        }
        // post game
        stopInput = true; // lets the thd thred suspent
        Console.Clear();
    }
    private void setInPlace()
    {
        //to be added gives the tile in the fild array another char (for coloring later on), also no not move the tile in the doMove()
        debugInfo.Add("setInPlace called");
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (!(tile.X + x < fildWidth && tile.X + x >= 0 && tile.Y + y < fildHeigh && tile.Y + y >= 0)) // another out of bounce check. i know that that is alot of code dupelication
                {
                    continue;
                }
                if (fild[tile.X+x,tile.Y+y] == 'X')
                {
                    fild[tile.X + x, tile.Y + y] = tile.endCharacter;
                }
            }
        }

    }
    private void clearRowAndMoveAllOtherDown(int y)
    {
        debugInfo.Add($"in clearRowAndMoveAllOtherDown() y level: {y}");
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
        debugInfo.Add($"in checkFullRow() y level: {y}");
        if (!(y < fildWidth && y >= 0)) // out of bounce 
        {
            return false;
        }
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
        for (int i = 0; i < fildWidth; i++)
        {
            if (fild[i,0] != ' ' && fild[i,0] != 'X') // tile on the upper bound of fild (locked in place)
            {
                debugInfo.Add($"in checkLose() x: {i}, fild: '{fild[0,i]}'");
                return true;
            }
        }
        return false; 
    }
    private CollisionInfo collision(char direction) // this will work on the first time testing it without but please 
    {
        debugInfo.Add($"in collision() direction: {direction}");
        bool rotation = false, vertical = false, horizontal = false;
        if (direction == 'w') //rotation
        {
            Tile testTile = tile;
            int newTileShape = 0;
            if ((int)tile.shape % 4 == 3) // checks if adding 1 to the shape (with roration enum) courses the shape to change
            {
                newTileShape = (int)tile.shape - 3;
            }
            else // add 1 to the enum or 90 degrees to the shape 
            {
                newTileShape = (int)tile.shape + 1;
            }
            testTile.fillMap((Shape)newTileShape);
            for (int x = 0; x<4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (testTile.X + x < fildWidth && testTile.X + x >= 0 && testTile.Y + y < fildHeigh && testTile.Y + y >= 0 && testTile.map[x, y] == 'X') // checks for a out of bounce
                    {
                        if (fild[testTile.X + x, testTile.Y + y] != ' ' && testTile.map[x, y] == 'X') // collision occoured
                        {
                            rotation = true;
                            goto end;
                        }
                    } 
                }
            }
        }
        else if (direction == 'd') // right
        {
            const int move = 1;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (tile.X + x + move < fildWidth && tile.Y + y < fildHeigh&& tile.map[x, y] == 'X') //out of bounce check 
                    {
                        if (fild[tile.X + x + move,tile.Y + y] != ' ') //collision check 
                        {
                            horizontal = true;
                            goto end;
                        }
                    }
                }
            }
        }
        else if (direction == 'a') // left
        {
            const int move = -1;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (tile.X + x + move >= 0 && tile.Y + y < fildHeigh && tile.map[x, y] == 'X') //out of bounce check 
                    {
                        if (fild[tile.X + x + move, tile.Y + y] != ' ') //collision check 
                        {
                            horizontal = true;
                            goto end;
                        }
                    }
                }
            }
        }
        else if (direction == 's') // down
        {
            const int move = 1;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (tile.Y + y + move < fildHeigh && tile.X + x < fildWidth && tile.map[x, y] == 'X') //out of bounce check 
                    {
                        if (fild[tile.X + x, tile.Y + y + move] != ' ') //collision check 
                        {
                            vertical = true;
                            goto end;
                        }
                    }
                }
            }
        }
        end:
        return new CollisionInfo(horizontal,vertical,rotation);
    }
    private Tile createNewTile()
    {
        return new(fildWidth / 2 + 2, 0, (Shape)(rand.Next(0, 7) * 4));
    }
    private void doMove(char direction) 
    {
        debugInfo.Add($"in doMove() direction: {direction}");
        // clear old one
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (!(tile.Y + y >= 0 && tile.Y + y < fildHeigh && tile.X +x < fildWidth && tile.X >= 0)) // out of bounce 
                {
                    continue;
                }
                if (fild[tile.X+x,tile.Y+y] == 'X')
                {
                    fild[tile.X + x, tile.Y + y] = ' ';
                }
            }
        }
        // set new position
        if (direction == 'w')
        {
            int newTileShape = 1;
            if ((int)tile.shape%4==3) // checks if adding 1 to the shape (with roration enum) courses the shape to change
            {
                newTileShape = (int)tile.shape-3;
            }
            else // add 1 to the enum or 90 degrees to the shape 
            {
                newTileShape = (int)tile.shape+1;
            }
            tile.fillMap((Shape)newTileShape);
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
                    if (!(tile.X + x < fildWidth && tile.X + x >= 0 && tile.Y + y < fildHeigh && tile.Y + y >= 0)) // a out of bounce on an empty fild that can be skiped
                    {
                        continue;
                    }
                    fild[tile.X + x, tile.Y + y] = tile.map[x, y]; //lets just hope that that works 
                }
            }
        }
    }
    private void getInputAsync()
    {
        debugInfo.Add("in getInputAsyne()");
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

