using System;

namespace ConsoleGame.Global.Saving
{
    /// <summary>
    /// Атрибут, используемый для сохранения свойства сущности
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class DataToSaveAttribute : Attribute
    {
        /// <summary>
        /// Определяет то, как свойство будет использоваться в операциях сохранения игры и загрузки сохранения
        /// </summary>
        /// <param name="indexInConstructor">Позиция свойства в числе передаваемых параметров в конструктор сущности
        /// <br/>Если значение = -1, то свойста нет среди параметров конструктора
        /// <br/>По умолчанию значение равно -1</param>
        public DataToSaveAttribute(int indexInConstructor = -1) => IndexOnConstructor = indexInConstructor;

        /// <summary>
        /// Определяет позицию свойства в числе передаваемых параметров в конструктор сущности
        /// </summary>
        public int IndexOnConstructor { get; }
    }
}
