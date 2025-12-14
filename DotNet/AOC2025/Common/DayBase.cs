namespace AOC2025.Common;

using System.Diagnostics;

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
        
        // Part 1 with timeout
        var sw = Stopwatch.StartNew();
        try
        {
            var task = System.Threading.Tasks.Task.Run(() => Part1(input));
            if (task.Wait(TimeSpan.FromSeconds(10)))
            {
                sw.Stop();
                var part1Elapsed = sw.Elapsed;
                Console.WriteLine($"  Part 1: {task.Result} ({part1Elapsed.TotalMilliseconds:F2}ms)");
            }
            else
            {
                sw.Stop();
                Console.WriteLine($"  Part 1: TIMEOUT after 10 seconds");
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            Console.WriteLine($"  Part 1: ERROR - {ex.GetBaseException().Message}");
        }
        
        // Part 2 with timeout
        sw.Restart();
        try
        {
            var task = System.Threading.Tasks.Task.Run(() => Part2(input));
            if (task.Wait(TimeSpan.FromSeconds(10)))
            {
                sw.Stop();
                var part2Elapsed = sw.Elapsed;
                Console.WriteLine($"  Part 2: {task.Result} ({part2Elapsed.TotalMilliseconds:F2}ms)");
            }
            else
            {
                sw.Stop();
                Console.WriteLine($"  Part 2: TIMEOUT after 10 seconds");
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            Console.WriteLine($"  Part 2: ERROR - {ex.GetBaseException().Message}");
        }
    }
}
