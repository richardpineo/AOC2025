# Advent of Code 2025 — .NET Solutions

## Overview

This repository contains C# solutions for [Advent of Code 2025](https://adventofcode.com/2025) using .NET 9.

**Status: 12/25 days scaffolded, 9/24 parts complete**
- Days 1-9: Both parts complete ✅
- Day 10: Part 1 complete, Part 2 timeout ⏱️
- Day 11: Part 1 complete, Part 2 timeout ⏱️
- Day 12: Scaffolded, implementation incomplete 🚧

## Quick Start

```bash
cd DotNet/AOC2025

# Run all days (shows test and input results)
dotnet run

# Run a specific day
dotnet run -- --day 5

# Run test cases only
dotnet run -- --test-only

# Run input only
dotnet run -- --input-only

# Day 2 diagnostics: verbose invalid ID reporting
dotnet run diag2

# Day 2 diagnostics: check a single ID
dotnet run diag2id 123123
```

## Project Structure

```
DotNet/AOC2025/
├── Days/              # One file per day (Day01.cs - Day12.cs)
├── Common/            # Shared utilities
│   ├── DayBase.cs    # Base class with timing and file loading
│   └── FileHelper.cs # Input file path helpers
├── Resources/         # Puzzle inputs
│   ├── Day01_Input.txt
│   ├── Day01_Test.txt
│   └── ...
└── Program.cs         # Main runner with CLI options
```

## Implementation Notes

### Completed Solutions
- **Day 1**: Safe combination lock simulation with modular arithmetic
- **Day 2**: Pattern-based ID validation with diagnostic tooling
- **Day 3**: Greedy monotonic stack algorithm for max k-digit selection
- **Day 4**: Iterative grid simulation with 8-neighbor accessibility rules
- **Day 5**: Range merging and overlap detection
- **Day 6**: Column-wise parsing with both space-separated and fixed-width modes
- **Day 7**: Beam splitter simulation with path counting and memoization
- **Day 8**: 3D constellation grouping using Union-Find (Kruskal-style MST)
- **Day 9**: Coordinate compression with even-odd rule for interior detection

### Challenging Problems
- **Day 10 Part 2**: A* search with heuristics and component decomposition (times out on large inputs)
- **Day 11 Part 2**: Path counting with constraints (#P-complete, times out despite optimizations)
- **Day 12**: 2D bin packing / polyomino tiling (NP-complete, needs Dancing Links or better heuristics)

### Performance
- Timeout limit: 5 seconds per part
- Most solutions complete in < 200ms
- Day 8 Part 1 is slowest at ~180ms (many distance calculations)

## Adding a New Day

1. Create `DotNet/AOC2025/Days/DayXX.cs`:
   ```csharp
   using AOC2025.Common;
   namespace AOC2025.Days;
   
   public class DayXX : DayBase
   {
       public DayXX() : base(XX) { }
       
       public override string Part1(string input) => "Not implemented";
       public override string Part2(string input) => "Not implemented";
   }
   ```

2. Add resource files:
   - `Resources/DayXX_Input.txt` (your personal puzzle input)
   - `Resources/DayXX_Test.txt` (example from problem description)

3. Register in `Program.cs`:
   ```csharp
   var days = new DayBase[] { ..., new DayXX(), };
   ```

## Fetching Inputs

Use the provided script to download puzzle inputs and descriptions:

```bash
# Set your session cookie first
export AOC_SESSION="your_session_cookie_from_browser"

# Fetch a specific day
./scripts/fetch_aoc.sh 12 2025
```

## Development Notes

- All solutions include header comments explaining the problem and approach
- Code follows .NET naming conventions and best practices
- No external NuGet dependencies (uses only .NET 9 BCL)
- Diagnostic modes available for debugging (see Day 2 example)

## Authorship

This project was developed with AI assistance using GitHub Copilot. The solutions represent a collaborative "vibe coding" workflow where problem-solving strategies were discussed and implemented iteratively.
