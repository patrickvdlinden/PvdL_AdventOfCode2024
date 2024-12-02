namespace AdventOfCode2024;

public interface IPuzzle
{
    bool IsInputParsed { get; }

    void ParseInput(string input);

    IPuzzleResult Solve();
}