using AOC2025.Days;
using AOC2025.Common;

// Diagnostic runner: pass `diag2` to print per-range invalid IDs for Day 2
// Diagnostic: pass `diag2id <number>` to check a single ID (prints patterns and validity)
if (args is not null && args.Length > 0 && args[0] == "diag2id")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run diag2id <number>");
        return;
    }

    if (!long.TryParse(args[1], out var id))
    {
        Console.WriteLine($"Invalid number: {args[1]}");
        return;
    }

    var matches = Day02.GetMatchingPatterns(id);
    if (matches.Count == 0)
    {
        Console.WriteLine($"{id} is considered VALID (no repeated-double patterns detected).");
    }
    else
    {
        Console.WriteLine($"{id} is considered INVALID. Matching repeated patterns: {string.Join(", ", matches)}");
    }
    return;
}

if (args is not null && args.Length > 0 && args[0] == "diag2")
{
    Day02.Verbose = true;
}

// Normal runner
var days = new DayBase[]
{
    new Day01(),
    new Day02(),
    // Add more days as you solve them
};

foreach (var day in days)
{
    Console.WriteLine($"=== Day {day.DayNumber:D2} ===");

    // Run test
    if (File.Exists(day.TestPath))
    {
        Console.WriteLine("TEST:");
        day.RunBoth(useTest: true);
    }

    // Run actual
    Console.WriteLine("INPUT:");
    day.RunBoth(useTest: false);

    Console.WriteLine();
}
