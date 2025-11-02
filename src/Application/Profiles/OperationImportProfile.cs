using Domain.Common;
using Domain.Entities;
using Domain.Factories;

namespace Application.Profiles
{
    public class OperationImportProfile(IDomainFactory factory) : IImportProfile<Operation, OperationDto>
    {
        public IEnumerable<Operation> Map(IEnumerable<OperationDto> dtos)
        {
            return dtos.Select(d => factory.CreateOperation(
                d.Type, d.BankAccountId, d.CategoryId, d.Amount, d.Date, d.Description, d.Id));
        }
    }
}