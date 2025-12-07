using AOC2025.Days;
using AOC2025.Common;

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
