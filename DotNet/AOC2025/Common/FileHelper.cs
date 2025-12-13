namespace AOC2025.Common;

public static class FileHelper
{
    public static string GetInputPath(int day)
    {
        var baseDir = AppContext.BaseDirectory;
        return Path.Combine(baseDir, "Resources", $"Day{day:D2}_Input.txt");
    }

    public static string GetTestPath(int day)
    {
        var baseDir = AppContext.BaseDirectory;
        return Path.Combine(baseDir, "Resources", $"Day{day:D2}_Test.txt");
    }

    public static string ReadFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        return File.ReadAllText(path);
    }

    public static string[] ReadLines(string path)
    {
        return ReadFile(path).Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
}
