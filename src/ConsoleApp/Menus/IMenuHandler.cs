namespace ConsoleApp.Menus
{
    public interface IMenuHandler
    {
        Task HandleAsync();
        string Name { get; }
    }
}