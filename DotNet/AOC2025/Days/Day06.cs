using AOC2025.Common;

namespace AOC2025.Days;

public class Day06 : DayBase
{
    public Day06() : base(6) { }

    public override string Part1(string input)
    {
        var lines = input.TrimEnd('\n').Split('\n');
        if (lines.Length == 0) return "0";

        int rows = lines.Length;
        int cols = lines.Max(l => l.Length);

        // Pad all lines to same length
        for (int i = 0; i < rows; i++)
        {
            if (lines[i].Length < cols)
                lines[i] = lines[i].PadRight(cols);
        }

        // Parse problems: each problem is a group of non-blank columns
        // Within each problem column, read vertically to get one number
        var problems = new List<(List<long> numbers, char op)>();
        int col = 0;
        
        while (col < cols)
        {
            // Skip blank columns
            while (col < cols && IsBlankColumn(lines, col, rows))
                col++;
            
            if (col >= cols) break;

            // Found start of a problem - read columns until blank or end
            int problemStart = col;
            while (col < cols && !IsBlankColumn(lines, col, rows))
                col++;
            
            int problemEnd = col;
            
            // Now parse this problem (columns problemStart to problemEnd-1)
            // Read each ROW horizontally within this problem to get numbers
            var numbers = new List<long>();
            char op = ' ';
            
            // Read each row (except last) to get numbers
            for (int r = 0; r < rows - 1; r++)
            {
                var numStr = lines[r].Substring(problemStart, problemEnd - problemStart).Trim();
                if (!string.IsNullOrEmpty(numStr))
                {
                    if (long.TryParse(numStr, out long num))
                    {
                        numbers.Add(num);
                    }
                }
            }
            
            // Get operator from last row (any non-space character)
            for (int c = problemStart; c < problemEnd; c++)
            {
                char opChar = lines[rows - 1][c];
                if (opChar == '+' || opChar == '*')
                {
                    op = opChar;
                    break;
                }
            }
            
            if (numbers.Count > 0 && op != ' ')
            {
                problems.Add((numbers, op));
            }
        }

        // Calculate grand total
        long grandTotal = 0;
        foreach (var (numbers, op) in problems)
        {
            if (numbers.Count == 0) continue;
            
            long result = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                if (op == '+')
                    result += numbers[i];
                else if (op == '*')
                    result *= numbers[i];
            }
            grandTotal += result;
        }

        return grandTotal.ToString();
    }
    
    private static bool IsBlankColumn(string[] lines, int col, int rows)
    {
        for (int row = 0; row < rows; row++)
        {
            if (lines[row][col] != ' ')
                return false;
        }
        return true;
    }

    public override string Part2(string input)
    {
        // TODO: Implement Day 6 Part 2
        return "0";
    }
}
