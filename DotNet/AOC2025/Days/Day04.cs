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
        // Part 2 not implemented yet
        return "0";
    }
}
