using AOC2025.Common;

namespace AOC2025.Days;

public class Day09 : DayBase
{
    public Day09() : base(9) { }

    public override string Part1(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // Parse red tile coordinates
        var tiles = new List<(int x, int y)>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            tiles.Add((int.Parse(parts[0]), int.Parse(parts[1])));
        }
        
        // Find the largest rectangle using any two tiles as opposite corners
        // Area includes both corner tiles, so add 1 to each dimension
        long maxArea = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = i + 1; j < tiles.Count; j++)
            {
                long width = Math.Abs(tiles[j].x - tiles[i].x) + 1;
                long height = Math.Abs(tiles[j].y - tiles[i].y) + 1;
                long area = width * height;
                maxArea = Math.Max(maxArea, area);
            }
        }
        
        return maxArea.ToString();
    }

    public override string Part2(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // TODO: Implement part 2
        return "0";
    }
}
