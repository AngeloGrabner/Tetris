public static class Display // to be added
{
    private static CHAR_INFO[] f;
    private static int width = 10*2+1, height = 20;
    static Display()
    {
        f = new CHAR_INFO[width*height];
        ColorSupport.setup(width,height);
    }

    public static void update(char[,] input)
    {
        draw();
        ColorSupport.displayFrame(f);
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

    }
    private static int convert(int x, int y) // makes a 2d coordinate into a 1d one 
    {
        return y * width + x; 
    }
}
