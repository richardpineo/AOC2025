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
        
        // Count all paths from "you" to "out" using DFS
        int pathCount = CountPaths(graph, "you", new HashSet<string>());
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
        // TODO: Implement Day 11 Part 2
        return "0";
    }
}
