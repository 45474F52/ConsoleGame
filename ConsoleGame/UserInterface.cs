using ConsoleGame.Entities.Alive.Heroes;
using System;
using System.Drawing;
using System.Threading;

namespace ConsoleGame
{
    /// <summary>
    /// Пользовательский интерфейс
    /// </summary>
    internal class UserInterface
    {
        private readonly Hero _hero;
        private readonly int _arenaWidth;
        private Timer _timer;

        /// <summary>
        /// Создаёт экземпляр интерфейса
        /// </summary>
        /// <param name="arenaWidth"></param>
        /// <param name="hero"></param>
        /// <param name="time"></param>
        public UserInterface(int arenaWidth, ref Hero hero, int time)
        {
            _hero = hero;
            _arenaWidth = arenaWidth;
            _hero.OnDie += (a, b) => ShowDieMessage();
            DrawInterface(arenaWidth);
            ChangingDateTimeInMS = time;
            SetDayTimer();
            UpdateDayInterface();
        }

        public delegate void Time(int day);
        /// <summary>
        /// Возникает при смене дня
        /// </summary>
        public event Time OnDayChanged;

        private (int X, int Y) _curPoint;

        /// <summary>
        /// Координаты углов рамки интерфейса
        /// </summary>
        /// <remarks>leftTop - левый вверхний угол, rightBottom - правый нижний угол</remarks>
        public (Point leftTop, Point rightBottom) CornersCoords { get; private set; }

        /// <summary>
        /// Очки, набранные игроком
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Текущий игровой день
        /// </summary>
        public int Day { get; set; } = 1;

        /// <summary>
        /// Продолжительность дня в миллисекундах
        /// </summary>
        /// <remarks>По умолчанию = 60 000 (1 минута)</remarks>
        public int ChangingDateTimeInMS { get; private set; } = 60_000;

        /// <summary>
        /// ОТрисовывает интерфейс в консоли
        /// </summary>
        /// <param name="arenaWidth">Ширина арены</param>
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

        /// <summary>
        /// Обновляет отображаемую об игроке информацию
        /// </summary>
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

        /// <summary>
        /// Обновляет отображаемый игровой день
        /// </summary>
        public void UpdateDayInterface()
        {
            _curPoint = (CornersCoords.leftTop.X, CornersCoords.rightBottom.Y + 2);
            Draw($"Day: {Day}");
            OnDayChanged?.Invoke(Day);
        }

        /// <summary>
        /// Полностью очищает отображаемую об игроке информацию
        /// </summary>
        private void ClearUI()
        {
            Console.SetCursorPosition(CornersCoords.leftTop.X + 1, CornersCoords.rightBottom.Y / 2);
            Console.Write(new string(' ', 50));
        }

        /// <summary>
        /// Отрисовывает строку в консоли, используя текущую позицию курсора <see cref="_curPoint"/>
        /// </summary>
        /// <param name="text">Текст для отрисовки</param>
        private void Draw(string text)
        {
            Console.SetCursorPosition(_curPoint.X, _curPoint.Y);
            Console.Write(text);
        }

        /// <summary>
        /// Инициализирует таймер <see cref="_timer"/> для смены игрового дня
        /// </summary>
        private void SetDayTimer()
        {
            TimerCallback tcb = new TimerCallback(obj =>
            {
                ++Day;
                UpdateDayInterface();
            });
            _timer = new Timer(tcb, null, ChangingDateTimeInMS, ChangingDateTimeInMS);
        }

        /// <summary>
        /// Останавливает таймер <see cref="_timer"/> и освобождает ресурсы объекта
        /// </summary>
        public void StopDayChanging() => _timer.Dispose();

        /// <summary>
        /// Останавливает смену игрового дня и отображает сообщение о проигрыше
        /// </summary>
        private void ShowDieMessage()
        {
            StopDayChanging();
            ShowScreen();
        }

        /// <summary>
        /// Отображает сообщение о проигрыше
        /// </summary>
        private void ShowScreen()
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
                Globals.ShowInCenterScreen(message, heightOffset: -1);
                Globals.ShowInCenterScreen(score, heightOffset: 1);
            });

            Timer timer = new Timer(tcb, null, 0, 1000);

            Console.ReadKey(true);
        }

        /// <summary>
        /// Возвращает случайный цвет для консоли, кроме чёрного
        /// </summary>
        /// <returns>Рандомный цвет <see cref="ConsoleColor"/></returns>
        private ConsoleColor GetRandomColor(ref Random random) => (ConsoleColor)random.Next(1, 16);

        /// <summary>
        /// Возвращает данные для сохранения
        /// </summary>
        public string Data => $"{ChangingDateTimeInMS}-{Day}-{Score}";

        public void ReDraw()
        {
            DrawInterface(_arenaWidth);
            SetDayTimer();
            UpdateDayInterface();
        }
    }
}