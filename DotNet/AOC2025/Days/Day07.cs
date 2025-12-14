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
        
        // Track distinct timelines by their endpoint
        var timelinesByEndpoint = new Dictionary<(int row, int col), int>();
        var queue = new Queue<(List<(int row, int col)>, bool isTerminal)>();
        var processedPaths = new HashSet<string>();
        
        var initialPath = new List<(int row, int col)> { (0, startCol) };
        queue.Enqueue((initialPath, false));
        
        while (queue.Count > 0)
        {
            var (path, _) = queue.Dequeue();
            var (row, col) = path[^1];
            
            // Check if we've already processed this exact path
            var pathId = string.Join("|", path);
            if (processedPaths.Contains(pathId))
                continue;
            processedPaths.Add(pathId);
            
            // Out of bounds - this timeline ends
            if (row < 0 || row >= grid.Length || col < 0 || col >= grid[0].Length)
            {
                // Count this as an endpoint
                var lastPos = path[^2];  // Second to last element
                if (!timelinesByEndpoint.ContainsKey(lastPos))
                    timelinesByEndpoint[lastPos] = 0;
                timelinesByEndpoint[lastPos]++;
                continue;
            }
            
            char cell = grid[row][col];
            
            if (cell == '^')
            {
                // Splitter: two paths branch
                queue.Enqueue((new List<(int row, int col)>(path) { (row, col - 1) }, false));
                queue.Enqueue((new List<(int row, int col)>(path) { (row, col + 1) }, false));
            }
            else
            {
                // Continue down
                queue.Enqueue((new List<(int row, int col)>(path) { (row + 1, col) }, false));
            }
        }
        
        // Debug
        if (input.Length < 500)
        {
            Console.WriteLine($"    [DEBUG] Unique endpoint positions: {timelinesByEndpoint.Count}");
            Console.WriteLine($"    [DEBUG] Total paths processed: {processedPaths.Count}");
        }
        
        return timelinesByEndpoint.Count.ToString();
    }
}
