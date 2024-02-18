using System;
using System.Windows.Input;
using ConsoleGame.Extensions;
using ConsoleGame.Global;
using ConsoleGame.Global.Saving;

namespace ConsoleGame
{
    /// <summary>
    /// Инкапсулирует свойства и методы взаимодействия с игрой
    /// </summary>
    internal sealed class Game
    {
        static Game() =>
            SaveSystem.SavePath = SettingsSystem.Get("SavePath") as string ?? Environment.CurrentDirectory;

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
            Console.CursorVisible = false;
            Console.Title = WindowTitle;
            Console.SetWindowSize(WindowSize.width, WindowSize.height);
            ConsoleExtensions.SetToCenterScreen();

            Menu = new Menu(SaveSystem.GetData());
        }
    }
}