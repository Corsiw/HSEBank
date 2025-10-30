using Domain.Enums;

namespace Domain.Entities
{
    public class Operation
    {
        public Guid Id { get; private set; }
        public MoneyType Type { get; private set; }
        public Guid BankAccountId { get; private set; }
        public Guid CategoryId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string? Description { get; private set; }
        
#pragma warning disable CS8618 // For EFCore correct mapping
        private Operation() { }
#pragma warning restore CS0168 // Re-enable the warning
        
        public Operation(MoneyType type, Guid bankAccountId, Guid categoryId, decimal amount, DateTime date, string? description = null, Guid? id = null)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive.", nameof(amount));
            }

            Id = id ?? Guid.NewGuid();
            Type = type;
            BankAccountId = bankAccountId;
            CategoryId = categoryId;
            Amount = amount;
            Date = date;
            Description = description;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }
}