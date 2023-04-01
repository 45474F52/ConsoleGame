using System;
using ConsoleGame.Global;

namespace ConsoleGame
{
    /// <summary>
    /// Инкапсулирует свойства и методы взаимодействия с игрой
    /// </summary>
    internal sealed class Game
    {
        /// <summary>
        /// Игровое меню
        /// </summary>
        public Menu Menu { get; private set; }

        /// <summary>
        /// Заголовок консоли
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Размер консоли
        /// </summary>
        public (int width, int height) WindowSize { get; set; }

        /// <summary>
        /// Начинает игру. Изменяет консоль и запускает игровое меню (<see cref="ConsoleGame.Menu"/>)
        /// </summary>
        public void Start()
        {
            Console.Title = WindowTitle;
            Console.SetWindowSize(WindowSize.width, WindowSize.height);

            Menu = new Menu(SaveSystem.GetSavedGame());
        }
    }
}