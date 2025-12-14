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
        
        // BFS - track all cells visited
        var queue = new Queue<(int row, int col, int drrow, int drCol)>();
        var visitedStates = new HashSet<(int row, int col, int drrow, int drCol)>();
        var visitedCells = new HashSet<(int row, int col)>();
        
        queue.Enqueue((0, startCol, 1, 0));  // Start at S, moving down
        
        while (queue.Count > 0)
        {
            var (row, col, drrow, drCol) = queue.Dequeue();
            
            // Out of bounds
            if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            {
                continue;
            }
            
            // Skip if we've seen this state before
            if (visitedStates.Contains((row, col, drrow, drCol)))
            {
                continue;
            }
            visitedStates.Add((row, col, drrow, drCol));
            visitedCells.Add((row, col));
            
            char cell = grid[row][col];
            
            if (cell == '^')
            {
                // Splitter: two new beams created to the left and right
                queue.Enqueue((row, col - 1, 1, 0));  // Beam to the left, moving down
                queue.Enqueue((row, col + 1, 1, 0));  // Beam to the right, moving down
            }
            else
            {
                // Empty space or 'S': continue moving
                queue.Enqueue((row + drrow, col + drCol, drrow, drCol));
            }
        }
        
        return visitedCells.Count.ToString();
    }
}
