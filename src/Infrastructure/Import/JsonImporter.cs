using Domain.Common;
using System.Text.Json;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Factories;
using Infrastructure.Repositories;

namespace Infrastructure.Import
{
    public class JsonImporter<T>(IRepository<T> repository, IDomainFactory factory)
        : FileImporterBase<T>(repository) where T : class
    {
        public override string FileExtension => ".json";

        protected override IEnumerable<T> Parse(string content)
        {
            try
            {
                if (typeof(T) == typeof(BankAccount))
                {
                    List<BankAccountDto>? dtos = JsonSerializer.Deserialize<List<BankAccountDto>>(content)!;
                    return dtos.Select(d => factory.CreateBankAccount(d.Name, d.Balance, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Category))
                {
                    List<CategoryDto>? dtos = JsonSerializer.Deserialize<List<CategoryDto>>(content)!;
                    return dtos.Select(d => factory.CreateCategory(d.Type, d.Name, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Operation))
                {
                    List<OperationDto>? dtos = JsonSerializer.Deserialize<List<OperationDto>>(content)!;
                    return dtos.Select(d => factory.CreateOperation(
                        d.Type, d.BankAccountId, d.CategoryId, d.Amount, d.Date, d.Description, d.Id) as T)!;
                }

                throw new ImportException($"Тип {typeof(T).Name} не поддерживается JSON импортером.");
            }
            catch (Exception ex)
            {
                throw new ImportException($"Ошибка импорта JSON: {ex.Message}", ex);
            }
        }
    }
}
