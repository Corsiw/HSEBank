namespace ConsoleApp
{
    public static class InputHelper
    {
        public static string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        public static decimal ReadDecimal(string prompt)
        {
            Console.Write(prompt);
            return decimal.TryParse(Console.ReadLine(), out decimal value) ? value : 0m;
        }

        public static Guid ReadGuid(string prompt)
        {
            Console.Write(prompt);
            return Guid.TryParse(Console.ReadLine(), out Guid id) ? id : Guid.Empty;
        }

        public static T ReadEnum<T>(string prompt) where T : struct, Enum
        {
            Console.Write(prompt);
            return Enum.TryParse(Console.ReadLine(), out T value) ? value : default;
        }
    }
}