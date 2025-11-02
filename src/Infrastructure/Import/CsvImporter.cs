using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Factories;
using Infrastructure.Repositories;

namespace Infrastructure.Import
{
    public class CsvImporter<T>(IRepository<T> repository, IDomainFactory factory)
        : FileImporterBase<T>(repository) where T : class
    {
        public override string FileExtension => ".csv";

        protected override IEnumerable<T> Parse(string content)
        {
            using StringReader reader = new(content);
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using CsvReader csv = new(reader, config);

            try
            {
                if (typeof(T) == typeof(BankAccount))
                {
                    List<BankAccountDto> dtos = csv.GetRecords<BankAccountDto>().ToList();
                    return dtos.Select(d => factory.CreateBankAccount(d.Name, d.Balance, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Category))
                {
                    List<CategoryDto> dtos = csv.GetRecords<CategoryDto>().ToList();
                    return dtos.Select(d => factory.CreateCategory(d.Type, d.Name, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Operation))
                {
                    List<OperationDto> dtos = csv.GetRecords<OperationDto>().ToList();
                    return dtos.Select(d => factory.CreateOperation(
                        d.Type, d.BankAccountId, d.CategoryId, d.Amount, d.Date, d.Description, d.Id) as T)!;
                }

                throw new ImportException($"Тип {typeof(T).Name} не поддерживается CSV импортером.");
            }
            catch (Exception ex)
            {
                throw new ImportException($"Ошибка импорта CSV: {ex.Message}", ex);
            }
        }
    }
}
