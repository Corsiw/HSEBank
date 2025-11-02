using Domain.Common;
using Domain.Entities;
using Domain.Factories;

namespace Application.Import
{
    public class CategoryImportProfile(IDomainFactory factory) : IImportProfile<Category, CategoryDto>
    {
        public IEnumerable<Category> Map(IEnumerable<CategoryDto> dtos)
        {
            return dtos.Select(d => factory.CreateCategory(d.Type, d.Name, d.Id));
        }
    }
}