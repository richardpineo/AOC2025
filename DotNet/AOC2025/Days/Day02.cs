namespace AOC2025.Days;

using AOC2025.Common;

public class Day02 : DayBase
{
    public Day02() : base(2) { }

    private static bool IsInvalidId(string id)
    {
        // Check if the ID is made of a sequence repeated twice
        int half = id.Length / 2;
        if (id.Length % 2 != 0)
            return false;

        string first = id.Substring(0, half);
        string second = id.Substring(half);

        return first == second;
    }

    private static long SumInvalidIds(long start, long end)
    {
        long sum = 0;
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidId(id.ToString()))
            {
                sum += id;
            }
        }
        return sum;
    }

    public override string Part1(string input)
    {
        var ranges = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        long totalSum = 0;

        foreach (var range in ranges)
        {
            var parts = range.Trim().Split('-');
            if (long.TryParse(parts[0], out var start) && long.TryParse(parts[1], out var end))
            {
                totalSum += SumInvalidIds(start, end);
            }
        }

        return totalSum.ToString();
    }

    public override string Part2(string input)
    {
        // Part 2 - TBD
        return "Not implemented";
    }
}
