using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    internal static class Debug
    {
        private static string _path;
        public static string Path
        {

        }

        public static async Task Write(string text)
        {
            using (FileStream fs = new FileStream(Path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}