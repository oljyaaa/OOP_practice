using PublishingHouse.Core.Models;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class MainMenu : MenuBase
    {
        private readonly JsonFileService<PublishingData> _storage;

        public MainMenu(PublishingService service, JsonFileService<PublishingData> storage)
            : base(service)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        protected override string Title => "Книжкове видавництво — головне меню";

        protected override string ExitText => "Зберегти і вийти";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Автори", () => new AuthorMenu(Service).Run()),
            new("Редактори", () => new EditorMenu(Service).Run()),
            new("Рукописи", () => new ManuscriptMenu(Service).Run()),
            new("Тиражі", () => new PrintRunMenu(Service).Run()),
            new("Звіти", () => new ReportMenu(Service).Run()),
            new("Зберегти дані", Save)
        };

        private void Save()
        {
            _storage.Save(Service.ExportData());
            ConsoleHelper.Success($"Дані збережено у {_storage.FilePath}");
        }
    }
}
