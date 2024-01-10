using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    struct Array2D<T>
    {
        private T[] mBuffer = new T[0];
        private int mWidth = 0, mHeight = 0;

        public T[] Buffer { get { return mBuffer; } set { mBuffer = value; } }
        public int Width { get { return mWidth; } }
        public int Height { get { return mHeight; } }
        public Array2D(T[] buffer,int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mBuffer = buffer;
        }
        public Array2D(int width, int height)
        {
            mWidth = Math.Max(width,0);
            mHeight = Math.Max(height,0);

            mBuffer = new T[mWidth * mHeight];
        }
        public T? Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= mWidth || y >= mHeight)
                return default(T);
            return mBuffer[x + mWidth *y];
        }
        public void Set(int x, int y, T c)
        {
            if (x < 0 || y < 0 || x >= mWidth || y >= mHeight)
                return;
            mBuffer[x + mWidth * y] = c;
        }
        public void Fill(T c)
        {
            for (int i = 0; i < mWidth * mHeight; i++) 
            {
                mBuffer[i] = c;
            }
        }
    }
    internal class Screen
    {
        static Array2D<char> mArr;

        static public int Width { get { return mArr.Width; } }
        static public int Height { get { return mArr.Height; } }

        static Screen()
        {
            mArr = new Array2D<char>(Console.BufferWidth,Console.BufferHeight - 1);
            Console.CursorVisible = false;
        }
        public static void Clear()
        {
            var w = Console.BufferWidth;
            var h = Console.BufferHeight-1;
            if (w != mArr.Width || h != mArr.Height)
            {
                mArr = new Array2D<char>(w,h);
            }
            mArr.Fill(' ');
        }
        public static void Print(int x, int y,string s)
        {
            for (int X = 0; X < s.Length; X++)
            {
                mArr.Set(X+x, y, s[X]);
            }
        }
        public static void Set(int x, int y, char c)
        {
            mArr.Set(x, y, c);
        }
        public static void Vline(int x, int y, int height, char c)
        {
            if (height < 0)
            {
                y += height;
                height = Math.Abs(height);
            }
            int X = Math.Clamp(x, 0, mArr.Width);

            for (int Y = Math.Max(0, y); Y < Math.Min(height + y, mArr.Height); Y++)
            {
                mArr.Set(X,Y,c);
            }
        }
        public static void Hline(int x, int y, int width, char c)
        {
            if (width < 0) 
            {
                x += width;
                width = Math.Abs(width);
            }
            int Y = Math.Clamp(y, 0, mArr.Height);
            
            for (int X = Math.Max(0, x);X < Math.Min(width+x, mArr.Width); X++)
            {
                mArr.Set(X, Y, c);
            }
        }
        public static void Rect(int x, int y, int width, int height, char c)
        {
            Vline(x,y,height,c);
            Vline(x+width-1,y,height,c);
            Hline(x+1,y,width-2,c);
            Hline(x+1,y+height-1,width-2,c);
        }
        public static void Copy(int x, int y, Array2D<char> arr)
        {
            for (int Y = 0; Y < arr.Height; Y++)
                for (int X = 0; X < arr.Width; X++)
                {
                    mArr.Set(X + x, Y + y, arr.Get(X,Y));
                }
        }
        public static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(mArr.Buffer);
        }
    }
}
