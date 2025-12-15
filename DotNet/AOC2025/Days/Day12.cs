// Day 12: Christmas Tree Farm - 2D bin packing / polyomino tiling puzzle
// Part 1: Count how many regions can fit all their required presents
// Part 2: TBD
//
// CURRENT STATUS: INCOMPLETE - Times out on test cases
// 
// Problem Description:
// - Given: Present shapes (polyominoes) and rectangular regions
// - Each region specifies counts of each shape type needed
// - Goal: Determine which regions can fit ALL their required presents
// - Constraints:
//   * Presents can be rotated (4 orientations) and flipped (8 total transformations)
//   * '#' cells from different presents cannot overlap
//   * '.' cells in shape definitions don't block other presents
//   * All presents must align to integer grid coordinates
//
// This is an NP-complete problem (variant of 2D bin packing / exact cover)
//
// IMPLEMENTATION STATUS:
// ✓ Parsing shapes and regions from input
// ✓ Generating all unique transformations (rotations + flips)
// ✓ Backtracking solver framework with placement validation
// ✗ Performance: Times out even on small test cases (4x4 with 2 pieces)
//
// WHY IT'S SLOW:
// - Naive backtracking explores O(T^P × (W×H)^P) states where:
//   * T = transformations per shape (~8)
//   * P = total pieces to place
//   * W×H = region dimensions
// - For test region 3 (12×5 with 6 pieces): ~8^6 × 60^6 = astronomical search space
// - Current implementation tries all positions × all transformations without pruning
//
// OPTIMIZATIONS NEEDED:
//
// 1. **First Empty Cell Constraint** (CRITICAL)
//    - Always place next piece to cover the first (top-leftmost) empty cell
//    - This reduces position search from O(W×H) to O(P) per piece
//    - Implementation: Find first empty cell, only try placements covering it
//
// 2. **Dancing Links / Algorithm X** (RECOMMENDED)
//    - Model as exact cover problem: each cell must be covered exactly once
//    - Use Knuth's DLX (Dancing Links) for efficient backtracking
//    - This is the standard approach for polyomino tiling
//    - Provides automatic constraint propagation
//
// 3. **Constraint Propagation**
//    - Before placing piece, check if remaining pieces can possibly fit remaining space
//    - Prune branches where total remaining cell count < available space
//    - Check connectivity: ensure remaining empty cells form valid regions
//
// 4. **Piece Ordering Heuristics**
//    - Place most constrained pieces first (largest, most irregular shapes)
//    - Current implementation sorts by size (largest first) but could be smarter
//    - Consider: piece with fewest valid placements first
//
// 5. **Symmetry Breaking**
//    - For identical pieces, enforce ordering (e.g., piece A must be placed above piece B)
//    - Reduces redundant search paths
//
// 6. **Memoization / Transposition Table**
//    - Cache failed grid states (hash of occupied cells + remaining pieces)
//    - If same state encountered again, skip immediately
//
// 7. **Parallel Region Solving**
//    - Each region is independent - can solve in parallel
//    - Use Parallel.ForEach or Task.WhenAll
//
// POTENTIAL APPROACH (Priority Order):
//
// Step 1: Implement "first empty cell" heuristic
//   - This alone should make small cases solvable
//   - Modify TryPlacePresents to find first empty cell
//   - Only try placements where that cell is covered by the piece
//
// Step 2: Add basic pruning
//   - Check if sum of remaining piece areas <= empty cell count
//   - Early exit if impossible
//
// Step 3: If still too slow, implement DLX
//   - Convert to exact cover matrix representation
//   - Each row = (piece, transformation, position)
//   - Each column = (cell to cover) or (piece instance used)
//   - Use DLX algorithm for fast backtracking
//
// Step 4: Polish and optimize
//   - Add symmetry breaking for identical pieces
//   - Implement transposition table
//   - Parallelize region solving
//
// ALTERNATIVE: If DLX is too complex, try constraint satisfaction (CSP) solver
// - Model as CSP with variables = piece placements, constraints = no overlap
// - Use existing CSP library if available
//
// REFERENCES:
// - Knuth's Dancing Links: https://arxiv.org/abs/cs/0011047
// - Polyomino tiling: Classic puzzle-solving domain
// - Similar to pentomino puzzles, tetris piece packing

using AOC2025.Common;

namespace AOC2025.Days;

public class Day12 : DayBase
{
    public Day12() : base(12) { }

    private class Shape
    {
        public int Index { get; set; }
        public List<(int row, int col)> Cells { get; set; } = new();
        public List<Shape> Transformations { get; set; } = new();
    }

    private class Region
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<int> Counts { get; set; } = new();
    }

    public override string Part1(string input)
    {
        var (shapes, regions) = ParseInput(input);
        
        // Generate all transformations for each shape
        foreach (var shape in shapes)
        {
            shape.Transformations = GenerateTransformations(shape.Cells);
        }

        int canFitCount = 0;
        foreach (var region in regions)
        {
            if (CanFitAllPresents(shapes, region))
            {
                canFitCount++;
            }
        }

        return canFitCount.ToString();
    }

    public override string Part2(string input)
    {
        return "Not implemented";
    }

    private (List<Shape> shapes, List<Region> regions) ParseInput(string input)
    {
        var shapes = new List<Shape>();
        var regions = new List<Region>();
        
        var sections = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        
        // Parse shapes section
        var shapeLines = new List<string>();
        var regionLines = new List<string>();
        bool inRegions = false;
        
        foreach (var line in input.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            if (line.Contains("x") && line.Contains(":") && char.IsDigit(line[0]) && line.Contains(" "))
            {
                inRegions = true;
            }
            
            if (inRegions)
            {
                regionLines.Add(line);
            }
            else
            {
                shapeLines.Add(line);
            }
        }
        
        // Parse shapes
        int currentIndex = -1;
        var currentCells = new List<(int row, int col)>();
        int row = 0;
        
        foreach (var line in shapeLines)
        {
            if (line.Contains(":") && !line.Contains("x"))
            {
                // Save previous shape if any
                if (currentIndex >= 0 && currentCells.Count > 0)
                {
                    shapes.Add(new Shape { Index = currentIndex, Cells = new List<(int, int)>(currentCells) });
                }
                
                currentIndex = int.Parse(line.Split(':')[0]);
                currentCells.Clear();
                row = 0;
            }
            else if (line.Contains("#") || line.Contains("."))
            {
                for (int col = 0; col < line.Length; col++)
                {
                    if (line[col] == '#')
                    {
                        currentCells.Add((row, col));
                    }
                }
                row++;
            }
        }
        
        // Save last shape
        if (currentIndex >= 0 && currentCells.Count > 0)
        {
            shapes.Add(new Shape { Index = currentIndex, Cells = new List<(int, int)>(currentCells) });
        }
        
        // Parse regions
        foreach (var line in regionLines)
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries);
            var dims = parts[0].Split('x');
            var counts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();
            
            regions.Add(new Region 
            { 
                Width = int.Parse(dims[0]), 
                Height = int.Parse(dims[1]), 
                Counts = counts 
            });
        }
        
        return (shapes, regions);
    }

    private List<Shape> GenerateTransformations(List<(int row, int col)> cells)
    {
        var transformations = new HashSet<string>();
        var results = new List<Shape>();
        
        // Generate rotations and flips
        foreach (var transformed in new[] { cells, Flip(cells) })
        {
            var current = transformed;
            for (int i = 0; i < 4; i++)
            {
                var normalized = Normalize(current);
                var key = string.Join(",", normalized.OrderBy(c => c.row).ThenBy(c => c.col));
                
                if (transformations.Add(key))
                {
                    results.Add(new Shape { Cells = normalized });
                }
                
                current = Rotate90(current);
            }
        }
        
        return results;
    }

    private List<(int row, int col)> Rotate90(List<(int row, int col)> cells)
    {
        return cells.Select(c => (c.col, -c.row)).ToList();
    }

    private List<(int row, int col)> Flip(List<(int row, int col)> cells)
    {
        return cells.Select(c => (c.row, -c.col)).ToList();
    }

    private List<(int row, int col)> Normalize(List<(int row, int col)> cells)
    {
        if (cells.Count == 0) return cells;
        
        int minRow = cells.Min(c => c.row);
        int minCol = cells.Min(c => c.col);
        
        return cells.Select(c => (c.row - minRow, c.col - minCol)).ToList();
    }

    private bool CanFitAllPresents(List<Shape> shapes, Region region)
    {
        var grid = new bool[region.Height, region.Width];
        var presentsToPlace = new List<(int shapeIndex, int instanceId)>();
        
        // Build list of all presents to place
        for (int i = 0; i < region.Counts.Count && i < shapes.Count; i++)
        {
            for (int j = 0; j < region.Counts[i]; j++)
            {
                presentsToPlace.Add((i, j));
            }
        }
        
        // Sort by shape size (largest first) for better pruning
        presentsToPlace = presentsToPlace
            .OrderByDescending(p => shapes[p.shapeIndex].Cells.Count)
            .ToList();
        
        // Early check: total cells required vs available
        int totalCellsNeeded = presentsToPlace.Sum(p => shapes[p.shapeIndex].Cells.Count);
        if (totalCellsNeeded > region.Width * region.Height)
            return false;
        
        return TryPlacePresents(shapes, grid, presentsToPlace, 0);
    }

    private bool TryPlacePresents(List<Shape> shapes, bool[,] grid, 
        List<(int shapeIndex, int instanceId)> presents, int presentIdx)
    {
        if (presentIdx >= presents.Count)
            return true; // All presents placed successfully
        
        int shapeIndex = presents[presentIdx].shapeIndex;
        var shape = shapes[shapeIndex];
        
        // Try all transformations of this shape
        foreach (var transformation in shape.Transformations)
        {
            // Try all possible positions
            for (int r = 0; r <= grid.GetLength(0); r++)
            {
                for (int c = 0; c <= grid.GetLength(1); c++)
                {
                    if (CanPlace(grid, transformation.Cells, r, c))
                    {
                        Place(grid, transformation.Cells, r, c, true);
                        
                        if (TryPlacePresents(shapes, grid, presents, presentIdx + 1))
                            return true;
                        
                        Place(grid, transformation.Cells, r, c, false); // Backtrack
                    }
                }
            }
        }
        
        return false;
    }

    private bool CanPlace(bool[,] grid, List<(int row, int col)> cells, int startRow, int startCol)
    {
        foreach (var (r, c) in cells)
        {
            int row = startRow + r;
            int col = startCol + c;
            
            if (row < 0 || row >= grid.GetLength(0) || 
                col < 0 || col >= grid.GetLength(1) || 
                grid[row, col])
            {
                return false;
            }
        }
        return true;
    }

    private void Place(bool[,] grid, List<(int row, int col)> cells, int startRow, int startCol, bool occupy)
    {
        foreach (var (r, c) in cells)
        {
            grid[startRow + r, startCol + c] = occupy;
        }
    }
}
