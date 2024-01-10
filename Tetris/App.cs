using System.Diagnostics;

namespace Tetris
{
    internal class App
    {
        private Game mGame;
        Menu mMenu;

        bool mRunning = true;
        bool mGameOver = true;
        public App()
        {
            mGame = new Game();
            mMenu = new Menu();
            Console.Title = "Tetris";
        }
        void Startup()
        {
            mMenu.Load();
        }
        public void Run()
        {
            Startup();
            Console.Clear();
            while (mRunning)
            {
                if (mGameOver)
                {
                    switch (mMenu.Update())
                    {
                        case MenuState.NewGame:
                            mGame = new();
                            mGame.Settings = mMenu.GetSettings();
                            mGameOver = false;
                            break;
                        case MenuState.Quit:
                            mRunning = false;
                            break;

                        case MenuState.Running:
                        default:
                            break;
                    }
                }
                else if (!mGame.Update())
                {
                    mMenu.SetScore(mGame.Score);
                    mMenu.GameOver();
                    mGameOver = true;
                }

            }
            Shutdown();
            Console.Clear();
        }
        void Shutdown()
        {
            mMenu.Save();
        }
    }
}
