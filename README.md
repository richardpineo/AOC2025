# Advent of Code 2025 — .NET solutions

## Overview

- This workspace contains solutions for Advent of Code 2025 in multiple languages.
- The .NET project is at `DotNet/AOC2025` and uses .NET 9.

## Quickstart (run solutions)

```bash
cd DotNet/AOC2025
# Run all days (prints test + input results)
dotnet run

# Verbose diagnostics for Day 2 (per-range invalid IDs)
dotnet run diag2

# Check a single ID for Day 2 diagnostics
dotnet run diag2id 111
```

## Project layout

- `DotNet/AOC2025/`
  - `Days/` — one source file per day (e.g., `Day01.cs`, `Day02.cs`)
  - `Common/` — shared helpers (e.g., `FileHelper.cs`, `DayBase.cs`)
  - `Resources/` — input files (e.g., `Day01_Input.txt`, `Day02_Test.txt`)
  - `Program.cs` — runner and diagnostic hooks

## Adding a new day

1. Add `DotNet/AOC2025/Days/DayXX.cs` inheriting from `DayBase`.
2. Implement `Part1(string input)` and `Part2(string input)`.
3. Add input files to `DotNet/AOC2025/Resources/` (copy your AoC personal input into `DayXX_Input.txt`).
4. Register the new day in `Program.cs` (add `new DayXX(),` to the `days` array).

## Notes

- `DayBase` provides `InputPath` and `TestPath` helpers; `Program.cs` runs both test and input by default.
- `.gitignore` is configured to exclude `bin/` and `obj/` build artifacts.

## Want more?

- I can add unit tests, faster analytical counting for large ranges, or CI steps. Tell me which and I'll scaffold them.

## Authorship & AI Assist

This project was developed using a "vibe coding" workflow: the C# solutions were generated and iterated with the help of an AI coding assistant. The user did not hand-write the C# code in this repository.

This project was developed with AI assistance.

If you prefer different wording or want this statement removed or expanded (for example to mention testing/verification steps), tell me and I'll update it.
