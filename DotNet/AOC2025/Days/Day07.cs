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
        
        // Use a more efficient representation: only track (position, incoming_direction) to detect true collisions
        // A "timeline" only collapses if it reaches the same position from the same direction
        var activeBeams = new HashSet<(int row, int col, int fromRow, int fromCol)> 
        { 
            (0, startCol, -1, 0)  // Start position with "came from above" indicator
        };
        
        var allVisited = new HashSet<(int row, int col, int fromRow, int fromCol)>();
        
        while (activeBeams.Count > 0)
        {
            var nextBeams = new HashSet<(int row, int col, int fromRow, int fromCol)>();
            
            foreach (var (row, col, fromRow, fromCol) in activeBeams)
            {
                // Skip if we've seen this exact state before (infinite loop detection)
                if (allVisited.Contains((row, col, fromRow, fromCol)))
                {
                    continue;
                }
                allVisited.Add((row, col, fromRow, fromCol));
                
                // Bounds check
                if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
                {
                    continue;
                }
                
                char cell = grid[row][col];
                
                if (cell == '^')
                {
                    // Splitter: create two beams (left and right)
                    nextBeams.Add((row, col - 1, row, col));  // Left beam, came from current position
                    nextBeams.Add((row, col + 1, row, col));  // Right beam, came from current position
                }
                else
                {
                    // Empty space or 'S': continue moving down
                    nextBeams.Add((row + 1, col, row, col));
                }
            }
            
            activeBeams = nextBeams;
        }
        
        // Count unique cells visited (not counting direction)
        var uniqueCells = new HashSet<(int row, int col)>();
        foreach (var (row, col, _, _) in allVisited)
        {
            uniqueCells.Add((row, col));
        }
        
        return uniqueCells.Count.ToString();
    }
}
