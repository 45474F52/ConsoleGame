using ConsoleGame.Input;
using ConsoleGame.Global;
using ConsoleGame.Entities.Alive.Heroes;
using System.Media;

namespace ConsoleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game()
            {
                WindowTitle = "Anechka`s GAME",
                WindowSize = (150, 40)
            };
            game.Start();

            Arena arena = null;
            Hero hero = null;
            UserInterface ui = null;
            if (game.Menu.Arena == null)
            {
                arena = new Arena();
                hero = new Hero(game.Menu.SelectedCharacter, game.Menu.HeroColor);
                arena.Instantiate(hero, arena.Center);
                ui = new UserInterface(arena.Width, ref hero, game.Menu.ChangingDateTime);
            }
            else
            {
                arena = game.Menu.Arena;
                hero = arena.Entities.Find(e => e.GetType().Equals(typeof(Hero))) as Hero;
                ui = game.Menu.UI;
            }

            arena.SetUI(ui);
            ui.OnDayChanged += day => arena.SetEntities(day);

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
            InputSystem.Bindings[Inputs.Save].Callback = async () =>
            {
                SystemSounds.Exclamation.Play();
                await SaveSystem.SaveGame(arena, ui);
            };
            hero.Run();
        }
    }
}