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
        
        // Count unique leaf endpoints (where particle exits)
        var endpoints = new HashSet<(int row, int col)>();
        var visitedStates = new HashSet<(int row, int col, int dirRow, int dirCol)>();
        
        DFS(0, startCol, 1, 0, grid, endpoints, visitedStates);
        
        return endpoints.Count.ToString();
    }
    
    private void DFS(int row, int col, int dirRow, int dirCol, char[][] grid, 
                     HashSet<(int row, int col)> endpoints, 
                     HashSet<(int row, int col, int dirRow, int dirCol)> visitedStates)
    {
        // Out of bounds - this is an endpoint
        if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
        {
            return;
        }
        
        // Prevent infinite loops
        if (visitedStates.Contains((row, col, dirRow, dirCol)))
        {
            // Mark as endpoint if we hit a loop
            endpoints.Add((row, col));
            return;
        }
        visitedStates.Add((row, col, dirRow, dirCol));
        
        char cell = grid[row][col];
        
        if (cell == '^')
        {
            // Splitter: create two branches
            DFS(row, col - 1, 1, 0, grid, endpoints, visitedStates);  
            DFS(row, col + 1, 1, 0, grid, endpoints, visitedStates);   
        }
        else
        {
            // Empty space or 'S': continue
            DFS(row + dirRow, col + dirCol, dirRow, dirCol, grid, endpoints, visitedStates);
        }
    }
}
