namespace Infrastructure.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}