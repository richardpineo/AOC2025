// Day 2: Invalid ID Detection
// Part 1: Sum all IDs that are exactly repeated twice (e.g., 123123, 77)
// Part 2: Sum all IDs with any repeating pattern (2+ repetitions, e.g., 123123, 77, 555)
//
// Problem: Given ranges of numeric IDs, identify "invalid" ones based on digit patterns
// and calculate their sum. Part 1 uses strict doubling, Part 2 uses any repetition.

namespace AOC2025.Days;

using AOC2025.Common;

public class Day02 : DayBase
{
    public Day02() : base(2) { }

    // When true, Part2 will print per-range diagnostics (invalid IDs found)
    public static bool Verbose { get; set; } = false;

    private static bool IsInvalidId(long id)
    {
        var idStr = id.ToString();
        int len = idStr.Length;

        // Check for any pattern that repeats >= 2 times to make up the full ID
        for (int patternLen = 1; patternLen <= len / 2; patternLen++)
        {
            if (len % patternLen != 0) continue;
            int reps = len / patternLen;
            if (reps < 2) continue;

            var pattern = idStr.Substring(0, patternLen);
            bool ok = true;
            for (int i = patternLen; i < len; i += patternLen)
            {
                if (idStr.Substring(i, patternLen) != pattern) { ok = false; break; }
            }
            if (ok) return true;
        }

        return false;
    }

    // Part 1 predicate: invalid if the ID is made of some sequence repeated exactly twice
    private static bool IsInvalidPart1(long id)
    {
        var s = id.ToString();
        int len = s.Length;
        // pattern must be exactly half the length
        if (len % 2 != 0) return false;
        int half = len / 2;
        var a = s.Substring(0, half);
        var b = s.Substring(half, half);
        return a == b;
    }

    private static long SumInvalidIds(long start, long end)
    {
        long sum = 0;
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidPart1(id))
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
        // Part 2 - Sum all invalid IDs (an ID is invalid if any digit-sequence repeats >= 2 times)
        var ranges = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToArray();
        long totalSum = 0;

        foreach (var range in ranges)
        {
            var parts = range.Split('-');
            if (!long.TryParse(parts[0], out var start) || !long.TryParse(parts[1], out var end))
                continue;

            if (Verbose)
            {
                var found = new List<long>();
                for (long id = start; id <= end; id++)
                {
                    if (IsInvalidId(id)) found.Add(id);
                }

                if (found.Count == 0)
                    Console.WriteLine($"{range} still contains no invalid IDs.");
                else if (found.Count == 1)
                    Console.WriteLine($"{range} still has one invalid ID, {found[0]}.");
                else if (found.Count == 2)
                    Console.WriteLine($"{range} has two invalid IDs, {found[0]} and {found[1]}.");
                else
                    Console.WriteLine($"{range} has {found.Count} invalid IDs: {string.Join(", ", found)}.");

                totalSum += found.Sum();
            }
            else
            {
                totalSum += SumInvalidIdsPart2(start, end);
            }
        }

        return totalSum.ToString();
    }

    private static long SumInvalidIdsPart2(long start, long end)
    {
        long sum = 0;
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidId(id)) sum += id;
        }
        return sum;
    }

    // Public diagnostics helpers
    public static List<string> GetMatchingPatterns(long id)
    {
        var idStr = id.ToString();
        var matches = new List<string>();
        int len = idStr.Length;
        for (int patternLen = 1; patternLen <= len / 2; patternLen++)
        {
            if (len % patternLen != 0) continue;
            int reps = len / patternLen;
            if (reps < 2) continue;

            var pattern = idStr.Substring(0, patternLen);
            bool ok = true;
            for (int i = patternLen; i < len; i += patternLen)
            {
                if (idStr.Substring(i, patternLen) != pattern) { ok = false; break; }
            }
            if (ok) matches.Add(pattern);
        }
        return matches;
    }

    public static bool IsInvalidPublic(long id) => GetMatchingPatterns(id).Count > 0;
}
