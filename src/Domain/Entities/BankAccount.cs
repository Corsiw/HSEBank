namespace Domain.Entities
{
    public class BankAccount
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        public BankAccount(string name, decimal balance, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Account name cannot be empty.", nameof(name));
            }

            if (balance < 0)
            {
                throw new ArgumentException("Balance cannot be negative.", nameof(balance));
            }

            Id = id ?? Guid.NewGuid();
            Name = name;
            Balance = balance;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Account name cannot be empty.", nameof(name));
            }

            Name = name;
        }

        public void UpdateBalance(decimal newBalance)
        {
            if (newBalance < 0)
            {
                throw new ArgumentException("New balance cannot be negative.");
            }

            Balance = newBalance;
        }
    }
}