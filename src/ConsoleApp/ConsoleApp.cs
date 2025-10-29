using ConsoleApp.Menus;

namespace ConsoleApp
{
    public class ConsoleApp(IEnumerable<IMenuHandler> menuHandlers)
    {
        private readonly List<IMenuHandler> _menuHandlers = menuHandlers.ToList();

        public async Task RunAsync()
        {
            string? cmd;
            do
            {
                Console.WriteLine($"{Environment.NewLine}=== Модуль 'Учет финансов' ===");
                for (int i = 0; i < _menuHandlers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_menuHandlers[i].Name}");
                }

                Console.WriteLine($"{_menuHandlers.Count + 1}. Выйти");

                Console.Write("Выберите раздел: ");
                cmd = Console.ReadLine();
                if (int.TryParse(cmd, out int idx) && idx >= 1 && idx <= _menuHandlers.Count)
                {
                    await _menuHandlers[idx - 1].HandleAsync();
                }

            } while (cmd != (_menuHandlers.Count + 1).ToString());
        }
    }
}