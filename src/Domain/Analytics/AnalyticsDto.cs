namespace Domain.Analytics
{
    public record CategoryExpense(Guid CategoryId, decimal Total);
    public record DateBalance(DateTime Date, decimal Balance);

}