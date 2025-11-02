namespace Application.Profiles
{
    public interface IImportProfile<TDomain, TDto>
    {
        IEnumerable<TDomain> Map(IEnumerable<TDto> dtos);
    }
}