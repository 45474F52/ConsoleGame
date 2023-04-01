using System;

namespace ConsoleGame
{
    internal static class GlobalParameters
    {
        public static int UpdateUITimeInMS { get; set; } = 100;

        public static void ShowInCenterScreen(string line, int heightOffset = 0)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) - (line.Length / 2), (Console.WindowHeight / 2) + heightOffset);
            Console.Write(line);
        }
    }
}