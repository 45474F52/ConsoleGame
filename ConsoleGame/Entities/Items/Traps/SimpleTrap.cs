using System;

namespace ConsoleGame.Entities.Items.Traps
{
    /// <summary>
    /// Представляет сущность ловушку
    /// </summary>
    internal class SimpleTrap : Entity
    {
        /// <summary>
        /// Создаёт ловушку с передаваемыми символом, цветом и очками урона
        /// </summary>
        /// <param name="character">Символ сущности</param>
        /// <param name="color">Цвет сущности</param>
        /// <param name="dmgP">Очки урона</param>
        public SimpleTrap(char character, ConsoleColor color, float dmgP) : base(character, color) => DamagePoints = dmgP;

        /// <summary>
        /// Очки урона
        /// </summary>
        [DataToSave(3)]
        public float DamagePoints { get; set; }
    }
}