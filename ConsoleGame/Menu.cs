using ConsoleGame.Extensions;
using ConsoleGame.Global.GUI;
using ConsoleGame.Global.Input;
using ConsoleGame.Global.Saving;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using static ConsoleGame.Globals;

namespace ConsoleGame
{
    internal sealed class Menu
    {
        private readonly ConsoleColor _oldColor = Console.ForegroundColor;

        public readonly Arena Arena = null;
        public readonly UserInterface UI = null;

        public char HeroCharacter { get; private set; } = '▲';
        public ConsoleColor HeroColor { get; private set; } = ConsoleColor.White;
        public float HP { get; private set; } = 100f;
        public float DMGP { get; private set; } = 20f;
        public float DFP { get; private set; } = 15f;
        public int ATKS { get; private set; } = 500;

        public int ChangingDateTime { get; private set; } = 10_000;

        public Menu(string progress = "")
        {
            if (string.IsNullOrWhiteSpace(progress))
            {
                Actions();
            }
            else
            {
                string date = new string(progress.TakeWhile(c => c != SaveSystem.DataSeparator).ToArray())
                    .Replace(Environment.NewLine, string.Empty);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Load save \"{0}\" ?\n(y/n)", date);
                    string value = Console.ReadLine();
                    if (value == "y")
                    {
                        Console.Clear();
                        SaveSystem.ParseSavedData(progress, out Arena, out UI);
                        return;
                    }
                    else if (value == "n")
                    {
                        Actions();
                        break;
                    }
                }
            }

            Console.ForegroundColor = _oldColor;
            Console.Clear();
        }

        private void Actions()
        {
            ConsoleGUI.StartChoosingActionCycle(
                    actions: new[]
                    {
                        new ActionGUI("Start game!", delegate { }),
                        new ActionGUI("Character settings", CharacterSettings),
                        new ActionGUI("Game settings", GameSettings)
                    },
                    title: Console.Title,
                    breakerActions: 0);
        }

        private void GameSettings()
        {
            ConsoleGUI.StartChoosingActionCycle(
                actions: new[]
                {
                    new ActionGUI("Day changing system", DayChangingSystem),
                    new ActionGUI("Inputs system", InputsSystem),
                    new ActionGUI("Save system", SavesSystem)
                },
                title: "Choose action:");
        }

        private void InputsSystem()
        {
            string[] actions = InputSystem.GetBindingsDataAsync().Result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            actions[0] = ConsoleGUI.Arrow + actions[0];
            for (int i = 0; i < actions.Length; i++)
                actions[i] = actions[i].Replace("0 + ", string.Empty).Replace("-", " - ");

            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Choose action type:", 0, -5);

                for (int i = 0; i < actions.Length; i++)
                    ShowInCenterScreen(actions[i], selected == i ? -1 : 0, i - 1);

                ConsoleKey key = Console.ReadKey(true).Key;

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

                    actions[selected] = ConsoleGUI.Arrow + actions[selected];
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

                    actions[selected] = ConsoleGUI.Arrow + actions[selected];
                }
                else if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    while (true)
                    {
                        Console.Clear();

                        ShowInCenterScreen("Press keys:", 0, -5);

                        for (int i = 0; i < actions.Length; i++)
                            ShowInCenterScreen(actions[i], selected == i ? -1 : 0, i - 1);
                        Console.WriteLine();
                        Console.CursorVisible = true;
                        var (left, top) = ConsoleExtensions.Center;
                        Console.SetCursorPosition(left, top + actions.Length);
                        HotKey input = (HotKey)Console.ReadKey(true);
                        if (InputSystem.Bindings.Values.FirstOrDefault(g => g.HotKey.Equals(input)) == null)
                        {
                            InputSystem.Bindings[(Inputs)selected + 1].HotKey = input;
                            actions[selected] = ConsoleGUI.Arrow + ((Inputs)selected + 1) + " - " + input.ToString().Replace("0 + ", string.Empty);
                        }
                        break;
                    }

                    Console.CursorVisible = false;
                }
                else if (key == ConsoleKey.Escape)
                    break;
            }
        }

        private void SavesSystem()
        {
            ConsoleGUI.StartChoosingActionCycle(
                actions: new[]
                {
                    new ActionGUI("Change path to save file", ChangePath),
                    new ActionGUI("Delete save file", DeleteSave)
                },
                title: "Choose action:");
        }

        private void DeleteSave()
        {
            ConsoleGUI.StartChoosingActionCycle(
                actions: new[]
                {
                    new ActionGUI("Confirm", () =>
                    {
                        if (File.Exists(SaveSystem.SavePath))
                            File.Delete(SaveSystem.SavePath);
                    })
                },
                title: "Delete save file?",
                breakerActions: 0);
        }

        private void ChangePath()
        {
            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Write new path:", 0, -5);
                ShowInCenterScreen("Path: " + SaveSystem.SavePath, 0, -2);
                Console.WriteLine();
                Console.CursorVisible = true;
                char key = Console.ReadKey(false).KeyChar;

                if (key != (char)ConsoleKey.Escape && key != (char)ConsoleKey.Enter)
                {
                    string path = key + Console.ReadLine();
                    try
                    {
                        SaveSystem.SavePath = path;
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        ShowInCenterScreen(ex.Message);
                        Console.ResetColor();
                        Console.CursorVisible = false;
                        Thread.Sleep(2000);
                    }
                }
                else break;
            }

            Console.CursorVisible = false;
        }

        private void DayChangingSystem()
        {
            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Write delay of changing days in milliseconds:", 0, -5);
                Console.WriteLine();
                Console.CursorVisible = true;
                char key = Console.ReadKey(false).KeyChar;
                if (key != (char)ConsoleKey.Escape && key != (char)ConsoleKey.Enter)
                {
                    string value = key + Console.ReadLine();
                    if (int.TryParse(value, out int time) && time >= 1000 && time <= 86_400_000)
                    {
                        ChangingDateTime = time;
                        break;
                    }
                }
                else break;
            }
            Console.CursorVisible = false;
        }

        private void CharacterSettings()
        {
            Array colors = Enum.GetValues(typeof(ConsoleColor));
            ActionGUI[] actions = new ActionGUI[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                ConsoleColor color = (ConsoleColor)colors.GetValue(i);
                actions[i] = new ActionGUI(color.ToString(), () => HeroColor = color);
            }

            ConsoleGUI.StartChoosingActionCycle(actions, "Choose character`s color:", true, x =>
            {
                if (x != 0)
                    Console.ForegroundColor = (ConsoleColor)colors.GetValue(x);
            }, -1);
        }
    }
}