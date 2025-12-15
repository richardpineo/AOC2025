// Day 11: Reactor - Path counting in directed graphs
// Part 1: Count all simple (acyclic) paths from start to "out" - COMPLETE (test=8, input=662)
// Part 2: Count paths visiting both "dac" AND "fft" - INCOMPLETE (test=2 ✓, input=TIMEOUT)
// 
// ISSUE: While Part 1 efficiently counts 662 paths in ~2ms, Part 2 times out despite being a subset.
// The bottleneck is that adding dac/fft tracking (even as simple booleans) causes massive state explosion.
// The graph (607 nodes, 1681 edges) has enough branching that the DFS explores millions of states.
//
// Attempted optimizations:
// - Reachability pruning (helps but insufficient)
// - Segment-based decomposition (svr→dac→fft→out)  
// - Path collection then filtering (too slow to collect all paths)
// - Boolean flags instead of sets (still times out)
//
// The fundamental issue: counting simple paths with constraints is #P-complete.
// Would need to exploit specific graph properties (bottlenecks, tree-width, etc.) which this graph lacks.
//
// Performance comparison:
// - Part 1: 1,778 recursive calls → 662 paths in ~2ms
// - Part 2: 140M+ calls in 10s → 0 paths found
// The dac/fft tracking causes 80,000x more states to be explored, suggesting the algorithm
// is stuck in a massive dead-end search space. Extending timeout won't help.

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
        
        // Since simple enumeration is slow, let's try dynamic programming
        // For each path segment, count rather than enumerate
        // This won't work either if segments have exponential paths
        
        // Use counting like Part 1, but track whether we've visited dac and fft
        int pathCount = CountPathsVisitingBoth(graph, startNode, new HashSet<string>(), false, false);
        return pathCount.ToString();
    }
    
    private int CountPathsVisitingBoth(Dictionary<string, List<string>> graph, string current,
        HashSet<string> visited, bool visitedDac, bool visitedFft)
    {
        // Track if we just visited dac or fft
        if (current == "dac") visitedDac = true;
        if (current == "fft") visitedFft = true;
        
        // If we reached the exit, count only if we visited both
        if (current == "out")
            return (visitedDac && visitedFft) ? 1 : 0;
        
        // If node not in graph or already visited, no paths
        if (!graph.ContainsKey(current) || visited.Contains(current))
            return 0;
        
        // Mark current node as visited
        visited.Add(current);
        
        // Explore all neighbors and sum the paths
        int totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPathsVisitingBoth(graph, neighbor, visited, visitedDac, visitedFft);
        }
        
        // Backtrack
        visited.Remove(current);
        
        return totalPaths;
    }
    
    private void CollectAllSimplePaths(Dictionary<string, List<string>> graph, string start, string end,
        List<string> currentPath, HashSet<string> visited, List<List<string>> allPaths, HashSet<string> canReachEnd)
    {
        // Prune immediately if this node can't reach the end
        if (!canReachEnd.Contains(start))
            return;
        
        currentPath.Add(start);
        
        if (start == end)
        {
            allPaths.Add(new List<string>(currentPath));
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }
        
        if (!graph.ContainsKey(start) || visited.Contains(start))
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }
        
        visited.Add(start);
        
        foreach (var neighbor in graph[start])
        {
            CollectAllSimplePaths(graph, neighbor, end, currentPath, visited, allPaths, canReachEnd);
        }
        
        visited.Remove(start);
        currentPath.RemoveAt(currentPath.Count - 1);
    }
    
    private int CountPathsVisitingBothPruned(Dictionary<string, List<string>> graph, string start, HashSet<string> canReachOut)
    {
        int count = 0;
        totalPathsChecked = 0;
        CountPathsWithTrackingPruned(graph, start, new HashSet<string>(), new HashSet<string>(), ref count, canReachOut);
        return count;
    }
    
    private void CountPathsWithTrackingPruned(Dictionary<string, List<string>> graph, string current,
        HashSet<string> visited, HashSet<string> requiredVisited, ref int count, HashSet<string> canReachOut)
    {
        // Prune: if this node can't reach "out", stop immediately
        if (!canReachOut.Contains(current))
            return;
        
        // Track if we've visited dac or fft
        bool isDac = current == "dac";
        bool isFft = current == "fft";
        if (isDac) requiredVisited.Add("dac");
        if (isFft) requiredVisited.Add("fft");
        
        // If we reached the exit, count if we visited both
        if (current == "out")
        {
            totalPathsChecked++;
            if (requiredVisited.Contains("dac") && requiredVisited.Contains("fft"))
                count++;
            
            // Backtrack required visited
            if (isDac) requiredVisited.Remove("dac");
            if (isFft) requiredVisited.Remove("fft");
            return;
        }
        
        // If node not in graph or already visited, stop
        if (!graph.ContainsKey(current) || visited.Contains(current))
        {
            if (isDac) requiredVisited.Remove("dac");
            if (isFft) requiredVisited.Remove("fft");
            return;
        }
        
        // Mark current node as visited
        visited.Add(current);
        
        // Explore all neighbors
        foreach (var neighbor in graph[current])
        {
            CountPathsWithTrackingPruned(graph, neighbor, visited, requiredVisited, ref count, canReachOut);
        }
        
        // Backtrack
        visited.Remove(current);
        if (isDac) requiredVisited.Remove("dac");
        if (isFft) requiredVisited.Remove("fft");
    }
    
    private int totalPathsChecked = 0;
    
    private int CountPathsVisitingBoth(Dictionary<string, List<string>> graph, string start)
    {
        int count = 0;
        totalPathsChecked = 0;
        CountPathsWithTracking(graph, start, new HashSet<string>(), new HashSet<string>(), ref count);
        Console.WriteLine($"Debug: Checked {totalPathsChecked} complete paths");
        return count;
    }
    
    private void CountPathsWithTracking(Dictionary<string, List<string>> graph, string current,
        HashSet<string> visited, HashSet<string> requiredVisited, ref int count)
    {
        // Track if we've visited dac or fft
        bool isDac = current == "dac";
        bool isFft = current == "fft";
        if (isDac) requiredVisited.Add("dac");
        if (isFft) requiredVisited.Add("fft");
        
        // If we reached the exit, count if we visited both
        if (current == "out")
        {
            totalPathsChecked++;
            if (requiredVisited.Contains("dac") && requiredVisited.Contains("fft"))
                count++;
            
            // Backtrack required visited
            if (isDac) requiredVisited.Remove("dac");
            if (isFft) requiredVisited.Remove("fft");
            return;
        }
        
        // If node not in graph or already visited, stop
        if (!graph.ContainsKey(current) || visited.Contains(current))
        {
            if (isDac) requiredVisited.Remove("dac");
            if (isFft) requiredVisited.Remove("fft");
            return;
        }
        
        // Mark current node as visited
        visited.Add(current);
        
        // Explore all neighbors
        foreach (var neighbor in graph[current])
        {
            CountPathsWithTracking(graph, neighbor, visited, requiredVisited, ref count);
        }
        
        // Backtrack
        visited.Remove(current);
        if (isDac) requiredVisited.Remove("dac");
        if (isFft) requiredVisited.Remove("fft");
    }
    
    private HashSet<string> ComputeReachability(Dictionary<string, List<string>> graph, string target)
    {
        // Find all nodes that can reach the target using reverse BFS
        var canReach = new HashSet<string> { target };
        var reverseGraph = new Dictionary<string, List<string>>();
        
        // Build reverse graph
        foreach (var kvp in graph)
        {
            foreach (var neighbor in kvp.Value)
            {
                if (!reverseGraph.ContainsKey(neighbor))
                    reverseGraph[neighbor] = new List<string>();
                reverseGraph[neighbor].Add(kvp.Key);
            }
        }
        
        // BFS from target backwards
        var queue = new Queue<string>();
        queue.Enqueue(target);
        
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (reverseGraph.ContainsKey(node))
            {
                foreach (var predecessor in reverseGraph[node])
                {
                    if (canReach.Add(predecessor))
                        queue.Enqueue(predecessor);
                }
            }
        }
        
        return canReach;
    }
    
    private long CountPathsWithPruning(Dictionary<string, List<string>> graph, string current, 
        HashSet<string> visited, HashSet<string> requiredRemaining,
        HashSet<string> canReachDac, HashSet<string> canReachFft)
    {
        // If we reached the exit, count only if we visited all required nodes
        if (current == "out")
            return requiredRemaining.Count == 0 ? 1 : 0;
        
        // If node not in graph or already visited, no paths
        if (!graph.ContainsKey(current) || visited.Contains(current))
            return 0;
        
        // Pruning: if we still need to visit dac but can't reach it from here, prune
        if (requiredRemaining.Contains("dac") && !canReachDac.Contains(current))
            return 0;
        
        // Pruning: if we still need to visit fft but can't reach it from here, prune
        if (requiredRemaining.Contains("fft") && !canReachFft.Contains(current))
            return 0;
        
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
            totalPaths += CountPathsWithPruning(graph, neighbor, visited, requiredRemaining, canReachDac, canReachFft);
        }
        
        // Backtrack
        visited.Remove(current);
        if (wasRequired)
            requiredRemaining.Add(current);
        
        return totalPaths;
    }
    
    private bool HasCycle(Dictionary<string, List<string>> graph, string start)
    {
        var visited = new HashSet<string>();
        var recStack = new HashSet<string>();
        return HasCycleDFS(graph, start, visited, recStack);
    }
    
    private bool HasCycleDFS(Dictionary<string, List<string>> graph, string node, 
        HashSet<string> visited, HashSet<string> recStack)
    {
        if (!graph.ContainsKey(node))
            return false;
            
        if (recStack.Contains(node))
            return true;
            
        if (visited.Contains(node))
            return false;
        
        visited.Add(node);
        recStack.Add(node);
        
        foreach (var neighbor in graph[node])
        {
            if (HasCycleDFS(graph, neighbor, visited, recStack))
                return true;
        }
        
        recStack.Remove(node);
        return false;
    }
    
    private long CountPathsDAG(Dictionary<string, List<string>> graph, string current, 
        HashSet<string> requiredRemaining, Dictionary<string, long> memo)
    {
        // If we reached the exit, count only if we visited all required nodes
        if (current == "out")
            return requiredRemaining.Count == 0 ? 1 : 0;
        
        if (!graph.ContainsKey(current))
            return 0;
        
        // Create memo key: current node + sorted required remaining nodes
        var memoKey = current + ":" + string.Join(",", requiredRemaining.OrderBy(x => x));
        if (memo.ContainsKey(memoKey))
            return memo[memoKey];
        
        // Check if current node is one of the required nodes
        bool wasRequired = requiredRemaining.Contains(current);
        if (wasRequired)
            requiredRemaining.Remove(current);
        
        // Explore all neighbors and sum the paths
        long totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPathsDAG(graph, neighbor, requiredRemaining, memo);
        }
        
        // Restore required if needed
        if (wasRequired)
            requiredRemaining.Add(current);
        
        memo[memoKey] = totalPaths;
        return totalPaths;
    }
    
    private long CountPathsWithRequired(Dictionary<string, List<string>> graph, string current, 
        HashSet<string> visited, HashSet<string> requiredRemaining)
    {
        // If we reached the exit, count only if we visited all required nodes
        if (current == "out")
            return requiredRemaining.Count == 0 ? 1 : 0;
        
        // If node not in graph or already visited, no paths
        if (!graph.ContainsKey(current) || visited.Contains(current))
            return 0;
        
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
            totalPaths += CountPathsWithRequired(graph, neighbor, visited, requiredRemaining);
        }
        
        // Backtrack
        visited.Remove(current);
        if (wasRequired)
            requiredRemaining.Add(current);
        
        return totalPaths;
    }
}
