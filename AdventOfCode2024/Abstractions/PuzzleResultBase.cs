namespace AdventOfCode2024;

public abstract record PuzzleResultBase : IPuzzleResult
{
    protected PuzzleResultBase(bool isSolved)
    {
        IsSolved = isSolved;
    }

    public bool IsSolved { get; }
}