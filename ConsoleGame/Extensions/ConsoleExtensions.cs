using System;
using System.Text;

namespace ConsoleGame.Extensions
{
    internal static class ConsoleExtensions
    {
        public static (int left, int top) Center => (Console.WindowWidth / 2, Console.WindowHeight / 2);

        public static char ReadChar() => ReadChar(((short, short))(Console.CursorLeft, Console.CursorTop));

        public static char ReadChar((short, short) point)
        {
            char[] buffer = new char[1];
            PInvoke.ReadConsoleOutputCharacter(PInvoke.GetStdHandle(STD_HANDLES.STD_OUTPUT_HANDLE),
                                               buffer,
                                               buffer.Length,
                                               new PInvoke.COORD() { X = point.Item1, Y = point.Item2 },
                                               out int readCount);
            return buffer[0];
        }

        public static string ReadRow((short, short) from, int length)
        {
            if (length > Console.BufferWidth)
                throw new ArgumentException(nameof(length), "Длинна для чтения символов не может быть больше длины буфера консоли");

            StringBuilder str = new StringBuilder(length);
            PInvoke.ReadConsoleOutputCharacter(PInvoke.GetStdHandle(STD_HANDLES.STD_OUTPUT_HANDLE),
                                                str,
                                                length,
                                                new PInvoke.COORD() { X = from.Item1, Y = from.Item2 },
                                                out int readCount);
            return str.ToString();
        }

        public static void SetToCenterScreen()
        {
            IntPtr window = PInvoke.GetConsoleWindow();
            int width = PInvoke.GetSystemMetrics(0);
            int height = PInvoke.GetSystemMetrics(1);
            PInvoke.GetWindowRect(window, out PInvoke.RECT rect);
            int cWidth = rect.Right - rect.Left;
            int cHeight = rect.Bottom - rect.Top;
            int newX = (width - cWidth) / 2;
            int newY = (height - cHeight) / 2;
            PInvoke.SetWindowPos(
                window,
                (IntPtr)SWP_CONST.HWND_TOP,
                newX,
                newY,
                0, 0,
                (uint)SWP_CONST.SWP_NOSIZE | (uint)SWP_CONST.SWP_SHOWWINDOW);
        }
    }
}