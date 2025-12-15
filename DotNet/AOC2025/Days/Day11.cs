using AOC2025.Common;

namespace AOC2025.Days;

public class Day11 : DayBase
{
    public Day11() : base(11) { }

    public override string Part1(string input)
    {
        // Parse the graph: each line is "device: output1 output2 ..."
        var graph = new Dictionary<string, List<string>>();
        
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2) continue;
            
            var device = parts[0];
            var outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            graph[device] = outputs;
        }
        
        // Determine start node based on what's in the graph
        string startNode = graph.ContainsKey("you") ? "you" : "svr";
        
        // Count all paths using DFS
        int pathCount = CountPaths(graph, startNode, new HashSet<string>());
        return pathCount.ToString();
    }
    
    private int CountPaths(Dictionary<string, List<string>> graph, string current, HashSet<string> visited)
    {
        // If we reached the exit, count this path
        if (current == "out")
            return 1;
        
        // If node not in graph or already visited, no paths
        if (!graph.ContainsKey(current) || visited.Contains(current))
            return 0;
        
        // Mark current node as visited
        visited.Add(current);
        
        // Explore all neighbors and sum the paths
        int totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPaths(graph, neighbor, visited);
        }
        
        // Backtrack: unmark current node so other paths can visit it
        visited.Remove(current);
        
        return totalPaths;
    }

    public override string Part2(string input)
    {
        // Parse the graph
        var graph = new Dictionary<string, List<string>>();
        
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2) continue;
            
            var device = parts[0];
            var outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            graph[device] = outputs;
        }
        
        // Determine start node based on what's in the graph
        string startNode = graph.ContainsKey("svr") ? "svr" : "you";
        
        // Count paths that visit both "dac" and "fft" using memoization
        var requiredNodes = new HashSet<string> { "dac", "fft" };
        var memo = new Dictionary<string, long>();
        long pathCount = CountPathsWithRequiredMemo(graph, startNode, new HashSet<string>(), requiredNodes, memo);
        return pathCount.ToString();
    }
    
    private long CountPathsWithRequiredMemo(Dictionary<string, List<string>> graph, string current, 
        HashSet<string> visited, HashSet<string> requiredRemaining, Dictionary<string, long> memo)
    {
        // If we reached the exit, count only if we visited all required nodes
        if (current == "out")
            return requiredRemaining.Count == 0 ? 1 : 0;
        
        // If node not in graph or already visited, no paths
        if (!graph.ContainsKey(current) || visited.Contains(current))
            return 0;
        
        // Create memo key: current node + sorted required remaining nodes
        var memoKey = current + ":" + string.Join(",", requiredRemaining.OrderBy(x => x));
        if (memo.ContainsKey(memoKey))
            return memo[memoKey];
        
        // Mark current node as visited
        visited.Add(current);
        
        // Check if current node is one of the required nodes
        bool wasRequired = requiredRemaining.Contains(current);
        if (wasRequired)
            requiredRemaining.Remove(current);
        
        // Explore all neighbors and sum the paths
        long totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPathsWithRequiredMemo(graph, neighbor, visited, requiredRemaining, memo);
        }
        
        // Backtrack
        visited.Remove(current);
        if (wasRequired)
            requiredRemaining.Add(current);
        
        memo[memoKey] = totalPaths;
        return totalPaths;
    }
}
