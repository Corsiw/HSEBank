namespace Infrastructure.Data;

public static class DbPathProvider
{
    public static string GetDatabasePath()
    {
        // Базовая директория сборки
        string baseDir = AppContext.BaseDirectory;

        // Путь к корню решения (на уровне .sln)
        // поднимаемся на 5 уровней (ConsoleApp/bin/Debug/net8.0 -> src/ConsoleApp -> src -> root)
        string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));

        // Каталог data
        string dataDir = Path.Combine(solutionRoot, "data");
        Directory.CreateDirectory(dataDir);

        return Path.Combine(dataDir, "finance.db");
    }

    public static string GetConnectionString()
    {
        return $"Data Source={GetDatabasePath()}";
    }
}