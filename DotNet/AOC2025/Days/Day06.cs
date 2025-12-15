// Day 6: Column-wise Calculator
// Part 1: Parse space-separated columns, apply operator from bottom row to each column
// Part 2: Parse fixed-width columns from right-to-left, group by blank column separators
//
// Problem: Multi-line input represents vertical calculations. Numbers are in columns,
// operator in bottom row. Part 1 uses space-separated parsing, Part 2 uses fixed-width
// character positions and reads right-to-left.

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
        var lines = input.TrimEnd('\n').Split('\n');
        if (lines.Length == 0) return "0";

        // Get max length for processing
        int maxLen = lines.Max(l => l.Length);
        
        // Pad all lines to same length
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length < maxLen)
                lines[i] = lines[i].PadRight(maxLen);
        }
        
        int rows = lines.Length;
        int cols = maxLen;
        
        // Process columns right-to-left
        // Each column contains digits of one number (top to bottom)
        // When we hit an operator at bottom, it's the last number of a problem
        // Blank columns separate problems
        long grandTotal = 0;
        
        var currentProblem = new List<long>();
        char currentOp = ' ';
        
        // Read from right to left
        for (int col = cols - 1; col >= 0; col--)
        {
            bool isBlank = IsBlankColumn(lines, col, rows);
            
            if (isBlank)
            {
                // If we have a problem in progress, calculate and save it
                if (currentProblem.Count > 0 && currentOp != ' ')
                {
                    long result = currentProblem[0];
                    for (int i = 1; i < currentProblem.Count; i++)
                    {
                        if (currentOp == '+')
                            result += currentProblem[i];
                        else if (currentOp == '*')
                            result *= currentProblem[i];
                    }
                    grandTotal += result;
                    currentProblem.Clear();
                    currentOp = ' ';
                }
                continue;
            }
            
            // Non-blank column - read digits top to bottom to form a number
            var digits = new List<char>();
            for (int row = 0; row < rows - 1; row++)
            {
                char c = lines[row][col];
                if (c >= '0' && c <= '9')
                    digits.Add(c);
            }
            
            // Check if this column has an operator at the bottom
            char opChar = lines[rows - 1][col];
            bool hasOperator = (opChar == '+' || opChar == '*');
            
            // If we have digits, form a number
            if (digits.Count > 0)
            {
                string numStr = new string(digits.ToArray());
                if (long.TryParse(numStr, out long num))
                {
                    currentProblem.Add(num);
                }
            }
            
            // If this column has an operator, it marks the end of this problem
            if (hasOperator)
            {
                currentOp = opChar;
                
                // Calculate the problem result
                if (currentProblem.Count > 0)
                {
                    long result = currentProblem[0];
                    for (int i = 1; i < currentProblem.Count; i++)
                    {
                        if (currentOp == '+')
                            result += currentProblem[i];
                        else if (currentOp == '*')
                            result *= currentProblem[i];
                    }
                    grandTotal += result;
                    currentProblem.Clear();
                    currentOp = ' ';
                }
            }
        }
        
        // Don't forget the last problem if it didn't have an operator column
        // (shouldn't happen but just in case)
        if (currentProblem.Count > 0 && currentOp != ' ')
        {
            long result = currentProblem[0];
            for (int i = 1; i < currentProblem.Count; i++)
            {
                if (currentOp == '+')
                    result += currentProblem[i];
                else if (currentOp == '*')
                    result *= currentProblem[i];
            }
            grandTotal += result;
        }

        return grandTotal.ToString();
    }
}
