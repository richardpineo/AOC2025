namespace AOC2025.Days;

using AOC2025.Common;

public class Day01 : DayBase
{
    private class Turn
    {
        public bool ToRight { get; set; }
        public int Count { get; set; }
    }

    private class Safe
    {
        public List<Turn> Turns { get; set; } = new();

        public int NumberOfZeroes()
        {
            var pos = 50;
            var count = 0;
            foreach (var turn in Turns)
            {
                pos += (turn.ToRight ? 1 : -1) * turn.Count;
                pos %= 100;
                if (pos == 0)
                {
                    count++;
                }
            }
            return count;
        }

        public int NumberOfZeroes2()
        {
            var pos = 50;
            var count = 0;
            foreach (var turn in Turns)
            {
                var startPos = pos;
                for (int i = 0; i < turn.Count; i++)
                {
                    pos += (turn.ToRight ? 1 : -1);
                    pos %= 100;
                    if (pos == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public static Safe Load(string input)
        {
            var safe = new Safe();
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var dir = line[0];
                if (int.TryParse(line.Substring(1), out var num))
                {
                    safe.Turns.Add(new Turn { ToRight = dir == 'R', Count = num });
                }
            }

            return safe;
        }
    }

    public Day01() : base(1) { }

    public override string Part1(string input)
    {
        var safe = Safe.Load(input);
        return safe.NumberOfZeroes().ToString();
    }

    public override string Part2(string input)
    {
        var safe = Safe.Load(input);
        return safe.NumberOfZeroes2().ToString();
    }
}

