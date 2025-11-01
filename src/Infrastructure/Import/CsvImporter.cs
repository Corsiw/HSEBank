using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Factories;
using Infrastructure.Repositories;

namespace Infrastructure.Import
{
    [Importer(".csv")]
    public class CsvImporter<T>(IRepository<T> repository, IDomainFactory domainFactory)
        : FileImporterBase<T>(repository) where T : class
    {
        protected override IEnumerable<T> Parse(string content)
        {
            try
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

                if (typeof(T) == typeof(BankAccount))
                {
                    List<BankAccountDto> dtos = csv.GetRecords<BankAccountDto>().ToList();
                    return dtos.Select(d => domainFactory.CreateBankAccount(d.Name, d.Balance, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Category))
                {
                    List<CategoryDto> dtos = csv.GetRecords<CategoryDto>().ToList();
                    return dtos.Select(d => domainFactory.CreateCategory(d.Type, d.Name, d.Id) as T)!;
                }

                if (typeof(T) == typeof(Operation))
                {
                    List<OperationDto> dtos = csv.GetRecords<OperationDto>().ToList();
                    return dtos.Select(d => domainFactory.CreateOperation(
                        d.Type, d.BankAccountId, d.CategoryId, d.Amount, d.Date, d.Description, d.Id) as T)!;
                }

                throw new ImportException($"Тип {typeof(T).Name} не поддерживается для CSV импорта.");
            }
            catch (HeaderValidationException ex)
            {
                throw new ImportException($"Ошибка структуры CSV-файла: {ex.Message}", ex);
            }
            catch (ReaderException ex)
            {
                throw new ImportException($"Ошибка чтения CSV-файла: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is FormatException or InvalidDataException)
            {
                throw new ImportException($"Ошибка формата CSV: {ex.Message}", ex);
            }
        }
    }
}
