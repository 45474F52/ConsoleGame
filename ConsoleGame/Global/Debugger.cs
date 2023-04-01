using System;
using System.IO;
using System.Text;

namespace ConsoleGame.Global
{
    internal static class Debugger
    {
        public static string LogPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Log.txt");

        public static void Log(string message)
        {
            using (FileStream fs = new FileStream(LogPath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }
}