using ConsoleGame.Global.Saving;
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
                Console.Beep(250, 900);
                Console.Beep(400, 1000);
            });
        }
    }
}