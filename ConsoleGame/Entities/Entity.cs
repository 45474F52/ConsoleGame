using ConsoleGame.Global.Saving;
using System;
using System.Drawing;

namespace ConsoleGame.Entities
{
    /// <summary>
    /// Представляет базовый класс сущности
    /// </summary>
    internal abstract class Entity
    {
        /// <summary>
        /// Создаёт сущность с указанным символом и цветом
        /// </summary>
        /// <param name="character">Символ сущности</param>
        /// <param name="color">Цвет сущности</param>
        protected Entity(char character, ConsoleColor color)
        {
            Character = character;
            SkinColor = color;
        }

        /// <summary>
        /// Символ сущности
        /// </summary>
        [DataToSave(0)]
        public char Character { get; protected internal set; }

        /// <summary>
        /// Цвет сущности
        /// </summary>
        [DataToSave(1)]
        public ConsoleColor SkinColor { get; protected internal set; }

        /// <summary>
        /// Позиция сущности в консоли
        /// </summary>
        [DataToSave]
        public Point Position { get; set; }

        public override string ToString() => $"{Character}:{SkinColor}:{Position}";
    }

    /// <summary>
    /// Возможные направления сущности
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