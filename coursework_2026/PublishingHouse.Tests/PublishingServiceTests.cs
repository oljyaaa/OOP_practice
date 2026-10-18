using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class PublishingServiceTests
    {
        private PublishingService _service = null!;
        private Author _author = null!;
        private Editor _editor = null!;

        [TestInitialize]
        public void SetUp()
        {
            _service = new PublishingService();
            _author = new Author { FirstName = "Ліна", LastName = "Костенко", Email = "lina@mail.com", RoyaltyPercent = 12 };
            _editor = new Editor { FirstName = "Петро", LastName = "Редько", Email = "petro@mail.com", RatePerPage = 10, Specialization = Genre.Poetry };
            _service.AddAuthor(_author);
            _service.AddEditor(_editor);
        }

        private Manuscript CreateReadyForPrint()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Маруся Чурай", Genre.Poetry, 180);
            _service.AssignEditor(m.Id, _editor.Id);
            _service.AdvanceStage(m.Id);                    // UnderReview
            _service.Review(m.Id, true, "Чудово");          // Editing
            _service.AdvanceStage(m.Id);                    // Proofreading
            _service.AdvanceStage(m.Id);                    // Layout
            _service.AdvanceStage(m.Id);                    // ReadyForPrint
            return m;
        }

        [TestMethod]
        public void FullWorkflow_ReachesPrinted()
        {
            Manuscript m = CreateReadyForPrint();
            Assert.AreEqual(ManuscriptStage.ReadyForPrint, m.Stage);

            PrintRun run = _service.CreatePrintRun(m.Id, 2000, PaperType.Offset, CoverType.Hardcover, 450, 70);

            Assert.AreEqual(ManuscriptStage.Printed, m.Stage);
            Assert.AreEqual(1, _service.PrintRuns.Count);
            Assert.AreEqual(_service.EstimatePrintRun(run.Id).ExpectedProfit, _service.GetTotalExpectedProfit());
        }

        [TestMethod]
        public void SubmitManuscript_UnknownAuthor_Throws()
        {
            Assert.ThrowsExactly<EntityNotFoundException>(() =>
                _service.SubmitManuscript(Guid.NewGuid(), "Х", Genre.Fiction, 10));
        }

        [TestMethod]
        public void Review_Rejected_SetsStageAndThrows()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Чернетка", Genre.Fiction, 50);
            _service.AdvanceStage(m.Id);

            var ex = Assert.ThrowsExactly<ManuscriptRejectedException>(() => _service.Review(m.Id, false, "Слабкий сюжет"));

            Assert.AreEqual(m.Id, ex.ManuscriptId);
            Assert.AreEqual(ManuscriptStage.Rejected, m.Stage);
        }

        [TestMethod]
        public void Review_ApprovedWithoutEditor_ThrowsAndKeepsStage()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Без редактора", Genre.Fiction, 50);
            _service.AdvanceStage(m.Id);

            Assert.ThrowsExactly<PublishingException>(() => _service.Review(m.Id, true, "Ок"));
            Assert.AreEqual(ManuscriptStage.UnderReview, m.Stage);
        }

        [TestMethod]
        public void AdvanceStage_FromUnderReview_RequiresReview()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);
            _service.AdvanceStage(m.Id);

            Assert.ThrowsExactly<InvalidStageTransitionException>(() => _service.AdvanceStage(m.Id));
        }

        [TestMethod]
        public void Review_NotUnderReview_Throws()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);

            Assert.ThrowsExactly<InvalidStageTransitionException>(() => _service.Review(m.Id, true, "Ок"));
        }

        [TestMethod]
        public void CreatePrintRun_NotReady_Throws()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);

            Assert.ThrowsExactly<InvalidStageTransitionException>(() =>
                _service.CreatePrintRun(m.Id, 1000, PaperType.Offset, CoverType.Paperback, 100, 50));
        }

        [TestMethod]
        [DataRow(99)]
        [DataRow(100_001)]
        public void CreatePrintRun_InvalidSize_Throws(int quantity)
        {
            Manuscript m = CreateReadyForPrint();

            var ex = Assert.ThrowsExactly<InvalidPrintRunSizeException>(() =>
                _service.CreatePrintRun(m.Id, quantity, PaperType.Offset, CoverType.Paperback, 100, 50));

            Assert.AreEqual(quantity, ex.RequestedSize);
            Assert.AreEqual(ManuscriptStage.ReadyForPrint, m.Stage);
        }

        [TestMethod]
        public void AddAuthor_DuplicateEmail_Throws()
        {
            var twin = new Author { FirstName = "Інша", LastName = "Особа", Email = "LINA@mail.com" };

            Assert.ThrowsExactly<PublishingException>(() => _service.AddAuthor(twin));
        }

        [TestMethod]
        public void RemoveAuthor_WithManuscripts_Throws()
        {
            _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);

            Assert.ThrowsExactly<PublishingException>(() => _service.RemoveAuthor(_author.Id));
        }

        [TestMethod]
        public void ImportData_BrokenReference_ThrowsAndKeepsCurrentData()
        {
            var data = new PublishingData
            {
                Manuscripts = { new Manuscript { Title = "Сирота", AuthorId = Guid.NewGuid(), PageCount = 10 } }
            };

            Assert.ThrowsExactly<PublishingException>(() => _service.ImportData(data));
            Assert.AreEqual(1, _service.Authors.Count);
        }
    }
}
