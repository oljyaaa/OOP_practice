using PublishingHouse.Core.Comparers;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    internal sealed class ReportMenu : MenuBase
    {
        public ReportMenu(PublishingService service)
            : base(service)
        {
        }

        protected override string Title => "Звіти";

        protected override IReadOnlyList<MenuItem> CreateItems() => new List<MenuItem>
        {
            new("Рейтинг тиражів за прибутком", ProfitRanking),
            new("Загальний прогнозований прибуток", TotalProfit),
            new("Кількість рукописів за етапами", StageStatistics)
        };

        private void ProfitRanking()
        {
            var runs = Service.PrintRuns.GetAll().ToList();
            if (runs.Count == 0)
            {
                ConsoleHelper.Warning("Тиражів ще немає.");
                return;
            }

            // Кешуємо розрахунки, щоб не перераховувати при кожному порівнянні
            var estimates = runs.ToDictionary(r => r.Id, r => Service.Estimate(r));
            runs.Sort(new PrintRunByProfitComparer(r => estimates[r.Id].ExpectedProfit));

            for (int i = 0; i < runs.Count; i++)
            {
                PrintRun run = runs[i];
                string title = Service.Manuscripts.GetById(run.ManuscriptId).Title;
                PrintRunEstimate e = estimates[run.Id];
                Console.WriteLine($"  {i + 1}. «{title}», {run.Quantity} прим. — прибуток {ConsoleHelper.Money(e.ExpectedProfit)} " +
                                  $"(собівартість {ConsoleHelper.Money(e.TotalCost)})");
            }
        }

        private void TotalProfit()
        {
            ConsoleHelper.Info($"  Тиражів: {Service.PrintRuns.Count}");
            ConsoleHelper.Info($"  Загальний прогнозований прибуток: {ConsoleHelper.Money(Service.GetTotalExpectedProfit())}");
        }

        private void StageStatistics()
        {
            foreach (ManuscriptStage stage in Enum.GetValues<ManuscriptStage>())
            {
                int count = Service.Manuscripts.Find(m => m.Stage == stage).Count;
                Console.WriteLine($"  {DisplayNames.Of(stage),-18} {count}");
            }
            Console.WriteLine($"  {"Усього",-18} {Service.Manuscripts.Count}");
        }
    }
}
