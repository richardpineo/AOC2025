using AOC2025.Common;

namespace AOC2025.Days;

public class Day05 : DayBase
{
    public Day05() : base(5) { }

    public override string Part1(string input)
    {
        var parts = input.Trim().Split("\n\n");
        if (parts.Length != 2) return "0";

        // Parse ranges
        var ranges = new List<(long min, long max)>();
        foreach (var line in parts[0].Split('\n'))
        {
            var range = line.Split('-');
            ranges.Add((long.Parse(range[0]), long.Parse(range[1])));
        }

        // Check each ingredient ID
        int freshCount = 0;
        foreach (var line in parts[1].Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            long id = long.Parse(line.Trim());
            
            // Check if ID falls in any range
            bool isFresh = ranges.Any(r => id >= r.min && id <= r.max);
            if (isFresh) freshCount++;
        }

        return freshCount.ToString();
    }

    public override string Part2(string input)
    {
        // TODO: Implement Day 5 Part 2
        return "0";
    }
}
