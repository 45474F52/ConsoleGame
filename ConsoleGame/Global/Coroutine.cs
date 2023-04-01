using System;
using System.Threading.Tasks;

namespace ConsoleGame
{ 
    internal static class Coroutine
    {
        public static async Task Invoke(Action action, int delayBefore) => await Invoke(action, delayBefore, 0);

        public static async Task Invoke(Action action, int delayBefore, int delayAfter)
        {
            await Task.Delay(delayBefore);

            await Task.Run(() => action());

            await Task.Delay(delayAfter);
        }
    }
}