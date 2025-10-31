using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Factories;
using Infrastructure.Repositories;
using System.Text.Json;

namespace Infrastructure.Import
{
    [Importer(".json")]
    public class JsonImporter<T>(IRepository<T> repository, IDomainFactory domainFactory)
        : FileImporterBase<T>(repository) where T : class
    {
        protected override IEnumerable<T> Parse(string content)
        {
            try
            {
                // Здесь мы можем использовать фабрику для создания доменных объектов
                // если T — это BankAccount, Category или Operation
                if (typeof(T) == typeof(BankAccount))
                {
                    List<BankAccountDto>? dtos = JsonSerializer.Deserialize<List<BankAccountDto>>(content)!;
                    return dtos.Select(d => domainFactory.CreateBankAccount(d.Name, d.Balance, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Category))
                {
                    List<CategoryDto>? dtos = JsonSerializer.Deserialize<List<CategoryDto>>(content)!;
                    return dtos.Select(d => domainFactory.CreateCategory(d.Type, d.Name, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Operation))
                {
                    List<OperationDto>? dtos = JsonSerializer.Deserialize<List<OperationDto>>(content)!;
                    return dtos.Select(d => domainFactory.CreateOperation(d.Type, d.BankAccountId, d.CategoryId,
                        d.Amount, d.Date, d.Description, d.Id) as T)!;
                }
            }
            catch (Exception e) when (e is FileNotFoundException or JsonException or FormatException or InvalidDataException)
            {
                throw new ImportException($"Ошибка формата Json: {e.Message}");
            }

            throw new NotSupportedException($"Type {typeof(T).Name} is not supported for JSON import.");
        }
    }
}
