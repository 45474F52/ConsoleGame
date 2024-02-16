using System;
using Microsoft.Win32;

namespace ConsoleGame.Global
{
    /// <summary>
    /// Предоставляет методы чтения и записи данных в ветку реестра
    /// </summary>
    internal static class SettingsSystem
    {
        private static readonly string _path = @"SOFTWARE\ConsoleGame\Settings";

        /// <summary>
        /// Проверяет наличие ветки для сохранений параметров
        /// </summary>
        /// <remarks>Если ветки не существует, она будет заново создана</remarks>
        static SettingsSystem()
        {
            RegistryKey settingsKey = null;

            try
            {
                settingsKey = Registry.LocalMachine.OpenSubKey(_path);
                if (settingsKey == null)
                {
                    settingsKey = Registry.LocalMachine.CreateSubKey(_path, true);
                    settingsKey.SetValue("SavePath", Environment.CurrentDirectory);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                settingsKey.Close();
            }
        }

        /// <summary>
        /// Устанавливает значение в ветку реестра по указанному имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public static void Set(string name, object value)
        {
            using (RegistryKey settingsKey = Registry.LocalMachine.OpenSubKey(_path, true))
            {
                settingsKey.SetValue(name, value);
            }
        }

        /// <summary>
        /// Возвращает значение из ветки реестра по указанному имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <returns>Возвращает объект, хранящийся по имени <paramref name="name"/> или <see langword="null"/>, если указанного значения нет</returns>
        public static object Get(string name)
        {
            using (RegistryKey settingsKey = Registry.LocalMachine.OpenSubKey(_path))
            {
                return settingsKey.GetValue(name);
            }
        }

        /// <summary>
        /// Возвращает значение из ветки реестра по указанному имени и конвертирует тип в <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Имя параметра</typeparam>
        /// <param name="key"></param>
        /// <returns>Возвращает объект с типом <typeparamref name="T"/>, 
        /// хранящийся по имени <paramref name="name"/> или <see langword="null"/>, если указанного значения нет</returns>
        public static T Get<T>(string key) where T : new()
        {
            using (RegistryKey settingsKey = Registry.LocalMachine.OpenSubKey(_path))
            {
                object value = settingsKey.GetValue(key);
                TypeCode typeCode = Convert.GetTypeCode(new T());
                return (T)Convert.ChangeType(value, typeCode);
            }
        }
    }
}