using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class PrintRunMenu : MenuBase
    {
        public PrintRunMenu(PublishingService service)
            : base(service)
        {
        }

        protected override string Title => "Тиражі";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Список тиражів", ShowAll),
            new("Створити тираж", Create),
            new("Калькуляція тиражу", ShowEstimate)
        };

        private string Format(PrintRun run)
        {
            string title = Service.Manuscripts.TryGetById(run.ManuscriptId, out Manuscript? m) && m is not null ? m.Title : "?";
            return $"«{title}»: {run.Quantity} прим., обкладинка {DisplayNames.Of(run.CoverType).ToLower()}, " +
                   $"папір {DisplayNames.Of(run.PaperType).ToLower()}, ціна {ConsoleHelper.Money(run.RetailPrice)}, " +
                   $"від {run.CreatedOn:dd.MM.yyyy}";
        }

        private List<PrintRun> GetSorted() => Service.PrintRuns.GetAll().OrderByDescending(r => r.CreatedOn).ToList();

        private void ShowAll()
        {
            List<PrintRun> runs = GetSorted();
            if (runs.Count == 0)
            {
                ConsoleHelper.Warning("Тиражів ще немає.");
                return;
            }
            foreach (PrintRun run in runs)
                Console.WriteLine($"  • {Format(run)}");
        }

        private void Create()
        {
            List<Manuscript> ready = Service.Manuscripts.Find(m =>
                m.Stage is ManuscriptStage.ReadyForPrint or ManuscriptStage.Printed);
            ready.Sort();
            Manuscript? manuscript = InputHelper.Choose(ready, ManuscriptMenu.Format,
                "Рукописи, готові до друку (повторний тираж також можливий):");
            if (manuscript is null) return;

            // Межі тиражу навмисно не обмежуються тут: їх перевіряє бізнес-логіка (InvalidPrintRunSizeException)
            int quantity = InputHelper.ReadInt(
                $"Кількість примірників ({PublishingService.MinPrintRunSize}–{PublishingService.MaxPrintRunSize}): ",
                1, int.MaxValue);
            PaperType paper = InputHelper.ReadEnum<PaperType>("Тип паперу:");
            CoverType cover = InputHelper.ReadEnum<CoverType>("Тип обкладинки:");

            var draft = new PrintRun();
            InputHelper.ReadValid("Роздрібна ціна, грн: ", s => draft.RetailPrice = InputHelper.ParseDecimal(s));
            InputHelper.ReadValid("Прогноз продажу, % (0–100): ",
                s => draft.ExpectedSellThroughPercent = InputHelper.ParseDecimal(s));

            PrintRun run = Service.CreatePrintRun(manuscript.Id, quantity, paper, cover,
                draft.RetailPrice, draft.ExpectedSellThroughPercent);
            ConsoleHelper.Success($"Тираж створено. Рукопис «{manuscript.Title}» — {DisplayNames.Of(manuscript.Stage)}.");
            PrintEstimate(Service.Estimate(run));
        }

        private void ShowEstimate()
        {
            PrintRun? run = InputHelper.Choose(GetSorted(), Format, "Оберіть тираж:");
            if (run is null) return;
            PrintEstimate(Service.Estimate(run));
        }

        public static void PrintEstimate(PrintRunEstimate e)
        {
            Console.WriteLine($"  Постійні витрати:          {ConsoleHelper.Money(e.FixedCosts)}");
            Console.WriteLine($"  Змінні витрати:            {ConsoleHelper.Money(e.VariableCosts)}");
            Console.WriteLine($"  Собівартість тиражу:       {ConsoleHelper.Money(e.TotalCost)}");
            Console.WriteLine($"  Собівартість примірника:   {ConsoleHelper.Money(e.CostPerCopy)}");
            Console.WriteLine($"  Прогнозована виручка:      {ConsoleHelper.Money(e.ExpectedRevenue)}");
            Console.WriteLine($"  Роялті автора:             {ConsoleHelper.Money(e.Royalty)}");
            string profit = $"  Прогнозований прибуток:    {ConsoleHelper.Money(e.ExpectedProfit)}";
            if (e.IsProfitable)
                ConsoleHelper.Success(profit.TrimStart());
            else
                ConsoleHelper.Warning($"{profit.TrimStart()} — тираж збитковий");
        }
    }
}
