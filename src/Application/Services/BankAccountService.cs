using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Factories;

namespace Application.Services
{
    public class BankAccountService(IRepository<BankAccount> repository, IDomainFactory factory)
        : IBankAccountService
    {
        public async Task<Result<BankAccount>> CreateAccountAsync(string name, decimal initialBalance)
        {
            try
            {
                BankAccount account = factory.CreateBankAccount(name, initialBalance);
                await repository.AddAsync(account);

                return Result<BankAccount>.Ok(account);
            }
            catch (ArgumentException ex)
            {
                return Result<BankAccount>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<BankAccount>.Fail($"Ошибка при создании счёта: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAccountAsync(Guid id)
        {
            try
            {
                await repository.DeleteAsync(id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении счёта: {ex.Message}");
            }
        }

        public async Task<Result<BankAccount>> UpdateBalanceAsync(Guid id, decimal newBalance)
        {
            try
            {
                BankAccount? account = await repository.GetAsync(id);
                if (account == null)
                {
                    return Result<BankAccount>.Fail("Счёт не найден.");
                }

                account.UpdateBalance(newBalance);
                await repository.UpdateAsync(account);

                return Result<BankAccount>.Ok(account);
            }
            catch (ArgumentException ex)
            {
                return Result<BankAccount>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<BankAccount>.Fail($"Ошибка при обновлении счёта: {ex.Message}");
            }
        }

        public async Task<Result<BankAccount>> UpdateNameAsync(Guid id, string newName)
        {
            try
            {
                BankAccount? account = await repository.GetAsync(id);
                if (account == null)
                {
                    return Result<BankAccount>.Fail("Счёт не найден.");
                }

                account.UpdateName(newName);
                await repository.UpdateAsync(account);

                return Result<BankAccount>.Ok(account);
            }
            catch (ArgumentException ex)
            {
                return Result<BankAccount>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<BankAccount>.Fail($"Ошибка при обновлении счёта: {ex.Message}");
            }
        }

        public async Task<BankAccount?> GetByIdAsync(Guid id)
        {
            return await repository.GetAsync(id);
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            return await repository.ListAsync();
        }
    }
}
