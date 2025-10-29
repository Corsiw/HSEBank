using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IOperationService
    {
        Task<Result<Operation>> AddOperationAsync(
            MoneyType type,
            Guid bankAccountId,
            Guid categoryId,
            decimal amount,
            DateTime date,
            string? description = null
        );

        Task<Result> DeleteOperationAsync(Guid id);
        Task<Result<Operation>> UpdateDescriptionAsync(Guid id, string description);
        Task<Operation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Operation>> GetAllAsync();
    }
}