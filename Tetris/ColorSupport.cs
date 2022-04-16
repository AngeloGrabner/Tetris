using System.Runtime.InteropServices;
using System.Drawing;
// just copied the code from https://github.com/AngeloGrabner/tictactoe-opp/blob/color/tictactoe-opp/ColorSupport.cs + a bit of color manipulation
// win32error: 87 invalid argument (prob. on consolescreenbufferinfoex struct)
public enum NewColors // we dont need more colors 9, are enough
{
   black = 0,
   yellow = 1,
   lightBlue = 2,
   red = 3,
   green = 4,
   orange = 5,
   pink = 6,
   purple = 8,
   white = 15
}
[StructLayout(LayoutKind.Sequential)]
public struct COLORREF
{
    public uint ColorDWORD;

    public COLORREF(Color color)
    {
        ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
    }
    public COLORREF(uint color)
    {
        ColorDWORD = color;
    }
    public COLORREF(uint r, uint g, uint b)
    {
        ColorDWORD = r + (g << 8) + (b << 16);
    }

    internal Color GetColor()
    {
        return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                              (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
    }

    internal void SetColor(Color color)
    {
        ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct CONSOLE_SCREEN_BUFFER_INFO_EX
{
    public int cbSize ;
    public COORD dwSize;
    public COORD dwCursorPosition;
    public ushort wAttributes;
    public SMALL_RECT srWindow;
    public COORD dwMaximumWindowSize;

    public ushort wPopupAttributes;
    public bool bFullscreenSupported;

    public COLORREF black;
    public COLORREF darkBlue;
    public COLORREF darkGreen;
    public COLORREF darkCyan;
    public COLORREF darkRed;
    public COLORREF darkMagenta;
    public COLORREF darkYellow;
    public COLORREF gray;
    public COLORREF darkGray;
    public COLORREF blue;
    public COLORREF green;
    public COLORREF cyan;
    public COLORREF red;
    public COLORREF magenta;
    public COLORREF yellow;
    public COLORREF white;
}
public struct SMALL_RECT
{
    public SMALL_RECT(short Left, short Top, short Right, short Bottom)
    {
        this.Left = Left;
        this.Top = Top;
        this.Right = Right;
        this.Bottom = Bottom;
    }
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;

}
[StructLayout(LayoutKind.Sequential)]
public struct COORD
{
    public short X;
    public short Y;

    public COORD(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
}
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)] //encoding seems to make problems
public struct CHAR_INFO
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(0)]
    public char AsciiChar;
    [FieldOffset(2)] //2 bytes seems to work properly
    public UInt16 Attributes;
}
internal static class ColorSupport
{
    private static CONSOLE_SCREEN_BUFFER_INFO_EX conScreBufInfo; // great variable naming
    private static SMALL_RECT _small_rect;
    private static int _width = 20, _height = 10;
    private const int genericWrite = -11;
    private static IntPtr outputHandle;
    private static bool errorFlag = false;
    private static List<int> errors = new();
    static ColorSupport()
    {
        outputHandle = GetStdHandle(genericWrite);
        conScreBufInfo = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        setup(_width,_height);
        changeColors();
    }
    public static void setup(int width, int height) // width and height of the buffer thats going to be displaied
    {
        _width = width;
        _height = height;
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("width and/or height must me grater 0");
        }
        _small_rect = new SMALL_RECT(0, 0, (short)_width, (short)_height);
    }
    public static void displayFrame(CHAR_INFO[] input)
    {
        if (errorFlag)
        {
            return;
        }
        if (!WriteConsoleOutput(outputHandle, input, new COORD((short)_width, (short)_height), new COORD(0, 0), ref _small_rect))
        {
            errorFlag = true;
            errors.Add(Marshal.GetLastWin32Error());
            Console.WriteLine("Latest Win32Error: " + errors[errors.Count-1]);
        }
    }
    private static void changeColors()
    {
        conScreBufInfo.cbSize = Marshal.SizeOf<CONSOLE_SCREEN_BUFFER_INFO_EX>(conScreBufInfo);
        if (GetConsoleScreenBufferInfoEx(outputHandle, ref conScreBufInfo))
        {        
            //we'll see how the RGB values look in the game!
            conScreBufInfo.darkBlue =  new(Color.Yellow); // yellow for O
            conScreBufInfo.darkGreen = new(Color.SkyBlue); // light blue for I
            conScreBufInfo.darkCyan = new(Color.Crimson);  // red for S
            conScreBufInfo.darkRed = new(Color.LimeGreen);   // green for Z
            conScreBufInfo.darkMagenta = new(Color.Orange); // orange for L
            conScreBufInfo.darkYellow = new(Color.HotPink);  // pink for J
            conScreBufInfo.darkGray = new(Color.Violet);  // purple for T
            conScreBufInfo.cbSize = Marshal.SizeOf<CONSOLE_SCREEN_BUFFER_INFO_EX>(conScreBufInfo);
            if (!SetConsoleScreenBufferInfoEx(outputHandle, ref conScreBufInfo))
            {
                errors.Add(Marshal.GetLastWin32Error());
                Console.WriteLine("error: " + errors[errors.Count-1]);
            }
        }
        else
        {
            errors.Add(Marshal.GetLastWin32Error());
            Console.WriteLine("error: " + errors[errors.Count-1]);
        }
    }
    public static List<int> getAllErrors()
    {
        return errors;
    }





    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleScreenBufferInfoEx(
    IntPtr hConsoleOutput,
    ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfo
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleScreenBufferInfoEx(
    IntPtr ConsoleOutput,
    ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfoEx
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] //this has done the trick
    private static extern bool WriteConsoleOutput(
    IntPtr hConsoleOutput,
    CHAR_INFO[] lpBuffer,
    COORD dwBufferSize,
    COORD dwBufferCoord,
    ref SMALL_RECT lpWriteRegion
    );
}