// Day 4: Toilet Paper Roll Accessibility
// Part 1: Count rolls accessible to elves (< 4 adjacent roll neighbors)
// Part 2: Iteratively remove accessible rolls until none remain, count total removed
//
// Problem: Given a grid where '@' represents toilet paper rolls, determine which
// rolls are "accessible" (have fewer than 4 of the 8 neighbors also being rolls).
// Part 2 simulates elves taking accessible rolls iteratively.

using AOC2025.Common;

namespace AOC2025.Days;

public class Day04 : DayBase
{
    public Day04() : base(4) { }

    public override string Part1(string input)
    {
        var lines = input.Trim('\n').Split('\n').Select(l => l.TrimEnd()).ToArray();
        if (lines.Length == 0) return "0";

        int rows = lines.Length;
        int cols = lines[0].Length;

        int[] dr = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dc = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };

        int accessible = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (lines[r][c] != '@') continue;

                int neighbors = 0;
                for (int k = 0; k < 8; k++)
                {
                    int nr = r + dr[k];
                    int nc = c + dc[k];
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (lines[nr][nc] == '@') neighbors++;
                }

                if (neighbors < 4) accessible++;
            }
        }

        return accessible.ToString();
    }

    public override string Part2(string input)
    {
        var lines = input.Trim('\n').Split('\n').Select(l => l.TrimEnd()).ToArray();
        if (lines.Length == 0) return "0";

        int rows = lines.Length;
        int cols = lines[0].Length;
        
        // Create mutable grid
        var grid = new char[rows][];
        for (int r = 0; r < rows; r++)
        {
            grid[r] = lines[r].ToCharArray();
        }

        int[] dr = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dc = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };

        int totalRemoved = 0;
        bool changed = true;
        
        while (changed)
        {
            changed = false;
            var toRemove = new List<(int r, int c)>();
            
            // Find all accessible rolls
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r][c] != '@') continue;

                    int neighbors = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        int nr = r + dr[k];
                        int nc = c + dc[k];
                        if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                        if (grid[nr][nc] == '@') neighbors++;
                    }

                    if (neighbors < 4)
                    {
                        toRemove.Add((r, c));
                    }
                }
            }
            
            // Remove all accessible rolls
            if (toRemove.Count > 0)
            {
                changed = true;
                totalRemoved += toRemove.Count;
                foreach (var (r, c) in toRemove)
                {
                    grid[r][c] = '.';
                }
            }
        }

        return totalRemoved.ToString();
    }
}
