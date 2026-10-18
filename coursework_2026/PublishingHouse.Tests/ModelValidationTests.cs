using PublishingHouse.Core.Models;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class ModelValidationTests
    {
        [TestMethod]
        public void Author_ValidData_IsAccepted()
        {
            var author = new Author { FirstName = "Ольга", LastName = "Петренко", Email = "olha@example.com", RoyaltyPercent = 10 };

            Assert.AreEqual("Ольга Петренко", author.FullName);
            Assert.AreEqual("Автор", author.Role);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("no-at.com")]
        [DataRow("a@b")]
        [DataRow("a@@b.com")]
        public void Person_InvalidEmail_Throws(string email)
        {
            var author = new Author();
            Assert.ThrowsExactly<ArgumentException>(() => author.Email = email);
        }

        [TestMethod]
        public void Person_NameWithDigits_Throws()
        {
            var editor = new Editor();
            Assert.ThrowsExactly<ArgumentException>(() => editor.FirstName = "Ivan2");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(51)]
        public void Author_RoyaltyOutOfRange_Throws(int royalty)
        {
            var author = new Author();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => author.RoyaltyPercent = royalty);
        }

        [TestMethod]
        public void Editor_NonPositiveRate_Throws()
        {
            var editor = new Editor();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => editor.RatePerPage = 0);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(5001)]
        public void Manuscript_PageCountOutOfRange_Throws(int pages)
        {
            var manuscript = new Manuscript();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => manuscript.PageCount = pages);
        }

        [TestMethod]
        public void Manuscript_EmptyTitleOrAuthor_Throws()
        {
            var manuscript = new Manuscript();
            Assert.ThrowsExactly<ArgumentException>(() => manuscript.Title = "   ");
            Assert.ThrowsExactly<ArgumentException>(() => manuscript.AuthorId = Guid.Empty);
        }

        [TestMethod]
        public void Manuscript_CompareTo_SortsByTitle()
        {
            var list = new List<Manuscript>
            {
                new() { Title = "Віхола" },
                new() { Title = "Абетка" }
            };

            list.Sort();

            Assert.AreEqual("Абетка", list[0].Title);
        }

        [TestMethod]
        public void PrintRun_InvalidValues_Throw()
        {
            var run = new PrintRun();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => run.Quantity = 0);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => run.RetailPrice = -5);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => run.ExpectedSellThroughPercent = 120);
        }
    }
}
