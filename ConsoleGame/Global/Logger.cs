using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame.Global
{
    /// <summary>
    /// Предоставляет методы и свойства для логирования
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// Путь до Log-файла
        /// </summary>
        public static string LogPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Log.txt");

        /// <summary>
        /// Выполняет асинхронную запись в файл по пути <see cref="LogPath"/>
        /// </summary>
        /// <param name="message">Текст для записи в log</param>
        public static async Task Log(string message)
        {
            using (FileStream fs = new FileStream(LogPath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}