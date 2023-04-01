using System.Drawing;

namespace ConsoleGame.Entities
{
    /// <summary>
    /// Представляет метод, определяющий, может ли сущность переместиться на указанную позицию
    /// </summary>
    /// <param name="point">Точка, куда сущность хочет переместиться</param>
    /// <returns></returns>
    public delegate bool IsEmpty(Point point);

    /// <summary>
    /// Представляет метод для отрисовки символа на указанной позиции
    /// </summary>
    /// <param name="point">Точка, где нужно отрисовать символ</param>
    /// <param name="obj">Символ для отрисовки</param>
    public delegate void Draw(Point point, char obj);

    /// <summary>
    /// Представляет метод и события для сущности, которая может перемещаться
    /// </summary>
    internal interface IMovable
    {
        /// <summary>
        /// Метод перемещения сущности на указанную позицию
        /// </summary>
        /// <param name="to">Точка, куда переместится сущность</param>
        void Move(Point to);

        /// <summary>
        /// Возникает при необходимости перемещения сущности
        /// </summary>
        event IsEmpty CanMove;

        /// <summary>
        /// Возникает при перемещении сущности
        /// </summary>
        event Draw OnMove;
    }
}