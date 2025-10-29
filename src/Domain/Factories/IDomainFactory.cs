using Domain.Entities;
using Domain.Enums;

namespace Domain.Factories
{
    public interface IDomainFactory
    {
        BankAccount CreateBankAccount(string name, decimal initialBalance, Guid? id = null);
        Category CreateCategory(MoneyType type, string name, Guid? id = null);
        Operation CreateOperation(MoneyType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null, Guid? id = null);
    }
}