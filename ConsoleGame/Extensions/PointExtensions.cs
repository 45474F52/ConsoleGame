using System;
using System.Linq;
using System.Drawing;

namespace ConsoleGame.Extensions
{
    internal static class PointExtensions
    {
        public static Point FromString(this Point _, string value)
        {
            string[] coords = new string(value.Skip(3).TakeWhile(c => c != '}').ToArray()).Split(new[] { ",Y=" }, StringSplitOptions.None);
            return new Point(int.Parse(coords[0]), int.Parse(coords[1]));
        }
    }
}