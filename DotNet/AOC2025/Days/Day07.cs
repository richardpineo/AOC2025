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
        
        // Use memoization to count unique paths efficiently
        var memo = new Dictionary<(int row, int col, int pathHash), int>();
        
        int CountPaths(int row, int col, int pathHash)
        {
            // Out of bounds
            if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            {
                return 1;  // One unique path terminates here
            }
            
            var key = (row, col, pathHash);
            if (memo.ContainsKey(key))
                return memo[key];
            
            char cell = grid[row][col];
            int result = 0;
            
            if (cell == '^')
            {
                // Splitter: two different paths
                int leftHash = pathHash * 31 + 0;  // Hash for "left"
                int rightHash = pathHash * 31 + 1; // Hash for "right"
                
                result = CountPaths(row, col - 1, leftHash) + 
                         CountPaths(row, col + 1, rightHash);
            }
            else
            {
                // Empty or 'S': continue down
                result = CountPaths(row + 1, col, pathHash);
            }
            
            memo[key] = result;
            return result;
        }
        
        int uniquePaths = CountPaths(0, startCol, 0);
        return uniquePaths.ToString();
    }
}
