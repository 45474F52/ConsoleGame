using System;

namespace ConsoleGame.Entities.Alive.Monsters
{
    internal class Monster : AliveEntity
    {
        public Monster(char character, ConsoleColor color, float hp, float dmgP, float dfsP) : base(character, color, hp, dmgP, dfsP) { }

        [DataToSave]
        public bool IsBoss { get; set; }

        protected internal override void PlayDeadSong()
        {
            SoundPlayer.Play(() =>
            {
                Console.Beep(150, 800);
                Console.Beep(200, 600);
                Console.Beep(300, 450);
                Console.Beep(400, 300);
            });
        }
    }
}