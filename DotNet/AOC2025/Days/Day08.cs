using AOC2025.Common;

namespace AOC2025.Days;

public class Day08 : DayBase
{
    public Day08() : base(8) { }

    private class UnionFind
    {
        private int[] parent;
        private int[] size;

        public UnionFind(int n)
        {
            parent = new int[n];
            size = new int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
        }

        public int Find(int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent[x]);
            return parent[x];
        }

        public void Union(int x, int y)
        {
            int px = Find(x);
            int py = Find(y);
            if (px == py) return;

            // Union by size
            if (size[px] < size[py])
                (px, py) = (py, px);

            parent[py] = px;
            size[px] += size[py];
        }

        public int GetSize(int x)
        {
            return size[Find(x)];
        }
    }

    public override string Part1(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // Parse coordinates
        var points = new List<(double x, double y, double z)>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            points.Add((double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2])));
        }

        // Calculate all pairwise distances
        var distances = new List<(double dist, int i, int j)>();
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                double dx = points[i].x - points[j].x;
                double dy = points[i].y - points[j].y;
                double dz = points[i].z - points[j].z;
                double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                distances.Add((dist, i, j));
            }
        }

        // Sort by distance
        distances.Sort((a, b) => a.dist.CompareTo(b.dist));

        // Union-Find: connect the appropriate number of pairs
        // For small inputs (test), use 10. For large inputs, use 1000.
        int targetConnections = points.Count <= 20 ? 10 : Math.Min(1000, distances.Count);
        
        var uf = new UnionFind(points.Count);
        
        // Process the N closest pairs (even if already connected)
        for (int i = 0; i < Math.Min(targetConnections, distances.Count); i++)
        {
            var (dist, a, b) = distances[i];
            if (uf.Find(a) != uf.Find(b))
            {
                uf.Union(a, b);
            }
        }

        // Get sizes of all circuits (only count root nodes)
        var circuitSizes = new List<int>();
        for (int i = 0; i < points.Count; i++)
        {
            int root = uf.Find(i);
            if (root == i)  // Only add size if this is a root
            {
                circuitSizes.Add(uf.GetSize(i));
            }
        }

        // Sort and multiply the three largest
        var sorted = circuitSizes.OrderByDescending(x => x).ToList();
        
        if (sorted.Count < 3)
            return "0"; // Not enough circuits
        
        long result = (long)sorted[0] * sorted[1] * sorted[2];
        return result.ToString();
    }

    public override string Part2(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // Parse coordinates
        var points = new List<(double x, double y, double z)>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            points.Add((double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2])));
        }

        // Calculate all pairwise distances
        var distances = new List<(double dist, int i, int j)>();
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                double dx = points[i].x - points[j].x;
                double dy = points[i].y - points[j].y;
                double dz = points[i].z - points[j].z;
                double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                distances.Add((dist, i, j));
            }
        }

        // Sort by distance
        distances.Sort((a, b) => a.dist.CompareTo(b.dist));

        // Union-Find: connect pairs until all are in one circuit
        var uf = new UnionFind(points.Count);
        int lastI = -1, lastJ = -1;
        
        foreach (var (dist, i, j) in distances)
        {
            if (uf.Find(i) != uf.Find(j))
            {
                uf.Union(i, j);
                lastI = i;
                lastJ = j;
                
                // Check if all nodes are now in one circuit
                if (uf.GetSize(i) == points.Count)
                    break;
            }
        }

        // Multiply X coordinates of the last connection
        long result = (long)points[lastI].x * (long)points[lastJ].x;
        return result.ToString();
    }
}
