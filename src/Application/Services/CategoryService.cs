using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Factories;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class CategoryService(IRepository<Category> repository, IDomainFactory factory) : ICategoryService
    {
        public async Task<Result<Category>> CreateCategoryAsync(MoneyType type, string name)
        {
            try
            {
                Category category = factory.CreateCategory(type, name);
                await repository.AddAsync(category);
                return Result<Category>.Ok(category);
            }
            catch (ArgumentException ex)
            {
                return Result<Category>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Category>.Fail($"Ошибка при создании категории: {ex.Message}");
            }
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            try
            {
                await repository.DeleteAsync(id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении категории: {ex.Message}");
            }
        }

        public async Task<Result<Category>> UpdateNameAsync(Guid id, string newName)
        {
            try
            {
                Category? category = await repository.GetAsync(id);
                if (category == null)
                {
                    return Result<Category>.Fail("Категория не найдена.");
                }

                category.UpdateName(newName);
                await repository.UpdateAsync(category);

                return Result<Category>.Ok(category);
            }
            catch (ArgumentException ex)
            {
                return Result<Category>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Category>.Fail($"Ошибка при обновлении категории: {ex.Message}");
            }
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await repository.GetAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await repository.ListAsync();
        }
    }
}