using System;

namespace ConsoleGame
{
    internal static class Globals
    {
        public static int UpdateUITimeInMS { get; set; } = 100;

        public static void ShowInCenterScreen(string line, int widthOffset = 0, int heightOffset = 0)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) - (line.Length / 2) + widthOffset, (Console.WindowHeight / 2) + heightOffset);
            Console.Write(line);
        }
    }
}