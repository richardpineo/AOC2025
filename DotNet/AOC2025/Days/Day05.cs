// Day 5: Fresh Ingredient Detection
// Part 1: Count how many ingredient IDs fall within "fresh" ranges
// Part 2: Count total unique IDs covered by all merged fresh ranges
//
// Problem: Given numeric ranges defining "fresh" ingredient IDs and a list of
// ingredient IDs to check, determine freshness. Part 2 requires merging overlapping
// ranges and counting total coverage.

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
        var parts = input.Trim().Split("\n\n");
        if (parts.Length != 2) return "0";

        // Parse ranges
        var ranges = new List<(long min, long max)>();
        foreach (var line in parts[0].Split('\n'))
        {
            var range = line.Split('-');
            ranges.Add((long.Parse(range[0]), long.Parse(range[1])));
        }

        // Merge overlapping ranges to count unique IDs
        ranges = ranges.OrderBy(r => r.min).ToList();
        var merged = new List<(long min, long max)>();
        
        foreach (var range in ranges)
        {
            if (merged.Count == 0 || range.min > merged[^1].max + 1)
            {
                // No overlap, add new range
                merged.Add(range);
            }
            else
            {
                // Overlap or adjacent, merge with last range
                var last = merged[^1];
                merged[^1] = (last.min, Math.Max(last.max, range.max));
            }
        }

        // Count total IDs in merged ranges
        long totalIds = 0;
        foreach (var (min, max) in merged)
        {
            totalIds += (max - min + 1);
        }

        return totalIds.ToString();
    }
}
