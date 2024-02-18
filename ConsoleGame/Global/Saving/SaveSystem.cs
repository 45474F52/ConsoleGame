using ConsoleGame.Entities;
using ConsoleGame.Extensions;
using ConsoleGame.Global.Input;
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

namespace ConsoleGame.Global.Saving
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
                if (_savePath != value)
                {
                    if (File.Exists(value))
                        _savePath = value;
                    else if (Directory.Exists(value))
                    {
                        _savePath = Path.Combine(value, SaveFileName);
                        SettingsSystem.Set("SavePath", value);
                    }
                    else
                        throw new ArgumentException($"Путь {value} указан не верно или его не существует");
                }
            }
        }

        /// <summary>
        /// Считывает сохранение
        /// </summary>
        /// <returns>Возвращает данные о сохранённом прогрессе в виде строки</returns>
        public static string GetData() => GetDataFrom(SavePath);

        /// <summary>
        /// Считывает данные из файла по пути <paramref name="path"/>
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns>Возвращает считанные из файла данные, если файл найден, иначе <see cref="string.Empty"/></returns>
        public static string GetDataFrom(string path)
        {
            string data = string.Empty;
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
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
        public static async Task SaveData(Arena arena, UserInterface ui)
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                string inputsData = await InputSystem.GetBindingsDataAsync();
                string arenaData = await arena.GetDataAsync();
                string data = GetFormatedData(DateTime.Now.ToString() + Environment.NewLine, inputsData, arenaData, ui.Data);
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Форматирует данные для их сохранения
        /// </summary>
        /// <param name="data">Набор параметров для сохранения</param>
        /// <returns>Возвращает строку с форматированными данными</returns>
        private static string GetFormatedData(params string[] data)
        {
            StringBuilder formatedData = new StringBuilder();

            foreach (string unit in data)
                formatedData.AppendFormat("{0}{1}", unit, DataSeparator);

            formatedData.Remove(formatedData.Length - 1, 1);

            return formatedData.ToString();
        }

        /// <summary>
        /// Создаёт и инициализирует объекты согласно сохранённому прогрессу
        /// </summary>
        /// <param name="progress">Сохранение</param>
        /// <param name="arena">Арена</param>
        /// <param name="ui">Пользовательский интерф wейс</param>
        public static void ParseSavedData(string progress, out Arena arena, out UserInterface ui)
        {
            string[] data = progress.Split(new[] { DataSeparator }, StringSplitOptions.RemoveEmptyEntries);

            string[] inputBindings = data[1].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string[] arenaRawData = new string[2];
            arenaRawData[0] = new string(data[2].TakeWhile(c => c != '\r').ToArray());
            arenaRawData[1] = new string(data[2].Skip(arenaRawData[0].Length).ToArray());

            string[] arenaData = arenaRawData[0].Split('-');
            string[] entitiesData = arenaRawData[1].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string[] uiData = data[3].Split('-');

            arena = new Arena(int.Parse(arenaData[0]), int.Parse(arenaData[1]), (ConsoleColor)Enum.Parse(typeof(ConsoleColor), arenaData[2]));

            WriteInputSystem(inputBindings);
            WriteEntities(entitiesData, arena);
            WriteUI(uiData, arena, out ui);
        }

        /// <summary>
        /// Инициализирует комбинации клавиш
        /// </summary>
        /// <param name="bindings">Сохранённые комбинации клавиш</param>
        private static void WriteInputSystem(string[] bindings)
        {
            for (int i = 0; i < InputSystem.Bindings.Count; i++)
            {
                string[] bindingData = bindings[i].Split('-');
                Inputs inputType = (Inputs)Enum.Parse(typeof(Inputs), bindingData[0]);
                string[] modifiersKeys = bindingData[1].Split(new[] { " + " }, StringSplitOptions.None);
                ConsoleModifiers modifiers = (ConsoleModifiers)Enum.Parse(typeof(ConsoleModifiers), modifiersKeys[0]);
                ConsoleKey key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), modifiersKeys[1]);
                InputSystem.Bindings[inputType] = new GlobalHotKey(new HotKey(key, modifiers), delegate { });
            }
        }

        /// <summary>
        /// Добавляет сущности на арену
        /// </summary>
        /// <param name="entitiesData">Информация о сущностях</param>
        /// <param name="arena">Арена</param>
        private static void WriteEntities(string[] entitiesData, Arena arena)
        {
            foreach (Entity entity in ParseEntities(entitiesData))
            {
                if (entity is Monster monster)
                    arena.InitializeMonsterEvents(monster, monster.IsBoss ? 50 : 10);
                arena.Instantiate(entity, entity.Position);
            }
        }

        /// <summary>
        /// Создаёт сущности и иницилизирует их
        /// </summary>
        /// <param name="entitiesData">Информация о сущностях</param>
        /// <returns>Возвращает список сущностей</returns>
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

        /// <summary>
        /// Создаёт пару СВОЙСТВО-ЗНАЧЕНИЕ
        /// </summary>
        /// <param name="props">Неформатированные строки свойство-значение</param>
        /// <returns>Возвращает пары СВОЙСТВО-ЗНАЧЕНИЕ из неформатированных строк</returns>
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

        /// <summary>
        /// Инициализирует свойства пользовательского интерфейса
        /// </summary>
        /// <param name="uiData">Форматированные значения свойств</param>
        /// <param name="arena">Арена</param>
        /// <param name="ui">Пользовательский интерфейс</param>
        private static void WriteUI(string[] uiData, Arena arena, out UserInterface ui)
        {
            Hero hero = arena.entities.Find(e => e.GetType().Equals(typeof(Hero))) as Hero;
            ui = new UserInterface(arena.width, hero, int.Parse(uiData[0]))
            {
                Day = int.Parse(uiData[1]),
                Score = int.Parse(uiData[2])
            };
            ui.UpdateDayInterface();
        }
    }
}