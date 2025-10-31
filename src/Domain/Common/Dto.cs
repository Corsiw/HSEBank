using Domain.Enums;

namespace Domain.Common
{
    public record BankAccountDto(Guid? Id, string Name, decimal Balance);

    public record CategoryDto(Guid? Id, MoneyType Type, string Name);

    public record OperationDto(
        Guid? Id,
        MoneyType Type,
        Guid BankAccountId,
        Guid CategoryId,
        decimal Amount,
        DateTime Date,
        string? Description);
}