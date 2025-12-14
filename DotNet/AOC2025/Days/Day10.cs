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

    public override string Part2(string input)
    {
        // TODO: Implement Part 2
        return "0";
    }
}
