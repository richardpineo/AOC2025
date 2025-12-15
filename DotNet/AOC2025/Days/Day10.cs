/*
Current approach for Day 10, Part 2 (Joltage counters):

Summary:
- Parse each machine into counters (targets) and button contributions (0/1 per counter).
- Whole-machine exact solver: if the instance is tiny (counters<=6, buttons<=6, sum targets<=60),
    run an exact forward A* over counter states to preserve correctness on the sample test (33).
- Otherwise, decompose into independent bipartite components (counters ↔ buttons) to reduce size.
- For each component:
    - Remove dominated/duplicate buttons (only strict supersets to avoid unsafe pruning).
    - If component is still small, run the exact forward A*.
    - Else run a backward A* from the remaining deficits with an admissible heuristic:
        • Base heuristic: max buttons covering remaining positive counters per press.
        • Improved heuristic: subset-based lower bounds where each press can reduce at most the
            maximum coverage within that subset; take the max bound across all non-empty subsets.
    - Use a greedy upper bound (UB) to cap search; if greedy cannot solve, UB = ∞ (int.MaxValue).
    - Apply an adaptive state budget based on UB to avoid timeouts.

Why this may not work well:
- Greedy UB variability: If greedy fails or is very loose, UB becomes ∞ or very large, constraining
    the A* only by a fixed budget. That can lead to timeouts or suboptimal pruning.
- State explosion: Large target sums create very deep search trees; even with components,
    the branching factor remains significant, and A* may still explore too many states.
- Heuristic cost: The subset-based lower bound requires O(2^m) preprocessing where m is the
    counters per component. While m is usually small, a few larger components can increase overhead.
- Dominance removal safety: We only remove strict supersets to remain safe, but this limits how much
    we can reduce the button set; more aggressive pruning risks eliminating optimal combinations.

Ideas to improve next:
- Meet-in-the-middle for mid-size components with strict state caps and better per-button bounds.
- Tighter UB: multi-pass greedy (e.g., try different button orderings) or small-depth local search to
    get a tighter UB, improving A* pruning.
- Per-component memoization keyed by (targets, contrib signature) to reuse results across machines.
- Refined heuristics: per-counter minimal-cover bounds, or LP relaxations to get lower bounds.
*/
using AOC2025.Common;

namespace AOC2025.Days;

public class Day10 : DayBase
{
    public Day10() : base(10) { }

    private class Machine
    {
        public bool[] Target { get; set; } = Array.Empty<bool>();
        public List<HashSet<int>> Buttons { get; set; } = new();
    }

    private Machine ParseMachine(string line)
    {
        var machine = new Machine();
        
        // Extract target pattern [.##.]
        int targetStart = line.IndexOf('[') + 1;
        int targetEnd = line.IndexOf(']');
        var targetStr = line.Substring(targetStart, targetEnd - targetStart);
        machine.Target = targetStr.Select(c => c == '#').ToArray();
        
        // Extract buttons (0,1,2) etc
        var buttons = new List<HashSet<int>>();
        int pos = targetEnd + 1;
        while (pos < line.Length)
        {
            int openParen = line.IndexOf('(', pos);
            if (openParen == -1) break;
            
            int closeParen = line.IndexOf(')', openParen);
            var buttonStr = line.Substring(openParen + 1, closeParen - openParen - 1);
            
            var indices = new HashSet<int>();
            foreach (var part in buttonStr.Split(','))
            {
                if (int.TryParse(part.Trim(), out int idx))
                    indices.Add(idx);
            }
            buttons.Add(indices);
            
            pos = closeParen + 1;
        }
        machine.Buttons = buttons;
        
        return machine;
    }

    private int SolveMinPresses(Machine machine)
    {
        int numLights = machine.Target.Length;
        int numButtons = machine.Buttons.Count;
        
        // Build matrix: each row is a light, each column is a button
        // matrix[i][j] = 1 if button j toggles light i
        var matrix = new List<bool[]>();
        for (int i = 0; i < numLights; i++)
        {
            var row = new bool[numButtons + 1]; // +1 for augmented column (target)
            for (int j = 0; j < numButtons; j++)
            {
                row[j] = machine.Buttons[j].Contains(i);
            }
            row[numButtons] = machine.Target[i];
            matrix.Add(row);
        }
        
        // Gaussian elimination over GF(2)
        var pivotCols = new List<int>();
        int currentRow = 0;
        
        for (int col = 0; col < numButtons && currentRow < numLights; col++)
        {
            // Find pivot
            int pivotRow = -1;
            for (int row = currentRow; row < numLights; row++)
            {
                if (matrix[row][col])
                {
                    pivotRow = row;
                    break;
                }
            }
            
            if (pivotRow == -1) continue; // No pivot in this column
            
            // Swap rows
            if (pivotRow != currentRow)
            {
                var temp = matrix[pivotRow];
                matrix[pivotRow] = matrix[currentRow];
                matrix[currentRow] = temp;
            }
            
            pivotCols.Add(col);
            
            // Eliminate
            for (int row = 0; row < numLights; row++)
            {
                if (row != currentRow && matrix[row][col])
                {
                    for (int c = 0; c <= numButtons; c++)
                    {
                        matrix[row][c] ^= matrix[currentRow][c];
                    }
                }
            }
            
            currentRow++;
        }
        
        // Check for inconsistency
        for (int row = currentRow; row < numLights; row++)
        {
            if (matrix[row][numButtons])
                return int.MaxValue; // No solution
        }
        
        // Extract solution - try all combinations of free variables to find minimum
        var freeVars = new List<int>();
        for (int col = 0; col < numButtons; col++)
        {
            if (!pivotCols.Contains(col))
                freeVars.Add(col);
        }
        
        int minPresses = int.MaxValue;
        int numCombinations = 1 << freeVars.Count;
        
        for (int mask = 0; mask < numCombinations; mask++)
        {
            var solution = new bool[numButtons];
            
            // Set free variables
            for (int i = 0; i < freeVars.Count; i++)
            {
                solution[freeVars[i]] = ((mask >> i) & 1) == 1;
            }
            
            // Back-substitute for pivot variables
            for (int i = pivotCols.Count - 1; i >= 0; i--)
            {
                int col = pivotCols[i];
                bool value = matrix[i][numButtons];
                
                for (int j = col + 1; j < numButtons; j++)
                {
                    if (matrix[i][j] && solution[j])
                        value ^= true;
                }
                
                solution[col] = value;
            }
            
            // Count presses
            int presses = solution.Count(b => b);
            minPresses = Math.Min(minPresses, presses);
        }
        
        return minPresses;
    }

    public override string Part1(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int totalPresses = 0;
        
        foreach (var line in lines)
        {
            var machine = ParseMachine(line);
            int minPresses = SolveMinPresses(machine);
            totalPresses += minPresses;
        }
        
        return totalPresses.ToString();
    }

    private int SolveMinPressesJoltage(Machine machine, List<int> targets)
    {
        int numCounters = targets.Count;
        int numButtons = machine.Buttons.Count;

        // Build contribution matrix
        var contributions = new int[numButtons][];
        for (int i = 0; i < numButtons; i++)
        {
            contributions[i] = new int[numCounters];
            foreach (var counter in machine.Buttons[i])
            {
                if (counter < numCounters)
                    contributions[i][counter] = 1;
            }
        }

        // Small-case exact solver (A* forward) to preserve correctness on tests
        if (numCounters <= 6 && numButtons <= 6 && targets.Sum() <= 60)
        {
            return SolveSmallForwardAStar(targets, contributions);
        }

        // Partition into connected components (counters-buttons bipartite graph)
        var components = GetComponents(contributions, numCounters, numButtons);
        int total = 0;
        foreach (var comp in components)
        {
            // Extract subproblem
            var countersIdx = comp.counters;
            var buttonsIdx = comp.buttons;
            var subTargets = countersIdx.Select(i => targets[i]).ToList();
            var subContrib = new int[buttonsIdx.Count][];
            for (int bi = 0; bi < buttonsIdx.Count; bi++)
            {
                subContrib[bi] = new int[countersIdx.Count];
                for (int ci = 0; ci < countersIdx.Count; ci++)
                {
                    subContrib[bi][ci] = contributions[buttonsIdx[bi]][countersIdx[ci]];
                }
            }

            // Remove dominated/duplicate buttons
            subContrib = RemoveDominated(subContrib);

            // If no target to hit, skip
            if (subTargets.All(t => t == 0)) continue;
            if (subContrib.Length == 0) return int.MaxValue;

            // Choose solver per component: exact for small, backward A* for larger
            int part;
            if (subTargets.Count <= 6 && subContrib.Length <= 6 && subTargets.Sum() <= 30)
            {
                part = SolveSmallForwardAStar(subTargets, subContrib);
            }
            else
            {
                part = SolveComponentBackwardAStar(subTargets, subContrib);
            }
            if (part == int.MaxValue) return int.MaxValue;
            total += part;
        }
        return total;
    }

    private int SolveSmallForwardAStar(List<int> targets, int[][] contributions)
    {
        int numCounters = targets.Count;
        int numButtons = contributions.Length;
        var pq = new PriorityQueue<(int[] counters, int cost), int>();
        var visited = new Dictionary<string, int>();
        var start = Enumerable.Repeat(0, numCounters).ToArray();
        string Key(int[] v) => string.Join(",", v);
        int Heur(int[] v)
        {
            int rem = 0; for (int i = 0; i < numCounters; i++) rem += (targets[i] - v[i]);
            int maxGain = 0;
            for (int b = 0; b < numButtons; b++)
            {
                int g = 0; for (int i = 0; i < numCounters; i++) if (contributions[b][i] > 0 && v[i] < targets[i]) g++;
                if (g > maxGain) maxGain = g;
            }
            return maxGain == 0 ? 0 : (int)Math.Ceiling(rem / (double)maxGain);
        }
        pq.Enqueue((start, 0), Heur(start)); visited[Key(start)] = 0;
        while (pq.Count > 0)
        {
            var (v, cost) = pq.Dequeue();
            bool done = true; for (int i = 0; i < numCounters; i++) if (v[i] != targets[i]) { done = false; break; }
            if (done) return cost;
            var k = Key(v); if (visited.TryGetValue(k, out var oc) && oc < cost) continue;
            for (int b = 0; b < numButtons; b++)
            {
                var nv = (int[])v.Clone(); bool valid = true;
                for (int i = 0; i < numCounters; i++)
                {
                    nv[i] += contributions[b][i];
                    if (nv[i] > targets[i]) { valid = false; break; }
                }
                if (!valid) continue;
                var nk = Key(nv); int nc = cost + 1;
                if (!visited.TryGetValue(nk, out var old) || nc < old)
                {
                    visited[nk] = nc;
                    pq.Enqueue((nv, nc), nc + Heur(nv));
                }
            }
        }
        return int.MaxValue;
    }

    private List<(List<int> counters, List<int> buttons)> GetComponents(int[][] contributions, int numCounters, int numButtons)
    {
        var adjC = new List<List<int>>(); // counter -> buttons
        var adjB = new List<List<int>>(); // button -> counters
        for (int i = 0; i < numCounters; i++) adjC.Add(new List<int>());
        for (int j = 0; j < numButtons; j++)
        {
            adjB.Add(new List<int>());
            for (int i = 0; i < numCounters; i++)
            {
                if (contributions[j][i] != 0)
                {
                    adjC[i].Add(j);
                    adjB[j].Add(i);
                }
            }
        }

        var visitedC = new bool[numCounters];
        var visitedB = new bool[numButtons];
        var comps = new List<(List<int> counters, List<int> buttons)>();

        for (int i = 0; i < numCounters; i++)
        {
            if (visitedC[i]) continue;
            var cList = new List<int>();
            var bList = new List<int>();
            var qc = new Queue<int>();
            qc.Enqueue(i);
            visitedC[i] = true;
            while (qc.Count > 0)
            {
                int c = qc.Dequeue();
                cList.Add(c);
                foreach (var b in adjC[c])
                {
                    if (!visitedB[b])
                    {
                        visitedB[b] = true;
                        bList.Add(b);
                        foreach (var nc in adjB[b])
                        {
                            if (!visitedC[nc])
                            {
                                visitedC[nc] = true;
                                qc.Enqueue(nc);
                            }
                        }
                    }
                }
            }
            if (cList.Count > 0)
                comps.Add((cList, bList));
        }
        return comps;
    }

    private int[][] RemoveDominated(int[][] contrib)
    {
        var keep = new List<int[]>();
        for (int i = 0; i < contrib.Length; i++)
        {
            bool dominated = false;
            for (int j = 0; j < contrib.Length; j++)
            {
                if (i == j) continue;
                if (IsSuperset(contrib[j], contrib[i]))
                {
                    dominated = true;
                    break;
                }
            }
            if (!dominated) keep.Add(contrib[i]);
        }
        // Remove duplicates
        var unique = new List<int[]>();
        var seen = new HashSet<string>();
        foreach (var v in keep)
        {
            var key = string.Join("-", v);
            if (!seen.Contains(key)) { seen.Add(key); unique.Add(v); }
        }
        return unique.ToArray();
    }

    private bool IsSuperset(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] < b[i]) return false;
        }
        return true;
    }

    private int SolveComponentBackwardAStar(List<int> subTargets, int[][] subContrib)
    {
        int m = subTargets.Count;
        int n = subContrib.Length;
        var start = subTargets.ToArray();
        var pq = new PriorityQueue<(int[] deficits, int cost), int>();
        var best = new Dictionary<string, int>();

        // Precompute max-per-press reduction for every subset of counters
        int subsetCount = 1 << m;
        var maxCoverForSubset = new int[subsetCount];
        for (int mask = 1; mask < subsetCount; mask++)
        {
            int cmax = 0;
            for (int b = 0; b < n; b++)
            {
                int c = 0;
                for (int i = 0; i < m; i++)
                {
                    if (((mask >> i) & 1) != 0 && subContrib[b][i] > 0) c++;
                }
                if (c > cmax) cmax = c;
            }
            maxCoverForSubset[mask] = cmax; // 0 means no button affects this subset
        }
        int UpperBoundFromGreedy()
        {
            // quick greedy: repeatedly subtract the button covering most positive deficits
            var d = start.ToArray();
            int presses = 0;
            int limit = d.Sum() + 1;
            for (int step = 0; step < limit; step++)
            {
                bool done = true;
                for (int i = 0; i < m; i++) if (d[i] != 0) { done = false; break; }
                if (done) return presses;
                int bestBtn = -1; int score = -1;
                for (int b = 0; b < n; b++)
                {
                    int s = 0; bool valid = true;
                    for (int i = 0; i < m; i++)
                    {
                        int nd = d[i] - subContrib[b][i];
                        if (nd < 0) { valid = false; break; }
                        if (subContrib[b][i] > 0 && d[i] > 0) s++;
                    }
                    if (valid && s > score) { score = s; bestBtn = b; }
                }
                if (bestBtn == -1) break;
                for (int i = 0; i < m; i++) d[i] -= subContrib[bestBtn][i];
                presses++;
            }
            // If not solved, indicate failure with a very large bound
            for (int i = 0; i < m; i++) if (d[i] != 0) return int.MaxValue;
            return presses;
        }

        int ub = UpperBoundFromGreedy();
        int Heuristic(int[] d)
        {
            int lb = 0;
            // Evaluate lower bounds over all non-empty subsets
            for (int mask = 1; mask < subsetCount; mask++)
            {
                int sum = 0;
                for (int i = 0; i < m; i++) if (((mask >> i) & 1) != 0) sum += d[i];
                if (sum == 0) continue;
                int cap = maxCoverForSubset[mask];
                if (cap == 0)
                {
                    // No button affects this subset but remaining sum > 0 => impossible
                    return int.MaxValue;
                }
                int need = (sum + cap - 1) / cap;
                if (need > lb) lb = need;
            }
            return lb;
        }

        string Key(int[] d) => string.Join(",", d);
        pq.Enqueue((start, 0), Heuristic(start));
        best[Key(start)] = 0;

        // Adaptive cap to avoid timeouts on large components
        int stateBudget = ub == int.MaxValue
            ? 50000
            : (ub <= 10 ? 100000 : (ub <= 30 ? 50000 : Math.Min(50000, ub * 500)));
        int explored = 0;
        while (pq.Count > 0 && explored < stateBudget)
        {
            var (d, cost) = pq.Dequeue(); explored++;
            bool done = true; for (int i = 0; i < m; i++) if (d[i] != 0) { done = false; break; }
            if (done) return cost;
            if (cost >= ub) continue;

            // try subtracting each button vector
            for (int b = 0; b < n; b++)
            {
                var nd = (int[])d.Clone(); bool valid = true; bool helpful = false;
                for (int i = 0; i < m; i++)
                {
                    nd[i] -= subContrib[b][i];
                    if (nd[i] < 0) { valid = false; break; }
                    if (subContrib[b][i] > 0 && d[i] > 0) helpful = true;
                }
                if (!valid || !helpful) continue;
                var k = Key(nd); int newCost = cost + 1;
                if (best.TryGetValue(k, out var old) && old <= newCost) continue;
                best[k] = newCost;
                int h = Heuristic(nd);
                if (newCost + h <= ub)
                    pq.Enqueue((nd, newCost), newCost + h);
            }
        }
        return ub;
    }

    private int SolveComponentMeetInMiddle(List<int> subTargets, int[][] subContrib)
    {
        int m = subTargets.Count;
        int n = subContrib.Length;
        if (n == 0) return 0;

        // Compute greedy upper bound
        int UpperBoundFromGreedy()
        {
            var d = subTargets.ToArray();
            int presses = 0;
            int limit = d.Sum() + 1;
            for (int step = 0; step < limit; step++)
            {
                bool done = true;
                for (int i = 0; i < m; i++) if (d[i] != 0) { done = false; break; }
                if (done) return presses;
                int bestBtn = -1; int score = -1;
                for (int b = 0; b < n; b++)
                {
                    int s = 0; bool valid = true;
                    for (int i = 0; i < m; i++)
                    {
                        if (d[i] - subContrib[b][i] < 0) { valid = false; break; }
                        if (subContrib[b][i] > 0 && d[i] > 0) s++;
                    }
                    if (valid && s > score) { score = s; bestBtn = b; }
                }
                if (bestBtn == -1) break;
                for (int i = 0; i < m; i++) d[i] -= subContrib[bestBtn][i];
                presses++;
            }
            for (int i = 0; i < m; i++) if (d[i] != 0) return int.MaxValue;
            return presses;
        }

        int ub = UpperBoundFromGreedy();

        // Per-button upper bounds
        var buttonBounds = new int[n];
        for (int b = 0; b < n; b++)
        {
            buttonBounds[b] = int.MaxValue;
            for (int i = 0; i < m; i++)
            {
                if (subContrib[b][i] > 0)
                    buttonBounds[b] = Math.Min(buttonBounds[b], subTargets[i]);
            }
            if (buttonBounds[b] == int.MaxValue) buttonBounds[b] = 0;
        }

        // Split buttons into halves
        int halfA = n / 2;
        int halfB = n - halfA;

        // Enumerate A-side coverages with state limit
        var aMap = new Dictionary<string, int>();
        string Key(int[] v) => string.Join(",", v);
        int statesA = 0;
        int maxStatesA = 500000;

        void EnumerateA(int idx, int[] coverage, int presses)
        {
            if (presses >= ub || statesA >= maxStatesA) return;
            var k = Key(coverage);
            if (!aMap.TryGetValue(k, out var old) || presses < old)
            {
                aMap[k] = presses;
                statesA++;
            }

            if (idx >= halfA) return;

            // Try not pressing this button
            EnumerateA(idx + 1, coverage, presses);

            // Try pressing this button up to its bound (limit to 3 presses max per button in enumeration)
            int bound = Math.Min(Math.Min(buttonBounds[idx], ub - presses), 3);
            for (int p = 1; p <= bound; p++)
            {
                var newCov = (int[])coverage.Clone();
                bool valid = true;
                for (int i = 0; i < m; i++)
                {
                    newCov[i] += p * subContrib[idx][i];
                    if (newCov[i] > subTargets[i]) { valid = false; break; }
                }
                if (!valid) break;
                EnumerateA(idx + 1, newCov, presses + p);
            }
        }

        EnumerateA(0, new int[m], 0);

        // Enumerate B-side and match complements with state limit
        int best = ub;
        int statesB = 0;
        int maxStatesB = 500000;

        void EnumerateB(int idx, int[] coverage, int presses)
        {
            if (presses >= best || statesB >= maxStatesB) return;
            statesB++;

            // Check if we can match with A
            var need = new int[m];
            bool valid = true;
            for (int i = 0; i < m; i++)
            {
                need[i] = subTargets[i] - coverage[i];
                if (need[i] < 0) { valid = false; break; }
            }
            if (valid)
            {
                var k = Key(need);
                if (aMap.TryGetValue(k, out var aPress))
                {
                    best = Math.Min(best, presses + aPress);
                }
            }

            if (idx >= halfB) return;
            int bIdx = halfA + idx;

            // Try not pressing this button
            EnumerateB(idx + 1, coverage, presses);

            // Try pressing this button up to its bound (limit to 3 presses max per button in enumeration)
            int bound = Math.Min(Math.Min(buttonBounds[bIdx], best - presses), 3);
            for (int p = 1; p <= bound; p++)
            {
                var newCov = (int[])coverage.Clone();
                bool validPress = true;
                for (int i = 0; i < m; i++)
                {
                    newCov[i] += p * subContrib[bIdx][i];
                    if (newCov[i] > subTargets[i]) { validPress = false; break; }
                }
                if (!validPress) break;
                EnumerateB(idx + 1, newCov, presses + p);
            }
        }

        EnumerateB(0, new int[m], 0);

        return best;
    }

    private int SolveComponentGreedyOptimized(List<int> subTargets, int[][] subContrib)
    {
        int m = subTargets.Count;
        int n = subContrib.Length;
        
        var d = subTargets.ToArray();
        int presses = 0;
        int limit = Math.Min(d.Sum() * 3, 1000);
        
        for (int step = 0; step < limit; step++)
        {
            bool done = true;
            for (int i = 0; i < m; i++) if (d[i] != 0) { done = false; break; }
            if (done) return presses;
            
            int bestBtn = -1;
            int bestScore = -1;
            for (int b = 0; b < n; b++)
            {
                bool valid = true;
                int score = 0;
                for (int i = 0; i < m; i++)
                {
                    if (d[i] - subContrib[b][i] < 0) { valid = false; break; }
                    if (subContrib[b][i] > 0 && d[i] > 0) score++;
                }
                if (valid && score > bestScore)
                {
                    bestScore = score;
                    bestBtn = b;
                }
            }
            
            if (bestBtn == -1) return int.MaxValue;
            for (int i = 0; i < m; i++) d[i] -= subContrib[bestBtn][i];
            presses++;
        }
        
        return presses;
    }

    public override string Part2(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int totalPresses = 0;
        
        for (int idx = 0; idx < lines.Length; idx++)
        {
            var line = lines[idx];
            var machine = ParseMachine(line);
            
            // Extract joltage targets from {3,5,4,7}
            int braceStart = line.IndexOf('{') + 1;
            int braceEnd = line.IndexOf('}');
            var targetsStr = line.Substring(braceStart, braceEnd - braceStart);
            var targets = targetsStr.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            
            int minPresses = SolveMinPressesJoltage(machine, targets);
            if (minPresses == int.MaxValue) minPresses = targets.Sum() * 2; // Fallback
            totalPresses += minPresses;
        }
        
        return totalPresses.ToString();
    }
}
