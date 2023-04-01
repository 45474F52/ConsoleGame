using ConsoleGame.Input;
using ConsoleGame.Global;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConsoleGame
{
    internal sealed class Menu
    {
        public sealed class Character
        {
            public Character(char @char, float hP, float dmgP, float dFP, int attackSpeed, string description = "—")
            {
                Char = @char;
                HP = hP;
                DMGP = dmgP;
                DFP = dFP;
                ATKS = attackSpeed;
                Description = description;
            }

            public char Char { get; }
            public float HP { get; }
            public float DMGP { get; }
            public float DFP { get; }
            public int ATKS { get; }
            public string Description { get; }

            public override string ToString()
            {
                return
                    $"\t{Char}{Environment.NewLine}" +
                    $"HP: {HP}{Environment.NewLine}" +
                    $"DMGP: {DMGP}{Environment.NewLine}" +
                    $"DFP: {DFP}{Environment.NewLine}" +
                    $"Description: {Description}{Environment.NewLine}";
            }
        }

        private readonly ConsoleColor _oldColor = Console.ForegroundColor;

        public readonly Arena Arena = null;
        public readonly UserInterface UI = null;

        public char HeroCharacter { get; private set; } = '▲';
        public ConsoleColor HeroColor { get; private set; } = ConsoleColor.White;
        public Character SelectedCharacter { get; private set; } = new Character('▲', 100f, 20f, 15f, 500);
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
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Choose action:");
                Console.WriteLine("0\tReturn\n1\tCharacter settings\n2\tGame settings\n3\tStart game!\n");
                if (int.TryParse(Console.ReadLine(), out int action))
                {
                    switch (action)
                    {
                        case 0:
                            Console.Clear();
                            break;
                        case 1:
                            Console.Clear();
                            CharacterSettings();
                            break;
                        case 2:
                            Console.Clear();
                            GameSettings();
                            break;
                        case 3:
                            Console.Clear();
                            return;
                        default:
                            Wrong();
                            break;
                    }
                }
                else
                {
                    Wrong();
                }
            }
        }

        private void GameSettings()
        {
        GameSettings:
            while (true)
            {
                Console.WriteLine("Choose action:");
                Console.WriteLine("0\tReturn\n1\tReturn to game settings\n2\tDays changing\n3\tInputs\n4\tSave system\n");
                if (int.TryParse(Console.ReadLine(), out int action))
                {
                    switch (action)
                    {
                        case 0:
                            return;
                        case 1:
                            Console.Clear();
                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("Write time of changing the days\n\tor write \"0\\1\" for return\n");
                            if (int.TryParse(Console.ReadLine(), out int value))
                            {
                                switch (value)
                                {
                                    case 0:
                                        return;
                                    case 1:
                                        Console.Clear();
                                        goto GameSettings;
                                    default:
                                        ChangingDateTime = value;
                                        goto case 1;
                                }
                            }
                            else break;
                        case 3:
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine("Write position and value for changing\nex:\n  1-G\n  2-Control-R\n\n\tor write \"0\\1\" for return\n\n");
                                string spaces = new string(' ', InputSystem.Bindings.Keys.Select(i => i.ToString().Length).OrderByDescending(l => l).First());
                                for (int i = 0; i < InputSystem.Bindings.Count; i++)
                                {
                                    var pair = InputSystem.Bindings.ElementAt(i);
                                    Console.WriteLine("{0}: {1}{2}{3}",
                                        i, pair.Key,
                                        new string(' ', spaces.Length + (spaces.Length - pair.Key.ToString().Length)),
                                        pair.Value);
                                }

                                Console.WriteLine();
                                string pattern = Console.ReadLine();
                                if (int.TryParse(pattern, out int num))
                                {
                                    switch (num)
                                    {
                                        case 0:
                                            return;
                                        case 1:
                                            Console.Clear();
                                            goto GameSettings;
                                        default:
                                            Wrong();
                                            break;
                                    }
                                }
                                else
                                {
                                    string[] pair = pattern.Trim().Split('-');
                                    if (pair.Length >= 2 && pair.Length <= 3)
                                    {
                                        Inputs key = InputSystem.Bindings.ElementAt(int.Parse(pair[0])).Key;
                                        HotKey hotKey = new HotKey((ConsoleKey)Enum.Parse(typeof(ConsoleKey), pair[pair.Length - 1]), 0);
                                        if (pair.Length == 3)
                                            hotKey.ConsoleModifiers = (ConsoleModifiers)Enum.Parse(typeof(ConsoleModifiers), pair[1]);
                                        InputSystem.Bindings[key].HotKey = hotKey;
                                    }
                                    else break;
                                }
                            }
                            break;
                        case 4:
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine("Choose action:\n0\tReturn\n1\tReturn to game settings\n2\tChange path to save\n3\tDelete saves");
                                if (int.TryParse(Console.ReadLine(), out int action2))
                                {
                                    switch (action2)
                                    {
                                        case 0:
                                            return;
                                        case 1:
                                            Console.Clear();
                                            goto GameSettings;
                                        case 2:
                                            Console.WriteLine("\nPath:\t{0}\nWrite new path:", SaveSystem.SavePath);
                                            string path = Console.ReadLine();
                                            try
                                            {
                                                SaveSystem.SavePath = path;
                                            }
                                            catch (ArgumentException ex)
                                            {
                                                Wrong(ex.Message);
                                            }
                                            goto case 1;
                                        case 3:
                                            Console.WriteLine("\nPath\t{0}\nDelete all saves? y/n", SaveSystem.SavePath);
                                            string answ = Console.ReadLine();
                                            if (answ.ToLower() == "y")
                                                if (File.Exists(SaveSystem.SavePath))
                                                    File.Delete(SaveSystem.SavePath);
                                                else if (answ.ToLower() == "n")
                                                    goto case 1;
                                                else
                                                    Wrong();
                                            break;
                                        default:
                                            Wrong();
                                            break;
                                    }
                                }
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                Wrong();
            }
        }

        private void CharacterSettings()
        {
            Console.WriteLine("Choose character`s color:\n\tor write \"0\" for return\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{HeroCharacter}\tw");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{HeroCharacter}\tg");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{HeroCharacter}\tb");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{HeroCharacter}\ty");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{HeroCharacter}\tm");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{HeroCharacter}\tc");
            Console.WriteLine();
            Console.ForegroundColor = _oldColor;
            string color = string.Empty;
            string[] colors = new[] { "w", "g", "b", "y", "m", "c" };
            bool isValid = false;
            while (!isValid)
            {
                color = Console.ReadLine();
                if (color == "w" || color == "g" || color == "b" || color == "y" || color == "m" || color == "c")
                {
                    isValid = true;
                }
                else
                {
                    bool parsed = int.TryParse(color, out int value);
                    if (value <= colors.Length && value > 0)
                    {
                        color = colors[value - 1];
                        isValid = true;
                    }
                    else if (value == 0 && parsed)
                        return;
                }
            }

            switch (color)
            {
                case "w":
                    HeroColor = ConsoleColor.White;
                    break;
                case "g":
                    HeroColor = ConsoleColor.Green;
                    break;
                case "b":
                    HeroColor = ConsoleColor.Blue;
                    break;
                case "y":
                    HeroColor = ConsoleColor.Yellow;
                    break;
                case "m":
                    HeroColor = ConsoleColor.Magenta;
                    break;
                case "c":
                    HeroColor = ConsoleColor.Cyan;
                    break;
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        private void Wrong(string message = "Wrong answer!")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Thread.Sleep(1000);
            Console.Clear();
        }
    }
}