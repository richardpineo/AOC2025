namespace AOC2025.Common;

public abstract class DayBase
{
    public int DayNumber { get; }
    public string InputPath { get; }
    public string TestPath { get; }

    protected DayBase(int dayNumber)
    {
        DayNumber = dayNumber;
        InputPath = FileHelper.GetInputPath(dayNumber);
        TestPath = FileHelper.GetTestPath(dayNumber);
    }

    public abstract string Part1(string input);
    public abstract string Part2(string input);

    public void RunBoth(bool useTest = false)
    {
        var path = useTest ? TestPath : InputPath;
        if (!File.Exists(path))
        {
            Console.WriteLine($"  (File not found: {Path.GetFileName(path)})");
            return;
        }

        var input = FileHelper.ReadFile(path);

        var part1Result = Part1(input);
        var part2Result = Part2(input);

        Console.WriteLine($"  Part 1: {part1Result}");
        Console.WriteLine($"  Part 2: {part2Result}");
    }
}
