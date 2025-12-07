namespace AOC2025.Days;

using AOC2025.Common;

public class Day02 : DayBase
{
    public Day02() : base(2) { }

    private static bool IsInvalidId(long id)
    {
        var idStr = id.ToString();
        int len = idStr.Length;

        // Check for patterns repeated exactly 2 times (not more)
        for (int patternLen = 1; patternLen <= len / 2; patternLen++)
        {
            if (len == patternLen * 2)  // Pattern length times 2 equals total length
            {
                string pattern = idStr.Substring(0, patternLen);
                string second = idStr.Substring(patternLen, patternLen);
                
                if (pattern == second)
                    return true;
            }
        }

        return false;
    }

    private static long SumInvalidIds(long start, long end)
    {
        long sum = 0;
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidId(id))
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
        // Part 2 - Count how many valid IDs are in the ranges
        var ranges = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        long totalCount = 0;

        foreach (var range in ranges)
        {
            var parts = range.Trim().Split('-');
            if (long.TryParse(parts[0], out var start) && long.TryParse(parts[1], out var end))
            {
                long rangeSize = end - start + 1;
                long invalidCount = CountInvalidIds(start, end);
                totalCount += rangeSize - invalidCount;
            }
        }

        return totalCount.ToString();
    }

    private static long CountInvalidIds(long start, long end)
    {
        long count = 0;
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidId(id))
            {
                count++;
            }
        }
        return count;
    }
}
