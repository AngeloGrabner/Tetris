using System.Text;

namespace Tetris
{
    enum MenuState
    {
        Running,
        NewGame,
        Quit
    }
    // warning the fowwlowing code is disgusting, but it works
    internal class Menu
    {
        enum State
        {
            Main,
            GameOver,
            Users,
            AddUser,
            Settings,
            ChangeSetting,
            LAST
        }
        MenuState mRetState = MenuState.Running;
        bool mSelectionMode = true;
        int mSelected = 0;
        ConsoleKey mInput = ConsoleKey.None;
        State mState = State.LAST;
        string[][] mTexts = new string[(int)State.LAST][];
        List<KeyValuePair<string, int>> mUsers;
        int mSelectedUser = 0;
        int mLastScore = -1;
        int mSelectedSetting = 0;
        string mInputText = "";
        readonly string mSettingsPath = "Settings.txt", mUsersPath = "Users.txt";
        List<KeyValuePair<string, ConsoleKey>> mSettings = new List<KeyValuePair<string, ConsoleKey>>();
       
        public Menu()
        {
            mState = State.Main;
            mUsers = new List<KeyValuePair<string, int>>();
            mUsers.Add(new KeyValuePair<string, int>("Default_User", 0));
            CreateTexts();
        }
        public void Load()
        {
            //could've used lambdas and what not here
            {
                if (!File.Exists(mSettingsPath))
                {
                    File.Create(mSettingsPath).Close();
                }
                StreamReader stream = File.OpenText(mSettingsPath);
                string? line;
                do
                {
                    line = stream.ReadLine();
                    var strs = line?.Replace("\0", "").Split(' ');
                    if (strs?.Length == 2)
                    {
                        mSettings.Add(new KeyValuePair<string, ConsoleKey>(strs[0], (ConsoleKey)(strs[1][0])));
                    }
                } while (line != null);
                stream.ReadLine();
                stream.Close();
            }

            if (mSettings.Count == 0)
            {
                LoadDefaultSettings();
            }

            {
                if (!File.Exists(mUsersPath))
                {
                    File.Create(mUsersPath).Close();
                }
                StreamReader stream = File.OpenText(mUsersPath);
                string? line;
                do
                {
                    line = stream.ReadLine();
                    var strs = line?.Split(' ');
                    if (strs?.Length == 2)
                    {
                        mUsers.Add(new KeyValuePair<string, int>(strs[0], int.Parse(strs[1])));
                    }
                } while (line != null);
                stream.ReadLine();
                stream.Close();
            }
            CreateUserTexts();
            CreateSettingsTexts();
        }
        void LoadDefaultSettings()
        {
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("LEFT", ConsoleKey.A));
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("RIGHT", ConsoleKey.D));
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("DOWN", ConsoleKey.S));
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("ROTATE", ConsoleKey.W));
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("PAUSE", ConsoleKey.Spacebar));
            mSettings.Add(new KeyValuePair<string, ConsoleKey>("Quit", ConsoleKey.Escape));
        }
        public void Save()
        {
            {
                string writeMe = string.Empty;
                foreach (var setting in mSettings)
                {
                    writeMe += $"{setting.Key} {(char)setting.Value}\n";
                }
                var stream = File.OpenWrite(mSettingsPath);
                byte[] buff = Encoding.Unicode.GetBytes(writeMe);
                stream?.Write(buff, 0, buff.Length);
                stream?.Close();
            }

            {
                string writeMe = string.Empty;
                foreach (var user in mUsers)
                {
                    if (user.Key != "Default_User")
                        writeMe += $"{user.Key} {user.Value}\n";
                }
                var stream = new StreamWriter(mUsersPath);
                stream?.Write(writeMe);
                stream?.Close();
            }
        }
        public MenuState Update()
        {
            if (mSelectionMode)
            {
                HandleInput();
            }
            else
                HandleTyping();
            Draw();
            return mRetState;
        }
        public void GameOver()
        {
            mRetState = MenuState.Running;
            mState = State.GameOver;
            mSelected = 0;
        }
        void Draw()
        {
            Screen.Clear();
            string[] TETRIS = {
                "  _______     _          _      ",
                " |__   __|   | |        (_)     ",
                "    | |  ___ | |_  _ __  _  ___ ",
               @"    | | / _ \| __|| '__|| |/ __|",
               @"    | ||  __/| |_ | |   | |\__ \",
               @"    |_| \___| \__||_|   |_||___/"
                };
            int offsetY = TETRIS.Length+2;
            for (int i = 0; i < TETRIS.Length; i++)
            {
                Screen.Print(Screen.Width / 2 - TETRIS[0].Length / 2, 1+i, TETRIS[i]);
            }
            if (mSelectionMode)
            {
                Screen.Print(1, offsetY + 1, "Name: " + mUsers[mSelectedUser].Key);
                Screen.Print(1, offsetY + 3, "High score: " + mUsers[mSelectedUser].Value);
                if (mLastScore != -1)
                    Screen.Print(1, offsetY + 4, "Last score: " + mLastScore);
                for (int i = 0; i < mTexts[(int)mState].Length; i++)
                {
                    int x = Screen.Width / 2 - mTexts[(int)mState][i].Length / 2;
                    Screen.Print(x, i + offsetY, mTexts[(int)mState][i]);
                    if (i == mSelected)
                    {
                        Screen.Set(x - 2, i + offsetY, '>');
                        Screen.Set(x + mTexts[(int)mState][i].Length + 1, i + offsetY, '<');
                    }
                }
            }
            else
            {
                string trimedText = mInputText.Trim().Replace(" ", "_").Replace("\0","");
                string ttext = "Press ENTER to confirm";
                int x = Screen.Width / 2 - trimedText.Length / 2;
                Screen.Print(Screen.Width / 2 - ttext.Length / 2, offsetY, ttext);
                Screen.Print(x, offsetY + 3,trimedText);
                Screen.Hline(Screen.Width / 2 - ttext.Length / 2, offsetY + 2, ttext.Length, '-');
                Screen.Hline(Screen.Width / 2 - ttext.Length / 2, offsetY + 4, ttext.Length, '-');
            }
            Screen.Draw();
        }
        void HandleTyping()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                    mSelectionMode = true;
                else if (key.Key == ConsoleKey.Backspace)
                    mInputText = mInputText.Substring(0, mInputText.Length - 1);               
                else
                    mInputText += key.KeyChar;
            }
        }
        void HandleInput()
        {
            mInput = ConsoleKey.None;
            while (Console.KeyAvailable)
            {
                Console.CursorVisible = false;
                mInput = Console.ReadKey().Key;

                if (mInput == ConsoleKey.UpArrow || mInput == ConsoleKey.W)
                {
                    mSelected--;
                }
                else if (mInput == ConsoleKey.DownArrow || mInput == ConsoleKey.S)
                {
                    mSelected++;
                }
                else if (mInput == ConsoleKey.Enter)
                {
                    LoadNextMenu();
                }
                else if (mInput == ConsoleKey.Escape)
                {
                    mState = State.Main;
                }
            }
            if (mSelectionMode)
                mSelected = Math.Clamp(mSelected, 0, mTexts[(int)mState].Length-1);
        }
        void CreateUserTexts()
        {
            mTexts[(int)State.Users] = new string[mUsers.Count+3];
            mTexts[(int)State.Users][0] = "ADD USER";
            mTexts[(int)State.Users][1] = "Go Back";
            mTexts[(int)State.Users][2] = "";
            for (int i = 0; i < mUsers.Count; i++)
            {
                mTexts[(int)State.Users][i+3] = $"{mUsers[i].Key} {mUsers[i].Value}";
            }
        }
        void CreateSettingsTexts()
        {
            mTexts[(int)State.Settings] = new string[mSettings.Count+2];
            mTexts[(int)State.Settings][0] = "Go Back";
            mTexts[(int)State.Settings][1] = "";
            int i = 2;
            foreach (var s in mSettings) 
            {
                mTexts[(int)State.Settings][i] = $"{s.Key}: {s.Value.ToString()}";
                i++;
            }
        }
        private void CreateTexts()
        {
            mTexts[(int)State.Main] = new string[4];

            mTexts[(int)State.Main][0] = "NEW GAME";
            mTexts[(int)State.Main][1] = "USERS";
            mTexts[(int)State.Main][2] = "SETTINGS";
            mTexts[(int)State.Main][3] = "QUIT";


            mTexts[(int)State.GameOver] = new string[2];

            mTexts[(int)State.GameOver][0] = "Try Again";
            mTexts[(int)State.GameOver][1] = "Go Back";


            mTexts[(int)State.AddUser] = new string[1];
            
            mTexts[(int)State.AddUser][0] = "Succsessfully added a user";


            mTexts[(int)State.ChangeSetting] = new string[1];

            mTexts[(int)State.ChangeSetting][0] = "Succsessfully changed Setting";
        }
        void LoadNextMenu()
        {
            switch (mState) 
            { 
                case State.Main:
                    InMain();
                break; 
                
                case State.GameOver:
                    InGameOver();
                break;

                case State.Users:
                    InUsers();
                break;

                case State.AddUser:
                    InAddUser();
                break;

                case State.Settings:
                    InSettings();
                break;

                case State.ChangeSetting:
                    InChnageSetting();
                break;

                case State.LAST:
                default:
                    break;
                
            }
            mSelected = 0;
        }
        void InMain()
        {
            switch (mSelected)
            {
                case 0:
                    mRetState = MenuState.NewGame;
                    break;
                case 1:
                    mState = State.Users;
                    break;
                case 2:
                    mState = State.Settings;
                    break;
                case 3:
                    mRetState = MenuState.Quit;
                    break;

                default:
                    break;
            }
        }
        void InGameOver()
        {
            switch (mSelected)
            {
                case 0:
                    mRetState = MenuState.NewGame;
                    break;
                case 1:
                    mState = State.Main;
                    break;
            }
        }
        void InUsers()
        {
            if (mSelected == 0)
            {
                mSelectionMode = false;
                mInputText = "";
                mState = State.AddUser;
            }
            else if (mSelected == 1)
            {
                mState = State.Main;
            }
            else if (mSelected > 2)
            { 
                mSelectedUser = mSelected-3;
                mState = State.Main;
                mLastScore = -1;
            }
        }
        void InAddUser()
        {  
            string trimedText = mInputText.Trim().Replace(" ", "_").Replace("\0", "");
            mUsers.Add(new KeyValuePair<string, int>(trimedText, 0));
            CreateUserTexts();
            mState = State.Users;
        }
        void InSettings()
        { 
            if (mSelected == 0)
            {
                mState = State.Main;
            }
            else if (mSelected > 1)
            {
                mSelectionMode = false;
                mInputText = "";
                mState = State.ChangeSetting;
                mSelectedSetting = mSelected-2;
            }
        }
        void InChnageSetting()
        {
            mSettings[mSelectedSetting] = new KeyValuePair<string, ConsoleKey>(mSettings[mSelectedSetting].Key,(ConsoleKey)mInputText.ToUpper()[0]);
            mState = State.Settings;
            CreateSettingsTexts();
        }
        public Dictionary<string, ConsoleKey> GetSettings()
        {
            Dictionary<string, ConsoleKey> mDict = new Dictionary<string, ConsoleKey>();
            foreach (var s in mSettings)
            {
                mDict[s.Key] = s.Value;
            }
            return mDict;
        }
        public void SetScore(int score)
        {
            mLastScore = score;
            score = Math.Max(score, mUsers[mSelectedUser].Value);
            mUsers[mSelectedUser] = new KeyValuePair<string, int>(mUsers[mSelectedUser].Key, score);
            CreateUserTexts();
        }
    }
}