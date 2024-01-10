using System.Diagnostics;

namespace Tetris
{
    internal class Game
    {
        Dictionary<string, ConsoleKey> mSettings;
        ConsoleKey mInput = ConsoleKey.None;
        bool mRunning = true;
        List<Array2D<bool>> mTiles;
        Array2D<bool> mCurrentTile, mNextTile, mMap;
        int mTileX = 0, mTileY = 0;
        int mScore = 0;
        double mGravityDelay = 1.0;
        double mGravityMinDelay = 0.3;
        double mTileForceDownDelay = 0.1;
        Stopwatch mForceTileDownSw;
        Stopwatch mGravitySw;
        bool mForceTileDown = false;
        public int Score { get { return mScore; } }
        public Dictionary<string, ConsoleKey> Settings { get { return mSettings; } set { mSettings = value; } }
        public Game() 
        {
            mSettings = new Dictionary<string, ConsoleKey>();
            //load settings
            mTiles = new List< Array2D<bool>>();
            mMap = new Array2D<bool>(10,20);
            mForceTileDownSw = new();
            mGravitySw = new();
            CreateShapes();
            mCurrentTile = mTiles[(new Random()).Next(0, mTiles.Count)];
            mNextTile = mTiles[(new Random()).Next(0, mTiles.Count)];
        }

        public bool Update()
        {
            mGravitySw.Start();
            mForceTileDownSw.Start();

            HandleInput();

            ForceTileDown();

            if (mGravitySw.Elapsed.TotalSeconds >= mGravityDelay)
            {
                mGravitySw.Restart();
                if (Collide(0, 1))
                {
                    CalcGravityDelay();
                    mForceTileDown = false;
                    mScore++;
                    SetTileInPlace();
                    SpawnNewTile();
                    HandleFallenTiles();
                }
                else if (!mForceTileDown)
                {
                    mTileY += 1;
                }
            }

            Screen.Clear();
            Draw();
            Screen.Draw();

            return mRunning;
        }
        void CalcGravityDelay()
        {
            int scoreMaxGravity = 1000;
            mGravityDelay = Math.Max(mGravityMinDelay, 1.0 - (mScore / scoreMaxGravity * (1.0 - mGravityMinDelay)));
        }
        void ForceTileDown()
        {
            if (mForceTileDownSw.Elapsed.TotalSeconds >= mTileForceDownDelay && mForceTileDown)
            {
                mForceTileDownSw.Restart();
                if (!Collide(0, 1))
                {
                    mTileY += 1;
                }
                else
                    mForceTileDown = false;
            }
        }
        void Draw()
        {
            int offsetX = Screen.Width / 2  - mMap.Width / 2;
            int offsetY = 1; 
            char block = '\u2588';

            Screen.Rect(offsetX - 1, offsetY - 1, mMap.Width + 2, mMap.Height + 2, block);

            // draw map
            for (int y = 0; y < mMap.Height; y++)
            {
                for (int x = 0; x < mMap.Width; x++) 
                {
                    if (mMap.Get(x, y))
                    {
                        Screen.Set(x + offsetX, y + offsetY, '@');
                    }
                    //else
                    //{
                    //    mScreen.Set(x + offsetX, y + offsetY, '.');
                    //}

                }
            }

            // draw current tile
            for (int y = 0; y < mCurrentTile.Height; y++)
            {
                for (int x = 0; x < mCurrentTile.Width; x++)
                {
                    if (mCurrentTile.Get(x, y))
                    {
                        Screen.Set(x + mTileX + offsetX, y + mTileY + offsetY, '\u2593');
                    }
                }
            }

            // draw next tile

            offsetX += mMap.Width + 6;
            offsetY+=3;
            Screen.Rect(offsetX-1, offsetY-1, mNextTile.Width+2, mNextTile.Height+2, block);

            Screen.Print(offsetX - 3, offsetY - 3, "Next Piece:");

            for (int y = 0; y < mNextTile.Height; y++)
            {
                for (int x = 0; x < mNextTile.Width; x++)
                {
                    if (mNextTile.Get(x, y))
                        Screen.Set(x + offsetX, y + offsetY, '+');
                }
            }

            offsetX -= 3;
            offsetY += 6;

            Screen.Print(offsetX, offsetY, $"Score: {mScore}");
        }
        void SetTileInPlace()
        {
            for (int y = 0; y < mCurrentTile.Height; y++) 
            {
                for (int x = 0; x < mCurrentTile.Width; x++) 
                {
                    if (mCurrentTile.Get(x,y))
                    {
                        mMap.Set(x + mTileX, y + mTileY, mCurrentTile.Get(x, y));
                    }
                }
            }
        }
        bool Collide(int offX, int offY)
        {
            for (int y = 0; y < mCurrentTile.Height; y++)
            {
                for (int x = 0; x < mCurrentTile.Width; x++)
                {
                    if ((mMap.Get(x + mTileX + offX, y + mTileY + offY) && mCurrentTile.Get(x,y)) ||
                        (mCurrentTile.Get(x, y) && y + mTileY + offY >= mMap.Height) || 
                        (mCurrentTile.Get(x,y) && x + mTileX + offX >= mMap.Width) ||
                        (mCurrentTile.Get(x, y) && x + mTileX + offX < 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        void HandleFallenTiles()
        {
            for (int y = 0; y < mMap.Height; y++)
            {
                int blockCounter = 0;
                for (int x = 0; x < mMap.Width; x++) 
                {
                    if (mMap.Get(x,y))
                    {
                        blockCounter++;
                    }
                }
                if (blockCounter == mMap.Width)
                {
                    mScore += mMap.Width * 2;
                    EmptyRowAndMoveDown(y);
                }
            }
        }
        void EmptyRowAndMoveDown(int y)
        {
            for (int i = 0; i < mMap.Width; i++)
            {
                mMap.Set(i, y, false);
            }

            for (int i = y;i >= 0; i--) 
            { 
                for (int j = 0; j < mMap.Width; j++)
                {
                    if (i-1>= 0)
                    {
                        mMap.Set(j, i, mMap.Get(j,i-1));
                        mMap.Set(j, i - 1, false);
                    }
                }
            }
        }
        void SpawnNewTile()
        {
            mTileX = mMap.Width / 2 - 2;
            mTileY = 0;
            mCurrentTile = mNextTile;
            Random random = new Random();
            mNextTile = mTiles[random.Next(0, mTiles.Count)];
            if (Collide(0,0))
            {
                mRunning = false;
            }
        }
        void HandleInput()
        {
            mInput = ConsoleKey.None;
            while (Console.KeyAvailable)
            {
                Console.CursorVisible = false;
                mInput = Console.ReadKey().Key;


                if (mSettings["LEFT"] == mInput)
                {
                    if (!Collide(-1,0))
                        mTileX -= 1;
                }
                else if (mSettings["RIGHT"] == mInput)
                {
                    if (!Collide(1,0))
                        mTileX += 1;
                }
                else if (mSettings["DOWN"] == mInput)
                {
                    mForceTileDown = true;
                    mForceTileDownSw.Start();
                }
                else if (mSettings["ROTATE"] == mInput)
                {
                    Rotate();
                }
                else if (mSettings["Quit"] == mInput)
                {
                    mRunning = false;
                }
            }

        }
        void Rotate()
        {
            Array2D<bool> temp = new Array2D<bool>(mCurrentTile.Height, mCurrentTile.Width);
            Array2D<bool> backup = new Array2D<bool>(mCurrentTile.Height, mCurrentTile.Width);

            backup = mCurrentTile;

            for (int x = 0; x < mCurrentTile.Width; x++)
            {
                for (int y = 0; y < mCurrentTile.Height; y++)
                {
                    temp.Set(y, x, mCurrentTile.Get(x, y));
                }
            }

            mCurrentTile = temp;
            temp = new Array2D<bool>(mCurrentTile.Height, mCurrentTile.Width);

            for (int x = 0; x < mCurrentTile.Width; x++)
            {
                for (int y = 0; y < mCurrentTile.Height; y++)
                {
                    temp.Set(mCurrentTile.Width - (x + 1), y, mCurrentTile.Get(x, y));
                }
            }
            mCurrentTile = temp;
            if (Collide(0,0))
            {
                mCurrentTile = backup;
            }
        }
        private void CreateShapes()
        {
            mTiles.Add(new Array2D<bool>( // L
            [false,true,false,false,
             false,true,false,false,
             false,true,false,false,
             false,true,true,false,],4,4));
            mTiles.Add(new Array2D<bool>( // J
            [false,false,true,false,
             false,false,true,false,
             false,false,true,false,
             false,true,true,false,],4,4));
            mTiles.Add(new Array2D<bool>( // I
            [false,false,true,false,
             false,false,true,false,
             false,false,true,false,
             false,false,true,false,],4,4));
            mTiles.Add(new Array2D<bool>( // O
            [false,false,false,false,
             false,true,true,false,
             false,true,true,false,
             false,false,false,false,],4,4));
            mTiles.Add(new Array2D<bool>( // T
            [false,false,false,false,
             false,true,true,true,
             false,false,true,false,
             false,false,false,false,],4,4));
            mTiles.Add(new Array2D<bool>( // S
            [false,false,false,false,
             false,false,true,true,
             false,true,true,false,
             false,false,false,false,],4,4));
            mTiles.Add(new Array2D<bool>( // Z
            [false,false,false,false,
             false,true,true,false,
             false,false,true,true,
             false,false,false,false,],4,4));
        }
    }
}
