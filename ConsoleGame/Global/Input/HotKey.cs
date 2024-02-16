using System;

namespace ConsoleGame.Global.Input
{
    /// <summary>
    /// Представляет комбинацию клавиш, нажатой в консоли
    /// </summary>
    internal sealed class HotKey
    {
        /// <summary>
        /// Создаёт комбинацию клавиш из типов нажатых стандартной и управляющей клавишей
        /// </summary>
        /// <param name="key">Стандартная клавиша</param>
        /// <param name="modifiers">Управляющая клавиша</param>
        public HotKey(ConsoleKey key, ConsoleModifiers modifiers)
        {
            ConsoleKey = key;
            ConsoleModifiers = modifiers;
        }

        /// <summary>
        /// Стандартная клавиша
        /// </summary>
        public ConsoleKey ConsoleKey { get; private set; }

        /// <summary>
        /// Управляющая клавиша
        /// </summary>
        public ConsoleModifiers ConsoleModifiers { get; set; }

        /// <summary>
        /// Оператор преобразования <see cref="ConsoleKeyInfo"/> в <see cref="HotKey"/>
        /// </summary>
        /// <param name="info">Информация о нажатой комбинации клавиш, полученной из метода консоли <see cref="Console.ReadKey"/></param>
        public static explicit operator HotKey(ConsoleKeyInfo info) => new HotKey(info.Key, info.Modifiers);

        /// <summary>
        /// Определяет равны ли комбинации клавиш
        /// </summary>
        /// <param name="left">Комбинация клавиш в условии слева</param>
        /// <param name="right">Комбинация клавиш в условии справа</param>
        /// <returns>Возвращает <see langword="true"/>, если комбинации клавиш равны, иначе <see langword="false"/></returns>
        public static bool operator ==(HotKey left, HotKey right) =>
            left.ConsoleKey.Equals(right.ConsoleKey) && left.ConsoleModifiers.Equals(right.ConsoleModifiers);

        /// <summary>
        /// Определяет не равны ли комбинации клавиш
        /// </summary>
        /// <param name="left">Комбинация клавиш в условии слева</param>
        /// <param name="right">Комбинация клавиш в условии справа</param>
        /// <returns>Возвращает <see langword="true"/>, если комбинации клавиш не равны, иначе <see langword="false"/></returns>
        public static bool operator !=(HotKey left, HotKey right) =>
            !left.ConsoleKey.Equals(right.ConsoleKey) || !left.ConsoleModifiers.Equals(right.ConsoleModifiers);

        public override string ToString() => $"{ConsoleModifiers} + {ConsoleKey}";

        public override bool Equals(object obj)
        {
            if (obj is HotKey hk)
            {
                if (hk.ConsoleKey.Equals(this.ConsoleKey) && hk.ConsoleModifiers.Equals(this.ConsoleModifiers))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -1119532059;
            hashCode = hashCode * -1521134295 + ConsoleKey.GetHashCode();
            hashCode = hashCode * -1521134295 + ConsoleModifiers.GetHashCode();
            return hashCode;
        }
    }
}