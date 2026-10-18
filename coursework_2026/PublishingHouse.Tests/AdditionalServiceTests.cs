using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Repositories;
using PublishingHouse.Core.Services;

namespace PublishingHouse.Tests
{
    /// <summary>
    /// Граничні випадки сервісу та моделей, не покриті основними тестами.
    /// </summary>
    [TestClass]
    public sealed class AdditionalServiceTests
    {
        private PublishingService _service = null!;
        private Author _author = null!;
        private Editor _editor = null!;

        [TestInitialize]
        public void SetUp()
        {
            _service = new PublishingService();
            _author = new Author { FirstName = "Леся", LastName = "Українка", Email = "lesia@mail.com", RoyaltyPercent = 8 };
            _editor = new Editor { FirstName = "Олег", LastName = "Коваль", Email = "oleh@mail.com", RatePerPage = 12, Specialization = Genre.Fiction };
            _service.AddAuthor(_author);
            _service.AddEditor(_editor);
        }

        private Manuscript CreatePrinted()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Лісова пісня", Genre.Fiction, 120);
            _service.AssignEditor(m.Id, _editor.Id);
            _service.AdvanceStage(m.Id);
            _service.Review(m.Id, true, "Схвалено");
            for (int i = 0; i < 3; i++)
                _service.AdvanceStage(m.Id);
            _service.CreatePrintRun(m.Id, 500, PaperType.Offset, CoverType.Paperback, 200, 90);
            return m;
        }

        [TestMethod]
        public void RemoveEditor_WithActiveManuscript_Throws()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);
            _service.AssignEditor(m.Id, _editor.Id);

            Assert.ThrowsExactly<PublishingException>(() => _service.RemoveEditor(_editor.Id));
        }

        [TestMethod]
        public void RemoveEditor_WithFinishedWork_StillThrows()
        {
            CreatePrinted();

            Assert.ThrowsExactly<PublishingException>(() => _service.RemoveEditor(_editor.Id));
        }

        [TestMethod]
        public void RemoveEditor_Unassigned_Succeeds()
        {
            _service.RemoveEditor(_editor.Id);

            Assert.AreEqual(0, _service.Editors.Count);
        }

        [TestMethod]
        public void EnsureEmailAvailable_OwnEmail_IsAllowed()
        {
            _service.EnsureEmailAvailable("LESIA@mail.com", _author.Id);
            Assert.ThrowsExactly<PublishingException>(() => _service.EnsureEmailAvailable("lesia@mail.com", _editor.Id));
        }

        [TestMethod]
        public void RemoveManuscript_WithPrintRun_Throws()
        {
            Manuscript m = CreatePrinted();

            Assert.ThrowsExactly<PublishingException>(() => _service.RemoveManuscript(m.Id));
        }

        [TestMethod]
        public void RemoveManuscript_WithoutPrintRuns_Succeeds()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);

            _service.RemoveManuscript(m.Id);

            Assert.AreEqual(0, _service.Manuscripts.Count);
        }

        [TestMethod]
        public void AssignEditor_ToPrintedManuscript_Throws()
        {
            Manuscript m = CreatePrinted();

            Assert.ThrowsExactly<PublishingException>(() => _service.AssignEditor(m.Id, _editor.Id));
        }

        [TestMethod]
        public void AssignEditor_UnknownEditor_Throws()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);

            Assert.ThrowsExactly<EntityNotFoundException>(() => _service.AssignEditor(m.Id, Guid.NewGuid()));
        }

        [TestMethod]
        public void CreatePrintRun_ForPrintedManuscript_AllowsReprint()
        {
            Manuscript m = CreatePrinted();

            _service.CreatePrintRun(m.Id, 1000, PaperType.Coated, CoverType.Hardcover, 350, 60);

            Assert.AreEqual(2, _service.PrintRuns.Count);
            Assert.AreEqual(ManuscriptStage.Printed, m.Stage);
        }

        [TestMethod]
        public void AdvanceStage_FromTerminalStage_Throws()
        {
            Manuscript m = CreatePrinted();

            var ex = Assert.ThrowsExactly<InvalidStageTransitionException>(() => _service.AdvanceStage(m.Id));
            Assert.AreEqual(ManuscriptStage.Printed, ex.From);
        }

        [TestMethod]
        public void Review_EmptyComment_Throws()
        {
            Manuscript m = _service.SubmitManuscript(_author.Id, "Рукопис", Genre.Fiction, 50);
            _service.AdvanceStage(m.Id);

            Assert.ThrowsExactly<ArgumentException>(() => _service.Review(m.Id, false, "  "));
            Assert.AreEqual(ManuscriptStage.UnderReview, m.Stage);
        }

        [TestMethod]
        public void ImportData_ValidSnapshot_ReplacesAllData()
        {
            CreatePrinted();
            PublishingData snapshot = _service.ExportData();

            var other = new PublishingService();
            other.ImportData(snapshot);

            Assert.AreEqual(1, other.Authors.Count);
            Assert.AreEqual(1, other.Editors.Count);
            Assert.AreEqual(1, other.Manuscripts.Count);
            Assert.AreEqual(_service.GetTotalExpectedProfit(), other.GetTotalExpectedProfit());
        }

        [TestMethod]
        public void ImportData_PrintRunWithoutManuscript_Throws()
        {
            var data = new PublishingData
            {
                PrintRuns = { new PrintRun { ManuscriptId = Guid.NewGuid(), Quantity = 100, RetailPrice = 10 } }
            };

            Assert.ThrowsExactly<PublishingException>(() => _service.ImportData(data));
        }

        [TestMethod]
        public void GetTotalExpectedProfit_NoPrintRuns_IsZero()
        {
            Assert.AreEqual(0m, _service.GetTotalExpectedProfit());
        }

        [TestMethod]
        public void EntityBase_EmptyId_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => _author.Id = Guid.Empty);
        }

        [TestMethod]
        public void Editor_RateAboveMax_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _editor.RatePerPage = Editor.MaxRatePerPage + 1);
        }

        [TestMethod]
        public void Summaries_ContainKeyData()
        {
            Manuscript m = CreatePrinted();

            StringAssert.Contains(_author.ToString(), "Леся Українка");
            StringAssert.Contains(_editor.GetSummary(), "oleh@mail.com");
            StringAssert.Contains(m.GetSummary(), "Лісова пісня");
            StringAssert.Contains(_service.PrintRuns.GetAll()[0].GetSummary(), "500");
        }

        [TestMethod]
        public void Repository_EmptyEntityName_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new Repository<Author>(" "));
        }

        [TestMethod]
        public void PrintRunEstimate_ZeroQuantity_CostPerCopyIsZero()
        {
            var estimate = new PrintRunEstimate(100m, 0m, 0, 0m, 0m);

            Assert.AreEqual(0m, estimate.CostPerCopy);
            Assert.AreEqual(-100m, estimate.ExpectedProfit);
        }
    }
}
