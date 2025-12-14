using AOC2025.Common;

namespace AOC2025.Days;

public class Day09 : DayBase
{
    public Day09() : base(9) { }
    
    private class SpatialIndex
    {
        private Dictionary<(int, int), List<(int x, int y)>> grid = new();
        private int cellSize;
        
        public SpatialIndex(int cellSize)
        {
            this.cellSize = cellSize;
        }
        
        public void Add(int x, int y)
        {
            var cell = (x / cellSize, y / cellSize);
            if (!grid.ContainsKey(cell))
                grid[cell] = new List<(int x, int y)>();
            grid[cell].Add((x, y));
        }
        
        public long CountInRectangle(int minX, int maxX, int minY, int maxY)
        {
            long count = 0;
            int minCellX = minX / cellSize;
            int maxCellX = maxX / cellSize;
            int minCellY = minY / cellSize;
            int maxCellY = maxY / cellSize;
            
            for (int cx = minCellX; cx <= maxCellX; cx++)
            {
                for (int cy = minCellY; cy <= maxCellY; cy++)
                {
                    if (grid.TryGetValue((cx, cy), out var tiles))
                    {
                        foreach (var (x, y) in tiles)
                        {
                            if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                                count++;
                        }
                    }
                }
            }
            return count;
        }
    }

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
        
        // Parse red tile coordinates
        var redTiles = new List<(int x, int y)>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            redTiles.Add((int.Parse(parts[0]), int.Parse(parts[1])));
        }
        // Coordinate compression for X/Y
        var uniqueX = redTiles.Select(t => t.x).Distinct().OrderBy(v => v).ToList();
        var uniqueY = redTiles.Select(t => t.y).Distinct().OrderBy(v => v).ToList();
        var xIndex = new Dictionary<int, int>();
        var yIndex = new Dictionary<int, int>();
        for (int i = 0; i < uniqueX.Count; i++) xIndex[uniqueX[i]] = i;
        for (int i = 0; i < uniqueY.Count; i++) yIndex[uniqueY[i]] = i;

        // Build vertical edges from the red loop (wrap-around)
        var verticalEdges = new List<(int xi, int yStart, int yEnd)>();
        for (int i = 0; i < redTiles.Count; i++)
        {
            var a = redTiles[i];
            var b = redTiles[(i + 1) % redTiles.Count];
            if (a.x == b.x)
            {
                int xi = xIndex[a.x];
                int ya = yIndex[a.y];
                int yb = yIndex[b.y];
                int ys = Math.Min(ya, yb);
                int ye = Math.Max(ya, yb);
                verticalEdges.Add((xi, ys, ye));
            }
        }

        // For each compressed Y strip, compute interior X intervals via even-odd rule
        var interiorIntervalsPerRow = new List<List<(int xL, int xR)>>();
        for (int yRow = 0; yRow < uniqueY.Count - 1; yRow++)
        {
            var crossings = new List<int>();
            foreach (var (xi, yStart, yEnd) in verticalEdges)
            {
                if (yRow >= yStart && yRow < yEnd)
                    crossings.Add(xi);
            }
            crossings.Sort();
            var intervals = new List<(int xL, int xR)>();
            for (int k = 0; k + 1 < crossings.Count; k += 2)
            {
                intervals.Add((crossings[k], crossings[k + 1]));
            }
            interiorIntervalsPerRow.Add(intervals);
        }

        // Rectangle validity: for each Y strip between corners, ensure X span is covered by an interval
        long maxArea = 0;
        for (int i = 0; i < redTiles.Count; i++)
        {
            for (int j = i + 1; j < redTiles.Count; j++)
            {
                var c1 = redTiles[i];
                var c2 = redTiles[j];
                int iL = xIndex[Math.Min(c1.x, c2.x)];
                int iR = xIndex[Math.Max(c1.x, c2.x)];
                int jL = yIndex[Math.Min(c1.y, c2.y)];
                int jR = yIndex[Math.Max(c1.y, c2.y)];

                bool ok = true;
                for (int yRow = jL; yRow < jR && ok; yRow++)
                {
                    var intervals = interiorIntervalsPerRow[yRow];
                    // Binary search for interval containing [iL, iR]
                    int idx = intervals.BinarySearch((iL, 0), Comparer<(int xL, int xR)>.Create((a, b) => a.xL.CompareTo(b.xL)));
                    if (idx < 0) idx = ~idx - 1;
                    if (idx < 0) { ok = false; break; }
                    var (xL, xR) = intervals[idx];
                    if (xL > iL || xR < iR) ok = false;
                }

                if (ok)
                {
                    long width = Math.Abs(c2.x - c1.x) + 1;
                    long height = Math.Abs(c2.y - c1.y) + 1;
                    long area = width * height;
                    if (area > maxArea) maxArea = area;
                }
            }
        }

        return maxArea.ToString();
    }
    
    private bool IsInsideLoop(int x, int y, List<(int x, int y)> loop)
    {
        // Ray casting algorithm
        int crossings = 0;
        for (int i = 0; i < loop.Count; i++)
        {
            var p1 = loop[i];
            var p2 = loop[(i + 1) % loop.Count];
            
            if ((p1.y <= y && p2.y > y) || (p2.y <= y && p1.y > y))
            {
                double xIntersect = p1.x + (double)(y - p1.y) / (p2.y - p1.y) * (p2.x - p1.x);
                if (x < xIntersect)
                {
                    crossings++;
                }
            }
        }
        return (crossings % 2) == 1;
    }
    
    private bool IsRectangleValidFast((int x, int y) corner1, (int x, int y) corner2, SpatialIndex spatialIndex)
    {
        int minX = Math.Min(corner1.x, corner2.x);
        int maxX = Math.Max(corner1.x, corner2.x);
        int minY = Math.Min(corner1.y, corner2.y);
        int maxY = Math.Max(corner1.y, corner2.y);
        
        long expectedArea = (long)(maxX - minX + 1) * (maxY - minY + 1);
        long validCount = spatialIndex.CountInRectangle(minX, maxX, minY, maxY);
        
        // Rectangle is valid only if it's completely filled with valid tiles
        return validCount == expectedArea;
    }
}
