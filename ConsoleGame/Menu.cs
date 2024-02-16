using ConsoleGame.Extensions;
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
        private const string Arrow = "► ";

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
                Actions();
            else
            {
                string date = new string(progress.TakeWhile(c => c != SaveSystem.DataSeparator).ToArray()).Replace(Environment.NewLine, string.Empty);
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
            string[] actions = new string[]
            {
                Arrow + "Start game!", 
                "Character settings",
                "Game settings"
            };

            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen(Console.Title, 0, -5);

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
                            return;
                        case 1:
                            CharacterSettings();
                            break;
                        case 2:
                            GameSettings();
                            break;
                    }
                }
            }
        }

        private void GameSettings()
        {
            string[] actions = new string[]
            {
                Arrow + "Day changing system",
                "Inputs system",
                "Save system"
            };

            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Choose action:", 0, -5);
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
                            DayChangingSystem();
                            break;
                        case 1:
                            InputsSystem();
                            break;
                        case 2:
                            SavesSystem();
                            break;
                    }
                }
                else if (key == ConsoleKey.Escape)
                    break;
            }
        }

        private void InputsSystem()
        {
            string[] actions = InputSystem.GetBindingsDataAsync().Result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            actions[0] = Arrow + actions[0];
            for (int i = 0; i < actions.Length; i++)
                actions[i] = actions[i].Replace("0 + ", string.Empty).Replace("-", " - ");

            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Choose action type:", 0, -5);

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
                            actions[selected] = Arrow + ((Inputs)selected + 1) + " - " + input.ToString().Replace("0 + ", string.Empty);
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
            string[] actions = new string[]
            {
                "Change path to save file",
                "Delete save file"
            };

            bool first = true;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Choose action:", 0, -5);
                ShowInCenterScreen(first ? Arrow + actions[0] : actions[0], first ? -1 : 0, -1);
                ShowInCenterScreen(!first ? Arrow + actions[1] : actions[1], !first ? -1 : 0, 0);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow || key == ConsoleKey.S || key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                    first = !first;
                else if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    switch (first)
                    {
                        case true:
                            ChangePath();
                            break;
                        case false:
                            DeleteSave();
                            break;
                    }
                }
                else if (key == ConsoleKey.Escape)
                    break;
            }
        }

        private void DeleteSave()
        {
            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Delete save file?", 0, -5);
                ShowInCenterScreen(Arrow + "Confirm", -1, -1);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    if (File.Exists(SaveSystem.SavePath))
                        File.Delete(SaveSystem.SavePath);
                    break;
                }
                else if (key == ConsoleKey.Escape)
                    break;
            }
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
                var key = Console.ReadKey(false).KeyChar;

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
                var key = Console.ReadKey(false).KeyChar;
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
            int colorsCount = 6;
            int selected = default;

            while (true)
            {
                Console.Clear();

                ShowInCenterScreen("Choose character`s color:", 0, -5);
                Console.ForegroundColor = ConsoleColor.White;
                ShowInCenterScreen(selected == 0 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 0 ? -1 : 0, -1);
                Console.ForegroundColor = ConsoleColor.Green;
                ShowInCenterScreen(selected == 1 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 1 ? -1 : 0, 0);
                Console.ForegroundColor = ConsoleColor.Blue;
                ShowInCenterScreen(selected == 2 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 2 ? -1 : 0, 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                ShowInCenterScreen(selected == 3 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 3 ? -1 : 0, 2);
                Console.ForegroundColor = ConsoleColor.Magenta;
                ShowInCenterScreen(selected == 4 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 4 ? -1 : 0, 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                ShowInCenterScreen(selected == 5 ? Arrow + HeroCharacter.ToString() : HeroCharacter.ToString(), selected == 5 ? -1 : 0, 4);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.DownArrow || key == ConsoleKey.S)
                {
                    if (selected < colorsCount - 1)
                        ++selected;
                    else
                        selected = default;
                }
                else if (key == ConsoleKey.UpArrow || key == ConsoleKey.W)
                {
                    if (selected > 0)
                        --selected;
                    else
                        selected = colorsCount - 1;
                }
                else if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    switch (selected)
                    {
                        case 0:
                            HeroColor = ConsoleColor.White;
                            return;
                        case 1:
                            HeroColor = ConsoleColor.Green;
                            return;
                        case 2:
                            HeroColor = ConsoleColor.Blue;
                            return;
                        case 3:
                            HeroColor = ConsoleColor.Yellow;
                            return;
                        case 4:
                            HeroColor = ConsoleColor.Magenta;
                            return;
                        case 5:
                            HeroColor = ConsoleColor.Cyan;
                            return;
                    }
                }
                else if (key == ConsoleKey.Escape)
                    return;
            }
        }
    }
}