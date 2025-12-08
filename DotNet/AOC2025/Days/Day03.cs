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
        // TODO: Implement Part 2
        return "0";
    }
}
