using System;
using System.Threading.Tasks;

namespace ConsoleGame
{
    /// <summary>
    /// Представляет статические методы воспроизведения звука в консоли
    /// </summary>
    internal static class SoundPlayer
    {
        /// <summary>
        /// Воспроизводит звук в консоли с заданной частотой и длительностью
        /// </summary>
        /// <param name="frequency">Частота звука</param>
        /// <param name="duration">Длительность звука</param>
        public static void Play(int frequency, int duration) => Task.Run(() => Console.Beep(frequency, duration));

        /// <summary>
        /// Запускает задачу с воспроизведением звука в консоли, описанной в передаваемом методе
        /// </summary>
        /// <param name="sound">Метод, предположительно, воспроизводящий звуки в консоли</param>
        public static void Play(Action sound) => Task.Run(sound);
    }
}