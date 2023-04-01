using ConsoleGame.Input;
using ConsoleGame.Entities;
using ConsoleGame.Extensions;
using ConsoleGame.Entities.Alive.Heroes;
using ConsoleGame.Entities.Alive.Monsters;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsoleGame.Global
{
    internal static class SaveSystem
    {
        public const string SaveFileName = "ConsoleGame.sdt";
        public const char DataSeparator = '█';

        private static string _savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SaveFileName);
        /// <summary>
        /// Путь до файла сохранения игры
        /// </summary>
        public static string SavePath
        {
            get => _savePath;
            set
            {
                if (File.Exists(value))
                    _savePath = value;
                else if (Directory.Exists(value))
                    _savePath = Path.Combine(value, SaveFileName);
                else
                    throw new ArgumentException($"Путь {value} указан не верно");
            }
        }

        /// <summary>
        /// Считывает сохранение
        /// </summary>
        /// <returns>Возвращает данные о сохранённом прогрессе в виде строки</returns>
        public static string GetSavedGame()
        {
            string data = string.Empty;
            if (File.Exists(SavePath))
            {
                using (FileStream fs = File.OpenRead(SavePath))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    data = Encoding.UTF8.GetString(buffer);
                }
            }
            return data;
        }

        /// <summary>
        /// Сохраняет игровой прогресс по пути <see cref="SavePath"/>
        /// </summary>
        /// <param name="arena">Игровая арена</param>
        public static async Task SaveGame(Arena arena, UserInterface ui)
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate))
            {
                string inputsData = await InputSystem.GetBindingsDataAsync();
                string arenaData = await arena.GetDataAsync();
                string data = GetFormatedData(DateTime.Now.ToString() + Environment.NewLine, inputsData, arenaData, ui.Data);
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        private static string GetFormatedData(params string[] data)
        {
            StringBuilder formatedData = new StringBuilder();

            foreach (string unit in data)
                formatedData.AppendFormat("{0}{1}", unit, DataSeparator);

            formatedData.Remove(formatedData.Length - 1, 1);

            return formatedData.ToString();
        }

        public static void ParseSavedData(string progress, out Arena arena, out UserInterface ui)
        {
            arena = new Arena();

            string[] data = progress.Split(new[] { DataSeparator }, StringSplitOptions.RemoveEmptyEntries);

            string[] inputBindings = data[1].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] entitiesData = data[2].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] uiData = data[3].Split('-');

            WriteInputSystem(inputBindings);
            WriteArena(entitiesData, ref arena);
            WriteUI(uiData, ref arena, out ui);
        }

        private static void WriteInputSystem(string[] bindings)
        {
            for (int i = 0; i < InputSystem.Bindings.Count; i++)
            {
                string[] bindingData = bindings[i].Split('-');
                Inputs inputType = (Inputs)Enum.Parse(typeof(Inputs), bindingData[0]);
                string[] modifiersKeys = bindingData[1].Split(new[] { " + " }, StringSplitOptions.None);
                ConsoleModifiers modifiers = (ConsoleModifiers)Enum.Parse(typeof(ConsoleModifiers), modifiersKeys[0]);
                ConsoleKey key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), modifiersKeys[1]);
                InputSystem.Bindings[inputType] = new GlobalHotKey(new HotKey(key, modifiers), () => { });
            }
        }

        private static void WriteArena(string[] entitiesData, ref Arena arena)
        {
            foreach (Entity entity in ParseEntities(entitiesData))
            {
                if (entity is Monster monster)
                    arena.InitializeMonsterEvents(monster, monster.IsBoss ? 50 : 10);
                arena.Instantiate(entity, entity.Position);
            }
        }

        private static IEnumerable<Entity> ParseEntities(string[] entitiesData)
        {
            IEnumerable<Entity> entityList = new List<Entity>();

            for (int e = 0; e < entitiesData.Length; e++)
            {
                string[] entityData = entitiesData[e].Split(':', ';');
                Type entityType = Type.GetType(entityData[0]);
                string[] ctorProps = entityData[1].Trim().Split(' ');
                (string prop, string value)[] ctorPropsPairs = GetPropsPairs(ctorProps);
                string[] publicProps = entityData[2].Trim().Split(' ');
                (string prop, string value)[] publicPropsPairs = GetPropsPairs(publicProps);

                object[] ctorParams = new object[ctorPropsPairs.Length];
                for (int i = 0; i < ctorPropsPairs.Length; i++)
                {
                    Type propType = Type.GetType(ctorPropsPairs[i].prop);
                    if (propType.IsEnum)
                        ctorParams[i] = Convert.ChangeType(Enum.Parse(propType, ctorPropsPairs[i].value), propType);
                    else
                        ctorParams[i] = Convert.ChangeType(ctorPropsPairs[i].value, propType);
                }
                Entity newEntity = Activator.CreateInstance(entityType, ctorParams) as Entity;

                var entitySavedProps = entityType.GetProperties().Where(p => p.GetCustomAttribute<DataToSaveAttribute>() != null);

                foreach (var (prop, value) in publicPropsPairs)
                {
                    var entityProp = entityType.GetProperty(prop);
                    var propType = entityProp.PropertyType;
                    object convertedValue;

                    if (propType.Equals(typeof(Point)))
                        convertedValue = new Point().FromString(value);
                    else if (propType.IsEnum)
                        convertedValue = Convert.ChangeType(Enum.Parse(propType, value), propType);
                    else
                        convertedValue = Convert.ChangeType(value, propType);

                    entitySavedProps.First(p => p.Name.Equals(prop)).SetValue(newEntity, convertedValue);
                }

                entityList = entityList.Append(newEntity);
            }

            return entityList;
        }

        private static (string prop, string value)[] GetPropsPairs(string[] props)
        {
            (string prop, string value)[] propsPairs = new (string prop, string value)[props.Length];
            for (int i = 0; i < propsPairs.Length; i++)
            {
                string[] values = props[i].Split('-');
                propsPairs[i] = (prop: values[0], value: values[1]);
            }
            return propsPairs;
        }

        private static void WriteUI(string[] uiData, ref Arena arena, out UserInterface ui)
        {
            
            Hero hero = arena.Entities.Find(e => e.GetType().Equals(typeof(Hero))) as Hero;
            ui = new UserInterface(arena.Width, ref hero, int.Parse(uiData[0]))
            {
                Day = int.Parse(uiData[1]),
                Score = int.Parse(uiData[2])
            };
            ui.UpdateDayInterface();
        }
    }
}