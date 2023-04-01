using System;

namespace ConsoleGame.Input
{
    /// <summary>
    /// Инкапсулирует дополнительную информацию о комбинации клавиш и методе, который должен выполниться при нажатии на комбинацию
    /// </summary>
    internal sealed class GlobalHotKey
    {
        /// <summary>
        /// Создаёт объект, хранящий комбинацию клавиш и метод, который должен выполниться при нажатии на неё
        /// </summary>
        /// <param name="hotKey">Комбинация клавиш</param>
        /// <param name="callback">Ответный метод на нажатие на комбинацию клавиш</param>
        /// <param name="canExecute">Можно ли выполнить ответный метод</param>
        public GlobalHotKey(HotKey hotKey, Action callback, bool canExecute = true)
        {
            HotKey = hotKey;
            Callback = callback;
            CanExecute = canExecute;
        }

        /// <summary>
        /// Комбинация клавиш
        /// </summary>
        public HotKey HotKey { get; set; }

        /// <summary>
        /// Ответный метод на нажатие на комбинацию клавиш
        /// </summary>
        public Action Callback { get; set; }

        /// <summary>
        /// Определяет нажата ли комбинация клавиш
        /// </summary>
        public bool Pressed { get; set; }

        /// <summary>
        /// Определяет можно ли выполнить ответный метод <see cref="Callback"/>
        /// </summary>
        public bool CanExecute { get; set; }

        public override string ToString() => HotKey.ToString();
    }
}