using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> CreateCategoryAsync(MoneyType type, string name);
        Task<Result> DeleteCategoryAsync(Guid id);

        Task<Result<Category>> UpdateNameAsync(Guid id, string newName);
        Task<Category?> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetAllAsync();
    }
}