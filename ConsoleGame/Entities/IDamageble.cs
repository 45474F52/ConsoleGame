using System;
using System.Drawing;

namespace ConsoleGame.Entities
{
    /// <summary>
    /// Представляет метод, получающий текущий цвет сущности и новый, для эффекта вспышки
    /// </summary>
    /// <param name="oldColor">Текущий цвет</param>
    /// <param name="newColor">Новый цвет</param>
    public delegate void Flash(ConsoleColor oldColor, ConsoleColor newColor);

    /// <summary>
    /// Представляет методы и события для сущности, которой можно нанести урон
    /// </summary>
    internal interface IDamageble
    {
        /// <summary>
        /// Метод получения урона и возможности отбросить сущность
        /// </summary>
        /// <param name="damage">Урон</param>
        /// <param name="newPosition">Позиция после отбрасывания</param>
        void IncreaseDamage(float damage, Point newPosition);

        /// <summary>
        /// Метод получения урона
        /// </summary>
        /// <param name="damage">Урон</param>
        void IncreaseDamage(float damage);

        /// <summary>
        /// Возникает при получении урона
        /// </summary>
        event Flash OnDamaged;

        /// <summary>
        /// Возникает при смерти сущности
        /// </summary>
        event Flash OnDie;
    }
}