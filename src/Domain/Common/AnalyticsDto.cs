namespace Domain.Common
{
    public record CategoryExpense(Guid CategoryId, decimal Total);
    public record DateBalance(DateTime Date, decimal Balance);

}