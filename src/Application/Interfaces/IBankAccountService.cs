using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBankAccountService
    {
        Task<Result<BankAccount>> CreateAccountAsync(string name, decimal initialBalance);
        Task<Result> DeleteAccountAsync(Guid id);
        Task<Result<BankAccount>> UpdateBalanceAsync(Guid id, decimal newBalance);
        Task<Result<BankAccount>> UpdateNameAsync(Guid id, string newName);
        Task<BankAccount?> GetByIdAsync(Guid id);
        Task<IEnumerable<BankAccount>> GetAllAsync();
    }
}