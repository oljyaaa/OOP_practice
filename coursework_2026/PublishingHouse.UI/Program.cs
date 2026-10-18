using System.Globalization;
using System.Text;
using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Services;
using PublishingHouse.UI.Menus;

namespace PublishingHouse.UI
{
    /// <summary>
    /// Точка входу консольного застосування «Книжкове видавництво».
    /// Завантажує дані з JSON при старті та зберігає їх при виході.
    /// </summary>
    internal static class Program
    {
        private const string DataFileName = "publishing_data.json";

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

            var storage = new JsonFileService<PublishingData>(Path.Combine(Environment.CurrentDirectory, DataFileName));
            var service = new PublishingService();

            Console.WriteLine("=== Книжкове видавництво (Publishing House) ===");
            LoadData(storage, service);

            try
            {
                new MainMenu(service, storage).Run();
            }
            catch (EndOfInputException)
            {
                ConsoleHelper.Warning("Ввід завершено.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Непередбачена помилка: {ex.Message}");
            }

            SaveData(storage, service);
        }

        private static void LoadData(JsonFileService<PublishingData> storage, PublishingService service)
        {
            if (!storage.FileExists)
            {
                ConsoleHelper.Info("Файл даних не знайдено — починаємо з порожньої бази.");
                return;
            }

            try
            {
                service.ImportData(storage.Load());
                ConsoleHelper.Success($"Дані завантажено: авторів {service.Authors.Count}, редакторів {service.Editors.Count}, " +
                                      $"рукописів {service.Manuscripts.Count}, тиражів {service.PrintRuns.Count}.");
            }
            catch (PublishingException ex)
            {
                ConsoleHelper.Error(ex.Message);
                BackUpCorruptedFile(storage.FilePath);
            }
        }

        /// <summary>
        /// Пошкоджений файл не перезаписуємо мовчки — зберігаємо копію для ручного відновлення.
        /// </summary>
        private static void BackUpCorruptedFile(string filePath)
        {
            string backupPath = $"{filePath}.{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            try
            {
                File.Copy(filePath, backupPath, overwrite: true);
                ConsoleHelper.Warning($"Пошкоджений файл збережено як {backupPath}. Робота продовжується з порожньою базою.");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ConsoleHelper.Error($"Не вдалося створити резервну копію: {ex.Message}");
            }
        }

        private static void SaveData(JsonFileService<PublishingData> storage, PublishingService service)
        {
            try
            {
                storage.Save(service.ExportData());
                ConsoleHelper.Success($"Дані збережено у {storage.FilePath}. До побачення!");
            }
            catch (DataStorageException ex)
            {
                ConsoleHelper.Error(ex.Message);
            }
        }
    }
}
