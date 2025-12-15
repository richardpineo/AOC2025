// Day 7: Beam Splitter Manifold
// Part 1: Count total beam splits when beam travels down through splitters ('^')
// Part 2: Count total distinct paths from start 'S' to bottom of manifold
//
// Problem: Given a grid with a start position 'S' and splitter symbols '^',
// simulate a beam traveling downward. When hitting '^', beam splits into left/right.
// Part 1 counts splits, Part 2 counts all possible paths using memoization.

using AOC2025.Common;

namespace AOC2025.Days;

public class Day07 : DayBase
{
    public Day07() : base(7) { }

    public override string Part1(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var grid = lines.Select(l => l.ToCharArray()).ToArray();
        
        // Find starting position 'S'
        int startCol = -1;
        for (int col = 0; col < grid[0].Length; col++)
        {
            if (grid[0][col] == 'S')
            {
                startCol = col;
                break;
            }
        }
        
        // Simulate beams
        var activeBeams = new HashSet<(int row, int col)> { (0, startCol) };
        int splitCount = 0;
        
        while (activeBeams.Count > 0)
        {
            var nextBeams = new HashSet<(int row, int col)>();
            
            foreach (var (row, col) in activeBeams)
            {
                // Move beam down
                int nextRow = row + 1;
                
                if (nextRow >= grid.Length)
                {
                    // Beam exits the manifold
                    continue;
                }
                
                char nextCell = grid[nextRow][col];
                
                if (nextCell == '^')
                {
                    // Hit a splitter - beam stops, create two new beams
                    splitCount++;
                    
                    // Add beam going left from splitter
                    if (col > 0)
                    {
                        nextBeams.Add((nextRow, col - 1));
                    }
                    
                    // Add beam going right from splitter
                    if (col < grid[nextRow].Length - 1)
                    {
                        nextBeams.Add((nextRow, col + 1));
                    }
                }
                else
                {
                    // Empty space or 'S' - beam continues downward
                    nextBeams.Add((nextRow, col));
                }
            }
            
            activeBeams = nextBeams;
        }
        
        return splitCount.ToString();
    }

    public override string Part2(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var grid = lines.Select(l => l.ToCharArray()).ToArray();
        
        // Find starting position 'S'
        int startCol = -1;
        for (int col = 0; col < grid[0].Length; col++)
        {
            if (grid[0][col] == 'S')
            {
                startCol = col;
                break;
            }
        }
        
        // Memoization: (row, col) -> number of distinct paths from this position to bottom
        var memo = new Dictionary<(int, int), long>();
        
        long CountPaths(int row, int col)
        {
            // Out of bounds = 1 path (we reached the bottom successfully)
            if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
                return 1;
            
            // Check memo
            var key = (row, col);
            if (memo.ContainsKey(key))
                return memo[key];
            
            char cell = grid[row][col];
            long paths;
            
            if (cell == '^')
            {
                // Splitter: sum of paths going left and right
                long leftPaths = CountPaths(row, col - 1);
                long rightPaths = CountPaths(row, col + 1);
                paths = leftPaths + rightPaths;
            }
            else
            {
                // Empty space or 'S': continue downward
                paths = CountPaths(row + 1, col);
            }
            
            memo[key] = paths;
            return paths;
        }
        
        long result = CountPaths(0, startCol);
        return result.ToString();
    }
}
