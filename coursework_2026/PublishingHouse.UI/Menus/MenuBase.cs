using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Services;

namespace PublishingHouse.UI.Menus
{
    /// <summary>
    /// Базове меню: показує пункти, виконує обраний і перехоплює помилки бізнес-логіки,
    /// щоб помилка в одній операції не завершувала програму.
    /// </summary>
    internal abstract class MenuBase
    {
        protected MenuBase(PublishingService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected PublishingService Service { get; }

        protected abstract string Title { get; }

        protected virtual string ExitText => "Назад";

        protected abstract IReadOnlyList<MenuItem> CreateItems();

        public void Run()
        {
            IReadOnlyList<MenuItem> items = CreateItems();
            while (true)
            {
                ConsoleHelper.Header(Title);
                for (int i = 0; i < items.Count; i++)
                    Console.WriteLine($"  {i + 1}. {items[i].Text}");
                Console.WriteLine($"  0. {ExitText}");

                int choice = InputHelper.ReadInt("Ваш вибір: ", 0, items.Count);
                if (choice == 0)
                    return;

                Execute(items[choice - 1]);
            }
        }

        private static void Execute(MenuItem item)
        {
            try
            {
                item.Action();
            }
            catch (ManuscriptRejectedException ex)
            {
                ConsoleHelper.Warning(ex.Message);
            }
            catch (InvalidStageTransitionException ex)
            {
                // Повідомлення з бізнес-логіки містить технічні назви етапів — показуємо українські
                ConsoleHelper.Error(ex.From == ex.To
                    ? $"Рукопис на етапі «{DisplayNames.Of(ex.From)}» не має наступного етапу."
                    : $"Неможливо перевести рукопис з етапу «{DisplayNames.Of(ex.From)}» на етап «{DisplayNames.Of(ex.To)}».");
            }
            catch (PublishingException ex)
            {
                ConsoleHelper.Error(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ConsoleHelper.Error(InputHelper.CleanMessage(ex));
            }
            catch (InvalidOperationException ex)
            {
                ConsoleHelper.Error(ex.Message);
            }
        }
    }
}
