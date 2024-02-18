using System;
using System.Linq;
using static ConsoleGame.Globals;

namespace ConsoleGame.Global.GUI
{
    internal static class ConsoleGUI
    {
        public const string Arrow = "► ";

        public static void StartChoosingActionCycle(
            ActionGUI[] actions,
            string title,
            bool breakeInEscape = true,
            Action<int> onPrintAction = null,
            params int[] breakerActions)
        {
            actions[0].title = Arrow + actions[0].title;
            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen(title, 0, -5);
                for (int i = 0; i < actions.Length; i++)
                {
                    onPrintAction?.Invoke(i);
                    ShowInCenterScreen(actions[i].title, selected == i ? -1 : 0, i - 1);
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    if (selected < actions.Length - 1)
                    {
                        ++selected;
                        actions[selected - 1].title = actions[selected - 1].title.Remove(0, 2);
                    }
                    else
                    {
                        selected = default;
                        actions[actions.Length - 1].title = actions[actions.Length - 1].title.Remove(0, 2);
                    }

                    actions[selected].title = Arrow + actions[selected].title;
                }
                else if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    if (selected > 0)
                    {
                        --selected;
                        actions[selected + 1].title = actions[selected + 1].title.Remove(0, 2);
                    }
                    else
                    {
                        selected = actions.Length - 1;
                        actions[0].title = actions[0].title.Remove(0, 2);
                    }

                    actions[selected].title = Arrow + actions[selected].title;
                }
                else if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    actions[selected].action.Invoke();
                    if (AllActionsIsBreakers(breakerActions) || breakerActions.Contains(selected))
                        break;
                }
                else if (key == ConsoleKey.Escape && breakeInEscape)
                    break;
            }
        }

        private static bool AllActionsIsBreakers(params int[] actions)
        {
            if (actions.Length != 0 && actions.First() == -1)
                return true;
            return false;
        }
    }
}