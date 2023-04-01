using ConsoleGame.Entities.Alive.Heroes;
using System;
using System.Drawing;
using System.Threading;

namespace ConsoleGame
{
    internal class UserInterface
    {
        private readonly Hero _hero;
        private Timer _timer;

        public UserInterface(int arenaWidth, ref Hero hero, int time)
        {
            _hero = hero;
            _hero.OnDie += (a, b) => ShowDieMessage();
            DrawInterface(arenaWidth);
            ChangingDateTimeInMS = time;
            SetDayTimer();
            UpdateDayInterface();
        }

        public delegate void Time(int day);
        public event Time OnDayChanged;

        private (int X, int Y) _curPoint;

        public (Point leftTop, Point rightBottom) CornersCoords { get; private set; }

        public int Score { get; set; }

        public int Day { get; set; } = 1;

        public int ChangingDateTimeInMS { get; private set; } = 60_000;

        private void DrawInterface(int arenaWidth)
        {
            int offset = 5;

            Console.SetCursorPosition(arenaWidth + offset, 0);
            Console.Write(new string('▄', 52));
            for (int i = 1; i <= 5; i++)
            {
                Console.SetCursorPosition(arenaWidth + offset, i);
                Console.Write('▐' + new string(' ', 50) + '▌');
            }
            Console.SetCursorPosition(arenaWidth + offset, 6);
            Console.Write(new string('▀', 52));

            CornersCoords = (new Point(arenaWidth + offset, 0), new Point(arenaWidth + offset + 52, 7));

            UpdateUI();
        }

        public void UpdateUI()
        {
            ClearUI();

            _curPoint = (CornersCoords.leftTop.X + 5, CornersCoords.rightBottom.Y / 2);
            Draw($"$ {Score}");
            _curPoint = (Console.CursorLeft + 3, Console.CursorTop);
            Draw($"♥ {_hero.HealthPoints}");
            _curPoint = (Console.CursorLeft + 3, Console.CursorTop);
            Draw($"DP: {_hero.DefensePoints}");
        }

        public void UpdateDayInterface()
        {
            _curPoint = (CornersCoords.leftTop.X + 5, CornersCoords.rightBottom.Y + 2);
            Draw($"Day: {Day}");
            OnDayChanged?.Invoke(Day);
        }

        private void ClearUI()
        {
            Console.SetCursorPosition(CornersCoords.leftTop.X + 1, CornersCoords.rightBottom.Y / 2);
            Console.Write(new string(' ', 50));
        }

        private void Draw(string text)
        {
            Console.SetCursorPosition(_curPoint.X, _curPoint.Y);
            Console.Write(text);
        }

        private void SetDayTimer()
        {
            TimerCallback tcb = new TimerCallback(obj =>
            {
                ++Day;
                UpdateDayInterface();
            });
            _timer = new Timer(tcb, null, ChangingDateTimeInMS, ChangingDateTimeInMS);
        }

        private void StopDayChanging() => _timer.Dispose();

        private void ShowDieMessage()
        {
            StopDayChanging();
            StartTimer();
        }

        private void StartTimer()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            Console.Clear();
            string message = "MISSION FAILED";
            string score = "Score: " + Score;

            Random rnd = new Random();
            TimerCallback tcb = new TimerCallback(obj =>
            {
                if (Console.WindowWidth != width || Console.WindowHeight != height)
                {
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    Console.Clear();
                }

                Console.ForegroundColor = GetRandomColor(ref rnd);
                GlobalParameters.ShowInCenterScreen(message, -1);
                GlobalParameters.ShowInCenterScreen(score, 1);
            });

            Timer timer = new Timer(tcb, null, 0, 1000);

            Console.ReadKey(true);
        }

        private ConsoleColor GetRandomColor(ref Random random) => (ConsoleColor)random.Next(1, 16);

        public string Data => $"{ChangingDateTimeInMS}-{Day}-{Score}";
    }
}