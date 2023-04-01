using System;

namespace ConsoleGame
{
    /// <summary>
    /// Атрибут, используемый для сохранения свойства сущности
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class DataToSaveAttribute : Attribute
    {
        /// <summary>
        /// Определяет то, как свойство будет использоваться в операциях сохранения игры и загрузки сохранения
        /// </summary>
        /// <param name="indexInConstructor">Позиция свойства в числе передаваемых параметров в конструктор сущности.
        /// Если значение -1, то свойста нет среди параметров конструктора
        /// <br>По умолчанию значение равно -1</br></param>
        public DataToSaveAttribute(int indexInConstructor = -1) => IndexOnConstructor = indexInConstructor;

        /// <summary>
        /// Определяет позицию свойства в числе передаваемых параметров в конструктор сущности
        /// </summary>
        /// <remarks>Указание значения для этого свойства не имеет смысла, если свойство <see cref="UsedInConstructor"/>
        /// равно <see langword="false"/></remarks>
        public int IndexOnConstructor { get; }
    }
}