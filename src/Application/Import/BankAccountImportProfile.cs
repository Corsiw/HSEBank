using Domain.Common;
using Domain.Entities;
using Domain.Factories;

namespace Application.Import
{
    public class BankAccountImportProfile(IDomainFactory factory) : IImportProfile<BankAccount, BankAccountDto>
    {
        public IEnumerable<BankAccount> Map(IEnumerable<BankAccountDto> dtos)
        {
            return dtos.Select(d => factory.CreateBankAccount(d.Name, d.Balance, d.Id));
        }
    }
}