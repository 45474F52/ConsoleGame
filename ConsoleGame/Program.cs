using System.Media;
using ConsoleGame.Global.Input;
using ConsoleGame.Global.Saving;
using ConsoleGame.Entities.Alive.Heroes;
using System;

namespace ConsoleGame
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Game game = new Game()
            {
                WindowTitle = "SUPER HERO GAME",
                WindowSize = (150, 40)
            };
            game.Start();

            Arena arena = null;
            Hero hero = null;
            UserInterface ui = null;

            if (game.Menu.Arena == null)
            {
                arena = new Arena();
                hero = new Hero(game.Menu.HeroCharacter, game.Menu.HeroColor, game.Menu.HP, game.Menu.DMGP, game.Menu.DFP, game.Menu.ATKS);
                arena.Instantiate(hero, arena.center);
                ui = new UserInterface(arena.width, hero, game.Menu.ChangingDateTime);
            }
            else
            {
                arena = game.Menu.Arena;
                hero = arena.entities.Find(e => e.GetType().Equals(typeof(Hero))) as Hero;
                ui = game.Menu.UI;
            }

            arena.SetUI(ui);
            ui.OnDayChanged += day => arena.SetEntities(day);

            InputSystem.Bindings[Inputs.Save].Callback = async () =>
            {
                await SaveSystem.SaveData(arena, ui);
                SystemSounds.Exclamation.Play();
            };

            hero.CanMove += point => arena.CanMoveTo(ref hero, point);
            hero.OnMove += (point, obj) => arena.DrawIn(point, obj, hero.SkinColor);
            hero.OnDirChanged += (point, obj) => arena.DrawIn(point, obj, hero.SkinColor);
            hero.OnInteracting += (targetPosition, value) => arena.Interaction(targetPosition, hero.InteractionType, value);
            hero.OnDamaged += (oldColor, newColor) =>
            {
                arena.FlashIn(hero.Position, hero.Character, oldColor, newColor);
                ui.UpdateUI();
            };
            hero.OnDie += (oldColor, newColor) => arena.DrawIn(hero.Position, '#', newColor);

            ui.UpdateUI();



            hero.Run();
        }
    }
}