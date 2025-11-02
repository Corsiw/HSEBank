using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Factories;

namespace Application.Services
{
    public class OperationService(
        IRepository<Operation> repository,
        IRepository<BankAccount> bankAccountRepository,
        IRepository<Category> categoryRepository,
        IDomainFactory factory)
        : IOperationService
    {
        public async Task<Result<Operation>> AddOperationAsync(
            MoneyType type,
            Guid bankAccountId,
            Guid categoryId,
            decimal amount,
            DateTime date,
            string? description = null)
        {
            try
            {
                // Проверяем на корректность в Application слое, потому что Domain не знает про репозитории
                BankAccount? account = await bankAccountRepository.GetAsync(bankAccountId);
                if (account == null)
                {
                    return Result<Operation>.Fail("Счёт не найден.");
                }

                Category? category = await categoryRepository.GetAsync(categoryId);
                if (category == null)
                {
                    return Result<Operation>.Fail("Категория не найдена.");
                }

                Operation operation =
                    factory.CreateOperation(type, bankAccountId, categoryId, amount, date, description);

                if (type == MoneyType.Income)
                {
                    account.UpdateBalance(account.Balance + amount);
                }
                else
                {
                    account.UpdateBalance(account.Balance - amount);
                }

                await repository.AddAsync(operation);
                await bankAccountRepository.UpdateAsync(account);

                return Result<Operation>.Ok(operation);
            }
            catch (ArgumentException ex)
            {
                return Result<Operation>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Operation>.Fail($"Ошибка при добавлении операции: {ex.Message}");
            }
        }

        public async Task<Result> DeleteOperationAsync(Guid id)
        {
            try
            {
                await repository.DeleteAsync(id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении операции: {ex.Message}");
            }
        }

        public async Task<Result<Operation>> UpdateDescriptionAsync(Guid id, string description)
        {
            try
            {
                Operation? operation = await repository.GetAsync(id);
                if (operation == null)
                {
                    return Result<Operation>.Fail("Операция не найдена.");
                }

                operation.UpdateDescription(description);
                await repository.UpdateAsync(operation);

                return Result<Operation>.Ok(operation);
            }
            catch (ArgumentException ex)
            {
                return Result<Operation>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Operation>.Fail($"Ошибка при обновлении операции: {ex.Message}");
            }
        }

        public async Task<Operation?> GetByIdAsync(Guid id)
        {
            return await repository.GetAsync(id);
        }

        public async Task<IEnumerable<Operation>> GetAllAsync()
        {
            return await repository.ListAsync();
        }
    }
}