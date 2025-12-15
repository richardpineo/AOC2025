using AOC2025.Days;
using AOC2025.Common;

// Normalize args to avoid nullable warnings and simplify parsing
var argv = args ?? Array.Empty<string>();

// Diagnostic runner: pass `diag2` to print per-range invalid IDs for Day 2
// Diagnostic: pass `diag2id <number>` to check a single ID (prints patterns and validity)
if (argv.Length > 0 && argv[0] == "diag2id")
{
    if (argv.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run diag2id <number>");
        return;
    }

    if (!long.TryParse(argv[1], out var id))
    {
        Console.WriteLine($"Invalid number: {argv[1]}");
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

if (argv.Length > 0 && argv[0] == "diag2")
{
    Day02.Verbose = true;
}

// Normal runner with optional CLI filters: --day <n> --part <1|2> [--test-only|--input-only]
var days = new DayBase[]
{
    new Day01(),
    new Day02(),
    new Day03(),
    new Day04(),
    new Day05(),
    new Day06(),
    new Day07(),
    new Day08(),
    new Day09(),
    new Day10(),
    new Day11(),
    new Day12(),
    // Add more days as you solve them
};

int? argDay = null; // single day filter
int? argPart = null; // 1 or 2
bool testOnly = false;
bool inputOnly = false;
bool showHelp = false;

// Basic args parsing
for (int i = 0; i < argv.Length; i++)
{
    switch (argv[i])
    {
        case "--day":
        case "-d":
            if (i + 1 < argv.Length && int.TryParse(argv[i + 1], out var d))
            {
                argDay = d;
                i++;
            }
            break;
        case "--part":
        case "-p":
            if (i + 1 < argv.Length && int.TryParse(argv[i + 1], out var p) && (p == 1 || p == 2))
            {
                argPart = p;
                i++;
            }
            break;
        case "--test-only":
            testOnly = true;
            break;
        case "--input-only":
            inputOnly = true;
            break;
        case "--help":
        case "-h":
            showHelp = true;
            break;
    }
}

if (showHelp)
{
    Console.WriteLine("Usage: dotnet run -- [--day <n>] [--part 1|2] [--test-only|--input-only] [--help]\n" +
                      "Examples:\n" +
                      "  dotnet run -- --day 10\n" +
                      "  dotnet run -- --day 10 --part 2 --test-only\n" +
                      "  dotnet run -- --day 10 --part 2 --input-only");
    return;
}

IEnumerable<DayBase> selected = days;
if (argDay.HasValue)
{
    selected = days.Where(d => d.DayNumber == argDay.Value);
}

void RunOnePart(DayBase day, int part, bool useTest)
{
    var path = useTest ? day.TestPath : day.InputPath;
    if (!File.Exists(path))
    {
        Console.WriteLine($"  (File not found: {Path.GetFileName(path)})");
        return;
    }
    var input = FileHelper.ReadFile(path);
    var sw = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        var task = System.Threading.Tasks.Task.Run(() => part == 1 ? day.Part1(input) : day.Part2(input));
        if (task.Wait(TimeSpan.FromSeconds(10)))
        {
            sw.Stop();
            Console.WriteLine($"  Part {part}: {task.Result} ({sw.Elapsed.TotalMilliseconds:F2}ms)");
        }
        else
        {
            sw.Stop();
            Console.WriteLine($"  Part {part}: TIMEOUT after 10 seconds");
        }
    }
    catch (Exception ex)
    {
        sw.Stop();
        Console.WriteLine($"  Part {part}: ERROR - {ex.GetBaseException().Message}");
    }
}

foreach (var day in selected)
{
    Console.WriteLine($"=== Day {day.DayNumber:D2} ===");

    bool runTest = !inputOnly;  // default run both
    bool runInput = !testOnly;

    if (argPart.HasValue)
    {
        if (runTest)
        {
            Console.WriteLine("TEST:");
            RunOnePart(day, argPart.Value, useTest: true);
        }
        if (runInput)
        {
            Console.WriteLine("INPUT:");
            RunOnePart(day, argPart.Value, useTest: false);
        }
    }
    else
    {
        if (runTest)
        {
            Console.WriteLine("TEST:");
            day.RunBoth(useTest: true);
        }
        if (runInput)
        {
            Console.WriteLine("INPUT:");
            day.RunBoth(useTest: false);
        }
    }

    Console.WriteLine();
}
