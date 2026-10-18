using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class CostCalculatorTests
    {
        [TestMethod]
        public void Estimate_ComputesCostRevenueAndProfit()
        {
            var author = new Author { RoyaltyPercent = 10 };
            var editor = new Editor { RatePerPage = 20 };
            var manuscript = new Manuscript { PageCount = 200 };
            var run = new PrintRun
            {
                Quantity = 1000,
                PaperType = PaperType.Offset,
                CoverType = CoverType.Paperback,
                RetailPrice = 300,
                ExpectedSellThroughPercent = 80
            };

            PrintRunEstimate estimate = new CostCalculator().Estimate(run, manuscript, author, editor);

            // постійні: 20*200 + 15*200 + 5000 = 12 000
            Assert.AreEqual(12000m, estimate.FixedCosts);
            // змінні: 1000 * (200*(0.30+0.20) + 12) = 112 000
            Assert.AreEqual(112000m, estimate.VariableCosts);
            Assert.AreEqual(124m, estimate.CostPerCopy);
            // виручка: 800 * 300 = 240 000; роялті 24 000
            Assert.AreEqual(240000m, estimate.ExpectedRevenue);
            Assert.AreEqual(24000m, estimate.Royalty);
            // прибуток: 240 000 − 124 000 − 24 000 = 92 000
            Assert.AreEqual(92000m, estimate.ExpectedProfit);
            Assert.IsTrue(estimate.IsProfitable);
        }

        [TestMethod]
        public void Estimate_WithoutEditor_HasNoEditingCost()
        {
            var estimate = new CostCalculator().Estimate(
                new PrintRun { Quantity = 100, RetailPrice = 10, CoverType = CoverType.Hardcover, PaperType = PaperType.Coated },
                new Manuscript { PageCount = 100 },
                new Author { RoyaltyPercent = 0 },
                editor: null);

            Assert.AreEqual(6500m, estimate.FixedCosts);
            // 100 * (100*(0.60+0.20) + 45) = 12 500
            Assert.AreEqual(12500m, estimate.VariableCosts);
            Assert.IsFalse(estimate.IsProfitable);
        }
    }
}
