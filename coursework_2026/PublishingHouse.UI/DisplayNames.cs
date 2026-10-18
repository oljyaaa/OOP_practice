using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.UI
{
    /// <summary>
    /// Українські назви значень перелічень для інтерфейсу.
    /// </summary>
    internal static class DisplayNames
    {
        public static string Of<T>(T value) where T : struct, Enum
        {
            object boxed = value;
            return boxed switch
            {
                ManuscriptStage stage => stage switch
                {
                    ManuscriptStage.Submitted => "Подано",
                    ManuscriptStage.UnderReview => "На рецензії",
                    ManuscriptStage.Editing => "Редагування",
                    ManuscriptStage.Proofreading => "Коректура",
                    ManuscriptStage.Layout => "Верстка",
                    ManuscriptStage.ReadyForPrint => "Готовий до друку",
                    ManuscriptStage.Printed => "Надруковано",
                    ManuscriptStage.Rejected => "Відхилено",
                    _ => stage.ToString()
                },
                Genre genre => genre switch
                {
                    Genre.Fiction => "Художня література",
                    Genre.NonFiction => "Нон-фікшн",
                    Genre.Science => "Наукова",
                    Genre.Children => "Дитяча",
                    Genre.Poetry => "Поезія",
                    Genre.Textbook => "Підручник",
                    _ => genre.ToString()
                },
                CoverType cover => cover == CoverType.Hardcover ? "Тверда" : "М'яка",
                PaperType paper => paper switch
                {
                    PaperType.Newsprint => "Газетний",
                    PaperType.Offset => "Офсетний",
                    PaperType.Coated => "Крейдований",
                    _ => paper.ToString()
                },
                _ => value.ToString()
            };
        }
    }
}
