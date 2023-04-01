using ConsoleGame.Extensions;
using ConsoleGame.Input;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static ConsoleGame.Menu;

namespace ConsoleGame.Entities.Alive.Heroes
{
    /// <summary>
    /// Представляет игрока
    /// </summary>
    internal class Hero : AliveEntity, IMovable
    {
        /// <summary>
        /// Создаёт игрока, передавая цвет и основную часть свойств сущности через объект <see cref="Character"/>
        /// </summary>
        /// <param name="character">Объект свойств игрока</param>
        /// <param name="color">Цвет сущности</param>
        public Hero(Character character, ConsoleColor color) : this(character.Char, color, character.HP, character.DMGP, character.DFP, character.ATKS) { }

        /// <summary>
        /// Создаёт игрока с передаваемыми параметрами для инициализации сущности
        /// </summary>
        /// <param name="char">Символ сущности</param>
        /// <param name="color">Цвет сущности</param>
        /// <param name="HP">Очки жизни</param>
        /// <param name="DMGP">Очки урона</param>
        /// <param name="DFP">Очки защиты</param>
        /// <param name="ATKS">Задержка после атаки</param>
        public Hero(char @char, ConsoleColor color, float HP, float DMGP, float DFP, int ATKS) : base(@char, color, HP, DMGP, DFP)
        {
            this.ATKS = ATKS;
            InitializeInputs();
        }

        /// <summary>
        /// Представляет метод для проведения операции взаимодействия игрока с другой сущностью
        /// </summary>
        /// <remarks>Тип взаимодействия определяется свойством <see cref="InteractionType"/></remarks>
        /// <param name="targetPosition">Позиция сущности, с которой нужно провзаимодействовать</param>
        /// <param name="value">Объект, используется при взаимодействии</param>
        public delegate void Interaction(Point targetPosition, object value);

        /// <summary>
        /// Возникает при необходимости взаимодействия игрока с другой сущностью
        /// </summary>
        public event Interaction OnInteracting;
        public virtual event IsEmpty CanMove;
        public virtual event Draw OnMove;

        /// <summary>
        /// Возникает при смене направления игрока
        /// </summary>
        public virtual event Draw OnDirChanged;

        /// <summary>
        /// Время задержки после атаки
        /// </summary>
        [DataToSave(5)]
        public int ATKS { get; private set; }

        private bool _mayInput = true;
        private bool _mayAttack = true;

        private Direction _direction;
        /// <summary>
        /// Направление игрока
        /// </summary>
        [DataToSave]
        public Direction Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    OnDirChanged?.Invoke(Position, Character);
                }
            }
        }

        /// <summary>
        /// Тип текущего взаимодействия игрока с другой сущностью
        /// </summary>
        public InteractionType InteractionType { get; private set; }

        /// <summary>
        /// Инициализирует список клавиш методами, которые будут выполняться при нажатии этих клавиш
        /// </summary>
        private void InitializeInputs()
        {
            InputSystem.Bindings[Inputs.TurnLeft].Callback = TurnLeft;
            InputSystem.Bindings[Inputs.TurnUp].Callback = TurnUp;
            InputSystem.Bindings[Inputs.TurnRight].Callback = TurnRight;
            InputSystem.Bindings[Inputs.TurnDown].Callback = TurnDown;
            InputSystem.Bindings[Inputs.Attack].Callback = Attack;
        }

        /// <summary>
        /// Запускает игрока в действие
        /// </summary>
        public void Run()
        {
            while (HealthPoints > 0f)
            {
                if (Console.KeyAvailable && _mayInput)
                {
                    _mayInput = false;
                    InputCooldown(GlobalParameters.UpdateUITimeInMS);
                    HotKey key = (HotKey)Console.ReadKey(true);

                    InputSystem.Bindings.Values.FirstOrDefault(g => g.HotKey == key)?.Callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Метод перемещения игрока налево
        /// </summary>
        private void TurnLeft()
        {
            Character = '◄';
            Direction = Direction.Left;
            Move(new Point(Position.X - 1, Position.Y));
        }

        /// <summary>
        /// Метод перемещения игрока вверх
        /// </summary>
        private void TurnUp()
        {
            Character = '▲';
            Direction = Direction.Up;
            Move(new Point(Position.X, Position.Y - 1));
        }

        /// <summary>
        /// Метод перемещения игрока вправо
        /// </summary>
        private void TurnRight()
        {
            Character = '►';
            Direction = Direction.Right;
            Move(new Point(Position.X + 1, Position.Y));
        }

        /// <summary>
        /// Метод перемещения игрока вниз
        /// </summary>
        private void TurnDown()
        {
            Character = '▼';
            Direction = Direction.Down;
            Move(new Point(Position.X, Position.Y + 1));
        }

        /// <summary>
        /// Метод нанесения урона
        /// </summary>
        private void Attack()
        {
            if (_mayAttack)
            {
                _mayAttack = false;
                AttackCooldown(ATKS);
                InteractionType = InteractionType.Attacking;

                if (PositionNotNull(out Point targetPos))
                    OnInteracting?.Invoke(targetPos, DamagePoints);

                InteractionType = InteractionType.None;
            }
        }

        /// <summary>
        /// Выполняет задержку возможности нажатия клавиши
        /// </summary>
        /// <param name="delay">Время задержки</param>
        private void InputCooldown(int delay)
        {
            Task.Run(async () => await Coroutine.Invoke(() => _mayInput = true, delay));
        }

        /// <summary>
        /// Выполняет задержку возможности нанесения урона
        /// </summary>
        /// <param name="delay"></param>
        private void AttackCooldown(int delay)
        {
            Task.Run(async () => await Coroutine.Invoke(() => _mayAttack = true, delay));
        }

        /// <summary>
        /// Определяет то, является ли ячейка перед игроком не пустой и возвращает координаты этой ячейки
        /// </summary>
        /// <remarks>Выбор ячейки перед игроком зависит от его направления <see cref="Direction"/></remarks>
        /// <param name="targetPos"></param>
        /// <returns>Возвращает <see langword="true"/>, если ячейка занята перед игроком, иначе <see langword="false"/></returns>
        private bool PositionNotNull(out Point targetPos)
        {
            targetPos = Position;
            switch (Direction)
            {
                case Direction.Left:
                    targetPos = new Point(Position.X - 1, Position.Y);
                    break;
                case Direction.Right:
                    targetPos = new Point(Position.X + 1, Position.Y);
                    break;
                case Direction.Up:
                    targetPos = new Point(Position.X, Position.Y - 1);
                    break;
                case Direction.Down:
                    targetPos = new Point(Position.X, Position.Y + 1);
                    break;
            }
            return !char.IsWhiteSpace(ConsoleExtensions.ReadChar(((short, short))(targetPos.X, targetPos.Y)));
        }

        public void Move(Point to)
        {
            if (CanMove?.Invoke(to) ?? false)
            {
                OnMove?.Invoke(Position, ' ');
                Position = to;
                OnMove?.Invoke(Position, Character);
            }
        }
    }

    /// <summary>
    /// Типы взаимодействия игрока с сущностями
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// Нет взаимодействия
        /// </summary>
        None,

        /// <summary>
        /// Взаимодействие атаки
        /// </summary>
        Attacking,

        /// <summary>
        /// Взаимодействие открытия/зарытия
        /// </summary>
        OpenClose
    }

    /// <summary>
    /// Возможные направления игрока
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Налево
        /// </summary>
        Left,

        /// <summary>
        /// Направо
        /// </summary>
        Right,

        /// <summary>
        /// Вверх
        /// </summary>
        Up,

        /// <summary>
        /// Вниз
        /// </summary>
        Down
    }
}