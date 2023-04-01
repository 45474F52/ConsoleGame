using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleGame.Input
{
    /// <summary>
    /// Представляет систему нажатия на комбинации клавиш
    /// </summary>
    internal static class InputSystem
    {
        /// <summary>
        /// Словарь действий <see cref="Inputs"/> и соответсвующих комбинаций клавиш
        /// </summary>
        public static Dictionary<Inputs, GlobalHotKey> Bindings = new Dictionary<Inputs, GlobalHotKey>()
        {
            { Inputs.Save, new GlobalHotKey(new HotKey(ConsoleKey.S, ConsoleModifiers.Control), () => { }) },
            { Inputs.TurnLeft, new GlobalHotKey(new HotKey(ConsoleKey.A, 0), () => { }) },
            { Inputs.TurnUp, new GlobalHotKey(new HotKey(ConsoleKey.W, 0), () => { }) },
            { Inputs.TurnRight, new GlobalHotKey(new HotKey(ConsoleKey.D, 0), () => { }) },
            { Inputs.TurnDown, new GlobalHotKey(new HotKey(ConsoleKey.S, 0), () => { }) },
            { Inputs.Attack, new GlobalHotKey(new HotKey(ConsoleKey.Spacebar, 0), () => { }) }
        };

        public static async Task<string> GetBindingsDataAsync()
        {
            StringBuilder data = new StringBuilder();

            await Task.Run(() =>
            {
                foreach (var kv in Bindings)
                    data.AppendFormat("{0}-{1}{2}", kv.Key, kv.Value, Environment.NewLine);
            });

            return data.ToString();
        }
    }

    /// <summary>
    /// Действия, связанные с комбинацией клавиш
    /// </summary>
    public enum Inputs
    {
        /// <summary>
        /// Нет действия
        /// </summary>
        None = 0,

        /// <summary>
        /// Сохранить игру
        /// </summary>
        Save,

        /// <summary>
        /// Движение налево
        /// </summary>
        TurnLeft,

        /// <summary>
        /// Движение вверх
        /// </summary>
        TurnUp,

        /// <summary>
        /// Движение направо
        /// </summary>
        TurnRight,

        /// <summary>
        /// Движение вниз
        /// </summary>
        TurnDown,

        /// <summary>
        /// Атака
        /// </summary>
        Attack
    }
}