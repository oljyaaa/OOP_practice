namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Результат калькуляції тиражу. Незмінний об'єкт.
    /// </summary>
    public class PrintRunEstimate
    {
        public PrintRunEstimate(decimal fixedCosts, decimal variableCosts, int quantity,
            decimal expectedRevenue, decimal royalty)
        {
            FixedCosts = fixedCosts;
            VariableCosts = variableCosts;
            Quantity = quantity;
            ExpectedRevenue = expectedRevenue;
            Royalty = royalty;
        }

        /// <summary>
        /// Постійні витрати: редагування, верстка, дизайн обкладинки.
        /// </summary>
        public decimal FixedCosts { get; }

        /// <summary>
        /// Змінні витрати на весь тираж: папір, друк, обкладинка.
        /// </summary>
        public decimal VariableCosts { get; }

        public int Quantity { get; }

        public decimal ExpectedRevenue { get; }

        public decimal Royalty { get; }

        public decimal TotalCost => FixedCosts + VariableCosts;

        public decimal CostPerCopy => Quantity > 0 ? TotalCost / Quantity : 0m;

        public decimal ExpectedProfit => ExpectedRevenue - TotalCost - Royalty;

        public bool IsProfitable => ExpectedProfit > 0;
    }
}
