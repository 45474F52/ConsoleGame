using ConsoleGame.Entities;
using ConsoleGame.Extensions;
using ConsoleGame.Entities.Items.Traps;
using ConsoleGame.Entities.Alive.Heroes;
using ConsoleGame.Entities.Alive.Monsters;
using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ConsoleGame
{
    internal sealed class Arena
    {
        private static readonly Random _rnd;

        private const char Up = '-';//'▄';
        private const char Down = '-';//'▀';
        private const char Left = '|';//'▐';
        private const char Right = '|';//'▌';

        private const int FlashDelay = 350;

        private UserInterface _UI;

        public List<Entity> Entities { get; private set; }

        static Arena() => _rnd = new Random();

        public Arena(int width = 50, int height = 20, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.CursorVisible = false;

            Width = width;
            Height = height;
            Color = color;

            Entities = new List<Entity>();

            Center = new Point(Width / 2, Height / 2);

            DrawArena();
        }

        //public delegate void TakeDamage(float damage, Point newPos);

        public ConsoleColor Color { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Point Center { get; }

        /// <summary>
        /// Инициализирует свойство интерфейса игрока <see cref="_UI"/>
        /// </summary>
        /// <param name="ui">Интерфейс</param>
        public void SetUI(UserInterface ui) => _UI = ui;

        /// <summary>
        /// Рисует зону арены в консоли
        /// </summary>
        private void DrawArena()
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;

            Console.WriteLine(new string(Up, Width + 2));
            for (int h = 1; h <= Height; h++)
                Console.WriteLine(Left + new string(' ', Width) + Right);
            Console.WriteLine(new string(Down, Width + 2));

            Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Проверяет пуст ли символ по переданной координате
        /// </summary>
        /// <param name="point">Координата символа</param>
        /// <returns>Возвращает <see langword="true"/>, если на координате <paramref name="point"/> пустой символ, иначе <see langword="false"/></returns>
        private bool IsEmpty(Point point) =>
            char.IsWhiteSpace(ConsoleExtensions.ReadChar(((short, short))(point.X, point.Y)));

        /// <summary>
        /// Находит из списка сущность по её позиции в консоли
        /// </summary>
        /// <param name="point">Позиция сущности</param>
        /// <returns>Возвращает <see cref="Entity"/> из списка <see cref="Entities"/>,
        /// если объект найден по переданной позиции, иначе <see langword="null"/></returns>
        /// <exception cref="InvalidOperationException"/>
        private Entity GetEntity(Point point) => Entities.SingleOrDefault(e => e.Position == point);

        /// <summary>
        /// Добавляет сущность на арену
        /// </summary>
        /// <param name="entity">Объект сущности</param>
        /// <param name="position">Позиция сущности в консоли</param>
        public void Instantiate(Entity entity, Point position)
        {
            entity.Position = position;
            DrawIn(entity.Position, entity.Character, entity.SkinColor);
            Entities.Add(entity);
        }

        /// <summary>
        /// Отрисовывает в консоли символ с определённым цветом по указанной позиции
        /// </summary>
        /// <param name="point">Позиция, где должен отрисоваться символ</param>
        /// <param name="obj">Символ</param>
        /// <param name="color">Цвет символа</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void DrawIn(Point point, char obj, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.SetCursorPosition(point.X, point.Y);
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(obj);
            Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Создаёт эффект вспышки у символа
        /// </summary>
        /// <param name="point">Позиция символа</param>
        /// <param name="obj">Символ</param>
        /// <param name="oldColor">Цвет символа перед вспышкой</param>
        /// <param name="flashColor">Цвет символа во время вспышки</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void FlashIn(Point point, char obj, ConsoleColor oldColor, ConsoleColor flashColor)
        {
            DrawIn(point, obj, flashColor);
            Thread.Sleep(FlashDelay);
            DrawIn(point, obj, oldColor);
        }

        /// <summary>
        /// Выполняет действие над сущностью
        /// </summary>
        /// <param name="targetPosition">Позиция сущности</param>
        /// <param name="intType">Тип взаимодействия</param>
        /// <param name="value">Объект, необходимый для взаимодействия</param>
        /// <exception cref="InvalidOperationException"/>
        public void Interaction(Point targetPosition, InteractionType intType, object value)
        {
            Entity entity = GetEntity(targetPosition);
            switch (intType)
            {
                case InteractionType.Attacking:
                    if (entity is IDamageble damagebleEntity)
                        damagebleEntity.IncreaseDamage((float)value);
                    break;
                case InteractionType.OpenClose:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Определяет может ли игрок передвинуться на указанную позицию
        /// </summary>
        /// <param name="hero">Игрок</param>
        /// <param name="point">Целевая позиция</param>
        /// <returns>Возвращает <see langword="true"/>, если позиция свободна, иначе <see langword="false"/></returns>
        public bool CanMoveTo(ref Hero hero, Point point)
        {
            bool withinBorderX = point.X <= Width && point.X > 0;
            bool withinBorderY = point.Y <= Height && point.Y > 0;

            if (withinBorderX && withinBorderY)
            {
                if (IsEmpty(point))
                    return true;
                else
                {
                    if (GetEntity(point) is SimpleTrap trap)
                        hero.IncreaseDamage(trap.DamagePoints);
                }
            }

            return false;
        }

        /// <summary>
        /// Удаляет сущность
        /// </summary>
        /// <param name="entity">Объект сущности</param>
        private void DeleteEntity(Entity entity)
        {
            DrawIn(entity.Position, ' ');
            Entities.Remove(entity);
        }

        /// <summary>
        /// Возвращает рандомную свободную позицию на арене
        /// </summary>
        /// <param name="redraw">Можно ли использовать занятую позицию для перерисовки сущности</param>
        /// <returns>Возвращает свободную позицию на арене для сущности</returns>
        private Point GetRandomPosition(bool redraw = false)
        {
            Point rndPoint = Center;
            if (redraw)
            {
                do
                    rndPoint = new Point(_rnd.Next(1, Width + 1), _rnd.Next(1, Height + 1));
                while (Entities.FirstOrDefault(e => e.Position == rndPoint) is Monster && Entities.Count < Width * Height);
            }
            else
            {
                do
                    rndPoint = new Point(_rnd.Next(1, Width + 1), _rnd.Next(1, Height + 1));
                while (Entities.Any(e => e.Position == rndPoint) && Entities.Count < Width * Height);
            }

            return rndPoint;
        }

        /// <summary>
        /// Добавляет сущности в определённый игровой день
        /// </summary>
        /// <param name="day">День</param>
        public void SetEntities(int day)
        {
            if (day % 10 == 0)
            {
                Monster monster = new Monster('B', ConsoleColor.Gray, 300f, 50f, 100f) { IsBoss = true };
                SimpleTrap trap = new SimpleTrap('X', ConsoleColor.DarkRed, 50f);
                InitializeMonsterEvents(monster, 50);
                Instantiate(monster, GetRandomPosition(true));
                Instantiate(trap, GetRandomPosition());
            }
            else
            {
                int count = _rnd.Next(1, (day % 10 == 5) ? 5 : 2);
                
                for (int i = 0; i < count; i++)
                {
                    Monster monster = new Monster('M', ConsoleColor.DarkGreen, 100f, 25f, 30f);
                    InitializeMonsterEvents(monster);
                    Instantiate(monster, GetRandomPosition());
                }
            }
        }

        /// <summary>
        /// Инициализирует события для нового объекта <see cref="Monster"/>
        /// </summary>
        /// <param name="monster">Объект сущности</param>
        /// <param name="score">Очки, которые начислятся игроку, который убьёт эту сущность. По умолчанию = 10</param>
        public void InitializeMonsterEvents(Monster monster, int score = 10)
        {
            monster.OnDamaged += (oldColor, newColor) => FlashIn(monster.Position, monster.Character, oldColor, newColor);
            monster.OnDie += (oldColor, newColor) =>
            {
                _UI.Score += score;
                _UI.UpdateUI();
                DeleteEntity(monster);
            };
        }

        /// <summary>
        /// Возвращает информацию об объектах арены для их сохранения
        /// </summary>
        /// <remarks>Шаблон записи:<br/>
        /// &lt;EntityTypeFullName&gt;: [ConstructorProperties &lt;FullTypeName-Value&gt;]; [PublicProperties &lt;Name-Value&gt;]
        /// </remarks>
        /// <returns>Возвращает в строковом представлении информацию о свойствах и конструкторах сущностей арены
        /// для их восстановления при загрузке сохранения</returns>
        public async Task<string> GetDataAsync()
        {
            StringBuilder builder = new StringBuilder();

            await Task.Run(() =>
            {
                foreach (var item in Entities)
                {
                    Type type = item.GetType();
                    IEnumerable<PropertyInfo> ctorProps = new Collection<PropertyInfo>();
                    IEnumerable<PropertyInfo> savedProps = new Collection<PropertyInfo>();

                    IEnumerable<(PropertyInfo prop, DataToSaveAttribute attr)> enumerable() =>
                        from prop in type.GetProperties().Where(p => p.GetCustomAttribute<DataToSaveAttribute>() != null)
                        let attr = prop.GetCustomAttribute<DataToSaveAttribute>()
                        select (prop, attr);

                    foreach (var (prop, attr) in enumerable())
                    {
                        if (attr.IndexOnConstructor >= 0)
                            ctorProps = ctorProps.Append(prop);
                        else
                            savedProps = savedProps.Append(prop);
                    }

                    ctorProps = ctorProps.OrderBy(p => p.GetCustomAttribute<DataToSaveAttribute>().IndexOnConstructor);

                    StringBuilder propText = new StringBuilder();
                    foreach (var prop in ctorProps)
                    {
                        propText.Append($" {prop.PropertyType.FullName}-{prop.GetValue(item)}");
                    }

                    if (savedProps.Any())
                    {
                        propText.Append(';');
                        foreach (var prop in savedProps)
                        {
                            propText.Append($" {prop.Name}-{prop.GetValue(item)}");
                        }
                    }

                    builder.AppendLine($"{type.FullName}:{propText}");
                }
            });

            return builder.ToString();
        }
    }
}