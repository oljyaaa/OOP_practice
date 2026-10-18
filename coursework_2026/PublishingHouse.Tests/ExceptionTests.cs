using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class ExceptionTests
    {
        [TestMethod]
        public void ManuscriptRejected_KeepsDetailsInMessage()
        {
            var id = Guid.NewGuid();
            var ex = new ManuscriptRejectedException(id, "Тіні", "слабкий сюжет");

            Assert.AreEqual(id, ex.ManuscriptId);
            Assert.AreEqual("слабкий сюжет", ex.Reason);
            StringAssert.Contains(ex.Message, "«Тіні»");
            StringAssert.Contains(ex.Message, "слабкий сюжет");
        }

        [TestMethod]
        public void InvalidPrintRunSize_KeepsLimits()
        {
            var ex = new InvalidPrintRunSizeException(50, 100, 100000);

            Assert.AreEqual(50, ex.RequestedSize);
            Assert.AreEqual(100, ex.MinSize);
            Assert.AreEqual(100000, ex.MaxSize);
            StringAssert.Contains(ex.Message, "50");
        }

        [TestMethod]
        public void InvalidStageTransition_KeepsStages()
        {
            var ex = new InvalidStageTransitionException(ManuscriptStage.Submitted, ManuscriptStage.Printed);

            Assert.AreEqual(ManuscriptStage.Submitted, ex.From);
            Assert.AreEqual(ManuscriptStage.Printed, ex.To);
        }

        [TestMethod]
        public void AllDomainExceptions_DeriveFromPublishingException()
        {
            Exception[] exceptions =
            {
                new ManuscriptRejectedException(Guid.NewGuid(), "A", "B"),
                new InvalidPrintRunSizeException(1, 100, 100000),
                new InvalidStageTransitionException(ManuscriptStage.Editing, ManuscriptStage.Submitted),
                new EntityNotFoundException("Автор", Guid.NewGuid())
            };

            foreach (var ex in exceptions)
                Assert.IsInstanceOfType<PublishingException>(ex);
        }
    }
}
