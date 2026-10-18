namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Базовий абстрактний клас для всіх сутностей предметної області.
    /// </summary>
    public abstract class EntityBase
    {
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Унікальний ідентифікатор сутності.
        /// </summary>
        public Guid Id
        {
            get => _id;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор не може бути порожнім.");
                _id = value;
            }
        }

        /// <summary>
        /// Короткий опис сутності для виведення в консоль.
        /// </summary>
        public abstract string GetSummary();

        public override string ToString() => GetSummary();
    }
}
