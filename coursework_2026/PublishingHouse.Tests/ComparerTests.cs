using PublishingHouse.Core.Comparers;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class ComparerTests
    {
        [TestMethod]
        public void PersonByName_SortsByLastNameThenFirstName()
        {
            var authors = new List<Author>
            {
                new() { FirstName = "Олена", LastName = "Пчілка" },
                new() { FirstName = "Іван", LastName = "Франко" },
                new() { FirstName = "Ганна", LastName = "Пчілка" }
            };

            authors.Sort(new PersonByNameComparer());

            CollectionAssert.AreEqual(new[] { "Ганна", "Олена", "Іван" }, authors.Select(a => a.FirstName).ToArray());
        }

        [TestMethod]
        public void ManuscriptByPageCount_SortsAscending()
        {
            var list = new List<Manuscript> { new() { PageCount = 300 }, new() { PageCount = 120 } };

            list.Sort(new ManuscriptByPageCountComparer());

            Assert.AreEqual(120, list[0].PageCount);
        }

        [TestMethod]
        public void ManuscriptByStage_SortsByStageThenDate()
        {
            var early = new Manuscript { Stage = ManuscriptStage.Editing, SubmittedOn = new DateTime(2026, 1, 1) };
            var late = new Manuscript { Stage = ManuscriptStage.Editing, SubmittedOn = new DateTime(2026, 5, 1) };
            var submitted = new Manuscript { Stage = ManuscriptStage.Submitted };
            var list = new List<Manuscript> { late, early, submitted };

            list.Sort(new ManuscriptByStageComparer());

            CollectionAssert.AreEqual(new[] { submitted, early, late }, list);
        }

        [TestMethod]
        public void PrintRunByProfit_SortsDescending()
        {
            var small = new PrintRun { Quantity = 100 };
            var big = new PrintRun { Quantity = 1000 };
            var list = new List<PrintRun> { small, big };

            list.Sort(new PrintRunByProfitComparer(run => run.Quantity * 10m));

            Assert.AreSame(big, list[0]);
        }
    }
}
