using AOC2025.Common;

namespace AOC2025.Days;

public class Day06 : DayBase
{
    public Day06() : base(6) { }

    public override string Part1(string input)
    {
        var lines = input.TrimEnd('\n').Split('\n');
        if (lines.Length == 0) return "0";

        // Split each line by spaces to get columns
        var rows = new List<string[]>();
        foreach (var line in lines)
        {
            rows.Add(line.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        if (rows.Count == 0) return "0";
        
        // Get number of columns (from the row with most columns)
        int numCols = rows.Max(r => r.Length);
        
        // Process each column
        long grandTotal = 0;
        for (int col = 0; col < numCols; col++)
        {
            var numbers = new List<long>();
            char op = ' ';
            
            // Read all rows for this column
            for (int row = 0; row < rows.Count - 1; row++)
            {
                if (col < rows[row].Length)
                {
                    if (long.TryParse(rows[row][col], out long num))
                    {
                        numbers.Add(num);
                    }
                }
            }
            
            // Get operator from last row
            if (col < rows[^1].Length)
            {
                string opStr = rows[^1][col];
                if (opStr.Length > 0)
                    op = opStr[0];
            }
            
            // Calculate factor for this column
            if (numbers.Count > 0 && (op == '+' || op == '*'))
            {
                long factor = numbers[0];
                for (int i = 1; i < numbers.Count; i++)
                {
                    if (op == '+')
                        factor += numbers[i];
                    else if (op == '*')
                        factor *= numbers[i];
                }
                
                grandTotal += factor;
            }
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
