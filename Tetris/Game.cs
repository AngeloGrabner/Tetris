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
    private const int forceDwonConst = 3;
    private char[,] fild;
    private bool gameOver = false, stopInput = false, tileExists = false, newInputkeyFlag = true;
    private char inputAsync = ' ', input = ' ';
    private int errorFlag = 0, cooldown = 250, forceDownCounter = 0, score = 0, fildWidth, fildHeigh, count = 0;
    public int ErrorFlag { get { return errorFlag; } }
    public int Score { get { return score; } }
    private Thread thd;
    private CollisionInfo move;
    private Tile tile;
    private Random rand = new Random();
    public Game(int fildWidth = 10, int fildHeigh = 20) 
    {
        this.fildWidth = fildWidth;
        this.fildHeigh = fildHeigh;

        Display.setup(fildWidth, fildHeigh); 
        tile = new(0,0,Shape.I_0); 
        fild = new char[this.fildWidth,this.fildHeigh];
        for (int x = 0; x < fild.GetLength(0);x++)
        {
            for (int y = 0; y< fild.GetLength(1);y++)
            {
                fild[x, y] = ' ';
            }
        }

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
        bool currentInputKeyFlag = newInputkeyFlag;
        while (!gameOver)
        {
            input = getInput();

            count++;
#if DEBUG
            debugInfo.Add($"in run() interation: {count}");
#endif
            forceDownCounter++;
            if (inputAsync == '\u001b') // ESC key
            {
                break;
            }

            if (!tileExists)
            {

                //tile = new(fildWidth / 2 - 2, -2, Shape.I_0);
                tile = createNewTile();
                tileExists = true;
            }

            if (forceDownCounter == forceDwonConst) // this should work 
            {
                input = 's';
                forceDownCounter = 0;
            }
            move = collision(input);
            if (move.vertical)
            {
                for (int i = 0; i < 4; i++) //height of the fild array 
                {
                    if (checkFullRow(tile.Y + i)) // check for any full rows 
                    {
                        calculateCooldown(++score);
                        clearRowAndMoveAllOtherDown(tile.Y + i);
                    }

                }
                // lock tile in place spawn new tile etc.
                setInPlace();
                tileExists = false;
            }
            else if (!(move.rotation || move.horizontal))
            {
                doMove(input);
            }

            gameOver = checkLose();
            Display.update(fild);
            //Console.ReadLine();
            Thread.Sleep(cooldown);
        }
        // post game
        stopInput = true; // lets the thd thred suspent
        thd.Join();
        Console.ReadKey(true);
        Console.Clear();
    }
    private void calculateCooldown(int gameScore)
    {
        cooldown = cooldown - (gameScore*2);
        if (cooldown < 0)
        {
            cooldown = 0;
        }
    }
    private void setInPlace()
    {
#if DEBUG
        debugInfo.Add("setInPlace called");
#endif
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (outOfBounceCheck(tile.X + x,tile.Y + y)) // another out of bounce check. i know that that is alot of code dupelication
                {
                    if (fild[tile.X+x,tile.Y+y] == 'X')
                    {
                        fild[tile.X + x, tile.Y + y] = tile.endCharacter;
                    }
                }

            }
        }

    }
    private void clearRowAndMoveAllOtherDown(int y)
    {
#if DEBUG
        debugInfo.Add($"in clearRowAndMoveAllOtherDown() y level: {y}");
#endif
        for (int i = 0; i < fildWidth; i++)
        {
            fild[i, y] = ' ';
        }

        for (int i = 0; i < fildWidth;i++)
        {
            for (int j = y; j > 0; j--) 
            {
                fild[i,j] = fild[i,j-1]; 
            }
        }
    }
    private bool checkFullRow(int y)
    {
        if (!(y < fildHeigh && y >= 0)) // out of bounce 
        {
#if DEBUG
            debugInfo.Add($"in checkFullRow() y level: {y}, False (1)");
#endif
            return false;
        }
        for (int i = 0; i < fildWidth; i++)
        {
            if (fild[i, y] == ' ')
            {
#if DEBUG
                debugInfo.Add($"in checkFullRow() y level: {y}, True");
#endif
                return false;
            }
        }
#if DEBUG
        debugInfo.Add($"in checkFullRow() y level: {y}, False (2)");
#endif
        return true;
    }
    private bool checkLose() 
    {
#if DEBUG
        debugInfo.Add("in checkLose()");
#endif
        for (int i = 0; i < fildWidth; i++)
        {
            if (fild[i,0] != ' ' && fild[i,0] != 'X') // tile on the upper bound of fild (locked in place)
            {
#if DEBUG
                debugInfo.Add($"in checkLose() x: {i}, fild: '{fild[0,i]}'");
#endif
                return true;
            }
        }
        return false; 
    }
    private CollisionInfo collision(char direction) // sometimes a out of bounce can happen (to be fixed)
    {
        bool rotation = false, vertical = false, horizontal = false;
        if (direction == 'w') //rotation
        {
            Tile testTile = new(tile.X,tile.Y,tile.shape);
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
                    if (outOfBounceCheck(testTile.X + x,  testTile.Y + y)) 
                    {
                        if (fild[testTile.X + x, testTile.Y + y] != ' ' && fild[testTile.X + x, testTile.Y + y] != 'X' && testTile.map[x, y] == 'X') // collision occoured
                        {
                            rotation = true;
                            goto end;
                        }
                    }
                    else if (testTile.map[x, y] == 'X')
                    {
                        rotation = true;
                        goto end;
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
                    if (outOfBounceCheck(tile.X + x + move, tile.Y + y)) 
                    {
                        if (fild[tile.X + x + move,tile.Y + y] != ' ' && fild[tile.X + x + move, tile.Y + y] != 'X' && tile.map[x, y] == 'X') //collision check 
                        {
                            horizontal = true;
                            goto end;
                        }
                    }
                    else if (tile.map[x, y] == 'X')
                    {
                        horizontal = true;
                        goto end;
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
                    if (outOfBounceCheck(tile.X + x + move, tile.Y + y)) 
                    {
                        if (fild[tile.X + x + move, tile.Y + y] != ' '  && fild[tile.X + x + move, tile.Y + y] != 'X' && tile.map[x, y] == 'X') //collision check 
                        {
                            horizontal = true;
                            goto end;
                        }
                    }
                    else if (tile.map[x, y] == 'X')
                    {
                        horizontal = true;
                        goto end;
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
                    if (outOfBounceCheck(tile.X + x, tile.Y + y + move)) 
                    {
                        if (fild[tile.X + x, tile.Y + y + move] != ' ' && fild[tile.X + x, tile.Y + y + move] != 'X' && tile.map[x, y] == 'X') //collision check 
                        {
                            vertical = true;
                            goto end;
                        }
                    }
                    else if (tile.map[x, y] == 'X') // not optimal 
                    {
                        vertical = true;
                        goto end;
                    }
                }
            }
        }
    end:
#if DEBUG
        debugInfo.Add($"in collision() direction: {direction}, h: {horizontal}, v: {vertical}, r: {rotation}");
#endif
        return new CollisionInfo(horizontal,vertical,rotation);
    }
    private bool outOfBounceCheck(int x, int y)
    {
        return x < fildWidth && x >= 0 && y < fildHeigh && y >= 0;
    }
    private Tile createNewTile() //somehow the shapes werent random 
    {
        int randNum = rand.Next(7);
#if DEBUG
        debugInfo.Add($"in reateNewTile() randNum: {randNum}");
#endif
        switch (randNum)
        {
            case 0:
                return new(fildWidth / 2 - 2, -2, Shape.I_0);
            case 1:
                return new(fildWidth / 2 - 2, -1, Shape.J_0);
            case 2:
                return new(fildWidth / 2 - 2, -1, Shape.L_0);
            case 3:
                return new(fildWidth / 2 - 2, -1, Shape.O_0);
            case 4:
                return new(fildWidth / 2 - 2, -1, Shape.S_0);
            case 5:
                return new(fildWidth / 2 - 2, -1, Shape.T_0);
            case 6:
                return new(fildWidth / 2 - 2, -1, Shape.Z_0);
        }
        return null;
        
    }
    private void doMove(char direction) 
    {
#if DEBUG
        debugInfo.Add($"in doMove() direction: {direction}, tile.X {tile.X}, tile.Y {tile.Y}");
#endif
        if (direction == ' ')
        {
            return;
        }
        // clear old one
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (outOfBounceCheck(tile.X + x, tile.Y + y)) 
                {
                    if (fild[tile.X + x, tile.Y + y] == 'X')
                    {
                        fild[tile.X + x, tile.Y + y] = ' ';
                    }
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
            tile.shape = (Shape)newTileShape;
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
                    if (outOfBounceCheck(tile.X + x, tile.Y + y))
                    {
                        fild[tile.X + x, tile.Y + y] = tile.map[x, y]; //lets just hope that that works 
                    }
                }
            }
        }
    }
    private char getInput() 
    {
        if (newInputkeyFlag)
        {
            newInputkeyFlag = false;
            if (inputAsync == 'w' || inputAsync == 's' || inputAsync == 'a' || inputAsync == 'd' || inputAsync == '\u001b')
            {
                return inputAsync;
            }
        }
        return ' ';
    }
    private void getInputAsync() // this one will be in another thread
    {
#if DEBUG
        debugInfo.Add("in getInputAsyne()");
#endif
        while (!stopInput)
        {
            inputAsync = Console.ReadKey(true).KeyChar;
            newInputkeyFlag = true;
        }
    }
}