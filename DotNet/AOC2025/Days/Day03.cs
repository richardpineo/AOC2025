// Day 3: Battery Bank Joltage
// Part 1: Find largest 2-digit joltage from each battery bank (pick 2 batteries)
// Part 2: Find largest 12-digit joltage using greedy monotonic stack algorithm
//
// Problem: Each line is a string of digit characters representing available batteries.
// Select k batteries to maximize the resulting k-digit number formed by their digits
// in the order they appear in the line.

using AOC2025.Common;

namespace AOC2025.Days;

public class Day03 : DayBase
{
    public Day03() : base(3) { }

    public override string Part1(string input)
    {
        var lines = input.Trim().Split('\n');
        long totalJoltage = 0;

        foreach (var line in lines)
        {
            // Find the largest 2-digit value by checking all pairs
            int maxJoltage = 0;
            
            for (int i = 0; i < line.Length - 1; i++)
            {
                for (int j = i + 1; j < line.Length; j++)
                {
                    // Form a 2-digit number from positions i and j
                    int digit1 = line[i] - '0';
                    int digit2 = line[j] - '0';
                    int joltage = digit1 * 10 + digit2;
                    
                    maxJoltage = Math.Max(maxJoltage, joltage);
                }
            }
            
            totalJoltage += maxJoltage;
        }

        return totalJoltage.ToString();
    }

    public override string Part2(string input)
    {
        var lines = input.Trim().Split('\n');
        long totalJoltage = 0;

        const int k = 12; // select exactly 12 batteries per bank for Part 2
        foreach (var line in lines)
        {
            long maxJoltage = FindLargestKDigit(line, k);
            totalJoltage += maxJoltage;
        }

        return totalJoltage.ToString();
    }

    private static long FindLargestKDigit(string line, int k)
    {
        if (line.Length < k)
            return 0;

        int n = line.Length;
        var stack = new List<char>(k);

        for (int i = 0; i < n; i++)
        {
            char c = line[i];
            int remaining = n - i; // including current

            // While we can pop and still fill k digits later, and the last in stack is smaller than current, pop it
            while (stack.Count > 0 && stack[stack.Count - 1] < c && (stack.Count - 1) + remaining >= k)
            {
                stack.RemoveAt(stack.Count - 1);
            }

            // If we still have room, push current digit
            if (stack.Count < k)
            {
                stack.Add(c);
            }
            // otherwise skip this digit
        }

        // Build the resulting k-digit number from the first k digits in the stack
        long result = 0;
        for (int i = 0; i < k; i++)
        {
            result = result * 10 + (stack[i] - '0');
        }

        return result;
    }
}
