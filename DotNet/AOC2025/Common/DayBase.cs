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
            var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
            var task = System.Threading.Tasks.Task.Run(() => Part1(input), cts.Token);
            var part1Result = task.Result;
            sw.Stop();
            var part1Elapsed = sw.Elapsed;
            Console.WriteLine($"  Part 1: {part1Result} ({part1Elapsed.TotalMilliseconds:F2}ms)");
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            Console.WriteLine($"  Part 1: TIMEOUT after 10 seconds");
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
            var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
            var task = System.Threading.Tasks.Task.Run(() => Part2(input), cts.Token);
            var part2Result = task.Result;
            sw.Stop();
            var part2Elapsed = sw.Elapsed;
            Console.WriteLine($"  Part 2: {part2Result} ({part2Elapsed.TotalMilliseconds:F2}ms)");
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            Console.WriteLine($"  Part 2: TIMEOUT after 10 seconds");
        }
        catch (Exception ex)
        {
            sw.Stop();
            Console.WriteLine($"  Part 2: ERROR - {ex.GetBaseException().Message}");
        }
    }
}
