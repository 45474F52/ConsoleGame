using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleGame.Extensions
{
    internal static class PInvoke
    {
        /// <summary>
        /// Определяет координаты символьного знакоместа в экранном буфере консоли
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            /// <summary>
            /// Значение столбца
            /// </summary>
            public short X;

            /// <summary>
            /// Значение ряда
            /// </summary>
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Копирует ряд символов из последовательных ячеек экранного буфера консоли, начинающихся в заданном месте
        /// </summary>
        /// <param name="hConsoleOutput">Дескриптор экранного буфера</param>
        /// <param name="lpCharacter">Символьный буфер</param>
        /// <param name="nLength">Число ячеек для чтения</param>
        /// <param name="dwReadCoord">Координаты первой ячейки</param>
        /// <param name="lpNumberOfCharsRead">Число прочитанных ячеек</param>
        /// <returns>Возвращает <see langword="true"/>, если чтение прошло успешно</returns>
        [DllImport("kernel32.dll")]
        public static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput,
                                                             [Out] char[] lpCharacter,
                                                             int nLength,
                                                             COORD dwReadCoord,
                                                             out int lpNumberOfCharsRead);
        /// <summary>
        /// Копирует ряд символов из последовательных ячеек экранного буфера консоли, начинающихся в заданном месте
        /// </summary>
        /// <param name="hConsoleOutput">Дескриптор экранного буфера</param>
        /// <param name="lpCharacter">Символьный буфер</param>
        /// <param name="nLength">Число ячеек для чтения</param>
        /// <param name="dwReadCoord">Координаты первой ячейки</param>
        /// <param name="lpNumberOfCharsRead">Число прочитанных ячеек</param>
        /// <returns>Возвращает <see langword="true"/>, если чтение прошло успешно</returns>
        [DllImport("kernel32.dll")]
        public static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput,
                                                             [Out] StringBuilder lpCharacter,
                                                             int nLength,
                                                             COORD dwReadCoord,
                                                             out int lpNumberOfCharsRead);

        /// <summary>
        /// Извлекает дескриптор для стандартного ввода данных, стандартного вывода или стандартной ошибки устройства
        /// </summary>
        /// <param name="nStdHandle">Ввод, вывод или ошибка устройства</param>
        /// <returns>Возвращает дескриптор определяемого устройства, если завершается успешно, иначе флаг INVALID_HANDLE_VALUE</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(STD_HANDLES nStdHandle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }

    public enum STD_HANDLES
    {
        STD_INPUT_HANDLE = -10,
        STD_OUTPUT_HANDLE = -11,
        STD_ERROR_HANDLE = -12,
    }

    public enum SWP_CONST
    {
        SWP_SHOWWINDOW = 0x0040,
        HWND_TOP = 0,
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002
    }
}