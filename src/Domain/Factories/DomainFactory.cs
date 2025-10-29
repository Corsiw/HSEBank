using Domain.Entities;
using Domain.Enums;

namespace Domain.Factories
{
    public class DomainFactory : IDomainFactory
    {
        public BankAccount CreateBankAccount(string name, decimal initialBalance, Guid? id = null)
        {
            return new BankAccount(name, initialBalance, id);
        }

        public Category CreateCategory(MoneyType type, string name, Guid? id = null)
        {
            return new Category(type, name, id);
        }

        public Operation CreateOperation(MoneyType type, Guid accountId, Guid categoryId, decimal amount, DateTime date,
            string? description = null, Guid? id = null)
        {
            return new Operation(type, accountId, categoryId, amount, date, description, id);
        }
    }
}