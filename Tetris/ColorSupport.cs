using System.Runtime.InteropServices;
// just copied the code from https://github.com/AngeloGrabner/tictactoe-opp/blob/color/tictactoe-opp/ColorSupport.cs + a bit of color manipulation
public enum NewColors // we dont need more colors 9, are enough
{
   black = 0,
   yellow = 8,
   lightBlue = 2,
   red = 3,
   green = 4,
   orange = 1,
   pink = 11,
   purple = 10,
   white = 15
}

[StructLayout(LayoutKind.Sequential)]
public struct CONSOLE_SCREEN_BUFFER_INFO_EX
{
    public uint cbSize;
    public COORD dwSize;
    public COORD dwCursorPosition;
    public short wAttributes;
    public SMALL_RECT srWindow;
    public COORD dwMaximumWindowSize;

    public ushort wPopupAttributes;
    public bool bFullscreenSupported;

    internal uint black; // hopfully works as a uint insteat of COLORREF
    internal uint darkBlue;
    internal uint darkGreen;
    internal uint darkCyan;
    internal uint darkRed;
    internal uint darkMagenta;
    internal uint darkYellow;
    internal uint gray;
    internal uint darkGray;
    internal uint blue;
    internal uint green;
    internal uint cyan;
    internal uint red;
    internal uint magenta;
    internal uint yellow;
    internal uint white;
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
    private static int _width = 0, _height = 0;
    private const int outputHandle = -11;
    private static IntPtr handle;
    static ColorSupport()
    {
        handle = GetStdHandle(outputHandle);
        conScreBufInfo = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        changeColors();
    }
    public static void setup(int width, int height) // width and height of the buffer thats going to be displaied
    {
        _width = width;
        _height = height;
        if (!(width > 0 && height > 0))
        {
            throw new ArgumentException("width and/or height must me grater 0");
        }
        _small_rect = new SMALL_RECT(0, 0, (short)_width, (short)_height);
    }
    public static void displayFrame(CHAR_INFO[] input)
    {
        if (!WriteConsoleOutput(handle, input, new COORD((short)_width, (short)_height), new COORD(0, 0), ref _small_rect))
        {
            Console.WriteLine("Latest Win32Error: " + Marshal.GetLastWin32Error());
        }
    }
    private static void changeColors()
    {
        if (GetConsoleScreenBufferInfoEx(handle, ref conScreBufInfo))
        {
            //we'll see how the RGB values look in the game!
            conScreBufInfo.darkGray = 0x00FFFF00;     // yellow for O
            conScreBufInfo.darkGreen = 0x0033DDFF;   // light blue for I
            conScreBufInfo.darkCyan = 0x00FF0000;   // red for S
            conScreBufInfo.darkRed = 0x0026E600;   // green for Z
            conScreBufInfo.darkBlue = 0x00FFAA00; // orange for L
            conScreBufInfo.cyan = 0x00FF00FF;    // pink for J
            conScreBufInfo.green = 0x008800CC;  // purple for T
            if (!SetConsoleScreenBufferInfoEx(handle, ref conScreBufInfo))
            {
                Console.WriteLine("error: " + Marshal.GetLastWin32Error());
            }
        }
        else
        {
            Console.WriteLine("error: " + Marshal.GetLastWin32Error());
        }
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