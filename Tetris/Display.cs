public static class Display // 90 degrees roration for some reason
{
    private static bool errorFlag = false;
    private static NewColors color = new();
    private const char fullBlock = '\u2588'; 
    private static char[,] s_input = new char[0,0];
    private static CHAR_INFO[] f;
    private static int width = 10*2+1, height = 20;
    static Display()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        f = new CHAR_INFO[width*height];
        ColorSupport.setup(width,height);
        if (width <= Console.LargestWindowWidth && height <= Console.LargestWindowHeight)
        {
            #pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Console.WindowHeight = height;
            Console.WindowWidth = width;
            #pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        }
    }

    public static void update(char[,] input)
    {
        s_input = input;
        draw();
        if (!errorFlag)
        {
            ColorSupport.displayFrame(f);
        }
        else
        {
            throw new Exception("Display error");
        }
    }
    private static void draw()
    {
        for (int i = 0; i < f.Length; i++) // clear old 
        {
            f[i].UnicodeChar = ' ';
            f[i].Attributes = (ushort)ConsoleColor.White;
        }
        for (int i = width-1;i<f.Length;i+=width)
        {
            f[i].UnicodeChar = '\n';
        }

        for (int x = 0; x < s_input.GetLength(0); x++)
        {
            for (int y = 0; y < s_input.GetLength(1); y++)
            {
                if (s_input[x,y] != ' ' && s_input[x,y] != '\n')
                {
                    f[convert(x * 2, y)].UnicodeChar = fullBlock; //left side 
                    f[convert((x * 2) + 1, y)].UnicodeChar = fullBlock; // right side 
                    switch (s_input[x,y])
                    {
                        case 'I':
                            color = NewColors.lightBlue;
                            break;
                        case 'J':
                            color = NewColors.pink;
                            break;
                        case 'L':
                            color = NewColors.orange;
                            break;
                        case 'O':
                            color= NewColors.yellow;
                            break;
                        case 'S':
                            color = NewColors.red;
                            break;
                        case 'Z':
                            color = NewColors.green;
                            break;
                        case 'T':
                            color = NewColors.purple;
                            break;
                        case 'X':
                        default:
                            color = NewColors.white;
                            break;
                    }
                    f[convert(x * 2, y)].Attributes = (ushort)color;
                    f[convert((x * 2)+1, y)].Attributes = (ushort)color;
                }
            }
        }
    }
    private static int convert(int x, int y) // makes a 2d coordinate into a 1d one 
    {
        return y * width + x; 
    }
}
