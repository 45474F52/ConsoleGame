using ConsoleGame.Global.Saving;
using System;
using System.Drawing;

namespace ConsoleGame.Entities.Alive
{
    /// <summary>
    /// Представляет базовый класс для сущности, которой можно нанести урон
    /// </summary>
    internal abstract class AliveEntity : Entity, IDamageable
    {
        /// <summary>
        /// Создаёт сущность с передаваемыми символом, цветом, количеством очков жизни, урона и защиты
        /// </summary>
        /// <param name="character">Символ сущности</param>
        /// <param name="color">Цвет сущности</param>
        /// <param name="hp">Очки жизни</param>
        /// <param name="dmgP">Очки урона</param>
        /// <param name="dfsP">Очки защиты</param>
        protected AliveEntity(char character, ConsoleColor color, float hp, float dmgP, float dfsP) : base(character, color)
        {
            HealthPoints = hp;
            DamagePoints = dmgP;
            DefensePoints = dfsP;
        }

        public virtual event Flash OnDamaged;
        public virtual event Flash OnDie;

        /// <summary>
        /// Очки жизни
        /// </summary>
        [DataToSave(2)]
        public float HealthPoints { get; set; }

        /// <summary>
        /// Очки урона
        /// </summary>
        [DataToSave(3)]
        public float DamagePoints { get; set; }

        /// <summary>
        /// Очки защиты
        /// </summary>
        [DataToSave(4)]
        public float DefensePoints { get; set; }

        public void IncreaseDamage(float damage, Point newPosition)
        {
            Position = newPosition;
            IncreaseDamage(damage);
        }

        public virtual void IncreaseDamage(float damage)
        {
            if (DefensePoints < damage)
            {
                float remainingDamage = damage - DefensePoints;
                DefensePoints = 0f;

                if (HealthPoints < remainingDamage)
                    HealthPoints = 0f;
                else
                    HealthPoints -= remainingDamage;

                OnDamaged?.Invoke(SkinColor, ConsoleColor.Red);

                if (HealthPoints <= 0f)
                {
                    PlayDeadSong();
                    OnDie?.Invoke(SkinColor, ConsoleColor.Red);
                }
            }
            else
            {
                DefensePoints -= damage;
                OnDamaged?.Invoke(SkinColor, ConsoleColor.Blue);
            }
        }

        /// <summary>
        /// Метод, воспроизводящий звук смерти сущности
        /// </summary>
        protected internal virtual void PlayDeadSong()
        {
            SoundPlayer.Play(() =>
            {
                Console.Beep(235, 1000);
                Console.Beep(185, 1000);
                Console.Beep(115, 1200);
            });
        }
    }
}