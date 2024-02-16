using System;
using static ConsoleGame.Globals;

namespace ConsoleGame
{
    internal static class PauseMenu
    {
        private const string Arrow = "► ";
        private static bool _paused = false;

        public delegate void Pause();
        public static event Pause OnPause;
        public static event Pause OnResume;

        public static void Invoke()
        {
            if (_paused)
                return;
            else
                Show();
        }

        private static void Show()
        {
            _paused = true;
            OnPause?.Invoke();
            DrawMenu();
        }

        private static void DrawMenu()
        {
            string[] actions = new string[]
            {
                Arrow + "Resume",
                "Shop",
                "Menu"
            };

            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("PAUSE", 0, -10);

                for (int i = 0; i < actions.Length; i++)
                    ShowInCenterScreen(actions[i], selected == i ? -1 : 0, i - 1);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    if (selected < actions.Length - 1)
                    {
                        ++selected;
                        actions[selected - 1] = actions[selected - 1].Remove(0, 2);
                    }
                    else
                    {
                        selected = default;
                        actions[actions.Length - 1] = actions[actions.Length - 1].Remove(0, 2);
                    }

                    actions[selected] = Arrow + actions[selected];
                }
                else if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    if (selected > 0)
                    {
                        --selected;
                        actions[selected + 1] = actions[selected + 1].Remove(0, 2);
                    }
                    else
                    {
                        selected = actions.Length - 1;
                        actions[0] = actions[0].Remove(0, 2);
                    }

                    actions[selected] = Arrow + actions[selected];
                }
                else if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    switch (selected)
                    {
                        case 0:
                            Hide();
                            break;
                        case 1:
                            //CharacterSettings();
                            break;
                        case 2:
                            //GameSettings();
                            break;
                    }
                }
                else if (key == ConsoleKey.Escape)
                    Hide();
            }
        }

        private static void Hide()
        {
            _paused = false;
            Console.Clear();
            OnResume?.Invoke();
        }
    }
}